// Copyright © 2018 Douglas P. Fields, Jr. All Rights Reserved.
// symbolics@lisp.engineer
// https://symbolics.lisp.engineer/
// Twitter @LispEngineer

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// This handles our main UI, including scaling thereof, etc.
/// </summary>

public class UIManager : MonoBehaviour {
    
    /// Called whenever our dialog box size changes WHILE it's open.
    public delegate void DialogSizeChange();

    //////////////////////////////////////////////////////////////

    /// The Canvas GameObject with all the other components
    public Canvas canvas;
    
    /// And its scaler
    public CanvasScaler canvasScaler;
    
    /// The Message Box, which can be opened or closed.
    public GameObject messageBox;
    
    /// The optional Text Scroll Manager, for scrolling text box
    /// when opened/closed.
    public TextScrollManager messageLog;
    
    /// The dialog box which can be open and closed.
    public GameObject dialogBox;
    
    /// The interior of the dialog box, which is actually the display area
    /// which needs to be resized.
    public GameObject dbInterior;
    
    /// The text contents of the dialog box.
    public TextMeshProUGUI dialogText;
    
    /// What to do when our open dialog box size changes.
    [Header("Event Listeners")]
    public UnityEvent dialogSizeChangeListeners;
    
    
    ////////////////////////////////////////////////////////////
    
    /// The starting vertical size of the messageBox, which is
    /// used when the messageBox is closed.
    protected float mbClosedHeight;
    
    /// Is the message box open or closed?
    protected bool mbIsClosed;
    
    /// The default UI scaling factor we're using.
    protected float defaultScale;
    
    /// The screen height, last time we checked
    protected int lastScreenHeight;
    
    /// Open Height Percentage for Message Box
    protected float openMBHeightPct;
    
    /// Open Height/Width percentage for Dialog Box
    protected Vector2 openDBPct;
    
