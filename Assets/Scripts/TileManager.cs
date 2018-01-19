using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class TileManager : MonoBehaviour
{

	/// The parent of all tiles
	public GameObject tileContainer;

	/// The prefab of a tile
	public GameObject tilePrefab;
	
	/// Track the resolution so we can redraw if needed.
	protected int sh, sw;

	void Awake() {}
	
	// Use this for initialization
	void Start () {
		sh = Screen.height;
		sw = Screen.width;
		SetupTiles();
	}
	
	/// Creates all the tile objects that will fit on the screen.
	public void SetupTiles() {
		Vector3 z0;
		Renderer rend;
		float width;
		float height;
		
		// First, clear all our tiles
		foreach (Transform child in tileContainer.transform) {
			Destroy(child.gameObject);
		}
		
		// Create the first and put it at the top left of the screen at z-layer 0
		GameObject newTile = Instantiate(tilePrefab);
		newTile.transform.parent = tileContainer.transform;
		newTile.name = "0,0";
		z0 = Camera.main.ViewportToWorldPoint(new Vector3(0,1,0));
		z0.z = 0;
		newTile.transform.position = z0; 
		newTile.SetActive(true);
		rend = newTile.GetComponent<Renderer>();
		width = rend.bounds.max.x - rend.bounds.min.x;
		height = rend.bounds.max.y - rend.bounds.min.y;
		
		// Make the last tile, in the bottom right
		GameObject lastTile = Instantiate(tilePrefab);
		lastTile.transform.parent = tileContainer.transform;
		lastTile.name = "Last";
		z0 = Camera.main.ViewportToWorldPoint(new Vector3(1,0,0));
		z0.z = 0;
		z0.x -= width;
		z0.y += height;
		lastTile.transform.position = z0; 
		lastTile.SetActive(true);
		
		// Create all the tiles in between
		int xi, yi = 0;
		for (float y = newTile.transform.position.y; 
			y > lastTile.transform.position.y; 
			y -= height, yi += 1) {
			
			xi = 0;
			for (float x = newTile.transform.position.x; 
				x < lastTile.transform.position.x; 
				x += width, xi += 1) {
				
				GameObject t = Instantiate(tilePrefab);
				t.transform.parent = tileContainer.transform;
				t.name = String.Format("{0},{1}", xi, yi); // boxing
				z0.z = 0;
				z0.x = x;
				z0.y = y;
				t.transform.position = z0; 
				t.SetActive(true);
			}
		}
			
		// And get rid of our temporary first and last tile
		Destroy(lastTile);
		Destroy(newTile);
	}
	
	// Update is called once per frame
	void Update () {
		if (sh != Screen.height || sw != Screen.width) {
			sh = Screen.height;
			sw = Screen.width;
			Debug.Log("Resizing screen to: " + sw + "x" + sh);
			SetupTiles();
		}
	}
}
