using UnityEngine;

public class DestroyOnOutOfBounds : MonoBehaviour
{
    public ProjectilePool projectilePool;

    private const int OOB_Z = 30;

    void Update()
    {
        if (this.isActiveAndEnabled && (this.transform.position.z >= OOB_Z || this.transform.position.z <= -OOB_Z))
        {
            projectilePool.ReturnToPool(gameObject);
        }
    }
}
