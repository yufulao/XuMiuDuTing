// ******************************************************************
//       /\ /|       @file       CfgSkill.cs
//       \ V/        @brief      excel数据解析(由python自动生成) ./xlsx//Skill.xlsx
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
    public class RowCfgSkill
    {
        public string key; //key
        public string description; //skill描述
        public bool unlockDefault; //默认是否解锁
        public bool needSelect; //是否需要选择目标
        public bool isBattleStartCommand; //是否是先制指令
        public int bpNeed; //bp消耗量
        public string selectType; //目标选择类型
        public string objCameraStateType; //默认摄像机视角(需要选择目标)
        public int selectCount; //选择的目标数量
    }

    public class CfgSkill
    {
        private readonly Dictionary<string, RowCfgSkill> _configs = new Dictionary<string, RowCfgSkill>(); //cfgId映射row
        public RowCfgSkill this[string key] => _configs.ContainsKey(key) ? _configs[key] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{key}");
        public RowCfgSkill this[int id] => _configs.ContainsKey(id.ToString()) ? _configs[id.ToString()] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{id}");
        public List<RowCfgSkill> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgSkill Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgSkill Find(string i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgSkill.txt", 3);
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
        private RowCfgSkill ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 9)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgSkill();
            var rowHelper = new RowHelper(col);
            data.key = CsvUtility.ToString(rowHelper.ReadNextCol()); //key
            data.description = CsvUtility.ToString(rowHelper.ReadNextCol()); //skill描述
            data.unlockDefault = CsvUtility.ToBool(rowHelper.ReadNextCol()); //默认是否解锁
            data.needSelect = CsvUtility.ToBool(rowHelper.ReadNextCol()); //是否需要选择目标
            data.isBattleStartCommand = CsvUtility.ToBool(rowHelper.ReadNextCol()); //是否是先制指令
            data.bpNeed = CsvUtility.ToInt(rowHelper.ReadNextCol()); //bp消耗量
            data.selectType = CsvUtility.ToString(rowHelper.ReadNextCol()); //目标选择类型
            data.objCameraStateType = CsvUtility.ToString(rowHelper.ReadNextCol()); //默认摄像机视角(需要选择目标)
            data.selectCount = CsvUtility.ToInt(rowHelper.ReadNextCol()); //选择的目标数量
            return data;
        }
    }
}