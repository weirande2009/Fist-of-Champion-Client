using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer
{
    // Player Info
    public PlayerInfo playerInfo;

    // State
    public bool dead;

    // Champion
    public ChampionObject champion;

    // Bag
    public List<PlayerItem> items;

    public GamePlayer(string _playerName, bool _mainPlayer, ChampionObject.ChampionType _championType, ChampionObject _champion)
    {
        playerInfo = new PlayerInfo(_championType, _mainPlayer, _playerName);
        playerInfo.online = true;
        champion = _champion;
        if (_mainPlayer)
        {
            champion.SetMainPlayer();
        }
        items = new List<PlayerItem>();
        dead = false;
    }

    /// <summary>
    /// Add item to player bag
    /// </summary>
    /// <param name="item"></param>
    /// <returns>if no room, return false, else true</returns>
    public bool AddPlayerItem(PlayerItem item)
    {
        if (items.Count >= GameController.MAX_ITEM_NUMBER_IN_BAG)
        {
            return false;
        }
        // If has this armor, destroy it
        if(item.type == ItemInBag.ItemType.Armor)
        {
            if (HasArmor(item.armorNo))
            {
                return true;
            }
        }
        items.Add(item);
        return true;
    }

    public void TakeOffEquipment(ItemInBag.ItemType itemType)
    {
        if (itemType == ItemInBag.ItemType.Weapon && champion.weapon != null)
        {
            PlayerItem item = new PlayerItem(ItemInBag.ItemType.Weapon, champion.weapon.type, champion.weapon.weaponNo, -1);
            items.Add(item);
            champion.weapon = null;
        }
        else if (itemType == ItemInBag.ItemType.Armor && champion.armor != null)
        {
            PlayerItem item = new PlayerItem(ItemInBag.ItemType.Armor, WeaponInfoController.WeaponType.Low, -1, champion.armor.armorNo);
            items.Add(item);
            champion.armor = null;
        }
    }

    public bool HasArmor(int armorNo)
    {
        // Find in bag
        foreach(PlayerItem item in items)
        {
            if(item.type == ItemInBag.ItemType.Armor)
            {
                if(item.armorNo == armorNo)
                {
                    return true;
                }
            }
        }
        // Find in armed armor
        if(champion.armor != null)
        {
            if(champion.armor.armorNo == armorNo)
            {
                return true;
            }
        }
        return false;
    }

    public void SetOffline()
    {
        lock (playerInfo.onlineLock)
        {
            playerInfo.onlineChanged = true;
            playerInfo.online = false;
        }
    }
}

public class PlayerItem
{
    public ItemInBag.ItemType type;
    public WeaponInfoController.WeaponType weaponType;
    public int weaponNo;
    public int armorNo;
    
    public PlayerItem(ItemInBag.ItemType _type, WeaponInfoController.WeaponType _weaponType, int _weaponNo, int _armorNo)
    {
        type = _type;
        weaponType = _weaponType;
        weaponNo = _weaponNo;
        armorNo = _armorNo;
    }
}

public class PlayerInfo
{
    public ChampionObject.ChampionType championType;
    public bool mainPlayer;
    public string playerName;
    public bool online;
    public bool onlineChanged;
    public object onlineLock;

    public PlayerInfo(ChampionObject.ChampionType _championType, bool _mainPlayer, string _playerName)
    {
        championType = _championType;
        mainPlayer = _mainPlayer;
        playerName = _playerName;
        online = false;
        onlineChanged = false;
        onlineLock = new object();
    }
}
