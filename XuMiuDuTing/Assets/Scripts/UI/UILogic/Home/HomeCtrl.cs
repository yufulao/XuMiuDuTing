using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yu
{
    public class HomeCtrl : UICtrlBase
    {
        private HomeModel _model;
        private HomeView _view;
        
        public override void OnInit(params object[] param)
        {
            _model = new HomeModel();
            _view = GetComponent<HomeView>();
        }

        public override void OpenRoot(params object[] param)
        {
            gameObject.SetActive(true);
        }

        public override void CloseRoot()
        {
            gameObject.SetActive(false);
        }

        public override void BindEvent()
        {
            _view.btnStart.onClick.AddListener(BtnOnClickStart);
            _view.btnCharacter.onClick.AddListener(BtnOnClickCharacter);
            _view.btnSetting.onClick.AddListener(BtnOnClickSetting);
            _view.btnQuit.onClick.AddListener(BtnOnClickQuit);
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        private static void BtnOnClickStart()
        {
            UIManager.Instance.OpenWindow("ChapterSelectView");
        }
        
        /// <summary>
        /// 角色图鉴
        /// </summary>
        private static void BtnOnClickCharacter()
        {
            
        }
        
        /// <summary>
        /// 设置
        /// </summary>
        private static void BtnOnClickSetting()
        {
            UIManager.Instance.OpenWindow("SettingView");
        }
        
        /// <summary>
        /// 退出
        /// </summary>
        private static void BtnOnClickQuit()
        {
#if UNITY_STANDALONE
            Application.Quit();
#endif

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}