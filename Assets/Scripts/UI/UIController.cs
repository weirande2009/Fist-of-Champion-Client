using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    /* Instance */
    public static UIController Instance;// Static Instance

    /* Constant */
    public const int CHANGED = 1;
    public const int NO_CHANGE = -1;

    /* Login Scene */
    public static object loginSceneLock;            // Lock of login scene
    public int loginState;                          // State of loginErrorText
    public int loginStateChange;                    // State of loginErrorText change
    public int registerState;                       // State of loginErrorText
    public int registerStateChange;                 // State of loginErrorText change

    /* Hall Scene */
    public static object hallSceneLock;             // Lock of hall scene
    public RoomInfo[] roomInfo;                     // Room Info
    public int hallRoomChange;                      // State of room info change
    public List<FriendInfo> friendInfo;             // List of Friend info
    public int friendChange;                        // State of friend change
    public int friendUpdateChange;                  // State of friend update, index in friendInfo
    public int addFriendUpdateChange;               // State of add friend update change, index in friendInfo
    public string otherAddFriendName;               // Name of other add friend name
    public int otherAddFriendChange;                // State of other add friend name
    public int failAddFriendCanvasChange;           // State of fail add friend canvas change
    public int createRoomState;                     // State of create room
    public int createRoomStateChange;               // State of create room change
    public int enterRoomState;                      // State of enter room
    public int enterRoomStateChange;                // State of enter room change
    public int exitLoginState;                      // Exit State
    public int exitLoginStateChange;                // Exit State change
    public string roomName;                         // Room Name
    public string roomNo;                           // Room No
    public int seatNo;                              // Room Seat No

    /* Room Scene */
    public static object roomSceneLock;             // Lock of hall scene
    public SeatInfo[] seatInfo;                     // List of Seat info
    public int seatInfoChange;                      // State of seat info change
    public int seatInfoUpdateChange;                // State of seat info update change, index in seatInfo
    public int readyState;                          // Ready State
    public int readyStateChange;                    // Ready State change
    public int exitRoomState;                       // Exit State
    public int exitRoomStateChange;                 // Exit State change



    // Start is called before the first frame update
    void Start()
    {
        /* Set Instance */
        Instance = this;
        /* Set Login Scene */
        loginState = 0;
        loginStateChange = NO_CHANGE;
        registerState = 0;
        registerStateChange = NO_CHANGE;
        /* Set Hall Scene */
        roomInfo = new RoomInfo[MainClient.MAX_ROOM_NUMBER_IN_PAGE];
        for(int i=0; i< MainClient.MAX_ROOM_NUMBER_IN_PAGE; i++)
        {
            roomInfo[i] = new RoomInfo();
        }
        hallRoomChange = NO_CHANGE;
        friendInfo = new List<FriendInfo>();
        friendChange = NO_CHANGE;
        friendUpdateChange = NO_CHANGE;
        addFriendUpdateChange = NO_CHANGE;
        otherAddFriendName = string.Empty;
        otherAddFriendChange = NO_CHANGE;
        failAddFriendCanvasChange = NO_CHANGE;
        createRoomState = 0;
        createRoomStateChange = NO_CHANGE;
        /* Set Room Scene */
        seatInfo = new SeatInfo[MainClient.MAX_PLAYER_NUM_IN_ROOM];
        for(int i=0; i<MainClient.MAX_PLAYER_NUM_IN_ROOM; i++)
        {
            seatInfo[i] = new SeatInfo();
        }
        seatInfoChange = NO_CHANGE;
        seatInfoUpdateChange = NO_CHANGE;
        readyState = MainClient.PLAYER_NO_READY;
        readyStateChange = NO_CHANGE;
        exitRoomState = 0;
        exitRoomStateChange = NO_CHANGE;
        /* Set Lock */
        loginSceneLock = new object();
        hallSceneLock = new object();
        roomSceneLock = new object();
        /* Set Properties */
        DontDestroyOnLoad(this);  // Client Object exist when changing scene
    }

    // Update is called once per frame
    void Update()
    {
        /* Update Login Scene */
        lock (loginSceneLock)
        {
            if(loginStateChange == CHANGED)
            {
                LoginSceneController.Instance.SetLoginState(loginState);
                loginStateChange = NO_CHANGE;
            }
            if(registerStateChange == CHANGED)
            {
                LoginSceneController.Instance.SetRegisterState(registerState);
                registerStateChange = NO_CHANGE;
            }
        }
        /* Update Hall Scene */
        lock (hallSceneLock)
        {
            if(hallRoomChange == CHANGED)
            {
                for(int i=0; i<MainClient.MAX_ROOM_NUMBER_IN_PAGE; i++)
                {
                    HallSceneController.Instance.UpdateRoomInfo(roomInfo[i]);
                }
                hallRoomChange = NO_CHANGE;
            }
            if(friendChange == CHANGED)
            {
                for (int i = 0; i < friendInfo.Count; i++)
                {
                    HallSceneController.Instance.AddFriendInfoCanvas(friendInfo[i]);
                }
                friendChange = NO_CHANGE;
            }
            if(friendUpdateChange != NO_CHANGE)
            {
                HallSceneController.Instance.SetFriendInfoCanvas(friendInfo[friendUpdateChange]);
                friendUpdateChange = NO_CHANGE;
            }
            if(addFriendUpdateChange != NO_CHANGE)
            {
                HallSceneController.Instance.AddFriendInfoCanvas(friendInfo[addFriendUpdateChange]);
                addFriendUpdateChange = NO_CHANGE;
            }
            if(otherAddFriendChange == CHANGED)
            {
                HallSceneController.Instance.AddOtherAddFriendContent(otherAddFriendName);
                otherAddFriendChange = NO_CHANGE;
            }
            if(failAddFriendCanvasChange == CHANGED)
            {
                HallSceneController.Instance.ShowFailAddOtherFriendCanvas();
                failAddFriendCanvasChange = NO_CHANGE;
            }
            if(createRoomStateChange == CHANGED)
            {
                if(createRoomState == 1)
                {
                    HallSceneController.Instance.CreateRoomSuccess(roomNo);
                }
                createRoomStateChange = NO_CHANGE;
            }
            if(enterRoomStateChange == CHANGED)
            {
                if(enterRoomState == 1)
                {
                    HallSceneController.Instance.EnerRoomState(enterRoomState);
                }
                enterRoomStateChange = NO_CHANGE;
            }
            if(exitLoginStateChange == CHANGED)
            {
                HallSceneController.Instance.ExitLogin();
                exitLoginStateChange = NO_CHANGE;
            }
        }
        /* Update Room Scene */
        lock (roomSceneLock)
        {
            if (seatInfoChange == CHANGED)
            {
                for (int i = 0; i < MainClient.MAX_PLAYER_NUM_IN_ROOM; i++) 
                {
                    RoomSceneController.Instance.SetSeatInfo(seatInfo[i]);
                    if(seatInfo[i].owner && i == seatNo)
                    {
                        RoomSceneController.Instance.SetOwner();
                    }
                }
                seatInfoChange = NO_CHANGE;
            }
            if(seatInfoUpdateChange != NO_CHANGE)
            {
                RoomSceneController.Instance.SetSeatInfo(seatInfo[seatInfoUpdateChange]);
                seatInfoUpdateChange = NO_CHANGE;
            }
            if(readyStateChange == CHANGED)
            {
                RoomSceneController.Instance.SetReadyButton(readyState);
                readyStateChange = NO_CHANGE;
            }
            if(exitRoomStateChange == CHANGED)
            {
                RoomSceneController.Instance.ExitRoomState(exitRoomState);
                exitRoomStateChange = NO_CHANGE;
            }
        }
    }
}
