#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Syncfusion.Windows.Forms.HTMLUI;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Interface which publishes to the user the main functionality of each HTML element.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public interface IHTMLElement
    {
        #region Interface properties
        /// <summary>
        /// Gets the list of attributes set to the current element.
        /// </summary>
        IHTMLAttributesCollection Attributes
        { 
            get; 
        }

        /// <summary>
        /// Gets the list of Events supported by the current element.
        /// </summary>
        IHTMLEventsCollection Events 
        { 
            get; 
        }

        /// <summary>
        /// Gets the sub elements of the current HTML element.
        /// </summary>
        IHTMLElementsCollection Children
        { 
            get; 
        }

        /// <summary>
        /// Gets or sets the value for the attribute with the specified attribute name.
        /// </summary>
        /// <param name="attributeName">String attribute name</param>
        string this[string attributeName]
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets the parent element of the current element.
        /// </summary>
        IHTMLElement Parent 
        { 
            get; 
        }

        /// <summary>
        /// Gets the unique id of the element. Control guarantees that this id is
        /// always unique for the document.
        /// </summary>
        string UniqueID 
        { 
            get; 
        }

        /// <summary>
        /// Gets or sets the unique identifier of the HTML element in the HTML elements objects tree. This
        /// id is specified by the user.
        /// </summary>
        string ID 
        { 
            get;
            set;
         }

        /// <summary>
        /// Gets the unique name of the HTML element in the HTML elements objects tree.
        /// </summary>
        string Name 
        { 
            get;
        }

        /// <summary>
        /// Gets or sets the hot key which used for fast access of the HTML element.
        /// </summary>
        Keys AccessKey 
        { 
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets the tab index for the HTML element.
        /// </summary>
        /// <remarks>
        /// Objects with a positive TabIndex are selected in increasing order and in source order to resolve duplicates.
        /// Objects with an TabIndex of zero are selected in source order. 
        /// Objects with a negative TabIndex are omitted from the tabbing order.
        /// </remarks>
        int TabIndex 
        { 
            get; 
            set;
        }

        /// <summary>
        /// Gets the real Location of the element after rendering in control client coordinates.
        /// </summary>
        Point Location 
        { 
            get; 
        }

        /// <summary>
        /// Gets the real Size of the element after rendering.
        /// </summary>
        Size Size 
        { 
            get; 
        }

        /// <summary>
        /// Gets or sets the HTML inner text of the current HTML element, without the current element declaration.
        /// </summary>
        string InnerHTML
        { 
            get;
            set;
        }

        /// <summary>
        /// Gets the HTML text including the inner and current element text.
        /// </summary>
        string OuterHTML         
        { 
            get;
        }

        /// <summary>
        /// Gets the formatting of element used for rendering.
        /// </summary>
        IHTMLFormat Format 
        { 
            get; 
        }

        /// <summary>
        ///  Gets a value indicating whether the current element's visibility to user.
        /// </summary>
        bool IsVisible 
        { 
            get; 
        }

        /// <summary>
        /// Gets a value indicating whether the current element is resizable if document is changed.
        /// </summary>
        bool IsResizable 
        { 
            get;
        }

        /// <summary>
        /// Gets the reference of the control which holds this object.
        /// </summary>
        HTMLUIControl Control 
        { 
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the element has input focus.
        /// </summary>
        bool Focused 
        { 
            get; 
        }

        /// <summary>
        /// Gets the text of the element and its children.
        /// </summary>
        string Text 
        { 
            get; 
        }

        /// <summary>
        /// Gets the selected text of the element and its children.
        /// </summary>
        string SelectedText { get; }
        #endregion

        #region Interface events
        /// <summary>
        /// Event. Raised before the CSS style of the element ( Format ) is merged.
        /// </summary>
        event PreStyleCalculatedEventHandler BeforeStyleCalculated;

        /// <summary>
        /// Event. Raised before the element or its part is painted.
        /// </summary>
        event ElementPaintEventHandler Paint;

        /// <summary>
        /// Event. Raised when the size of the element is calculated.
        /// </summary>
        event EventHandler SizeCalculated;

        /// <summary>
        /// Event. Raised when the location of the element is calculated.
        /// </summary>
        event EventHandler LocationCalculated;

        /// <summary>
        /// Event. Raised when the mouse is clicked.
        /// </summary>
        event EventHandler Click;

        /// <summary>
        /// Event. Raised when mouse is double clicked.
        /// </summary>
        event EventHandler DoubleClick;

        /// <summary>
        /// Event. Raised when mouse moves.
        /// </summary>
        event EventHandler MouseMove;

        /// <summary>
        /// Event. Raised when mouse enters.
        /// </summary>
        event EventHandler MouseEnter;

        /// <summary>
        /// Event. Raised when mouse leaves.
        /// </summary>
        event EventHandler MouseLeave;

        /// <summary>
        /// Event. Raised when mouse button is down.
        /// </summary>
        event EventHandler MouseDown;

        /// <summary>
        /// Event. Raised when key is down.
        /// </summary>
        event EventHandler KeyDown;

        /// <summary>
        /// Event. Raised when key is up.
        /// </summary>
        event EventHandler KeyUp;

        /// <summary>
        /// Event. Raised when key is pressed.
        /// </summary>
        event EventHandler KeyPress;

        /// <summary>
        /// Event. Raised when the element gets focus.
        /// </summary>
        event EventHandler GotFocus;

        /// <summary>
        /// Event. Raised when the element has lost focus.
        /// </summary>
        event EventHandler Leave;

        /// <summary>
        /// Event. Raised when the TabIndex property value has been changed.
        /// </summary>
        event EventHandler TabIndexChanged;
        #endregion

        #region Interface Methods
        /// <summary>
        /// Calculates the format from arrays of possible formats.
        /// </summary>
        void CalculateFormat();

        /// <summary>
        /// Calculates the size of the element from it's content.
        /// </summary>
        void CalculateSize();

        /// <summary>
        /// Calculates the position of the element for rendering.
        /// </summary>
        void CalculatePosition();

        /// <summary>
        /// Draws the element into the control.
        /// </summary>
        /// <param name="e">PaintEventArgs instance</param>
        void DrawElement(PaintEventArgs e);

        /// <summary>
        /// Disables event raising by the element.
        /// </summary>
        void BeginUpdate();

        /// <summary>
        /// Enables event raising by the element.
        /// </summary>
        void EndUpdate();

        /// <summary>
        /// Sets the input focus to the element.
        /// </summary>
        /// <returns>True if the input focus request was successful; false otherwise.</returns>
        bool Focus();

        /// <summary>
        /// Applies the format to the element.
        /// </summary>
        /// <param name="format">Format to be attached to the element.</param>
        void ApplyFormat(IHTMLFormat format);
        #endregion
    }
}