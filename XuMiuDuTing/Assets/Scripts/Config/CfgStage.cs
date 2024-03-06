// ******************************************************************
//       /\ /|       @file       CfgStage.cs
//       \ V/        @brief      excel数据解析(由python自动生成) ./xlsx//Stage.xlsx
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |
//      /  \\        @Modified   2022-04-25 13:25:11
//    *(__\_\        @Copyright  Copyright (c)  2022, Shadowrabbit
// ******************************************************************

using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Rabi
{
    public class RowCfgStage
    {
        public string key; //key
        public string stageName; //关卡标题
        public string stageDesc; //关卡描述
        public string conversationAName; //对话A名
        public bool isBattle; //是否有战斗
        public string conversationBName; //对话B名
        public List<string> fixCharacterTeam; //固定角色队伍
        public List<string> enemyTeam; //敌人列表
        public List<string> unlockStageList; //通关后解锁的关卡列表
    }

    public class CfgStage
    {
        private readonly Dictionary<string, RowCfgStage> _configs = new Dictionary<string, RowCfgStage>(); //cfgId映射row
        public RowCfgStage this[string key] => _configs.ContainsKey(key) ? _configs[key] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{key}");
        public RowCfgStage this[int id] => _configs.ContainsKey(id.ToString()) ? _configs[id.ToString()] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{id}");
        public List<RowCfgStage> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgStage Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgStage Find(string i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgStage.txt", 3);
            var rows = reader.GetRowCount();
            for (var i = 0; i < rows; ++i)
            {
                var row = reader.GetColValueArray(i);
                var data = ParseRow(row);
                if (!_configs.ContainsKey(data.key))
                {
                    _configs.Add(data.key, data);
                }
            }
        }

        /// <summary>
        /// 解析行
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        private RowCfgStage ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 9)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgStage();
            var rowHelper = new RowHelper(col);
            data.key = CsvUtility.ToString(rowHelper.ReadNextCol()); //key
            data.stageName = CsvUtility.ToString(rowHelper.ReadNextCol()); //关卡标题
            data.stageDesc = CsvUtility.ToString(rowHelper.ReadNextCol()); //关卡描述
            data.conversationAName = CsvUtility.ToString(rowHelper.ReadNextCol()); //对话A名
            data.isBattle = CsvUtility.ToBool(rowHelper.ReadNextCol()); //是否有战斗
            data.conversationBName = CsvUtility.ToString(rowHelper.ReadNextCol()); //对话B名
            data.fixCharacterTeam = CsvUtility.ToList<string>(rowHelper.ReadNextCol()); //固定角色队伍
            data.enemyTeam = CsvUtility.ToList<string>(rowHelper.ReadNextCol()); //敌人列表
            data.unlockStageList = CsvUtility.ToList<string>(rowHelper.ReadNextCol()); //通关后解锁的关卡列表
            return data;
        }
    }
}