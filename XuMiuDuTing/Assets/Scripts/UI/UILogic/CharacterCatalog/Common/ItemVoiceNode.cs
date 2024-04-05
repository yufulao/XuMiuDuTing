using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Yu
{
    public class ItemVoiceNode : MonoBehaviour
    {
        [SerializeField] private Button btnItemVoiceNode;
        [SerializeField] private TextMeshProUGUI textVoiceName;
        [SerializeField] private Animator animatorPlaying;

        private string _cacheSfxKey;
        private RowCfgCharacterVoice _cacheRowCfgCharacterVoice;
        private UnityAction<ItemVoiceNode> _callbackOnBtnClick;


        private void Start()
        {
            btnItemVoiceNode.onClick.AddListener(OnBtnClickItemVoiceNode);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void OnInit(RowCfgCharacterVoice rowCfgCharacterVoice,UnityAction<ItemVoiceNode> callbackOnBtnClick)
        {
            _cacheRowCfgCharacterVoice = rowCfgCharacterVoice;
            _cacheSfxKey = _cacheRowCfgCharacterVoice.sfxKey;
            _callbackOnBtnClick = callbackOnBtnClick;
            textVoiceName.text = _cacheRowCfgCharacterVoice.voiceDisplayName;
        }

        /// <summary>
        /// 获取RowCfgCharacterVoice
        /// </summary>
        /// <returns></returns>
        public RowCfgCharacterVoice GetRowCfgCharacterVoice()
        {
            return _cacheRowCfgCharacterVoice;
        }

        /// <summary>
        /// 停止播放语音
        /// </summary>
        public void StopPlay()
        {
            if (string.IsNullOrEmpty(_cacheSfxKey))
            {
                return;
            }
            SFXManager.Instance.Stop(_cacheSfxKey);
            animatorPlaying.Play("Idle");
        }

        /// <summary>
        /// 当点击时
        /// </summary>
        private void OnBtnClickItemVoiceNode()
        {
            _callbackOnBtnClick?.Invoke(this);
            if (string.IsNullOrEmpty(_cacheSfxKey))
            {
                return;
            }
            SFXManager.Instance.PlaySfx(_cacheSfxKey);
            animatorPlaying.Play("Spin");
        }
    }
}
