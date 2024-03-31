using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yu;

public class EnemyEntityCtrlQuChi : EnemyEntityCtrl
{
    public override void SetCommandAI(int currentRound)
    {
        var target = BattleManager.Instance.EnemySelectTarget(1)[0];
        var cantSelectBuff = BattleManager.CheckBuff(this, "不可选中");
        if (cantSelectBuff.Count>0)
        {
            if (cantSelectBuff[0].roundDuring==1)
            {
                commandList.Add(BattleManager.Instance.QuChiAttackAfterRemoveBuffCantSelect(this,target));
                BattleManager.Instance.AddCommandInfo(new BattleCommandInfo(
                    true, BattleCommandType.Skill, false, 0, new List<BattleEntityCtrl> {target}, this));
            }

            return;
        }
        
        //技能1和2还有普通攻击，三个随机
        var randomIndex = Random.Range(0, 3);//int不包含上限,float包含上限
        switch (randomIndex)
        {
            case 0 ://普通攻击
                commandList.Add(BattleManager.Instance.EnemyAttack(this, new List<BattleEntityCtrl> {target}));
                BattleManager.Instance.AddCommandInfo(new BattleCommandInfo(
                    true, BattleCommandType.Attack, false, 0, new List<BattleEntityCtrl> {target}, this));
                break;
            case 1://技能1
                commandList.Add(BattleManager.Instance.QuChiSkill1(this));
                BattleManager.Instance.AddCommandInfo(new BattleCommandInfo(
                    true, BattleCommandType.Skill, false, 0, new List<BattleEntityCtrl> {target}, this));
                break;
            case  2://技能2
                commandList.Add(BattleManager.Instance.QuChiSkill2(this));
                BattleManager.Instance.AddCommandInfo(new BattleCommandInfo(
                    true, BattleCommandType.Skill, false, 0, new List<BattleEntityCtrl> {target}, this));
                break;
        }
    }
}