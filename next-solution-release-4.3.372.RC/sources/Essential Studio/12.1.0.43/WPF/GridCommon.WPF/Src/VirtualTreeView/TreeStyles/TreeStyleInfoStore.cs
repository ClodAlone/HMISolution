#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows;
using Syncfusion.Windows.Controls.Cells;

using Syncfusion.Windows.Styles;

namespace Syncfusion.Windows.Controls.VirtualTreeView
{

    /// <summary>
    /// Provides storage for the <see cref="TreeStyleInfo"/> object.
    /// </summary>
    [
    Serializable,
    StaticDataField("sd")
    ]
    public class TreeStyleInfoStore : StyleInfoStore
    {
        static StaticData sd = new StaticData(typeof(TreeStyleInfoStore), typeof(TreeStyleInfo), false);

        internal static StaticData StaticData
        {
            get
            {
                return sd;
            }
        }

        /// <summary>
        /// Provides information about the <see cref="TreeStyleInfo.CellRenderer"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CellRendererProperty = sd.CreateStyleInfoProperty(typeof(TreeCellRenderer), "CellRenderer");

        /// <summary>
        /// Provides information about the <see cref="TreeStyleInfo.CellValue"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CellValueProperty = sd.CreateStyleInfoProperty(typeof(object), "CellValue");

        /// <summary>
        /// Provides information about the <see cref="TreeStyleInfo.CellValueType"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CellValueTypeProperty = sd.CreateStyleInfoProperty(typeof(Type), "CellValueType");

        /// <summary>
        /// Provides information about the <see cref="TreeStyleInfo.Format"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FormatProperty = sd.CreateStyleInfoProperty(typeof(string), "Format");

        /// <summary>
        /// Provides information about the <see cref="TreeStyleInfo.Background"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BackgroundProperty = sd.CreateStyleInfoProperty(typeof(string), "Background");

        /// <summary>
        /// Provides information about the <see cref="TreeStyleInfo.CultureInfo"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CultureInfoProperty = sd.CreateStyleInfoProperty(typeof(CultureInfo), "CultureInfo", StyleInfoPropertyOptions.Serializable);
        
        /// <summary>
        /// Provides information about the <see cref="TreeStyleInfo.BorderMargins"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BorderMarginsProperty = sd.CreateStyleInfoProperty(typeof(CellMarginsInfo), "BorderMargins");

        /// <summary>
        /// Provides information about the <see cref="TreeStyleInfo.Borders"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BordersProperty = sd.CreateStyleInfoProperty(typeof(CellBordersInfo), "Borders");

        /// <summary>
        /// Provides information about the <see cref="TreeStyleInfo.CellTemplate"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CellTemplateProperty = sd.CreateStyleInfoProperty(typeof(DataTemplate), "CellTemplate");

        /// <summary>
        /// Provides information about the <see cref="TreeStyleInfo.CellTemplateKey"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CellTemplateKeyProperty = sd.CreateStyleInfoProperty(typeof(string), "CellTemplateKey");

        /// <summary>
        /// Static data must be declared static in derived classes (this avoids collisions
        /// when StyleInfoStore is used in the same project for different types of style
        /// classes).
        /// </summary>
        /// <value></value>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }


        /// <overload>
        /// Initializes a <see cref="TreeStyleInfoStore"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="TreeStyleInfoStore"/>.
        /// </summary>
        public TreeStyleInfoStore()
        {
            if (sd.IsEmpty)
                new TreeStyleInfo();
        }

        static TreeStyleInfoStore()
        {
            BorderMarginsProperty.CreateObject = new CreateSubObjectHandler(CellMarginsInfo.CreateObject);
            BordersProperty.CreateObject = new CreateSubObjectHandler(CellBordersInfo.CreateObject);
            TreeStyleInfoStore.CellValueProperty.IsAnyObject = true;
        }

#if !SyncfusionFramework4_0
        /// <summary>
        /// Initializes a new <see cref="TreeStyleInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        [System.Security.SecurityCritical()]
        protected TreeStyleInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (sd.IsEmpty)
                new TreeStyleInfo();
        }
#endif
        /// <summary>
        /// Creates an exact copy of the current object.
        /// </summary>
        /// <returns>
        /// A <see cref="StyleInfoStore"/> with same data as the current object.
        /// </returns>
        public override object Clone()
        {
            StyleInfoStore target = new TreeStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }

}
