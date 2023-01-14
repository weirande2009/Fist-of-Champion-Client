using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorAnimationController : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }



    public void StartAnimation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector2 dis = new Vector2();
            dis.x = transform.position.x - hit.point.x;
            dis.y = transform.position.y - hit.point.y;
            if (dis.magnitude > 1e-4)
            {
                transform.position = hit.point;
            }
        }
        animator.SetTrigger("Clicked");
    }
}
