using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class TreasureChestSpawner : NetworkBehaviour
{
    [SerializeField] Transform treasurePrefab;
    [SerializeField] List<Vector3> possibleSpawnPositions;
    private List<TreasureChest> spawnedTreasureChests; 
    List<bool> occupiedSpawnPoints;
    [SerializeField] private int treasuresToSpawn;
    
    public static TreasureChestSpawner Instance { get; private set; }
    public List<TreasureChest> SpawnedTreasureChests { get => spawnedTreasureChests;  }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        spawnedTreasureChests = new List<TreasureChest>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsHost) { return; }
        SpawnTreasureChests(treasuresToSpawn);
    }

    public void SpawnTreasureChests(int numChests)
    {
        Assert.IsTrue(numChests <= possibleSpawnPositions.Count);
        occupiedSpawnPoints = Enumerable.Repeat(false, possibleSpawnPositions.Count).ToList();
        for (int i = 0; i < numChests; i++)
        {
            bool valid = false;
            while (!valid)
            {
                int n = UnityEngine.Random.Range(0, possibleSpawnPositions.Count);
                if (!occupiedSpawnPoints[n])
                {
                    Transform spawnedChest = Instantiate(treasurePrefab, possibleSpawnPositions[n], Quaternion.identity, null);
                    spawnedChest.GetComponent<NetworkObject>().Spawn(true);
                    valid = true;
                    occupiedSpawnPoints[n] = true;
                    spawnedTreasureChests.Add(spawnedChest.GetComponent<TreasureChest>());
                }
            }
        }
    }
    
    

    
}
