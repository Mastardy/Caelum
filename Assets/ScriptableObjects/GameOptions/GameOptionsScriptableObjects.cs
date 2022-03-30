using UnityEngine;

[CreateAssetMenu(fileName = "GameOptions", menuName = "ScriptableObjects/Save", order = 1)]
public class GameOptionsScriptableObjects : ScriptableObject
{
    // Gameplay
    public float fieldOfView;
    public bool compassVisibility;
    
    // User Interface
    public bool showChat;
    public bool showNameTags;
    public bool showGameTips;
    
    // Volume
    public float masterVolume;
    public float musicVolume;
    public float gameSoundsVolume;
    public float voiceVolume;
    
    // Input Settings
    public float mouseSensitivity;
    public bool toggleSprint;
    public bool toggleDuck;
    
    // KeyBinds - Movement
    public KeyCode forwardKey;
    public KeyCode backwardKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode sprintKey;
    public KeyCode duckKey;
    public KeyCode jumpKey;
    
    // KeyBinds - Actions
    public KeyCode primaryAttackKey;
    public KeyCode secondaryAttackKey;
    public KeyCode useKey;
    public KeyCode inventoryKey;
    public KeyCode chatKey;
    
    // Screen
    public GameResolution gameResolution;
    public GameWindowMode windowMode;
    public bool verticalSync;
    public int fpsLimit;
}
