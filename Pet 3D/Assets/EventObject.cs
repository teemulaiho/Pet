using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObject : MonoBehaviour
{
    [SerializeField] UIController uIController;

    public void OpenEvents()
    {
        if (uIController != null)
            uIController.OpenEventWindow();
    }
}
