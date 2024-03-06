using DG.Tweening;
using TMPro;
using UnityEngine;

public class DescribeItem : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private TextMeshProUGUI textInfo;
    [SerializeField] private CanvasGroup canvasGroup;

    /// <summary>
    /// 打开描述面板
    /// </summary>
    /// <param name="info"></param>
    /// <param name="position"></param>
    public void Open(string info,Vector3 position)
    {
        textInfo.text = info;
        canvasGroup.alpha = 0;
        mainPanel.transform.position = position;
        canvasGroup.DOFade(1f,0.2f);
        mainPanel.SetActive(true);
    }

    /// <summary>
    /// 关闭描述面板
    /// </summary>
    public void Close()
    {
        canvasGroup.DOFade(0f,0.2f);
        mainPanel.SetActive(false);
    }

    /// <summary>
    /// 强制关闭面板
    /// </summary>
    public void ForceClose()
    {
        mainPanel.SetActive(false);
    }
}
