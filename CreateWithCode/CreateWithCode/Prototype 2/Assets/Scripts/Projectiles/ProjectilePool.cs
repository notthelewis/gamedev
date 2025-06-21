using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int poolSize = 50;

    private readonly Queue<GameObject> pool = new();
    private bool isReturning = false;

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
        return projectile;
    }

    public void ReturnToPool(GameObject projectile)
    {
        if (isReturning) return;
        isReturning = true;

        projectile.SetActive(false);
        pool.Enqueue(projectile);

        isReturning = false;
    }
}