using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventWindow : MonoBehaviour
{
    public void RaceEvent()
    {
        SceneManager.LoadScene("RaceScene");
    }
}
