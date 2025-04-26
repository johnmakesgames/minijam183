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

        rb.MovePosition(this.transform.position + movementUpdate);
        UpdateHeadBob(Time.deltaTime, movementUpdate.magnitude);
    }

    void UpdateHeadBob(float dt, float movementSpeed)
    {
        Vector3 updatedCameraPos = this.transform.position;
        updatedCameraPos.y = (Mathf.Sin(sinTime) * headBobScale) + headBobOffset + this.transform.position.y;
        playerCamera.transform.position = updatedCameraPos;
        sinTime += dt * headBobSpeed * movementSpeed;
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
                Ray ray = new Ray();
                ray.origin = this.transform.position + this.transform.forward;
                ray.direction = this.transform.forward;

                RaycastHit hitResults = new RaycastHit();
                float distance = 500;
                if (Physics.Raycast(ray, out hitResults))
                {
                    Debug.DrawRay(ray.origin, ray.direction * hitResults.distance, Color.green, 3);

                    if (hitResults.transform.gameObject.GetComponent<EnemyController>())
                    {
                        Debug.Log("Hit Enemy");

                        EnemyNumeric result = hitResults.transform.gameObject.GetComponent<EnemyController>().DoDamage(1);
                        if (result != EnemyNumeric.EnumCount)
                        {
                            // Do builder here
                            Debug.Log("Killed Enemy");
                            if (result != EnemyNumeric.Equals)
                            {
                                HitResultsBuilder.Symbol(result);
                            }
                            else
                            {
                                GoalManager.CheckGoal(HitResultsBuilder.Result());
                            }
                        }
                    }
                }
                else
                {
                    Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, 3);
                }


                timeSinceShot = 0;
            }
        }
    }

    public void DoDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Debug.Log("DEAD");
        }
    }
}
