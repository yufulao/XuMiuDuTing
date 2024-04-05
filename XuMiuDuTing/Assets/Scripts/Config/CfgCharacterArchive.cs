// ******************************************************************
//       /\ /|       @file       CfgCharacterArchive.cs
//       \ V/        @brief      excel数据解析(由python自动生成) ./xlsx//Character.xlsx
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
    public class RowCfgCharacterArchive
    {
        public string key; //key
        public string archiveName; //档案名
        public string archiveText; //档案内容
    }

    public class CfgCharacterArchive
    {
        private readonly Dictionary<string, RowCfgCharacterArchive> _configs = new Dictionary<string, RowCfgCharacterArchive>(); //cfgId映射row
        public RowCfgCharacterArchive this[string key] => _configs.ContainsKey(key) ? _configs[key] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{key}");
        public RowCfgCharacterArchive this[int id] => _configs.ContainsKey(id.ToString()) ? _configs[id.ToString()] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{id}");
        public List<RowCfgCharacterArchive> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgCharacterArchive Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgCharacterArchive Find(string i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgCharacterArchive.txt", 3);
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
        private RowCfgCharacterArchive ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 3)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgCharacterArchive();
            var rowHelper = new RowHelper(col);
            data.key = CsvUtility.ToString(rowHelper.ReadNextCol()); //key
            data.archiveName = CsvUtility.ToString(rowHelper.ReadNextCol()); //档案名
            data.archiveText = CsvUtility.ToString(rowHelper.ReadNextCol()); //档案内容
            return data;
        }
    }
}