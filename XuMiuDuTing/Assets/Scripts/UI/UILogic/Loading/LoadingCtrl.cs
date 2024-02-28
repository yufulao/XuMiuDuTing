using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yu;

public class LoadingCtrl : UICtrlBase
{
    [SerializeField] private Animator animator;
    [SerializeField] private CanvasGroup mainCanvasGroup;
    private float _progress;
    
    public override void OnInit(params object[] param)
    {
    }

    public override void OpenRoot(params object[] param)
    {
        mainCanvasGroup.alpha = 0;
        gameObject.SetActive(true);
        StartCoroutine(IOpenRoot());
    }

    public override void CloseRoot()
    {
        StartCoroutine(ICloseRoot());
    }

    public override void BindEvent()
    {
        
    }

    public void UpdateProgress(float progress)
    {
        _progress = progress;
    }

    /// <summary>
    /// OpenRoot()的协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator IOpenRoot()
    {
        yield return Utils.PlayAnimation(animator, "LoadingEnter");
    }
    
    /// <summary>
    /// CloseRoot()的协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator ICloseRoot()
    {
        mainCanvasGroup.alpha = 1;
        yield return Utils.PlayAnimation(animator, "LoadingExit");
        gameObject.SetActive(false);
    }
}
