using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPlayer : MonoBehaviour
{
     public float speedMovement;
     public float speedRotation;
     public CharacterController characterController;
     public Transform transformPlayer;
     public Camera cameraPlayer;

      Vector3 movement;
      float rotationX;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MovementOfPlayer();
        MovementOfCamera();
    }

    void MovementOfPlayer()
    {
        float movX = Input.GetAxis("Horizontal");
        float movZ = Input.GetAxis("Vertical");

        movement = transform.right* movX + transform.forward* movZ;
        characterController.SimpleMove(movement*speedMovement);
    }

    void MovementOfCamera()
    {
        float ratonX = Input.GetAxis("Mouse X") * speedRotation;
        float ratonY = Input.GetAxis("Mouse Y") * speedRotation ;

        rotationX -= ratonY;
        rotationX -= Mathf.Clamp(rotationX, -90f, 90f);

        cameraPlayer.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transformPlayer.Rotate(Vector3.up * ratonX);
    }
}
