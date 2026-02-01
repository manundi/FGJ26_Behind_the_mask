using UnityEngine;

public class SceneChanger : MonoBehaviour
{

public static SceneChanger instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Menu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public void Game()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
