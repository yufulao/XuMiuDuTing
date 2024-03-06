using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yu;

public class LoadingCtrl : UICtrlBase
{
    [SerializeField] private Animator animator;
    [SerializeField] private CanvasGroup mainCanvasGroup;
    private bool _isOn;
    
    public override void OnInit(params object[] param)
    {
    }

    public override void OpenRoot(params object[] param)
    {
        if (_isOn)
        {
            return;
        }

        _isOn = true;
        mainCanvasGroup.alpha = 0;
        gameObject.SetActive(true);
        StartCoroutine(IOpenRoot());
    }

    public override void CloseRoot()
    {
        if (!_isOn)
        {
            return;
        }
        _isOn = false;
        StartCoroutine(ICloseRoot());
    }

    public override void BindEvent()
    {
        
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
