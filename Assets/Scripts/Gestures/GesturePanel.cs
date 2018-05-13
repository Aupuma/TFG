﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GesturePanel : MonoBehaviour {

    GestureManager gestureManager;
    public string gestureName;
    private bool solved;

    // Update is called once per frame
    void Update () {
		
	}

    public void ShowSolution()
    {
        GetComponent<MeshRenderer>().material.color = Color.green;
    }
    /*
    [Command]
    public void CmdShowSolutionOverNetwork()
    {
        RpcShowSolutionLocally();
    }

    [ClientRpc]
    public void RpcShowSolutionLocally()
    {
        solved = true;
        GetComponent<MeshRenderer>().material.color = Color.green;
        //Comunico al gameManager que he sido resuelto para que lo comunique en red
    }*/
}