using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Yu;

public class StageItem : MonoBehaviour
{
    [SerializeField] private Toggle toggleSelect;
    [SerializeField] private TextMeshProUGUI textStageName;
    [SerializeField] private GameObject objPassTip;
    [SerializeField] private GameObject objSelectMask;

    private string _stageName;
    private UnityAction<string> _btnOnClickSelect;

    private void Start()
    {
        toggleSelect.onValueChanged.AddListener(ToggleOnValueChangeSelect);
    }

    private void OnDestroy()
    {
        toggleSelect.onValueChanged.RemoveAllListeners();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="stageDataEntry"></param>
    /// <param name="isSelect"></param>
    public void Refresh(StageDataEntry stageDataEntry,bool isSelect=false)
    {
        _stageName = stageDataEntry.stageName;
        textStageName.text = _stageName;
        gameObject.SetActive(stageDataEntry.isUnlock);
        objPassTip.SetActive(stageDataEntry.isPass);
        toggleSelect.isOn = isSelect;
        objSelectMask.SetActive(isSelect);
    }

    /// <summary>
    /// 设置SelectBtn的OnClick事件
    /// </summary>
    /// <param name="func"></param>
    public void SetToggleSelectOnClick(UnityAction<string> func)
    {
        _btnOnClickSelect = func;
    }

    /// <summary>
    /// BtnSelect被点击
    /// </summary>
    private void ToggleOnValueChangeSelect(bool isOn)
    {
        objSelectMask.SetActive(isOn);
        if (isOn)
        {
            _btnOnClickSelect?.Invoke(_stageName);
        }
    }
}
