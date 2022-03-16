using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleScoreScreen : MonoBehaviour
{
    public RaceNamePlate[] namePlates;

    public TMP_Text leaveText;

    private void Update()
    {
        leaveText.alpha = (Mathf.Sin(Time.realtimeSinceStartup * 5.0f) + 1.0f) / 2.0f;
    }
}
