using System;

public enum EventName{
    Click,
    ChangeScene,
    
    //ui
    OnPauseViewClose,
    
    //Procedure
    TeamEditStateEnter,
    ConversationAStateEnter,
    BattleStateEnter,
    ConversationBStateEnter,

    //Battle
    OnRoundEnd,//回合结束
}
