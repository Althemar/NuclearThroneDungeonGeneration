using System.Collections.Generic;
using UnityEngine;

/*
 * MapGenerator
 * A Nuclear Throne maps generator using a pathwalkers technique.
 */

public class MapGenerator : MonoBehaviour {

    /*
     * Public members
     */

    public static MapGenerator Instance;        // Map generator singleton.

    [Header("Tile Prefabs")]
    public GameObject floorPrefab;              // Prefab for the floor tile.
    public GameObject wallPrefab;               // Prefab for the wall tile.
    public Transform tilesContainers;           // Transform parent of all tiles.

    [Header("Collectibles prefabs")]
    public GameObject weaponChestPrefab;        // Prefab for the weapon chests.
    public GameObject ammoChestPrefab;          // Prefab for the ammo chests.
    public Transform collectiblesContainers;    // Transform parent of all collectibles.

    [Header("Actors prefabs")]
    public GameObject playerPrefab;             // Prefab for the player to spawn.
    public GameObject pathWalkerPrefab;         // Prefab for the pathwalkers.
    public Transform pathWalkersContainer;      // Trasform parent of the pathwalkers.
    
    [Header("Map Size")]
    public int maxFloorCount = 110;             // The maximum number of floors in the map.
    public int xSize = 50;                      // The x size of the map.
    public int ySize = 50;                      // The y size of the map.
    public float mapScale = 1;                  // The scale of the map.

    [Header("Path Walker Attributes")]
    public int PathWalkerDistance = 10;         // The max pathwalker number of steps.
    public int PathWalkerChanceToSpawn;         // The chance of a pathwalker to spawn an other pathwalker.

    public int chanceForward;                   // The chance of a pathwalker to go forward.
    public int chanceRight;                     // The chance of a pathwalker to go right.
    public int chanceLeft;                      // The chance of a pathwalker to go left.
    public int chanceBackward;                  // The chance of a pathwalker to go backward.           

    [Header("Room Attributes")]
    public int roomChanceToSpawn;               // The chance of pathwalker to spawn a room.
    public int roomSizeX;                       // The x size of a room spawned by a pathwalker.
    public int roomSizeY;                       // The y size of a room spawned by a pathwalker.
    
    /*
     * Private members
     */

    Vector2 origin;                 // The origin of the first pathwalker.

    List<PathWalker> pathWalkers;   // The list of pathwalkers.
    List<GameObject> ammoChests;    // The list of ammo chests.
    List<GameObject> weaponChests;  // The list of weapon chests.

    List<List<GameObject>> tiles;   // The list of tiles.

    int purcentageForward;          // The purcentage of going forward.
    int purcentageLeft;             // The purcentage of going left.
    int purcentageRight;            // The purcentage of going right.
    int purcentageBackward;         // the purcentage of going backward.

    int floorCount = 0;             // The current number of floor tiles.

    int frameCount = 0;             // Frame count to control the speed of pathwalkers updates.

    /*
     * Properties
     */

    public List<List<GameObject>> Tiles
    {
        get { return tiles; }
    }

    public int PurcentageForward
    {
        get { return purcentageForward; }
    }

    public int PurcentageLeft
    {
        get { return purcentageLeft; }
    }

    public int PurcentageRight
    {
        get { return purcentageRight; }
    }

    public int PurcentageBackward
    {
        get { return purcentageBackward; }
    }

    public int FloorCount
    {
        get { return floorCount; }
    }

    public List<PathWalker> PathWalkers
    {
        get { return pathWalkers; }
    }

    /*
     * Methods
     */ 

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
    
    void Start () {
        pathWalkers = new List<PathWalker>();
        ammoChests = new List<GameObject>();
        weaponChests = new List<GameObject>();
    }

    // Initialize the map generation
    public void InitGeneration() {
        origin = new Vector2(xSize * mapScale / 2, ySize * mapScale / 2);
        Camera.main.transform.position = new Vector3(origin.x, origin.y, Camera.main.transform.position.z);

        // Destroy all previous tiles, collectibles and pathwalkers
        foreach (Transform tile in tilesContainers) {
            Destroy(tile.gameObject);
        }

        foreach (Transform collectible in collectiblesContainers) {
            Destroy(collectible.gameObject);
        }

        foreach (Transform walker in pathWalkersContainer) {
            Destroy(walker.gameObject);
        }

        // Clear all lists
        pathWalkers.Clear();
        ammoChests.Clear();
        weaponChests.Clear();

        // Fill the list with empty tiles
        tiles = new List<List<GameObject>>();
        for (int x = 0; x < xSize; x++) {
            tiles.Add(new List<GameObject>());
            for (int y = 0; y < ySize; y++) {
                tiles[x].Add(null);
            }
        }

        // Initialize purcentage
        purcentageForward = chanceForward;
        purcentageLeft = purcentageForward + chanceLeft;
        purcentageRight = purcentageLeft + chanceRight;
        purcentageBackward = purcentageRight + chanceBackward;

        // Create a new pathwalker
        PathWalker pathWalker = Instantiate(pathWalkerPrefab, origin, Quaternion.identity, pathWalkersContainer).GetComponent<PathWalker>();
        pathWalker.Initialize(new Vector2(xSize / 2, ySize / 2), PathWalkerDistance);
    }

