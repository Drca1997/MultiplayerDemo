using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] mapChunks;
    [SerializeField] private FinalTreasureSpawner finalTreasureManager;

    public static MapGenerator Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateScenario()
    {
        foreach(GameObject chunk in mapChunks)
        {
            chunk.GetComponent<SpawnSmallRooms>().SpawnSmallRoom();
        }
        finalTreasureManager.SpawnFinalTreasure();

    }
}
