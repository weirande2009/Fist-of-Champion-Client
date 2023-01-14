using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using ChampionFist;
using Google.Protobuf;


public class MainClient : Client
{
    /* Constant */
    /* Room */
    public const int MAX_ROOM_NUMBER_IN_PAGE = 8;   // Max room number in a page of client
    public const int MAX_ROOM_NAME_LENGTH = 32;     // Max length of room name
    public const int ROOM_NO_LENGTH = 8;            // Max length of room no
    public const int SEAT_NUM_IN_ROOM = 8;          // Seat number in a room
    public const int SEAT_EMPTY = -1;               // State of seat empty
    public const int SEAT_OCCUPIED = 1;             // State of sear occupied
    public const int ENTER_ROOM_OK = 1;             // State of full room
    public const int NO_AVAILABLE_SEAT = -1;        // State of full room
    public const int NO_SUCH_ROOM = -2;             // State of wrong room name
    public const int ROOM_EMPTY = -1;               // State of empty room
    public const int EXIT_ROOM_OK = 1;              // State of exit room ok
    public const int EXIT_ROOM_FAIL = 0;            // State of exit room fail
    public const int ROOM_IN_GAME = 1;              // State of room in game
    public const int ROOM_NOT_IN_GAME = 0;          // State of room not in game
    public const int ROOM_OWNER = 1;                // State of room owner
    public const int ROOM_NOT_OWNER = -1;           // State of not room owner
    /* Player */
    public const int MAX_PLAYER_NUM_IN_ROOM = 8;    // Max player number in a room
    public const int MAX_PLAYER_NAME_LENGTH = 32;   // Max length of player name
    public const int PLAYER_ID_LENGTH = 8;          // Length of player id
    public const int PLAYER_ONLINE = 1;             // State of player online
    public const int PLAYER_OFFLINE = 2;            // State of player offline
    public const int PLAYER_READY = 1;              // State of player ready
    public const int PLAYER_NO_READY = 0;           // State of player no ready
    /* Network */
    public const int LENGTH_MD5 = 128;              // Length of md5 code
    public const int S_CMD_NUM = 18;                // Server command number
    public const int C_CMD_NUM = 14;                // Client command number
    /* Client */
    public const int CLIENT_NO_LOGIN = 0;           // Client no login
    public const int CLIENT_LOGIN = 1;              // Client login
    /* Login */
    public const int LOGIN_SUCCESS = 1;             // State of login success
    public const int LOGIN_FAIL = -1;               // State of login fail
    public const int REGISTER_SUCCESS = 1;          // State of register success
    public const int REGISTER_FAIL = -1;            // State of register fail
    /* Game */
    public const int GAME_START = 1;                // State of game start
    public const int GAME_NO_START = 0;             // State of game no start
    public const int GAME_OVER = 2;                 // State of game over
    /* Friend */
    public const int SEND_FRIEND_OK = 1;            // State of send msg to friend ok
    public const int SEND_FRIEND_FAIL = -1;         // State of send msg to friend fail
    public const int REPLY_FRIEND_OK = 2;           // State of replying from friend ok
    public const int REPLY_FRIEND_FAIL = -2;        // State of replying from friend fail
    /* MainClient */
    public const int MAIN_CLIENT_CLOSE = -1;        // State of main client close


    /* Variables */
    /* Player */
    public string playerRoomNo;                           // Present Player Room No.



