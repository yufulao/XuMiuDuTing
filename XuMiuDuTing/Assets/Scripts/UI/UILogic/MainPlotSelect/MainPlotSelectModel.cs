using System;
using System.Collections.Generic;
using System.Linq;
using Rabi;
using UnityEngine;

namespace Yu
{
    public class MainPlotSelectModel
    {
        private int _currentPlotIndex;
        private string _currentStageName;
        private List<string> _plotNameList = new List<string>();
        private List<string> _stageNameList = new List<string>();
        private StageData _stageData;
        private List<StageItem> _currentStageItemList = new List<StageItem>();
        private readonly Dictionary<string, GameObject> _stageFrameDic = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, List<StageItem>> _stageItemDic = new Dictionary<string, List<StageItem>>();

        public void OnInit()
        {
            _plotNameList = ConfigManager.Instance.cfgChapter[DefChapterType.DMainPlot].plotList;
        }

        /// <summary>
        /// 下一个Plot
        /// </summary>
        public void NextPlot()
        {
            _currentPlotIndex++;
            if (_currentPlotIndex >= _plotNameList.Count)
            {
                _currentPlotIndex = 0;
            }

            ReloadStageList();
        }

        /// <summary>
        /// 上一个Plot
        /// </summary>
        public void PrePlot()
        {
            _currentPlotIndex--;
            if (_currentPlotIndex < 0)
            {
                _currentPlotIndex = _plotNameList.Count - 1;
            }

            ReloadStageList();
        }

        /// <summary>
        /// 设置当前选择的数据
        /// </summary>
        public void SetMainPlotData()
        {
            _stageData=SaveManager.GetT("StageData", new StageData());
            SetPlotIndexInSave();
            ReloadStageList();
            SetStageNameInSave();
        }

        /// <summary>
        /// 获取当前的PlotName
        /// </summary>
        /// <returns></returns>
        public string GetCurrentPlotName()
        {
            return ConfigManager.Instance.cfgChapter[DefChapterType.DMainPlot].plotList[_currentPlotIndex];
        }

        /// <summary>
        /// 获取stageDataEntry
        /// </summary>
        /// <param name="stageNameT"></param>
        /// <returns></returns>
        public StageDataEntry GetStageDataEntry(string stageNameT)
        {
            var allStage = _stageData.allStage;
            if (allStage.ContainsKey(stageNameT))
            {
                return allStage[stageNameT];
            }

            var newStageDataEntry = new StageDataEntry()
            {
                stageName = stageNameT,
                isPass = false,
                isUnlock = false
            };
            allStage.Add(stageNameT, newStageDataEntry);
            return newStageDataEntry;
        }

        /// <summary>
        /// 获取StageFrameDic
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, GameObject> GetStageFrameDic()
        {
            return _stageFrameDic;
        }
        
        /// <summary>
        /// 获取StageItemDic
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<StageItem>> GetStageItemDic()
        {
            return _stageItemDic;
        }

        /// <summary>
        /// 获取当前的stageItem列表
        /// </summary>
        /// <returns></returns>
        public List<StageItem> GetCurrentStageItemList()
        {
            return _currentStageItemList;
        }
        
        /// <summary>
        /// 设置当前的stageItem列表
        /// </summary>
        /// <returns></returns>
        public void SetCurrentStageItemList(List<StageItem> stageItemList)
        {
            _currentStageItemList = stageItemList;
        }

        /// <summary>
        /// 判断是不是直接就可以通关
        /// </summary>
        public void CheckPassStageOfNoBattle()
        {
            var rowCfgStage = ConfigManager.Instance.cfgStage[_currentStageName];
            if (!rowCfgStage.isBattle)
            {
                GameManager.Instance.PassStage(_currentStageName);
            }
        }

        /// <summary>
        /// 获取StageNameList
        /// </summary>
        /// <returns></returns>
        public List<string> GetStageNameList()
        {
            return _stageNameList;
        }
        
        /// <summary>
        /// 获取当前选择的关卡名字
        /// </summary>
        /// <returns></returns>
        public string GetCurrentStageName()
        {
            return _currentStageName;
        }

        /// <summary>
        /// 设置当前所选的关卡名
        /// </summary>
        /// <param name="stageName"></param>
        public void SetCurrentStageName(string stageName)
        {
            _currentStageName = stageName;
        }

        /// <summary>
        /// 重新加载StageList
        /// </summary>
        private void ReloadStageList()
        {
            _stageNameList = ConfigManager.Instance.cfgMainPlot[_plotNameList[_currentPlotIndex]].stageList;
            SetCurrentStageName(null); //重置关卡名
        }

        /// <summary>
        /// 设置存档中的所选Plot
        /// </summary>
        private void SetPlotIndexInSave()
        {
            var plotName = SaveManager.GetString("PlotNameInMainPlot", "Null");

            for (var i = 0; i < _plotNameList.Count; i++)
            {
                if (!plotName.Equals(_plotNameList[i]))
                {
                    continue;
                }

                _currentPlotIndex = i;
                return;
            }

            SaveManager.SetString("PlotNameInMainPlot", _plotNameList[0]);
            _currentPlotIndex = 0;
        }

        /// <summary>
        /// 设置存档中的所选关卡
        /// </summary>
        private void SetStageNameInSave()
        {
            var stageName = SaveManager.GetString("StageName", "Null");
            if (_stageNameList.Any(stageNameTemp => stageName.Equals(stageNameTemp)))
            {
                _currentStageName = stageName;
                return;
            }

            stageName = _stageNameList[0];
            SaveManager.SetString("StageName", stageName);
            _currentStageName = stageName;
        }
    }
}