using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Yu
{
    public class DoubleConfirmCtrl : UICtrlBase
    {
        private DoubleConfirmModel _model;
        private DoubleConfirmView _view;

        public override void OnInit(params object[] param)
        {
            _model = new DoubleConfirmModel();
            _view = GetComponent<DoubleConfirmView>();
        }

        /// <summary>
        /// param[0]提示字段，param[1]确认的转发事件
        /// </summary>
        /// <param name="param"></param>
        public override void OpenRoot(params object[] param)
        {
            _view.OpenWindow();
            if (param.Length<3)
            {
                Debug.LogError("二次确认没有传入参数");
                return;
            }

            _view.RefreshTipText(param[0].ToString());
            _model.SetFuncConfirm((UnityAction)param[1], (UnityAction)param[2]);
        }

        public override void CloseRoot()
        {
            _view.CloseWindow();
        }

        public override void BindEvent()
        {
            _view.btnBack.onClick.AddListener(BtnOnClickBack);
            _view.btnConfirm.onClick.AddListener(BtnOnClickConfirm);
        }

        /// <summary>
        /// 点击BtnBack时
        /// </summary>
        private void BtnOnClickBack()
        {
            CloseRoot();
            _model.GetFuncCancel()?.Invoke();
        }

        /// <summary>
        /// 点击BtnConfirm时
        /// </summary>
        private void BtnOnClickConfirm()
        {
            CloseRoot();
            _model.GetFuncConfirm()?.Invoke();
        }
    }
}