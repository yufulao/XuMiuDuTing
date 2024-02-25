using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;
using UnityEngine.Audio;


public class SfxManager : BaseSingleTon<SfxManager>, IMonoManager
{
    private CfgSfx _cfgSfx;
    private AudioMixerGroup _sfxMixerGroup;

    private Dictionary<string, RowCfgSfx> _dataDictionary;
    private Dictionary<RowCfgSfx, AudioSource> _sfxItems;

    /// <summary>
    /// 初始化Manager，设置SfxItem，为每个sfx生成SfxItem
    /// </summary>
    public void OnInit()
    {
        _cfgSfx = ConfigManager.Instance.cfgSfx;
        _sfxMixerGroup = AssetManager.Instance.LoadAsset<AudioMixer>("Assets/AddressableAssets/AudioMixer/AudioMixer.mixer").FindMatchingGroups("sfx")[0];

        var root = new GameObject("SfxManager");
        root.transform.SetParent(GameManager.Instance.transform, false);

        _dataDictionary = new Dictionary<string, RowCfgSfx>();
        _sfxItems = new Dictionary<RowCfgSfx, AudioSource>();
        for (int i = 0; i < _cfgSfx.AllConfigs.Count; i++)
        {
            if (!string.IsNullOrEmpty(_cfgSfx.AllConfigs[i].key))
            {
                GameObject sfxObjTemp = new GameObject(_cfgSfx.AllConfigs[i].key);
                sfxObjTemp.transform.SetParent(root.transform);
                AudioSource sfxObjAudioSource = sfxObjTemp.AddComponent<AudioSource>();
                sfxObjAudioSource.outputAudioMixerGroup = _sfxMixerGroup;
                sfxObjAudioSource.playOnAwake = false;

                _sfxItems.Add(_cfgSfx.AllConfigs[i], sfxObjAudioSource);
                _dataDictionary.Add(_cfgSfx.AllConfigs[i].key, _cfgSfx.AllConfigs[i]);
            }
        }
    }

    /// <summary>
    /// 播放Sfx
    /// </summary>
    /// <param name="sfxName">sfx名称</param>
    public void PlaySfx(string sfxName)
    {
        if (!_dataDictionary.ContainsKey(sfxName))
        {
            Debug.LogError("没有这个sfx：" + sfxName);
            return;
        }

        RowCfgSfx rowCfgSfx = _dataDictionary[sfxName];
        AudioClip clip = AssetManager.Instance.LoadAsset<AudioClip>(rowCfgSfx.audioClipPaths[Random.Range(0, rowCfgSfx.audioClipPaths.Count)]);
        if (rowCfgSfx.oneShot)
        {
            _sfxItems[rowCfgSfx].PlayOneShot(clip, rowCfgSfx.volume);
        }
        else
        {
            _sfxItems[rowCfgSfx].Stop();
            _sfxItems[rowCfgSfx].clip = clip;
            _sfxItems[rowCfgSfx].loop = rowCfgSfx.loop;
            _sfxItems[rowCfgSfx].volume = rowCfgSfx.volume;
            _sfxItems[rowCfgSfx].Play();
            // Debug.Log(rowCfgSfx.key+"   "+_sfxItems[rowCfgSfx].loop);
        }
    }

    /// <summary>
    /// 停止播放音效
    /// </summary>
    /// <param name="sfxName">音效名称</param>
    public void Stop(string sfxName)
    {
        if (_dataDictionary.ContainsKey(sfxName))
        {
            RowCfgSfx rowCfgSfx = _dataDictionary[sfxName];
            _sfxItems[rowCfgSfx].Stop();
        }
        else
        {
            Debug.LogError("没有这个sfx名：" + sfxName);
            return;
        }
    }

    /// <summary>
    /// 播放sfx过程中设置sfx音量
    /// </summary>
    /// <param name="volumeBase">要改变的音量</param>
    public void SetVolumeRuntime(float volumeBase)
    {
        foreach (var sfxItem in _sfxItems)
        {
            sfxItem.Value.volume = volumeBase;
        }
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