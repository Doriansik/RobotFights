using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public void OnStartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OnCredits()
    {
        SceneManager.LoadScene(1);
    }

    public void OnQuitCredits()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
