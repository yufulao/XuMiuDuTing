// ******************************************************************
//       /\ /|       @file       CfgBuff.cs
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
    public class RowCfgBuff
    {
        public string key; //key
        public string description; //buff描述
        public string buffType; //buff类型
        public bool isDebuff; //是不是debuff
        public bool canExistWithSame; //相同buff是否可并行存在
        public bool canAddLayer; //是否可叠层
        public int maxLayer; //最高层数
        public bool canDispel; //是否可驱散
        public bool isUpdateDuringRepeatCast; //重复释放是否刷新持续时间
    }

    public class CfgBuff
    {
        private readonly Dictionary<string, RowCfgBuff> _configs = new Dictionary<string, RowCfgBuff>(); //cfgId映射row
        public RowCfgBuff this[string key] => _configs.ContainsKey(key) ? _configs[key] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{key}");
        public RowCfgBuff this[int id] => _configs.ContainsKey(id.ToString()) ? _configs[id.ToString()] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{id}");
        public List<RowCfgBuff> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgBuff Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgBuff Find(string i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgBuff.txt", 3);
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
        private RowCfgBuff ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 9)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgBuff();
            var rowHelper = new RowHelper(col);
            data.key = CsvUtility.ToString(rowHelper.ReadNextCol()); //key
            data.description = CsvUtility.ToString(rowHelper.ReadNextCol()); //buff描述
            data.buffType = CsvUtility.ToString(rowHelper.ReadNextCol()); //buff类型
            data.isDebuff = CsvUtility.ToBool(rowHelper.ReadNextCol()); //是不是debuff
            data.canExistWithSame = CsvUtility.ToBool(rowHelper.ReadNextCol()); //相同buff是否可并行存在
            data.canAddLayer = CsvUtility.ToBool(rowHelper.ReadNextCol()); //是否可叠层
            data.maxLayer = CsvUtility.ToInt(rowHelper.ReadNextCol()); //最高层数
            data.canDispel = CsvUtility.ToBool(rowHelper.ReadNextCol()); //是否可驱散
            data.isUpdateDuringRepeatCast = CsvUtility.ToBool(rowHelper.ReadNextCol()); //重复释放是否刷新持续时间
            return data;
        }
    }
}