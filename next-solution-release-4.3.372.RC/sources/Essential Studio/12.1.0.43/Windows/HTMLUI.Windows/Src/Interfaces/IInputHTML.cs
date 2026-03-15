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
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Interface which is between the Control and the HTMLParser.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public interface IInputHTML
    {
        /// <summary>
        /// Gets a value indicating whether input HTML document is loaded from file.
        /// </summary>
        bool IsFileName 
        { 
            get; 
        }

        /// <summary>
        /// Gets a value indicating whether input HTML document is loaded by Uri.
        /// </summary>
        bool IsUri 
        { 
            get;
        }

        /// <summary>
        /// Gets a value indicating whether input HTML document is loaded from stream.
        /// </summary>
        bool IsStream 
        { 
            get;
        }

        /// <summary>
        /// Gets the format manager object.
        /// </summary>
        FormatManager Formats 
        { 
            get; 
        }

        /// <summary>
        /// Gets the source for parser as file; Null otherwise.
        /// Property has the higher priority.
        /// </summary>
        string FileName 
        { 
            get; 
        }

        /// <summary>
        /// Gets the source for parser specified by URI; Null otherwise.
        /// Second property by priority for source checks.
        /// </summary>
        Uri Uri 
        { 
            get; 
        }

        /// <summary>
        /// Gets the source specified as stream; Null otherwise.
        /// Has the lowest priority.
        /// </summary>
        Stream Stream
        { 
            get; 
        }

        /// <summary>
        /// Gets the reference of output document which will be infilled by converted HTML.
        /// </summary>
        XmlDocument Document 
        { 
            get;
        }

        /// <summary>
        /// Gets the root element of the document. In most cases this is HTML tag.
        /// </summary>
        IHTMLElement Root
        { 
            get; 
        }

        /// <summary>
        /// Gets the instance to the Body tag element.
        /// </summary>
        /// <remarks>This element is the root for rendering.</remarks>
        IHTMLElement RenderRoot 
        { 
            get; 
        }

        /// <summary>
        /// Gets the exception object which occurred while document parsing and rendering.
        /// </summary>
        Exception RenderException
        { 
            get; 
        }

        /// <summary>
        /// Gets or sets the point where the document will start to paint.
        /// </summary>
        [Obsolete("Don't use this property. Use Margins property instead.")]
        Point StartPoint
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets the margins for the document.
        /// </summary>
        /// <remarks>This property exposes leftmargin, topmargin, rightmargin and bottommargin
        /// of the BODY tag.</remarks>
        Margins Margins 
        { 
            get; 
        }

        /// <summary>
        /// Gets or sets the client size of the document.
        /// </summary>
        Size ClientSize 
        {             
            get;
            set;
        }

        /// <summary>
        /// Gets an array of script compile errors.
        /// </summary>
        string[] CompileErrors 
        { 
            get; 
        }

        /// <summary>
        /// Gets or sets the current directory for this document.
        /// </summary>
        string CurrentDirectory { get; set; }

        /// <summary>
        /// Creates and return the hashtable where key is element's UniqueID and
        /// value is element reference.
        /// </summary>
        /// <returns>Hashtable of output document. UniqueID-to-Element</returns>
        Hashtable GetElementsByUniqueIdHash();

        /// <summary>
        /// Creates and returns the hashtable where key is Tag Name (Element.Name)
        /// and value is Array of elements with current name.
        /// </summary>
        /// <returns>Hashtable of output document. Element Name-to-Array of Elements</returns>
        Hashtable GetElementsByNameHash();

        /// <summary>
        /// Creates and returns the hashtable where key is UserID (id attribute
        /// specified by user for tag) and value is element reference.
        /// </summary>
        /// <returns>Hashtable of output document. UserID-to-Element</returns>
        Hashtable GetElementsByUserIdHash();

        /// <summary>
        /// Creates and returns the hashtable where key is UniqueID of element
        /// and value is Array of Formats which influence on element rendering.
        /// </summary>
        /// <returns>Hashtable of output document. UniqueID-to-Array of Formats</returns>
        Hashtable GetCSSFormatsToElementHash();

        /// <summary>
        /// Returns the html element by its unique ID, if such exists; NULL otherwise.
        /// </summary>
        /// <param name="uniqueID">Unique ID of the element.</param>
        /// <returns>Element object by its unique ID.</returns>
        IHTMLElement GetElementByUniqueId(string uniqueID);

        /// <summary>
        /// Returns the html element by its ID, defined in the HTML document if such exists;
        /// NULL otherwise.
        /// </summary>
        /// <param name="userID">ID defined in the HTML document.</param>
        /// <returns>Element by its ID defined in HTML document by id attribute.</returns>
        IHTMLElement GetElementByUserId(string userID);

        /// <summary>
        /// Returns the custom control with the specified parent tag element.
        /// </summary>
        /// <param name="parent">Parent element containing the custom control.</param>
        /// <returns>Control contained in the element if it exists; NULL otherwise.</returns>
        Control GetControlByElement(IHTMLElement parent);

        /// <summary>
        /// Returns an array of elements with the specified tag name.
        /// </summary>
        /// <param name="name">Name of the tag.</param>
        /// <returns>Array of elements with the specified name; NULL otherwise.</returns>
        IHTMLElement[] GetElementsByName(string name);

        /// <summary>
        /// Disables the momentary reaction of the document on some attributes changing.
        /// </summary>
        void BeginUpdate();

        /// <summary>
        /// Enables the momentary reaction of the document on some attributes changing.
        /// </summary>
        void EndUpdate();

        /// <summary>
        /// Draws the document to the specified region.
        /// </summary>
        /// <param name="e">Graphics context.</param>
        /// <param name="location">Start location for the drawing.</param>
        void Draw(PaintEventArgs e, Point location);

        /// <summary>
        /// Recalculates the document corresponding to the specified properties.
        /// </summary>
        void Recalculate();

        /// <summary>
        /// Converts the point from client coordinates to global coordinates.
        /// </summary>
        /// <param name="point">Point in client coordinates.</param>
        /// <returns>Point in global coordinates.</returns>
        Point ClientToGlobal(Point point);

        /// <summary>
        /// Converts the point from global coordinates to client coordinates.
        /// </summary>
        /// <param name="point">Point in global coordinates.</param>
        /// <returns>Point in client coordinates.</returns>
        Point GlobalToClient(Point point);
    }
}
