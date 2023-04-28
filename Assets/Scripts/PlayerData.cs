using System;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerData: IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientID;
    public int colorID;
    public FixedString64Bytes playerName;


    public bool Equals(PlayerData other)
    {
        return clientID == other.clientID && colorID == other.colorID && playerName == other.playerName;
    }

    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientID);
        serializer.SerializeValue(ref colorID);
        serializer.SerializeValue(ref playerName);
    }
}
