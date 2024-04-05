using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Yu
{
    public class ItemDialogueNode : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textActorName;
        [SerializeField] private TextMeshProUGUI textDialogueText;

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="actorName"></param>
        /// <param name="dialogueText"></param>
        public void Refresh(string actorName,string dialogueText)
        {
            textActorName.text = actorName;
            textDialogueText.text = dialogueText;
        }
    }
}