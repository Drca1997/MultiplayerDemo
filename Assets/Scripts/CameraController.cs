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
    [SerializeField]
    private Vector3 offset;
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
    }

    private void OnPlayerSpawn(object sender, PlayerController.OnPlayerSpawnArgs args)
    {
        player = args.playerTransform;
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
            Vector3 desiredPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
            transform.LookAt(player);
        }
    }

}
