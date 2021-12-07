using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public TrailRenderer trail;

    private void Awake()
    {
        trail = GetComponent<TrailRenderer>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
            trail.enabled = false;
        }

        if (other.gameObject.layer == 13)
        {
            Destroy(gameObject);
            trail.enabled = false;

        }
   
    }
    
}
