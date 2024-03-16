using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yu;

public class CharacterCommandInputState : IFsmState
{
    public void OnEnter(params object[] objs)
    {
        BattleManager.Instance.OnEnterCharacterCommandInputState();
    }

    public void OnUpdate(params object[] objs)
    {
        
    }

    public void OnExit()
    {
        
    }
}
