using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PixelCrushers.DialogueSystem;
using Rabi;
using SCPE;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using StandardUIContinueButtonFastForward = PixelCrushers.DialogueSystem.Wrappers.StandardUIContinueButtonFastForward;


namespace Yu
{
    public class ToolForDialogueSystem : MonoBehaviour
    {
        [SerializeField] private Image imageBg;
        private Material _materialBg;
        [SerializeField] private Animator imageBgAddonAnimator;
        [SerializeField] private CanvasGroup canvasGroupBlackMask;
        [SerializeField] private CanvasGroup canvasGroupBlackMaskBg;
        [SerializeField] private CanvasGroup canvasGroupBlackMaskWithoutCommonElement;
        [SerializeField] private CanvasGroup canvasGroupThinkingBg;
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

        private void Start()
        {
            CloneMaterialBg();
        }

        /// <summary>
        /// 对话开始之前
        /// </summary>
        public void OnConversationStart()
        {
            canvasGroupBlackMaskBg.alpha = 0f;
            canvasGroupBlackMaskWithoutCommonElement.alpha = 0f;
            canvasGroupThinkingBg.alpha = 0f;
            ResetAllSubPortraitAnimator();
            ResetAllSubtitlePanelAnimator();
            objContinueButtonMask.SetActive(false);
            imageBgAddonAnimator.SetTrigger("Hide");
            CloseAllVFX();
        }

        /// <summary>
        /// 关闭conversation时,交给dialogueManager调用
        /// </summary>
        public void OnConversationStop()
        {
            CloseAllVFX();
            SFXManager.Instance.StopAllSfx();
        }

        /// <summary>
        /// 关闭所有特效
        /// </summary>
        public void CloseAllVFX()
        {
            VFXRain(false);
            VFXFog(false);
            VFXFire(false);
            // VFXColorAdjustmentsSaturation(0f);
            ForceResetVFXProcessing();
            VFXScreenWater(false);
        }

        /// <summary>
        /// 设置btn点击时
        /// </summary>
        public void OnBtnClickSetting()
        {
            UIManager.Instance.OpenWindow("SettingView");
        }

        /// <summary>
        /// skip点击时
        /// </summary>
        public void OnBtnClickSkip()
        {
            UIManager.Instance.OpenWindow("DoubleConfirmView", "确定要退出剧情吗", new UnityAction(EnterNextStageProcedure), null);
        }

