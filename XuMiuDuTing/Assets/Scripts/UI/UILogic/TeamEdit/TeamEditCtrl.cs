using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rabi;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Yu
{
    public class TeamEditCtrl : UICtrlBase
    {
        private TeamEditModel _model;
        private TeamEditView _view;

        //param[0]是ChapterType，param[1]PlotName），param[2]是string[4] 固定编队
        public override void OnInit(params object[] param)
        {
            _model = new TeamEditModel();
            _view = GetComponent<TeamEditView>();
        }

        public override void OpenRoot(params object[] param)
        {
            _model.OnOpen();
            CheckFixedTeamArray(param);
            UpdateTeamArray();
            _view.OpenWindow(param);
            StartCoroutine(BGMManager.Instance.PlayBgmFadeDelay("战前编队界面", 0.3f, 0f, 0f));
        }

        public override void CloseRoot()
        {
            StartCoroutine(BGMManager.Instance.PlayBgmFadeDelay("主界面-章节选择界面", 0.3f, 0f, 0f));
            _view.CloseWindow();
        }

        public override void BindEvent()
        {
            _view.btnBack.onClick.AddListener(BtnOnClickBack);
            _view.btnEnter.onClick.AddListener(BtnOnClickEnter);
            foreach (var teamItem in (_view.teamItemList))
            {
                teamItem.SetBtnOnClickEdit(TeamItemOnClick);
            }
        }

        /// <summary>
        /// 交换队员，给CharacterSelect窗口调用
        /// </summary>
        public void ChangeTeammate(int teammateIndex, string replaceCharacterName)
        {
            _model.ChangeTeammate(teammateIndex, replaceCharacterName);
            UpdateTeamArray();
        }

        /// <summary>
        /// 移除队员，给CharacterSelect窗口调用
        /// </summary>
        public void RemoveTeammate(int teammateIndex)
        {
            _model.RemoveTeammate(teammateIndex);
            UpdateTeamArray();
        }

        /// <summary>
        /// 进入战斗关卡
        /// </summary>
        /// <returns></returns>
        public IEnumerator EnterBattleScene()
        {
            UIManager.Instance.OpenWindow("LoadingView");
            yield return new WaitForSeconds(0.5f);
            UIManager.Instance.CloseAllLayerWindows("NormalLayer");
            yield return SceneManager.Instance.ChangeSceneAsync(ConfigManager.Instance.cfgScene["Battle"].scenePath);
            var battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
            battleManager.EnterBattle(_model.GetTeamArray());
        }

        /// <summary>
        /// 检测并设置固定队伍
        /// </summary>
        private void CheckFixedTeamArray(IReadOnlyList<object> param)
        {
            string[] teamArray = null;
            var isFixed = false;
            if (param.Count > 2 && (param[2] is string[]))
            {
                teamArray = (string[]) param[2];
                isFixed = true;
                if (teamArray.Length != 4)
                {
                    Debug.LogError("传入的固定队伍参数不正确");
                    return;
                }
            }

            _model.SetFixedTeamArray(isFixed, teamArray);
        }

        /// <summary>
        /// 刷新编队
        /// </summary>
        private void UpdateTeamArray()
        {
            var teamArray = _model.GetTeamArray();
            var teamItemList = _view.teamItemList;
            for (var i = 0; i < teamItemList.Count; i++)
            {
                teamItemList[i].Refresh(teamArray[i], i);
            }

            _view.objTeamFixedMask.SetActive(_model.GetIsFixed());
        }

        /// <summary>
        /// teamItem的点击回调
        /// </summary>
        /// <param name="characterName"></param>
        /// <param name="teammateIndex"></param>
        private void TeamItemOnClick(string characterName, int teammateIndex)
        {
            UIManager.Instance.OpenWindow("CharacterSelectView", _model.GetTeamArray(), teammateIndex, characterName);
        }

        /// <summary>
        /// 返回
        /// </summary>
        private void BtnOnClickBack()
        {
            StartCoroutine(BGMManager.Instance.PlayBgmFadeDelay("主界面-章节选择界面", 0.3f, 0f, 0f));
            CloseRoot();
        }

        /// <summary>
        /// enter点击时
        /// </summary>
        private void BtnOnClickEnter()
        {
            if (_model.IsTeamArrayEmpty())
            {
                return;
            }
            _model.SaveTeamData();
            ProcedureManager.Instance.EnterNextStageProcedure();
        }
    }
}