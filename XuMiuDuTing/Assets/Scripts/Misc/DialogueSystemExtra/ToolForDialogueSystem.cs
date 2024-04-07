using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PixelCrushers.DialogueSystem;
using Rabi;
using SCPE;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Toggle = UnityEngine.UI.Toggle;


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
        [SerializeField] private GameObject objContinueButtonMaskForAuto;
        [SerializeField] private List<StandardUISubtitlePanel> allSubtitlePanelList = new List<StandardUISubtitlePanel>();
        [SerializeField] private List<Animator> allSubtitlePanelAnimatorList = new List<Animator>();
        [SerializeField] private List<Animator> allSubPortraitAnimatorList = new List<Animator>();
        [SerializeField] private Volume vfxVolume;

        //VFX
        [SerializeField] private Animator animatorFlash;
        [SerializeField] private ParticleSystem particleRain;
        [SerializeField] private ParticleSystem particleFog;
        [SerializeField] private ParticleSystem particleFire;

        //log
        [SerializeField] private Animator animatorLogView;
        [SerializeField] private ScrollRect scrollRectLogView;
        [SerializeField] private Transform itemDialogueNodeContainer;
        [SerializeField] private VerticalLayoutGroup verticalLayoutGroupScrollRectLogView;
        [SerializeField] private ContentSizeFitter contentSizeFitterScrollRectLogView;
        [SerializeField] private GameObject prefabItemDialogueNode;
        private TextMeshProUGUI _currentActorNameTextMeshPro;

        //Auto
        [SerializeField] private Toggle toggleAuto;
        [SerializeField] private Animator animatorToggleAuto;
        private TextMeshProTypewriterEffect _currentTypewriter;
        private TextMeshProUGUI _currentTypewriterTextMeshPro;
        private Sequencer _currentAutoSequencer;
        private bool _isAuto; //这个用来控制是否自动播放
        private bool _canAuto; //这个用来解决冲突
        private float _cacheAutoSpeed = 50f;

        private void Start()
        {
            CloneMaterialBg();
            EventManager.Instance.AddListener(EventName.OnPauseViewClose,OnBtnClickLogBack);
        }

        /// <summary>
        /// 对话开始之前
        /// </summary>
        public void OnConversationStart()
        {
            _canAuto = true;
            LoadAutoSetting();
            canvasGroupBlackMaskBg.alpha = 0f;
            canvasGroupBlackMaskWithoutCommonElement.alpha = 0f;
            canvasGroupThinkingBg.alpha = 0f;
            ResetAllSubPortraitAnimator();
            ResetAllSubtitlePanelAnimator();
            objContinueButtonMask.SetActive(false);
            imageBgAddonAnimator.SetTrigger("Hide");
            ResetAllActorName();
            CloseAllVFX();
            
            animatorLogView.gameObject.SetActive(false);
            CleanLog();
        }

        /// <summary>
        /// 对话取消暂停时，（关闭设置界面和Pause界面）
        /// </summary>
        public void OnConversationUnpause()
        {
            LoadAutoSetting();
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
        /// 打字机结束时
        /// </summary>
        public void OnTypewriterEnd(TextMeshProTypewriterEffect typewriter)
        {
            OnTypewriterEndLogAddon(typewriter);
            AutoContinue();
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

        #region Auto

        /// <summary>
        /// 从设置加载是否auto
        /// </summary>
        private void LoadAutoSetting()
        {
            _cacheAutoSpeed = SaveManager.GetFloat("AutoSpeed", 50f);
            var isAuto = SaveManager.GetInt("Auto", 0) == 1;
            if (toggleAuto.isOn != isAuto)
            {
                toggleAuto.isOn = isAuto;
                //自动执行onValueChange
                return;
            }

            //如果一样的话，手动调用一次
            OnToggleAutoValueChange(isAuto);
        }

        /// <summary>
        /// 自动播放toggle回调
        /// </summary>
        public void OnToggleAutoValueChange(bool isOn)
        {
            SaveManager.SetInt("Auto", isOn ? 1 : 0);

            if (isOn)
            {
                _isAuto = true;
                animatorToggleAuto.Play("Spin");
                objContinueButtonMaskForAuto.SetActive(true);
                if (_currentTypewriter && _currentTypewriter.isPlaying && string.IsNullOrWhiteSpace(_currentTypewriterTextMeshPro.text))
                {
                    return;
                }

                DoAutoContinue(1f);
                return;
            }

            _isAuto = false;
            if (animatorToggleAuto.gameObject.activeInHierarchy)
            {
                animatorToggleAuto.Play("Idle");
            }

            objContinueButtonMaskForAuto.SetActive(false);
            if (_currentAutoSequencer) //阻断本句话的continue
            {
                _currentAutoSequencer.Stop();
            }
        }

        /// <summary>
        /// 解决自动播放冲突
        /// </summary>
        /// <param name="canAuto"></param>
        public void SetCanAuto(bool canAuto)
        {
            _canAuto = canAuto;
        }

        /// <summary>
        /// 交给三个text输入调用
        /// </summary>
        private void AutoContinue()
        {
            if (!_isAuto || !_canAuto)
            {
                return;
            }

            if (!_currentTypewriter)
            {
                DoAutoContinue(1f);
                return;
            }

            var text = _currentTypewriterTextMeshPro.text;
            if (string.IsNullOrWhiteSpace(text))
            {
                DoAutoContinue(1f);
                return;
            }

            var charCount = text.Length;
            if (charCount == 0)
            {
                DoAutoContinue(1f);
                return;
            }

            var delayTime = 50f / _cacheAutoSpeed * charCount * 0.1f + 1f;
            if (delayTime < 1.5f)
            {
                delayTime = 1.5f;
            }

            DoAutoContinue(delayTime);
        }

        /// <summary>
        /// 执行continue
        /// </summary>
        /// <param name="delayTime"></param>
        private void DoAutoContinue(float delayTime)
        {
            if (_currentAutoSequencer)
            {
                _currentAutoSequencer.Stop();
            }

            _currentAutoSequencer = DialogueManager.instance.PlaySequence("Continue()@" + delayTime);
            //Debug.Log("AutoContinueDelayTime--->" + delayTime);
        }

        /// <summary>
        /// 注册当前的打字机，由三个打字机的begin调用
        /// </summary>
        public void RegisterCurrentTypewriter(TextMeshProTypewriterEffect typewriter)
        {
            _currentTypewriter = typewriter;
        }

        /// <summary>
        /// 注册当前的打字机的TMP，由三个打字机的begin调用
        /// </summary>
        public void RegisterCurrentTypewriterTMP(TextMeshProUGUI tmp)
        {
            _currentTypewriterTextMeshPro = tmp;
        }

        /// <summary>
        /// 注册当前onFocus的角色名的TMP，由5个SubtitlePanel的OnFocus调用
        /// </summary>
        public void RegisterCurrentActorName(TextMeshProUGUI tmp)
        {
            _currentActorNameTextMeshPro = tmp;
        }

        #endregion

        #region Log

        /// <summary>
        /// 打字机打完字时的log功能
        /// </summary>
        private void OnTypewriterEndLogAddon(TextMeshProTypewriterEffect typewriter)
        {
            if (typewriter != _currentTypewriter)
            {
                return;
            }

            var dialogueText = _currentTypewriterTextMeshPro.text;
            if (string.IsNullOrWhiteSpace(dialogueText))
            {
                return;
            }

            //没有dialogueText也不用获取actorName了
            var actorName = _currentActorNameTextMeshPro ? _currentActorNameTextMeshPro.text : " ";

            var itemDialogueNode = Instantiate(prefabItemDialogueNode, itemDialogueNodeContainer).GetComponent<ItemDialogueNode>();
            itemDialogueNode.Refresh(actorName, dialogueText);
        }

        /// <summary>
        /// 清空log,进入新的conversation时
        /// </summary>
        private void CleanLog()
        {
            for (var i = 0; i < itemDialogueNodeContainer.childCount; i++)
            {
                //要销毁gameObj，不然报错，尝试销毁transform，但布局组件依赖transform所以销毁失败
                Destroy(itemDialogueNodeContainer.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 点击log摁钮时
        /// </summary>
        public void OnBtnClickLog()
        {
            animatorLogView.gameObject.SetActive(true);
            Utils.ForceUpdateContentSizeFilter(itemDialogueNodeContainer);
            verticalLayoutGroupScrollRectLogView.CalculateLayoutInputVertical();
            contentSizeFitterScrollRectLogView.SetLayoutVertical();
            scrollRectLogView.verticalNormalizedPosition = 0;
            animatorLogView.SetTrigger("Show");
            DialogueManager.instance.Pause();
        }

        /// <summary>
        /// log界面点击返回键时
        /// </summary>
        public void OnBtnClickLogBack()
        {
            if (!animatorLogView.gameObject.activeInHierarchy)
            {
                return;
            }
            StartCoroutine(OnBtnClickLogBackIEnumerator());
            DialogueManager.instance.Unpause();
        }

        /// <summary>
        /// log界面点击返回键时的协程
        /// </summary>
        private IEnumerator OnBtnClickLogBackIEnumerator()
        {
            yield return Utils.PlayAnimation(animatorLogView, "Hide");
            animatorLogView.gameObject.SetActive(false);
        }

        #endregion

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
        /// 后处理恢复有问题，强制清零
        /// </summary>
        private void ForceResetVFXProcessing()
        {
            if (!vfxVolume.profile.TryGet<Ripples>(out var ripples))
            {
                return;
            }

            ripples.strength.value = 0f;

            if (!vfxVolume.profile.TryGet<ChromaticAberration>(out var chromaticAberration))
            {
                return;
            }

            chromaticAberration.intensity.value = 0f;

            if (!vfxVolume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
            {
                return;
            }

            colorAdjustments.saturation.value = 0f;
        }

        /// <summary>
        /// ContinueByAutoCantClick的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator ContinueByAutoCantClickIEnumerator(float during)
        {
            SetCanAuto(false);
            objContinueButtonMask.SetActive(true);
            yield return new WaitForSeconds(during);
            objContinueButtonMask.SetActive(false);
            DialogueManager.instance.PlaySequence("Continue()");
            SetCanAuto(true);
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
            SetCanAuto(false);
            canvasGroupBlackMask.alpha = 0;
            canvasGroupBlackMask.gameObject.SetActive(true);
            var tweener = canvasGroupBlackMask.DOFade(1, fadeInTime);
            yield return tweener.WaitForCompletion();
            imageBg.sprite = AssetManager.Instance.LoadAsset<Sprite>(ConfigManager.Instance.cfgSprite[spriteName].path);
            yield return new WaitForSeconds(during);
            DialogueManager.instance.PlaySequence("Continue()");
            SetCanAuto(true);
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

        /// <summary>
        /// 关闭所有特效
        /// </summary>
        private void CloseAllVFX()
        {
            VFXRain(false);
            VFXFog(false);
            VFXFire(false);
            // VFXColorAdjustmentsSaturation(0f);
            ForceResetVFXProcessing();
            VFXScreenWater(false);
        }

        /// <summary>
        /// 手动重置所有角色名
        /// </summary>
        private void ResetAllActorName()
        {
            DialogueSystemController.ChangeActorName("旁白", " ");
            DialogueSystemController.ChangeActorName("维克多", "维克多");
            DialogueSystemController.ChangeActorName("VK", "维多利亚");
            DialogueSystemController.ChangeActorName("维克多？？？", "？？？");
            DialogueSystemController.ChangeActorName("维克多_左", "维克多");
            DialogueSystemController.ChangeActorName("VK_右", "维多利亚");
            DialogueSystemController.ChangeActorName("大堂经理", "大堂经理");
            DialogueSystemController.ChangeActorName("大堂经理_左", "大堂经理");
            DialogueSystemController.ChangeActorName("薇薇安", "薇薇安");
            DialogueSystemController.ChangeActorName("异界人食客", "异界人食客");
            DialogueSystemController.ChangeActorName("龋齿_左", "？？？");
            DialogueSystemController.ChangeActorName("龋齿_居中", " ");
            DialogueSystemController.ChangeActorName("龋齿_右", " ");
            DialogueSystemController.ChangeActorName("维基", "维基");
            DialogueSystemController.ChangeActorName("李_中", "李");
            DialogueSystemController.ChangeActorName("李_右", "李");
        }
    }
}