        /// <summary>
        /// 是否自动播放
        /// </summary>
        public void AutoContinue(bool auto)
        {
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
        /// 强制关闭所有辅助
        /// </summary>
        public void CloseAllSubPortrait()
        {
            foreach (var subPortraitAnimator in allSubPortraitAnimatorList)
            {
                subPortraitAnimator.SetTrigger("Hide");
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
        /// bg淡入黑幕
        /// </summary>
        public void FadeInBlackBg(float fadeInTime)
        {
            StartCoroutine(FadeInBlackBgIEnumerator(fadeInTime));
        }

        /// <summary>
        /// bg淡出黑幕
        /// </summary>
        public void FadeOutBlackBg(float fadeOutTime)
        {
            StartCoroutine(FadeOutBlackBgIEnumerator(fadeOutTime));
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
        /// 内心活动bg淡入
        /// </summary>
        public void FadeInThinkingBg(float fadeInTime)
        {
            StartCoroutine(FadeInThinkingBgIEnumerator(fadeInTime));
        }

        /// <summary>
        /// 内心活动bg淡出
        /// </summary>
        public void FadeOutThinkingBg(float fadeOutTime)
        {
            StartCoroutine(FadeOutThinkingBgIEnumerator(fadeOutTime));
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
        /// <param name="rate"></param>
        public void VFXRipple(float rate)
        {
            if (!vfxVolume.profile.TryGet<Ripples>(out var ripples))
            {
                return;
            }

            var strength = ripples.strength.value;
            //startValue,param,endValue,during
            DOTween.To(() => strength, x => ripples.strength.value = x, rate * 1f, 1f);
        }

        /// <summary>
        /// 色差特效
        /// </summary>
        /// <param name="rate"></param>
        public void VFXChromaticAberration(float rate)
        {
            if (!vfxVolume.profile.TryGet<ChromaticAberration>(out var chromaticAberration))
            {
                return;
            }

            var intensity = chromaticAberration.intensity.value;
            //startValue,param,endValue,during
            DOTween.To(() => intensity, x => chromaticAberration.intensity.value = x, rate * 0.3f, 1f);
        }

        /// <summary>
        /// 饱和度特效
        /// </summary>
        /// <param name="rate"></param>
        public void VFXColorAdjustmentsSaturation(float rate)
        {
            if (!vfxVolume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
            {
                return;
            }

            var saturation = colorAdjustments.saturation.value;
            //startValue,param,endValue,during
            DOTween.To(() => saturation, x => colorAdjustments.saturation.value = x, 0 + rate * 100f, 1f);
        }
        
        /// <summary>
        /// 后处理恢复有问题，强制清零
        /// </summary>
        private void ForceResetVFXProcessing()
        {
            if (!vfxVolume.profile.TryGet<Ripples>(out var ripples))
            {
                return;
            }

            ripples.strength.value=0f;
            
            if (!vfxVolume.profile.TryGet<ChromaticAberration>(out var chromaticAberration))
            {
                return;
            }

            chromaticAberration.intensity.value=0f;
            
            if (!vfxVolume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
            {
                return;
            }
            colorAdjustments.saturation.value=0f;
        }

        /// <summary>
        /// 下雨屏幕挂水特效
        /// </summary>
        /// <param name="active"></param>
        public void VFXScreenWater(bool active)
        {
            //获取material所有参数
            // Shader shader = _materialBg.shader;
            // int propertyCount = ShaderUtil.GetPropertyCount(shader);
            //
            // Debug.Log("Material parameters:");
            // for (int i = 0; i < propertyCount; i++)
            // {
            //     string propertyName = ShaderUtil.GetPropertyName(shader, i);
            //     ShaderUtil.ShaderPropertyType propertyType = ShaderUtil.GetPropertyType(shader, i);
            //     
            //     Debug.Log("Name: " + propertyName + ", Type: " + propertyType);
            // }
            if (active)
            {
                _materialBg.DOFloat(-2, "_Distortion", 1f);
                return;
            }

            _materialBg.DOFloat(0, "_Distortion", 1f);
        }

        /// <summary>
        /// ContinueByAutoCantClick的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator ContinueByAutoCantClickIEnumerator(float during)
        {
            objContinueButtonMask.SetActive(true);
            yield return new WaitForSeconds(during);
            objContinueButtonMask.SetActive(false);
            DialogueManager.instance.PlaySequence("Continue()");
        }

        /// <summary>
        /// FadeInBlackBg的协程
        /// </summary>
        private IEnumerator FadeInBlackBgIEnumerator(float fadeInTime)
        {
            canvasGroupBlackMaskBg.alpha = 0;
            var tweener = canvasGroupBlackMaskBg.DOFade(1, fadeInTime);
            canvasGroupBlackMaskBg.gameObject.SetActive(true);
            yield return tweener.WaitForCompletion();
        }

        /// <summary>
        /// FadeOutBlackBg的协程
        /// </summary>
        private IEnumerator FadeOutBlackBgIEnumerator(float fadeOutTime)
        {
            var tweener = canvasGroupBlackMaskBg.DOFade(0, fadeOutTime);
            yield return tweener.WaitForCompletion();
            canvasGroupBlackMaskBg.gameObject.SetActive(false);
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
        /// FadeInThinkingBg的协程
        /// </summary>
        private IEnumerator FadeInThinkingBgIEnumerator(float fadeInTime)
        {
            canvasGroupThinkingBg.alpha = 0;
            var tweener = canvasGroupThinkingBg.DOFade(1, fadeInTime);
            canvasGroupThinkingBg.gameObject.SetActive(true);
            yield return tweener.WaitForCompletion();
        }

        /// <summary>
        /// FadeOutThinkingBg的协程
        /// </summary>
        private IEnumerator FadeOutThinkingBgIEnumerator(float fadeOutTime)
        {
            var tweener = canvasGroupThinkingBg.DOFade(0, fadeOutTime);
            yield return tweener.WaitForCompletion();
            canvasGroupThinkingBg.gameObject.SetActive(false);
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

        /// <summary>
        /// 克隆bg的material，避免直接修改源文件
        /// </summary>
        private void CloneMaterialBg()
        {
            _materialBg = Instantiate(imageBg.material);
            imageBg.material = _materialBg;
        }
    }
}