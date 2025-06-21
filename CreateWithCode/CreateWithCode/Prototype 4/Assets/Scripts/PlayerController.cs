using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float powerUpStrength = 10f;
    [SerializeField] private float powerUpTimeLimit = 7f;
    [SerializeField] private GameObject powerupIndicator; 

    private PlayerControls controls;
    private Rigidbody rb;
    private Transform cameraTransform;
    private Vector3 moveInput;
    private bool hasPowerup = false;

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    void Awake()
    {
        controls = new PlayerControls();

        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;

        SetupEventListeners();
    }

    private void Update()
    {
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleMove(); 
    }

    void HandleMove()
    {
        if (moveInput.sqrMagnitude <= 0.01f) return;

        Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
        Vector3 moveDirection = (right * moveInput.x + forward * moveInput.y).normalized;

        rb.AddForce(moveDirection * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            powerupIndicator.SetActive(true);

            Destroy(other.gameObject);
            StartCoroutine(PowerupCountdownRoutine());
        } 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasPowerup && collision.gameObject.CompareTag("Enemy"))
        {
            Rigidbody erb = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 delta = collision.gameObject.transform.position - transform.position;
            erb.AddForce(delta * powerUpStrength, ForceMode.Impulse);
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(powerUpTimeLimit);
        hasPowerup = false;
        powerupIndicator.SetActive(false);
    }

    void SetupEventListeners()
    {
        controls.Player.Move.performed += EPlayerMovePerformed; 
        controls.Player.Move.canceled += EPlayerMoveCancelled; 
    }

    void EPlayerMovePerformed(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    void EPlayerMoveCancelled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }
}
