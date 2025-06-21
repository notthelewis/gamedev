using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject pools;

    private ProjectilePool[] poolArray; 
    private SpawnControls controls;
    private const bool isInDevMode = false;
    private bool spawnQueued = false;
    private float startDelay = 2;
    private float spawnInterval = 1.5f;

    public void OnEnable() => controls.Enable();
    public void OnDisable() => controls.Disable();

    private void Awake()
    {
        // Cache the lookup of children to prevent wasteful lookups
        poolArray = pools.GetComponentsInChildren<ProjectilePool>()
            .Where(p => p.name != "PizzaProjectile")
            .ToArray();

        controls = new SpawnControls();
        controls.Spawn.enemy.performed += ctx => spawnQueued = isInDevMode;
    }

    private void Start()
    {
        InvokeRepeating(
            nameof(SpawnRandomPooledAnimal),
            startDelay,
            spawnInterval
        );
    }

    void Update()
    {
        if (spawnQueued)
        {
            SpawnRandomPooledAnimal();
            spawnQueued = false;
        }
    }

    void SpawnRandomPooledAnimal()
    {
        // take an enemy from a random pool
        var enemy = poolArray[UnityEngine.Random.Range(0, poolArray.Length - 1)].GetProjectile();

        // Put that enemy in a random X position  
        enemy.transform.position = new Vector3
        {
            x = UnityEngine.Random.Range(-9.8f, 9.8f),
            y = 0,
            z = 20.77f
        };

    }
}
