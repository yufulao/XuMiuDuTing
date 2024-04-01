using System.Collections.Generic;
using Rabi;
using UnityEngine;

namespace Yu
{
    public class EnemyEntityCtrl : BattleEntityCtrl
    {
        private EnemyEntityModel _model;
        private EnemyEntityView _view;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="enemyName"></param>
        /// <param name="infoItem"></param>
        /// <param name="entityHud"></param>
        public void Init(string enemyName, EnemyInfoItem infoItem, EntityHUD entityHud)
        {
            isEnemy = true;
            _model = new EnemyEntityModel();
            _view = GetComponent<EnemyEntityView>();
            _model.Init(enemyName);
            _view.Init(enemyName, infoItem, entityHud);
            RefreshInfoItem();
            RefreshEntityHud();
        }

        /// <summary>
        /// ai输入指令
        /// </summary>
        public virtual void SetCommandAI(int currentRound)
        {
            var targetList = BattleManager.Instance.EnemySelectTarget(1);
            //全部普通攻击
            commandList.Add(BattleManager.Instance.EnemyAttack(this, new List<BattleEntityCtrl> {targetList[0]}));
                BattleManager.Instance.AddCommandInfo(new BattleCommandInfo(
                    true, BattleCommandType.Attack, false, 0, new List<BattleEntityCtrl> {targetList[0]}, this));
        }

        /// <summary>
        /// 获取配置数据
        /// </summary>
        public RowCfgEnemy GetRowCfgEnemy()
        {
            return _model.rowCfgEnemy;
        }

        /// <summary>
        /// 获取entityHud
        /// </summary>
        /// <returns></returns>
        public override EntityHUD GetEntityHud()
        {
            return _view.entityHud;
        }

        /// <summary>
        /// 获取InfoItem
        /// </summary>
        /// <returns></returns>
        public EnemyInfoItem GetEnemyInfoItem()
        {
            return _view.infoItem;
        }

        public override BaseInfoItem GetInfoItem()
        {
            return _view.infoItem;
        }

        /// <summary>
        /// 获取characterName
        /// </summary>
        /// <returns></returns>
        public override string GetName()
        {
            return _model.enemyName;
        }

        /// <summary>
        /// 获取mp值
        /// </summary>
        /// <returns></returns>
        public override int GetMp()
        {
            return _model.mp;
        }

        /// <summary>
        /// 修改mp值
        /// </summary>
        /// <param name="mpAddon"></param>
        public override void UpdateMp(int mpAddon)
        {
            _model.mp += mpAddon;
            if (_model.mp > 100)
            {
                _model.mp = 100;
            }
        }

        /// <summary>
        /// 是否死亡
        /// </summary>
        /// <returns></returns>
        public override bool IsDie()
        {
            return _model.isDie;
        }

        /// <summary>
        /// 设置死亡
        /// </summary>
        public override void SetDie(bool isDie)
        {
            _model.isDie = isDie;
        }

        /// <summary>
        /// 设置受击entity转移
        /// </summary>
        /// <param name="target"></param>
        public override void SetHurtToEntity(BattleEntityCtrl target)
        {
            _model.hurtToEntity = target;
        }

        /// <summary>
        /// 获取受击entity转移
        /// </summary>
        public override BattleEntityCtrl GetHurtToEntity()
        {
            return _model.hurtToEntity;
        }

        /// <summary>
        /// 激活描边
        /// </summary>
        /// <returns></returns>
        public override void SetOutlineActive(bool active)
        {
            _view.objOutline.SetActive(active);
        }

        /// <summary>
        /// 获取hp值
        /// </summary>
        /// <returns></returns>
        public override int GetHp()
        {
            return _model.hp;
        }

        /// <summary>
        /// 获取最大生命值
        /// </summary>
        /// <returns></returns>
        public override int GetMaxHp()
        {
            return _model.maxHp;
        }

        /// <summary>
        /// 修改hp值
        /// </summary>
        /// <param name="hpAddon"></param>
        public override void UpdateHp(int hpAddon)
        {
            _model.hp += hpAddon;
            if (_model.hp > _model.maxHp)
            {
                _model.hp = _model.maxHp;
            }

            if (_model.hp < 0)
            {
                _model.hp = 0;
            }
        }

        /// <summary>
        /// 设置hp值
        /// </summary>
        /// <param name="hp"></param>
        public override void SetHp(int hp)
        {
            _model.hp = hp;
            if (_model.hp > _model.maxHp)
            {
                _model.hp = _model.maxHp;
            }

            if (_model.hp < 0)
            {
                _model.hp = 0;
            }
        }

        /// <summary>
        /// 获取防御值
        /// </summary>
        /// <returns></returns>
        public override int GetDefend()
        {
            return _model.defend;
        }

        /// <summary>
        /// 修改防御值
        /// </summary>
        /// <param name="addonType"></param>
        /// <param name="defendAddon"></param>
        public override void SetDefendAddon(string addonType, int defendAddon)
        {
            if (_model.defendAddonDic.ContainsKey(addonType))
            {
                _model.defendAddonDic[addonType] += defendAddon;
                return;
            }

            _model.defendAddonDic.Add(addonType, defendAddon);
        }

        /// <summary>
        /// 获取防御附加值
        /// </summary>
        /// <returns></returns>
        public override int GetDefendAddon()
        {
            var defendAddonSum = 0;
            foreach (var kvp in _model.defendAddonDic)
            {
                defendAddonSum += kvp.Value;
            }

            return defendAddonSum;
        }

        /// <summary>
        /// 获取HurtRate
        /// </summary>
        /// <returns></returns>
        public override float GetHurtRate()
        {
            return _model.hurtRate;
        }

        /// <summary>
        /// 修改受伤乘数
        /// </summary>
        public override void UpdateHurtRate(float hurtRateAddon)
        {
            _model.hurtRate += hurtRateAddon;
            if (_model.hurtRate < 0f)
            {
                _model.hurtRate = 0f;
            }
        }

        /// <summary>
        /// 获取DamageRate
        /// </summary>
        /// <returns></returns>
        public override float GetDamageRate()
        {
            return _model.damageRate;
        }

        /// <summary>
        /// 获取speed值
        /// </summary>
        /// <returns></returns>
        public override int GetSpeed()
        {
            return _model.speed;
        }

        /// <summary>
        /// 获取攻击值
        /// </summary>
        /// <returns></returns>
        public override int GetDamage()
        {
            return _model.damage;
        }

        /// <summary>
        /// 修改攻击力
        /// </summary>
        public override void UpdateDamage(int damageAddon)
        {
            _model.damage += damageAddon;
        }

        /// <summary>
        /// 初始化EntityHud
        /// </summary>
        public void RefreshEntityHud()
        {
            var entityHud = _view.entityHud;
            entityHud.sliderHp.maxValue = _model.maxHp;
            entityHud.sliderHp.value = _model.hp;
            entityHud.toggleSelect.isOn = _model.isSelected;
        }

        /// <summary>
        /// 初始化infoItem
        /// </summary>
        public void RefreshInfoItem()
        {
            var infoItem = _view.infoItem;
            infoItem.RefreshHp(_model.hp, _model.maxHp);
        }
    }
}