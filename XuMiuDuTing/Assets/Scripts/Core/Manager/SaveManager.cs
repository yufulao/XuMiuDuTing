using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yu
{
    public class SaveManager
    {
        public static void SetInt(string key, int i)
        {
            ES3.Save(key, i);
        }

        public static int GetInt(string key, int defaultValue)
        {
            if (!ES3.KeyExists(key))
            {
                SetInt(key, defaultValue);
            }

            return ES3.Load(key, defaultValue, ES3Settings.defaultSettings);
        }

        public static void SetFloat(string key, float f)
        {
            ES3.Save(key, f);
        }

        public static float GetFloat(string key, float defaultValue)
        {
            if (!ES3.KeyExists(key))
            {
                SetFloat(key, defaultValue);
            }

            return ES3.Load(key, defaultValue, ES3Settings.defaultSettings);
        }

        public static void SetString(string key, string s)
        {
            ES3.Save(key, s);
        }

        public static string GetString(string key, string defaultValue)
        {
            if (!ES3.KeyExists(key))
            {
                SetString(key, defaultValue);
            }

            return ES3.LoadString(key, defaultValue, ES3Settings.defaultSettings);
        }
        
        public static void SetT<T>(string key, T t)
        {
            ES3.Save<T>(key, t);
        }
        
        public static T GetT<T>(string key, T defaultValue)
        {
            return ES3.Load<T>(key, defaultValue, ES3Settings.defaultSettings);
        }
    }
}