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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;

using Syncfusion.Documentation;
using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Implements the data store for the <see cref="ChartBordersInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    /// <internalonly/>
    [Serializable, StaticDataField("sd"),
 DocumentationExclude()]
    public class ChartBordersInfoStore : StyleInfoStore
    {
        #region Constants
        private static StaticData sd = new StaticData(typeof(ChartBordersInfoStore), typeof(ChartBordersInfo), true);
        #endregion

        #region Members

        /// <summary>
        /// The Outer Property.
        /// </summary>
        /// <internalonly/>
        public static readonly StyleInfoProperty OuterProperty = sd.CreateStyleInfoProperty(typeof(ChartBorder), "Inner");

        /// <summary>
        /// The Inner Property.
        /// </summary>
        /// <internalonly/>
        public static readonly StyleInfoProperty InnerProperty = sd.CreateStyleInfoProperty(typeof(ChartBorder), "Outer");

        /// <summary>
        /// Static data must be declared static in derived classes (this avoids collisions
        /// when StyleInfoStore is used in the same project for different types of style
        /// classes).
        /// </summary>
        /// <value>The Static Data Store.</value>
        /// <override/>
        protected override StaticData StaticDataStore
        {
            get
            {
                return sd;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartBordersInfoStore"/> class.
        /// </summary>
        /// <internalonly/>
        public ChartBordersInfoStore()
        {
        }


        /// <summary>
        /// Initializes the <see cref="ChartBordersInfoStore"/> class.
        /// </summary>
        static ChartBordersInfoStore()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartBordersInfoStore"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        private ChartBordersInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Creates an exact copy of the current object.
        /// </summary>
        /// <returns>
        /// A <see cref="T:Syncfusion.Styles.StyleInfoStore"/> with same data as the current object.
        /// </returns>
        /// <internalonly/>
        public override object Clone()
        {
            StyleInfoStore target = new ChartBordersInfoStore();
            CopyTo(target);
            return target;
        }
        #endregion
    }

    /// <summary>
    /// Provides a <see cref="StyleInfoSubObjectBase"/> object for borders in a symbol. The inner / outer border of
    /// the symbol can be configured individually with a <see cref="ChartBorder"/> value. Borders that
    /// have not been initialized will inherit default values from a base style.
    /// </summary>
    public class ChartBordersInfo : ChartSubStyleInfoBase
    {
        #region Constants
        // Static fields.
        private static ChartBordersInfo defaultBorders;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartBordersInfo"/> class.
        /// </summary>
        [DebuggerStepThrough()]
        public ChartBordersInfo()
            : base(new ChartBordersInfoStore())
        {
        }
        ~ChartBordersInfo()
        {
            if (defaultBorders != null)
                defaultBorders = null;
        }
        /// <summary>
        /// Initalizes a new <see cref="ChartBordersInfo"/>  instance and associates it with an existing <see cref="StyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="StyleInfoSubObjectIdentity"/> that holds the identity for this <see cref="ChartBordersInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public ChartBordersInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new ChartBordersInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="ChartBordersInfo"/> object and associates it with an existing <see cref="StyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="StyleInfoSubObjectIdentity"/> that holds the identity for this <see cref="ChartBordersInfo"/>.
        /// <param name="store">A <see cref="ChartBordersInfoStore"/> that holds data for this <see cref="ChartBordersInfo"/>.
        /// All changes made in this style object will be saved in the <see cref="ChartBordersInfoStore"/> object.</param>
        /// </param>
        [DebuggerStepThrough()]
        public ChartBordersInfo(StyleInfoSubObjectIdentity identity, ChartBordersInfoStore store)
            : base(identity, store)
        {
        }
        #endregion


        /// <override/>
        [DebuggerStepThrough()]
        public override IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip)
        {
            return new ChartBordersInfo(newOwner.CreateSubObjectIdentity(sip), (ChartBordersInfoStore)Store.Clone());
        }

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new ChartBordersInfo(identity, store as ChartBordersInfoStore);
            }

            return new ChartBordersInfo(identity);
        }

        // Default.
        /// <summary>
        /// Returns a default <see cref="ChartBordersInfo"/> to be used with a default style.
        /// </summary>
        /// <remarks>
        /// The <see cref="ChartStyleInfo.Default"/> of the <see cref="ChartStyleInfo"/> class
        /// will return the default border info that this method generates through it's
        /// overridden version of <see cref="GetDefaultStyle"/>.
        ///  </remarks>
        public static ChartBordersInfo Default
        {
            get
            {

                if (defaultBorders == null)
                {
                    defaultBorders = new ChartBordersInfo();
                    defaultBorders.Inner = new ChartBorder(ChartBorderStyle.Standard, SystemColors.WindowFrame, ChartBorderWeight.Thin);
                    defaultBorders.Outer = new ChartBorder(ChartBorderStyle.Standard, SystemColors.WindowFrame, ChartBorderWeight.Thin);
                }

                return defaultBorders;
            }
        }

        /// <summary>
        /// Overridden. Returns a ChartBordersInfo object with default values.
        /// </summary>
        /// <returns>A <see cref="ChartBordersInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }

        #region Properties
        /// <summary>
        /// Sets the inner and outer border with one command.
        /// </summary>
        /// <value>All.</value>
        [Browsable(false)]
        public ChartBorder All
        {
            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartBordersInfoStore.InnerProperty, value);
                SetValue(ChartBordersInfoStore.OuterProperty, value);
            }
        }

        /// <summary>
        /// Resets the inner and outer border with one command.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetAll()
        {
            ResetValue(ChartBordersInfoStore.InnerProperty);
            ResetValue(ChartBordersInfoStore.OuterProperty);
        }
        //// Properties

        #region Inner

        /// <summary>
        /// Gets or sets the inner border.
        /// </summary>
        /// <value>The inner.</value>
        [
        Browsable(true),
        Category("Appearance"),
        Description("The inner border")
        ]

        public ChartBorder Inner
        {
            [DebuggerStepThrough()]
            get
            {
                return (ChartBorder)GetValue(ChartBordersInfoStore.InnerProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartBordersInfoStore.InnerProperty, value);
            }
        }

        /// <summary>
        /// Resets the inner border.
        /// </summary>
        [
        DebuggerStepThrough()
        ]

        public void ResetInner()
        {
            ResetValue(ChartBordersInfoStore.InnerProperty);
        }

        /// <summary>
        /// Should the serialize inner.
        /// </summary>
        /// <returns>Returns bool.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeInner()
        {
            return HasValue(ChartBordersInfoStore.InnerProperty);
        }

        /// <summary>
        /// Indicates whether the inner border has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasInner
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartBordersInfoStore.InnerProperty);
            }
        }

        #endregion

        #region Outer

        /// <summary>
        /// Gets or sets the outer border.
        /// </summary>
        [Browsable(true),
        Category("Appearance"),
        Description("The outer border")]

        public ChartBorder Outer
        {
            [DebuggerStepThrough()]
            get
            {
                return (ChartBorder)GetValue(ChartBordersInfoStore.OuterProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartBordersInfoStore.OuterProperty, value);
            }
        }

        /// <summary>
        /// Resets the outer border.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetOuter()
        {
            ResetValue(ChartBordersInfoStore.OuterProperty);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeOuter()
        {
            return HasValue(ChartBordersInfoStore.OuterProperty);
        }
        #endregion

        #endregion
    }
}