using UnityEngine;

[CreateAssetMenu()]
public class SoundScriptableObject : ScriptableObject
{
    [Foldout("Music", true)] 
    public AudioClip mainMenuTheme;

    [Foldout("SoundScape", true)]
    public AudioClip daySoundScape;

    [Foldout("Player Movement", true)] 
    public AudioClip[] jumps;
    public AudioClip[] crouches;
    public AudioClip[] dashes;
    
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

    [Foldout("Axe", true)] 
    public AudioClip axeSwing;
    public AudioClip axeHit;

    [Foldout("Pickaxe", true)]
    public AudioClip pickaxeSwing;
    public AudioClip pickaxeHit;

    [Foldout("Bow", true)]
    public AudioClip bowPull;
    public AudioClip bowShoot;
    
    [Foldout("Sword", true)]
    public AudioClip swordSwing;
    public AudioClip swordHit;

    [Foldout("Spear", true)] 
    public AudioClip spearThrow;
    public AudioClip spearSwing;
    public AudioClip spearHit;
    
    [Foldout("Hookshot", true)]
    public AudioClip grappleSwing;
}
