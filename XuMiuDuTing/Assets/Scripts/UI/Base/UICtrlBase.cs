using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class UICtrlBase : MonoBehaviour
{
    public abstract void OnInit(params object[] param);

    public abstract void OpenRoot();

    public abstract void CloseRoot();

    protected abstract void BindEvent();
}