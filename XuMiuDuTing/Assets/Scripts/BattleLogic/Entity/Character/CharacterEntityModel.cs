using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;
using UnityEngine.Events;

namespace Yu
{
    public class CharacterEntityModel
    {
        public string characterName;
        public int bp;
        public int bpPreview;
        public int braveCount;
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
        public bool hadUniqueSkill;
        public BattleEntityCtrl hurtToEntity; //被掩护，转移伤害给的目标
        public RowCfgCharacter rowCfgCharacter;
    
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="characterNameT"></param>
        public void Init(string characterNameT)
        {
            characterName = characterNameT;
            rowCfgCharacter = ConfigManager.Instance.cfgCharacter[characterName];
            ResetAllValue();
        }
    
        /// <summary>
        /// 重置所有数值
        /// </summary>
        private void ResetAllValue()
        {
            if (rowCfgCharacter==null)
            {
                return;
            }
            bp = 0;
            bpPreview = 0;
            braveCount = 0;
            maxHp = rowCfgCharacter.maxHp;
            hp = maxHp;
            mp = 0;
            isDie = false;
            damage = rowCfgCharacter.damage;
            damageRate = rowCfgCharacter.damageRate;
            defend = rowCfgCharacter.defend;
            hurtRate = rowCfgCharacter.hurtRate;
            speed = rowCfgCharacter.speed;
        }
    }
}