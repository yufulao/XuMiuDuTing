// ******************************************************************
//       /\ /|       @file       CfgObjCameraStateType.cs
//       \ V/        @brief      excel数据解析(由python自动生成) ./xlsx//Camera.xlsx
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
    public class RowCfgObjCameraStateType
    {
        public string key; //key
        public string annotate; //注释
        public string defName; //枚举名称
    }

    public class CfgObjCameraStateType
    {
        private readonly Dictionary<string, RowCfgObjCameraStateType> _configs = new Dictionary<string, RowCfgObjCameraStateType>(); //cfgId映射row
        public RowCfgObjCameraStateType this[string key] => _configs.ContainsKey(key) ? _configs[key] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{key}");
        public RowCfgObjCameraStateType this[int id] => _configs.ContainsKey(id.ToString()) ? _configs[id.ToString()] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{id}");
        public List<RowCfgObjCameraStateType> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgObjCameraStateType Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgObjCameraStateType Find(string i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgObjCameraStateType.txt", 3);
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
        private RowCfgObjCameraStateType ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 3)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgObjCameraStateType();
            var rowHelper = new RowHelper(col);
            data.key = CsvUtility.ToString(rowHelper.ReadNextCol()); //key
            data.annotate = CsvUtility.ToString(rowHelper.ReadNextCol()); //注释
            data.defName = CsvUtility.ToString(rowHelper.ReadNextCol()); //枚举名称
            return data;
        }
    }
}