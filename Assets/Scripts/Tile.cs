using UnityEngine;

/*
 * Tile
 * A tile in the map.
 */

public class Tile : MonoBehaviour {

    /*
     * Members
     */

    public TileType tileType;           // The type of the tile.

    CollectibleType collectibleType;    // The collectible on the tile.

    /*
     * Properties
     */

    public CollectibleType CollectibleType
    {
        get { return collectibleType; }
        set { collectibleType = value; }
    }
}
