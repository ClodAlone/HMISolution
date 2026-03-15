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
    /// TableStyleTableProperties is used for representing formatting properties of the table. 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class TableStyleTableProperties : FormatBase
    {
        #region Class constants
        internal const int BordersKey = 1;
        internal const int PaddingsKey = 3;
        internal const int ColumnStripeKey = 4;
        internal const int RowStripeKey = 5;
        internal const int CellSpacingKey = 52;
        internal const int LeftIndentKey = 53;
        internal const int AllowPageBreaksKey = 8;
        internal const int RowAlignmentKey = 105;
        internal const int ShadingColorKey = 108;
        internal const int ForeColorKey = 111;
        internal const int TextureStyleKey = 110;
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
        /// Gets / sets the spacing between cells.
        /// </summary>
        internal float CellSpacing
        {
            get
            {
                return (float)GetPropertyValue(CellSpacingKey);
            }
            set
            {
                SetPropertyValue(CellSpacingKey, value);
            }
        }
        /// <summary>
        /// Gets / sets the table indent.
        /// </summary>
        internal float LeftIndent
        {
            get
            {
                return (float)GetPropertyValue(LeftIndentKey);
            }
            set
            {
                SetPropertyValue(LeftIndentKey, value);
            }
        }
        /// <summary>
        /// Gets / sets the boolean value indicating if table allow page breaks
        /// </summary>
        internal bool AllowPageBreaks
        {
            get
            {
                return (bool)GetPropertyValue(AllowPageBreaksKey);
            }
            set
            {
                SetPropertyValue(AllowPageBreaksKey, value);
            }
        }
        /// <summary>
        /// Gets / sets horizontal alignment of the table. 
        /// </summary>
        internal RowAlignment HorizontalAlignment
        {
            get
            {
                return (RowAlignment)GetPropertyValue(RowAlignmentKey);
            }
            set
            {
                SetPropertyValue(RowAlignmentKey, value);
            }
        }
        /// <summary>
        /// Gets / sets number of columns in column band.
        /// </summary>
        internal long ColumnStripe
        {
            get
            {
                return (long)GetPropertyValue(ColumnStripeKey);
            }
            set
            {
                SetPropertyValue(ColumnStripeKey, value);
            }
        }
        /// <summary>
        /// Gets / sets number of rows in row band.
        /// </summary>
        internal long RowStripe
        {
            get
            {
                return (long)GetPropertyValue(RowStripeKey);
            }
            set
            {
                SetPropertyValue(RowStripeKey, value);
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the TableStyleTableProperties class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal TableStyleTableProperties(IWordDocument doc)
            : base(doc)
        {
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Gets as table format.
        /// </summary>
        /// <returns></returns>
        internal FormatBase GetAsTableFormat()
        {
            RowFormat tableFormat = new RowFormat();
            // Copy properties
            tableFormat.UpdateProperties(this);
            
            if (BaseFormat != null)
            {
                tableFormat.ApplyBase((BaseFormat as TableStyleTableProperties).GetAsTableFormat());
            }

            return tableFormat;
        }
        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <returns></returns>
        internal object GetPropertyValue(int propertyKey)
        {
            UpdateTableProperties(propertyKey);
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
        /// Updates the table properties.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        internal void UpdateTableProperties(int propertyKey)
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

            Borders.ApplyBase((baseFormat as TableStyleTableProperties).Borders);
            Paddings.ApplyBase((baseFormat as TableStyleTableProperties).Paddings);
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
                case ColumnStripeKey:
                    return (long)1;
                case RowStripeKey:
                    return (long)1;
                case CellSpacingKey:
                    return (float)-1;
                case LeftIndentKey:
                    return (float)0;
                case AllowPageBreaksKey:
                    return true;
                case RowAlignmentKey:
                    return RowAlignment.Left;
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
