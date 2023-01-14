using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon
{
    // Basic Attribute
    public float attack;
    public float attackSpeed;
    public float moveSpeed;
    // Type
    public WeaponInfoController.WeaponType type;
    public int weaponNo;
    public int changeAttributeTag;          // Tag for change attribute
    // Effect object
    public enum EffectObject
    {
        None = 0,
        Target = 1,
        Self = 2,
    }
    public EffectObject effectObject;
    // Effect Stage
    public enum EffectStage
    {
        Attack = 0,
        Damage = 1,
    }

    public Weapon(WeaponInfoController.WeaponType _type, int _weaponNo, float _attack, float _attackSpeed, float _moveSpeed, EffectObject _effectObject)
    {
        type = _type;
        weaponNo = _weaponNo;
        attack = _attack;
        attackSpeed = _attackSpeed;
        moveSpeed = _moveSpeed;
        effectObject = _effectObject;
        changeAttributeTag = ChangeAttibuteRecord.GetNewTag(ChangeAttibuteRecord.SourceType.Weapon);
    }

    /// <summary>
    /// Spell effect on a fist object which may be self or enemy
    /// </summary>
    /// <param name="fistObject"></param>
    public virtual void Effect(FistObject fistObject)
    {

    }

    /// <summary>
    /// Attack a fist object, must be an enemy
    /// </summary>
    /// <param name="fistObject"></param>
    public virtual void Attack(FistObject fistObject, float damage)
    {
        fistObject.Damage(damage);
    }

    public virtual void Update(float dt)
    {

    }

}

/****************************************** Low Level Weapon ******************************************/

public class LowWeapon1 : Weapon
{
    private static WeaponInfo weaponInfo = WeaponInfoController.Instance.WeaponInfoDict[WeaponInfoController.WeaponType.Low][0];

    public LowWeapon1() : base(weaponInfo.weaponBasicInfo.weaponType, weaponInfo.weaponBasicInfo.weaponNo, weaponInfo.attack, weaponInfo.attackSpeed, weaponInfo.moveSpeed, EffectObject.None)
    {
        
    }

    public override void Effect(FistObject fistObject)
    {
        
    }
}

public class LowWeapon2 : Weapon
{
    private static WeaponInfo weaponInfo = WeaponInfoController.Instance.WeaponInfoDict[WeaponInfoController.WeaponType.Low][1];

    public LowWeapon2() : base(weaponInfo.weaponBasicInfo.weaponType, weaponInfo.weaponBasicInfo.weaponNo, weaponInfo.attack, weaponInfo.attackSpeed, weaponInfo.moveSpeed, EffectObject.None)
    {

    }

    public override void Effect(FistObject fistObject)
    {

    }
}

public class LowWeapon3 : Weapon
{
    private static WeaponInfo weaponInfo = WeaponInfoController.Instance.WeaponInfoDict[WeaponInfoController.WeaponType.Low][2];

    public LowWeapon3() : base(weaponInfo.weaponBasicInfo.weaponType, weaponInfo.weaponBasicInfo.weaponNo, weaponInfo.attack, weaponInfo.attackSpeed, weaponInfo.moveSpeed, EffectObject.None)
    {

    }

    public override void Effect(FistObject fistObject)
    {

    }
}

/****************************************** Middle Level Weapon ******************************************/

public class MiddleWeapon1 : Weapon
{
    private static WeaponInfo weaponInfo = WeaponInfoController.Instance.WeaponInfoDict[WeaponInfoController.WeaponType.Middle][0];

    // Variables
    private float increaseAttackSpeed = 20;
    private float increaseAttackSpeedDuration = 2;


    public MiddleWeapon1() : base(weaponInfo.weaponBasicInfo.weaponType, weaponInfo.weaponBasicInfo.weaponNo, weaponInfo.attack, weaponInfo.attackSpeed, weaponInfo.moveSpeed, EffectObject.None)
    {
        effectObject = EffectObject.Self;
    }

    public override void Effect(FistObject fistObject)
    {
        fistObject.ChangeAttribute(FistObject.AttributeType.AttackSpeed, ChangeAttibuteRecord.SourceType.Weapon, changeAttributeTag, increaseAttackSpeed, increaseAttackSpeedDuration);
    }

}

public class MiddleWeapon2 : Weapon
{
    private static WeaponInfo weaponInfo = WeaponInfoController.Instance.WeaponInfoDict[WeaponInfoController.WeaponType.Middle][1];

