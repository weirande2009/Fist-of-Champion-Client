using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HallSceneController : MonoBehaviour
{
    /* Instance */
    public static HallSceneController Instance;         // Class Instance

    /* UserName */
    public Text userNameText;                           // User Name Text

    /* Friend */
    private GameObject friendContent;                   // Friend content contain prefab
    public GameObject friendInfoCanvasPrefab;           // Friend Info Prefab
    private List<GameObject> friendInfoCanvasList;      // Friend Info Canvas
    public GameObject addFriendCanvas;                  // Add Friend Canvas
    public GameObject otherAddFriendCanvas;             // Other Add Friend Canvas
    public GameObject otherAddFriendContentPrefab;      // Other Add Friend Content Prefab
    public GameObject otherAddFriendContents;           // Other Add Friend Content
    public GameObject failAddOtherFriendCanvas;         // Fail to Add other friend canvas
    public GameObject failEnterRoomCanvas;              // Fail to Add other friend canvas
    public GameObject createRoomCanvas;                 // Create Room Canvas

    /* Room */
    private List<GameObject> roomInfoCanvasList;        // Room Info Canvas


    // Start is called before the first frame update
    void Start()
    {
        /* Set Instance */
        Instance = this;
        /* Set Friend */
        friendInfoCanvasList = new List<GameObject>();
        friendContent = GameObject.Find("FriendContent");
        DeleteAllFriendInfoCanvas();
        /* Set Room */
        roomInfoCanvasList = new List<GameObject>();
        GameObject roomInfoCanvas = GameObject.Find("RoomInfoCanvas");
        for(int i=0; i<MainClient.MAX_ROOM_NUMBER_IN_PAGE; i++)
        {
            Transform roomInfoCanvasChild = roomInfoCanvas.transform.Find("RoomInfo"+(i+1).ToString()+"Canvas");
            // Set Canvas Inactive
            roomInfoCanvasChild.gameObject.SetActive(false);
            roomInfoCanvasList.Add(roomInfoCanvasChild.gameObject);
        }
        /* Set Add Friend Canvas*/
        addFriendCanvas.SetActive(false);
        /* Set Other Add Friend Canvas*/
        otherAddFriendCanvas.SetActive(false);
        //otherAddFriendContents = GameObject.Find("OtherFriendContent");
        /* Set FailAddOtherFriend Canvas */
        failAddOtherFriendCanvas.SetActive(false);
        /* Set failEnterRoomCanvas Canvas */
        failEnterRoomCanvas.SetActive(false);
        /* Set CreateRoom Canvas */
        createRoomCanvas.SetActive(false);
        /* Set User Name Text */
        userNameText.text = GlobalController.Instance.userName;
        /* Send HallRoom */
        GlobalController.Instance.mainClient.SendHallRoom();
        /* Send Friend */
        GlobalController.Instance.mainClient.SendFriend();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Set Friend Info Canvas
    /// </summary>
    public void SetFriendInfoCanvas(FriendInfo friendInfo)
    {
        // Find friend
        foreach (GameObject friendInfoCanvas in friendInfoCanvasList)
        {
            if(friendInfoCanvas != null && friendInfoCanvas.name == friendInfo.friendName + "Canvas")
            {
                // Set State
                Transform offlineImage = friendInfoCanvas.transform.Find("OfflineImage");
                Transform onlineImage = friendInfoCanvas.transform.Find("OnlineImage");
                if (friendInfo.state == MainClient.PLAYER_ONLINE)
                {
                    onlineImage.gameObject.SetActive(true);
                    offlineImage.gameObject.SetActive(false);
                }
                else
                {
                    offlineImage.gameObject.SetActive(true);
                    onlineImage.gameObject.SetActive(false);
                }
            }
        }
    }

    public void DeleteAllFriendInfoCanvas()
    {
        for (int i = friendContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(friendContent.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Add Friend Info Canvas
    /// </summary>
    public void AddFriendInfoCanvas(FriendInfo friendInfo)
    {
        // Find whether there already one
        GameObject gameObject = GameObject.Find(friendInfo.friendName + "Canvas");
        if(gameObject != null)
        {
            return;
        }
        // Instantiate FriendInfoCanvas object
        GameObject friendInfoCanvas = Instantiate(friendInfoCanvasPrefab, friendContent.transform);
        // Set Canvas Name
        friendInfoCanvas.name = friendInfo.friendName + "Canvas";
        // Set Friend Name
        Transform nameText = friendInfoCanvas.transform.Find("NameText");
        nameText.GetComponent<Text>().text = friendInfo.friendName;
        // Set State
        if (friendInfo.state == MainClient.PLAYER_ONLINE)
        {
            Transform offlineImage = friendInfoCanvas.transform.Find("OfflineImage");
            offlineImage.gameObject.SetActive(false);
        }
        else
        {
            Transform onlineImage = friendInfoCanvas.transform.Find("OnlineImage");
            onlineImage.gameObject.SetActive(false);
        }
        // Add FriendInfoCanvas to List
        friendInfoCanvasList.Add(friendInfoCanvas);
        // Delete Other Friend Canvas if exist
        DeleteOtherAddFriend(friendInfo.friendName);
    }

    /// <summary>
    /// Update Room Info
    /// </summary>
    public void UpdateRoomInfo(RoomInfo roomInfo)
    {
        // Get object
        GameObject roomInfoCanvas = roomInfoCanvasList[roomInfo.roomIndex];
        if (roomInfo.empty)
        {
            // Set Canvas Inactive
            roomInfoCanvas.SetActive(false);
            return;
        }
        // Set Canvas Active
        roomInfoCanvas.SetActive(true);
        // Set Room No Text
        roomInfoCanvas.transform.Find("RoomNoText").GetComponent<Text>().text = roomInfo.roomNo;
        // Set Room Name Text
        roomInfoCanvas.transform.Find("RoomNameText").GetComponent<Text>().text = roomInfo.roomName;
        // Set Present Player Number Text
        roomInfoCanvas.transform.Find("RoomPlayerNumberText").GetComponent<Text>().text = "人数："+ roomInfo.prePlayerNumber.ToString()+"/8"; 
        // Set Button Text
        if(roomInfo.state == MainClient.GAME_START)
        {
            roomInfoCanvas.transform.Find("EnterRoomButton/Text").GetComponent<Text>().text = "正在对局中";
            roomInfoCanvas.transform.Find("EnterRoomButton").GetComponent<Button>().enabled = false;
        }
        else if(roomInfo.state == MainClient.GAME_NO_START)
        {
            roomInfoCanvas.transform.Find("EnterRoomButton/Text").GetComponent<Text>().text = "进入房间";
            roomInfoCanvas.transform.Find("EnterRoomButton").GetComponent<Button>().enabled = true;
        }
    }


    /// <summary>
    /// Callback function of click refresh button
    /// </summary>
    public void ClickRefreshButton()
    {
        GlobalController.Instance.mainClient.SendHallRoom();
    }

    /// <summary>
    /// Callback function of click enter room button
    /// </summary>
    public void ClickEnterRoomButton()
    {
        GameObject clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        for(int i=0; i<MainClient.MAX_ROOM_NUMBER_IN_PAGE; i++)
        {
            if (clickedButton.transform.parent.name == "RoomInfo" + (i + 1).ToString() + "Canvas")
            {
                string roomNo = clickedButton.transform.parent.transform.Find("RoomNoText").GetComponent<Text>().text;
                EnerRoom(roomNo);
                break;
            }
        }
    }

    /// <summary>
    /// Callback function of AddFriendCanvas Button
    /// </summary>
    public void ClickAddFriendCanvas()
    {
        // Set Canvas Active
        addFriendCanvas.SetActive(true);
    }

    /// <summary>
    /// Callback function of Cancel Button in AddFriendCanvas
    /// </summary>
    public void ClickCancelAddFriendCanvas()
    {
        // Set Canvas Inactive
        addFriendCanvas.SetActive(false);
    }

    /// <summary>
    /// Callback function of Confirm Button in AddFriendCanvas
    /// </summary>
    public void ClickConfirmAddFriendCanvas()
    {
        // Get Friend Name
        string friendName = addFriendCanvas.transform.Find("FriendNameInputField").GetComponent<InputField>().text;
        if(friendName.Length != 0)
        {
            GlobalController.Instance.mainClient.SendAddFriend(friendName);
            // Set Canvas Active
            addFriendCanvas.SetActive(false);
        }
    }

    /// <summary>
    /// Callback function of OtherAddFriendCanvas Button
    /// </summary>
    public void ClickOtherAddFriendCanvas()
    {
        // Set Other Add Friend Canvas Active
        otherAddFriendCanvas.SetActive(true);
    }

    /// <summary>
    /// Callback function of Cancel OtherAddFriendCanvas Button
    /// </summary>
    public void ClickCancelOtherAddFriendCanvas()
    {
        // Set Other Add Friend Canvas Active
        otherAddFriendCanvas.SetActive(false);
    }

    /// <summary>
    /// Add OtherAddFriendContent
    /// </summary>
    public void AddOtherAddFriendContent(string name)
    {
        // Instantiate OtherAddFriendContent object
        GameObject otherAddFriendContent = Instantiate(otherAddFriendContentPrefab, otherAddFriendContents.transform);
        // Set Name
        otherAddFriendContent.transform.Find("OtherAddFriendName").GetComponent<Text>().text = name;
        otherAddFriendContent.name = "OtherFriend: " + name;
        // Bind Click
        Transform acceptButton = otherAddFriendContent.transform.Find("AcceptFriendButton");
        acceptButton.GetComponent<Button>().onClick.AddListener(ClickAcceptOtherAddFriend);
        Transform cancelButton = otherAddFriendContent.transform.Find("RefuseFriendButton");
        cancelButton.GetComponent<Button>().onClick.AddListener(ClickCancelOtherAddFriend);
    }

    /// <summary>
    /// Callback function of Accept OtherAddFriend Button
    /// </summary>
    public void ClickAcceptOtherAddFriend()
    {
        // Get Clicked Button
        GameObject clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        // Get Friend Name
        string friendName = clickedButton.transform.parent.Find("OtherAddFriendName").GetComponent<Text>().text;
        // Send
        GlobalController.Instance.mainClient.SendReplyFriend(friendName, MainClient.REPLY_FRIEND_OK);
    }

    /// <summary>
    /// Callback function of Cancel OtherAddFriend Button
    /// </summary>
    public void ClickCancelOtherAddFriend()
    {
        // Get Clicked Button
        GameObject clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        // Get Friend Name
        string friendName = clickedButton.transform.parent.Find("OtherAddFriendName").GetComponent<Text>().text;
        // Send
        GlobalController.Instance.mainClient.SendReplyFriend(friendName, MainClient.REPLY_FRIEND_FAIL);
    }

    public void DeleteOtherAddFriend(string name)
    {
        GameObject otherAddFriendContent = GameObject.Find("OtherFriend: " + name);
        if(otherAddFriendContent != null)
        {
            Destroy(otherAddFriendContent);
        }
    }

    /// <summary>
    /// Show FailAddOtherFriendCanvas
    /// </summary>
    public void ShowFailAddOtherFriendCanvas()
    {
        failAddOtherFriendCanvas.SetActive(true);
    }

    /// <summary>
    /// Callback function of cancel FailAddOtherFriendCanvas
    /// </summary>
    public void ClickCancelFailAddOtherFriendCanvas()
    {
        failAddOtherFriendCanvas.SetActive(false);
    }

    /// <summary>
    /// Callback function of CreateRoomButton
    /// </summary>
    public void ClickCreateRoomButton()
    {
        createRoomCanvas.SetActive(true);
    }

    /// <summary>
    /// Callback function of Cancel CreateRoomButton
    /// </summary>
    public void ClickCancelCreateRoomButton()
    {
        createRoomCanvas.SetActive(false);
    }

    /// <summary>
    /// Callback function of ConfirmCreateRoomButton
    /// </summary>
    public void ClickConfirmCreateRoomButton()
    {
        // Get Room Name
        string roomName = createRoomCanvas.transform.Find("CreateRoomInputField").GetComponent<InputField>().text;
        if (roomName.Length != 0)
        {
            GlobalController.Instance.mainClient.SendCreateRoom(roomName);
            // Set Canvas Active
            createRoomCanvas.SetActive(false);
        }
    }

    /// <summary>
    /// Enter Room
    /// </summary>
    public void EnerRoom(string roomNo)
    {
        GlobalController.Instance.mainClient.SendEnterRoom(roomNo);
    }

    /// <summary>
    /// Callback function of enter room when receive entering command from server
    /// </summary>
    public void EnerRoomState(int state)
    {
        if(state == MainClient.ENTER_ROOM_OK)
        {
            SceneManager.LoadScene("Room");
        }
        else
        {
            failEnterRoomCanvas.SetActive(true);
        }
    }

    /// <summary>
    /// Callback function of create room success
    /// </summary>
    public void CreateRoomSuccess(string roomNo)
    {
        GlobalController.Instance.mainClient.SendEnterRoom(roomNo);
    }

    public void ClickCancelEnterRoomFailCanvas()
    {
        failEnterRoomCanvas.SetActive(false);
    }

    public void ClickExitLogin()
    {
        GlobalController.Instance.mainClient.SendExitLogin();
    }

    public void ExitLogin()
    {
        SceneManager.LoadScene("Login");
    }


}

public class RoomInfo
{
    public string roomName;
    public int prePlayerNumber;
    public string roomNo;
    public int roomIndex;
    public int state;
    public bool empty;
}

public class FriendInfo
{
    public string friendName;
    public int state;
}


