using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    private bool showConsole;
    private bool focused;

    private string input;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash)) ToggleConsole();
    }

    public void ToggleConsole()
    {
        showConsole = !showConsole;
        focused = false;
        input = string.Empty;
    }

    private void OnGUI()
    {
        if (!showConsole) return;

        float y = 0;

        GUI.Box(new Rect(0, y, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        GUI.SetNextControlName("DebugConsoleInput");
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width-20f, 20f), input);
        if (!focused)
        {
            GUI.FocusControl("DebugConsoleInput");
            focused = true;
        }

        if(input != null)
            if (input.Contains(@"\"))
                ToggleConsole();
    }
}
