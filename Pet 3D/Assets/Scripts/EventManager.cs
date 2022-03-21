using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    [SerializeField] Event eventPrefab;
    [SerializeField] Transform eventParentTransform;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartEvent(Event eventToStart)
    {
        string eventName = eventToStart.EventName;

        if (eventName.Contains("Race"))
        {
           SceneManager.LoadScene("RaceScene");
        }
        else if (eventName.Contains("Skill Contest"))
        {
            SceneManager.LoadScene("SkillContestScene");
        }
        else if (eventName.Contains("Battle"))
        {
            SceneManager.LoadScene("BattleScene");
        }
    }
}
