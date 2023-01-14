using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Robot : ChampionObject
{
    /* Initial Attribute */
    private static string configPath = "Fist/Champion/Robot/Documents/config";
    private static float initLife;                          // Initial life
    private static float initAttack;                        // Initial attack
    private static float initMoveSpeed;                     // Initial move speed
    private static float initAttackSpeed;                   // Initial attack speed
    private static float initAttackRange;                   // Initial attack range
    private static float initDefense;                       // Initial defense
    // Skill Related
    private static float initWSkillCold;                    // Inital w skill cold
    private static float initWSkillDistance;                // Inital w skill distance
    private static float initWSkillRange;                   // Inital w skill range
    private static float initWSkillDamage;                  // Inital w skill damage
    private static float initWSkillDamageRatio;             // Inital w skill damage ratio
    private static float initWSkillDuration;                // Inital w skill duration
    private static float initWSkillStartAttackAngle;        // Inital w skill start attack angle
    private static float initWSkillStunDuration;            // Inital w skill stun duration

    private static float initESkillCold;                    // Inital e skill cold
    private static float initESkillMaxDistance;             // Inital e skill radius
    private static float initESkillDuration;                // Inital e skill Speed
    private static float initESkillDamage;                  // Inital e skill damage
    private static float initESkillDamageRatio;             // Inital e skill damage ratio

    private static float initRSkillCold;                    // Inital r skill cold
    private static float initRSkillDuration;                // Inital r skill duration
    private static float initRSkillIncreaseMaxLife;         // Inital r skill attack rate
    private static float initRSkillIncreaseAttack;          // Inital r skill decrease speed ratio
    private static float initRSkillIncreaseMoveSpeed;       // Inital r skill decrease speed duration

    // Robot Info
    private static Dictionary<ChampionSkillInfo.SkillType, int> skillChangeAttributeTag;    // skill change attribute tag

    public Transform attackPrefab;

    public Transform skillWPrefab;

    public Transform skillEPrefab;



    public static void RobotInitialize()
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
        initWSkillRange = float.Parse(skillWNode.SelectSingleNode("Range").InnerText);
        initWSkillDamage = float.Parse(skillWNode.SelectSingleNode("Damage").InnerText);
        initWSkillDamageRatio = float.Parse(skillWNode.SelectSingleNode("DamageRatio").InnerText);
        initWSkillDuration = float.Parse(skillWNode.SelectSingleNode("Duration").InnerText);
        initWSkillStartAttackAngle = float.Parse(skillWNode.SelectSingleNode("StartAttackAngle").InnerText);
        initWSkillStunDuration = float.Parse(skillWNode.SelectSingleNode("StunDuration").InnerText);
        XmlNode skillENode = skillNode.SelectSingleNode("E");
        initESkillCold = float.Parse(skillENode.SelectSingleNode("Cold").InnerText);
        initESkillMaxDistance = float.Parse(skillENode.SelectSingleNode("MaxDistance").InnerText);
        initESkillDuration = float.Parse(skillENode.SelectSingleNode("Duration").InnerText);
        initESkillDamage = float.Parse(skillENode.SelectSingleNode("Damage").InnerText);
        initESkillDamageRatio = float.Parse(skillENode.SelectSingleNode("DamageRatio").InnerText);
        XmlNode skillRNode = skillNode.SelectSingleNode("R");
        initRSkillCold = float.Parse(skillRNode.SelectSingleNode("Cold").InnerText);
        initRSkillDuration = float.Parse(skillRNode.SelectSingleNode("Duration").InnerText);
        initRSkillIncreaseMaxLife = float.Parse(skillRNode.SelectSingleNode("IncreaseMaxLife").InnerText);
        initRSkillIncreaseAttack = float.Parse(skillRNode.SelectSingleNode("IncreaseAttack").InnerText);
        initRSkillIncreaseMoveSpeed = float.Parse(skillRNode.SelectSingleNode("IncreaseMoveSpeed").InnerText);
    }

    void Awake()
    {
        // Set Static Info
        InitializeChangeAttributeTag(skillChangeAttributeTag);

        // Set FistObject
        InitFistObject(initLife, initAttack, initMoveSpeed, initAttackSpeed, initAttackRange, initDefense);

        // Set ChampionObject
        InitChampionObject(initWSkillCold, initESkillCold, initRSkillCold, "Fist/Champion/Robot/Weapon/");
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
            attackWeapon.GetComponent<RobotAttackController>().Initialize(realAttribute.attackRange, realAttribute.attackSpeed / 2, transform.position, mouseMapPosition, this);
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
            skillWtransform.GetComponent<RobotSkillWController>().Initialize(initWSkillDistance, initWSkillRange, initWSkillDuration, initWSkillStartAttackAngle, transform.position, mouseMapPosition, this);
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
            skillWtransform.GetComponent<RobotSkillEController>().Initialize(initESkillMaxDistance, initESkillDuration, transform.position, mouseMapPosition, this);
        }
    }

    protected override void SkillR()
    {
        if (championOperation.rClicked && skillInfoDict[ChampionSkillInfo.SkillType.R].ready)
        {
            // Set not ready
            skillInfoDict[ChampionSkillInfo.SkillType.R].ready = false;
            // Skill
            ChangeAttribute(AttributeType.MaxLife, ChangeAttibuteRecord.SourceType.ChampionSkill, skillChangeAttributeTag[ChampionSkillInfo.SkillType.R], initRSkillIncreaseMaxLife, initRSkillDuration);
            ChangeAttribute(AttributeType.Life, ChangeAttibuteRecord.SourceType.ChampionSkill, skillChangeAttributeTag[ChampionSkillInfo.SkillType.R], initRSkillIncreaseMaxLife, initRSkillDuration);
            ChangeAttribute(AttributeType.Attack, ChangeAttibuteRecord.SourceType.ChampionSkill, skillChangeAttributeTag[ChampionSkillInfo.SkillType.R], initRSkillIncreaseAttack, initRSkillDuration);
            ChangeAttribute(AttributeType.MoveSpeed, ChangeAttibuteRecord.SourceType.ChampionSkill, skillChangeAttributeTag[ChampionSkillInfo.SkillType.R], initRSkillIncreaseMoveSpeed, initRSkillDuration);
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
        // Inflict
        float finalWSkillDamage = initWSkillDamage + realAttribute.attack * initWSkillDamageRatio;
        Inflict(targetFist, finalWSkillDamage);
        // Control
        ControlEffect controlEffect = new ControlEffect(ControlEffect.ControlType.Stunned, initWSkillStunDuration, ChangeAttibuteRecord.SourceType.ChampionSkill, skillChangeAttributeTag[ChampionSkillInfo.SkillType.W]);
        targetFist.SpellControl(controlEffect);
    }

    public override void SkillEDamage(FistObject targetFist)
    {
        // Weapon
        WeaponEffect(targetFist);
        float finalESkillDamage = initESkillDamage + realAttribute.attack * initESkillDamageRatio;
        // Damage
        Inflict(targetFist, finalESkillDamage);
    }

    public override void SkillRDamage(FistObject targetFist)
    {
        
    }

    /************************* Other Update Function *************************/



    /************************* Collision Event *************************/



}
