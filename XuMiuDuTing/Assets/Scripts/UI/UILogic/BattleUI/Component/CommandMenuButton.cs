using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Yu;

public class CommandMenuButton : Button
{
    private readonly Color _disableColor= new Color(1f, 1f, 1f, 0.3f);
    private readonly Color _enableColor= new Color(1f, 1f, 1f, 1f);
    
    /// <summary>
    /// 设置不可点击
    /// </summary>
    public void SetClickDisable()
    {
        image.color = _disableColor;
        onClick.RemoveAllListeners();
        onClick.AddListener(ClickDisableButton);
    }
    
    /// <summary>
    /// 设置可以点击
    /// </summary>
    /// <param name="func"></param>
    public void SetClickEnable(UnityAction func)
    {
        image.color = _enableColor;
        onClick.RemoveAllListeners();
        onClick.AddListener(func);
    }

    /// <summary>
    /// 点击不可点击状态时
    /// </summary>
    private static void ClickDisableButton()
    {
        //SFXManager.Instance.PlaySfx("error不可用");
    }
}
