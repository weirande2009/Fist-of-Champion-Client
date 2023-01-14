using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Sorcerer : ChampionObject
{
    /* Initial Attribute */
    private static string configPath = "Fist/Champion/Sorcerer/Documents/config";
    private static float initLife;                          // Initial life
    private static float initAttack;                        // Initial attack
    private static float initMoveSpeed;                     // Initial move speed
    private static float initAttackSpeed;                   // Initial attack speed
    private static float initAttackRange;                   // Initial attack range
    private static float initDefense;                       // Initial defense
    // Skill Related
    private static float initWSkillCold;                    // Inital w skill cold
    private static float initWSkillDistance;                // Inital w skill distance
    private static float initWSkillSpeed;                   // Inital w skill speed
    private static float initWSkillDamage;                  // Inital w skill damage
    private static float initWSkillDamageRatio;             // Inital w skill damage ratio
    private static float initWSkillDecreaseSpeed;           // Inital w skill decrease speed
    private static float initWSkillDecreaseSpeedDuration;   // Inital w skill decrease speed duration

    private static float initESkillCold;                    // Inital e skill cold
    private static float initESkillDuration;                // Inital e skill duration
    private static float initESkillDamage;                  // Inital e skill damage
    private static float initESkillDamageRatio;             // Inital e skill damage ratio
    private static float initESkillDamageCoef;              // Inital e skill damage

    private static float initRSkillCold;                    // Inital r skill cold
    private static float initRSkillMaxDistance;             // Inital r skill max distance
    private static float initRSkillDamage;                  // Inital r skill damage
    private static float initRSkillDamageRatio;             // Inital r skill damage ratio
    private static float initRSkillDuration;                // Inital r skill duration

    // Robot Info
    private static Dictionary<ChampionSkillInfo.SkillType, int> skillChangeAttributeTag;    // skill change attribute tag

    public Transform attackPrefab;

    public Transform skillWPrefab;

    public Transform skillEPrefab;

    public Transform skillRPrefab;


    public static void SorcererInitialize()
    {
        // Init Tag
        skillChangeAttributeTag = new Dictionary<ChampionSkillInfo.SkillType, int>();
        foreach (ChampionSkillInfo.SkillType foo in System.Enum.GetValues(typeof(ChampionSkillInfo.SkillType)))
        {
            skillChangeAttributeTag[foo] = ChangeAttibuteRecord.GetNewTag(ChangeAttibuteRecord.SourceType.ChampionSkill);
        }
        // Init Config
        InitializeConfig();
    }

    public static void InitializeConfig()
    {
        XmlNode xmlRoot = UtilityTool.GetXmlRoot(configPath);
        XmlNode basicAttributeNode = xmlRoot.SelectSingleNode("BasicAttribute");
        XmlNode skillNode = xmlRoot.SelectSingleNode("Skill");
        // Set Basic Attribute
        initLife = float.Parse(basicAttributeNode.SelectSingleNode("Life").InnerText);
        initAttack = float.Parse(basicAttributeNode.SelectSingleNode("Attack").InnerText);
        initMoveSpeed = float.Parse(basicAttributeNode.SelectSingleNode("MoveSpeed").InnerText);
        initAttackSpeed = float.Parse(basicAttributeNode.SelectSingleNode("AttackSpeed").InnerText);
        initAttackRange = float.Parse(basicAttributeNode.SelectSingleNode("AttackRange").InnerText);
        initDefense = float.Parse(basicAttributeNode.SelectSingleNode("Defense").InnerText);
        // Set Skill Value
        XmlNode skillWNode = skillNode.SelectSingleNode("W");
        initWSkillCold = float.Parse(skillWNode.SelectSingleNode("Cold").InnerText);
        initWSkillDistance = float.Parse(skillWNode.SelectSingleNode("Distance").InnerText);
        initWSkillDamage = float.Parse(skillWNode.SelectSingleNode("Damage").InnerText);
        initWSkillDamageRatio = float.Parse(skillWNode.SelectSingleNode("DamageRatio").InnerText);
        initWSkillSpeed = float.Parse(skillWNode.SelectSingleNode("Speed").InnerText);
        initWSkillDecreaseSpeed = float.Parse(skillWNode.SelectSingleNode("DecreaseSpeed").InnerText);
        initWSkillDecreaseSpeedDuration = float.Parse(skillWNode.SelectSingleNode("DecreaseSpeedDuration").InnerText);
        XmlNode skillENode = skillNode.SelectSingleNode("E");
        initESkillCold = float.Parse(skillENode.SelectSingleNode("Cold").InnerText);
        initESkillDuration = float.Parse(skillENode.SelectSingleNode("Duration").InnerText);
        initESkillDamage = float.Parse(skillENode.SelectSingleNode("Damage").InnerText);
        initESkillDamageRatio = float.Parse(skillENode.SelectSingleNode("DamageRatio").InnerText);
        initESkillDamageCoef = float.Parse(skillENode.SelectSingleNode("DamageCoef").InnerText);
        XmlNode skillRNode = skillNode.SelectSingleNode("R");
        initRSkillCold = float.Parse(skillRNode.SelectSingleNode("Cold").InnerText);
        initRSkillDuration = float.Parse(skillRNode.SelectSingleNode("Duration").InnerText);
        initRSkillMaxDistance = float.Parse(skillRNode.SelectSingleNode("MaxDistance").InnerText);
        initRSkillDamage = float.Parse(skillRNode.SelectSingleNode("Damage").InnerText);
        initRSkillDamageRatio = float.Parse(skillRNode.SelectSingleNode("DamageRatio").InnerText);
    }

    void Awake()
    {
        // Set Static Info
        InitializeChangeAttributeTag(skillChangeAttributeTag);

        // Set FistObject
        InitFistObject(initLife, initAttack, initMoveSpeed, initAttackSpeed, initAttackRange, initDefense);

        // Set ChampionObject
        InitChampionObject(initWSkillCold, initESkillCold, initRSkillCold, "Fist/Champion/Sorcerer/Weapon/");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    protected override void Update()
    {
        base.Update();
    }

    public override void LogicUpdate(ChampionOperation frameOperation)
    {
        base.LogicUpdate(frameOperation);
    }

    protected override void SyncTimer(float dt)
    {
        base.SyncTimer(dt);
    }


    /************************* Attack, Skills, Property,  *************************/

    protected override void Attack()
    {
        if (stopAttack)
        {
            return;
        }
        if (championOperation.qClicked && attackReady)
        {
            // Instantiate attack weapon
            Transform attackWeapon = GlobalController.Instance.gameController.fistManager.Instantiate(attackPrefab, transform.position, Quaternion.identity);
            attackWeapon.GetComponent<SorcererAttackController>().Initialize(realAttribute.attackRange, realAttribute.attackSpeed / 2, transform.position, mouseMapPosition, this);
            // Set attackReady to false
            attackReady = false;
        }
    }

    protected override void SkillW()
    {
        if (championOperation.wClicked && skillInfoDict[ChampionSkillInfo.SkillType.W].ready)
        {
            // Set not ready
            skillInfoDict[ChampionSkillInfo.SkillType.W].ready = false;
            // Instantiate Skill
            Transform skillWtransform = GlobalController.Instance.gameController.fistManager.Instantiate(skillWPrefab, transform.position, transform.rotation);
            skillWtransform.GetComponent<SorcererSkillWController>().Initialize(initWSkillSpeed, initWSkillDistance, transform.position, mouseMapPosition, this);
        }
    }

    protected override void SkillE()
    {
        if (championOperation.eClicked && skillInfoDict[ChampionSkillInfo.SkillType.E].ready)
        {
            // Set not ready
            skillInfoDict[ChampionSkillInfo.SkillType.E].ready = false;
            // Instantiate Skill
            Transform skillWtransform = GlobalController.Instance.gameController.fistManager.Instantiate(skillEPrefab, transform.position, transform.rotation);
            skillWtransform.GetComponent<SorcererSkillEController>().Initialize(initESkillDuration, transform.position, mouseMapPosition, this);
        }
    }

    protected override void SkillR()
    {
        if (championOperation.rClicked && skillInfoDict[ChampionSkillInfo.SkillType.R].ready)
        {
            // Set not ready
            skillInfoDict[ChampionSkillInfo.SkillType.R].ready = false;
            // Skill
            Transform skillWtransform = GlobalController.Instance.gameController.fistManager.Instantiate(skillRPrefab, transform.position, transform.rotation);
            skillWtransform.GetComponent<SorcererSkillRController>().Initialize(initRSkillMaxDistance, initRSkillDuration, transform.position, mouseMapPosition, this);
        }
    }

    protected override void UseProperty()
    {

    }

    /************************* Damage *************************/

    public override void AttackDamage(FistObject targetFist)
    {
        // Weapon
        WeaponEffect(targetFist);
        // Damage
        Inflict(targetFist, realAttribute.attack);
    }

    public override void SkillWDamage(FistObject targetFist)
    {
        // Weapon
        WeaponEffect(targetFist);
        // Damage
        float finalWSkillDamage = initWSkillDamage + realAttribute.attack * initWSkillDamageRatio;
        Inflict(targetFist, finalWSkillDamage);
        // Control
        targetFist.ChangeAttribute(AttributeType.MoveSpeed, ChangeAttibuteRecord.SourceType.ChampionSkill, skillChangeAttributeTag[ChampionSkillInfo.SkillType.W], initWSkillDecreaseSpeed, initWSkillDecreaseSpeedDuration);
    }

    public override void SkillEDamage(FistObject targetFist)
    {
        // Weapon
        WeaponEffect(targetFist);
        // Damage
        float finalESkillDamage = initESkillDamage + realAttribute.attack * initESkillDamageRatio;
        if (targetFist.speedDecreased)
        {
            finalESkillDamage *= initESkillDamageCoef;
        }
        Inflict(targetFist, finalESkillDamage);
    }

    public override void SkillRDamage(FistObject targetFist)
    {
        // Weapon
        WeaponEffect(targetFist);
        // Damage
        float finalRSkillDamage = initRSkillDamage + realAttribute.attack * initRSkillDamageRatio;
        Inflict(targetFist, finalRSkillDamage);
    }

    /************************* Other Update Function *************************/



    /************************* Collision Event *************************/



}
