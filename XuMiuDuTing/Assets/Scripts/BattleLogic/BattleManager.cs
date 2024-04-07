using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Yu
{
    public partial class BattleManager : MonoSingleton<BattleManager>
    {
        public Transform entityObjContainer;
        public List<Transform> characterSpawnPointList = new List<Transform>();
        public List<Transform> enemySpawnPointList = new List<Transform>();

        private BattleMainCtrl _uiCtrl;
        private BattleFsm _fsm;
        private BattleModel _model;

        private void Start()
        {
            InitFsm();
        }

        private void OnDestroy()
        {
            //一定要解除监听，不然重新进入一次战斗，eventManger会调用到上一个battleManager注册的事件，但场景已经销毁，会有一堆东西报空
            EventManager.Instance.RemoveListener(EventName.OnRoundEnd, OnRoundEnd);
        }

        /// <summary>
        /// 进入战斗
        /// </summary>
        /// <param name="rowCfgStage"></param>
        /// <param name="teamArray"></param>
        public void EnterBattle(RowCfgStage rowCfgStage, string[] teamArray)
        {
            _model = new BattleModel();
            _model.SetTeamArray(teamArray);
            _fsm.ChangeFsmState(typeof(BattleInitState), rowCfgStage);
        }

        /// <summary>
        /// 初始化阶段
        /// </summary>
        public IEnumerator OnEnterBattleInitState(RowCfgStage rowCfgStage)
        {
            _model.Init();
            InitUI();
            SpawnEntity();
            _model.SortEntityList();
            ResetCommand();
            EventManager.Instance.AddListener(EventName.OnRoundEnd, OnRoundEnd);
            switch (rowCfgStage.battleBgm.Count)
            {
                case 1:
                    StartCoroutine(BGMManager.Instance.PlayBgmFadeDelay(rowCfgStage.battleBgm[0], 0f, 0.3f, 0f, 1f));
                    break;
                case 2:
                    StartCoroutine(BGMManager.Instance.PlayLoopBgmWithIntro(
                        rowCfgStage.battleBgm[0], rowCfgStage.battleBgm[1], 0f, 0.3f, 0f, 1f));
                    break;
            }

            RefreshAllEntityInfoItem();
            RefreshAllCommandMenu();
            StartCoroutine(AllEntityPlayEnterAnimation());
            yield return new WaitForSeconds(1f);
            UIManager.Instance.CloseWindow("LoadingView");
            _fsm.ChangeFsmState(typeof(CharacterCommandInputState));
        }

        /// <summary>
        /// 角色输入指令阶段
        /// </summary>
        public void OnEnterCharacterCommandInputState()
        {
            SetCommand();
            StartCoroutine(CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DIdleCommand, 0));
        }

        /// <summary>
        /// 敌方输入指令阶段，点击GoBattle摁钮后切换到此state
        /// </summary>
        public void OnEnterEnemyCommandInputState()
        {
            _uiCtrl.view.animatorBtnGoBattle.SetTrigger("close");
            _uiCtrl.view.btnGoBattle.interactable = false;
            _uiCtrl.view.btnUndoCommand.gameObject.SetActive(false);
            _model.SetFirstCanInputCommandEnemy();
            EnemySetCommandAI();
        }

        /// <summary>
        /// 执行指令阶段
        /// </summary>
        public void OnEnterExecutingState()
        {
            StartCoroutine(PrepareAllCommandList());
        }

        /// <summary>
        /// 敌人依据仇恨值随机选取目标
        /// </summary>
        public List<BattleEntityCtrl> EnemySelectTarget(int targetCount, bool onlyCharacter = true)
        {
            //todo model缓存list
            var canSelectEntityList = new List<BattleEntityCtrl>();
            foreach (var entity in _model.allEntities)
            {
                if (entity.isEnemy && onlyCharacter) //只能选取角色
                {
                    continue;
                }

                if (entity.IsDie())
                {
                    continue;
                }

                canSelectEntityList.Add(entity);
            }

            if (canSelectEntityList.Count < targetCount)
            {
                Debug.Log("敌人找不到合适的目标");
                _uiCtrl.view.objMask.SetActive(true); //一般是战斗结束后还点击continueBtn才会报
                return null;
            }

            var canSelectEntityForHatredList = new List<BattleEntityCtrl>();
            foreach (var entity in canSelectEntityList)
            {
                //todo 仇恨设置概率
                // for (var j = 0; j < characterEntityCtrl.hatred; j++)
                // {
                canSelectEntityForHatredList.Add(entity);
                //}
            }
            //Debug.Log(liveCharactersForHatredList.Count);

            var targetList = new List<BattleEntityCtrl>();
            for (var i = 0; i < targetCount; i++)
            {
                targetList.Add(canSelectEntityForHatredList[Random.Range(0, canSelectEntityForHatredList.Count)]);
            }

            return targetList;
        }

        /// <summary>
        /// 加进commandList中占位的
        /// </summary>
        /// <param name="caster"></param>
        /// <returns></returns>
        private IEnumerator CharacterBrave(CharacterEntityCtrl caster)
        {
            yield return null;
            ExecuteCommandList();
        }

        /// <summary>
        /// (先制)防御指令
        /// </summary>
        /// <param name="caster"></param>
        /// <returns></returns>
        private IEnumerator CharacterDefend(CharacterEntityCtrl caster)
        {
            //在RoundListener中注册了事件，检测是否输入default并加回HurtRate
            caster.UpdateHurtRate(-0.3f);
            yield return null;
            ExecuteBattleStartCommandList();
        }

        /// <summary>
        /// Attack指令
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="bpNeed"></param>
        /// <param name="selectEntityList"></param>
        /// <returns></returns>
        private IEnumerator CharacterAttack(CharacterEntityCtrl caster, int bpNeed, List<BattleEntityCtrl> selectEntityList)
        {
            Debug.Log(0);
            float damagePoint = caster.GetDamage();
            foreach (var selectEntity in selectEntityList)
            {
                //Debug.Log(selectEntity[i]);
                if (selectEntity.IsDie())
                {
                    continue;
                }

                yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DCharacter, 0);
                yield return Utils.PlayAnimation(caster.animatorEntity, "attack");
                yield return CameraManager.Instance.MoveObjCameraByEntityIsEnemy(selectEntity, 0);
                yield return new WaitForSeconds(0.2f);
                EntityGetDamage(selectEntity, caster, damagePoint);
                caster.UpdateBp(-bpNeed);
                RefreshAllEntityInfoItem();
                yield return new WaitForSeconds(0.7f);
            }

            ExecuteCommandList();
        }

        /// <summary>
        /// 敌人输入指令的ai
        /// </summary>
        private void EnemySetCommandAI()
        {
            var currentEnemyEntity = _model.GetCurrentEnemyEntity();
            currentEnemyEntity.SetCommandAI(_model.currentRound);
            _model.currentEnemyEntityIndex++;

            if (_model.currentEnemyEntityIndex == _model.allEnemyEntities.Count) //此时currentBallteId是比allEntity的最大index多1
            {
                SetCommandReachEnd();
                return;
            }

            EnemySetCommandAI();
        }

        /// <summary>
        /// 先制指令加进commandList中占位的
        /// </summary>
        /// <param name="caster"></param>
        /// <returns></returns>
        private IEnumerator BattleStartCommandInCommandList(CharacterEntityCtrl caster)
        {
            yield return null;
            ExecuteCommandList();
        }

        /// <summary>
        /// 生成角色和敌人并初始化
        /// </summary>
        private void SpawnEntity()
        {
            //清空敌人和角色entity列表
            _model.characterNumber = 0;
            _model.enemyNumber = 0;
            _model.allEntities.Clear();
            for (var i = 0; i < entityObjContainer.transform.childCount; i++)
            {
                Destroy(entityObjContainer.transform.GetChild(i).gameObject);
            }

            _uiCtrl.ClearAllEntityHud();

            var characterTeam = _model.GetCharacterTeamArray();
            //设置角色
            if (characterSpawnPointList.Count < characterTeam.Length)
            {
                Debug.LogError("队伍角色数量大于可用战斗位置数量");
            }

            for (var i = 0; i < characterTeam.Length; i++)
            {
                var characterName = characterTeam[i];
                if (string.IsNullOrEmpty(characterName))
                {
                    _uiCtrl.view.characterInfoItemList[i].gameObject.SetActive(false);
                    continue;
                }

                _uiCtrl.view.characterInfoItemList[i].gameObject.SetActive(true);
                var rowCfgCharacter = ConfigManager.Instance.cfgCharacter[characterName];
                //生成角色
                var characterObj = Instantiate(AssetManager.Instance.LoadAsset<GameObject>(rowCfgCharacter.entityObjPath), entityObjContainer.transform);
                //设置角色站位
                characterObj.transform.position = characterSpawnPointList[i].position;
                //设置entity属性
                var characterEntity = characterObj.GetComponent<CharacterEntityCtrl>();
                var infoItem = _uiCtrl.view.characterInfoItemList[i];
                var entityHud = Instantiate(AssetManager.Instance.LoadAsset<GameObject>(
                    ConfigManager.Instance.cfgUICommon["EntityHUD"].path), _uiCtrl.view.entityHudContainer).GetComponent<EntityHUD>();
                characterEntity.Init(characterName, infoItem, entityHud);
                //设置HUD跟随组件
                var uiFollowObj = characterEntity.gameObject.AddComponent<UIFollowObj>();
                uiFollowObj.objFollowed = characterObj.transform.Find("HudFollowPoint");
                uiFollowObj.rectFollower = entityHud.GetComponent<RectTransform>();
                uiFollowObj.offset = new Vector2(0f, 15f);
                //一些初始设置
                characterEntity.SetOutlineActive(false);
                //加进entityList
                _model.characterNumber++;
                _model.allEntities.Add(characterEntity);
                _model.allCharacterEntities.Add(characterEntity);
                // characterEntity.UpdateMp(100);
            }

            var enemyTeam = _model.GetEnemyTeam();
            //生成敌人
            if (enemyTeam.Count > enemySpawnPointList.Count)
            {
                Debug.LogError("敌人不够生成位置");
            }

            for (var i = 0; i < enemyTeam.Count; i++)
            {
                var enemyName = enemyTeam[i];
                if (string.IsNullOrEmpty(enemyName))
                {
                    _uiCtrl.view.enemyInfoItemList[i].gameObject.SetActive(false);
                    continue;
                }

                _uiCtrl.view.enemyInfoItemList[i].gameObject.SetActive(true);
                var rowCfgEnemy = ConfigManager.Instance.cfgEnemy[enemyTeam[i]];
                //生成敌人
                var enemyObj = Instantiate(AssetManager.Instance.LoadAsset<GameObject>(rowCfgEnemy.entityObjPath), entityObjContainer.transform);
                //设置敌人站位
                enemyObj.transform.position = enemySpawnPointList[i].position;
                //设置entity属性
                var enemyEntity = enemyObj.GetComponent<EnemyEntityCtrl>();
                var infoItem = _uiCtrl.view.enemyInfoItemList[i];
                var entityHud = Instantiate(AssetManager.Instance.LoadAsset<GameObject>(
                    ConfigManager.Instance.cfgUICommon["EntityHUD"].path), _uiCtrl.view.entityHudContainer).GetComponent<EntityHUD>();
                enemyEntity.Init(enemyName, infoItem, entityHud);
                //设置HUD跟随组件
                var uiFollowObj = enemyEntity.gameObject.AddComponent<UIFollowObj>();
                uiFollowObj.objFollowed = enemyObj.transform.Find("HudFollowPoint");
                uiFollowObj.rectFollower = entityHud.GetComponent<RectTransform>();
                uiFollowObj.offset = new Vector2(0f, 15f);
                //一些初始设置
                enemyEntity.SetOutlineActive(false);
                //加进entityList
                _model.enemyNumber++;
                _model.allEntities.Add(enemyEntity);
                _model.allEnemyEntities.Add(enemyEntity);
            }

            var enemyInfoItemList = _uiCtrl.view.enemyInfoItemList;
            //设置左上角敌人列表
            for (var i = enemyTeam.Count; i < enemyInfoItemList.Count; i++)
            {
                enemyInfoItemList[i].gameObject.SetActive(false);
            }

            InitAimSelectToggleList(); // 初始化entity的复选框
        }

        /// <summary>
        /// 使entity受到攻击
        /// </summary>
        /// <param name="target"></param>
        /// <param name="caster"></param>
        /// <param name="damagePoint"></param>
        /// <param name="damageRateAddition"></param>
        private void EntityGetDamage(BattleEntityCtrl target, BattleEntityCtrl caster, float damagePoint, float damageRateAddition = 0)
        {
            if (target.IsDie()) //如果角色已经死了
            {
                return;
            }

            //Debug.Log(entityGet.characterInfoBtn != null);
            //Debug.Log(entityGet.characterInfoBtn != null && entityGet.characterInfoBtn.CheckBuff(BuffName.被掩护) > 0);
            //Debug.Log(entityGet.hurtToEntity != null);
            if (CheckBuff(target, "被掩护").Count > 0 && target.GetHurtToEntity()) //有掩护方
            {
                var hurtToEntity = target.GetHurtToEntity();
                Debug.Log(target + "的受伤转移给" + hurtToEntity);
                target = hurtToEntity;
            }

            EntityGetDamageNoReDamageBuffCheck(target, caster, damagePoint, damageRateAddition);

            if (CheckBuff(target, "反击架势").Count > 0) //有反击架势
            {
                var reDamagePoint = target.GetDamage() * 0.7f;
                EntityGetDamageNoReDamageBuffCheck(caster, caster, reDamagePoint, damageRateAddition);
            }
        }

        /// <summary>
        /// entity受伤检测buff
        /// </summary>
        /// <param name="target"></param>
        /// <param name="caster"></param>
        /// <param name="damagePoint"></param>
        /// <param name="damageRateAddition"></param>
        private void EntityGetDamageNoReDamageBuffCheck(BattleEntityCtrl target, BattleEntityCtrl caster, float damagePoint, float damageRateAddition)
        {
            target.animatorEntity.Play("damaged");
            var hurtPoint = (int) (Mathf.Clamp((damagePoint - target.GetDefend() - target.GetDefendAddon()), 0f, 1000000f)
                                   * target.GetHurtRate() * (caster.GetDamageRate() + damageRateAddition));
            // Debug.Log("damage=" + damagePoint + ",defend=" + target.GetDefend()+",defendAddon"+target.GetDefendAddon() + ",hurtRate=" + target.GetHurtRate() + 
            //           ",damageRate=" + caster.GetDamageRate() + ",damageRateAddition"+damageRateAddition+"\n" +
            //     "伤害-防御=" + (int)(Mathf.Clamp((damagePoint - target.GetDefend()- target.GetDefendAddon()), 0f, 1000000f)) + "\n" +
            //     "伤害rate*受伤rate=" + target.GetHurtRate() * (caster.GetDamageRate()+damageRateAddition) + "\n" +
            //     "总伤害=" + (int) (Mathf.Clamp((damagePoint - target.GetDefend() - target.GetDefendAddon()), 0f, 1000000f)
            //                     * target.GetHurtRate() * (caster.GetDamageRate() + damageRateAddition)));

            //暂时设定成挨打会加10点MP==================================================================================
            target.UpdateMp(10);
            ForceDecreaseEntityHp(target, caster, hurtPoint);
        }

        /// <summary>
        /// 强制减少hp
        /// </summary>
        /// <param name="target"></param>
        /// <param name="caster"></param>
        /// <param name="hpDecreaseCount"></param>
        private void ForceDecreaseEntityHp(BattleEntityCtrl target, BattleEntityCtrl caster, int hpDecreaseCount)
        {
            var entityHud = target.GetEntityHud();
            var textHurtPoint = entityHud.textHurtPoint;
            textHurtPoint.text = "-" + hpDecreaseCount.ToString();
            Utils.TextFly(textHurtPoint, entityHud.textHurtPointOriginalTransform.position, 10);

            target.UpdateHp(-hpDecreaseCount);

            if (target.GetHp() <= 0)
            {
                var buffInfoList = CheckBuff(target, "返生");
                if (buffInfoList.Count > 0) //有返生buff
                {
                    var buffInfo = buffInfoList[0];
                    ForceRemoveBuffNoCallback(target, buffInfo);
                    target.SetHp(1);
                    RefreshAllEntityInfoItem();
                    return;
                }

                StartCoroutine(EntityDieIEnumerator(target));
            }

            RefreshAllEntityInfoItem(); //更新下方角色信息列表
        }

        /// <summary>
        /// 回复entity生命值前检测buff，不检查buff请直接调用entity.UpdateHp()
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="hpAddValue"></param>
        private void RecoverEntityHpWithBuff(BattleEntityCtrl entity, int hpAddValue)
        {
            if (CheckBuff(entity, "燃烬").Count <= 0)
            {
                var entityHud = entity.GetEntityHud();
                var textHurtPoint = entityHud.textHurtPoint;
                textHurtPoint.text = "+" + hpAddValue.ToString();
                Utils.TextFly(textHurtPoint, entityHud.textHurtPointOriginalTransform.position, 5);
                entity.UpdateHp(hpAddValue);
            }
        }

        /// <summary>
        /// entity死亡
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private IEnumerator EntityDieIEnumerator(BattleEntityCtrl entity)
        {
            entity.SetDie(true);
            entity.SetHp(0);
            yield return Utils.PlayAnimation(entity.animatorEntity, "die");
            ClearBuff(entity);
            yield return new WaitForSeconds(0.5f);
            entity.GetEntityHud().gameObject.SetActive(false);
            entity.gameObject.SetActive(false);
            if (entity.isEnemy)
            {
                _model.enemyNumber--;
                if (CheckGameWinSpecialConditionAfterEnemyDie() || _model.enemyNumber == 0)
                {
                    GameWin();
                }

                yield break;
            }

            _model.characterNumber--;
            if (_model.characterNumber == 0)
            {
                GameLoss();
            }
        }

        /// <summary>
        /// 有敌人死亡时检测战斗胜利的特殊情况
        /// </summary>
        private bool CheckGameWinSpecialConditionAfterEnemyDie()
        {
            //当全部存活的敌人都是isExceptBattleWinCheck时算作战斗胜利
            foreach (var enemyEntity in _model.allEnemyEntities)
            {
                if (!enemyEntity.IsDie() && !enemyEntity.GetRowCfgEnemy().isExceptBattleWinCheck)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 敌人攻击指令
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="targetList"></param>
        /// <returns></returns>
        public IEnumerator EnemyAttack(BattleEntityCtrl caster, IEnumerable<BattleEntityCtrl> targetList)
        {
            if (caster.IsDie())
            {
                ExecuteCommandList();
                yield break;
            }

            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DEnemy, 0f);
            yield return Utils.PlayAnimation(caster.animatorEntity, "attack");

            float damagePoint = caster.GetDamage();
            foreach (var targetEntity in targetList)
            {
                yield return CameraManager.Instance.MoveObjCameraByEntityIsEnemy(targetEntity, 0f);
                EntityGetDamage(targetEntity, caster, damagePoint);
            }

            yield return new WaitForSeconds(0.5f);
            ExecuteCommandList();
        }

        /// <summary>
        /// 回合结束事件
        /// </summary>
        private void OnRoundEnd()
        {
            foreach (var entity in _model.allEntities)
            {
                if (entity.IsDie())
                {
                    continue;
                }

                if (entity.animatorEntity)
                {
                    //恢复动画
                    if (CheckBuff(entity, "不可选中").Count <= 0)
                    {
                        entity.animatorEntity.Play("idle");
                    }

                    entity.animatorEntity.SetBool("default", false);
                    entity.animatorEntity.SetBool("ready", false);
                }

                //buff的during--
                DoBuffEffectAtRoundEnd(entity);
            }

            foreach (var characterEntity in _model.allCharacterEntities)
            {
                if (characterEntity.IsDie())
                {
                    continue;
                }

                characterEntity.SetHadUniqueSkill(characterEntity.GetMp() >= 100);
            }

            //解除防御
            foreach (var commandInfo in _commandInfoList)
            {
                if (commandInfo.commandType != BattleCommandType.Default)
                {
                    continue;
                }

                if (!commandInfo.caster.IsDie())
                {
                    commandInfo.caster.UpdateHurtRate(0.3f);
                }
            }
        }

        /// <summary>
        /// 游戏胜利
        /// </summary>
        private void GameWin()
        {
            StartCoroutine(GameFinishIEnumerator(true));
        }

        /// <summary>
        /// 游戏失败
        /// </summary>
        private void GameLoss()
        {
            StartCoroutine(GameFinishIEnumerator(false));
        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        /// <param name="isWin"></param>
        /// <returns></returns>
        private IEnumerator GameFinishIEnumerator(bool isWin)
        {
            if (!_model.isGaming)
            {
                yield break;
            }

            Debug.Log("战斗" + (isWin ? "胜利" : "失败"));
            _model.isGaming = false;

            yield return new WaitForSeconds(1f);
            if (isWin)
            {
                GameManager.Instance.PassStage();
                ProcedureManager.Instance.EnterNextStageProcedure();
                yield break;
            }

            UIManager.Instance.OpenWindow("DoubleConfirmView", "是否跳过该场战斗？", new UnityAction(() =>
            {
                GameManager.Instance.PassStage();
                ProcedureManager.Instance.EnterNextStageProcedure();
            }), new UnityAction(ProcedureManager.Instance.EndStageProcedure));
        }

        /// <summary>
        /// 所有entity入场动画
        /// </summary>
        /// <returns></returns>
        private IEnumerator AllEntityPlayEnterAnimation()
        {
            foreach (var entity in _model.allEntities)
            {
                StartCoroutine(Utils.PlayAnimation(entity.animatorEntity, "start"));
            }

            yield return new WaitForSeconds(Utils.GetAnimatorLength(_model.allEntities[0].animatorEntity, "start"));
        }

        /// <summary>
        /// 初始化UI
        /// </summary>
        private void InitUI()
        {
            UIManager.Instance.OpenWindow("BattleMainView");
            _uiCtrl = UIManager.Instance.GetCtrl<BattleMainCtrl>("BattleMainView");
        }

        /// <summary>
        /// 初始化状态机
        /// </summary>
        private void InitFsm()
        {
            _fsm = FsmManager.Instance.GetFsm<BattleFsm>();
            _fsm.SetFsm(new Dictionary<Type, IFsmState>()
            {
                {typeof(BattleInitState), new BattleInitState()},
                {typeof(CharacterCommandInputState), new CharacterCommandInputState()},
                {typeof(EnemyCommandInputState), new EnemyCommandInputState()},
                {typeof(ExecutingState), new ExecutingState()}
            });
        }
    }
}