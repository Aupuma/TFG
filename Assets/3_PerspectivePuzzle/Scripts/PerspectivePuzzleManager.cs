﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PerspectivePuzzleManager : Puzzle {

    [Header("References")]
    public float rotationTime = 0.35f;
    public Transform[] puzzlePieces;
    public Transform[] pillars;
    private AudioSource moveObjSound;

    [SyncVar]
    private bool isRotating = false;

    #region SINGLETON
    public static PerspectivePuzzleManager instance;

    public void Awake()
    {
        instance = this;
    } 
    #endregion //SINGLETON

    // Use this for initialization
    public override void Start () {
        base.Start();
        moveObjSound = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {  
        if (!isServer && !isRotating && Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = new RaycastHit();

            // Detectamos el touch en las piezas
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "RotatingPiece")
                {
                    //Comparamos la pieza tocada con las que hay en el array
                    for (int i = 0; i < puzzlePieces.Length; i++)
                    {
                        if(puzzlePieces[i] == hit.collider.transform)
                        {
                            Debug.Log(i);
                            GameManager.instance.POVConnection.CmdRotationCall(i);
                            break;
                        }
                    }
                }
            }
        }
    }

    [ClientRpc]
    public void RpcRotateElements(int index)
    {
        if (isServer) //Rotamos el pilar en el jugador AR
        {
            isRotating = true;
            pillars[index].DOLocalRotate(new Vector3(
                pillars[index].localEulerAngles.x,
                pillars[index].localEulerAngles.y,
                pillars[index].localEulerAngles.z + 90f), 
                rotationTime).
                OnComplete(RotationFinished); 
        }
        else //Rotamos la pieza en el jugador POV
        {
            Debug.Log("rotating piece");
            puzzlePieces[index].DOLocalRotate(new Vector3(
                puzzlePieces[index].localEulerAngles.x,
                puzzlePieces[index].localEulerAngles.y,
                puzzlePieces[index].localEulerAngles.z - 90f),
                rotationTime);
        }
        moveObjSound.Play();
    }

    public void RotationFinished()
    {
        isRotating = false;
        foreach (var pillar in pillars)
        {
            if (pillar.rotation.y != 0) return;
        }

        PuzzleCompleted();
    }

    public override void PuzzleCompleted()
    {
        RpcClosePOVWalls();
        animator.SetTrigger("Solved");
    }
}
