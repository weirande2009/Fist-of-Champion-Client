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
    public string gameServerIP;
    public int gameServerPort;
    public NewMainClient newMainClient;
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
    public enum GameType
    {
        Online = 1,
        Offline = 2,
    }
    private GameType gameType;

    // Start is called before the first frame update
    void Start()
    {
        /* Instance */
        Instance = this;
        /* Set Controller Info */
        gameStartLock = new object();
        gameStart = false;
        gameOverLock = new object();
        gameOver = false;
        /* Set Client */
        newMainClient = null;
        gameClient = null;
        offlineServer = null;
        gameController = null;
    }

    public void StartMyGame(GameType type)
    {
        gameType = type;
        if (gameType == GameType.Online)
        {
            InitializeForOnline();
        }
        else if(gameType == GameType.Offline)
        {
            InitializeForOffline();
        }
    }

    private void InitializeForOnline()
    {
        /* Set Properties */
        // DontDestroyOnLoad(this);  // Client Object exist when changing scene
        Application.targetFrameRate = 60;

        /* Set Controller Info */
        gameStartLock = new object();
        gameStart = false;
        gameOverLock = new object();
        gameOver = false;

        /* Client */
        mainServerIP = "108.61.142.36";
        mainServerPort = 12345;
        newMainClient = new NewMainClient(mainServerIP, mainServerPort);
        newMainClient.Start();
    }

    private void InitializeForOffline()
    {
        /* Set Properties */
        // DontDestroyOnLoad(this);  // Client Object exist when changing scene
        Application.targetFrameRate = 60;

        /* Fake Client */
        gameClient = new GameClient("127.0.0.1", 12345, true);
        championNo = 0;
        playerSeatNo = 0;
        gameController = new GameController();

        /* Fake Server */
        offlineServer = new OfflineServer(2);
    }

    private void Update()
    {
        UpdateForGame();
    }

    void UpdateForGame()
    {
        // Watch MainClient
        if(newMainClient != null)
        {
            newMainClient.WatchExit();
        }
        // Watch Start Game
        lock (gameStartLock)
        {
            if (gameStart)
            {
                gameStart = false;
                // Load Scene
                // SceneManager.LoadScene("Game");
                // Start Game
                StartGame();
            }
        }
        // Watch GameClient
        if (gameClient != null)
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
        // New a game client
        gameClient = new GameClient(gameServerIP, gameServerPort);
        GameSceneController.Instance.gameInfoController.AddInfo("开始连接服务器...");
        gameClient.Start();
        // New a game controller
        GameSceneController.Instance.gameInfoController.AddInfo("开始初始化游戏控制器...");
        gameController = new GameController();
        // Connect
        GameSceneController.Instance.gameInfoController.AddInfo("开始向服务器发送连接请求...");
        gameClient.SendConnect(playerName, championNo, playerSeatNo);
    }

    public void EndGame()
    {
        // Delete game client
        if(gameType == GameType.Online)
        {
            newMainClient.Disconnect();
            gameClient.Disconnect();
        }
        Start();
        GameSceneController.Instance.ResetScene();
    }

    public void ConnectGame()
    {
        if(gameType == GameType.Online)
        {
            newMainClient.SendParticipate();
        }
        else if(gameType == GameType.Offline)
        {
            gameClient.SendConnect(playerName, championNo, playerSeatNo);
        }
    }



}
