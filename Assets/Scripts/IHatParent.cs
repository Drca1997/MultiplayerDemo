using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IHatParent
{
    public Transform GetHatFollowTransform();

    public void SetHat(Hat kitchenObject);

    public Hat GetHat();

    public void ClearHat();

    public bool HasHat();

    public NetworkObject GetNetworkObject();
}
