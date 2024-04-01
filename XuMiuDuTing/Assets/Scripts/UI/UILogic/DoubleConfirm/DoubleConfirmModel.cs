using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Yu
{
    public class DoubleConfirmModel
    {
        private UnityAction _funcConfirm;
        private UnityAction _funcCancel;

        /// <summary>
        /// 设置二次确认转发的事件
        /// </summary>
        /// <param name="funcConfirm"></param>
        /// <param name="funcCancel"></param>
        public void SetFuncConfirm(UnityAction funcConfirm,UnityAction funcCancel)
        {
            _funcConfirm = funcConfirm;
            _funcCancel = funcCancel;
        }

        /// <summary>
        /// 获取确定转发的事件
        /// </summary>
        /// <returns></returns>
        public UnityAction GetFuncConfirm()
        {
            return _funcConfirm;
        }
        
        /// <summary>
        /// 获取取消转发的事件
        /// </summary>
        /// <returns></returns>
        public UnityAction GetFuncCancel()
        {
            return _funcCancel;
        }
    }
}
