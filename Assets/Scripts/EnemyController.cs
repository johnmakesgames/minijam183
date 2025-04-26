using UnityEngine;

public class EnemyController : MonoBehaviour
{
    GameObject player;
    PlayerController playerController;
    Rigidbody rb;

    [SerializeField]
    float movementSpeed;

    [SerializeField]
    EnemyNumeric myValue;

    [SerializeField]
    float Health;

    [SerializeField]
    float Damage;

    float distanceToGoToPlayer = 100;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distToPlayer = (player.transform.position - this.transform.position).magnitude;
        if (distToPlayer <= distanceToGoToPlayer)
        {
            Vector3 dirToPlayer = (player.transform.position - this.transform.position).normalized;

            Ray ray = new Ray();
            ray.origin = this.transform.position + dirToPlayer;
            ray.direction = dirToPlayer;
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.blue, Time.deltaTime);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (hitInfo.transform.gameObject.CompareTag("Player"))
                {
                    dirToPlayer.y = 0;
                    rb.AddForce(dirToPlayer * movementSpeed * Time.deltaTime);
                }
            }
        }

        if (Health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player)
        {
            playerController.DoDamage(Damage);
        }

        if (collision.gameObject.GetComponent<EnemyController>())
        {
            Vector3 away = (this.transform.position - collision.transform.position).normalized;
            rb.AddForce(away * 20000 * Time.deltaTime);
        }
    }

    public EnemyNumeric DoDamage(float damage)
    {
        Health -= damage;
        return myValue;
    }
}
