using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;

public class UIManager : BaseSingleTon<UIManager>, IMonoManager
{
    private CfgUI _cfgUI;
    private Transform _uiRoot;
    private Dictionary<string, Transform> _layers;
    private Dictionary<string, UICtrlBase> _allViews;
    private Dictionary<string, Stack<UICtrlBase>> _layerStacks;


    public void OnInit()
    {
        _cfgUI = ConfigManager.Instance.cfgUI;

        _uiRoot = GameObject.Find("UIRoot").transform;

        _layers = new Dictionary<string, Transform>();
        _layers.Add("SceneLayer", GameObject.Find("SceneLayer").transform);
        _layers.Add("NormalLayer", GameObject.Find("NormalLayer").transform);
        _layers.Add("TopLayer", GameObject.Find("TopLayer").transform);

        _layerStacks = new Dictionary<string, Stack<UICtrlBase>>();
        _layerStacks.Add("SceneLayer", new Stack<UICtrlBase>());
        _layerStacks.Add("NormalLayer", new Stack<UICtrlBase>());
        _layerStacks.Add("TopLayer", new Stack<UICtrlBase>());

        _allViews = new Dictionary<string, UICtrlBase>();
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }

    public void LateUpdate()
    {
    }

    public void OnClear()
    {
    }

    /// <summary>
    /// 获取uiRoot
    /// </summary>
    /// <returns></returns>
    public Transform GetUIRoot()
    {
        return _uiRoot;
    }

    /// <summary>
    /// 打开页面
    /// </summary>
    /// <param name="windowName"></param>
    public UICtrlBase OpenWindow(string windowName)
    {
        UICtrlBase ctrl = GetCtrl<UICtrlBase>(windowName);

        _layerStacks[ConfigManager.Instance.cfgUI[windowName].layer].Push(ctrl);
        ctrl.OpenRoot();
        return ctrl;
    }

    /// <summary>
    /// 关闭页面
    /// </summary>
    /// <param name="windowName"></param>
    public void CloseWindows(string windowName)
    {
        UICtrlBase ctrl = GetCtrl<UICtrlBase>(windowName);
        string layer = ConfigManager.Instance.cfgUI[windowName].layer;
        while (_layerStacks[layer].Count != 0)
        {
            UICtrlBase ctrlBefore = _layerStacks[layer].Pop();
            if (ctrlBefore == ctrl)
            {
                break;
            }

            ctrlBefore.CloseRoot();
        }

        ctrl.CloseRoot();
    }

    /// <summary>
    /// 关闭所有页面
    /// </summary>
    public void CloseAllWindows()
    {
        foreach (var layerStack in _layerStacks)
        {
            if (layerStack.Value.Count == 0)
            {
                continue;
            }

            while (layerStack.Value.Count > 0)
            {
                UICtrlBase ctrl = layerStack.Value.Pop();
                ctrl.CloseRoot();
            }
        }
    }

    /// <summary>
    /// 获取ui的controller
    /// </summary>
    /// <param name="windowName"></param>
    /// <param name="param"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetCtrl<T>(string windowName, params object[] param) where T : UICtrlBase
    {
        if (_allViews.ContainsKey(windowName))
        {
            return (T) _allViews[windowName];
        }

        return (T) CreatNewView<T>(windowName, param);
    }

    /// <summary>
    /// 判断是不是对应的ctrl
    /// </summary>
    /// <param name="viewName"></param>
    /// <param name="ctrl"></param>
    /// <returns></returns>
    private bool IsViewName2Ctrl(string viewName, UICtrlBase ctrl)
    {
        if (!_allViews.ContainsKey(viewName))
        {
            return false;
        }

        if (_allViews[viewName] != ctrl)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 创建一个新的ui
    /// </summary>
    /// <param name="windowName"></param>
    /// <param name="param"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private T CreatNewView<T>(string windowName, params object[] param) where T : UICtrlBase
    {
        RowCfgUI rowCfgUi = _cfgUI[windowName];
        GameObject rootObj = GameObject.Instantiate(AssetManager.Instance.LoadAsset<GameObject>(rowCfgUi.uiPath), _layers[rowCfgUi.layer]);

        //rootObj上的ctrl开始start并实例化view和model
        rootObj.SetActive(false);
        rootObj.GetComponent<Canvas>().sortingOrder = rowCfgUi.sortOrder;

        T ctrlNew = null;
        Component[] components = rootObj.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] is UICtrlBase)
            {
                ctrlNew = components[i] as T;
                break;
            }
        }

        if (ctrlNew == null)
        {
            Debug.LogError("找不到viewObj挂载的ctrl" + rootObj.name);
            return null;
        }

        _allViews.Add(windowName, ctrlNew);
        ctrlNew.OnInit(param);
        return ctrlNew;
    }
}