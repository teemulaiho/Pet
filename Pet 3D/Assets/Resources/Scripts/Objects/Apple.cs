using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : Food
{
    public Item itemData;

    private void Awake()
    {
        healthGain = 25.0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FoodCatcher"))
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FoodCatcher"))
        {
            Destroy(this.gameObject);
        }
    }
}
