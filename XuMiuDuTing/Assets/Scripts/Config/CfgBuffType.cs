// ******************************************************************
//       /\ /|       @file       CfgBuffType.cs
//       \ V/        @brief      excel数据解析(由python自动生成) ./xlsx//Buff.xlsx
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
    public class RowCfgBuffType
    {
        public string key; //key
        public string annotate; //注释
        public string defName; //枚举名称
    }

    public class CfgBuffType
    {
        private readonly Dictionary<string, RowCfgBuffType> _configs = new Dictionary<string, RowCfgBuffType>(); //cfgId映射row
        public RowCfgBuffType this[string key] => _configs.ContainsKey(key) ? _configs[key] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{key}");
        public RowCfgBuffType this[int id] => _configs.ContainsKey(id.ToString()) ? _configs[id.ToString()] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{id}");
        public List<RowCfgBuffType> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgBuffType Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgBuffType Find(string i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgBuffType.txt", 3);
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
        private RowCfgBuffType ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 3)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgBuffType();
            var rowHelper = new RowHelper(col);
            data.key = CsvUtility.ToString(rowHelper.ReadNextCol()); //key
            data.annotate = CsvUtility.ToString(rowHelper.ReadNextCol()); //注释
            data.defName = CsvUtility.ToString(rowHelper.ReadNextCol()); //枚举名称
            return data;
        }
    }
}