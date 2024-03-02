using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEntityCtrl : MonoBehaviour
{
    [HideInInspector] public bool isEnemy;
    
    public List<IEnumerator> commandList = new List<IEnumerator>();
    public List<IEnumerator> battleStartCommandList = new List<IEnumerator>();

    public virtual int GetMp()
    {
        return 0;
    }
    
    public virtual int GetHp()
    {
        return 0;
    }
    
    public virtual int GetSpeed()
    {
        return 0;
    }
    
    public virtual EntityHUD GetEntityHud()
    {
        return null;
    }
    
    public virtual bool IsDie()
    {
        return false;
    }
}
