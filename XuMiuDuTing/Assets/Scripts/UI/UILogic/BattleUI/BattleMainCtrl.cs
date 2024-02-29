using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rabi;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Yu
{
    public class BattleMainCtrl : UICtrlBase
    {
        private BattleMainModel _model;
        private BattleMainView _view;


        public override void OnInit(params object[] param)
        {
            _model = new BattleMainModel();
            _view = GetComponent<BattleMainView>();
            _model.OnInit();
            _view.OnInit();
        }

        public override void OpenRoot(params object[] param)
        {
            _view.OpenWindow();
        }

        public override void CloseRoot()
        {
            _view.CloseWindow();
        }

        public override void BindEvent()
        {
        }

        
    }
}