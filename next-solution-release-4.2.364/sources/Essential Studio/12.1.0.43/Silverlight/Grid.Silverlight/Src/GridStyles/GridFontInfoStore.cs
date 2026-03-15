#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Runtime.Serialization;
using System.Windows;

#if !WinRT
using System.Windows.Documents;
using System.Windows.Media;
using Syncfusion.Windows.Styles;

namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.Styles;
using Windows.UI.Xaml.Media;
using Windows.UI.Text;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
    /// <summary>
    /// Implements the data store for the <see cref="GridFontInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    [
    StaticDataField("sd")
    ]
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridFontInfoStore : StyleInfoStore
    {
        static StaticData sd = new StaticData(typeof(GridFontInfoStore), typeof(GridFontInfo), true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.FontFamily"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FontFamilyProperty = sd.CreateStyleInfoProperty(typeof(FontFamily), "FontFamily");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.FontSize"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FontSizeProperty = sd.CreateStyleInfoProperty(typeof(double), "FontSize");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.FontStretch"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FontStretchProperty = sd.CreateStyleInfoProperty(typeof(FontStretch), "FontStretch");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.FontStyle"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FontStyleProperty = sd.CreateStyleInfoProperty(typeof(FontStyle), "FontStyle");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.FontWeight"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FontWeightProperty = sd.CreateStyleInfoProperty(typeof(FontWeight), "FontWeight");

        ///// <summary>
        ///// Provides information about the <see cref="GridStyleInfo.Typography"/> property.
        ///// </summary>
        //public readonly static StyleInfoProperty TypographyProperty = sd.CreateStyleInfoProperty(typeof(Typography), "Typography");
#if !WinRT
        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.TextDecorations"/> property.
        /// </summary>
        public readonly static StyleInfoProperty TextDecorationsProperty = sd.CreateStyleInfoProperty(typeof(TextDecorationCollection), "TextDecorations");
#endif
       /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Orientation"/> property.
        /// </summary>
        public readonly static StyleInfoProperty OrientationProperty = sd.CreateStyleInfoProperty(typeof(int), "Orientation");

        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        /// <overload>
        /// Initializes a <see cref="GridFontInfoStore"/>
        /// </overload>
        /// <summary>
        /// Initializes an empty <see cref="GridFontInfoStore"/>.
        /// </summary>
        public GridFontInfoStore()
        {
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // I assume calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        public override object Clone()
        {
            StyleInfoStore target = new GridFontInfoStore();
            CopyTo(target);
            return target;
        }

        static GridFontInfoStore()
        {
        }
    }

}
