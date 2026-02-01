using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{

    public CameraController cameraController;
    public Monster monster;
    public PlayerController playerController;
    public LevelCreator levelCreator;
    static public Game instance;

    void Awake()
    {
            print("Game Awake");
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

      // called third
  
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode  )
    {
    

        cameraController = FindFirstObjectByType<CameraController>();
        monster = FindFirstObjectByType<Monster>();
        playerController = FindFirstObjectByType<PlayerController>();
        levelCreator = FindFirstObjectByType<LevelCreator>();
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

    public void InvokeDie()
    {
       Invoke("Die", 3f);
    }

    public void Die()
    {
        SceneChanger.instance.Menu();
    }

    public void Intro()
    {
      

      Invoke("EnableControls", 3f);
    }

    public void EnableControls()
    {
        Game.instance.playerController.enabled = true;
        Game.instance.cameraController.enabled = true;
    }
}
