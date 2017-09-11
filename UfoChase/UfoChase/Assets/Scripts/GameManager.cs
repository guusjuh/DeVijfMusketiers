﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all instances in the game 
/// </summary>
public class GameManager : MonoBehaviour {
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }

    [SerializeField]
    private Vector3 spawnPosition = Vector3.zero;

    public PlayerScript Player { get; private set; }
    public UfoScript UFO { get; private set; }

    public bool InGame { get; private set; }

    public void Start()
    {
        InGame = false;

        Player = FindObjectOfType<PlayerScript>();
        Player.Initialize(spawnPosition);

        UFO = FindObjectOfType<UfoScript>();
        UFO.Initialize();

        return;
    }

    public void Update()
    {
        Player.Loop();
        UFO.Loop();

        return;
    }
}
