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
    public void Open(string info)
    {
        //todo 设置出现位置
        textInfo.text = info;
        canvasGroup.alpha = 0;
        mainPanel.SetActive(true);
        canvasGroup.DOFade(1f,0.2f);
    }

    /// <summary>
    /// 关闭描述面板
    /// </summary>
    public void Close()
    {
        canvasGroup.DOFade(0f,0.2f);
        mainPanel.SetActive(false);
    }
}
