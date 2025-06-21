using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject pools;

    private ProjectilePool[] poolArray;
    private float startDelay = 2;
    private float spawnInterval = 1.5f;

    private void Awake()
    {
        // Cache the lookup of children to prevent wasteful lookups
        poolArray = pools.GetComponentsInChildren<ProjectilePool>()
            .Where(p => p.name != "PizzaProjectile")
            .ToArray();
    }

    private void Start()
    {
        InvokeRepeating(
            nameof(SpawnRandomPooledAnimal),
            startDelay,
            spawnInterval
        );
    }

    void SpawnRandomPooledAnimal()
    {
        // take an enemy from a random pool
        var pool = poolArray[UnityEngine.Random.Range(0, poolArray.Length - 1)];
        var enemy = pool.GetProjectile();

        // Put that enemy in a random X position
        enemy.transform.position = new Vector3
        {
            x = UnityEngine.Random.Range(-9.8f, 9.8f),
            y = 0,
            z = 33.3f
        };

        // Shoot the enemy forward
        if (!enemy.TryGetComponent<Projectile>(out var logic))
            Debug.Log("Unable to get projectile from enemy");
        else
        {
            logic
                .Initialize(GetProjectileTypeFromPoolName(pool.name), pool, null)
                .Fire(Vector3.forward);
        }
    }

    private ProjectileType GetProjectileTypeFromPoolName(string poolName)
    {
        switch (poolName)
        {
            case "DoePool": return ProjectileType.Doe;
            case "FoxPool": return ProjectileType.Fox;
            case "MoosePool": return ProjectileType.Moose;
            default:
                Debug.Log("Unknown pool name: " + poolName);
                return ProjectileType.Unknown;
        }
    }
}
