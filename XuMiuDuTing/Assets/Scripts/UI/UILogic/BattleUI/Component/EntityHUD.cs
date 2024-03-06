using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Yu;

public class EntityHUD : MonoBehaviour
{
    //todo HUD移出BattleMainView，独立作为一个canvas
    public UIHangAnimationComponent uiHangAnimationComponent;
    public Slider sliderHp;
    public Toggle toggleSelect;
    public TextMeshProUGUI textHurtPoint;

    /// <summary>
    /// 设置selectToggle的回调
    /// </summary>
    /// <param name="func"></param>
    public void SetToggleSelectOnValueChange(UnityAction<bool> func)
    {
        toggleSelect.onValueChanged.RemoveAllListeners();
        toggleSelect.onValueChanged.AddListener(func);
    }
}
