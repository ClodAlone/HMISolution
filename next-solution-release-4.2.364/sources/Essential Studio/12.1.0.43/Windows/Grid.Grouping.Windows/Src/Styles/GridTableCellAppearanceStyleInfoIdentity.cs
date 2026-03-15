//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableCellAppearanceStyleInfoIdentity.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
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
    /// Provides identity information for a <see cref="GridTableCellStyleInfo"/> object of a
    /// <see cref="GridTableCellAppearance"/> and defines the inheritance of style properties.
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
    public class GridTableCellAppearanceStyleInfoIdentity : StyleInfoIdentityBase
    {
        int version = -1;
        // Cache
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
#else
        IStyleInfo[] cachedBaseStyles;
#endif
        // Identity properties
        GridTableCellAppearance appearance;
        GridTableCellType tableCellType;
        GridEngine engine;

        /// <override/>
        /// <summary>Disposes this object.</summary>
        public override void Dispose()
        {
            cachedBaseStyles = null;
            engine = null;
            appearance = null;

            base.Dispose();
        }
        
        /// <summary>
        /// The parent <see cref="GridTableCellAppearance"/> that owns the <see cref="GridTableCellStyleInfo"/> object.
        /// </summary>
        public GridTableCellAppearance Appearance
        {
            get
            {
                return appearance;
            }

            set
            {
                appearance = value;
            }
        }

        /// <summary>
        /// The <see cref="GridTableCellType"/> that uniquely identifies the <see cref="GridTableCellStyleInfo"/>
        /// with the <see cref="GridTableCellAppearance"/> that owns it.
        /// </summary>
        public GridTableCellType TableCellType
        {
            get
            {
                return tableCellType;
            }
            
            set
            {
                tableCellType = value;
            }
        }

        /// <summary>
        /// The <see cref="GridEngine"/> this object belongs to.
        /// </summary>
        public GridEngine Engine
        {
            get
            {
                if (engine == null)
                {
                    IGridTableCellAppearanceSource owner = appearance.Owner;

                    // Base Apperance objects
                    while (owner != null)
                    {
                        if (owner is GridEngine)
                        {
                            engine = (GridEngine) owner;
                            break;
                        }

                        GridTableCellAppearance baseAppearance = owner.GetBaseAppearance();
                        if (baseAppearance == null)
                        {
                            break;
                        }

                        owner = baseAppearance.Owner;
                    }
                }

                return engine;
            }
          
            set
            {
                engine = value;
            }
        }

        /// <summary>
        /// Initializes the identity object.
        /// </summary>
        /// <param name="engine">The <see cref="GridEngine"/> this object belongs to.</param>
        /// <param name="appearance">The parent <see cref="GridTableCellAppearance"/> that owns the <see cref="GridTableCellStyleInfo"/> object.</param>
        /// <param name="tableCellType">The <see cref="GridTableCellType"/> that uniquely identifies the <see cref="GridTableCellStyleInfo"/>
        /// with the <see cref="GridTableCellAppearance"/> that owns it.</param>
        public GridTableCellAppearanceStyleInfoIdentity(GridEngine engine, GridTableCellAppearance appearance, GridTableCellType tableCellType)
        {
            this.appearance = appearance;
            this.tableCellType = tableCellType;
            this.engine = engine;
        }

        /// <summary>
        /// Initializes a new <see cref="GridStyleInfoIdentity"/> and copies its data from an existing object.
        /// </summary>
        /// <param name="other">The existing object to copy data from.</param>
        protected GridTableCellAppearanceStyleInfoIdentity(GridTableCellAppearanceStyleInfoIdentity other)
        {
            this.engine = other.engine;
            this.appearance = other.appearance;
            this.tableCellType = other.tableCellType;
        }

        void AddAppearanceStyle(ArrayList styleList, GridTableCellAppearance appearance)
        {
            if (appearance.IsModifiedStyle(this.tableCellType))
            {
                styleList.Add(appearance.GetStyle(this.tableCellType));
            }

            foreach (GridTableCellStyleInfo style in appearance.GetBaseStyles(this.tableCellType))
            {
                if (style != null)
                {
                    styleList.Add(style);
                }
            }
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
        /// <param name="thisStyleInfo">A reference to a <see cref="IStyleInfo"/></param>
        /// <returns>An array of base styles.</returns>
        public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
        {
            //// GridTableCellAppearanceStyleInfoIdentity.GetBaseStyles call triggered by:
            //// GridStyleInfo style = column.Appearance.AnyRecordFieldCell;
            
            //// Appearance owner
            IGridTableCellAppearanceSource owner = appearance.Owner;
            GridColumnDescriptor column = owner as GridColumnDescriptor;

            if (cachedBaseStyles == null || (column != null && version != column.Collection.Version) || (column == null && engine != null && version != engine.Version))
            {
                ArrayList styleList = new ArrayList();

                //// NOTE: If we use Engine property instead of engine field, PortfolioGrid sample will work
                GridTableCellStyleInfoIdentity.AddBaseStyle(thisStyleInfo, styleList, Engine);
                ////GridTableCellStyleInfoIdentity.AddBaseStyle( thisStyleInfo, styleList, engine );

                //// Appearance
                GridTableCellStyleInfoIdentity.AddStyles(appearance.GetBaseStyles(this.tableCellType), styleList, engine);

                //// Base Apperance objects
                while (owner != null)
                {
                    GridTableCellAppearance baseAppearance = owner.GetBaseAppearance();
                    if (baseAppearance == null)
                    {
                        break;
                    }

                    owner = baseAppearance.Owner;
                    if (owner is GridEngine)
                    {
                        break;
                    }

                    GridTableCellStyleInfoIdentity.AddStyle(baseAppearance.GetStyle(this.tableCellType), styleList, engine);
                    GridTableCellStyleInfoIdentity.AddStyles(baseAppearance.GetBaseStyles(this.tableCellType), styleList, engine);
                }

                //// Engine
                ////                if (engine != null)
                ////                {
                ////                    AddAppearanceSourceStyle(styleList, engine as IGridTableCellAppearanceSource);
                ////                }
                
                if (engine == null)
                {
                    engine = owner as GridEngine;
                }

                //// Default Cell Value Type
                if (column != null)
                {
                    if (tableCellType == GridTableCellType.AnyRecordFieldCell
                        || tableCellType == GridTableCellType.RecordFieldCell
                        || tableCellType == GridTableCellType.AlternateRecordFieldCell
                        || tableCellType == GridTableCellType.AddNewRecordFieldCell)
                    {
                        GridTableCellStyleInfoIdentity.AddColumnStyles(styleList, column);
                    }
                }

                //// Base Styles
                if (engine != null)
                {
                    GridTableCellStyleInfoIdentity.AddStyle(engine.Appearance.GetStyle(this.tableCellType), styleList, engine);
                    GridTableCellStyleInfoIdentity.AddStyles(engine.Appearance.GetBaseStyles(this.tableCellType), styleList, engine);

                    //// Optional default style (DefaultAppearance is not serialized...)
                    if (engine.InternalDefaultAppearanceSource != null)
                    {
                        GridTableCellStyleInfoIdentity.AddStyle(engine.DefaultAppearance.GetStyle(this.tableCellType), styleList, engine);
                        GridTableCellStyleInfoIdentity.AddStyles(engine.DefaultAppearance.GetBaseStyles(this.tableCellType), styleList, engine);
                    }
                }

                GridTableCellStyleInfoIdentity.AddStyle(GridTableCellAppearance.Default.GetStyle(this.tableCellType), styleList, engine);
                GridTableCellStyleInfoIdentity.AddStyles(GridTableCellAppearance.Default.GetBaseStyles(this.tableCellType), styleList, engine);
                if (engine != null)
                {
                    GridTableCellStyleInfoIdentity.AddStyle(engine.engineDefaultStyle, styleList, engine);
                }

                cachedBaseStyles = new IStyleInfo[styleList.Count];
                styleList.CopyTo(cachedBaseStyles);
                if (column != null && column.Collection != null)
                {
                    version = column.Collection.Version;
                }
                else if (engine != null)
                {
                    version = engine.Version;
                }
            }

            return cachedBaseStyles;
        }

        /// <override/>
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        [DebuggerStepThrough()] public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append(" {");
            sb.AppendFormat("{0}", tableCellType);
            if (appearance != null && appearance.Owner != null)
            {
                sb.AppendFormat(", Appearance.Owner = {0}", appearance.Owner.GetType().Name);
            }

            sb.Append(" }");
            return sb.ToString();
        }

        /// <override/>
        /// <summary>Occurs when a property in <see cref="StyleInfoBase"/> is changed.</summary>
        /// <param name="style">The StyleInfoBase instance that has changed.</param>
        /// <param name="sip">An identity for the property to operate on.</param>
        public override void OnStyleChanged(StyleInfoBase style, StyleInfoProperty sip)
        {
            cachedBaseStyles = null;
            if (this.engine != null && this.engine.TableModel != null && sip == GridStyleInfoStore.AutoSizeProperty && !this.Engine.UseOldListChangedHandler)
            {
                IGridTableCellAppearanceSource owner = appearance.Owner;
                GridColumnDescriptor column = owner as GridColumnDescriptor;

                if (style.GetValue(sip).Equals(true) && style.GetValue(GridStyleInfoStore.WrapTextProperty).Equals(false))
                    this.engine.TableModel.ColWidths.ResizeToFit(GridRangeInfo.Table(), GridResizeToFitOptions.NoShrinkSize | GridResizeToFitOptions.ResizeCoveredCells);
                else
                    if (style.GetValue(sip).Equals(true) && style.GetValue(GridStyleInfoStore.WrapTextProperty).Equals(true))
                        this.engine.TableModel.RowHeights.ResizeToFit(GridRangeInfo.Table(), GridResizeToFitOptions.NoShrinkSize | GridResizeToFitOptions.ResizeCoveredCells);

            }
            appearance.RaiseChanged(this.tableCellType, new StyleChangedEventArgs(sip));
        }

        /// <override/>
        /// <summary>Occurs before a property in <see cref="StyleInfoBase"/> is changing.</summary>
        /// <param name="style">The StyleInfoBase instance that has changed.</param>
        /// <param name="sip">An identity for the property to operate on.</param>
        public override void OnStyleChanging(StyleInfoBase style, StyleInfoProperty sip)
        {
            appearance.RaiseChanging(this.tableCellType, new StyleChangedEventArgs(sip));
        }

        /// <summary>
        /// Results of ToString method.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Info
        {
            get
            {
                return ToString();
            }
        }
    }
}
