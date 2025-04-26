using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    float spawnTime;
    float timeSincespawn;

    [SerializeField]
    GameObject[] potentialSpawns;

    void Start()
    {
        
    }

    void Update()
    {
        timeSincespawn += Time.deltaTime;
        if (timeSincespawn >= spawnTime)
        {
            timeSincespawn = 0;
            timeSincespawn -= Random.Range(0, 3);
            GameObject.Instantiate(potentialSpawns[Random.Range(0, potentialSpawns.Length)], this.transform.position, this.transform.rotation);
        }
    }
}
