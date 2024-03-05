using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;
using UnityEngine.UI;

namespace Yu
{
    public class TeamEditView : MonoBehaviour
    {
        public Button btnBack;
        public Button btnEnter;
        public GameObject objTeamFixedMask;
        public GameObject objMask;
        public Image imagePlotBg;
        public List<TeamItem> teamItemList = new List<TeamItem>();
        public Animator animator;

        /// <summary>
        /// 打开窗口
        /// </summary>
        public void OpenWindow(IReadOnlyList<object> param)
        {
            objMask.SetActive(false);
            gameObject.SetActive(true);
            animator.Play("Show", 0, 0f);
            RefreshPlotBg(param);
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

        /// <summary>
        /// 设置编队界面的背景PlotBg
        /// </summary>
        private void RefreshPlotBg(IReadOnlyList<object> param)
        {
            if (param.Count < 2)
            {
                Debug.LogError("没有传入Plot信息");
                return;
            }

            var chapterType = param[0].ToString();
            var plotName = param[1].ToString();
            if (chapterType == DefChapterType.DMainPlot)
            {
                imagePlotBg.sprite = AssetManager.Instance.LoadAsset<Sprite>(ConfigManager.Instance.cfgMainPlot[plotName].plotBG);
            }

            if (chapterType == DefChapterType.DSubPlot)
            {
                imagePlotBg.sprite = AssetManager.Instance.LoadAsset<Sprite>(ConfigManager.Instance.cfgSubPlot[plotName].plotBG);
            }
        }
    }
}