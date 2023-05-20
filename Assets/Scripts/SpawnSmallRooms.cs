using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(SpawnPoints))]
public class SpawnSmallRooms : NetworkBehaviour
{
    [SerializeField] private GameObject[] smallRoomsPrefabs;

    private void Start()
    {
        Transform [] spawnPoints = GetComponent<SpawnPoints>().SpawnPointsArray;
        foreach (Transform spawnPoint in spawnPoints)
        {
            int room = Random.Range(0, smallRoomsPrefabs.Length);
            Vector3 rotation = spawnPoint.rotation.eulerAngles + smallRoomsPrefabs[room].transform.rotation.eulerAngles;

            GameObject spawnedRoom = Instantiate(smallRoomsPrefabs[room], spawnPoint.position - new Vector3(0f, 1.7f, 0f), Quaternion.Euler(rotation.x, rotation.y, rotation.z), null);
            spawnedRoom.GetComponent<NetworkObject>().Spawn(true);
        }
    }
}
