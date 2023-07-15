using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerScript : NetworkBehaviour
{

    public int movementSpeed;
    public int jumpHeight;
    public int lookAtMouseSpeed;
    public int knockbackForce;
    public int health;
    public bool controlsDisabled = false;
    public bool ready = false;
    public GameObject projectile;
    public Camera mainCamera;
    public Renderer playerMesh;
    public CharacterController characterController;
    public Canvas deathCanvas;
    public GameManagerScript gms;
    public NetworkVariable<bool> grounded;
    public NetworkVariable<bool> playerDead;
    public GameState gameState;
    public int readyCount;

    void Start()
    {
        if (IsServer) {
            gameState.list.Add(new GameState.PlayerState{ id = gameObject.GetComponent<NetworkObject>().OwnerClientId});
            gms = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            gms.addPlayer(gameObject.GetComponent<NetworkObject>().OwnerClientId);
        }

        if (IsOwner) {
            mainCamera = Camera.main;
        }

        SpawnPlayersServerRpc(); // Todo: fix positions on all players
    }

    void Update()
    {
        if (IsOwner) {
            if (gms.readyUpPlayers.Count == 0)
            {
                if (controlsDisabled) return;
                PlayerCamera();
                PlayerLookAtMouse();
                PlayerMovement();
                PlayerShoot();
            }
            ReadyUp(); // Todo: Not working atm
        }

        if (!IsServer) return;
        Debug.Log(gms.readyUpPlayers.Count);
        if (gms.readyUpPlayers.Count == 0)
        {
            PlayerGroundedCheck();
            PlayerGravity();
        }
    }
    
    private void PlayerCamera() 
    {
        mainCamera.gameObject.transform.position = gameObject.transform.position + new Vector3(0, 10, -10);
    }

    private void PlayerMovement() 
    {
        var moveDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W)) moveDir.z += 1;
        if (Input.GetKey(KeyCode.A)) moveDir.x += -1;
        if (Input.GetKey(KeyCode.S)) moveDir.z += -1;
        if (Input.GetKey(KeyCode.D)) moveDir.x += 1;

        if(moveDir == new Vector3(0, 0, 0)) return;

        PlayerMovementServerRpc(moveDir);

    } 

    [ServerRpc]
    private void PlayerMovementServerRpc(Vector3 moveDir)
    {
        characterController.Move(moveDir.normalized * Time.deltaTime * movementSpeed);
    }

        private void PlayerLookAtMouse()
    {
        if (mainCamera is not { }) return;

        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = mainCamera.ScreenPointToRay (Input.mousePosition);
        float hitdist = 0.0f;

        if (!playerPlane.Raycast (ray, out hitdist)) return;

        Vector3 targetPoint = ray.GetPoint(hitdist);
        Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
        PlayerLookAtMouseServerRpc(targetRotation);
    }

    [ServerRpc]
    private void PlayerLookAtMouseServerRpc(Quaternion targetRotation)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookAtMouseSpeed * Time.deltaTime);
    }

    private void PlayerShoot()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0) && IsOwner){
            PlayerShootServerRpc();
        }
    }

    [ServerRpc]
    private void PlayerShootServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var myProjectile = Instantiate(projectile, transform.position + gameObject.transform.forward, transform.rotation);
        myProjectile.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
    }

    private void PlayerGroundedCheck() {
        if(characterController.isGrounded) grounded.Value = true;
        else grounded.Value = false;
    }

    private void PlayerGravity() {
        if(grounded.Value) return;
        characterController.Move(new Vector3(0, -9.82f, 0) * Time.deltaTime);
    }

    [ClientRpc]
    private void PlayerDeathClientRPC() {
        if (IsOwner)  {
            GameObject.Find("DeathCanvas").GetComponent<Canvas>().enabled = true;
            controlsDisabled = true;
            deathCanvas.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!IsServer) return;
        if (other.gameObject.tag == "Lava"){
            health--;
            if (health < 1) {
                // TODO : Disable Player Mesh on Death.
                playerDead.Value = true;
                PlayerDeathClientRPC();
                gms.removePlayer(gameObject.GetComponent<NetworkObject>().OwnerClientId);
                gms.endGame();
            }
        }
        if (other.gameObject.name == "Projectile(Clone)" && other.GetComponent<NetworkObject>().OwnerClientId != OwnerClientId)
        {
            Vector3 vec3 = gameObject.transform.position - other.transform.position;
            vec3 = new Vector3(vec3.x, 0.0f, vec3.z).normalized * Time.deltaTime * knockbackForce;
            characterController.Move(vec3);
        }
        if (other.gameObject.name == "ResetTrigger")
        {
            characterController.enabled = false;
            transform.position = new Vector3(0, 10, 0);
            characterController.enabled = true;
        }
    }

    [ServerRpc]
    private void SpawnPlayersServerRpc()
    {
        foreach (var player in gameState.list)
        {
            double radius = (Math.PI / 180) * (360 / gameState.list.Count);
            int index = gameState.list.FindIndex(x => x.id == gameObject.GetComponent<NetworkObject>().OwnerClientId);
            gameObject.GetComponent<NetworkObject>().transform.position = new Vector3((float)Math.Cos(radius * index), 0, (float)Math.Sin(radius * index)).normalized * 5;
        }
    }

    private void ReadyUp() 
    {
        if (Input.GetKey(KeyCode.Space)) ReadyUpServerRpc();
    }

    [ServerRpc]
    private void ReadyUpServerRpc()
    {
        gms.readyPlayer(gameObject.GetComponent<NetworkObject>().OwnerClientId);
    }

}
