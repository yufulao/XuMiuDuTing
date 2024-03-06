namespace Yu
{
    public class ConversationBState : IFsmState
    {
        public void OnEnter()
        {
            ProcedureManager.Instance.OnEnterConversationBState();
        }

        public void OnUpdate()
        {
        
        }

        public void OnExit()
        {
        
        }
    }
}