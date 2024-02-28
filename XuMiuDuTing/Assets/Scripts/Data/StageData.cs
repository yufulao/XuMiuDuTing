using System;


[Serializable]
public class StageData
{
    public string stageName;
    public bool isUnlock;
    public bool isPass;
    
    public StageData(){}

    public StageData(string stageName)
    {
        this.stageName = stageName;
    }
}
