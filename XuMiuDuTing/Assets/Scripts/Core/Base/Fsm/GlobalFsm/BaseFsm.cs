using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseFsm
{
    public string fsmName;
    private Dictionary<Type, IFsmState> _fsmStateDic = new Dictionary<Type, IFsmState>();
    private IFsmState _currentFsmState;

    /// <summary>
    /// 初始化这个状态机
    /// </summary>
    /// <param name="states">状态机将持有的状态</param>
    public void SetFsm(Dictionary<Type, IFsmState> states)
    {
        _fsmStateDic = states;
    }
    
    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="stateName">状态机名Enum</param>
    public void ChangeFsmState(Type stateName)
    {
        if (!_fsmStateDic.ContainsKey(stateName))
        {
            Debug.Log("fsm里没有这个状态");
            return;
        }
        
        _currentFsmState?.OnExit();
        _currentFsmState = _fsmStateDic[stateName];
        _currentFsmState.OnEnter();
    }
    
    /// <summary>
    /// Update每个状态机的OnUpdate
    /// </summary>
    public void OnUpdate()
    {
        _currentFsmState?.OnUpdate();
    }
}
