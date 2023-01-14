using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInBag : MonoBehaviour
{
    // Type
    public enum ItemState
    {
        Empty = 0,
        InBag = 1,
        Armed = 2,
        InStore = 3,
    }

    public enum ItemType
    {
        Weapon = 0,
        Armor = 1,
    }

    public ItemType type;       // item type
    public ItemState state;     // item type
    public WeaponInfoController.WeaponType weaponType;
    public int weaponNo;
    public int armorNo;
    public int index;

    private Image image;
    private Button button;
    public GameObject chosenFrame;

    public void Awake()
    {
        state = ItemState.Empty;
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        button.interactable = false;
        button.onClick.AddListener(() => GameSceneController.Instance.ClickItemInBag());
        chosenFrame = transform.Find("ChosenFrame").gameObject;
        chosenFrame.SetActive(false);
    }

    public void SetInfo(ItemType _type, WeaponInfoController.WeaponType _weaponType, int _weaponNo, int _armorNo, int _index)
    {
        type = _type;
        weaponType = _weaponType;
        weaponNo = _weaponNo;
        armorNo = _armorNo;
        index = _index;
        state = ItemState.InBag;
        if (_type == ItemType.Weapon)
        {
            image.sprite = WeaponInfoController.Instance.WeaponInfoDict[_weaponType][_weaponNo].sprite;
        }
        else
        {
            image.sprite = ArmorInfoController.Instance.armorInfo[_armorNo].sprite;
        }
        button.interactable = true;
    }

    public void Clear()
    {
        state = ItemState.Empty;
        image.sprite = GameSceneController.Instance.UISprite;
        button.interactable = false;
    }
}
