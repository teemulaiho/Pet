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

    public void ThrowItem(Item item, Player player, Vector3 direction)
    {
        //Vector3 spawnPos = player.transform.position;
        //spawnPos += player.transform.forward;
        //spawnPos.y += 0.5f;
        //GameObject newGo = Instantiate(item.prefab, player.debugPointerTip.transform.position, Quaternion.identity);
        //Vector3 forceDirection = Vector3.zero;
        //if (direction.magnitude > 0)
        //    forceDirection = direction;
        //else
        //    forceDirection = player.transform.forward;
        //float forceToAdd = 1f;
        //forceToAdd *= 500f;
        //forceToAdd *= Mathf.Clamp(player.MouseLeftButtonHoldTime * 0.5f, 1f, player.MaxThrowPower);
        //newGo.GetComponent<Rigidbody>().AddForce(forceDirection * forceToAdd);
        GameObject newGo = Instantiate(item.prefab, player.debugPointerTip.transform.position, Quaternion.identity);

        if (newGo.TryGetComponent(out Ball ball)) ball.Throw(player.debugPointerTip.transform.position, player.GetThrowDirection(), player.GetThrowForce(), false);
        else newGo.GetComponent<Rigidbody>().AddForce(player.GetThrowDirection() * player.GetThrowForce());
    }

    public void Track(bool value)
    {
        tracking = value;
    }
}
