using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] GameObject appleUIParent;
    [SerializeField] TMP_Text foodInventoryCount;
    [SerializeField] Apple applePrefab;

    public GameObject mouseHitPos;
    Ray ray;
    RaycastHit raycastHit;

    private void Start()
    {
        SetAppleCount();

        PlayerInventory.current.onInventoryValueChange += UpdateFoodUI;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!MouseLook.IsMouseOverUI())
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit))
            {
                if (mouseHitPos)
                    mouseHitPos.transform.position = raycastHit.point;

                if (Input.GetMouseButtonDown(0))
                {
                    SpawnFood();
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            AddFoodToInventory();
        }
    }

    void SetAppleCount()
    {
        PlayerInventory.current.AddItemToPlayerInventory(applePrefab.name);
        PlayerInventory.current.AddItemToPlayerInventory(applePrefab.name);
        PlayerInventory.current.AddItemToPlayerInventory(applePrefab.name);

        foodInventoryCount.text = "x" + PlayerInventory.current.GetInventoryCount(applePrefab.name).ToString();
    }

    void SpawnFood()
    {
        if (!HasFoodInInventory())
            return;

        Vector3 mousePos = raycastHit.point;
        mousePos.y += 2f;

        Instantiate(applePrefab, mousePos, Quaternion.identity);
        RemoveFoodFromInventory();
    }

    bool HasFoodInInventory()
    {
       return PlayerInventory.current.HasItemInventory(applePrefab.name);
    }

    void AddFoodToInventory()
    {
        PlayerInventory.current.AddItemToPlayerInventory(applePrefab.name);
        UpdateFoodUI();
    }

    void RemoveFoodFromInventory()
    {
        PlayerInventory.current.RemoveItemFromPlayerInventory(applePrefab.name);
        UpdateFoodUI();
    }

    void UpdateFoodUI()
    {
        foodInventoryCount.text = "x" + PlayerInventory.current.GetInventoryCount(applePrefab.name).ToString();
    }

    private void OnEnable()
    {
        appleUIParent.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (appleUIParent)
            appleUIParent.gameObject.SetActive(false);
    }
}
