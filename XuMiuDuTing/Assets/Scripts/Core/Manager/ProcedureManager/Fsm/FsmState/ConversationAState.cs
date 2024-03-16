using UnityEngine.InputSystem;

namespace Yu
{
    public class ConversationAState : IFsmState
    {
        public void OnEnter(params object[] objs)
        {
            ProcedureManager.Instance.OnEnterConversationAState();
        }

        public void OnUpdate(params object[] objs)
        {
            GameManager.Instance.OnUpdateCheckPause();
        }

        public void OnExit()
        {
            
        }
    }
}
