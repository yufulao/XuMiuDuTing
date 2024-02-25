using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager
{
    public static void SetInt(string Key, int i)
    {
        ES3.Save(Key, i);
    }

    public static int GetInt(string Key, int i)
    {
        if (!ES3.KeyExists(Key))
        {
            SetInt(Key,i);
        }
        return ES3.Load(Key, i, ES3Settings.defaultSettings);
    }

    public static void SetFloat(string Key, float f)
    {
        ES3.Save(Key, f);
    }

    public static float GetFloat(string Key, float f)
    {
        if (!ES3.KeyExists(Key))
        {
            SetFloat(Key,f);
        }
        return ES3.Load(Key, f, ES3Settings.defaultSettings);
    }

    public static void SetString(string Key, string s)
    {
        ES3.Save(Key, s);
    }

    public static string GetString(string Key, string s)
    {
        if (!ES3.KeyExists(Key))
        {
            SetString(Key,s);
        }
        return ES3.Load<string>(Key, s, ES3Settings.defaultSettings);
    }
}
