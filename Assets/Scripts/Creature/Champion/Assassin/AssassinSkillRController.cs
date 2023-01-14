using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinSkillRController : FistBase
{
    private float attackRate;           // Rate of attack
    private float radius;               // Radius of attack
    private float duration;             // Duration
    private ChampionObject owner;       // Owner No

    // Attack
    private List<DurationChampionInfo> attackList;


    // Start is called before the first frame update
    void Awake()
    {
        attackList = new List<DurationChampionInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void LogicUpdate()
    {
        
    }

    protected override void SyncTimer(float dt)
    {
        UpdateAttack(dt);
        UpdateDuration(dt);
    }

    private void UpdateDuration(float dt)
    {
        // Set Time
        duration -= dt;
        if (duration <= 0)
        {
            GlobalController.Instance.gameController.fistManager.Destroy(this);
        }
    }

    private void UpdateAttack(float dt)
    {
        // Watch inside players
        foreach (DurationChampionInfo championInfo in attackList)
        {
            if (championInfo.durationTimer > attackRate)
            {
                // Damage
                owner.SkillRDamage(championInfo.champion);
                championInfo.durationTimer = 0;
            }
            // Set Timer
            championInfo.durationTimer += dt;
        }
    }


    public void SetInfo(float _duration, float _attackRate, ChampionObject _owner)
    {
        // Set Info
        owner = _owner;
        duration = _duration;
        attackRate = _attackRate;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only process champion
        if (collision.transform.tag == "Champion")
        {
            // Add champion to attack List
            ChampionObject championObject = collision.transform.GetComponent<ChampionObject>();
            if(championObject != owner)
            {
                attackList.Add(new DurationChampionInfo(championObject));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Only process champion
        if (collision.transform.tag == "Champion")
        {
            // Remove champion from attack List
            ChampionObject championObject = collision.transform.GetComponent<ChampionObject>();
            if (championObject != owner)
            {
                for (int i = 0; i < attackList.Count; i++)
                {
                    if (attackList[i].champion == championObject)
                    {
                        attackList.RemoveAt(i);
                        i--;
                    }
                }
            }
                
        }
    }
}

