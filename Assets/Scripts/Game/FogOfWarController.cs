using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarController : MonoBehaviour
{
    // Texture
    private Texture2D fogOfWar;             // texture of fog of war

    // Vision Provider
    private List<VisionProvider> visionProviders;   // vision provider list



    // Start is called before the first frame update
    void Start()
    {
        fogOfWar = GetComponent<Texture2D>();
        visionProviders = new List<VisionProvider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



}

public class VisionProvider
{
    public float range;                 // vision range
    public Vector2 position;            // provider position

    public VisionProvider(float _range, Vector2 _position)
    {
        range = _range;
        position = _position;
    }
}



