using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private List<SkillItem> skillItemList = new List<SkillItem>();

        private string _characterName;
        private SkillData _skillData;
        private UnityAction<string> _btnSkillItemOnClick;

        public void Init()
        {
            _skillData = SaveManager.GetT("SkillData", new SkillData());
            btnBackMask.onClick.AddListener(Close);
            objMainPanel.SetActive(false);
        }

        /// <summary>
        /// 给SkillSelectItem绑定onClick事件
        /// </summary>
        /// <param name="onSkillItemClick"></param>
        public void SetSkillItemOnClick(UnityAction<string> onSkillItemClick)
        {
            foreach (var skillItem in skillItemList)
            {
                skillItem.SetSelectItemOnClick(onSkillItemClick);
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
            if (skillNameList.Count > skillItemList.Count)
            {
                Debug.LogError("技能obj列表不够用");
                return;
            }

            for (var i = 0; i < skillNameList.Count; i++)
            {
                var skillName = skillNameList[i];
                var rowCfgSkill = ConfigManager.Instance.cfgSkill[skillName];
                if (!_skillData.allSkill.ContainsKey(skillName))
                {
                    //todo 检测所有获得条件，后期添加的技能满足条件都直接解锁
                    _skillData.allSkill.Add(skillName,new SkillDataEntry{isUnlock = rowCfgSkill.unlockDefault});
                }
                if (_skillData.allSkill[skillName].isUnlock)
                {
                    skillItemList[i].RefreshSkillName(skillName, i);
                    skillItemList[i].SetSkillSelectItemOnPointEnter(BattleManager.Instance.OpenSkillDescribe);
                    skillItemList[i].SetSkillSelectItemOnPointExit(BattleManager.Instance.CloseSkillDescribe);
                    skillItemList[i].gameObject.SetActive(true);
                    continue;
                }

                skillItemList[i].gameObject.SetActive(false);
            }
            SaveManager.SetT("SkillData",_skillData);

            for (var i = skillNameList.Count; i < skillItemList.Count; i++)
            {
                skillItemList[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 关闭技能选择面板
        /// </summary>
        public void Close()
        {
            canvasGroup.DOFade(0, 0.2f);
            objMainPanel.SetActive(false);
        }
    }
}