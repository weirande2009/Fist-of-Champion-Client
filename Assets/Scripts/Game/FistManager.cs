using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChampionFistGame;

public class FistManager
{
    private class FistItem
    {
        public bool destroyed;
        public FistBase fist;
        public FistItem(FistBase _fist)
        {
            fist = _fist;
            destroyed = false;
        }
    }

    private List<FistItem> fistItemList;
    private List<ChampionObject> championObjectList;

    public FistManager()
    {
        fistItemList = new List<FistItem>();
        championObjectList = new List<ChampionObject>();
    }

    public Transform Instantiate(Transform prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        Transform newObject = Object.Instantiate(prefab, position, rotation, parent);
        // Check whether is champion
        ChampionObject championObject = newObject.GetComponent<ChampionObject>();
        FistBase fistBase = newObject.GetComponent<FistBase>();
        if (championObject != null)
        {
            championObjectList.Add(championObject);
        }
        else if (fistBase != null)
        {
            fistItemList.Add(new FistItem(fistBase));
        }
        else
        {
            Object.Destroy(newObject.gameObject);
        }
        return newObject;
    }

    public Transform Instantiate(Transform prefab, Vector3 position, Quaternion rotation)
    {
        Transform newObject = Object.Instantiate(prefab, position, rotation);
        // Check whether is champion
        ChampionObject championObject = newObject.GetComponent<ChampionObject>();
        FistBase fistBase = newObject.GetComponent<FistBase>();
        if (championObject != null)
        {
            championObjectList.Add(championObject);
        }
        else if (fistBase != null)
        {
            fistItemList.Add(new FistItem(fistBase));
        }
        else
        {
            Object.Destroy(newObject.gameObject);
        }
        return newObject;
    }

    public void Destroy(FistBase fist)
    {
        if (fist != null)
        {
            foreach (var f in fistItemList)
            {
                if (f.fist == fist)
                {
                    f.destroyed = true;
                    Object.Destroy(f.fist.gameObject);
                }
            }
        }
        else
        {
            Debug.Log("Destroy null Object");
            return;
        }
    }

    public void ClearDestroyedObject()
    {
        for (int i = 0; i < fistItemList.Count; i++)
        {
            if (fistItemList[i].fist == null && !fistItemList[i].destroyed)
            {
                Debug.Log(fistItemList[i].fist);
            }
            if (fistItemList[i].destroyed || fistItemList[i].fist is null)
            {
                fistItemList.RemoveAt(i);
                i--;
            }
        }
    }

    /// <summary>
    /// Register only for FistBase Object
    /// </summary>
    /// <param name="gameObject"></param>
    public void Register(FistBase fist)
    {
        foreach (var f in fistItemList)
        {
            if (f.fist == fist)
            {
                return;
            }
        }
        fistItemList.Add(new FistItem(fist));
    }

    public void SyncLastFrame(List<OperationFrame> lastFrameOperation)
    {
        if (lastFrameOperation.Count != championObjectList.Count)
        {
            Debug.Log("英雄数量出现问题");
            Application.Quit();
        }
        // Sync Champion
        for (int i = 0; i < championObjectList.Count; i++)
        {
            championObjectList[i].SyncLastLogicFrame(GameController.FRAME_INTERVAL, new ChampionOperation(lastFrameOperation[i]));
        }
        // Sync Other
        foreach (var f in fistItemList)
        {
            if(f.fist != null)
            {
                f.fist.SyncLastLogicFrame(GameController.FRAME_INTERVAL);
            }
            else
            {
                f.destroyed = true;
            }
        }
    }

    public void HandleLogicFrame(List<OperationFrame> lastFrameOperation)
    {
        if (lastFrameOperation.Count != championObjectList.Count)
        {
            Debug.Log("英雄数量出现问题");
            Application.Quit();
        }
        // Champion
        for (int i = 0; i < championObjectList.Count; i++)
        {
            championObjectList[i].LogicUpdate(new ChampionOperation(lastFrameOperation[i]));
        }
        // Other
        for (int i = 0; i < fistItemList.Count; i++)
        {
            if (fistItemList[i].fist != null)
            {
                fistItemList[i].fist.LogicUpdate();
            }
            else
            {
                fistItemList[i].destroyed = true;
            }
            
        }
    }

    public void DetroyAll()
    {
        // destroy all items and neutral 
        foreach(var f in fistItemList)
        {
            Destroy(f.fist);
        }
        ClearDestroyedObject();

        // destroy all champions
        for(int i=0; i<championObjectList.Count; i++)
        {
            Object.Destroy(championObjectList[i].gameObject);
        }

    }

}
