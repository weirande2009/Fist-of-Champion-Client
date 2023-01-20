using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;

using ChampionFist;
using Google.Protobuf;


public class Client
{
    /* Constant */
    /* Protocol */
    public const int PACKAGE_HEAD_LENGTH = 8;
    /* GameClient */
    public const int CLIENT_CLOSE = -1;        // State of main client close

    /* Variables */
    /* Process Function pool */
    public delegate void ProcessFunctionPool();
    public List<ProcessFunctionPool> processFunctionPool;
    /* Client and Server info */
    protected string serverIp;                          // Server IP address
    protected int serverPort;                           // Server port no.
    protected Socket ClientSocket;                      // Main client socket
    protected IPEndPoint endPoint;                      // End Point of Connection
    public object ClientLock;                           // Lock of MainClient
    public int ClientState;                             // Main Client State
    /* Receive info */
    protected int waitingReadLength;                    // Length of waiting to read
    protected int receivedLength;                       // Received length
    protected byte[] receiveBuf = new byte[1024];       // Receive Buffer
    /* Thread */
    protected Thread threadListenServer;                // Thread for read data from server
    protected Thread threadSendServer;                  // Thread for send data to server
    public object sendThreadLock;                       // Lock for send thread
    protected int sendThreadSleepTime = 1;              // Sleep time for send thread
    /* SendPackages */
    protected List<byte[]> sendPackages;                // Send Packages List

    /* Functions */
    /* private */
    // Start is called before the first frame update
    public Client(string _ip, int _port)
    {
        /* Client and Server */
        serverIp = _ip;
        serverPort = _port;
        

        /* MainClient */
        ClientLock = new object();
        ClientState = 0;

        /* Receive info */
        waitingReadLength = PACKAGE_HEAD_LENGTH;
        receivedLength = 0;

        /* Process Function Pool */
        processFunctionPool = new List<ProcessFunctionPool>();

        /* Set sendPackages */
        sendPackages = new List<byte[]>();
    }

    public void Start()
    {
        ConnectServer();
    }

