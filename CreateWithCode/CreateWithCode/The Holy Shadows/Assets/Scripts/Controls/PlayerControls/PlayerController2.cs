using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    // Game objects 
    private PlayerControls controls;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    [SerializeField] private LayerMask groundMask;

    // Player constants
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float playerHeight = 2.0f;
    [SerializeField] private float crouchHeight = 1.0f;

    // Jump physics
    private const float coyoteTime = 0.10f;
    private const float fallMultiplier = 2.5f;
    private const float jumpForce = 5.4f;
    private const float jumpBufferTime = 0.1f;

    // State
    private Vector3 moveInput = Vector3.zero;
    private float lastGroundTime;
    private float lastColliderHeight = -1;
    private float lastJumpTime = 0;
    private bool isGrounded = false;
    private bool didJump = false;
    private bool didCrouch = false;

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    // Awake is called when the script is loaded
    private void Awake()
    {
        controls = new PlayerControls();
        capsuleCollider = GetComponent<CapsuleCollider>();
        lastColliderHeight = capsuleCollider.height;
        Debug.Log(capsuleCollider.center);

        SetupRigidBody();
        SetupEventHandlers(); 
    }

    private void FixedUpdate()
    {
        isGrounded = IsGrounded();
        Debug.Log($"isGrounded: {isGrounded}");
        HandleMovement();
        HandleJump();
        HandleFall();
        HandleCrouch();
    }

    private void HandleMovement()
    {
        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            // Move along X axis
            rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * new Vector3(moveInput.x, 0, 0));
        }

    }

    private void HandleFall()
    {
        // Fall faster than jump
        if (rb.linearVelocity.y < 0)
        {
            // Avoid implicit alloc during multiplication of `Vector3.up` multipliciation 
            Vector3 velocity = rb.linearVelocity;
            velocity.y += (fallMultiplier - 1) * Physics.gravity.y * Time.fixedDeltaTime;
            rb.linearVelocity = velocity;
        }
    }

    private void HandleJump()
    {
        if (isGrounded && didJump)
        {
            lastGroundTime = Time.time;
            didJump = false; 
        }

        bool canUseCoyoteTime = Time.time - lastGroundTime <= coyoteTime;
        bool jumpBuffered = Time.time - lastJumpTime <= jumpBufferTime;
        
        if ((isGrounded || canUseCoyoteTime) && jumpBuffered && !didJump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            didJump = true;
            lastJumpTime = -1f;
            return;
        }
    }

    private void HandleCrouch()
    {
        float newHeight = didCrouch ? crouchHeight : playerHeight;
        if (Mathf.Approximately(newHeight, lastColliderHeight)) return;
        capsuleCollider.height = newHeight;
        capsuleCollider.center = Vector3.zero;
        lastColliderHeight = newHeight;
    }

    private void OnDrawGizmosSelected()
    {
        if (capsuleCollider == null) return;
        Gizmos.color = Color.red;
        Vector3 origin = transform.position + Vector3.down * 0.1f;
        float radius = capsuleCollider.radius * 0.95f;
        Gizmos.DrawWireSphere(origin, radius);
    }

    private bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.down * 0.85f;
        float radius = capsuleCollider.radius * 0.95f;
        return Physics.CheckSphere(origin, radius, groundMask);
    } 
        

    //private bool IsGrounded()
    //{
    //    // Cast a sphere instead of a ray so that the player can still jump even if they're on a ledge
    //    Vector3 sphereOrigin = transform.position + Vector3.down * 0.1f;
    //    float radius = capsuleCollider.radius * 0.95f;
    //    return Physics.CheckSphere(sphereOrigin, radius, groundMask);
    //}


    private void SetupRigidBody()
    {
        rb = GetComponent<Rigidbody>();

        // Reduce drag in air
        rb.linearDamping = 0;
    }


    private void SetupEventHandlers()
    {
        // Since input & physics are on different clocks, queue jump input and handle it in FixedUpdate()
        controls.Player.Jump.performed += ctx => lastJumpTime = Time.time; 

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Crouch.performed += ctx => didCrouch = true;
        controls.Player.Crouch.canceled += ctx => didCrouch = false;
    }
}
