using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] static EventManager eventManager;

    [Space]
    [Header("Prefab References")]
    [SerializeField] static Transform npcParent;
    [SerializeField] static Mole molePrefab;

    static List<NPC> instantiatedNPCList;

    private void Awake()
    {
        eventManager = FindObjectOfType<EventManager>();

        npcParent = transform.GetChild(0);
        molePrefab = Resources.Load<Mole>("Prefabs/NPCs/Mole/Mole");
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
        InstantiateNPC();
    }

    public static void InstantiateNPC(int context = 0)
    {
        if (instantiatedNPCList.Count == 0)
        {
            NPC npc = Instantiate(molePrefab, npcParent);
            Mole mole = npc as Mole;

            if (mole)
            {
                if (context == 0) // Delivering Event
                    mole.Initialize(false, true);
                else if (context == 1) // Delivering Tetherball
                    mole.Initialize(true, false);
            }

            instantiatedNPCList.Add(npc);
            NotificationManager.ReceiveNotification(NotificationType.NPCSpawn, 1);
        }
    }
}
