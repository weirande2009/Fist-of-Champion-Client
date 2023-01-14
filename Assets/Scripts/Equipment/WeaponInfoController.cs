using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class WeaponInfoController
{
    // Instance
    public static WeaponInfoController Instance;

    // Enum
    public enum WeaponType
    {
        Low = 0,
        Middle = 1,
        High = 2,
    }

    // Config Info
    private string lowWeaponConfigPath;
    private string middleWeaponConfigPath;
    private string highWeaponConfigPath;

    // Low Weapon
    public int lowWeaponAmount;

    // Middle Weapon
    public int middleWeaponAmount;

    // High Weapon
    public int highWeaponAmount;

    public Dictionary<WeaponType, List<WeaponInfo>> WeaponInfoDict;

    public WeaponInfoController()
    {
        // Instance
        Instance = this;
        // Set Config
        lowWeaponConfigPath = "Documents/Config/Weapons/LowWeapon";
        middleWeaponConfigPath = "Documents/Config/Weapons/MiddleWeapon";
        highWeaponConfigPath = "Documents/Config/Weapons/HighWeapon";
    }

    public void Initialize()
    {
        // Init
        lowWeaponAmount = 3;
        middleWeaponAmount = 5;
        highWeaponAmount = 5;
        WeaponInfoDict = new Dictionary<WeaponType, List<WeaponInfo>>();
        // Read Config
        ReadWeaponConfig(lowWeaponConfigPath, WeaponType.Low);
        ReadWeaponConfig(middleWeaponConfigPath, WeaponType.Middle);
        ReadWeaponConfig(highWeaponConfigPath, WeaponType.High);
        SetSprites();
        SetCombineRoute();
    }

    private void ReadWeaponConfig(string configPath, WeaponType type)
    {
        // Get root node
        XmlNode xmlRoot = UtilityTool.GetXmlRoot(configPath);
        string weaponName;
        string weaponEffect;
        string attackString;
        string attackSpeedString;
        string moveSpeedString;
        string weaponNoString;
        float weaponAttack;
        float weaponAttackSpeed;
        float weaponMoveSpeed;
        int weaponNo;

        List<WeaponInfo> weaponInfoList = new List<WeaponInfo>();
        int i = 0;
        foreach (XmlNode node in xmlRoot.SelectNodes("Weapon"))
        {
            weaponName = node.SelectSingleNode("name").InnerText;
            weaponEffect = node.SelectSingleNode("effect").InnerText;
            attackString = node.SelectSingleNode("attribute/attack").InnerText;
            attackSpeedString = node.SelectSingleNode("attribute/attackSpeed").InnerText;
            moveSpeedString = node.SelectSingleNode("attribute/moveSpeed").InnerText;
            weaponNoString = node.Attributes["no"].InnerText;
            weaponAttack = float.Parse(attackString);
            weaponAttackSpeed = float.Parse(attackSpeedString);
            weaponMoveSpeed = float.Parse(moveSpeedString);
            weaponNo = int.Parse(weaponNoString);
            WeaponInfo weaponInfo = new WeaponInfo(weaponName, weaponEffect, weaponAttack, weaponAttackSpeed, weaponMoveSpeed, weaponNo);
            weaponInfo.SetBasicInfo(type, i);
            
            // Read combine route
            if(type == WeaponType.Middle || type == WeaponType.High)
            {
                string combineWeaponTypeString;
                string combineWeaponNoString;
                WeaponType combineWeaponType = WeaponType.Low;
                int combineWeaponNo;
                foreach (XmlNode subnode in node.SelectNodes("combine/item"))
                {
                    combineWeaponTypeString = subnode.SelectSingleNode("type").InnerText;
                    combineWeaponNoString = subnode.SelectSingleNode("no").InnerText;
                    if(combineWeaponTypeString == "low")
                    {
                        combineWeaponType = WeaponType.Low;
                    }
                    else if(combineWeaponTypeString == "middle")
                    {
                        combineWeaponType = WeaponType.Middle;
                    }
                    else if (combineWeaponTypeString == "high")
                    {
                        combineWeaponType = WeaponType.High;
                    }
                    combineWeaponNo = int.Parse(combineWeaponNoString) - 1;
                    weaponInfo.combineWeapons.Add(new WeaponBasicInfo(combineWeaponType, combineWeaponNo));
                }
            }
            weaponInfoList.Add(weaponInfo);
            i++;
        }
        WeaponInfoDict[type] = weaponInfoList;
    }

    private void SetSprites()
    {
        for (int i = 0; i < lowWeaponAmount; i++)
        {
            Sprite sprite = Resources.Load<Sprite>("Sprites/Weapon/LowWeapon/LowWeapon" + (i + 1).ToString());
            WeaponInfoDict[WeaponType.Low][i].SetSprite(sprite);
        }
        for (int i = 0; i < middleWeaponAmount; i++)
        {
            Sprite sprite = Resources.Load<Sprite>("Sprites/Weapon/MiddleWeapon/MiddleWeapon" + (i + 1).ToString());
            WeaponInfoDict[WeaponType.Middle][i].SetSprite(sprite);
        }
        for (int i = 0; i < highWeaponAmount; i++)
        {
            Sprite sprite = Resources.Load<Sprite>("Sprites/Weapon/HighWeapon/HighWeapon" + (i + 1).ToString());
            WeaponInfoDict[WeaponType.High][i].SetSprite(sprite);
        }
    }

    private void SetCombineRoute()
    {
        for (int i = 0; i < lowWeaponAmount; i++)
        {
            Transform route = GameSceneController.Instance.bagCanvas.transform.Find("CombineRoute/LowWeapon" + (i + 1).ToString() + "Route");
            //GameObject route = GameObject.Find("LowWeapon" + (i + 1).ToString() + "Route");
            route.gameObject.SetActive(false);
            WeaponInfoDict[WeaponType.Low][i].SetCombineRoute(route.gameObject);
        }
        for (int i = 0; i < middleWeaponAmount; i++)
        {
            Transform route = GameSceneController.Instance.bagCanvas.transform.Find("CombineRoute/MiddleWeapon" + (i + 1).ToString() + "Route");
            //GameObject route = GameObject.Find("MiddleWeapon" + (i + 1).ToString() + "Route");
            route.gameObject.SetActive(false);
            WeaponInfoDict[WeaponType.Middle][i].SetCombineRoute(route.gameObject);
        }
        for (int i = 0; i < highWeaponAmount; i++)
        {
            Transform route = GameSceneController.Instance.bagCanvas.transform.Find("CombineRoute/HighWeapon" + (i + 1).ToString() + "Route");
            //GameObject route = GameObject.Find("HighWeapon" + (i + 1).ToString() + "Route");
            route.gameObject.SetActive(false);
            WeaponInfoDict[WeaponType.High][i].SetCombineRoute(route.gameObject);
        }
    }

}


