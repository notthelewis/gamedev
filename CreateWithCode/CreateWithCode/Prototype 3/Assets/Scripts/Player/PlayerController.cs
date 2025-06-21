using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public bool gameOver = false;

    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip crashSound;
    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private ParticleSystem dirtParticle;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float gravityModifier = 1f;

    private Rigidbody rb;
    private Animator playerAnimator;
    private AudioSource playerAudio;
    private PlayerControls controls;

    private bool isOnGround = true;

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    void Awake()
    {
        controls = new PlayerControls();

        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 0;

        playerAnimator = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();

        Physics.gravity *= gravityModifier;

        SetupEvents();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            dirtParticle.Play();
            return;
        } 

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            gameOver = true;
            dirtParticle.Stop();
            playerAudio.PlayOneShot(crashSound);
            playerAnimator.SetBool("Death_b", true);
            playerAnimator.SetInteger("DeathType_int", 1);
            explosionParticle.Play();
        }
    }

    void SetupEvents()
    {
        controls.Player.Jump.performed += EPlayerJumpPerformed;
    }

    private void EPlayerJumpPerformed(InputAction.CallbackContext context)
    {
        if (!gameOver && isOnGround)
        {
            isOnGround = false;
            dirtParticle.Stop();
            playerAudio.PlayOneShot(jumpSound);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            playerAnimator.SetTrigger("Jump_trig");
        }
    }
}
