using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUICtrl : UICtrlBase
{
    private PauseUIModel _model;

    [SerializeField] private Button backMask;
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button returnTitleBtn;
    [SerializeField] private Button quitBtn;

    public override void OnInit(params object[] param)
    {
        _model = new PauseUIModel();
        _model.InitModel();
        BindEvent();
    }

    public override void OpenRoot()
    {
        gameObject.SetActive(true);
        _model.SetTimeScale(Time.timeScale);
        GameManager.Instance.SetTimeScale(0f);
    }

    public override void CloseRoot()
    {
        gameObject.SetActive(false);
        GameManager.Instance.SetTimeScale(_model.GetTimeScale());
    }

    protected override void BindEvent()
    {
        backMask.onClick.AddListener(()=>
        {
            CloseRoot();
        });
        continueBtn.onClick.AddListener(() =>
        {
            CloseRoot();
        });
        returnTitleBtn.onClick.AddListener(() =>
        {
            OnReturnTitleBtnClick();
        });
        quitBtn.onClick.AddListener(()=>
        {
            OnQuitBtnClick();
        });
    }

    /// <summary>
    /// 返回主界面btn
    /// </summary>
    private void OnReturnTitleBtnClick()
    {
        GameManager.Instance.ReturnToTitle();
        CloseRoot();
    }

    /// <summary>
    /// 退出游戏btn
    /// </summary>
    private void OnQuitBtnClick()
    {
        CloseRoot();
        GameManager.Instance.QuitApplication();
    }
}