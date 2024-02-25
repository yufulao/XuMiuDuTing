using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFsmComponentState<in Owner>
{
    /// <summary>
    /// 状态开始
    /// </summary>
    void OnEnter(Owner owner);

    /// <summary>
    /// 状态update
    /// </summary>
    void OnUpdate(Owner owner);

    /// <summary>
    /// 状态退出
    /// </summary>
    void OnExit(Owner owner);
}
