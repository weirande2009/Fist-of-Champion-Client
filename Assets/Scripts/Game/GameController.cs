using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChampionFistGame;

public class GameController
{
    // Definition
    public const int MAX_ITEM_NUMBER_IN_BAG = 18;
    public const int CHAMPION_NUMBER = 3;
    public const float FRAME_INTERVAL = 0.066667f;
    public const float PHYSICS_SIMULATE_TIME = 0.01f;

    // Lock
    public object gameControllerLock;               // game controller lock
    public object operationLock;                    // operation lock
    public object gameDelayLock;                    // game delay lock

    // State
    public bool initializing;                       // start initializing
    public bool starting;                           // whether start
    public bool operating;                          // whether permit operating
    public bool inGame;                             // whether in game
    public bool endGame;                            // end game flag
    public object endGameLock;                      // end game lock
    private bool gameOver;                          // game over

    // Main
    private int mainPlayerNo;                       // Main player no. in game
    public int randomSeed;                          // Random seed
    public GameObject camera;                       // Main camera
    public GameObject cameraBorder;
    private GameObject waitingImage;

    // Players
    private List<PlayerInfo> playersInfo;           // Players infomation
    public List<GamePlayer> players;                // Players
    public int playerNum;                           // Player number

    // Controller
    public WeaponInfoController weaponInfoController;
    public ArmorInfoController armorInfoController;
    public MapController mapController;
    public PropertyController propertyController;
    public PoisonController poisonController;
    public NeutralController neutralController;

    // Champion Prefab
    private string[] championPrefabName;                                                // Champion prefabs name
    public Dictionary<ChampionObject.ChampionType, Transform> championPrefabs;          // Champion prefabs

    // Operation
    public List<byte[]> allOperationStreamList;     // All operation stream list

    // Delay
    public bool setDelay;
    public float gameDelayBeginTime;
    public float getDelayInterval;
    public float getDelayTimer;

    // Game Information
    public float gameTime;
    public float gameTimer;
    public float gameTimerTick;
    public float gameFpsTimer;
    public float gameFpsTimerTick;
    public int fpsCounter;

    // Watching Game
    public bool watchingGame;
    public int watchedPlayerNo;

    // Logic Frame
    public int syncFrameId;
    public bool logicFrameUpdate;
    public S_LogicFrame logicFrame;

    // Main Player
    private OperationFrame mainOperation;                   // Operation of Main Champion
    private List<OperationFrame> lastFrameOperation;        // Last Frame Operation

    // Fist Manager
    public FistManager fistManager;

    public GameController()
    {
        // Set Cursor
        //Cursor.visible = false;
        // Set Camera
        camera = GameObject.Find("CM vcam1");
        cameraBorder = GameObject.Find("CameraConfiner");
        cameraBorder.transform.localScale = new Vector3(MapController.ONE_MAP_WIDTH * MapController.MAP_WIDTH_NUMBER, MapController.ONE_MAP_HEIGHT * MapController.MAP_HEIGHT_NUMBER, 1);
        cameraBorder.transform.position = Vector3.zero;
        // Set Lock
        gameControllerLock = new object();
        operationLock = new object();
        gameDelayLock = new object();
        // Set Player
        players = new List<GamePlayer>();
        playersInfo = new List<PlayerInfo>();
        // State
        initializing = false;
        starting = false;
        operating = false;
        inGame = false;
        endGame = false;
        endGameLock = new object();
        gameOver = false;
        // Controller
        weaponInfoController = new WeaponInfoController();
        armorInfoController = new ArmorInfoController();
        mapController = new MapController();
        propertyController = new PropertyController();
        poisonController = new PoisonController(mapController);
        neutralController = new NeutralController(players, mapController);
        // Set Prefabs
        championPrefabName = new string[CHAMPION_NUMBER];
        championPrefabName[0] = "Fist/Champion/Assassin/Appearence/Assassin";
        championPrefabName[1] = "Fist/Champion/Robot/Appearence/Robot";
        championPrefabName[2] = "Fist/Champion/Sorcerer/Appearence/Sorcerer";
        championPrefabs = new Dictionary<ChampionObject.ChampionType, Transform>();
        // Operation
        mainOperation = new OperationFrame();
        // Delay
        setDelay = false;
        getDelayInterval = 1.0f;
        getDelayTimer = 0;
        // Game Time
        gameTime = 0;
        gameTimer = 0;
        gameTimerTick = 1;
        gameFpsTimer = 0;
        gameFpsTimerTick = 1;
        fpsCounter = 0;
        // Waiting Image
        waitingImage = GameObject.Find("WaitingImage");
        /* Logic Frame */
        syncFrameId = -1;
        logicFrameUpdate = false;
        logicFrame = new S_LogicFrame();
        // Fist Manager
        fistManager = new FistManager();
    }

