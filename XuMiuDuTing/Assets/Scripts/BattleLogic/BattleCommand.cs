using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Yu;

namespace Yu
{
    //BattleCommand指令部分
    public partial class BattleManager
    {
        private bool inCommandExcuting; //是否正在执行指令的流程中
        private bool inBattleStartExcuting; //是否正在先制指令的流程中
        public int currentBattleId; //当前输入指令的entity的battleId
        public List<BattleCommandInfo> commandInfoList = new List<BattleCommandInfo>();
        private List<BattleCommandInfo> sortCommandInfoList = new List<BattleCommandInfo>();
        private List<IEnumerator> allCommandList = new List<IEnumerator>();
        private List<IEnumerator> battleStartAllCommandList = new List<IEnumerator>();


        /// <summary>
        /// 初始化和每轮动画结算完毕后调用
        /// </summary>
        private void InitCommand()
        {
            inCommandExcuting = false;
            inBattleStartExcuting = false;
            var allEntities = _model.allEntities;

            for (var i = 0; i < allEntities.Count; i++)
            {
                if (allEntities[i].IsDie())
                {
                    continue;
                }

                currentBattleId = i; //将第一个活着的entity的索引设为currentBattleId
                break;
            }

            foreach (var entityCtrl in allEntities) //清空所有entity的指令List
            {
                entityCtrl.commandList.Clear();
                entityCtrl.battleStartCommandList.Clear();
            }

            commandInfoList.Clear();
            sortCommandInfoList.Clear();
            allCommandList.Clear();
            battleStartAllCommandList.Clear();

            _uiCtrl.view.btnGoBattle.GetComponent<Animator>().Play("Idle");
            _uiCtrl.view.btnUndoCommand.gameObject.SetActive(false);
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

            _model.currentMenuLastIndex = -1;
            SetCommand();
        }

        /// <summary>
        /// 输入指令
        /// </summary>
        private void SetCommand()
        {
            StartCoroutine(SetCommandIEnumerator());
        }

