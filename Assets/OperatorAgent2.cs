using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OperatorAgent2 : MonoBehaviour
{
    public MobileRobotAgent robot;
    public MobileRobotAgent robot2;
    public MobileRobotAgent robot3;
    public Transform Target1;
    public Transform Target2;
    public Transform Target3;
    public Transform Target4;
    public Transform Target5;
    public Transform Target6;
    public Transform Target7;

    //Called on start of episode
    public void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.Warp(new Vector3(UnityEngine.Random.Range(-13f, 13f), 0.125f, UnityEngine.Random.Range(-13f, 13f)));//Spawn at a random location
        agent.destination = agent.transform.position;
        agent.destination = Target4.position;//Set fourth machine
    }

    //Loop operator destinations and trigger robot collision penalization
    void OnTriggerEnter(Collider collision)
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (collision.gameObject.name == "Target 4")
        {
            agent.destination = Target5.position;
        }

        if (collision.gameObject.name == "Target 5")
        {
            agent.destination = Target6.position;
        }

        if (collision.gameObject.name == "Target 6")
        {
            agent.destination = Target4.position;
        }

        if (collision.gameObject.name == "Mobile Robot 1")
        {
            robot.OpCollision();
        }

        if (collision.gameObject.name == "Mobile Robot 2")
        {
            robot2.OpCollision();
        }

        if (collision.gameObject.name == "Mobile Robot 3")
        {
            robot3.OpCollision();
        }
    }
}
