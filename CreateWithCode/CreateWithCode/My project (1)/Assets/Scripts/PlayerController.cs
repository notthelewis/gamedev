using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    private readonly float speedInMetersPerSecond = 20;
    private readonly float turnSpeed = 40;
    private InputAction moveAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Call expensive `FindAction` one time at object instantiation  
        moveAction = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        // Move vehicle forward @ 20 meters/second
        transform.Translate(speedInMetersPerSecond * Time.deltaTime * Vector3.forward);

        // Rotate instead of translate, so that the vehicle turns instead of sliding
        transform.Rotate(moveAction.ReadValue<Vector2>().x * Time.deltaTime * turnSpeed * Vector3.up);
    }
}
