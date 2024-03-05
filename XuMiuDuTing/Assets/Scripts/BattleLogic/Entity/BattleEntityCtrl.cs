using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace Yu
{
    public class BattleEntityCtrl : MonoBehaviour
    {
        [HideInInspector] public bool isEnemy;

        public readonly List<IEnumerator> commandList = new List<IEnumerator>();
        public readonly List<IEnumerator> battleStartCommandList = new List<IEnumerator>();
        [HideInInspector] public List<BuffItem> buffItemList;
        [HideInInspector] public List<BuffItem> debuffItemList;
        public Animator animatorEntity;
        public Animator animatorBuff;
        public Animator animatorSkill;

        public virtual string GetName()
        {
            return null;
        }

        public virtual int GetMp()
        {
            return 0;
        }

        public virtual void UpdateMp(int mpAddon)
        {
        }

        public virtual int GetHp()
        {
            return 0;
        }

        public virtual void UpdateHp(int hpAddon)
        {
        }

        public virtual void SetHp(int hp)
        {
        }

        public virtual int GetDefend()
        {
            return 0;
        }

        public virtual void SetDefendAddon(string addonType, int defendAddon)
        {
        }

        public virtual int GetDefendAddon()
        {
            return 0;
        }

        public virtual float GetHurtRate()
        {
            return 0;
        }

        public virtual void UpdateHurtRate(float hurtRateAddon)
        {
        }

        public virtual float GetDamageRate()
        {
            return 0;
        }

        public virtual int GetSpeed()
        {
            return 0;
        }

        public virtual int GetDamage()
        {
            return 0;
        }

        public virtual void UpdateDamage(int damageAddon)
        {
        }

        public virtual EntityHUD GetEntityHud()
        {
            return null;
        }

        public virtual BaseInfoItem GetInfoItem()
        {
            return null;
        }

        public virtual void SetHurtToEntity(BattleEntityCtrl target)
        {
        }

        public virtual BattleEntityCtrl GetHurtToEntity()
        {
            return null;
        }

        public virtual bool IsDie()
        {
            return false;
        }

        public virtual void SetDie(bool isDie)
        {
        }

        public virtual void SetOutlineActive(bool active)
        {
        }
    }
}