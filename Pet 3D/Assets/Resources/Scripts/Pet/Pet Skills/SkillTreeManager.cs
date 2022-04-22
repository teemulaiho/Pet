using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeManager : MonoBehaviour
{
    UIController uiController;
    [SerializeField] UISkill uiSkillPrefab;
    [SerializeField] Transform uiSkillListParent;
    Dictionary<string, UISkill> uiSkillDictionary;

    private void Awake()
    {
        uiController = FindObjectOfType<UIController>();
        uiSkillPrefab = Resources.Load<UISkill>("Prefabs/UI/UISkill");
        uiSkillDictionary = new Dictionary<string, UISkill>();
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var pair in Persistent.petSkills._skillDictionary)
        {
            UISkill skill = Instantiate(uiSkillPrefab, uiSkillListParent);
            skill.Initialize(pair.Key, pair.Value._unlocked, pair.Value._skillUnlockRequirement);

            uiSkillDictionary.Add(pair.Key, skill);
        }

        Persistent.petSkills.onSkillUnlocked += SkillUnlocked;
        GetComponentInChildren<UIButton>().close += Close;
    }

    void SkillUnlocked(string skillName)
    {
        uiSkillDictionary[skillName].Initialize(skillName, true);
    }

    void Close()
    {
        uiController.CloseUIWindw(this.gameObject, false);
    }
}
