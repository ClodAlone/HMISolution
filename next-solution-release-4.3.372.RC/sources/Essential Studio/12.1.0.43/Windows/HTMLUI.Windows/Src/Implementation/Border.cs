#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class which represents the border of the tag elements.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter)), Serializable]
    public class Border : IBorder, ICloneable
    {
        #region Class members
        /// <summary>
        /// Sets and retrieves the width of the border line.
        /// </summary>
        private int m_width;

        /// <summary>
        /// Sets and retrieves the style of the border line.
        /// </summary>
        private BordersStyle m_style;

        /// <summary>
        /// Sets and retrieves the color of the border line.
        /// </summary>
        private Color m_color;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the width of the border line.
        /// </summary>
        [Category("Appearance"), Browsable(true), Description("Gets or sets width of border line.")]
        public int Width
        {
            get
            {
                return m_width;
            }
            set
            {
                if (value < 0) return;

                if (value != m_width)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_width, value);
                    m_width = value;
                    OnWidthChanged(args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the style of the border line.
        /// </summary>
        [Category("Appearance"), Browsable(true), Description("Gets or sets style of border line.")]
        public BordersStyle Style
        {
            get
            {
                return m_style;
            }
            set
            {
                if (value != m_style)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_style, value);
                    m_style = value;
                    OnStyleChanged(args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the border line.
        /// </summary>
        [Category("Appearance"), Browsable(true), Description("Gets or sets color of border line.")]
        public Color Color
        {
            get
            {
                return m_color;
            }
            set
            {
                if (value != m_color)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_color, value);
                    m_color = value;
                    OnColorChanged(args);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether border is empty.
        /// </summary>
        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return Width == 0 && Style == BordersStyle.Solid && Color == Color.Black;
            }
        }
        #endregion

        #region Class events
        /// <summary>
        /// Event. Raised when the width property is changed.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler WidthChanged;

        /// <summary>
        /// Event. Raised when the style property is changed.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler StyleChanged;

        /// <summary>
        /// Event. Raised when the color property is changed.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler ColorChanged;
        #endregion

        #region  Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the Border class
        /// </summary>
        public Border()
            : this(0, BordersStyle.Solid, Color.Black)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Border class
        /// </summary>
        /// <param name="width">Width of the border.</param>
        /// <param name="style">Style of the border.</param>
        /// <param name="color">Color of the border.</param>
        public Border(int width, BordersStyle style, Color color)
        {
            m_width = width;
            m_style = style;
            m_color = color;
        }
        #endregion

        #region Class event raisers
        /// <summary>
        /// Raises the WidthChanged event.
        /// </summary>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected void RaiseWidthChanged(ValueChangedEventArgs args)
        {
            if (WidthChanged != null)
            {
                WidthChanged(this, args);
            }
        }

        /// <summary>
        /// Raises the StyleChanged event.
        /// </summary>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected void RaiseStyleChanged(ValueChangedEventArgs args)
        {
            if (StyleChanged != null)
            {
                StyleChanged(this, args);
            }
        }

        /// <summary>
        /// Raises the ColorChanged event.
        /// </summary>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected void RaiseColorChanged(ValueChangedEventArgs args)
        {
            if (ColorChanged != null)
            {
                ColorChanged(this, args);
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Raises the WidthChanged event.
        /// </summary>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnWidthChanged(ValueChangedEventArgs args)
        {
            RaiseWidthChanged(args);
        }

        /// <summary>
        /// Raises the StyleChanged event.
        /// </summary>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnStyleChanged(ValueChangedEventArgs args)
        {
            RaiseStyleChanged(args);
        }

        /// <summary>
        /// Raises the ColorChanged event.
        /// </summary>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnColorChanged(ValueChangedEventArgs args)
        {
            RaiseColorChanged(args);
        }
        #endregion

        #region Clone and Copy operations
        /// <summary>
        /// Clones object.
        /// </summary>
        /// <returns>Clone of this object.</returns>
        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Create a new copy of the existing object.
        /// </summary>
        /// <returns>Clone of this object.</returns>
        public IBorder Clone()
        {
            return (Border)((ICloneable)this).Clone();
        }

        /// <summary>
        /// Overloaded. Copies all settings of the current object to the new object.
        /// </summary>
        /// <param name="twin">Another object for properties copying.</param>
        public void CopyTo(IBorder twin)
        {
            this.CopyTo((Border)twin);
        }

        /// <summary>
        /// Copies all settings of the current object to a new object.
        /// </summary>
        /// <param name="twin">Border object for values copying.</param>
        public void CopyTo(Border twin)
        {
            twin.m_width = this.m_width;
            twin.m_style = this.m_style;
            twin.m_color = this.m_color;
        }
        #endregion

        #region Class Serializable methods
        /// <summary>
        /// Indicates whether the forecolor property is serialized.
        /// </summary>
        /// <returns>bool value</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldSerializeWidth()
        {
            return this.Width != 0;
        }

        /// <summary>
        /// Indicates whether the forecolor property is serialized.
        /// </summary>
        /// <returns>bool value</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldSerializeStyle()
        {
            return this.Style != BordersStyle.Solid;
        }

        /// <summary>
        /// Indicates whether the forecolor property is serialized.
        /// </summary>
        /// <returns>bool value</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldSerializeColor()
        {
            return this.Color != Color.Black;
        }
        #endregion
    }
}