using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerControlActions
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
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float slamForce = 30f;
    [SerializeField] private float slamCooldown = 0.5f;

    // Jump physics
    private const float coyoteTime = 0.10f;
    private const float fallMultiplier = 2.5f;
    private const float jumpForce = 5.4f;
    private const float jumpBufferTime = 0.1f;
    private const byte MAX_PLAYER_JUMPS = 2;
    byte jumpsLeft = MAX_PLAYER_JUMPS;

    // Player State
    private PlayerStateMachine fsm;
    private Vector3 moveInput = Vector3.zero;
    private float lastGroundTime;
    private float lastColliderHeight = -1;
    private float lastJumpTime = -1;
    private float lastDashTime = -999f;
    private float lastSlamTime = -999f;
    private bool isGrounded = true;
    private bool isSlamming = false;
    private bool didJump = false;
    private bool didCrouch = false;

    private Coroutine move;

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Awake()
    {
        fsm = new PlayerStateMachine(this);
        controls = new PlayerControls();
        capsuleCollider = GetComponent<CapsuleCollider>();

        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 0; // Reduce drag in air

        lastColliderHeight = capsuleCollider.height;

        fsm.Init(PlayerState.Idle);

        SetupEventListeners();
    }

    private void FixedUpdate()
    {
        isGrounded = IsGrounded();
        fsm.Tick();
    }

    public void UpdateWalk()
    {
        move = StartCoroutine(nameof(Move));
    }

    public IEnumerator Move()
    {
        while (true)
        {
            if (Mathf.Abs(moveInput.x) > 0.01f)
            {
                rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * new Vector3(moveInput.x, 0, 0));
                yield return null;
            }

            yield break;
        }
    }

    public void EndWalk()
    {
        move = null;
    }

    public void UpdateFall()
    {
        // Fall faster than jump
        if (rb.linearVelocity.y < 0)
        {
            // Doing this avoids implicit alloc during multiplication of Vector3.up
            Vector3 velocity = rb.linearVelocity;
            velocity.y += (fallMultiplier - 1) * Physics.gravity.y * Time.fixedDeltaTime;
            rb.linearVelocity = velocity;
        }
    }

    public void UpdateJump()
    {
        if (isGrounded && didJump)
        {
            lastGroundTime = Time.time;
            didJump = false;
            jumpsLeft = MAX_PLAYER_JUMPS;
        }

        bool jumpBuffered = Time.time - lastJumpTime <= jumpBufferTime;
        bool canUseCoyoteTime = Time.time - lastGroundTime <= coyoteTime;
        bool canJump = (isGrounded || canUseCoyoteTime) && jumpBuffered && didJump;

        if ((canJump || (jumpBuffered && jumpsLeft > 0)) && !didJump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            didJump = true;
            lastJumpTime = Time.time;
        }
    }

    public void UpdateSlam()
    {
        if (!isGrounded && !isSlamming && Time.time - lastSlamTime > slamCooldown)
        {
            isSlamming = true;
            lastSlamTime = Time.time;

            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0; 
            rb.linearVelocity = velocity;

            rb.AddForce(Vector3.down * slamForce, ForceMode.Impulse);
            isSlamming = false;
        }
    }

    public void EndCrouch()
    {
        float newHeight = playerHeight; 
        if (!Mathf.Approximately(lastColliderHeight, newHeight))
        {
            capsuleCollider.height = newHeight;
            capsuleCollider.center = Vector3.zero; 
            lastColliderHeight = newHeight;
        }
    }

    private void SetupEventListeners()
    {
        controls.Player.Jump.performed += ctx =>
        {
            Debug.Log("Jumpevent");
            lastJumpTime = Time.time;
            fsm.ChangeState(PlayerState.Jumping);
        };

        controls.Player.Move.performed += ctx =>
        {
            Debug.Log("MOVEEVEBNT");
            moveInput = ctx.ReadValue<Vector2>();
            fsm.ChangeState(PlayerState.Walking);
        };

        //controls.Player.Move.canceled += ctx =>
        //{
        //    Debug.Log("END MOVE ");
        //    moveInput = new Vector3(0, 0, 0);
        //};

        controls.Player.Crouch.performed += ctx =>
        {
            Debug.Log("CROUCHPERf");
            didCrouch = true;
            fsm.ChangeState(PlayerState.Crouching);
        };

        controls.Player.Crouch.canceled += ctx =>
        {
            Debug.Log("CROUCHCANCL");
            didCrouch = false;
            fsm.ChangeState(PlayerState.Idle);
        };

        controls.Player.Dash.performed += ctx =>
        {
            Debug.Log("DASHPERF");

            if (Time.time - lastDashTime > dashCooldown)
            {
                lastDashTime = Time.time;
            }
        };

    }

    private bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.down * 0.85f;
        float radius = capsuleCollider.radius * 0.95f;
        return Physics.CheckSphere(origin, radius, groundMask);
    }
}

//public class PlayerController : MonoBehaviour
//{
//    // Game objects 
//    private PlayerControls controls;
//    private Rigidbody rb;
//    private CapsuleCollider capsuleCollider;
//    [SerializeField] private LayerMask groundMask;

//    // Player constants
//    [SerializeField] private float moveSpeed = 5f;
//    [SerializeField] private float playerHeight = 2.0f;
//    [SerializeField] private float crouchHeight = 1.0f;
//    [SerializeField] private float dashDistance = 5f;
//    [SerializeField] private float dashCooldown = 1f;
//    [SerializeField] private float slamForce = 30f;
//    [SerializeField] private float slamCooldown = 0.5f;

