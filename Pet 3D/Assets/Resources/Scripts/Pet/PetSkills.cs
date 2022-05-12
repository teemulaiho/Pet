using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillRequirement // Should be same as PetStats -Teemu
{
    Name,
    Level,
    Health,
    Energy,
    Stamina,
    Intellect,
    Strength,
    Experience
}

public class PetSkill
{
    public string _skillName;
    public bool _unlocked;
    public SkillRequirement _skillRequirementType;
    public int _skillUnlockRequirement;
    public string _description;
}

public class PetSkills
{
    public Dictionary<string, PetSkill> _skillDictionary;
    //public bool throwSkill;
    //public bool catchSkill;
    //public bool whistleSkill;

    public delegate void OnSkillUnlocked(string skillUnlocked);
    public OnSkillUnlocked onSkillUnlocked;

    List<PetSkill> _skillUnlockList = new List<PetSkill>();

    public void Init()
    {
        var catchSkill = new PetSkill
        {
            _skillName = "Catch",
            _unlocked = false,
            _skillRequirementType = SkillRequirement.Intellect,
            _skillUnlockRequirement = 2,
            _description = "Pet can now catch the ball!"
        };

        // throwSkill is a Dummy Skill to Pad the SkillTree -Teemu
        var throwSkill = new PetSkill
        {
            _skillName = "Throw",
            _unlocked = false,
            _skillRequirementType = SkillRequirement.Intellect,
            _skillUnlockRequirement = 4,
            _description = "You can now charge up throwing speed!"
        };

        // whistleSkill is a Dummy Skill to Pad the SkillTree -Teemu
        var whistleSkill = new PetSkill
        {
            _skillName = "Whistle",
            _unlocked = false,
            _skillRequirementType = SkillRequirement.Intellect,
            _skillUnlockRequirement = 6,
            _description = "Pet can now be called with a Whistle!"
        };

        _skillDictionary = new Dictionary<string, PetSkill>
        {
            {"Catch", catchSkill},
            {"Throw", throwSkill },
            {"Whistle", whistleSkill },
        };
    }

    public PetSkill GetLatestSkillUnlocked()
    {
        if (_skillUnlockList.Count > 0)
            return _skillUnlockList[_skillUnlockList.Count - 1];

        return null;
    }

    public void UnlockSkill(string skillUnlocked)
    {
        _skillDictionary[skillUnlocked]._unlocked = true;
        _skillUnlockList.Add(_skillDictionary[skillUnlocked]);

        if (onSkillUnlocked != null)
            onSkillUnlocked(skillUnlocked);
    }

    public bool CheckForSkillUnlock(SkillRequirement requirementType, int currentLevel)
    {
        foreach (var pair in _skillDictionary)
        {
            if (pair.Value._skillRequirementType == requirementType)
            {
                if (currentLevel >= pair.Value._skillUnlockRequirement && !pair.Value._unlocked)
                {
                    UnlockSkill(pair.Value._skillName);
                    return true;
                }
            }
        }

        return false;
    }
}