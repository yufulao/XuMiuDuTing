using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Yu
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(EventTrigger))]
    public class BtnHangAnimationComponent : MonoBehaviour
    {
        private Animator _animator;
        private EventTrigger _eventTrigger;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _eventTrigger = GetComponent<EventTrigger>();
            Utils.AddTriggersListener(_eventTrigger, EventTriggerType.PointerEnter, OnPointEnter);
            Utils.AddTriggersListener(_eventTrigger, EventTriggerType.PointerExit, OnPointExit);
        }

        private void OnPointEnter(BaseEventData eventData)
        {
            _animator.SetBool("selected", true);
        }
        
        private void OnPointExit(BaseEventData eventData)
        {
            _animator.SetBool("selected", false);
        }
    }
}
