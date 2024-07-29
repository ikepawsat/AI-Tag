using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetGame : MonoBehaviour
{
    public Vector3 playerTeleportDestination;
    public Vector3 enemyTeleportDestination;
    private EnemyTracker et;
    private PlayerTracker pt;
    private NN neuralNetwork;
    private Movement move;
    public float timer = 21f; //public so can pass into NN, ended up taking it out, may put back later though thats why its still public

    void Start()
    {
        et = FindObjectOfType<EnemyTracker>();
        pt = FindObjectOfType<PlayerTracker>();
        neuralNetwork = FindObjectOfType<NN>();
        move = FindObjectOfType<Movement>();
        playerTeleportDestination = new Vector3(-8f, 1f, -6f);
        enemyTeleportDestination = new Vector3(10f, 1f, 12f);
        et.runs = 1;
        pt.runs = 1;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            et.Reward(5);
            pt.Reward(-5);
            resetScene();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            Debug.Log("Player was tagged");
            et.Reward(-10);
            pt.Reward(10);
            resetScene();
        }
    }

    private void resetScene()
    {
        TeleportObjects(playerTeleportDestination, enemyTeleportDestination);
        timer = 21f;
        move.Move(0,0);
        et.runs += 0.5; //adds twice, could figure out why... but I could just do 0.5, I think it is a unity thing
        pt.runs += 0.5;
    }

    void TeleportObjects(Vector3 destinationPlayer, Vector3 destinationEnemy)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = destinationPlayer;
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        enemy.transform.position = destinationEnemy;
    }
}
