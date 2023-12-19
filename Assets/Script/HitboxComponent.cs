using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxComponent : MonoBehaviour
{
    private playerMoveComponent _playerMoveComponent;
    
    private void Start()
    {
        _playerMoveComponent = GetComponentInParent<playerMoveComponent>();
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(_playerMoveComponent.isAttacking)
        {
            if (collision.transform.tag == "Enemy")
                collision.gameObject.GetComponent<enemyComponent>().GetHit();
        }
    }
}
