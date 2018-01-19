// Copyright © 2008 Douglas P. Fields, Jr. All Rights Reserved.
// symbolics@lisp.engineer
// https://symbolics.lisp.engineer/
// Twitter @LispEngineer

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

/// <summary>
/// The TileManager handles the display layer of a 2D game with
/// stacked sprites on a 2D grid. Each tile can have a visibility
/// of unseen, seen and visible. Each layer can have things that
/// are displayed when "seen," and only displayed when "visible".
/// 
/// The main functionality is:
/// 
/// 1. Initialize with the appropriate size of tiles,
///    which are all initialized as "unseen", and with the
///    camera positioned over an approximately central tile.
/// 2. Set the layers (from bottom to top) on each tile
///    as they become visibile.
/// 3. Set each tile that goes out of sight as "seen"
///    which will show it dimmer and not show the layers that
///    are displayed only when "visible."
/// 4. Set the camera position so it is directly over the
///    center of a specified tile.
/// 5. Set the zoom of the camera, so it can show more or
///    fewer tiles.
/// 
/// The TileManager has an array of Sprites from which all
/// the layers shown in the Tile are drawn.
/// 
/// Each tile is a GameObject with a Tile component, which
/// itself consists of a hierarchy of components.
/// </summary>
public class TileManager : MonoBehaviour
{
	
	/// Our camera, for moving it appropriately over the player
	public GameObject mainCamera;
	
	/// All the sprites we can use, which must all be the same
	/// exact texture size.
	public Sprite[] allSprites;
	
	/// A blank sprite in case an invalid one is requested
	public Sprite blankSprite;

	/// The parent of all tiles
	public GameObject tileContainer;

	/// The prefab of a tile
	public GameObject tilePrefab;
	
	/// The prefab of a layer, so we can get the size.
	/// This _must_ have a SpriteRenderer in it, which
	/// also contains a sprite from the allSprites,
	/// so we can discern the size of a tile layer.
	public GameObject layerPrefab;
	
	protected Tile[,] tiles; 
	
	/// Track the resolution so we can redraw if needed.
	protected int sh, sw;
	
	/// The starting orthographic size of the camera for later
	/// scaling.
	protected float originalSize;
	protected float originalHeight;

	void Awake() {}
	
	/// Throws an ArgumentOutOfRangeException if the indexes to
	/// tiles is not valid
	private void CheckTileIndexes(int x, int y) {
		if (tiles == null) {
			throw new ArgumentOutOfRangeException("Not yet initialized");
		}
		if (x >= tiles.GetLength(0) ||
		    y >= tiles.GetLength(1)) {
			throw new ArgumentOutOfRangeException(x + "x" + y +
			                                      " is outside the initialized size");
		}
	}
	
	/// For convenience, allow indexing the TileManager to get direct access
	/// to the (Initialized) tiles.
	public Tile this[int x, int y] {
		get {
			CheckTileIndexes(x, y);
			return tiles[x, y];
		}
		// We probably shouldn't allow public access to the setter.
		protected set {
			CheckTileIndexes(x, y);
			tiles[x, y] = value;
		}
	} // Indexer to the tiles
	
	/// Returns one of the predefined sprites by their ID (index into the
	/// array of sprites), or the blank sprite if not valid.
	public Sprite GetSpriteByID(int spriteID) {
		if (spriteID < 0 || spriteID >= allSprites.Length) {
			Debug.LogWarning("Invalid spriteID: " + spriteID);
			return blankSprite;
		}
		return allSprites[spriteID];
	} // GetSpriteByID
	
	// Use this for initialization
	void Start () {
		sw = Screen.width;
		originalHeight = sh = Screen.height;
		originalSize = mainCamera.GetComponent<Camera>().orthographicSize;
		Test1();
	}
	
	/// This checks if we have resized the window. If so, we rescale the
	/// orthographic camera so that the same amount of information is shown
	/// on the screen at all times.
	/// TODO: Make a version where the information on the screen gets more
	/// shown instead, with the relative size of the stuff shown in the
	/// tile layout remaining (mostly) the same.
	void Update () {
		if (sh != Screen.height || sw != Screen.width) {
			sh = Screen.height;
			sw = Screen.width;
			// We want the orthographicSize to be 6 when the game is in a
			// 1024x768 window, so we scale it to the width.
			float os = (float)sh / originalHeight * originalSize;
			mainCamera.GetComponent<Camera>().orthographicSize = os; 
			Debug.Log("Resizing screen to: " + sw + "x" + sh);
		}
	}
	/// Initializes all the tiles to a specific (game) grid size.
	/// Tiles are displayed with 0,0 at the top left, with increasing
	/// indexes going right and down.
	public void Initialize(int w, int h) {
		if (w <= 0 || h <= 0) {
			Debug.LogError("Bad size: " + w + "x" + h);
			return;
		}
		
		// Clean out things first
		foreach (Transform tile in tileContainer.transform) {
			Destroy(tile.gameObject);
		}
		
		tiles = new Tile[w, h];

		// Calculate the size of our tiles in Unity-coordinates
		Renderer str = layerPrefab.GetComponentInChildren<Renderer>();
		float sizeX = str.bounds.max.x - str.bounds.min.x;
		float sizeY = str.bounds.max.y - str.bounds.min.y;
		Vector3 p;
		
		// Create all our tiles and position them properly
		for (int y = 0; y < h; y++) {
			for (int x = 0; x < w; x++) {
				GameObject tgo = Instantiate(tilePrefab);
				tgo.transform.parent = tileContainer.transform;
				tgo.name = String.Format("{0},{1}", x, y); // boxing
				
				p.x = x * sizeX;
				p.y = -(y * sizeY);
				p.z = tgo.transform.position.z;
				tgo.transform.position = p;
				
				
				tiles[x, y] = tgo.GetComponent<Tile>();
				tiles[x, y].SetLayers(null, null, Tile.Visibility.UNSEEN);
			}
		}
		
		// Position the camera over the center-ish tile
		int cx = w / 2;
		int cy = h / 2;
		p = mainCamera.transform.position;
		
		p.x = tiles[cx, cy].transform.position.x + sizeX / 2;
		p.y = tiles[cx, cy].transform.position.y - sizeY / 2;
		mainCamera.transform.position = p;
	} // Initialize
		
	
	// Tests initialization and setting up of tiles
	protected void Test1() {
		int ns = allSprites.Length;
		int s = 0;
		const int increment = 37; // Nice prime number
		int size = (int)Math.Floor(Math.Sqrt(ns)) * 2;
		
		Initialize(size, size);
		
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {
				tiles[x, y].SetLayers(new int[] { s, (s + 1) % ns }, new bool[] { true }, Tile.Visibility.VISIBLE);
				// Put a different sprite in the next one
				s = (s + increment) % ns;
			}
		}
	} // Test1
	
	
	/// Creates all the tile objects that will fit on the screen.
	public void OLD_SetupTiles() {
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
	
}
