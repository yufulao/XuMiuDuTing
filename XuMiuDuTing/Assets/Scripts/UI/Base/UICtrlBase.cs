using UnityEngine;

namespace Yu
{
    public abstract class UICtrlBase : MonoBehaviour
    {
        //先OnInit，再BindEvent，再OpenRoot
        
        /// <summary>
        /// 第一次打开时的初始化
        /// </summary>
        /// <param name="param"></param>
        public abstract void OnInit(params object[] param);

        /// <summary>
        /// 每次打开都执行一次，包括第一次
        /// </summary>
        /// <param name="param"></param>
        public abstract void OpenRoot(params object[] param);

        /// <summary>
        /// 关闭时调用一次
        /// </summary>
        public abstract void CloseRoot();

        /// <summary>
        /// 绑定事件，自动在OnInit调用了
        /// </summary>
        public abstract void BindEvent();
    }
}