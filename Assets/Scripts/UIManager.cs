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
    
    /// The Message Box, which can be opened or closed.
    public GameObject messageBox;
    
    /// The optional Text Scroll Manager, for scrolling text box
    /// when opened/closed.
    public TextScrollManager messageLog;
    
    /// The starting vertical size of the messageBox, which is
    /// used when the messageBox is closed.
    protected float mbClosedHeight;
    
    /// Is the message box open or closed?
    protected bool mbIsClosed;
    
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
        
        // Get the height of the box
        RectTransform rt = (RectTransform)messageBox.transform;
        Vector2 s = rt.sizeDelta;
        mbClosedHeight = s.y;
        mbIsClosed = true; // We start closed
    }

    public void Update() {
        HandleUIScaling();
        HandleMessageBox();
    }
    
    /// Handles the opening/closing of the message box, and keyboard
    /// scrolling thereto.
    public void HandleMessageBox() {
        if (Input.GetKeyDown(KeyCode.BackQuote)) {
            if (mbIsClosed) {
                // Open
                // Get the height of the canvas
                Vector2 canvasSize = ((RectTransform)canvas.transform).sizeDelta;
                Vector2 mbSize = ((RectTransform)messageBox.transform).sizeDelta;
                
                mbSize.y = canvasSize.y - mbClosedHeight;
                ((RectTransform)messageBox.transform).sizeDelta = mbSize;
            } else {
                // Close
                Vector2 mbSize = ((RectTransform)messageBox.transform).sizeDelta;
                
                mbSize.y = mbClosedHeight;
                ((RectTransform)messageBox.transform).sizeDelta = mbSize;
            }
            mbIsClosed = !mbIsClosed;
            if (messageLog != null) {
                messageLog.ScrollToBottom();
            }
        }
    } // HandleMessageBox();
    
    /// Handle scaling of the UI with Command-Shift- dash/equals/0
    public void HandleUIScaling() {
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
    } // HandleUIScaling()
    
   
}
