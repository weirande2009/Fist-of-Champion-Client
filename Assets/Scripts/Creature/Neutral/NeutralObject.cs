using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralObject : FistObject
{

    // Neutral Type
    public enum NeutralType
    {
        Low = 0,
        Middle = 1,
        High = 2,
    }
    protected NeutralType neutralType;

    // SkillInfo
    protected List<NeutralSkillInfo> skillInfoList;

    // Region Border
    protected Vector2 leftTopBorder;
    protected Vector2 rightBottomBorder;

    // Born Position
    protected Vector2 bornPosition;

    // Drop weapon or armor
    protected float dropProbability;
    protected List<WeaponBasicInfo> dropWeaponList;
    protected List<int> dropArmorList;

    // Intellegence Related Variables
    protected ChampionObject nearestChampion;
    protected float watchRadius;


    /************************* Initialize *************************/
    protected void InitNeutralObject(NeutralType _neutralType, float _watchRadius, List<NeutralSkillInfo> _skillInfoList, List<WeaponBasicInfo> _dropWeaponList, List<int> _dropArmorList, float _dropProbability)
    {
        // Tag
        tag = "Neutral";
        // Basic Info
        neutralType = _neutralType;
        skillInfoList = _skillInfoList;
        dropProbability = _dropProbability;
        dropWeaponList = _dropWeaponList;
        dropArmorList = _dropArmorList;
        // Position
        bornPosition = transform.position;
        // Intellegence Related Variables
        nearestChampion = null;
        watchRadius = _watchRadius;
    }

    public void SetBasicInfo(Vector2 _leftTopBorder, Vector2 _rightBottomBorder)
    {
        // Region Border
        InitRegionBorder(_leftTopBorder, _rightBottomBorder);
    }

    protected void InitRegionBorder(Vector2 _leftTopBorder, Vector2 _rightBottomBorder)
    {
        // Region Border
        leftTopBorder = _leftTopBorder;
        rightBottomBorder = _rightBottomBorder;
    }



    /************************* Update *************************/
    protected override void Update()
    {
        base.Update();
    }

    public override void LogicUpdate()
    {
        // Fist Update
        base.LogicUpdate();
        // Intellegence
        Intellengence();
    }

    protected override void SyncTimer(float dt)
    {
        base.SyncTimer(dt);
    }

    public override void SyncLastLogicFrame(float dt)
    {
        base.SyncLastLogicFrame(dt);
    }

    protected override void StartFrame()
    {
        // base
        base.StartFrame();
        // Find nearest champion
        nearestChampion = GlobalController.Instance.gameController.neutralController.FindNearestChampion(transform.position, leftTopBorder, rightBottomBorder, watchRadius);
    }

    protected override void Death()
    {
        // Drop Equipment
        DropEquipment();
        // Destroy
        Destroy(gameObject);
    }

    protected override void UpdateMove(float dt, bool logic = false)
    {
        if (stopMove)
        {
            return;
        }
        if (!UtilityTool.ObjectArrive(transform.position, targetMovePosition, targetMoveDirection))
        {

            Vector2 tmpTargetPostion = new Vector2(transform.position.x, transform.position.y) + targetMoveDirection * realAttribute.moveSpeed * dt;
            if(UtilityTool.InRegion(tmpTargetPostion, leftTopBorder, rightBottomBorder))  // if in region, move
            {
                transform.position = tmpTargetPostion;
            }
            else  // if not, come back to born position
            {
                MoveBack();
            }
        }
    }

    protected override void Animate()
    {
        // Animate Move
        AnimateMove();
    }

    /************************* Intellegence *************************/
    protected virtual void Intellengence()
    {
        // Watch Move
        WatchMove();

        // Watch Attack
        WatchAttack();
    }

    /// <summary>
    /// This is the default attack algorithm: when nearest champion is within attack range, attack him/her.
    /// </summary>
    protected virtual void WatchAttack()
    {
        if(nearestChampion != null)
        {
            if (attackReady)
            {
                float distance = (nearestChampion.transform.position - transform.position).magnitude;
                if(distance <= realAttribute.attackRange)  // if less than attack range, attack
                {
                    Attack();
                    attackReady = false;
                }
            }
        }
    }

    /// <summary>
    /// This is the default move algorithm: only follow the nearest champion, if there is none, come back to born position
    /// </summary>
    protected virtual void WatchMove()
    {
        if (nearestChampion != null)  // there is, go to him
        {
            Vector2 position = transform.position;
            targetMovePosition = nearestChampion.transform.position;
            Vector2 dis = targetMovePosition - position;
            targetMoveDirection = dis.normalized;
            targetMovePosition = position + targetMoveDirection * (dis.magnitude - realAttribute.attackRange + 0.1f);
        }
        else  // there is none, come back
        {
            MoveBack();
        }
    }

    /// <summary>
    /// Set Object Move Target as Born Position
    /// </summary>
    protected void MoveBack()
    {
        targetMovePosition = bornPosition;
        targetMoveDirection = (bornPosition - new Vector2(transform.position.x, transform.position.y)).normalized;
    }

    /************************* Attack and Skills *************************/
    /// <summary>
    /// Default attack, and target is nearestChampion
    /// </summary>
    protected virtual void Attack()
    {

    }

    protected virtual void Attack(Vector2 targetPosition)
    {

    }

    /// <summary>
    /// Default Skill, and target is nearestChampion
    /// </summary>
    protected virtual void Skill()
    {

    }

    protected virtual void Skill(Vector2 targetPosition)
    {

    }

    /************************* Damage *************************/
    public virtual void AttackDamage(FistObject fistObject)
    {
        fistObject.Damage(realAttribute.attack);
    }

    /************************* Drop Equipment *************************/
    protected virtual void DropEquipment()
    {
        if(Random.value < dropProbability)
        {
            int totalDropTypeNum = dropWeaponList.Count + dropArmorList.Count;
            int dropNo = (int)(Random.value * totalDropTypeNum);
            if(dropNo < dropWeaponList.Count)
            {
                GlobalController.Instance.gameController.propertyController.GenerateWeapon(dropWeaponList[dropNo].weaponType, dropWeaponList[dropNo].weaponNo, transform.position);
            }
            else
            {
                GlobalController.Instance.gameController.propertyController.GenerateArmor(dropArmorList[dropNo - dropWeaponList.Count], transform.position);
            }


        }
    }

}

public class NeutralSkillInfo : SkillInfo
{
    public int no;

    public NeutralSkillInfo(float _cold, int _no) : base(_cold)
    {
        no = _no;
    }
}

