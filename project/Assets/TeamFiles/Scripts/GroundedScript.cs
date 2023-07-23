using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GroundedScript : NetworkBehaviour
{
    // lists and variables
    public List<Vector3> rayDirections = new List<Vector3>();
    public List<float> rayLengths = new List<float>();
    public List<bool> rayHits = new List<bool>();
    public List<string> rayHitNames = new List<string>();
    public List<string> rayHitTags = new List<string>();

    void Start()
    {
        if (!IsServer) return;
        InitializeRays();
    }

    public bool GroundedCheck(Transform playerTransform)
    {
        CastGroundRays(playerTransform);
        // if(CheckIfTrigger() || CheckIfLava()) return false;
        if(!CheckHitTag("Terrain") || !CheckHitName("Terrain")) return false;
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

    public bool CheckIfGrounded()
    {
        return (rayHits[0] || rayHits[1] || rayHits[2] || rayHits[3] || rayHits[4]);
    }

    public bool CheckHitTag(string hitTag)
    {
        return (rayHitTags[0] == hitTag || rayHitTags[1] == hitTag || rayHitTags[2] == hitTag || rayHitTags[3] == hitTag || rayHitTags[4] == hitTag);
    }

    public bool CheckHitName(string hitName)
    {
        return (rayHitTags[0] == hitName || rayHitTags[1] == hitName || rayHitTags[2] == hitName || rayHitTags[3] == hitName || rayHitTags[4] == hitName);
    }
}