    /*********************************** Update ***********************************/

    public void Update()
    {
        if (gameOver)
        {
            return;
        }
        // Watch Initialize
        WatchInitialize();
        if (inGame)
        {
            // Watch Operation
            DetectMainOperation();
            // Watch Logic Frame
            WatchLogicFrame();
            // Set Delay
            WatchDelay();
            // Set Fps
            WatchGameInfo();
            // Watch Player Online
            WatchOnline();
            // Watch Game Watch
            WatchPlayerWatchingGame();
        }
    }

    public void LogicUpdate(float dt)
    {
        // PoisonController 
        WatchPoison(dt);
        // Watch Game Info
        WatchLogicGameInfo(dt);
        // Watch Player Dead
        WatchPlayerDead();
        // Watch Main Player Damaged
        WatchMainPlayerDamaged();
    }

    private void WatchInitialize()
    {
        lock (gameControllerLock)
        {
            if (initializing)
            {
                GameSceneController.Instance.gameInfoController.AddInfo("开始初始化游戏...");
                initializing = false;
                Initialize();
                GlobalController.Instance.gameClient.SendLoad();
            }
            if (starting)
            {
                GameSceneController.Instance.gameInfoController.AddInfo("开始游戏...");
                waitingImage.SetActive(false);
                operating = true;
                starting = false;
                inGame = true;
                if(GlobalController.Instance.offlineServer != null)
                {
                    GlobalController.Instance.offlineServer.Start();
                }
            }
        }
    }

    private void WatchLogicFrame()
    {
        lock (operationLock)
        {
            if (logicFrameUpdate)
            {
                logicFrameUpdate = false;
                // if the frame has been synchronized, return
                if (logicFrame.FrameId < syncFrameId)
                {
                    return;
                }

                // Synchronize last frame operation for champions
                if(lastFrameOperation != null)
                {
                    SyncLastFrame(lastFrameOperation);
                }

                // Synchronize syncFrameId + 1 -> logicFrame.FrameId - 1(which are all the lost frames)
                for (int i = 0; i < logicFrame.UnsyncFrames.Count; i++)  // frame id in this list is increase
                {
                    if(logicFrame.UnsyncFrames[i].FrameId <= syncFrameId)  // if this frame has been sync
                    {
                        continue;
                    }
                    else if(logicFrame.UnsyncFrames[i].FrameId >= logicFrame.FrameId)  // if this frame is the newest frame
                    {
                        break;
                    }
                    JumpOverFrame(UtilityTool.GetOperationFrameList(logicFrame.UnsyncFrames[i].AllPlayersOpt));
                }

                // Process the newest logic frame, use this frame as the present operation for all players
                syncFrameId = logicFrame.FrameId;  // Synchronize frame id
                if(logicFrame.UnsyncFrames.Count > 0)
                {
                    lastFrameOperation = UtilityTool.GetOperationFrameList(logicFrame.UnsyncFrames[logicFrame.UnsyncFrames.Count - 1].AllPlayersOpt);
                    fistManager.HandleLogicFrame(lastFrameOperation);
                }
                else
                {
                    lastFrameOperation = null;
                }

                // Get the operation for the next frame and send to server
                GlobalController.Instance.gameClient.SendOperation(syncFrameId + 1, mainOperation);
                ChampionOperation.ClearOperationFrame(mainOperation);
            }
        }
    }

