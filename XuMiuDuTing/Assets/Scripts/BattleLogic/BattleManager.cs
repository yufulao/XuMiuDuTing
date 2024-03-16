using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Rabi;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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

        /// <summary>
        /// 进入战斗
        /// </summary>
        /// <param name="teamArray"></param>
        public void EnterBattle(string[] teamArray)
        {
            _model = new BattleModel();
            _model.SetTeamArray(teamArray);
            _fsm.ChangeFsmState(typeof(BattleInitState));
        }
        
        /// <summary>
        /// 初始化阶段
        /// </summary>
        public void OnEnterBattleInitState()
        {
            _model.Init();
            InitUI();
            SpawnEntity();
            _model.SortEntityList();
            ResetCommand();
            EventManager.Instance.AddListener(EventName.OnRoundEnd, OnRoundEnd);
            StartCoroutine(BGMManager.Instance.PlayLoopBgmWithIntro("普通战斗a", "普通战斗b", 0f, 0.3f, 0f, 1f));
            RefreshAllEntityInfoItem();
            RefreshAllCommandMenu();
            StartCoroutine(AllEntityPlayEnterAnimation());
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
            caster.SetDefendAddon("Default", 500); //在RoundListener中注册了事件，检测是否输入default并减去defend
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
        private IEnumerator CharacterAttack(CharacterEntityCtrl caster, int bpNeed, IEnumerable<BattleEntityCtrl> selectEntityList)
        {
            float damagePoint = caster.GetDamage();
            //Debug.Log(selectEntity.Count);
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
                CheckDecreaseBp(caster, bpNeed);
                RefreshAllEntityInfoItem();
            }

            ExecuteCommandList();
        }

        /// <summary>
        /// 敌人输入指令的ai
        /// </summary>
        private void EnemySetCommandAI()
        {
            var liveCharacters = new List<CharacterEntityCtrl>();
            var currentEnemyEntity = _model.GetCurrentEnemyEntity();
            foreach (var characterEntityCtrl in _model.allCharacterEntities)
            {
                if (characterEntityCtrl.IsDie())
                {
                    continue;
                }

                liveCharacters.Add(characterEntityCtrl);
            }

            if (liveCharacters.Count == 0)
            {
                Debug.LogError("敌人找不到没死的角色");
            }

            //按hatred设定选取概率
            var liveCharactersForHatredList = new List<CharacterEntityCtrl>();
            foreach (var characterEntityCtrl in liveCharacters)
            {
                //todo 仇恨设置概率
                // for (var j = 0; j < characterEntityCtrl.hatred; j++)
                // {
                liveCharactersForHatredList.Add(characterEntityCtrl);
                //}
            }

            //Debug.Log(liveCharactersForHatredList.Count);
            var target = liveCharactersForHatredList[Random.Range(0, liveCharactersForHatredList.Count)];

            //四回合输入一次技能1
            if (_model.currentRound % 4 == 0)
            {
                currentEnemyEntity.commandList.Add(EnemyASkill1(currentEnemyEntity, target));
                _commandInfoList.Add(new BattleCommandInfo(true, BattleCommandType.Skill, false, 0, new List<BattleEntityCtrl> {target}, currentEnemyEntity));
            }
            else
            {
                currentEnemyEntity.commandList.Add(EnemyAttack(currentEnemyEntity, new List<BattleEntityCtrl> {target}));
                _commandInfoList.Add(new BattleCommandInfo(true, BattleCommandType.Attack, false, 0, new List<BattleEntityCtrl> {target}, currentEnemyEntity));
            }

            // Debug.Log("实体" + currentBattleId + "输入了指令" + currentCharacter.entityCommandList[currentCharacter.entityCommandList.Count - 1] + "，commandInfo的数量为" +
            //           _commandInfoList.Count);

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
            //Debug.Log("damage=" + damagePoint + ",defand=" + entityGet.defend + ",hurtRate=" + entityGet.hurtRate + ",damageRate=" + caster.GetDamage()Rate + "\n" +
            //    "伤害-防御=" + (int)(Mathf.Clamp((damagePoint - entityGet.defend), 0f, 1000000f)) + "\n" +
            //    "伤害rate*受伤rate=" + entityGet.hurtRate * caster.GetDamage()Rate + "\n" +
            //    "总伤害=" + (int)(Mathf.Clamp((damagePoint - entityGet.defend), 0f, 1000000f) * entityGet.hurtRate * caster.GetDamage()Rate));
            var textHurtPoint = target.GetEntityHud().textHurtPoint;
            textHurtPoint.text = hurtPoint.ToString();
            Utils.TextFly(textHurtPoint, textHurtPoint.gameObject.transform.position);

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
            target.UpdateHp(-hpDecreaseCount);

            if (target.GetHp() <= 0)
            {
                var buffInfoList = CheckBuff(target, "返生");
                if (buffInfoList.Count > 0) //有返生buff
                {
                    var buffInfo = buffInfoList[0];
                    ForceRemoveBuffNoCallback(target, buffInfo);
                    DoRemoveBuffEffect(buffInfo);
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
                entity.UpdateHp(hpAddValue);
            }
        }

        /// <summary>
        /// entity死亡
        /// </summary>
        /// <param name="entityEntity"></param>
        /// <returns></returns>
        private IEnumerator EntityDieIEnumerator(BattleEntityCtrl entityEntity)
        {
            entityEntity.SetDie(true);
            entityEntity.SetHp(0);
            entityEntity.GetEntityHud().gameObject.SetActive(false);
            entityEntity.animatorEntity.Play("die");

            yield return new WaitForSeconds(1f);
            entityEntity.gameObject.SetActive(false);
            if (entityEntity.isEnemy)
            {
                _model.enemyNumber--;
                if (_model.enemyNumber == 0)
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
        /// 敌人攻击指令
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="targetList"></param>
        /// <returns></returns>
        private IEnumerator EnemyAttack(BattleEntityCtrl caster, IEnumerable<BattleEntityCtrl> targetList)
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

                //恢复动画
                entity.animatorEntity.Play("idle");
                entity.animatorEntity.SetBool("default", false);
                entity.animatorEntity.SetBool("ready", false);
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
                DoBuffEffectAtRoundEnd(characterEntity);
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
                    commandInfo.caster.SetDefendAddon("Default", 0);
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
            Debug.Log("战斗" + (isWin ? "胜利" : "失败"));
            yield return new WaitForSeconds(2f);
            if (isWin)
            {
                GameManager.Instance.PassStage();
                ProcedureManager.Instance.EnterNextStageProcedure();
                yield break;
            }

            UIManager.Instance.OpenWindow("LoadingView");
            yield return new WaitForSeconds(0.5f);
            GameManager.Instance.ReturnToTitle(0.5f,
                () => { UIManager.Instance.OpenWindow(SaveManager.GetString("ChapterType", "MainPlot").Equals("MainPlot") ? "MainPlotSelectView" : "SubPlotSelectView"); });
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