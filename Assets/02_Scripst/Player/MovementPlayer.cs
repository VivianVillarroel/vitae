using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPlayer : MonoBehaviour
{
    public float speedMovement = 5f;
    public float speedRotation = 250f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    public CharacterController characterController;
    public Transform transformPlayer;
    public Transform cameraHolder;
    public Animator animator;

    private Vector3 velocity;
    private Vector3 movement;
    private float rotationX;
    private bool isGrounded;
    private bool isFirstPerson = true;

    private Vector3 firstPersonPos = new Vector3(0f, 0.59f, 0.16f);
    private Vector3 thirdPersonPos = new Vector3(0f, 2f, -4f);

    void Start()
    {
        cameraHolder.localPosition = firstPersonPos;
    }

    void Update()
    {
        isGrounded = characterController.isGrounded;

        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCameraView();
        }

        MovementOfPlayer();
        MovementOfCamera();
        HandleJump();
    }

    void MovementOfPlayer()
    {
        float movZ = Input.GetAxis("Vertical"); // W/S
        float rotX = Input.GetAxis("Horizontal"); // A/D

        // Movimiento adelante y atrás
        movement = transform.forward * movZ;
        characterController.Move(movement * speedMovement * Time.deltaTime);

        // Rotación con A y D
        transformPlayer.Rotate(Vector3.up * rotX * speedRotation * Time.deltaTime);

        // Actualizar parámetro de velocidad para animaciones
        animator.SetFloat("Speed", movZ);
    }

    void MovementOfCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * speedRotation * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * speedRotation * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        cameraHolder.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transformPlayer.Rotate(Vector3.up * mouseX);
    }

    void HandleJump()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            animator.SetBool("IsJumping", false); // aterriza
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("IsJumping", true); // activa salto
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    void ToggleCameraView()
    {
        isFirstPerson = !isFirstPerson;

        cameraHolder.localPosition = isFirstPerson ? firstPersonPos : thirdPersonPos;
    }
}