    private void WatchDelay()
    {
        lock (gameDelayLock)
        {
            if (setDelay)
            {
                // Set on UI
                GameSceneController.Instance.SetDelayText(Time.time - gameDelayBeginTime);
                setDelay = false;
            }
        }
        // Watch Delay
        if (getDelayTimer > getDelayInterval)
        {
            gameDelayBeginTime = Time.time;
            GlobalController.Instance.gameClient.SendDelay();
            getDelayTimer = 0;
        }
        getDelayTimer += Time.deltaTime;
    }

    private void WatchPoison(float dt)
    {
        poisonController.Update(players, dt);
        if (poisonController.poisonChanged)
        {
            GameSceneController.Instance.gameInfoController.AddInfo("增加毒区");
            poisonController.poisonChanged = false;
        }
    }

    private void WatchGameInfo()
    {
        if (gameFpsTimer > gameFpsTimerTick)
        {
            GameSceneController.Instance.SetGameFPSText(fpsCounter);
            fpsCounter = 0;
            gameFpsTimer = 0;
        }
        fpsCounter++;
        gameFpsTimer += Time.deltaTime;
    }

    private void WatchLogicGameInfo(float dt)
    {
        if (gameTimer > gameTimerTick)
        {
            GameSceneController.Instance.SetGameTimeText(gameTime);
            gameTimer = 0;
        }
        gameTime += dt;
        gameTimer += dt;
    }

    private void WatchOnline()
    {
        foreach(var player in players)
        {
            lock (player.playerInfo.onlineLock)
            {
                if (player.playerInfo.onlineChanged)
                {
                    player.playerInfo.onlineChanged = false;
                    GameSceneController.Instance.gameInfoController.AddInfo("玩家"+player.playerInfo.playerName+"断开连接");
                }
            }
        }
    }

    private void WatchPlayerDead()
    {
        int aliveNumber = 0;
        List<string> deadPlayerName = new List<string>();
        // Find dead player
        foreach(var player in players)
        {
            if (player.champion.Dead)  // if player dead
            {
                if (!player.dead)
                {
                    if (player.champion.mainChampion)  // if main
                    {
                        ProcessDead();
                    }
                    else
                    {
                        GameSceneController.Instance.gameInfoController.AddInfo("玩家" + player.playerInfo.playerName + "被击杀");
                    }
                    deadPlayerName.Add(player.playerInfo.playerName);
                    player.dead = true;
                }
            }
            else
            {
                aliveNumber++;
            }
        }
        if(aliveNumber <= 1)
        {
            if(deadPlayerName.Count > 0)
            {
                if (aliveNumber == 0)
                {
                    string name = deadPlayerName[0];
                    for (int i = 1; i < deadPlayerName.Count; i++)
                    {
                        name += ", " + deadPlayerName[i];
                    }
                    GameSceneController.Instance.SetChampionNameText(name);
                }
                else
                {
                    foreach(GamePlayer player in players)
                    {
                        if (!player.dead)
                        {
                            GameSceneController.Instance.SetChampionNameText(player.playerInfo.playerName);
                        }
                    }
                }
                // Send Game Over to Server
                GlobalController.Instance.gameClient.SendGameOver();
                gameOver = true;
                GameSceneController.Instance.ActivateGameOverCanvas();
            }
            else
            {

            }
        }
    }

    private void WatchMainPlayerDamaged()
    {
        if (players[mainPlayerNo].champion.damaged)
        {
            GameSceneController.Instance.ShowDamagedEffect();
            players[mainPlayerNo].champion.damaged = false;
        }
    }

