using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinAttackController : AttackController
{
    // Move
    private float angleRange;       // Angle of attack range
    private float radius;           // Radius of attack
    private float preAngle;         // Present angle
    private float startAngle;       // Start angle
    private Vector2 playePosition;  // Player position

    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
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
        preAngle -= angleRange * speed * dt;
        if (preAngle < startAngle - angleRange)
        {
            if (logic)
            {
                moving = false;
                GlobalController.Instance.gameController.fistManager.Destroy(this);
            }
            return;
        }
        Vector2 tmpPosition = new Vector2(Mathf.Cos(preAngle) * radius, Mathf.Sin(preAngle) * radius);
        transform.position = tmpPosition + playePosition;
    }

    public void Initialize(float _radius, float _speed, Vector2 _playePosition, Vector2 mousePosition, ChampionObject _ownerChampion)
    {
        // Set Info
        ownerChampion = _ownerChampion;
        radius = _radius;
        speed = _speed;
        playePosition = _playePosition;
        angleRange = 45 * 3.14f / 180;
        // Set position
        startAngle = UtilityTool.Angle(playePosition, mousePosition) + 0.5f * angleRange;
        preAngle = startAngle;
        Vector2 tmpPosition = new Vector2(Mathf.Cos(startAngle) * radius, Mathf.Sin(startAngle) * radius);
        transform.position = tmpPosition + playePosition;
        // Record last position
        lastSyncPosition = transform.position;
        moving = true;
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Champion")
        {
            ChampionObject collisionObject = collision.transform.GetComponent<ChampionObject>();
            if(collisionObject != ownerChampion)
            {
                ownerChampion.AttackDamage(collisionObject);
            }
        }
        else if(collision.transform.tag == "Neutral")
        {
            ownerChampion.AttackDamage(collision.transform.GetComponent<FistObject>());
        }
    }

}
