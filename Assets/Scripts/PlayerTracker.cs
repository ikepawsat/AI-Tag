using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerTracker : MonoBehaviour
{
    public Transform playerTransform; //Look at EnemyTracker, I built that first and modified this to work for other.
    private Movement movement;
    private NN neuralNetwork;
    private ResetGame game;
    private RayTracing rayTracing;

    public double reward;
    public double pastReward;
    public double distance;
    private double average;
    private double max = 0;
    public double runs;

    void Start()
    {
        movement = GetComponent<Movement>();
        neuralNetwork = GetComponent<NN>();
        game = GetComponent<ResetGame>();
        rayTracing = GetComponent<RayTracing>();
        Application.targetFrameRate = 60;
    }

    public void Update()
    {
        Vector3 enemyPosition = transform.position;
        Vector3 playerPosition = playerTransform.position;
        Vector3 relativePlayerPosition = playerPosition - enemyPosition;

        float northDistance = rayTracing.RaycastDirection(Vector3.forward);
        float southDistance = rayTracing.RaycastDirection(Vector3.back);
        float eastDistance = rayTracing.RaycastDirection(Vector3.right);
        float westDistance = rayTracing.RaycastDirection(Vector3.left);

        float[] inputs = {
            relativePlayerPosition.x, 
            relativePlayerPosition.y, 
            relativePlayerPosition.z,
            northDistance,
            southDistance,
            eastDistance,
            westDistance
        };
        float[] output = neuralNetwork.Brain(inputs);

        MakeDecision(output);
        Reward(0);
    }

    public void Reward(double change)
    {
        Vector3 enemyPosition = transform.position;
        Vector3 playerPosition = playerTransform.position;
        Vector3 relativePlayerPosition = playerPosition - enemyPosition;

        distance = Math.Sqrt(relativePlayerPosition.x * relativePlayerPosition.x
                                   + relativePlayerPosition.y * relativePlayerPosition.y
                                   + relativePlayerPosition.z * relativePlayerPosition.z);

        reward += 1 / (distance + 1); // Positve for being closer because trying to tag, + 1 makes sure doesnt go to infinity
        reward += change;

        if (game.timer <= 0.0f)
        {
            average = (average * (runs - 1)) + reward;
            average /= runs;

            if (reward > max) //this is more important for enemy than player
            {
                max = reward;
                neuralNetwork.MaxControl(true); //Dont need to copy because will copy later because will be > pastreward if max
            }
            else if (runs > 1000 && reward < average * 0.25)
            {
                Debug.Log("Enemy Reset to Max!");
                neuralNetwork.MaxControl(false); //set it back to the max NN here
                runs = 1; //reset runs counter so it doesnt just keep reseting it, gives it chance to find a new path
                average = reward;
                pastReward = reward;
                return; //need to break out of these if statements because then below could run.
            }

            if (reward < 0)
            {
                pastReward = reward;
                reward = 0;
                neuralNetwork.MutateNetwork(0.3f, 0.66f); //heavier mutations because needs to change a lot
            }
            else if (reward > pastReward)
            {
                pastReward = reward;
                reward = 0;
                neuralNetwork.CopyToNewGen(); //if its not broken don't fix it right?
            }
            else
            {
                pastReward = reward;
                reward = 0;
                neuralNetwork.MutateNetwork(0.1f, 0.5f); //lower mutations because only minor needed
            }
            Debug.Log("Enemy average: " + average + " Enemy max: " + max);
            Debug.Log("--------------------------------------");
        }
        else if (game.timer > 0.0f && change == 10) //change lets it not change every time, and know if theres a tag will be 10 so
        {
            average = (average * (runs - 1)) + reward;
            average /= runs;

            if (reward > max)
            {
                max = reward;
                neuralNetwork.MaxControl(true); //Dont need to copy because will copy later because will be > pastreward if max
            }
            pastReward = reward;
            reward = 0;
            neuralNetwork.CopyToNewGen(); //if its not broken don't fix it right?

            Debug.Log("Enemy average: " + average + " Enemy max: " + max);
            Debug.Log("--------------------------------------");
        }
    }

    void MakeDecision(float[] output)
    {
        float forwardOutput = output[0];
        float backOutput = output[1];
        float leftOutput = output[2];
        float rightOutput = output[3];
        float jumpOutput = output[4];

        float highestOutput = Mathf.Max(forwardOutput, backOutput, leftOutput, rightOutput, jumpOutput);

        if (highestOutput == forwardOutput)
        {
            movement.Move(1, 0);
        }
        else if (highestOutput == backOutput)
        {
            movement.Move(-1, 0);
        }
        else if (highestOutput == leftOutput)
        {
            movement.Move(0, -1);
        }
        else if (highestOutput == rightOutput)
        {
            movement.Move(0, 1);
        }
        else if (highestOutput == jumpOutput)
        {
            movement.Jump();
        }
    }
}
