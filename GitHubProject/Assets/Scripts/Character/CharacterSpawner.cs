using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterSpawner : NetworkBehaviour
{
    [SerializeField] private CharacterDatabase database;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) {return;}

        foreach (var client in ServerManager.Instance.ClientData)
        {
            var character =  database.GetCharacterById(client.Value.characterId);
            if(character != null)
            {
                //So that the characters spawn not on top of each other
                var spawnPos = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
                //Here we have to set the spawn point positions
                var characterInstance = Instantiate(character.GameplayPrefab, spawnPos, Quaternion.identity);
                //Spawn the objects for all the clients as their playerObject | passing the ownership to the client that selected this character
                characterInstance.SpawnAsPlayerObject(client.Value.clientId);
            }
        }
    }
}
