using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.Netcode;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameState", order = 1)]
public class GameState : ScriptableObject
{
    public List<PlayerState> list = new List<PlayerState>();

    // void Start()
    // {
    //     if (!IsServer) return;
    // }

    public class PlayerState
    {
        public ulong id;
        public bool isReady = false;
        public bool isAlive = true;
    }
}
