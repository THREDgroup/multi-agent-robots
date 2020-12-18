using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Collections.Specialized;
using System.CodeDom;
using System.Diagnostics;
using System;

public class MobileRobotAgent : Agent //Derived from Agent class
{
    Rigidbody rBody; //Rigid body movement

    //Access to communication information from other agents and triggers to start/end episode
    public Machines Master;
    public OperatorAgent1 Operator1;
    public OperatorAgent2 Operator2;

    public int k; //Identifier for machine
    public int c = 0; //Action count
    public int e = 0; //Episode count
    public int r_count = 1; //Job count

    public Color Red = new Color(1f, 0f, 0f, 1f); // Chnage machine color to red when job is scheduled at a machine

    //Start game
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    //Start episode
    public override void OnEpisodeBegin()
    {
        this.r_count = 1; //Reset reward count
        this.e = e + 1; //Count episode
        this.c = 0; //Reset action count

        //Initialize robot to random location with 0 velocity
        this.transform.localPosition = new Vector3(UnityEngine.Random.Range(-13f, 13f), 0.125f, UnityEngine.Random.Range(-13f, 13f));
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = new Vector3(0f, 0f, 0f);

        //Start all machines
        Master.Start();

        //Start all operators
        Operator1.Start();
        Operator2.Start();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Self location and velocity
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);

        //Other robots' location and velocity
        if (this.name == "Mobile Robot 1")
        {
            sensor.AddObservation(Master.robot2.transform.localPosition);
            sensor.AddObservation(Master.robot2.rBody.velocity.x);
            sensor.AddObservation(Master.robot2.rBody.velocity.z);
            sensor.AddObservation(Master.robot3.transform.localPosition);
            sensor.AddObservation(Master.robot3.rBody.velocity.x);
            sensor.AddObservation(Master.robot3.rBody.velocity.z);
        }

        else if (this.name == "Mobile Robot 2")
        {
            sensor.AddObservation(Master.robot3.transform.localPosition);
            sensor.AddObservation(Master.robot3.rBody.velocity.x);
            sensor.AddObservation(Master.robot3.rBody.velocity.z);
            sensor.AddObservation(Master.robot.transform.localPosition);
            sensor.AddObservation(Master.robot.rBody.velocity.x);
            sensor.AddObservation(Master.robot.rBody.velocity.z);
        }

        else
        {
            sensor.AddObservation(Master.robot.transform.localPosition);
            sensor.AddObservation(Master.robot.rBody.velocity.x);
            sensor.AddObservation(Master.robot.rBody.velocity.z);
            sensor.AddObservation(Master.robot2.transform.localPosition);
            sensor.AddObservation(Master.robot2.rBody.velocity.x);
            sensor.AddObservation(Master.robot2.rBody.velocity.z);
        }

        //Machine states
        sensor.AddObservation(Master.Flags[0]);
        sensor.AddObservation(Master.Flags[1]);
        sensor.AddObservation(Master.Flags[2]);
        sensor.AddObservation(Master.Flags[3]);
        sensor.AddObservation(Master.Flags[4]);
        sensor.AddObservation(Master.Flags[5]);
        sensor.AddObservation(Master.Flags[6]);

