using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;

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
    public float hurtRate;
    public int speed;
    public BattleEntityCtrl hurtToEntity; //被掩护，转移伤害给的目标
    public bool isSelected;

    private RowCfgEnemy _rowCfgEnemy;
    
    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(string enemyNameT)
    {
        enemyName = enemyNameT;
        _rowCfgEnemy = ConfigManager.Instance.cfgEnemy[enemyName];
        ResetAllValue();
        // todo 鼠标进入时infoItem显示选中状态
        //selectToggle.transform.GetComponent<UIButton>().RegisterUiButton(entityInfoUi.transform.Find("SelectSpineBg").GetComponent<Animator>());
    }
    
    /// <summary>
    /// 重置所有数值
    /// </summary>
    public void ResetAllValue()
    {
        if (_rowCfgEnemy==null)
        {
            return;
        }
        maxHp = _rowCfgEnemy.maxHp;
        hp = maxHp;
        mp = 0;
        isDie = false;
        damage = _rowCfgEnemy.damage;
        damageRate = _rowCfgEnemy.damageRate;
        defend = _rowCfgEnemy.defend;
        hurtRate = _rowCfgEnemy.hurtRate;
        speed = _rowCfgEnemy.speed;
        isSelected = false;
    }
}