//    // Jump physics
//    private const float coyoteTime = 0.10f;
//    private const float fallMultiplier = 2.5f;
//    private const float jumpForce = 5.4f;
//    private const float jumpBufferTime = 0.1f;
//    private const byte MAX_PLAYER_JUMPS = 2;
//    byte jumpsLeft = MAX_PLAYER_JUMPS;

//    // Player State
//    private Vector3 moveInput = Vector3.zero;
//    private float lastGroundTime;
//    private float lastColliderHeight = -1;
//    private float lastJumpTime = -1; 
//    private float lastDashTime = -999f;
//    private float lastSlamTime = -999f;
//    private bool isGrounded = true;
//    private bool isSlamming = false;
//    private bool didJump = false;
//    private bool didCrouch = false;

//    private void OnEnable() => controls.Enable();
//    private void OnDisable() => controls.Disable();

//    private void Awake()
//    {
//        controls = new PlayerControls();
//        capsuleCollider = GetComponent<CapsuleCollider>();
//        lastColliderHeight = capsuleCollider.height;

//        SetupRigidBody();
//        SetupEventHandlers(); 
//    }

//    private void FixedUpdate()
//    {
//        isGrounded = IsGrounded();

//        HandleMovement();
//        HandleJump();
//        HandleFall();
//        HandleCrouch();
//    }

//    private void HandleMovement()
//    {
//        if (Mathf.Abs(moveInput.x) > 0.01f)
//        {
//            // Move along X axis
//            rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * new Vector3(moveInput.x, 0, 0));
//        }
//    }

//    private void HandleFall()
//    {
//        // Fall faster than jump
//        if (rb.linearVelocity.y < 0)
//        {
//            // Avoid implicit alloc during multiplication of `Vector3.up` multipliciation 
//            Vector3 velocity = rb.linearVelocity;
//            velocity.y += (fallMultiplier - 1) * Physics.gravity.y * Time.fixedDeltaTime;
//            rb.linearVelocity = velocity;
//        }
//    }

//    private void HandleJump()
//    {
//        if (isGrounded && didJump)
//        {
//            lastGroundTime = Time.time;
//            didJump = false;
//            jumpsLeft = MAX_PLAYER_JUMPS;
//        }

//        bool jumpBuffered = Time.time - lastJumpTime <= jumpBufferTime;
//        bool canUseCoyoteTime = Time.time - lastGroundTime <= coyoteTime;
//        bool canJump = (isGrounded || canUseCoyoteTime) && jumpBuffered && !didJump;

//        if ((canJump || (jumpBuffered && jumpsLeft > 0)) && !didJump)
//        {
//            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
//            didJump = true;
//            lastJumpTime = -1f;
//            return;
//        }
//    }

//    private void HandleCrouch()
//    {
//        if (!isGrounded && !isSlamming && Time.time - lastSlamTime > slamCooldown)
//        {
//            HandleSlam();
//            return;
//        }

//        float newHeight = didCrouch ? crouchHeight : playerHeight;
//        if (Mathf.Approximately(newHeight, lastColliderHeight)) return;
//        capsuleCollider.height = newHeight;

//        capsuleCollider.center = Vector3.zero;
//        lastColliderHeight = newHeight;
//    }

//    private void HandleDash()
//    {
//        if (Time.time - lastDashTime < dashCooldown) return;

//        // Use move direction if there's input, fallback to facing direction
//        Vector3 dashDirection = new Vector3(moveInput.x, moveInput.y, 0).normalized;
//        if (dashDirection == Vector3.zero)
//            dashDirection = transform.right; // In 2D, right is "forward"

//        Vector3 dashTarget = rb.position + dashDirection * dashDistance;
//        rb.MovePosition(dashTarget);

//        lastDashTime = Time.time;
//    }

//    private void HandleSlam()
//    {
//        isSlamming = true;
//        lastSlamTime = Time.time;

//        Vector3 velocity = rb.linearVelocity;
//        velocity.y = 0;
//        rb.linearVelocity = velocity;

//        // Splat
//        rb.AddForce(Vector3.down * slamForce, ForceMode.Impulse);
//        isSlamming = false;
//    }

//    private void OnDrawGizmosSelected()
//    {
//        if (capsuleCollider == null) return;
//        Gizmos.color = Color.red;
//        Vector3 origin = transform.position + Vector3.down * 0.85f;
//        float radius = capsuleCollider.radius * 0.95f;
//        Gizmos.DrawWireSphere(origin, radius);
//    }

//    private bool IsGrounded()
//    {
//        Vector3 origin = transform.position + Vector3.down * 0.85f;
//        float radius = capsuleCollider.radius * 0.95f;
//        return Physics.CheckSphere(origin, radius, groundMask);
//    } 

//    private void SetupRigidBody()
//    {
//        rb = GetComponent<Rigidbody>();

//        // Reduce drag in air
//        rb.linearDamping = 0;
//    }


//    private void SetupEventHandlers()
//    {
//        // Since input & physics are on different clocks, queue jump input and handle it in FixedUpdate()
//        controls.Player.Jump.performed += ctx => lastJumpTime = Time.time; 

//        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
//        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

//        controls.Player.Crouch.performed += ctx => didCrouch = true;
//        controls.Player.Crouch.canceled += ctx => didCrouch = false;

//        controls.Player.Dash.performed += ctx => HandleDash();
//    }
//}
