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
    private float radius;

    public void Init(ulong throwerID, float speed, Vector3 direction)
    {
        this.speed = speed;
        this.throwerID = throwerID;
        this.direction = direction;
        init = true;
        transform.LookAt(transform.position + direction);
    }

    // Start is called before the first frame update
    void Start()
    {
        radius = GetComponent<SphereCollider>().radius * transform.lossyScale.x;
        Debug.Log(radius);
    }

    private void Update()
    {
        if (!IsHost) { return; }
        if (!init) { return; }

        float moveSpeed = speed * Time.deltaTime;
        
        
        //check collisions
        if (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hit, moveSpeed))
        {
            Debug.Log("Hit " + hit.collider.name);
            if (hit.collider.GetComponent<IDamageable>() != null)
            {
                ulong hitPlayerID = hit.collider.GetComponent<PlayerController>().OwnerClientId;
                hit.collider.GetComponent<IDamageable>().ProcessHit(hitPlayerID);
                //StunPlayerServerRpc(hitPlayerID);
            }
            Destroy(gameObject);
        }
        else
        {
            transform.position += direction * moveSpeed;
            SnowballTrajectoryUpdateClientRpc(transform.position.x, transform.position.y, transform.position.z);
        }
        
    }


    [ClientRpc]
    private void SnowballTrajectoryUpdateClientRpc(float x, float y, float z)
    {
        transform.position = new Vector3(x, y, z);
    }

    /*

    [ClientRpc]
    private void StunPlayerServerRpc(ulong hitPlayer)
    {
        NetworkManager.Singleton.ConnectedClients[hitPlayer].PlayerObject.GetComponent<IDamageable>().ProcessHit();
    }*/
    /*
    public void OnDrawGizmosSelected()
    {
        if (init)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
            Debug.DrawLine(transform.position, transform.position + transform.forward * radius, Color.red);
            if (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hit, speed* Time.deltaTime))
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, hit.point);
                Gizmos.DrawWireSphere(hit.point, radius);
            }
            else
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, transform.position + transform.forward * speed * Time.deltaTime);
            }
        }
    }*/
}
