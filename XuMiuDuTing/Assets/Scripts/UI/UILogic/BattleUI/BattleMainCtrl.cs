using Rabi;
using UnityEngine;

namespace Yu
{
    public class BattleMainCtrl : UICtrlBase
    {
        private BattleMainModel _model;
        public BattleMainView view;


        public override void OnInit(params object[] param)
        {
            _model = new BattleMainModel();
            view = GetComponent<BattleMainView>();
            _model.OnInit();
            view.OnInit();
        }

        public override void OpenRoot(params object[] param)
        {
            view.OpenWindow();
        }

        public override void CloseRoot()
        {
            view.CloseWindow();
        }

        public override void BindEvent()
        {
            //CommandMenu绑定
            foreach (var commandMenu in view.commandMenuList)
            {
                SetMenuBtnEnable(commandMenu, BattleCommandType.Attack, true);
                SetMenuBtnEnable(commandMenu, BattleCommandType.Brave, true);
                SetMenuBtnEnable(commandMenu, BattleCommandType.Skill, true);
                SetMenuBtnEnable(commandMenu, BattleCommandType.UniqueSkill, true);
                SetMenuBtnEnable(commandMenu, BattleCommandType.Default, false);
            }

            SetMenuBtnEnable(view.commandMenuList[3], BattleCommandType.Brave, false);
            SetMenuBtnEnable(view.commandMenuList[0], BattleCommandType.Default, true);

            //SkillSelectPanel绑定
            view.skillSelectPanel.SetSkillItemOnClick(BattleManager.Instance.OnSkillItemClick);
            view.aimSelectPanel.toggleSwitchCamera.onValueChanged.AddListener(OnValueChangeToggleSwitchCamera);
            view.aimSelectPanel.btnBack.onClick.AddListener(BattleManager.Instance.BtnOnClickSelectUndo);
            view.aimSelectPanel.btnSelectEnd.onClick.AddListener(BattleManager.Instance.BtnOnClickSelectOk);
            view.btnUndoCommand.onClick.AddListener(BattleManager.Instance.BtnOnClickUndoCommand);
            view.btnGoBattle.onClick.AddListener(BattleManager.Instance.BtnOnClickGoBattle);
        }

