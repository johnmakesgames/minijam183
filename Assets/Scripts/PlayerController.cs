using System.Collections.Generic;
using UnityEngine;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sinTime = 0;
        Cursor.lockState = CursorLockMode.Locked;
        HitResultsBuilder = new HitResultsBuilder();
        GoalManager = new GoalManager();
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

    float timeSinceShot = 0;
    const float minimumCooldown = 0.2f;
    void UpdateShooting(float dt)
    {
        timeSinceShot += dt;

        // Input
        if (timeSinceShot > minimumCooldown && Input.GetMouseButtonDown(0))
        {
            // Not currently shooting
            if (handsAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "PlayerShoot")
            {
                // Play Anim
                handsAnimator.SetTrigger("Shoot");

                // Do shooting
                List<RaycastHit> hits = FireMultipleRays(this.transform.position + this.transform.forward, this.transform.forward, 80, 3, out bool result);
                if (result)
                {
                    bool sorted = true;
                    do
                    {
                        sorted = true;
                        for (int i = 1; i < hits.Count; i++)
                        {
                            if (hits[i - 1].distance > hits[i].distance)
                            {
                                RaycastHit hit1 = hits[i - 1];
                                hits[i - 1] = hits[i];
                                hits[i] = hit1;
                                sorted = false;
                            }
                        }
                    }
                    while (!sorted);

                    foreach (var hit in hits)
                    {
                        if (hit.transform.gameObject.GetComponent<EnemyController>())
                        {
                            Debug.Log("Hit Enemy");

                            EnemyNumeric numeric = hit.transform.gameObject.GetComponent<EnemyController>().DoDamage(1);
                            if (numeric != EnemyNumeric.EnumCount)
                            {
                                // Do builder here
                                Debug.Log("Killed Enemy");
                                if (numeric != EnemyNumeric.Equals)
                                {
                                    HitResultsBuilder.Symbol(numeric);
                                }
                                else
                                {
                                    GoalManager.CheckGoal(HitResultsBuilder.Result());
                                }
                            }

                            break;
                        }
                    }
                }

                timeSinceShot = 0;
            }
        }
    }

    List<RaycastHit> FireMultipleRays(Vector3 centralPosition, Vector3 direciton, int verticalRayCount, int horizontalRayCount, out bool result)
    {
        result = false;
        List<RaycastHit> hits = new List<RaycastHit>();
        int halfHorizontalRayCount = horizontalRayCount / 2;
        for (int j = -halfHorizontalRayCount; j < horizontalRayCount - halfHorizontalRayCount; j++)
        {
            int halfVerticalRayCount = verticalRayCount / 2;
            for (int i = -halfVerticalRayCount; i < verticalRayCount - halfVerticalRayCount; i++)
            {
                Vector3 origin = centralPosition;
                origin.y += (float)i / 5;
                origin.x += (float)j / 10;

                Ray ray = new Ray();
                ray.origin = origin;
                ray.direction = direciton;
                RaycastHit hitResult = new RaycastHit();

                bool thisOneHit = false;
                if (Physics.Raycast(ray, out hitResult))
                {
                    hits.Add(hitResult);
                    result = true;
                    thisOneHit = true;
                }

                Debug.DrawRay(origin, direciton * 1000, (thisOneHit) ? Color.green : Color.red, 5);
            }
        }

        return hits;
    }

    public void DoDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Debug.Log("DEAD");
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
