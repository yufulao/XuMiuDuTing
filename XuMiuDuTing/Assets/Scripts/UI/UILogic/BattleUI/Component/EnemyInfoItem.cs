using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Yu
{
    public class EnemyInfoItem : MonoBehaviour
    {
        public Animator animatorSelectedBg;
        [SerializeField] private Slider sliderHp;
        [SerializeField] private TextMeshProUGUI textHp;
        [SerializeField] private Transform buffsContainer;
        // public List<BuffObject> buffs;
        // public List<BuffObject> debuffs;

        /// <summary>
        /// 死亡时更新
        /// </summary>
        public void RefreshOnDie()
        {
            RefreshHp(0);
        }
        
        /// <summary>
        /// 没死亡时更新
        /// </summary>
        public void RefreshOnNotDie(int hp)
        {
            RefreshHp(hp);
        }
        
        /// <summary>
        /// 刷新生命值显示
        /// </summary>
        public void RefreshHp(int hp,int maxHp)
        {
            sliderHp.maxValue = maxHp;
            RefreshHp(hp);
        }
        
        /// <summary>
        /// 刷新生命值显示
        /// </summary>
        public void RefreshHp(int hp)
        {
            sliderHp.value = hp;
            textHp.text = hp.ToString();
        }
    }
}