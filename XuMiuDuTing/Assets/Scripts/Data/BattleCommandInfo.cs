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
        public List<BattleEntityCtrl> targets;
        public BattleEntityCtrl caster;

        public BattleCommandInfo(bool isEnemyT, BattleCommandType commandTypeT, bool isBattleStartCommandT, int bpNeedT, List<BattleEntityCtrl> targetsT, BattleEntityCtrl casterT)
        {
            isEnemy = isEnemyT;
            commandType = commandTypeT;
            isBattleStartCommand = isBattleStartCommandT;
            bpNeed = bpNeedT;
            targets = targetsT;
            caster = casterT;
        }
    }
}
