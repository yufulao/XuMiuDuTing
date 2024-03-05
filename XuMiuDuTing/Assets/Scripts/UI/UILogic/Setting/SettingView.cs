using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yu
{
    public class SettingView : MonoBehaviour
    {
        public Button btnCredit;
        public Button btnBack;
        public Slider sliderBGM;
        public Slider sliderSe;
        public Slider sliderVoice;
        public Toggle toggleFullscreen;
        public Toggle toggleAuto;
        public Dropdown dropdownResolution;
        public GameObject objMask;

        public Animator animator;
        
        /// <summary>
        /// 从设置中获取所有值并刷新状态
        /// </summary>
        public void RefreshAllSetting()
        {
            sliderBGM.value = SaveManager.GetFloat("BGMVolume", 0f);
            sliderSe.value = SaveManager.GetFloat("SeVolume", 0f);
            sliderVoice.value = SaveManager.GetFloat("VoiceVolume", 0f);
            toggleFullscreen.isOn = SettingModel.GetFullscreenInSave();
            toggleAuto.isOn = SaveManager.GetInt("Auto", 0) == 1;
            //分辨率下拉列表在ctrl的init里
        }
        
        /// <summary>
        /// 刷新分辨率下拉列表的选项
        /// </summary>
        /// <param name="resolutionStringList"></param>
        /// <param name="dropdownIndex"></param>
        public void RefreshResolutionDropdown(List<string> resolutionStringList,int dropdownIndex)
        {
            dropdownResolution.ClearOptions();
            dropdownResolution.AddOptions(resolutionStringList);
            dropdownResolution.value = dropdownIndex;
            dropdownResolution.captionText.text = resolutionStringList[dropdownIndex];
        }
        
        public void RefreshResolution()
        {
            dropdownResolution.value = SaveManager.GetInt("Resolution", 0);
        }
        
        /// <summary>
        /// 打开窗口
        /// </summary>
        public void OpenWindow()
        {
            objMask.SetActive(false);
            gameObject.SetActive(true);
            animator.Play("Show", 0, 0f);
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public void CloseWindow()
        {
            GameManager.Instance.StartCoroutine(CloseWindowIEnumerator());
        }

        /// <summary>
        /// 关闭窗口的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator CloseWindowIEnumerator()
        {
            objMask.SetActive(true);
            yield return Utils.PlayAnimation(animator, "Hide");
            gameObject.SetActive(false);
        }
    }
}