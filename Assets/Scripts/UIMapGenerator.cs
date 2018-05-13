using UnityEngine;
using UnityEngine.UI;

/*
 * UIMapGenerator
 * Handle the UI to change the properties of the generator
 */

public class UIMapGenerator : MonoBehaviour {

    public MapGenerator mapGenerator;   // The associated map generator

    [Header("Pathwalker variables")]
    public InputField maxDistance;      
    public InputField spawnChance;
    public InputField forward;  
    public InputField right;
    public InputField left;
    public InputField backward;

    [Header("Room variables")]
    public InputField roomSpawn;
    public InputField roomSizeX;
    public InputField roomSizeY;

    public void Start() {
        maxDistance.text    = mapGenerator.PathWalkerDistance.ToString();
        spawnChance.text    = mapGenerator.PathWalkerChanceToSpawn.ToString();
        forward.text        = mapGenerator.chanceForward.ToString();
        right.text          = mapGenerator.chanceRight.ToString();
        left.text           = mapGenerator.chanceLeft.ToString();
        backward.text       = mapGenerator.chanceBackward.ToString();
        roomSpawn.text      = mapGenerator.roomChanceToSpawn.ToString();
        roomSizeX.text      = mapGenerator.roomSizeX.ToString();
        roomSizeY.text      = mapGenerator.roomSizeY.ToString();
    }

    public void SetMaxDistance(string distance) {
        mapGenerator.PathWalkerDistance = int.Parse(distance);
    }

    public void SetSpawnChance(string spawnChance) {
        mapGenerator.PathWalkerChanceToSpawn = int.Parse(spawnChance);
    }

    public void SetForward(string forward) {
        mapGenerator.chanceForward = int.Parse(forward);
    }

    public void SetRight(string right) {
        mapGenerator.chanceRight = int.Parse(right);
    }

    public void SetLeft(string left) {
        mapGenerator.chanceLeft = int.Parse(left);
    }

    public void SetBackward(string backward) {
        mapGenerator.chanceBackward = int.Parse(backward);
    }

    public void SetRoomChance(string roomChance) {
        mapGenerator.roomChanceToSpawn = int.Parse(roomChance);
    }

    public void SetRoomSizeX(string roomSizeX) {
        mapGenerator.roomSizeX = int.Parse(roomSizeX);
    }

    public void SetRoomSizeY(string roomSizeY) {
        mapGenerator.roomSizeY = int.Parse(roomSizeY);
    }
}
