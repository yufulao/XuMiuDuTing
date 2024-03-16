using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yu
{
    public class TeamEditState : IFsmState
    {
        public void OnEnter(params object[] objs)
        {
            ProcedureManager.Instance.OnEnterTeamEditState();
        }

        public void OnUpdate(params object[] objs)
        {

        }

        public void OnExit()
        {

        }
    }
}
