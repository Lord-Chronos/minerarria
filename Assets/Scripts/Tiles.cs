using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour {

    public Transform airTile;
    public Transform dirtTile;
    public Transform stoneTile;
    public Transform grassTile;
    public Transform planksTile;
    public Transform glassTile;
    public Transform poppyTile;
    public Transform daffodilTile;
    public Transform torchTile;

    private Transform[] tiles;

    public List<int> transparentIDs;

	// Use this for initialization
	void Start () {
        tiles = new Transform[] { airTile, dirtTile, stoneTile, grassTile, planksTile, glassTile, poppyTile, daffodilTile, torchTile };
        transparentIDs = new List<int> { 0, 5, 6, 7, 8 };
	}
	
	public Transform GetTileFromID(int id)
    {
        return tiles[id];
    }
    public bool GetRotationFromID(int id)
    {
        switch (id)
        {
            case 0: { return false; }
            case 1: { return true; }
            case 2: { return true; }
            case 3: { return false; }
            case 4: { return false; }
            case 5: { return false; }
            case 6: { return false; }
            default: { return false; }
        }
    }
}
