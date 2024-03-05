using Rabi;

namespace Yu
{
    public class BuffInfo
    {
        public string buffName;
        public BattleEntityCtrl caster;
        public BattleEntityCtrl target;
        public int layer;
        public int roundDuring; //持续时间
        public object[] buffStringParams; //buff描述的string参数
        public object[] buffValues; //buff缓存值
        public RowCfgBuff RowCfgBuff => ConfigManager.Instance.cfgBuff[buffName];
    }
}

