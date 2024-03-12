using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PixelCrushers.DialogueSystem;
using Rabi;
using UnityEngine;
using UnityEngine.UI;


namespace Yu
{
    public class ToolForDialogueSystem : MonoBehaviour
    {
        [SerializeField] private Image imageBg;
        [SerializeField] private CanvasGroup canvasGroupBlackMask;
        [SerializeField] private CanvasGroup canvasGroupBlackMaskWithoutCommonElement;
        [SerializeField] private List<StandardUISubtitlePanel> allSubtitlePanelList=new List<StandardUISubtitlePanel>();
        [SerializeField] private Animator animatorFlash;
        
        /// <summary>
        /// 进入下一个关卡步骤
        /// </summary>
        public void EnterNextStageProcedure()
        {
            StartCoroutine(EnterNextStageProcedureIEnumerator());
        }

        /// <summary>
        /// 切换bgm
        /// </summary>
        public void PlayBgm(string bgmName)
        {
            StartCoroutine(BGMManager.Instance.PlayBgmFadeDelay(bgmName, 0.2f, 0f, 0f, 1f));
        }
        
        /// <summary>
        /// 停止bgm
        /// </summary>
        public void StopBgm()
        {
            StartCoroutine(BGMManager.Instance.StopBgmFadeDelay(0f,0.5f));
        }
        
        /// <summary>
        /// 播放音效
        /// </summary>
        public void PlaySfx(string sfxName)
        {
            SFXManager.Instance.PlaySfx(sfxName);
        }

        /// <summary>
        /// 开始另一个conversation
        /// </summary>
        /// <param name="conversationName"></param>
        public void StartConversation(string conversationName)
        {
            DialogueManager.instance.StopAllConversations();
            DialogueManager.instance.StartConversation(conversationName);
        }
        
        /// <summary>
        /// 停止音效
        /// </summary>
        public void StopSfx(string sfxName)
        {
            StartCoroutine(SFXManager.Instance.StopDelayFadeIEnumerator(sfxName, 0f, 0.5f));
        }

        /// <summary>
        /// 切换abb类型bgm，a和b的bgm名间用'&'分隔
        /// </summary>
        public void PlayLoopBgmWithIntro(string bgmNameAAndBString)
        {
            var bgmNameStringList = bgmNameAAndBString.Split('&');
            if (bgmNameStringList.Length < 2)
            {
                return;
            }

            StartCoroutine(BGMManager.Instance.PlayLoopBgmWithIntro(bgmNameStringList[0], bgmNameStringList[1], 0.2f, 0f, 0f, 1f));
        }

        /// <summary>
        /// 黑幕
        /// </summary>
        public void BlackMask(string fadeInAndDuringAndOutTime)
        {
            var fadeInAndDuringAndOutTimeList = fadeInAndDuringAndOutTime.Split('&');
            if (fadeInAndDuringAndOutTimeList.Length < 3)
            {
                return;
            }
            var fadeInTime = float.Parse(fadeInAndDuringAndOutTimeList[0]);
            var during = float.Parse(fadeInAndDuringAndOutTimeList[1]);
            var fadeOutTime = float.Parse(fadeInAndDuringAndOutTimeList[2]);
            StartCoroutine(BlackMaskIEnumerator(fadeInTime, during, fadeOutTime));
        }

        /// <summary>
        /// 切换背景图
        /// </summary>
        public void ChangeBg(string spriteNameAndFadeInAndDuringAndOutTime)
        {
            var spriteNameAndFadeInAndDuringAndOutTimeList = spriteNameAndFadeInAndDuringAndOutTime.Split('&');
            if (spriteNameAndFadeInAndDuringAndOutTimeList.Length < 4)
            {
                return;
            }

            var spriteName = spriteNameAndFadeInAndDuringAndOutTimeList[0];
            var fadeInTime = float.Parse(spriteNameAndFadeInAndDuringAndOutTimeList[1]);
            var during = float.Parse(spriteNameAndFadeInAndDuringAndOutTimeList[2]);
            var fadeOutTime = float.Parse(spriteNameAndFadeInAndDuringAndOutTimeList[3]);
            StartCoroutine(ChangeBgIEnumerator(spriteName,fadeInTime, during, fadeOutTime));
        }