        private IEnumerator SetCommandIEnumerator()
        {
            var isSwitchCharacter = false;
            var lastEntityId = currentBattleId;
            var commandMenuList = _uiCtrl.view.commandMenuList;
            var allCharacterEntities = _model.allCharacterEntities;
            var characterCount = _model.characterCount;
            var characterNumber = _model.characterNumber;
            var characterInfoUiList = _uiCtrl.view.characterInfoItemList;
            var characterEntity = allCharacterEntities[currentBattleId];
            yield return new WaitForSeconds(0.1f);

            if (commandInfoList.Count != 0) //控制back摁钮
            {
                _uiCtrl.view.btnUndoCommand.gameObject.SetActive(true);
            }

            if (_model.currentMenuLastIndex == 0) //控制menu关闭
            {
                CharacterReadyEvent(lastEntityId);
                Tweener tweener = commandMenuList[0].GetComponent<RectTransform>().DOAnchorPosX(500f, 0.3f);
                yield return tweener.WaitForCompletion();
                for (var i = 0; i < commandMenuList.Count; i++)
                {
                    commandMenuList[i].gameObject.SetActive(false);
                }

                _model.currentMenuLastIndex = -1;

                currentBattleId++;
                //todo buff
                // for (var i = currentBattleId; i < characterCount + 1; i++) //选出没死又有bp，并且没有眩晕buff，的最先的角色
                // {
                //     if (!allCharacterEntities[currentBattleId].IsDie() && allCharacterEntities[currentBattleId].GetBp() >= 0)
                //     {
                //         // if (buffManager.CheckBuff(allEntities[currentBattleId].entityInfoUi, BuffName.眩晕).Count <= 0)
                //         // {
                //         //     break;
                //         // }
                //     }
                //
                //     currentBattleId++;
                // }

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
                //todo buff
                // for (var i = currentBattleId; i < characterCount + 1; i++) //选出没死又有bp，并且没有眩晕buff，的最先的角色
                // {
                //     if (!allCharacterEntities[currentBattleId].IsDie() && allCharacterEntities[currentBattleId].GetBp() >= 0)
                //     {
                //         if (buffManager.CheckBuff(allEntities[currentBattleId].entityInfoUi, BuffName.眩晕).Count <= 0)
                //         {
                //             break;
                //         }
                //     }
                //
                //     currentBattleId++;
                // }
            }


            if (currentBattleId >= characterCount) //此时currentBallteId是比allEntity的最大index多1
            {
                currentBattleId--;
                if (characterNumber > 0) //避免游戏输或赢后还显示
                {
                    for (var i = 0; i < characterInfoUiList.Count; i++)
                    {
                        if (!characterInfoUiList[i].gameObject.activeInHierarchy)
                        {
                            continue;
                        }

                        var animatorStateInfo = characterInfoUiList[i].selectedBgAnimator.GetCurrentAnimatorStateInfo(0);
                        if (animatorStateInfo.IsName("SelectedBgOpen"))
                        {
                            characterInfoUiList[i].QuitSelect();
                            break;
                        }
                    }

                    _uiCtrl.view.btnGoBattle.GetComponent<Animator>().SetTrigger("open"); //显示ContinueButton
                }

                for (var i = 0; i < commandMenuList.Count; i++)
                {
                    commandMenuList[i].btnBrave.SetClickEnable(_uiCtrl.BtnOnClickBrave);
                }

                commandMenuList[3].btnBrave.SetClickDisable();
                commandMenuList[0].btnDefault.SetClickEnable(_uiCtrl.BtnOnClickDefault);
            }
            else //当前还是角色
            {
                if (isSwitchCharacter)
                {
                    for (var i = 0; i < characterInfoUiList.Count; i++)
                    {
                        if (characterInfoUiList[i].gameObject.activeInHierarchy)
                        {
                            var animatorStateInfo = characterInfoUiList[i].selectedBgAnimator.GetCurrentAnimatorStateInfo(0);
                            if (i != currentBattleId && animatorStateInfo.IsName("SelectedBgOpen"))
                            {
                                characterInfoUiList[i].QuitSelect();
                                break;
                            }
                        }
                    }

                    characterInfoUiList[currentBattleId].EnterSelect();
                }

                if (_model.currentMenuLastIndex == -1) //当前menu一个都没打开
                {
                    commandMenuList[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(500f, 0f);
                    commandMenuList[0].gameObject.SetActive(true);
                    commandMenuList[0].GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, 0f), 0.3f);
                    _model.currentMenuLastIndex = 0;

                    foreach (var commandMenu in commandMenuList)
                    {
                        commandMenu.btnBrave.SetClickEnable(_uiCtrl.BtnOnClickBrave);
                        if (allCharacterEntities[currentBattleId].GetMp() >= 100)
                        {
                            commandMenu.btnUniqueSkill.SetClickEnable(_uiCtrl.BtnOnClickUniqueSkill);
                            continue;
                        }

                        commandMenu.btnUniqueSkill.SetClickDisable();
                    }

                    commandMenuList[3].btnBrave.SetClickDisable();
                    commandMenuList[0].btnDefault.SetClickEnable(_uiCtrl.BtnOnClickDefault);
                }
                else //如果当前还有打开的menu，说明在brave阶段
                {
                    for (var i = 0; i < commandMenuList.Count - 1; i++)
                    {
                        commandMenuList[i].btnBrave.SetClickDisable();
                    }

                    commandMenuList[0].btnDefault.SetClickDisable();
                }

                RefreshMenuInfo();
                _uiCtrl.view.UpdateAllEntityUIInfo(allCharacterEntities, _model.allEnemyEntities);
            }
        }

        /// <summary>
        /// 角色输入完成指令后，的事件
        /// </summary>
        /// <param name="entityId"></param>
        private void CharacterReadyEvent(int entityId)
        {
            var allEntities = _model.allEntities;
            //todo 设置动画
            // if (commandInfoList[^1].commandType == BattleCommandType.Default)
            // {
            //     allEntities[entityId].animator.Play("default_start");
            //     allEntities[entityId].animator.SetBool("default", true);
            // }
            // else
            // {
            //     allEntities[entityId].animator.Play("ready_start");
            //     allEntities[entityId].animator.SetBool("ready", true);
            // }
        }
    }
}