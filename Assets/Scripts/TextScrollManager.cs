// Copyright © 2018 Douglas P. Fields, Jr. All Rights Reserved.
// symbolics@lisp.engineer
// https://symbolics.lisp.engineer/
// Twitter @LispEngineer

using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// A component that manages a text scroll. It should be set up on the
/// root of a GameObject hierarchy that looks like this:
/// 
/// GameObject: MessageScroll
///   Size: The bounding box of what you want shown in the text
///   Component: Scroll Rect
///     Content: GameObject: MessageLogText
///     Horizontal: Off
///   Component: Image
///     Source Image: None
///     Color: Black, fully opaque (or as desired)
///   Component: Mask
/// 
///   Child: GameObject: AvoidContentSizeFitterError
///     (Used to be necessary in 5.4, maybe test without it)
///     Size: Stretch to full size of parent
///     An empty GameObject
/// 
///     Child: GameObject: MessageLogText
///       Size: Stretch horizontally, anchor to bottom
///       Component: Text
///         Horizontal Overflow: Wrap
///         Vertical Overflow: Truncate
///       Component: Content Size Fitter
///         Horizontal Fit: Unconstrained
///         Vertical Fit: Preferred Size
/// 
/// This API allows for adding text to the bottom, and
/// setting the maximum length of text.
/// </summary>

public class TextScrollManager : MonoBehaviour {

    /// The size of maximumSize if left non-positive
    public readonly int DEFAULT_MAXIMUM_SIZE = 5000;
    
    /// The scrolling window of messages for the player to read.
    public TextMeshProUGUI messageLog;

    /// The maximum text length to display in characters.
    /// Note that this includes markup
    public int maximumSize;
    
    /// The ScrollRect that is handling our text; determined at
    /// awakening.
    protected ScrollRect mlsr;

    /// Initialization
    void Awake() {
        // TODO: Check for unset values
        if (maximumSize <= 0) {
            maximumSize = DEFAULT_MAXIMUM_SIZE;
        }
        mlsr = messageLog.GetComponentInParent<ScrollRect>();
    } // Awake()

    /// Move to the bottom at start
    void Start() {
        ScrollToBottom();
    }
    
    /// Scrolls message log to the bottom. ONLY!
    protected void ScrollToBottom() {
        // Then scroll the parent ScrollRect to the bottom
        Canvas.ForceUpdateCanvases();
        mlsr.verticalNormalizedPosition = 0.0f;
        Canvas.ForceUpdateCanvases();
    } // ScrollToBottom()
    
    /// Adds text to the message log and scrolls it to the bottom.
    // It has a limitation as to the total length.
    public void AddText(string msg, string color = null) {
		
        // First add the text
        messageLog.text += "\n";
        if (!string.IsNullOrEmpty(color)) {
            messageLog.text += "<color=" + color + ">";
        }
        messageLog.text += msg;
        if (!string.IsNullOrEmpty(color)) {
            messageLog.text += "</color>";
        }

        // Truncate this when it gets too long
        string t = messageLog.text; // TODO: Consider rich text?
        while (t.Length > maximumSize) {
            int idx = t.IndexOf('\n');
            if (idx < 0) {
                // Take the last 3000 characters
                t = t.Substring(t.Length - maximumSize);
            } else {
                t = t.Substring(idx + 1);
            }
        }
        messageLog.text = t;

        ScrollToBottom();
    } // addMessageLogText
    
}