        /// <summary>
        /// 强制关闭所有立绘
        /// </summary>
        public void CloseAllSubtitlePanel()
        {
            foreach (var subtitlePanel in allSubtitlePanelList)
            {
                subtitlePanel.Close();
            }
        }
        
        /// <summary>
        /// 打开SubtitlePanel
        /// </summary>
        /// <param name="index"></param>
        public void OpenSubtitlePanel(int index )
        {
            if (index>=allSubtitlePanelList.Count)
            {
                Debug.LogError(index+"超出范围"+allSubtitlePanelList.Count);
                return;
            }
            allSubtitlePanelList[index].Open();
        }

        /// <summary>
        /// 除了CommonElement之外其他淡入黑幕
        /// </summary>
        public void FadeInBlackWithoutCommonElement(float fadeInTime)
        {
            StartCoroutine(FadeInBlackWithoutCommonElementIEnumerator(fadeInTime));
        }
        
        /// <summary>
        /// 除了CommonElement之外其他淡出黑幕
        /// </summary>
        public void FadeOutBlackWithoutCommonElement(float fadeOutTime)
        {
            StartCoroutine(FadeOutBlackWithoutCommonElementIEnumerator(fadeOutTime));
        }

        /// <summary>
        /// 闪白特效
        /// </summary>
        public void VFXFlash()
        {
            animatorFlash.SetTrigger("flash");
        }

        /// <summary>
        /// FadeInBlackWithoutCommonElement的协程
        /// </summary>
        private IEnumerator FadeInBlackWithoutCommonElementIEnumerator(float fadeInTime)
        {
            canvasGroupBlackMaskWithoutCommonElement.alpha = 0;
            var tweener = canvasGroupBlackMaskWithoutCommonElement.DOFade(1, fadeInTime);
            canvasGroupBlackMaskWithoutCommonElement.gameObject.SetActive(true);
            yield return tweener.WaitForCompletion();
        }
        
        /// <summary>
        /// FadeOutBlackWithoutCommonElement的协程
        /// </summary>
        private IEnumerator FadeOutBlackWithoutCommonElementIEnumerator(float fadeOutTime)
        {
            var tweener = canvasGroupBlackMaskWithoutCommonElement.DOFade(0, fadeOutTime);
            yield return tweener.WaitForCompletion();
            canvasGroupBlackMaskWithoutCommonElement.gameObject.SetActive(false);
        }

        /// <summary>
        /// ChangeBg的协程
        /// </summary>
        private IEnumerator ChangeBgIEnumerator(string spriteName,float fadeInTime, float during, float fadeOutTime)
        {
            canvasGroupBlackMask.alpha = 0;
            var tweener = canvasGroupBlackMask.DOFade(1, fadeInTime);
            canvasGroupBlackMask.gameObject.SetActive(true);
            yield return tweener.WaitForCompletion();
            imageBg.sprite = AssetManager.Instance.LoadAsset<Sprite>(ConfigManager.Instance.cfgSprite[spriteName].path);
            yield return new WaitForSeconds(during);
            tweener = canvasGroupBlackMask.DOFade(0, fadeOutTime);
            yield return tweener.WaitForCompletion();
            canvasGroupBlackMask.gameObject.SetActive(false);
        }

        /// <summary>
        /// 黑幕协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator BlackMaskIEnumerator(float fadeInTime, float during, float fadeOutTime)
        {
            canvasGroupBlackMask.alpha = 0;
            var tweener = canvasGroupBlackMask.DOFade(1, fadeInTime);
            canvasGroupBlackMask.gameObject.SetActive(true);
            yield return tweener.WaitForCompletion();
            yield return new WaitForSeconds(during);
            tweener = canvasGroupBlackMask.DOFade(0, fadeOutTime);
            yield return tweener.WaitForCompletion();
            canvasGroupBlackMask.gameObject.SetActive(false);
        }

        /// <summary>
        /// 进入下一个关卡步骤协程
        /// </summary>
        private IEnumerator EnterNextStageProcedureIEnumerator()
        {
            UIManager.Instance.OpenWindow("LoadingView");
            yield return new WaitForSeconds(0.3f);
            DialogueManager.instance.StopAllConversations();
            GameManager.Instance.EnterNextStageProcedure();
        }
    }
}