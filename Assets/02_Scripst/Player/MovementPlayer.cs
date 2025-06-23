using UnityEngine;

public class MovementPlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speedMovement = 5f;
    public float speedRotation = 250f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float backwardSpeed = 3f;

    [Header("References")]
    public WeatherSystem weatherSystem;
    public CharacterController characterController;
    public Animator animator;

    [Header("Camera References")]
    public Transform cameraPivot;
    public Transform mainCamera;

    [Header("Zoom Settings")]
    public float zoomSpeed = 2f;
    public float minZoom = 0.16f; // cerca como primera persona
    public float maxZoom = 12f;   // más lejos
    private float currentZoom = 5f;

    private float rotationX = 20f; // desde arriba
    private float rotationY = 0f;

    private Vector3 velocity;
    private bool isGrounded;
    public float baseSpeed;

    void Start()
    {
        baseSpeed = speedMovement;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        rotationY = transform.eulerAngles.y;
        UpdateCameraPosition();
    }

    void Update()
    {
        isGrounded = characterController.isGrounded;

        // Lluvia reduce velocidad
        speedMovement = (weatherSystem != null && weatherSystem.isRaining) ? baseSpeed * 0.6f : baseSpeed;

        HandleCamera();
        HandleMovement();
        HandleJump();
        UpdateAnimations();
    }

    void HandleMovement()
    {
        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");

        // Rotar jugador con A y D
        transform.Rotate(Vector3.up * moveX * speedRotation * Time.deltaTime);

        Vector3 move = transform.forward * moveZ;

        if (moveZ > 0)
            characterController.Move(move * speedMovement * Time.deltaTime);
        else if (moveZ < 0)
            characterController.Move(move * backwardSpeed * Time.deltaTime);
    }

    void HandleJump()
    {
        if (isGrounded)
        {
            if (velocity.y < 0)
            {
                velocity.y = -2f;
                animator.SetBool("IsJumping", false);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetBool("IsJumping", true);
            }
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    void UpdateAnimations()
    {
        float moveZ = Input.GetAxis("Vertical");
        animator.SetFloat("Speed", moveZ);
    }

    void HandleCamera()
    {
        // Zoom con la rueda
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // Solo rotar la cámara si el click derecho está presionado
        if (Input.GetMouseButton(1)) // Click derecho
        {
            float mouseX = Input.GetAxis("Mouse X") * speedRotation * Time.deltaTime;
            rotationY += mouseX;
        }

        cameraPivot.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        // Calculamos cuánto alejarnos en base al zoom actual
        Vector3 cameraOffset = new Vector3(0f, currentZoom * 0.6f, -currentZoom);

        mainCamera.localPosition = cameraOffset;

        // Si está muy cerca (modo primera persona), ajustamos a los valores dados
        if (currentZoom <= 0.2f)
        {
            mainCamera.localPosition = new Vector3(0f, 0.59f, 0.16f);
        }

        mainCamera.localRotation = Quaternion.Euler(20f, 0f, 0f);
    }
}
