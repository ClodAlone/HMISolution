#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.Styles;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Cells
{
    /// <summary>
    /// Implements the data store for the <see cref="CellMarginsInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    [
    Serializable,
    StaticDataField("sd")
    ]
    public class CellMarginsInfoStore : StyleInfoStore, IDisposable 
    {
        static StaticData sd = new StaticData(typeof(CellMarginsInfoStore), typeof(CellMarginsInfo), true);

        /// <summary>
        /// Provides information about the <see cref="CellMarginsInfo.Left"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty LeftProperty = sd.CreateStyleInfoProperty(typeof(double), "Left");

        /// <summary>
        /// Provides information about the <see cref="CellMarginsInfo.Top"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty TopProperty = sd.CreateStyleInfoProperty(typeof(double), "Top");

        /// <summary>
        /// Provides information about the <see cref="CellMarginsInfo.Right"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty RightProperty = sd.CreateStyleInfoProperty(typeof(double), "Right");

        /// <summary>
        /// Provides information about the <see cref="CellMarginsInfo.Bottom"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty BottomProperty = sd.CreateStyleInfoProperty(typeof(double), "Bottom");

        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        /// <overload>
        /// Initializes a <see cref="CellMarginsInfoStore"/>
        /// </overload>
        /// <summary>
        /// Initializes an empty <see cref="CellMarginsInfoStore"/>.
        /// </summary>
        public CellMarginsInfoStore()
        {
        }

#if !SyncfusionFramework4_0
#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new <see cref="CellMarginsInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        [System.Security.SecurityCritical()]
        protected CellMarginsInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
#endif


        }
