using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : FistBase
{
    // Move
    protected float speed;                // Speed of attack
    protected float distance;             // Distance of attack
    protected Vector2 startPosition;      // Start position
    protected Vector2 targetPosition;     // Target position
    protected Vector2 targetDirection;    // Target direction

    // Owner
    protected ChampionObject ownerChampion;

    // Start is called before the first frame update
    protected virtual void  Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
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

        if (!UtilityTool.ObjectArrive(transform.position, targetPosition, targetDirection))
        {
            Vector3 tmpPosition = targetDirection * dt / speed * distance;
            transform.position += tmpPosition;
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

    public virtual void Initialize(float _distance, float _speed, Vector3 playerPosition, Vector2 mousePosition, ChampionObject _ownerChampion, bool defaultMove=true)
    {
        // Set Info
        ownerChampion = _ownerChampion;
        distance = _distance;
        speed = _speed;
        // Record last position
        lastSyncPosition = transform.position;
        moving = true;
        // Set movement
        if (defaultMove)
        {
            DefaultSetMovement(playerPosition, mousePosition);
        }
    }

    /// <summary>
    /// Default method of movement
    /// </summary>
    private void DefaultSetMovement(Vector3 playerPosition, Vector2 mousePosition)
    {
        // Set position
        float angle = UtilityTool.Angle(playerPosition, mousePosition);
        Vector3 tmpPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
        //transform.position = tmpPosition + playerPosition;
        transform.rotation = Quaternion.Euler(0, 0, angle * 180 / Mathf.PI);
        // Record
        startPosition = transform.position;
        targetDirection = tmpPosition.normalized;
        targetPosition = startPosition + targetDirection * distance;
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Champion")
        {
            ChampionObject collisionObject = collision.transform.GetComponent<ChampionObject>();
            if (collisionObject != ownerChampion)
            {
                ownerChampion.AttackDamage(collisionObject);
            }
        }
        else if (collision.transform.tag == "Neutral")
        {
            ownerChampion.AttackDamage(collision.transform.GetComponent<FistObject>());
        }
    }

}
