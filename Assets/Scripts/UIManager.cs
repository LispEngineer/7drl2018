// Copyright © 2018 Douglas P. Fields, Jr. All Rights Reserved.
// symbolics@lisp.engineer
// https://symbolics.lisp.engineer/
// Twitter @LispEngineer

using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// This handles our main UI, including scaling thereof, etc.
/// </summary>

public class UIManager : MonoBehaviour {

    /// The Canvas GameObject with all the other components
    public Canvas canvas;
    
    /// And its scaler
    public CanvasScaler canvasScaler;
    
    /// The default UI scaling factor we're using.
    protected float defaultScale;
    
    
    ///////////////////////////////////////////////////////////
    // Unity Interface
    
    public void Awake() {
        Debug.Log("DPI: " + Screen.dpi);
        
        defaultScale = canvasScaler.scaleFactor;
        if (Screen.dpi > 100 && Screen.height > 800) { // Arbitrary...
            defaultScale *= 2.0f;
            canvasScaler.scaleFactor = defaultScale;
        }
    }

    /// Handle scaling of the UI with Command-Shift- dash/equals/0
    public void Update() {
        if (Input.GetKeyDown(KeyCode.Equals) && 
            Input.GetKey(KeyCode.LeftCommand) &&
            Input.GetKey(KeyCode.LeftShift)) {
            canvasScaler.scaleFactor *= 1.1f;
            // FIXME: Don't hardcode
            if (canvasScaler.scaleFactor > 3.0f * defaultScale) {
                canvasScaler.scaleFactor = 3.0f * defaultScale;
            }
        }

        if (Input.GetKeyDown(KeyCode.Minus) && 
            Input.GetKey(KeyCode.LeftCommand) &&
            Input.GetKey(KeyCode.LeftShift)) {
            canvasScaler.scaleFactor *= 0.90909f;
            // FIXME: Don't hardcode
            if (canvasScaler.scaleFactor < 0.3f * defaultScale) {
                canvasScaler.scaleFactor = 0.3f;
            }
        }
		
        if (Input.GetKeyDown(KeyCode.Alpha0) && Input.GetKey(KeyCode.LeftCommand) &&
            Input.GetKey(KeyCode.LeftShift)) {
            canvasScaler.scaleFactor = defaultScale;
        }
    } // Update()
    
   
}
