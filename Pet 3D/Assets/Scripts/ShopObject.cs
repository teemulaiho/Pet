using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopObject : MonoBehaviour
{
    [SerializeField] UIController uIController;

    public void OpenShop()
    {
        if (uIController != null)
            uIController.OpenShopWindow();
    }
}
