using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    [SerializeField] private Transform [] spawnPoints;

    public Transform[] SpawnPointsArray { get => spawnPoints; }

}
