#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    #region Enum
    /// <summary>
    /// Enum which indicates the type of the format.
    /// How the format is connected to HTML Element.
    /// </summary>
    public enum FormatType
    {
        /// <summary>
        /// Default Format.
        /// </summary>
        Default,

        /// <summary>
        /// Format is connected by class attribute ( &lt;td class="someStyle" &gt; )
        /// .someStyle { ... }.
        /// </summary>
        Class,

        /// <summary>
        /// Format is connected through id attribute ( &lt;td id="someStyle" &gt; ).
        /// #someStyle { ... }
        /// </summary>
        Id,

        /// <summary>
        /// Format is connected by equals of names &lt;someElement&gt;
        /// someElement { ... }.
        /// </summary>
        Name,

        /// <summary>
        /// Format is final to the element ( after merging ).
        /// </summary>
        Merged 
    }
    #endregion

    /// <summary>
    /// Special formatting which is used by the renderer to show the HTML element to the user.
    /// </summary>
    public interface IHTMLFormat
    {
        /// <summary>
        /// Gets the format which was inherited by this format.
        /// </summary>
        IHTMLFormat FormatParent 
        { 
            get;
        }

        /// <summary>
        /// Gets the index of the format in the document.
        /// </summary>
        long Index 
        { 
            get; 
        }

        /// <summary>
        /// Gets or sets the font which must be used for rendering the element of HTML.
        /// </summary>
        Font Font 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the fore color of the HTML element for rendering.
        /// </summary>
        Color ForeColor 
        { 
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets the background Color of the HTML Element.
        /// </summary>
        Color BackgroundColor 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the vertical Alignment of string if HTML element has one.
        /// </summary>
        StringAlignment VerticalAlign 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of string in HTML element rectangle.
        /// </summary>
        StringAlignment HorizontalAlign 
        { 
            get;
            set;
        }

        /// <summary>
        /// Gets the left border settings of the HTML Element.
        /// </summary>
        IBorder Left 
        { 
            get;
        }

        /// <summary>
        /// Gets the right border settings of the HTML Element.
        /// </summary>
        IBorder Right
        { 
            get;
        }

        /// <summary>
        /// Gets the top border settings of the HTML Element.
        /// </summary>
        IBorder Top 
        { 
            get;
        }

        /// <summary>
        /// Gets the bottom border settings of the HTML Element.
        /// </summary>
        IBorder Bottom 
        { 
            get; 
        }

        /// <summary>
        /// Gets or sets the cursor which must be used by the control on mouse enter into special HTML element.
        /// </summary>
        Cursor Cursor 
        { 
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets the amount of space to insert between the left border of the
        /// object and the content.
        /// </summary>
        Rectangle Padding 
        { 
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets the unique name of the format ( mostly used for CSS styles declarations ).
        /// </summary>
        string Name 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets the format type of the format.
        /// </summary>
        FormatType Type
        { 
            get; 
        }

        /// <summary>
        /// Gets the width of the element block.
        /// </summary>
        int Width 
        { 
            get; 
        }

        /// <summary>
        /// Gets the height of the element block.
        /// </summary>
        int Height 
        { 
            get;
        }

        /// <summary>
        /// Gets or sets the Background Image of the format.
        /// </summary>
        Bitmap BackgroundImage 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the background repeat property of the format.
        /// </summary>
        RepeatStyle BackgroundImageRepeat 
        { 
            get; 
            set; 
        }
         }
}