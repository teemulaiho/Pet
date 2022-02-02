using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    float healthGain = 0.5f;

    public float HealthGain
    {
        get { return healthGain; }
        set { healthGain = value; }
    }

    public void Eat()
    {
        Destroy(this.gameObject);
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
