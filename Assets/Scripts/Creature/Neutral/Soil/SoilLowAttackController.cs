using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilLowAttackController : FistBase
{
    private NeutralObject owner;
    private float speed;
    private Vector2 targetPosition;
    private Vector2 targetDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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

    public void Initialize(Vector2 startPosition, Vector2 endPosition, float radius, float _speed, NeutralObject _owner)
    {
        // Set Basic Info
        owner = _owner;
        speed = _speed;
        // Set Start Postion
        transform.position = startPosition;
        // Set Rotation
        float angle = UtilityTool.Angle(startPosition, endPosition);
        transform.rotation = Quaternion.Euler(0, 0, angle * 180 / Mathf.PI);  // Here rotation need degree, not radian
        // Set Target Position
        Vector2 tmpPosition = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        targetPosition = tmpPosition + startPosition;
        targetDirection = tmpPosition.normalized;
        // Set Last Frame Position
        lastSyncPosition = transform.position;
        moving = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Champion")
        {
            owner.AttackDamage(collision.transform.GetComponent<FistObject>());
            GlobalController.Instance.gameController.fistManager.Destroy(this);
        }
    }

}
