using System;
using Unity.Netcode;
using UnityEngine;

public class ProjectileScript : NetworkBehaviour
{
    public int projectileSpeed;
    public float projectileTimer = 0.0f;

    void Start()
    {
        if(!IsServer) return;
    }

    void Update()
    {
        if (!IsServer) return;
        Vector3 vec3 = transform.forward * Time.deltaTime * projectileSpeed;
        transform.position += vec3;
        if(projectileTimer < 1)
        {
            projectileTimer += Time.deltaTime;
        }
        if(projectileTimer >= 1)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}