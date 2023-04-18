using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public bool IsStunned { get; set; }
    public void ProcessHit();
}
