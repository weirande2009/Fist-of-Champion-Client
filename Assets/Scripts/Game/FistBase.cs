using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the base object of all GameObject in this game
/// </summary>
public class FistBase : MonoBehaviour
{
    protected Vector3 lastSyncPosition;
    protected bool moving = false;

    /// <summary>
    /// Give Command to Object
    /// </summary>
    public virtual void LogicUpdate()
    {

    }

    protected virtual void SyncTimer(float dt)
    {

    }

    /// <summary>
    /// This function mainly sync position of object
    /// </summary>
    /// <param name="dt"></param>
    public virtual void SyncLastLogicFrame(float dt)
    {
        // Sync Position
        if (moving)
        {
            transform.position = lastSyncPosition;
            UpdateMove(dt, true);
        }
        lastSyncPosition = transform.position;
        // Sync Timer
        SyncTimer(dt);
    }

    /// <summary>
    /// Update Move of Fist
    /// </summary>
    /// <param name="logic">Whether this move is logic or animation</param>
    protected virtual void UpdateMove(float dt, bool logic=false)
    {

    }


}
