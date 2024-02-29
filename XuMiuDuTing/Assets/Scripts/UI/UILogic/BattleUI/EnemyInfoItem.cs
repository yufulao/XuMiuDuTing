using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Yu
{
    public class EnemyInfoItem : MonoBehaviour
    {
        [SerializeField] private GameObject objSelectedBg;
        [SerializeField] private Slider sliderHp;
        [SerializeField] private TextMeshProUGUI textHp;
        [SerializeField] private Transform buffsContainer;

        [SerializeField] private Animator selectedBgAnimator;
        // public List<BuffObject> buffs;
        // public List<BuffObject> debuffs;

        /// <summary>
        /// 刷新生命值显示
        /// </summary>
        public void RefreshHp(int hp,int maxHp)
        {
            sliderHp.maxValue = maxHp;
            sliderHp.value = hp;
            textHp.text = hp.ToString();
        }
    
        /// <summary>
        /// 被选择时
        /// </summary>
        public void EnterSelect()
        {
            selectedBgAnimator.SetTrigger("selectedBgOpen");
        }

        /// <summary>
        /// 取消选择时
        /// </summary>
        public void QuitSelect()
        {
            selectedBgAnimator.SetTrigger("selectedBgClose");
        }

        /// <summary>
        /// 没有被选择时
        /// </summary>
        public void NoSelected()
        {
            selectedBgAnimator.Play("idle");
        }
    }
}