    // Variables
    private float decreaseMoveSpeed = -15;
    private float decreaseMoveSpeedDuration = 1;

    public MiddleWeapon2() : base(weaponInfo.weaponBasicInfo.weaponType, weaponInfo.weaponBasicInfo.weaponNo, weaponInfo.attack, weaponInfo.attackSpeed, weaponInfo.moveSpeed, EffectObject.None)
    {
        effectObject = EffectObject.Target;
    }

    public override void Effect(FistObject fistObject)
    {
        fistObject.ChangeAttribute(FistObject.AttributeType.MoveSpeed, ChangeAttibuteRecord.SourceType.Weapon, changeAttributeTag, decreaseMoveSpeed, decreaseMoveSpeedDuration);
    }
}

public class MiddleWeapon3 : Weapon
{
    private static WeaponInfo weaponInfo = WeaponInfoController.Instance.WeaponInfoDict[WeaponInfoController.WeaponType.Middle][2];

    public MiddleWeapon3() : base(weaponInfo.weaponBasicInfo.weaponType, weaponInfo.weaponBasicInfo.weaponNo, weaponInfo.attack, weaponInfo.attackSpeed, weaponInfo.moveSpeed, EffectObject.None)
    {

    }

    public override void Effect(FistObject fistObject)
    {

    }
}

public class MiddleWeapon4 : Weapon
{
    private static WeaponInfo weaponInfo = WeaponInfoController.Instance.WeaponInfoDict[WeaponInfoController.WeaponType.Middle][3];

    // Variables
    private float decreaseMoveSpeed = -30;
    private float decreaseMoveSpeedDuration = 2;

    public MiddleWeapon4() : base(weaponInfo.weaponBasicInfo.weaponType, weaponInfo.weaponBasicInfo.weaponNo, weaponInfo.attack, weaponInfo.attackSpeed, weaponInfo.moveSpeed, EffectObject.None)
    {
        effectObject = EffectObject.Self;
    }

    public override void Effect(FistObject fistObject)
    {
        fistObject.ChangeAttribute(FistObject.AttributeType.MoveSpeed, ChangeAttibuteRecord.SourceType.Weapon, changeAttributeTag, decreaseMoveSpeed, decreaseMoveSpeedDuration);
    }
}

public class MiddleWeapon5 : Weapon
{
    private static WeaponInfo weaponInfo = WeaponInfoController.Instance.WeaponInfoDict[WeaponInfoController.WeaponType.Middle][4];

    // Variables
    private float increaseMoveSpeed = 15;
    private float increaseMoveSpeedDuration = 2;

    public MiddleWeapon5() : base(weaponInfo.weaponBasicInfo.weaponType, weaponInfo.weaponBasicInfo.weaponNo, weaponInfo.attack, weaponInfo.attackSpeed, weaponInfo.moveSpeed, EffectObject.None)
    {
        effectObject = EffectObject.Self;
    }

    public override void Effect(FistObject fistObject)
    {
        fistObject.ChangeAttribute(FistObject.AttributeType.MoveSpeed, ChangeAttibuteRecord.SourceType.Weapon, changeAttributeTag, increaseMoveSpeed, increaseMoveSpeedDuration);
    }
}

/****************************************** High Level Weapon ******************************************/

public class HighWeapon1 : Weapon
{
    private static WeaponInfo weaponInfo = WeaponInfoController.Instance.WeaponInfoDict[WeaponInfoController.WeaponType.High][0];

    private float increaseDamageTimes = 0.3f;

    public HighWeapon1() : base(weaponInfo.weaponBasicInfo.weaponType, weaponInfo.weaponBasicInfo.weaponNo, weaponInfo.attack, weaponInfo.attackSpeed, weaponInfo.moveSpeed, EffectObject.None)
    {

    }

    public override void Effect(FistObject fistObject)
    {

    }

    public override void Attack(FistObject fistObject, float damage)
    {
        fistObject.Damage(damage * (1 + increaseDamageTimes));
    }

}

public class HighWeapon2 : Weapon
{
    private static WeaponInfo weaponInfo = WeaponInfoController.Instance.WeaponInfoDict[WeaponInfoController.WeaponType.High][1];
    private static int MAX_LAYER = 6;

