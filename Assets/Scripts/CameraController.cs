using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("Referência ao objeto Player, para a câmara o seguir")]
    [SerializeField]
    private Transform player;
    [SerializeField]
    private float smoothSpeed = 0.125f;
    [SerializeField]
    private Vector3 offset;


    // Start is called before the first frame update
    void Start()
    {
        PlayerController.OnPlayerSpawn += OnPlayerSpawn;
    }

    private void OnPlayerSpawn(object sender, PlayerController.OnPlayerSpawnArgs args)
    {
        player = args.playerTransform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player != null)
        {
            Vector3 desiredPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
            transform.LookAt(player);
        }
           
    }
}
