using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class StartUpScript : MonoBehaviour
{
    public PlayerState playerState;
    // Start is called before the first frame update
    void Start()
    {
        if (playerState.isHost) {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                "127.0.0.1",
                (ushort)7777
            );
            NetworkManager.Singleton.StartServer();
        }
        else {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                "127.0.0.1",
                (ushort)7777
            );
            NetworkManager.Singleton.StartClient();
        }
        
    }
}
