using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingController : MonoBehaviour {

    public float impedence;
    public float lightemitlevel;

    private BlockController maincontroller;
    private SpriteRenderer frontblock;
    private SpriteRenderer backblock;
    public float lightlevel;

	// Use this for initialization
	public void Initialise () {
        maincontroller = gameObject.GetComponent<BlockController>();
        frontblock = gameObject.GetComponent<SpriteRenderer>();
        lightlevel = 0;
        PropagateLight(0,0);
	}
	
	// Update is called once per frame
	public void PropagateLight (float intensity, int depth) {
        if (depth >= 100) { return; } // Limits recursion depth - the higher this is, the better the lighting but the worse the performance
        else if (intensity > lightlevel) // If the incoming light is brighter than the current light
        {
            lightlevel = intensity; // Update the lightlevel
            frontblock.color = new Color(0.05f * lightlevel, 0.05f * lightlevel, 0.05f * lightlevel); // Light the front block
            if (maincontroller.getBackTile() != null)
            {
                backblock = maincontroller.getBackTile().GetComponent<SpriteRenderer>(); // Light the back block, if it exists
                backblock.color = new Color(0.6f * 0.05f * lightlevel, 0.6f * 0.05f * lightlevel, 0.6f * 0.05f * lightlevel);
            }
            foreach (Transform tile in maincontroller.GetAdjacent())
            {
                float transferlight = intensity - impedence; // Attentuate the light
                tile.GetComponent<LightingController>().PropagateLight(transferlight, depth+1); // Propagate the light to adjacent tiles
            }
        }
        else { return; }
	}
    public void ResetLight()
    {
        lightlevel = 0; // Set light to zero
        frontblock.color = new Color(0f,0f,0f);
        if (maincontroller.getBackTile() != null)
        {
            backblock = maincontroller.getBackTile().GetComponent<SpriteRenderer>();
            backblock.color = new Color(0f, 0f, 0f); // Darken the tile
        }
    }
}
