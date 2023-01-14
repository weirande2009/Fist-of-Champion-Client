using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistObject : FistBase
{
    // Basic attributes
    protected bool dead;                                // Dead
    public bool Dead { get { return dead; } }           // Get dead
    protected FistAttribute attribute;                  // attribute
    protected FistAttribute realAttribute;              // attribute
    protected FistAttribute lastFrameRealAttribute;     // attribute of last frame
    public bool lifeNeedUpdate;                         // life change, need updating
    public bool attributeNeedUpdate;                    // attribute accept life, nedd updating
    protected float attackTimer;                        // Attack Timer
    protected bool attackReady;                         // Attack Ready

    public bool damaged;                                // Damaged
    public bool speedDecreased;                         // Speed decreased

    protected ControlEffectController controlEffectController;          // Control effect controller

    // Basic Component
    protected Rigidbody2D rbody;
    protected Animator animator;
    protected BoxCollider2D boxCollider;

    // Attribute Change
    public enum AttributeType
    {
        Attack = 0,
        AttackSpeed = 1,
        AttackRange = 2,
        MoveSpeed = 3,
        Defense = 4,
        MaxLife = 5,
        Life = 6,
    }
    protected Dictionary<AttributeType, ChangeAttibuteRecordList> changeAttibuteRecords;

    // Move Related Variables
    protected Vector2 targetMovePosition;                   // Target of move position
    protected Vector2 lastFramePosition;                    // Position of last frame
    protected Vector2 targetMoveDirection;                  // Target of move direction

    // Stop Flags
    protected bool stopMove;                                // Stop move
    protected bool stopAttack;                              // Stop attack
    protected bool stopAnimate;                             // Stop animate

    // State
    public enum FistState
    {
        Normal = 0,
        Dead = 1,
        Silent = 2,
        Stunned = 3,
    }
    protected FistState fistState;                  // champion state

    /************************* Initialize *************************/

    protected void InitFistObject(float _life, float _attack, float _moveSpeed, float _attackSpeed, float _attackRange, float _defense)
    {
        // Basic Component
        rbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        // Basic Attribute
        dead = false;
        attribute = new FistAttribute(_life, _life, _attack, _moveSpeed, _defense, _attackSpeed, _attackRange);
        realAttribute = new FistAttribute(_life, _life, _attack, _moveSpeed, _defense, _attackSpeed, _attackRange);
        lastFrameRealAttribute = new FistAttribute();
        // Attribute Related Variables
        changeAttibuteRecords = new Dictionary<AttributeType, ChangeAttibuteRecordList>();
        changeAttibuteRecords[AttributeType.MaxLife] = new ChangeAttibuteRecordList(ChangeAttibuteRecordList.AttributeType.MaxLife);
        changeAttibuteRecords[AttributeType.Life] = new ChangeAttibuteRecordList(ChangeAttibuteRecordList.AttributeType.Life);
        changeAttibuteRecords[AttributeType.Attack] = new ChangeAttibuteRecordList(ChangeAttibuteRecordList.AttributeType.Attack);
        changeAttibuteRecords[AttributeType.AttackSpeed] = new ChangeAttibuteRecordList(ChangeAttibuteRecordList.AttributeType.AttackSpeed);
        changeAttibuteRecords[AttributeType.MoveSpeed] = new ChangeAttibuteRecordList(ChangeAttibuteRecordList.AttributeType.MoveSpeed);
        changeAttibuteRecords[AttributeType.Defense] = new ChangeAttibuteRecordList(ChangeAttibuteRecordList.AttributeType.Defense);
        lifeNeedUpdate = false;
        attributeNeedUpdate = false;
        // Attack Related Variables
        attackTimer = 0;
        attackReady = true;
        // Move Related Variables
        targetMovePosition = transform.position;
        lastFramePosition = transform.position;
        targetMoveDirection = Vector2.zero;
        // Stop Flags
        stopMove = false;
        stopAttack = false;
        stopAnimate = false;
        // State
        fistState = FistState.Normal;
        damaged = false;
        speedDecreased = false;
        // Control Effect
        controlEffectController = new ControlEffectController();
        // Set Last Frame Position
        lastSyncPosition = transform.position;
    }

    /************************* Steps in Update *************************/
    protected virtual void Update()
    {
        // Begin
        StartFrame();
        // Move
        if (fistState != FistState.Stunned)
        {
            UpdateMove(Time.deltaTime);
        }
        // Animation
        Animate();
        // End
        EndFrame();
    }


    public override void LogicUpdate()
    {
        
    }

    protected override void SyncTimer(float dt)
    {
        // Set Timer
        UpdateAttackTimer(dt);

        // Control
        controlEffectController.Update(dt);

        // Update Attribute
        UpdateAttribute(dt);
    }

    public override void SyncLastLogicFrame(float dt)
    {
        base.SyncLastLogicFrame(dt);
    }


    /// <summary>
    /// Called at the beginning of Update function
    /// </summary>
    protected virtual void StartFrame()
    {
        // Record position
        lastFramePosition = transform.position;
        // Get state
        fistState = controlEffectController.CurrentState();
    }

    /// <summary>
    /// Called at the end of Update function
    /// </summary>
    protected virtual void EndFrame()
    {
        
    }

    protected override void UpdateMove(float dt, bool logic = false)
    {
        if (stopMove)
        {
            return;
        }
        if (!UtilityTool.ObjectArrive(transform.position, targetMovePosition, targetMoveDirection))
        {
            Vector2 position = transform.position;
            transform.position = position + targetMoveDirection * realAttribute.moveSpeed * dt;
        }
        else
        {
            if (logic)
            {
                moving = false;
            }
        }
    }

    protected void UpdateAttackTimer(float dt)
    {
        if(attackTimer > (1 / realAttribute.attackSpeed))
        {
            attackTimer = 0;
            attackReady = true;
        }
        if (!attackReady)
        {
            attackTimer += dt;
        }
    }

    protected void UpdateAttribute(float dt)
    {
        // MaxLife
        realAttribute.maxLife = attribute.maxLife + changeAttibuteRecords[AttributeType.MaxLife].Sum();
        // Life
        realAttribute.life = realAttribute.life + changeAttibuteRecords[AttributeType.Life].Sum();
        if(realAttribute.life > realAttribute.maxLife)
        {
            realAttribute.life = realAttribute.maxLife;
        }
        // AttackW
        realAttribute.attack = attribute.attack + changeAttibuteRecords[AttributeType.Attack].Sum();
        // Attack Speed
        realAttribute.attackSpeed = attribute.attackSpeed * (1 + changeAttibuteRecords[AttributeType.AttackSpeed].Sum() / 100.0f);
        // Move speed
        realAttribute.moveSpeed = attribute.moveSpeed * (1 + changeAttibuteRecords[AttributeType.MoveSpeed].Sum() / 100.0f);
        if(realAttribute.moveSpeed < attribute.moveSpeed)
        {
            speedDecreased = true;
        }
        else
        {
            speedDecreased = false;
        }
        // Defense
        realAttribute.defense = attribute.defense + (changeAttibuteRecords[AttributeType.Defense].Sum());
        // Set Time
        changeAttibuteRecords[AttributeType.MaxLife].SetTime(dt);
        changeAttibuteRecords[AttributeType.Life].SetTime(dt);
        changeAttibuteRecords[AttributeType.Attack].SetTime(dt);
        changeAttibuteRecords[AttributeType.AttackSpeed].SetTime(dt);
        changeAttibuteRecords[AttributeType.MoveSpeed].SetTime(dt);
        changeAttibuteRecords[AttributeType.Defense].SetTime(dt);
        // Check Change
        if (lastFrameRealAttribute.LifeChanged(realAttribute))
        {
            lifeNeedUpdate = true;
        }
        if (lastFrameRealAttribute.AttributeChanged(realAttribute))
        {
            attributeNeedUpdate = true;
        }
        lastFrameRealAttribute = new FistAttribute(realAttribute);
    }

    protected virtual void Animate()
    {
        // Animate Move
        AnimateMove();
    }

    protected void AnimateMove()
    {
        if (stopAnimate)
        {
            return;
        }
        // Set Direction
        Vector2 position = transform.position;
        if ((lastFramePosition - position).x < 0)
        {
            animator.SetBool("Direction", true);
        }
        else if ((lastFramePosition - position).x > 0)
        {
            animator.SetBool("Direction", false);
        }
        // Set Speed
        if ((lastFramePosition - position).magnitude > 1e-6)
        {
            animator.SetFloat("Speed", (position - lastFramePosition).magnitude / Time.deltaTime);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }

    /************************* Damage *************************/
    public void Damage(float damage, float broken=0)
    {
        float finalDefense = Mathf.Clamp((realAttribute.defense - broken) / 100, 0, 1);
        // Debug.Log("ÕÊº“ ‹µΩ…À∫¶: " + damage * (1 - finalDefense));
        realAttribute.life -= damage * (1 - finalDefense);
        damaged = true;
        if (realAttribute.life < 0)
        {
            realAttribute.life = 0;
        }
        JudgeDead();
    }

    public void JudgeDead()
    {
        if (realAttribute.life <= 0)
        {
            dead = true;
            Death();
        }
    }

    protected virtual void Death()
    {
        Destroy(gameObject);
    }

    /************************* Attribute *************************/

    public void ChangeAttribute(AttributeType attributeType, ChangeAttibuteRecord.SourceType sourceType, int tag, float amount, float duration=0)
    {
        ChangeAttibuteRecord record = new ChangeAttibuteRecord(amount, duration, sourceType, tag);
        changeAttibuteRecords[attributeType].Add(record);
    }

    public FistAttribute GetAttribute()
    {
        return attribute;
    }

    public FistAttribute GetRealAttribute()
    {
        return realAttribute;
    }

    /************************* Control Effect *************************/
    public void SpellControl(ControlEffect controlEffect)
    {
        controlEffectController.Add(controlEffect);
    }

}

public class ChangeAttibuteRecordList
{
    // Main List
    public List<ChangeAttibuteRecord> records;

    // Attribute Type
    public enum AttributeType
    {
        Attack = 0,
        Defense = 1,
        MoveSpeed = 2,
        AttackSpeed = 3,
        MaxLife = 4,
        Life = 5,
    }

    public AttributeType attributeType;

    // Attribute from weapon or armor
    public float equipmentAttribute;

    public ChangeAttibuteRecordList(AttributeType _attributeType)
    {
        records = new List<ChangeAttibuteRecord>();
        attributeType = _attributeType;
        equipmentAttribute = 0;
    }

    public void Add(ChangeAttibuteRecord record)
    {
        bool flag = false;
        foreach(ChangeAttibuteRecord attibuteRecord in records)
        {
            if(attibuteRecord.sourceType == record.sourceType && attibuteRecord.tag == record.tag)
            {
                attibuteRecord.duration = record.duration;
                attibuteRecord.amount = record.amount;
                flag = true;
            }
        }
        if (!flag)
        {
            records.Add(record);
        }
    }

    public float Sum()
    {
        float sum = 0;
        // Sum from records
        foreach (ChangeAttibuteRecord record in records)
        {
            sum += record.amount;
        }
        // Add equipment attribute
        sum += equipmentAttribute;
        return sum;
    }

    public void SetEquipmentAttribute(float attr)
    {
        equipmentAttribute = attr;
    }

    public void SetTime(float time)
    {
        int removedNumber = 0;
        // Set decrease duration
        for (int i = 0; i < records.Count; i++)
        {
            records[i].duration -= time;
            if (records[i].duration < 0)
            {
                records.RemoveAt(i - removedNumber);
                removedNumber++;
            }
        }
    }

}

public class ChangeAttibuteRecord
{
    // Static
    public static Dictionary<SourceType, List<int>> sourceTypeTagDict;

    public static void Initialize()
    {
        sourceTypeTagDict = new Dictionary<SourceType, List<int>>();
        foreach (SourceType foo in System.Enum.GetValues(typeof(SourceType)))
        {
            sourceTypeTagDict[foo] = new List<int>();
        }
    }

    public static int GetNewTag(SourceType _sourceType)
    {
        int newTag = sourceTypeTagDict[_sourceType].Count;
        sourceTypeTagDict[_sourceType].Add(newTag);
        return newTag;
    }

    // Class
    public float amount;        // Decrease amount
    public float duration;      // Decrease duration
    public enum SourceType
    {
        ChampionSkill = 0,
        Weapon = 1,
        Map = 2,
    }
    public SourceType sourceType;   // Source type
    public int tag;                 // Tag of record, using to denote a specific type of ChangeAttribute

    public ChangeAttibuteRecord(float _amount, float _duration, SourceType _sourceType, int _tag)
    {
        amount = _amount;
        duration = _duration;
        sourceType = _sourceType;
        tag = _tag;
    }
}

public class FistAttribute
{
    public float maxLife;                   // max life
    public float life;                      // Life
    public float attack;                    // Attack
    public float moveSpeed;                 // Move speed
    public float defense;                   // Defense
    public float attackSpeed;               // Attack Speed
    public float attackRange;               // Attack Range

    public FistAttribute(FistAttribute attr)
    {
        maxLife = attr.maxLife;
        life = attr.life;
        attack = attr.attack;
        moveSpeed = attr.moveSpeed;
        defense = attr.defense;
        attackSpeed = attr.attackSpeed;
        attackRange = attr.attackRange;
    }
    public FistAttribute(float _maxLife = 0, float _life = 0, float _attack = 0, float _moveSpeed = 0, float _defense = 0, float _attackSpeed = 0, float _attackRange = 0)
    {
        maxLife = _maxLife;
        life = _life;
        attack = _attack;
        moveSpeed = _moveSpeed;
        defense = _defense;
        attackSpeed = _attackSpeed;
        attackRange = _attackRange;
    }

    public bool LifeChanged(FistAttribute attr)
    {
        if(attr.life != life || attr.maxLife != maxLife)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public bool AttributeChanged(FistAttribute attr)
    {
        if (attr.attack == attack && attr.moveSpeed == moveSpeed && attr.attackSpeed == attackSpeed && attr.defense == defense)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

}

public class ControlEffect
{
    public enum ControlType
    {
        Silent = 0,
        Stunned = 1,
    }

    public ControlType type;
    public float duration;
    public ChangeAttibuteRecord.SourceType sourceType;  // Source type
    public int tag;                                     // Tag of record, using to denote a specific type of ChangeAttribute

    public ControlEffect(ControlType _type, float _duration, ChangeAttibuteRecord.SourceType _sourceType, int _tag)
    {
        type = _type;
        duration = _duration;
        sourceType = _sourceType;
        tag = _tag;
    }
}

public class ControlEffectController
{
    // ControlEffect list
    public List<ControlEffect> controlEffectList;


    public ControlEffectController()
    {
        controlEffectList = new List<ControlEffect>();
    }

    public FistObject.FistState CurrentState()
    {
        FistObject.FistState currentState = FistObject.FistState.Normal;
        foreach (ControlEffect effect in controlEffectList)
        {
            if (effect.type == ControlEffect.ControlType.Silent)
            {
                currentState = FistObject.FistState.Silent;
            }
            else if (effect.type == ControlEffect.ControlType.Stunned)
            {
                currentState = FistObject.FistState.Stunned;
                // if has stunned, the final state is stunned
                break;
            }
        }
        return currentState;
    }

    public void Update(float dt)
    {
        // Update duration
        foreach (ControlEffect effect in controlEffectList)
        {
            effect.duration -= dt;
        }
        // Clear 
        int romoveNum = 0;
        for (int i = 0; i < controlEffectList.Count; i++)
        {
            if (controlEffectList[i].duration <= 0)
            {
                controlEffectList.RemoveAt(i - romoveNum);
                romoveNum++;
            }
        }
    }

    public void Add_old(ControlEffect controlEffect)
    {
        bool flag = false;
        foreach (ControlEffect effect in controlEffectList)
        {
            if (effect.sourceType == controlEffect.sourceType && effect.tag == controlEffect.tag)
            {
                effect.duration = controlEffect.duration;
                flag = true;
            }
        }
        if (!flag)
        {
            controlEffectList.Add(controlEffect);
        }
    }

    public void Add(ControlEffect controlEffect)
    {
        controlEffectList.Add(controlEffect);
    }
}

public class SkillInfo
{
    public float cold;                  // Skill Cold
    public float timer;                 // Skill Timer
    public bool ready;                  // Skill Ready

    public SkillInfo(float _cold)
    {
        cold = _cold;
        timer = 0;
        ready = true;
    }
}

