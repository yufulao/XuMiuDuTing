using UnityEngine;
using System.Collections;
using DG.Tweening;
using Rabi;
using UnityEngine.Events;
using UnityEngine.Audio;

namespace Yu
{
    public class BGMManager : BaseSingleTon<BGMManager>, IMonoManager
    {
        private AudioMixer _audioMixer;
        private AudioMixerGroup _bgmMixerGroup;
        private CfgBGM _cfgBGM;
        private AudioSource _audioSource;

        public void OnInit()
        {
            _cfgBGM = ConfigManager.Instance.cfgBGM;
            _audioMixer = AssetManager.Instance.LoadAsset<AudioMixer>("Assets/AddressableAssets/AudioMixer/AudioMixer.mixer");
            _bgmMixerGroup = _audioMixer.FindMatchingGroups("BGM")[0];
            var root = new GameObject("BGMManager");
            root.transform.SetParent(GameManager.Instance.transform, false);
            _audioSource = root.AddComponent<AudioSource>();
            _audioSource.outputAudioMixerGroup = _bgmMixerGroup;
            _audioSource.playOnAwake = false;
        }

        /// <summary>
        /// 重新设置音量，在OnInit设置，有bug，进到Start里莫名其妙变回0
        /// </summary>
        public void ReloadVolume()
        {
            _audioMixer.SetFloat("BGMVolume", SaveManager.GetFloat("BGMVolume", 0f));
        }

        /// <summary>
        /// 播放bgm
        /// </summary>
        /// <param name="bgmName">bgm名称</param>
        /// <param name="baseVolume">bgm初始音量</param>
        public void PlayBgm(string bgmName, float baseVolume = 1f)
        {
            _audioSource.Stop();
            var audioClip= AssetManager.Instance.LoadAsset<AudioClip>(_cfgBGM[bgmName].audioClipPath);
            PlayBgm(audioClip, baseVolume);
        }

        /// <summary>
        /// 异步获取audioClip后播放
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="baseVolume"></param>
        private void PlayBgm(AudioClip clip, float baseVolume)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
            _audioSource.volume = baseVolume;
        }

        /// <summary>
        /// 停止播放当前bgm
        /// </summary>
        public void StopBgm()
        {
            _audioSource.Stop();
        }

        /// <summary>
        /// 淡出上一个bgm，等待间隔时间，淡入下一个bgm
        /// </summary>
        /// <param name="bgmName">下一个bgm的名称</param>
        /// <param name="fadeOutTime">淡出上一个bgm的时长</param>
        /// <param name="delayTime">延迟播放时间</param>
        /// <param name="fadeInTime">淡入下一个bgm的时长</param>
        /// <param name="baseVolume">初始音量</param>
        /// <returns></returns>
        public IEnumerator PlayBgmFadeDelay(string bgmName, float fadeOutTime, float delayTime, float fadeInTime, float baseVolume = 1f)
        {
            //Debug.Log(bgmName);
            _audioSource.loop = true;
            _audioSource.DOFade(0, fadeOutTime); //音量降为0
            yield return new WaitForSeconds(fadeOutTime);
            StopBgm();
            yield return new WaitForSeconds(delayTime);
            PlayBgm(bgmName, 0f);
            var tweener= _audioSource.DOFade(baseVolume, fadeInTime);
            yield return tweener.WaitForCompletion();
            // Debug.Log(_audioSource.volume);
        }
        
        /// <summary>
        /// a播放一次然后b循环
        /// </summary>
        /// <returns></returns>
        public IEnumerator PlayLoopBgmWithIntro(string bgmNameA,string bgmNameB, float fadeOutTime, float delayTime, float fadeInTime, float baseVolume = 1f)
        {
            var audioClipA= AssetManager.Instance.LoadAsset<AudioClip>(_cfgBGM[bgmNameA].audioClipPath);
            var audioClipB= AssetManager.Instance.LoadAsset<AudioClip>(_cfgBGM[bgmNameB].audioClipPath);
            _audioSource.loop = true;
            _audioSource.DOFade(0, fadeOutTime); //音量降为0
            yield return new WaitForSeconds(fadeOutTime);
            StopBgm();
            yield return new WaitForSeconds(delayTime);
            PlayBgm(audioClipA, 0f);
            _audioSource.DOFade(baseVolume, fadeInTime);
            yield return new WaitForSeconds(audioClipA.length);
            PlayBgm(audioClipB, baseVolume);
        }

        /// <summary>
        /// 延迟淡出bgm
        /// </summary>
        /// <param name="delayTime">延迟时间</param>
        /// <param name="fadeOutTime">淡出时长</param>
        /// <param name="callback">淡出至停止bgm时执行的回调</param>
        /// <returns></returns>
        public IEnumerator StopBgmFadeDelay(float delayTime, float fadeOutTime, UnityAction callback = null)
        {
            yield return new WaitForSeconds(delayTime);
            _audioSource.DOFade(0, fadeOutTime); //音量降为0
            yield return new WaitForSeconds(fadeOutTime);
            StopBgm();
            callback?.Invoke();
        }

        /// <summary>
        /// bgm播放过程中设置音量
        /// </summary>
        /// <param name="volume"></param>
        public void SetVolumeRuntime(float volume)
        {
            _audioMixer.SetFloat("BGMVolume", volume);
        }
        
        /// <summary>
        /// 静音
        /// </summary>
        public void MuteVolume()
        {
            _audioMixer.SetFloat("BGMVolume", -100f);
        }

        public void Update()
        {
        }

        public void FixedUpdate()
        {
        }

        public void LateUpdate()
        {
        }

        public void OnClear()
        {
        }
    }
}