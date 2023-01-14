using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using ChampionFistGame;

public class UtilityTool
{

    public static bool ObjectArrive(Vector2 currentPosition, Vector2 targetPosition, Vector2 targetDirection)
    {
        Vector2 dis = targetPosition - currentPosition;
        if(dis.x * targetDirection.x > 0 && dis.y * targetDirection.y > 0)
        {
            return false;
        }
        return true;
    }

    public static bool InRegion(Vector2 currentPosition, Vector2 leftTopBorder, Vector2 rightBottomBorder)
    {
        if(currentPosition.x > leftTopBorder.x && currentPosition.y < leftTopBorder.y && currentPosition.x < rightBottomBorder.x && currentPosition.y > rightBottomBorder.y)
        {
            return true;
        }
        return false;
    }

    public static float Angle(Vector2 startPosition, Vector2 endPosition)
    {
        Vector2 dis = endPosition - startPosition;
        float angle;
        if (dis.x > 0)
        {
            angle = Mathf.Atan((endPosition.y - startPosition.y) / (endPosition.x - startPosition.x));
        }
        else
        {
            angle = Mathf.Atan((endPosition.y - startPosition.y) / (endPosition.x - startPosition.x)) + Mathf.PI;
        }
        return angle;
    }

    public static XmlNode GetXmlRoot(string path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        xmlDoc.LoadXml(textAsset.text);
        // Get root node
        return xmlDoc.DocumentElement;
    }

    public static List<OperationFrame> GetOperationFrameList(Google.Protobuf.Collections.RepeatedField<OperationFrame> operationFrames)
    {
        List<OperationFrame> list = new List<OperationFrame>();
        for(int i = 0; i < operationFrames.Count; i++)
        {
            list.Add(operationFrames[i]);
        }
        return list;
    }

}
