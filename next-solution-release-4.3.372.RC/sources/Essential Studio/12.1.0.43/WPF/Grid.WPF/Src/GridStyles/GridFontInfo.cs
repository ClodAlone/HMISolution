#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

using Syncfusion.Windows.Styles;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Provides a <see cref="StyleInfoSubObjectBase"/> object for font settings in a cell. 
    /// Each font property of the cell can be configured individually. <para/>
    /// Font properties thathave not been initialized will inherit default 
    /// values from a base style.
    /// </summary>
    [TypeConverter(typeof(StyleInfoBaseConverter))]
    public class GridFontInfo : StyleInfoSubObjectBase
    {
        // Static Fields
        private static GridFontInfo defaultFont;

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
                return new GridFontInfo(identity, store as GridFontInfoStore);
            return new GridFontInfo(identity);
        }

        private WeakReference _font = null;

        /// <summary>
        /// Releases the all resources used by the Component.
        /// </summary>
        public override void Dispose()
        {
            // Do not call font.Dispose here, gdi font object might have been assigned to a TextBox or other cell type.
            _font = null;
            base.Dispose();
        }

        // Constructors
        /// <overload>
        /// Initializes a <see cref="GridFontInfo"/>
        /// </overload>
        /// <summary>
        /// Initializes a <see cref="GridFontInfo"/>
        /// </summary>
        [DebuggerStepThrough()]
        public GridFontInfo()
            : base(new GridFontInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridFontInfo"/>  object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridFontInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridFontInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridFontInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridFontInfo"/>  object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridFontInfo"/>.
        /// <param name="store">A <see cref="GridFontInfoStore"/> that holds data for this <see cref="GridFontInfo"/>.
        /// All changes in this style object will saved in the <see cref="GridFontInfoStore"/> object.</param>
        /// </param>
        [DebuggerStepThrough()]
        public GridFontInfo(StyleInfoSubObjectIdentity identity, GridFontInfoStore store)
            : base(identity, store)
        {
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // I assume calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        [DebuggerStepThrough()]
        public override IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip)
        {
            return new GridFontInfo(newOwner.CreateSubObjectIdentity(sip), (GridFontInfoStore)Store.Clone());
        }

        /// <summary>
        /// Returns a default <see cref="GridFontInfo"/> to be used with a default style.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridStyleInfo.Default"/> of the <see cref="GridStyleInfo"/> class
        /// will return the default border info that this method generates through its
        /// overriden version of <see cref="GetDefaultStyle"/>.
        ///  </remarks>
        public static GridFontInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultFont == null)
                {
                    defaultFont = new GridFontInfo();
                    defaultFont.FontFamily = new FontFamily("Segoe UI");
                    defaultFont.FontSize = 12;
                    defaultFont.FontStretch = new FontStretch();
                    defaultFont.FontStyle = new FontStyle();
                    defaultFont.FontWeight = new FontWeight();
                    //defaultFont.Typography = null;
                    defaultFont.TextDecorations = null;
                    defaultFont.Orientation = 0;
                }

                return defaultFont;
            }
        }

        /// <override/>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }


        // Font
        #region FontFamily
        /// <summary>
        /// Gets or sets the font family of this <see cref="GridFontInfo"/> object.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public FontFamily FontFamily
        {
            get
            {
                return (FontFamily)GetValue(GridFontInfoStore.FontFamilyProperty);
            }
            set
            {
                SetValue(GridFontInfoStore.FontFamilyProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.FontFamily"/>.
        /// </summary>
        public void ResetFontFamily()
        {
            ResetValue(GridFontInfoStore.FontFamilyProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeFontFamily()
        {
            return HasValue(GridFontInfoStore.FontFamilyProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.FontFamily"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFontFamily
        {
            get
            {
                return HasValue(GridFontInfoStore.FontFamilyProperty);
            }
        }
        #endregion
        #region FontSize
        /// <summary>
        /// Gets or sets the font size of this <see cref="GridFontInfo"/> object.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public double FontSize
        {
            get
            {
                return (double)GetValue(GridFontInfoStore.FontSizeProperty);
            }
            set
            {
                SetValue(GridFontInfoStore.FontSizeProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.FontSize"/>.
        /// </summary>
        public void ResetFontSize()
        {
            ResetValue(GridFontInfoStore.FontSizeProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeFontSize()
        {
            return HasValue(GridFontInfoStore.FontSizeProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.FontSize"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFontSize
        {
            get
            {
                return HasValue(GridFontInfoStore.FontSizeProperty);
            }
        }
        #endregion
        #region FontStretch
        /// <summary>
        /// Gets or sets the degree to which the font has been stretched compared
        /// to the normal aspect ratio of the font.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public FontStretch FontStretch
        {
            get
            {
                return (FontStretch)GetValue(GridFontInfoStore.FontStretchProperty);
            }
            set
            {
                SetValue(GridFontInfoStore.FontStretchProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.FontStretch"/>.
        /// </summary>
        public void ResetFontStretch()
        {
            ResetValue(GridFontInfoStore.FontStretchProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeFontStretch()
        {
            return HasValue(GridFontInfoStore.FontStretchProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.FontStretch"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFontStretch
        {
            get
            {
                return HasValue(GridFontInfoStore.FontStretchProperty);
            }
        }
        #endregion
        #region FontStyle
        /// <summary>
        /// Gets or sets the font style of this <see cref="GridFontInfo"/> object.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public FontStyle FontStyle
        {
            get
            {
                return (FontStyle)GetValue(GridFontInfoStore.FontStyleProperty);
            }
            set
            {
                SetValue(GridFontInfoStore.FontStyleProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.FontStyle"/>.
        /// </summary>
        public void ResetFontStyle()
        {
            ResetValue(GridFontInfoStore.FontStyleProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeFontStyle()
        {
            return HasValue(GridFontInfoStore.FontStyleProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.FontStyle"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFontStyle
        {
            get
            {
                return HasValue(GridFontInfoStore.FontStyleProperty);
            }
        }
        #endregion
        #region FontWeight
        /// <summary>
        /// Gets or sets the font weight of this <see cref="GridFontInfo"/> object.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public FontWeight FontWeight
        {
            get
            {
                return (FontWeight)GetValue(GridFontInfoStore.FontWeightProperty);
            }
            set
            {
                SetValue(GridFontInfoStore.FontWeightProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.FontWeight"/>.
        /// </summary>
        public void ResetFontWeight()
        {
            ResetValue(GridFontInfoStore.FontWeightProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeFontWeight()
        {
            return HasValue(GridFontInfoStore.FontWeightProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.FontWeight"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFontWeight
        {
            get
            {
                return HasValue(GridFontInfoStore.FontWeightProperty);
            }
        }
        #endregion
        #region Orientation
        /// <summary>
        /// Gets or sets the orientation of this <see cref="GridFontInfo"/> object.
        /// </summary>
        [
        Browsable(true),
        Description("Gets or sets the orientation of this font object."),
        Category(""),
        ]
        public int Orientation
        {
            get
            {
                return (int) GetValue(GridFontInfoStore.OrientationProperty);
            }
            set
            {
                while (value < 0)
                    value += 360;

                while (value >= 360)
                    value -= 360;

                SetValue(GridFontInfoStore.OrientationProperty, value);
            }
        }
        /// <summary>
        /// Resets the <see cref="Orientation"/> property.
        /// </summary>
        public void ResetOrientation()
        {
            ResetValue(GridFontInfoStore.OrientationProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeOrientation()
        {
            return HasValue(GridFontInfoStore.OrientationProperty);
        }
        /// <summary>
        /// Determines if the <see cref="Orientation"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasOrientation
        {
            get
            {
                return HasValue(GridFontInfoStore.OrientationProperty);
            }
        }
        #endregion
        #region TextDecorations
        /// <summary>
        /// Gets or sets the text decoration of this <see cref="GridFontInfo"/> object.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public TextDecorationCollection TextDecorations
        {
            get
            {
                return (TextDecorationCollection)GetValue(GridFontInfoStore.TextDecorationsProperty);
            }
            set
            {
                SetValue(GridFontInfoStore.TextDecorationsProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.TextDecorations"/>.
        /// </summary>
        public void ResetTextDecorations()
        {
            ResetValue(GridFontInfoStore.TextDecorationsProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTextDecorations()
        {
            return HasValue(GridFontInfoStore.TextDecorationsProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.TextDecorations"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTextDecorations
        {
            get
            {
                return HasValue(GridFontInfoStore.TextDecorationsProperty);
            }
        }
        #endregion
        #region Typeface
        /// <summary>
        /// Gets the type face.
        /// </summary>
        /// <returns>Type face.</returns>
        public Typeface GetTypeface()
        {
            FontFamily fontFamily = FontFamily;
            FontStyle fontStyle = FontStyle;
            FontWeight fontWeight = FontWeight;
            FontStretch fontStretch = FontStretch;

            return new Typeface(fontFamily, fontStyle, fontWeight, fontStretch);
        }

        /// <summary>
        /// Creates or returns a cached Typeface generated from font information of
        /// this style object.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Typeface Typeface
        {
            get
            {
                Typeface font = null;
                if (_font != null && _font.IsAlive)
                    font = (Typeface)_font.Target;
                if (font == null)
                {
                    _font = new WeakReference(font = GetTypeface());
                }
                return font;
            }
        }

        /// <exclude/>
        /// <summary>Resets the type face.</summary>
        public void ResetTypeface()
        {
            _font = null;
        }

        protected override void OnStyleChanged(StyleInfoProperty sip)
        {
            _font = null;
            base.OnStyleChanged(sip);
        }

        /// <summary>
        /// Gets actual value of LineHeight property. If LineHeight is Double.Nan, returns FontSize*FontFamily.LineSpacing 
        /// </summary>
        public double GetLineHeightValue()
        {
            FontFamily fontFamily = FontFamily;
            double fontSize = FontSize;
            double lineHeight = fontFamily.LineSpacing * fontSize;
            return Math.Max(TextDpi.MinWidth, Math.Min(TextDpi.MaxWidth, lineHeight));
        }


        internal static class TextDpi
        {
            // ------------------------------------------------------------------
            // Minimum width for text measurement. 
            // -----------------------------------------------------------------
            internal static double MinWidth { get { return _minSize; } }

            // ------------------------------------------------------------------ 
            // Maximum width for text measurement.
            // ------------------------------------------------------------------ 
            internal static double MaxWidth { get { return _maxSize; } }

            // -----------------------------------------------------------------
            // Make sure that LS/PTS limitations are not exceeded. 
            // -----------------------------------------------------------------
            static TextDpi()
            {
                _scale = 300.0;
                _maxSizeInt = 0x3ffffffe;
                _minSizeInt = 0x00000001;
                _maxSize = ((double)_maxSizeInt) / _scale; // = 3,579,139.40 pixels 
                _minSize = ((double)_minSizeInt) / _scale; // > 0
                //_maxObjSize = _maxSize / 3;                // = 1,193,046.46 pixels 
            }

            // -----------------------------------------------------------------
            // Measuring unit for PTS presenters is int, but logical measuring 
            // for the rest of the system is is double.
            // Logical measuring dpi        = 96 
            // PTS presenters measuring dpi = 28800 
            // ------------------------------------------------------------------
            private static double _scale;

            // -----------------------------------------------------------------
            // PTS/LS limitation for max size.
            // ------------------------------------------------------------------ 
            private static int _maxSizeInt;
            private static double _maxSize;

            // ------------------------------------------------------------------
            // PTS/LS limitation for min size. 
            // -----------------------------------------------------------------
            private static int _minSizeInt;
            private static double _minSize;
        }

        #endregion
    };


}
