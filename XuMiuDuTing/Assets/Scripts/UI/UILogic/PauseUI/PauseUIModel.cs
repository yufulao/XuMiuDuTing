using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUIModel
{
    private float _cacheTimeScale;

    public void InitModel()
    {
        _cacheTimeScale = 1f;
    }
    
    /// <summary>
    /// 获取暂停之前的时间速率
    /// </summary>
    /// <returns></returns>
    public float GetTimeScale()
    {
        return _cacheTimeScale;
    }
    
    /// <summary>
    /// 设置暂停之前的时间速率
    /// </summary>
    /// <param name="timeScale"></param>
    public void SetTimeScale(float timeScale)
    {
        _cacheTimeScale=timeScale;
    }
}
