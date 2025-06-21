using System;
using System.Collections;
using UnityEngine;

public enum ProjectileType : byte
{
    Doe,
    Fox,
    Moose,
    Pizza,
    Unknown,
}

/// <summary>
/// This projectile class helps to facillitate a pooled, event-driven projectile system. Other relevant classes are `SpawnManager` and `ProjectilePool` 
/// </summary>
public class Projectile : MonoBehaviour
{
    // Callback action to allow for custom behaviour when the projectile collides with another GameObject
    private Action<Collider> OnCollisionCallback; 

    [SerializeField] private float projectileSpeed = 40f;

    private ProjectileType projectileType = ProjectileType.Unknown;
    private ProjectilePool owningPool;

    private Vector3 direction;
    private const byte OOB_Z = 35;
    private const byte OOB_X = 30;

    private Coroutine movementCoroutine;
    private bool isReturning = false;

    public Projectile Initialize(ProjectileType projType, ProjectilePool projectilePool, Action<Collider> onHit)
    {
        projectileType = projType;
        owningPool = projectilePool;
        OnCollisionCallback = onHit;
        return this;
    }

    /// <summary>
    /// Move the projectile in a straight line from it's current position until it reaches out of bounds or collides with another objectm at which point it's returned to the provided pool
    /// </summary>
    /// <param name="velocity">The direction the projectile should fire</param>
    public void Fire(Vector3 velocity)
    {
        if (projectileType == ProjectileType.Unknown)
        {
            Debug.LogWarning("Tried to fire an unknown projectile type...");
            if (owningPool != null)
            {
                ReturnToPool();
            }
            return;
        }

        direction = velocity.normalized;
        movementCoroutine = StartCoroutine(MoveUntilReturnedToPool());
    }

    public void ReturnToPool()
    {
        if (isReturning) return;
        isReturning = true;

        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        owningPool.ReturnToPool(gameObject);

        // Reset state
        movementCoroutine = null;
        projectileType = ProjectileType.Unknown;
        direction = Vector3.zero;
        owningPool = null;
        isReturning = false;
    }

    private IEnumerator MoveUntilReturnedToPool()
    {
        while (true)
        {
            // Basic firing mechanism- just pings in a straight line
            transform.Translate(projectileSpeed * Time.deltaTime * direction);

            if (Mathf.Abs(transform.position.z) > OOB_Z || Mathf.Abs(transform.position.x) > OOB_X)
            {
                ReturnToPool();
                yield break;
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        OnCollisionCallback?.Invoke(other);
        ReturnToPool();
    }
}
