using UnityEngine;

/*
 * PathWalker
 * A pathwalker creates a path in the map.
 */ 

public class PathWalker : MonoBehaviour {

    /*
     * Private members
     */

    MapGenerator mapGenerator;      // The associated map generator.
    Vector2 position;               // Position of the pathwalker.
    PathWalkerDirection direction;  // The direction of the pathwalker.
    int maxSteps;                   // The max steps the pathwalker can walk.
    int steps = 0;                  // The current number of steps.

    /*
     * Methods
     */

    private void Start() {
        direction = (PathWalkerDirection)Random.Range(0, 3);
    }

    // Initialize a new pathwalker.
    public void Initialize(Vector2 _position, int _maxSteps) {
        mapGenerator = MapGenerator.Instance;
        position = _position;
        maxSteps = _maxSteps;
        Bounds bounds = GetComponent<SpriteRenderer>().sprite.bounds;
        transform.localScale = new Vector3(1 / bounds.size.x * mapGenerator.mapScale, 1 / bounds.size.y * mapGenerator.mapScale, 1);
        mapGenerator.PathWalkers.Add(this);
    }

    // Update a pathwalker position.
    public void UpdatePathWalker() {

        steps++;

        transform.position = position * mapGenerator.mapScale;


        mapGenerator.SetFloor(position);

        if (mapGenerator.FloorCount > mapGenerator.maxFloorCount) {
            DestroyPathWalker();
        }

        SpawnPathWalker();
        SpawnRoom();
        SetDirection();
        

        if (steps == maxSteps) {

            DestroyPathWalker();
        }
        MoveForward();

    }

    // Destroy a pathwalker.
    public void DestroyPathWalker() {
        mapGenerator.SpawnCollectible(position, CollectibleType.AmmoChest);
        if (mapGenerator.pathWalkersContainer.childCount == 1) {
            mapGenerator.EndGeneration();
        }
        Destroy(gameObject);
    }

    // Set a new direction for the pathwalker.
    private void SetDirection() {
        int random = Random.Range(0, 101);
        if (random <= mapGenerator.chanceForward) {
            
        }
        else if (random <= mapGenerator.PurcentageLeft) {
            switch (direction) {
                case PathWalkerDirection.Right:
                    direction = PathWalkerDirection.Up;
                    break;
                case PathWalkerDirection.Left:
                    direction = PathWalkerDirection.Down;
                    break;
                case PathWalkerDirection.Up:
                    direction = PathWalkerDirection.Left;
                    break;
                case PathWalkerDirection.Down:
                    direction = PathWalkerDirection.Right;
                    break;
            }
        }
        else if (random <= mapGenerator.PurcentageRight) {
            switch (direction) {
                case PathWalkerDirection.Right:
                    direction = PathWalkerDirection.Down;
                    break;
                case PathWalkerDirection.Left:
                    direction = PathWalkerDirection.Up;
                    break;
                case PathWalkerDirection.Up:
                    direction = PathWalkerDirection.Right;
                    break;
                case PathWalkerDirection.Down:
                    direction = PathWalkerDirection.Left;
                    break;
            }
        }
        else if (random <= mapGenerator.PurcentageBackward) {

            switch (direction) {
                case PathWalkerDirection.Right:
                    direction = PathWalkerDirection.Left;
                    break;
                case PathWalkerDirection.Left:
                    direction = PathWalkerDirection.Right;
                    break;
                case PathWalkerDirection.Up:
                    direction = PathWalkerDirection.Down;
                    break;
                case PathWalkerDirection.Down:
                    direction = PathWalkerDirection.Up;
                    break;
            }
            mapGenerator.SpawnCollectible(position, CollectibleType.WeaponChest);
        }
    }

    // Spawn a new pathwalker.
    private void SpawnPathWalker() {
        int random = Random.Range(0, 101);
        if (random < mapGenerator.PathWalkerChanceToSpawn) {
            PathWalker pathWalker = Instantiate(mapGenerator.pathWalkerPrefab, transform.position, Quaternion.identity, mapGenerator.pathWalkersContainer).GetComponent<PathWalker>();
            pathWalker.Initialize(position, maxSteps / 2);
        }
    }

    // Create a floor at the position of the pathwalker.
    private void SpawnRoom() {
        int random = Random.Range(0, 101);
        if (random < mapGenerator.roomChanceToSpawn) {

            Vector2 roomOrigin;
            switch (direction) {
                case PathWalkerDirection.Right:
                    roomOrigin = position;
                    break;
                case PathWalkerDirection.Left:
                    roomOrigin = new Vector2(position.x - mapGenerator.roomSizeX + 1, position.y - mapGenerator.roomSizeY + 1);
                    break;
                case PathWalkerDirection.Up:
                    roomOrigin = new Vector2(position.x - mapGenerator.roomSizeX + 1, position.y + mapGenerator.roomSizeY - 1);
                    break;
                case PathWalkerDirection.Down:
                    roomOrigin = new Vector2(position.x + mapGenerator.roomSizeX - 1, position.y - mapGenerator.roomSizeY + 1);
                    break;
            }

            for (int x = 0; x < mapGenerator.roomSizeX; x++) {
                for (int y = 0; y < mapGenerator.roomSizeY; y++) {
                    mapGenerator.SetFloor(new Vector2(position.x + x , position.y + y));
                }
            }
        }
    }

    // Move the pathwalker forward.
    private void MoveForward() {
        switch (direction) {
            case PathWalkerDirection.Right:
                position.x += 1;
                break;
            case PathWalkerDirection.Left:
                position.x -= 1;
                break;
            case PathWalkerDirection.Up:
                position.y += 1;
                break;
            case PathWalkerDirection.Down:
                position.y -= 1;
                break;
        }
    }
}
