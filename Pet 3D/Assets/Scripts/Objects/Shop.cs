using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] GameObject shopWindowGO;
    [SerializeField] CanvasGroup shopWindow;

    public bool IsOpen { get; set; }

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

    public void ChangeItem(int value)
    {
        currentItemIndex += value;

        if (currentItemIndex >= shopItemsForSale.Count)
            currentItemIndex = 0;
        else if (currentItemIndex < 0)
            currentItemIndex = shopItemsForSale.Count - 1;

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
        MoveController.CanMove = false;

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
        MoveController.CanMove = true;

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
