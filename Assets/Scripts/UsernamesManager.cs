using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsernamesManager : MonoBehaviour
{
    public static UsernamesManager Instance { get; private set; }
    
    [SerializeField] CameraController cameraController;
    [SerializeField] GameObject nameLabelPrefab;

    private const float labelHeight = 2f;

    private List<Transform> playerRefs;
    private List<TMPro.TextMeshPro> nameLabels;
    private void Awake()
    {
        Instance = this;
        playerRefs = new List<Transform>();
        nameLabels = new List<TMPro.TextMeshPro>();
    }

    public void GetPlayerRef(Transform playerTrans, string name)
    {
        playerRefs.Add(playerTrans);
        GameObject nameLabelOb = Instantiate(nameLabelPrefab);
        TMPro.TextMeshPro label = nameLabelOb.GetComponent<TMPro.TextMeshPro>();
        nameLabels.Add(label);
        label.text = name;
    }

    private void Update()
    {
        UpdateNameLabels();
    }

    
    private void UpdateNameLabels()
    {
        for (int i = 0; i < playerRefs.Count; i++)
        {
            nameLabels[i].transform.position = playerRefs[i].transform.position + new Vector3(0, labelHeight, 0);    
            nameLabels[i].transform.LookAt(cameraController.transform);
        }
    }

}
