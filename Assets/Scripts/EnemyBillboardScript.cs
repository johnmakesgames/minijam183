using UnityEngine;

public class EnemyBillboardScript : MonoBehaviour
{
    GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forwardPos = this.transform.forward;
        forwardPos.y = 0;
        Vector3 dirToPlayer = (this.transform.position - player.transform.position).normalized;
        dirToPlayer.y = 0;
        Vector3 result = Vector3.RotateTowards(forwardPos, dirToPlayer, 0.2f, 1.0f);
        this.transform.forward = result;
    }
}
