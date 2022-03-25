using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Netcode.Transports.Facepunch;

public class DebugConsole : MonoBehaviour
{
    private bool showConsole;
    private bool showHelp;
    
    private bool focused;
    private string input;

    private float lastKey;

    private List<DebugCommandBase> commandList = new();

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Backslash)) ToggleConsole();
    }

    private void Awake()
    {
        commandList.Add(new DebugCommand("Disconnect", "Disconnects from the lobby", "disconnect",
            () => NetworkManager.Singleton.GetComponent<FacepunchTransport>().DisconnectLocalClient()));
        
        commandList.Add(new DebugCommand<string[]>("Say", "Escreve na consola alguma coisa", "say",
            (args) => 
            {
                if (args.Length <= 1) return;
    
                string message = args[1];
                
                for (int i = 2; i < args.Length; i++)
                {
                    message += " " + args[i];
                }
                
                Debug.Log(message);
            }));
        
        commandList.Add(new DebugCommand("Quit", "Quits from the game", "quit", () =>
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }));

        commandList.Add(new DebugCommand("Help", "Shows all the commands", "help", () => showHelp = true));
    }
    
    private Vector2 scroll;
    
    private void OnGUI()
    {
        if (!showConsole) return;

        if (Event.current.isKey)
        {
            switch (Event.current.keyCode)
            {
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    HandleInput();
                    input = string.Empty;
                    Event.current.Use();
                    break;
            }
        }

        if (showHelp)
        {
            GUI.Box(new Rect(0, 30, Screen.width, 100), "");

            Rect viewport = new Rect(0, 30, Screen.width - 30, 20 * commandList.Count);

            scroll = GUI.BeginScrollView(new Rect(0, 35f, Screen.width, 90), scroll, viewport);

            for (int i = 0; i < commandList.Count; i++)
            {
                var command = commandList[i];

                string label = $"{command.CommandFormat} - {command.CommandDescription}";

                Rect labelRect = new Rect(5, 30 + 20 * i, viewport.width - 100, 20);
                
                GUI.Label(labelRect, label);
            }
            
            GUI.EndScrollView();
        }

        GUI.Box(new Rect(0, 0, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        GUI.SetNextControlName("DebugConsoleInput");
        input = GUI.TextField(new Rect(10f, 5f, Screen.width-20f, 20f), input);
        if (!focused)
        {
            GUI.FocusControl("DebugConsoleInput");
            focused = true;
        }

        if (input == null) return;

        if (input.Contains(@"\"))
        {
            ToggleConsole();
            return;
        }
        
        if (Time.time - lastKey < 0.2f) return;
        
        if (Event.current.isKey)
        {
            switch (Event.current.keyCode)
            {
                case KeyCode.Escape:
                    ToggleConsole();
                    break;
                
            }
        }
    }

    private void HandleInput()
    {
        string[] args = input.Split(' ');
        
        for (var i = 0; i < commandList.Count; i++)
        {
            var commandBase = commandList[i];
            if (input.Contains(commandBase.CommandFormat))
            {
                if (commandList[i] is DebugCommand command)
                    command.Invoke();
                else if (commandList[i] is DebugCommand<string[]> commandStr)
                    commandStr.Invoke(args);
            }
        }
    }

    private void ToggleConsole()
    {
        showConsole = !showConsole;
        showHelp = false;
        focused = false;
        input = string.Empty;

        lastKey = Time.time;
    }
}
