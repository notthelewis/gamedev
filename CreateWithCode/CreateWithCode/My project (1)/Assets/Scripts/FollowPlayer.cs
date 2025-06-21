using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // This allows Unity to allocate a reference to another object, in this case "player"
    public GameObject player;

    // The distance between the camera and the player 
    private readonly Vector3 cameraDistance = new (0, 7, -9);

    void Start()
    {
    }

    // Update is called once per frame
    //void Update()
    //{
    //    // Offset camera behind player by adding to players position 
    //    transform.position = player.transform.position + cameraDistance;
    //}


    // This ensures that the vehicle moves first, then the camera moves
    private void LateUpdate()
    {
        transform.position = player.transform.position + cameraDistance;
    }
}
