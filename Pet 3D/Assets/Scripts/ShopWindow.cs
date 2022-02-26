using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopWindow : MonoBehaviour
{
    public ShopItemSlot[] itemSlots;
    private List<Item> shopItems;

    [SerializeField] GameObject quantityParent;
    [SerializeField] Slider quantitySlider;
    [SerializeField] TMP_Text quantityValue;

    [SerializeField] Button purchaseButton;

    private Item selectedItem;

    private void Awake()
    {
        shopItems = new List<Item>();
        quantityParent.SetActive(false);
        quantitySlider.value = quantitySlider.minValue;
        purchaseButton.interactable = false;

        foreach (ShopItemSlot slot in itemSlots)
            slot.AssignDelegate(SelectSlot);
    }

    private void Start()
    {
        foreach (Item item in Persistent.itemDatabase)
            shopItems.Add(item);

        UpdateSlots();
    }

    private void Update()
    {
        if (selectedItem != null)
        {
            quantityValue.text = "x" + quantitySlider.value.ToString();

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

            quantitySlider.value = quantitySlider.minValue;
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
    }

    public void PurchaseItem()
    {
        Persistent.playerInventory.AddItem(selectedItem, (int)quantitySlider.value);
    }
}
