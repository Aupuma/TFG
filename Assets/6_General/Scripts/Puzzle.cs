﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;

public class Puzzle : NetworkBehaviour {

    [Header ("Player dependent objects")]
    public GameObject[] AR_Player_Objects;
    public GameObject[] POV_Player_Objects;
    public GameObject debugSandPlane;

    private bool hasStarted = false;
    [HideInInspector]
    public Animator animator;

    public static bool placed = false;

    public virtual void Start()
    {
        animator = GetComponent<Animator>();

        //Situamos el nivel en el punto elegido en AR, esto solo lo hará el primer nivel en cargar
        if (isServer && ARWorldOrigin.instance != null && !placed)
        {
            ARWorldOrigin.instance.PlaceLevelAtOrigin(this.transform);
            placed = true;
        }
        if(isServer && ARWorldOrigin.instance == null)
        {
            debugSandPlane.SetActive(true);
        }

        HidePlayerDependentObjects();
    }

    public void HidePlayerDependentObjects()
    {
        if (isServer) //Somos el jugador en AR
        {
            foreach (var obj in POV_Player_Objects)
            {
                if(obj!=null) obj.SetActive(false);
            }
        }
        else //Somos el jugador en primera persona
        {
            foreach (var obj in AR_Player_Objects)
            {
                obj.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Llamado desde el final de la animación de aparecer escenario
    /// </summary>
    public virtual void OnPuzzleReady()
    {
        hasStarted = true;
        //if (isDebug) NetDiscovery.instance.StartAsServer();
    }

    public virtual void WaitToComplete()
    {
        Invoke("PuzzleCompleted", 1.5f);
    }

    /// <summary>
    /// Llamado una vez se ha completado el puzzle, activa animación de
    /// desaparición del escenario
    /// </summary>
    public virtual void PuzzleCompleted()
    {
        RpcClosePOVWalls();
        animator.SetTrigger("Disappear");
    }

    /// <summary>
    /// Activamos la animación de cerrar las paredes de la habitación POV
    /// </summary>
    [ClientRpc]
    public void RpcClosePOVWalls()
    {
        if (!isServer) POVRoom.instance.animator.SetTrigger("CloseWalls");
    }

    /// <summary>
    /// Llamado desde el final de la animacion de desaparecer escenario
    /// </summary>
    public void LoadNextLevel()
    {
        if (hasStarted) GameManager.instance.LoadNextScene();
    }
}
