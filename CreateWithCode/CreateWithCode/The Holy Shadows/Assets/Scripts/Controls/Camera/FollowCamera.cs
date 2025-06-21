using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject cameraObject;
    private Vector3 offset;

    void Start()
    {
        // Ensure that distance remains the same as set in the scene view
        // This offloads the responsibility of camera positioning to Unity editor, which is a nicer design imo.
        offset = cameraObject.transform.position - this.transform.position;
    }

    void LateUpdate()
    {
        transform.transform.position = cameraObject.transform.position - offset;
    }
}
