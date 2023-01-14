using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using ChampionFistGame;

public class OfflineServer
{
    protected Thread threadSendServer;                  // Thread for send data to server
    private int syncFrameId;
    private List<int> playerFrameId;
    private int playerNum;
    private List<UnsyncFrame> allLogicFrames;
    private List<OperationFrame> playerOperations;
    private object playerOperationLock;
    private bool operationUpdated;

    // Start is called before the first frame update
    public OfflineServer(int _playerNum)
    {
        playerOperationLock = new object();
        playerNum = _playerNum;
        syncFrameId = -1;
        allLogicFrames = new List<UnsyncFrame>();
        playerOperations = new List<OperationFrame>();
        playerFrameId = new List<int>();
        for (int i = 0; i < playerNum; i++)
        {
            playerOperations.Add(new OperationFrame());
            playerFrameId.Add(-1);
        }
        ClearOperation();
        operationUpdated = true;
    }

    public void SetPlayerOperation(int seatNo, int frameId, OperationFrame operationFrame)
    {
        lock (playerOperationLock)
        {
            playerOperations[seatNo] = new OperationFrame(operationFrame);
            playerFrameId[seatNo] = frameId - 1;
            operationUpdated = true;
        }
    }

    private void ClearOperation()
    {
        for (int i = 0; i < playerNum; i++)
        {
            playerOperations[i].ClickQ = false;
            playerOperations[i].ClickW = false;
            playerOperations[i].ClickE = false;
            playerOperations[i].ClickR = false;
            playerOperations[i].ClickProperty = false;
            playerOperations[i].ClickMouse = false;
            playerOperations[i].ChangeWeapon = false;
            playerOperations[i].ChangeArmor = false;
            playerOperations[i].MousePosX = 0;
            playerOperations[i].MousePosY = 0;
            playerOperations[i].ArmorNo = -1;
            playerOperations[i].LowWeaponNo = -1;
            playerOperations[i].MiddleWeaponNo = -1;
            playerOperations[i].HighWeaponNo = -1;
        }
    }

    public void Start()
    {
        threadSendServer = new Thread(SendLogicFrame);
        threadSendServer.IsBackground = true;
        threadSendServer.Name = "threadOfflineServer";
        threadSendServer.Start();
    }

    private void SendLogicFrame()
    {
        Thread.Sleep(1000);
        while (true)
        {
            lock (playerOperationLock)
            {
                if (operationUpdated)
                {
                    operationUpdated = false;
                    // Generate new frame
                    syncFrameId++;
                    Debug.Log("Send Logic Frame: " + syncFrameId);
                    UnsyncFrame newFrame = new UnsyncFrame();
                    newFrame.FrameId = syncFrameId;
                    for (int i = 0; i < playerNum; i++)
                    {
                        newFrame.AllPlayersOpt.Add(new OperationFrame(playerOperations[i]));
                    }
                    allLogicFrames.Add(newFrame);
                    // Send Logic Frame
                    for (int i = 0; i < 1; i++)
                    {
                        S_LogicFrame sLogicFrame = new S_LogicFrame();
                        sLogicFrame.FrameId = syncFrameId;
                        for (int j = playerFrameId[i] + 1; j <= syncFrameId; j++)
                        {
                            sLogicFrame.UnsyncFrames.Add(allLogicFrames[j]);
                        }
                        GlobalController.Instance.gameClient.ProcessLogicFrame(sLogicFrame);
                    }
                    ClearOperation();
                }
            }
            Thread.Sleep(66);
        }
    }




}
