using Google.Protobuf;
using NewChampionFist;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewMainClient : Client
{

    /* Functions */
    /* private */
    public NewMainClient(string _ip, int _port) : base(_ip, _port)
    {
        /* Process Function Pool */
        processFunctionPool = new List<ProcessFunctionPool>();
        processFunctionPool.Add(ProcessStart);
        processFunctionPool.Add(ProcessStart);
    }

    /// <summary>
    /// Process Start from server
    /// </summary>
    private void ProcessStart()
    {
        Debug.Log("Receive Command: Start");
        S_Start sStart = new S_Start();
        sStart.MergeFrom(receiveBuf, PACKAGE_HEAD_LENGTH, receivedLength - PACKAGE_HEAD_LENGTH);
        lock (GlobalController.Instance.gameStartLock)
        {
            GlobalController.Instance.gameStart = true;
            GlobalController.Instance.gameServerIP = sStart.GameServerIp;
            GlobalController.Instance.gameServerPort = sStart.GameServerPort;
        }
    }

    /// <summary>
    /// Send start Participate to server
    /// </summary>
    public void SendParticipate()
    {
        C_Participate cParticipate = new C_Participate();
        /* Send */
        SendData(cParticipate.ToByteArray(), (int)ClientCommandType.CParticipate);
    }


}
