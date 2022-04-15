using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool cameraSetOrthographicSize;
    [SerializeField] List<Collider2D> cameraFollowColliders = new List<Collider2D>();
    float buffer = 5f;

    void Update()
    {
        if (cameraSetOrthographicSize)
            CameraSetOrthographicsSize();
    }

    private void CameraSetOrthographicsSize()
    {
        var (center, size) = CalculateOrthographicSize();
        //Camera.main.transform.position = center;
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, size, Time.deltaTime);
    }

    private (Vector3 center, float size) CalculateOrthographicSize()
    {
        var bounds = new Bounds();
        bounds.center = cameraFollowColliders[0].bounds.center; // new Bounds contains center 0,0,0 - unless set otherwise


        //foreach (var col in FindObjectsOfType<Collider2D>()) bounds.Encapsulate(col.bounds);
        //foreach (var col in cameraFollowColliders) bounds.Encapsulate(col.bounds);
        foreach (var col in cameraFollowColliders) bounds.Encapsulate(col.bounds);

        bounds.Expand(buffer);

        var vertical = bounds.size.y;
        var horizontal = bounds.size.x * Camera.main.pixelHeight / Camera.main.pixelWidth;
        Debug.Log("vertical: " + vertical + " horizontal: " + horizontal);

        var size = Mathf.Max(horizontal, vertical) * 0.5f;
        var center = bounds.center + new Vector3(0, 0, -10);

        Debug.Log("bounds: " + bounds);

        return (center, size);
    }

    public void AddFollowCollider(Collider2D colliderToAdd)
    {
        cameraFollowColliders.Add(colliderToAdd);
    }
}
