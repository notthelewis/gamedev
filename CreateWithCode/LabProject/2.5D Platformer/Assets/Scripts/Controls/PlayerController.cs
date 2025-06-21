using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayer
{
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float crouchTransitionDuration = 0.1f;
    [SerializeField] private LayerMask groundMask;

    private const float playerHeight = 2.0f;
    private const float playerCrouchingHeight = 1.0f;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private PlayerControls controls;

    private PlayerState state;
    private Coroutine crouchRoutine;

    private Vector2 moveInput = Vector2.zero;
    private Vector3 targetPos = Vector3.zero;

    private bool isGrounded = false;
    private bool jumpConsumed = false; 
    private float lastJumpTime = -1f;
    private float lastGroundTime = 0f;

    private void Awake()
    {
        controls = new PlayerControls();
        state = new PlayerState(this);

        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb.linearDamping = 0; // Reduce drag in air

        state.Init(State.Idle);

        SetupInputEventListeners();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    private void FixedUpdate()
    {
        isGrounded = IsGrounded();
        if (isGrounded)
        {
            lastGroundTime = Time.time;
            state.Clear(State.Jumping);
        }

        state.Tick();
    }

    public void Move()
    {
        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            targetPos.x = moveInput.x;
            rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * targetPos);
        }
    }

    public void Crouch()
    {
        if (crouchRoutine != null)
            StopCoroutine(crouchRoutine);

        crouchRoutine = StartCoroutine(SmoothCrouch(playerHeight, playerCrouchingHeight));
    }

    public void Stand()
    {
        if (crouchRoutine != null)
            StopCoroutine(crouchRoutine);

        crouchRoutine = StartCoroutine(SmoothCrouch(playerCrouchingHeight, playerHeight));
    }

    private IEnumerator SmoothCrouch(float fromHeight, float toHeight)
    {
        float elapsed = 0f;
        float halfHeightDiff = (fromHeight - toHeight) / 2f;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos - new Vector3(0, halfHeightDiff,0);

        while (elapsed < crouchTransitionDuration)
        {
            float t = elapsed / crouchTransitionDuration;
            float height = Mathf.Lerp(fromHeight, toHeight, t);

            capsuleCollider.height = height;
            transform.position = Vector3.Lerp(startPos, endPos, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        capsuleCollider.height = toHeight;
        transform.position = endPos;
    }

    public void CheckJump()
    {
        if (isGrounded) lastGroundTime = Time.time;

        bool canUseCoyoteTime = Time.time - lastGroundTime <= coyoteTime;
        bool jumpBuffered = Time.time - lastJumpTime <= jumpBufferTime;

        if ((isGrounded || canUseCoyoteTime) && jumpBuffered && !jumpConsumed)
        {
            lastJumpTime = -1f;
            jumpConsumed = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            state.Clear(State.Jumping);
        }

        if (isGrounded) jumpConsumed = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (capsuleCollider == null) return;
        Gizmos.color = Color.red;
        Vector3 origin = transform.position + Vector3.down * 0.1f;
        float radius = capsuleCollider.radius * 0.95f;
        Gizmos.DrawWireSphere(origin, radius);
    }

    public bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.down * 0.85f;
        float radius = capsuleCollider.radius * 0.85f;
        return Physics.CheckSphere(origin, radius, groundMask);
    }

    void SetupInputEventListeners()
    {
        controls.Player.Move.performed += EPlayerMovePerformed;
        controls.Player.Move.canceled += EPlayerMoveCancelled;

        controls.Player.Crouch.performed += EPlayerCrouchPerformed;
        controls.Player.Crouch.canceled += EPlayerCrouchCancelled;

        controls.Player.Jump.performed += EPlayerJumpPerformed; 
    }

    private protected void EPlayerMovePerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
        state.Set(State.Moving);
    }

    private protected void EPlayerMoveCancelled(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
        state.Clear(State.Moving);
    }

    private protected void EPlayerCrouchPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        state.Set(State.Crouching);
    }

    private protected void EPlayerCrouchCancelled(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        state.Clear(State.Crouching);
    }

    private protected void EPlayerJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        lastJumpTime = Time.time;
        state.Set(State.Jumping);
    }
}
