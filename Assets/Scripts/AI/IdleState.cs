﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class IdleState : IEnemyState
{
    private AIAgent agent;

    public bool enablePatrolling;
    public enum PatrolType { round,pingPong};
    public PatrolType patrolType = PatrolType.round;
    bool pingPongState = true;

    public enum Substate { idle,walkingToTarget,waitingAtNode,returningToPost};
    public Substate substate = Substate.idle;
    public float currentTimer = 0;

    public float patrolSpeed = 5;
    public float waitAtNodeTime = 1.0f;

    [Header("Looking")]
    Quaternion initialLookRotation;
    Quaternion targetRotation;
    float stayAtLookTime = 1.5f;
    float turningTime = 0.75f;
    float currentLookTime = 0;

    public PatrolNode startingPatrolNode;
    public PatrolNode currentPatrolNode;
    public List<PatrolNode> patrolNodes;


    public bool fixedPosition = false;

    public AIAgent.StateType getStateType()
    {
        return AIAgent.StateType.idle;
    }

    public void Start(AIAgent newAgent)
    {
        
    

        agent = newAgent;

        if (agent.chasing)
        {
            agent.chasing = false;
            GameLogic.instance.RemoveChaser();

        }
        currentPatrolNode = startingPatrolNode;
        agent.lastPatrolNode = currentPatrolNode;
        agent.navAgent.Stop();

        
        if (enablePatrolling)
        {
            patrolNodes = new List<PatrolNode>();
            patrolNodes.Add(startingPatrolNode);

            pingPongState = true;

            while (patrolNodes[patrolNodes.Count - 1].nextNode != null && !patrolNodes.Contains(patrolNodes[patrolNodes.Count - 1].nextNode))
            {
                patrolNodes.Add(patrolNodes[patrolNodes.Count - 1].nextNode);
            }

            if (fixedPosition == true)
            {
                SetSubstate(Substate.returningToPost);
                return;
            }

            SetNextNode(currentPatrolNode);
        }
    }

    void SetNextNode(PatrolNode node)
    {
        if (currentPatrolNode.nodeType == PatrolNode.NodeType.walkNode)
        {
            SetDestination(currentPatrolNode.transform);
        }
        else
        {
            SetSubstate(Substate.walkingToTarget);
            currentLookTime = 0;
            initialLookRotation = agent.transform.rotation;
            agent.transform.LookAt(currentPatrolNode.transform.position);
            targetRotation = agent.transform.rotation;
            agent.transform.rotation = initialLookRotation;

            agent.navAgent.Stop();
        }
    }

    void SetDestination(Transform targetNode)
    {
        agent.navAgent.SetTarget(targetNode,patrolSpeed,0.25f);
        SetSubstate(Substate.walkingToTarget);
    }

    void GetNextNode()
    {
        if(currentPatrolNode.nextNode==null && pingPongState == true)
        {
            pingPongState = false;
        }
        if(currentPatrolNode.previousNode==null && pingPongState == false)
        {
            pingPongState = true;
        }

        if (pingPongState)
        {
            currentPatrolNode = currentPatrolNode.nextNode;
        }
        else
        {
            currentPatrolNode = currentPatrolNode.previousNode;
        }

        SetNextNode(currentPatrolNode);
        
    }

    void SetSubstate(Substate newSubstate)
    {
        currentTimer = 0;
        substate = newSubstate;

        switch (substate)
        {
            case Substate.idle:
                break;
            case Substate.walkingToTarget:

                break;
            case Substate.waitingAtNode:
                break;
            case Substate.returningToPost:
                agent.navAgent.SetTarget(agent.patrolPost, patrolSpeed, 0.25f);
                break;
        }
    }

    public void UpdateState()
    {
        currentTimer += Time.deltaTime;
        switch (substate)
        {
            case Substate.idle:
                break;
            case Substate.walkingToTarget:

                if (currentPatrolNode.nodeType == PatrolNode.NodeType.lookNode)
                {
                    if (currentTimer < turningTime)
                    {
                        agent.transform.rotation = Quaternion.Lerp(initialLookRotation, targetRotation, currentTimer / turningTime);

                        agent.navAgent.threadController.moving = true;
                        agent.navAgent.threadController.speed = 0.5f;

                    }
                    else
                    {
                        agent.navAgent.threadController.moving = false;
                        agent.transform.rotation = targetRotation;
                        SetSubstate(Substate.waitingAtNode);
                    }

                }
                break;
            case Substate.waitingAtNode:
                if (currentTimer > waitAtNodeTime)
                {
                    GetNextNode();
                }
                break;
        }
    }

    public void End()
    {

    }
    public bool OnTriggerEnter(Collider other)
    {
        return false;
    }


    public bool OnLoseSight()
    {
        agent.setState(agent.seekingState);
        return false;
    }

    public bool OnSeeEnemyStart(Transform enemy)
    {
        agent.navAgent.SetTarget(enemy, 0, 0,true);
        substate = Substate.idle;
        return true;
    }

    public bool OnSeeEnemy(Transform enemy)
    {
       // agent.audioSource.PlayOneShot(AudioManager.getInstance().enemyAlert);
        agent.target = enemy.transform;
        agent.GetLastTargetPosition().position = enemy.transform.position;
       // agent.setState(agent.combatState);
        return false;
    }

    public bool OnDamage(Transform attacker)
    {
        if (attacker != null && agent.target!=null)
        {
            agent.target = attacker;
            agent.GetLastTargetPosition().position = attacker.position;
        }
        //agent.eyeController.SetDamaged();
        return false;
    }


    public bool OnKnockbackStart()
    {
        agent.character.SetState(Character.States.hitStun);
       // agent.navAgent.StopAllMovement();
        agent.setState(agent.damagedState);
        return false;
    }

    public bool OnKnockbackEnd()
    {
       // agent.eyeController.SetNormal();
        if (agent.aiSight.seenCharacter == null)
        {
            agent.setState(agent.seekingState);
        }
        else
        {
         //   agent.setState(agent.combatState);
        }
        
        return false;
    }

    public bool OnArriveAtTarget()
    {
        if (substate == Substate.returningToPost)
        {
            SetSubstate(Substate.waitingAtNode);
        }

        if (enablePatrolling)
        {
            SetSubstate(Substate.waitingAtNode);
            return true;
        }
        agent.character.SetState(Character.States.idle);
        return true;
    }

    public bool OnCompleteAction()
    {
        return false;
    }

    public bool OnDeath()
    {
        return false;
    }

    public bool OnReceiveAlert(Transform alertTarget)
    {

        return false;
    }

    public bool OnReceiveAlertStart(Transform alertTarget)
    {
        return false;
    }

    public bool OnDamagePlayer()
    {
        return false;
    }

}