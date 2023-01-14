using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindMapController : FistBase
{
    // Static
    private static int changeAttributeTag;
    private static float moveSpeedUpRatio;     // Speed up ratio
    private static float increaseRate;

    public static void Initialize()
    {
        changeAttributeTag = ChangeAttibuteRecord.GetNewTag(ChangeAttibuteRecord.SourceType.Map);
        moveSpeedUpRatio = 5.3f;
        increaseRate = 1;
    }

    private List<DurationChampionInfo> insideList;


    // Start is called before the first frame update
    void Awake()
    {
        insideList = new List<DurationChampionInfo>();
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
        foreach (var championInfo in insideList)
        {
            if (championInfo.durationTimer > increaseRate)
            {
                championInfo.champion.ChangeAttribute(FistObject.AttributeType.MoveSpeed, ChangeAttibuteRecord.SourceType.Map, changeAttributeTag, moveSpeedUpRatio, increaseRate);
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
            insideList.Add(new DurationChampionInfo(championObject));
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Champion")
        {
            ChampionObject championObject = collision.transform.GetComponent<ChampionObject>();
            for (int i = 0; i < insideList.Count; i++)
            {
                if (insideList[i].champion == championObject)
                {
                    insideList.RemoveAt(i);
                }
            }
        }
    }


}