    private void WatchPlayerWatchingGame()
    {
        if (watchingGame)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                SetWatchPlayer(FindLastAlivePlayer());
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                SetWatchPlayer(FindNextAlivePlayer());
            }
        }
    }

    /*********************************** Set Info ***********************************/

    public void SetBasicInfo(int _playerNum, int _mainPlayerNo)
    {
        playerNum = _playerNum;
        mainPlayerNo = _mainPlayerNo;
    }

    /// <summary>
    /// Add a player to game
    /// </summary>
    /// <param name="playerName"></param>
    /// <param name="mainPlayer"></param>
    /// <param name="championType"></param>
    public void AddPlayerInfo(string playerName, bool mainPlayer, int championType)
    {
        PlayerInfo playerInfo = new PlayerInfo((ChampionObject.ChampionType)championType, mainPlayer, playerName);
        playersInfo.Add(playerInfo);
    }

    /*********************************** Initialize ***********************************/

    private void InitializePlayers()
    {
        // Get Prefabs
        for (int i = 0; i < playersInfo.Count; i++)
        {
            if (!championPrefabs.ContainsKey(playersInfo[i].championType))
            {
                championPrefabs[playersInfo[i].championType] = Resources.Load<Transform>(championPrefabName[(int)playersInfo[i].championType]);
            }
        }
        // Initialize Player
        for (int i = 0; i < playersInfo.Count; i++)
        {
            while (true)
            {
                float x = Random.value * MapController.MAP_WIDTH + MapController.LEFT_BORDER;
                float y = Random.value * MapController.MAP_HEIGHT + MapController.BOTTOM_BORDER;
                Vector3 playerPosition = new Vector3(x, y, 0);
                //playerPosition = new Vector3(-30, 40, 0);
                if (!mapController.InBossMap(playerPosition))
                {
                    Transform champion = fistManager.Instantiate(championPrefabs[playersInfo[i].championType], playerPosition, Quaternion.identity);
                    GamePlayer player = new GamePlayer(playersInfo[i].playerName, playersInfo[i].mainPlayer, playersInfo[i].championType, champion.GetComponent<ChampionObject>());
                    player.champion.playerNo = i;
                    GameSceneController.Instance.gameInfoController.AddInfo("玩家" + (i + 1).ToString() + "主角色: " + playersInfo[i].mainPlayer);
                    players.Add(player);
                    break;
                }
            }
        }
    }

    public void Initialize()
    {
        GameSceneController.Instance.gameInfoController.AddInfo("正在初始化随机种子...");
        // Initialize game parameter
        Random.InitState(randomSeed);

        // Initialize Champion Info
        ChangeAttibuteRecord.Initialize();

        // Initialize map
        GameSceneController.Instance.gameInfoController.AddInfo("正在初始化地图...");
        mapController.Initialize();

        // Initialize Champion
        ChampionObject.ChampionInitialize();
        Assassin.AssassinInitialize();
        Robot.RobotInitialize();
        Sorcerer.SorcererInitialize();

        // Initialize players
        GameSceneController.Instance.gameInfoController.AddInfo("正在初始化玩家...");
        InitializePlayers();

        // Initialize Weapon and Armor Info
        GameSceneController.Instance.gameInfoController.AddInfo("正在初始化武器与防具信息...");
        WeaponInfoController.Instance.Initialize();
        ArmorInfoController.Instance.Initialize();

        // Initialize Property
        GameSceneController.Instance.gameInfoController.AddInfo("正在初始化物品控制器...");
        //propertyController.Initialize();

        // Initialize neutral
        SoilLow.SoilLowIntialize();
        SoilMiddle.SoilMiddleIntialize();
        FireLow.FireLowIntialize();
        FireMiddle.FireMiddleIntialize();
        WindLow.WindLowIntialize();
        WindMiddle.WindMiddleIntialize();
        WoodLow.WoodLowIntialize();
        WoodMiddle.WoodMiddleIntialize();
        LightHigh.LightHighIntialize();
        DarkHigh.DarkHighIntialize();
        neutralController.Initialize();

        // Initialize UI
        GameSceneController.Instance.gameInfoController.AddInfo("正在初始化游戏场景控制器...");
        GameSceneController.Instance.Initialize();

        // Initialize Camera
        GameSceneController.Instance.gameInfoController.AddInfo("正在初始化游戏镜头...");
        SetCameraFollow(mainPlayerNo);
    }

    /*********************************** Get Info ***********************************/

    public ChampionObject GetMainChampion()
    {
        return players[mainPlayerNo].champion;
    }

    public GamePlayer GetMainPlayer()
    {
        return players[mainPlayerNo];
    }

    /*********************************** Exit and Over ***********************************/
    public void Exit()
    {
        lock (endGameLock)
        {
            endGame = true;
        }
    }

    public void ProcessDead()
    {
        int aliveNumber = 0;
        foreach(GamePlayer player in players)
        {
            if (!player.dead)
            {
                aliveNumber++;
            }
        }
        GameSceneController.Instance.SetRankText(aliveNumber);
        GameSceneController.Instance.ActivateDeadCanvas();
    }

    public void ProcessGameOver()
    {
        GameSceneController.Instance.ActivateGameOverCanvas();
    }

    /*********************************** Watch Game ***********************************/
    public void StartWatchingGame()
    {
        watchingGame = true;
        SetWatchPlayer(FindFirstAlivePlayer());
    }

    public void SetWatchPlayer(int playerNo)
    {
        // Set Camera
        SetCameraFollow(playerNo);
        // Set Champion Watch
        foreach(GamePlayer player in players)
        {
            player.champion.watchedChampion = false;
        }
        players[playerNo].champion.watchedChampion = true;
        // Set Record
        watchedPlayerNo = playerNo;
    }

    /************************* Detect Main Operation *************************/

    public void SetWeaponInfoInOperation(WeaponInfoController.WeaponType weaponType, int weaponNo)
    {
        mainOperation.ChangeWeapon = true;
        mainOperation.LowWeaponNo = -1;
        mainOperation.MiddleWeaponNo = -1;
        mainOperation.HighWeaponNo = -1;
        if (weaponType == WeaponInfoController.WeaponType.Low)
        {
            mainOperation.LowWeaponNo = weaponNo;
        }
        else if (weaponType == WeaponInfoController.WeaponType.Middle)
        {
            mainOperation.MiddleWeaponNo = weaponNo;
        }
        else if (weaponType == WeaponInfoController.WeaponType.High)
        {
            mainOperation.HighWeaponNo = weaponNo;
        }
    }

    public void SetArmorInfoInOperation(int armorNo)
    {
        mainOperation.ChangeArmor = true;
        mainOperation.ArmorNo = armorNo;
    }

    protected void DetectMainOperation()
    {
        bool recordMouse = false;
        // Watch Key down
        if (Input.GetKeyDown(KeyCode.Q))
        {
            mainOperation.ClickQ = true;
            recordMouse = true;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            mainOperation.ClickW = true;
            recordMouse = true;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            mainOperation.ClickE = true;
            recordMouse = true;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            mainOperation.ClickR = true;
            recordMouse = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            mainOperation.ClickProperty = true;
            recordMouse = true;
        }
        if (Input.GetMouseButtonDown(1))
        {
            mainOperation.ClickMouse = true;
            recordMouse = true;
        }
        // Get mouse position on map
        if (recordMouse)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))  // if hit something
            {
                if (hit.collider.tag == "Background")
                {
                    mainOperation.MousePosX = Mathf.RoundToInt(hit.point.x * 1000);
                    mainOperation.MousePosY = Mathf.RoundToInt(hit.point.y * 1000);
                }
            }
        }
    }

    /*********************************** Synchronize Logic Frame ***********************************/
    private void SyncLastFrame(List<OperationFrame> operationFrame)
    {
        // Start Simulation
        Physics2D.autoSyncTransforms = true;
        // Sync Last Frame
        fistManager.SyncLastFrame(operationFrame);
        Physics2D.autoSyncTransforms = false;
        Physics2D.Simulate(FRAME_INTERVAL);
        // Update for GameController
        LogicUpdate(FRAME_INTERVAL);
        // Clear Fist
        fistManager.ClearDestroyedObject();
    }

    private void JumpOverFrame(List<OperationFrame> operationFrame)
    {
        // Start Simulation
        fistManager.HandleLogicFrame(operationFrame);
        Physics2D.autoSyncTransforms = true;
        fistManager.SyncLastFrame(operationFrame);
        //fistManager.JumpLogicFrame(operationFrame);
        Physics2D.autoSyncTransforms = false;
        Physics2D.Simulate(FRAME_INTERVAL);
        // Update for GameController
        LogicUpdate(FRAME_INTERVAL);
        // Clear Fist
        fistManager.ClearDestroyedObject();
    }

    private void HandleFrame(List<OperationFrame> operationFrame)
    {
        fistManager.HandleLogicFrame(operationFrame);
    }

    /*********************************** Utility ***********************************/
    public int FindFirstAlivePlayer()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].dead)
            {
                return i;
            }
        }
        return -1;
    }

    public int FindNextAlivePlayer()
    {
        int playerNo = 0;
        int prePlayerNo = watchedPlayerNo;
        while (true)
        {
            prePlayerNo = (prePlayerNo + 1) % players.Count;
            if (!players[prePlayerNo].dead)
            {
                playerNo = prePlayerNo;
                break;
            }
            if(prePlayerNo == watchedPlayerNo)
            {
                playerNo = -1;
                break;
            }
        }
        return playerNo;
    }

    public int FindLastAlivePlayer()
    {
        int playerNo = 0;
        int prePlayerNo = watchedPlayerNo;
        while (true)
        {
            if (prePlayerNo - 1 < 0)
            {
                prePlayerNo = players.Count - 1;
            }
            else
            {
                prePlayerNo--;
            }
            if (!players[prePlayerNo].dead)
            {
                playerNo = prePlayerNo;
                break;
            }
            if (prePlayerNo == watchedPlayerNo)
            {
                playerNo = -1;
                break;
            }
        }
        return prePlayerNo;
    }

    public void SetCameraFollow(int playerNo)
    {
        camera.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = players[playerNo].champion.transform;
    }

}

