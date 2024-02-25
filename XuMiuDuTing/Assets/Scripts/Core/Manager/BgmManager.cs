using UnityEngine;
using System.Collections;
using DG.Tweening;
using Rabi;
using UnityEngine.Events;
using UnityEngine.Audio;

public class BgmManager : BaseSingleTon<BgmManager>,IMonoManager
{
    private AudioMixerGroup _bgmMixerGroup;
    private CfgBgm _cfgBgm;
    private AudioSource _audioSource;
    
    public void OnInit()
    {
        _cfgBgm=ConfigManager.Instance.cfgBgm;
        _bgmMixerGroup = AssetManager.Instance.LoadAsset<AudioMixer>("Assets/AddressableAssets/AudioMixer/AudioMixer.mixer").FindMatchingGroups("bgm")[0];
        var root = new GameObject("BgmManager");
        root.transform.SetParent(GameManager.Instance.transform, false);
        _audioSource = root.AddComponent<AudioSource>();
        _audioSource.outputAudioMixerGroup = _bgmMixerGroup;
        _audioSource.playOnAwake = false;
    }
    
    /// <summary>
    /// 播放bgm
    /// </summary>
    /// <param name="bgmName">bgm名称</param>
    /// <param name="baseVolume">bgm初始音量</param>
    public IEnumerator PlayBGM(string bgmName,float baseVolume=1f)
    {
        _audioSource.Stop();
        yield return AssetManager.Instance.LoadAssetAsync<AudioClip>(
            _cfgBgm[bgmName].audioClipPath,
            (clip) => { PlayBgmAsync(clip, baseVolume); });
        yield break;
    }

    /// <summary>
    /// 异步获取audioClip后播放
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="baseVolume"></param>
    private void PlayBgmAsync(AudioClip clip,float baseVolume)
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
    /// <param name="callback">开始播放下一个bgm时的回调，不等待淡入</param>
    /// <returns></returns>
    public IEnumerator PlayBgmFadeDelay(string bgmName, float fadeOutTime, float delayTime, float fadeInTime, float baseVolume=1f, UnityAction callback = null)
    {
        _audioSource.loop = true;
        _audioSource.DOFade(0, fadeOutTime);//音量降为0
        yield return new WaitForSeconds(fadeOutTime);
        StopBgm();
        yield return new WaitForSeconds(delayTime);
        yield return PlayBGM(bgmName, 0f);
        _audioSource.DOFade(baseVolume, fadeInTime);
        callback?.Invoke();
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
        _audioSource.DOFade(0, fadeOutTime);//音量降为0
        yield return new WaitForSeconds(fadeOutTime);
        StopBgm();
        callback?.Invoke();
    }
    
    /// <summary>
    /// bgm播放过程中设置音量
    /// </summary>
    /// <param name="volumePre"></param>
    public void SetVolumeRuntime(float volumePre)
    {
        _audioSource.volume = volumePre;
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