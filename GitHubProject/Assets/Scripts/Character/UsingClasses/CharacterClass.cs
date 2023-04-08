using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterClass : MonoBehaviour
{
    [SerializeField] private int id = -1;
    [SerializeField] private string displayName = "New Display Name";
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject introPrefab;
    [SerializeField] private NetworkObject gameplayPrefab;

    public int Id => id;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public GameObject IntroPrefab => introPrefab;
    public NetworkObject GameplayPrefab => gameplayPrefab;

    public void SetGameplayPrefab(NetworkObject gameplayPrefabToSet)
    {
        gameplayPrefab = gameplayPrefabToSet;
    }
}
