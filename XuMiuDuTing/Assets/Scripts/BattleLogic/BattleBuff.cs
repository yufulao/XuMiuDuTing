using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rabi;
using UnityEngine;

namespace Yu
{
    public partial class BattleManager
    {
        /// <summary>
        /// 添加buff时执行的效果
        /// </summary>
        /// <param name="buffName"></param>
        /// <param name="caster"></param>
        /// <param name="target"></param>
        /// <param name="roundDuring"></param>
        /// <param name="layer"></param>
        /// <param name="buffValues"></param>
        /// <returns></returns>
        private IEnumerator AddBuff(string buffName, BattleEntityCtrl caster, BattleEntityCtrl target, int roundDuring, int layer, params object[] buffValues)
        {
            var buffInfo = new BuffInfo()
            {
                buffName = buffName,
                caster = caster,
                target = target,
                layer = layer,
                roundDuring = roundDuring,
                buffValues = buffValues,
            };
            //添加buff的叠层逻辑
            var sameBuffInfoList = CheckBuff(target, buffInfo.buffName);
            var rowCfgBuff = buffInfo.RowCfgBuff;
            if (sameBuffInfoList.Count > 0) //有同名BuffObj
            {
                var sameBuffInfo = sameBuffInfoList[0];
                if (rowCfgBuff.canExistWithSame) //相同buff可以并行存在
                {
                    AddBuffObj(target, buffInfo);
                    yield return StartCoroutine(DoAddBuffEffect(buffInfo));
                    yield break;
                }

                if (rowCfgBuff.canAddLayer) //相同buff不可并行存在，可以叠层
                {
                    UpdateLayer(sameBuffInfo, buffInfo);
                    if (rowCfgBuff.isUpdateDuringRepeatCast) //相同buff不可并行存在，可以叠层，重复添加刷新during
                    {
                        sameBuffInfo.roundDuring = buffInfo.roundDuring;
                        yield return StartCoroutine(DoAddBuffEffect(sameBuffInfo));
                        yield break;
                    }

                    //相同buff不可并行存在，可以叠层，重复添加不刷新during
                    yield return StartCoroutine(DoAddBuffEffect(sameBuffInfo));
                }

                //相同buff不可并行存在，不可以叠层，重复添加刷新during
                // if (rowCfgBuff.isUpdateDuringRepeatCast) 
                // {
                sameBuffInfo.roundDuring = buffInfo.roundDuring;
                yield break;
                //}

                //相同buff不可并行存在，不可以叠层，重复添加不刷新during
            }

            //没有同名buffObj
            AddBuffObj(target, buffInfo);
            StartCoroutine(DoAddBuffEffect(buffInfo));
        }

        /// <summary>
        /// 添加buffObj
        /// </summary>
        /// <param name="target"></param>
        /// <param name="buffInfo"></param>
        private void AddBuffObj(BattleEntityCtrl target, BuffInfo buffInfo)
        {
            var rowCfgBuff = buffInfo.RowCfgBuff;
            var infoItem = target.GetInfoItem();
            var buffItem = Instantiate(AssetManager.Instance.LoadAsset<GameObject>(ConfigManager.Instance.cfgUICommon["BuffItem"].path)).GetComponent<BuffItem>();
            buffItem.Init(buffInfo);
            buffItem.SetBuffItemOnPointEnter(OpenBuffDescribe);
            buffItem.SetBuffItemOnPointExit(CloseBuffDescribe);
            buffItem.Refresh();

            if (rowCfgBuff.isDebuff)
            {
                buffItem.gameObject.transform.SetParent(infoItem.buffsContainer.Find("Debuffs"));
                target.debuffItemList.Add(buffItem);
                return;
            }

            buffItem.gameObject.transform.SetParent(infoItem.buffsContainer.Find("Buffs"));
            target.buffItemList.Add(buffItem);
        }

        /// <summary>
        /// 修改层数
        /// </summary>
        /// <param name="mainBuffInfo">需要修改的buffInfo</param>
        /// <param name="useBuffInfo">传入的参照buffInfo</param>
        private static void UpdateLayer(BuffInfo mainBuffInfo, BuffInfo useBuffInfo)
        {
            var mainRowCfgBuff = mainBuffInfo.RowCfgBuff;
            mainBuffInfo.layer += useBuffInfo.layer;
            if (mainBuffInfo.layer > mainRowCfgBuff.maxLayer)
            {
                mainBuffInfo.layer = mainRowCfgBuff.maxLayer;
            }
        }

