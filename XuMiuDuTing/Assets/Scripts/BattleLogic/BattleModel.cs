using System.Collections.Generic;
using Rabi;
using UnityEngine;
using UnityEngine.UI;

namespace Yu
{
    public class BattleModel
    {
        private string[] _characterTeamArray;
        private StageDataEntry _stageDataEntry;
        private RowCfgStage _rowCfgStage;
        private StageData _stageData;
        public int currentRound;
        public readonly List<BattleEntityCtrl> allEntities = new List<BattleEntityCtrl>();
        public readonly List<BattleEntityCtrl> sortEntities = new List<BattleEntityCtrl>(); //根据速度排序后的entityIdList
        public readonly List<CharacterEntityCtrl> allCharacterEntities = new List<CharacterEntityCtrl>();
        public readonly List<EnemyEntityCtrl> allEnemyEntities = new List<EnemyEntityCtrl>();
        public int characterNumber;
        public int enemyNumber;
        public int currentMenuLastIndex;
        public int currentCharacterEntityIndex; //当前输入指令的角色在AllEntity中的下标
        public int currentEnemyEntityIndex; //当前输入指令的敌人在AllEntity中的下标

        //AimSelect
        public BattleCommandType selectMenuType;
        public int canSelectCount;
        public readonly List<Toggle> activeToggleList = new List<Toggle>(); //激活了的ToggleSelect
        public readonly List<BattleEntityCtrl> selectedEntityList = new List<BattleEntityCtrl>();
        public SkillInfo cacheCurrentSkillInfo;

        /// <summary>
        /// 设置友方编队
        /// </summary>
        public void SetTeamArray(string[] teamArray)
        {
            _characterTeamArray = teamArray;
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            _stageData = SaveManager.GetT("StageData", new StageData());
            var stageName = SaveManager.GetString("StageName", "1-1");
            _stageDataEntry = _stageData.allStage[stageName];
            _rowCfgStage = ConfigManager.Instance.cfgStage[_stageDataEntry.stageName];
            currentRound = 1;
        }

        /// <summary>
        /// 获取当前的角色
        /// </summary>
        /// <returns></returns>
        public CharacterEntityCtrl GetCurrentCharacterEntity()
        {
            return allCharacterEntities[currentCharacterEntityIndex];
        }

        /// <summary>
        /// 获取当前的敌人
        /// </summary>
        /// <returns></returns>
        public EnemyEntityCtrl GetCurrentEnemyEntity()
        {
            return allEnemyEntities[currentEnemyEntityIndex];
        }

        /// <summary>
        /// 设置当前角色
        /// </summary>
        /// <returns></returns>
        public void SetCurrentCharacterEntity(CharacterEntityCtrl currentCharacterEntity)
        {
            currentCharacterEntityIndex = allCharacterEntities.IndexOf(currentCharacterEntity);
        }

        /// <summary>
        /// 转换characterEntity
        /// </summary>
        /// <returns></returns>
        public CharacterEntityCtrl GetCharacterEntityByBaseEntity(BattleEntityCtrl baseEntity)
        {
            var characterEntity = baseEntity as CharacterEntityCtrl;
            if (!characterEntity)
            {
                Debug.LogError("转换失败" + baseEntity.GetName());
            }

            return characterEntity;
        }

        /// <summary>
        /// 转换enemyEntity
        /// </summary>
        /// <returns></returns>
        public EnemyEntityCtrl GetEnemyEntityByBaseEntity(BattleEntityCtrl baseEntity)
        {
            var enemyEntity = baseEntity as EnemyEntityCtrl;
            if (!enemyEntity)
            {
                Debug.LogError("转换失败" + baseEntity.GetName());
            }

            return enemyEntity;
        }

        /// <summary>
        /// 设置第一个可以输入指令的角色
        /// </summary>
        public void SetFirstCanInputCommandCharacter()
        {
            for (var i = 0; i < allCharacterEntities.Count; i++)
            {
                if (allCharacterEntities[i].IsDie())
                {
                    continue;
                }

                if (allCharacterEntities[i].GetBp() < 0)
                {
                    continue;
                }

                if (BattleManager.CheckBuff(allCharacterEntities[i], "眩晕").Count > 0)
                {
                    continue;
                }

                currentCharacterEntityIndex = i; //将第一个活着的entity的索引设为currentBattleId
                return;
            }

            currentCharacterEntityIndex = allCharacterEntities.Count;//溢出，已处理
        }

        /// <summary>
        /// 选出下一个可以输入指令的角色
        /// </summary>
        public void SetNextCanInputCommandCharacter()
        {
            for (var i = currentCharacterEntityIndex; i < allCharacterEntities.Count; i++)
            {
                if (allCharacterEntities[i].IsDie())
                {
                    continue;
                }

                if (allCharacterEntities[i].GetBp() < 0)
                {
                    continue;
                }

                if (BattleManager.CheckBuff(allCharacterEntities[i], "眩晕").Count > 0)
                {
                    continue;
                }

                currentCharacterEntityIndex = i; //将第一个活着的entity的索引设为currentBattleId
                return;
            }

            currentCharacterEntityIndex = allCharacterEntities.Count;//溢出，已处理
        }

        /// <summary>
        /// 设置第一个可以输入指令的敌人
        /// </summary>
        public void SetFirstCanInputCommandEnemy()
        {
            for (var i = 0; i < allEnemyEntities.Count; i++)
            {
                if (allEnemyEntities[i].IsDie())
                {
                    continue;
                }

                currentEnemyEntityIndex = i; //将第一个活着的entity的索引设为currentBattleId
                return;
            }
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
        /// 依据entity的Speed对allEntities排序
        /// </summary>
        public void SortEntityList()
        {
            sortEntities.Clear();
            foreach (var entity in allEntities)
            {
                sortEntities.Add(entity);
            }

            sortEntities.Sort((x, y) => y.GetSpeed().CompareTo(x.GetSpeed()));
            //测试排序
            var log = "排序后的character顺序：";
            for (var i = 0; i < sortEntities.Count; i++)
            {
                if (!sortEntities[i].isEnemy)
                {
                    log += (sortEntities[i].GetName() + "--");
                }
            }

            Debug.Log(log);
        }
    }
}