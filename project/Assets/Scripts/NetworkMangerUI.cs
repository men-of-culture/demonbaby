using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkMangerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;

    private void Start()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        foreach (var item in args) 
        {
            if (item == "-launch-as-server")
            {
                NetworkManager.Singleton.StartServer();
            }
        }
    }
    private void Awake()
    {
        serverBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
        });

        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
        });

        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
    }
}
