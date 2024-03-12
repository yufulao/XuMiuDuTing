using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Yu
{
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
        /// <param name="param"></param>
        public void OpenWindow(string windowName, params object[] param)
        {
            var ctrl = GetCtrl<UICtrlBase>(windowName, param);

            _layerStacks[ConfigManager.Instance.cfgUI[windowName].layer].Push(ctrl);
            ctrl.OpenRoot(param);
        }

        /// <summary>
        /// 关闭页面
        /// </summary>
        /// <param name="windowName"></param>
        public void CloseWindow(string windowName)
        {
            var ctrl = GetCtrl<UICtrlBase>(windowName);
            var layer = ConfigManager.Instance.cfgUI[windowName].layer;
            while (_layerStacks[layer].Count != 0)
            {
                var ctrlBefore = _layerStacks[layer].Pop();
                if (ctrlBefore == ctrl)
                {
                    break;
                }

                ctrlBefore.CloseRoot();
            }

            ctrl.CloseRoot();
        }
        
        /// <summary>
        /// 关闭指定层级的所有页面
        /// </summary>
        /// <param name="layerName"></param>
        public void CloseAllLayerWindows(string layerName)
        {
            var windowsStack = _layerStacks[layerName];
            while (windowsStack.Count != 0)
            {
                windowsStack.Pop().CloseRoot();;
            }
        }

        /// <summary>
        /// 销毁窗口
        /// </summary>
        /// <param name="windowName"></param>
        public void DestroyWindow(string windowName)
        {
            if (!_allViews.ContainsKey(windowName))
            {
                return;
            }
            var ctrl = _allViews[windowName];
            _allViews.Remove(windowName);
            var layer = ConfigManager.Instance.cfgUI[windowName].layer;
            while (_layerStacks[layer].Count != 0)
            {
                var ctrlBefore = _layerStacks[layer].Pop();
                if (ctrlBefore == ctrl)
                {
                    break;
                }

                ctrlBefore.CloseRoot();
            }
            Object.Destroy(ctrl.gameObject);
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
                    var ctrl = layerStack.Value.Pop();
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
            var rowCfgUi = _cfgUI[windowName];
            var rootObj = GameObject.Instantiate(AssetManager.Instance.LoadAsset<GameObject>(rowCfgUi.uiPath), _layers[rowCfgUi.layer]);

            //rootObj上的ctrl开始start并实例化view和model
            rootObj.SetActive(false);
            var canvas=rootObj.GetComponent<Canvas>();
            canvas.worldCamera = CameraManager.Instance.GetUICamera();
            canvas.sortingOrder = rowCfgUi.sortOrder;

            T ctrlNew = null;
            var components = rootObj.GetComponents<Component>();
            for (var i = 0; i < components.Length; i++)
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
            ctrlNew.BindEvent();
            return ctrlNew;
        }
    }
}