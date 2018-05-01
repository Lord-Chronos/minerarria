using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    public int renderDistance;

    public Transform chunk;

    private Transform maincamera;
    private List<Transform> chunks;
    private List<int> ChunkIDs;
    private float perlinPerChunk;
    private float seed;
    private float xStart;
    private List<storedChunk> generatedChunks;
    private List<int> genchunkids; // I use this as a check because it's quicker to search for ints than classes.

	// Use this for initialization
	void Start () {
        maincamera = GameObject.Find("Main Camera").GetComponent<Transform>(); // Get the camera's transform
        seed = Random.Range(1f, 1E10f) / 1E10f;
        xStart = Random.Range(1f, 1E10f) / 1E5f; // Generate a seed and a random start in the x value (we can't start sampling at zero because it becomes symmetrical)
        Debug.Log("Seed: " + seed); // Logs the seed
        Transform tempChunk = GameObject.Instantiate(chunk, new Vector3(0, 0, 0), gameObject.transform.rotation); // Creates a temporary chunk to get the perlin interval
        perlinPerChunk = tempChunk.GetComponent<ChunkController>().perlinInterval* tempChunk.GetComponent<ChunkController>().xLength; // Calculate the amount of perlin co-ords we need for each chunk
        Destroy(tempChunk.gameObject);
        chunks = new List<Transform>();
        generatedChunks = new List<storedChunk>();
        genchunkids = new List<int>();
        ChunkIDs = new List<int>(); // Initialise all of the world arrays
        InvokeRepeating("UpdateChunks", 0.0f, 1.0f); // Update chunks every second starting in 0.1 seconds
	}
	
	void UpdateChunks ()
    {
        float cameraPos = maincamera.position.x;
        int cameraChunkPos = System.Convert.ToInt32(cameraPos / 8); // Get the camera position and work out the chunk it is in
        List<int> chunksToLoad = Enumerable.Range(cameraChunkPos - renderDistance, renderDistance * 2).ToList(); // Generate a list containing the ids of all chunks that should be loaded
        List<int> loadedChunks = new List<int>(); // Create a list to store all the chunk ids that are already loaded
        List<Transform> oldChunks = new List<Transform>(); // Create a list to store all of the chunk ids that should be deleted
        IndexChunks(chunksToLoad, loadedChunks, oldChunks);
        UnloadOldChunks(oldChunks);
        List<int> chunksToGen = GetNeededChunks(chunksToLoad, loadedChunks);
        BuildChunks(chunksToGen);
    }

    private void BuildChunks(List<int> chunksToGen)
    {
        foreach (int pos in chunksToGen) // For every chunk we need to generate
        {
            if (!genchunkids.Contains(pos)) // If it's not generated
            {
                int absX = 8 * pos; // Work out where it starts in the world
                float perlinX = xStart + (pos * perlinPerChunk); // Work out the starting perlin co-ords
                CreateChunk(absX, -5f, perlinX, pos); // Generate the chunk
            }
            else
            {
                int absX = 8 * pos; // Work out where it starts in the world
                int index = genchunkids.IndexOf(pos); // Index the int part of it
                storedChunk chunkToLoad = generatedChunks[index]; // Use the index to get the chunk proper
                CreateChunkFromMap(absX, -5f, pos, chunkToLoad.map, chunkToLoad.backMap); // Create the chunk
            }
        }
    }

    private static List<int> GetNeededChunks(List<int> chunksToLoad, List<int> loadedChunks)
    {
        List<int> chunksToGen = new List<int>(); // Create a list of new chunks to generate
        foreach (int pos in chunksToLoad)
        { // Iterate over every chunk we need
            if (!loadedChunks.Contains(pos)) { chunksToGen.Add(pos); } // If it's not already loaded, prepare it for generation
        }

        return chunksToGen;
    }

    private void UnloadOldChunks(List<Transform> oldChunks)
    {
        foreach (Transform toDelete in oldChunks) // For every old chunk
        {
            chunks.Remove(toDelete); // Remove it from the list of chunks
            ChunkController controller = toDelete.GetComponent<ChunkController>(); // Get its controller
            if (genchunkids.Contains(controller.orderX)) // If the chunk is already stored,
            {
                int index = genchunkids.IndexOf(controller.orderX); // Get the index
                genchunkids.RemoveAt(index);
                generatedChunks.RemoveAt(index); // Delete it
            }
            generatedChunks.Add(new storedChunk(controller.orderX, controller.map, controller.backMap)); // Store the chunk
            generatedChunks = generatedChunks.OrderBy(o => o.orderX).ToList(); // Sort the chunk list
            genchunkids.Add(controller.orderX); // Add it to the index list
            genchunkids.Sort(); // Sort it
            ChunkIDs.Remove(controller.orderX);
            Destroy(toDelete.gameObject); // Delete it from the map
        }
    }

    private void IndexChunks(List<int> chunksToLoad, List<int> loadedChunks, List<Transform> oldChunks)
    {
        foreach (Transform currentChunk in chunks) // For every loaded chunk
        {
            if (!chunksToLoad.Contains(currentChunk.GetComponent<ChunkController>().orderX))
            { // If it shouldn't be loaded
                oldChunks.Add(currentChunk); // Add it to the clearing list
            }
            else // Otherwise,
            {
                loadedChunks.Add(currentChunk.GetComponent<ChunkController>().orderX); // Add it to the already loaded chunks
            }
        }
    }

    void CreateChunk(float x, float y, float perlinX, int orderX)
    {
        Transform newChunk = Instantiate(chunk, gameObject.transform); // Create a new chunk
        newChunk.transform.position = new Vector3(x, y); // Move it to the proper position
        ChunkController controller = newChunk.GetComponent<ChunkController>(); // Grab the controller
        controller.orderX = orderX; // Set it's position
        controller.GenerateChunk(perlinX, seed); // Generate the terrain
        IEnumerator coroutine = controller.CreateTiles();
        StartCoroutine(coroutine); // Create all tiles
        chunks.Add(newChunk); // Add it to the loaded chunks
        ChunkIDs.Add(orderX); // ID the new chunk
        chunks = chunks.OrderBy(o => o.GetComponent<ChunkController>().orderX).ToList();
        ChunkIDs.Sort(); // Sort the chunks
    }
    void CreateChunkFromMap(float x, float y, int orderX, int[,] map, int[,] backMap)
    {
        Debug.Log("Loading Chunk from Memory");
        Transform newChunk = Instantiate(chunk, gameObject.transform); // Create a new chunk
        newChunk.transform.position = new Vector3(x, y); // Move it to the proper position
        ChunkController controller = newChunk.GetComponent<ChunkController>(); // Get the controller
        controller.orderX = orderX; // Set the position
        controller.map = map; // Create the map from the data
        controller.backMap = backMap;
        controller.CreateTiles(); // Create all tiles
        chunks.Add(newChunk); // Add to loaded chunks
        ChunkIDs.Add(orderX); // ID the new chunk
        chunks = chunks.OrderBy(o => o.GetComponent<ChunkController>().orderX).ToList();
        ChunkIDs.Sort(); // Sort the chunks
    }
    
    public ChunkController GetChunk(int id)
    {
        if (ChunkIDs.Contains(id))
        {
            int index = ChunkIDs.IndexOf(id); // Get the index
            return chunks[index].GetComponent<ChunkController>(); // Return the chunk
        }
        else { return null; }
    }
}

class storedChunk // Container class for stored chunk data
{
    public int orderX;
    public int[,] map;
    public int[,] backMap;

    public storedChunk(int pos, int[,] tiledata, int[,] backTiledata)
    {
        orderX = pos;
        map = tiledata;
        backMap = backTiledata;
    }
}
