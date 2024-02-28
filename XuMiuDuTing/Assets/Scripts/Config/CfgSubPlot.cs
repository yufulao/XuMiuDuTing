// ******************************************************************
//       /\ /|       @file       CfgSubPlot.cs
//       \ V/        @brief      excel数据解析(由python自动生成) ./xlsx//Chapter.xlsx
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
    public class RowCfgSubPlot
    {
        public string key; //key
        public string plotTitle; //故事标题图片路径
        public string plotBG; //故事背景图片路径
        public List<string> stageList; //关卡列表
    }

    public class CfgSubPlot
    {
        private readonly Dictionary<string, RowCfgSubPlot> _configs = new Dictionary<string, RowCfgSubPlot>(); //cfgId映射row
        public RowCfgSubPlot this[string key] => _configs.ContainsKey(key) ? _configs[key] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{key}");
        public RowCfgSubPlot this[int id] => _configs.ContainsKey(id.ToString()) ? _configs[id.ToString()] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{id}");
        public List<RowCfgSubPlot> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgSubPlot Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgSubPlot Find(string i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgSubPlot.txt", 3);
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
        private RowCfgSubPlot ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 4)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgSubPlot();
            var rowHelper = new RowHelper(col);
            data.key = CsvUtility.ToString(rowHelper.ReadNextCol()); //key
            data.plotTitle = CsvUtility.ToString(rowHelper.ReadNextCol()); //故事标题图片路径
            data.plotBG = CsvUtility.ToString(rowHelper.ReadNextCol()); //故事背景图片路径
            data.stageList = CsvUtility.ToList<string>(rowHelper.ReadNextCol()); //关卡列表
            return data;
        }
    }
}