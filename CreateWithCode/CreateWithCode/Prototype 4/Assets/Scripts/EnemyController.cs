using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float speed = 2.0f;

    private Rigidbody rb;
    private GameObject player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
    }

    void FixedUpdate()
    {
        Vector3 delta = (player.transform.position - transform.position).normalized;
        rb.AddForce(delta * speed);
    }
}
