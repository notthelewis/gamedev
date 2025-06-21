using UnityEngine;

public class FallDetector : MonoBehaviour
{
    private SpawnManager spawnManager;

    private void Start()
    {
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            spawnManager.currentEnemies--;
        }
    }
}
