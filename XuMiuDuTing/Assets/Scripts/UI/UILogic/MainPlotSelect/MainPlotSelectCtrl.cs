using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Rabi;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Yu
{
    public class MainPlotSelectCtrl : UICtrlBase
    {
        private MainPlotSelectModel _model;
        private MainPlotSelectView _view;


        public override void OnInit(params object[] param)
        {
            _model = new MainPlotSelectModel();
            _view = GetComponent<MainPlotSelectView>();
            //默认开放第一关
            GameManager.Instance.UnlockStage(ConfigManager.Instance.cfgMainPlot[ConfigManager.Instance.cfgChapter[DefChapterType.DMainPlot].plotList[1]].stageList[0]);
            GameManager.Instance.UnlockStage(ConfigManager.Instance.cfgMainPlot[ConfigManager.Instance.cfgChapter[DefChapterType.DMainPlot].plotList[0]].stageList[0]);
            _model.OnInit();
            InitAllStageItem();
        }

        public override void OpenRoot(params object[] param)
        {
            _model.SetMainPlotData();
            UpdateAll();
            _view.OpenWindow();
        }

        public override void CloseRoot()
        {
            _view.CloseWindow();
        }

        public override void BindEvent()
        {
            _view.btnBack.onClick.AddListener(BtnOnClickBack);
            _view.btnEnter.onClick.AddListener(BtnOnClickEnter);
            _view.btnNextPlot.onClick.AddListener(BtnOnClickNextPlot);
            _view.btnPrePlot.onClick.AddListener(BtnOnClickPrePlot);
        }

        /// <summary>
        /// 设置关卡是否通关
        /// </summary>
        /// <param name="stageNameT"></param>
        /// <param name="isUnlock"></param>
        /// <param name="isPass"></param>
        private void SetStageData(string stageNameT, bool isUnlock, bool isPass)
        {
            var stageDataEntry = _model.GetStageDataEntry(stageNameT);
            stageDataEntry.isUnlock = isUnlock;
            stageDataEntry.isPass = isPass;
            _model.SaveStageData();
        }

        /// <summary>
        /// 返回
        /// </summary>
        private void BtnOnClickBack()
        {
            CloseRoot();
        }

        /// <summary>
        /// 进入关卡
        /// </summary>
        private void BtnOnClickEnter()
        {
            SaveManager.SetString("PlotNameInMainPlot", _model.GetCurrentPlotName());
            SaveManager.SetString("StageName", _model.GetCurrentStageName());
            _model.CheckPassStageOfNoBattle();
            ProcedureManager.Instance.SetStageProcedure(_model.GetCurrentStageName());
        }

        /// <summary>
        /// 下一个Plot
        /// </summary>
        private void BtnOnClickNextPlot()
        {
            StartCoroutine(IBtnOnClickNextPlot());
        }

        private IEnumerator IBtnOnClickNextPlot()
        {
            SFXManager.Instance.PlaySfx("上一章下一章");
            _view.canvasGroupFadeMask.gameObject.SetActive(true);
            _view.canvasGroupFadeMask.DOFade(1f, 0.5f);
            yield return new WaitForSeconds(0.5f);
            _model.NextPlot();
            UpdateAll();
            _view.ForceCloseStageInfo();
            _view.canvasGroupFadeMask.DOFade(0f, 0.5f);
            yield return new WaitForSeconds(0.5f);
            _view.canvasGroupFadeMask.gameObject.SetActive(false);
        }

        /// <summary>
        /// 上一个Plot
        /// </summary>
        private void BtnOnClickPrePlot()
        {
            StartCoroutine(IBtnOnClickPrePlot());
        }

        private IEnumerator IBtnOnClickPrePlot()
        {
            SFXManager.Instance.PlaySfx("上一章下一章");
            _view.canvasGroupFadeMask.gameObject.SetActive(true);
            _view.canvasGroupFadeMask.DOFade(1f, 0.5f);
            yield return new WaitForSeconds(0.5f);
            _model.PrePlot();
            UpdateAll();
            _view.ForceCloseStageInfo();
            _view.canvasGroupFadeMask.DOFade(0f, 0.5f);
            yield return new WaitForSeconds(0.5f);
            _view.canvasGroupFadeMask.gameObject.SetActive(false);
        }

        /// <summary>
        /// 刷新所有视图
        /// </summary>
        private void UpdateAll()
        {
            var rowCfgMainPlot = ConfigManager.Instance.cfgMainPlot[_model.GetCurrentPlotName()];
            var stageName = _model.GetCurrentStageName();
            RowCfgStage rowCfgStage = null;
            if (!string.IsNullOrEmpty(stageName))
            {
                rowCfgStage = ConfigManager.Instance.cfgStage[stageName];
            }

            _view.RefreshAll(rowCfgMainPlot, rowCfgStage);
            UpdateAllStageItem();
        }

        /// <summary>
        /// 初始化所有StageItem
        /// </summary>
        private void InitAllStageItem()
        {
            foreach (var stageItem in _view.stageItemList)
            {
                stageItem.SetToggleSelectOnClick(OnSelectStageItemClick);
            }
        }

        /// <summary>
        /// 刷新所有StageItem
        /// </summary>
        private void UpdateAllStageItem()
        {
            var currentStageName = _model.GetCurrentStageName();
            var stageNameList = _model.GetStageNameList();
            for (var i = 0; i < stageNameList.Count; i++)
            {
                _view.stageItemList[i].Refresh(_model.GetStageDataEntry(stageNameList[i]), !string.IsNullOrEmpty(currentStageName) && currentStageName.Equals(stageNameList[i]));
            }

            for (var i = stageNameList.Count; i < _view.stageItemList.Count; i++)
            {
                _view.stageItemList[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 点击选择StageItem时
        /// </summary>
        private void OnSelectStageItemClick(string stageName)
        {
            if (string.IsNullOrEmpty(stageName))
            {
                return;
            }

            var rowCfgStage = ConfigManager.Instance.cfgStage[stageName];
            _view.RefreshStageInfo(rowCfgStage.stageName, rowCfgStage.stageDesc);
            _model.SetCurrentStageName(stageName);
        }
    }
}