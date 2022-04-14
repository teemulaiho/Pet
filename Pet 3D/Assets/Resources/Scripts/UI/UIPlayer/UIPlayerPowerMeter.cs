using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerPowerMeter : MonoBehaviour
{
    Player player;
    Slider powerMeter;
    Image backgroundImage;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        powerMeter = GetComponentInChildren<Slider>();
        backgroundImage = GetComponentInChildren<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player.onMouseLeftButtonHold += PowerMeter;
        powerMeter.maxValue = player.MaxThrowPower;
        powerMeter.value = 0f;

        powerMeter.gameObject.SetActive(false);
        backgroundImage.gameObject.SetActive(false);
    }

    public void PowerMeter(float currentValue)
    {
        if (currentValue == 0)
            ReleasePowerMeter();
        else
            ChargePowerMeter(currentValue);
    }

    public void ChargePowerMeter(float currentValue)
    {
        powerMeter.gameObject.SetActive(true);
        backgroundImage.gameObject.SetActive(true);

        powerMeter.value = currentValue;
    }

    public void ReleasePowerMeter()
    {
        powerMeter.value = 0f;

        powerMeter.gameObject.SetActive(false);
        backgroundImage.gameObject.SetActive(false);
    }
}
