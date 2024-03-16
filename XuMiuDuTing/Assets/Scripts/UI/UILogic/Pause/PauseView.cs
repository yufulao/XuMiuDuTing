using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yu
{
    public class PauseView : MonoBehaviour
    {
        public Button btnResume;
        public Button btnSetting;
        public Button btnReturnTitle;

        public GameObject objMask;
        public Animator animator;

        
        /// <summary>
        /// 打开窗口，做成协程，不然打开动画没播放完就timeScale=0不动了
        /// </summary>
        public IEnumerator OpenWindowIEnumerator()
        {
            objMask.SetActive(false);
            gameObject.SetActive(true);
            yield return Utils.PlayAnimation(animator, "Show");
        }

        /// <summary>
        /// 关闭窗口的协程
        /// </summary>
        /// <returns></returns>
        public IEnumerator CloseWindowIEnumerator()
        {
            objMask.SetActive(true);
            yield return Utils.PlayAnimation(animator, "Hide");
            gameObject.SetActive(false);
        }
    }
}