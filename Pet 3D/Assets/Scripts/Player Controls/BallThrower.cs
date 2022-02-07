using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallThrower : MonoBehaviour
{
    [SerializeField] GameObject ballUIParent;
    [SerializeField] Ball ballPrefab;
    [SerializeField] List<GameObject> balls;
    int ballCount = 0;

    [SerializeField] Color unusedColor;
    [SerializeField] Color usedColor;

    private void Awake()
    {
        ballPrefab = Resources.Load<Ball>("Prefabs/Ball");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ThrowBall();
        }

        if (Input.GetMouseButtonDown(1))
        {
            RemoveBall();
        }
    }

    void ThrowBall()
    {
        if (ballCount > 1)
            return;

        Ball ball = Instantiate(ballPrefab, Camera.main.transform.position, Quaternion.identity, null);
        ball.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 300);

        ballUIParent.transform.GetChild(0).GetComponent<Image>().color = usedColor;

        ballCount++;
    }

    void RemoveBall()
    {
        if (ballCount <= 0)
            return;

        ballUIParent.transform.GetChild(0).GetComponent<Image>().color = unusedColor;
        ballCount--;
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
