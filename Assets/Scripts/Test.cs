using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;


public class Test : MonoBehaviour
{

    public GameObject square;

    // Start is called before the first frame update
    void Start()
    {
        square.transform.position = Vector3.zero;
        square.transform.localScale = new Vector3(100, 1, 1);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
