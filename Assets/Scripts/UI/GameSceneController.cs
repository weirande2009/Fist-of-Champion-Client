using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSceneController : MonoBehaviour
{
    // Instance
    public static GameSceneController Instance;

    // Attribute canvas
    private float attack;               // attack
    private float defense;              // defense
    private float life;                 // life
    private float attackSpeed;          // attack speed
    private float attackRange;          // attack range
    private float moveSpeed;            // move speed

    // Skill
    private float skillQCold;           // skill q cold
    private float skillWCold;           // skill w cold
    private float skillECold;           // skill e cold
    private float skillRCold;           // skill r cold

    // UI
    public Text attackText;             // attack text
    public Text defenseText;            // defense text
    public Text lifeText;               // life text
    public Text attackSpeedText;        // attack speed text
    public Text attackRangeText;        // attack range text
    public Text moveSpeedText;          // move speed text
    public Image lifeImage;             // life image
    public Button skillQButton;         // skill q button
    public Button skillWButton;         // skill w button
    public Button skillEButton;         // skill e button
    public Button skillRButton;         // skill r button
    private Image skillQImage;          // skill q image
    private Image skillWImage;          // skill w image
    private Image skillEImage;          // skill e image
    private Image skillRImage;          // skill r image

    // Bag
    public GameObject bagCanvas;                        // bag canvas
    // Bag Attribute Canvas
    public GameObject bagAttributeCanvas;               // bag canvas
    public Text bagAttributeAttackText;                 // bag attribute attack text
    public Text bagAttributeDefenseText;                // bag attribute defense text
    public Text bagAttributeAttackSpeedText;            // bag attribute attack speed text
    public Text bagAttributeMoveSpeedText;              // bag attribute move speed text
    public Text bagAttributeName;                       // bag attribute name text
    public Text bagAttributeEffectText;                 // bag attribute effect text
    public Image bagAttributeSprite;                    // bag attribute sprite
    // Bag Combine Route Background
    public GameObject lowCombineRoute;                  // low combine route
    public GameObject middleCombineRoute;               // middle combine route
    public GameObject highCombineRoute;                 // high combine route
    private Dictionary<WeaponInfoController.WeaponType, GameObject> combineRoutes;      // combine routes
    // Bag My Item
    public List<GameObject> myItems;                    // my item list
    // Bag Weared Equipment
    public GameObject wearedWeaponItem;
    public GameObject wearedArmorItem;
    // UI Sprite
    public Sprite UISprite;
    // Operation
    public ItemInBag clickedItemInBag;                  // Record clicked item
    // Delay Text
    public Text delayText;                              // delay text
    // Game Info Text
    public GameInfoController gameInfoController;       // game info controller
    public Text gameInfoText;                           // game info text
    // Game Time 
    public Text gameTimeText;                           // game time text
    // Game FPS
    public Text gameFPSText;                            // game fps text
    // Dead Canvas
    public GameObject deadCanvas;                       // dead canvas
    public Text rankText;                               // rank text
    // Game Over Canvas
    public GameObject gameOverCanvas;                   // game over canvas
    public Text championNameText;                       // champion name text
    // Enter Info Canvas
    public GameObject enterInfoCanvas;                  // Enter Info Canvas
    public GameObject chooseCharacterCanvas;            // Choose Character Canvas
    public int chosenCharacter;                         // Chosen Character
    private List<Sprite> characterSpriteList;           // Character Sprites
    public Image chosenCharacterImage;                  // Chosen Character Image
    public Text playerNameText;                         // Player Name Text
    // Damaged Effect
    public GameObject damagedEffectImage;               // Damaged Effect Image
    private bool inDamagedEffect;                       // Whether in Damaged Effect
    private float damagedEffectTime;                    // Damaged Effect Time
    private float damagedEffectTimer;                   // Damaged Effect Timer
    // MiniMap
    public Camera minimapCamera;  // 3*3 -> 150  4*4 -> 120  5*5 -> 90  6*6 -> 50

    private void Awake()
    {
        // Instance
        Instance = this;
        // Game info controller
        gameInfoController = new GameInfoController(gameInfoText);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set Attribute
        attack = 0;
        defense = 0;
        life = 0;
        attackSpeed = 0;
        attackRange = 0;
        moveSpeed = 0;
        // Get Image
        skillQImage = skillQButton.GetComponent<Image>();
        skillWImage = skillWButton.GetComponent<Image>();
        skillEImage = skillEButton.GetComponent<Image>();
        skillRImage = skillRButton.GetComponent<Image>();
        // Bag
        bagCanvas.SetActive(false);
        lowCombineRoute.SetActive(false);
        middleCombineRoute.SetActive(false);
        highCombineRoute.SetActive(false);
        deadCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);
        combineRoutes = new Dictionary<WeaponInfoController.WeaponType, GameObject>();
        combineRoutes[WeaponInfoController.WeaponType.Low] = lowCombineRoute;
        combineRoutes[WeaponInfoController.WeaponType.Middle] = middleCombineRoute;
        combineRoutes[WeaponInfoController.WeaponType.High] = highCombineRoute;
        myItems = new List<GameObject>();
        for (int i = 0; i < GameController.MAX_ITEM_NUMBER_IN_BAG; i++) 
        {
            Transform item = bagCanvas.transform.Find("MyItemCanvas/ItemButton" + (i + 1).ToString());
            myItems.Add(item.gameObject);
        }
        wearedWeaponItem = bagCanvas.transform.Find("WearedEquipmentCanvas/WearedWeaponButton").gameObject;
        wearedArmorItem = bagCanvas.transform.Find("WearedEquipmentCanvas/WearedArmorButton").gameObject;
        UISprite = wearedArmorItem.GetComponent<Image>().sprite;
        /* Load Character Sprite */
        characterSpriteList = new List<Sprite>();
        for (int i = 0; i < 8; i++)
        {
            Sprite sprite = Resources.Load<Sprite>("CharactersInRoom/Character" + (i + 1).ToString());
            characterSpriteList.Add(sprite);
        }
        chosenCharacter = 0;
        chooseCharacterCanvas.SetActive(false);
        for (int i = 1; i < 8; i++)
        {
            Transform character = chooseCharacterCanvas.transform.Find("CharactersCanvas/Character" + (i + 1).ToString() + "/Choose");
            // Set Canvas Inactive
            character.gameObject.SetActive(false);
        }
        // Set Damaged Effect
        damagedEffectImage.SetActive(false);
        damagedEffectImage.GetComponent<CanvasGroup>().alpha = 0.5f;
        inDamagedEffect = false;
        damagedEffectTime = 0.25f;
        damagedEffectTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(GlobalController.Instance.gameController != null && GlobalController.Instance.gameController.operating)
        {
            // Update Attribute
            if (GlobalController.Instance.gameController.GetMainChampion().lifeNeedUpdate)
            {
                SetLife(GlobalController.Instance.gameController.GetMainChampion().GetRealAttribute().life, GlobalController.Instance.gameController.GetMainChampion().GetRealAttribute().maxLife);
                GlobalController.Instance.gameController.GetMainChampion().lifeNeedUpdate = false;
            }
            if (GlobalController.Instance.gameController.GetMainChampion().attributeNeedUpdate)
            {
                FistAttribute realAttribute = GlobalController.Instance.gameController.GetMainChampion().GetRealAttribute();
                SetAttackText(realAttribute.attack);
                SetDefenseText(realAttribute.defense);
                SetAttackSpeedText(realAttribute.attackSpeed);
                SetAttackRangeText(realAttribute.attackRange);
                SetMoveSpeedText(realAttribute.moveSpeed);
                GlobalController.Instance.gameController.GetMainChampion().attributeNeedUpdate = false;
            }
        }
        if (inDamagedEffect)
        {
            damagedEffectTimer += Time.deltaTime;
            damagedEffectImage.GetComponent<CanvasGroup>().alpha = (damagedEffectTime - damagedEffectTimer) / damagedEffectTime * 0.5f;
            if (damagedEffectTimer > damagedEffectTime)
            {
                inDamagedEffect = false;
                damagedEffectTimer = 0;
                damagedEffectImage.SetActive(false);
            }
        }
    }

    public void SetAttackText(float _attack)
    {
        attackText.text = "¹¥»÷Á¦£º" + Mathf.Round(_attack);
        attack = _attack;
    }

    public void SetDefenseText(float _defense)
    {
        defenseText.text = "·ÀÓùÁ¦£º" + Mathf.RoundToInt(_defense * 100) / 100.0f;
        defense = _defense;
    }

    public void SetAttackSpeedText(float _attackSpeed)
    {
        attackSpeedText.text = "¹¥»÷ËÙ¶È£º" + Mathf.RoundToInt(_attackSpeed * 100) / 100.0f;
        attackSpeed = _attackSpeed;
    }

    public void SetAttackRangeText(float _attackRange)
    {
        attackRangeText.text = "¹¥»÷·¶Î§£º" + Mathf.RoundToInt(_attackRange * 100) / 100.0f;
        attackRange = _attackRange;
    }

    public void SetMoveSpeedText(float _moveSpeed)
    {
        moveSpeedText.text = "ÒÆ¶¯ËÙ¶È£º" + Mathf.RoundToInt(_moveSpeed * 100) / 100.0f;
        moveSpeed = _moveSpeed;
    }

    public void SetLife(float _realLife, float _life)
    {
        // Set text
        lifeText.text = Mathf.RoundToInt(_realLife) +" / " + Mathf.RoundToInt(_life);
        life = _realLife;
        // Set Image
        lifeImage.fillAmount = _realLife / _life;
    }

    public void SetSkillQ(float cold, bool ready)
    {
        if (ready)
        {
            skillQImage.fillAmount = 1;
        }
        else
        {
            skillQImage.fillAmount = cold;
        }
    }

    public void SetSkillW(float cold, bool ready)
    {
        if (ready)
        {
            skillWImage.fillAmount = 1;
        }
        else
        {
            skillWImage.fillAmount = cold;
        }
    }

    public void SetSkillE(float cold, bool ready)
    {
        if (ready)
        {
            skillEImage.fillAmount = 1;
        }
        else
        {
            skillEImage.fillAmount = cold;
        }
    }

    public void SetSkillR(float cold, bool ready)
    {
        if (ready)
        {
            skillRImage.fillAmount = 1;
        }
        else
        {
            skillRImage.fillAmount = cold;
        }
    }

    public void SetSkill(float cold, bool ready, ChampionSkillInfo.SkillType skillType)
    {
        switch (skillType)
        {
            case ChampionSkillInfo.SkillType.W:
                SetSkillW(cold, ready);
                break;
            case ChampionSkillInfo.SkillType.E:
                SetSkillE(cold, ready);
                break;
            case ChampionSkillInfo.SkillType.R:
                SetSkillR(cold, ready);
                break;
        }
    }

    public void ClickBag()
    {
        bagCanvas.SetActive(true);
        // Set My Item
        LoadMyItemInBag();
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("BagItemButton");
        foreach (GameObject button in buttons)
        {
            button.GetComponent<ItemInBag>().chosenFrame.SetActive(false);
        }
        clickedItemInBag = null;
    }

    public void ClickCloseBag()
    {
        bagCanvas.SetActive(false);
    }

    private void InitBag()
    {
        // Init All Item
        for (int i = 0; i < WeaponInfoController.Instance.lowWeaponAmount; i++)
        {
            Transform button = bagCanvas.transform.Find("AllItemCanvas/LowButton" + (i + 1).ToString());
            button.GetComponent<Image>().sprite = WeaponInfoController.Instance.WeaponInfoDict[WeaponInfoController.WeaponType.Low][i].sprite;
            button.GetComponent<ItemInBag>().SetInfo(ItemInBag.ItemType.Weapon, WeaponInfoController.WeaponType.Low, i, -1, -1);
            button.GetComponent<ItemInBag>().state = ItemInBag.ItemState.InStore;
        }
        for (int i = 0; i < WeaponInfoController.Instance.middleWeaponAmount; i++)
        {
            Transform button = bagCanvas.transform.Find("AllItemCanvas/MiddleButton" + (i + 1).ToString());
            button.GetComponent<Image>().sprite = WeaponInfoController.Instance.WeaponInfoDict[WeaponInfoController.WeaponType.Middle][i].sprite;
            button.GetComponent<ItemInBag>().SetInfo(ItemInBag.ItemType.Weapon, WeaponInfoController.WeaponType.Middle, i, -1, -1);
            button.GetComponent<ItemInBag>().state = ItemInBag.ItemState.InStore;
        }
        for (int i = 0; i < WeaponInfoController.Instance.highWeaponAmount; i++)
        {
            Transform button = bagCanvas.transform.Find("AllItemCanvas/HighButton" + (i + 1).ToString());
            button.GetComponent<Image>().sprite = WeaponInfoController.Instance.WeaponInfoDict[WeaponInfoController.WeaponType.High][i].sprite;
            button.GetComponent<ItemInBag>().SetInfo(ItemInBag.ItemType.Weapon, WeaponInfoController.WeaponType.High, i, -1, -1);
            button.GetComponent<ItemInBag>().state = ItemInBag.ItemState.InStore;
        }
    }

    public void LoadMyItemInBag()
    {
        List<PlayerItem> playerItems = GlobalController.Instance.gameController.GetMainPlayer().items;
        for (int i = 0; i < playerItems.Count; i++)
        {
            myItems[i].GetComponent<ItemInBag>().SetInfo(playerItems[i].type, playerItems[i].weaponType, playerItems[i].weaponNo, playerItems[i].armorNo, i);
        }
        for (int i = playerItems.Count; i < myItems.Count; i++)
        {
            myItems[i].GetComponent<ItemInBag>().Clear();
        }
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("BagItemButton");
        foreach (GameObject button in buttons)
        {
            button.GetComponent<ItemInBag>().chosenFrame.SetActive(false);
        }
    }

    public void SetAllButtonUnchosen()
    {

    }

    public void SetWeaponAttributeInBag(WeaponInfoController.WeaponType weaponType, int weaponNo)
    {
        bagAttributeAttackText.text = WeaponInfoController.Instance.WeaponInfoDict[weaponType][weaponNo].attack.ToString();
        bagAttributeDefenseText.text = "0";
        bagAttributeAttackSpeedText.text = WeaponInfoController.Instance.WeaponInfoDict[weaponType][weaponNo].attackSpeed.ToString();
        bagAttributeMoveSpeedText.text = WeaponInfoController.Instance.WeaponInfoDict[weaponType][weaponNo].moveSpeed.ToString();
        bagAttributeSprite.sprite = WeaponInfoController.Instance.WeaponInfoDict[weaponType][weaponNo].sprite;
        bagAttributeName.text = WeaponInfoController.Instance.WeaponInfoDict[weaponType][weaponNo].name;
        bagAttributeEffectText.text = WeaponInfoController.Instance.WeaponInfoDict[weaponType][weaponNo].effectDescription;
    }

    public void SetArmorAttributeInBag(int armorNo)
    {
        bagAttributeAttackText.text = "0";
        bagAttributeDefenseText.text = ArmorInfoController.Instance.armorInfo[armorNo].defense.ToString();
        bagAttributeAttackSpeedText.text = "0";
        bagAttributeMoveSpeedText.text = "0";
        bagAttributeSprite.sprite = ArmorInfoController.Instance.armorInfo[armorNo].sprite;
        bagAttributeName.text = ArmorInfoController.Instance.armorInfo[armorNo].name;
        bagAttributeEffectText.text = ArmorInfoController.Instance.armorInfo[armorNo].effect;
    }

    public void ShowCombineRoute(WeaponInfoController.WeaponType weaponType, int weaponNo)
    {
        for (int i = 0; i < WeaponInfoController.Instance.lowWeaponAmount; i++)
        {
            WeaponInfoController.Instance.WeaponInfoDict[WeaponInfoController.WeaponType.Low][i].combineRoute.SetActive(false);
        }
        for (int i = 0; i < WeaponInfoController.Instance.middleWeaponAmount; i++)
        {
            WeaponInfoController.Instance.WeaponInfoDict[WeaponInfoController.WeaponType.Middle][i].combineRoute.SetActive(false);
        }
        for (int i = 0; i < WeaponInfoController.Instance.highWeaponAmount; i++)
        {
            WeaponInfoController.Instance.WeaponInfoDict[WeaponInfoController.WeaponType.High][i].combineRoute.SetActive(false);
        }
        WeaponInfoController.Instance.WeaponInfoDict[weaponType][weaponNo].combineRoute.SetActive(true);
        lowCombineRoute.SetActive(false);
        middleCombineRoute.SetActive(false);
        highCombineRoute.SetActive(false);
        combineRoutes[weaponType].SetActive(true);
    }



    public void ClickItemInBag()
    {
        // Get click button
        GameObject clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        ItemInBag item = clickedButton.GetComponent<ItemInBag>();
        // Set Bag Canvas
        if(item.type == ItemInBag.ItemType.Weapon)
        {
            SetWeaponAttributeInBag(item.weaponType, item.weaponNo);
        }
        else if(item.type == ItemInBag.ItemType.Armor)
        {
            SetArmorAttributeInBag(item.armorNo);
        }
        // Set Combine Route
        if(item.state == ItemInBag.ItemState.InStore)
        {
            ShowCombineRoute(item.weaponType, item.weaponNo);
        }
        // Record
        clickedItemInBag = item;
        // Set Chosen Frame
        if (item.chosenFrame.activeSelf)
        {
            item.chosenFrame.SetActive(false);
        }
        else
        {
            GameObject[] buttons = GameObject.FindGameObjectsWithTag("BagItemButton");
            foreach (GameObject button in buttons)
            {
                button.GetComponent<ItemInBag>().chosenFrame.SetActive(false);
            }
            item.chosenFrame.SetActive(true);
        }
    }

    public void ClickWear()
    {
        if(clickedItemInBag == null)
        {
            return;
        }
        if(clickedItemInBag.state == ItemInBag.ItemState.InBag)
        {
            // Get Main Player
            GamePlayer mainPlayer = GlobalController.Instance.gameController.GetMainPlayer();
            // Take off equipment
            mainPlayer.TakeOffEquipment(clickedItemInBag.type);
            // Delete item
            mainPlayer.items.RemoveAt(clickedItemInBag.index);
            // Set Button
            if(clickedItemInBag.type == ItemInBag.ItemType.Weapon)
            {
                wearedWeaponItem.GetComponent<ItemInBag>().SetInfo(clickedItemInBag.type, clickedItemInBag.weaponType, clickedItemInBag.weaponNo, clickedItemInBag.armorNo, -1);
                wearedWeaponItem.GetComponent<ItemInBag>().state = ItemInBag.ItemState.Armed;
                // Set operation
                GlobalController.Instance.gameController.SetWeaponInfoInOperation(clickedItemInBag.weaponType, clickedItemInBag.weaponNo);
            }
            else if(clickedItemInBag.type == ItemInBag.ItemType.Armor)
            {
                wearedArmorItem.GetComponent<ItemInBag>().SetInfo(clickedItemInBag.type, clickedItemInBag.weaponType, clickedItemInBag.weaponNo, clickedItemInBag.armorNo, -1);
                wearedArmorItem.GetComponent<ItemInBag>().state = ItemInBag.ItemState.Armed;
                // Set operation
                GlobalController.Instance.gameController.SetArmorInfoInOperation(clickedItemInBag.armorNo);
            }
            // Reload my items
            LoadMyItemInBag();
        }
    }

    public void ClickGetWeapon()
    {
        if (clickedItemInBag == null)
        {
            return;
        }
        if (clickedItemInBag.state == ItemInBag.ItemState.InStore)
        {
            if(clickedItemInBag.weaponType == WeaponInfoController.WeaponType.Low)
            {
                return;
            }
            // Get combine route weapon
            List<WeaponBasicInfo> combineRouteWeapon = WeaponInfoController.Instance.WeaponInfoDict[clickedItemInBag.weaponType][clickedItemInBag.weaponNo].combineWeapons;
            // Find needed weapon in bag
            List<int> ingredientIndex = new List<int>();
            List<PlayerItem> playerItems = GlobalController.Instance.gameController.GetMainPlayer().items;
            Weapon weapon = GlobalController.Instance.gameController.GetMainPlayer().champion.weapon;
            for (int i = 0; i < combineRouteWeapon.Count; i++)
            {
                // Look for in weared weapon
                if (weapon != null)
                {
                    if(weapon.type == combineRouteWeapon[i].weaponType && weapon.weaponNo == combineRouteWeapon[i].weaponNo)
                    {
                        if (!ingredientIndex.Contains(-1))
                        {
                            ingredientIndex.Add(-1);    // -1 means weared weapon
                            continue;
                        }
                    }
                }
                for(int j = 0; j < playerItems.Count; j++)
                {
                    if(playerItems[j].weaponType == combineRouteWeapon[i].weaponType && playerItems[j].weaponNo == combineRouteWeapon[i].weaponNo)
                    {
                        if (!ingredientIndex.Contains(j))
                        {
                            ingredientIndex.Add(j);
                            break;
                        }
                    }
                }
            }
            // If has all needed weapons, combine
            ingredientIndex.Sort();
            if (ingredientIndex.Count == combineRouteWeapon.Count)
            {
                // Delete all ingredient
                if (ingredientIndex.Contains(-1))   // if contain weared weapon
                {
                    GlobalController.Instance.gameController.GetMainPlayer().champion.ClearWeapon();
                    // Set Button
                    wearedWeaponItem.GetComponent<ItemInBag>().Clear();
                    wearedWeaponItem.GetComponent<ItemInBag>().state = ItemInBag.ItemState.Armed;
                    ingredientIndex.RemoveAt(0);
                }

                for (int i = 0; i < ingredientIndex.Count; i++)
                {
                    playerItems.RemoveAt(ingredientIndex[i] - i);
                }
                // Add combined weapon
                PlayerItem item = new PlayerItem(clickedItemInBag.type, clickedItemInBag.weaponType, clickedItemInBag.weaponNo, -1);
                playerItems.Add(item);
                // Reload my items in bag
                LoadMyItemInBag();
            }
        }
    }

    private void InitAttribute()
    {
        // Get Main Player Info
        FistAttribute attribute = GlobalController.Instance.gameController.GetMainChampion().GetAttribute();
        FistAttribute realAttribute = GlobalController.Instance.gameController.GetMainChampion().GetRealAttribute();
        SetAttackText(realAttribute.attack);
        SetDefenseText(realAttribute.defense);
        SetAttackSpeedText(realAttribute.attackSpeed);
        SetAttackRangeText(realAttribute.attackRange);
        SetMoveSpeedText(realAttribute.moveSpeed);
        SetLife(realAttribute.life, attribute.life);
    }

    public void Initialize()
    {
        InitAttribute();
        InitBag();
    }

    /// <summary>
    /// Set Delay Text On UI
    /// </summary>
    /// <param name="delay">seconds</param>
    public void SetDelayText(float delay)
    {
        int delayInt = (int)(delay * 1000);
        delayText.text = "Ping: " + delayInt.ToString() + "ms";
    }

    public void SetGameTimeText(float time)
    {
        string minute = ((int)(time / 60)).ToString();
        string second = ((int)(time % 60)).ToString();
        gameTimeText.text = minute + " : " + second;
    }

    public void SetGameFPSText(int fps)
    {
        gameFPSText.text = "FPS: " + fps.ToString();
    }

    public void ActivateDeadCanvas()
    {
        deadCanvas.SetActive(true);
    }

    public void SetRankText(int rank)
    {
        rankText.text = "µÚ " + rank.ToString() + "Ãû";
    }

    public void ActivateGameOverCanvas()
    {
        gameOverCanvas.SetActive(true);
    }

    public void SetChampionNameText(string name)
    {
        championNameText.text = name;
    }

    public void ClickExitGame()
    {
        GlobalController.Instance.gameClient.SendExit();
    }

    public void ClickResumeGame()
    {
        deadCanvas.SetActive(false);
        GlobalController.Instance.gameController.StartWatchingGame();
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
        chosenCharacterImage.sprite = characterSpriteList[chosenCharacter];
        chooseCharacterCanvas.SetActive(false);
    }

    public void ClickConnectGame()
    {
        enterInfoCanvas.SetActive(false);
        GlobalController.Instance.championNo = chosenCharacter;
        GlobalController.Instance.playerName = playerNameText.text;
        GlobalController.Instance.ConnectGame();
    }

    public void ShowDamagedEffect()
    {
        damagedEffectImage.SetActive(true);
        inDamagedEffect = true;
        damagedEffectTimer = 0;
    }

}

public class GameInfoController
{
    public Text gameInfoText;
    public RectTransform gameInfoRectTransform;


    public GameInfoController(Text text)
    {
        gameInfoText = text;
        gameInfoText.text = string.Empty;
        gameInfoRectTransform = gameInfoText.GetComponent<RectTransform>();
    }

    public void AddInfo(string info)
    {
        gameInfoText.text += info + "\n";
        Vector2 textSize = gameInfoRectTransform.rect.size;
        gameInfoRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textSize.x);
        gameInfoRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, gameInfoText.preferredHeight);
    }
}








