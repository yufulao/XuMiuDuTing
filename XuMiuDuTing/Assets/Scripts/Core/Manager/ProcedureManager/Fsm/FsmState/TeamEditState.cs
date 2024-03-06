using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yu
{
    public class TeamEditState : IFsmState
    {
        public void OnEnter()
        {
            ProcedureManager.Instance.OnEnterTeamEditState();
        }

        public void OnUpdate()
        {

        }

        public void OnExit()
        {

        }
    }
}
