using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopObject : MonoBehaviour
{
    UIController uiController;
    ShopWindow shopWindow;
    [SerializeField] GameObject icon;
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

    private void Update()
    {
        if (icon)
            icon.transform.Rotate(Vector3.right, 45.0f * Time.deltaTime);
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
