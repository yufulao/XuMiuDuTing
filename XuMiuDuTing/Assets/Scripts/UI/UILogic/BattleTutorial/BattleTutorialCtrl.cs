using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Yu
{
    public class BattleTutorialCtrl : UICtrlBase
    {
        private BattleTutorialModel _model;
        private BattleTutorialView _view;

        public override void OnInit(params object[] param)
        {
            _model = new BattleTutorialModel();
            _view = GetComponent<BattleTutorialView>();
        }

        /// <summary>
        /// param[0]提示字段，param[1]确认的转发事件
        /// </summary>
        /// <param name="param"></param>
        public override void OpenRoot(params object[] param)
        {
            _model.OnOpen(_view.allPage.Count);
            _view.ResetPage(_model.GetCurrentPageIndex());
            _view.OpenWindow();
        }

        public override void CloseRoot()
        {
            _view.CloseWindow();
        }

        public override void BindEvent()
        {
            _view.btnConfirm.onClick.AddListener(BtnOnClickConfirm);
            _view.btnNext.onClick.AddListener(NextPage);
            _view.btnPre.onClick.AddListener(PrePage);
        }

        /// <summary>
        /// 点击BtnConfirm时
        /// </summary>
        private void BtnOnClickConfirm()
        {
            CloseRoot();
        }
        
        /// <summary>
        /// 下一页
        /// </summary>
        private void NextPage()
        {
            var movePage = _view.allPage[_model.GetCurrentPageIndex()];
            _model.NextPage();
            _view.MovePageToTarget(movePage);
            if (_model.IsLastPage())
            {
                _view.SetBtnConfirmActive(true);
                _view.SetBtnNextActive(false);
                return;
            }
            _view.SetBtnPreActive(true);
        }

        /// <summary>
        /// 上一页
        /// </summary>
        private void PrePage()
        {
            _model.PrePage();
            var movePage = _view.allPage[_model.GetCurrentPageIndex()];
            _view.MovePageToOriginal(movePage);
            if (_model.IsFirstPage())
            {
                _view.SetBtnPreActive(false);
                return;
            }
            _view.SetBtnNextActive(true);
            _view.SetBtnConfirmActive(false);
        }
    }
}