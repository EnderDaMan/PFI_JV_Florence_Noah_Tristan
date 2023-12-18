
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public enum NodeState { Running, Success, Failure }
public abstract class Node
{
    protected NodeState state { get; set; }
    public Node parent = null;
    protected List<Node> children = new();

    Dictionary<string, object> data = new Dictionary<string, object>();

    public void SetData(string key, object value)
    {
        data[key] = value;
    }

    public object GetData(string key)
    {
        if (data.TryGetValue(key, out object value))
            return value;
        if (parent != null)
            return parent.GetData(key);

        return null;
    }

    public bool RemoveData(string key)
    {
        if (data.Remove(key))
            return true;

        if (parent != null)
            return parent.RemoveData(key);

        return false;
    }

    public Node()
    {
        state = NodeState.Running;
        parent = null;
    }

    public Node(List<Node> pChildren)
    {
        state = NodeState.Running;
        parent = null;
        foreach (Node child in pChildren)
            Attach(child);
    }

    protected void Attach(Node n)
    {
        children.Add(n);
        n.parent = this;
    }
    public abstract NodeState Evaluate();
}

public class Selector : Node
{
    public Selector(List<Node> n) : base(n) { }

    public override NodeState Evaluate()
    {
        foreach (Node n in children)
        {
            NodeState localstate = n.Evaluate();
            switch (localstate)
            {
                case NodeState.Failure:
                    continue;
                case NodeState.Success:
                case NodeState.Running:
                    state = localstate;
                    return state;
            }
        }
        state = NodeState.Failure;
        return state;
    }
}

public class Sequence : Node
{
    public Sequence(List<Node> n) : base(n) { }

    public override NodeState Evaluate()
    {
        foreach (Node n in children)
        {

            NodeState localstate = n.Evaluate();
            switch (localstate)
            {
                case NodeState.Success:
                    continue;
                case NodeState.Failure:
                case NodeState.Running:
                    state = localstate;
                    return state;
            }
        }
        state = NodeState.Success;
        return state;
    }
}

public class Inverter : Node
{
    public Inverter(Node n) : base()
    {
        Attach(n);
    }

    public override NodeState Evaluate()
    {
        switch (children[0].Evaluate())
        {
            case NodeState.Success:
                state = NodeState.Failure;
                break;
            case NodeState.Failure:
                state = NodeState.Success;
                break;
            case NodeState.Running:
                state = NodeState.Running;
                break;
        }
        return state;
    }
}

public class GoToTarget : Node
{
    Transform target;
    NavMeshAgent agent;

    public GoToTarget(Transform target, NavMeshAgent agent)
    {
        this.target = target;
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        var playerComponent = target.gameObject.GetComponent<playerMoveComponent>();
        if (agent.transform.position.x > target.position.x && playerComponent.isOnEdgeRight ||
            agent.transform.position.x < target.position.x && playerComponent.isOnEdgeLeft)
        {
            agent.destination = target.position;
            agent.speed = 2.5f;
        }
        else if (agent.transform.position.x > target.position.x && playerComponent.isOnEdgeLeft ||
            agent.transform.position.x < target.position.x && playerComponent.isOnEdgeRight)
        {
            agent.destination = new Vector3(-(target.position.x + 8), target.position.y, target.position.z);
            agent.speed = 1f;
        }
        else
        {
            agent.destination = target.position;
            agent.speed = 1.5f;
        }

        if (agent.transform.position.x > target.position.x)
            agent.transform.rotation = new Quaternion(0, 180, 0, 0);
        else
            agent.transform.rotation = new Quaternion(0, 0, 0, 0);

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            state = NodeState.Success;
            return state;
        }

        state = NodeState.Running;
        return state;
    }
}

public class IsWithinRange : Node
{
    Transform target;
    Transform self;
    float detectionRange;

    public IsWithinRange(Transform target, Transform self, float detectionRange)
    {
        this.target = target;
        this.self = self;
        this.detectionRange = detectionRange;
    }

    public override NodeState Evaluate()
    {
        state = NodeState.Failure;
        if (Vector3.Distance(self.position, target.position) <= detectionRange)
            state = NodeState.Success;

        return state;
    }
}

public class Attack : Node
{
    Transform target;
    enemyAnimationComponent enemyAnimationComponent;

    public Attack(Transform target, enemyAnimationComponent enemyAnimationComponent)
    {
        this.target = target;
        this.enemyAnimationComponent = enemyAnimationComponent;
    }

    public override NodeState Evaluate()
    {
        Debug.Log(1);
        state = NodeState.Success;

        enemyAnimationComponent.AttackAnimation();

        target.GetComponent<playerMoveComponent>().GetHit();

        return state;
    }
}