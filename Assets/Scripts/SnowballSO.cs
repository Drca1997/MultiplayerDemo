using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SnowballSO : ScriptableObject
{
    public Transform prefab;
    public float speed;
    public float cooldown;
}
