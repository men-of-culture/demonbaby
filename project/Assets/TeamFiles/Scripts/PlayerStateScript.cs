using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayerState", order = 1)]
public class PlayerState : ScriptableObject
{
    public bool isHost = false;
}
