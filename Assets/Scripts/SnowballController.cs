using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SnowballController : NetworkBehaviour
{
    private float speed;
    private NetworkObject thrower;
    private Vector3 direction;
    private bool init = false;
    private float radius;

    public void Init(NetworkObject thrower, float speed, Vector3 direction)
    {
        this.speed = speed;
        this.thrower = thrower;
        this.direction = direction;
        init = true;
        transform.LookAt(transform.position + direction);
    }

    // Start is called before the first frame update
    void Start()
    {
        radius = GetComponent<SphereCollider>().radius * transform.lossyScale.x;
    }

    private void Update()
    {
        if (!IsHost) { return; }
        if (!init) { return; }

        float moveSpeed = speed * Time.deltaTime;
        
        
        //check collisions
        if (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hit, moveSpeed))
        {
            if (hit.collider.GetComponent<IDamageable>() != null)
            {
                ulong hitPlayerID = hit.collider.GetComponent<PlayerController>().OwnerClientId;
                ClientRpcParams hitclientParams = new ClientRpcParams();
                ClientRpcSendParams hitSendParams = new ClientRpcSendParams { TargetClientIds = new List<ulong> { hitPlayerID } };
                hitclientParams.Send = hitSendParams;
                StunPlayerClientRpc(hit.collider.GetComponent<NetworkObject>(), hitclientParams);
                ClientRpcParams throwerClientParams = new ClientRpcParams();
                ClientRpcSendParams throwerSendParams = new ClientRpcSendParams { TargetClientIds = new List<ulong> { thrower.OwnerClientId } };
                throwerClientParams.Send = throwerSendParams;
                UpdateThrowerScoreClientRpc(thrower, throwerClientParams);
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
    private void UpdateThrowerScoreClientRpc(NetworkObjectReference throwerPlayerReference, ClientRpcParams clientParams)
    {
        throwerPlayerReference.TryGet(out NetworkObject throwerPlayerObject);
        throwerPlayerObject.GetComponent<PlayerController>().Score += GameDesignConstants.ON_THROW_SCORE;
    }

    [ClientRpc]
    private void SnowballTrajectoryUpdateClientRpc(float x, float y, float z)
    {
        transform.position = new Vector3(x, y, z);
    }


    [ClientRpc]
    private void StunPlayerClientRpc(NetworkObjectReference hitPlayerReference, ClientRpcParams clientParams)
    {
        hitPlayerReference.TryGet(out NetworkObject hitPlayerObject);
        if (!hitPlayerObject.GetComponent<IDamageable>().IsStunned)
        {
            hitPlayerObject.GetComponent<IDamageable>().ProcessHit();
        }
    }
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
