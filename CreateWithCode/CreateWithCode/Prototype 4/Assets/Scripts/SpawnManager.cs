using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject powerupPrefab;

    private const float spawnRange = 9f;

    private byte wave = 1;
    public byte currentEnemies = 0; 

    void Update()
    {
        if (currentEnemies == 0)
        {
            SpawnWave();
        }
    }

    void SpawnWave()
    {
        currentEnemies = wave;

        for (byte i = 0; i < wave; i++)
        {
            Instantiate(enemyPrefab, GenerateRandomSpawnPos(), enemyPrefab.transform.rotation);
        }

        wave++;
        StartCoroutine(SpawnPowerupAtRandomInterval());
    }

    IEnumerator SpawnPowerupAtRandomInterval()
    {
        yield return new WaitForSeconds(Random.Range(0, wave));
        Instantiate(powerupPrefab);
    }

    Vector3 GenerateRandomSpawnPos()
    {
        float spawnX = Random.Range(-spawnRange, spawnRange);
        float spawnZ = Random.Range(-spawnRange, spawnRange);
        return new(spawnX, 0, spawnZ);
    }
}
