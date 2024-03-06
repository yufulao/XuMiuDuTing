using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yu;

public class BattleState : IFsmState
{
    public void OnEnter()
    {
        ProcedureManager.Instance.OnEnterBattleState();
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        
    }
}
