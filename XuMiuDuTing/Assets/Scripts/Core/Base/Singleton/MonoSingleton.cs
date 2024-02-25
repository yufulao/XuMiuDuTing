using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance => _instance;

    protected virtual void Awake()
    {
        if (_instance != null)
            Destroy(gameObject);
        else
        {
            _instance = this as T;
        }       
           
    }
}