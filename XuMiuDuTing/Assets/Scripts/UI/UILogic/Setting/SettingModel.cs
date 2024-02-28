using System.Collections.Generic;
using UnityEngine;

namespace Yu
{
    public class SettingModel
    {
        private readonly List<string> _resolutionStringList = new List<string>();

        public void OnInit()
        {
            ReloadResolutionStringList();
        }
        
        /// <summary>
        /// 获取存档是否全屏
        /// </summary>
        /// <returns></returns>
        public static bool GetFullscreenInSave()
        {
            return SaveManager.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        }
        
        /// <summary>
        /// 获取存档的分辨率的index
        /// </summary>
        /// <returns></returns>
        public int GetResolutionIndexInSave()
        {
            return SaveManager.GetInt("Resolution", GetCurrentResolutionDropdownIndex());
        }
        
        /// <summary>
        /// 获取当前下拉列表的value在Screen.resolutions中的index
        /// </summary>
        /// <param name="dropdownIndex"></param>
        /// <returns></returns>
        public int ResolutionDropdownIndexToScreenResolutionsIndex(int dropdownIndex)
        {
            if (dropdownIndex >= 0 && dropdownIndex < _resolutionStringList.Count)
            {
                var dropdownString = _resolutionStringList[dropdownIndex];
                for (var i = 0; i < Screen.resolutions.Length; i++)
                {
                    if (string.Equals(GetResolutionString(Screen.resolutions[i]), dropdownString))
                    {
                        return i;
                    }
                }
            }

            for (var i = 0; i < Screen.resolutions.Length; i++)
            {
                if (Equals(Screen.resolutions[i], Screen.currentResolution))
                {
                    return i;
                }
            }

            return 0;
        }

        /// <summary>
        /// 获取resolutionStringList
        /// </summary>
        /// <returns></returns>
        public List<string> GetResolutionStringList()
        {
            return _resolutionStringList;
        }
        
        /// <summary>
        /// 设置下拉列表的分辨率string列表
        /// </summary>
        private void ReloadResolutionStringList()
        {
            _resolutionStringList.Clear();
            foreach (var resolution in Screen.resolutions)
            {
                _resolutionStringList.Add(GetResolutionString(resolution));
            }
        }
        
        /// <summary>
        /// 获取分辨率的字符串显示
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        private static string GetResolutionString(Resolution resolution)
        {
            return (resolution.refreshRateRatio.value > 0)
                ? (resolution.width + "x" + resolution.height + " " + resolution.refreshRateRatio.value + "Hz")
                : (resolution.width + "x" + resolution.height);
        }

        /// <summary>
        /// 获取当前分辨率在下拉列表的index
        /// </summary>
        /// <returns></returns>
        private int GetCurrentResolutionDropdownIndex()
        {
            var currentString = GetResolutionString(Screen.currentResolution);
            for (var i = 0; i < _resolutionStringList.Count; i++)
            {
                if (string.Equals(_resolutionStringList[i], currentString)) return i;
            }

            return 0;
        }
    }
}
