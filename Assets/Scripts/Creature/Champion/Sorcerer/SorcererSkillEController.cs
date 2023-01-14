using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SorcererSkillEController : FistBase
{
    private float duration;             // Duration
    private float timer;
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

    public override void SyncLastLogicFrame(float dt)
    {
        base.SyncLastLogicFrame(dt);
    }

    protected override void SyncTimer(float dt)
    {
        if (timer > duration)
        {
            GlobalController.Instance.gameController.fistManager.Destroy(this);
        }
        else
        {
            timer += dt;
        }

    }

    public void Initialize(float _duration, Vector2 playerPosition, Vector2 mousePosition, ChampionObject _owner)
    {
        // Set Info
        owner = _owner;
        duration = _duration;
        timer = 0;
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
        owner.SkillEDamage(fistObject);
        attackList.Add(fistObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
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
