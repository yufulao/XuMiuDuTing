using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Yu
{
    public class DoubleConfirmModel
    {
        private UnityAction _funcConfirm;

        /// <summary>
        /// 设置二次确认转发的事件
        /// </summary>
        /// <param name="func"></param>
        public void SetFuncConfirm(UnityAction func)
        {
            _funcConfirm = func;
        }

        /// <summary>
        /// 获取二次确认转发的事件
        /// </summary>
        /// <returns></returns>
        public UnityAction GetFuncConfirm()
        {
            return _funcConfirm;
        }
    }
}
