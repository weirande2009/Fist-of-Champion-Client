using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnranceController : MonoBehaviour
{
    public int playerNo;            // PlayerNo


    // Start is called before the first frame update
    void Start()
    {
        playerNo = -1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Champion")
        {
            if(collision.transform.GetComponent<ChampionObject>().playerNo == playerNo)
            {
                gameObject.SetActive(false);
            }
        }
    }

}
