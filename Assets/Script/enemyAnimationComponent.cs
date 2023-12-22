using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAnimationComponent : MonoBehaviour
{
    Animator Animator;

    private enemyComponent _enemyComponent;
    
    // Start is called before the first frame update
    void Awake()
    {
        Animator = GetComponent<Animator>();
        _enemyComponent = GetComponentInParent<enemyComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_enemyComponent.health > 0 && !_enemyComponent.target)
        {
            Animator.SetBool("IsWalking", false);
        }
    }

    public void Walk()
    {
        if (_enemyComponent.health > 0)
            Animator.SetBool("IsWalking", true);
    }

    public void AttackAnimation()
    {
        Animator.SetTrigger("Attack");
    }

    public void GetHitAnimation()
    {
        Animator.SetTrigger("GetHit");
    }

    public void DeathAnimation()
    {
        Animator.SetTrigger("Death");
    }
}
