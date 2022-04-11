using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerThrowDirection : MonoBehaviour
{
    Player player;
    [SerializeField] RectTransform reticleArea;
    [SerializeField] RectTransform reticle;
    Vector2 aimPos = Vector2.zero;

    private void Awake()
    {
        player = FindObjectOfType<Player>();    
    }

    // Start is called before the first frame update
    void Start()
    {
        player.onAim += Aim;
    }

    public void Aim(Vector3 aimDirection)
    {
        if (aimDirection.magnitude == 0f)
            ResetReticle();
        else
        {
            aimPos.x =  aimDirection.x / Screen.width;
            aimPos.y =  aimDirection.y / Screen.height;

            aimPos.x = aimPos.x * reticleArea.rect.width - reticle.rect.width;
            aimPos.y = aimPos.y * reticleArea.rect.height - reticle.rect.height;

            reticle.anchoredPosition = aimPos;
        }
    }

    public void ResetReticle()
    {
        aimPos = Vector2.zero;
        reticle.anchoredPosition = aimPos;
    }
}
