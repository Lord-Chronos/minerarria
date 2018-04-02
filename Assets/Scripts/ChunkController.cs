using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkController : MonoBehaviour {

    public int xLength;
    public int yLength;
    public int yScale; // The higher this is, the higher the terrain
    public float yPower;
    public float tileDims;
    public float perlinInterval; // The higher this is, the steeper the terrain
    public int dirtLayers;

    public Transform stoneTile;
    public Transform dirtTile;
    public Transform grassTile;
    public Transform planksTile;

    public int[,] map;
    public int[,] backMap;
    public Transform[,] tiles;
    public Transform[,] backTiles;
    public int orderX;

    private Tiles tilelist;

    void Start()
    {
        //GenerateChunk(0);
        //CreateTiles();
        // Deprecated code
    }

    // Use this for initialization
    public void GenerateChunk(float noiseXStart, float seed) {
        map = new int[xLength,yLength]; // Initialise the map
        for (int x = 0; x < xLength; x++)
        {
            float perlinSample = Mathf.PerlinNoise(noiseXStart + (x*perlinInterval), seed); // Takes the first perlin sample
            perlinSample += 0.5f*Mathf.PerlinNoise(2*(noiseXStart + (x * perlinInterval)), seed); // Takes the second perlin sample at a lower frequency
            //perlinSample += 0.25f * Mathf.PerlinNoise(4 * (noiseXStart + (x * perlinInterval)), seed); // Deprecated code
            //int height = System.Convert.ToInt32(perlinSample * yScale); // Deprecated code
            int height = System.Convert.ToInt32(System.Math.Pow(perlinSample, yPower)*yScale); // Transforms the perlin sample into an elevation
            if (height > yLength - 1) { height = yLength - 2; } // If the height is too great, cap it at the max. tile height (-1 for foliage placement)
            for (int y = 0; y < height; y++) // For every value in the height-range
            {
                if (height-y == 1) {
                    map[x, y] = 3;
                    if (Random.Range(0, 5) == 0)
                    {
                        map[x, y+1] = Random.Range(6,8); // Randomly generate flowers on the map
                    }
                } // If it's the top layer, grass it over
                else if (height-y < dirtLayers) { map[x, y] = 1; } // If it's within the dirt layers, place dirt
                else { map[x, y] = 2; } // Otherwise place stone
            }
        }
        backMap = new int[xLength, yLength];
	}
	
	// Creates all of the tiles in the chunkmap. This must be called AFTER GenerateChunk()!
	public void CreateTiles () {
        tilelist = GameObject.Find("World").GetComponent<Tiles>();
        tiles = new Transform[xLength, yLength]; // Creates the tile array
        backTiles = new Transform[xLength, yLength];
        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++) // For every piece of map data in the array
            {
                placeTile(tilelist.GetTileFromID(map[x, y]), x, y, tilelist.GetRotationFromID(map[x, y]));
                if (backMap[x,y] != 0)
                {
                    placeTile(tilelist.GetTileFromID(backMap[x, y]), x, y, tilelist.GetRotationFromID(backMap[x, y]), true);
                }
              
            }
        }
        LightUpdate();
	}

    public void placeTile(Transform tile, int x, int y, bool rotate, bool back = false)
    {
        Transform currentTile = GameObject.Instantiate(tile, gameObject.transform); // Create the tile
        currentTile.transform.localPosition = new Vector3(x * tileDims, y * tileDims); // Position it
        currentTile.localScale = new Vector3(tileDims, tileDims, 1); // Scale it
        SpriteRenderer currentSprite = currentTile.GetComponent<SpriteRenderer>(); // Get the sprite renderer
        if (rotate) // If it is a rotatable block
        {
            int flipX = Random.Range(0, 2);
            int flipY = Random.Range(0, 2);
            if (flipX == 1) { currentSprite.flipX = true; }
            if (flipY == 1) { currentSprite.flipY = true; } // Give the texture a random rotation
        }
        if (!back)
        {
            currentTile.GetComponent<BlockController>().AssignCoords(x, y, orderX, this);
            currentTile.GetComponent<LightingController>().Initialise();
            if (tiles[x,y] != null) { Destroy(tiles[x, y].gameObject); }
            tiles[x, y] = currentTile; // Add the tile to the tiles array
            currentTile.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground"; // Sort it in the foreground
        }
        else
        {
            if (!currentTile.CompareTag("CollisionLess")) { currentTile.GetComponent<Rigidbody2D>().simulated = false; } // Turn off the physics for the background tile
            currentTile.GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f); // Darken the tile
            currentTile.GetComponent<SpriteRenderer>().sortingLayerName = "Background"; // Sort it in the background
            backTiles[x, y] = currentTile; // Set the tile in the array
        }
    }

    public void breakTile(int x, int y, bool back = false)
    {
        if (!back) // If foreground
        {
            try
            {
                if (map[x, y] != 0) // If there is a tile there
                {
                    map[x, y] = 0; // Destroy it in the map
                    Destroy(tiles[x, y].gameObject); // Destroy the tile
                    placeTile(tilelist.GetTileFromID(0), x, y, false, false);
                }
            }
            catch
            {
                Debug.Log("Failed to break block at: " + x + " " + y);
            }
        }
        else // If background
        {
            if (backMap[x, y] != 0) // If there is a tile there
            {
                backMap[x, y] = 0; // Destroy it in the map 
                Destroy(backTiles[x, y].gameObject); // Destroy the tile
            }
        }
    }

    public void LightUpdate(bool reset = true)
    {
        List<LightEmitter> emitters = new List<LightEmitter>();
        foreach (Transform tile in tiles)
        {
            LightingController lightcontroller = tile.GetComponent<LightingController>();
            if (reset) { lightcontroller.ResetLight(); }
            if (lightcontroller.lightemitlevel != 0)
            {
                LightEmitter emitter = new LightEmitter();
                emitter.intensity = lightcontroller.lightemitlevel;
                emitter.x = tile.GetComponent<BlockController>().x;
                emitter.y = tile.GetComponent<BlockController>().y;
                emitters.Add(emitter);
            }
        }
        for (int x = 0; x < xLength; x++) // For every x co-ord
        {
            for (int y = yLength - 1; y > 0; y--) // Iterate from top to bottom
            {
                if (map[x, y] != 0) // When you find a non-air block
                {
                    if (y < map.GetLength(1))
                    {
                        tiles[x, y + 1].GetComponent<LightingController>().PropagateLight(20, 0); // Light the air block above it
                    }
                    if (!tilelist.transparentIDs.Contains(map[x, y])) // If not a transparent block
                    {
                        y = 0; // End the loop
                    }
                }
            }
        }
        foreach (LightEmitter emitter in emitters)
        {
            tiles[emitter.x, emitter.y].GetComponent<LightingController>().PropagateLight(emitter.intensity, 0);
        }
    }

    public void ResetLight()
    {
        foreach (Transform tile in tiles) {
            LightingController lightcontroller = tile.GetComponent<LightingController>();
            lightcontroller.ResetLight();
        }
    }

    struct LightEmitter
    {
        public float intensity;
        public int x;
        public int y;
    }
}
