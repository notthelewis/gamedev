using UnityEngine;
using UnityEngine.InputSystem;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 2f;

    private PlayerControls controls;

    //            yee
    private float yaw;
    private float pitch;

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Awake()
    {
        controls = new PlayerControls();
        SetupEventListeners();
    }

    void SetupEventListeners()
    {
        controls.Player.Look.performed += EPlayerLookPerformed;
    }

    void EPlayerLookPerformed(InputAction.CallbackContext ctx) 
    {
        Vector2 delta = ctx.ReadValue<Vector2>();
        yaw = delta.x;
        pitch = Mathf.Clamp(pitch, -85f, -85f);

        transform.Rotate(Vector3.up, yaw * rotationSpeed * Time.deltaTime);
    }
}