        /// <summary>
        /// 执行Buff效果
        /// </summary>
        /// <param name="buffInfo"></param>
        /// <returns></returns>
        private static IEnumerator DoAddBuffEffect(BuffInfo buffInfo)
        {
            var target = buffInfo.target;

            switch (buffInfo.buffName)
            {
                case "星":
                    //降低防御力
                    var defendDecrease = (int) (target.GetDefend() * 0.2f);
                    target.SetDefendAddon("星", -defendDecrease);
                    buffInfo.buffStringParams = new object[1] {"20%"};
                    break;
                case "激情":
                    buffInfo.buffStringParams = new object[1] {"20%"};
                    break;
                case "攻击力提升":
                    target.UpdateDamage(int.Parse(buffInfo.buffValues[0].ToString()));
                    break;
                case "攻击力下降":
                    target.UpdateDamage(-int.Parse(buffInfo.buffValues[0].ToString()));
                    break;
                case "受伤加重":
                    target.UpdateHurtRate(float.Parse(buffInfo.buffValues[0].ToString()));
                    break;
                case "虚缪":
                    buffInfo.buffStringParams = new object[1] {"70%"};
                    break;
                case "苦旅":
                    buffInfo.buffStringParams = new object[1] {"2"};
                    break;
                case "忍耐":
                    target.UpdateHurtRate(-float.Parse(buffInfo.buffValues[0].ToString()));
                    break;
                case "反击架势":
                    buffInfo.buffStringParams = new object[1] {"70%"};
                    break;
                case "被掩护":
                    target.SetHurtToEntity(buffInfo.buffValues[0] as BattleEntityCtrl); //设置伤害转移的目标
                    buffInfo.buffStringParams = new object[1] {buffInfo.buffValues[0]};
                    break;
                case "增生":
                    buffInfo.buffStringParams = new object[1] {"30%"};
                    break;
                case "返生":
                    buffInfo.buffStringParams = new object[1] {"1"};
                    break;
                case "眩晕":
                    break;
                case "燃烬":
                    break;
                default:
                    Debug.LogError("没有添加这个buff的效果" + buffInfo.buffName);
                    break;
            }

            yield return null;
        }

        /// <summary>
        /// 移除buff时执行的效果
        /// </summary>
        /// <param name="buffInfo"></param>
        private static void DoRemoveBuffEffect(BuffInfo buffInfo)
        {
            var target = buffInfo.target;
            var caster = buffInfo.caster;

            for (var i = 0; i < buffInfo.layer; i++)
            {
                switch (buffInfo.buffName)
                {
                    case "星":
                        target.SetDefendAddon("星", 0);
                        break;
                    case "激情":
                        break;
                    case "攻击力提升":
                        target.UpdateDamage(-int.Parse(buffInfo.buffValues[0].ToString()));
                        break;
                    case "攻击力下降":
                        target.UpdateDamage(int.Parse(buffInfo.buffValues[0].ToString()));
                        break;
                    case "受伤加重":
                        target.UpdateHurtRate(-float.Parse(buffInfo.buffValues[0].ToString()));
                        break;
                    case "虚缪":
                        break;
                    case "苦旅":
                        break;
                    case "忍耐":
                        target.UpdateHurtRate(float.Parse(buffInfo.buffValues[0].ToString()));
                        break;
                    case "反击架势":
                        break;
                    case "被掩护":
                        target.SetHurtToEntity(null);
                        break;
                    case "增生":
                        break;
                    case "返生":
                        target.SetHp(1);
                        break;
                    case "眩晕":
                        break;
                    case "燃烬":
                        break;
                    default:
                        Debug.LogError("没有添加这个buff的效果" + buffInfo.buffName);
                        break;
                }
            }
        }

        /// <summary>
        /// 角色减少bp时检测buff激情
        /// </summary>
        /// <param name="characterEntity"></param>
        /// <param name="bpDecrease"></param>
        public void CheckDecreaseBp(CharacterEntityCtrl characterEntity, int bpDecrease)
        {
            if (CheckBuff(characterEntity, "激情").Count <= 0)
            {
                return;
            }

            for (var j = 0; j < bpDecrease; j++)
            {
                var damageAddon = (int) (characterEntity.GetRowCfgCharacter().damage * 0.2f);
                StartCoroutine(AddBuff("攻击力提升", characterEntity, characterEntity, 1, damageAddon));
            }
        }

        /// <summary>
        /// 清除所有buff
        /// </summary>
        /// <param name="entity"></param>
        private void ClearBuff(BattleEntityCtrl entity)
        {
            //todo 对象池处理
            foreach (var debuffObj in entity.debuffItemList)
            {
                Destroy(debuffObj.gameObject);
            }

            foreach (var buffObj in entity.buffItemList)
            {
                Destroy(buffObj.gameObject);
            }

            entity.debuffItemList.Clear();
            entity.buffItemList.Clear();
        }

        /// <summary>
        /// 每回合buff的during减一
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private List<BuffInfo> DoBuffEffectAtRoundEnd(BattleEntityCtrl target)
        {
            var cleanBuffInfoList = new List<BuffInfo>();
            foreach (var buffItem in target.buffItemList.Concat(target.buffItemList))
            {
                var buffInfo = buffItem.GetBuffInfo();
                buffInfo.roundDuring--;
                if (buffInfo.roundDuring > 0)
                {
                    continue;
                }

                cleanBuffInfoList.Add(buffInfo);
                switch (buffInfo.RowCfgBuff.isDebuff)
                {
                    case true:
                        target.debuffItemList.Remove(buffItem);
                        break;
                    case false:
                        target.buffItemList.Remove(buffItem);
                        break;
                }

                Destroy(buffItem.gameObject);
            }

            foreach (var buffInfo in cleanBuffInfoList)
            {
                DoRemoveBuffEffect(buffInfo);
            }

            return cleanBuffInfoList;
        }

