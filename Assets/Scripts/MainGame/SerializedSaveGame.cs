using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class SerializedSaveGame
{
    public int playerHPNew;
    public int currentWaypointIndex;

    public float playerPositionX, playerPositionY, playerPositionZ;
    public float playerRotationX, playerRotationY, playerRotationZ;
    
    
}
