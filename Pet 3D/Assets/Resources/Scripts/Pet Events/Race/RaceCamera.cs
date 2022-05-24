using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceCamera : MonoBehaviour
{
    private Camera cam;

    private Vector3 targetPosition;
    private Vector3 startLine;
    private Vector3 finishLine;

    private Vector3 velocity = Vector3.zero;
    private float smoothTime = 0.5f;

    public void Init(Vector3 finishPosition)
    {
        cam = Camera.main;

        startLine = transform.position;
        targetPosition = transform.position;
        finishLine = finishPosition;
    }

    public void TrackTargets(Racer[] racers)
    {
        float xPosition = 0;
        float xOffset = 3.0f;
        foreach (Racer racer in racers)
            xPosition += racer.transform.position.x;

        xPosition /= racers.Length;
        xPosition += xOffset;

        xPosition = Mathf.Clamp(xPosition, startLine.x, finishLine.x);
        targetPosition.x = xPosition;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
