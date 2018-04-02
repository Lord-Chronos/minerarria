using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockPicker : MonoBehaviour {

    private string[] tiles;
    private int selection;
    bool backLayer;

	// Use this for initialization
	void Start () {
        tiles = new string[] { "Dirt", "Stone", "Grass", "Planks", "Glass", "Poppy", "Daffodil", "Torch" }; // Define the names of all blocks
        selection = 0; // Sets the selection
        backLayer = false; // Sets the forelayer as selected by default
        UpdateText(); // Update the displayed text
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(2)) { backLayer = !(backLayer); UpdateText(); } // If the middle-mouse button is pressed, alternate the focused layer
        float scrollwheel = Input.GetAxis("Mouse ScrollWheel"); // Get the scroll wheel
        if (scrollwheel < 0) // If scroll down,
        {
            selection++; // Increase the selection
            if (selection >= tiles.Length) { selection = 0; } // Wrap around
            UpdateText();
        }
        else if (scrollwheel > 0) // If scroll up
        {
            selection--; // Decrease the selection
            if (selection < 0) { selection = tiles.Length-1; } // Wrap around
            UpdateText();
        }
	}

    private void UpdateText() {
        gameObject.GetComponent<Text>().text = ("Equipped: " + tiles[selection]) + (", Foreground: " + (!backLayer).ToString());
    }

    public int GetSelectedBlockID()
    {
        return selection + 1; // Return a block id based on selection
    }

    public bool GetBackLayer()
    {
        return backLayer;
    }
}
