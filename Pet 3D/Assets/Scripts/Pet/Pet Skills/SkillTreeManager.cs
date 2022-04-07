using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    [SerializeField] UISkill uiSkillPrefab;
    [SerializeField] Transform uiSkillListParent;
    Dictionary<string, UISkill> uiSkillDictionary;

    private void Awake()
    {
        uiSkillPrefab = Resources.Load<UISkill>("Prefabs/UI/UISkill");
        uiSkillDictionary = new Dictionary<string, UISkill>();
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var pair in Persistent.petSkills.skillDictionary)
        {
            UISkill skill = Instantiate(uiSkillPrefab, uiSkillListParent);
            skill.Initialize(pair.Key, pair.Value);

            uiSkillDictionary.Add(pair.Key, skill);
        }

        Persistent.petSkills.onSkillUnlocked += SkillUnlocked;
    }

    void SkillUnlocked(string skillName)
    {
        uiSkillDictionary[skillName].Initialize(skillName, true);
    }
}
