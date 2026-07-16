using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawn
{
    public float delayOnBegin = 1;      //delay for first enemy
    public GameObject[] enemy;    //enemy spawned
    public int numberWaves = 5;     //the number of enemy need spawned
    [Range(1, 5)]
    public int spawmAmountEnemyTogether = 1;   //how many enemy can spawn at the same time together
    [Range(0.5f, 10)]
    public float waveRate = 1;  //time delay spawn next enemy
}