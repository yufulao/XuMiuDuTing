using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yu;

public class BattleInitState : IFsmState
{
    public void OnEnter(params object[] objs)
    {
        BattleManager.Instance.OnEnterBattleInitState();
    }

    public void OnUpdate(params object[] objs)
    {
        
    }

    public void OnExit()
    {
        
    }
}
