using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Yu
{
    public class SkillItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textSkillName;
        [SerializeField] private Button btnPress;
        [SerializeField] private EventTrigger eventTrigger;
        [SerializeField] private Animator animatorHang;
        
        public RowCfgSkill RowCfgSkill => ConfigManager.Instance.cfgSkill[_skillName];
        private UnityAction<string> _btnOnClickPress;
        private string _skillName;
        
        private UnityAction<SkillItem, BaseEventData> _onPointEnter;
        private UnityAction<SkillItem, BaseEventData> _onPointExit;

        /// <summary>
        /// 注册监听
        /// </summary>
        private void Start()
        {
            btnPress.onClick.AddListener(BtnOnClickPress);
            Utils.AddTriggersListener(eventTrigger, EventTriggerType.PointerEnter, OnPointEnter);
            Utils.AddTriggersListener(eventTrigger, EventTriggerType.PointerExit, OnPointExit);
        }

        /// <summary>
        /// 刷新技能名字
        /// </summary>
        /// <param name="skillName"></param>
        /// <param name="index"></param>
        public void RefreshSkillName(string skillName,int index)
        {
            animatorHang.SetBool("selected",false);
            _skillName = skillName;
            textSkillName.text ="no"+index.ToString()+" "+ _skillName;
        }

        /// <summary>
        /// 设置SkillItem点击回调
        /// </summary>
        /// <param name="func"></param>
        public void SetSelectItemOnClick(UnityAction<string> func)
        {
            _btnOnClickPress = func;
        }

        /// <summary>
        /// SkillItem点击时
        /// </summary>
        private void BtnOnClickPress()
        {
            _btnOnClickPress?.Invoke(_skillName);
        }
        
        /// <summary>
        /// 设置buffItem悬停时的回调
        /// </summary>
        public void SetSkillSelectItemOnPointEnter(UnityAction<SkillItem, BaseEventData> onPointEnter)
        {
            _onPointEnter = onPointEnter;
        }
        
        /// <summary>
        /// 设置buffItem离开悬停时的回调
        /// </summary>
        public void SetSkillSelectItemOnPointExit(UnityAction<SkillItem, BaseEventData> onPointExit)
        {
            _onPointExit = onPointExit;
        }
        
        /// <summary>
        /// 当鼠标悬停时
        /// </summary>
        /// <param name="baseEventData"></param>
        private void OnPointEnter(BaseEventData baseEventData)
        {
            animatorHang.SetBool("selected",true);
            _onPointEnter?.Invoke(this,baseEventData);
        }

        /// <summary>
        /// 当鼠标离开悬停时
        /// </summary>
        /// <param name="baseEventData"></param>
        private void OnPointExit(BaseEventData baseEventData)
        {
            animatorHang.SetBool("selected",false);
            _onPointExit?.Invoke(this,baseEventData);
        }
    }
}
