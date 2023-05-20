using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(SpawnPoints))]
public class SpawnTreasureChests : NetworkBehaviour
{
    [SerializeField] private Transform treasurePrefab;
    [Range(0, 2)]
    [SerializeField]
    private int treasuresMax;

    public override void OnNetworkSpawn()
    {
        if (!IsHost) { return; }

        Transform [] spawnPoints = GetComponent<SpawnPoints>().SpawnPointsArray;
        List<Transform> possibleSpawnPoints = GetArrayShallowCopy(spawnPoints); 
        int treasureCount = Random.Range(0, treasuresMax + 1);
        for (int i = 0; i < treasureCount; i++)
        {
            int spawnPointIndex = Random.Range(0, possibleSpawnPoints.Count);
            TreasureChestManager.Instance.SpawnTreasureChest(possibleSpawnPoints[spawnPointIndex]);
            possibleSpawnPoints.RemoveAt(spawnPointIndex);
        }
    }

    private List<Transform> GetArrayShallowCopy(Transform[]array )
    {
        List<Transform> shallowCopy = new List<Transform>();
        foreach (Transform t in array)
        {
            shallowCopy.Add(Instantiate(t, t.position, t.rotation));
        }
        return shallowCopy;
    }
}
