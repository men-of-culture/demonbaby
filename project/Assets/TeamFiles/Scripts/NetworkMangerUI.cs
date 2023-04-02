using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
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
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                    "35.228.146.171",
                    (ushort)9000
                    // "10.0.0.4",
                    // (ushort)7777 //
                );
                NetworkManager.Singleton.StartServer();
            }
        }
    }
    private void Awake()
    {
        serverBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                "127.0.0.1",
                (ushort)7777
            );
            NetworkManager.Singleton.StartServer();
        });

        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                "127.0.0.1",
                (ushort)7777
            );
            NetworkManager.Singleton.StartHost();
        });

        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                "35.228.146.171",
                (ushort)9000
                // "127.0.0.1", //"20.67.245.84"
                // (ushort)7777
            );
            NetworkManager.Singleton.StartClient();
        });
    }
}
