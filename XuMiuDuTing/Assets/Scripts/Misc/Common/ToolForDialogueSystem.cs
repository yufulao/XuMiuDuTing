using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;


namespace Yu
{
    public class ToolForDialogueSystem : MonoBehaviour
    {
        /// <summary>
        /// 进入下一个关卡步骤
        /// </summary>
        public void EnterNextStageProcedure()
        {
            GameManager.Instance.StartCoroutine(EnterNextStageProcedureIEnumerator());
        }
        
        /// <summary>
        /// 进入下一个关卡步骤协程
        /// </summary>
        private IEnumerator EnterNextStageProcedureIEnumerator()
        {
            UIManager.Instance.OpenWindow("LoadingView");
            yield return new WaitForSeconds(0.3f);
            DialogueManager.instance.StopAllConversations();
            GameManager.Instance.EnterNextStageProcedure();
        }
    }
}
