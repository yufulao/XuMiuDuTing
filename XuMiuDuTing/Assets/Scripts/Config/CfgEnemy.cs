// ******************************************************************
//       /\ /|       @file       CfgEnemy.cs
//       \ V/        @brief      excel数据解析(由python自动生成) ./xlsx//Enemy.xlsx
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
    public class RowCfgEnemy
    {
        public string key; //key
        public string entityObjPath; //实体路径
        public List<string> skillNameList; //技能名列表
        public string uniqueSkillName; //必杀技名
        public int maxHp; //最大生命值
        public int damage; //攻击值
        public float damageRate; //攻击乘率
        public int defend; //防御值
        public float hurtRate; //受击乘率
        public int speed; //速度
    }

    public class CfgEnemy
    {
        private readonly Dictionary<string, RowCfgEnemy> _configs = new Dictionary<string, RowCfgEnemy>(); //cfgId映射row
        public RowCfgEnemy this[string key] => _configs.ContainsKey(key) ? _configs[key] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{key}");
        public RowCfgEnemy this[int id] => _configs.ContainsKey(id.ToString()) ? _configs[id.ToString()] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{id}");
        public List<RowCfgEnemy> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgEnemy Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgEnemy Find(string i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgEnemy.txt", 3);
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
        private RowCfgEnemy ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 10)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgEnemy();
            var rowHelper = new RowHelper(col);
            data.key = CsvUtility.ToString(rowHelper.ReadNextCol()); //key
            data.entityObjPath = CsvUtility.ToString(rowHelper.ReadNextCol()); //实体路径
            data.skillNameList = CsvUtility.ToList<string>(rowHelper.ReadNextCol()); //技能名列表
            data.uniqueSkillName = CsvUtility.ToString(rowHelper.ReadNextCol()); //必杀技名
            data.maxHp = CsvUtility.ToInt(rowHelper.ReadNextCol()); //最大生命值
            data.damage = CsvUtility.ToInt(rowHelper.ReadNextCol()); //攻击值
            data.damageRate = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //攻击乘率
            data.defend = CsvUtility.ToInt(rowHelper.ReadNextCol()); //防御值
            data.hurtRate = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //受击乘率
            data.speed = CsvUtility.ToInt(rowHelper.ReadNextCol()); //速度
            return data;
        }
    }
}