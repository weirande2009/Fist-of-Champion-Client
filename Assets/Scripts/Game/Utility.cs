using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;


public class Utility
{
    public static string SetImageToString(string imgPath)
    {
        FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
        byte[] imgByte = new byte[fs.Length];
        fs.Read(imgByte, 0, imgByte.Length);
        fs.Close();
        return Convert.ToBase64String(imgByte);
    }

    public static Texture2D GetTextureByString(string textureStr)
    {
        Texture2D tex = new Texture2D(1, 1);
        byte[] arr = Convert.FromBase64String(textureStr);
        tex.LoadImage(arr);
        tex.Apply();
        return tex;
    }
}
