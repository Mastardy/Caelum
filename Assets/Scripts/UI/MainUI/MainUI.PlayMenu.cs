using UnityEngine.SceneManagement;

public partial class MainUI
{
    public void NewGame()
    {
        AudioManager.Instance.PlaySound(sounds.uiIn);
        SceneManager.LoadScene(1);
    }

    public void LoadGame()
    {
        AudioManager.Instance.PlaySound(sounds.uiIn);
    }
}
