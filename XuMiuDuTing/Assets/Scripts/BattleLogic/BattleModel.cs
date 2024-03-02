using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;
using UnityEngine.UI;
using Yu;

public class BattleModel
{
    private string[] _characterTeamArray;
    private StageDataEntry _stageDataEntry;
    private RowCfgStage _rowCfgStage;
    private StageData _stageData;
    public int currentRound;
    public List<BattleEntityCtrl> allEntities = new List<BattleEntityCtrl>();
    private List<BattleEntityCtrl> sortEntities = new List<BattleEntityCtrl>();//根据速度排序后的entityIdList
    public List<CharacterEntityCtrl> allCharacterEntities = new List<CharacterEntityCtrl>();
    public List<EnemyEntityCtrl> allEnemyEntities = new List<EnemyEntityCtrl>();
    public int characterNumber;
    public int characterCount;
    public int enemyNumber;
    public int canSelectCount;
    public List<Toggle> activedToggleList = new List<Toggle>();//可以进行选择的目标toggle
    public List<BattleEntityCtrl> selectedEntityList = new List<BattleEntityCtrl>();
    public int currentMenuLastIndex;
    

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="teamArray"></param>
    public void Init(string[] teamArray)
    {
        _characterTeamArray = teamArray;
        _stageData = SaveManager.GetT("StageData", new StageData());
        var stageName = SaveManager.GetString("StageName", "1-1");
        _stageDataEntry = _stageData.allStage[stageName];
        _rowCfgStage = ConfigManager.Instance.cfgStage[_stageDataEntry.stageName];
        currentRound = 1;
        SortEntityList();
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
    
    /// <summary>
    /// 依据entity的Speed对allEntities排序
    /// </summary>
    private void SortEntityList()
    {
        sortEntities.Clear();
        for (int i = 0; i < allEntities.Count; i++)
        {
            sortEntities.Add(allEntities[i]);
        }

        sortEntities.Sort((x, y) => { return y.GetSpeed().CompareTo(x.GetSpeed()); });
        //测试排序
        var log = "排序后的character顺序：";
        for (var i = 0; i < sortEntities.Count; i++)
        {
            if (!sortEntities[i].isEnemy)
            {
                log += (sortEntities[i] as CharacterEntityCtrl)?.GetCharacterName() + 1 + "--";
            }
        }
        Debug.Log(log);
    }
}