using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(SpawnPoints))]
public class SpawnSmallRooms : NetworkBehaviour
{
    [SerializeField] private GameObject[] smallRoomsPrefabs;

    public void SpawnSmallRoom()
    {
        if (!IsServer) { return; }
        Transform [] spawnPoints = GetComponent<SpawnPoints>().SpawnPointsArray;
        foreach (Transform spawnPoint in spawnPoints)
        {
            int room = Random.Range(0, smallRoomsPrefabs.Length);
            Vector3 rotation = spawnPoint.rotation.eulerAngles + smallRoomsPrefabs[room].transform.rotation.eulerAngles;

            GameObject spawnedRoom = Instantiate(smallRoomsPrefabs[room], spawnPoint.position, Quaternion.Euler(rotation.x, rotation.y, rotation.z), null);
            spawnedRoom.GetComponent<NetworkObject>().Spawn(true);
        }
    }
}
