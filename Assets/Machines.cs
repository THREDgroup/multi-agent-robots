using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class Machines : MonoBehaviour
{
    //Instantiate machine transforms
    public Transform Target1;
    public Transform Target2;
    public Transform Target3;
    public Transform Target4;
    public Transform Target5;
    public Transform Target6;
    public Transform Target7;

    //Access to robot agents to end their training episode when jobs at all machines are completed
    public MobileRobotAgent robot;
    public MobileRobotAgent robot2;
    public MobileRobotAgent robot3;

    //Machine states, 0 = 'Completed processing' state, 1 = 'Processing' state
    public int[] Flags = {0, 0, 0, 0, 0, 0, 0};
    
    //Store time when processing starts, uncomment for repeated cycle testing
    //public int[] Temp = {100, 100, 100, 100, 100, 100, 100};
    //Processing time, uncomment when testing repeated cycles
    //public int[] Process = {0, 0, 0, 0, 0, 0, 0};

    //Color for completed processing state
    public Color Green = new Color(0f, 1f, 0f, 1f);

    //Color for processing state
    public Color Red = new Color(1f, 0f, 0f, 1f);

    //Called on episode beginning to reset all machines
    public void Start()
    {
        Flags[0] = 0;
        Flags[1] = 0;
        Flags[2] = 0;
        Flags[3] = 0;
        Flags[4] = 0;
        Flags[5] = 0;
        Flags[6] = 0;  
        Target1.GetComponent<MeshRenderer>().material.color = Green;
        Target1.tag = "Machine A";
        Target2.GetComponent<MeshRenderer>().material.color = Green;
        Target2.tag = "Machine B";
        Target3.GetComponent<MeshRenderer>().material.color = Green;
        Target3.tag = "Machine C";
        Target4.GetComponent<MeshRenderer>().material.color = Green;
        Target4.tag = "Machine D";
        Target5.GetComponent<MeshRenderer>().material.color = Green;
        Target5.tag = "Machine E";
        Target6.GetComponent<MeshRenderer>().material.color = Green;
        Target6.tag = "Machine F";
        Target7.GetComponent<MeshRenderer>().material.color = Green;
        Target7.tag = "Machine G";
    }

    public void Check()
    {
        if (Flags[0] == 1 && Flags[1] == 1 && Flags[2] == 1 && Flags[3] == 1 && Flags[4] == 1 && Flags[5] == 1 && Flags[6] == 1) //If all jobs are assigned,
        {
            UnityEngine.Debug.Log("All Targets Met");
            EndAll();//End episodes
        }
    }

    //Uncomment for repeated cycle testing
    /*void Update()
    {
        if ((int)Time.time == Temp[0] + Process[0]) //If processing time is over, switch state
        {
            Flags[0] = 0;
            Target1.GetComponent<MeshRenderer>().material.color = Green;
            Target1.tag = "Machine A";
        }

        if ((int)Time.time == Temp[1] + Process[1])
        {
            Flags[1] = 0;
            Target2.GetComponent<MeshRenderer>().material.color = Green;
            Target2.tag = "Machine B";
        }

        if ((int)Time.time == Temp[2] + Process[2])
        {
            Flags[2] = 0;
            Target3.GetComponent<MeshRenderer>().material.color = Green;
            Target3.tag = "Machine C";
        }

        if ((int)Time.time == Temp[3] + Process[3])
        {
            Flags[3] = 0;
            Target4.GetComponent<MeshRenderer>().material.color = Green;
            Target4.tag = "Machine D";
        }

        if ((int)Time.time == Temp[4] + Process[4])
        {
            Flags[4] = 0;
            Target5.GetComponent<MeshRenderer>().material.color = Green;
            Target5.tag = "Machine E";
        }

        if ((int)Time.time == Temp[5] + Process[5])
        {
            Flags[5] = 0;
            Target6.GetComponent<MeshRenderer>().material.color = Green;
            Target6.tag = "Machine F";
        }

        if ((int)Time.time == Temp[6] + Process[6])
        {
            Flags[6] = 0;
            Target7.GetComponent<MeshRenderer>().material.color = Green;
            Target7.tag = "Machine G";
        }

        //Machine stuck in processing state
        if ((int)Time.time > 48)
        {
            Flags[4] = 1;
            Target5.GetComponent<MeshRenderer>().material.color = Red;
            Target5.tag = "Null";
        }
    }*/

    //End episodes for all agents
    public void EndAll()
    {
        robot.End();
        robot2.End();
        robot3.End();
    }
}