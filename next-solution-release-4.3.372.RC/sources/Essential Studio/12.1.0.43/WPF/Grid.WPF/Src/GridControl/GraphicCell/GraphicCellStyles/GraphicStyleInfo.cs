#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows.Styles;
using System.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Syncfusion.Windows.Controls.Grid
{
    public class GraphicStyleInfo : StyleInfoBase, IDisposable
    {
        #region Default
        
        private static GraphicStyleInfo defaultStyle = null;

        public static GraphicStyleInfo Default
        {
            get
            {
                if (GraphicStyleInfo.defaultStyle == null)
                {
                    defaultStyle = new GraphicStyleInfo();
                    defaultStyle.CellType = "RichTextBox";
                    defaultStyle.ReadOnly = false;
                    defaultStyle.Enabled = true;
                    defaultStyle.Background = new SolidColorBrush(Colors.White);
                    defaultStyle.BorderBrush = new SolidColorBrush(Colors.Black);
                    defaultStyle.BorderThickness = new Thickness(1);
                    defaultStyle.HorizontalAlignment = HorizontalAlignment.Left;
                    defaultStyle.VerticalAlignment = VerticalAlignment.Top;
                }
                return GraphicStyleInfo.defaultStyle;
            }
        }

        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }

        #endregion

        #region Ctor
        // Constructors.
        static GraphicStyleInfo()
        {
        }

        /// <summary>
        /// Initializes a new style object.
        /// </summary>
        public GraphicStyleInfo()
            : base(new GraphicStyleInfoStore())
        {
        }

        /// <summary>
        /// Initializes a new style object and copies all data from an existing style object.
        /// </summary>
        /// <param name="style">The style object that contains the original data.</param>
        public GraphicStyleInfo(GraphicStyleInfo style)
            : base(style.Store)
        {
        }

        /// <summary>
        /// Initializes a new style object and associates it with an existing <see cref="GridStyleInfoStore"/>.
        /// </summary>
        /// <param name="store">A <see cref="GridStyleInfoStore"/> that holds data for this <see cref="GridStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridStyleInfoStore"/> object.</param>
        public GraphicStyleInfo(GraphicStyleInfoStore store)
            : base(store)
        {
        }

        /// <summary>
        /// Initializes a new style object and associates it with an existing <see cref="GridStyleInfoIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoIdentity"/> that holds the indentity for this <see cref="GridStyleInfo"/>.
        /// </param>
        public GraphicStyleInfo(StyleInfoIdentityBase identity)
            : base(identity, new GraphicStyleInfoStore())
        {
        }

        /// <summary>
        /// Initializes a new style object and associates it with an existing <see cref="GridStyleInfoIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoIdentity"/> that holds the indentity for this <see cref="GridStyleInfo"/>.
        /// </param>
        /// <param name="store">A <see cref="GridStyleInfoStore"/> that holds data for this <see cref="GridStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridStyleInfoStore"/> object.
        /// </param>
        public GraphicStyleInfo(StyleInfoIdentityBase identity, GraphicStyleInfoStore store)
            : base(identity, store)
        {
        }
        #endregion

        #region Identity
        /// <summary>
        /// Holds identity information such as row and column index for the current <see cref="GridStyleInfo"/>.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public GraphicStyleInfoIdentity CellIdentity
        {
            get
            {
                return base.Identity as GraphicStyleInfoIdentity;
            }
            set
            {
                base.Identity = value;
            }
        }

        /// <summary>
        /// Returns the grid model.
        /// </summary>
        public GraphicModel GraphicModel
        {
            get
            {
                GraphicStyleInfoIdentity cellIdentity = CellIdentity;
                if (cellIdentity != null)
                    return CellIdentity.GridModel;
                return null;
            }
        }

        public GraphicCellControl GraphicCellControl
        {
            get
            {
                return (GraphicCellControl)GetValue(GraphicStyleInfoStore.GraphicCellControlProperty);
            }
            set
            {
                SetValue(GraphicStyleInfoStore.GraphicCellControlProperty, value);
            }
        }

        public string CellName
        {
            get
            {
                return (string)GetValue(GraphicStyleInfoStore.CellNameProperty);
            }
            set
            {
                SetValue(GraphicStyleInfoStore.CellNameProperty, value);
            }
        }

        /// <summary>
        /// Returns the cell row column index.
        /// </summary>
        public int CellIndex
        {
            get
            {
                GraphicStyleInfoIdentity cellIdentity = CellIdentity;
                if (cellIdentity != null)
                    return CellIdentity.CellIndex;
                return int.MinValue;
            }
        }

        /// <summary>
        /// The <see cref="GridStyleInfoStore"/> object that holds all the data for this style object.
        /// </summary>
        public new GraphicStyleInfoStore Store
        {
            get { return (GraphicStyleInfoStore)base.Store; }
        }

        /// <override/>
        /// <summary>
        /// Creates a new <see cref="StyleInfoSubObjectIdentity"/> object and associate it with
        /// this <see cref="GridStyleInfo"/> object.
        /// </summary>
        /// <param name="sip">The StyleInfoProperty descriptor for this subobject.</param>
        /// <returns>The <see cref="StyleInfoSubObjectIdentity"/> object that this method creates.</returns>
        public override StyleInfoSubObjectIdentity CreateSubObjectIdentity(StyleInfoProperty sip)
        {
            return new CachedStyleInfoSubObjectIdentity(this, sip);
        }

        /// <summary>
        /// Creates a new <see cref="GridStyleInfo"/> and copies its cell and identity information from the current object. The new
        /// instance will be made off line so that changes in this style object are not be stored in the GridData
        /// </summary>
        /// <returns>A new <see cref="GridStyleInfo"/> instance.</returns>
        /// <remarks>
        /// Lets a style object load base styles and default values but disables
        /// saving changes back to the grid. (see OnStyleChanged below)
        /// </remarks>
        public GraphicStyleInfo GetOffLineCopy()
        {
            return new GraphicStyleInfo(((GraphicStyleInfoIdentity)Identity).MakeOfflineIdentity(), (GraphicStyleInfoStore)Store.Clone());
        }
        #endregion

        #region Background

        [Description(""),Browsable(true),Category("Appearance")]
        public Brush Background
        {
            get
            {
                return (Brush)GetValue(GraphicStyleInfoStore.BackgroundProperty);
            }
            set
            {
                SetValue(GraphicStyleInfoStore.BackgroundProperty, value);
            }
        }

        public void ResetBackground()
        {
            ResetValue(GraphicStyleInfoStore.BackgroundProperty);
        }

        public bool HasBackground
        {

            get
            {
                return HasValue(GraphicStyleInfoStore.BackgroundProperty);
            }
        }
        #endregion

        #region Foreground

        public Brush Foreground
        {
            get
            {
                return (Brush)GetValue(GraphicStyleInfoStore.ForegroundProperty);
            }
            set
            {
                SetValue(GraphicStyleInfoStore.ForegroundProperty, value);
            }
        }

        public void ResetForeground()
        {
            ResetValue(GraphicStyleInfoStore.ForegroundProperty);
        }

        public bool HasForeground
        {
            get
            {
                return HasValue(GraphicStyleInfoStore.ForegroundProperty);
            }
        }
        #endregion

        #region CellType

        public string CellType
        {
            get
            {
                return (string)GetValue(GraphicStyleInfoStore.CellTypeProperty);
            }
            set
            {
                SetValue(GraphicStyleInfoStore.CellTypeProperty, value);
            }
        }

        public bool HasCellType
        {
            get
            {
                return HasValue(GraphicStyleInfoStore.CellTypeProperty);
            }
        }
        #endregion

        #region CellValue

        public object CellValue
        {
            get
            {
                return GetValue(GraphicStyleInfoStore.CellValueProperty);
            }
            set
            {
                SetValue(GraphicStyleInfoStore.CellValueProperty, value);
            }
        }

        public void ResetCellValue()
        {
            ResetValue(GraphicStyleInfoStore.CellValueProperty);
        }

        public bool HasCellValue
        {
            get
            {
                return HasValue(GraphicStyleInfoStore.CellValueProperty);
            }
        }
        #endregion

        #region Text
        public string Text
        {
            get
            {
                return (string)GetValue(GraphicStyleInfoStore.TextProperty);
            }
            set
            {
                SetValue(GraphicStyleInfoStore.TextProperty, value);
            }
        }
        #endregion

        #region ReadOnly

        [ Description(""), Browsable(true), Category("Data") ]
        public bool ReadOnly
        {
            get
            {
                return GetShortValue(GraphicStyleInfoStore.ReadOnlyProperty) != 0;
            }
            set
            {
                SetValue(GraphicStyleInfoStore.ReadOnlyProperty, value ? 1 : 0);
            }
        }
        
        public void ResetReadOnly()
        {
            ResetValue(GraphicStyleInfoStore.ReadOnlyProperty);
        }

        public bool HasReadOnly
        {
            get
            {
                return HasValue(GraphicStyleInfoStore.ReadOnlyProperty);
            }
        }
        #endregion

        #region Enabled
        /// <summary>
        /// Gets or sets a value indicating whether the Graphic cell is locked or not.
        /// </summary>
       public bool Enabled
        {
            get
            {
                return GetShortValue(GraphicStyleInfoStore.EnabledProperty) != 0;
            }
            set
            {
                SetValue(GraphicStyleInfoStore.EnabledProperty, value ? 1 : 0);
            }
        }
        
        public void ResetEnabled()
        {
            ResetValue(GraphicStyleInfoStore.EnabledProperty);
        }
        
        public bool HasEnabled
        {
            get
            {
                return HasValue(GraphicStyleInfoStore.EnabledProperty);
            }
        }
        #endregion

        #region BorderBrush

        public Brush BorderBrush
        {
            get
            {
                return (Brush)GetValue(GraphicStyleInfoStore.BorderBrushProperty);
            }
            set
            {
                SetValue(GraphicStyleInfoStore.BorderBrushProperty, value);
            }
        }

        public void ResetBorderBrush()
        {
            ResetValue(GraphicStyleInfoStore.BorderBrushProperty);
        }

        public bool HasBorderBrush
        {
            get
            {
                return HasValue(GraphicStyleInfoStore.BorderBrushProperty);
            }
        }

        #endregion

        #region BorderThickness

        public Thickness BorderThickness
        {
            get
            {
                return (Thickness)GetValue(GraphicStyleInfoStore.BorderThicknessProperty);
            }
            set
            {
                SetValue(GraphicStyleInfoStore.BorderThicknessProperty, value);
            }
        }

        public void ResetBorderThickness()
        {
            ResetValue(GraphicStyleInfoStore.BorderThicknessProperty);
        }

        public bool HasBorderThickness
        {
            get
            {
                return HasValue(GraphicStyleInfoStore.BorderThicknessProperty);
            }
        }
        #endregion

        #region HorizontalAlignment

        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment)GetValue(GraphicStyleInfoStore.HorizontalAlignmentProperty);
            }
            set
            {
                SetValue(GraphicStyleInfoStore.HorizontalAlignmentProperty, value);
            }
        }

        public void ResetHorizontalAlignment()
        {
            ResetValue(GraphicStyleInfoStore.HorizontalAlignmentProperty);
        }

        public bool HasHorizontalAlignment
        {
            get
            {
                return HasValue(GraphicStyleInfoStore.HorizontalAlignmentProperty);
            }
        }
        #endregion

        #region VerticalAlignment

        public VerticalAlignment VerticalAlignment
        {
            get
            {
                return (VerticalAlignment)GetValue(GraphicStyleInfoStore.VerticalAlignmentProperty);
            }
            set
            {
                SetValue(GraphicStyleInfoStore.VerticalAlignmentProperty, value);
            }
        }

        public void ResetVerticalAlignment()
        {
            ResetValue(GraphicStyleInfoStore.VerticalAlignmentProperty);
        }

        public bool HasVerticalAlignment
        {
            get
            {
                return HasValue(GraphicStyleInfoStore.VerticalAlignmentProperty);
            }
        }
        #endregion

    }
}
