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
        /// <param name="fsmNameT">状态机名</param>
        /// <typeparam name="T">状态机类型</typeparam>
        /// <returns></returns>
        public T GetFsmByName<T>(string fsmNameT) where T : BaseFsm, new()
        {
            if (_fsmDict.ContainsKey(fsmNameT))
            {
                return _fsmDict[fsmNameT] as T;
            }

            T newFsm = new T {fsmName = fsmNameT};
            _fsmDict.Add(fsmNameT, newFsm);
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