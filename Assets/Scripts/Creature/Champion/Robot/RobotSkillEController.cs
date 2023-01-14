using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSkillEController : FistBase
{
    private float maxDistance;          // Distance
    private float duration;             // Duration
    private float timer;                // Timer
    private ChampionObject owner;       // Owner No.
    private List<FistObject> attackList;


    // Start is called before the first frame update
    void Awake()
    {
        attackList = new List<FistObject>();
    }

    private void Update()
    {
        
    }

    public override void LogicUpdate()
    {

    }

    public override void SyncLastLogicFrame(float dt)
    {
        base.SyncLastLogicFrame(dt);
    }

    protected override void SyncTimer(float dt)
    {
        if(timer > duration)
        {
            GlobalController.Instance.gameController.fistManager.Destroy(this);
        }
        else 
        {
            timer += dt;
        }

    }

    public void Initialize(float _maxDistance, float _duration, Vector2 playerPosition, Vector2 mousePosition, ChampionObject _owner)
    {
        // Set Info
        maxDistance = _maxDistance;
        duration = _duration;
        owner = _owner;
        timer = 0;
        // Set Start
        float angle = UtilityTool.Angle(playerPosition, mousePosition);
        float distance;
        if((mousePosition - playerPosition).magnitude > maxDistance)
        {
            distance = maxDistance;
        }
        else
        {
            distance = (mousePosition - playerPosition).magnitude;
        }
        Vector3 tmpPosition = new Vector3(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance, 0);
        transform.position += tmpPosition;
    }

    private void Attack(FistObject fistObject)
    {
        if (attackList.Contains(fistObject))
        {
            return;
        }
        owner.SkillEDamage(fistObject);
        attackList.Add(fistObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
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
