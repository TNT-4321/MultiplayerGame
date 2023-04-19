using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Interactor : NetworkBehaviour
{
    private void Update() 
    {
        if(!IsOwner) {return;}

        
    }
}