public class ChampionOperation
{
    // Constance
    public const int OPERATION_STREAM_LENGTH = 13;          // operation stream length
    public const float MOUSE_POSITION_RATIO = 1000;         // mouse position multi ratio
    public bool moveClicked;                                // Whether click mouse right button
    public bool qClicked;                                   // Whether click q button
    public bool wClicked;                                   // Whether click w button
    public bool eClicked;                                   // Whether click e button
    public bool rClicked;                                   // Whether click r button
    public bool propertyClicked;                            // Whether click property(1) button
    public bool changeWeapon;                               // Whether change weapon button
    public bool changeArmor;                                // Whether change armor button
    public Vector2 mouseTargetPosition;                     // Mouse Target Position on map
    public int armorNo;                                     // Armor No.
    public int lowLevelWeaponNo;                            // No. of low level weapon
    public int middleLevelWeaponNo;                         // No. of middle level weapon
    public int highLevelWeaponNo;                           // No. of high level weapon

    public ChampionOperation()
    {
        moveClicked = false;
        qClicked = false;
        wClicked = false;
        eClicked = false;
        rClicked = false;
        propertyClicked = false;
        changeWeapon = false;
        changeArmor = false;
        mouseTargetPosition = Vector2.zero;
        armorNo = -1;
        lowLevelWeaponNo = -1;
        middleLevelWeaponNo = -1;
        highLevelWeaponNo = -1;
    }

