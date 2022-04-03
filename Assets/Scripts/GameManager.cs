using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameOptionsScriptableObjects gameOptions;

    protected override void Awake()
    {
        base.Awake();
    }
}