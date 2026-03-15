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
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// TableStyleRowProperties is used for representing formatting properties of the table. 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class TableStyleRowProperties : FormatBase
    {
        #region Class constants
        internal const int CellSpacingKey = 52;
        internal const int IsHiddenKey = 4;
        internal const int IsHeaderKey = 5;
        internal const int IsBreakAcrossPagesKey = 106;
        internal const int RowAlignmentKey = 105;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets / sets a boolean value indicating if row is hidden
        /// </summary>
        internal bool IsHidden
        {
            get
            {
                return (bool)GetPropertyValue(IsHiddenKey);
            }
            set
            {
                SetPropertyValue(IsHiddenKey, value);
            }
        }
        /// <summary>
        /// Gets / sets a boolean value indicating if row is table header
        /// </summary>
        internal bool IsHeader
        {
            get
            {
                return (bool)GetPropertyValue(IsHeaderKey);
            }
            set
            {
                SetPropertyValue(IsHeaderKey, value);
            }
        }
        /// <summary>
        /// Gets / sets a boolean value indicating if there is a break across pages
        /// </summary>
        internal bool IsBreakAcrossPages
        {
            get
            {
                return (bool)GetPropertyValue(IsBreakAcrossPagesKey);
            }
            set
            {
                SetPropertyValue(IsBreakAcrossPagesKey, value);
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
        /// Gets / sets horizontal alignment of the row. 
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
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the TableStyleRowProperties class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal TableStyleRowProperties(IWordDocument doc)
            : base(doc)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets as row format.
        /// </summary>
        /// <returns></returns>
        internal FormatBase GetAsRowFormat()
        {
            RowFormat rowFormat = new RowFormat();
            // Copy properties
            rowFormat.UpdateProperties(this);
            
            if (BaseFormat != null)
            {
                rowFormat.ApplyBase((BaseFormat as TableStyleRowProperties).GetAsRowFormat());
            }
            return rowFormat;
        }
        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <returns></returns>
        internal object GetPropertyValue(int propertyKey)
        {
            UpdateRowProperties(propertyKey);
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
        /// Updates the row properties.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        internal void UpdateRowProperties(int propertyKey)
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
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override object GetDefValue(int key)
        {
            switch (key)
            {
                case IsHiddenKey:
                    return false;
                case IsHeaderKey:
                    return false;
                case IsBreakAcrossPagesKey:
                    return true;
                case CellSpacingKey:
                    return (float)-1;
                case RowAlignmentKey:
                    return RowAlignment.Left;
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion
    }
}
