using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    private Vector3 movementVector;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 redSpawnPoint;
    [SerializeField] private Vector3 blueSpawnPoint;
    [SerializeField] private Transform ballPrefab;


    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            transform.position = redSpawnPoint;
        }
        else
        {
            transform.position = blueSpawnPoint;
            GetComponentInChildren<MeshRenderer>().material.color = new Color32(0, 0, 255, 255);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        x = Mathf.Clamp(x, -1.0f, 1.0f);
        y = Mathf.Clamp(y, -1.0f, 1.0f);
        movementVector = new Vector3(x, 0, y).normalized;

        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Kicking Ball");
            //ball.GetComponent<Rigidbody>();
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (movementVector.magnitude >= 0.1f) //Se houve input do jogador
        {
            transform.position += speed * Time.fixedDeltaTime * movementVector;
        }
    }
}
