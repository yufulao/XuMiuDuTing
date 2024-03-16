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
        /// 过滤掉死亡的角色
        /// </summary>
        /// <param name="targetList"></param>
        private static void FilterDieSelectEntity(List<BattleEntityCtrl> targetList)
        {
            //1.不能用foreach循环，如果targetList中死了某个，会报错列表被修改，操作被禁止
            //2.用倒序for循环，解决边遍历边修改元素个数
            for (var i = targetList.Count - 1; i >= 0; i--)
            {
                if (targetList[i].IsDie())
                {
                    targetList.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 向skillInfo中添加bpNeed和targetList，并通过skillName向currentCharacter添加command和battleStartCommand
        /// </summary>
        /// <param name="skillInfo"></param>
        private void AddCharacterSkillCommand(SkillInfo skillInfo)
        {
            var rowCfgSkill = skillInfo.RowCfgSkill;
            if (rowCfgSkill.needSelect && skillInfo.targetList == null)
            {
                Debug.LogError("选择了目标但目标列表为空");
                return;
            }

            //todo 协程池处理
            var caster = _model.GetCharacterEntityByBaseEntity(skillInfo.caster);
            IEnumerator command = null;
            IEnumerator battleStartCommand = null;
            switch (skillInfo.skillName)
            {
                case "星光": //反击架势，先制指令
                    command = VectoriaSkill1(caster, rowCfgSkill.bpNeed, skillInfo.targetList);
                    break;
                case "失却的激情":
                    command = VectoriaSkill2(caster, rowCfgSkill.bpNeed, skillInfo.targetList);
                    break;
                case "荒芜星原":
                    command = VectoriaSkill3(caster, rowCfgSkill.bpNeed, _model.allEntities);
                    break;
                case "颓废的智慧":
                    command = VectoriaSkill4(caster, rowCfgSkill.bpNeed, _model.allEntities);
                    break;
                case "虚谬":
                    command = VectoriaSkill5(caster, rowCfgSkill.bpNeed, skillInfo.targetList);
                    break;
                case "循此苦旅":
                    command = VectoriaSkill6(caster, rowCfgSkill.bpNeed);
                    break;
                case "Ad Astra":
                    command = VectoriaUniqueSkill(caster, rowCfgSkill.bpNeed, _model.allEntities);
                    break;
                case "反击架势": //反击架势，先制指令
                    command = BattleStartCommandInCommandList(caster);
                    battleStartCommand = VectorSkill1(caster, rowCfgSkill.bpNeed);
                    break;
                case "掩护":
                    command = VectorSkill2(caster, rowCfgSkill.bpNeed, skillInfo.targetList);
                    break;
                case "灼烧印记":
                    command = VectorSkill3(caster, rowCfgSkill.bpNeed, skillInfo.targetList);
                    break;
                case "不死花":
                    command = VectorSkill4(caster, rowCfgSkill.bpNeed);
                    break;
                case "绝境的火焰":
                    command = VectorSkill5(caster, rowCfgSkill.bpNeed, skillInfo.targetList);
                    break;
                case "绽放":
                    command = VectorSkill6(caster, rowCfgSkill.bpNeed, _model.allEntities);
                    break;
                case "迷惘燃烬":
                    command = VectorUniqueSkill(caster, rowCfgSkill.bpNeed, skillInfo.targetList);
                    break;
            }

            if (command != null)
            {
                caster.commandList.Add(command);
                _commandInfoList.Add(new BattleCommandInfo(false, BattleCommandType.Skill, rowCfgSkill.isBattleStartCommand, rowCfgSkill.bpNeed, skillInfo.targetList, caster));
            }

            if (battleStartCommand != null)
            {
                caster.battleStartCommandList.Add(battleStartCommand);
            }
        }

        //Vectoria
        private IEnumerator VectoriaSkill1(CharacterEntityCtrl caster, int bpNeed, List<BattleEntityCtrl> targetList)
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DCharacter, 0f);
            yield return new WaitForSeconds(0.2f);
            FilterDieSelectEntity(targetList);
            if (targetList.Count != 0)
            {
                var damagePoint = caster.GetDamage() * 0.5f;
                foreach (var target in targetList)
                {
                    yield return CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f);
                    AddBuff("星", caster, target, 2, 1);
                    EntityGetDamage(target, caster, damagePoint);
                    yield return new WaitForSeconds(0.2f);
                }
                
                CheckDecreaseBp(caster, bpNeed);
                RefreshAllEntityInfoItem();
            }

            ExecuteCommandList();
            yield return null;
        }

        private IEnumerator VectoriaSkill2(CharacterEntityCtrl caster, int bpNeed, List<BattleEntityCtrl> targetList)
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DCharacter, 0f);
            FilterDieSelectEntity(targetList);

            if (targetList.Count != 0)
            {
                foreach (var target in targetList)
                {
                    yield return CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f);
                    AddBuff("激情", caster, target, 3, 1);
                }

                CheckDecreaseBp(caster, bpNeed);
                RefreshAllEntityInfoItem();
            }

            ExecuteCommandList();
            yield return null;
        }

        private IEnumerator VectoriaSkill3(CharacterEntityCtrl caster, int bpNeed, List<BattleEntityCtrl> targetList)
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DCharacter, 0f);
            FilterDieSelectEntity(targetList);

            if (targetList.Count != 0)
            {
                var damagePointNormal = caster.GetDamage() * 0.7f;
                var damagePointWithBuff = caster.GetDamage() * 2.5f;
                foreach (var target in targetList)
                {
                    if (target.isEnemy)
                    {
                        var damagePointFix = CheckBuff(target, "星").Count > 0 ? damagePointWithBuff : damagePointNormal;

                        yield return CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f);
                        yield return new WaitForSeconds(0.2f);
                        //todo 敌人类型
                        // if (target.enemyDataEntry.enemyType == EnemyType.大型敌人)
                        // {
                        //     battleSystem.EntityGetDamage(target, caster, damagePointFix, 0.4f);
                        // }
                        // else
                        // {
                        //     battleSystem.EntityGetDamage(target, caster, damagePointFix);
                        // }
                    }
                }

                CheckDecreaseBp(caster, bpNeed);
                RefreshAllEntityInfoItem();
            }

            ExecuteCommandList();
            yield return null;
        }

        private IEnumerator VectoriaSkill4(CharacterEntityCtrl caster, int bpNeed, List<BattleEntityCtrl> targetList)
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DCharacter, 0f);
            FilterDieSelectEntity(targetList);

            if (targetList.Count != 0)
            {
                foreach (var target in targetList)
                {
                    if (target.isEnemy)
                    {
                        yield return CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f);
                        AddBuff("攻击力下降", caster, target, 3, 1, ConfigManager.Instance.cfgEnemy[target.GetName()].damage * 0.2f);
                        AddBuff("受伤加重", caster, target, 3, 1, 0.2f);
                    }
                }

                CheckDecreaseBp(caster, bpNeed);
                RefreshAllEntityInfoItem();
            }

            ExecuteCommandList();
            yield return null;
        }

        private IEnumerator VectoriaSkill5(CharacterEntityCtrl caster, int bpNeed, List<BattleEntityCtrl> targetList)
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DCharacter, 0f);
            FilterDieSelectEntity(targetList);

            if (targetList.Count != 0)
            {
                foreach (var target in targetList)
                {
                    yield return CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f);

                    var disperseDebuffs = DisperseDebuff(target);
                    for (var j = 0; j < disperseDebuffs.Count; j++)
                    {
                        AddBuff("虚缪", caster, target, 5, 1, 0f);
                    }
                }

                CheckDecreaseBp(caster, bpNeed);
                RefreshAllEntityInfoItem();
            }

            ExecuteCommandList();
            yield return null;
        }

        private IEnumerator VectoriaSkill6(CharacterEntityCtrl caster, int bpNeed)
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DCharacter, 0f);

            AddBuff("苦旅", caster, caster, 5, 1);
            AddBuff("忍耐", caster, caster, 5, 1, 0.1f);

            CheckDecreaseBp(caster, bpNeed);
            ExecuteCommandList();
        }

        private IEnumerator VectoriaUniqueSkill(CharacterEntityCtrl caster, int bpNeed, List<BattleEntityCtrl> targetList)
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DCharacter, 0f);
            FilterDieSelectEntity(targetList);

            if (targetList.Count != 0)
            {
                caster.animatorSkill.Play("dog_skill_3", 0, 0f);
                SFXManager.Instance.PlaySfx("攻击123");
                yield return Utils.PlayAnimation(caster.animatorEntity, "skill1");
                yield return CameraManager.Instance.MoveObjCameraByEntityIsEnemy(targetList[0], 0f);
                yield return new WaitForSeconds(0.2f);

                var damagePoint = caster.GetDamage() * 1.9f;
                var damageOtherPoint = caster.GetDamage() * 0.7f;
                foreach (var target in targetList)
                {
                    var damageHalf = 1f; //有星buff的伤害减半
                    var entityGetBuffCount = CheckHadBuffOrDebuff(target, false).Count;
                    damageHalf = entityGetBuffCount != 0 ? 0.5f : 1f;
                    EntityGetDamage(target, caster, damagePoint * damageHalf);
                    var entityGetDebuffCount = CheckHadBuffOrDebuff(target, true).Count;
                    for (var j = 0; j < entityGetDebuffCount; j++)
                    {
                        EntityGetDamage(target, caster, damageOtherPoint * damageHalf);
                    }

                    AddBuff("星", caster, target, 2, 1);
                    CheckDecreaseBp(caster, bpNeed);
                    caster.UpdateMp(-100);
                    RefreshAllEntityInfoItem();
                }
            }

            ExecuteCommandList();
            yield return null;
        }

        //Vector
        private IEnumerator VectorSkill1(CharacterEntityCtrl caster, int bpNeed) //先制指令
        {
            SFXManager.Instance.PlaySfx("攻击123");
            yield return Utils.PlayAnimation(caster.animatorEntity, "skill2");
            StartCoroutine(Utils.PlayAnimation(caster.animatorBuff, "def_buff"));

            CheckDecreaseBp(caster, bpNeed);
            caster.UpdateBp(1);
            AddBuff("反击架势", caster, caster, 1, 1);
            ExecuteBattleStartCommandList();
        }

        private IEnumerator VectorSkill2(CharacterEntityCtrl caster, int bpNeed, List<BattleEntityCtrl> targetList)
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DCharacter, 0f);
            FilterDieSelectEntity(targetList);

            if (targetList.Count != 0)
            {
                yield return Utils.PlayAnimation(caster.animatorEntity, "skill2");
                foreach (var target in targetList)
                {
                    SFXManager.Instance.PlaySfx("技能12");
                    yield return StartCoroutine(CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f));
                    yield return Utils.PlayAnimation(target.animatorBuff, "def_buff");
                    AddBuff("被掩护", caster, target, 1, 1, caster);
                }

                CheckDecreaseBp(caster, bpNeed);
            }

            ExecuteCommandList();
        }

        private IEnumerator VectorSkill3(CharacterEntityCtrl caster, int bpNeed, List<BattleEntityCtrl> targetList)
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DCharacter, 0f);
            FilterDieSelectEntity(targetList);

            if (targetList.Count != 0)
            {
                var damagePoint = caster.GetDamage() * 1.5f;
                SFXManager.Instance.PlaySfx("攻击123");
                yield return Utils.PlayAnimation(caster.animatorEntity, "skill1");
                foreach (var target in targetList)
                {
                    yield return CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f);
                    StartCoroutine(Utils.PlayAnimation(target.animatorSkill, "dog_skill3"));
                    yield return new WaitForSeconds(0.2f);
                    EntityGetDamage(target, caster, damagePoint);
                }

                //todo 仇恨
                //caster.hatred++;
                ForceDecreaseEntityHp(caster, caster, (int) (caster.GetMaxHp() * 0.1f));

                CheckDecreaseBp(caster, bpNeed);
            }

            ExecuteCommandList();
        }

        private IEnumerator VectorSkill4(CharacterEntityCtrl caster, int bpNeed)
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DCharacter, 0f);

            SFXManager.Instance.PlaySfx("技能12");
            yield return Utils.PlayAnimation(caster.animatorEntity, "skill2");
            StartCoroutine(Utils.PlayAnimation(caster.animatorSkill, "dog_skill4&6"));
            AddBuff("增生", caster, caster, 3, 1);
            yield return Utils.PlayAnimation(caster.animatorBuff, "heal");
            AddBuff("返生", caster, caster, 5, 1);
            yield return new WaitForSeconds(0.2f);

            CheckDecreaseBp(caster, bpNeed);
            ExecuteCommandList();
        }

        private IEnumerator VectorSkill5(CharacterEntityCtrl caster, int bpNeed, List<BattleEntityCtrl> targetList)
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DCharacter, 0f);
            FilterDieSelectEntity(targetList);

            if (targetList.Count != 0)
            {
                var hpRate = (float) caster.GetHp() / (float) caster.GetMaxHp();
                var damagePoint = caster.GetDamage() * (-2.5f * hpRate + 3f);
                //Debug.Log(hpRate);
                //Debug.Log(damagePoint);
                SFXManager.Instance.PlaySfx("攻击123");
                yield return Utils.PlayAnimation(caster.animatorEntity, "skill1");
                foreach (var target in targetList)
                {
                    yield return CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f);
                    yield return new WaitForSeconds(0.2f);
                    StartCoroutine(Utils.PlayAnimation(target.animatorSkill, "dog_skill5"));
                    EntityGetDamage(target, caster, damagePoint);
                }

                ForceDecreaseEntityHp(caster, caster, (int) (caster.GetMaxHp() * 0.1f));
                CheckDecreaseBp(caster, bpNeed);
            }

            ExecuteCommandList();
        }

        private IEnumerator VectorSkill6(CharacterEntityCtrl caster, int bpNeed, List<BattleEntityCtrl> targetList)
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DCharacter, 0f);
            FilterDieSelectEntity(targetList);

            SFXManager.Instance.PlaySfx("技能12");
            yield return Utils.PlayAnimation(caster.animatorEntity, "skill2");

            foreach (var target in targetList)
            {
                StartCoroutine(Utils.PlayAnimation(target.animatorBuff, "heal"));
                RecoverEntityHpWithBuff(target, (int) (caster.GetMaxHp() * 0.7f));
            }

            yield return new WaitForSeconds(Utils.GetAnimatorLength(caster.animatorBuff, "heal"));
            yield return new WaitForSeconds(0.2f);

            foreach (var target in targetList)
            {
                StartCoroutine(Utils.PlayAnimation(target.animatorSkill, "dog_skill4&6"));
                AddBuff("增生", caster, target, 3, 1);
            }

            yield return new WaitForSeconds(Utils.GetAnimatorLength(caster.animatorSkill, "dog_skill4&6"));

            CheckDecreaseBp(caster, bpNeed);
            ExecuteCommandList();
        }

        private IEnumerator VectorUniqueSkill(CharacterEntityCtrl caster, int bpNeed, List<BattleEntityCtrl> targetList)
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DCharacter, 0f);
            FilterDieSelectEntity(targetList);

            if (targetList.Count != 0)
            {
                var damagePoint = caster.GetDamage() * 5.0f;
                //Debug.Log(damagePoint);

                foreach (var target in targetList)
                {
                    yield return CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f);
                    yield return new WaitForSeconds(0.2f);
                    EntityGetDamage(target, caster, damagePoint);
                    AddBuff("燃烬", caster, target, 5, 1);
                }

                CheckDecreaseBp(caster, bpNeed);
                caster.UpdateMp(-100);
            }

            ExecuteCommandList();
        }

        //EnemyA
        private IEnumerator EnemyASkill1(EnemyEntityCtrl caster, BattleEntityCtrl target)
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DEnemy, 0f);

            yield return Utils.PlayAnimation(caster.animatorEntity, "skill1");
            yield return CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f);
            yield return new WaitForSeconds(0.2f);
            EntityGetDamage(target, caster, caster.GetDamage());
            yield return Utils.PlayAnimation(target.animatorSkill, "attack_A");
            AddBuff("眩晕", caster, target, 1, 1);

            RefreshAllEntityInfoItem();
            ExecuteCommandList();
        }
    }
}