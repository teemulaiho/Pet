using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopWindow : MonoBehaviour
{
    UIController uiController;

    public ShopItemSlot[] itemSlots;
    private List<Item> shopItems;

    [SerializeField] TMP_Text itemTitle;
    [SerializeField] GameObject quantityParent;
    [SerializeField] Slider quantitySlider;
    [SerializeField] TMP_Text quantityValue;
    [SerializeField] TMP_Text costValue;

    [SerializeField] Button purchaseButton;

    private Item selectedItem;

    public delegate void OnWindowClose(bool isOpen);
    public event OnWindowClose onWindowClose;

    [SerializeField] GameObject tetherBall;

    private void Awake()
    {
        uiController = FindObjectOfType<UIController>();

        tetherBall = FindObjectOfType<TetherBall>().gameObject;
        tetherBall.SetActive(false);

        shopItems = new List<Item>();
        quantityParent.SetActive(false);
        quantitySlider.value = quantitySlider.minValue;
        purchaseButton.interactable = false;

        foreach (ShopItemSlot slot in itemSlots)
            slot.AssignDelegate(SelectSlot);
    }

    private void Start()
    {
        if (itemTitle)
            itemTitle.text = "";

        foreach (Item item in Persistent.itemDatabase.items)
            shopItems.Add(item);

        UpdateSlots();

        GetComponentInChildren<UIButton>().close += Close;
    }

    private void Update()
    {
        if (selectedItem != null)
        {
            quantityValue.text = "x" + quantitySlider.value.ToString();
            costValue.text = (quantitySlider.value * selectedItem.cost).ToString() + "$";

            if (selectedItem.cost * quantitySlider.value <= Persistent.playerInventory.money)
            {
                purchaseButton.interactable = true;
            }
            else
            {
                purchaseButton.interactable = false;
            }
        }
    }

    private void UpdateSlots()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < shopItems.Count)
            {
                itemSlots[i].AssignItem(shopItems[i]);
            }
            else
            {
                itemSlots[i].UnassignItem();
            }
        }
    }

    public void SelectSlot(ShopItemSlot shopItemSlot)
    {
        if (shopItemSlot.item != null)
        {
            selectedItem = shopItemSlot.item;

            if (selectedItem.itemName.Contains("Tetherball"))
                quantitySlider.gameObject.SetActive(false);
            else
            {
                quantitySlider.gameObject.SetActive(true);
                quantitySlider.value = quantitySlider.minValue;
            }

            quantityParent.SetActive(true);

            foreach (ShopItemSlot slot in itemSlots)
            {
                if (slot == shopItemSlot)
                {
                    slot.Highlight(true);
                }
                else
                    slot.Highlight(false);
            }
        }

        if (itemTitle && shopItemSlot && shopItemSlot.item)
            itemTitle.text = shopItemSlot.item.itemName;
        else
            itemTitle.text = "";
    }

    public void PurchaseItem()
    {
        if (!selectedItem)
            return;

        if (Persistent.playerInventory.money < selectedItem.cost)
            return;

        if (selectedItem.itemName.Contains("Tether"))
        {
            tetherBall.SetActive(true);
            shopItems.Remove(selectedItem);
            UpdateSlots();
            NotificationManager.ReceiveNotification(NotificationType.Tetherball, 0);
            NPCManager.InstantiateNPC(1);
        }
        else
            Persistent.playerInventory.AddItem(selectedItem, (int)quantitySlider.value);

        Persistent.playerInventory.DecreaseMoney(selectedItem.cost * (int)quantitySlider.value);
    }

    void Close()
    {
        selectedItem = null;
        onWindowClose(false);
        uiController.CloseUIWindw(this.gameObject, false);
    }
}
