using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Rabi;

public class CameraManager : BaseSingleTon<CameraManager>, IMonoManager
{
    private Transform _cameraContainer;
    private Camera _objCamera;
    private Camera _uiCamera;
    private Sequence _objSequence;

    private Camera _cacheMainObjCamera;


    public void OnInit()
    {
        _cameraContainer = GameObject.Find("CameraContainer").transform;
        _objCamera = _cameraContainer.Find("ObjCamera").GetComponent<Camera>();
        _cacheMainObjCamera = _objCamera;
        _uiCamera = _cameraContainer.Find("UICamera").GetComponent<Camera>();
        EventManager.Instance.AddListener(EventName.ChangeScene, OnChangeScene);
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
        _objSequence?.Kill();
    }

    public void ResetObjCamera()
    {
        if (_objCamera)
        {
            _objCamera.gameObject.SetActive(false);
        }

        _objCamera = _cacheMainObjCamera;
        _objCamera.gameObject.SetActive(true);
    }

    public IEnumerator MoveObjCamera(string cameraName, float during = 0f)
    {
        if (!_objCamera)
        {
            yield break;
        }

        List<float> positionParams = ConfigManager.Instance.cfgCamera[cameraName].position;
        List<float> rotationParams = ConfigManager.Instance.cfgCamera[cameraName].rotation;
        Vector3 cameraPosition = new Vector3(positionParams[0], positionParams[1], positionParams[2]);
        Vector3 cameraRotation = new Vector3(rotationParams[0], rotationParams[1], rotationParams[2]);
        float cameraFieldOfView = ConfigManager.Instance.cfgCamera[cameraName].fieldOfView;
        
        _objSequence?.Kill();
        _objSequence = DOTween.Sequence();
        _objSequence.Join(_objCamera.transform.DOLocalMove(cameraPosition, during));
        _objSequence.Join(_objCamera.transform.DOLocalRotateQuaternion(Quaternion.Euler(cameraRotation), during));
        _objSequence.Join(_objCamera.DOFieldOfView(cameraFieldOfView, during));
        _objSequence.SetAutoKill(false);
        yield return _objSequence.WaitForCompletion();
    }

    public Camera GetObjCamera()
    {
        return _objCamera;
    }

    private void OnChangeScene()
    {
        _objSequence?.Kill();
        // _cacheMainObjCamera.gameObject.SetActive(false);
        // _objCamera = Utils.FindTByName<Camera>("Main Camera") ?? _cacheMainObjCamera;
        // _objCamera.gameObject.SetActive(true);
    }
}