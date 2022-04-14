using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] EventManager eventManager;

    [Space]
    [Header("Prefab References")]
    [SerializeField] Transform npcParent;
    [SerializeField] Mole molePrefab;

    List<NPC> instantiatedNPCList;

    private void Awake()
    {
        instantiatedNPCList = new List<NPC>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (eventManager)
        {
            eventManager.onEventAvailable += EventAvailable;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void EventAvailable()
    {
        if (instantiatedNPCList.Count == 0)
            instantiatedNPCList.Add(Instantiate(molePrefab, npcParent));
    }
}