    private void Update() {
        frameCount++;
        if (frameCount == 10) {
            frameCount = 0;
            for (int i = 0; i < pathWalkers.Count; i++) {
                if (pathWalkers[i]) {
                    pathWalkers[i].UpdatePathWalker();
                }
            }
        }
    }

    // End the generation when all pathwalkers are destroyed
    public void EndGeneration() {
        // Keep one chest of each type
        KeepFurthestChest(CollectibleType.WeaponChest);
        KeepFurthestChest(CollectibleType.AmmoChest);

        // Spawn the player
        SpriteRenderer player = Instantiate(playerPrefab, origin, Quaternion.identity, pathWalkersContainer).GetComponent<SpriteRenderer>();
        Bounds bounds = player.sprite.bounds;
        var xSize = bounds.size.x;
        player.transform.localScale = new Vector3(1 / xSize * mapScale, 1 / bounds.size.y * mapScale, 1);

        // Create wall tiles around the path
        for (int x = 0; x < tiles.Count; x++) {
            for (int y = 0; y < tiles[x].Count; y++) {
                if (tiles[x][y]) {
                    Tile tile = tiles[x][y].GetComponent<Tile>();
                    if (tiles[x][y] != null && tile.tileType == TileType.Floor) {
                        for (int xWall = x - 1; xWall <= x + 1; xWall++) {
                            for (int yWall = y - 1; yWall <= y + 1; yWall++) {
                                if (tiles[xWall][yWall] == null) {
                                    GameObject g = Instantiate(wallPrefab, new Vector2(xWall * mapScale, yWall * mapScale), Quaternion.identity, tilesContainers);
                                    bounds = g.GetComponent<SpriteRenderer>().sprite.bounds;
                                    g.transform.localScale = new Vector3(1 / bounds.size.x * mapScale, 1 / bounds.size.y * mapScale, 1);
                                    tiles[xWall][yWall] = g;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // Create a new floor tile
    public void SetFloor(Vector2 position) {
        int posX = (int)position.x;
        int posY = (int)position.y;
        GameObject tile = tiles[posX][posY];
        if (tile == null || tile.GetComponent<Tile>().tileType == TileType.Wall) {
            GameObject g = Instantiate(floorPrefab, new Vector2(posX * mapScale, posY * mapScale), Quaternion.identity, tilesContainers);
            Bounds bounds = g.GetComponent<SpriteRenderer>().sprite.bounds;
            g.transform.localScale = new Vector3(1 / bounds.size.x * mapScale, 1 / bounds.size.y * mapScale, 1);       
            tiles[posX][posY] = g;
            floorCount++;
        }
    }

    // Spawn a collectible
    public void SpawnCollectible(Vector2 position, CollectibleType collectibleType) {
        int posX = (int)position.x;
        int posY = (int)position.y;

        Tile tile = tiles[posX][posY].GetComponent<Tile>();
        if (tile.tileType == TileType.Wall) {
            return;
        }
        
        tile.CollectibleType = collectibleType;

        GameObject collectibleToSpawn = null;

        List<GameObject> listToAddObject = new List<GameObject>();
        if (collectibleType == CollectibleType.WeaponChest) {
            collectibleToSpawn = weaponChestPrefab;
            listToAddObject = weaponChests;
        }
        else if (collectibleType == CollectibleType.AmmoChest) {
            collectibleToSpawn = ammoChestPrefab;
            listToAddObject = ammoChests;

        }

        GameObject g = Instantiate(collectibleToSpawn, new Vector2(posX * mapScale, posY * mapScale), Quaternion.identity, collectiblesContainers);
        Bounds bounds = g.GetComponent<SpriteRenderer>().sprite.bounds;
        var xSize = bounds.size.x;
        g.transform.localScale = new Vector3(0.5f / xSize * mapScale, 0.5f / bounds.size.y * mapScale, 1);

        listToAddObject.Add(g);
    }

    // Keep the furthest collectible of one type from the player spawn
    public void KeepFurthestChest(CollectibleType collectibleType) {
        List<GameObject> collectibles = new List<GameObject>();
        if (collectibleType == CollectibleType.WeaponChest) {
            collectibles = weaponChests;
        }
        else if (collectibleType == CollectibleType.AmmoChest) {
            collectibles = ammoChests;
        }

        float maxDistance = 0;
        int keepIndex = 0;

        for (int i = 0; i < collectibles.Count; i++) {
            float distance = Vector2.Distance(origin, collectibles[i].transform.position);
            if (distance  >= maxDistance) {
                maxDistance = distance;
                keepIndex = i;
            }
        }

        for (int i = 0; i < collectibles.Count; i++) {
            if (i != keepIndex) {
                Destroy(collectibles[i]);
            }
        }
    }
}
