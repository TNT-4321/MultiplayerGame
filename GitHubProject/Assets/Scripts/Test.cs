using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Test : NetworkBehaviour
{
    private Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) {return;}

        if(Input.GetKeyDown(KeyCode.O))
        {
            rigidbody.velocity = new Vector3(0, 5f, 0);
        }
    }
}
