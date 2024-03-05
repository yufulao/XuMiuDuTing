using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;

namespace Yu
{
    public class EnemyEntityModel
    {
        public string enemyName;
        public int hp;
        public int maxHp;
        public int mp;

        public bool isDie;

        //private int hatred;
        public int damage;
        public float damageRate;
        public int defend;
        public readonly Dictionary<string,int> defendAddonDic=new Dictionary<string, int>();
        public float hurtRate;
        public int speed;
        public BattleEntityCtrl hurtToEntity; //被掩护，转移伤害给的目标
        public bool isSelected;
        public RowCfgEnemy rowCfgEnemy;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(string enemyNameT)
        {
            enemyName = enemyNameT;
            rowCfgEnemy = ConfigManager.Instance.cfgEnemy[enemyName];
            ResetAllValue();
        }

        /// <summary>
        /// 重置所有数值
        /// </summary>
        private void ResetAllValue()
        {
            if (rowCfgEnemy == null)
            {
                return;
            }

            maxHp = rowCfgEnemy.maxHp;
            hp = maxHp;
            mp = 0;
            isDie = false;
            damage = rowCfgEnemy.damage;
            damageRate = rowCfgEnemy.damageRate;
            defend = rowCfgEnemy.defend;
            hurtRate = rowCfgEnemy.hurtRate;
            speed = rowCfgEnemy.speed;
            isSelected = false;
        }
    }
}
