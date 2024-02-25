using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 事件管理器
/// </summary>
public class EventManager : BaseSingleTon<EventManager>, IMonoManager
{
    private readonly Dictionary<EventName, Delegate> _eventDic = new Dictionary<EventName, Delegate>();

    public delegate void TypeEvent();

    public delegate void TypeEvent<in T1>(T1 t1);//泛型类型参数默认是不变的，用in来声明类型参数的逆变性，委托类型是逆变的

    /// <summary>
    /// 添加一个事件的监听者
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="action">事件处理函数</param>
    public void AddListener(EventName eventName, TypeEvent action)
    {
        if (!_eventDic.ContainsKey(eventName))
        {
            _eventDic.Add(eventName, null);
        }

        var curDelegate = _eventDic[eventName];
        if (curDelegate != null && curDelegate.GetType() != action.GetType())
        {
            Debug.LogError("事件参数类型错误 eventId:" + eventName + "/delegate类型是" + curDelegate.GetType().Name + "/添加的action类型是" + action.GetType().Name);
            return;
        }

        _eventDic[eventName] = (TypeEvent) _eventDic[eventName] + action;
    }

    /// <summary>
    /// 添加一个事件的监听者
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="action">事件处理函数</param>
    public void AddListener<T1>(EventName eventName, TypeEvent<T1> action)
    {
        if (!_eventDic.ContainsKey(eventName))
        {
            _eventDic.Add(eventName, null);
        }

        var curDelegate = _eventDic[eventName];
        if (curDelegate != null && curDelegate.GetType() != action.GetType())
        {
            Debug.LogError("事件参数类型错误 eventId:" + eventName + "/delegate类型是" + curDelegate.GetType().Name + "/添加的action类型是" + action.GetType().Name);
            return;
        }

        _eventDic[eventName] = (TypeEvent<T1>) _eventDic[eventName] + action;
    }


    /// <summary>
    /// 移除一个事件的监听者
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="action">事件处理函数</param>
    public void RemoveListener(EventName eventName, TypeEvent action)
    {
        if (!_eventDic.ContainsKey(eventName))
        {
            return;
        }

        var curDelegate = _eventDic[eventName];
        if (curDelegate == null)
        {
            return;
        }

        if (curDelegate.GetType() != action.GetType())
        {
            Debug.LogError("事件参数类型错误 eventId:" + eventName + "/delegate类型是" + curDelegate.GetType().Name + "/添加的action类型是" + action.GetType().Name);
            return;
        }

        _eventDic[eventName] = (TypeEvent) _eventDic[eventName] - action;
        if (_eventDic[eventName] == null)
        {
            _eventDic.Remove(eventName);
        }
    }

    /// <summary>
    /// 移除一个事件的监听者
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="action">事件处理函数</param>
    public void RemoveListener<T1>(EventName eventName, TypeEvent<T1> action)
    {
        if (!_eventDic.ContainsKey(eventName))
        {
            return;
        }

        var curDelegate = _eventDic[eventName];
        if (curDelegate == null)
        {
            return;
        }

        if (curDelegate.GetType() != action.GetType())
        {
            Debug.LogError("事件参数类型错误 eventId:" + eventName + "/delegate类型是" + curDelegate.GetType().Name + "/添加的action类型是" + action.GetType().Name);
            return;
        }

        _eventDic[eventName] = (TypeEvent<T1>) _eventDic[eventName] - action;
        if (_eventDic[eventName] == null)
        {
            _eventDic.Remove(eventName);
        }
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    public void Dispatch(EventName eventName)
    {
        if (!_eventDic.ContainsKey(eventName) || _eventDic[eventName] == null)
        {
            return;
        }
        ((TypeEvent)_eventDic[eventName]).Invoke();
    }
    
    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="param1"></param>
    /// <typeparam name="T1"></typeparam>
    public void Dispatch<T1>(EventName eventName,T1 param1)
    {
        if (!_eventDic.ContainsKey(eventName) || _eventDic[eventName] == null)
        {
            return;
        }
        ((TypeEvent<T1>)_eventDic[eventName]).Invoke(param1);
    }

    /// <summary>
    /// 删除指定事件名
    /// </summary>
    /// <param name="eventName"></param>
    public void RemoveEvent(EventName eventName)
    {
        if (_eventDic.ContainsKey(eventName))
        {
            _eventDic.Remove(eventName);
        }
    }
    
    /// <summary>
    /// 清空所有事件
    /// </summary>
    public void Clear()
    {
        _eventDic.Clear();
    }

    public void OnInit()
    {
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }

    public void LateUpdate()
    {
    }

    public void OnClear()
    {
    }
}