using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;

using ChampionFistGame;
using Google.Protobuf;


public class GameClient : UDPClient
{
    /* Variables */
    private bool offline;

    /* Functions */
    /* private */
    // Start is called before the first frame update
    public GameClient(string _ip, int _port, bool _offline=false) : base(_ip, _port)
    {
        offline = _offline;

        /* Process Function Pool */
        processFunctionPool.Add(ProcessConnect);
        processFunctionPool.Add(ProcessInitialize);
        processFunctionPool.Add(ProcessLoad);
        processFunctionPool.Add(ProcessExitGame);
        processFunctionPool.Add(ProcessOtherExit);
        processFunctionPool.Add(ProcessLogicFrame);
        processFunctionPool.Add(ProcessDelay);
        processFunctionPool.Add(ProcessStart);
    }

    /// <summary>
    /// Process Login data from server
    /// </summary>
    private void ProcessConnect()
    {
        //Debug.Log("Receive Command: Connect");
    }

    /// <summary>
    /// Process Initialize data from server
    /// </summary>
    private void ProcessInitialize()
    {
        //Debug.Log("Receive Command: Initialize");
        S_Initialize sInitialize = new S_Initialize();
        sInitialize.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receiveBuf.Length - PACKAGE_HEAD_LENGTH);
        // Set Initialize Signal
        lock (GlobalController.Instance.gameController.gameControllerLock)
        {
            int mainPlayerNo = 0;
            GlobalController.Instance.playerSeatNo = sInitialize.SeatNo;
            for (int i = 0; i < sInitialize.GamePlayersInfo.Count; i++)
            {
                if (sInitialize.GamePlayersInfo[i].SeatNo == GlobalController.Instance.playerSeatNo)
                {
                    GlobalController.Instance.gameController.AddPlayerInfo(sInitialize.GamePlayersInfo[i].Name, true, sInitialize.GamePlayersInfo[i].ChampionNo);
                    mainPlayerNo = i;
                }
                else
                {
                    GlobalController.Instance.gameController.AddPlayerInfo(sInitialize.GamePlayersInfo[i].Name, false, sInitialize.GamePlayersInfo[i].ChampionNo);
                }
            }
            GlobalController.Instance.gameController.SetBasicInfo(sInitialize.GamePlayersInfo.Count, mainPlayerNo);
            GlobalController.Instance.gameController.randomSeed = sInitialize.RandomSeed;
            GlobalController.Instance.gameController.initializing = true;
        }
    }

    /// <summary>
    /// Process Load data from server
    /// </summary>
    private void ProcessLoad()
    {
        //Debug.Log("Receive Command: Load");
    }

    /// <summary>
    /// Process Start data from server
    /// </summary>
    private void ProcessStart()
    {
        //Debug.Log("Receive Command: Start");
        lock (GlobalController.Instance.gameController.gameControllerLock)
        {
            GlobalController.Instance.gameController.starting = true;
        }
    }

    /// <summary>
    /// Process Exit Game data from server
    /// </summary>
    private void ProcessExitGame()
    {
        //Debug.Log("Receive Command: Exit");
        GlobalController.Instance.gameController.Exit();
    }

    /// <summary>
    /// Process Other Exit data from server
    /// </summary>
    private void ProcessOtherExit()
    {
        //Debug.Log("Receive Command: Other Exit");
        S_OtherExit sOtherExit = new S_OtherExit();
        sOtherExit.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receiveBuf.Length - PACKAGE_HEAD_LENGTH);
        GlobalController.Instance.gameController.players[sOtherExit.SeatNo].SetOffline();
    }

    /// <summary>
    /// Process Logic Frame data from server
    /// </summary>
    private void ProcessLogicFrame()
    {
        //Debug.Log("Receive Command: Logic Frame");
        S_LogicFrame sLogicFrame = new S_LogicFrame();
        sLogicFrame.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receiveBuf.Length - PACKAGE_HEAD_LENGTH);
        lock (GlobalController.Instance.gameController.operationLock)
        {
            GlobalController.Instance.gameController.logicFrame = sLogicFrame;
            GlobalController.Instance.gameController.logicFrameUpdate = true;
        }
    }

    public void ProcessLogicFrame(S_LogicFrame sLogicFrame)
    {
        sLogicFrame.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receiveBuf.Length - PACKAGE_HEAD_LENGTH);
        lock (GlobalController.Instance.gameController.operationLock)
        {
            GlobalController.Instance.gameController.logicFrame = sLogicFrame;
            GlobalController.Instance.gameController.logicFrameUpdate = true;
        }
    }

    /// <summary>
    /// Process Delay data from server
    /// </summary>
    private void ProcessDelay()
    {
        //Debug.Log("Receive Command: Delay");
        lock (GlobalController.Instance.gameController.gameDelayLock)
        {
            GlobalController.Instance.gameController.setDelay = true;
        }
    }

    /* public */
    /// <summary>
    /// Send Connect to server
    /// </summary>
    public void SendConnect(string name, int championNo, int seatNo)
    {
        C_Connect cConnect = new C_Connect();
        /* Set Name */
        cConnect.Name = name;
        /* Set Champion No. */
        cConnect.ChampionNo = championNo;
        /* Set Seat No. */
        cConnect.SeatNo = seatNo;
        /* Send */
        if (!offline)
        {
            SendData(cConnect.ToByteArray(), (int)ClientCommandType.CConnect);
        }
        else
        {
            S_Initialize sInitialize = new S_Initialize();
            sInitialize.SeatNo = 0;
            sInitialize.RandomSeed = 0;
            GamePlayerInfo gamePlayerInfo = new GamePlayerInfo();
            gamePlayerInfo.SeatNo = 0;
            gamePlayerInfo.Name = name;
            gamePlayerInfo.ChampionNo = championNo;
            sInitialize.GamePlayersInfo.Add(gamePlayerInfo);
            GamePlayerInfo gamePlayerInfo2 = new GamePlayerInfo();
            gamePlayerInfo2.SeatNo = 1;
            gamePlayerInfo2.Name = "TEST2";
            gamePlayerInfo2.ChampionNo = 1;
            sInitialize.GamePlayersInfo.Add(gamePlayerInfo2);
            /*GamePlayerInfo gamePlayerInfo3 = new GamePlayerInfo();
            gamePlayerInfo3.SeatNo = 2;
            gamePlayerInfo3.Name = "TEST3";
            gamePlayerInfo3.ChampionNo = 0;
            sConnect.GamePlayersInfo.Add(gamePlayerInfo3);*/
            receiveBuf = new byte[PACKAGE_HEAD_LENGTH + sInitialize.ToByteArray().Length];
            sInitialize.ToByteArray().CopyTo(receiveBuf, PACKAGE_HEAD_LENGTH);
            ProcessInitialize();
        }
    }

    /// <summary>
    /// Send Load to server
    /// </summary>
    public void SendLoad()
    {
        /* Send */
        if (!offline)
        {
            SendData((int)ClientCommandType.CLoad);
        }
        else
        {
            lock (GlobalController.Instance.gameController.gameControllerLock)
            {
                GlobalController.Instance.gameController.starting = true;
            }
        }
    }

    /// <summary>
    /// Send Exit to server
    /// </summary>
    public void SendExit()
    {
        /* Send */
        if (!offline)
        {
            SendData((int)ClientCommandType.CExit);
        }
        else
        {
            ProcessExitGame();
        }
        
    }

    /// <summary>
    /// Send Operation to server
    /// </summary>
    public void SendOperation(int frameId, OperationFrame operationFrame)
    {
        if (!offline)
        {
            C_PlayerFrame cPlayerFrame = new C_PlayerFrame();
            cPlayerFrame.FrameId = frameId;
            cPlayerFrame.PlayerOptFrame = operationFrame;
            SendData(cPlayerFrame.ToByteArray(), (int)ClientCommandType.CPlayerFrame);
        }
        else
        {
            GlobalController.Instance.offlineServer.SetPlayerOperation(0, frameId, operationFrame);
            /*S_LogicFrame sLogicFrame = new S_LogicFrame();
            sLogicFrame.FrameId = frameId;
            UnsyncFrame unsyncFrame = new UnsyncFrame();
            unsyncFrame.FrameId = frameId;
            AllPlayerOperation allPlayerOperation = new AllPlayerOperation();
            allPlayerOperation.Operations.Add(operationFrame);
            unsyncFrame.AllPlayersOpt = allPlayerOperation;
            sLogicFrame.UnsyncFrames.Add(unsyncFrame);
            sLogicFrame.ToByteArray().CopyTo(receiveBuf, PACKAGE_HEAD_LENGTH);
            receivedLength = PACKAGE_HEAD_LENGTH + sLogicFrame.ToByteArray().Length;
            ProcessLogicFrame();*/
        }
    }

    /// <summary>
    /// Send Delay to server
    /// </summary>
    public void SendDelay()
    {
        /* Send */
        if (!offline)
        {
            SendData((int)ClientCommandType.CDelay);
        }
    }

    public void SendGameOver()
    {
        /* Send */
        if (!offline)
        {
            SendData((int)ClientCommandType.CGameover);
        }
    }


}


