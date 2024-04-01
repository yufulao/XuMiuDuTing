using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;
using Yu;

public class BattleInitState : IFsmState
{
    public void OnEnter(params object[] objs)
    {
        if (objs.Length <= 0)
        {
            Debug.LogError("没有关卡数据");
            return;
        }

        BattleManager.Instance.StartCoroutine(BattleManager.Instance.OnEnterBattleInitState((RowCfgStage) objs[0]));
    }

    public void OnUpdate(params object[] objs)
    {
    }

    public void OnExit()
    {
    }
}