using System.Collections;
using System.Collections.Generic;
using Rabi;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Yu
{
    public class BaseInfoItem : MonoBehaviour
    {
        [SerializeField] protected Slider sliderHp;
        [SerializeField] protected TextMeshProUGUI textHp;
        [SerializeField] public Transform buffsContainer;

        /// <summary>
        /// 死亡时更新
        /// </summary>
        public virtual void RefreshOnDie()
        {
        }

        /// <summary>
        /// 没死亡时更新
        /// </summary>
        public virtual void RefreshOnNotDie(int hp,int mp,int bp)
        {
        }

        /// <summary>
        /// 刷新生命值显示
        /// </summary>
        public virtual void RefreshHp(int hp, int maxHp)
        {
        }

        /// <summary>
        /// 刷新生命值显示
        /// </summary>
        public virtual void RefreshHp(int hp)
        {
        }
    }
}