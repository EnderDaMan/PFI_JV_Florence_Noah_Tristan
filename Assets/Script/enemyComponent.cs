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

    [SerializeField] public float health = 20;

    // Start is called before the first frame update
    void Start()
    {
        if (!target && GameObject.FindGameObjectWithTag("Player"))
            target = GameObject.FindGameObjectWithTag("Player").transform;
        
        agent = GetComponent<NavMeshAgent>();
        SetupTree();

        
        if (GameObject.FindGameObjectWithTag("killcount"))
            text = GameObject.FindGameObjectWithTag("killcount").GetComponent<TextMeshPro>();
        
        if (GameObject.FindGameObjectWithTag("skull"))
            skullAnimator = GameObject.FindGameObjectWithTag("skull").GetComponent<Animator>();
        
    }

    private void SetupTree()
    {
        if (!target || !agent)
            return;
        
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
        if (root != null && health > 0)
            root.Evaluate();
        else if (root == null)
        {
            if (!target && GameObject.FindGameObjectWithTag("Player"))
                target = GameObject.FindGameObjectWithTag("Player").transform;
            
            SetupTree();
        }
    }

    public void GetHit()
    {
        health -= 10;

        enemyAnimationComponent.GetHitAnimation();

        if(health <= 0)
        {
            int killCount;
            
            if (text && skullAnimator)
            {
                int.TryParse(text.GetParsedText(), out killCount);
                killCount++;
                text.text = killCount.ToString();
                skullAnimator.SetTrigger("Move");
            }
            
            StartCoroutine(DeathCoroutine());
        }


    }

    IEnumerator DeathCoroutine()
    {
        GetComponent<BoxCollider>().enabled = false;
        
        enemyAnimationComponent.DeathAnimation();

        yield return new WaitForSeconds(2);

        agent.speed = 0;
        Destroy(gameObject);
    }
}
