// ******************************************************************
//       /\ /|       @file       CfgCamera.cs
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
    public class RowCfgCamera
    {
        public string key; //key
        public List<float> position; //坐标
        public List<float> rotation; //rotation
        public float fieldOfView; //视野
    }

    public class CfgCamera
    {
        private readonly Dictionary<string, RowCfgCamera> _configs = new Dictionary<string, RowCfgCamera>(); //cfgId映射row
        public RowCfgCamera this[string key] => _configs.ContainsKey(key) ? _configs[key] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{key}");
        public RowCfgCamera this[int id] => _configs.ContainsKey(id.ToString()) ? _configs[id.ToString()] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{id}");
        public List<RowCfgCamera> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgCamera Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgCamera Find(string i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgCamera.txt", 3);
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
        private RowCfgCamera ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 4)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgCamera();
            var rowHelper = new RowHelper(col);
            data.key = CsvUtility.ToString(rowHelper.ReadNextCol()); //key
            data.position = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //坐标
            data.rotation = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //rotation
            data.fieldOfView = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //视野
            return data;
        }
    }
}