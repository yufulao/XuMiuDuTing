using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yu
{
    public class BattleManager : MonoSingleton<BattleManager>
    {
        public List<Transform> characterSpawnPointList = new List<Transform>();
        public List<Transform> enemySpawnPointList = new List<Transform>();
        
        private BattleMainCtrl _uiCtrl;
        private BattleFsm _fsm;
        private BattleModel _model;

        protected override void Awake()
        {
            base.Awake();
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="teamArray"></param>
        /// <param name="stageName"></param>
        public void Init(string[] teamArray,string stageName)
        {
            _model=new BattleModel();
            _model.Init(teamArray,stageName);
            InitFsm();
            InitUI();
        }

        /// <summary>
        /// 初始化UI
        /// </summary>
        private void InitUI()
        {
            UIManager.Instance.OpenWindow("BattleMainView");
            _uiCtrl = UIManager.Instance.GetCtrl<BattleMainCtrl>("BattleMainView");
        }

        /// <summary>
        /// 初始化状态机
        /// </summary>
        private void InitFsm()
        {
            _fsm = FsmManager.Instance.GetFsm<BattleFsm>();
            _fsm.SetFsm(new Dictionary<Type, IFsmState>()
            {
                {typeof(CharacterCommandInputState),new CharacterCommandInputState()},
                {typeof(EnemyCommandInputState),new EnemyCommandInputState()},
                {typeof(ExecutingState),new ExecutingState()}
            });
        }
        
    }
}
