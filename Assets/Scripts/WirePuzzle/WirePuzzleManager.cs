﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;

public class WirePuzzleManager : Puzzle {

    [SyncVar(hook = "OnChangeIndex")]
    public int currentPuzzleIndex = 0;

    public GameObject[] panelLights;
    private bool[] lightsOn;
    private Draggable currentDraggedObject;

    #region SINGLETON
    public static WirePuzzleManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion //SINGLETON

    public override void Start () {
        base.Start();
        lightsOn = new bool[panelLights.Length];
	}

    private void Update()
    {
        if (currentDraggedObject != null) //Estamos moviendo un objeto
        {
            if (Input.GetMouseButtonUp(0))
            {
                currentDraggedObject.Selected = false;
                currentDraggedObject = null;
            }
        }
        else //Lanzamos raycast para saber si estamos tocando un handle
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit = new RaycastHit();
                // Construct a ray from the current touch coordinates
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Handle")
                    {
                        currentDraggedObject = hit.transform.GetComponentInParent<Draggable>();
                        currentDraggedObject.Selected = true;
                    }
                }
            }
        }
    }

    public void NextNodeConnected()
    {
        currentPuzzleIndex = currentPuzzleIndex + 1;
    }

    public void NodeLostConnexion()
    {
        currentPuzzleIndex = currentPuzzleIndex - 1;
    }

    //SyncVar Hook
    void OnChangeIndex(int index)
    {
        if(isServer)
        {
            Debug.Log(index);
            if (index == panelLights.Length) PuzzleCompleted();
        }
        else
        {
            Debug.Log(index);
            currentPuzzleIndex = index;
            for (int i = 0; i < panelLights.Length; i++)
            {
                if(i < currentPuzzleIndex)
                {
                    if(lightsOn[i] == false)
                    {
                        lightsOn[i] = true;
                        panelLights[i].GetComponent<Animator>().SetTrigger("fadeIn");
                    }
                }
                else if(lightsOn[i] == true)
                {
                    lightsOn[i] = false;
                    panelLights[i].GetComponent<Animator>().SetTrigger("fadeOut");
                }
            }
        }
    }

    public void WaitToComplete()
    {
        Invoke("PuzzleCompleted", 1.5f);
    }

    public override void PuzzleCompleted()
    {
        if (currentDraggedObject != null) currentDraggedObject.Selected = false;
        base.PuzzleCompleted();
    }
}
