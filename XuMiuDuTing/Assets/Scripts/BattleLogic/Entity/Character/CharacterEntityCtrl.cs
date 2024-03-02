using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yu;

public class CharacterEntityCtrl : BattleEntityCtrl
{
    private CharacterEntityModel _model;
    private CharacterEntityView _view;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="characterName"></param>
    /// <param name="infoItem"></param>
    /// <param name="entityHud"></param>
    public void Init(string characterName, CharacterInfoItem infoItem, EntityHUD entityHud)
    {
        isEnemy = false;
        _model = new CharacterEntityModel();
        _view = GetComponent<CharacterEntityView>();
        _model.Init(characterName);
        _view.Init(characterName, infoItem, entityHud);
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
    public CharacterInfoItem GetInfoItem()
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
    /// 获取Bp指
    /// </summary>
    /// <returns></returns>
    public int GetBp()
    {
        return _model.bp;
    }
    
    /// <summary>
    /// 获取BpPreview值
    /// </summary>
    /// <returns></returns>
    public int GetBpPreview()
    {
        return _model.bpPreview;
    }
    
    /// <summary>
    /// 修改brave次数
    /// </summary>
    public void SetBpPreview(int bpPreview)
    {
        _model.bpPreview = bpPreview;
    }

    /// <summary>
    /// 修改bp值
    /// </summary>
    /// <param name="bpAddValue"></param>
    public void UpdateBp(int bpAddValue)
    {
        _model.bp += bpAddValue;
    }

    /// <summary>
    /// 修改brave次数
    /// </summary>
    public int GetBraveCount()
    {
        return _model.braveCount;
    }

    /// <summary>
    /// 修改brave次数
    /// </summary>
    public void SetBraveCount(int braveCount)
    {
        _model.braveCount += braveCount;
    }

    /// <summary>
    /// 获取characterName
    /// </summary>
    /// <returns></returns>
    public string GetCharacterName()
    {
        return _model.characterName;
    }
    
    /// <summary>
    /// 获取mp值
    /// </summary>
    /// <returns></returns>
    public override int GetMp()
    {
        return _model.mp;
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
    }

    /// <summary>
    /// 初始化infoItem
    /// </summary>
    public void RefreshInfoItem()
    {
        var infoItem = _view.infoItem;
        infoItem.RefreshCharacterName(_model.characterName);
        infoItem.RefreshHp(_model.hp, _model.maxHp);
        infoItem.RefreshMp(_model.mp);
        infoItem.RefreshBp(_model.bp);
        infoItem.SetActiveObjReadyTip(false);
    }
}