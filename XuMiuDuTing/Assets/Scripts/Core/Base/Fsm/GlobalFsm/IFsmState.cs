using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFsmState 
{
    /// <summary>
    /// 状态开始
    /// </summary>
    void OnEnter();

    /// <summary>
    /// 状态update
    /// </summary>
    void OnUpdate();

    /// <summary>
    /// 状态退出
    /// </summary>
    void OnExit();
}
