using System.Collections.Generic;
using Rabi;

namespace Yu
{
    public class SkillInfo
    {
        public string skillName;
        public BattleEntityCtrl caster;
        public List<BattleEntityCtrl> targetList; //选择目标后进行组装

        public RowCfgSkill RowCfgSkill => ConfigManager.Instance.cfgSkill[skillName];

        public SkillInfo()
        {
        }
        
        public SkillInfo(string skillName, BattleEntityCtrl caster, List<BattleEntityCtrl> targetList)
        {
            this.skillName = skillName;
            this.caster = caster;
            this.targetList = targetList;
        }
    }
}