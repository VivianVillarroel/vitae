using UnityEngine;

public class MovementPlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speedMovement = 5f;
    public float speedRotation = 250f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float backwardSpeed = 3f; // Velocidad para retroceder

    [Header("References")]
    public WeatherSystem weatherSystem;
    public CharacterController characterController;
    public Transform cameraHolder;
    public Animator animator;

    [Header("Camera Settings")]
    public Vector3 firstPersonPos = new Vector3(0f, 0.59f, 0.16f);
    public Vector3 thirdPersonPos = new Vector3(0f, 2f, -4f);

    public float baseSpeed;
    private Vector3 velocity;
    private float rotationX;
    private bool isGrounded;
    private bool isFirstPerson = true;

    void Start()
    {
        baseSpeed = speedMovement;
        cameraHolder.localPosition = firstPersonPos;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        isGrounded = characterController.isGrounded;

        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCameraView();
        }

        // Aplicar reducción de velocidad cuando llueve
        speedMovement = (weatherSystem != null && weatherSystem.isRaining) ? baseSpeed * 0.6f : baseSpeed;

        HandleMovement();
        HandleCamera();
        HandleJump();
        UpdateAnimations();
    }

    void HandleMovement()
    {
        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");

        Vector3 movement = transform.forward * moveZ;

        // Movimiento hacia adelante/atrás con velocidades diferentes
        if (moveZ > 0)
        {
            characterController.Move(movement * speedMovement * Time.deltaTime);
        }
        else if (moveZ < 0)
        {
            characterController.Move(movement * backwardSpeed * Time.deltaTime);
        }

        // Rotación del personaje
        transform.Rotate(Vector3.up * moveX * speedRotation * Time.deltaTime);
    }

    void HandleCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * speedRotation * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * speedRotation * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        cameraHolder.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleJump()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            animator.SetBool("IsJumping", false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("IsJumping", true);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    void UpdateAnimations()
    {
        float moveZ = Input.GetAxis("Vertical");

        // Animación de caminar/retroceder
        animator.SetFloat("Speed", moveZ);

        // Animación de salto (ya manejada en HandleJump)
    }

    void ToggleCameraView()
    {
        isFirstPerson = !isFirstPerson;
        cameraHolder.localPosition = isFirstPerson ? firstPersonPos : thirdPersonPos;
    }
}