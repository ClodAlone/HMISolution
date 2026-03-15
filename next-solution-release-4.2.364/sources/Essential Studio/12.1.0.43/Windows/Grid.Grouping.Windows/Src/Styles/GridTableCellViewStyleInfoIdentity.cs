//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableCellViewStyleInfoIdentity.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

using Syncfusion.Grouping;

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// Provides identity information for a temporary <see cref="GridTableCellStyleInfo"/> object
    /// that is created by the <see cref="GridTableControl.GetTableViewStyleInfo"/> of a <see cref="GridTableControl"/>
    /// just before a cell is drawn and disposed afterwards.
    /// </summary>
    /// <remarks>
    /// Changes made to style properties that have a GridTableCellViewStyleInfoIdentity object are not
    /// permanent. When the object is disposed, any changes are discarded. This has the benefit that
    /// you can change temporary the style settings for drawing the cell.
    /// </remarks>
    public class GridTableCellViewStyleInfoIdentity : GridTableCellStyleInfoIdentity
    {
        private GridTableCellStyleInfo parentStyle;
#if WEAKREF
        WeakReference __cachedBaseStyles;

        IStyleInfo[] cachedBaseStyles
        {
            get
            {
                if (__cachedBaseStyles != null)
                    return (IStyleInfo[]) __cachedBaseStyles.Target;
                return null;
            }
            set
            {
                if (value != null)
                    __cachedBaseStyles = new WeakReference(value);
                else
                    __cachedBaseStyles = null;
            }
        }

//        ~GridTableCellViewStyleInfoIdentity()
//        {
//        }

#else
        IStyleInfo[] cachedBaseStyles;
#endif

        /// <summary>
        /// The parent style object.
        /// </summary>
        public GridTableCellStyleInfo ParentStyle
        {
            get
            {
                return this.parentStyle;
            }

            set
            {
                this.parentStyle = value;
            }
        }

        private GridTableCellStyleInfoIdentity parentIdentity;

        /// <summary>
        /// The Identity object of the <see cref="ParentStyle"/> object.
        /// </summary>
        public GridTableCellStyleInfoIdentity ParentIdentity
        {
            get
            {
                return this.parentIdentity;
            }

            set
            {
                this.parentIdentity = value;
            }
        }

        /// <summary>
        /// Initializes the identity object.
        /// </summary>
        /// <param name="parentStyle">The parent style object.</param>
        /// <param name="parentIdentity">The Identity object of the <see cref="ParentStyle"/> object.</param>
        public GridTableCellViewStyleInfoIdentity(GridTableCellStyleInfo parentStyle, GridTableCellStyleInfoIdentity parentIdentity)
            : base(parentIdentity)
        {
            this.parentStyle = parentStyle;
            this.parentIdentity = parentIdentity;
        }

        /// <override/>
        /// <summary>Disposes the current object.</summary>
        public override void Dispose()
        {
            cachedBaseStyles = null;
            base.Dispose();
        }

        /// <summary>
        /// Overridden. Returns BaseStyles that define the inheritance of style properties.
        /// </summary>
        /// <remarks>
        /// Inheritance of style properties is defined by the <see cref="GridTableCellStyleInfoIdentity"/>
        /// object. It has a <see cref="GridTableCellStyleInfoIdentity.GetBaseStyles"/> method that returns
        /// the <see cref="GridStyleInfo"/> objects that form an inheritance chain. Check the
        /// <see cref="GridTableCellStyleInfoIdentity.GetBaseStyleNames"/> method to get string / debug
        /// information about the inheritance chain for a specific element. Also, the designer will show
        /// this debug information about the inheritance chain in a ToolTip when you hover the mouse over
        /// a cell within the "Preview and Edit" window.
        /// <para/>
        /// </remarks>
        /// <param name="thisStyleInfo">A reference to a <see cref="IStyleInfo"/>.</param>
        /// <returns>An array of BaseStyles.</returns>
        public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
        {
            if (cachedBaseStyles == null)
            {
                GridEngine engine = ParentIdentity.Table.Engine;

                ArrayList styleList = new ArrayList();

                //// In case BaseStyle was specified in PrepareViewStyleInfo
                AddBaseStyle(thisStyleInfo, styleList, engine);

                AddStyle(ParentStyle, styleList, engine);

                //// base Appearance, default styles ...
                AddStyles(this.parentIdentity.GetBaseStyles(thisStyleInfo), styleList, engine);

                cachedBaseStyles = new IStyleInfo[styleList.Count];
                styleList.CopyTo(cachedBaseStyles, 0);
            }

            return cachedBaseStyles;
        }

        /// <override/>
        /// <summary>Occurs when a property in <see cref="StyleInfoBase"/> is changed.</summary>
        /// <param name="style">A StyleInfoBase instance that was changed.</param>
        /// <param name="sip">An identity for the property to operate on.</param>
        public override void OnStyleChanged(StyleInfoBase style, StyleInfoProperty sip)
        {
            cachedBaseStyles = null;
        }
    }
}
