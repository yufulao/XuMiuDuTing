using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yu
{
    public class BattleCommandInfo
    {
        public bool isEnemy;
        public BattleCommandType commandType;
        public bool isBattleStartCommand;
        public int bpNeed;
        public List<BattleEntityCtrl> selectEntityList;
        public int battleId;

        public BattleCommandInfo(bool isEnemyT, BattleCommandType commandTypeT, bool isBattleStartCommandT, int bpNeedT, List<BattleEntityCtrl> selectEntityListT, int entityIdT)
        {
            isEnemy = isEnemyT;
            commandType = commandTypeT;
            isBattleStartCommand = isBattleStartCommandT;
            bpNeed = bpNeedT;
            selectEntityList = selectEntityListT;
            battleId = entityIdT;
        }
    }
}
