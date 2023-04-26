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
    [SerializeField]
    private float sensivity;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;
    private float rotationX = 0f;
    private float rotationY = 0f;


    // Start is called before the first frame update
    void Awake()
    {
        PlayerController.OnPlayerSpawn += OnPlayerSpawn;
    }

    private void OnPlayerSpawn(object sender, PlayerController.OnPlayerSpawnArgs args)
    {
        player = args.playerTransform;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            rotationY += Input.GetAxis("Mouse X") * sensivity;
            rotationX -= Input.GetAxis("Mouse Y") * sensivity;
            transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
        }

    }

    private void FixedUpdate()
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
