using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour {

    public ChunkController currentChunk;
    public int chunkID;
    public int x;
    public int y;

    public List<int[]> adjacentcoords;
    public List<Transform> adjacenttiles;
    public Transform backtile;

    // Use this for initialization
    void Start () {
	}

    public void AssignCoords(int xloc, int yloc, int chunkIDval, ChunkController chunk)
    {
        x = xloc;
        y = yloc;
        chunkID = chunkIDval;
        currentChunk = chunk;
        adjacentcoords = new List<int[]>(); // Store block metadata
    }

	// Update is called once per frame
	public void CalculateAdjacent () {
        if (y != 71) { adjacentcoords.Add(new int[3] { chunkID, x, y+1 }); }
        if (y != 0) { adjacentcoords.Add(new int[3] { chunkID, x, y - 1 }); }
        if (x == 0) { adjacentcoords.Add(new int[3] { chunkID-1, 63, y }); }
        else { adjacentcoords.Add(new int[3] { chunkID, x-1, y }); }
        if (x == 63) { adjacentcoords.Add(new int[3] { chunkID + 1, 0, y }); }
        else { adjacentcoords.Add(new int[3] { chunkID, x+1, y }); } // Build up a co-ord array of all of the adjacent blocks.
        // The if statments locate blocks in adjacent chunks
	}

    public List<Transform> GetAdjacent()
    {
        if (adjacentcoords.Count == 0)
        {
            CalculateAdjacent();
        }
        adjacenttiles = new List<Transform>();
        foreach (int[] coords in adjacentcoords) // For all adjacent co-ordinates
        {
            if (coords[0] == chunkID)
            {
                //Debug.Log(coords[1] + " " + coords[2]);
                try
                {
                    adjacenttiles.Add(currentChunk.tiles[coords[1], coords[2]]); // Add the tile from the current chunk
                }
                catch
                {
                    Debug.Log("Error in GetAdjacent()! Tried to access tile with co-ords: " + coords[1] + " " + coords[2]);
                }
            }
            else
            {
                WorldController world = GameObject.FindGameObjectWithTag("World").GetComponent<WorldController>();
                ChunkController fetchedchunk = world.GetChunk(coords[0]); // Get an adjacent chunk
                if (fetchedchunk != null) // If the chunk is loaded,
                {
                    adjacenttiles.Add(world.GetChunk(coords[0]).tiles[coords[1], coords[2]]); // Get the tile
                }
            }
        }
        return adjacenttiles; // Return the adjacent tiles
    } 

    public Transform getBackTile()
    {
        return currentChunk.backTiles[x, y];
    }
}
