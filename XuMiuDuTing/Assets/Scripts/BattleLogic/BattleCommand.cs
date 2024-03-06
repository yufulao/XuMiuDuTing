using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rabi;
using UnityEngine;

namespace Yu
{
    //BattleCommand指令部分
    public partial class BattleManager
    {
        private bool _inCommandExecuting; //是否正在执行指令的流程中
        private bool _inBattleStartExecuting; //是否正在先制指令的流程中
        private readonly List<BattleCommandInfo> _commandInfoList = new List<BattleCommandInfo>();
        private readonly List<BattleCommandInfo> _sortCommandInfoList = new List<BattleCommandInfo>();
        private readonly List<IEnumerator> _allCommandList = new List<IEnumerator>();
        private readonly List<IEnumerator> _battleStartAllCommandList = new List<IEnumerator>();


        /// <summary>
        /// 初始化和每轮动画结算完毕后调用
        /// </summary>
        private void InitCommand()
        {
            _inCommandExecuting = false;
            _inBattleStartExecuting = false;
            ResetAllCommand();
            _uiCtrl.view.btnGoBattle.GetComponent<Animator>().Play("Idle");
            _uiCtrl.view.btnGoBattle.interactable = true;
            _uiCtrl.view.btnUndoCommand.gameObject.SetActive(false);
            Add1ToAllCharacterBp();
            _model.currentMenuLastIndex = -1;
            SetCommand();
            StartCoroutine(CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DIdleCommand, 0));
        }

        /// <summary>
        /// 清空所有指令
        /// </summary>
        private void ResetAllCommand()
        {
            foreach (var entityCtrl in _model.allEntities) //清空所有entity的指令List
            {
                entityCtrl.commandList.Clear();
                entityCtrl.battleStartCommandList.Clear();
            }

            _commandInfoList.Clear();
            _sortCommandInfoList.Clear();
            _allCommandList.Clear();
            _battleStartAllCommandList.Clear();
        }

        /// <summary>
        /// 所有角色bp+1
        /// </summary>
        private void Add1ToAllCharacterBp()
        {
            foreach (var characterEntity in _model.allCharacterEntities)
            {
                var bp = characterEntity.GetBp();
                if (bp < 3)
                {
                    characterEntity.UpdateBp(1);
                }

                characterEntity.SetBpPreview(bp);
                characterEntity.SetBraveCount(0);
            }
        }

        /// <summary>
        /// 输入指令
        /// </summary>
        private void SetCommand()
        {
            StartCoroutine(SetCommandIEnumerator());
        }
        
