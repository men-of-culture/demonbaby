using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GroundedScript : NetworkBehaviour
{
    // lists and variables
    // public bool grounded = false;
    // public bool onTerrain = false;
    public List<Vector3> rayDirections = new List<Vector3>();
    public List<float> rayLengths = new List<float>();
    public List<bool> rayHits = new List<bool>();
    public List<string> rayHitNames = new List<string>();
    public List<string> rayHitTags = new List<string>();

    void FixedUpdate()
    {
        // if (!IsServer) return;
        // // direction of vector
        // rayDirections.AddRange(new Vector3[] {Vector3.down, new Vector3(1, -3, -1), new Vector3(1, -3, 1), new Vector3(-1, -3, 1), new Vector3(-1, -3, -1)});

        // // length of rays
        // // Mathf.Infinity
        // rayLengths.AddRange(new float[] {1.2f, 1.2f, 1.2f, 1.2f, 1.2f});

        // // hit state of rays
        // rayHits.AddRange(new bool[] {false, false, false, false, false});

        // // hit name of rays
        // rayHitNames.AddRange(new string[] {"none", "none", "none", "none", "none"});
    }

    void Start()
    {
        if (!IsServer) return;
        InitializeRays();
    }

    public bool GroundedCheck(Transform playerTransform)
    {
        CastGroundRays(playerTransform);
        //ResetTriggerCheck();
        if(CheckIfTrigger()) return false;
        return CheckIfGrounded();
    }

    public void InitializeRays()
    {
        // direction of vector
        rayDirections.AddRange(new Vector3[] {Vector3.down, new Vector3(1, -3, -1), new Vector3(1, -3, 1), new Vector3(-1, -3, 1), new Vector3(-1, -3, -1)});

        // length of rays
        // Mathf.Infinity
        rayLengths.AddRange(new float[] {1.2f, 1.2f, 1.2f, 1.2f, 1.2f});

        // hit state of rays
        rayHits.AddRange(new bool[] {false, false, false, false, false});

        // hit name of rays
        rayHitNames.AddRange(new string[] {"none", "none", "none", "none", "none"});

        // hit tag of rays
        rayHitTags.AddRange(new string[] {"none", "none", "none", "none", "none"});
    }

    public void CastGroundRays(Transform playerTransform)
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
                Debug.DrawRay(playerTransform.position, transform.TransformDirection(rayDirections[i]) * hit.distance,
                    Color.red);
                rayHits[i] = true;
                rayHitNames[i] = hit.collider.gameObject.name;
                rayHitTags[i] = hit.collider.gameObject.tag;
            }
            else
            {
                Debug.DrawRay(playerTransform.position, playerTransform.TransformDirection(rayDirections[i]) * 1000, Color.white);
                rayHits[i] = false;
                rayHitNames[i] = "none";
                rayHitTags[i] = "none";
            }
        }
    }

    public void ResetTriggerCheck()
    {
        if(rayHitNames[0] == "ResetTrigger" || rayHitNames[1] == "ResetTrigger" || rayHitNames[2] == "ResetTrigger" || rayHitNames[3] == "ResetTrigger" || rayHitNames[4] == "ResetTrigger")
        {
            GetComponent<CharacterController>().enabled = false;
            transform.position = new Vector3(0, 10, 0);
            GetComponent<CharacterController>().enabled = true;
        }
    }

    public bool CheckIfGrounded()
    {
        // return IsServer ? rayHits[0] || rayHits[1] || rayHits[2] || rayHits[3] || rayHits[4] ? false : true : false;
        // if (IsServer)
        // {
            // ground player if hit ground
            //Debug.Log($"grounded: {grounded}");
            //return (rayHits[0] || rayHits[1] || rayHits[2] || rayHits[3] || rayHits[4]);
        // }
        return (rayHits[0] || rayHits[1] || rayHits[2] || rayHits[3] || rayHits[4]);
    }

    public bool CheckIfOnTerrain()
    {
        // return IsServer ? (rayHitNames[0] == "Terrain" || rayHitNames[1] == "Terrain" || rayHitNames[2] == "Terrain" || rayHitNames[3] == "Terrain" || rayHitNames[4] == "Terrain") ? false : true : false;
        // if (!IsServer) 
        // check if player is on terrain
        // Debug.Log($"onTerrain: {onTerrain}");
        return (rayHitNames[0] == "Terrain" || rayHitNames[1] == "Terrain" || rayHitNames[2] == "Terrain" || rayHitNames[3] == "Terrain" || rayHitNames[4] == "Terrain");
    }

    public bool CheckIfTrigger()
    {
        return (rayHitTags[0] == "Trigger" || rayHitTags[1] == "Trigger" || rayHitTags[2] == "Trigger" || rayHitTags[3] == "Trigger" || rayHitTags[4] == "Trigger");
    }
}