using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public TextMeshProUGUI textStageName;
        public TextMeshProUGUI textStageDesc;
        public Transform stageFrameContainer;
        public GameObject objMask;
        [HideInInspector] public ParticleSystem vfxFogRightBottom;

        public CanvasGroup objStageInfoBgCanvasGroup;
        public CanvasGroup canvasGroupFadeMask;
        public Animator animator;

        public void Init()
        {
            vfxFogRightBottom = UIManager.Instance.GetUIRoot().Find("VFX").Find("ParticleFogMainPlot").GetComponent<ParticleSystem>();
        }

        /// <summary>
        /// 刷新所有视图
        /// </summary>
        public void RefreshAll(RowCfgMainPlot rowCfgMainPlot, RowCfgStage rowCfgStage)
        {
            imagePlotBg.sprite = AssetManager.Instance.LoadAsset<Sprite>(rowCfgMainPlot.plotBG);
            SetVFXFogActive(rowCfgMainPlot.vfxFogActive);

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
            textStageDesc.text = stageDesc?.Replace("\\n", "\n");
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
        /// 开启关闭右下角迷雾特效
        /// </summary>
        public void SetVFXFogActive(bool active)
        {
            if (active)
            {
                vfxFogRightBottom.gameObject.SetActive(true);
                vfxFogRightBottom.Play();
                return;
            }

            vfxFogRightBottom.Stop();
            vfxFogRightBottom.gameObject.SetActive(false);
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
            SetVFXFogActive(false);
            objMask.SetActive(true);
            yield return Utils.PlayAnimation(animator, "Hide");
            gameObject.SetActive(false);
        }
    }
}