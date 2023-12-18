using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAnimationComponent : MonoBehaviour
{
    Animator Animator;

    // Start is called before the first frame update
    void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
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
