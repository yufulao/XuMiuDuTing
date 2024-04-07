// ******************************************************************
//       /\ /|       @file       CfgCharacterVoice.cs
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
    public class RowCfgCharacterVoice
    {
        public string key; //key
        public string voiceDisplayName; //语音显示的名字
        public string sfxKey; //音效配置key
        public string voiceText; //语音文本
    }

    public class CfgCharacterVoice
    {
        private readonly Dictionary<string, RowCfgCharacterVoice> _configs = new Dictionary<string, RowCfgCharacterVoice>(); //cfgId映射row
        public RowCfgCharacterVoice this[string key] => _configs.ContainsKey(key) ? _configs[key] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{key}");
        public RowCfgCharacterVoice this[int id] => _configs.ContainsKey(id.ToString()) ? _configs[id.ToString()] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{id}");
        public List<RowCfgCharacterVoice> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgCharacterVoice Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgCharacterVoice Find(string i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgCharacterVoice.txt", 3);
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
        private RowCfgCharacterVoice ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 4)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgCharacterVoice();
            var rowHelper = new RowHelper(col);
            data.key = CsvUtility.ToString(rowHelper.ReadNextCol()); //key
            data.voiceDisplayName = CsvUtility.ToString(rowHelper.ReadNextCol()); //语音显示的名字
            data.sfxKey = CsvUtility.ToString(rowHelper.ReadNextCol()); //音效配置key
            data.voiceText = CsvUtility.ToString(rowHelper.ReadNextCol()); //语音文本
            return data;
        }
    }
}