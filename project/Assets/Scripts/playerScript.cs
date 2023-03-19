using UnityEngine;
using Unity.Netcode;
using System;

public class playerScript : NetworkBehaviour
{
    public int movementSpeed;
    public GameObject projectile;
    private float projectileCooldown = 0f;
    public GameObject mainCamera;


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        gameObject.name = Guid.NewGuid().ToString();
        gameObject.transform.position = new Vector3(-3, 3,-1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKey(KeyCode.W)) movementServerRpc(new Vector3(0, 0, 1));
        if (Input.GetKey(KeyCode.S)) movementServerRpc(new Vector3(0, 0, -1));
        if (Input.GetKey(KeyCode.A)) movementServerRpc(new Vector3(-1, 0, 0));
        if (Input.GetKey(KeyCode.D)) movementServerRpc(new Vector3(1, 0, 0));

        if (Input.GetKeyUp(KeyCode.Mouse0)) projectileServerRpc();

        mainCamera.transform.position = gameObject.transform.position + new Vector3(0, 20, -20);        
    }

    [ServerRpc]
    private void movementServerRpc(Vector3 moveDir)
    {
        transform.position += moveDir * Time.deltaTime * movementSpeed;
    }

    [ServerRpc]
    private void projectileServerRpc()
    {
        var newProjectile = Instantiate(projectile);
        newProjectile.GetComponent<projectileScript>().playerOriginName = gameObject.name;
    }
}
