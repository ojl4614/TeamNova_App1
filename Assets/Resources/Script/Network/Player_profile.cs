using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class Player_profile : MonoBehaviour
{
    public string name_;
    // Start is called before the first frame update
    private void Awake()
    {
        name_ = "";
    }
}
