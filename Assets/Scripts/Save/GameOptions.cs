using UnityEngine;

[System.Serializable]
public enum GameResolution
{
    W1920H1080 = 0,
    W1600H900 = 1,
    W1366H768 = 2,
    W1280H720 = 3,
    W1024H576 = 4,
    W960H540 = 5,
    W640H360 = 6
}

[System.Serializable]
public enum GameWindowMode
{
    Fullscreen = 0,
    FullscreenWindowed = 1,
    Windowed = 2
}

[System.Serializable]
public class GameOptions
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
    public bool toggleParachute;
    
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
    
    // TODO: Graphics Quality
    
    public GameOptions()
    {
        fieldOfView = 90.0f;
        compassVisibility = true;

        showChat = true;
        showNameTags = true;
        showGameTips = true;

        masterVolume = 0.75f;
        musicVolume = 0.9f;
        gameSoundsVolume = 1.0f;
        voiceVolume = 0.7f;

        mouseSensitivity = 10.0f;
        toggleSprint = false;
        toggleDuck = false;
        toggleParachute = true;

        forwardKey = KeyCode.W;
        backwardKey = KeyCode.S;
        leftKey = KeyCode.A;
        rightKey = KeyCode.D;
        sprintKey = KeyCode.LeftShift;
        duckKey = KeyCode.LeftControl;
        jumpKey = KeyCode.Space;

        primaryAttackKey = KeyCode.Mouse0;
        secondaryAttackKey = KeyCode.Mouse1;
        useKey = KeyCode.E;
        inventoryKey = KeyCode.Tab;
        chatKey = KeyCode.Y;

        gameResolution = GameResolution.W1920H1080;
        windowMode = GameWindowMode.Fullscreen;
        verticalSync = false;
        fpsLimit = 0;
    }

    public void SetHighestResolution()
    {
        Resolution highestResolution = Screen.resolutions[^1];

        if (highestResolution.width < 960 || highestResolution.height < 540) gameResolution =  GameResolution.W640H360;
        else if (highestResolution.width < 1024 || highestResolution.height < 576) gameResolution =  GameResolution.W960H540;
        else if (highestResolution.width < 1280 || highestResolution.height < 720) gameResolution =  GameResolution.W1024H576;
        else if (highestResolution.width < 1336 || highestResolution.height < 768) gameResolution =  GameResolution.W1280H720;
        else if (highestResolution.width < 1600 || highestResolution.height < 900) gameResolution =  GameResolution.W1366H768;
        else if (highestResolution.width < 1920 || highestResolution.height < 1080) gameResolution =  GameResolution.W1600H900;
        else gameResolution =  GameResolution.W1920H1080;
    }
}