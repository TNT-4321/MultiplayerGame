using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICar
{
    public bool StartDriving(PlayerNetworkController driver);
    public bool StopDriving(PlayerNetworkController driver);
    public float XInput {get;}
    public float YInput {get;}
}
