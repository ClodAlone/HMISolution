//-------------------------------------------------------------------------------------------------
// <copyright file="GridBordersInfo.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the data store for the <see cref="GridBordersInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    [Serializable,
    StaticDataField("sd")]
    public class GridBordersInfoStore : StyleInfoStore
    {
        static StaticData sd = new StaticData(typeof(GridBordersInfoStore), typeof(GridBordersInfo), true);

        /// <summary>
        /// Provides information about the <see cref="GridBordersInfo.Top"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty TopProperty = sd.CreateStyleInfoProperty(typeof(GridBorder), "Top");

        /// <summary>
        /// Provides information about the <see cref="GridBordersInfo.Left"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty LeftProperty = sd.CreateStyleInfoProperty(typeof(GridBorder), "Left");

        /// <summary>
        /// Provides information about the <see cref="GridBordersInfo.Bottom"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty BottomProperty = sd.CreateStyleInfoProperty(typeof(GridBorder), "Bottom");

        /// <summary>
        /// Provides information about the <see cref="GridBordersInfo.Right"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty RightProperty = sd.CreateStyleInfoProperty(typeof(GridBorder), "Right");

        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        /// <overload>
        /// Initializes a <see cref="GridBordersInfoStore"/>
        /// </overload>
        /// <summary>
        /// Initializes an empty <see cref="GridBordersInfoStore"/>
        /// </summary>
        public GridBordersInfoStore()
        {
        }

        static GridBordersInfoStore()
        {
            TopProperty.SerializeXmlBehavior = SerializeXmlBehavior.SerializeAsString;
            LeftProperty.SerializeXmlBehavior = SerializeXmlBehavior.SerializeAsString;
            BottomProperty.SerializeXmlBehavior = SerializeXmlBehavior.SerializeAsString;
            RightProperty.SerializeXmlBehavior = SerializeXmlBehavior.SerializeAsString;
        }

        /// <summary>
        /// Initializes a new <see cref="GridBordersInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        private GridBordersInfoStore(SerializationInfo info, StreamingContext context)
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
        /// <summary>
        /// Creates an exact copy of the current object.
        /// </summary>
        /// <returns>Copy of current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridBordersInfoStore();
            CopyTo(target);
            return target;
        }
    }

    /// <summary>
    /// Provides a <see cref="StyleInfoSubObjectBase"/> object for borders in a cell. Each border side of
    /// the cell can be configured individually with a <see cref="GridBorder"/> value. Border sides that
    /// have not been initialized will inherit default values from a base style.
    /// </summary>
    /// <example>
    /// The following code changes border information for cells:
    /// <code lang="C#">
    /// <para/>
    ///             GridBorder border = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(57, 73, 122));
    ///             model[rowIndex, colIndex].Borders.Bottom = border;
    ///             model[rowIndex, colIndex].Borders.Right = border;
    /// </code>
    /// The following code hides grid lines for specific cells:
    /// <code lang="C#">
    ///             GridBorder border = new GridBorder(GridBorderStyle.None);
    ///             model[rowIndex, colIndex].Borders.Bottom = border;
    ///             model[rowIndex, colIndex].Borders.Right = border;
    /// </code>
    /// </example>
    public class GridBordersInfo : GridStyleInfoSubObject
    {
        // Static Fields
        private static GridBordersInfo defaultBorders;

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridBordersInfo(identity, store as GridBordersInfoStore);
            }

            return new GridBordersInfo(identity);
        }

        // Constructors

        /// <overload>
        /// Initializes a new empty <see cref="GridBordersInfo"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridBordersInfo"/> object.
        /// </summary>
        [DebuggerStepThrough()]
        public GridBordersInfo()
            : base(new GridBordersInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridBordersInfo"/>  object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridBordersInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridBordersInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridBordersInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridBordersInfo"/>  object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridBordersInfo"/></param>.
        /// <param name="store">A <see cref="GridBordersInfoStore"/> that holds data for this <see cref="GridBordersInfo"/>.
        /// All changes in this style object will saved in the <see cref="GridBordersInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public GridBordersInfo(StyleInfoSubObjectIdentity identity, GridBordersInfoStore store)
            : base(identity, store)
        {
        }

        /// <override/>
        /// <summary>
        /// Makes an exact copy of the current object.
        /// </summary>
        /// <param name="newOwner">The new owner style object for the copied object.</param>
        /// <param name="sip">The identifier for this object.</param>
        /// <returns>The copy of the current object.</returns>
        [DebuggerStepThrough()]
        public override IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip)
        {
            return new GridBordersInfo(newOwner.CreateSubObjectIdentity(sip), (GridBordersInfoStore)Store.Clone());
        }

        // Default

        /// <summary>
        /// Gets a default <see cref="GridBordersInfo"/> to be used with a default style.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridStyleInfo.Default"/> of the <see cref="GridStyleInfo"/> class
        /// will return the default border info that this method generates through its
        /// overriden version of <see cref="GetDefaultStyle"/>.
        /// </remarks>
        public static GridBordersInfo Default
        {
            get
            {
                if (defaultBorders == null)
                {
                    defaultBorders = new GridBordersInfo();
                    defaultBorders.Top = new GridBorder(GridBorderStyle.Standard, SystemColors.WindowFrame, GridBorderWeight.Thin);
                    defaultBorders.Left = new GridBorder(GridBorderStyle.Standard, SystemColors.WindowFrame, GridBorderWeight.Thin);
                    defaultBorders.Bottom = new GridBorder(GridBorderStyle.Standard, SystemColors.WindowFrame, GridBorderWeight.Thin);
                    defaultBorders.Right = new GridBorder(GridBorderStyle.Standard, SystemColors.WindowFrame, GridBorderWeight.Thin);
                }

                return defaultBorders;
            }
        }

        /// <summary>
        /// Returns <see cref="GridBordersInfo.Default"/>
        /// </summary>
        /// <returns>A <see cref="GridBordersInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }

        /// <summary>
        /// Sets all four border sides with one command.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// model[2, 2].Borders.All = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(100, 238, 122, 3));
        /// </code>
        /// </example>
        [Browsable(false)]
        public GridBorder All
        {
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridBordersInfoStore.TopProperty, value);
                SetValue(GridBordersInfoStore.LeftProperty, value);
                SetValue(GridBordersInfoStore.BottomProperty, value);
                SetValue(GridBordersInfoStore.RightProperty, value);
            }
        }

        /// <summary>
        /// Resets all four border sides with one command.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetAll()
        {
            ResetValue(GridBordersInfoStore.TopProperty);
            ResetValue(GridBordersInfoStore.LeftProperty);
            ResetValue(GridBordersInfoStore.BottomProperty);
            ResetValue(GridBordersInfoStore.RightProperty);
        }

        /// <summary>
        /// Returns the <see cref="GridBorder"/> for the specified <see cref="GridBorderSide"/>
        /// </summary>
        public GridBorder this[GridBorderSide side]
        {
            [DebuggerStepThrough()]
            get
            {
                switch (side)
                {
                    case GridBorderSide.Top: return Top;
                    case GridBorderSide.Left: return Left;
                    case GridBorderSide.Right: return Right;
                    case GridBorderSide.Bottom: return Bottom;
                }

                throw new ArgumentException("Unknown value", "side");
            }

            [DebuggerStepThrough()]
            set
            {
                switch (side)
                {
                    case GridBorderSide.Top: 
                        Top = value; 
                        break;
                    case GridBorderSide.Left: 
                        Left = value; 
                        break;
                    case GridBorderSide.Right: 
                        Right = value; 
                        break;
                    case GridBorderSide.Bottom: 
                        Bottom = value; 
                        break;
                    default:
                        throw new ArgumentException("Unknown value", "side");
                }
            }
        }

        //// Properties

        #region Top
        /// <summary>
        /// Gets or sets the top border
        /// </summary>
        [Browsable(true),
        SRCategory("StyleCategoryAppearance"),
        Description("The top border"),
        NotifyParentProperty(true)]
        public GridBorder Top
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridBorder)GetValue(GridBordersInfoStore.TopProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridBordersInfoStore.TopProperty, value);
            }
        }

        /// <summary>
        /// Resets the top border
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetTop()
        {
            ResetValue(GridBordersInfoStore.TopProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTop()
        {
            return HasValue(GridBordersInfoStore.TopProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the top border has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTop
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridBordersInfoStore.TopProperty);
            }
        }

        #endregion
        #region Left
        /// <summary>
        /// Gets or sets the left border
        /// </summary>
        [Browsable(true),
        SRCategory("StyleCategoryAppearance"),
        Description("The left border"),
        NotifyParentProperty(true)]
        public GridBorder Left
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridBorder)GetValue(GridBordersInfoStore.LeftProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridBordersInfoStore.LeftProperty, value);
            }
        }

        /// <summary>
        /// Resets the left border
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetLeft()
        {
            ResetValue(GridBordersInfoStore.LeftProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeLeft()
        {
            return HasValue(GridBordersInfoStore.LeftProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the left border has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasLeft
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridBordersInfoStore.LeftProperty);
            }
        }

        #endregion
        #region Bottom
        /// <summary>
        /// Gets or sets the bottom border
        /// </summary>
        [Browsable(true),
        SRCategory("StyleCategoryAppearance"),
        Description("The bottom border"),
        NotifyParentProperty(true)]
        public GridBorder Bottom
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridBorder)GetValue(GridBordersInfoStore.BottomProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridBordersInfoStore.BottomProperty, value);
            }
        }

        /// <summary>
        /// Resets the bottom border
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetBottom()
        {
            ResetValue(GridBordersInfoStore.BottomProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBottom()
        {
            return HasValue(GridBordersInfoStore.BottomProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the bottom border has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBottom
        {
            get
            {
                return HasValue(GridBordersInfoStore.BottomProperty);
            }
        }

        #endregion
        #region Right
        /// <summary>
        /// Gets or sets the right border
        /// </summary>
        [Browsable(true),
        SRCategory("StyleCategoryAppearance"),
        Description("The right border"),
        NotifyParentProperty(true)]
        public GridBorder Right
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridBorder)GetValue(GridBordersInfoStore.RightProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridBordersInfoStore.RightProperty, value);
            }
        }

        /// <summary>
        /// Resets the right border
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetRight()
        {
            ResetValue(GridBordersInfoStore.RightProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeRight()
        {
            return HasValue(GridBordersInfoStore.RightProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the right border has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasRight
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridBordersInfoStore.RightProperty);
            }
        }
        #endregion
    }
}
