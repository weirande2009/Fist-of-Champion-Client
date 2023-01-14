using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomSceneController : MonoBehaviour
{
    /* Instance */
    public static RoomSceneController Instance;         // Instance
    /* Room Info */
    public GameObject roomNameText;                     // Room Name Text
    public GameObject roomNoText;                       // Room No Text
    /* User Info */
    private List<GameObject> UserInfoCanvasList;        // Friend Info Canvas
    /* Ready State */
    private int readyState;                             // Ready State
    /* Character Sprite */
    private List<Sprite> characterSpriteList;           // Character Sprites
    /* Owner */
    public bool owner;                                  // Owner
    /* Choose Character Canvas */
    public GameObject chooseCharacterCanvas;            // Choose Character Canvas
    public int chosenCharacter;                         // Chosen Character


    // Start is called before the first frame update
    void Start()
    {
        /* Set Instance */
        Instance = this;
        /* Load Character Sprite */
        characterSpriteList = new List<Sprite>();
        for (int i = 0; i < 8; i++)
        {
            string imagePath = Application.dataPath + "/Resources/CharactersInRoom/Character" + (i + 1).ToString() + ".png";
            Texture2D texture = Utility.GetTextureByString(Utility.SetImageToString(imagePath));

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            /*sprite = Resources.Load<Sprite>("CharactersInRoom / Character" + (i + 1).ToString());*/
            characterSpriteList.Add(sprite);
        }
        /* User Info */
        UserInfoCanvasList = new List<GameObject>();
        for (int i = 0; i < MainClient.MAX_PLAYER_NUM_IN_ROOM; i++) 
        {
            UserInfoCanvasList.Add(GameObject.Find("SeatCanvas" + (i + 1).ToString()));
            UserInfoCanvasList[i].transform.Find("SeatInfoCanvas").gameObject.SetActive(false);
            UserInfoCanvasList[i].transform.Find("SeatInfoCanvas/Ready").gameObject.SetActive(false);
            UserInfoCanvasList[i].transform.Find("SeatInfoCanvas/RoomOwner").gameObject.SetActive(false);
            UserInfoCanvasList[i].transform.Find("SeatInfoCanvas/Character").GetComponent<Image>().sprite = characterSpriteList[i];
        }
        /* Set owner */
        owner = false;
        /* Set Ready State */
        readyState = MainClient.PLAYER_NO_READY;
        /* Set Room Info */
        GameObject.Find("RoomNameText").GetComponent<Text>().text = UIController.Instance.roomName;
        GameObject.Find("RoomNoText").GetComponent<Text>().text = UIController.Instance.roomNo;
        /* Set Choose Charater Canvas */
        chosenCharacter = 0;
        chooseCharacterCanvas.SetActive(false);
        for(int i=1; i<8; i++)
        {
            Transform character = chooseCharacterCanvas.transform.Find("CharactersCanvas/Character" + (i + 1).ToString()+"/Choose");
            // Set Canvas Inactive
            character.gameObject.SetActive(false);
        }
        /* Send RoomInfo */
        GlobalController.Instance.mainClient.SendRoomInfo(UIController.Instance.roomNo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSeatInfo(SeatInfo seatInfo)
    {
        if (seatInfo.empty)
        {
            // Set Canvas Inactive
            UserInfoCanvasList[seatInfo.seatNo].transform.Find("SeatInfoCanvas").gameObject.SetActive(false);
        }
        else
        {
            // Set Canvas Active
            UserInfoCanvasList[seatInfo.seatNo].transform.Find("SeatInfoCanvas").gameObject.SetActive(true);
            // Set Name
            UserInfoCanvasList[seatInfo.seatNo].transform.Find("SeatInfoCanvas/PlayerNameText").GetComponent<Text>().text = seatInfo.playerName;
            // Set Ready
            UserInfoCanvasList[seatInfo.seatNo].transform.Find("SeatInfoCanvas/Ready").gameObject.SetActive(seatInfo.ready);
            // Set Owner
            UserInfoCanvasList[seatInfo.seatNo].transform.Find("SeatInfoCanvas/RoomOwner").gameObject.SetActive(seatInfo.owner);
            if(seatInfo.owner && seatInfo.seatNo == UIController.Instance.seatNo)
            {
                GameObject.Find("ReadyButtonText").GetComponent<Text>().text = "开始游戏";
            }
            // Set Character
            UserInfoCanvasList[seatInfo.seatNo].transform.Find("SeatInfoCanvas/Character").GetComponent<Image>().sprite = characterSpriteList[seatInfo.character];
        }
    }


    /// <summary>
    /// Callback function of click ready button
    /// </summary>
    public void ClickReadyButton()
    {
        if (owner)  // if owner, send start game
        {
            GlobalController.Instance.mainClient.SendStartGame();
        }
        else
        {
            if(readyState == MainClient.PLAYER_NO_READY)
            {
                GlobalController.Instance.mainClient.SendReady();
            }
            else
            {
                GlobalController.Instance.mainClient.SendCancelReady();
            }
        }
    }

    public void SetReadyButton(int state)
    {
        if(state == MainClient.PLAYER_READY)
        {
            readyState = MainClient.PLAYER_READY;
            GameObject.Find("ReadyButtonText").GetComponent<Text>().text = "取消准备";
        }
        else
        {
            readyState = MainClient.PLAYER_NO_READY;
            GameObject.Find("ReadyButtonText").GetComponent<Text>().text = "准备";
        }
    }

    public void SetOwner()
    {
        owner = true;
    }

    public void ClickExitRoom()
    {
        GlobalController.Instance.mainClient.SendExitRoom();
    }

    public void ExitRoomState(int state)
    {
        if(state == MainClient.EXIT_ROOM_OK)
        {
            SceneManager.LoadScene("Hall");
        }
    }

    public void ClickChooseCharacterCanvas()
    {
        chooseCharacterCanvas.SetActive(true);
    }

    public void ClickCancelChooseCharacterCanvas()
    {
        chooseCharacterCanvas.SetActive(false);
    }

    public void ClickChooseCharacter()
    {
        // Get character
        GameObject clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        // Set Other Not Chosen
        for (int i = 0; i < 8; i++)
        {
            string buttonName = "CharactersCanvas/Character" + (i + 1).ToString() + "/Choose";
            Transform character = chooseCharacterCanvas.transform.Find(buttonName);
            if ("Character" + (i + 1).ToString() != clickedButton.name)
            {
                // Set Canvas Inactive
                character.gameObject.SetActive(false);
            }
            else
            {
                character.gameObject.SetActive(true);
                chosenCharacter = i;
            }
        }
    }

    public void ClickConfirmCharacter()
    {
        GlobalController.Instance.mainClient.SendModifyChar(chosenCharacter);
        chooseCharacterCanvas.SetActive(false);
    }

}


public class SeatInfo
{
    public bool empty;
    public int seatNo;
    public string playerName;
    public int character;
    public bool ready;
    public bool owner;
}


