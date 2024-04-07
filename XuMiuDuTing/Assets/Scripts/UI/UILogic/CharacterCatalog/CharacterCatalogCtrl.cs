using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rabi;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Yu
{
    public class CharacterCatalogCtrl : UICtrlBase
    {
        private CharacterCatalogModel _model;
        private CharacterCatalogView _view;

        public override void OnInit(params object[] param)
        {
            _model = new CharacterCatalogModel();
            _view = GetComponent<CharacterCatalogView>();
        }

        //param[] characterName
        public override void OpenRoot(params object[] param)
        {
            BGMManager.Instance.UpdateVolumeRuntime(-0.75f);
            _model.OnOpen(param);
            if (!_model.IsSameCharacter())
            {
                UpdateAll();
            }
            
            _view.OpenWindow();
        }

        public override void CloseRoot()
        {
            BGMManager.Instance.ResetBgmVolume();
            StopCurrentVoiceItem();
            _view.CloseWindow();
        }

        public override void BindEvent()
        {
            _view.btnBack.onClick.AddListener(BtnOnClickBack);
            _view.btnCharacterVoice.onClick.AddListener(BtnOnClickCharacterVoice);
            _view.btnCharacterArchive.onClick.AddListener(BtnOnClickCharacterArchive);
        }

        /// <summary>
        /// 刷新所有
        /// </summary>
        private void UpdateAll()
        {
            _view.imagePortrait.sprite = AssetManager.Instance.LoadAsset<Sprite>(_model.GetRowCfgCharacter().idlePortraitPath);
            
            for (var i = 0; i < _view.containerItemVoiceNode.childCount; i++)
            {
                Destroy(_view.containerItemVoiceNode.GetChild(i).gameObject);
            }

            for (var i = 0; i < _view.containerItemArchiveNode.childCount; i++)
            {
                Destroy(_view.containerItemArchiveNode.GetChild(i).gameObject);
            }

            var rowCfgVoiceList = _model.GetRowCfgVoiceList();
            var rowCfgArchiveList = _model.GetRowCfgArchiveList();
            foreach (var rowCfgCharacterVoice in rowCfgVoiceList)
            {
                var itemVoiceNode = Instantiate(_view.prefabItemVoiceNode, _view.containerItemVoiceNode);
                itemVoiceNode.OnInit(rowCfgCharacterVoice,CallbackOnItemVoiceNodeClick);
            }

            foreach (var rowCfgCharacterArchive in rowCfgArchiveList)
            {
                var itemArchiveNode = Instantiate(_view.prefabItemArchiveNode, _view.containerItemArchiveNode);
                itemArchiveNode.Refresh(rowCfgCharacterArchive.archiveName,rowCfgCharacterArchive.archiveText);
            }
        }

        /// <summary>
        /// btnCharacterVoice点击时
        /// </summary>
        private void BtnOnClickCharacterVoice()
        {
            _view.OpenCharacterVoicePanel();
            _view.CloseCharacterArchivePanel();
        }

        /// <summary>
        /// btnCharacterArchive点击时
        /// </summary>
        private void BtnOnClickCharacterArchive()
        {
            _view.OpenCharacterArchivePanel();
            _view.CloseCharacterVoicePanel();
            StopCurrentVoiceItem();
        }

        /// <summary>
        /// voiceNodeClick的点击回调
        /// </summary>
        private void CallbackOnItemVoiceNodeClick(ItemVoiceNode itemVoiceNode)
        {
            StopCurrentVoiceItem();
            _model.SetCurrentItemVoiceNode(itemVoiceNode);
            _view.textVoiceText.text = itemVoiceNode.GetRowCfgCharacterVoice().voiceText.Replace("\\n", "\n");
            _view.OpenCharacterVoiceDesc();
        }

        /// <summary>
        /// 关闭当前VoiceItem
        /// </summary>
        private void StopCurrentVoiceItem()
        {
            var lastItemVoiceNode = _model.GetCurrentItemVoiceNode();
            if (lastItemVoiceNode)
            {
                lastItemVoiceNode.StopPlay();
            }
        }

        /// <summary>
        /// 返回
        /// </summary>
        private void BtnOnClickBack()
        {
            CloseRoot();
        }
    }
}