namespace Yu
{
    public class ConversationAState : IFsmState
    {
        public void OnEnter()
        {
            ProcedureManager.Instance.OnEnterConversationAState();
        }

        public void OnUpdate()
        {

        }

        public void OnExit()
        {

        }
    }
}
