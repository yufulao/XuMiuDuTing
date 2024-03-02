using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;
using UnityEngine.Events;

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
    public float hurtRate;
    public int speed;
    public BattleEntityCtrl hurtToEntity; //被掩护，转移伤害给的目标

    private RowCfgCharacter _rowCfgCharacter;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="characterNameT"></param>
    public void Init(string characterNameT)
    {
        characterName = characterNameT;
        _rowCfgCharacter = ConfigManager.Instance.cfgCharacter[characterName];
        ResetAllValue();
    }

    /// <summary>
    /// 重置所有数值
    /// </summary>
    public void ResetAllValue()
    {
        if (_rowCfgCharacter==null)
        {
            return;
        }
        bp = 0;
        bpPreview = 0;
        braveCount = 0;
        maxHp = _rowCfgCharacter.maxHp;
        hp = maxHp;
        mp = 0;
        isDie = false;
        damage = _rowCfgCharacter.damage;
        damageRate = _rowCfgCharacter.damageRate;
        defend = _rowCfgCharacter.defend;
        hurtRate = _rowCfgCharacter.hurtRate;
        speed = _rowCfgCharacter.speed;
    }
}