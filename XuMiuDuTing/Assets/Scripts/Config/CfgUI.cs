// ******************************************************************
//       /\ /|       @file       CfgUI.cs
//       \ V/        @brief      excel数据解析(由python自动生成) ./xlsx//UI.xlsx
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
    public class RowCfgUI
    {
        public string key; //key
        public string layer; //所属层级
        public int sortOrder; //canvas的层级
        public string uiPath; //预设路径
    }

    public class CfgUI
    {
        private readonly Dictionary<string, RowCfgUI> _configs = new Dictionary<string, RowCfgUI>(); //cfgId映射row
        public RowCfgUI this[string key] => _configs.ContainsKey(key) ? _configs[key] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{key}");
        public RowCfgUI this[int id] => _configs.ContainsKey(id.ToString()) ? _configs[id.ToString()] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{id}");
        public List<RowCfgUI> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgUI Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgUI Find(string i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgUI.txt", 3);
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
        private RowCfgUI ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 4)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgUI();
            var rowHelper = new RowHelper(col);
            data.key = CsvUtility.ToString(rowHelper.ReadNextCol()); //key
            data.layer = CsvUtility.ToString(rowHelper.ReadNextCol()); //所属层级
            data.sortOrder = CsvUtility.ToInt(rowHelper.ReadNextCol()); //canvas的层级
            data.uiPath = CsvUtility.ToString(rowHelper.ReadNextCol()); //预设路径
            return data;
        }
    }
}