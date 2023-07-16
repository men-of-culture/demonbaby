using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManagerScript : NetworkBehaviour
{

    public List<ulong> listOfPlayers; // Total amount of Players
    public List<ulong> remainingPlayers; // Remaining players still in the game.
    
    public void addPlayer(ulong id){
        if (listOfPlayers.Contains(id)) return;
        listOfPlayers.Add(id);
        remainingPlayers.Add(id);
        Debug.Log(listOfPlayers.Count);
    }
    
    public void removePlayer(ulong id){
        if (!listOfPlayers.Contains(id)) return;
        remainingPlayers.Remove(id);
        Debug.Log(listOfPlayers.Count);
    }
    
    public void endGame(){
        if(listOfPlayers.Count >= 2 && remainingPlayers.Count <= 1) {
            endGameClientRPC();
        }
    }
    
    [ClientRpc]
    public void endGameClientRPC(){
        GameObject.Find("EndCanvas").GetComponent<Canvas>().enabled = true;
    }
    
}
