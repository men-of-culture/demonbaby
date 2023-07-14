using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.Netcode;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameState", order = 1)]
public class GameState : ScriptableObject
{
    public List<ulong> list = new List<ulong>();

    // void Start()
    // {
    //     if (!IsServer) return;
    // }
}
