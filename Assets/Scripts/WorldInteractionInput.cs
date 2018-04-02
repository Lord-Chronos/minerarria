using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldInteractionInput : MonoBehaviour {

    public float tileDims;
    private Transform selector;
    private SelectorController selectorcontroller;

	// Use this for initialization
	void Start () {
        selector = GameObject.Find("Selector").transform; // Get the selector
        selector.localScale = new Vector2(tileDims, tileDims); // Rescale it
        selectorcontroller = selector.GetComponent<SelectorController>(); // Get the controller
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Get the mouse position in the world
        double tileX = RoundDown(mousePos.x, tileDims);
        double tileY = RoundDown(mousePos.y, tileDims); // Round it to the nearest tile
        if (selector.position.x != tileX || selector.position.y != tileY) // If it has changed,
        {
            selectorcontroller.SetPos(System.Convert.ToSingle(tileX), System.Convert.ToSingle(tileY)); // Move the selector
        }
    }

    double RoundDown(float n, float x)
    {
        return (System.Math.Round(n / x)) * x;
    }
}
