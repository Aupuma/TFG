﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MazeManager : Puzzle {

    [Header("References")]
    public Animator[] lockedElements;
    public Animator[] hiddenTrapsBeforeKey;
    public Animator[] hiddenTrapsAfterKey;

    #region SINGLETON
    public static MazeManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion SINGLETON

    public override void Start()
    {
        base.Start();
        if (isServer) animator.SetTrigger("Move");

        //spawnearemos el prefab del player pov
        //else GameManager.instance.POVConnection.CmdSpawnPOVPlayerObj();
    }

    public void EnableFirstTraps()
    {
        RpcEnableFirstTraps();
        RpcDisableTrapSymbolsOnPov();
    }

    [ClientRpc]
    public void RpcDisableTrapSymbolsOnPov()
    {
        if (!isServer)
        {
            foreach (var trap in hiddenTrapsAfterKey)
            {
                trap.GetComponent<MeshRenderer>().enabled = false;
            }
            foreach (var trap in hiddenTrapsBeforeKey)
            {
                trap.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    [ClientRpc]
    public void RpcEnableFirstTraps()
    {
        foreach (var item in hiddenTrapsBeforeKey)
        {
            item.SetTrigger("fadeIn");
        }
    }

    [ClientRpc]
    public void RpcUnlockElements()
    {
        foreach (var item in lockedElements)
        {
            item.SetTrigger("Unlock"); //Movemos hacia abajo las paredes bloqueadas
        }
        foreach (var item in hiddenTrapsBeforeKey)
        {
            item.SetTrigger("fadeOut");
        }
        foreach (var item in hiddenTrapsAfterKey)
        {
            item.SetTrigger("fadeIn");
        }
    }

    [ClientRpc]
    public void RpcMazeCompleted()
    {
        foreach (var item in hiddenTrapsAfterKey)
        {
            item.SetTrigger("fadeOut");
        }
        if(isServer) PuzzleCompleted();
    }
}
