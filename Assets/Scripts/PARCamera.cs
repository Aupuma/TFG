﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PARCamera : MonoBehaviour {

    public static PARCamera instance;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(this.gameObject);
	}
}
