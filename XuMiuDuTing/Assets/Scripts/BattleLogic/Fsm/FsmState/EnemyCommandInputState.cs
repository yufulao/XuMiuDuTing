using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yu;

public class EnemyCommandInputState : IFsmState
{
    public void OnEnter(params object[] objs)
    {
        BattleManager.Instance.OnEnterEnemyCommandInputState();
    }

    public void OnUpdate(params object[] objs)
    {
        
    }

    public void OnExit()
    {
        
    }
}
