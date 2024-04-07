using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Yu
{
    public class CharacterCatalogView : MonoBehaviour
    {
        public Image imagePortrait;
        //voice
        public Animator animatorCharacterVoicePanel;
        public TextMeshProUGUI textVoiceText;
        public Transform containerItemVoiceNode;
        public Button btnCharacterVoice;
        public Animator animatorBtnCharacterVoice;
        public ItemVoiceNode prefabItemVoiceNode;
        public Animator animatorVoiceDesc;
        //Archive
        public Animator animatorCharacterArchivePanel;
        public Transform containerItemArchiveNode;
        public Button btnCharacterArchive;
        public Animator animatorBtnCharacterArchive;
        public ItemArchiveNode prefabItemArchiveNode;
        
        public Button btnBack;
        public GameObject objMask;
        public Animator animator;
        

        /// <summary>
        /// 打开角色语音面板
        /// </summary>
        public void OpenCharacterVoicePanel()
        {
            animatorCharacterVoicePanel.gameObject.SetActive(true);
            animatorVoiceDesc.gameObject.SetActive(false);
            animatorCharacterVoicePanel.Play("Show", 0, 0f);
            animatorBtnCharacterVoice.Play("OnSelect", 0, 0f);
        }

        /// <summary>
        /// 关闭角色语音面板
        /// </summary>
        public void CloseCharacterVoicePanel()
        {
            GameManager.Instance.StartCoroutine(CloseCharacterVoicePanelIEnumerator());
        }
        
        /// <summary>
        /// 关闭语音窗口的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator CloseCharacterVoicePanelIEnumerator()
        {
            yield return Utils.PlayAnimation(animatorCharacterVoicePanel, "Hide");
            animatorCharacterVoicePanel.gameObject.SetActive(false);
            yield return Utils.PlayAnimation(animatorBtnCharacterVoice, "UnSelect");
        }
        
        /// <summary>
        /// 打开角色语音描述
        /// </summary>
        public void OpenCharacterVoiceDesc()
        {
            animatorVoiceDesc.gameObject.SetActive(true);
            animatorVoiceDesc.Play("Show", 0, 0f);
        }

        /// <summary>
        /// 打开角色档案面板
        /// </summary>
        public void OpenCharacterArchivePanel()
        {
            animatorCharacterArchivePanel.gameObject.SetActive(true);
            animatorCharacterArchivePanel.Play("Show", 0, 0f);
            animatorBtnCharacterArchive.Play("OnSelect", 0, 0f);
        }

        /// <summary>
        /// 关闭角色档案面板
        /// </summary>
        public void CloseCharacterArchivePanel()
        {
            GameManager.Instance.StartCoroutine(CloseCharacterArchivePanelIEnumerator());
        }
        
        /// <summary>
        /// 关闭档案窗口的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator CloseCharacterArchivePanelIEnumerator()
        {
            yield return Utils.PlayAnimation(animatorCharacterArchivePanel, "Hide");
            animatorCharacterArchivePanel.gameObject.SetActive(false);
            yield return Utils.PlayAnimation(animatorBtnCharacterArchive, "UnSelect");
        }
        
        /// <summary>
        /// 打开窗口
        /// </summary>
        public void OpenWindow()
        {
            objMask.SetActive(false);
            gameObject.SetActive(true);
            animator.Play("Show", 0, 0f);
            //默认打开voice面板
            animatorCharacterArchivePanel.gameObject.SetActive(false);
            OpenCharacterVoicePanel();
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public void CloseWindow()
        {
            GameManager.Instance.StartCoroutine(CloseWindowIEnumerator());
        }

        /// <summary>
        /// 关闭窗口的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator CloseWindowIEnumerator()
        {
            objMask.SetActive(true);
            yield return Utils.PlayAnimation(animator, "Hide");
            gameObject.SetActive(false);
        }
    }
}