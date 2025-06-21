using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerControllerX : MonoBehaviour
{
    private readonly float speed = 40;
    private readonly float rotationSpeed = 100;

    private InputAction moveAction;

    // Start is called before the first frame update
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    //void FixedUpdate()
    //{
    //    // get the user's vertical input
    //    verticalInput = Input.GetAxis("Vertical");
    //    // move the plane forward at a constant rate
    //    transform.Translate(Vector3.back * speed);
    //    // tilt the plane up/down based on up/down arrow keys
    //    transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
    //}

    private void Update()
    {
        // Move plane forward at a constant rate
        transform.Translate(speed * Time.deltaTime * Vector3.forward);

        // Tilt dependent on up/down arrow keys
        transform.Rotate(moveAction.ReadValue<Vector2>().y * Time.deltaTime * rotationSpeed, 0, 0);
    }
}
