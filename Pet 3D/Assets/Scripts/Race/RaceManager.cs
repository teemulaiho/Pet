using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    private Camera cam;
    public GameObject racerPrefab;

    private int raceDifficulty;
    private bool running = false;

    private Vector3[] startPositions;
    private Racer[] racers;
    private int playerIndex;

    private void Awake()
    {
        cam = Camera.main;

        raceDifficulty = 1;

        float spacing = -0.75f;

        startPositions = new Vector3[5];
        for (int i = 0; i < startPositions.Length; i++)
            startPositions[i] = new Vector3(-8f, spacing * i, 0f);

        playerIndex = Random.Range(0, 5);

        racers = new Racer[5];
        for (int i = 0; i < racers.Length; i++)
        {
            racers[i] = Instantiate(racerPrefab, startPositions[i], Quaternion.identity).GetComponent<Racer>();

            if (i == playerIndex)
            {
                PetStats stats = new PetStats();
                stats.health = 100.0f;
                stats.stamina = 2;
                racers[i].SetStats(stats);
            }
            else
            {
                PetStats stats = new PetStats();
                stats.health = Random.Range(75.0f, 100.0f);
                stats.stamina = raceDifficulty + Random.Range(-1, 2);
                racers[i].SetStats(stats);
            }
        }
    }

    private void Update()
    {
        if (!running)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartRace();
            }
        }
        else
        {

        }
    }

    private void StartRace()
    {
        foreach (Racer racer in racers)
            racer.Release();

        running = true;
    }
}
