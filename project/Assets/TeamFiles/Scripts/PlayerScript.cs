using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;

public class PlayerScript : NetworkBehaviour
{
    public int movementSpeed;
    public int jumpHeight;
    public int lookAtMouseSpeed;
    public int knockbackForce;
    public Camera mainCamera;
    public GameObject projectilePrefab;
    public GameObject myProjectile;
    public CharacterController characterController;
    public float shotCooldown = 0.5f;
    public float shotCooldownTimer = 0.0f;
    public NetworkVariable<bool> grounded;

    void Start()
    {
        // clients
        SetPlayerReferences();

        // owner
        if(IsOwner)
        {
            mainCamera = Camera.main;
        }

        // server
        if (!IsServer) return;
    }

    void Update()
    {
        // clients

        // owner
        if (IsOwner)
        {
            PlayerCamera();
            PlayerLookAtMouse();
            PlayerMovement();
            PlayerJump();
            PlayerShot();
            RaycastForward();
        }
        
        // server
        if (!IsServer) return;
        PlayerGroundedCheck();
        PlayerGravity();
        PlayerShotServerSide();
    }

    private void SetPlayerReferences()
    {
        characterController = GetComponent<CharacterController>();
        gameObject.name = "Player"+((float)GetComponent<NetworkObject>().OwnerClientId).ToString();
    }

    private void RaycastForward()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1f, Color.white);
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f))
        {
            if(hit.transform.gameObject == null) return;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Ray hit "+hit.transform.gameObject.name);
        }
    }

    private void PlayerGroundedCheck()
    {
        if(characterController.isGrounded) grounded.Value = true;
        else grounded.Value = false;
    }

    private void PlayerGravity()
    {
        if(grounded.Value != false) return;
        characterController.Move(new Vector3(0, -9.82f, 0) * Time.deltaTime);
    }

    private void PlayerCamera()
    {
        mainCamera.gameObject.transform.position = gameObject.transform.position + new Vector3(0, 10, -10);
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

    private void PlayerMovement()
    {
        var moveDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W)) moveDir.z += 1;
        if (Input.GetKey(KeyCode.S)) moveDir.z += -1;
        if (Input.GetKey(KeyCode.A)) moveDir.x += -1;
        if (Input.GetKey(KeyCode.D)) moveDir.x += 1;
        if(moveDir == new Vector3(0, 0, 0)) return;

        PlayerMovementServerRpc(moveDir);
    }

    [ServerRpc]
    private void PlayerMovementServerRpc(Vector3 moveDir)
    {
        characterController.Move(moveDir.normalized * Time.deltaTime * movementSpeed);
    }

    private void PlayerJump()
    {
        if(grounded.Value == true & Input.GetKeyDown(KeyCode.Space)){
            PlayerJumpServerRpc();
        }
    }

    [ServerRpc]
    private void PlayerJumpServerRpc()
    {
        characterController.Move(new Vector3(0, 1, 0) * jumpHeight);
    }

    private void PlayerShot()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0) && IsOwner){
            PlayerShotServerRpc();
        }
    }

    [ServerRpc]
    private void PlayerShotServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if(shotCooldownTimer >= shotCooldown){
            myProjectile = Instantiate(projectilePrefab, transform.position + gameObject.transform.forward, transform.rotation);
            myProjectile.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
            shotCooldownTimer = 0;
        }
    }

    private void PlayerShotServerSide()
    {
        if(shotCooldownTimer < shotCooldown){
            shotCooldownTimer += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!IsServer) return;
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
}