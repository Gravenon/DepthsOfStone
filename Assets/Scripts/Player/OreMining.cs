using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class OreMining : MonoBehaviour
{
    public float interactRange = 1.5f;
    public LayerMask resourceLayer;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryMineUnderMouse();
        }
    }

    void TryMineUnderMouse()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D hit = Physics2D.OverlapPoint(mousePosition, resourceLayer);
        if (hit != null)
        {
            float distance = Vector2.Distance(hit.ClosestPoint(transform.position), transform.position);

            if (distance <= interactRange)
            {
                Ore ore = hit.GetComponent<Ore>();
                if (ore == null)
                {
                    ore = hit.gameObject.AddComponent<Ore>();
                }
                ore.Mine();
            }
        }

    }
}
