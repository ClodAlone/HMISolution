#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// The EnumEditComboBox class.
    /// </summary>
    /// <internalonly/>
    [ToolboxItem(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Syncfusion.Documentation.DocumentationExclude()]
    public class EnumEditComboBox : ComboBox
    {
        #region Members
        private int m_maxItemWidth = 0;
        private Type m_enumType;
        #endregion

        #region Properties
        /// <summary>
        /// Gets an object representing the collection of the items contained in this <see cref="T:System.Windows.Forms.ComboBox"/>.
        /// </summary>
        /// <value></value>
        /// <internalonly/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ComboBox.ObjectCollection Items
        {
            get { return base.Items; }
        }
        
        /// <summary>
        /// Gets or sets the type of the enum.
        /// </summary>
        /// <value>The type of the enum.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Type EnumType
        {
            get
            {
                return m_enumType;
            }

            set
            {
                m_enumType = value;
                this.DataSource = Enum.GetValues(value);
            }
        }

        /// <summary>
        /// Gets or sets the selected enum value.
        /// </summary>
        /// <value>The selected enum value.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SelectedEnumValue
        {
            get
            {
                return this.SelectedItem;
            }

            set
            {
                this.SelectedItem = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumEditComboBox"/> class.
        /// </summary>
        public EnumEditComboBox()
        {
            this.Sorted = false;
            this.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ComboBox.DropDown"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        /// <internalonly/>
        protected override void OnDropDown(System.EventArgs e)
        {
            this.DropDownWidth = m_maxItemWidth + 5;
            base.OnDropDown(e);
        }
        #endregion
    }

    /// <summary>
    /// The InteriorEditor class.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class InteriorEditor
    {
        #region Members
        private BrushInfo _brushInfo;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the interior.
        /// </summary>
        /// <value>The interior.</value>
        [CategoryAttribute("")]
        public BrushInfo Interior
        {
            get
            {
                return _brushInfo;
            }

            set
            {
                this._brushInfo = value;
            }
        }
        #endregion
    }

    /// <summary>
    /// The OffsetEditor class.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class OffsetEditor
    {
        private Size _shadowOffset;

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>The offset.</value>
        [CategoryAttribute("")]
        public Size Offset
        {
            get
            {
                return _shadowOffset;
            }

            set
            {
                _shadowOffset = value;
            }
        }
    }

    /// <summary>
    /// The FontEditor class.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class FontEditor
    {
        private ChartFontInfo _font;

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        [CategoryAttribute("")]
        public ChartFontInfo Font
        {
            get
            {
                return _font;
            }

            set
            {
                _font = value;
            }
        }
    }

    /// <summary>
    /// The TextEditor class.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class TextEditor
    {
        private string _text;
        private Color _textColor;
        private string _textFormat;
        private float _textOffset;
        private ChartTextOrientation _textOrientation;

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [CategoryAttribute("")]
        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>The color of the text.</value>
        [CategoryAttribute("")]
        public Color TextColor
        {
            get
            {
                return _textColor;
            }

            set
            {
                _textColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the text format.
        /// </summary>
        /// <value>The text format.</value>
        [CategoryAttribute("")]
        public string TextFormat
        {
            get
            {
                return _textFormat;
            }

            set
            {
                _textFormat = value;
            }
        }

        /// <summary>
        /// Gets or sets the text offset.
        /// </summary>
        /// <value>The text offset.</value>
        [CategoryAttribute("")]
        public float TextOffset
        {
            get
            {
                return _textOffset;
            }

            set
            {
                _textOffset = value;
            }
        }

        /// <summary>
        /// Gets or sets the text orientation.
        /// </summary>
        /// <value>The text orientation.</value>
        [CategoryAttribute("")]
        public ChartTextOrientation TextOrientation
        {
            get
            {
                return _textOrientation;
            }

            set
            {
                _textOrientation = value;
            }
        }
    }

    /// <summary>
    /// The SymbolEditor class.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class SymbolEditor
    {
        private ChartSymbolInfo _symbol;

        /// <summary>
        /// Gets or sets the symbol.
        /// </summary>
        /// <value>The symbol.</value>
        [CategoryAttribute("")]
        public ChartSymbolInfo Symbol
        {
            get
            {
                return _symbol;
            }

            set
            {
                _symbol = value;
            }
        }
    }
}