public class WeaponInfo
{
    public string name;
    public string effectDescription;
    public float attack;
    public float attackSpeed;
    public float moveSpeed;
    public Sprite sprite;
    public int no;
    public GameObject combineRoute;
    public WeaponBasicInfo weaponBasicInfo;
    public List<WeaponBasicInfo> combineWeapons;

    public WeaponInfo(string _name, string _effectDescription, float _attack, float _attackSpeed, float _moveSpeed, int _no)
    {
        name = _name;
        effectDescription = _effectDescription;
        attack = _attack;
        attackSpeed = _attackSpeed;
        moveSpeed = _moveSpeed;
        no = _no;
        combineWeapons = new List<WeaponBasicInfo>();
        weaponBasicInfo = new WeaponBasicInfo();
    }

    public void SetSprite(Sprite _sprite)
    {
        sprite = _sprite;
    }

    public void SetCombineRoute(GameObject route)
    {
        combineRoute = route;
    }

    public void SetBasicInfo(WeaponInfoController.WeaponType weaponType, int weaponNo)
    {
        weaponBasicInfo.weaponType = weaponType;
        weaponBasicInfo.weaponNo = weaponNo;
    }
}

public class WeaponBasicInfo
{
    public WeaponInfoController.WeaponType weaponType;
    public int weaponNo;

    public WeaponBasicInfo()
    {

    }

    public WeaponBasicInfo(WeaponInfoController.WeaponType _weaponType, int _weaponNo)
    {
        weaponType = _weaponType;
        weaponNo = _weaponNo;
    }
}








