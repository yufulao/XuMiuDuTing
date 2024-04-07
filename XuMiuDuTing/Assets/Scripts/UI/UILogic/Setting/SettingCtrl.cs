using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PixelCrushers.DialogueSystem;
using Rabi;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Yu
{
    public class SettingCtrl : UICtrlBase
    {
        private SettingModel _model;
        private SettingView _view;


        public override void OnInit(params object[] param)
        {
            _model = new SettingModel();
            _view = GetComponent<SettingView>();
            _model.OnInit();
            EventManager.Instance.AddListener(EventName.OnPauseViewClose,CloseRoot);
        }

        public override void OpenRoot(params object[] param)
        {
            _view.OpenWindow();
            //pause剧情，pause界面打开时已经pause过了，不重复pause
            if (!UIManager.Instance.CheckViewActiveInHierarchy("PauseView"))
            {
                DialogueManager.instance.Pause();
            }
            
            UpdateAllSetting();
        }

        public override void CloseRoot()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            _view.CloseWindow();
            //unpause剧情，暂停界面打开的话不unpause，由pause界面关闭时unpause
            if (!UIManager.Instance.CheckViewActiveInHierarchy("PauseView"))
            {
                DialogueManager.instance.Unpause();
            }
        }
        
        public override void BindEvent()
        {
            _view.btnDeveloper.onClick.AddListener(BtnOnClickDeveloper);
            _view.btnDeveloperPanel.onClick.AddListener(BtnOnClickDeveloperPanel);
            _view.btnBack.onClick.AddListener(BtnOnClickBack);
            _view.sliderBGM.onValueChanged.AddListener(SliderOnValueChangeBGM);
            _view.sliderSe.onValueChanged.AddListener(SliderOnValueChangeSe);
            _view.sliderVoice.onValueChanged.AddListener(SliderOnValueChangeVoice);
            //_view.sliderAutoSpeed.onValueChanged.AddListener(SliderOnValueChangeAutoSpeed);//无需操作
            _view.toggleFullscreen.onValueChanged.AddListener(ToggleOnValueChangeFullscreen);
            _view.toggleAuto.onValueChanged.AddListener(ToggleOnValueChangeAuto);
            _view.dropdownResolution.onValueChanged.AddListener(DropdownOnValueChangeResolution);
        }

        /// <summary>
        /// 开发者名单
        /// </summary>
        private void BtnOnClickDeveloper()
        {
            _view.btnDeveloperPanel.interactable = true;
            _view.animatorDeveloperPanel.gameObject.SetActive(true);
            _view.animatorDeveloperPanel.SetTrigger("Show");
        }

        /// <summary>
        /// 开发者名单关闭
        /// </summary>
        private void BtnOnClickDeveloperPanel()
        {
            GameManager.Instance.StartCoroutine(BtnOnClickDeveloperPanelIEnumerator());
        }

        /// <summary>
        /// 关闭开发者名单的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator BtnOnClickDeveloperPanelIEnumerator()
        {
            _view.btnDeveloperPanel.interactable = false;
            yield return Utils.PlayAnimation(_view.animatorDeveloperPanel, "Hide");
            _view.animatorDeveloperPanel.gameObject.SetActive(false);
        }


        /// <summary>
        /// 返回
        /// </summary>
        private void BtnOnClickBack()
        {
            SaveAll();
            CloseRoot();
        }

        /// <summary>
        /// 保存所有
        /// </summary>
        private void SaveAll()
        {
            SaveManager.SetFloat(DefSFXType.DSe + "Volume", _view.sliderSe.value <= _view.sliderSe.minValue ? -100f : _view.sliderSe.value);
            SaveManager.SetFloat(DefSFXType.DVoice + "Volume", _view.sliderVoice.value <= _view.sliderVoice.minValue ? -100f : _view.sliderVoice.value);
            SaveManager.SetFloat("BGMVolume", _view.sliderBGM.value <= _view.sliderBGM.minValue ? -100f : _view.sliderBGM.value);
            SaveManager.SetFloat("AutoSpeed", _view.sliderAutoSpeed.value);
        }

        /// <summary>
        /// BGM音量
        /// </summary>
        /// <param name="value"></param>
        private void SliderOnValueChangeBGM(float value)
        {
            if (value <= _view.sliderBGM.minValue)
            {
                BGMManager.Instance.MuteVolume();
                return;
            }

            BGMManager.Instance.SetVolumeRuntime(value);
        }

        /// <summary>
        /// Se音量
        /// </summary>
        /// <param name="value"></param>
        private void SliderOnValueChangeSe(float value)
        {
            if (value <= _view.sliderSe.minValue)
            {
                SFXManager.Instance.MuteVolume(DefSFXType.DSe);
                return;
            }

            SFXManager.Instance.SetVolumeRuntime(DefSFXType.DSe, value);
        }

        /// <summary>
        /// Voice音量
        /// </summary>
        /// <param name="value"></param>
        private void SliderOnValueChangeVoice(float value)
        {
            if (value <= _view.sliderSe.minValue)
            {
                SFXManager.Instance.MuteVolume(DefSFXType.DVoice);
                return;
            }

            SFXManager.Instance.SetVolumeRuntime(DefSFXType.DVoice, value);
        }

        /// <summary>
        /// 是否全屏
        /// </summary>
        /// <param name="value"></param>
        private void ToggleOnValueChangeFullscreen(bool value)
        {
            Screen.fullScreen = value;
            SaveManager.SetInt("Fullscreen", value ? 1 : 0);
            DropdownOnValueChangeResolution(_model.GetResolutionIndexInSave()); //重新应用分辨率
        }

        /// <summary>
        /// 是否Auto
        /// </summary>
        /// <param name="value"></param>
        private static void ToggleOnValueChangeAuto(bool value)
        {
            SaveManager.SetInt("Auto", value ? 1 : 0);
        }

        /// <summary>
        /// 点击设置了新的分辨率
        /// </summary>
        /// <param name="index"></param>
        private void DropdownOnValueChangeResolution(int index)
        {
            if (index < 0 || index >= _model.GetResolutionStringList().Count)
            {
                return;
            }

            //转成Screen.resolutions中的index
            var resolutionIndexInScreen = _model.ResolutionDropdownIndexToScreenResolutionsIndex(index);
            if (resolutionIndexInScreen < 0 || resolutionIndexInScreen >= Screen.resolutions.Length)
            {
                return;
            }

            //应用分辨率
            var resolution = Screen.resolutions[resolutionIndexInScreen];
            Screen.SetResolution(resolution.width, resolution.height, SettingModel.GetFullscreenInSave());
            SaveManager.SetInt("Resolution", index);
        }

        /// <summary>
        /// 重新导入所有值
        /// </summary>
        private void UpdateAllSetting()
        {
            UpdateDropdownResolution();
            _view.RefreshAllSetting();
        }

        /// <summary>
        /// 更新分辨率下拉列表
        /// </summary>
        private void UpdateDropdownResolution()
        {
            var resolutionStringList = _model.GetResolutionStringList();
            var dropdownIndex = Mathf.Clamp(_model.GetResolutionIndexInSave(), 0, resolutionStringList.Count - 1);
            _view.RefreshResolutionDropdown(resolutionStringList, dropdownIndex);
        }
    }
}