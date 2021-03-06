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
    public Transform[] lockedWalls;
    public Transform mazeTransform;

    #region SINGLETON
    public static MazeManager instance;

    public void Awake()
    {
        instance = this;
    }
    #endregion SINGLETON

    public override void Start()
    {
        base.Start();

        if (isServer) //Si somos jugador AR activamos la animacion de aparicion desde el exterior
        {
            animator.SetTrigger("Move");
            foreach (var item in lockedElements)
            {
                item.SetTrigger("Lock"); //Movemos arriba las paredes bloqueadas
            }
        }
        else //Si somos jugador POV situamos los elementos a vista de jugador
        {
            animator.SetTrigger("StayStatic");
            foreach (var item in lockedElements)
            {
                item.SetTrigger("StayStatic"); //Movemos arriba las paredes bloqueadas
            }
            //foreach (var item in lockedWalls)
            //{
            //    item.position = Vector3.zero;
            //}
            //mazeTransform.localPosition = new Vector3(0f, mazeTransform.position.y + 3.53f, 0f);
            //Debug.Log(mazeTransform.position);
        }
    }

    public void EnableFirstTraps()
    {
        RpcEnableFirstTraps();
        RpcDisableTrapSymbolsOnPov();
    }

    /// <summary>
    /// Desactivamos los símbolos de las trampas en POV para que no los pueda ver
    /// </summary>
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

    /// <summary>
    /// Activa las primeras trampas cuando se conecta el jugador
    /// </summary>
    [ClientRpc]
    public void RpcEnableFirstTraps()
    {
        foreach (var item in hiddenTrapsBeforeKey)
        {
            item.SetTrigger("fadeIn");
        }
    }

    /// <summary>
    /// Desbloquea dos paredes y cambia las trampas de sitio al coger la llave
    /// </summary>
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

    /// <summary>
    /// Activada cuando el jugador llega a la salida
    /// Desactivamos todas las trampas y activamos la animación de desaparecer
    /// </summary>
    [ClientRpc]
    public void RpcMazeCompleted()
    {
        foreach (var item in hiddenTrapsAfterKey)
        {
            item.SetTrigger("fadeOut");
        }
        if (isServer) PuzzleCompleted();
        else GameManager.instance.FadeOnTeleport();
    }

    public override void PuzzleCompleted()
    {
        animator.SetTrigger("Disappear");
    }
}