    public ChampionOperation(OperationFrame operationFrame)
    {
        moveClicked = operationFrame.ClickMouse;
        qClicked = operationFrame.ClickQ;
        wClicked = operationFrame.ClickW;
        eClicked = operationFrame.ClickE;
        rClicked = operationFrame.ClickR;
        propertyClicked = operationFrame.ClickProperty;
        changeWeapon = operationFrame.ChangeWeapon;
        changeArmor = operationFrame.ChangeArmor;
        armorNo = operationFrame.ArmorNo;
        lowLevelWeaponNo = operationFrame.LowWeaponNo;
        middleLevelWeaponNo = operationFrame.MiddleWeaponNo;
        highLevelWeaponNo = operationFrame.HighWeaponNo;
        mouseTargetPosition = new Vector2(operationFrame.MousePosX / 1000.0f, operationFrame.MousePosY / 1000.0f);
    }

    public void ParseFromByte(byte[] operationStream)
    {
        if (operationStream.Length != OPERATION_STREAM_LENGTH)
        {
            return;
        }
        qClicked = (operationStream[0] & 0x01) > 0;
        wClicked = (operationStream[0] & 0x02) > 0;
        eClicked = (operationStream[0] & 0x04) > 0;
        rClicked = (operationStream[0] & 0x08) > 0;
        propertyClicked = (operationStream[0] & 0x10) > 0;
        moveClicked = (operationStream[0] & 0x20) > 0;
        changeWeapon = (operationStream[0] & 0x40) > 0;
        changeArmor = (operationStream[0] & 0x80) > 0;
        mouseTargetPosition = new Vector2(System.BitConverter.ToInt32(operationStream, 1) / MOUSE_POSITION_RATIO, System.BitConverter.ToInt32(operationStream, 5) / MOUSE_POSITION_RATIO);
        armorNo = operationStream[9];
        lowLevelWeaponNo = operationStream[10];
        middleLevelWeaponNo = operationStream[11];
        highLevelWeaponNo = operationStream[12];
    }