    /* Functions */
    /* private */
    // Start is called before the first frame update
    public MainClient(string _ip, int _port) : base(_ip, _port)
    {
        /* Process Function Pool */
        processFunctionPool = new List<ProcessFunctionPool>();
        processFunctionPool.Add(ProcessLogin);
        processFunctionPool.Add(ProcessRegister);
        processFunctionPool.Add(ProcessHallRoom);
        processFunctionPool.Add(ProcessFriend);
        processFunctionPool.Add(ProcessFriendUpdate);
        processFunctionPool.Add(ProcessEnterRoom);
        processFunctionPool.Add(ProcessRoomInfo);
        processFunctionPool.Add(ProcessExitRoom);
        processFunctionPool.Add(ProcessUpdateRoom);
        processFunctionPool.Add(ProcessModifyChar);
        processFunctionPool.Add(ProcessReady);
        processFunctionPool.Add(ProcessCancelReady);
        processFunctionPool.Add(ProcessStartGame);
        processFunctionPool.Add(ProcessExitLogin);
        processFunctionPool.Add(ProcessQuit);
        processFunctionPool.Add(ProcessAddFriend);
        processFunctionPool.Add(ProcessOtherAddFriend);
        processFunctionPool.Add(ProcessCreateRoom);
    }

    /// <summary>
    /// Process Login data from server
    /// </summary>
    private void ProcessLogin()
    {
        Debug.Log("Receive Command: Login");
        S_Login sLogin = new S_Login();
        sLogin.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receivedLength - PACKAGE_HEAD_LENGTH);
        Debug.Log("State: "+sLogin.State);
        lock (UIController.loginSceneLock)
        {
            UIController.Instance.loginState = sLogin.State;
            UIController.Instance.loginStateChange = UIController.CHANGED;
        }
    }

    /// <summary>
    /// Process Register data from server
    /// </summary>
    private void ProcessRegister()
    {
        Debug.Log("Receive Command: Register");
        S_Register sRegister = new S_Register();
        sRegister.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receivedLength - PACKAGE_HEAD_LENGTH);
        Debug.Log("State: " + sRegister.State);
        lock (UIController.loginSceneLock)
        {
            UIController.Instance.registerState = sRegister.State;
            UIController.Instance.registerStateChange = UIController.CHANGED;
        }
    }

    /// <summary>
    /// Process HallRoom data from server
    /// </summary>
    private void ProcessHallRoom()
    {
        Debug.Log("Receive Command: HallRoom");
        S_HallRoom sHallRoom = new S_HallRoom();
        sHallRoom.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receivedLength - PACKAGE_HEAD_LENGTH);
        Debug.Log("Room Number:" + sHallRoom.PageRoomNum);
        for (int i = 0; i < sHallRoom.PageRoomNum; i++)
        {
            Debug.Log("No." + i);
            Debug.Log("Room Name" + sHallRoom.RoomInfo[i].RoomName);
            Debug.Log("Room No" + sHallRoom.RoomInfo[i].RoomNo);
            Debug.Log("Present Player Number" + sHallRoom.RoomInfo[i].PlayerNumber);
            Debug.Log("Room Index" + sHallRoom.RoomInfo[i].RoomIndex);
            Debug.Log("Room State" + sHallRoom.RoomInfo[i].State);
        }
        lock (UIController.hallSceneLock)
        {
            for(int i=0; i < MAX_ROOM_NUMBER_IN_PAGE; i++)
            {
                if(i < sHallRoom.PageRoomNum)
                {
                    UIController.Instance.roomInfo[i].roomName = sHallRoom.RoomInfo[i].RoomName;
                    UIController.Instance.roomInfo[i].roomNo = sHallRoom.RoomInfo[i].RoomNo;
                    UIController.Instance.roomInfo[i].prePlayerNumber = sHallRoom.RoomInfo[i].PlayerNumber;
                    UIController.Instance.roomInfo[i].roomIndex = sHallRoom.RoomInfo[i].RoomIndex;
                    UIController.Instance.roomInfo[i].state = sHallRoom.RoomInfo[i].State;
                    UIController.Instance.roomInfo[i].empty = false;
                }
                else
                {
                    UIController.Instance.roomInfo[i].empty = true;
                }
            }
            UIController.Instance.hallRoomChange = UIController.CHANGED;
        }
    }

    /// <summary>
    /// Process Friend data from server
    /// </summary>
    private void ProcessFriend()
    {
        Debug.Log("Receive Command: Friend");
        S_Friend sFriend = new S_Friend();
        sFriend.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receivedLength - PACKAGE_HEAD_LENGTH);
        for (int i = 0; i < sFriend.FriendInfo.Count; i++)
        {
            Debug.Log("Friend No." + i);
            Debug.Log("Friend Name: " + sFriend.FriendInfo[i].PlayerName);
            Debug.Log("State: " + sFriend.FriendInfo[i].State);
        }

        lock (UIController.hallSceneLock)
        {
            for (int i = 0; i < sFriend.FriendInfo.Count; i++)
            {
                FriendInfo friendInfo = new FriendInfo();
                friendInfo.friendName = sFriend.FriendInfo[i].PlayerName;
                friendInfo.state = sFriend.FriendInfo[i].State;
                UIController.Instance.friendInfo.Add(friendInfo);
            }
            UIController.Instance.friendChange = UIController.CHANGED;
        }
    }

    /// <summary>
    /// Process FriendUpdate data from server
    /// </summary>
    private void ProcessFriendUpdate()
    {
        Debug.Log("Receive Command: FriendUpdate");
        S_FriendUpdate sFriendUpdate = new S_FriendUpdate();
        sFriendUpdate.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receivedLength - PACKAGE_HEAD_LENGTH);
        Debug.Log("Friend Name: " + sFriendUpdate.FriendInfo.PlayerName);
        Debug.Log("Friend State: " + sFriendUpdate.FriendInfo.State);
        lock (UIController.hallSceneLock)
        {
            bool flag = false;
            for (int i = 0; i < UIController.Instance.friendInfo.Count; i++)
            {
                if(UIController.Instance.friendInfo[i].friendName == sFriendUpdate.FriendInfo.PlayerName)
                {
                    flag = true;
                    UIController.Instance.friendInfo[i].state = sFriendUpdate.FriendInfo.State;
                    UIController.Instance.friendUpdateChange = i;
                }
            }
            if (!flag)  // if not exist, means new friend
            {
                FriendInfo friendInfo = new FriendInfo();
                friendInfo.friendName = sFriendUpdate.FriendInfo.PlayerName;
                friendInfo.state = sFriendUpdate.FriendInfo.State;
                UIController.Instance.friendInfo.Add(friendInfo);
                UIController.Instance.addFriendUpdateChange = UIController.Instance.friendInfo.Count - 1;
            }
        }
    }

    /// <summary>
    /// Process EnterRoom data from server
    /// </summary>
    private void ProcessEnterRoom()
    {
        Debug.Log("Receive Command: EnterRoom");
        S_EnterRoom sEnterRoom = new S_EnterRoom();
        sEnterRoom.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receivedLength - PACKAGE_HEAD_LENGTH);
        Debug.Log("State: " + sEnterRoom.State);
        if (sEnterRoom.State == ENTER_ROOM_OK)
        {
            Debug.Log("Room Name: " + sEnterRoom.RoomName);
            Debug.Log("Room No: " + sEnterRoom.RoomNo);
            Debug.Log("Seat No: " + sEnterRoom.SeatNo);
        }
        lock (UIController.hallSceneLock)
        {
            UIController.Instance.enterRoomState = sEnterRoom.State;
            if (sEnterRoom.State == ENTER_ROOM_OK)
            {
                UIController.Instance.roomName = sEnterRoom.RoomName;
                UIController.Instance.roomNo = sEnterRoom.RoomNo;
                UIController.Instance.seatNo = sEnterRoom.SeatNo;
            }
            UIController.Instance.enterRoomStateChange = UIController.CHANGED;
        }
    }
    /// <summary>
    /// Process RoomInfo data from server
    /// </summary>
    private void ProcessRoomInfo()
    {
        Debug.Log("Receive Command: RoomInfo");
        S_RoomInfo sRoomInfo = new S_RoomInfo();
        sRoomInfo.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receivedLength - PACKAGE_HEAD_LENGTH);
        for (int i = 0; i < MAX_PLAYER_NUM_IN_ROOM; i++)
        {
            Debug.Log("Seat No." + i);
            if (sRoomInfo.SeatInfo[i].Empty == SEAT_EMPTY)
            {
                Debug.Log("Empty");
            }
            else
            {
                Debug.Log("Player Name: " + sRoomInfo.SeatInfo[i].PlayerInfo.PlayerName);
                Debug.Log("Character: " + sRoomInfo.SeatInfo[i].PlayerInfo.CharType);
                Debug.Log("Seat No: " + i);
                if (sRoomInfo.SeatInfo[i].PlayerInfo.Ready == PLAYER_READY)
                {
                    Debug.Log("Ready");
                }
                else
                {
                    Debug.Log("Not Ready");
                }
                if (sRoomInfo.SeatInfo[i].PlayerInfo.Owner == ROOM_OWNER)
                {
                    Debug.Log("Owner");
                }
                else
                {
                    Debug.Log("Not Owner");
                }
            }
        }
        lock (UIController.roomSceneLock)
        {
            for(int i=0; i<MAX_PLAYER_NUM_IN_ROOM; i++)
            {
                if (sRoomInfo.SeatInfo[i].Empty == SEAT_EMPTY)
                {
                    UIController.Instance.seatInfo[i].empty = true;
                    UIController.Instance.seatInfo[i].seatNo = i;
                }
                else
                {
                    UIController.Instance.seatInfo[i].empty = false;
                    UIController.Instance.seatInfo[i].playerName = sRoomInfo.SeatInfo[i].PlayerInfo.PlayerName;
                    UIController.Instance.seatInfo[i].character = sRoomInfo.SeatInfo[i].PlayerInfo.CharType;
                    UIController.Instance.seatInfo[i].seatNo = i;
                    if (sRoomInfo.SeatInfo[i].PlayerInfo.Ready == PLAYER_READY)
                    {
                        UIController.Instance.seatInfo[i].ready = true;
                    }
                    else
                    {
                        UIController.Instance.seatInfo[i].ready = false;
                    }
                    if(sRoomInfo.SeatInfo[i].PlayerInfo.Owner == ROOM_OWNER)
                    {
                        UIController.Instance.seatInfo[i].owner = true;
                    }
                    else
                    {
                        UIController.Instance.seatInfo[i].owner = false;
                    }
                }
            }
            UIController.Instance.seatInfoChange = UIController.CHANGED;
        }
    }

    /// <summary>
    /// Process ExitRoom data from server
    /// </summary>
    private void ProcessExitRoom()
    {
        Debug.Log("Receive Command: ExitRoom");
        S_ExitRoom sExitRoom = new S_ExitRoom();
        sExitRoom.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receivedLength - PACKAGE_HEAD_LENGTH);
        Debug.Log("State: " + sExitRoom.State);
        lock (UIController.roomSceneLock)
        {
            UIController.Instance.exitRoomState = sExitRoom.State;
            UIController.Instance.exitRoomStateChange = UIController.CHANGED;
        }
    }

    /// <summary>
    /// Process UpdateRoom data from server
    /// </summary>
    private void ProcessUpdateRoom()
    {
        Debug.Log("Receive Command: UpdateRoom");
        S_UpdateRoom sUpdateRoom = new S_UpdateRoom();
        sUpdateRoom.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receivedLength - PACKAGE_HEAD_LENGTH);
        Debug.Log("Seat No." + sUpdateRoom.SeatInfo.SeatNo);
        if (sUpdateRoom.SeatInfo.Empty == SEAT_EMPTY)
        {
            Debug.Log("Empty");
        }
        else
        {
            Debug.Log("Player Name" + sUpdateRoom.SeatInfo.PlayerInfo.PlayerName);
            Debug.Log("Character" + sUpdateRoom.SeatInfo.PlayerInfo.CharType);
            if (sUpdateRoom.SeatInfo.PlayerInfo.Ready == PLAYER_READY)
            {
                Debug.Log("Ready");
            }
            else
            {
                Debug.Log("Not Ready");
            }
            if (sUpdateRoom.SeatInfo.PlayerInfo.Owner == ROOM_OWNER)
            {
                Debug.Log("Owner");
            }
            else
            {
                Debug.Log("Not Owner");
            }
        }
        lock (UIController.roomSceneLock)
        {
            if (sUpdateRoom.SeatInfo.Empty == SEAT_EMPTY)
            {
                UIController.Instance.seatInfo[sUpdateRoom.SeatInfo.SeatNo].empty = true;
            }
            else
            {
                UIController.Instance.seatInfo[sUpdateRoom.SeatInfo.SeatNo].empty = false;
                UIController.Instance.seatInfo[sUpdateRoom.SeatInfo.SeatNo].playerName = sUpdateRoom.SeatInfo.PlayerInfo.PlayerName;
                UIController.Instance.seatInfo[sUpdateRoom.SeatInfo.SeatNo].character = sUpdateRoom.SeatInfo.PlayerInfo.CharType;
                UIController.Instance.seatInfo[sUpdateRoom.SeatInfo.SeatNo].seatNo = sUpdateRoom.SeatInfo.SeatNo;
                if (sUpdateRoom.SeatInfo.PlayerInfo.Ready == PLAYER_READY)
                {
                    UIController.Instance.seatInfo[sUpdateRoom.SeatInfo.SeatNo].ready = true;
                }
                else
                {
                    UIController.Instance.seatInfo[sUpdateRoom.SeatInfo.SeatNo].ready = false;
                }
                if (sUpdateRoom.SeatInfo.PlayerInfo.Owner == ROOM_OWNER)
                {
                    UIController.Instance.seatInfo[sUpdateRoom.SeatInfo.SeatNo].owner = true;
                }
                else
                {
                    UIController.Instance.seatInfo[sUpdateRoom.SeatInfo.SeatNo].owner = false;
                }
            }
            UIController.Instance.seatInfoUpdateChange = sUpdateRoom.SeatInfo.SeatNo;
        }
    }

    /// <summary>
    /// Process ModifyChar data from server
    /// </summary>
    private void ProcessModifyChar()
    {
        Debug.Log("Receive Command: ModifyChar");
    }

    /// <summary>
    /// Process Ready data from server
    /// </summary>
    private void ProcessReady()
    {
        Debug.Log("Receive Command: Ready");
        S_Ready sReady = new S_Ready();
        sReady.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receivedLength - PACKAGE_HEAD_LENGTH);
        Debug.Log("State: " + sReady.State);
        lock (UIController.roomSceneLock)
        {
            if(sReady.State == 1)
            {
                UIController.Instance.readyState = PLAYER_READY;
                UIController.Instance.readyStateChange = UIController.CHANGED;
            }
        }
    }

    /// <summary>
    /// Process CancelReady data from server
    /// </summary>
    private void ProcessCancelReady()
    {
        Debug.Log("Receive Command: CancelReady");
        S_CancelReady sCancelReady = new S_CancelReady();
        sCancelReady.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receivedLength - PACKAGE_HEAD_LENGTH);
        Debug.Log("State: " + sCancelReady.State);
        lock (UIController.roomSceneLock)
        {
            if (sCancelReady.State == 1)
            {
                UIController.Instance.readyState = PLAYER_NO_READY;
                UIController.Instance.readyStateChange = UIController.CHANGED;
            }
        }
    }

    /// <summary>
    /// Process StartGame data from server
    /// </summary>
    private void ProcessStartGame()
    {
        Debug.Log("Receive Command: StartGame");
        S_StartGame sStartGame = new S_StartGame();
        sStartGame.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receivedLength - PACKAGE_HEAD_LENGTH);
        // Record 
        GlobalController.Instance.championNo = RoomSceneController.Instance.chosenCharacter;
        GlobalController.Instance.playerSeatNo = UIController.Instance.seatNo;
        // Set port
        GlobalController.Instance.gameServerPort = sStartGame.Port;
        // Set Flag
        lock (GlobalController.Instance.gameStartLock)
        {
            GlobalController.Instance.gameStart = true;
        }
    }

    /// <summary>
    /// Process ExitLogin data from server
    /// </summary>
    private void ProcessExitLogin()
    {
        Debug.Log("Receive Command: ExitLogin");
        S_ExitLogin sExitLogin = new S_ExitLogin();
        sExitLogin.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receivedLength - PACKAGE_HEAD_LENGTH);
        lock (UIController.hallSceneLock)
        {
            UIController.Instance.exitLoginStateChange = UIController.CHANGED;
        }
    }

    /// <summary>
    /// Process Quit data from server
    /// </summary>
    private void ProcessQuit()
    {
        // Close connection
        /*if (mainClientSocket != null)
        {
            Debug.Log("¶Ï¿ªÁ¬½Ó");
            try
            {
                mainClientSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception)
            {
                throw;
            }
            mainClientSocket.Close();
        }*/

        // Close Application
    }

    /// <summary>
    /// Process AddFriend data from server
    /// </summary>
    private void ProcessAddFriend()
    {
        Debug.Log("Receive Command: AddFriend");
        S_AddFriend sAddFriend = new S_AddFriend();
        sAddFriend.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receivedLength - PACKAGE_HEAD_LENGTH);
        Debug.Log("State: " + sAddFriend.State);
        if (sAddFriend.State == SEND_FRIEND_FAIL)
        {
            lock (UIController.hallSceneLock)
            {
                UIController.Instance.failAddFriendCanvasChange = UIController.CHANGED;
            }
        }
    }

    /// <summary>
    /// Process OtherAddFriend data from server
    /// </summary>
    private void ProcessOtherAddFriend()
    {
        Debug.Log("Receive Command: OtherAddFriend");
        S_OtherAddFriend sOtherAddFriend = new S_OtherAddFriend();
        sOtherAddFriend.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receivedLength - PACKAGE_HEAD_LENGTH);
        Debug.Log("Other Friend Name: " + sOtherAddFriend.FriendInfo.FriendName);
        lock (UIController.hallSceneLock)
        {
            UIController.Instance.otherAddFriendName = sOtherAddFriend.FriendInfo.FriendName;
            UIController.Instance.otherAddFriendChange = UIController.CHANGED;
        }
    }

    /// <summary>
    /// Process CreateRoom data from server
    /// </summary>
    private void ProcessCreateRoom()
    {
        Debug.Log("Receive Command: CreateRoom");
        S_CreateRoom SCreateRoom = new S_CreateRoom();
        SCreateRoom.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receivedLength - PACKAGE_HEAD_LENGTH);
        Debug.Log("State: " + SCreateRoom.State);
        lock (UIController.hallSceneLock)
        {
            UIController.Instance.roomNo = SCreateRoom.RoomNo;
            UIController.Instance.roomName = SCreateRoom.RoomName;
            UIController.Instance.createRoomState = SCreateRoom.State;
            UIController.Instance.createRoomStateChange = UIController.CHANGED;
        }
    }

    /* public */
    /// <summary>
    /// Send Login to server
    /// </summary>
    public void SendLogin(string name, string password)
    {
        C_Login c_login = new C_Login();
        /* Set Name */
        c_login.Name = name;
        /* Set Password */
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] password_md5 = md5.ComputeHash(Encoding.UTF8.GetBytes(password));    // Compute MD5 of password
        c_login.PasswordMd5 = Encoding.ASCII.GetString(password_md5);
        /* Send */
        SendData(c_login.ToByteArray(), (int)ClientCommandType.CLogin);
    }

    /// <summary>
    /// Send Register to server
    /// </summary>
    public void SendRegister(string name, string password)
    {
        C_Register cRegister = new C_Register();
        /* Set Name */
        cRegister.Name = name;
        /* Set Password */
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] passwordMd5 = md5.ComputeHash(Encoding.UTF8.GetBytes(password));    // Compute MD5 of password
        cRegister.PasswordMd5 = Encoding.ASCII.GetString(passwordMd5);
        /* Send */
        SendData(cRegister.ToByteArray(), (int)ClientCommandType.CRegister);
    }

    /// <summary>
    /// Send HallRoom to server
    /// </summary>
    public void SendHallRoom()
    {
        /* Send */
        SendData((int)ClientCommandType.CHallRoom);
    }

    /// <summary>
    /// Send Friend to server
    /// </summary>
    public void SendFriend()
    {
        /* Send */
        SendData((int)ClientCommandType.CFriend);
    }

    /// <summary>
    /// Send EnterRoom to server
    /// </summary>
    public void SendEnterRoom(string roomNo)
    {
        C_EnterRoom cEnterRoom = new C_EnterRoom();
        /* Set room_no */
        cEnterRoom.RoomNo = roomNo;
        /* Set Player Room No */
        playerRoomNo = roomNo;
        /* Send */
        SendData(cEnterRoom.ToByteArray(), (int)ClientCommandType.CEnterRoom);
    }

    /// <summary>
    /// Send RoomInfo to server
    /// </summary>
    public void SendRoomInfo(string roomNo)
    {
        C_RoomInfo cRoomInfo  = new C_RoomInfo();
        /* Set room_no */
        cRoomInfo.RoomNo = roomNo;
        /* Send */
        SendData(cRoomInfo.ToByteArray(), (int)ClientCommandType.CRoomInfo);
    }

    /// <summary>
    /// Send ModifyChar to server
    /// </summary>
    public void SendModifyChar(int charType)
    {
        C_ModChar cModChar = new C_ModChar();
        /* Set char_type */
        cModChar.CharType = charType;
        /* Send */
        SendData(cModChar.ToByteArray(), (int)ClientCommandType.CModifyChar);
    }

    /// <summary>
    /// Send Ready to server
    /// </summary>
    public void SendReady()
    {
        /* Send */
        SendData((int)ClientCommandType.CReady);
    }

    /// <summary>
    /// Send CancelReady to server
    /// </summary>
    public void SendCancelReady()
    {
        /* Send */
        SendData((int)ClientCommandType.CCancelReady);
    }

    /// <summary>
    /// Send StartGame to server
    /// </summary>
    public void SendStartGame()
    {
        /* Send */
        SendData((int)ClientCommandType.CStartGame);
    }

    /// <summary>
    /// Send ExitLogin to server
    /// </summary>
    public void SendExitLogin()
    {
        /* Send */
        SendData((int)ClientCommandType.CExitLogin);
    }

    /// <summary>
    /// Send Quit to server
    /// </summary>
    public void SendQuit()
    {
        /* Send */
        SendData((int)ClientCommandType.CQuit);
    }

    /// <summary>
    /// Send ExitRoom to server
    /// </summary>
    public void SendExitRoom()
    {
        /* Send */
        SendData((int)ClientCommandType.CExitRoom);
    }

    /// <summary>
    /// Send CreateRoom to server
    /// </summary>
    public void SendCreateRoom(string roomName)
    {
        C_CreateRoom cCreateRoom = new C_CreateRoom();
        /* Set char_type */
        cCreateRoom.RoomName = roomName;
        /* Send */
        SendData(cCreateRoom.ToByteArray(), (int)ClientCommandType.CCreateRoom);
    }

    /// <summary>
    /// Send AddFriend to server
    /// </summary>
    public void SendAddFriend(string friendName)
    {
        C_AddFriend cAddFriend = new C_AddFriend();
        /* Set friend_name */
        cAddFriend.FriendName = friendName;
        /* Send */
        SendData(cAddFriend.ToByteArray(), (int)ClientCommandType.CAddFriend);
    }

    /// <summary>
    /// Send ReplyFriend to server
    /// </summary>
    public void SendReplyFriend(string friendName, int state)
    {
        C_ReplyFriend cReplyFriend = new C_ReplyFriend();
        /* Set friend_name */
        cReplyFriend.FriendName = friendName;
        /* Set state */
        cReplyFriend.State = state;
        /* Send */
        SendData(cReplyFriend.ToByteArray(), (int)ClientCommandType.CReplyFriend);
    }


}


