using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetSkills
{
    public Dictionary<string, bool> skillDictionary;
    public bool throwSkill;
    public bool catchSkill;
    public bool whistleSkill;

    public delegate void OnSkillUnlocked(string skillUnlocked);
    public OnSkillUnlocked onSkillUnlocked;

    public void Init()
    {
        skillDictionary = new Dictionary<string, bool>
        {
            { "Throw", throwSkill },
            { "Catch", catchSkill },
            { "Whistle", whistleSkill }
        };
    }

    public void UnlockSkill(string skillUnlocked)
    {
        skillDictionary[skillUnlocked] = true;

        if (onSkillUnlocked != null)
            onSkillUnlocked(skillUnlocked);
    }
}