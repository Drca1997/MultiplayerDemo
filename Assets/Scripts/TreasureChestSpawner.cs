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
    List<bool> occupiedSpawnPoints;
    public static TreasureChestSpawner Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsHost) { return; }
        SpawnTreasureChests(possibleSpawnPositions.Count);
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
                int n = Random.Range(0, possibleSpawnPositions.Count);
                if (!occupiedSpawnPoints[n])
                {
                    Transform spawnedChest = Instantiate(treasurePrefab, possibleSpawnPositions[n], Quaternion.identity, null);
                    spawnedChest.GetComponent<NetworkObject>().Spawn(true);
                    valid = true;
                    occupiedSpawnPoints[n] = true;
                }
            }
        }
    }
}
