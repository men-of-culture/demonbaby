using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class playerScript : NetworkBehaviour
{
    public int movementSpeed;
    public GameObject projectile;
    
    private float projectileCooldown;
    // Start is called before the first frame update
    void Start()
    {
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

        // projectileServerRpc();
        
    }

    [ServerRpc]
    private void movementServerRpc(Vector3 moveDir)
    {
        transform.position += moveDir * Time.deltaTime * movementSpeed;
    }

    // [ServerRpc]
    // private void projectileServerRpc()
    // {
    //     if (Input.GetKeyUp(KeyCode.Mouse0))
    //     {
    //         if (projectileCooldown >= 2 )
    //         {
    //             projectileCooldown = 0;
    //             var newProjectile = Instantiate(projectile);
    //             newProjectile.GetComponent<projectileScript>().playerOrigin = gameObject;
    //         }
            
    //     }

    //     if (projectileCooldown < 2)
    //     {
    //         projectileCooldown += Time.deltaTime;
    //     }
    // }
}
