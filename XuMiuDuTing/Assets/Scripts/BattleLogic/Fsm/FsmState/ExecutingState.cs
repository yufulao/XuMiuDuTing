using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yu;

public class ExecutingState : IFsmState
{
    public void OnEnter(params object[] objs)
    {
        BattleManager.Instance.OnEnterExecutingState();
    }

    public void OnUpdate(params object[] objs)
    {
        
    }

    public void OnExit()
    {
        
    }
}
