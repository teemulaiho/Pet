using UnityEngine;

public class BillboardBehaviour : MonoBehaviour
{
    public Transform cam;

    public void Start()
    {
        if (Camera.main)
            cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (cam)
            LookAtCamera();
        else
            if (Camera.main)
                cam = Camera.main.transform;
    }

    private void LookAtCamera()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
