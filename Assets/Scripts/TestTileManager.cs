// Copyright © 2018 Douglas P. Fields, Jr. All Rights Reserved.
// symbolics@lisp.engineer
// https://symbolics.lisp.engineer/
// Twitter @LispEngineer

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// Test code for the TileManager & UIManager.
/// </summary>
public class TestTileManager : MonoBehaviour {

	/// The managers to run our test code against.
	public TileManager tileMgr;
	public UIManager uiMgr;
	
	
	/// What mode is the game in? Mostly so we handle input properly.
	public enum GameMode {
		/// The main game is being played.
		Play,
		/// The inventory window is open.
		Inventory
	}
	
	/// Current game mode.
	protected GameMode currentMode = GameMode.Play;

	/// Our (bogus list of) inventory items
	protected string[] inventory = { "Boots of Walking",
		"Shoes of Running", "Chainmail of Elvish Lore",
		"Greaves of the Grave", "Potion of Elixir Drinking" };
	/// Negative if nothing is selected, or the index into inventory[]
	protected int selectedInventoryItem = -1;

	/// All letter keycodes.
	protected static readonly KeyCode[] LETTER_KEYCODES = {
		KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E,
		KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J,
		KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O,
		KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T,
		KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y,
		KeyCode.Z };
	
	/// Calls our Test setup routine.
	public void Awake() {
		Test1();
	}

	/// Frame updates:
	/// 1. Check for key commands
	/// 2. Update UI information as necessary
	public void Update() {
		CheckMove();
		CheckInventory();
	}
	
	/// <summary>
	/// Reports a single letter (alphabetical priority) which is pressed
	/// this frame.
	/// </summary>
	/// <returns>Negative on no letter pressed, or a number 0-25 corresponding
	/// to the letters A-Z.</returns>
	public int LetterKeyDown() {
		for (int i = 0; i < LETTER_KEYCODES.Length; i++) {
			if (Input.GetKeyDown(LETTER_KEYCODES[i])) {
				return i;
			}
		}
		return -1;
	} // LetterKeyDown
	
	/// Implements vi-style Roguelike movement.
	/// Just moves the camera for now.
	public void CheckMove() {
		if (currentMode != GameMode.Play) {
			return;
		}
		int dx = 0, dy = 0;
		
		if (Input.GetKeyDown(KeyCode.H)) { dx = -1; }
		if (Input.GetKeyDown(KeyCode.J)) { dy =  1; }
		if (Input.GetKeyDown(KeyCode.K)) { dy = -1; }
		if (Input.GetKeyDown(KeyCode.L)) { dx =  1; }
		if (Input.GetKeyDown(KeyCode.Y)) { dy = -1; dx = -1; }
		if (Input.GetKeyDown(KeyCode.U)) { dy = -1; dx =  1; }
		if (Input.GetKeyDown(KeyCode.B)) { dy =  1; dx = -1; }
		if (Input.GetKeyDown(KeyCode.N)) { dy =  1; dx =  1; }
		
		if (dx != 0 || dy != 0) {
			Debug.Log("Moving camera from " + tileMgr.CameraX + "," + tileMgr.CameraY +
				      " by delta " + dx + "," + dy);
			// This will clamp and ignore out-of-bounds calls.
			tileMgr.LookAt(tileMgr.CameraX + dx,
						   tileMgr.CameraY + dy);
		
			uiMgr.messageLog.AddText("Moving: " + dx + "," + dy);
		}
	} // Update
	
	/// Draws the inventory into the dialog box.
	/// Unity probably loves all these object allocations for string manipulation.
	public void DrawInventory() {
		Vector2Int size = uiMgr.dialogTextSize;
		
		string t = "<b><u>Inventory</u></b>";
		int numPageDigits = 1;
		
		// Bold, underlined is rendered wider by Unity/TextMeshPro
		t += new string(' ', size.x - 9 - numPageDigits - 1 - numPageDigits - 1);
		t += "1/1"; // Page number / number of pages
		t += "\n";
		// t += string.Concat(Enumerable.Repeat("1234567890", 10));
		t += "\n"; // Blank line
		
		// Lines:
		// Header
		// Blank
		// <inventory>
		// Blank
		// Commands
		for (int l = 0; l < size.y - 4; l++) {
			if (l < inventory.Length && l < 26) {
				if (selectedInventoryItem == l) {
					t += "<mark=#ffff0055>";
				}
				t += (char)('A' + l);
				t += " - ";
				t += inventory[l];
				if (selectedInventoryItem == l) {
					t += "</mark>";
				}
			}
			t += "\n";
		}
		
		t += "\n";
		
		if (selectedInventoryItem < 0) {
			t += "<mark=#ff000055>[A-Z]</mark> - Select, <mark=#ff000055>ESC</mark> - Exit";
		} else {
			t += "<mark=#ff000055>ESC</mark> - Unselect, <mark=#ff000055>D</mark> - drop, <mark=#ff000055>...</mark>";
		}
		
		uiMgr.dialogBoxText = t;
	} // DrawInventory()
	
	/// Shows a dialog box pretending to be inventory
	public void CheckInventory() {
		Boolean switchedToInventory = false;
		
		if (currentMode == GameMode.Play) {
			if (Input.GetKeyDown(KeyCode.I)) {
				currentMode = GameMode.Inventory;
				switchedToInventory = true;
			}
		} // End Play Mode (may have switched to Inventory Mode
		
		if (currentMode != GameMode.Inventory) {
			return;
		}
		// Set up the initial inventory dialog box and show it.
		if (switchedToInventory) {
			selectedInventoryItem = -1;
			uiMgr.dialogBoxOpen = true;
			DrawInventory();
		}
		
		// TODO: Handle dialog box resizing?

		// Keyboard input handler
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (selectedInventoryItem < 0) {
				// Close dialog box and go to normal play mode
				uiMgr.dialogBoxOpen = false;
				currentMode = GameMode.Play;
			} else {
				// Unselect the item
				selectedInventoryItem = -1;
				DrawInventory();
			}
		}
		
		if (selectedInventoryItem < 0) {
			// Handle keyboard letters for selecting things
			int letter = LetterKeyDown();
			if (letter >= 0 && letter < inventory.Length) {
				selectedInventoryItem = letter;
				DrawInventory();
			}
		} else {
			// Handle keyboard letters for doing something with the selected
			// item.
			if (Input.GetKeyDown(KeyCode.D)) {
				// Drop an item
				uiMgr.messageLog.AddText("Dropped " + inventory[selectedInventoryItem] + ".");
				// Super inefficient use of LINQ?
				inventory =
					inventory.Where((source, index) => index != selectedInventoryItem).ToArray();
					// new List<string>(inventory).RemoveAt(selectedInventoryItem).ToArray();
				selectedInventoryItem = -1;
				DrawInventory();
			}
		}
	} // CheckDialog
	
	/// Sets up a lot of random-ish tiles. 
	protected void Test1() {
		int ns = tileMgr.allSprites.Length;
		int s = 0;
		const int increment = 37; // Nice prime number
		int size = (int)Math.Floor(Math.Sqrt(ns)) * 2;
		
		tileMgr.Initialize(size, size);
		
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {
				tileMgr[x, y].SetLayers(new int[] { s, (s + 1) % ns }, new bool[] { true }, Tile.Visibility.VISIBLE);
				// Put a different sprite in the next one
				s = (s + increment) % ns;
			}
		}
	} // Test1
	
	
}
