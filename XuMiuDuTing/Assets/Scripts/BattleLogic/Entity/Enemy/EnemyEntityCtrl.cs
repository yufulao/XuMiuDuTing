using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yu;

public class EnemyEntityCtrl : BattleEntityCtrl
{
    private EnemyEntityModel _model;
    private EnemyEntityView _view;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="enemyName"></param>
    /// <param name="infoItem"></param>
    /// <param name="entityHud"></param>
    public void Init(string enemyName,EnemyInfoItem infoItem,EntityHUD entityHud)
    {
        isEnemy = true;
        _model = new EnemyEntityModel();
        _view = GetComponent<EnemyEntityView>();
        _model.Init(enemyName);
        _view.Init(enemyName,infoItem,entityHud);
        RefreshInfoItem();
        RefreshEntityHud();
    }

    /// <summary>
    /// 启用隐藏spine描边
    /// </summary>
    public void SetEntitySpineOutlineActive(bool active)
    {
        _view.outlineComponent.enabled = active;
    }
    
    /// <summary>
    /// 获取entityHud
    /// </summary>
    /// <returns></returns>
    public override EntityHUD GetEntityHud()
    {
        return _view.entityHud;
    }
    
    /// <summary>
    /// 获取InfoItem
    /// </summary>
    /// <returns></returns>
    public EnemyInfoItem GetInfoItem()
    {
        return _view.infoItem;
    }
    
    /// <summary>
    /// 是否死亡
    /// </summary>
    /// <returns></returns>
    public override bool IsDie()
    {
        return _model.isDie;
    }
    
    /// <summary>
    /// 获取hp值
    /// </summary>
    /// <returns></returns>
    public override int GetHp()
    {
        return _model.hp;
    }
    
    /// <summary>
    /// 获取speed值
    /// </summary>
    /// <returns></returns>
    public override int GetSpeed()
    {
        return _model.speed;
    }

    /// <summary>
    /// 初始化EntityHud
    /// </summary>
    public void RefreshEntityHud()
    {
        var entityHud = _view.entityHud;
        entityHud.sliderHp.maxValue = _model.maxHp;
        entityHud.sliderHp.value = _model.hp;
        entityHud.toggleSelect.isOn = _model.isSelected;
    }

    /// <summary>
    /// 初始化infoItem
    /// </summary>
    public void RefreshInfoItem()
    {
        var infoItem = _view.infoItem;
        infoItem.RefreshHp(_model.hp,_model.maxHp);
    }
}
