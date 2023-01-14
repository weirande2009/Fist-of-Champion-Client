using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionObject : FistObject
{
    // Static
    private static Dictionary<FistState, Sprite> championStateSpriteDict;  

    public static void ChampionInitialize()
    {
        championStateSpriteDict = new Dictionary<FistState, Sprite>();
        championStateSpriteDict[FistState.Normal] = null;
        championStateSpriteDict[FistState.Dead] = null;
        championStateSpriteDict[FistState.Silent] = Resources.Load<Sprite>("Sprites/Other/Silent");
        championStateSpriteDict[FistState.Stunned] = Resources.Load<Sprite>("Sprites/Other/Stunned");
    }

    // Debug
    float debugInterval = 1;

    // Champion
    public enum ChampionType
    {
        Assassin = 0,
        Robot = 1,
        Wizard = 2,
        Sorcerer = 3,
        Ranger = 4,
        Minister = 5,
        Berserker = 6,
        Knight = 7,
    }

    // Main
    public bool mainChampion;                               // Whether the champion is main (the champion you control)
    public bool watchedChampion;                            // Whether the champion is watched
    protected GameObject minimapFrame;                      // Minimap Frame
    public int playerNo;                                    // Player No.

    // Move variables of Champion
    protected Vector2 mouseMapPosition;                     // Position of mouse on map

    // Character
    protected Sprite deadSprite;
    protected GameObject championWeaponImage;               // Champion weapon image
    protected SpriteRenderer championWeaponSprite;          // Champion weapon sprite
    protected SpriteRenderer championStateSprite;           // Champion state sprite

    // Music

    // Equipment
    public Weapon weapon;
    public Armor armor;
    public Dictionary<WeaponInfoController.WeaponType, Sprite> weaponSpriteDict;

    // Operation
    protected ChampionOperation championOperation;

    // Skill
    protected Dictionary<ChampionSkillInfo.SkillType, ChampionSkillInfo> skillInfoDict;     // skill info dict

    /************************* Initialize and Reset *************************/

    protected void InitChampionObject(float _wSkillCold, float _eSkillCold, float _rSkillCold, string weaponSpriteDirName)
    {
        // Set tag
        tag = "Champion";
        // Main Player Related Variables
        mainChampion = false;
        watchedChampion = false;
        // Champion Appearance
        championWeaponImage = transform.Find("ChampionWeapon").gameObject;
        championWeaponSprite = championWeaponImage.GetComponent<SpriteRenderer>();
        championStateSprite = transform.Find("StateImage").GetComponent<SpriteRenderer>();
        championWeaponSprite.sprite = null;
        // Mouse 
        mouseMapPosition = transform.position;
        // Weapon and Armor
        weaponSpriteDict = new Dictionary<WeaponInfoController.WeaponType, Sprite>();
        weaponSpriteDict[WeaponInfoController.WeaponType.Low] = Resources.Load<Sprite>(weaponSpriteDirName + "LowWeapon");
        weaponSpriteDict[WeaponInfoController.WeaponType.Middle] = Resources.Load<Sprite>(weaponSpriteDirName + "MiddleWeapon");
        weaponSpriteDict[WeaponInfoController.WeaponType.High] = Resources.Load<Sprite>(weaponSpriteDirName + "HighWeapon");
        // Skill Info
        skillInfoDict = new Dictionary<ChampionSkillInfo.SkillType, ChampionSkillInfo>();
        skillInfoDict.Add(ChampionSkillInfo.SkillType.W, new ChampionSkillInfo(_wSkillCold));
        skillInfoDict.Add(ChampionSkillInfo.SkillType.E, new ChampionSkillInfo(_eSkillCold));
        skillInfoDict.Add(ChampionSkillInfo.SkillType.R, new ChampionSkillInfo(_rSkillCold));
        // Minimap Frame
        minimapFrame = GameObject.Find("MainCharacterFrame");
    }

    protected void InitializeChangeAttributeTag(Dictionary<ChampionSkillInfo.SkillType, int> skillChangeAttributeTag)
    {
        if (skillChangeAttributeTag == null)
        {
            skillChangeAttributeTag = new Dictionary<ChampionSkillInfo.SkillType, int>();
            foreach (ChampionSkillInfo.SkillType skillType in System.Enum.GetValues(typeof(ChampionSkillInfo.SkillType)))
            {
                skillChangeAttributeTag[skillType] = ChangeAttibuteRecord.GetNewTag(ChangeAttibuteRecord.SourceType.ChampionSkill);
            }
        }
    }

    /************************* Set Infomation *************************/

    public void SetMainPlayer()
    {
        mainChampion = true;
        watchedChampion = true;
        minimapFrame.transform.position = transform.position;
    }

    /************************* Update *************************/

    protected override void Update()
    {
        if (dead)
        {
            return;
        }
        // Begin
        StartFrame();
        // Move
        if (fistState != FistState.Stunned)
        {
            UpdateMove(Time.deltaTime);
        }
        // Animation
        Animate();
        // MiniMap Frame
        UpdateMiniMapFrame();
        // End
        EndFrame();
    }

    public virtual void LogicUpdate(ChampionOperation frameOperation)
    {
        // Set Operation
        championOperation = frameOperation;

        // Champion Operation
        ChampionOperationUpdate();

        // Fist
        base.LogicUpdate();
    }

    protected override void SyncTimer(float dt)
    {
        // Fist Sync Timer
        base.SyncTimer(dt);
        
        // Control
        championStateSprite.sprite = championStateSpriteDict[fistState];

        // Set Timer
        UpdateSkillTimer(dt);

        // Weapon
        if (weapon != null)
        {
            weapon.Update(dt);
        }
    }

    public virtual void SyncLastLogicFrame(float dt, ChampionOperation frameOperation)
    {
        // Set Operation
        championOperation = frameOperation;
        // Champion Operation
        ChampionOperationUpdate();
        base.SyncLastLogicFrame(dt);
    }

    /// <summary>
    /// ChampionOperationUpdate is called when receiving key frame from server
    /// </summary>
    protected void ChampionOperationUpdate()
    {
        if(fistState == FistState.Normal)
        {
            ProcessMouseClick();
            Attack();
            SkillW();
            SkillE();
            SkillR();
            DetectEquipment();
        }
        else if(fistState == FistState.Silent)
        {
            ProcessMouseClick();
            Attack();
            DetectEquipment();
        }
        else if(fistState == FistState.Stunned)
        {
            ProcessMouseClick();
            DetectEquipment();
        }
    }

    /// <summary>
    /// Called at the end of Update function
    /// </summary>
    protected override void EndFrame()
    {
        if (debugInterval > 1)
        {
            debugInterval = 0;
        }
        debugInterval += Time.deltaTime;
    }

    protected void ProcessMouseClick()
    {
        mouseMapPosition = championOperation.mouseTargetPosition;
        // If clicked, get mouse position
        if (championOperation.moveClicked && !stopMove)
        {
            targetMovePosition = mouseMapPosition;
            targetMoveDirection = (targetMovePosition - new Vector2(transform.position.x, transform.position.y)).normalized;
            moving = true;
        }
    }

    protected void DetectEquipment()
    {
        if (championOperation.changeWeapon)
        {
            AddWeapon();
        }
        if (championOperation.changeArmor)
        {
            AddArmor();
        }
    }

    protected void AddWeapon()
    {
        if (championOperation.lowLevelWeaponNo != 255)
        {
            switch (championOperation.lowLevelWeaponNo)
            {
                case 0:
                    weapon = new LowWeapon1();
                    break;
                case 1:
                    weapon = new LowWeapon2();
                    break;
                case 2:
                    weapon = new LowWeapon3();
                    break;
            }
            championWeaponSprite.sprite = weaponSpriteDict[WeaponInfoController.WeaponType.Low];
        }
        if (championOperation.middleLevelWeaponNo != 255)
        {
            switch (championOperation.middleLevelWeaponNo)
            {
                case 0:
                    weapon = new MiddleWeapon1();
                    break;
                case 1:
                    weapon = new MiddleWeapon2();
                    break;
                case 2:
                    weapon = new MiddleWeapon3();
                    break;
                case 3:
                    weapon = new MiddleWeapon4();
                    break;
                case 4:
                    weapon = new MiddleWeapon5();
                    break;
            }
            championWeaponSprite.sprite = weaponSpriteDict[WeaponInfoController.WeaponType.Middle];
        }
        if (championOperation.highLevelWeaponNo != 255)
        {
            switch (championOperation.highLevelWeaponNo)
            {
                case 0:
                    weapon = new HighWeapon1();
                    break;
                case 1:
                    weapon = new HighWeapon2();
                    break;
                case 2:
                    weapon = new HighWeapon3();
                    break;
                case 3:
                    weapon = new HighWeapon4();
                    break;
                case 4:
                    weapon = new HighWeapon5();
                    break;
            }
            championWeaponSprite.sprite = weaponSpriteDict[WeaponInfoController.WeaponType.High];
        }
        // Set Attribute
        changeAttibuteRecords[AttributeType.Attack].equipmentAttribute = weapon.attack;
        changeAttibuteRecords[AttributeType.AttackSpeed].equipmentAttribute = weapon.attackSpeed;
        changeAttibuteRecords[AttributeType.MoveSpeed].equipmentAttribute = weapon.moveSpeed;
    }

    protected void AddArmor()
    {
        switch (championOperation.armorNo)
        {
            case 0:
                armor = new LowArmor();
                break;
            case 1:
                armor = new MiddleArmor();
                break;
            case 2:
                armor = new HighArmor();
                break;
        }
        // Set attribute
        changeAttibuteRecords[AttributeType.Defense].equipmentAttribute = armor.defense;
    }

    protected void UpdateMiniMapFrame()
    {
        if (watchedChampion)
        {
            minimapFrame.transform.position = transform.position;
        }
    }


    /************************* Timer *************************/
    protected void UpdateSkillTimer(float dt)
    {
        // Set Skill Timer
        foreach(var skillInfo in skillInfoDict)
        {
            if (skillInfo.Value.timer > skillInfo.Value.cold)
            {
                skillInfo.Value.timer = 0;
                skillInfo.Value.ready = true;
            }
            if (!skillInfo.Value.ready)
            {
                skillInfo.Value.timer += dt;
                // If main, Set in scene
                if (mainChampion)
                {
                    GameSceneController.Instance.SetSkill(skillInfo.Value.timer / skillInfo.Value.cold, skillInfo.Value.ready, skillInfo.Key);
                }
            }

        }
        // Set In Game Scene
        if (mainChampion && !attackReady)
        {
            GameSceneController.Instance.SetSkillQ(attackTimer * realAttribute.attackSpeed, attackReady);
        }
    }
    

    /************************* Attack, Skills, Property,  *************************/

    protected virtual void Attack()
    {
        
    }

    protected virtual void SkillW()
    {

    }

    protected virtual void SkillE()
    {

    }

    protected virtual void SkillR()
    {

    }

    protected virtual void UseProperty()
    {

    }

    /************************* Damage *************************/

    public virtual void AttackDamage(FistObject targetFist)
    {

    }

    public virtual void SkillWDamage(FistObject targetFist)
    {
        
    }

    public virtual void SkillEDamage(FistObject targetFist)
    {
        
    }

    public virtual void SkillRDamage(FistObject targetFist)
    {

    }

    protected virtual void Inflict(FistObject targetFist, float damage)
    {
        if (weapon is null)
        {
            targetFist.Damage(damage);
        }
        else
        {
            weapon.Attack(targetFist, damage);
        }
    }

    protected void WeaponEffect(FistObject targetFist)
    {
        if(weapon != null)
        {
            switch (weapon.effectObject)
            {
                case Weapon.EffectObject.None:
                    break;
                case Weapon.EffectObject.Self:
                    weapon.Effect(this);
                    break;
                case Weapon.EffectObject.Target:
                    weapon.Effect(targetFist);
                    break;
            }
        }
    }

    /************************* Load Resources *************************/


    /************************* Other Functions *************************/
    public void ClearWeapon()
    {
        // Set weapon
        weapon = null;
        // Set Attribute
        changeAttibuteRecords[AttributeType.Attack].equipmentAttribute = 0;
        changeAttibuteRecords[AttributeType.AttackSpeed].equipmentAttribute = 0;
        changeAttibuteRecords[AttributeType.MoveSpeed].equipmentAttribute = 0;
    }

    public void ClearArmor()
    {
        // Set Armor
        armor = null;
        // Set Attribute
        changeAttibuteRecords[AttributeType.Defense].equipmentAttribute = 0;
    }

    protected override void Death()
    {
        // Set Animator
        animator.SetBool("Alive", false);
        // Remove Component
        Destroy(rbody);
        Destroy(boxCollider);
        Destroy(championWeaponImage.gameObject);
    }

}

public class ChampionSkillInfo : SkillInfo
{
    public enum SkillType
    {
        W = 0,
        E = 1,
        R = 2,
    }

    public ChampionSkillInfo(float _cold) : base(_cold)
    {
        
    }
}



public class DurationChampionInfo
{
    public ChampionObject champion;
    public float durationTimer;

    public DurationChampionInfo(ChampionObject _champion)
    {
        champion = _champion;
        durationTimer = 0;
    }
}



