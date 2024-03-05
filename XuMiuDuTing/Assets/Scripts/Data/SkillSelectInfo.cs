using System.Collections.Generic;

namespace Yu
{
    public class SkillSelectInfo
    {
        public bool needSelect;
        public bool isBattleStartCommand;
        public BattleEntityCtrl battleEntity;
        public int skillIndex;
        public List<BattleEntityCtrl> selectedEntityList;


        //输入完Skill进EntityCommandList后再赋的值
        public int bpNeed;

        public SkillSelectInfo(bool needSelectT, bool isBattleStartCommandT, BattleEntityCtrl battleEntityT, int skillIndexT, List<BattleEntityCtrl> selectedEntityListT)
        {
            needSelect = needSelectT;
            isBattleStartCommand = isBattleStartCommandT;
            battleEntity = battleEntityT;
            skillIndex = skillIndexT;
            selectedEntityList = selectedEntityListT;
        }
    }
}