    /// Dialog box size in terms of width, height (x, y) assuming
    /// monospaced characters.
    protected Vector2Int dialogBoxTextSize;
    
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
        
        
        // Determine size of the dialog box for later scaling,
        // and also its text size
        dialogBox.SetActive(true);
        Canvas.ForceUpdateCanvases();
        rt = (RectTransform)dbInterior.transform;
        Vector2 oMin = rt.offsetMin;
        // We start with DialogBox closed
        dialogBox.SetActive(false);
        // FYI: offsetMax is inverted
        // offsetMax.x = right, but it is NEGATIVE (right 5 shows as -5)
        openDBPct.x = oMin.x / Screen.width;
        openDBPct.y = oMin.y / Screen.height;
    }
    
    public void Start() {
        // We have to initialize this during Start() since it doesn't
        // seem to work during Awake(), which makes sense I guess, since
        // not all the objects are necessarily Awake()ned?
        dialogBoxTextSize = GetDialogTextSize();
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
    
    /// Sets the open dialog box size to the correct portion of the screen,
    /// based upon the size when the Scene was loaded.
    public void SetDialogBoxSize() {
        Vector2 canvasSize = ((RectTransform)canvas.transform).sizeDelta;
        Vector2 offset;
        
        offset.x = canvasSize.x * openDBPct.x;
        offset.y = canvasSize.y * openDBPct.y;
        ((RectTransform)dbInterior.transform).offsetMin = offset;
        offset.x = -offset.x;
        offset.y = -offset.y;
        ((RectTransform)dbInterior.transform).offsetMax = offset;
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
            
            // Resize the DialogBox to be same portion of screen.
            SetDialogBoxSize();
            dialogBoxTextSize = GetDialogTextSize();
            
            // Fire an event if the dialog box is open and
            // the player resizes things, so it can re-render.
            if (dialogBox.active) {
                dialogSizeChangeListeners.Invoke();
            }
        }
    } // HandleUIScaling()
    
    /////////////////////////////////////////////////////////////////
    // Dialog Box
    
    /// 300 lines of one character string for testing the visible height.
    private readonly string HEIGHT_TEST_STRING =
        string.Concat(Enumerable.Repeat("X\n", 300));
    
    /// <summary>
    /// This assumes a monospaced font, and will tell you how much will fit in
    /// each row and column for the TMPUGUI.
    /// 
    /// The TextMesh Pro settings should be as follows:
    ///    Wrapping & overflow: (doesn't really matter?) Disabled & truncate
    /// </summary>
    /// <returns>(x, y) as (cols, rows) or (width, height)</returns>
    public Vector2Int GetDialogTextSize() {
        // dialogText.firstOverflowCharacterIndex;
        string originalText = dialogText.text;
        int lineHeight;
        bool oldDBOpen = dialogBox.active;
        
        dialogBox.SetActive(true);
        dialogText.text = HEIGHT_TEST_STRING;
        // Reformat the display - see https://forum.unity.com/threads/linked-text-in-ugui-layout-groups.471477/
        // Both these calls seem to work - which is more efficient?
        // dialogText.Rebuild(CanvasUpdate.PreRender);
        dialogText.ForceMeshUpdate();

        /*
        Debug.Log("Overflowing? " + dialogText.isTextOverflowing +
                  ", at: " + dialogText.firstOverflowCharacterIndex);
        Debug.Log("Truncated? " + dialogText.isTextTruncated);
        // characterCount seems to count characters even if they have been
        // truncated from a long line. Disappointing.
        Debug.Log("Char count: " + dialogText.textInfo.characterCount +
                  ", line count: " + dialogText.textInfo.lineCount +
                  ", page count: " + dialogText.textInfo.pageCount);
        */
        /*
        Debug.Log("Max vis chars: " + dialogText.maxVisibleCharacters +
                  ", max vis lines: " + dialogText.maxVisibleLines +
                  ", max vis words: " + dialogText.maxVisibleWords);
        */
        
        // This works to get the height, but not the width.
        lineHeight = dialogText.textInfo.lineCount;
        
        // TRY to find the line length
        bool oldWW = dialogText.enableWordWrapping;
        TextOverflowModes oldTOM = dialogText.overflowMode;
        
        dialogText.text = "X";
        dialogText.enableWordWrapping = true;
        dialogText.overflowMode = TextOverflowModes.Overflow;
        
        int lineWidth = 300;
        for (int l = 1; l < lineWidth; l++) {
            dialogText.text = string.Concat(Enumerable.Repeat("X", l));
            // Mesh update is necessary
            dialogText.ForceMeshUpdate();
            /*
            Debug.Log("Length: " + l +
                      ", char count: " + dialogText.textInfo.characterCount +
                      ", line count: " + dialogText.textInfo.lineCount);
            */
            if (dialogText.textInfo.lineCount > 1) {
                lineWidth = l - 1;
                break;
            }
        }

        if (!oldDBOpen) {
            dialogBox.SetActive(false);
        }
        
        // Restore everything to the original settings
        dialogText.text = originalText;
        dialogText.enableWordWrapping = oldWW;
        dialogText.overflowMode = oldTOM;
        // dialogText.Rebuild(CanvasUpdate.PreRender);
        dialogText.ForceMeshUpdate();
        
        Debug.Log("Final text size: width: " + lineWidth + ", height: " + lineHeight);
        
        return new Vector2Int(lineWidth, lineHeight);
    } // GetDialogTextRows()
    
    /// The (width, height) of the dialog box in text characters
    public Vector2Int dialogTextSize {
        get { return dialogBoxTextSize; }
    }
    // Checking and opening/closing the dialog box
    public bool dialogBoxOpen {
        get { return dialogBox.active; }
        set { dialogBox.SetActive(value); }
    }
    // Checking and changing the dialog box text
    public string dialogBoxText {
        get { return dialogText.text; }
        set { dialogText.text = value; }
    }
   
}
