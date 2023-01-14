#define GAME_DEBUG
//#define OFFLINE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalController : MonoBehaviour
{
    // Instance
    public static GlobalController Instance;

    // Client
    private string mainServerIP;
    private int mainServerPort;
    private string gameServerIP;
    public int gameServerPort;
    public MainClient mainClient;
    public GameClient gameClient;

    // User
    public string userName;

    // Game
    public OfflineServer offlineServer;
    public object gameStartLock;
    public bool gameStart;
    public int championNo;
    public int playerSeatNo;
    public string playerName;
    public GameController gameController;
    public object gameOverLock;
    public bool gameOver;


    // Start is called before the first frame update
    void Start()
    {
#if OFFLINE
        InitializeForOffline(); 
#elif GAME_DEBUG
        InitializeForGameDebug();
#else
        Initialize();
#endif
    }

    private void Initialize()
    {
        /* Instance */
        Instance = this;

        /* Set Properties */
        DontDestroyOnLoad(this);  // Client Object exist when changing scene
        Application.targetFrameRate = 60;

        /* Set Controller Info */
        gameStartLock = new object();
        gameStart = false;
        gameOverLock = new object();
        gameOver = false;

        /* Client */
        mainServerIP = "114.116.101.46";
        mainServerPort = 12345;
        gameServerIP = "114.116.101.46";

        mainClient = new MainClient(mainServerIP, mainServerPort);
        mainClient.Start();
    }

    private void InitializeForGameDebug()
    {
        /* Instance */
        Instance = this;

        /* Set Properties */
        DontDestroyOnLoad(this);  // Client Object exist when changing scene
        Application.targetFrameRate = 60;

        /* Set Controller Info */
        gameStartLock = new object();
        gameStart = false;
        gameOverLock = new object();
        gameOver = false;

        /* Client */
        mainServerIP = "114.116.101.46";
        mainServerPort = 12345;
        gameServerIP = "3.145.137.146";
        gameServerPort = 12345;
        //mainClient = new MainClient(mainServerIP, mainServerPort);
        //mainClient.Start();
        gameClient = new GameClient(gameServerIP, gameServerPort);
        GameSceneController.Instance.gameInfoController.AddInfo("开始连接服务器...");
        gameClient.Start();
        GameSceneController.Instance.gameInfoController.AddInfo("开始初始化游戏控制器...");
        gameController = new GameController();
        GameSceneController.Instance.gameInfoController.AddInfo("开始向服务器发送连接请求...");
        championNo = 0;
        playerSeatNo = 0;
        playerName = "weiran";
        // gameClient.SendConnect(playerName, championNo, playerSeatNo);
    }

    private void InitializeForOffline()
    {
        /* Instance */
        Instance = this;

        /* Set Properties */
        DontDestroyOnLoad(this);  // Client Object exist when changing scene
        Application.targetFrameRate = 60;

        /* Set Controller Info */
        gameStartLock = new object();
        gameStart = false;
        gameOverLock = new object();
        gameOver = false;

        /* Fake Client */
        gameServerIP = "114.116.101.46";
        gameServerPort = 12346;
        gameClient = new GameClient(gameServerIP, gameServerPort, true);
        championNo = 0;
        playerSeatNo = 0;
        gameController = new GameController();

        /* Fake Server */
        offlineServer = new OfflineServer(2);
    }

    private void Update()
    {
#if GAME_DEBUG
        UpdateForGameDebug();
#else
        UpdateForGame();
#endif
    }

    void UpdateForGame()
    {
        // Watch MainClient
        mainClient.WatchExit();
        // Watch Start Game
        lock (gameStartLock)
        {
            if (gameStart)
            {
                gameStart = false;
                // Load Scene
                SceneManager.LoadScene("Game");
                // Start Game
                StartGame();
            }
        }
        // Watch GameClient
        if(gameClient != null)
        {
            gameClient.WatchExit();
        }
        // If in game
        if (gameController != null)
        {
            gameController.Update();
            lock (gameController.endGameLock)
            {
                if (gameController.endGame)
                {
                    EndGame();
                }
            }
            lock (gameOverLock)
            {
                if (gameOver)
                {
                    gameController.ProcessGameOver();
                    gameOver = false;
                }
            }
        }
    }


    void UpdateForGameDebug()
    {
        // Watch GameClient
        gameClient.WatchExit();
        // If in game
        if (gameController != null)
        {
            gameController.Update();
            lock (gameController.endGameLock)
            {
                if (gameController.endGame)
                {
                    EndGame();
                }
            }
        }
    }

    /// <summary>
    /// Start a game
    /// </summary>
    public void StartGame()
    {
        // New a game controller
        GameSceneController.Instance.gameInfoController.AddInfo("开始初始化游戏控制器...");
        gameController = new GameController();
        // New a game client
        gameClient = new GameClient(gameServerIP, gameServerPort);
        GameSceneController.Instance.gameInfoController.AddInfo("开始连接服务器...");
        gameClient.Start();
        // Connect
        GameSceneController.Instance.gameInfoController.AddInfo("开始向服务器发送连接请求...");
        gameClient.SendConnect(userName, championNo, playerSeatNo);
    }

    public void EndGame()
    {
        // Delete game controller
        gameController = null;
        // Delete game client
#if OFFLINE

#else
        gameClient.Disconnect();
#endif
        gameClient = null;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ConnectGame()
    {
        gameClient.SendConnect(playerName, championNo, playerSeatNo);
    }


}
