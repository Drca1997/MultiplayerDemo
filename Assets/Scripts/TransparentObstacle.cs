using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentObstacle : MonoBehaviour
{
    [SerializeField] 
    private Material transparentMaterial;
    
    private Material normalMaterial;

    private MeshRenderer meshRenderer;


    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        normalMaterial = meshRenderer.material;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTransparent()
    {
        meshRenderer.material = transparentMaterial;    
    }

    public void SetSolid()
    {
        meshRenderer.material = normalMaterial;
    }
}
