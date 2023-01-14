using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMapController : MonoBehaviour
{
    public bool occupied;               // A player in boss region
    public int playerNo;                // Player No.
    private int bossType;               // boss type

    private List<GameObject> entranceList;      // entrance list


    // Start is called before the first frame update
    void Start()
    {
        string bossName = transform.parent.name;

        entranceList = new List<GameObject>();
        for(int i=0; i < 4; i++)
        {
            if (bossName.EndsWith("dark(Clone)"))
            {
                entranceList.Add(GameObject.Find("DarkBossEntrance" + (i + 1).ToString()));
                entranceList[i].SetActive(false);
            }
            else
            {
                entranceList.Add(GameObject.Find("LightBossEntrance" + (i + 1).ToString()));
                entranceList[i].SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Champion")
        {
            occupied = true;
            playerNo = collision.transform.GetComponent<ChampionObject>().playerNo;
            for (int i = 0; i < 4; i++)
            {
                entranceList[i].SetActive(true);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Champion")
        {
            occupied = false;
            for (int i = 0; i < 4; i++)
            {
                entranceList[i].SetActive(false);
            }
        }
    }


}
