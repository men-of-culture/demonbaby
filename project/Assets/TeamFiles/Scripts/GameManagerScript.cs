using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using Unity.VisualScripting;

public class GameManagerScript : NetworkBehaviour
{
    public List<GameObject> listOfPlayers; // List of players as gameobjects

    void Start()
    {
        if (!IsServer) return;
    }

    void Update()
    {
        if (!IsServer) return;
    }

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
        if(listOfPlayers.Where(x => x.GetComponent<PlayerScript>().isReady == true).ToList().Count == listOfPlayers.Count)
        {
            foreach (var player in listOfPlayers)
            {
                player.GetComponent<PlayerScript>().allReady = true;
            }
            StartGameClientRPC(); // TODO: countdown on server here and update countdown canvas for clients
        }
    }

    [ClientRpc]
    public void StartGameClientRPC()
    {
        Debug.Log("Game started");
    }
    
    public void EndGame()
    {
        if(!IsServer) return;
        if(listOfPlayers.Count >= 2 && listOfPlayers.Where(x => x.GetComponent<PlayerScript>().isAlive == true).ToList().Count == 1) 
        {
            foreach(var player in listOfPlayers) 
            {
                if(player.GetComponent<PlayerScript>().isAlive)
                {
                    player.GetComponent<PlayerScript>().isAlive = false;
                    player.GetComponent<PlayerScript>().controlsDisabled = true;
                }
            }
            EndGameClientRPC();
        }
        // Todo: Consider draw endgame screen count == 0
    }
    
    [ClientRpc]
    public void EndGameClientRPC()
    {
        GameObject.Find("EndCanvas").GetComponent<Canvas>().enabled = true;
    }
}