    public byte[] ToByte()
    {
        // Initialize as 0
        byte[] operationByte = new byte[OPERATION_STREAM_LENGTH];
        if (qClicked)
        {
            operationByte[0] |= 0x01;
        }
        if (wClicked)
        {
            operationByte[0] |= 0x02;
        }
        if (eClicked)
        {
            operationByte[0] |= 0x04;
        }
        if (rClicked)
        {
            operationByte[0] |= 0x08;
        }
        if (propertyClicked)
        {
            operationByte[0] |= 0x10;
        }
        if (moveClicked)
        {
            operationByte[0] |= 0x20;
        }
        if (changeWeapon)
        {
            operationByte[0] |= 0x40;
        }
        if (changeArmor)
        {
            operationByte[0] |= 0x80;
        }
        System.BitConverter.GetBytes((int)(mouseTargetPosition.x * 1000)).CopyTo(operationByte, 1);
        System.BitConverter.GetBytes((int)(mouseTargetPosition.y * 1000)).CopyTo(operationByte, 5);
        operationByte[9] = (byte)armorNo;
        operationByte[10] = (byte)lowLevelWeaponNo;
        operationByte[11] = (byte)middleLevelWeaponNo;
        operationByte[12] = (byte)highLevelWeaponNo;
        return operationByte;
    }

    public void Reset()
    {
        moveClicked = false;
        qClicked = false;
        wClicked = false;
        eClicked = false;
        rClicked = false;
        propertyClicked = false;
        changeWeapon = false;
        changeArmor = false;
        mouseTargetPosition = Vector2.zero;
        armorNo = -1;
        lowLevelWeaponNo = -1;
        middleLevelWeaponNo = -1;
        highLevelWeaponNo = -1;
    }

    public bool IsEmpty()
    {
        bool flag;
        flag = !(moveClicked || qClicked || wClicked || eClicked || rClicked || propertyClicked || changeWeapon || changeArmor);
        return flag;
    }

    public static void ClearOperationFrame(OperationFrame operationFrame)
    {
        operationFrame.ClickQ = false;
        operationFrame.ClickW = false;
        operationFrame.ClickE = false;
        operationFrame.ClickR = false;
        operationFrame.ClickProperty = false;
        operationFrame.ClickMouse = false;
        operationFrame.ChangeWeapon = false;
        operationFrame.ChangeArmor = false;
        operationFrame.MousePosX = 0;
        operationFrame.MousePosY = 0;
        operationFrame.ArmorNo = -1;
        operationFrame.LowWeaponNo = -1;
        operationFrame.MiddleWeaponNo = -1;
        operationFrame.HighWeaponNo = -1;
    }

}



