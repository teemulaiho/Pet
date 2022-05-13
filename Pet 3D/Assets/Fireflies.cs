using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireflies : MonoBehaviour
{
    private Vector3 initialPosition;
    float distance = 0.5f;
    float angle = 0;
    float speed = 15f;
    public enum MovementType
    {
        Static,
        Circle
    }
    public MovementType movementType;

    private void Awake()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        if (movementType == MovementType.Circle)
        {
            angle += Time.deltaTime * speed;
            if (angle >= 360)
                angle -= 360;

            float angleInRad = angle * Mathf.Deg2Rad;

            Vector3 newPosition = initialPosition;
            newPosition.x += Mathf.Cos(angleInRad) * distance;
            newPosition.z += Mathf.Sin(angleInRad) * distance;

            transform.position = newPosition;
        }
    }
}
