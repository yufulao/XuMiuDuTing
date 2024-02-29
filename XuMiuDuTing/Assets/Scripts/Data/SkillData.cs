using System;
using System.Collections.Generic;

namespace Yu
{
    [Serializable]
    public class SkillData
    {
        public Dictionary<string, SkillDataEntry> allSkill = new Dictionary<string, SkillDataEntry>();

        public SkillData()
        {
        }
    }

    [Serializable]
    public class SkillDataEntry
    {
        public bool isUnlock;

        public SkillDataEntry()
        {
        }
    }
}