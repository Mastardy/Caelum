using System;
using UnityEditor.Timeline.Actions;

public class DebugCommand : DebugCommandBase
{
    private Action command;

    public DebugCommand(string name, string description, string format, Action command) : base(name, description, format)
    {
        this.command = command;
    }

    public void Invoke()
    {
        command.Invoke();
    }
}

public class DebugCommand<T> : DebugCommandBase
{
    private Action<T> command;

    public DebugCommand(string name, string description, string format, Action<T> command) : base(name, description, format)
    {
        this.command = command;
    }

    public void Invoke(T value)
    {
        command.Invoke(value);
    }
}