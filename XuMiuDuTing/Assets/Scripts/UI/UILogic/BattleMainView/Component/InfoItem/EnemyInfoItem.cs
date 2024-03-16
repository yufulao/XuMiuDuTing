using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Yu
{
    public class EnemyInfoItem : BaseInfoItem
    {
        public Animator animatorSelectedBg;

        /// <summary>
        /// 死亡时更新
        /// </summary>
        public override void RefreshOnDie()
        {
            RefreshHp(0);
        }
        
        /// <summary>
        /// 没死亡时更新
        /// </summary>
        public override void RefreshOnNotDie(int hp,int mp,int bp)
        {
            RefreshHp(hp);
        }
        
        /// <summary>
        /// 刷新生命值显示
        /// </summary>
        public override void RefreshHp(int hp,int maxHp)
        {
            sliderHp.maxValue = maxHp;
            RefreshHp(hp);
        }
        
        /// <summary>
        /// 刷新生命值显示
        /// </summary>
        public override void RefreshHp(int hp)
        {
            sliderHp.value = hp;
            textHp.text = hp.ToString();
        }
    }
}