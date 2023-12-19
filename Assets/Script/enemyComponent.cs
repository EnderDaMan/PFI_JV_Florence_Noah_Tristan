using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(NavMeshAgent))]

public class enemyComponent : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] enemyAnimationComponent enemyAnimationComponent;
    [SerializeField] float detectionRange = 3;
    TextMeshPro text;
    Animator skullAnimator;
    NavMeshAgent agent;
    Node root;

    [SerializeField] float health = 30;

    // Start is called before the first frame update
    void Start()
    {
        text = GameObject.FindGameObjectWithTag("killcount").GetComponent<TextMeshPro>();
        skullAnimator = GameObject.FindGameObjectWithTag("skull").GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        SetupTree();
    }

    private void SetupTree()
    {
        Node l1 = new IsWithinRange(target, transform, detectionRange);
        Node l2 = new GoToTarget(target, agent);
        Node seq1 = new Sequence(new List<Node>() { l1, l2 });
        Node l3 = new Attack(target, enemyAnimationComponent);
        Node sel2 = new Sequence(new List<Node>() { seq1, l3 });
        root = sel2; ;
    }

    // Update is called once per frame
    void Update()
    {
        root.Evaluate();
    }

    public void GetHit()
    {
        if(health <= 0)
        {
            int killCount;
            int.TryParse(text.GetParsedText(), out killCount);
            killCount++;
            text.text = killCount.ToString();
            skullAnimator.SetTrigger("Move");
            StartCoroutine(DeathCoroutine());
        }

        health -= 10;

        enemyAnimationComponent.GetHitAnimation();
    }

    IEnumerator DeathCoroutine()
    {
        enemyAnimationComponent.DeathAnimation();

        yield return new WaitForSeconds(2);

        agent.speed = 0;
        Destroy(gameObject);
    }
}
