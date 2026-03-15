//-------------------------------------------------------------------------------------------------
// <copyright file="GridMargins.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using System.Text;

using Syncfusion.Diagnostics;
using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the data store for the <see cref="GridMarginsInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    [Serializable,
    StaticDataField("sd")]
    public class GridMarginsInfoStore : StyleInfoStore
    {
        static StaticData sd = new StaticData(typeof(GridMarginsInfoStore), typeof(GridMarginsInfo), true);

        /// <summary>
        /// Provides information about the <see cref="GridMarginsInfo.Left"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty LeftProperty = sd.CreateStyleInfoProperty(typeof(byte), "Left", 255, true);

        /// <summary>
        /// Provides information about the <see cref="GridMarginsInfo.Top"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty TopProperty = sd.CreateStyleInfoProperty(typeof(byte), "Top", 255, true);

        /// <summary>
        /// Provides information about the <see cref="GridMarginsInfo.Right"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty RightProperty = sd.CreateStyleInfoProperty(typeof(byte), "Right", 255, true);

        /// <summary>
        /// Provides information about the <see cref="GridMarginsInfo.Bottom"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty BottomProperty = sd.CreateStyleInfoProperty(typeof(byte), "Bottom", 255, true);

        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        /// <overload>
        /// Initializes a <see cref="GridMarginsInfoStore"/>
        /// </overload>
        /// <summary>
        /// Initializes an empty <see cref="GridMarginsInfoStore"/>.
        /// </summary>
        public GridMarginsInfoStore()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridMarginsInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridMarginsInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // I assume calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>Creates a copy of the current object.</summary>
        /// <returns>A duplicate of current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridMarginsInfoStore();
            CopyTo(target);
            return target;
        }
    }

    /// <summary>
    /// Provides a <see cref="StyleInfoSubObjectBase"/> object for margins in a cell. Each margin side of
    /// the cell can be configured individually with a <see cref="GridMargins"/> value. Margin sides that
    /// have not been initialized will inherit default values from a base style.
    /// </summary>
    /// <example>
    /// The following code changes margin information for cells:
    /// <code lang="C#">
    /// <para/>
    ///             GridMargins margins = new GridMargins(1, 1, 2, 2);
    ///             model[rowIndex, colIndex].Margins = new GridMarginsInfo(margins);
    ///             model[rowIndex, colIndex+1].Margins.Right = 2;
    ///             model[rowIndex, colIndex+1].Margins.Left = 2;
    /// </code>
    /// </example>
    public class GridMarginsInfo : GridStyleInfoSubObject
    {
        // Static Fields
        private static GridMarginsInfo defaultMarginsInfo;
        private static GridMarginsInfo emptyMarginsInfo;

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridMarginsInfo(identity, store as GridMarginsInfoStore);
            }

            return new GridMarginsInfo(identity);
        }

        /// <overload>
        /// Initializes a <see cref="GridMarginsInfo"/>
        /// </overload>
        /// <summary>
        /// Initializes a <see cref="GridMarginsInfo"/> and saves left, top, right and bottom margins.
        /// </summary>
        /// <param name="left">Left margin.</param>
        /// <param name="top">The top margin.</param>
        /// <param name="right">The right margin.</param>
        /// <param name="bottom">The bottom margin.</param>
        public GridMarginsInfo(int left, int top, int right, int bottom)
            : base(new GridMarginsInfoStore())
        {
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;
        }

        /// <summary>
        /// Initializes a <see cref="GridMarginsInfo"/> and copies settings from a <see cref="GridMargins"/> object.
        /// </summary>
        /// <param name="margins">A <see cref="GridMargins"/> object used to initialize the current object.</param>
        public GridMarginsInfo(GridMargins margins)
            : base(new GridMarginsInfoStore())
        {
            this.Left = margins.Left;
            this.Right = margins.Right;
            this.Top = margins.Top;
            this.Bottom = margins.Bottom;
        }

        /// <summary>
        /// Initializes a new empty <see cref="GridMarginsInfo"/> object.
        /// </summary>
        public GridMarginsInfo()
            : base(new GridMarginsInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridMarginsInfo"/>  object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridMarginsInfo"/>.
        /// </param>
        public GridMarginsInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridMarginsInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridMarginsInfo"/>  object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridMarginsInfo"/>.</param>
        /// <param name="store">A <see cref="GridMarginsInfoStore"/> that holds data for this <see cref="GridMarginsInfo"/>.
        /// All changes in this style object will saved in the <see cref="GridMarginsInfoStore"/> object.</param>
        public GridMarginsInfo(StyleInfoSubObjectIdentity identity, GridMarginsInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Converts this object to a <see cref="GridMargins"/> object.
        /// </summary>
        /// <returns>A <see cref="GridMargins"/> object filled with the current objects settings.</returns>
        public GridMargins ToMargins()
        {
            return new GridMargins(Left, Top, Right, Bottom);
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // I assume calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>Creates an exact copy of the current object.</summary>
        /// <param name="newOwner">The new owner style object for the copied object.</param>
        /// <param name="sip">An identifier for this object.</param>
        /// <returns>A duplicate object.</returns>
        public override IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip)
        {
            return new GridMarginsInfo(newOwner.CreateSubObjectIdentity(sip), (GridMarginsInfoStore)Store.Clone());
        }

        /// <summary>
        /// Gets a default <see cref="GridMarginsInfo"/> to be used with a default style.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridStyleInfo.Default"/> of the <see cref="GridStyleInfo"/> class
        /// will return the default margin info that this method generates through its
        /// overriden version of <see cref="GetDefaultStyle"/>.
        /// </remarks>
        public static GridMarginsInfo Default
        {
            get
            {
                if (defaultMarginsInfo == null)
                {
                    defaultMarginsInfo = new GridMarginsInfo(1, 1, 1, 1);
                }

                return defaultMarginsInfo;
            }
        }

        /// <summary>
        /// Gets a empty <see cref="GridMarginsInfo"/>.
        /// </summary>
        public static GridMarginsInfo Empty
        {
            get
            {
                if (emptyMarginsInfo == null)
                {
                    emptyMarginsInfo = new GridMarginsInfo(0, 0, 0, 0);
                }

                return emptyMarginsInfo;
            }
        }

        /// <summary>
        /// Override this method to return a default style object for your derived class.
        /// </summary>
        /// <returns>A default style object.</returns>
        /// <override/>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }

        // Properties
        #region Top
        /// <summary>
        /// Gets or sets the top margin
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("Gets/Sets the top margin")]
        public int Top
        {
            get
            {
                return (int)GetShortValue(GridMarginsInfoStore.TopProperty);
            }

            set
            {
                SetValue(GridMarginsInfoStore.TopProperty, (short)value);
            }
        }

        /// <summary>
        /// Resets the top margin
        /// </summary>
        public void ResetTop()
        {
            ResetValue(GridMarginsInfoStore.TopProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTop()
        {
            return HasValue(GridMarginsInfoStore.TopProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the top margin has been initialized.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTop
        {
            get
            {
                return HasValue(GridMarginsInfoStore.TopProperty);
            }
        }

        #endregion
        #region Left
        /// <summary>
        /// Gets or sets the left margin
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("Gets/Sets the left margin")]
        public int Left
        {
            get
            {
                return (int)GetShortValue(GridMarginsInfoStore.LeftProperty);
            }

            set
            {
                SetValue(GridMarginsInfoStore.LeftProperty, (short)value);
            }
        }

        /// <summary>
        /// Resets the left margin
        /// </summary>
        public void ResetLeft()
        {
            ResetValue(GridMarginsInfoStore.LeftProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeLeft()
        {
            return HasValue(GridMarginsInfoStore.LeftProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the left margin has been initialized.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasLeft
        {
            get
            {
                return HasValue(GridMarginsInfoStore.LeftProperty);
            }
        }

        #endregion
        #region Bottom
        /// <summary>
        /// Gets or sets the bottom margin
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("Gets/Sets the bottom margin")]
        public int Bottom
        {
            get
            {
                return (int)GetShortValue(GridMarginsInfoStore.BottomProperty);
            }

            set
            {
                SetValue(GridMarginsInfoStore.BottomProperty, (short)value);
            }
        }

        /// <summary>
        /// Resets the bottom margin
        /// </summary>
        public void ResetBottom()
        {
            ResetValue(GridMarginsInfoStore.BottomProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBottom()
        {
            return HasValue(GridMarginsInfoStore.BottomProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the bottom margin has been initialized.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBottom
        {
            get
            {
                return HasValue(GridMarginsInfoStore.BottomProperty);
            }
        }
        #endregion
        #region Right
        /// <summary>
        /// Gets or sets the right margin
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("Gets/Sets the right margin")]
        public int Right
        {
            get
            {
                return (int)GetShortValue(GridMarginsInfoStore.RightProperty);
            }

            set
            {
                SetValue(GridMarginsInfoStore.RightProperty, (short)value);
            }
        }

        /// <summary>
        /// Resets the right margin
        /// </summary>
        public void ResetRight()
        {
            ResetValue(GridMarginsInfoStore.RightProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeRight()
        {
            return HasValue(GridMarginsInfoStore.RightProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the right margin has been initialized.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasRight
        {
            get
            {
                return HasValue(GridMarginsInfoStore.RightProperty);
            }
        }
        #endregion
    }

    /// <summary>
    /// This is an immutable object that provides storage for top, left
    /// bottom and right margins in a cell.
    /// </summary>
    /// <remarks>
    /// This is different from <see cref="GridMarginsInfo"/>. This is a stand-alone
    /// class that does not implement any inheritance mechanism. It simply holds the
    /// specified values.
    /// </remarks>
    [ImmutableObject(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class GridMargins : ICloneable
    {
        // Fields
        internal int _left;
        internal int _right;
        internal int _top;
        internal int _bottom;

        // Constructors

        /// <overload>
        /// Initializes a <see cref="GridMargins"/> object.
        /// </overload>
        /// <summary>
        ///   <para> Initializes a new instance of the <see cref="GridMargins" /> class.</para>
        /// </summary>
        public GridMargins()
            : this(0, 0, 0, 0)
        {
        }

        /// <summary>
        ///   <para> Initializes a new instance of the <see cref="GridMargins" /> class with the specified left, right, top, and bottom
        /// margins.</para>
        /// </summary>
        /// <param name="left">The left margin.</param>        
        /// <param name="top">The top margin.</param>
        /// <param name="right">The right margin.</param>
        /// <param name="bottom">The bottom margin.</param>
        public GridMargins(int left, int top, int right, int bottom)
        {
            this._left = left;
            this._right = right;
            this._top = top;
            this._bottom = bottom;
        }

        /// <summary>
        /// Swaps right and left margins.
        /// </summary>
        /// <returns> A copy of this object with right and left margins swapped.</returns>
        public GridMargins SwapRightToLeft()
        {
            return new GridMargins(_right, _top, _left, _bottom);
        }

        // Methods

        /// <summary>
        ///   <para>
        ///  Retrieves a duplicate of this object, member by member.
        /// </para>
        /// </summary>
        /// <returns>
        ///   <para>
        ///  A duplicate of this object.
        /// </para>
        /// </returns>
        public virtual /*ICloneable*/ object Clone()
        {
            GridMargins margins = new GridMargins();
            margins._left = this._left;
            margins._right = this._right;
            margins._top = this._top;
            margins._bottom = this._bottom;
            return margins;
        }

        /// <override/>
        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>Hash code.</returns>
        public override /*Object*/ int GetHashCode()
        {
            return base.GetHashCode();
            ////            uint left0 = ((uint)(this._left));
            ////            uint right1 = ((uint)(this._right));
            ////            uint top2 = ((uint)(this._top));
            ////            uint bottom3 = ((uint)(this._bottom));
            ////            uint u4 = left0 ^ ((int) right1 << 13) | (int) right1 >> 19) ^ (top2 << ((uint)(26/*0x1a*/)) | top2 >> ((uint)(6))) ^ (bottom3 << ((uint)(7)) | bottom3 >> ((uint)(25/*0x19*/)));
            ////            return ((int)(u4));
        }

        /// <override/>
        /// <summary>Checks if the specified object and the current object are equal.</summary>
        /// <param name="obj">An object to compare.</param>
        /// <returns>True if they are equal; False otherwise.</returns>
        public override /*Object*/ bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            if (obj is GridMargins)
            {
                GridMargins margins = (GridMargins)obj;
                return margins._left == this._left &&
                    margins._right == this._right &&
                    margins._top == this._top &&
                    margins._bottom == this._bottom;
            }

            return false;
        } // end of method Equals
        
        /// <override/>
        /// <summary>Returns a string holding the current object.</summary>
        /// <returns>String representation of the current object.</returns>
        public override /*Object*/ string ToString()
        {
            return String.Concat("[GridMargins Left=", _left.ToString(), " Right=", _right.ToString(), " Top=", _top.ToString(), " Bottom=", _bottom.ToString(), "]");
        }

        /// <summary>
        /// Gets results of ToString method.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Info
        {
            get
            {
                return ToString();
            }
        }
        
        /// <summary>
        /// <para>Gets the left margin.</para>
        /// </summary>
        public int Left
        {
            get
            {
                return this._left;
            }
            ////            set
            ////            {
            ////                this._left = value;
            ////            } 
        }

        /// <summary>
        /// Gets the right margin.
        /// </summary>
        public int Right
        {
            get
            {
                return this._right;
            }
            ////            set
            ////            {
            ////                this._right = value;
            ////            } 
        }

        /// <summary>
        ///   <para> Gets the top margin.</para>
        /// </summary>
        public int Top
        {
            get
            {
                return this._top;
            }
            ////            set
            ////            {
            ////                this._top = value;
            ////            } 
        }

        /// <summary>
        ///   <para> Gets the bottom margin.</para>
        /// </summary>
        public int Bottom
        {
            get
            {
                return this._bottom;
            }
            ////            set
            ////            {
            ////                this._bottom = value;
            ////            } 
        }

        /// <summary>
        /// Gets the total of the left and right margin.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Width
        {
            get
            {
                return _left + _right;
            }
        }

        /// <summary>
        /// Gets the total of the top and bottom margin.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Height
        {
            get
            {
                return _top + _bottom;
            }
        }

        /// <overload>
        /// Adds margins to a given <see cref="Size"/> or <see cref="Rectangle"/>
        /// </overload>
        /// <summary>
        /// Adds margins to a given <see cref="Size"/>
        /// </summary>
        /// <param name="size">The origial <see cref="Size"/>.</param>
        /// <param name="margins">The <see cref="GridMargins"/> to be added.</param>
        /// <returns>The resulting <see cref="Size"/>.</returns>
        public static Size AddMargins(Size size, GridMargins margins)
        {
            size.Width += margins.Width;
            size.Height += margins.Height;
            return size;
        }

        /// <overload>
        /// Removes margins from a given <see cref="Size"/> or <see cref="Rectangle"/>
        /// </overload>
        /// <summary>
        /// Removes margins from a given <see cref="Size"/>
        /// </summary>
        /// <param name="size">The origial <see cref="Size"/>.</param>
        /// <param name="margins">The <see cref="GridMargins"/> to be removed.</param>
        /// <returns>The resulting <see cref="Size"/>.</returns>
        public static Size RemoveMargins(Size size, GridMargins margins)
        {
            if (margins.Width >= size.Width || margins.Height >= size.Height)
            {
                return Size.Empty;
            }
            else
            {
                size.Width -= margins.Width;
                size.Height -= margins.Height;
            }

            return size;
        }

        /// <summary>
        /// Adds margins to a given <see cref="Rectangle"/>
        /// </summary>
        /// <param name="bounds">The origial <see cref="Rectangle"/>.</param>
        /// <param name="margins">The <see cref="GridMargins"/> to be added.</param>
        /// <returns>The resulting <see cref="Rectangle"/>.</returns>
        public static Rectangle AddMargins(Rectangle bounds, GridMargins margins)
        {
            return Rectangle.FromLTRB(bounds.Left - margins.Left, bounds.Top - margins.Top, bounds.Right + margins.Right, bounds.Bottom + margins.Bottom);
        }

        /// <summary>
        /// Removes margins from a given <see cref="Rectangle"/>
        /// </summary>
        /// <param name="bounds">The origial <see cref="Rectangle"/>.</param>
        /// <param name="margins">The <see cref="GridMargins"/> to be removed.</param>
        /// <returns>The resulting <see cref="Rectangle"/>.</returns>
        public static Rectangle RemoveMargins(Rectangle bounds, GridMargins margins)
        {
            if (margins.Width >= bounds.Width || margins.Height >= bounds.Height)
            {
                return Rectangle.Empty;
            }
            else
            {
                return Rectangle.FromLTRB(bounds.Left + margins.Left, bounds.Top + margins.Top, bounds.Right - margins.Right, bounds.Bottom - margins.Bottom);
            }
        }
    }
}
