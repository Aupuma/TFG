﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnectionObject : NetworkBehaviour {

    public GameObject PlayerUnitPrefab;
    public GameObject ARPlayerCamera;

    void Start () {
        DontDestroyOnLoad(gameObject);

        //Es éste mi PlayerObject local?
        if (isLocalPlayer == false)
        {
            return;
        }

        if(!isServer) //Soy el jugador en primera persona
        {
            CmdSpawnPOVPlayerObj();
        }
        //FindObjectOfType<GameManager>().connection = this;
    }

    //--------------------------------------COMMANDS
    //Commandos son funciones especiales que SOLO se ejecutan en el servidor

    [Command]
    void CmdSpawnPOVPlayerObj()
    {
        GameObject playerObject = Instantiate(PlayerUnitPrefab,this.transform.position,Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(playerObject,connectionToClient);
        RpcAssignConnectionToPOVPlayer(playerObject);
    }

    [Command]
    public void CmdActivateRemoteTraps()
    {
        RpcActivateRemoteTraps();
    }


    //-------------------------------------RPC
    //RPCs son funciones especiales que SOLO se ejecutan en los clientes
    [ClientRpc]
    void RpcAssignConnectionToPOVPlayer(GameObject playerObject)
    {
        if (!isServer) playerObject.GetComponent<PlayerInteractions>().myConnection = this;
    }

    [ClientRpc]
    void RpcActivateRemoteTraps()
    {
        if (isServer) TrapManager.instance.TrapsOnOff();
    }
    //---------------------------------------
}
