using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class GameManagerScript : NetworkBehaviour
{
    public List<GameObject> listOfPlayers; // List of players as gameobjects

    public void addPlayer(GameObject gameObject)
    {
        if (listOfPlayers.Contains(gameObject)) return;
        listOfPlayers.Add(gameObject);
    }
    
    public void endGame()
    {
        if(listOfPlayers.Count >= 2 && listOfPlayers.Where(x => x.GetComponent<PlayerScript>().isAlive == false).ToList().Count >= 1) endGameClientRPC();
    }
    
    [ClientRpc]
    public void endGameClientRPC()
    {
        GameObject.Find("EndCanvas").GetComponent<Canvas>().enabled = true;
    }
}
