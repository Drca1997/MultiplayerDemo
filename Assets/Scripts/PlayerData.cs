using System;
using Unity.Netcode;

public struct PlayerData: IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientID;

    
    public bool Equals(PlayerData other)
    {
        return clientID == other.clientID;
    }

    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientID);
    }
}
