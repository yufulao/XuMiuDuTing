// ******************************************************************
//       /\ /|       @file       CfgSFX.cs
//       \ V/        @brief      excel数据解析(由python自动生成) ./xlsx//SFX.xlsx
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
    public class RowCfgSFX
    {
        public string key; //SFX名
        public string sFXType; //音效类别
        public List<string> clipPaths; //资源路径
        public float volume; //初始音量
        public bool oneShot; //是否oneShot
        public bool loop; //是否循环
    }

    public class CfgSFX
    {
        private readonly Dictionary<string, RowCfgSFX> _configs = new Dictionary<string, RowCfgSFX>(); //cfgId映射row
        public RowCfgSFX this[string key] => _configs.ContainsKey(key) ? _configs[key] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{key}");
        public RowCfgSFX this[int id] => _configs.ContainsKey(id.ToString()) ? _configs[id.ToString()] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{id}");
        public List<RowCfgSFX> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgSFX Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgSFX Find(string i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgSFX.txt", 3);
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
        private RowCfgSFX ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 6)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgSFX();
            var rowHelper = new RowHelper(col);
            data.key = CsvUtility.ToString(rowHelper.ReadNextCol()); //SFX名
            data.sFXType = CsvUtility.ToString(rowHelper.ReadNextCol()); //音效类别
            data.clipPaths = CsvUtility.ToList<string>(rowHelper.ReadNextCol()); //资源路径
            data.volume = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //初始音量
            data.oneShot = CsvUtility.ToBool(rowHelper.ReadNextCol()); //是否oneShot
            data.loop = CsvUtility.ToBool(rowHelper.ReadNextCol()); //是否循环
            return data;
        }
    }
}