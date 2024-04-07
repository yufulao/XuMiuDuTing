using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Yu
{
    public class ItemArchiveNode : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textArchiveName;
        [SerializeField] private TextMeshProUGUI textArchiveText;

        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh(string archiveName, string archiveText)
        {
            textArchiveName.text = archiveName;
            textArchiveText.text = archiveText.Replace("\\n", "\n");;
        }
    }
}