        /// <summary>
        /// 通过buffInfo获取buffObj
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="buffInfo"></param>
        /// <returns></returns>
        private static BuffItem GetBuffObjByBuffInfo(BattleEntityCtrl entity, BuffInfo buffInfo)
        {
            var rowCfgBuff = buffInfo.RowCfgBuff;
            if (rowCfgBuff.isDebuff)
            {
                foreach (var debuffItem in entity.debuffItemList)
                {
                    if (debuffItem.GetBuffInfo() == buffInfo)
                    {
                        return debuffItem;
                    }
                }
            }

            foreach (var debuffItem in entity.buffItemList)
            {
                if (debuffItem.GetBuffInfo() == buffInfo)
                {
                    return debuffItem;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取指定的buff
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="buffName"></param>
        /// <returns></returns>
        private static IEnumerable<BuffItem> GetBuffObjByBuffName(BattleEntityCtrl entity, string buffName)
        {
            var buffItemList = new List<BuffItem>();
            foreach (var buffItem in entity.debuffItemList.Concat(entity.buffItemList))
            {
                if (buffItem.GetBuffInfo().buffName.Equals(buffName))
                {
                    buffItemList.Add(buffItem);
                }
            }

            return buffItemList;
        }

        /// <summary>
        /// 强制移除指定的最先的buff
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="buffName"></param>
        private List<BuffInfo> ForceRemoveBuffNoCallback(BattleEntityCtrl entity, string buffName)
        {
            var buffInfoList = new List<BuffInfo>();
            var buffItemList = GetBuffObjByBuffName(entity, buffName);
            foreach (var buffItem in buffItemList)
            {
                buffInfoList.Add(ForceRemoveBuffNoCallback(entity, buffItem));
            }

            return buffInfoList;
        }

        /// <summary>
        /// 强制移除指定的最先的buff
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="buffInfo"></param>
        private static BuffInfo ForceRemoveBuffNoCallback(BattleEntityCtrl entity, BuffInfo buffInfo)
        {
            var buffItem = GetBuffObjByBuffInfo(entity, buffInfo);
            return ForceRemoveBuffNoCallback(entity, buffItem);
        }

        /// <summary>
        /// 强制移除指定的最先的buff
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="buffItem"></param>
        private static BuffInfo ForceRemoveBuffNoCallback(BattleEntityCtrl entity, BuffItem buffItem)
        {
            if (!buffItem)
            {
                return null;
            }

            var buffInfo = buffItem.GetBuffInfo();
            if (buffInfo.RowCfgBuff.isDebuff)
            {
                entity.debuffItemList.Remove(buffItem);
                Destroy(buffItem.gameObject);
                return buffInfo;
            }

            entity.buffItemList.Remove(buffItem);
            Destroy(buffItem.gameObject);
            return buffInfo;
        }

        /// <summary>
        /// 根据buffName来获取buff
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="buffName"></param>
        /// <returns></returns>
        public static List<BuffInfo> CheckBuff(BattleEntityCtrl entity, string buffName)
        {
            var tempBuffInfoList = new List<BuffInfo>();
            var rowCfgBuff = ConfigManager.Instance.cfgBuff[buffName];
            switch (rowCfgBuff.isDebuff)
            {
                case true:
                    foreach (var buffItem in entity.debuffItemList)
                    {
                        var buffInfo = buffItem.GetBuffInfo();
                        if (buffInfo.buffName.Equals(buffName))
                        {
                            tempBuffInfoList.Add(buffInfo);
                        }
                    }

                    break;
                case false:
                    foreach (var buffItem in entity.buffItemList)
                    {
                        var buffInfo = buffItem.GetBuffInfo();
                        if (buffInfo.buffName.Equals(buffName))
                        {
                            tempBuffInfoList.Add(buffInfo);
                        }
                    }

                    break;
            }

            return tempBuffInfoList;
        }

        /// <summary>
        /// 检查是否有buff或debuff
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isDebuff"></param>
        /// <returns></returns>
        private static List<BuffInfo> CheckHadBuffOrDebuff(BattleEntityCtrl entity, bool isDebuff)
        {
            var resultList = new List<BuffInfo>();
            if (isDebuff)
            {
                foreach (var buffItem in entity.debuffItemList)
                {
                    resultList.Add(buffItem.GetBuffInfo());
                }

                return resultList;
            }

            foreach (var buffItem in entity.buffItemList)
            {
                resultList.Add(buffItem.GetBuffInfo());
            }

            return resultList;
        }

        /// <summary>
        /// 驱散debuff
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private List<BuffInfo> DisperseDebuff(BattleEntityCtrl entity)
        {
            var debuffInfoList = CheckHadBuffOrDebuff(entity, true);
            foreach (var debuffInfo in debuffInfoList)
            {
                if (!debuffInfo.RowCfgBuff.canDispel)
                {
                    continue;
                }

                var buffInfo = ForceRemoveBuffNoCallback(entity, debuffInfo);
                DoRemoveBuffEffect(buffInfo);
            }

            return debuffInfoList;
        }
    }
}