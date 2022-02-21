using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] GameObject shopWindowGO;
    [SerializeField] CanvasGroup shopWindow;

    bool isOpen;

    Coroutine ShopToggleCoroutine;

    [SerializeField] List<Item> shopItemsForSale;
    Dictionary<Item, int> shopInventory;

    [SerializeField] ShopItem shopItemSlot;
    [SerializeField] Item currentItem;
    [SerializeField] int currentItemIndex;

    private void Awake()
    {
        shopWindowGO = GameObject.FindGameObjectWithTag("UIShop");
        shopWindow = shopWindowGO.GetComponent<CanvasGroup>();

        shopItemSlot = shopWindowGO.GetComponentInChildren<ShopItem>();

        shopInventory = new Dictionary<Item, int>();

        foreach (var item in shopItemsForSale)
        {
            shopInventory.Add(item, 1);
        }

        currentItemIndex = 0;
        currentItem = shopItemsForSale[currentItemIndex];
        UpdateShopItemSlots();
    }

    // Start is called before the first frame update
    void Start()
    {
        shopWindow.alpha = 0f;
    }

    public void Toggle()
    {
        isOpen = !isOpen;

        if (isOpen)
            Open();
        else
            Close();
    }

    public void Open()
    {
        if (ShopToggleCoroutine == null)
            ShopToggleCoroutine = StartCoroutine(OpenShop());
    }

    public void Close()
    {
        if (ShopToggleCoroutine == null)
            ShopToggleCoroutine = StartCoroutine(CloseShop());
    }

    public void NextItem()
    {
        currentItemIndex++;
        if (currentItemIndex >= shopItemsForSale.Count)
        {
            currentItemIndex = 0;
        }

        currentItem = shopItemsForSale[currentItemIndex];

        UpdateShopItemSlots();
    }

    public void UpdateShopItemSlots()
    {
        shopItemSlot.UpdateShopItem(currentItem);
    }

    public void PurchaseItem()
    {
        Debug.Log("Buying item " + currentItem.GetItemName());
        PlayerInventory.current.AddItemToPlayerInventory(currentItem.GetItemName());
    }

    IEnumerator OpenShop()
    {
        MouseLook.ReleaseCursor();

        float openTime = 1f;
        float openDT = 0f;
        bool opening = true;

        while (opening)
        {
            openDT += Time.deltaTime;

            shopWindow.alpha = openDT / openTime;

            if (openDT >= openTime)
            {
                opening = false;
                ShopToggleCoroutine = null;
            }

            yield return null;
        }
    }
    IEnumerator CloseShop()
    {
        MouseLook.LockCursor();

        float closeTime = 1f;
        float closeDT = 0f;
        bool closing = true;

        while (closing)
        {
            closeDT += Time.deltaTime;

            shopWindow.alpha = 1 - closeDT / closeTime;

            if (closeDT >= closeTime)
            {
                closing = false;
                ShopToggleCoroutine = null;
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

        }
    }
}
