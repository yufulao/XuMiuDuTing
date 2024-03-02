using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rabi;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

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
            //CameraManager.Instance.SwitchObjCamera(CameraManager.ObjCameraState.IdleCommand, 0f);
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
                commandMenu.btnAttack.SetClickEnable(BtnOnClickAttack);
                commandMenu.btnBrave.SetClickEnable(BtnOnClickBrave);
                commandMenu.btnSkill.SetClickEnable(BtnOnClickSkill);
                commandMenu.btnUniqueSkill.SetClickEnable(BtnOnClickUniqueSkill);
                commandMenu.btnDefault.SetClickDisable();
            }
            view.commandMenuList[0].btnDefault.SetClickEnable(BtnOnClickDefault);
            view.commandMenuList[3].btnBrave.SetClickDisable();

            //SkillSelectPanel绑定
            view.skillSelectPanel.SetSkillItemOnClick(OnSkillSelectItemClick);
            view.aimSelectPanel.toggleSwitchCamera.onValueChanged.AddListener(OnValueChangeToggleSwitchCamera);
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
        /// 指令面板Attack点击事件
        /// </summary>
        public void BtnOnClickAttack()
        {
            
        }

        /// <summary>
        /// 指令面板Brave点击事件
        /// </summary>
        public void BtnOnClickBrave()
        {
            
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
            
        }

        /// <summary>
        /// 点击技能item事件
        /// </summary>
        /// <param name="skillName"></param>
        public void OnSkillSelectItemClick(string skillName)
        {
            
        }

        /// <summary>
        /// 点击切换摄像机toggle
        /// </summary>
        /// <param name="isOn"></param>
        public void OnValueChangeToggleSwitchCamera(bool isOn)
        {
            
        }
    }
}