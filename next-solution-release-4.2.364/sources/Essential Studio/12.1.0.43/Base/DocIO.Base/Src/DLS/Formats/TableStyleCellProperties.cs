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

#region file using directives
using System;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// TableStyleCellProperties is used for representing formatting properties of the table. 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class TableStyleCellProperties : FormatBase
    {
        #region Class constants
        internal const int BordersKey = 1;
        internal const int PaddingsKey = 3;
        internal const int TextWrapKey = 9;
        internal const int VerticalAlignmentKey = 2;
        internal const int ShadingColorKey = 4;
        internal const int ForeColorKey = 5;
        internal const int TextureStyleKey = 7;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets/sets the background color.
        /// </summary>
        internal Color BackColor
        {
            get
            {
                return (Color)GetPropertyValue(ShadingColorKey);
            }
            set
            {
                SetPropertyValue(ShadingColorKey, value);
            }
        }
        /// <summary>
        /// Gets/sets the foreground color.
        /// </summary>
        internal Color ForeColor
        {
            get
            {
                return (Color)GetPropertyValue(ForeColorKey);
            }
            set
            {
                SetPropertyValue(ForeColorKey, value);
            }
        }
        /// <summary>
        /// Gets or sets the texture style.
        /// </summary>
        /// <value>The texture style.</value>
        internal TextureStyle TextureStyle
        {
            get
            {
                return (TextureStyle)GetPropertyValue(TextureStyleKey);
            }
            set
            {
                SetPropertyValue(TextureStyleKey, value);
            }
        }

        /// <summary>
        /// Gets the borders.
        /// </summary>
        internal Borders Borders
        {
            get
            {
                return GetPropertyValue(BordersKey) as Borders;
            }
        }
        /// <summary>
        /// Gets the paddings.
        /// </summary>
        internal Paddings Paddings
        {
            get
            {
                return GetPropertyValue(PaddingsKey) as Paddings;
            }
        }
        /// <summary>
        /// Gets/sets vertical alignment of the cell.
        /// </summary>
        internal VerticalAlignment VerticalAlignment
        {
            get
            {
                return (VerticalAlignment)GetPropertyValue(VerticalAlignmentKey);
            }
            set
            {
                SetPropertyValue(VerticalAlignmentKey, value);
            }
        }
        /// <summary>
        /// Gets or sets a boolean value indicating whether to wrap text in cell or not.
        /// </summary>
        /// <value><c>true</c> if it specifies text wrap, set to <c>true</c>.</value>
        internal bool TextWrap
        {
            get
            {
                return (bool)this[TextWrapKey];
            }
            set
            {
                this[TextWrapKey] = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the TableStyleCellProperties class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal TableStyleCellProperties(IWordDocument doc)
            : base(doc)
        {
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <returns></returns>
        internal object GetPropertyValue(int propertyKey)
        {
            UpdateCellProperties(propertyKey);
            return this[propertyKey];
        }
        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <param name="value">The value.</param>
        internal void SetPropertyValue(int propertyKey, object value)
        {
            this[propertyKey] = value;
        }
        /// <summary>
        /// Updates the cell properties.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        internal void UpdateCellProperties(int propertyKey)
        {
            if (IsPropertyUpdated(propertyKey))
                return;
        }
        /// <summary>
        /// Determines whether the specified property key has value.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <returns>
        /// 	if the specified property key has value, set to <c>true</c>.
        /// </returns>
        internal override bool HasValue(int propertyKey)
        {
            if (HasKey(propertyKey))
                return true;

            return false;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Apply base style
        /// </summary>
        /// <param name="baseFormat"></param>
        internal override void ApplyBase(FormatBase baseFormat)
        {
            base.ApplyBase(baseFormat);

            Borders.ApplyBase((baseFormat as TableStyleCellProperties).Borders);
            Paddings.ApplyBase((baseFormat as TableStyleCellProperties).Paddings);
        }
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal override void EnsureComposites()
        {
            if (HasKey(BordersKey))
            {
                EnsureComposites(BordersKey);
            }
            if (HasKey(PaddingsKey))
            {
                EnsureComposites(PaddingsKey);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override object GetDefValue(int key)
        {
            switch (key)
            {
                case TextWrapKey:
                    return true;
                case VerticalAlignmentKey:
                    return VerticalAlignment.Top;
                case ShadingColorKey:
                    return Color.Empty;
                case ForeColorKey:
                    return Color.Empty;
                case TextureStyleKey:
                    return TextureStyle.TextureNone;
                default:
                    throw new NotImplementedException();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override FormatBase GetDefComposite(int key)
        {
            switch (key)
            {
                case BordersKey:
                    return GetDefComposite(BordersKey, new Borders(this, BordersKey));
                case PaddingsKey:
                    return GetDefComposite(PaddingsKey, new Paddings(this, PaddingsKey));
            }
            return null;
        }
        #endregion
    }
}