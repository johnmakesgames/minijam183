using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //CharacterController characterController;
    Rigidbody rb;

    [SerializeField]
    float movementScalar = 100;

    [SerializeField]
    float cameraRotation = 100;
    float previousMouseX;
    
    [SerializeField]
    float rotationDifScalar = 10;

    [SerializeField]
    GameObject playerCamera;
    [SerializeField]
    float headBobScale;
    [SerializeField]
    float headBobSpeed;
    [SerializeField]
    float headBobOffset;
    [SerializeField]
    float sinTime;

    [SerializeField]
    Animator handsAnimator;

    [SerializeField]
    float Health;

    [SerializeField]
    float MaxHealth;

    public HitResultsBuilder HitResultsBuilder;
    public GoalManager GoalManager;

    private List<IGuns> Guns;
    int currentGun;

    [SerializeField]
    AudioSource gunShootSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sinTime = 0;
        Cursor.lockState = CursorLockMode.Locked;
        HitResultsBuilder = new HitResultsBuilder();
        GoalManager = new GoalManager();

        Guns = new List<IGuns>();
        Guns.Add(new Shotgun(handsAnimator, HitResultsBuilder, GoalManager, gunShootSound));
        Guns.Add(new SuperShotgun(handsAnimator, HitResultsBuilder, GoalManager, gunShootSound));
        Guns.Add(new MiniGun(handsAnimator, HitResultsBuilder, GoalManager, gunShootSound));
        Guns.Add(new BFG(handsAnimator, HitResultsBuilder, GoalManager, gunShootSound));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMovement(Time.deltaTime);
    }

    void Update()
    {
        UpdateShooting(Time.deltaTime);
        UpdateRotation(Time.deltaTime);
    }

    void UpdateMovement(float dt)
    {
        Vector3 movementUpdate = new Vector3();

        if (Input.GetKey(KeyCode.W))
        {
            movementUpdate += this.transform.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            movementUpdate -= this.transform.forward;
        }

        if (Input.GetKey(KeyCode.D))
        {
            movementUpdate += this.transform.right;
        }

        if (Input.GetKey(KeyCode.A))
        {
            movementUpdate -= this.transform.right;
        }

        movementUpdate.Normalize();
        movementUpdate *= (dt * movementScalar);

        rb.AddForce(movementUpdate);
        UpdateHeadBob(Time.deltaTime, movementUpdate.magnitude);
    }

    void UpdateHeadBob(float dt, float movementSpeed)
    {
        Vector3 updatedCameraPos = this.transform.position;
        updatedCameraPos.y = (Mathf.Sin(sinTime) * headBobScale) + headBobOffset + this.transform.position.y;
        playerCamera.transform.position = updatedCameraPos;
        sinTime += dt * headBobSpeed * (movementSpeed/100);
    }

    void UpdateRotation(float dt)
    {
        Vector3 rotationChange = new Vector3();

        float mouseDif = Input.GetAxisRaw("Mouse X") * rotationDifScalar;
        rotationChange.y += mouseDif;
        rotationChange.Normalize();

        this.transform.Rotate(rotationChange, cameraRotation * dt * Mathf.Abs(mouseDif));
    }

    void UpdateShooting(float dt)
    {
        foreach (var gun in Guns)
        {
            gun.Update(dt);
        }


        if (Input.GetKeyDown(KeyCode.E))
        {
            currentGun++;
            if (currentGun > Guns.Count - 1)
            {
                currentGun = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentGun--;
            if (currentGun < 0)
            {
                currentGun = Guns.Count - 1;
            }
        }

        handsAnimator.SetInteger("GunChoice", Guns[currentGun].GunID);

        
        // Input
        if (Input.GetMouseButton(0))
        {
            Guns[currentGun].ShootGun(this.transform.position, this.transform.forward);
        }
    }

    public void DoDamage(float damage)
    {
        Health -= damage;
        if (Health < 0)
        {
            Debug.Log("DEAD");
            SceneManager.LoadScene("MainMenu");
        }
    }

    public float GetHealth()
    {
        return Health;
    }

    public float GetMaxHealth()
    {
        return MaxHealth;
    }
}
