using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Movement1 : MonoBehaviour
{
    public float speed;
    private bool isCgasing;

    private Rigidbody2D rb;
    private Transform player;

    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isCgasing == true)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(player == null)
            {
                player = collision.transform;
            }
            isCgasing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            rb.velocity = Vector2.zero;
            isCgasing = false;
        }
    }
}
