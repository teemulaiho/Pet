using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FoodSpawner : MonoBehaviour
{
    [SerializeField] GameObject appleUIParent;
    [SerializeField] List<GameObject> apples = new List<GameObject>();
    [SerializeField] Apple applePrefab;

    private int applesInInventory = 0;

    public GameObject mouseHitPos;

    [SerializeField] Color unusedColor;
    [SerializeField] Color usedColor;

    Ray ray;
    RaycastHit raycastHit;

    private void Start()
    {
        SetAppleCount();
    }

    // Update is called once per frame
    private void Update()
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

        if (Input.GetMouseButtonDown(1))
        {
            //AddFoodToInventory();
        }
    }

    void SetAppleCount()
    {
        for (int i = 0; i < appleUIParent.transform.childCount; i++)
        {
            apples.Add(appleUIParent.transform.GetChild(i).gameObject);
            applesInInventory++;
        }
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
