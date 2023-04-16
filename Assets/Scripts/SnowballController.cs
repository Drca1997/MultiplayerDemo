using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SnowballController : NetworkBehaviour
{
    private float speed;
    private ulong throwerID;
    private Vector3 direction;
    private bool init = false;
  
    public void Init(ulong throwerID, float speed, Vector3 direction)
    {
        this.speed = speed;
        this.throwerID = throwerID;
        this.direction = direction;
        init = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsHost) { return; }
        if (!init) { return; }

        transform.position += direction * speed * Time.fixedDeltaTime;
        SnowballTrajectoryUpdateClientRpc(transform.position.x, transform.position.y, transform.position.z);
    }

    [ClientRpc]
    private void SnowballTrajectoryUpdateClientRpc(float x, float y, float z)
    {
        transform.position = new Vector3(x, y, z);
    }
}
