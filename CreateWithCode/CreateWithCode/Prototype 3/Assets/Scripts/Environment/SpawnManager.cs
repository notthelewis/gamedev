using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject obj;
    public float startDelay = 2f;
    public float spawnFrequency = 2f;

    private PlayerController playerController;
    private Vector3 spawnPos = new(25, 0, -1.14f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        InvokeRepeating(nameof(SpawnObstacle), startDelay, spawnFrequency);
    }

    void Update()
    {
    }

    void SpawnObstacle()
    {
        if (!playerController.gameOver)
        {
            Instantiate(obj, spawnPos, obj.transform.rotation);
        }
    }
}
