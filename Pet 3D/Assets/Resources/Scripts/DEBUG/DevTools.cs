using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevTools : MonoBehaviour
{
    private void Awake()
    {
        if (!Application.isEditor)
            this.gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
            AddPetIntelligence();    
        
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.T))
            AddMoney();
    }

    void AddPetIntelligence()
    {
        Persistent.AddIntellect(1f);
    }

    void AddMoney()
    {
        Persistent.playerInventory.IncreaseMoney(1000);
    }
}
