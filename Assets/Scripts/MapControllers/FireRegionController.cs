using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRegionController : FistBase
{
    // Attribute
    private static float attackRate;
    private static float damage;

    public static void Initialize()
    {
        attackRate = 1;
        damage = 20;
    }

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
        foreach (var championInfo in attackList)
        {
            if (championInfo.durationTimer > attackRate)
            {
                championInfo.champion.Damage(damage);
                championInfo.durationTimer = 0;
            }
            championInfo.durationTimer += dt;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Champion")
        {
            ChampionObject championObject = collision.transform.GetComponent<ChampionObject>();
            attackList.Add(new DurationChampionInfo(championObject));
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Champion")
        {
            ChampionObject championObject = collision.transform.GetComponent<ChampionObject>();
            for (int i = 0; i < attackList.Count; i++)
            {
                if (attackList[i].champion == championObject)
                {
                    attackList.RemoveAt(i);
                }
            }
        }
    }





}
