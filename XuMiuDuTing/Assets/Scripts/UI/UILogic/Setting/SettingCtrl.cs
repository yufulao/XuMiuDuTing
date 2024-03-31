using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rabi;
using UnityEngine;
using UnityEngine.Rendering;

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
        }

        public override void OpenRoot(params object[] param)
        {
            _view.OpenWindow();
            UpdateAllSetting();
        }

        public override void CloseRoot()
        {
            _view.CloseWindow();
        }

        public override void BindEvent()
        {
            _view.btnCredit.onClick.AddListener(BtnOnClickCredit);
            _view.btnBack.onClick.AddListener(BtnOnClickBack);
            _view.sliderBGM.onValueChanged.AddListener(SliderOnValueChangeBGM);
            _view.sliderSe.onValueChanged.AddListener(SliderOnValueChangeSe);
            _view.sliderVoice.onValueChanged.AddListener(SliderOnValueChangeVoice);
            _view.toggleFullscreen.onValueChanged.AddListener(ToggleOnValueChangeFullscreen);
            _view.toggleAuto.onValueChanged.AddListener(ToggleOnValueChangeAuto);
            _view.dropdownResolution.onValueChanged.AddListener(DropdownOnValueChangeResolution);
        }

        /// <summary>
        /// 开发者名单
        /// </summary>
        private static void BtnOnClickCredit()
        {
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