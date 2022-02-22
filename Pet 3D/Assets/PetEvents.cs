using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PetEvents : MonoBehaviour
{
    public void RaceEvent()
    {
        SceneManager.LoadScene("RaceScene");
    }
}
