using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class GameManagerScript : NetworkBehaviour
{
    public List<GameObject> listOfPlayers; // List of players as gameobjects

    public void AddPlayer(GameObject gameObject)
    {
        if (listOfPlayers.Contains(gameObject)) return;
        listOfPlayers.Add(gameObject);
    }

    public void RemovePlayer(GameObject gameObject)
    {
        if (!listOfPlayers.Contains(gameObject)) return;
        listOfPlayers.Remove(gameObject);
    }

    public void StartGame()
    {
        if(listOfPlayers.Where(x => x.GetComponent<PlayerScript>().isReady == true).ToList().Count == listOfPlayers.Count) StartGameClientRPC();
    }

    [ClientRpc]
    public void StartGameClientRPC()
    {
        Debug.Log("Game started");
    }
    
    public void EndGame()
    {
        if(listOfPlayers.Count >= 2 && listOfPlayers.Where(x => x.GetComponent<PlayerScript>().isAlive == true).ToList().Count == 1) EndGameClientRPC();
        // Todo: Consider draw endgame screen count == 0
    }
    
    [ClientRpc]
    public void EndGameClientRPC()
    {
        GameObject.Find("EndCanvas").GetComponent<Canvas>().enabled = true;
    }
}
