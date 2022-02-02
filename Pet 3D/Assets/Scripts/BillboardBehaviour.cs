using UnityEngine;

public class BillboardBehaviour : MonoBehaviour
{
    public Transform cam;

    public void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        LookAtCamera();
    }

    private void LookAtCamera()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
