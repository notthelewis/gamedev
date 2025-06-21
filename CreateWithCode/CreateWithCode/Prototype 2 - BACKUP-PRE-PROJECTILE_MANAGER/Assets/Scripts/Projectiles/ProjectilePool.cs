using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int poolSize = 50;

    private readonly Queue<GameObject> pool = new();

    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject proj = Instantiate(projectilePrefab);
            proj.SetActive(false);
            pool.Enqueue(proj);
        }
    }

    public GameObject GetProjectile()
    {
        GameObject projectile = pool.Count > 0 ? pool.Dequeue() : Instantiate(projectilePrefab);
        projectile.SetActive(true);

        // In here we must manually inject the pool reference to the DestroyOnOutOfBounds script, as prefabs
        // may not withhold a reference to a scene object during edit time- as it is not a concrete object
        // until it has been instantiated.
        var destroyer = projectile.GetComponent<DestroyOnOutOfBounds>();
        if (destroyer != null)
        {
            destroyer.projectilePool = this;
        }
        else
        {
            throw new System.Exception("ProjectilePool::GetProjectile::Invalid destroyer ref. Ensure that the projectile has access to the `DestroyOnOutOfBounds` script yadick");
        }

        return projectile;

    }

    public void ReturnToPool(GameObject projectile)
    {
        projectile.SetActive(false);
        pool.Enqueue(projectile);
    }
}