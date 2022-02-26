using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject mouseHitPos;
    Ray ray;
    RaycastHit raycastHit;
    private bool tracking;

    private LineRenderer lineRend;

    private void Awake()
    {
        lineRend = GetComponent<LineRenderer>();
        tracking = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (tracking)
        {
            if (!UIController.IsMouseOverUI())
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out raycastHit))
                {
                    if (mouseHitPos)
                        mouseHitPos.transform.position = raycastHit.point;
                }
            }
        }
    }

    public void SpawnItem(Item item)
    {
        if (!Persistent.playerInventory.Contains(item))
            return;

        Vector3 mousePos = raycastHit.point;
        mousePos.y += 2f;

        Instantiate(item.prefab, mousePos, Quaternion.identity);
        Persistent.playerInventory.RemoveItem(item);
    }

    public void Track(bool value)
    {
        tracking = value;
    }
}
