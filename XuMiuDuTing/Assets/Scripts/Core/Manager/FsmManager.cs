using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yu
{
    public class FsmManager : BaseSingleTon<FsmManager>, IMonoManager
    {
        private readonly Dictionary<string, BaseFsm> _fsmDict = new Dictionary<string, BaseFsm>();

        /// <summary>
        /// 获取状态机
        /// </summary>
        /// <typeparam name="T">状态机类型</typeparam>
        /// <returns></returns>
        public T GetFsm<T>() where T : BaseFsm, new()
        {
            var fsmName = typeof(T).ToString();
            if (_fsmDict.ContainsKey(fsmName))
            {
                return _fsmDict[fsmName] as T;
            }

            var newFsm = new T();
            _fsmDict.Add(fsmName, newFsm);
            return newFsm;
        }

        public void OnInit()
        {
        }

        public void Update()
        {
            foreach (var fsm in _fsmDict.Values)
            {
                fsm.OnUpdate();
            }
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
}