        /// <summary>
        /// 输入指令协程
        /// </summary>
        private IEnumerator SetCommandIEnumerator()
        {
            var isSwitchCharacter = false;
            var commandMenuList = _uiCtrl.view.commandMenuList;
            var characterCount = _model.characterCount;
            var characterNumber = _model.characterNumber;

            if (_commandInfoList.Count != 0) //控制back摁钮
            {
                _uiCtrl.view.btnUndoCommand.gameObject.SetActive(true);
            }

            if (_model.currentMenuLastIndex == 0) //控制menu关闭
            {
                CharacterReadyEvent(_model.GetCurrentCharacterEntity());
                var tweener = commandMenuList[0].GetComponent<RectTransform>().DOAnchorPosX(500f, 0.3f);
                yield return tweener.WaitForCompletion();
                _uiCtrl.SetAllMenuActive(false);
                _model.currentMenuLastIndex = -1;
                _model.currentCharacterEntityIndex++;
                _model.SetNextCanInputCommandCharacter();
                isSwitchCharacter = true;
            }
            else if (_model.currentMenuLastIndex > 0)
            {
                commandMenuList[_model.currentMenuLastIndex].GetComponent<RectTransform>().DOAnchorPos(new Vector2(500f, 0f), 0.3f);

                _model.currentMenuLastIndex--;
                for (var i = 0; i < _model.currentMenuLastIndex + 1; i++)
                {
                    var originalX = commandMenuList[i].GetComponent<RectTransform>().anchoredPosition.x;
                    commandMenuList[i].GetComponent<RectTransform>().DOAnchorPosX(originalX + 20f, 0.3f);
                }
            }
            else if (_model.currentMenuLastIndex == -1) //刚开始游戏，演出结束时为-1
            {
                isSwitchCharacter = true;
                _model.SetFirstCanInputCommandCharacter();
            }


            if (_model.currentCharacterEntityIndex >= _model.allCharacterEntities.Count)//溢出
            {
                _model.currentCharacterEntityIndex--;
                if (characterNumber > 0) //避免游戏输或赢后还显示
                {
                    _uiCtrl.CloseAllCharacterInfoItemSelectBg();
                    _uiCtrl.SetBtnGoEnterActive(true); //显示ContinueButton
                }

                _uiCtrl.SetAllMenuBtnEnable(BattleCommandType.Brave, true);
                _uiCtrl.SetMenuBtnEnable(commandMenuList[3], BattleCommandType.Brave, false);
                _uiCtrl.SetMenuBtnEnable(commandMenuList[0], BattleCommandType.Default, true);
            }
            else //当前还是角色
            {
                if (isSwitchCharacter)
                {
                    _uiCtrl.CloseAllCharacterInfoItemSelectBg();
                    _model.GetCurrentCharacterEntity().GetCharacterInfoItem().EnterSelect();
                }

                if (_model.currentMenuLastIndex == -1) //当前menu一个都没打开
                {
                    commandMenuList[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(500f, 0f);
                    commandMenuList[0].gameObject.SetActive(true);
                    commandMenuList[0].GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, 0f), 0.3f);
                    _model.currentMenuLastIndex = 0;

                    foreach (var commandMenu in commandMenuList)
                    {
                        _uiCtrl.SetMenuBtnEnable(commandMenu, BattleCommandType.Brave, true);
                        if (_model.GetCurrentCharacterEntity().GetMp() >= 100)
                        {
                            _uiCtrl.SetMenuBtnEnable(commandMenu, BattleCommandType.UniqueSkill, true);
                            continue;
                        }

                        commandMenu.btnUniqueSkill.SetClickDisable();
                    }

                    _uiCtrl.SetMenuBtnEnable(commandMenuList[3], BattleCommandType.Brave, false);
                    _uiCtrl.SetMenuBtnEnable(commandMenuList[0], BattleCommandType.Default, true);
                }
                else //如果当前还有打开的menu，说明在brave阶段
                {
                    _uiCtrl.SetAllMenuBtnEnable(BattleCommandType.Brave, false);
                    _uiCtrl.SetMenuBtnEnable(commandMenuList[0], BattleCommandType.Default, false);
                }

                RefreshAllCommandMenu();
                RefreshAllEntityInfoItem();
            }
        }

        /// <summary>
        /// 角色输入完成指令后，的事件
        /// </summary>
        /// <param name="characterEntity"></param>
        private void CharacterReadyEvent(CharacterEntityCtrl characterEntity)
        {
            if (_commandInfoList[^1].commandType == BattleCommandType.Default)
            {
                characterEntity.animatorEntity.Play("default_start");
                characterEntity.animatorEntity.SetBool("default", true);
                return;
            }

            characterEntity.animatorEntity.Play("ready_start");
            characterEntity.animatorEntity.SetBool("ready", true);
        }
        
        /// <summary>
        /// 角色取消已输入完成的指令后的事件
        /// </summary>
        /// <param name="entityCtrl"></param>
        private void CharacterUnreadyEvent(BattleEntityCtrl entityCtrl)
        {
            entityCtrl.animatorEntity.Play("idle");
            entityCtrl.animatorEntity.SetBool("ready", false);
            entityCtrl.animatorEntity.SetBool("default", false);
        }

        /// <summary>
        /// 场上所有entity都输入完指令了
        /// </summary>
        private void SetCommandReachEnd()
        {
            Debug.Log("场上所有entity都输入完指令了");
            StartCoroutine(PrepareAllCommandList());
        }

        /// <summary>
        /// 预处理指令列表
        /// </summary>
        /// <returns></returns>
        private IEnumerator PrepareAllCommandList()
        {
            yield return CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DIdleAnimation, 0.3f);

            //按速度排序，然后把排序后的entity的commandList加进_allCommandList
            foreach (var entity in _model.sortEntities)
            {
                if (entity.IsDie())
                {
                    continue;
                }

                //Debug.Log(sortEntities[i].entitybattleStartCommandList.Count);
                foreach (var command in entity.commandList)
                {
                    _allCommandList.Add(command);
                }

                foreach (var battleStartCommand in entity.battleStartCommandList)
                {
                    _battleStartAllCommandList.Add(battleStartCommand);
                }
            }

            SortCommandInfoList(); //给_commandInfoList排序
            ExecuteBattleStartCommandList();
        }

        /// <summary>
        /// 依据entity的Speed对CommandInfoList排序
        /// </summary>
        private void SortCommandInfoList()
        {
            _sortCommandInfoList.Clear();
            foreach (var commandInfo in _commandInfoList)
            {
                _sortCommandInfoList.Add(commandInfo);
            }

            _sortCommandInfoList.Sort((x, y) => y.caster.GetSpeed().CompareTo(x.caster.GetSpeed()));
            //测试排序
            var originalLog = "排序前的_commandInfoList顺序：";
            foreach (var commandInfo in _commandInfoList)
            {
                originalLog += "\n" + commandInfo.caster.GetName() + "--的指令--" + commandInfo.commandType;
            }

            var newLog = "排序后的_sortCommandInfoList顺序：";
            foreach (var commandInfo in _sortCommandInfoList)
            {
                newLog += "\n" + commandInfo.caster.GetName() + "--的指令--" + commandInfo.commandType;
            }

            Debug.Log(originalLog + "\n" + newLog);

            //输出所有entity的先制指令集
            originalLog = "";
            for (var i = 0; i < _model.allEntities.Count; i++)
            {
                originalLog += "\n" + i + "的先制指令为\n";
                for (var j = 0; j < _model.allEntities[i].battleStartCommandList.Count; j++)
                {
                    originalLog += j + ".->" + _model.allEntities[i].battleStartCommandList[j] + "\n";
                }
            }

            Debug.Log("全部entity的先制指令集" + originalLog);

            //输出所有entity的指令集
            originalLog = "";
            for (var i = 0; i < _model.allEntities.Count; i++)
            {
                originalLog += "\n" + i + "的指令为\n";
                for (var j = 0; j < _model.allEntities[i].commandList.Count; j++)
                {
                    originalLog += j + ".->" + _model.allEntities[i].commandList[j] + "\n";
                }
            }

            Debug.Log("全部entity的指令集" + originalLog);
        }

        /// <summary>
        /// 执行先制指令
        /// </summary>
        private void ExecuteBattleStartCommandList()
        {
            //Debug.Log(_battleStartAllCommandList.Count);
            if (!_inBattleStartExecuting)
            {
                _inBattleStartExecuting = true;
            }

            if (_battleStartAllCommandList.Count == 0) //指令全部执行完毕
            {
                ExecuteBattleStartCommandReachEnd(); //当前轮次的指令列表全部执行完毕
                return;
            }

            var caster = _sortCommandInfoList[0].caster;
            if (CheckBuff(caster, "眩晕").Count > 0)
            {
                Debug.Log(caster.GetName() + "因为眩晕，放弃执行先制指令" + _battleStartAllCommandList[0]);
                _battleStartAllCommandList.RemoveAt(0);
                ExecuteBattleStartCommandList();
                return;
            }

            Debug.Log(caster.GetName() + "执行先制指令" + _battleStartAllCommandList[0]);
            StartCoroutine(_battleStartAllCommandList[0]);
            _battleStartAllCommandList.RemoveAt(0);
            if ((_sortCommandInfoList.Count == 1) || (caster.GetName() != _sortCommandInfoList[1].caster.GetName()))
            {
                if (!caster.IsDie() && !caster.isEnemy)
                {
                    ((CharacterEntityCtrl) caster).GetCharacterInfoItem().SetActiveObjReadyTip(false);
                }
            }

            //_sortCommandInfoList.RemoveAt(0);//不能执行，里面那个是占位的，不是先制指令的info，先制指令没有info
        }

        /// <summary>
        /// 当前回合的先制指令列表全部执行完毕
        /// </summary>
        private void ExecuteBattleStartCommandReachEnd()
        {
            _inBattleStartExecuting = false;
            Debug.Log("先制指令全部执行完毕");
            RefreshAllEntityInfoItem();
            Debug.Log("开始执行常规指令");
            ExecuteCommandList(); //执行指令列表队列的队首
            _inCommandExecuting = true;
        }

        /// <summary>
        /// 执行指令列表队列的队首
        /// </summary>
        private void ExecuteCommandList()
        {
            StartCoroutine(ExecuteCommandLisIEnumerator());
        }

        private IEnumerator ExecuteCommandLisIEnumerator()
        {
            yield return new WaitForSeconds(0.7f);

            if (!_inCommandExecuting)
            {
                _inCommandExecuting = true;
            }

            if (_allCommandList.Count == 0) //指令全部执行完毕
            {
                ExecuteCommandReachEnd();
                RefreshAllEntityInfoItem();
                yield break;
            }

            //不能执行指令的情况
            var entity = _sortCommandInfoList[0].caster;
            if (CheckBuff(entity, "眩晕").Count > 0)
            {
                Debug.Log(entity.GetName() + "因为眩晕，放弃执行指令" + _allCommandList[0]);
                SkipCommand();
                yield break;
            }

            if (!entity.isEnemy)
            {
                var characterEntity = _model.GetCharacterEntityByBaseEntity(entity);

                //如果bp不够就跳过
                if (characterEntity.GetBp() - _sortCommandInfoList[0].bpNeed < -4)
                {
                    Debug.Log(characterEntity.GetName() + "因为bp不够，放弃执行指令" + _allCommandList[0]);
                    SkipCommand();
                    yield break;
                }
            }

            //执行指令
            Debug.Log(entity.GetName() + "执行指令" + _allCommandList[0]);
            StartCoroutine(_allCommandList[0]);
            _allCommandList.RemoveAt(0);
            entity = _sortCommandInfoList[0].caster;
            if ((_sortCommandInfoList.Count == 1) || (entity.GetName() != _sortCommandInfoList[1].caster.GetName()))
            {
                if (!entity.IsDie())
                {
                    entity.animatorEntity.SetBool("ready", false);
                }
            }

            _sortCommandInfoList.RemoveAt(0);
            RefreshAllEntityInfoItem();
        }

        /// <summary>
        /// 当前回合的全部指令执行完毕
        /// </summary>
        private void ExecuteCommandReachEnd()
        {
            _model.currentRound++;
            EventManager.Instance.Dispatch(EventName.OnRoundEnd);
            Debug.Log("指令全部执行完毕");
            InitCommand();
            _uiCtrl.SetAllMenuBtnEnable(BattleCommandType.Brave, true);
            var commandMenuList = _uiCtrl.view.commandMenuList;
            _uiCtrl.SetMenuBtnEnable(commandMenuList[^1], BattleCommandType.Brave, false); //最后一个menu绝对不能有Brave
            _uiCtrl.SetMenuBtnEnable(commandMenuList[0], BattleCommandType.Default, true); //最后一个menu绝对不能有Brave
            StartCoroutine(CameraManager.Instance.MoveObjCamera(DefObjCameraStateType.DIdleCommand,0.3f));

            foreach (var entityCtrl in _model.allEntities)
            {
                if (entityCtrl.IsDie())
                {
                    continue;
                }

                // entityCtrl.animator.Play("idle");
                // entityCtrl.animator.SetBool("default", false);
                // entityCtrl.animator.SetBool("ready", false);
            }

            RefreshAllCommandMenu();
            RefreshAllEntityInfoItem();
        }

        /// <summary>
        /// 跳过当前指令
        /// </summary>
        private void SkipCommand()
        {
            _allCommandList.RemoveAt(0);
            _sortCommandInfoList.RemoveAt(0);
            ExecuteCommandList();
        }
    }
}