#endif
#endif
        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // I assume calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        public override object Clone()
        {
            StyleInfoStore target = new CellMarginsInfoStore();
            CopyTo(target);
            return target;
        }

        void IDisposable.Dispose()
        {
          
        }
    }

    /// <summary>
    /// Provides a <see cref="StyleInfoSubObjectBase"/> object for margins in a cell. Each margin side of
    /// the cell can be configured individually with a <see cref="Thickness"/> value. Margin sides that
    /// have not been initialized will inherit default values from a base style.
    /// </summary>
    /// <example>
    /// The following code changes margin information for cells:
    /// <code lang="C#">
    /// 
    ///             Thickness margins = new Thickness(1, 1, 2, 2);
    ///             model[rowIndex, colIndex].Margins = new CellMarginsInfo(margins);
    ///             model[rowIndex, colIndex+1].Margins.Right = 2;
    ///             model[rowIndex, colIndex+1].Margins.Left = 2;
    /// </code>
    /// </example>
    public class CellMarginsInfo : StyleInfoSubObjectBase, IDisposable 
    {
        // Static Fields
        private static CellMarginsInfo defaultMarginsInfo;
        private static CellMarginsInfo emptyMarginsInfo;

        /// <summary>
        /// Creates the CellMarginsInfo object.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="store">The store.</param>
        /// <returns></returns>
        public static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
                return new CellMarginsInfo(identity, store as CellMarginsInfoStore);
            return new CellMarginsInfo(identity);
        }

        /// <overload>
        /// Initializes a <see cref="CellMarginsInfo"/>
        /// </overload>
        /// <summary>
        /// Initializes a <see cref="CellMarginsInfo"/> and saves left, top, right and bottom margins.
        /// </summary>
        /// <param name="left">Left margin.</param>
        /// <param name="top">The top margin.</param>
        /// <param name="right">The right margin.</param>
        /// <param name="bottom">The bottom margin.</param>
        public CellMarginsInfo(double left, double top, double right, double bottom)
            : base(new CellMarginsInfoStore())
        {
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;
        }

        /// <summary>
        /// Initializes a <see cref="CellMarginsInfo"/> and copies settings from a <see cref="Thickness"/> object.
        /// </summary>
        public CellMarginsInfo(Thickness margins)
            : base(new CellMarginsInfoStore())
        {
            this.Left = margins.Left;
            this.Right = margins.Right;
            this.Top = margins.Top;
            this.Bottom = margins.Bottom;
        }

        /// <summary>
        /// Initializes a new empty <see cref="CellMarginsInfo"/> object.
        /// </summary>
        public CellMarginsInfo()
            : base(new CellMarginsInfoStore())
        {
        }

        /// <summary>
        /// Initializes a new empty <see cref="CellMarginsInfo"/> object with default value applied.
        /// </summary>
        public CellMarginsInfo(double value)
            : this(value, value, value, value)
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="CellMarginsInfo"/>  object and associates it with an existing <see cref="CachedStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="CachedStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="CellMarginsInfo"/>.
        /// </param>
        public CellMarginsInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new CellMarginsInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="CellMarginsInfo"/>  object and associates it with an existing <see cref="CachedStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="CachedStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="CellMarginsInfo"/>.
        /// <param name="store">A <see cref="CellMarginsInfoStore"/> that holds data for this <see cref="CellMarginsInfo"/>.
        /// All changes in this style object will saved in the <see cref="CellMarginsInfoStore"/> object.</param>
        /// </param>
        public CellMarginsInfo(StyleInfoSubObjectIdentity identity, CellMarginsInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Converts this object to a <see cref="Thickness"/> object.
        /// </summary>
        /// <returns>A <see cref="Thickness"/> object filled with the current objects settings.</returns>
        public Thickness ToThickness()
        {
            return new Thickness(Left, Top, Right, Bottom);
        }

        /// <summary>
        /// Creates a <see cref="Thickness"/> object comibing margins.
        /// </summary>
        /// <returns>A <see cref="Thickness"/> object filled with the current objects settings.</returns>
        public Thickness ToThickness(CellMarginsInfo other)
        {
            return new Thickness(Left + other.Left, Top + other.Top, Right + other.Right, Bottom + other.Bottom);
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // I assume calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        public override IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip)
        {
            return new CellMarginsInfo(newOwner.CreateSubObjectIdentity(sip), (CellMarginsInfoStore)Store.Clone());
        }

        /// <summary>
        /// Returns a default <see cref="CellMarginsInfo"/> to be used with a default style.
        /// </summary>
        public static CellMarginsInfo Default
        {
            get
            {
                if (defaultMarginsInfo == null)
                    defaultMarginsInfo = new CellMarginsInfo(1, 1, 1, 1);

                return defaultMarginsInfo;
            }
        }

        /// <summary>
        /// Returns a empty <see cref="CellMarginsInfo"/>.
        /// </summary>
        public static CellMarginsInfo Empty
        {
            get
            {
                if (emptyMarginsInfo == null)
                    emptyMarginsInfo = new CellMarginsInfo(0, 0, 0, 0);

                return emptyMarginsInfo;
            }
        }

        /// <override/>
        protected internal override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }

        // Properties
        #region Top
        /// <summary>
        /// The top margin
        /// </summary>
        [Description("Gets/Sets the top margin")]
        public double Top
        {
            get
            {
                return (double)GetValue(CellMarginsInfoStore.TopProperty);
            }
            set
            {
                SetValue(CellMarginsInfoStore.TopProperty, value);
            }
        }
        /// <summary>
        /// Resets the top margin
        /// </summary>
        public void ResetTop()
        {
            ResetValue(CellMarginsInfoStore.TopProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTop()
        {
            return HasValue(CellMarginsInfoStore.TopProperty);
        }
        /// <summary>
        /// Determines if the top margin has been initialized.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTop
        {
            get
            {
                return HasValue(CellMarginsInfoStore.TopProperty);
            }
        }
        #endregion
        #region Left
        /// <summary>
        /// The left margin
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("Gets/Sets the left margin")]
        public double Left
        {
            get
            {
                return (double)GetValue(CellMarginsInfoStore.LeftProperty);
            }
            set
            {
                SetValue(CellMarginsInfoStore.LeftProperty, value);
            }
        }
        /// <summary>
        /// Resets the left margin
        /// </summary>
        public void ResetLeft()
        {
            ResetValue(CellMarginsInfoStore.LeftProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeLeft()
        {
            return HasValue(CellMarginsInfoStore.LeftProperty);
        }
        /// <summary>
        /// Determines if the left margin has been initialized.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasLeft
        {
            get
            {
                return HasValue(CellMarginsInfoStore.LeftProperty);
            }
        }
        #endregion
        #region Bottom
        /// <summary>
        /// The bottom margin
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("Gets/Sets the bottom margin")]
        public double Bottom
        {
            get
            {
                return (double)GetValue(CellMarginsInfoStore.BottomProperty);
            }
            set
            {
                SetValue(CellMarginsInfoStore.BottomProperty, value);
            }
        }
        /// <summary>
        /// Resets the bottom margin
        /// </summary>
        public void ResetBottom()
        {
            ResetValue(CellMarginsInfoStore.BottomProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBottom()
        {
            return HasValue(CellMarginsInfoStore.BottomProperty);
        }
        /// <summary>
        /// Determines if the bottom margin has been initialized.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBottom
        {
            get
            {
                return HasValue(CellMarginsInfoStore.BottomProperty);
            }
        }
        #endregion
        #region Right
        /// <summary>
        /// The right margin
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("Gets/Sets the right margin")]
        public double Right
        {
            get
            {
                return (double)GetValue(CellMarginsInfoStore.RightProperty);
            }
            set
            {
                SetValue(CellMarginsInfoStore.RightProperty, value);
            }
        }
        /// <summary>
        /// Resets the right margin
        /// </summary>
        public void ResetRight()
        {
            ResetValue(CellMarginsInfoStore.RightProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeRight()
        {
            return HasValue(CellMarginsInfoStore.RightProperty);
        }
        /// <summary>
        /// Determines if the right margin has been initialized.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasRight
        {
            get
            {
                return HasValue(CellMarginsInfoStore.RightProperty);
            }
        }
        #endregion



         void IDisposable.Dispose()
        {
            emptyMarginsInfo = null;
            defaultMarginsInfo = null;
            this.identity = null;
            this.Identity.Dispose();
            this.Store.Dispose();

        }
    };

}
