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
        Vector3 mousePos = raycastHit.point;
        mousePos.y += 2f;

        Instantiate(item.prefab, mousePos, Quaternion.identity);
        Persistent.playerInventory.RemoveItem(item);
    }

    public void ThrowItem(Item item, Player player)
    {
        Vector3 spawnPos = player.transform.position;
        spawnPos += player.transform.forward;
        spawnPos.y += 0.5f;
        GameObject newGo = Instantiate(item.prefab, spawnPos, Quaternion.identity);

        Vector3 forceToAdd = player.transform.forward;
        forceToAdd *= 500f;
        newGo.GetComponent<Rigidbody>().AddForce(forceToAdd);
    }

    public void Track(bool value)
    {
        tracking = value;
    }
}
