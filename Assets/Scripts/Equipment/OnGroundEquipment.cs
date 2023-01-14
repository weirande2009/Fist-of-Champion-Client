using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundEquipment : MonoBehaviour
{
    // Info
    public ItemInBag.ItemType type;
    public int armorType;
    public WeaponInfoController.WeaponType weaponType;
    public int weaponNo;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(ItemInBag.ItemType _type, int _armorType, WeaponInfoController.WeaponType _weaponType, int _weaponNo, Sprite sprite)
    {
        type = _type;
        armorType = _armorType;
        weaponType = _weaponType;
        weaponNo = _weaponNo;
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Champion")
        {
            PlayerItem item = new PlayerItem(type, weaponType, weaponNo, armorType);
            if (GlobalController.Instance.gameController.players[collision.transform.GetComponent<ChampionObject>().playerNo].AddPlayerItem(item))
            {
                Destroy(gameObject);
            }
        }
    }

}
