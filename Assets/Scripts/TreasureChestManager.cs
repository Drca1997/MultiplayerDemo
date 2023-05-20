using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class TreasureChestManager : NetworkBehaviour
{
    [SerializeField] Transform treasurePrefab;
    private List<TreasureChest> spawnedTreasureChests;

    public static TreasureChestManager Instance { get; private set; }
    public List<TreasureChest> SpawnedTreasureChests { get => spawnedTreasureChests; set => spawnedTreasureChests = value; }

    private void Awake()
    {
        Instance = this;
        spawnedTreasureChests = new List<TreasureChest>();
    }

    private void Start()
    {

    }

    public override void OnNetworkSpawn()
    {
    }

    public void SpawnTreasureChest(Transform spawnPoint)
    {
        Transform spawnedChest = Instantiate(treasurePrefab, spawnPoint.position, spawnPoint.rotation, null);
        spawnedChest.GetComponent<NetworkObject>().Spawn(true);
        spawnedTreasureChests.Add(spawnedChest.GetComponent<TreasureChest>());
        Debug.Log(spawnedTreasureChests.Count);
    }
}
