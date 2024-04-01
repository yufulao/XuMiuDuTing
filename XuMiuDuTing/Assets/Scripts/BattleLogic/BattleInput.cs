using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rabi;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Yu
{
    public partial class BattleManager
    {
        /// <summary>
        /// goBattle摁钮点击时
        /// </summary>
        public void BtnOnClickGoBattle()
        {
            _fsm.ChangeFsmState(typeof(EnemyCommandInputState));
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
            OpenAimSelectPanel(BattleCommandType.Attack, DefSelectType.DAll, DefObjCameraStateType.DEnemy, 1);
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
            _uiCtrl.view.skillSelectPanel.Open(_model.GetCurrentCharacterEntity().GetName());
        }

        /// <summary>
        /// 当点击了skillItem时
        /// </summary>
        public void OnSkillItemClick(string skillName)
        {
            var skillInfo = new SkillInfo()
            {
                skillName = skillName,
                caster = _model.GetCurrentCharacterEntity()
            };
            _model.cacheCurrentSkillInfo = skillInfo;
            var rowCfgSkill = skillInfo.RowCfgSkill;
            if (rowCfgSkill.needSelect)
            {
                OpenAimSelectPanel(BattleCommandType.Skill, rowCfgSkill.selectType, rowCfgSkill.objCameraStateType, rowCfgSkill.selectCount);
            }
            else
            {
                AddCharacterSkillCommand(skillInfo,BattleCommandType.Skill); //添加指令
                SetCommand();
            }

            _uiCtrl.view.skillSelectPanel.Close();
            _uiCtrl.CloseSkillDescribe();
        }

        /// <summary>
        /// 指令面板UniqueSkill点击事件
        /// </summary>
        public void BtnOnClickUniqueSkill()
        {
            //禁用摁键
            _uiCtrl.SetAllMenuInteractable(false);
            
            var currentCharacterEntity = _model.GetCurrentCharacterEntity();
            var skillName = currentCharacterEntity.GetRowCfgCharacter().uniqueSkillName;
            var rowCfgSkill = ConfigManager.Instance.cfgSkill[skillName];
            var skillInfo = new SkillInfo()
            {
                skillName = skillName,
                caster = currentCharacterEntity
            };
            _model.cacheCurrentSkillInfo = skillInfo;
            
            if (rowCfgSkill.needSelect)
            {
                OpenAimSelectPanel(BattleCommandType.UniqueSkill, rowCfgSkill.selectType, rowCfgSkill.objCameraStateType, rowCfgSkill.selectCount);
            }
            else
            {
                AddCharacterSkillCommand(skillInfo,BattleCommandType.UniqueSkill); //添加指令
                SetCommand();
            }

            //启用摁键
            _uiCtrl.SetAllMenuInteractable(true);
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
            var skillInfo = _model.cacheCurrentSkillInfo;
            switch (_model.selectMenuType)
            {
                case BattleCommandType.Attack:
                    currentCharacter.commandList.Add(CharacterAttack(currentCharacter, 1, selectedEntity));
                    _commandInfoList.Add(new BattleCommandInfo(false, BattleCommandType.Attack, false, 1, selectedEntity, currentCharacter));
                    break;
                case BattleCommandType.Skill:
                    if (skillInfo == null)
                    {
                        return;
                    }

                    skillInfo.targetList = selectedEntity;
                    AddCharacterSkillCommand(skillInfo,BattleCommandType.Skill);
                    _uiCtrl.CloseSkillDescribe();
                    break;
                case BattleCommandType.UniqueSkill:
                    if (skillInfo == null)
                    {
                        return;
                    }

                    currentCharacter.SetHadUniqueSkill(false);
                    _uiCtrl.SetAllMenuBtnEnable(BattleCommandType.UniqueSkill, false);
                    skillInfo.targetList = selectedEntity;
                    AddCharacterSkillCommand(skillInfo,BattleCommandType.UniqueSkill);
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
        public void OpenSkillDescribe(SkillItem skillSelectItem, BaseEventData baseEventData)
        {
            _uiCtrl.OpenSkillDescribe(skillSelectItem);
        }

        /// <summary>
        /// 关闭skillItem描述弹窗
        /// </summary>
        public void CloseSkillDescribe(SkillItem skillSelectItem, BaseEventData baseEventData)
        {
            _uiCtrl.CloseSkillDescribe();
        }

        /// <summary>
        /// 点击撤销指令时的协程
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

            //统一撤销，包括先制指令有个占位的在commandList里
            Debug.Log("实体" + currentCharacterEntity.GetName() + "撤销了指令" + currentCharacterEntity.commandList[^1]);
            _commandInfoList.Remove(_commandInfoList[^1]);
            currentCharacterEntity.commandList.Remove(currentCharacterEntity.commandList[^1]);
            if (_commandInfoList.Count == 0)
            {
                _uiCtrl.view.btnUndoCommand.gameObject.SetActive(false);
            }

            //启用摁键
            _uiCtrl.SetAllMenuInteractable(true);
        }

        /// <summary>
        /// 撤销指令的协程，恢复上一个角色
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
                        _uiCtrl.SetAllMenuBtnEnable(BattleCommandType.UniqueSkill, currentCharacterEntity.GetHadUniqueSkill());
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
                        _uiCtrl.SetAllMenuBtnEnable(BattleCommandType.UniqueSkill, currentCharacterEntity.GetHadUniqueSkill());
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

                    currentCharacterEntity.SetHadUniqueSkill(true);
                    _uiCtrl.SetAllMenuBtnEnable(BattleCommandType.UniqueSkill, currentCharacterEntity.GetHadUniqueSkill());
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
        /// 以当前角色更新菜单面板
        /// </summary>
        private void RefreshAllCommandMenu()
        {
            var characterEntity = _model.GetCurrentCharacterEntity();
            _uiCtrl.SetAllMenuBtnEnable(BattleCommandType.UniqueSkill, characterEntity.GetHadUniqueSkill());
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
        /// <param name="objCameraStateType"></param>
        /// <param name="canSelectCountT"></param>
        private void OpenAimSelectPanel(BattleCommandType battleCommandType, string selectType, string objCameraStateType, int canSelectCountT)
        {
            var aimSelectPanel = _uiCtrl.view.aimSelectPanel;
            var activeToggleList = _model.activeToggleList;
            var currentCharacterEntity = _model.GetCurrentCharacterEntity();
            var cameraLookAt = !objCameraStateType.Equals(DefObjCameraStateType.DEnemy);
            activeToggleList.Clear();
            _model.canSelectCount = canSelectCountT;
            _model.selectMenuType = battleCommandType;
            _uiCtrl.SetAllMenuActive(false); //关闭所有面板
            aimSelectPanel.gameObject.SetActive(true);
            aimSelectPanel.toggleSwitchCamera.isOn = cameraLookAt;
            _uiCtrl.OnValueChangeToggleSwitchCamera(cameraLookAt);
            if (selectType.Equals(DefSelectType.DAll))
            {
                SetToggleSelectActive(true, true, null);
            }

            if (selectType.Equals(DefSelectType.DEnemy))
            {
                SetToggleSelectActive(true, false, null);
            }

            if (selectType.Equals(DefSelectType.DCharacter))
            {
                SetToggleSelectActive(false, true, null);
            }

            if (selectType.Equals(DefSelectType.DAllExceptSelf))
            {
                SetToggleSelectActive(true, true, currentCharacterEntity);
            }

            if (selectType.Equals(DefSelectType.DEnemyExceptSelf))
            {
                SetToggleSelectActive(true, false, currentCharacterEntity);
            }

            if (selectType.Equals(DefSelectType.DCharacterExceptSelf))
            {
                SetToggleSelectActive(false, true, currentCharacterEntity);
            }

            //将所有死亡和有不可选择buff的entity关闭toggleSelect
            foreach (var entityCtrl in _model.allEntities)
            {
                if (entityCtrl.IsDie()||CheckBuff(entityCtrl,"不可选中").Count>0)
                {
                    var entityHud = entityCtrl.GetEntityHud();
                    entityHud.toggleSelect.isOn = false;
                    entityHud.toggleSelect.group = null;
                    entityHud.toggleSelect.gameObject.SetActive(false);
                    if (activeToggleList.Contains(entityHud.toggleSelect))
                    {
                        activeToggleList.Remove(entityHud.toggleSelect);
                    }
                    if (!entityCtrl.IsDie())
                    {
                        entityCtrl.SetOutlineActive(false);
                    }
                }
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
        /// 给selectToggleOnValueChange调用
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
        /// 关闭所有entity的描边
        /// </summary>
        private void CloseAllEntityOutline()
        {
            foreach (var entity in _model.allEntities)
            {
                entity.SetOutlineActive(false);
            }
        }
        
    }
}
