using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class WoodLow : NeutralObject
{
    private static string configPath = "Fist/Neutral/Wood/WoodLow/Documents/config";
    private static float initLife;                              // Initial life
    private static float initAttack;                            // Initial attack
    private static float initMoveSpeed;                         // Initial move speed
    private static float initAttackSpeed;                       // Initial attack speed
    private static float initAttackRange;                       // Initial attack range
    private static float initDefense;                           // Initial defense
    private static List<WeaponBasicInfo> initDropWeaponList;    // Initial drop weapon list
    private static List<int> initDropArmorList;                 // Initial drop armor list
    private static float initDropProbalility;                   // Initial drop probability
    private static List<NeutralSkillInfo> initSkillInfoList;    // Initial skill info list
    private static float initAttackMoveSpeed;                   // Initial attack move speed
    private static float initWatchRadius;                       // Initial watch radius


    // Attack
    public Transform attackPrefab;
    private bool inAttack;
    private float attackDurationTimer;
    private float attackDurationTime;
    private Vector2 attackTargetPosition;
    private Vector2 attackTargetDirection;

    /************************* Initialize *************************/

    public static void WoodLowIntialize()
    {
        // InitializeConfig
        InitializeConfig();
    }

    public static void InitializeConfig()
    {
        XmlNode xmlRoot = UtilityTool.GetXmlRoot(configPath);
        // Set Basic Attribute
        XmlNode basicAttributeNode = xmlRoot.SelectSingleNode("BasicAttribute");
        initLife = float.Parse(basicAttributeNode.SelectSingleNode("Life").InnerText);
        initAttack = float.Parse(basicAttributeNode.SelectSingleNode("Attack").InnerText);
        initMoveSpeed = float.Parse(basicAttributeNode.SelectSingleNode("MoveSpeed").InnerText);
        initAttackSpeed = float.Parse(basicAttributeNode.SelectSingleNode("AttackSpeed").InnerText);
        initAttackRange = float.Parse(basicAttributeNode.SelectSingleNode("AttackRange").InnerText);
        initDefense = float.Parse(basicAttributeNode.SelectSingleNode("Defense").InnerText);
        // Set Other Attribute
        XmlNode otherAttributeNode = xmlRoot.SelectSingleNode("OtherAttribute");
        initAttackMoveSpeed = float.Parse(otherAttributeNode.SelectSingleNode("AttackMoveSpeed").InnerText);
        // Set Drop
        XmlNode dropNode = xmlRoot.SelectSingleNode("Drop");
        initDropProbalility = float.Parse(dropNode.SelectSingleNode("Probability").InnerText);
        XmlNode dropItemNode = dropNode.SelectSingleNode("Item");
        initDropWeaponList = new List<WeaponBasicInfo>();
        foreach (XmlNode node in dropItemNode.SelectNodes("Weapon"))
        {
            WeaponInfoController.WeaponType weaponType = WeaponInfoController.WeaponType.Low;
            string weaponTypeString = node.SelectSingleNode("Type").InnerText;
            if (weaponTypeString == "Low")
            {
                weaponType = WeaponInfoController.WeaponType.Low;
            }
            else if (weaponTypeString == "Middle")
            {
                weaponType = WeaponInfoController.WeaponType.Middle;
            }
            else if (weaponTypeString == "High")
            {
                weaponType = WeaponInfoController.WeaponType.High;
            }
            initDropWeaponList.Add(new WeaponBasicInfo(weaponType, int.Parse(node.SelectSingleNode("No").InnerText)));
        }
        initDropArmorList = new List<int>();
        foreach (XmlNode node in dropItemNode.SelectNodes("Armor"))
        {
            initDropArmorList.Add(int.Parse(node.SelectSingleNode("No").InnerText));
        }
        // Set Intellegence
        XmlNode intellegenceNode = xmlRoot.SelectSingleNode("Intellegence");
        initWatchRadius = float.Parse(intellegenceNode.SelectSingleNode("WatchRadius").InnerText);
    }

    private void Awake()
    {
        // Set Basic Info
    }

    private void Start()
    {
        // Set FistObject
        InitFistObject(initLife, initAttack, initMoveSpeed, initAttackSpeed, initAttackRange, initDefense);

        // Set NeutralObject
        InitNeutralObject(NeutralType.Low, initWatchRadius, initSkillInfoList, initDropWeaponList, initDropArmorList, initDropProbalility);

    }

    /************************* Update *************************/

    private void AttackUpdate()
    {

    }

    /************************* Attack *************************/
    protected override void Attack()
    {
        Transform prefab = GlobalController.Instance.gameController.fistManager.Instantiate(attackPrefab, transform.position, Quaternion.identity);
        prefab.GetComponent<SoilLowAttackController>().Initialize(transform.position, nearestChampion.transform.position, realAttribute.attackRange, initAttackMoveSpeed, this);
    }

}
