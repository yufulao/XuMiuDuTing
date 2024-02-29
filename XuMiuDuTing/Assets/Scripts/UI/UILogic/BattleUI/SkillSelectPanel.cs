using System;
using System.Collections.Generic;
using DG.Tweening;
using Rabi;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using Yu;

namespace Yu
{
    public class SkillSelectPanel : MonoBehaviour
{
    [SerializeField] private GameObject objMainPanel;
    [SerializeField] private Button btnBackMask;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private List<SkillSelectItem> skillSelectItemList = new List<SkillSelectItem>();

    private string _characterName;
    private SkillData _skillData;
    private UnityAction<string> _btnSkillItemOnClick;

    private void Init(UnityAction<string> onSkillItemClick)
    {
        _skillData = SaveManager.GetT("SkillData", new SkillData());
        btnBackMask.onClick.AddListener(Close);
        foreach (var skillSelectItem in skillSelectItemList)
        {
            skillSelectItem.SetSelectItemOnClick(onSkillItemClick);
        }
    }

    /// <summary>
    /// 打开技能选择面板
    /// </summary>
    /// <param name="characterName"></param>
    public void Open(string characterName)
    {
        canvasGroup.alpha = 0f;
        objMainPanel.SetActive(true);
        canvasGroup.DOFade(1, 0.2f);
        _characterName = characterName;
        var skillNameList = ConfigManager.Instance.cfgCharacter[_characterName].skillNameList;
        if (skillNameList.Count>skillSelectItemList.Count)
        {
            Debug.LogError("技能obj列表不够用");
            return; 
        }
        for (var i = 0; i < skillNameList.Count; i++)
        {
            var skillName = skillNameList[i];
            if (_skillData.allSkill[skillName].isUnlock)
            {
                skillSelectItemList[i].RefreshSkillName(skillName);
                skillSelectItemList[i].gameObject.SetActive(true);
                continue;
            }
            skillSelectItemList[i].gameObject.SetActive(false);
        }

        for (var i = skillNameList.Count; i < skillSelectItemList.Count; i++)
        {
            skillSelectItemList[i].gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// 关闭技能选择面板
    /// </summary>
    private void Close()
    {
        canvasGroup.DOFade(0, 0.2f);
        objMainPanel.SetActive(false);
    }
}
}