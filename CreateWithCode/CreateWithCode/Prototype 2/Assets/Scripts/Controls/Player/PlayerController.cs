using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 11.0f;
    public ProjectilePool projectilePool;

    private const float worldBoundLeft = -10.0f;
    private const float worldBoundRight = 10.0f;
    private const float worldBoundTop = 10.0f;
    private const float worldBoundBottom = -7.0f;

    private bool shotQueued = false;

    private PlayerControls controls;
    private Vector2 moveInput = Vector2.zero;


    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Fire.performed += ctx => shotQueued = true;
    }

    public void OnEnable() => controls.Enable();
    public void OnDisable() => controls.Disable();

    private void Update()
    {
        if (moveInput.sqrMagnitude > 0.01f) HandleMovement();
        if (shotQueued) Shoot();
    }

    private void HandleMovement()
    {
        // Apply the movement calculations in memory
        Vector3 movement = speed * Time.deltaTime * new Vector3(moveInput.x, 0, moveInput.y);
        Vector3 targetPos = transform.position + movement;

        // Clamping the values ensures that the player can not move out of bounds
        // This OOB check occurs _before_ applying the final movement, to ensure that there's no jittery behaviour 
        targetPos.x = Mathf.Clamp(targetPos.x, worldBoundLeft, worldBoundRight);
        targetPos.z = Mathf.Clamp(targetPos.z, worldBoundBottom, worldBoundTop);

        // Apply the movement calculation to the character
        transform.position = targetPos;
    }

    private void Shoot()
    {
        shotQueued = false;

        // Get projectile from pool
        GameObject proj = projectilePool.GetProjectile();
        proj.transform.SetPositionAndRotation(transform.position, proj.transform.rotation);

        if (!proj.TryGetComponent<Projectile>(out var logic)) 
        {
            Debug.Log("Unable to get projectile");
        } else
        {
            logic
                .Initialize(ProjectileType.Pizza, projectilePool, null)
                .Fire(Vector3.forward);
        }
    }
}
