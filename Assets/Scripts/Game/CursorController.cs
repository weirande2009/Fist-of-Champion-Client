using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    private GameObject cursorAnimationPrefab;

    private CursorAnimationController cursorAnimationController;

    // Start is called before the first frame update
    void Awake()
    {
        cursorAnimationPrefab = (GameObject)Resources.Load("Prefabs/Game/Other/CursorAnimation");
        cursorAnimationController = Instantiate(cursorAnimationPrefab).GetComponent<CursorAnimationController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            cursorAnimationController.transform.position = transform.position;
            cursorAnimationController.StartAnimation();
        }

    }
}
