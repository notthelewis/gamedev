using UnityEngine;
using UnityEngine.InputSystem;

public class Target : MonoBehaviour {
    public GameManager gameManager;
    public ParticleSystem explosionParticle;
    public int pointValue;
    public bool isBadObject;

    private Rigidbody rb;
    private PlayerControls controls;

    private float minSpeed = 12;
    private float maxSpeed = 16;
    private float torque = 9.5f;
    private float xRange = 4;
    private float ySpawnPos = -5.75f;

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Awake() {
        controls = new PlayerControls();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        rb = GetComponent<Rigidbody>();

        // Spawn below player view line at random X
        transform.position = new Vector3(Random.Range(-xRange, xRange), ySpawnPos);

        // Throw at a random strength
        rb.AddForce(Vector3.up * Random.Range(minSpeed, maxSpeed), ForceMode.Impulse);

        // Apply rotation (random X, Y, Z)
        rb.AddTorque(
            Random.Range(-torque, torque),
            Random.Range(-torque, torque),
            Random.Range(-torque, torque),
            ForceMode.Impulse
        );


        SetupEventListeners();
    }

    private void Update() {
        if (gameManager.isGameOver) {
            Kaboom();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!isBadObject) {
            Debug.Log($"{gameObject.name}");
            gameManager.GameOver(GameOverReason.MISS_GOOD);
        }
        Destroy(gameObject);
    }

    private void SetupEventListeners() {
        controls.Click.Click.performed += EPlayerClickPerformed;
    }

    private void EPlayerClickPerformed(InputAction.CallbackContext obj) {
        if (gameManager.isGameOver) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (!Physics.Raycast(ray, out var hit)) return;
        if (hit.collider.gameObject != gameObject) return;

        if (isBadObject)
            gameManager.GameOver(GameOverReason.HIT_BAD);
        else
            gameManager.AddScore(pointValue);

        Kaboom();
    }

    private void Kaboom() {
        Destroy(gameObject);
        Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
    }
}
