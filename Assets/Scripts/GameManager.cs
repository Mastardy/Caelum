public class GameManager : Singleton<GameManager>
{
    public GameOptionsScriptableObject gameOptions;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}