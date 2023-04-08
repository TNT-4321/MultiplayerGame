using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDatabaseClass : MonoBehaviour
{
    [SerializeField] private CharacterClass[] characters = new CharacterClass[0];

    public CharacterClass[] GetAllCharacters() => characters;

    private void Awake() 
    {
        DontDestroyOnLoad(gameObject);
    }

    public CharacterClass GetCharacterById(int id)
    {
        foreach(var character in characters)
        {
            if(character.Id == id)
            {
                return character;
            }
        }

        return null;
    }

    public bool IsValidCharacterId(int id)
    {
        return characters.Any(x => x.Id == id);
    }
}
