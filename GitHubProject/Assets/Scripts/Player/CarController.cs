using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CarController : NetworkBehaviour
{
    [SerializeField] private Camera carCam;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) 
        {
            carCam.gameObject.SetActive(false);
            return;
        }

        if(Input.GetKeyDown(KeyCode.U))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 20, transform.position.z);
        }
    }
}
