using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PixelCrushers.DialogueSystem;
using Rabi;
using SCPE;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using StandardUIContinueButtonFastForward = PixelCrushers.DialogueSystem.Wrappers.StandardUIContinueButtonFastForward;


namespace Yu
{
    public class ToolForDialogueSystem : MonoBehaviour
    {
        [SerializeField] private Image imageBg;
        [SerializeField] private CanvasGroup canvasGroupBlackMask;
        [SerializeField] private CanvasGroup canvasGroupBlackMaskWithoutCommonElement;
        [SerializeField] private CanvasGroup canvasGroupContinueButtonBg;
        [SerializeField] private GameObject objContinueButtonMask;
        [SerializeField] private List<StandardUISubtitlePanel> allSubtitlePanelList = new List<StandardUISubtitlePanel>();
        [SerializeField] private List<Animator> allSubtitlePanelAnimatorList = new List<Animator>();
        [SerializeField] private List<Animator> allSubPortraitAnimatorList = new List<Animator>();
        [SerializeField] private Volume vfxVolume;

        [SerializeField] private Animator animatorFlash;
        [SerializeField] private ParticleSystem particleRain;
        [SerializeField] private ParticleSystem particleFog;
        [SerializeField] private ParticleSystem particleFire;

        /// <summary>
        /// 对话开始之前
        /// </summary>
        public void OnConversationStart()
        {
            ResetAllSubPortraitAnimator();
            ResetAllSubtitlePanelAnimator();
            objContinueButtonMask.SetActive(false);
        }

        /// <summary>
        /// 关闭conversation时,交给dialogueManager调用
        /// </summary>
        public void OnConversationStop()
        {
            VFXRain(false);
            VFXFog(false);
            VFXFire(false);
            VFXRipple(false);
            VFXChromaticAberration(false);
        }

        /// <summary>
        /// 打开暂停界面
        /// </summary>
        public void OpenPauseView()
        {
            UIManager.Instance.OpenWindow("PauseView");
        }

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
            StartCoroutine(BGMManager.Instance.StopBgmFadeDelay(0f, 0.5f));
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        public void PlaySfx(string sfxName)
        {
            SFXManager.Instance.PlaySfx(sfxName);
        }

        /// <summary>
        /// 只能等待指定时间自动进入下一个对话内容，点击无效，进入后恢复点击可用
        /// </summary>
        public void ContinueByAutoCantClick(float during)
        {
            StartCoroutine(ContinueByAutoCantClickIEnumerator(during));
        }

