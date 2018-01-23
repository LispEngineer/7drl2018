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
    
    /// The screen height, last time we checked
    protected int lastScreenHeight;
    
    /// Open Height Percentage
    protected float openMBHeightPct;
    
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
        
        // Store our current Screen height for resizing
        lastScreenHeight = Screen.height;
        openMBHeightPct = 1.0f - (float)mbClosedHeight / (float)lastScreenHeight;
    }

    public void Update() {
        HandleUIScaling();
        HandleMessageBox();
        HandleUIResizing();
    }
    
    /// If the screen size vertically changes, we re-open an open message box
    /// to ensure it is sized properly.
    public void HandleUIResizing() {
        if (lastScreenHeight != Screen.height) {
            if (!mbIsClosed) {
                OpenMessageBox();
                if (messageLog != null) {
                    messageLog.ScrollToBottom();
                }
            }
            lastScreenHeight = Screen.height;
        }
    } // HandleUIResizing()
    
    /// Calculates a "most of the screen" vertical size of the message
    /// box and sets it to that.
    public void OpenMessageBox() {
        // Open
        // Get the height of the canvas
        Vector2 canvasSize = ((RectTransform)canvas.transform).sizeDelta;
        Vector2 mbSize = ((RectTransform)messageBox.transform).sizeDelta;
                
        mbSize.y = canvasSize.y * openMBHeightPct;
        ((RectTransform)messageBox.transform).sizeDelta = mbSize;
        Debug.Log("mbSize.y = " + mbSize.y);
    }
    
    /// Handles the opening/closing of the message box, and keyboard
    /// scrolling thereto.
    public void HandleMessageBox() {
        if (Input.GetKeyDown(KeyCode.BackQuote)) {
            if (mbIsClosed) {
                OpenMessageBox();
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
    } // HandleMessageBox()
    
    /// Handle scaling of the UI with Command-Shift- dash/equals/0
    public void HandleUIScaling() {
        bool changed = false;
        
        if (Input.GetKeyDown(KeyCode.Equals) && 
            Input.GetKey(KeyCode.LeftCommand) &&
            Input.GetKey(KeyCode.LeftShift)) {
            canvasScaler.scaleFactor *= 1.1f;
            // FIXME: Don't hardcode
            if (canvasScaler.scaleFactor > 3.0f * defaultScale) {
                canvasScaler.scaleFactor = 3.0f * defaultScale;
            }
            changed = true;
        }

        if (Input.GetKeyDown(KeyCode.Minus) && 
            Input.GetKey(KeyCode.LeftCommand) &&
            Input.GetKey(KeyCode.LeftShift)) {
            canvasScaler.scaleFactor *= 0.90909f;
            // FIXME: Don't hardcode
            if (canvasScaler.scaleFactor < 0.3f * defaultScale) {
                canvasScaler.scaleFactor = 0.3f;
            }
            changed = true;
        }
		
        if (Input.GetKeyDown(KeyCode.Alpha0) && Input.GetKey(KeyCode.LeftCommand) &&
            Input.GetKey(KeyCode.LeftShift)) {
            canvasScaler.scaleFactor = defaultScale;
            changed = true;
        }

        if (changed) {
            // Force the updates so our calculations of sizes use the
            // current canvasScaler settings...
            // Note that Canvas.ForceUpdateCanvases() does not scale the
            // canvas, which apparently only happens at Update() on the
            // Canvas Scaler
            canvasScaler.SendMessage("Update");

            if (!mbIsClosed) {
                // Resize an open message box upon rescale
                OpenMessageBox();
            }
            // And scroll contents to the bottom.
            if (messageLog != null) {
                messageLog.ScrollToBottom();
            }
        }
    } // HandleUIScaling()
    
   
}
