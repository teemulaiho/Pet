using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BallThrower : MonoBehaviour
{
    [SerializeField] GameObject ballUIParent;
    [SerializeField] TMP_Text ballInventoryCount;
    [SerializeField] Ball ballPrefab;
    [SerializeField] List<GameObject> balls;

    private void Awake()
    {
        ballPrefab = Resources.Load<Ball>("Prefabs/Ball");
    }

    private void Start()
    {
        UpdateBallUI();

        PlayerInventory.current.onInventoryValueChange += UpdateBallUI;
    }

    // Update is called once per frame
    void Update()
    {
        if (!MouseLook.IsMouseOverUI())
        {
            if (Input.GetMouseButtonDown(0))
            {
                ThrowBall();
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            //RemoveBall();
        }
    }

    void ThrowBall()
    {
        if (!PlayerInventory.current.HasItemInventory(ballPrefab.name))
            return;

        Ball ball = Instantiate(ballPrefab, Camera.main.transform.position, Quaternion.identity, null);
        ball.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 300);

        PlayerInventory.current.RemoveItemFromPlayerInventory(ballPrefab.name);
    }

    //void RemoveBall()
    //{
    //    PlayerInventory.RemoveItemFromPlayerInventory(ballPrefab.name);

    //    if (ballCount <= 0)
    //        return;

    //    ballUIParent.transform.GetChild(0).GetComponent<Image>().color = unusedColor;
    //    ballCount--;
    //}


    void UpdateBallUI()
    {
        ballInventoryCount.text = "x" + PlayerInventory.current.GetInventoryCount(ballPrefab.name).ToString();
    }

    private void OnEnable()
    {
        ballUIParent.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (ballUIParent)
            ballUIParent.gameObject.SetActive(false);
    }
}
