using UnityEngine;
using System.Collections.Generic;

public static class InputHelper 
{
    private static Dictionary<KeyCode, float> lastPress = new();

    public static bool GetKey(KeyCode key)
    {
        GetKeyDown(key);

        return Input.GetKey(key);
    }
    
    public static bool GetKeyDown(KeyCode key)
    {
        if (Input.GetKeyDown(key))
        {
            if (!lastPress.ContainsKey(key)) lastPress.Add(key, Time.time);
            else lastPress[key] = Time.time;
            return true;
        }
        return false;
    }

    public static bool GetKeyDown(KeyCode key, float delay)
    {
        if (!lastPress.ContainsKey(key)) return GetKeyDown(key);
        if (Time.time - lastPress[key] > delay) return GetKeyDown(key);
        return false;
    }
}