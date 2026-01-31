using UnityEngine;

public class Game : MonoBehaviour
{

    public CameraController cameraController;
    public Monster monster;
    public PlayerController playerController;
    public LevelCreator levelCreator;
    static public Game instance;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Intro();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void Intro()
    {
      

      Invoke("EnableControls", 3f);
    }

    public void EnableControls()
    {
        playerController.enabled = true;
        cameraController.enabled = true;
    }
}
