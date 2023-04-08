using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject disabledOverlay;
    [SerializeField] private Button button;

    private CharacterSelectDisplay characterSelect;
    public Character Character {get; private set;}
    //public CharacterClass CharacterClass {get; private set;}
    public bool IsDisabled {get; private set;}

    public void SetCharacter(CharacterSelectDisplay characterSelect, Character character/*, CharacterClass characterClass*/)
    {
        iconImage.sprite = character.Icon;

        this.characterSelect = characterSelect;
        
        Character = character;
        //CharacterClass = characterClass;
    }

    public void SelectCharacter()
    {
        characterSelect.Select(Character/*CharacterClass*/);
    }

    public void SetDisabled()
    {
        IsDisabled = true;
        disabledOverlay.SetActive(true);
        button.interactable = false;
    }
}