        /// <summary>
        /// 重置所有SubtitlePanelAnimator的float参数
        /// </summary>
        public void ResetAllSubtitlePanelAnimator()
        {
            foreach (var animator in allSubtitlePanelAnimatorList)
            {
                if (!animator.gameObject.activeInHierarchy)
                {
                    continue;
                }

                animator.gameObject.SetActive(false); //Animator的gameObject重新active会将所有param重置
                animator.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 重置所有辅助立绘的float参数
        /// </summary>
        public void ResetAllSubPortraitAnimator()
        {
            foreach (var animator in allSubPortraitAnimatorList)
            {
                if (!animator.gameObject.activeInHierarchy)
                {
                    continue;
                }

                animator.gameObject.SetActive(false); //Animator的gameObject重新active会将所有param重置
                animator.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 设置animator的float参数
        /// </summary>
        /// <param name="indexAndKeyAndValue"></param>
        public void SetSubtitlePanelAnimatorFloatParam(string indexAndKeyAndValue)
        {
            var indexAndPlayRateList = indexAndKeyAndValue.Split('&');
            if (indexAndPlayRateList.Length < 3)
            {
                return;
            }

            var index = int.Parse(indexAndPlayRateList[0]);
            var animator = allSubtitlePanelAnimatorList[index];
            var key = indexAndPlayRateList[1];
            var value = float.Parse(indexAndPlayRateList[2]);
            if (!animator.gameObject.activeInHierarchy)
            {
                animator.gameObject.transform.GetComponent<CanvasGroup>().alpha = 0;
                animator.gameObject.SetActive(true);
            }

            animator.SetFloat(key, value);
        }

        /// <summary>
        /// 设置辅助立绘animator的float参数
        /// </summary>
        /// <param name="indexAndKeyAndValue"></param>
        public void SetSubPortraitAnimatorFloatParam(string indexAndKeyAndValue)
        {
            var indexAndPlayRateList = indexAndKeyAndValue.Split('&');
            if (indexAndPlayRateList.Length < 3)
            {
                return;
            }

            var index = int.Parse(indexAndPlayRateList[0]);
            var animator = allSubPortraitAnimatorList[index];
            var key = indexAndPlayRateList[1];
            var value = float.Parse(indexAndPlayRateList[2]);
            if (!animator.gameObject.activeInHierarchy)
            {
                animator.gameObject.transform.GetComponent<CanvasGroup>().alpha = 0;
                animator.gameObject.SetActive(true);
            }

            animator.SetFloat(key, value);
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
            StartCoroutine(ChangeBgIEnumerator(spriteName, fadeInTime, during, fadeOutTime));
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
        public void OpenSubtitlePanel(int index)
        {
            if (index >= allSubtitlePanelList.Count)
            {
                Debug.LogError(index + "超出范围" + allSubtitlePanelList.Count);
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
        /// 下雨特效
        /// </summary>
        public void VFXRain(bool active)
        {
            if (active)
            {
                particleRain.gameObject.SetActive(true);
                particleRain.Play();
                return;
            }

            particleRain.Stop();
            particleRain.gameObject.SetActive(false);
        }

        /// <summary>
        /// 迷雾特效
        /// </summary>
        public void VFXFog(bool active)
        {
            if (active)
            {
                particleFog.gameObject.SetActive(true);
                particleFog.Play();
                return;
            }

            particleFog.Stop();
            particleFog.gameObject.SetActive(false);
        }

        /// <summary>
        /// 火焰特效
        /// </summary>
        public void VFXFire(bool active)
        {
            if (active)
            {
                particleFire.gameObject.SetActive(true);
                particleFire.Play();
                return;
            }

            particleFire.Stop();
            particleFire.gameObject.SetActive(false);
        }

        /// <summary>
        /// 液化特效
        /// </summary>
        /// <param name="active"></param>
        public void VFXRipple(bool active)
        {
            if (!vfxVolume.profile.TryGet<Ripples>(out var ripples))
            {
                return;
            }

            if (active)
            {
                //startValue,param,endValue,during
                DOTween.To(() => 0f, x => ripples.strength.value = x, 1f, 1f);
                return;
            }

            var strength = ripples.strength.value;
            if (strength > 0f)
            {
                DOTween.To(() => strength, x => ripples.strength.value = x, 0f, 1f);
            }
        }
        
        /// <summary>
        /// 色差特效
        /// </summary>
        /// <param name="active"></param>
        public void VFXChromaticAberration(bool active)
        {
            if (!vfxVolume.profile.TryGet<ChromaticAberration>(out var chromaticAberration))
            {
                return;
            }

            if (active)
            {
                //startValue,param,endValue,during
                DOTween.To(() => 0f, x => chromaticAberration.intensity.value = x, 0.3f, 1f);
                return;
            }

            var intensity = chromaticAberration.intensity.value;
            if (intensity > 0f)
            {
                DOTween.To(() => intensity, x => chromaticAberration.intensity.value = x, 0f, 1f);
            }
        }

        /// <summary>
        /// ContinueByAutoCantClick的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator ContinueByAutoCantClickIEnumerator(float during)
        {
            objContinueButtonMask.SetActive(true);
            yield return new WaitForSeconds(during);
            DialogueManager.instance.PlaySequence("Continue()");
            objContinueButtonMask.SetActive(false);
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
        private IEnumerator ChangeBgIEnumerator(string spriteName, float fadeInTime, float during, float fadeOutTime)
        {
            canvasGroupBlackMask.alpha = 0;
            canvasGroupBlackMask.gameObject.SetActive(true);
            var tweener = canvasGroupBlackMask.DOFade(1, fadeInTime);
            yield return tweener.WaitForCompletion();
            imageBg.sprite = AssetManager.Instance.LoadAsset<Sprite>(ConfigManager.Instance.cfgSprite[spriteName].path);
            yield return new WaitForSeconds(during);
            DialogueManager.instance.PlaySequence("Continue()");
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