        //Perception Sensor Observations are not included in this script but are attached separately to the agent. Check inspector in Unity for more details.
    }

    //Discrete Action Space
    public override void OnActionReceived(float[] vectorAction)
    {
        var movement = (int)vectorAction[0];

        //Uncomment this loop for testing, robot stuck in halt state
        //if ((int)Time.time > 96 && this.name == "Mobile Robot 2")
        //{
        //    this.rBody.velocity = (new Vector3(0f, 0f, 0f));
        //}

        //else

        //{
            //Penalize movement actions to find shortest path - penalization is low intially to prevent discouragement in exploration
            if (movement == 0)
            {
                this.rBody.velocity = (new Vector3(5f, 0f, 0f));
                if (this.e < 700)
                {
                    AddReward(-0.0001f * this.e / 700);
                }
                else
                {
                    AddReward(-0.0001f);
                }
            }

            else if (movement == 1)
            {
                this.rBody.velocity = (new Vector3(-5f, 0f, 0f));
                if (this.e < 700)
                {
                    AddReward(-0.0001f * this.e / 700);
                }
                else
                {
                    AddReward(-0.0001f);
                }
            }

            else if (movement == 2)
            {
                this.rBody.velocity = (new Vector3(0f, 0f, 5f));
                if (this.e < 700)
                {
                    AddReward(-0.0001f * this.e / 700);
                }
                else
                {
                    AddReward(-0.0001f);
                }
            }

            else if (movement == 3)
            {
                this.rBody.velocity = (new Vector3(0f, 0f, -5f));

                if (this.e < 700)
                {
                    AddReward(-0.0001f * this.e / 700);
                }
                else
                {
                    AddReward(-0.0001f);
                }
            }

            else if (movement == 4)
            {
                this.rBody.velocity = (new Vector3(0f, 0f, 0f));
            }
        //}

        c = c + 1; //Increment Action Count

        //Action count limit, comment for repeated cycle testing
        if (c == 3072)
        {
        UnityEngine.Debug.Log("Action Limit");
        Master.EndAll();
        }
    }

    //Simplified modelling of all interactions as collisions
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Floor")
        {
            if (collision.gameObject.name.Contains("Target"))
            {
                string s = collision.gameObject.name;
                string subs = s.Substring(s.Length - 1);
                k = Int32.Parse(subs) - 1; //Extract machine identifier

                if (Master.Flags[k] == 0) //If machine is not processing
                {
                    //Change machine color, status to processing, tag to Null, add and count reward
                    collision.gameObject.GetComponent<MeshRenderer>().material.color = Red;
                    collision.gameObject.tag = "Null";
                    Master.Flags[k] = 1;
                    AddReward(0.33f * this.r_count); //Envourage subsequent jobs
                    this.r_count = this.r_count + 1;

                    //Store time at which processing starts, uncomment for repeated cycle testing
                    //Master.Temp[k] = (int)Time.time;

                    //Assign processing time with delay, uncomment for repeated cycle testing
                    //Master.Process[k] = (int)UnityEngine.Random.Range(7.01f, 12.99f);

                    //Print schedules, uncomment for repeated cycle testing
                    //UnityEngine.Debug.Log("Schedule: " + this.name + ", Machine " + (k+1) + ", (t,delta t) = (" + (int)Time.time + "," + Master.Process[k] + ")");
                    Master.Check(); //Check if all jobs are completed, comment for repeated cycle testing
                }

                //Negative reward if navigating to processing machine
                if (Master.Flags[k] == 1)
                {
                    AddReward(-0.1f);
                }
            }

            //Negative rewards for obstacle collisions
            else if (collision.gameObject.tag == "Walls")
            {
                UnityEngine.Debug.Log("Wall Hit");
                AddReward(-0.0625f);

            }

            else if (collision.gameObject.tag == "Barrier")
            {
                UnityEngine.Debug.Log("Barrier Hit");
                AddReward(-0.0625f);

            }

            else if (collision.gameObject.tag == "Agent")
            {
                UnityEngine.Debug.Log("Agent collision");
                AddReward(-0.1f);
            }
        }
    }

    //Negative reward if operator is hit
    public void OpCollision()
    {
        AddReward(-0.1f);
        UnityEngine.Debug.Log("Operator Collision");
    }

    //End episode
    public void End()
    {
        EndEpisode();
    }

    //Play manually using W, A, S, D
    public override void Heuristic(float[] actionsOut)
    {
        if (Input.GetKey(KeyCode.A))
        {
            actionsOut[0] = 1;
        }

        else if (Input.GetKey(KeyCode.D))
        {
            actionsOut[0] = 0;
        }

        else if (Input.GetKey(KeyCode.W))
        {
            actionsOut[0] = 2;
        }

        else if (Input.GetKey(KeyCode.S))
        {
            actionsOut[0] = 3;
        }

        else
        {
            actionsOut[0] = 4;
        }
    }
}