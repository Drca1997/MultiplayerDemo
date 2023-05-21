using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatSpawnerManager : MonoBehaviour
{
    private class HatStruct
    {
        public Transform hat;
        public Transform playerHeadRef;

        public HatStruct(Transform playerRef)
        {
            this.playerHeadRef = playerRef;
            hat = null;
        }

        public void SetHat(Transform hat)
        {
            this.hat = hat;
        }
    }

    [SerializeField] private GameObject[] hatPrefabs;
    public static HatSpawnerManager Instance { get; private set; }
    public GameObject[] HatPrefabs { get => hatPrefabs; }

    private Dictionary<ulong, HatStruct> playerHats;

    [SerializeField] private float hatHeight = 1.45f;

    private void Awake()
    {
        Instance = this;
        playerHats = new Dictionary<ulong, HatStruct>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate() //TO DO: check if this fixes mini-lag between hats and players
    {
        UpdateHats();   
    }

    public void GetPlayerRef(ulong playerID, Transform playerTrans)
    {
        playerHats[playerID] = new HatStruct(playerTrans);
    }

    public void SpawnHat(ulong playerID, int hatPrefab)
    {
        GameObject hat = Instantiate(hatPrefabs[hatPrefab]);
        playerHats[playerID].SetHat(hat.transform);
    }

    private void UpdateHats()
    {
        foreach (KeyValuePair<ulong, HatStruct> pair in playerHats)
        {
            if (pair.Value.hat != null)
            {
                pair.Value.hat.position = pair.Value.playerHeadRef.position + new Vector3(0, hatHeight, 0);
                pair.Value.hat.rotation = pair.Value.playerHeadRef.rotation;
            }
        }
    }
}
