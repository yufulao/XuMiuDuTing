using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rabi;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Yu
{
    public class MainPlotSelectView : MonoBehaviour
    {
        public Button btnBack;
        public Button btnEnter;
        public Button btnNextPlot;
        public Button btnPrePlot;
        public Image imagePlotBg;
        public Image imagePlotName;
        public List<StageItem> stageItemList = new List<StageItem>();
        public TextMeshProUGUI textStageName;
        public TextMeshProUGUI textStageDesc;
        public CanvasGroup objStageInfoBgCanvasGroup;
        public CanvasGroup canvasGroupFadeMask;
        public Animator animator;

        /// <summary>
        /// 刷新所有视图
        /// </summary>
        public void RefreshAll(RowCfgMainPlot rowCfgMainPlot, RowCfgStage rowCfgStage)
        {
            imagePlotBg.sprite = AssetManager.Instance.LoadAsset<Sprite>(rowCfgMainPlot.plotBG);
            if (!string.IsNullOrEmpty(rowCfgMainPlot.plotTitle))
            {
                imagePlotName.sprite = AssetManager.Instance.LoadAsset<Sprite>(rowCfgMainPlot.plotTitle);
            }

            if (rowCfgStage == null)
            {
                RefreshStageInfo(null, null);
                return;
            }

            RefreshStageInfo(rowCfgStage.stageName, rowCfgStage.stageDesc);
        }

        /// <summary>
        /// 刷新关卡信息
        /// </summary>
        /// <param name="stageName"></param>
        /// <param name="stageDesc"></param>
        public void RefreshStageInfo(string stageName, string stageDesc)
        {
            textStageName.text = stageName;
            textStageDesc.text = stageDesc;
            if (string.IsNullOrEmpty(stageName))
            {
                objStageInfoBgCanvasGroup.DOFade(0f, 0.2f);
                btnEnter.gameObject.SetActive(false);
                return;
            }

            objStageInfoBgCanvasGroup.DOFade(1f, 0.2f);
            btnEnter.gameObject.SetActive(true);
        }

        /// <summary>
        /// 强制关闭关卡信息视窗============================================================================================
        /// </summary>
        public void ForceCloseStageInfo()
        {
            textStageName.text = null;
            textStageDesc.text = null;
            objStageInfoBgCanvasGroup.DOFade(0f, 0.2f);
            btnEnter.gameObject.SetActive(false);
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        public void OpenWindow()
        {
            btnBack.interactable = true;
            gameObject.SetActive(true);
            animator.Play("Show", 0, 0f);
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public void CloseWindow()
        {
            StartCoroutine(CloseWindowIEnumerator());
        }

        /// <summary>
        /// 关闭窗口的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator CloseWindowIEnumerator()
        {
            btnBack.interactable = false;
            yield return Utils.PlayAnimation(animator, "Hide");
            gameObject.SetActive(false);
        }
    }
}