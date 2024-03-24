// ******************************************************************
//       /\ /|       @file       CfgMainPlot.cs
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
    public class RowCfgMainPlot
    {
        public string key; //key
        public string plotBGM; //故事bgm
        public string plotBG; //故事背景图片路径
        public List<string> stageList; //关卡列表
        public string stageFramePath; //stageItemList预设路径
        public bool vfxFogActive; //右下角是否开启迷雾
    }

    public class CfgMainPlot
    {
        private readonly Dictionary<string, RowCfgMainPlot> _configs = new Dictionary<string, RowCfgMainPlot>(); //cfgId映射row
        public RowCfgMainPlot this[string key] => _configs.ContainsKey(key) ? _configs[key] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{key}");
        public RowCfgMainPlot this[int id] => _configs.ContainsKey(id.ToString()) ? _configs[id.ToString()] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{id}");
        public List<RowCfgMainPlot> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgMainPlot Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgMainPlot Find(string i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgMainPlot.txt", 3);
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
        private RowCfgMainPlot ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 6)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgMainPlot();
            var rowHelper = new RowHelper(col);
            data.key = CsvUtility.ToString(rowHelper.ReadNextCol()); //key
            data.plotBGM = CsvUtility.ToString(rowHelper.ReadNextCol()); //故事bgm
            data.plotBG = CsvUtility.ToString(rowHelper.ReadNextCol()); //故事背景图片路径
            data.stageList = CsvUtility.ToList<string>(rowHelper.ReadNextCol()); //关卡列表
            data.stageFramePath = CsvUtility.ToString(rowHelper.ReadNextCol()); //stageItemList预设路径
            data.vfxFogActive = CsvUtility.ToBool(rowHelper.ReadNextCol()); //右下角是否开启迷雾
            return data;
        }
    }
}