        /// <summary>
        /// 清除所有Entity的HUD
        /// </summary>
        public void ClearAllEntityHud()
        {
            for (var i = 0; i < view.entityHudContainer.childCount; i++)
            {
                Destroy(view.entityHudContainer.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 激活启用指令面板的指令btn
        /// </summary>
        /// <param name="commandMenu"></param>
        /// <param name="commandType"></param>
        /// <param name="active"></param>
        public void SetMenuBtnEnable(CommandMenu commandMenu, BattleCommandType commandType, bool active)
        {
            var battleManager = BattleManager.Instance;
            if (active)
            {
                switch (commandType)
                {
                    case BattleCommandType.Attack:
                        commandMenu.btnAttack.SetClickEnable(battleManager.BtnOnClickAttack);
                        break;
                    case BattleCommandType.Skill:
                        commandMenu.btnSkill.SetClickEnable(battleManager.BtnOnClickSkill);
                        break;
                    case BattleCommandType.Brave:
                        commandMenu.btnBrave.SetClickEnable(battleManager.BtnOnClickBrave);
                        break;
                    case BattleCommandType.UniqueSkill:
                        commandMenu.btnUniqueSkill.SetClickEnable(battleManager.BtnOnClickUniqueSkill);
                        break;
                    case BattleCommandType.Default:
                        commandMenu.btnDefault.SetClickEnable(battleManager.BtnOnClickDefault);
                        break;
                }

                return;
            }

            switch (commandType)
            {
                case BattleCommandType.Attack:
                    commandMenu.btnAttack.SetClickDisable();
                    break;
                case BattleCommandType.Skill:
                    commandMenu.btnSkill.SetClickDisable();
                    break;
                case BattleCommandType.Brave:
                    commandMenu.btnBrave.SetClickDisable();
                    break;
                case BattleCommandType.UniqueSkill:
                    commandMenu.btnUniqueSkill.SetClickDisable();
                    break;
                case BattleCommandType.Default:
                    commandMenu.btnDefault.SetClickDisable();
                    break;
            }
        }

        /// <summary>
        /// 激活启用所有指令面板的指令btn
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="active"></param>
        public void SetAllMenuBtnEnable(BattleCommandType commandType, bool active)
        {
            foreach (var commandMenu in view.commandMenuList)
            {
                SetMenuBtnEnable(commandMenu, commandType, active);
            }
        }

        /// <summary>
        /// 点击切换摄像机toggle
        /// </summary>
        /// <param name="isOn"></param>
        public void OnValueChangeToggleSwitchCamera(bool isOn)
        {
            StartCoroutine(CameraManager.Instance.MoveObjCamera(isOn ? DefObjCameraStateType.DCharacter : DefObjCameraStateType.DEnemy, 0.3f));
        }

        /// <summary>
        /// 关闭所有characterInfoItem的selectBg
        /// </summary>
        public void CloseAllCharacterInfoItemSelectBg(CharacterInfoItem exceptInfoItem = null)
        {
            foreach (var infoItem in view.characterInfoItemList)
            {
                if (infoItem == exceptInfoItem || !infoItem.gameObject.activeInHierarchy)
                {
                    continue;
                }

                if (Utils.IsAnimatorPlayingThisAnimation(infoItem.selectedBgAnimator, "SelectedBgOpen"))
                {
                    infoItem.QuitSelect();
                }
            }
        }

        /// <summary>
        /// 设置goBattle摁钮的启用
        /// </summary>
        public void SetBtnGoEnterActive(bool active)
        {
            var animatorBtnGoBattle = view.animatorBtnGoBattle;
            if (active)
            {
                animatorBtnGoBattle.SetTrigger("open"); //显示ContinueButton
                return;
            }

            //显示ContinueButton
            if (Utils.IsAnimatorPlayingThisAnimation(animatorBtnGoBattle, "ButtonGoBattleOpen"))
            {
                animatorBtnGoBattle.SetTrigger("close");
            }
        }

        /// <summary>
        /// 设置所有指令面板的激活
        /// </summary>
        public void SetAllMenuActive(bool active)
        {
            foreach (var commandMenu in view.commandMenuList)
            {
                commandMenu.gameObject.SetActive(active);
            }
        }

        /// <summary>
        /// 设置所有指令面板的交互激活启用
        /// </summary>
        public void SetAllMenuInteractable(bool active)
        {
            view.btnUndoCommand.interactable = active;
            var commandMenuList = view.commandMenuList;
            foreach (var commandMenu in commandMenuList)
            {
                commandMenu.SetMenuInteractable(active);
            }
        }

        /// <summary>
        /// 打开buff描述弹窗
        /// </summary>
        /// <param name="buffItem"></param>
        public void OpenBuffDescribe(BuffItem buffItem)
        {
            var describeItemBuff = view.describeItemBuff;
            var buffInfo = buffItem.GetBuffInfo();
            //inspector窗口的string输入时会自动把\n转成\\n ，所以要转回来，不然不换行，艹
            var buffValueString = "";
            if (buffInfo.buffStringParams.Length != 0)
            {
                buffValueString = buffInfo.buffStringParams[0].ToString();
                if (buffInfo.buffStringParams[0] is BattleEntityCtrl)
                {
                    var entity = buffInfo.buffStringParams[0] as BattleEntityCtrl;
                    if (!entity)
                    {
                        Debug.LogError("转换失败" + buffInfo.buffValues[0]);
                        return;
                    }

                    buffValueString = entity.GetName();
                }
            }

            describeItemBuff.Open(string.Format(buffInfo.RowCfgBuff.description, buffValueString), buffItem.gameObject.transform.position + new Vector3(18f, 17f, 0f));
            //强制更新contentSizeFilter，不然更新大小有延迟
            Utils.ForceUpdateContentSizeFilter(describeItemBuff.gameObject.transform);
        }

        /// <summary>
        /// 关闭buff描述弹窗
        /// </summary>
        public void CloseBuffDescribe()
        {
            view.describeItemBuff.Close();
        }

        /// <summary>
        /// 打开技能描述窗口
        /// </summary>
        /// <param name="skillItem"></param>
        public void OpenSkillDescribe(SkillItem skillItem)
        {
            var describeItemSkill = view.describeItemSkill;
            //通过将skillDescribe的缩放xy改为-1，然后字体的缩放xy改为-1，实现向左下角拓展
            describeItemSkill.Open(skillItem.RowCfgSkill.description, skillItem.gameObject.transform.position + new Vector3(-17f, 32.4f, 0f));
            //强制更新contentSizeFilter，不然更新大小有延迟
            Utils.ForceUpdateContentSizeFilter(describeItemSkill.gameObject.transform);
        }

        /// <summary>
        /// 关闭技能描述窗口
        /// </summary>
        public void CloseSkillDescribe()
        {
            view.describeItemSkill.Close();
        }
    }
}