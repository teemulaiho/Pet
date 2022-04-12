using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionPlaceHolder : MonoBehaviour
{
    [SerializeField] Transform mask;
    [SerializeField] RectTransform moveableArea;
    [SerializeField] RectTransform objectToMove;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] List<Sprite> objectToMoveSpriteList;

    Vector2 nextPos;
    float speed = 32f;

    private void Awake()
    {
        moveableArea = this.GetComponent<RectTransform>();
        mask = transform.GetChild(0);

        for (int i = 0; i< mask.childCount; i++)
        {
            if (mask.GetChild(i).name.Contains("ObjectToMove"))
                objectToMove = mask.GetChild(i).GetComponent<RectTransform>();
        }

        objectToMoveSpriteList = new List<Sprite>();
        objectToMoveSpriteList.AddRange(Resources.LoadAll<Sprite>("Sprites/UI/PlaceHolder"));

        if (objectToMoveSpriteList.Count == 0)
        {
            objectToMove.GetComponentInChildren<Image>().sprite = defaultSprite;
        }
        else
        {
            int randomIndex = Random.Range(0, objectToMoveSpriteList.Count);
            objectToMove.GetComponentInChildren<Image>().sprite = objectToMoveSpriteList[randomIndex];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        nextPos = objectToMove.anchoredPosition;
        nextPos.x -= speed * Time.deltaTime;
        objectToMove.anchoredPosition = nextPos;

        if(objectToMove.anchoredPosition.x <= -moveableArea.rect.width)
        {
            nextPos = objectToMove.anchoredPosition;
            nextPos.x = moveableArea.rect.width;
            objectToMove.anchoredPosition = nextPos;
        }
    }
}
