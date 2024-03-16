namespace Yu
{
    public class ConversationBState : IFsmState
    {
        public void OnEnter(params object[] objs)
        {
            ProcedureManager.Instance.OnEnterConversationBState();
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