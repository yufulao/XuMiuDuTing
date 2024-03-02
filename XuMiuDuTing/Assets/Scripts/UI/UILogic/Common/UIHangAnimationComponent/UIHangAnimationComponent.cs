using UnityEngine;
using UnityEngine.EventSystems;

namespace Yu
{
    [RequireComponent(typeof(EventTrigger))]
    public class UIHangAnimationComponent : MonoBehaviour
    {
        [SerializeField]private Animator animator;
        private EventTrigger _eventTrigger;

        public void RegisterAnimator(Animator animatorT)
        {
            animator = animatorT;
        }

        private void Start()
        {
            _eventTrigger = GetComponent<EventTrigger>();
            Utils.AddTriggersListener(_eventTrigger, EventTriggerType.PointerEnter, OnPointEnter);
            Utils.AddTriggersListener(_eventTrigger, EventTriggerType.PointerExit, OnPointExit);
        }

        private void OnPointEnter(BaseEventData eventData)
        {
            if (animator==null)
            {
                return;
            }
            animator.SetBool("selected", true);
        }
        
        private void OnPointExit(BaseEventData eventData)
        {
            if (animator==null)
            {
                return;
            }
            animator.SetBool("selected", false);
        }
    }
}