    public void WatchExit()
    {
        lock (ClientLock)
        {
            if (ClientState == Client.CLIENT_CLOSE)
            {
                Debug.Log("断开连接");
                Debug.Log("关闭程序");
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }

    /// <summary>
    /// Start connection
    /// </summary>
    private void ConnectServer()
    {

        // Set server info
        endPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
        // Instantiate socket
        GameSceneController.Instance.gameInfoController.AddInfo("建立客户端...");
        ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            // Connect server
            GameSceneController.Instance.gameInfoController.AddInfo("正在连接服务器...");
            ClientSocket.Connect(endPoint);
            // Start a new thread to process data from server
            GameSceneController.Instance.gameInfoController.AddInfo("开启监听接收数据线程...");
            threadListenServer = new Thread(ListeningRecvData);
            threadListenServer.IsBackground = true;
            threadListenServer.Name = "threadListenGameServer";
            threadListenServer.Start();
            // Start a new thread to process data to server
            /*GameSceneController.Instance.gameInfoController.AddInfo("开启监听发送数据线程...");
            threadSendServer = new Thread(ListeningSendData);
            threadSendServer.IsBackground = true;
            threadSendServer.Name = "threadSendGameServer";
            threadSendServer.Start();*/
            // Set Lock for send thread
            sendThreadLock = new object();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void Disconnect()
    {
        ClientSocket.Close();
    }

    /// <summary>
    /// Listen data from server in a new thread
    /// </summary>
    private void ListeningRecvData()
    {
        try
        {
            while (true)
            {
                // Waiting for data from server
                int rlen = ClientSocket.Receive(receiveBuf, receivedLength, waitingReadLength, SocketFlags.None);
                //Debug.Log("收到来自服务器的消息，长度为" + rlen.ToString());
                if (rlen < 0)
                {
                    // something wrong
                }
                else if (rlen == 0)
                {
                    // disconnect
                    Application.Quit();
                }
                else
                {
                    if (rlen < waitingReadLength)  // if data not enough
                    {
                        // record data and continue waiting
                        waitingReadLength -= rlen;
                        receivedLength += rlen;
                    }
                    else if (rlen > waitingReadLength)
                    {
                        // something wrong
                    }
                    else  // if data enough
                    {
                        receivedLength += rlen;
                        // Get command and package total length
                        int cmd = BitConverter.ToInt32(receiveBuf, 0);
                        int packageLength = BitConverter.ToInt32(receiveBuf, 4);
                        if (receivedLength == packageLength)  // if receive all
                        {
                            //Debug.Log("功能号：" + cmd.ToString() + " 长度：" + packageLength.ToString());
                            processFunctionPool[cmd - 1]();
                            receivedLength = 0;
                            waitingReadLength = PACKAGE_HEAD_LENGTH;
                        }
                        else  // if only receive package head
                        {
                            waitingReadLength = packageLength - PACKAGE_HEAD_LENGTH;
                        }
                    }
                }
            }
        }
        catch
        {

        }
        finally
        {
            /*mainClientSocket.Disconnect(true);
            mainClientSocket.Close();*/
            lock (ClientLock)
            {
                ClientState = CLIENT_CLOSE;
            }
        }

    }

    /// <summary>
    /// Listen data from server in a new thread
    /// </summary>
    private void ListeningSendData()
    {
        while (true)
        {
            lock (sendThreadLock)
            {
                for (int i = 0; i < sendPackages.Count; i++)
                {
                    ClientSocket.Send(sendPackages[i]);
                }
                sendPackages.Clear();
            }
            Thread.Sleep(sendThreadSleepTime);
        }
    }

    /// <summary>
    /// Send data with content to server
    /// </summary>
    protected void SendData(byte[] data, int cmd)
    {
        byte[] sent_data = new byte[data.Length + PACKAGE_HEAD_LENGTH];
        Array.Copy(BitConverter.GetBytes(cmd), 0, sent_data, 0, sizeof(int));
        Array.Copy(BitConverter.GetBytes(data.Length + PACKAGE_HEAD_LENGTH), 0, sent_data, sizeof(int), sizeof(int));
        Array.Copy(data, 0, sent_data, PACKAGE_HEAD_LENGTH, data.Length);
        // Send Data
        ClientSocket.Send(sent_data);
        // Add sent_data to send list
        /*lock (sendThreadLock)
        {
            sendPackages.Add(sent_data);
        }*/
    }

    /// <summary>
    /// Send data without content to server
    /// </summary>
    protected void SendData(int cmd)
    {
        byte[] sent_data = new byte[PACKAGE_HEAD_LENGTH];
        Array.Copy(BitConverter.GetBytes(cmd), 0, sent_data, 0, sizeof(int));
        Array.Copy(BitConverter.GetBytes(PACKAGE_HEAD_LENGTH), 0, sent_data, sizeof(int), sizeof(int));
        // Send Data
        ClientSocket.Send(sent_data);
        // Add sent_data to send list
        /*lock (sendThreadLock)
        {
            sendPackages.Add(sent_data);
        }*/
    }

}


public class UDPClient
{
    /* Constant */
    /* Protocol */
    public const int PACKAGE_HEAD_LENGTH = 8;
    /* GameClient */
    public const int CLIENT_CLOSE = -1;        // State of main client close

    /* Variables */
    /* Process Function pool */
    public delegate void ProcessFunctionPool();
    public List<ProcessFunctionPool> processFunctionPool;
    /* Client and Server info */
    UdpClient udpClient;                                // Udp Client Object
    IPEndPoint serverEndPoint;                          // Server End Point
    protected string serverIp;                          // Server IP address
    protected int serverPort;                           // Server port no.
    public object ClientLock;                           // Lock of MainClient
    public int ClientState;                             // Main Client State
    /* Receive info */
    protected byte[] receiveBuf;                        // Receive Buffer
    /* Thread */
    protected Thread threadListenServer;                // Thread for read data from server
    protected Thread threadSendServer;                  // Thread for send data to server
    public object sendThreadLock;                       // Lock for send thread
    protected int sendThreadSleepTime = 1;              // Sleep time for send thread
    /* SendPackages */
    protected List<byte[]> sendPackages;                // Send Packages List

    /* Functions */
    /* private */
    // Start is called before the first frame update
    public UDPClient(string _ip, int _port)
    {
        /* Client and Server */
        serverIp = _ip;
        serverPort = _port;
        serverEndPoint = new IPEndPoint(IPAddress.Parse(_ip), serverPort);

        /* MainClient */
        ClientLock = new object();
        ClientState = 0;

        /* Process Function Pool */
        processFunctionPool = new List<ProcessFunctionPool>();

        /* Set sendPackages */
        sendPackages = new List<byte[]>();
    }

    public void Start()
    {
        ConnectServer();
    }

    public void WatchExit()
    {
        lock (ClientLock)
        {
            if (ClientState == Client.CLIENT_CLOSE)
            {
                Debug.Log("断开连接");
            }
        }
    }

    /// <summary>
    /// Start connection
    /// </summary>
    private void ConnectServer()
    {
        try
        {
            // Connect server
            GameSceneController.Instance.gameInfoController.AddInfo("正在连接服务器...");
            udpClient = new UdpClient();
            udpClient.Connect(serverEndPoint);
            // Start a new thread to process data from server
            GameSceneController.Instance.gameInfoController.AddInfo("开启监听接收数据线程...");
            threadListenServer = new Thread(ListeningRecvData);
            threadListenServer.IsBackground = true;
            threadListenServer.Name = "threadListenGameServer";
            threadListenServer.Start();
            // Start a new thread to process data to server
            /*GameSceneController.Instance.gameInfoController.AddInfo("开启监听发送数据线程...");
            threadSendServer = new Thread(ListeningSendData);
            threadSendServer.IsBackground = true;
            threadSendServer.Name = "threadSendGameServer";
            threadSendServer.Start();*/
            // Set Lock for send thread
            sendThreadLock = new object();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void Disconnect()
    {
        udpClient.Close();
    }

    /// <summary>
    /// Listen data from server in a new thread
    /// </summary>
    private void ListeningRecvData()
    {
        try
        {
            while (true)
            {
                // Waiting for data from server
                receiveBuf = udpClient.Receive(ref serverEndPoint);
                //Debug.Log("收到来自服务器的消息，长度为" + rlen.ToString());
                // Get command and package total length
                int cmd = BitConverter.ToInt32(receiveBuf, 0);
                int packageLength = BitConverter.ToInt32(receiveBuf, 4);
                if (receiveBuf.Length == packageLength)  // if receive all
                {
                    //Debug.Log("功能号：" + cmd.ToString() + " 长度：" + packageLength.ToString());
                    processFunctionPool[cmd - 1]();
                }
                else  // if only receive package head
                {
                    Debug.Log("丢包了");
                }
            }
        }
        catch
        {

        }
        finally
        {
            /*mainClientSocket.Disconnect(true);
            mainClientSocket.Close();*/
            lock (ClientLock)
            {
                ClientState = CLIENT_CLOSE;
            }
        }

    }

    /// <summary>
    /// Send data with content to server
    /// </summary>
    protected void SendData(byte[] data, int cmd)
    {
        byte[] sent_data = new byte[data.Length + PACKAGE_HEAD_LENGTH];
        Array.Copy(BitConverter.GetBytes(cmd), 0, sent_data, 0, sizeof(int));
        Array.Copy(BitConverter.GetBytes(data.Length + PACKAGE_HEAD_LENGTH), 0, sent_data, sizeof(int), sizeof(int));
        Array.Copy(data, 0, sent_data, PACKAGE_HEAD_LENGTH, data.Length);
        // Send Data
        udpClient.Send(sent_data, sent_data.Length);
    }

    /// <summary>
    /// Send data without content to server
    /// </summary>
    protected void SendData(int cmd)
    {
        byte[] sent_data = new byte[PACKAGE_HEAD_LENGTH];
        Array.Copy(BitConverter.GetBytes(cmd), 0, sent_data, 0, sizeof(int));
        Array.Copy(BitConverter.GetBytes(PACKAGE_HEAD_LENGTH), 0, sent_data, sizeof(int), sizeof(int));
        // Send Data
        udpClient.Send(sent_data, sent_data.Length);
    }

}





