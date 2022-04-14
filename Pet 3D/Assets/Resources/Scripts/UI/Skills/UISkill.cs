using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISkill : MonoBehaviour
{
    bool isUnlocked;
    [SerializeField] TMP_Text skillName;
    [SerializeField] Button skillButton;

    public void Initialize(string name, bool unlocked)
    {
        skillButton = GetComponentInChildren<Button>();
        isUnlocked = unlocked;
        skillButton.interactable = isUnlocked;

        if (isUnlocked)
            skillName.text = name;
        else
            skillName.text = "Locked";
    }
}
