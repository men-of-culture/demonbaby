
using System;
using Unity.Netcode;
using UnityEngine;

public class projectileScript : NetworkBehaviour
{
    public int projectileSpeed;
    public Vector3 startPoint;
    public string playerOriginName;
    public GameObject playerOrigin;
    // Start is called before the first frame update
    void Start()
    {
        playerOrigin = GameObject.Find(playerOriginName);
        transform.position = playerOrigin.transform.position;
        startPoint = playerOrigin.transform.position;
        transform.position += new Vector3(0, 0, 10);
        gameObject.GetComponent<NetworkObject>().Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        updateProjectileServerRpc();
    }

    [ServerRpc]
    private void updateProjectileServerRpc() {
        if (transform.position.z >= startPoint.z + 200) Destroy(this.gameObject);
        transform.position += transform.forward * Time.deltaTime * projectileSpeed;
    }
}
