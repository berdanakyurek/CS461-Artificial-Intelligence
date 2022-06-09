using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void GameScene()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void SettingsScene()
    {
        SceneManager.LoadScene("Settings", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
