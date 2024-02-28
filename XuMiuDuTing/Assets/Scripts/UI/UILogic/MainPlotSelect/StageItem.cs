using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
    /// <param name="stageData"></param>
    /// <param name="isSelect"></param>
    public void Refresh(StageData stageData,bool isSelect=false)
    {
        _stageName = stageData.stageName;
        textStageName.text = _stageName;
        gameObject.SetActive(stageData.isUnlock);
        objPassTip.SetActive(stageData.isPass);
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
