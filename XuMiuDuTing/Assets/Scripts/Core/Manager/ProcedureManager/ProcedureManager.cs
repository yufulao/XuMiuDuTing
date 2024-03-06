using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PixelCrushers.DialogueSystem;
using Rabi;
using UnityEngine;

namespace Yu
{
    public class ProcedureManager : BaseSingleTon<ProcedureManager>, IMonoManager
    {
        private ProcedureFsm _fsm;
        private readonly List<Type> _currentStageProcedure = new List<Type>();
        private RowCfgStage _cacheRowCfgStage;

        public void OnInit()
        {
            InitFsm();
        }

        public void Update()
        {
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

        /// <summary>
        /// 设置关卡流程
        /// </summary>
        public void SetStageProcedure(string stageName)
        {
            _cacheRowCfgStage = ConfigManager.Instance.cfgStage[stageName];
            _currentStageProcedure.Clear();
            if (_cacheRowCfgStage.isBattle)
            {
                _currentStageProcedure.Add(typeof(TeamEditState));
            }

            if (!string.IsNullOrEmpty(_cacheRowCfgStage.conversationAName) && _cacheRowCfgStage.conversationAName != "None")
            {
                _currentStageProcedure.Add(typeof(ConversationAState));
            }

            if (_cacheRowCfgStage.isBattle)
            {
                _currentStageProcedure.Add(typeof(BattleState));
            }

            if (!string.IsNullOrEmpty(_cacheRowCfgStage.conversationBName) && _cacheRowCfgStage.conversationBName != "None")
            {
                _currentStageProcedure.Add(typeof(ConversationBState));
            }

            EnterNextStageProcedure();
        }

        /// <summary>
        /// 执行下一个关卡步骤
        /// </summary>
        public void EnterNextStageProcedure()
        {
            if (_currentStageProcedure.Count <= 0)
            {
                EndStageProcedure();
                return;
            }

            var currentProcedure = _currentStageProcedure[0];
            _fsm.ChangeFsmState(currentProcedure);
            _currentStageProcedure.RemoveAt(0);
        }

        /// <summary>
        /// TeamEditState的OnEnter
        /// </summary>
        public void OnEnterTeamEditState()
        {
            var chapterType = SaveManager.GetString("ChapterType", "MainPlot");
            var plotName = chapterType.Equals(DefChapterType.DMainPlot)
                ? SaveManager.GetString("PlotNameInMainPlot", "plotZero")
                : SaveManager.GetString("PlotNameInSubPlot", "plotZero");
            var fixCharacterTeamList = _cacheRowCfgStage.fixCharacterTeam;
            if (fixCharacterTeamList == null)
            {
                //param[0]是ChapterType，param[1]PlotName），param[2]是string[4] 固定编队
                UIManager.Instance.OpenWindow("TeamEditView", chapterType, plotName);
                return;
            }

            var fixCharacterTeamArray = new string[4];
            for (var i = 0; i < 4; i++)
            {
                fixCharacterTeamArray[i] = fixCharacterTeamList[i];
            }

            UIManager.Instance.OpenWindow("TeamEditView", chapterType, plotName, fixCharacterTeamArray);
        }

        /// <summary>
        /// ConversationAState的OnEnter
        /// </summary>
        public void OnEnterConversationAState()
        {
            GameManager.Instance.StartCoroutine(OnEnterConversationAStateIEnumerator());
        }

        private IEnumerator OnEnterConversationAStateIEnumerator()
        {
            UIManager.Instance.OpenWindow("LoadingView");
            yield return new WaitForSeconds(0.3f);
            BGMManager.Instance.StopBgm();
            UIManager.Instance.CloseWindow("LoadingView");
            DialogueManager.StartConversation(_cacheRowCfgStage.conversationAName);
        }

        /// <summary>
        /// BattleState的OnEnter
        /// </summary>
        public void OnEnterBattleState()
        {
            GameManager.Instance.StartCoroutine(UIManager.Instance.GetCtrl<TeamEditCtrl>("TeamEditView").EnterBattleScene());
        }

        /// <summary>
        /// ConversationBState的OnEnter
        /// </summary>
        public void OnEnterConversationBState()
        {
            GameManager.Instance.StartCoroutine(OnEnterConversationBStateIEnumerator());
        }
        
        private IEnumerator OnEnterConversationBStateIEnumerator()
        {
            UIManager.Instance.OpenWindow("LoadingView");
            yield return new WaitForSeconds(0.3f);
            BGMManager.Instance.StopBgm();
            UIManager.Instance.CloseWindow("LoadingView");
            DialogueManager.StartConversation(_cacheRowCfgStage.conversationBName);
        }

        /// <summary>
        /// 关卡流程执行完毕
        /// </summary>
        private void EndStageProcedure()
        {
            GameManager.Instance.ReturnToTitle(0.5f,
                () => { UIManager.Instance.OpenWindow(SaveManager.GetString("ChapterType", "MainPlot").Equals("MainPlot") ? "MainPlotSelectView" : "SubPlotSelectView"); });
        }

        /// <summary>
        /// 初始化状态机
        /// </summary>
        private void InitFsm()
        {
            _fsm = FsmManager.Instance.GetFsm<ProcedureFsm>();
            _fsm.SetFsm(new Dictionary<Type, IFsmState>()
            {
                {typeof(BattleState), new BattleState()},
                {typeof(ConversationAState), new ConversationAState()},
                {typeof(ConversationBState), new ConversationBState()},
                {typeof(TeamEditState), new TeamEditState()}
            });
        }
    }
}