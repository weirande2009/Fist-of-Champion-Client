using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SorcererSkillWController : FistBase
{
    // Move
    private float speed;                // Speed of attack, times of attack in a second
    private float distance;             // Distance of attack
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

    protected override void UpdateMove(float dt, bool logic = false)
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

    public void Initialize(float _speed, float _distance, Vector2 playePosition, Vector2 mousePosition, ChampionObject _owner)
    {
        // Set Info
        owner = _owner;
        speed = _speed;
        distance = _distance;
        // Set Target Position
        float angle = UtilityTool.Angle(playePosition, mousePosition);
        targetDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        targetPosition = targetDirection * distance + playePosition;
        // Record last position
        lastSyncPosition = transform.position;
        moving = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Champion")
        {
            if (collision.transform.GetComponent<ChampionObject>() != owner)
            {
                owner.SkillWDamage(collision.transform.GetComponent<FistObject>());
                moving = false;
                GlobalController.Instance.gameController.fistManager.Destroy(this);
            }
        }
        else if (collision.transform.tag == "Neutral")
        {
            owner.SkillWDamage(collision.transform.GetComponent<FistObject>());
            moving = false;
            GlobalController.Instance.gameController.fistManager.Destroy(this);
        }
    }
}
