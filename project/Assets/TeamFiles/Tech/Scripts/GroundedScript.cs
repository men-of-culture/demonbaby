using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GroundedScript : NetworkBehaviour
{
    // lists and variables
    public bool grounded = false;
    public bool onTerrain = false;
    public List<Vector3> rayDirections = new List<Vector3>();
    public List<float> rayLengths = new List<float>();
    public List<bool> rayHits = new List<bool>();
    public List<string> rayHitNames = new List<string>();

    void FixedUpdate()
    {
        if (!IsServer) return;
        // direction of vector
        rayDirections.AddRange(new Vector3[] {Vector3.down, new Vector3(1, -3, -1), new Vector3(1, -3, 1), new Vector3(-1, -3, 1), new Vector3(-1, -3, -1)});

        // length of rays
        // Mathf.Infinity
        rayLengths.AddRange(new float[] {1.2f, 1.2f, 1.2f, 1.2f, 1.2f});

        // hit state of rays
        rayHits.AddRange(new bool[] {false, false, false, false, false});

        // hit name of rays
        rayHitNames.AddRange(new string[] {"none", "none", "none", "none", "none"});
        
    }

    public void castGroundRays(Transform playerTransform)
    {
        if (!IsServer) return;
        // raycast
        RaycastHit hit;

        // Does the ray intersect any objects including/excluding only the selected layers(layerMask)
        for (int i = 0; i < 5; i++)
        {
            if (Physics.Raycast(playerTransform.position, playerTransform.TransformDirection(rayDirections[i]), out hit,
                rayLengths[i], LayerMask.GetMask("Default")))
            {
                // use this if you need to debug
                // Debug.DrawRay(transform.position, transform.TransformDirection(rayDirections[i]) * hit.distance,
                //     Color.red);
                rayHits[i] = true;
                rayHitNames[i] = hit.collider.gameObject.tag;
            }
            else
            {
                Debug.DrawRay(playerTransform.position, playerTransform.TransformDirection(rayDirections[i]) * 1000, Color.white);
                rayHits[i] = false;
                rayHitNames[i] = "none";
            }
        }
    }

    public bool checkIfGrounded()
    {
        return IsServer ? rayHits[0] || rayHits[1] || rayHits[2] || rayHits[3] || rayHits[4] ? false : true : false;
        // if (IsServer)
        // {
        //     // ground player if hit ground
        //     Debug.Log($"grounded: {grounded}");
        //     return (rayHits[0] || rayHits[1] || rayHits[2] || rayHits[3] || rayHits[4]);
        // }
    }

    public bool checkIfOnTerrain()
    {
        return IsServer ? (rayHitNames[0] == "Terrain" || rayHitNames[1] == "Terrain" || rayHitNames[2] == "Terrain" || rayHitNames[3] == "Terrain" || rayHitNames[4] == "Terrain") ? false : true : false;
        // if (!IsServer) 
        // // check if player is on terrain
        // Debug.Log($"onTerrain: {onTerrain}");
        // return (rayHitNames[0] == "Terrain" || rayHitNames[1] == "Terrain" || rayHitNames[2] == "Terrain" || rayHitNames[3] == "Terrain" || rayHitNames[4] == "Terrain");
    }
}