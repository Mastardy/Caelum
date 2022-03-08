using UnityEditor;

class InspectorLock : Editor
{
    [MenuItem("Tools/Toggle Inspector Lock %l")]
    public static void ToggleInspectorLock()
    {
        ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }
}