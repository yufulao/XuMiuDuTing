using System;
using UnityEngine;
using System.Collections;

namespace Yu
{
    public class UIFollowObj : MonoBehaviour
    {
        public Transform objFollowed; //3D物体（人物）
        public RectTransform rectFollower; //UI元素（如：血条等）
        public Vector2 offset; //偏移量

        private Camera _objCamera;

        private void Start()
        {
            _objCamera = CameraManager.Instance.GetObjCamera();
        }

        private void Update()
        {
            if (!objFollowed)
            {
                return;
            }

            rectFollower.anchoredPosition = (Vector2)_objCamera.WorldToScreenPoint(objFollowed.position) + offset;
        }
    }
}