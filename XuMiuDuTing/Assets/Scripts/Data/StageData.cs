using System;
using System.Collections.Generic;


namespace Yu
{
    [Serializable]
    public class StageData
    {
        public Dictionary<string, StageDataEntry> allStage = new Dictionary<string, StageDataEntry>();
    
        public StageData()
        {
        }
    }

    [Serializable]
    public class StageDataEntry
    {
        public string stageName;
        public bool isUnlock;
        public bool isPass;

        public StageDataEntry()
        {
        }
    }
}