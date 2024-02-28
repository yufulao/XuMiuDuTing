using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yu
{
    public class CharacterSelectView : MonoBehaviour
    {
        public Button btnBack;
        public Transform characterItemContainer;
        public Animator animator;
        
        
        /// <summary>
        /// 打开窗口
        /// </summary>
        public void OpenWindow()
        {
            btnBack.interactable = true;
            gameObject.SetActive(true);
            animator.Play("Show", 0, 0f);
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public void CloseWindow()
        {
            StartCoroutine(CloseWindowIEnumerator());
        }

        /// <summary>
        /// 关闭窗口的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator CloseWindowIEnumerator()
        {
            btnBack.interactable = false;
            yield return Utils.PlayAnimation(animator, "Hide");
            gameObject.SetActive(false);
        }
    }
}