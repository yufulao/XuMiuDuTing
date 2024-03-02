using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Yu
{
    public class SkillSelectItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textSkillName;
        [SerializeField] private Button btnPress;

        private UnityAction<string> _btnOnClickPress;
        private string _skillName;

        private void Start()
        {
            btnPress.onClick.AddListener(BtnOnClickPress);
        }

        /// <summary>
        /// 刷新技能名字
        /// </summary>
        /// <param name="skillName"></param>
        public void RefreshSkillName(string skillName)
        {
            _skillName = skillName;
            textSkillName.text = _skillName;
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
    }
}
