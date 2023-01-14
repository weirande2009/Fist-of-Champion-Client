using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;


public class ArmorInfoController
{
    // Instance
    public static ArmorInfoController Instance;

    // Config Info
    private string configPath;

    // Low Armor Info
    private ArmorInfo lowArmorInfo;
    private ArmorInfo middleArmorInfo;
    private ArmorInfo highArmorInfo;

    public List<ArmorInfo> armorInfo;


    public ArmorInfoController()
    {
        // Set Instance
        Instance = this;
        // Set Config
        configPath = "Documents/Config/Armor/armor";
        // Init
        armorInfo = new List<ArmorInfo>();
    }

    public void Initialize()
    {
        // Set Defense
        ReadConfig();
        armorInfo.Add(lowArmorInfo);
        armorInfo.Add(middleArmorInfo);
        armorInfo.Add(highArmorInfo);
    }

    private void ReadConfig()
    {
        // Use XmlDocument to read config file
        XmlDocument xmlDoc = new XmlDocument();
        TextAsset textAsset = Resources.Load<TextAsset>(configPath);
        xmlDoc.LoadXml(textAsset.text);
        // Get root node
        XmlNode xmlRoot = xmlDoc.DocumentElement;
        foreach (XmlNode node in xmlRoot.SelectNodes("Armor"))
        {
            if(node.SelectSingleNode("type").InnerText == "low")
            {
                string defenseString = node.SelectSingleNode("attribute/defense").InnerText;
                Sprite sprite = Resources.Load<Sprite>("Sprites/Armor/shield_1");
                lowArmorInfo = new ArmorInfo(float.Parse(defenseString), sprite, 0, "低级防具", "无");
            }
            else if (node.SelectSingleNode("type").InnerText == "middle")
            {
                string defenseString = node.SelectSingleNode("attribute/defense").InnerText;
                Sprite sprite = Resources.Load<Sprite>("Sprites/Armor/shield_2");
                middleArmorInfo = new ArmorInfo(float.Parse(defenseString), sprite, 1, "中级防具", "无");
            }
            else if (node.SelectSingleNode("type").InnerText == "high")
            {
                string defenseString = node.SelectSingleNode("attribute/defense").InnerText;
                Sprite sprite = Resources.Load<Sprite>("Sprites/Armor/shield_3");
                highArmorInfo = new ArmorInfo(float.Parse(defenseString), sprite, 2, "高级防具", "无");
            }
        }
    }
}

public class ArmorInfo
{
    public float defense;
    public Sprite sprite;
    public int type;
    public string name;
    public string effect;

    public ArmorInfo(float _defense, Sprite _sprite, int _type, string _name, string _effect)
    {
        defense = _defense;
        sprite = _sprite;
        type = _type;
        name = _name;
        effect = _effect;
    }
}

public class Armor
{
    // Attribute
    public float defense;
    // Type
    public int armorNo;


    public Armor()
    {
        
    }
}

public class LowArmor: Armor
{
    public LowArmor()
    {
        defense = ArmorInfoController.Instance.armorInfo[0].defense;
    }
}

public class MiddleArmor : Armor
{
    public MiddleArmor()
    {
        defense = ArmorInfoController.Instance.armorInfo[1].defense;
    }
}

public class HighArmor : Armor
{
    public HighArmor()
    {
        defense = ArmorInfoController.Instance.armorInfo[2].defense;
    }
}





