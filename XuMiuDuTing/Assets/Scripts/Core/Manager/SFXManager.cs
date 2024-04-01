using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rabi;
using UnityEngine;
using UnityEngine.Audio;

namespace Yu
{
    public class SFXManager : BaseSingleTon<SFXManager>, IMonoManager
    {
        private CfgSFX _cfgSfx;
        private AudioMixer _audioMix;
        private readonly Dictionary<string, AudioMixerGroup> _sfxMixerGroupDic=new Dictionary<string, AudioMixerGroup>();

        private readonly Dictionary<string, RowCfgSFX> _dataDictionary= new Dictionary<string, RowCfgSFX>();
        private Dictionary<RowCfgSFX, AudioSource> _sfxItems;

        /// <summary>
        /// 初始化Manager，设置SfxItem，为每个sfx生成SfxItem
        /// </summary>
        public void OnInit()
        {
            _cfgSfx = ConfigManager.Instance.cfgSFX;
            _audioMix = AssetManager.Instance.LoadAsset<AudioMixer>("Assets/AddressableAssets/AudioMixer/AudioMixer.mixer");
            _sfxMixerGroupDic.Add(DefSFXType.DSe, _audioMix.FindMatchingGroups(DefSFXType.DSe)[0]);
            _sfxMixerGroupDic.Add(DefSFXType.DVoice, _audioMix.FindMatchingGroups(DefSFXType.DVoice)[0]);

            var root = new GameObject("SfxManager");
            root.transform.SetParent(GameManager.Instance.transform, false);
            
            _sfxItems = new Dictionary<RowCfgSFX, AudioSource>();
            foreach (var rowCfgSfx in _cfgSfx.AllConfigs)
            {
                if (string.IsNullOrEmpty(rowCfgSfx.key))
                {
                    return;
                }

                var sfxObjTemp = new GameObject(rowCfgSfx.key);
                sfxObjTemp.transform.SetParent(root.transform);
                var sfxObjAudioSource = sfxObjTemp.AddComponent<AudioSource>();
                sfxObjAudioSource.outputAudioMixerGroup = _sfxMixerGroupDic[rowCfgSfx.sFXType];
                sfxObjAudioSource.playOnAwake = false;
                _sfxItems.Add(rowCfgSfx, sfxObjAudioSource);
                _dataDictionary.Add(rowCfgSfx.key, rowCfgSfx);
            }
        }
        
        /// <summary>
        /// 重新设置音量，在OnInit设置，有bug，进到Start里莫名其妙变回0
        /// </summary>
        public void ReloadVolume()
        {
            _audioMix.SetFloat("SeVolume", SaveManager.GetFloat(DefSFXType.DSe+"Volume", 0f));
            _audioMix.SetFloat("VoiceVolume", SaveManager.GetFloat(DefSFXType.DVoice+"Volume", 0f));
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

            var rowCfgSfx = _dataDictionary[sfxName];
            var clip = AssetManager.Instance.LoadAsset<AudioClip>(rowCfgSfx.clipPaths[Random.Range(0, rowCfgSfx.clipPaths.Count)]);
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
                var rowCfgSfx = _dataDictionary[sfxName];
                _sfxItems[rowCfgSfx].Stop();
            }
            else
            {
                Debug.LogError("没有这个sfx名：" + sfxName);
                return;
            }
        }

        /// <summary>
        /// 停止播放所有音效
        /// </summary>
        public void StopAllSfx()
        {
            foreach (var kvp in _sfxItems)
            {
                var sfxItem = kvp.Value;
                sfxItem.Stop();
            }
        }
        
        /// <summary>
        /// 延迟淡出停止音效
        /// </summary>
        /// <param name="sfxName"></param>
        /// <param name="delayTime"></param>
        /// <param name="fadeOutTime"></param>
        /// <returns></returns>
        public IEnumerator StopDelayFadeIEnumerator(string sfxName,float delayTime,float fadeOutTime)
        {
            if (_dataDictionary.ContainsKey(sfxName))
            {
                var rowCfgSfx = _dataDictionary[sfxName];
                var audioSource = _sfxItems[rowCfgSfx];
                
                yield return new WaitForSeconds(delayTime);
                audioSource.DOFade(0, fadeOutTime); //音量降为0
                yield return new WaitForSeconds(fadeOutTime);
                audioSource.Stop();
            }
            else
            {
                Debug.LogError("没有这个sfx名：" + sfxName);
            }
        }

        /// <summary>
        /// 播放sfx过程中设置sfx音量
        /// </summary>
        /// <param name="sfxType">音效类型</param>
        /// <param name="volumeBase">要改变的音量</param>
        public void SetVolumeRuntime(string sfxType, float volumeBase)
        {
            _audioMix.SetFloat(sfxType + "Volume", volumeBase);
        }

        public void MuteVolume(string sfxType)
        {
            _audioMix.SetFloat(sfxType + "Volume", -100f);
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