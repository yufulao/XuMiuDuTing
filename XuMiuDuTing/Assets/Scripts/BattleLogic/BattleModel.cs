using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;
using Yu;

public class BattleModel
{
    private string[] _characterTeamArray;
    private StageDataEntry _stageDataEntry;
    private RowCfgStage _rowCfgStage;
    private StageData _stageData;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="teamArray"></param>
    /// <param name="stageName"></param>
    public void Init(string[] teamArray, string stageName)
    {
        _characterTeamArray = teamArray;
        _stageData = SaveManager.GetT("StageData", new StageData());
        _stageDataEntry = _stageData.allStage[stageName];
        _rowCfgStage = ConfigManager.Instance.cfgStage[_stageDataEntry.stageName];
    }

    /// <summary>
    /// 获取角色队伍
    /// </summary>
    /// <returns></returns>
    public string[] GetCharacterTeamArray()
    {
        return _characterTeamArray;
    }

    /// <summary>
    /// 获取敌人队伍
    /// </summary>
    /// <returns></returns>
    public List<string> GetEnemyTeam()
    {
        return _rowCfgStage.enemyTeam;
    }

    /// <summary>
    /// 通关
    /// </summary>
    public void PassStage()
    {
        if (_stageDataEntry.isPass)
        {
            return;
        }

        _stageDataEntry.isPass = true;
        var unlockStageList = _rowCfgStage.unlockStageList;
        foreach (var unlockStageName in unlockStageList)
        {
            _stageData.allStage[unlockStageName].isUnlock = true;
        }

        SaveManager.SetT("StageData", _stageData);
        
        //其他通关特定关执行卡特定事件，下面写================================================================================================================
    }
}