using UnityEngine;

public class ForceAwakeSetActive : MonoBehaviour
{
    public GameObject setActiveObj;
    public bool setActiveBool;

    private void Awake()
    {
        if (setActiveObj.activeInHierarchy!= setActiveBool)
        {
            setActiveObj.SetActive(setActiveBool);
        }
    }
}
