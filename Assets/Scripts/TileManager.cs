// Copyright © 2018 Douglas P. Fields, Jr. All Rights Reserved.
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
public class TileManager : MonoBehaviour {
	
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

	//////////////////////////////////////////////////////////////
	
	/// The tiles that we're managing.
	protected Tile[,] tiles; 
	
	/// Track the resolution so we can redraw if needed.
	protected int sh, sw;
	
	/// Which tile the camera is currently looking at
	protected int cameraX, cameraY;
	
	/// The tile size in Unity game-space
	protected float sizeX, sizeY;
	
	/// The starting orthographic size of the camera for later
	/// scaling.
	protected float originalSize;
	protected float originalHeight;
	
	/// The user's requested size scaling.
	protected float userScale = 1.0f;
	
	/////////////////////////////////////////////////////////////////////
	/// Unity interface

	void Awake() {}
	
	// Use this for initialization
	void Start () {
		sw = Screen.width;
		originalHeight = sh = Screen.height;
		originalSize = mainCamera.GetComponent<Camera>().orthographicSize;
	}
	
	/// This checks if we have resized the window or want to change our scale.
	/// The information on the screen gets more
	/// shown instead, with the relative size of the stuff shown in the
	/// tile layout remaining (mostly) the same based upon the original size.
	/// 
	/// Additionally, we check for Command-- and Command-= to scale the screen.
	void Update () {
		bool changed = false;
		
		if (Input.GetKeyDown(KeyCode.Minus) && 
		    Input.GetKey(KeyCode.LeftCommand) &&
		    !Input.GetKey(KeyCode.LeftShift)) {
			userScale *= 1.1f;
			// FIXME: Don't hardcode
			if (userScale > 3.0) {
				userScale = 3.0f;
			}
			changed = true;
		}

		if (Input.GetKeyDown(KeyCode.Equals) && Input.GetKey(KeyCode.LeftCommand) &&
		    !Input.GetKey(KeyCode.LeftShift)) {
			userScale *= 0.9f;
			// FIXME: Don't hardcode
			if (userScale < 0.3) {
				userScale = 0.3f;
			}
			changed = true;
		}
		
		if (Input.GetKeyDown(KeyCode.Alpha0) && Input.GetKey(KeyCode.LeftCommand) &&
		    !Input.GetKey(KeyCode.LeftShift)) {
			userScale = 1.0f;
			changed = true;
		}
		
		if (changed || sh != Screen.height || sw != Screen.width) {
			sh = Screen.height;
			sw = Screen.width;
			
			// This scaling shows the same amount of stuff on the
			// screen, vertically, regardless of the resizing of the
			// window.
			float os =  originalSize * (float)sh / (float)originalHeight;
			
			// This scaling shows the same size of stuff on the screen,
			// vertically, regardless of the resizing of the window, so
			// bigger windows show more stuff.
			// DOES NOT WORK
			///// float os = originalSize * (float)originalHeight / (float)sh;
			
			///// float os = originalSize;

			// TODO: Consider also scaling by EditorGUIUtility.pixelsPerPoint,
			// to compensate for Retina and other scaled displays?
			
			mainCamera.GetComponent<Camera>().orthographicSize = os * userScale; 
			Debug.Log("Resizing screen to: " + sw + "x" + sh + " with scale " + userScale);
		}
	} // Update
	
	
	/////////////////////////////////////////////////////////////////////
	/// Accessors/Mutators
	
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
	
	/// Returns the width of the Initialized tiles we're storing
	public int TileWidth {
		get {
		    if (tiles == null) { return 0; }
			return tiles.GetLength(0);
		}
	}
	
	/// Returns the height of the Initialized tiles we're storing
	public int TileHeight {
		get {
			if (tiles == null) { return 0; }
			return tiles.GetLength(1);
		}
	}
	
	/// Which Tile the camera is looking at
	public int CameraX { get { return cameraX; } }
	/// Which Tile the camera is looking at
	public int CameraY { get { return cameraY; } }
	/// Which Tile the camera is looking at
	public Vector2Int CameraXY { get { return new Vector2Int(CameraX, CameraY); } }
	
	/// For convenience, allow indexing the TileManager to get direct access
	/// to the (Initialized) tiles.
	/// Use [x, y]
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
	
	//////////////////////////////////////////////////////////////
	// TileManager APIs
	
	/// Returns one of the predefined sprites by their ID (index into the
	/// array of sprites), or the blank sprite if not valid.
	public Sprite GetSpriteByID(int spriteID) {
		if (spriteID < 0 || spriteID >= allSprites.Length) {
			Debug.LogWarning("Invalid spriteID: " + spriteID);
			return blankSprite;
		}
		return allSprites[spriteID];
	} // GetSpriteByID
	
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
		sizeX = str.bounds.max.x - str.bounds.min.x;
		sizeY = str.bounds.max.y - str.bounds.min.y;
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
		LookAt(cx, cy);
	} // Initialize
	
	
	/// Tells the camera to look at the specified tile.
	/// <returns>false if the tile doesn't exist.</returns>
	public bool LookAt(int x, int y) {
		if (tiles == null) {
			Debug.LogError("tiles is null?");
			return false;
		}
		if (x < 0 || y < 0 ||
		    x >= tiles.GetLength(0) || y >= tiles.GetLength(1)) {
			Debug.LogWarning("x,y out of bounds: " + x + "," + y +
			                 " - bounds " + tiles.GetLength(0) + "," +
			                 tiles.GetLength(1));
			return false;
		}

		Vector3 p = mainCamera.transform.position;
		
		p.x = tiles[x, y].transform.position.x + sizeX / 2;
		p.y = tiles[x, y].transform.position.y - sizeY / 2;
		mainCamera.transform.position = p;
		cameraX = x;
		cameraY = y;
		return true;
	} // LookAt()
		
}
