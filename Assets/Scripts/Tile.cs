// Copyright © 2018 Douglas P. Fields, Jr. All Rights Reserved.
// symbolics@lisp.engineer
// https://symbolics.lisp.engineer/
// Twitter @LispEngineer

using UnityEngine;


/// <summary>
/// 
/// This represents a single tile
/// 
/// Visibility:
///   UNSEEN - The Tile will not be Active so nothing will be displayed.
///   SEEN - The Tile will be Active, the visible layers will be inActive,
///          and the Visibility layer (which dims things) will be Active.
///   VISIBLE - The Tile will be Active, all layers will be Active, and
///             the Visibility layer will be inActive.
/// </summary>

public class Tile : MonoBehaviour {
    
    /// The Position of this tile in GAME space (not Unity space)
    [HideInInspector]
    public int x, y;
    
    /// The container of the layers
    public GameObject layerContainer;
    
    /// The Visibility layer
    public GameObject visibilityLayer;
    
    /// The TileManager
    public TileManager tileManager;
    
    /// The actual requested layers
    protected int[] layerSpriteIDs;
    
    /// The actual requested whether we should show these layers when
    /// the layer is SEEN or VISIBLE. If true, we show the layer at
    /// all times (SEEN or VISIBLE); if false we show it only when VISIBLE.
    protected bool[] layerShowSeen;
    
    /// The GameObjects corresponding to layerSpriteIDs/layerShowSeen
    protected GameObject[] layerSprites;
    
    /// What our current visibility is
    protected Visibility currentVisibility;
    
    /// The visibility settings for this tile
    public enum Visibility {
        UNSEEN, SEEN, VISIBLE
    }
    
    //////////////////////////////////////////////////////////////
    
    /// TODO: DESCRIBE ME
    public void SetVisibility(Visibility vis) {
        currentVisibility = vis;
        
        switch (vis) {
        case Visibility.UNSEEN:
            gameObject.SetActive(false);
            break;
        case Visibility.SEEN:
            gameObject.SetActive(true);
            visibilityLayer.SetActive(true);
            // TODO: Hide all GameObjects that aren't showSeen
            break;
        case Visibility.VISIBLE:
            gameObject.SetActive(true);
            visibilityLayer.SetActive(false);
            // Show all layer objects
            foreach (Transform child in layerContainer.transform) {
                child.gameObject.SetActive(true);
            }
            break;
        }
    } // SetVisibility    
    
    /// <summary>
    /// Sets up the sprites to be displayed on this tile. The sprites are displayed
    /// from the rearmost (at indext 0) to the frontmost.
    /// </summary>
    /// <param name="spriteIDs"></param>
    /// <param name="showSeen"></param>
    /// <param name="vis"></param>
    public void SetLayers(int[] spriteIDs, bool[] showSeen, Visibility vis) {
        layerSpriteIDs = spriteIDs;
        layerShowSeen = showSeen;
        
        // First clean anything old in there
        layerSprites = null;
        foreach (Transform child in layerContainer.transform) {
            Destroy(child.gameObject);
        }
        
        if (spriteIDs == null || spriteIDs.Length == 0) {
            // Degenerate case
            SetVisibility(vis);
            return;
        }

        // Allocate all our sprite layers and set them up
        layerSprites = new GameObject[spriteIDs.Length];
        for (int i = 0; i < spriteIDs.Length; i++) {
            GameObject ls = Instantiate(tileManager.layerPrefab);
            ls.transform.parent = layerContainer.transform;
            ls.transform.position = transform.position;
            ls.name = "Layer " + i;
            SpriteRenderer sr = ls.GetComponent<SpriteRenderer>();
            sr.sprite = tileManager.GetSpriteByID(spriteIDs[i]);
            layerSprites[i] = ls;
        }
        
        SetVisibility(vis);
    } // SetLayers
    
}
