using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RaceManager : MonoBehaviour
{
    private Camera cam;
    private Vector3 camStartPosition;
    private Vector3 camTargetPosition;
    public Transform finishLine;
    public GameObject racerPrefab;

    public TMP_Text time;
    public RaceScoreScreen scoreScreen;

    private int raceDifficulty;
    private int prizePool;

    private Vector3[] startPositions;
    private Color[] colors;
    private Racer[] racers;
    private int playerIndex;

    private float startTimer;

    private float raceTimer;
    private int racersFinished;
    private bool running = false;
    private bool raceOver = false;

    private void Awake()
    {
        cam = Camera.main;
        camStartPosition = cam.transform.position;
        camTargetPosition = cam.transform.position;

        raceDifficulty = 1;
        prizePool = 225;

        float spacing = -0.75f;

        startPositions = new Vector3[5];
        for (int i = 0; i < startPositions.Length; i++)
            startPositions[i] = new Vector3(0f, spacing * i, 0f);

        playerIndex = Random.Range(0, 5);

        racers = new Racer[5];
        for (int i = 0; i < racers.Length; i++)
        {
            racers[i] = Instantiate(racerPrefab, startPositions[i], Quaternion.identity).GetComponent<Racer>();

            if (i == playerIndex)
            {
                racers[i].SetStats(Persistent.petStats);
                racers[i].isPlayerPet = true;
            }
            else
            {
                PetStats stats = new PetStats();
                stats.name = "AI Pet";
                stats.health = 100.0f;
                stats.stamina = raceDifficulty + Random.Range(-1, 2);

                racers[i].SetStats(stats);
            }
        }
        scoreScreen.gameObject.SetActive(false);

        raceTimer = 0f;
        racersFinished = 0;
    }

    private void Update()
    {
        if (!running)
        {
            if (raceOver)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    SceneManager.LoadScene("HomeScene");
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    StartRace();
            }
        }
        else
        {
            raceTimer += Time.deltaTime;
            int intTime = (int)raceTimer;
            int minutes = intTime / 60;
            int seconds = intTime % 60;
            int milliseconds = (int)(raceTimer * 100) % 100;
            time.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);

            foreach (Racer racer in racers)
            {
                if (!racer.Finished && racer.transform.position.x >= finishLine.transform.position.x)
                {
                    scoreScreen.namePlates[racersFinished].time.text = time.text;
                    scoreScreen.namePlates[racersFinished].petName.text = racer.Name;
                    scoreScreen.namePlates[racersFinished].place.text = (racersFinished + 1).ToString() + ".";

                    racer.Winnings = (prizePool / (racersFinished + 1));
                    scoreScreen.namePlates[racersFinished].winnings.text = racer.Winnings.ToString();

                    racer.Rank = racersFinished + 1;
                    racer.Finished = true;
                    racersFinished += 1;
                }
            }

            if (racersFinished == racers.Length)
            {
                scoreScreen.gameObject.SetActive(true);
                time.gameObject.SetActive(false);
                running = false;
                raceOver = true;
                DistributeRacerRewards();
                DistributePlayerPetExperience();
            }
        }
        UpdateCamera();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("HomeScene");
        }
    }

    private void StartRace()
    {
        foreach (Racer racer in racers)
            racer.Release();

        running = true;
    }

    private void UpdateCamera()
    {
        float xPosition = 0;
        float xOffset = 3.0f;
        foreach (Racer racer in racers)
            xPosition += racer.transform.position.x;

        xPosition /= racers.Length;
        xPosition += xOffset;

        xPosition = Mathf.Clamp(xPosition, camStartPosition.x, finishLine.transform.position.x);
        camTargetPosition.x = xPosition;
        cam.transform.position = Vector3.Lerp(cam.transform.position, camTargetPosition, 0.0125f);
    }

    private void DistributeRacerRewards()
    {
        foreach (Racer racer in racers)
        {
            if (racer.isPlayerPet)
                Persistent.playerInventory.IncreaseMoney(racer.Winnings);
        }
    }

    private void DistributePlayerPetExperience()
    {
        foreach (Racer racer in racers)
        {
            if (racer.isPlayerPet)
            {
                if (racer.Rank > 0)
                    Persistent.AddExperience(10f);
            }
        }
    }
}
