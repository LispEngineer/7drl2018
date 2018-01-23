// Copyright © 2018 Douglas P. Fields, Jr. All Rights Reserved.
// symbolics@lisp.engineer
// https://symbolics.lisp.engineer/
// Twitter @LispEngineer

using System;
using UnityEngine;

/// <summary>
/// Test code for the TileManager & UIManager.
/// </summary>
public class TestTileManager : MonoBehaviour {

	/// The managers to run our test code against.
	public TileManager tileMgr;
	public UIManager uiMgr;
	
	/// Calls our Test setup routine.
	public void Awake() {
		Test1();
	}

	/// Frame updates:
	/// 1. Check for key commands
	/// 2. Update UI information as necessary
	public void Update() {
		CheckMove();
		CheckDialog();
	}
	
	/// Implements vi-style Roguelike movement.
	/// Just moves the camera for now.
	public void CheckMove() {
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
	
	/// Shows a dialog box with neat text
	public void CheckDialog() {
		if (Input.GetKeyDown(KeyCode.I)) {
			uiMgr.dialogBoxOpen = !uiMgr.dialogBoxOpen;
		}
		
		if (uiMgr.dialogBoxOpen) {
			// TODO: Only update the text if necessary
			
			Vector2Int size = uiMgr.dialogTextSize;
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
