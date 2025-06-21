using UnityEngine;

public class FollowPlayerX : MonoBehaviour
{
    public GameObject plane;

    private Vector3 offset = new (40,5,-10);
    private Quaternion rotation = new(0, -90, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        // This is just so that the basic camera looks decent in preview and will match the value set
        // within here
        if (transform.rotation != rotation)
        {
            transform.rotation = rotation;
        }
    }

    void LateUpdate()
    {
        transform.position = plane.transform.position + offset; 
    }
}
