using UnityEngine;

[CreateAssetMenu()]
public class SoundScriptableObject : ScriptableObject
{
    [Foldout("Music", true)] 
    public AudioClip mainMenuTheme;

    [Foldout("SoundScape", true)]
    public AudioClip daySoundScape;
    
    [Foldout("Player Parachute", true)]
    public AudioClip parachuteDeploy;
    public AudioClip parachuteClose;
    public AudioClip parachuteGliding;
    
    [Foldout("Items", true)]
    public AudioClip itemDrop;
    public AudioClip itemPickUp;
    public AudioClip itemGroup;

    [Foldout("UI", true)]
    public AudioClip uiIn;
    public AudioClip uiOut;
    public AudioClip uiScrollWheel;


}
