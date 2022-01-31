using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FoodSpawner : MonoBehaviour
{
    [SerializeField] GameObject appleParent;
    [SerializeField] List<GameObject> apples = new List<GameObject>();
    [SerializeField] Apple applePrefab;

    private int applesInInventory = 0;

    [SerializeField] Color unusedColor;
    [SerializeField] Color usedColor;

    private void Start()
    {
        SetAppleCount();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpawnFood();
        }

        if (Input.GetMouseButtonDown(1))
        {
            AddFoodToInventory();
        }
    }

    void SetAppleCount()
    {
        for (int i = 0; i < appleParent.transform.childCount; i++)
        {
            apples.Add(appleParent.transform.GetChild(i).gameObject);
            applesInInventory++;
        }
    }

    void SpawnFood()
    {
        if (!HasFoodInInventory())
            return;

        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mousePos.z = 0f;

        Instantiate(applePrefab, mousePos, Quaternion.identity, this.transform);
        RemoveFoodFromInventory();
    }

    bool HasFoodInInventory()
    {
        return applesInInventory > 0;
    }

    void AddFoodToInventory()
    {
        if (applesInInventory > apples.Count - 1)
            return;

        apples[applesInInventory].GetComponent<Image>().color = unusedColor;
        applesInInventory++;
    }

    void RemoveFoodFromInventory()
    {
        apples[applesInInventory - 1].GetComponent<Image>().color = usedColor;
        applesInInventory--;
    }
}
