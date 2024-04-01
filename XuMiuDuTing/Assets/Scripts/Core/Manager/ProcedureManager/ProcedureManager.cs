using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PixelCrushers.DialogueSystem;
using Rabi;
using UnityEngine;
using UnityEngine.Events;

namespace Yu
{
    public class ProcedureManager : BaseSingleTon<ProcedureManager>, IMonoManager
    {
        private ProcedureFsm _fsm;
        private readonly List<Type> _currentStageProcedure = new List<Type>();
        private RowCfgStage _cacheRowCfgStage;
        private UnityAction _teamEditActionAddon;
        private UnityAction _conversationAActionAddon;
        private UnityAction _battleActionAddon;
        private UnityAction _conversationBActionAddon;

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

            CheckEnterSpecialStage(stageName);//添加特定关卡事件
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
        /// 设置空状态
        /// </summary>
        public void SetNullState()
        {
            _fsm.ChangeToNullState();
            _teamEditActionAddon = null;
            _conversationAActionAddon = null;
            _battleActionAddon = null;
            _conversationBActionAddon = null;
        }

        /// <summary>
        /// TeamEditState的OnEnter
        /// </summary>
        public void OnEnterTeamEditState()
        {
            EventManager.Instance.Dispatch(EventName.TeamEditStateEnter);
            _teamEditActionAddon?.Invoke();
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
            EventManager.Instance.Dispatch(EventName.ConversationAStateEnter);
            _conversationAActionAddon?.Invoke();
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
            GameManager.Instance.StartCoroutine(UIManager.Instance.GetCtrl<TeamEditCtrl>("TeamEditView").EnterBattleScene(_cacheRowCfgStage, () =>
            {
                EventManager.Instance.Dispatch(EventName.BattleStateEnter);
                _battleActionAddon?.Invoke();
            }));
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
            EventManager.Instance.Dispatch(EventName.ConversationBStateEnter);
            _conversationBActionAddon?.Invoke();
            UIManager.Instance.OpenWindow("LoadingView");
            yield return new WaitForSeconds(0.3f);
            BGMManager.Instance.StopBgm();
            UIManager.Instance.CloseWindow("LoadingView");
            DialogueManager.StartConversation(_cacheRowCfgStage.conversationBName);
        }

        /// <summary>
        /// 关卡流程执行完毕
        /// </summary>
        public void EndStageProcedure()
        {
            GameManager.Instance.StartCoroutine(EndStageProcedureIEnumerator());
        }

        /// <summary>
        /// 关卡流程执行完毕的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator EndStageProcedureIEnumerator()
        {
            yield return GameManager.Instance.ReturnToTitle(false, 0f);
            UIManager.Instance.OpenWindow(SaveManager.GetString("ChapterType", "MainPlot").Equals("MainPlot") ? "MainPlotSelectView" : "SubPlotSelectView");
        }
        
        /// <summary>
        /// 检测进入特定的关卡
        /// </summary>
        private void CheckEnterSpecialStage(string stageName)
        {
            if (stageName.Equals("0-0"))
            {
                _battleActionAddon = () => { UIManager.Instance.OpenWindow("BattleTutorialView"); };
            }
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