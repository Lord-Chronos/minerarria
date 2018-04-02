using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorController : MonoBehaviour {

    public float tileDims;
    public int chunkLength;
    public int chunkHeight;

    private int AbsTileX;
    private int AbsTileY;
    private int ChunkTileX;
    private int ChunkTileY;
    private int ChunkID;
    private ChunkController currentChunk; // Defines variables to keep track of the position on the map
    private Tiles tilelist;

    private void Start()
    {
        tilelist = GameObject.Find("World").GetComponent<Tiles>();
    }

    // Use this for initialization
    public void SetPos (float x, float y) {
        gameObject.transform.position = new Vector2(x, y); // Sets the position to the new position
        AbsTileX = System.Convert.ToInt32(x / tileDims); // Resolve the x position as absolute
        ChunkID = System.Convert.ToInt32(System.Math.Truncate(System.Convert.ToDecimal((AbsTileX) / chunkLength))); // Work out the chunk from the x co-ord
        if (x < 0)
        {
            ChunkID--; // Correct the weird difference I get around chunk 0
        }
        ChunkTileX = System.Convert.ToInt32(x / tileDims) % chunkLength; // Work out the position in the current chunk
        if (ChunkTileX < 0) { ChunkTileX = chunkLength+ChunkTileX; } // Make sure it isn't negative
        ChunkTileY = (System.Convert.ToInt32(y / tileDims)%chunkHeight)+(chunkHeight/2)+8; // Work out the y position - only works for this y-height
        if (ChunkTileY >= chunkHeight)
        {
            ChunkTileY = chunkHeight - 1; // Ensures the height can't go above the limit
        }
        currentChunk = GameObject.Find("World").GetComponent<WorldController>().GetChunk(ChunkID); // Get the current chunk that the cursor is in
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) // If LMB
        {
            currentChunk.breakTile(ChunkTileX, ChunkTileY, GameObject.Find("BlockPickerText").GetComponent<BlockPicker>().GetBackLayer()); // Break tile
            LightChunks();
        }
        if (Input.GetMouseButtonDown(1))
        {
            bool backLayer = GameObject.Find("BlockPickerText").GetComponent<BlockPicker>().GetBackLayer(); // Check the back layer
            if (!backLayer) // If forelayer
            {
                if (currentChunk.map[ChunkTileX, ChunkTileY] == 0) // If there is no tile
                {
                    int tileID = GameObject.Find("BlockPickerText").GetComponent<BlockPicker>().GetSelectedBlockID(); // Get the currently selected block
                    currentChunk.map[ChunkTileX, ChunkTileY] = tileID; // Update the chunkmap
                    currentChunk.placeTile(tilelist.GetTileFromID(tileID), ChunkTileX, ChunkTileY, tilelist.GetRotationFromID(tileID)); // Place the new tile
                    LightChunks();
                }
            }
            else // If backlayer
            {
                if (currentChunk.backTiles[ChunkTileX, ChunkTileY] == null) // If there is no tile
                {
                    int tileID = GameObject.Find("BlockPickerText").GetComponent<BlockPicker>().GetSelectedBlockID(); // Get the currently selected block
                    currentChunk.backMap[ChunkTileX, ChunkTileY] = tileID; // Update the chunkmap
                    currentChunk.placeTile(tilelist.GetTileFromID(tileID), ChunkTileX, ChunkTileY, tilelist.GetRotationFromID(tileID), true); // Place the new backtile
                    LightChunks();
                }
            }
        }
	}

    void LightChunks()
    {
        WorldController world = GameObject.FindGameObjectWithTag("World").GetComponent<WorldController>();
        ChunkController lowerchunk = world.GetChunk(ChunkID - 1);
        ChunkController higherchunk = world.GetChunk(ChunkID + 1);
        if (lowerchunk != null)
        {
            lowerchunk.ResetLight();
        }
        if (higherchunk != null)
        {
            higherchunk.ResetLight();
        }
        currentChunk.ResetLight();
        if (lowerchunk != null)
        {
            lowerchunk.LightUpdate(false);
        }
        if (higherchunk != null)
        {
            higherchunk.LightUpdate(false);
        }
        currentChunk.LightUpdate(false);
    }
}
