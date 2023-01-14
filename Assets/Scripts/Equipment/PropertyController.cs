using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyController
{

    // Weapon Number
    public int[] weaponNum;

    // Armor Number
    public int armor1Num;

    // Property Prefab
    public GameObject propertyPrefab;

    public PropertyController()
    {
        weaponNum = new int[3];
        weaponNum[0] = 10;
        weaponNum[1] = 10;
        weaponNum[2] = 10;
        armor1Num = 10;
        propertyPrefab = (GameObject)Resources.Load("Prefabs/Game/Property/Property");
    }

    public void Initialize()
    {
        // Generate equipment
        GenerateAllEquipment();

    }

    private void GenerateAllEquipment()
    {
        // Generate weapon
        for (int i = 0; i < weaponNum.Length; i++)
        {
            for (int j = 0; j < weaponNum[i]; j++) 
            {
                while (true)
                {
                    float x = Random.value * MapController.MAP_WIDTH + MapController.LEFT_BORDER;
                    float y = Random.value * MapController.MAP_HEIGHT + MapController.BOTTOM_BORDER;
                    Vector2 position = new Vector2(x, y);
                    if (!GlobalController.Instance.gameController.mapController.InBossMap(position))
                    {
                        GenerateWeapon(WeaponInfoController.WeaponType.Low, i, position);
                        break;
                    }
                }
            }
        }
        // Generate armor
        for (int i = 0; i < armor1Num; i++)
        {
            while (true)
            {
                float x = Random.value * MapController.MAP_WIDTH + MapController.LEFT_BORDER;
                float y = Random.value * MapController.MAP_HEIGHT + MapController.BOTTOM_BORDER;
                Vector2 position = new Vector2(x, y);
                if (!GlobalController.Instance.gameController.mapController.InBossMap(position))
                {
                    GenerateArmor(0, position);
                    break;
                }
            }
        }


    }

    public void GenerateWeapon(WeaponInfoController.WeaponType weaponType, int weaponNo, Vector2 position)
    {
        GameObject prefab = GameObject.Instantiate(propertyPrefab, position, Quaternion.identity);
        prefab.GetComponent<OnGroundEquipment>().Initialize(ItemInBag.ItemType.Weapon, -1, weaponType, weaponNo, WeaponInfoController.Instance.WeaponInfoDict[weaponType][weaponNo].sprite);
    }

    public void GenerateArmor(int armorNo, Vector2 position)
    {
        GameObject prefab = GameObject.Instantiate(propertyPrefab, position, Quaternion.identity);
        prefab.GetComponent<OnGroundEquipment>().Initialize(ItemInBag.ItemType.Armor, armorNo, WeaponInfoController.WeaponType.Low, -1, ArmorInfoController.Instance.armorInfo[armorNo].sprite);
    }

}
