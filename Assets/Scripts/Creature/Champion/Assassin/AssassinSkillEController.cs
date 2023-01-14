using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinSkillEController : FistBase
{
    // Move
    private float speed;                // Speed of attack, times of attack in a second
    private float radius;               // Radius of attack
    private float angleRange;           // Angle range
    private Vector2 targetPosition;     // Target position
    private Vector2 targetDirection;    // Target direction
    private ChampionObject owner;       // Owner No.


    // Start is called before the first frame update
    void Start()
    {
        
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

    protected override void UpdateMove(float dt, bool logic=false)
    {
        
        Vector2 currentPosition = transform.position;

        if (!UtilityTool.ObjectArrive(currentPosition, targetPosition, targetDirection))
        {
            Vector3 moveVector = (targetPosition - currentPosition).normalized * speed * dt;
            moveVector.z = 0;
            transform.position += moveVector;
        }
        else
        {
            if (logic)
            {
                moving = false;
                GlobalController.Instance.gameController.fistManager.Destroy(this);
            }
        }
    }

    public void Initiailize(float _speed, float _radius, Vector2 playePosition, Vector2 mousePosition, float no, ChampionObject _owner)
    {
        // Set Info
        owner = _owner;
        speed = _speed;
        radius = _radius;
        angleRange = 45 * Mathf.PI / 180;
        // Set Target Position
        float angle = UtilityTool.Angle(playePosition, mousePosition);
        if(no == 0)
        {
            angle += 0.5f * angleRange;
        }
        else if(no == 2)
        {
            angle -= 0.5f * angleRange;
        }
        Vector2 tmpPosition = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        targetPosition = tmpPosition + playePosition;
        targetDirection = tmpPosition.normalized;
        // Record last position
        lastSyncPosition = transform.position;
        moving = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Champion")
        {
            if(collision.transform.GetComponent<ChampionObject>() != owner)
            {
                owner.SkillEDamage(collision.transform.GetComponent<FistObject>());
            }
        }
        else if (collision.transform.tag == "Neutral")
        {
            owner.SkillEDamage(collision.transform.GetComponent<FistObject>());
        }
    }
}
