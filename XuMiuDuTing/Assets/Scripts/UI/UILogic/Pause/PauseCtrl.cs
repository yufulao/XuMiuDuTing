using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace Yu
{
    public class PauseCtrl : UICtrlBase
    {
        private PauseModel _model;
        private PauseView _view;

        public override void OnInit(params object[] param)
        {
            _model = new PauseModel();
            _view = GetComponent<PauseView>();
            _model.OnInit();
        }

        public override void OpenRoot(params object[] param)
        {
            GameManager.Instance.StartCoroutine(OpenRootIEnumerator());
        }

        public override void CloseRoot()
        {
            GameManager.Instance.StartCoroutine(CloseRootIEnumerator());
        }

        public override void BindEvent()
        {
            _view.btnResume.onClick.AddListener(BtnOnClickResume);
            _view.btnSetting.onClick.AddListener(BtnOnClickSetting);
            _view.btnReturnTitle.onClick.AddListener(BtnOnClickReturnTitle);
        }

        /// <summary>
        /// 获取当前是否是暂停状态
        /// </summary>
        /// <returns></returns>
        public bool GetIsOnPause()
        {
            return _model.GetOnPause();
        }

        /// <summary>
        /// 继续游戏
        /// </summary>
        private void BtnOnClickResume()
        {
            CloseRoot();
        }
        
        /// <summary>
        /// 打开设置界面
        /// </summary>
        private static void BtnOnClickSetting()
        {
            UIManager.Instance.OpenWindow("SettingView");
        }
        
        /// <summary>
        /// 返回标题
        /// </summary>
        private void BtnOnClickReturnTitle()
        {
            GameManager.Instance.ReturnToTitle(true);
        }
        
        /// <summary>
        /// 打开暂停窗口的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator OpenRootIEnumerator()
        {
            yield return _view.OpenWindowIEnumerator();
            //_model.SetTimeScale(Time.timeScale);
            //GameManager.Instance.SetTimeScale(0f);
            _model.SetOnPause(true);
        }
        
        /// <summary>
        /// 关闭暂停窗口的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator CloseRootIEnumerator()
        {
            yield return _view.CloseWindowIEnumerator();
            //GameManager.Instance.SetTimeScale(_model.GetTimeScale());
            _model.SetOnPause(false);
        }
        
    }
}