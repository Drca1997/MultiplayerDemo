using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FinalTreasureSpawner : NetworkBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject finalTreasurePrefab;

    // Start is called before the first frame update
    public void SpawnFinalTreasure()
    {
        if (!IsServer) { return; }

        GameObject finalTreasureObject = Instantiate(finalTreasurePrefab, spawnPoint.position, spawnPoint.rotation);
        finalTreasureObject.GetComponent<NetworkObject>().Spawn(true);
    }
}
