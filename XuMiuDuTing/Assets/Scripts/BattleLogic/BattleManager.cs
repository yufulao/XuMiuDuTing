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

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="teamArray"></param>
        public void Init(string[] teamArray)
        {
            _model = new BattleModel();
            _model.Init(teamArray);
            InitFsm();
            InitUI();
            EventManager.Instance.AddListener(EventName.OnRoundEnd, OnRoundEnd);
            // StartCoroutine(BGMManager.Instance.PlayBgmFadeDelay("战斗a", 0f, 0.5f, 0f, 1f,
            //     () => { StartCoroutine(BGMManager.Instance.PlayBgmFadeDelay("战斗b", 0f, 0.5f, 0f)); }));
            SpawnEntity();
            _model.SortEntityList();
            InitCommand();
            RefreshAllEntityInfoItem();
            RefreshAllCommandMenu();
            StartCoroutine(AllEntityPlayEnterAnimation());
            UIManager.Instance.CloseWindow("LoadingView");
        }

        /// <summary>
        /// goBattle摁钮点击时
        /// </summary>
        public void BtnOnClickGoBattle()
        {
            _uiCtrl.view.btnGoBattle.GetComponent<Animator>().SetTrigger("close");
            _uiCtrl.view.btnUndoCommand.gameObject.SetActive(false);
            _model.SetFirstCanInputCommandEnemy();
            EnemySetCommandAI();
        }

        /// <summary>
        /// 撤销指令btn点击时回调
        /// </summary>
        public void BtnOnClickUndoCommand()
        {
            StartCoroutine(BtnOnClickUndoCommandIEnumerator());
        }

        /// <summary>
        /// 指令面板Attack点击事件
        /// </summary>
        public void BtnOnClickAttack()
        {
            OpenSelectMenu(BattleCommandType.Attack, SelectType.All, false, 1);
        }

        /// <summary>
        /// 指令面板Brave点击事件
        /// </summary>
        public void BtnOnClickBrave()
        {
            //禁用摁键
            _uiCtrl.SetAllMenuInteractable(false);

            var currentCharacterEntity = _model.GetCurrentCharacterEntity();
            var menuPanelList = _uiCtrl.view.commandMenuList;
            StartCoroutine(Utils.PlayAnimation(currentCharacterEntity.animatorBuff, "Brave"));
            _model.currentMenuLastIndex++;

            for (var i = 0; i < _model.currentMenuLastIndex; i++) //把待输入指令的menu全部向左移动
            {
                var originalX = menuPanelList[i].GetComponent<RectTransform>().anchoredPosition.x;
                menuPanelList[i].GetComponent<RectTransform>().DOAnchorPosX(originalX - 20f, 0.3f);
            }

            menuPanelList[_model.currentMenuLastIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(500f, 0f);
            menuPanelList[_model.currentMenuLastIndex].gameObject.SetActive(true);
            menuPanelList[_model.currentMenuLastIndex].GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, 0f), 0.3f);

            currentCharacterEntity.commandList.Add(CharacterBrave(currentCharacterEntity));
            _commandInfoList.Add(new BattleCommandInfo(false, BattleCommandType.Brave, false, 1, null, currentCharacterEntity));
            Debug.Log("实体" + currentCharacterEntity.GetName()
                           + "输入了指令" + currentCharacterEntity.commandList[^1] + "，commandInfo的数量为" + _commandInfoList.Count);

            var braveCount = currentCharacterEntity.GetBraveCount();
            if (braveCount > 0)
            {
                currentCharacterEntity.UpdateBpPreview(-1);
            }

            if (braveCount == 0) //bp{review=-4就不能brave
            {
                currentCharacterEntity.UpdateBpPreview(-2);
            }

            if (currentCharacterEntity.GetBpPreview() <= -4)
            {
                _uiCtrl.SetAllMenuBtnEnable(BattleCommandType.Brave, false);
            }

            currentCharacterEntity.UpdateBraveCount(1);
            RefreshAllCommandMenu();
            _uiCtrl.view.btnUndoCommand.gameObject.SetActive(true);

            //启用摁键
            _uiCtrl.SetAllMenuInteractable(true);
        }

        /// <summary>
        /// 指令面板Skill点击事件
        /// </summary>
        public void BtnOnClickSkill()
        {
        }

        /// <summary>
        /// 指令面板UniqueSkill点击事件
        /// </summary>
        public void BtnOnClickUniqueSkill()
        {
        }

        /// <summary>
        /// 指令面板Default点击事件
        /// </summary>
        public void BtnOnClickDefault()
        {
            //禁用摁键
            _uiCtrl.SetAllMenuInteractable(false);

            var currentCharacterEntity = _model.GetCurrentCharacterEntity();
            currentCharacterEntity.battleStartCommandList.Add(CharacterDefend(currentCharacterEntity));
            currentCharacterEntity.commandList.Add(BattleStartCommandInCommandList(currentCharacterEntity)); //占位
            _commandInfoList.Add(new BattleCommandInfo(false, BattleCommandType.Default, true, 0, null, currentCharacterEntity));
            Debug.Log("实体" + currentCharacterEntity.GetName()
                           + "输入了指令" + currentCharacterEntity.commandList[^1] + "，commandInfo的数量为" + _commandInfoList.Count);
            SetCommand();

            //启用摁键
            _uiCtrl.SetAllMenuInteractable(true);
        }

        /// <summary>
        /// 从选择目标界面点击ok
        /// </summary>
        public void BtnOnClickSelectOk()
        {
            //禁用摁键
            _uiCtrl.SetAllMenuInteractable(false);

            //Debug.Log(currentBallteId);
            //Debug.Log(selectedEntityList.Count);
            if (_model.selectedEntityList.Count <= 0) //判断选择了目标没
            {
                return; //一个都没选
            }

            CloseAllEntityOutline();

            var selectedEntity = new List<BattleEntityCtrl>(); //深拷贝
            foreach (var entitySelected in _model.selectedEntityList)
            {
                selectedEntity.Add(entitySelected);
            }

            var currentCharacter = _model.GetCurrentCharacterEntity();
            var skillSelectInfo = _model.cacheCurrentSkillSelectInfo;
            switch (_model.selectMenuType)
            {
                case BattleCommandType.Attack:
                    currentCharacter.commandList.Add(CharacterAttack(currentCharacter, 1, selectedEntity));
                    _commandInfoList.Add(new BattleCommandInfo(false, BattleCommandType.Attack, false, 1, selectedEntity, currentCharacter));
                    break;
                case BattleCommandType.Skill:
                    skillSelectInfo.selectedEntityList = selectedEntity;
                    AddSkill(skillSelectInfo);
                    _uiCtrl.CloseSkillDescribe();
                    _commandInfoList.Add(new BattleCommandInfo(false, BattleCommandType.Skill, skillSelectInfo.isBattleStartCommand, skillSelectInfo.bpNeed, selectedEntity, currentCharacter));
                    break;
                case BattleCommandType.UniqueSkill:
                    currentCharacter.SetHadUniqueSkill(true);
                    RefreshUniqueSkillButton();
                    skillSelectInfo.selectedEntityList = selectedEntity;
                    AddSkill(skillSelectInfo);
                    _commandInfoList.Add(new BattleCommandInfo(false, BattleCommandType.UniqueSkill, skillSelectInfo.isBattleStartCommand, skillSelectInfo.bpNeed, selectedEntity, currentCharacter));
                    break;
                default:
                    Debug.LogError("目标输入待添加新的CommandType");
                    break;
            }

            Debug.Log("实体" + currentCharacter.GetName() + "输入了指令" + currentCharacter.commandList[^1] + "，commandInfo的数量为" + _commandInfoList.Count);

            SetCommand();
            _uiCtrl.view.aimSelectPanel.gameObject.SetActive(false);
            _uiCtrl.SetAllMenuActive(true); //打开所有指令面板
            StartCoroutine(CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DIdleCommand, 0.3f));
            ResetSelectEntity();

            //启用摁键
            _uiCtrl.SetAllMenuInteractable(true);
        }

        /// <summary>
        /// 从选择目标界面点击undo
        /// </summary>
        public void BtnOnClickSelectUndo()
        {
            //禁用摁键
            _uiCtrl.SetAllMenuInteractable(false);
            CloseAllEntityOutline();

            _uiCtrl.view.aimSelectPanel.gameObject.SetActive(false);
            _uiCtrl.SetAllMenuActive(true);
            StartCoroutine(CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DIdleCommand, 0.3f));
            ResetSelectEntity();

            //启用摁键
            _uiCtrl.SetAllMenuInteractable(true);
        }

        /// <summary>
        /// 打开buff描述弹窗
        /// </summary>
        private void OpenBuffDescribe(BuffItem buffItem, BaseEventData baseEventData)
        {
            _uiCtrl.OpenBuffDescribe(buffItem);
        }

        /// <summary>
        /// 关闭buff描述弹窗
        /// </summary>
        private void CloseBuffDescribe(BuffItem buffItem, BaseEventData baseEventData)
        {
            _uiCtrl.CloseBuffDescribe();
        }

        /// <summary>
        /// 打开skillItem描述弹窗
        /// </summary>
        public void OpenSkillDescribe(SkillSelectItem skillSelectItem, BaseEventData baseEventData)
        {
            _uiCtrl.OpenSkillDescribe(skillSelectItem);
        }

        /// <summary>
        /// 关闭skillItem描述弹窗
        /// </summary>
        public void CloseSkillDescribe(SkillSelectItem skillSelectItem, BaseEventData baseEventData)
        {
            _uiCtrl.CloseSkillDescribe();
        }

        /// <summary>
        /// 点击撤销指令时
        /// </summary>
        /// <returns></returns>
        private IEnumerator BtnOnClickUndoCommandIEnumerator()
        {
            //禁用摁键
            _uiCtrl.SetAllMenuInteractable(false);

            _uiCtrl.SetBtnGoEnterActive(false);
            yield return UndoCommandOfRestorePreCharacter(); //恢复上一个角色
            var currentCharacterEntity = _model.GetCurrentCharacterEntity();
            var isBraveCommand = false; //这个指令是否,是在brave界面输入的attack等指令
            yield return UndoCommandOfInitMenuAfterRestore(isBraveCommandT => { isBraveCommand = isBraveCommandT; }); //初始化
            RefreshAllCommandMenu();
            RefreshAllEntityInfoItem();
            yield return UndoCommandOfSwitchCommandType(isBraveCommand);
            //如果撤销的是先制指令
            if (_commandInfoList[^1].isBattleStartCommand)
            {
                currentCharacterEntity.battleStartCommandList.Remove(currentCharacterEntity.battleStartCommandList[^1]);
            }

            Debug.Log("实体" + currentCharacterEntity.GetName() + "撤销了先制指令" + currentCharacterEntity.commandList[^1] + "，commandInfo的数量+1为" + _commandInfoList.Count);
            //统一撤销，包括先制指令有个占位的在commandList里
            Debug.Log("实体" + currentCharacterEntity.GetName() + "撤销了指令" + currentCharacterEntity.commandList[^1] + "，commandInfo的数量+1为" + _commandInfoList.Count);
            currentCharacterEntity.commandList.Remove(currentCharacterEntity.commandList[^1]);
            _commandInfoList.Remove(_commandInfoList[^1]);
            if (_commandInfoList.Count == 0)
            {
                _uiCtrl.view.btnUndoCommand.gameObject.SetActive(false);
            }

            //启用摁键
            _uiCtrl.SetAllMenuInteractable(true);
        }

        /// <summary>
        /// 恢复上一个角色
        /// </summary>
        /// <returns></returns>
        private IEnumerator UndoCommandOfRestorePreCharacter()
        {
            var currentCharacterEntity = _model.GetCurrentCharacterEntity();
            var lastCommandCaster = _commandInfoList[^1].caster;
            switch (_commandInfoList.Count)
            {
                case 1:
                    if (currentCharacterEntity.GetName() != _commandInfoList[0].caster.GetName()) //这是最早的第一条指令
                    {
                        _model.currentMenuLastIndex = -1;
                        yield return UndoCommandOfRestorePreCharacterPiece();
                    }

                    break;
                case > 1:
                    if (currentCharacterEntity.GetName() != lastCommandCaster.GetName()) //还有上一个指令，并且要切换角色
                    {
                        yield return UndoCommandOfRestorePreCharacterPiece();
                    }

                    break;
            }
        }

        /// <summary>
        /// 隶属UndoCommandOfRestorePreCharacter
        /// </summary>
        /// <returns></returns>
        private IEnumerator UndoCommandOfRestorePreCharacterPiece()
        {
            var commandMenuList = _uiCtrl.view.commandMenuList;
            var lastCommandCaster = _commandInfoList[^1].caster;
            var newCurrentCharacterEntity = _model.GetCharacterEntityByBaseEntity(lastCommandCaster);
            _model.SetCurrentCharacterEntity(newCurrentCharacterEntity); //恢复上一个输入指令的角色
            CharacterUnreadyEvent(newCurrentCharacterEntity);
            var tween = commandMenuList[0].GetComponent<RectTransform>().DOAnchorPosX(500f, 0.3f);
            yield return tween.WaitForCompletion();
            commandMenuList[0].gameObject.SetActive(false);
            _uiCtrl.CloseAllCharacterInfoItemSelectBg();
            newCurrentCharacterEntity.GetCharacterInfoItem().EnterSelect();
        }

        /// <summary>
        /// 恢复上一个角色后的初始化Menu，隶属UndoCommandOfRestorePreCharacter
        /// </summary>
        /// <returns></returns>
        private IEnumerator UndoCommandOfInitMenuAfterRestore(UnityAction<bool> isBraveCommandCallback)
        {
            var commandMenuList = _uiCtrl.view.commandMenuList;
            var isBraveCommand = false; //这个指令是否,是在brave界面输入的attack等指令
            var currentCharacterEntity = _model.GetCurrentCharacterEntity();
            switch (_model.currentMenuLastIndex)
            {
                case -1:
                    if (currentCharacterEntity.GetBraveCount() > 0)
                    {
                        yield return StartCoroutine(InitDoneMenuForBack());
                    }
                    else if (currentCharacterEntity.GetBraveCount() == 0)
                    {
                        for (var i = 1; i < commandMenuList.Count; i++)
                        {
                            commandMenuList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(500f, 0f);
                            commandMenuList[i].gameObject.SetActive(false);
                        }

                        commandMenuList[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(500f, 0f);
                        commandMenuList[0].gameObject.SetActive(true);
                        var tween = commandMenuList[0].GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, 0f), 0.3f);
                        yield return tween.WaitForCompletion();
                        _model.currentMenuLastIndex = 0;

                        _uiCtrl.SetMenuBtnEnable(commandMenuList[0], BattleCommandType.Brave, true);
                        _uiCtrl.SetMenuBtnEnable(commandMenuList[0], BattleCommandType.Default, true);
                        RefreshUniqueSkillButton();
                    }

                    currentCharacterEntity.GetCharacterInfoItem().EnterSelect();
                    break;
                case 0:
                    if (currentCharacterEntity.GetBraveCount() > 0)
                    {
                        if (commandMenuList[1].gameObject.activeInHierarchy)
                        {
                            isBraveCommand = true;
                        }
                        else
                        {
                            yield return StartCoroutine(InitDoneMenuForBack());
                        }
                    }
                    else if (currentCharacterEntity.GetBraveCount() == 0)
                    {
                        commandMenuList[0].gameObject.SetActive(true);
                        var tween = commandMenuList[0].GetComponent<RectTransform>().DOAnchorPosX(0f, 0.3f);
                        yield return tween.WaitForCompletion();
                        _uiCtrl.SetMenuBtnEnable(commandMenuList[0], BattleCommandType.Brave, true);
                        _uiCtrl.SetMenuBtnEnable(commandMenuList[0], BattleCommandType.Default, true);
                        RefreshUniqueSkillButton();
                    }

                    break;
                case >0:
                    if (_model.currentMenuLastIndex + 1 < commandMenuList.Count && commandMenuList[_model.currentMenuLastIndex + 1].gameObject.activeInHierarchy)
                    {
                        isBraveCommand = true;
                    }

                    break;
            }

            isBraveCommandCallback?.Invoke(isBraveCommand);
        }

        /// <summary>
        /// 处理撤销的command类型，隶属UndoCommandOfRestorePreCharacter
        /// </summary>
        /// <returns></returns>
        private IEnumerator UndoCommandOfSwitchCommandType(bool isBraveCommand)
        {
            var commandMenuList = _uiCtrl.view.commandMenuList;
            var currentCharacterEntity = _model.GetCurrentCharacterEntity();
            var commandType = _commandInfoList[^1].commandType;
            switch (commandType)
            {
                case BattleCommandType.Attack:
                case BattleCommandType.Skill:
                    if (isBraveCommand)
                    {
                        var newWaitTime = 0f;
                        for (var i = 0; i < _model.currentMenuLastIndex + 1; i++)
                        {
                            var originalX = commandMenuList[i].GetComponent<RectTransform>().anchoredPosition.x;
                            var tween = commandMenuList[i].GetComponent<RectTransform>().DOAnchorPosX(originalX - 20f, 0.3f);
                            newWaitTime += tween.Duration();
                        }

                        commandMenuList[_model.currentMenuLastIndex + 1].GetComponent<RectTransform>().DOAnchorPosX(0f, 0.3f);
                        yield return new WaitForSeconds(newWaitTime);

                        _model.currentMenuLastIndex++;
                    }

                    break;

                case BattleCommandType.UniqueSkill:
                    if (isBraveCommand)
                    {
                        var newWaitTime = 0f;
                        for (var i = 0; i < _model.currentMenuLastIndex + 1; i++)
                        {
                            var originalX = commandMenuList[i].GetComponent<RectTransform>().anchoredPosition.x;
                            var tween = commandMenuList[i].GetComponent<RectTransform>().DOAnchorPosX(originalX - 20f, 0.3f);
                            newWaitTime += tween.Duration();
                        }

                        commandMenuList[_model.currentMenuLastIndex + 1].GetComponent<RectTransform>().DOAnchorPosX(0f, 0.3f);
                        yield return new WaitForSeconds(newWaitTime);

                        _model.currentMenuLastIndex++;
                    }

                    currentCharacterEntity.SetHadUniqueSkill(false);
                    RefreshUniqueSkillButton();
                    break;

                case BattleCommandType.Brave:
                    if (currentCharacterEntity.GetBraveCount() > 1)
                    {
                        currentCharacterEntity.UpdateBpPreview(1); //返还bp
                    }
                    else if (currentCharacterEntity.GetBraveCount() == 1)
                    {
                        currentCharacterEntity.UpdateBpPreview(2); //返还bp
                        _uiCtrl.SetAllMenuBtnEnable(BattleCommandType.Brave, true);
                        _uiCtrl.SetMenuBtnEnable(commandMenuList[0], BattleCommandType.Default, true);
                    }

                    currentCharacterEntity.UpdateBraveCount(-1);

                    float waitTime = 0;
                    for (var i = 0; i < _model.currentMenuLastIndex; i++)
                    {
                        var originalX = commandMenuList[i].GetComponent<RectTransform>().anchoredPosition.x;
                        var tween = commandMenuList[i].GetComponent<RectTransform>().DOAnchorPosX(originalX + 20f, 0.3f);
                        waitTime += tween.Duration();
                    }

                    commandMenuList[_model.currentMenuLastIndex].GetComponent<RectTransform>().DOAnchorPosX(500f, 0.3f);
                    yield return new WaitForSeconds(waitTime);

                    //函数要放里面，不然有时差，先减了后执行前面的回调,并且要在动画时关闭backButton不然出错
                    commandMenuList[_model.currentMenuLastIndex].gameObject.SetActive(false);
                    _model.currentMenuLastIndex--;

                    RefreshAllCommandMenu();
                    break;

                case BattleCommandType.Default:
                    break;
                default:
                    Debug.LogError("没有实现这个指令的撤销" + commandType);
                    break;
            }
        }

        /// <summary>
        /// 恢复到上一个角色最后的面板状态
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitDoneMenuForBack()
        {
            var currentCharacterEntity = _model.GetCurrentCharacterEntity();
            var commandMenuList = _uiCtrl.view.commandMenuList;
            var braveCount = currentCharacterEntity.GetBraveCount(); //比如角色设置了三层，就是2，撤回时，这个值就是2
            for (var i = 1; i < braveCount + 1; i++) //这个就是1,2
            {
                commandMenuList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(500f, 0f);
                commandMenuList[i].gameObject.SetActive(true);
            }

            for (var i = braveCount + 1; i < commandMenuList.Count; i++) //这个是3
            {
                commandMenuList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(500f, 0f);
                commandMenuList[i].gameObject.SetActive(false);
            }

            commandMenuList[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(500f, 0f);
            commandMenuList[0].gameObject.SetActive(true); //这个就是0
            var tween = commandMenuList[0].GetComponent<RectTransform>().DOAnchorPosX(0f, 0.3f);
            yield return tween.WaitForCompletion();
            _model.currentMenuLastIndex = 0;
            _uiCtrl.SetAllMenuBtnEnable(BattleCommandType.Brave, false);
            _uiCtrl.SetMenuBtnEnable(commandMenuList[0], BattleCommandType.Default, false);
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
        /// 以当前角色更新菜单面板
        /// </summary>
        private void RefreshAllCommandMenu()
        {
            var characterEntity = _model.GetCurrentCharacterEntity();
            _uiCtrl.view.RefreshMenuInfo(characterEntity.GetName(), characterEntity.GetBp(), characterEntity.GetBpPreview(), characterEntity.GetMp());
        }

        /// <summary>
        /// 刷新所有
        /// </summary>
        private void RefreshAllEntityInfoItem()
        {
            _uiCtrl.view.UpdateAllEntityInfoItem(_model.allCharacterEntities, _model.allEnemyEntities);
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
                StartCoroutine(Utils.PlayAnimation(selectEntity.animatorEntity, "attack"));
                yield return CameraManager.Instance.MoveObjCameraByEntityIsEnemy(selectEntity, 0);
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
            _model.characterCount = 0;
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
                // buffManager.ClearBuff(characterInfoUiList[i]);
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
                // characterEntity.skillVfxAnim = characterObj.transform.Find("SkillVfx").GetComponent<Animator>();
                // characterEntity.buffVfxAnim = characterObj.transform.Find("BuffVfx").GetComponent<Animator>();
                //设置HUD跟随组件
                var uiFollowObj = characterEntity.gameObject.AddComponent<UIFollowObj>();
                uiFollowObj.objFollowed = characterObj.transform.Find("HudFollowPoint");
                uiFollowObj.rectFollower = entityHud.GetComponent<RectTransform>();
                uiFollowObj.offset = new Vector2(0f, 15f);
                //一些初始设置
                characterEntity.SetEntitySpineOutlineActive(false);
                //加进entityList
                _model.characterNumber++;
                _model.characterCount++;
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
                //buffManager.ClearBuff(enemyInfoUiList[i]);
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
                // enemyEntity.skillVfxAnim = enemyObj.transform.Find("SkillVfx").GetComponent<Animator>();
                // enemyEntity.buffVfxAnim = enemyObj.transform.Find("BuffVfx").GetComponent<Animator>();
                //设置HUD跟随组件
                var uiFollowObj = enemyEntity.gameObject.AddComponent<UIFollowObj>();
                uiFollowObj.objFollowed = enemyObj.transform.Find("HudFollowPoint");
                uiFollowObj.rectFollower = entityHud.GetComponent<RectTransform>();
                uiFollowObj.offset = new Vector2(0f, 15f);
                //一些初始设置
                enemyEntity.SetEntitySpineOutlineActive(false);
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
        /// 初始化entity的复选框
        /// </summary>
        private void InitAimSelectToggleList()
        {
            var aimSelectPanel = _uiCtrl.view.aimSelectPanel;
            aimSelectPanel.gameObject.SetActive(false);
            _model.activeToggleList.Clear();
            foreach (var entity in _model.allEntities)
            {
                entity.GetEntityHud().SetToggleSelectOnValueChange((isOn) => SelectEntity(isOn, entity));
            }

            ResetSelectEntity();
        }

        /// <summary>
        /// 打开选择面板
        /// </summary>
        /// <param name="battleCommandType"></param>
        /// <param name="selectType"></param>
        /// <param name="cameraLookAt"></param>
        /// <param name="canSelectCountT"></param>
        private void OpenSelectMenu(BattleCommandType battleCommandType, SelectType selectType, bool cameraLookAt, int canSelectCountT)
        {
            var aimSelectPanel = _uiCtrl.view.aimSelectPanel;
            var activeToggleList = _model.activeToggleList;
            var currentCharacterEntity = _model.GetCurrentCharacterEntity();

            activeToggleList.Clear();
            _model.canSelectCount = canSelectCountT;
            _model.selectMenuType = battleCommandType;

            _uiCtrl.SetAllMenuActive(false); //关闭所有面板
            aimSelectPanel.gameObject.SetActive(true);
            aimSelectPanel.toggleSwitchCamera.isOn = cameraLookAt;
            _uiCtrl.OnValueChangeToggleSwitchCamera(cameraLookAt);

            switch (selectType)
            {
                case SelectType.All:
                    SetToggleSelectActive(true, true, null);
                    break;
                case SelectType.Enemy:
                    SetToggleSelectActive(true, false, null);
                    break;
                case SelectType.Character:
                    SetToggleSelectActive(false, true, null);
                    break;
                case SelectType.AllExceptSelf:
                    SetToggleSelectActive(true, true, currentCharacterEntity);
                    break;
                case SelectType.EnemyExceptSelf:
                    SetToggleSelectActive(true, false, currentCharacterEntity);
                    break;
                case SelectType.CharacterExceptSelf:
                    SetToggleSelectActive(false, true, currentCharacterEntity);
                    break;
            }

            //将所有死亡的entity关闭toggleSelect
            foreach (var entityCtrl in _model.allEntities)
            {
                if (!entityCtrl.IsDie())
                {
                    continue;
                }

                var entityHud = entityCtrl.GetEntityHud();
                entityHud.toggleSelect.isOn = false;
                entityHud.toggleSelect.group = null;
                entityHud.toggleSelect.gameObject.SetActive(false);
            }

            //将所有启用的toggleSelect设置toggleGroup
            foreach (var activeToggle in activeToggleList)
            {
                activeToggle.group = _model.canSelectCount == 1 ? _uiCtrl.view.selectToggleGroup : null;
            }

            _model.selectedEntityList.Clear();
        }

        /// <summary>
        /// 设置指定的toggle激活启用
        /// </summary>
        private void SetToggleSelectActive(bool enemyActive, bool characterActive, BattleEntityCtrl entityExcept)
        {
            foreach (var entityCtrl in _model.allEntities)
            {
                var toggleSelect = entityCtrl.GetEntityHud().toggleSelect;
                toggleSelect.isOn = false;
                if (entityCtrl.isEnemy)
                {
                    toggleSelect.gameObject.SetActive(enemyActive);
                    entityCtrl.SetOutlineActive(enemyActive);
                    if (enemyActive)
                    {
                        _model.activeToggleList.Add(toggleSelect);
                    }

                    continue;
                }

                toggleSelect.gameObject.SetActive(characterActive);
                entityCtrl.SetOutlineActive(characterActive);
                if (characterActive)
                {
                    _model.activeToggleList.Add(toggleSelect);
                }
            }

            if (!entityExcept)
            {
                return;
            }

            var entityExceptHud = entityExcept.GetEntityHud();
            var exceptToggleSelect = entityExceptHud.toggleSelect;
            exceptToggleSelect.isOn = false;
            exceptToggleSelect.gameObject.SetActive(false);
            entityExcept.SetOutlineActive(false);
            if (_model.activeToggleList.Contains(exceptToggleSelect))
            {
                _model.activeToggleList.Remove(exceptToggleSelect);
            }
        }

        /// <summary>
        /// 给selectAttackButton调用
        /// </summary>
        /// <param name="isOn"></param>
        /// <param name="entity"></param>
        private void SelectEntity(bool isOn, BattleEntityCtrl entity)
        {
            if (!_uiCtrl.view.aimSelectPanel.gameObject.activeInHierarchy)
            {
                return;
            }

            var selectedEntityList = _model.selectedEntityList;
            var activeToggleList = _model.activeToggleList;
            if (isOn)
            {
                selectedEntityList.Add(entity);
                _model.canSelectCount--;

                if (_model.canSelectCount <= 0 && activeToggleList.Count > 0 && !activeToggleList[0].group) //activeToggleList[0].group != null来判断是不是只允许选择单体,因为选择单体要设定group
                {
                    foreach (var activeToggle in activeToggleList)
                    {
                        if (!activeToggle.isOn)
                        {
                            activeToggle.gameObject.SetActive(false);
                        }
                    }
                }

                return;
            }

            selectedEntityList.Remove(entity);
            if (_model.canSelectCount == 0)
            {
                foreach (var activeToggle in activeToggleList)
                {
                    activeToggle.gameObject.SetActive(true);
                }
            }

            _model.canSelectCount++;
        }

        /// <summary>
        /// 重置选择目标
        /// </summary>
        private void ResetSelectEntity()
        {
            foreach (var entityCtrl in _model.allEntities)
            {
                var toggleSelect = entityCtrl.GetEntityHud().toggleSelect;
                toggleSelect.isOn = false;
                toggleSelect.gameObject.SetActive(false);
            }

            _model.selectedEntityList.Clear();
            _model.canSelectCount = -1;
        }

        /// <summary>
        /// 刷新必杀技btn
        /// </summary>
        private void RefreshUniqueSkillButton()
        {
            var currentCharacterEntity = _model.GetCurrentCharacterEntity();
            if (currentCharacterEntity.GetHadUniqueSkill())
            {
                _uiCtrl.SetAllMenuBtnEnable(BattleCommandType.UniqueSkill, false);
                return;
            }

            _uiCtrl.SetAllMenuBtnEnable(BattleCommandType.UniqueSkill, true);
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
            Utils.TextFly(textHurtPoint, Vector3.zero);

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
            yield return StartCoroutine(Utils.PlayAnimation(caster.animatorEntity, "attack"));

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
        /// 关闭所有entity的描边
        /// </summary>
        private void CloseAllEntityOutline()
        {
            foreach (var entity in _model.allEntities)
            {
                entity.SetOutlineActive(false);
            }
        }

        /// <summary>
        /// 回合结束事件
        /// </summary>
        private void OnRoundEnd()
        {
            //Buff生效
            //重置防御
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
            //todo 游戏结束
            //SystemFacade.instance.BattleFinish(isWin);
            yield return new WaitForSeconds(0.5f);
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
                {typeof(CharacterCommandInputState), new CharacterCommandInputState()},
                {typeof(EnemyCommandInputState), new EnemyCommandInputState()},
                {typeof(ExecutingState), new ExecutingState()}
            });
        }
    }
}