using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class AssetManager : BaseSingleTon<AssetManager>,IMonoManager
{
    //value是handle，获取资源的异步操作句柄，状态可以是isDone也可以是正在加载
    private readonly Dictionary<string, AsyncOperationHandle> _handleDict = new Dictionary<string, AsyncOperationHandle>();
    private bool _hadInit=false;
    
    public void OnInit()
    {
        if (!_hadInit)
        {
            Addressables.InitializeAsync();
            _hadInit = true;
        }
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
        //卸载所有加载了的资源
        foreach (var handle in _handleDict.Values)
        {
            Addressables.Release(handle);
        }

        _handleDict.Clear();
    }
    
    
    /// <summary>
    /// 异步加载(单个资源)
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径，如果为空则不会加载</param>
    /// <param name="callBack">回调函数</param>
    public IEnumerator LoadAssetAsync<T>(string path, Action<T> callBack) where T : Object
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("路径不能为空");
            yield break;
        }
        
        AsyncOperationHandle<T> loadHandle;
        if (_handleDict.ContainsKey(path))//已有handle，重复添加handle
        {
            loadHandle=_handleDict[path].Convert<T>();
            if (loadHandle.IsDone)//如果已经操作完成，输出回调并且跳出函数
            {
                callBack?.Invoke(loadHandle.Result);
                yield break;
            }
        }
        else//第一次添加这个handle
        {
            loadHandle=Addressables.LoadAssetAsync<T>(path);
            _handleDict.Add(path, loadHandle);
        }
        
        //如果操作没完成
        yield return loadHandle;
        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            callBack?.Invoke(loadHandle.Result);
            yield break;
        }
        
        Debug.Log("加载失败"+path);
        Release(path);
    }

    /// <summary>
    /// 同步加载(单个资源)
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径，如果为空则不会加载</param>
    public T LoadAsset<T>(string path) where T : Object
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("路径不能为空");
            return null;
        }
        
        AsyncOperationHandle<T> handle;
        if (_handleDict.ContainsKey(path))//dic中是否已经有handle操作
        {
            handle=_handleDict[path].Convert<T>();//只是获取handle，不确定是否已经complete
        }
        else
        {
            handle=Addressables.LoadAssetAsync<T>(path);
            _handleDict.Add(path, handle);
        }

        T asset = handle.WaitForCompletion();//挂起当前线程，直到操作完成为止
        
        if (!asset)
        {
            Debug.LogError("加载失败"+path);
        }

        return asset;
    }

    /// <summary>
    /// 释放handle
    /// </summary>
    /// <param name="path">资源路径</param>
    public void Release(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("要释放的资源的路径为空");
            return;
        }
        //释放句柄，并将这个键名从字典离移除
        if (!_handleDict.ContainsKey(path))
        {
            Debug.Log("没有这个handle"+path);
            return;
        }
        //释放handle
        Addressables.Release(_handleDict[path]);
        _handleDict.Remove(path);
    }
    
    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="path">场景path</param>
    /// <param name="callBack">回调中获取场景sceneInstance.Scene</param>
    /// <param name="loadSceneMode">single是替换当前场景，additive是在当前场景上追加新的场景</param>
    public IEnumerator LoadSceneSync(string path, Action<SceneInstance> callBack, LoadSceneMode loadSceneMode=LoadSceneMode.Single)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("路径不能为空");
            yield break;
        }
        //Debug.Log(path);
        AsyncOperationHandle<SceneInstance> sceneLoadHandle = Addressables.LoadSceneAsync(path, loadSceneMode);
        sceneLoadHandle.Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                callBack?.Invoke(handle.Result);
            }
            else
            {
                Debug.Log("异步加载场景失败" + handle.DebugName);
                Addressables.Release(handle);
            }
        };
        yield return sceneLoadHandle;
    }

    /// <summary>
    /// 卸载加载好了的场景，同时卸载加载了的场景内的资源，但是只能等场景加载完成后才能卸载
    /// </summary>
    public void UnloadScene(SceneInstance scene)
    {
        AsyncOperationHandle<SceneInstance> unloadSceneHandle = Addressables.UnloadSceneAsync(scene);
        unloadSceneHandle.Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Addressables.Release(handle);
                return;
            }
        };

        Debug.Log("场景卸载失败"+unloadSceneHandle.DebugName);
    }

    /// <summary>
    /// 释放未在使用的资源
    /// </summary>
    public static void ClearUnused()
    {
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
}