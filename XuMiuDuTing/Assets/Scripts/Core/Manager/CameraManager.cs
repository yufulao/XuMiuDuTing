using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Rabi;

namespace Yu
{
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

        /// <summary>
        /// 重置原来的objCamera
        /// </summary>
        public void ResetObjCamera()
        {
            if (_objCamera)
            {
                _objCamera.gameObject.SetActive(false);
            }

            _objCamera = _cacheMainObjCamera;
            _objCamera.gameObject.SetActive(true);
        }

        /// <summary>
        /// 移动摄像机
        /// </summary>
        /// <param name="cameraName"></param>
        /// <param name="during"></param>
        /// <returns></returns>
        public IEnumerator MoveObjCamera(string cameraName, float during = 0f)
        {
            if (!_objCamera)
            {
                yield break;
            }

            var objCameraTransform = _objCamera.gameObject.transform;
            var positionParams = ConfigManager.Instance.cfgCamera[cameraName].position;
            var rotationParams = ConfigManager.Instance.cfgCamera[cameraName].rotation;
            var cameraPosition = positionParams != null && positionParams.Count >= 3
                ? (new Vector3(positionParams[0], positionParams[1], positionParams[2]))
                : objCameraTransform.position;
            var cameraRotation = rotationParams != null && rotationParams.Count >= 3
                ? (new Vector3(rotationParams[0], rotationParams[1], rotationParams[2]))
                : objCameraTransform.rotation.eulerAngles;
            var cameraFieldOfView = ConfigManager.Instance.cfgCamera[cameraName].fieldOfView;
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

        /// <summary>
        /// 通过entity是不是敌人来设置摄像机
        /// </summary>
        public IEnumerator MoveObjCameraByEntityIsEnemy(BattleEntityCtrl entity,float during =0f)
        {
            yield return MoveObjCamera(entity.isEnemy ? DefObjCameraStateType.DEnemy : DefObjCameraStateType.DCharacter, during);
        }

        private void OnChangeScene()
        {
            _objSequence?.Kill();
            // _cacheMainObjCamera.gameObject.SetActive(false);
            // _objCamera = Utils.FindTByName<Camera>("Main Camera") ?? _cacheMainObjCamera;
            // _objCamera.gameObject.SetActive(true);
        }
    }
}