using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Yu
{
    public class DoubleConfirmView : MonoBehaviour
    {
        public Button btnBack;
        public Button btnConfirm;
        public TextMeshProUGUI textTip;

        public GameObject objMask;
        public Animator animator;


        /// <summary>
        /// 刷新提示
        /// </summary>
        /// <param name="tip"></param>
        public void RefreshTipText(string tip)
        {
            textTip.text = tip;
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