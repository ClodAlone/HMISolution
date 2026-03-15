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
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Documentation;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Defines the style of a <see cref="ChartLegendItem"/>.
    /// </summary>
    public class ChartLegendItemStyle : ICloneable
    {
        #region Keys enum
        /// <summary>
        /// Contains the keys for each properties of <see cref="ChartLegendItemStyle"/>.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public enum ChartLegendStyleKeys
        {
            /// <summary>
            /// The key of <see cref="ChartLegendItemStyle.Font"/> property.
            /// </summary>
            Font,

            /// <summary>
            /// The key of <see cref="ChartLegendItemStyle.Font"/> property.
            /// </summary>
            ImageIndex,

            /// <summary>
            /// The key of <see cref="ChartLegendItemStyle.ImageList"/> property.
            /// </summary>
            ImageList,

            /// <summary>
            /// The key of <see cref="ChartLegendItemStyle.Interior"/> property.
            /// </summary>
            Interior,

            /// <summary>
            /// The key of <see cref="ChartLegendItemStyle.RepresentationSize"/> property.
            /// </summary>
            RepresentationSize,

            /// <summary>
            /// The key of <see cref="ChartLegendItemStyle.ShowSymbol"/> property.
            /// </summary>
            ShowSymbol,

            /// <summary>
            /// The key of <see cref="ChartLegendItemStyle.Spacing"/> property.
            /// </summary>
            Spacing,

            /// <summary>
            /// The key of <see cref="ChartLegendItemStyle.Symbol"/> property.
            /// </summary>
            Symbol,

            /// <summary>
            /// The key of <see cref="ChartLegendItemStyle.Border"/> property.
            /// </summary>
            Border,

            /// <summary>
            /// The key of <see cref="ChartLegendItemStyle.TextColor"/> property.
            /// </summary>
            TextColor,

            /// <summary>
            /// The key of <see cref="ChartLegendItemStyle.Type"/> property.
            /// </summary>
            Type,

            /// <summary>
            /// The key of <see cref="ChartLegendItemStyle.ShowIcon"/> property.
            /// </summary>
            ShowIcon,

            /// <summary>
            /// The key of <see cref="ChartLegendItemStyle.IconAlignment"/> property.
            /// </summary>
            IconAlignment,

            /// <summary>
            /// The key of <see cref="ChartLegendItemStyle.TextAlignment"/> property.
            /// </summary>
            TextAlignment,

            /// <summary>
            /// The key of <see cref="ChartLegendItemStyle.VisibleCheckBox"/> property.
            /// </summary>
            VisibleCheckBox,

            /// <summary>
            /// The key of <see cref="ChartLegendItemStyle.ShowShadow"/> property.
            /// </summary>
            ShowShadow,

            /// <summary>
            /// The key of <see cref="ChartLegendItemStyle.ShadowOffset"/> property.
            /// </summary>
            ShadowOffset,

            /// <summary>
            /// The key of <see cref="ChartLegendItemStyle.ShadowColor"/> property.
            /// </summary>
            ShadowColor,

            /// <summary>
            /// The key of <see cref="ChartLegendItemStyle.BorderColor"/> property.
            /// </summary>
            BorderColor,

            /// <summary>
            /// The key of <see cref="ChartLegendItemStyle.Url"/> property.
            /// </summary>
            Url
        }
        #endregion

        #region Members
        private static readonly ChartLegendItemStyle s_default = ChartLegendItemStyle.CreateDefault();
        private string m_url = string.Empty;
        private Hashtable m_store = null;
        private ChartLegendItemStyle m_parensStyle = s_default;
        private bool m_isStyleChanges = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Url that is to be associated with a <see cref="ChartPoint"/>. This Url will be applied to the point if
        /// <see cref="EnableUrl"/> and <see cref="CalcRegion"/> property is set to True.This property is applicable only for ChartWeb.
        /// </summary>
        public String Url
        {
            get
            {
                m_url = (string)GetObject(ChartLegendStyleKeys.Url);
                if (m_url == null)
                    return m_url;
                if (m_url.StartsWith("www."))
                    return m_url = m_url.Insert(0, "http://");
                else
                    return m_url;
            }
            set
            {
                SetObject(ChartLegendStyleKeys.Url, value);
            }
        }
        /// <summary>
        /// Gets the default.
        /// </summary>
        /// <value>The default.</value>
        public static ChartLegendItemStyle Default
        {
            get
            {
                return s_default;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this style is empty.
        /// </summary>
        /// <value><c>true</c> if this style is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get
            {
                return m_store.Count == 0;
            }
        }
        internal bool IsStyleChanged
        {
            get
            {
                return m_isStyleChanges; 
            }

            set
            {
                m_isStyleChanges = value;
            }
        }
        /// <summary>
        /// Gets or sets the base style.
        /// </summary>
        /// <value>The base style.</value>
        public ChartLegendItemStyle BaseStyle
        {
            get
            {
                return m_parensStyle;
            }

            set
            {
                m_parensStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the font of the text.
        /// </summary>
        public Font Font
        {
            get
            {
                return GetObject(ChartLegendStyleKeys.Font) as Font;
            }

            set
            {
                SetObject(ChartLegendStyleKeys.Font, value);
            }
        }

        /// <summary>
        /// Gets or sets the image index value of the item in the item's image list.
        /// </summary>
        public int ImageIndex
        {
            get
            {
                return (int)GetObject(ChartLegendStyleKeys.ImageIndex);
            }

            set
            {
                SetObject(ChartLegendStyleKeys.ImageIndex, value);
            }
        }

        /// <summary>
        /// Gets or sets the ImageList associated with this item.
        /// </summary>
        public ChartImageCollection ImageList
        {
            get
            {
                return GetObject(ChartLegendStyleKeys.ImageList) as ChartImageCollection;
            }

            set
            {
                SetObject(ChartLegendStyleKeys.ImageList, value);
            }
        }

        /// <summary>
        /// Gets or sets the interior for the rectangular area that represents a legend.
        /// </summary>
        public BrushInfo Interior
        {
            get
            {
                return GetObject(ChartLegendStyleKeys.Interior) as BrushInfo;
            }

            set
            {
                SetObject(ChartLegendStyleKeys.Interior, value);
            }
        }

        /// <summary>
        /// Gets or sets the size of the rectangle holding the representation of the item.
        /// </summary>
        public Size RepresentationSize
        {
            get
            {
                return (Size)GetObject(ChartLegendStyleKeys.RepresentationSize);
            }

            set
            {
                SetObject(ChartLegendStyleKeys.RepresentationSize, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show symbol].
        /// </summary>
        /// <value><c>true</c> if [show symbol]; otherwise, <c>false</c>.</value>
        public bool ShowSymbol
        {
            get
            {
                return (bool)GetObject(ChartLegendStyleKeys.ShowSymbol);
            }

            set
            {
                SetObject(ChartLegendStyleKeys.ShowSymbol, value);
            }
        }

        /// <summary>
        /// Gets or sets the spacing of the item within the legend.
        /// </summary>
        public int Spacing
        {
            get
            {
                return (int)GetObject(ChartLegendStyleKeys.Spacing);
            }

            set
            {
                SetObject(ChartLegendStyleKeys.Spacing, value);
            }
        }

        /// <summary>
        /// Gets or sets the symbol that is to be associated with this item.
        /// </summary>
        public ChartSymbolInfo Symbol
        {
            get
            {
                return (ChartSymbolInfo)GetObject(ChartLegendStyleKeys.Symbol);
            }

            set
            {
                
                SetObject(ChartLegendStyleKeys.Symbol, value);
            }
        }

        /// <summary>
        /// Gets or sets the border that is to be associated with this item's border.
        /// </summary>
        public ChartLineInfo Border
        {
            get
            {
                return (ChartLineInfo)GetObject(ChartLegendStyleKeys.Border);
            }

            set
            {
                SetObject(ChartLegendStyleKeys.Border, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the text of the item.
        /// </summary>
        public Color TextColor
        {
            get
            {
                return (Color)GetObject(ChartLegendStyleKeys.TextColor);
            }

            set
            {
                SetObject(ChartLegendStyleKeys.TextColor, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the border of the item.
        /// </summary>
        public Color BorderColor
        {
            get
            {
                return (Color)GetObject(ChartLegendStyleKeys.BorderColor);
            }

            set
            {
                SetObject(ChartLegendStyleKeys.BorderColor, value);
            }
        }

        /// <summary>
        /// Gets or sets the type of representation for the legend item.
        /// </summary>
        public ChartLegendItemType Type
        {
            get
            {
                return (ChartLegendItemType)GetObject(ChartLegendStyleKeys.Type);
            }

            set
            {
                SetObject(ChartLegendStyleKeys.Type, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show icon].
        /// </summary>
        /// <value><c>true</c> if [show icon]; otherwise, <c>false</c>.</value>
        public bool ShowIcon
        {
            get
            {
                return (bool)GetObject(ChartLegendStyleKeys.ShowIcon);
            }

            set
            {
                SetObject(ChartLegendStyleKeys.ShowIcon, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the icon alignment.
        /// </summary>
        /// <value>The icon alignment.</value>
        public LeftRightAlignment IconAlignment
        {
            get
            {
                return (LeftRightAlignment)GetObject(ChartLegendStyleKeys.IconAlignment);
            }

            set
            {
                SetObject(ChartLegendStyleKeys.IconAlignment, value);
            }
        }
                       
        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        /// <value>The text alignment.</value>
        public VerticalAlignment TextAlignment
        {
            get
            {
                return (VerticalAlignment)GetObject(ChartLegendStyleKeys.TextAlignment);
            }

            set
            {
                SetObject(ChartLegendStyleKeys.TextAlignment, value);
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether [visible check box].
        /// </summary>
        /// <value><c>true</c> if [visible check box]; otherwise, <c>false</c>.</value>
        public bool VisibleCheckBox
        {
            get
            {
                return (bool)GetObject(ChartLegendStyleKeys.VisibleCheckBox);
            }

            set
            {
                SetObject(ChartLegendStyleKeys.VisibleCheckBox, value);
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether [show shadow].
        /// </summary>
        /// <value><c>true</c> if [show shadow]; otherwise, <c>false</c>.</value>
        public bool ShowShadow
        {
            get
            {
                return (bool)GetObject(ChartLegendStyleKeys.ShowShadow);
            }

            set
            {
                SetObject(ChartLegendStyleKeys.ShowShadow, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the shadow offset.
        /// </summary>
        /// <value>The shadow offset.</value>
        public Size ShadowOffset
        {
            get
            {
                return (Size)GetObject(ChartLegendStyleKeys.ShadowOffset);
            }

            set
            {
                SetObject(ChartLegendStyleKeys.ShadowOffset, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the color of the shadow.
        /// </summary>
        /// <value>The color of the shadow.</value>
        public Color ShadowColor
        {
            get
            {
                return (Color)GetObject(ChartLegendStyleKeys.ShadowColor);
            }

            set
            {
                SetObject(ChartLegendStyleKeys.ShadowColor, value);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartLegendItemStyle"/> class.
        /// </summary>
        public ChartLegendItemStyle()
        {
            m_store = new Hashtable();
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartLegendItemStyle"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        private ChartLegendItemStyle(Hashtable store)
        {
            m_store = store;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new ChartLegendItemStyle(m_store.Clone() as Hashtable);
        }

        /// <summary>
        /// Resets style value by the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Reset(ChartLegendStyleKeys key)
        {
            m_store.Remove(key);
        }

        /// <summary>
        /// Clears the style values.
        /// </summary>
        public void Clear()
        {
            m_store.Clear();
        }

        /// <summary>
        /// Creates the default style.
        /// </summary>
        /// <returns>Returns ChartLegendItemStyle instance.</returns>
        public static ChartLegendItemStyle CreateDefault()
        {
            ChartLegendItemStyle clis = new ChartLegendItemStyle();

            clis.BorderColor = Color.Black;
            clis.Font = null;
            clis.IconAlignment = LeftRightAlignment.Left;
            clis.TextAlignment = VerticalAlignment.Center;
            clis.TextColor = Color.Empty;
            clis.ImageIndex = -1;
            clis.ImageList = null;
            clis.Interior = new BrushInfo(Color.White);
            clis.RepresentationSize = new Size(20, 20);
            clis.ShadowColor = Color.Gray;
            clis.ShadowOffset = new Size(2, 2);
            clis.ShowIcon = true;
            clis.ShowSymbol = false;
            clis.ShowShadow = false;
            clis.Spacing = 0;
            clis.Symbol = ChartSymbolInfo.Default;
            clis.Border = ChartLineInfo.Default;
            clis.Type = ChartLegendItemType.Rectangle;
            clis.VisibleCheckBox = false;
            clis.Url = string.Empty;
            return clis;
        }

        /// <summary>
        /// Sets the parent to the lower level.
        /// </summary>
        /// <param name="style">The style.</param>
        public void SetToLowerLevel(ChartLegendItemStyle style)
        {
            ChartLegendItemStyle lStyle = this;

            while (lStyle.BaseStyle != s_default && lStyle.BaseStyle != style)
            {
                lStyle = lStyle.BaseStyle;
            }

            lStyle.BaseStyle = style;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets the object.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        private void SetObject(object key, object value)
        {
            m_store[key] = value;
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Return Oblect.</returns>
        private object GetObject(object key)
        {
            object res = null;

            if (m_store.ContainsKey(key))
            {
                res = m_store[key];
            }
            else if (m_parensStyle != null)
            {
                res = m_parensStyle.GetObject(key);
            }
            else
            {
                res = s_default.GetObject(key);
            }

            return res;
        }
        #endregion
    }
}
