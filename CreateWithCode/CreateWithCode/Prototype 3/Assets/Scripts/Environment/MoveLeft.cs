using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    [SerializeField] private float speed = 30f;


    private PlayerController playerController;
    private bool isGround;

    private const float OOB = -15;

    private void Awake()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        isGround = gameObject.CompareTag("Ground");
    }

    void Update()
    {
        if (!playerController.gameOver)
        {
            transform.Translate(speed * Time.deltaTime * Vector3.left);

            if (!isGround && transform.position.x < OOB)
            {
                Destroy(gameObject);
            }
        }
    }
}