    // Variables
    private float increaseAttackSpeed = 10;
    private float increaseAttackSpeedDuration = 2;
    private int layer;
    private float increaseAttackSpeedTimer;

    public HighWeapon2() : base(weaponInfo.weaponBasicInfo.weaponType, weaponInfo.weaponBasicInfo.weaponNo, weaponInfo.attack, weaponInfo.attackSpeed, weaponInfo.moveSpeed, EffectObject.None)
    {
        layer = 0;
        increaseAttackSpeedTimer = 0;
        effectObject = EffectObject.Self;
    }
    
    public override void Effect(FistObject fistObject)
    {
        layer = Mathf.Clamp(++layer, 1, MAX_LAYER);
        fistObject.ChangeAttribute(FistObject.AttributeType.AttackSpeed, ChangeAttibuteRecord.SourceType.Weapon, changeAttributeTag, increaseAttackSpeed * layer, increaseAttackSpeedDuration);
    }

    public override void Update(float dt)
    {
        if(layer > 0)
        {
            if (increaseAttackSpeedTimer > increaseAttackSpeedDuration)
            {
                layer = 0;
                increaseAttackSpeedTimer = 0;
            }
            increaseAttackSpeedTimer += dt;
        }
    }
}

public class HighWeapon3 : Weapon
{
    private static WeaponInfo weaponInfo = WeaponInfoController.Instance.WeaponInfoDict[WeaponInfoController.WeaponType.High][2];

    private float decreaseArmor = 30;

    public HighWeapon3() : base(weaponInfo.weaponBasicInfo.weaponType, weaponInfo.weaponBasicInfo.weaponNo, weaponInfo.attack, weaponInfo.attackSpeed, weaponInfo.moveSpeed, EffectObject.None)
    {

    }

    public override void Effect(FistObject fistObject)
    {

    }

    public override void Attack(FistObject fistObject, float damage)
    {
        fistObject.Damage(damage, decreaseArmor);
    }
}

public class HighWeapon4 : Weapon
{
    private static WeaponInfo weaponInfo = WeaponInfoController.Instance.WeaponInfoDict[WeaponInfoController.WeaponType.High][3];

    private float decreaseMoveSpeed = -30;
    private float decreaseDefense = -20;
    private float decreaseDuration = 1;

    public HighWeapon4() : base(weaponInfo.weaponBasicInfo.weaponType, weaponInfo.weaponBasicInfo.weaponNo, weaponInfo.attack, weaponInfo.attackSpeed, weaponInfo.moveSpeed, EffectObject.None)
    {
        effectObject = EffectObject.Target;
    }

    public override void Effect(FistObject fistObject)
    {
        fistObject.ChangeAttribute(FistObject.AttributeType.MoveSpeed, ChangeAttibuteRecord.SourceType.Weapon, changeAttributeTag, decreaseMoveSpeed, decreaseDuration);
        fistObject.ChangeAttribute(FistObject.AttributeType.Defense, ChangeAttibuteRecord.SourceType.Weapon, changeAttributeTag, decreaseDefense, decreaseDuration);
    }
}

public class HighWeapon5 : Weapon
{
    private static WeaponInfo weaponInfo = WeaponInfoController.Instance.WeaponInfoDict[WeaponInfoController.WeaponType.High][4];

    private float increaseDefense = 10;
    private float increaseAttackSpeed = 10;
    private float increaseMoveSpeed = 10;
    private float increaseDuration = 1;


    public HighWeapon5() : base(weaponInfo.weaponBasicInfo.weaponType, weaponInfo.weaponBasicInfo.weaponNo, weaponInfo.attack, weaponInfo.attackSpeed, weaponInfo.moveSpeed, EffectObject.None)
    {
        effectObject = EffectObject.Self;
    }

    public override void Effect(FistObject fistObject)
    {
        fistObject.ChangeAttribute(FistObject.AttributeType.Defense, ChangeAttibuteRecord.SourceType.Weapon, changeAttributeTag, increaseDefense, increaseDuration);
        fistObject.ChangeAttribute(FistObject.AttributeType.AttackSpeed, ChangeAttibuteRecord.SourceType.Weapon, changeAttributeTag, increaseAttackSpeed, increaseDuration);
        fistObject.ChangeAttribute(FistObject.AttributeType.MoveSpeed, ChangeAttibuteRecord.SourceType.Weapon, changeAttributeTag, increaseMoveSpeed, increaseDuration);
    }
}




