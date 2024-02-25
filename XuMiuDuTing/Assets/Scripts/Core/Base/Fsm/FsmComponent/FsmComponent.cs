using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FsmComponent<Owner>
{
    private readonly Owner _owner;
    public string fsmName;
    private Dictionary<Type,  IFsmComponentState<Owner>> _fsmStateDic = new Dictionary<Type, IFsmComponentState<Owner>>();
    private IFsmComponentState<Owner> _currentFsmState;

    /// <summary>
    /// 初始化这个状态机
    /// </summary>
    /// <param name="states">状态机将持有的状态</param>
    public void SetFsm(Dictionary<Type, IFsmComponentState<Owner>> states)
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
        
        _currentFsmState?.OnExit(_owner);
        _currentFsmState = _fsmStateDic[stateName];
        _currentFsmState.OnEnter(_owner);
    }
    
    /// <summary>
    /// Update每个状态机的OnUpdate，需要实现
    /// </summary>
    public void OnUpdate()
    {
        _currentFsmState?.OnUpdate(_owner);
    }

    /// <summary>
    /// 创建fsm组件
    /// </summary>
    /// <param name="owner"></param>
    public FsmComponent(Owner owner)
    {
        _owner = owner;
    }
}
