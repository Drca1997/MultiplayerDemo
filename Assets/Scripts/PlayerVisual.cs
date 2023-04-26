using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private MeshRenderer scarfMeshRenderer;

    private Material material;


    private void Awake()
    {
        material = new Material(scarfMeshRenderer.material);
        scarfMeshRenderer.material = material;
    }

   

    public void SetPlayerColor(Color color)
    {
        material.color = color;
    }
}
