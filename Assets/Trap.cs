using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OUCH");
            if (collision.transform.tag == "Enemy")
                collision.gameObject.GetComponent<enemyComponent>().health = -1;

    }
    
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("OUCH");
        if (collision.transform.tag == "Enemy")
            collision.gameObject.GetComponent<enemyComponent>().health = -1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
