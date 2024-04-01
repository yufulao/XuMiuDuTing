using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Yu
{
    public class BattleTutorialView : MonoBehaviour
    {
        public List<Image> allPage;
        public Button btnConfirm;
        public Button btnPre;
        public Button btnNext;
        public Transform pageOriginalTransform;
        public Transform pageTargetTransform;

        public GameObject objMask;
        public Animator animator;

        /// <summary>
        /// 重置所有
        /// </summary>
        public void ResetPage(int currentPageIndex)
        {
            for (var i = 0; i < currentPageIndex; i++)
            {
                allPage[i].gameObject.transform.position = pageTargetTransform.position;
            }

            for (var i = currentPageIndex; i < allPage.Count; i++)
            {
                allPage[i].gameObject.transform.position = pageOriginalTransform.position;
            }
            
            SetBtnPreActive(true);
            SetBtnNextActive(true);
            SetBtnConfirmActive(false);    

            if (currentPageIndex<=0)
            {
                SetBtnPreActive(false);
            }

            if (currentPageIndex>=allPage.Count-1)
            {
                SetBtnNextActive(false);
                SetBtnConfirmActive(true);
            }
        }

        /// <summary>
        /// 将页面移至目标位置
        /// </summary>
        /// <param name="movePage"></param>
        public void MovePageToTarget(Image movePage)
        {
            movePage.transform.DOMove(pageTargetTransform.position, 0.5f);
        }
        
        /// <summary>
        /// 将页面移至初始位置
        /// </summary>
        /// <param name="movePage"></param>
        public void MovePageToOriginal(Image movePage)
        {
            movePage.transform.DOMove(pageOriginalTransform.position, 0.5f);
        }

        /// <summary>
        /// 设置pre摁钮激活
        /// </summary>
        /// <param name="active"></param>
        public void SetBtnPreActive(bool active)
        {
            btnPre.gameObject.SetActive(active);
        }
        
        /// <summary>
        /// 设置Next摁钮激活
        /// </summary>
        /// <param name="active"></param>
        public void SetBtnNextActive(bool active)
        {
            btnNext.gameObject.SetActive(active);
        }
        
        /// <summary>
        /// 设置Next摁钮激活
        /// </summary>
        /// <param name="active"></param>
        public void SetBtnConfirmActive(bool active)
        {
            btnConfirm.gameObject.SetActive(active);
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