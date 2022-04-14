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

        reticleArea.gameObject.SetActive(false);
        reticle.gameObject.SetActive(false);
    }

    public void Aim(Vector3 aimDirection)
    {
        reticleArea.gameObject.SetActive(true);
        reticle.gameObject.SetActive(true);

        if (aimDirection.magnitude == 0f)
            ResetReticle();
        else
        {
            aimPos.x =  aimDirection.x / Screen.width;
            aimPos.y =  aimDirection.y / Screen.height;

            aimPos.x = aimPos.x * reticleArea.rect.width - reticle.rect.width * 2;
            aimPos.y = aimPos.y * reticleArea.rect.height - reticle.rect.height * 2;

            aimPos.x = Mathf.Clamp(aimPos.x, -reticle.rect.width * 2, reticleArea.rect.width - reticle.rect.width * 2);
            aimPos.y = Mathf.Clamp(aimPos.y, -reticle.rect.height * 2, reticleArea.rect.width - reticle.rect.height * 2);

            reticle.anchoredPosition = aimPos;
        }
    }

    public void ResetReticle()
    {
        aimPos = Vector2.zero;
        reticle.anchoredPosition = aimPos;

        reticleArea.gameObject.SetActive(false);
        reticle.gameObject.SetActive(false);
    }
}
