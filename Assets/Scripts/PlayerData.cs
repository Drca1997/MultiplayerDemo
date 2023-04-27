using System;
using Unity.Netcode;

public struct PlayerData: IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientID;
    public int colorID;

    
    public bool Equals(PlayerData other)
    {
        return clientID == other.clientID && colorID == other.colorID;
    }

    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientID);
        serializer.SerializeValue(ref colorID);
    }
}
