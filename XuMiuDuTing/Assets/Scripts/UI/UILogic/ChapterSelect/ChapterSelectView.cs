using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yu
{
    public class ChapterSelectView : MonoBehaviour
    {
        public Button btnBack;
        public Button btnEnter;
        public Button btnSubPlot;
        public Button btnMainPlot;
        public Image imageInfo;
        public GameObject objMask;
        
        public Animator animator;

        public void OnInit()
        {
        }

        /// <summary>
        /// 刷新简介图片
        /// </summary>
        /// <param name="sprite"></param>
        public void RefreshInfoImage(Sprite sprite)
        {
            imageInfo.sprite = sprite;
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        public void OpenWindow()
        {
            objMask.SetActive(false);
            gameObject.SetActive(true);
            animator.Play("Show", 0, 0f);
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