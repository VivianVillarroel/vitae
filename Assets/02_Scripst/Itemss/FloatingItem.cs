using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    public float rotationSpeed = 50f;      // Velocidad de rotación (grados por segundo)
    public float floatSpeed = 0.5f;        // Velocidad de movimiento vertical
    public float floatHeight = 0.25f;      // Altura máxima desde el punto base

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Rotar sobre el eje Y
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Movimiento vertical tipo senoidal
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
