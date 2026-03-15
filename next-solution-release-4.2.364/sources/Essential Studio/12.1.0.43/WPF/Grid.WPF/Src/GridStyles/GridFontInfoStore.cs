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
using System.Windows.Documents;
using System.Windows.Media;

using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.Styles;
using System.Windows.Interop;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Implements the data store for the <see cref="GridFontInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    [
    Serializable,
    StaticDataField("sd")
    ]
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
        public readonly static StyleInfoProperty FontStretchProperty = sd.CreateStyleInfoProperty(typeof(FontStretch), "FontStretch",9,true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.FontStyle"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FontStyleProperty = sd.CreateStyleInfoProperty(typeof(FontStyle), "FontStyle",2,true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.FontWeight"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FontWeightProperty = sd.CreateStyleInfoProperty(typeof(FontWeight), "FontWeight",15,true);

        ///// <summary>
        ///// Provides information about the <see cref="GridStyleInfo.Typography"/> property.
        ///// </summary>
        //public readonly static StyleInfoProperty TypographyProperty = sd.CreateStyleInfoProperty(typeof(Typography), "Typography");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.TextDecorations"/> property.
        /// </summary>
        public readonly static StyleInfoProperty TextDecorationsProperty = sd.CreateStyleInfoProperty(typeof(TextDecorationCollection), "TextDecorations");

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
            FontFamilyProperty.SerializeXmlBehavior = SerializeXmlBehavior.SerializeAsString;
            FontStretchProperty.SerializeXmlBehavior = SerializeXmlBehavior.SerializeAsString;
            FontStyleProperty.SerializeXmlBehavior = SerializeXmlBehavior.SerializeAsString;
            FontWeightProperty.SerializeXmlBehavior = SerializeXmlBehavior.SerializeAsString;
        }


#if !SyncfusionFramework4_0
        /// <summary>
        /// Initializes a new <see cref="GridFontInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        [System.Security.SecurityCritical()]
        protected GridFontInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
#if DEBUG
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                if (Switches.Serialization.TraceVerbose)
                    TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#endif
        }
#endif

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // I assume calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>Returns a copy of the current object.</summary>
        /// <returns>A copy of the current object.</returns>
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
