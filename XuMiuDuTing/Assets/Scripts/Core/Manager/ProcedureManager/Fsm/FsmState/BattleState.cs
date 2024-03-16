using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yu;

public class BattleState : IFsmState
{
    public void OnEnter(params object[] objs)
    {
        ProcedureManager.Instance.OnEnterBattleState();
    }

    public void OnUpdate(params object[] objs)
    {
        GameManager.Instance.OnUpdateCheckPause();
    }

    public void OnExit()
    {
        
    }
}
