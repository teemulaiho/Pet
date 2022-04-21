using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopObject : MonoBehaviour
{
    UIController uiController;
    ShopWindow shopWindow;
    bool _isOpen;

    private void Awake()
    {
        uiController = FindObjectOfType<UIController>();
        shopWindow = FindObjectOfType<ShopWindow>();
    }

    private void Start()
    {
        if (!shopWindow && uiController)
            shopWindow = uiController.GetShopWindow();

        if (shopWindow)
            shopWindow.onWindowClose += OnWindowClose;
    }

    public void Toggle()
    {
        if (uiController)
        {
            if (!_isOpen)
                _isOpen = uiController.OpenUIWindow(this.gameObject);
            else
                _isOpen = uiController.CloseUIWindw(this.gameObject);
        }
    }

    void OnWindowClose(bool isOpen)
    {
        _isOpen = isOpen;
    }
}
