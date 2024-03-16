using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace Yu
{
    public class BuffItem : MonoBehaviour
    {
        private BuffInfo _buffInfo;
        [SerializeField] private EventTrigger eventTrigger;
        public TextMeshProUGUI textLayer;
        private UnityAction<BuffItem, BaseEventData> _onPointEnter;
        private UnityAction<BuffItem, BaseEventData> _onPointExit;

        /// <summary>
        /// 注册监听
        /// </summary>
        private void Start()
        {
            Utils.AddTriggersListener(eventTrigger, EventTriggerType.PointerEnter, OnPointEnter);
            Utils.AddTriggersListener(eventTrigger, EventTriggerType.PointerExit, OnPointExit);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(BuffInfo buffInfo)
        {
            _buffInfo = buffInfo;
        }
        
        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            textLayer.text = _buffInfo?.layer.ToString();
        }

        /// <summary>
        /// 获取BuffInfo并进行修改
        /// </summary>
        public BuffInfo GetBuffInfo()
        {
            return _buffInfo;
        }

        /// <summary>
        /// 设置buffItem悬停时的回调
        /// </summary>
        public void SetBuffItemOnPointEnter(UnityAction<BuffItem, BaseEventData> onPointEnter)
        {
            _onPointEnter = onPointEnter;
        }
        
        /// <summary>
        /// 设置buffItem离开悬停时的回调
        /// </summary>
        public void SetBuffItemOnPointExit(UnityAction<BuffItem, BaseEventData> onPointExit)
        {
            _onPointExit = onPointExit;
        }

        /// <summary>
        /// 当鼠标悬停时
        /// </summary>
        /// <param name="baseEventData"></param>
        private void OnPointEnter(BaseEventData baseEventData)
        {
            _onPointEnter?.Invoke(this,baseEventData);
        }

        /// <summary>
        /// 当鼠标离开悬停时
        /// </summary>
        /// <param name="baseEventData"></param>
        private void OnPointExit(BaseEventData baseEventData)
        {
            _onPointExit?.Invoke(this,baseEventData);
        }
    }
}
