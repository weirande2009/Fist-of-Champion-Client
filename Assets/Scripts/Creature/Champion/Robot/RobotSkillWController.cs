using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSkillWController : FistBase
{
    private float distance;             // Distance
    private float duration;             // Duration
    private float range;                // Range
    private float startAttackAngle;     // Radius of attack
    private float startAngle;           // Start angle
    private float preAngle;             // Present angle
    private ChampionObject owner;       // Owner No.
    private bool attacking;
    private List<FistObject> attackList;


    // Start is called before the first frame update
    void Awake()
    {
        attacking = false;
        attackList = new List<FistObject>();
    }

    private void Update()
    {
        UpdateMove(Time.deltaTime);
    }

    public override void LogicUpdate()
    {

    }

    public override void SyncLastLogicFrame(float dt)
    {
        base.SyncLastLogicFrame(dt);
    }

    protected override void UpdateMove(float dt, bool logic = false)
    {
        if(preAngle <= startAngle - Mathf.PI / 2)
        {
            if (logic)
            {
                moving = false;
                GlobalController.Instance.gameController.fistManager.Destroy(this);
            }
        }
        else
        {
            if(preAngle <= startAngle - startAttackAngle)
            {
                attacking = true;
            }
            preAngle -= dt / duration * range;
            transform.rotation = Quaternion.Euler(0, 0, preAngle * 180 / Mathf.PI);
        }
    }

    public void Initialize(float _distance, float _range, float _duration, float _startAttackAngle, Vector2 playerPosition, Vector2 mousePosition, ChampionObject _owner)
    {
        // Set Info
        distance = _distance;
        range = _range;
        owner = _owner;
        duration = _duration;
        startAttackAngle = _startAttackAngle;
        // Set Start
        float angle = UtilityTool.Angle(playerPosition, mousePosition);
        Vector3 tmpPosition = new Vector3(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance, 0);
        transform.position += tmpPosition;
        transform.rotation = Quaternion.Euler(0, 0, (angle + range) * 180 / Mathf.PI);
        startAngle = angle + range;
        preAngle = startAngle;
        // Record last position
        lastSyncPosition = transform.position;
        moving = true;
    }

    private void Attack(FistObject fistObject)
    {
        if (attackList.Contains(fistObject))
        {
            return;
        }
        owner.SkillWDamage(fistObject);
        attackList.Add(fistObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (attacking)
        {
            if (collision.transform.tag == "Champion")
            {
                if (collision.transform.GetComponent<ChampionObject>() != owner)
                {
                    Attack(collision.transform.GetComponent<FistObject>());
                }
            }
            else if (collision.transform.tag == "Neutral")
            {
                Attack(collision.transform.GetComponent<FistObject>());
            }
        }
    }
}
