using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
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
                collision.gameObject.GetComponent<enemyComponent>().Death();

    }
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "Enemy")
            collision.gameObject.GetComponent<enemyComponent>().Death();

        if (collision.transform.tag == "Player")
        {
            collision.gameObject.GetComponent<playerMoveComponent>().GetHit(50f); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
