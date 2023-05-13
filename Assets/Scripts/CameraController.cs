using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    [Tooltip("Referência ao objeto Player, para a câmara o seguir")]
    [SerializeField]
    private Transform player;
    [SerializeField]
    private float smoothSpeed = 0.125f;
    [Header("Camera Offsets")]
    [SerializeField]
    private Vector3 defaultOffset;
    [SerializeField]
    private Vector3 northOffset;
    [SerializeField]
    private Vector3 northEastOffset;
    [SerializeField]
    private Vector3 eastOffset;
    [SerializeField]
    private Vector3 southEastOffset;
    [SerializeField]
    private Vector3 southOffset;
    [SerializeField]
    private Vector3 southWestOffset;
    [SerializeField]
    private Vector3 westOffset;
    [SerializeField]
    private Vector3 northWestOffset;

    private Vector3 currentOffset;

    private PlayerController playerController;

    public static event EventHandler<OnFollowPlayerArgs> OnFollowPlayer;
    public class OnFollowPlayerArgs: EventArgs
    {
        public Transform cameraTransform;
    }
    /*
    [SerializeField]
    private float sensivity;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;
    private float rotationX = 0f;
    private float rotationY = 0f;
    */
    private List<GameObject> obstaclesInPreviousFrame;

    // Start is called before the first frame update
    void Awake()
    {
        PlayerController.OnPlayerSpawn += OnPlayerSpawn;
        
        obstaclesInPreviousFrame = new List<GameObject>();
        currentOffset = defaultOffset;
    }

    private void OnPlayerSpawn(object sender, PlayerController.OnPlayerSpawnArgs args)
    {
        player = args.playerTransform;
        playerController = player.GetComponent<PlayerController>();
        OnFollowPlayer?.Invoke(this, new OnFollowPlayerArgs { cameraTransform = transform});
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (player != null)
        {
            rotationY += Input.GetAxis("Mouse X") * sensivity;
            rotationX -= Input.GetAxis("Mouse Y") * sensivity;
            transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
        }*/
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            RaycastHit [] hits = Physics.RaycastAll(transform.position, direction);
            GameObject[] objectsInTheWay = GetGameObjectsFromRaycastHits(hits); 
            Debug.DrawLine(transform.position, player.position, Color.red);
            foreach(RaycastHit hit in hits)
            {
                if (hit.collider.GetComponent<TransparentObstacle>() != null)
                {
                    if (!obstaclesInPreviousFrame.Contains(hit.collider.gameObject))
                    {
                        hit.collider.GetComponent<TransparentObstacle>().SetTransparent();
                    }
                }
            }
            for(int i = obstaclesInPreviousFrame.Count - 1; i >= 0; i--)
            {
                if (!objectsInTheWay.Contains(obstaclesInPreviousFrame[i]))
                {
                    if (obstaclesInPreviousFrame[i].GetComponent<TransparentObstacle>() != null)
                    {
                        obstaclesInPreviousFrame[i].GetComponent<TransparentObstacle>().SetSolid();
                        obstaclesInPreviousFrame.RemoveAt(i);
                    }
                }
            }
            foreach (GameObject hitOb in objectsInTheWay)
            {
                if (hitOb.GetComponent<TransparentObstacle>() != null)
                {
                    if (!obstaclesInPreviousFrame.Contains(hitOb))
                    {
                        obstaclesInPreviousFrame.Add(hitOb);
                    }
                }
            }
        }
    }

    private GameObject[] GetGameObjectsFromRaycastHits(RaycastHit[] hits)
    {
        GameObject[] gameObjects = new GameObject[hits.Length];
        for(int i = 0; i < hits.Length; i++)
        {
            gameObjects[i] = hits[i].collider.gameObject;
        }
        return gameObjects;
    } 

    private void FixedUpdate()
    {
        if (player != null)
        {
            ChangeOffset();
            Vector3 desiredPosition = player.position + currentOffset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
            transform.LookAt(player);
        }
    }

    private void ChangeOffset()
    { 

        if (playerController.MovementVector.z < 0) //south
        {
            if (playerController.MovementVector.x > 0) //east
            {
                currentOffset = southEastOffset;
            }
            else if (playerController.MovementVector.x < 0) //west
            {
                currentOffset = southWestOffset;
            }
            else
            {
                currentOffset = southOffset;
            }
        }
        else if (playerController.MovementVector.z > 0) //north
        {
            if (playerController.MovementVector.x > 0) //east
            {
                currentOffset = northEastOffset;
            }
            else if (playerController.MovementVector.x < 0) //west
            {
                currentOffset = northWestOffset;
            }
            else
            {
                currentOffset = northOffset;
            }
        }
        else
        {
            if (playerController.MovementVector.x > 0) //east
            {
                currentOffset = eastOffset;
            }
            else if (playerController.MovementVector.x < 0) //west
            {
                currentOffset = westOffset;
            }
            else
            {
                currentOffset= defaultOffset;
            }
        }
       
    }

}
