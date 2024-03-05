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
        private static void FilterDieSelectEntity(ref List<BattleEntityCtrl> targetList)
        {
            foreach (var target in targetList)
            {
                if (target.IsDie())
                {
                    targetList.Remove(target);
                }
            }
        }

        private SkillSelectInfo AddSkill(SkillSelectInfo skillSelectInfo)
        {
            if (skillSelectInfo.needSelect && skillSelectInfo.selectedEntityList == null)
            {
                Debug.LogError("选择了目标但目标列表为空");
            }

            var selectedEntityList= _model.selectedEntityList;
            var selectedEntity = new List<BattleEntityCtrl>(); //list是引用类型，必须要深拷贝
            foreach (var entity in selectedEntityList)
            {
                selectedEntity.Add(entity);
            }

            var currentCharacter = _model.GetCurrentCharacterEntity();
            var casterName = skillSelectInfo.battleEntity.GetName();
            if (casterName.Equals("维多利亚"))
            {
                switch (skillSelectInfo.skillIndex)
                    {
                        case 0: //反击架势，先制指令
                            skillSelectInfo.bpNeed = 1;
                            skillSelectInfo.battleEntity.commandList.Add(VectoriaSkill1(currentCharacter, skillSelectInfo.bpNeed, selectedEntity));
                            break;
                        case 1:
                            skillSelectInfo.bpNeed = 1;
                            skillSelectInfo.battleEntity.commandList.Add(VectoriaSkill2(currentCharacter, skillSelectInfo.bpNeed, selectedEntity));
                            break;
                        case 2:
                            skillSelectInfo.bpNeed = 1;
                            skillSelectInfo.battleEntity.commandList.Add(VectoriaSkill3(currentCharacter, skillSelectInfo.bpNeed, _model.allEntities));
                            break;
                        case 3:
                            skillSelectInfo.bpNeed = 2;
                            skillSelectInfo.battleEntity.commandList.Add(VectoriaSkill4(currentCharacter, skillSelectInfo.bpNeed, _model.allEntities));
                            break;
                        case 4:
                            skillSelectInfo.bpNeed = 1;
                            skillSelectInfo.battleEntity.commandList.Add(VectoriaSkill5(currentCharacter, skillSelectInfo.bpNeed, selectedEntity));
                            break;
                        case 5:
                            skillSelectInfo.bpNeed = 1;
                            skillSelectInfo.battleEntity.commandList.Add(VectoriaSkill6(currentCharacter, skillSelectInfo.bpNeed));
                            break;
                        case 100:
                            skillSelectInfo.bpNeed = 1;
                            skillSelectInfo.battleEntity.commandList.Add(VectoriaUniqueSkill(currentCharacter, skillSelectInfo.bpNeed, _model.allEntities));
                            break;
                    }
            }

            if (casterName.Equals("维克多"))
            {
                switch (skillSelectInfo.skillIndex)
                    {
                        case 0: //反击架势，先制指令
                            skillSelectInfo.bpNeed = 1;
                            skillSelectInfo.battleEntity.commandList.Add(BattleStartCommandInCommandList(currentCharacter));
                            skillSelectInfo.battleEntity.battleStartCommandList.Add(VectorSkill1(currentCharacter, skillSelectInfo.bpNeed));
                            break;
                        case 1:
                            skillSelectInfo.bpNeed = 1;
                            skillSelectInfo.battleEntity.commandList.Add(VectorSkill2(currentCharacter, skillSelectInfo.bpNeed, selectedEntity));
                            break;
                        case 2:
                            skillSelectInfo.bpNeed = 1;
                            skillSelectInfo.battleEntity.commandList.Add(VectorSkill3(currentCharacter, skillSelectInfo.bpNeed, selectedEntity));
                            break;
                        case 3:
                            skillSelectInfo.bpNeed = 2;
                            skillSelectInfo.battleEntity.commandList.Add(VectorSkill4(currentCharacter, skillSelectInfo.bpNeed));
                            break;
                        case 4:
                            skillSelectInfo.bpNeed = 1;
                            skillSelectInfo.battleEntity.commandList.Add(VectorSkill5(currentCharacter, skillSelectInfo.bpNeed, selectedEntity));
                            break;
                        case 5:
                            skillSelectInfo.bpNeed = 3;
                            skillSelectInfo.battleEntity.commandList.Add(VectorSkill6(currentCharacter, skillSelectInfo.bpNeed, _model.allEntities));
                            break;
                        case 100:
                            skillSelectInfo.bpNeed = 1;
                            skillSelectInfo.battleEntity.commandList.Add(VectorUniqueSkill(currentCharacter, skillSelectInfo.bpNeed, selectedEntity));
                            break;
                    }
            }

            return skillSelectInfo; //引用变量，绝对会在上面的Switch中被修改然后传出
        }

        //Vectoria
        private IEnumerator VectoriaSkill1(CharacterEntityCtrl caster, int bpNeed, List<BattleEntityCtrl> targetList)
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DCharacter, 0f);
            FilterDieSelectEntity(ref targetList);
            if (targetList.Count != 0)
            {
                var damagePoint = caster.GetDamage() * 0.5f;
                foreach (var target in targetList)
                {
                    yield return StartCoroutine(CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f));
                    yield return StartCoroutine(AddBuff("星", caster, target, 2, 1));
                    EntityGetDamage(target, caster, damagePoint);
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
            FilterDieSelectEntity(ref targetList);

            if (targetList.Count != 0)
            {
                foreach (var target in targetList)
                {
                    yield return StartCoroutine(CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f));
                    yield return StartCoroutine(AddBuff("激情", caster, target, 3, 1));
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
            FilterDieSelectEntity(ref targetList);

            if (targetList.Count != 0)
            {
                var damagePointNormal = caster.GetDamage() * 0.7f;
                var damagePointWithBuff = caster.GetDamage() * 2.5f;
                foreach (var target in targetList)
                {
                    if (target.isEnemy)
                    {
                        var damagePointFix = CheckBuff(target, "星").Count > 0 ? damagePointWithBuff : damagePointNormal;

                        yield return StartCoroutine(CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f));
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
            FilterDieSelectEntity(ref targetList);

            if (targetList.Count != 0)
            {
                foreach (var target in targetList)
                {
                    if (target.isEnemy)
                    {
                        yield return StartCoroutine(CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f));
                        yield return StartCoroutine(AddBuff("攻击力下降", caster, target, 3, 1,
                            ConfigManager.Instance.cfgEnemy[target.GetName()].damage * 0.2f));
                        yield return StartCoroutine(AddBuff("受伤加重", caster, target, 3, 1, 0.2f));
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
            FilterDieSelectEntity(ref targetList);

            if (targetList.Count != 0)
            {
                foreach (var target in targetList)
                {
                    yield return StartCoroutine(CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f));

                    List<BuffInfo> disperseDebuffs = DisperseDebuff(target);
                    for (int j = 0; j < disperseDebuffs.Count; j++)
                    {
                        yield return StartCoroutine(AddBuff("虚缪", caster, target, 5, 1, 0f));
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

            yield return StartCoroutine(AddBuff("苦旅", caster, caster, 5, 1));
            yield return StartCoroutine(AddBuff("忍耐", caster, caster, 5, 1, 0.1f));

            CheckDecreaseBp(caster, bpNeed);
            ExecuteCommandList();
            yield return null;
        }

        private IEnumerator VectoriaUniqueSkill(CharacterEntityCtrl caster, int bpNeed, List<BattleEntityCtrl> targetList)
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DCharacter, 0f);
            FilterDieSelectEntity(ref targetList);

            if (targetList.Count != 0)
            {
                var damagePoint = caster.GetDamage() * 1.9f;
                var damageOtherPoint = caster.GetDamage() * 0.7f;
                foreach (var target in targetList)
                {
                    //allEntities[battleId].animator.Play("skill1", 0, 0f);
                    //allEntities[battleId].skillObjAnim.Play("dog_skill_3", 0, 0f);
                    //SystemFacade.instance.PlaySFX("攻击123");
                    //yield return new WaitForSeconds(Utility.GetAnimatorLength(allEntities[battleId].animator, "skill1"));
                    var damageHalf = 1f; //有星buff的伤害减半
                    var entityGetBuffCount = CheckHadBuffOrDebuff(target, false).Count;
                    damageHalf = entityGetBuffCount != 0 ? 0.5f : 1f;
                    yield return StartCoroutine(CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f));
                    EntityGetDamage(target, caster, damagePoint * damageHalf);
                    var entityGetDebuffCount = CheckHadBuffOrDebuff(target, true).Count;
                    for (var j = 0; j < entityGetDebuffCount; j++)
                    {
                        EntityGetDamage(target, caster, damageOtherPoint * damageHalf);
                    }

                    yield return StartCoroutine(AddBuff("星", caster, target, 2, 1));
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
            yield return StartCoroutine(Utils.PlayAnimation(caster.animatorEntity, "skill2"));
            yield return StartCoroutine(Utils.PlayAnimation(caster.animatorBuff, "def_buff"));

            CheckDecreaseBp(caster, bpNeed);
            caster.UpdateBp(1);
            yield return StartCoroutine(AddBuff("反击架势", caster, caster, 1, 1));
            yield return new WaitForSeconds(0.2f);
            ExecuteBattleStartCommandList();
        }

        private IEnumerator VectorSkill2(CharacterEntityCtrl caster, int bpNeed, List<BattleEntityCtrl> targetList)
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DCharacter, 0f);
            FilterDieSelectEntity(ref targetList);

            if (targetList.Count != 0)
            {
                foreach (var target in targetList)
                {
                    SFXManager.Instance.PlaySfx("技能12");
                    yield return StartCoroutine(Utils.PlayAnimation(caster.animatorEntity, "skill2"));
                    yield return StartCoroutine(CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f));
                    yield return StartCoroutine(Utils.PlayAnimation(target.animatorBuff, "def_buff"));
                    yield return StartCoroutine(AddBuff("被掩护", caster, target, 1, 1, caster));
                    yield return new WaitForSeconds(0.2f);
                }

                CheckDecreaseBp(caster, bpNeed);
            }

            ExecuteCommandList();
        }

        private IEnumerator VectorSkill3(CharacterEntityCtrl caster, int bpNeed, List<BattleEntityCtrl> targetList)
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DCharacter, 0f);
            FilterDieSelectEntity(ref targetList);

            if (targetList.Count != 0)
            {
                var damagePoint = caster.GetDamage() * 1.5f;

                foreach (var target in targetList)
                {
                    SFXManager.Instance.PlaySfx("攻击123");
                    yield return StartCoroutine(Utils.PlayAnimation(caster.animatorEntity, "skill1"));

                    yield return StartCoroutine(CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f));
                    yield return StartCoroutine(Utils.PlayAnimation(target.animatorSkill, "dog_skill3"));

                    EntityGetDamage(target, caster, damagePoint);
                    ;
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
            yield return StartCoroutine(Utils.PlayAnimation(caster.animatorEntity, "skill2"));

            yield return StartCoroutine(Utils.PlayAnimation(caster.animatorSkill, "dog_skill4&6"));
            yield return StartCoroutine(AddBuff("增生", caster, caster, 3, 1));
            yield return new WaitForSeconds(0.2f);
            yield return StartCoroutine(Utils.PlayAnimation(caster.animatorBuff, "heal"));
            yield return StartCoroutine(AddBuff("返生", caster, caster, 5, 1));
            yield return new WaitForSeconds(0.2f);

            CheckDecreaseBp(caster, bpNeed);
            ExecuteCommandList();
        }

        private IEnumerator VectorSkill5(CharacterEntityCtrl caster, int bpNeed, List<BattleEntityCtrl> targetList)
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DCharacter, 0f);
            FilterDieSelectEntity(ref targetList);

            if (targetList.Count != 0)
            {
                var hpRate = (float) caster.GetHp() / (float) caster.GetMaxHp();
                var damagePoint = caster.GetDamage() * (-2.5f * hpRate + 3f);
                //Debug.Log(hpRate);
                //Debug.Log(damagePoint);

                foreach (var target in targetList)
                {
                    SFXManager.Instance.PlaySfx("攻击123");
                    yield return StartCoroutine(Utils.PlayAnimation(caster.animatorEntity, "skill1"));

                    yield return StartCoroutine(CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f));
                    yield return StartCoroutine(Utils.PlayAnimation(target.animatorSkill, "dog_skill5"));
                    yield return new WaitForSeconds(0.2f);

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
            FilterDieSelectEntity(ref targetList);

            SFXManager.Instance.PlaySfx("技能12");
            yield return StartCoroutine(Utils.PlayAnimation(caster.animatorEntity, "skill2"));

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
                StartCoroutine(AddBuff("增生", caster, target, 3, 1));
            }

            yield return new WaitForSeconds(Utils.GetAnimatorLength(caster.animatorSkill, "dog_skill4&6"));
            yield return new WaitForSeconds(0.2f);

            CheckDecreaseBp(caster, bpNeed);
            ExecuteCommandList();
        }

        private IEnumerator VectorUniqueSkill(CharacterEntityCtrl caster, int bpNeed, List<BattleEntityCtrl> targetList)
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DCharacter, 0f);
            FilterDieSelectEntity(ref targetList);

            if (targetList.Count != 0)
            {
                var damagePoint = caster.GetDamage() * 5.0f;
                //Debug.Log(damagePoint);

                foreach (var target in targetList)
                {
                    yield return StartCoroutine(CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f));
                    yield return new WaitForSeconds(0.2f);
                    EntityGetDamage(target, caster, damagePoint);
                    yield return StartCoroutine(AddBuff("燃烬", caster, target, 5, 1));
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

            yield return StartCoroutine(Utils.PlayAnimation(caster.animatorEntity, "skill1"));
            yield return StartCoroutine(CameraManager.Instance.MoveObjCameraByEntityIsEnemy(target, 0f));
            EntityGetDamage(target, caster, caster.GetDamage());
            yield return StartCoroutine(Utils.PlayAnimation(target.animatorSkill, "attack_A"));
            yield return StartCoroutine(AddBuff("眩晕", caster, target, 1, 1));
            yield return new WaitForSeconds(0.2f);

            RefreshAllEntityInfoItem();
            ExecuteCommandList();
        }
    }
}