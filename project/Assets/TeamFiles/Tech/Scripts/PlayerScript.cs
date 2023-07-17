using UnityEngine;
using Unity.Netcode;
using System;
using System.Linq;

public class PlayerScript : NetworkBehaviour
{

    public int movementSpeed;
    public int jumpHeight;
    public int lookAtMouseSpeed;
    public int knockbackForce;
    public int health;
    public bool controlsDisabled;
    public GameObject projectile;
    public Camera mainCamera;
    public CharacterController characterController;
    public GameManagerScript gms;
    public NetworkVariable<bool> grounded;
    public bool isAlive;
    public bool isReady;
    public bool allReady; // should be deleted when raycast is getting introduced to groundedPlayers

    void Start()
    {
        if (IsServer) 
        {
            gms = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            gms.AddPlayer(gameObject);
            SpawnPlayers();
        }

        if (IsOwner) 
        {
            mainCamera = Camera.main;
        }
    }

    void OnDestroy()
    {
        if (IsServer)
        {
            gms.RemovePlayer(gameObject);
        }
    }

    void Update()
    {
        if (IsServer)
        {
            if (gms.listOfPlayers.Where(x => x.GetComponent<PlayerScript>().isReady == true).ToList().Count == gms.listOfPlayers.Count && !allReady)
            {
                controlsDisabled = false;
                GroundPlayer();
            }
        }

        if (IsOwner) 
        {
            PlayerCamera();
            PlayerLookAtMouse();
            PlayerMovement();
            PlayerShoot();
            ReadyUp();
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

    private void ReadyUp()
    {
        if (Input.GetKeyDown(KeyCode.Space)) ReadyUpServerRpc();
    }

    [ServerRpc]
    private void ReadyUpServerRpc()
    {
        if (!IsServer) return;
        isReady = true;
        // gms.StartGame(); //Todo: implement canvas with coundown / waiting for players to ready up
    }

    [ServerRpc]
    private void PlayerMovementServerRpc(Vector3 moveDir)
    {
        if(controlsDisabled) return;
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
        if(controlsDisabled) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookAtMouseSpeed * Time.deltaTime);
    }

    private void PlayerShoot()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0) && IsOwner) PlayerShootServerRpc();
    }

    [ServerRpc]
    private void PlayerShootServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if(controlsDisabled) return;
        var myProjectile = Instantiate(projectile, transform.position + gameObject.transform.forward, transform.rotation);
        myProjectile.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
    }

    private void GroundPlayer() 
    {
        if(characterController.isGrounded) 
        {
            allReady = true;
            return;
        }
        else characterController.Move(new Vector3(0, -9.82f, 0) * Time.deltaTime);
    }

    [ClientRpc]
    private void PlayerDeathClientRPC()
    {
        if (IsOwner)
        {
            GameObject.Find("DeathCanvas").GetComponent<Canvas>().enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!IsServer) return;

        if (other.gameObject.tag == "Lava")
        {
            health--;
            if (health < 1)
            {
                // TODO : Disable Player Mesh on Death.
                GetComponent<AudioSource>().Play();
                PlayerDeathClientRPC();
                isAlive = false;
                controlsDisabled = true;
                gms.EndGame();
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

    private void SpawnPlayers()
    {
        foreach (var player in gms.listOfPlayers)
        {
            double radius = (Math.PI / 180) * (360 / gms.listOfPlayers.Count);
            int index = gms.listOfPlayers.FindIndex(x => x.Equals(player));
            player.gameObject.transform.position = new Vector3((float)Math.Cos(radius * index), 0, (float)Math.Sin(radius * index)).normalized * 5;
        }
    }
}
