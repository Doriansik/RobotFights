using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private GameObject levelSelectorPanel;

    public void OnStartGame()
    {
        SceneManager.LoadScene(1);
        Debug.Log("Working");
        Time.timeScale = 0f;
    }

    public void OnLevelSelectorOn()
    {
        if(levelSelectorPanel != null)
        {
            levelSelectorPanel.SetActive(true);
        }
    }

    public void OnCredits()
    {
        SceneManager.LoadScene(3);
    }

    public void OnOptions()
    {
        SceneManager.LoadScene(2);
    }

    public void OnBackButton()
    {
        SceneManager.LoadScene(0);
    }

    public void OnNextLevel()
    {
        SceneManager.LoadScene(4);
        Time.timeScale = 0f;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
