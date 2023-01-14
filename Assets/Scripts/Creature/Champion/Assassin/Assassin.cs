using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Assassin : ChampionObject
{
    /* Initial Attribute */
    private static string configPath = "Fist/Champion/Assassin/Documents/config";
    private static float initLife;                          // Initial life
    private static float initAttack;                        // Initial attack
    private static float initMoveSpeed;                     // Initial move speed
    private static float initAttackSpeed;                   // Initial attack speed
    private static float initAttackRange;                   // Initial attack range
    private static float initDefense;                       // Initial defense
    // Skill Related
    private static float initWSkillCold;                    // Inital w skill cold
    private static float initWSkillRadius;                  // Inital w skill range
    private static float initWSkillSpeed;                   // Inital w skill Speed
    private static float initWSkillDamage;                  // Inital w skill damage
    private static float initWSkillDamageRatio;             // Inital w skill damage ratio
    private static float initWSkillStunDuration;            // Inital w skill stun duration

    private static float initESkillCold;                    // Inital e skill cold
    private static float initESkillRadius;                  // Inital e skill radius
    private static float initESkillSpeed;                   // Inital e skill Speed
    private static float initESkillDamage;                  // Inital e skill damage
    private static float initESkillDamageRatio;             // Inital e skill damage ratio

    private static float initRSkillCold;                    // Inital r skill cold
    private static float initRSkillDuration;                // Inital r skill duration
    private static float initRSkillAttackRate;              // Inital r skill attack rate
    private static float initRSkillDecreaseSpeedRatio;      // Inital r skill decrease speed ratio
    private static float initRSkillDecreaseSpeedDuration;   // Inital r skill decrease speed duration
    private static float initRSkillDamage;                  // Inital r skill attack
    private static float initRSkillDamageRatio;             // Inital r skill damage ratio
    private static float initRSkillTransportRadius;         // Inital r skill transport radius

    // Assassin Info
    private static Dictionary<ChampionSkillInfo.SkillType, int> skillChangeAttributeTag;    // skill change attribute tag

    // Attack
    public Transform attackWeaponPrefab;                // Prefab of attack weapon                     

    // Skill W
    private bool inWSkill;                              // Whether are in w skill
    private Vector2 wSkillTargetPosition;               // Target position of w skill
    private Vector2 wSkillTargerDirection;              // Target direction of w skill

    // Skill E
    public Transform skillEPrefab;                      // e skill prefab

    // Skill R
    public Transform skillRPrefab;                      // r skill prefab

    public static void AssassinInitialize()
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
        initWSkillRadius = float.Parse(skillWNode.SelectSingleNode("Radius").InnerText);
        initWSkillSpeed = float.Parse(skillWNode.SelectSingleNode("Speed").InnerText);
        initWSkillDamage = float.Parse(skillWNode.SelectSingleNode("Damage").InnerText);
        initWSkillDamageRatio = float.Parse(skillWNode.SelectSingleNode("DamageRatio").InnerText);
        initWSkillStunDuration = float.Parse(skillWNode.SelectSingleNode("StunDuration").InnerText);
        XmlNode skillENode = skillNode.SelectSingleNode("E");
        initESkillCold = float.Parse(skillENode.SelectSingleNode("Cold").InnerText);
        initESkillRadius = float.Parse(skillENode.SelectSingleNode("Radius").InnerText);
        initESkillSpeed = float.Parse(skillENode.SelectSingleNode("Speed").InnerText);
        initESkillDamage = float.Parse(skillENode.SelectSingleNode("Damage").InnerText);
        initESkillDamageRatio = float.Parse(skillENode.SelectSingleNode("DamageRatio").InnerText);
        XmlNode skillRNode = skillNode.SelectSingleNode("R");
        initRSkillCold = float.Parse(skillRNode.SelectSingleNode("Cold").InnerText);
        initRSkillDuration = float.Parse(skillRNode.SelectSingleNode("Duration").InnerText);
        initRSkillDamage = float.Parse(skillRNode.SelectSingleNode("Damage").InnerText);
        initRSkillDamageRatio = float.Parse(skillRNode.SelectSingleNode("DamageRatio").InnerText);
        initRSkillAttackRate = float.Parse(skillRNode.SelectSingleNode("AttackRate").InnerText);
        initRSkillDecreaseSpeedRatio = float.Parse(skillRNode.SelectSingleNode("DecreaseSpeedRatio").InnerText);
        initRSkillDecreaseSpeedDuration = float.Parse(skillRNode.SelectSingleNode("DecreaseSpeedDuration").InnerText);
        initRSkillTransportRadius = float.Parse(skillRNode.SelectSingleNode("TransportRadius").InnerText);
    }

    void Awake()
    {
        // Set Static Info
        InitializeChangeAttributeTag(skillChangeAttributeTag);

        // Set FistObject
        InitFistObject(initLife, initAttack, initMoveSpeed, initAttackSpeed, initAttackRange, initDefense);

        // Set ChampionObject
        InitChampionObject(initWSkillCold, initESkillCold, initRSkillCold, "Fist/Champion/Assassin/Weapon/");
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
        // W Skill
        UpdateWSkill(dt);
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
            Transform attackWeapon = GlobalController.Instance.gameController.fistManager.Instantiate(attackWeaponPrefab, transform.position, Quaternion.identity);
            attackWeapon.GetComponent<AssassinAttackController>().Initialize(realAttribute.attackRange, realAttribute.attackSpeed * 2, transform.position, mouseMapPosition, this);
            // Set attackReady to false
            attackReady = false;
        }
    }

    protected override void SkillW()
    {
        if (!inWSkill)
        {
            if (championOperation.wClicked && skillInfoDict[ChampionSkillInfo.SkillType.W].ready)
            {
                // Set target position
                Vector2 position = transform.position;
                Vector2 dis = mouseMapPosition - position;
                wSkillTargetPosition = dis.normalized * initWSkillRadius + position;
                wSkillTargerDirection = dis.normalized;
                // Set is trigger
                boxCollider.isTrigger = true;
                // Set in skill
                inWSkill = true;
                // Set not ready
                skillInfoDict[ChampionSkillInfo.SkillType.W].ready = false;
                // Set stop
                stopAttack = true;
                stopMove = true;
                moving = false;
            }
        }
    }

    protected override void SkillE()
    {
        if (championOperation.eClicked && skillInfoDict[ChampionSkillInfo.SkillType.E].ready)
        {
            // Set not ready
            skillInfoDict[ChampionSkillInfo.SkillType.E].ready = false;
            // Instantiate Skill
            for(int i=0; i<3; i++)
            {
                Transform skillEtransform = GlobalController.Instance.gameController.fistManager.Instantiate(skillEPrefab, transform.position, transform.rotation);
                skillEtransform.GetComponent<AssassinSkillEController>().Initiailize(initESkillSpeed, initESkillRadius, transform.position, mouseMapPosition, i, this);
            }
        }
    }

    protected override void SkillR()
    {
        if (championOperation.rClicked && skillInfoDict[ChampionSkillInfo.SkillType.R].ready)
        {
            // Set not ready
            skillInfoDict[ChampionSkillInfo.SkillType.R].ready = false;
            // Instantiate Skill
            Transform skillRtransform = GlobalController.Instance.gameController.fistManager.Instantiate(skillRPrefab, transform.position, transform.rotation);
            skillRtransform.GetComponent<AssassinSkillRController>().SetInfo(initRSkillDuration, initRSkillAttackRate, this);
            // Randomly transport
            float angle = Mathf.PI * 2 * Random.value;
            Vector3 transportPosition = new Vector3(transform.position.x + Mathf.Cos(angle) * initRSkillTransportRadius, transform.position.y + +Mathf.Sin(angle) * initRSkillTransportRadius, transform.position.z);
            transform.position = transportPosition;
            targetMovePosition = transportPosition;
            // Set Moving False
            moving = false;
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
        ControlEffect controlEffect = new ControlEffect(ControlEffect.ControlType.Stunned, initWSkillStunDuration, ChangeAttibuteRecord.SourceType.ChampionSkill, skillChangeAttributeTag[ChampionSkillInfo.SkillType.W]);
        targetFist.SpellControl(controlEffect);
    }

    public override void SkillEDamage(FistObject targetFist)
    {
        // Weapon
        WeaponEffect(targetFist);
        // Damage
        float finalESkillDamage = initESkillDamage + realAttribute.attack * initESkillDamageRatio;
        Inflict(targetFist, finalESkillDamage);
    }

    public override void SkillRDamage(FistObject targetFist)
    {
        // Weapon
        WeaponEffect(targetFist);
        // Damage
        float finalRSkillDamage = initRSkillDamage + realAttribute.attack * initRSkillDamageRatio;
        Inflict(targetFist, finalRSkillDamage);
        // Decrease Speed
        ChangeAttribute(AttributeType.MoveSpeed, ChangeAttibuteRecord.SourceType.ChampionSkill, skillChangeAttributeTag[ChampionSkillInfo.SkillType.R], initRSkillDecreaseSpeedRatio, initRSkillDecreaseSpeedDuration);
    }

    /************************* Other Update Function *************************/

    private void UpdateWSkill(float dt)
    {
        if (inWSkill)
        {
            Vector2 position = transform.position;
            if (UtilityTool.ObjectArrive(position, wSkillTargetPosition, wSkillTargerDirection))
            {
                // Set not in skill
                inWSkill = false;
                // Set not trigger
                boxCollider.isTrigger = false;
                // Set stop
                stopAttack = false;
                stopMove = false;
                // Set target Position
                targetMovePosition = transform.position;
                return;
            }
            Vector2 dx = (wSkillTargetPosition - position).normalized * initWSkillSpeed * dt + position;
            transform.position = dx;
        }
    }


    /************************* Collision Event *************************/

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (inWSkill)
        {
            if (collision.transform.tag == "Champion")
            {
                SkillWDamage(collision.transform.GetComponent<FistObject>());
            }
            else if (collision.transform.tag == "Neutral")
            {
                SkillWDamage(collision.transform.GetComponent<FistObject>());
            }
        }
    }


}
