using System;
using Unity.Netcode;

public struct CharacterSelectState : INetworkSerializable, IEquatable<CharacterSelectState>
{
    //Here we put in all the data we want to sync
    public ulong ClientId;
    public int CharacterId;

    public CharacterSelectState(ulong clientId, int characterId = -1)
    {
        ClientId = clientId;
        CharacterId = characterId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref CharacterId);
    }

    public bool Equals(CharacterSelectState other)
    {
        //returns true if this is true
        return ClientId == other.ClientId && CharacterId == other.CharacterId;
    }
}
