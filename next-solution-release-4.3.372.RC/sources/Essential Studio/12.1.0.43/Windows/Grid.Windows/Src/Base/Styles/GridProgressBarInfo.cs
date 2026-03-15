//-------------------------------------------------------------------------------------------------
// <copyright file="GridProgressBarInfo.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Security.Permissions;

using System.Diagnostics;
using System.Globalization;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the data store for the <see cref="GridProgressBarInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    [Serializable,
    StaticDataField("sd")]
    public class GridProgressBarInfoStore : StyleInfoStore
    {
        static StaticData sd = new StaticData(typeof(GridProgressBarInfoStore), typeof(GridProgressBarInfo), true);

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.ProgressValue"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ProgressValueProperty = sd.CreateStyleInfoProperty(typeof(int), "ProgressValue");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.Minimum"/> property.
        /// </summary>
        public readonly static StyleInfoProperty MinimumProperty = sd.CreateStyleInfoProperty(typeof(int), "Minimum");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.Maximum"/> property.
        /// </summary>
        public readonly static StyleInfoProperty MaximumProperty = sd.CreateStyleInfoProperty(typeof(int), "Maximum");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.Step"/> property.
        /// </summary>
        public readonly static StyleInfoProperty StepProperty = sd.CreateStyleInfoProperty(typeof(int), "Step");

        ////        ///// <summary>
        ////        ///// Provides information about the <see cref="GridProgressBarInfo.WaitingGradientWidth"/> property.
        ////        ///// </summary>
        ////        public readonly static StyleInfoProperty WaitingGradientWidthProperty = sd.CreateStyleInfoProperty(typeof(int), "WaitingGradientWidth");
        ////
        ////        ///// <summary>
        ////        ///// Provides information about the <see cref="GridProgressBarInfo.WaitingGradientEnabled"/> property.
        ////        ///// </summary>
        ////        public readonly static StyleInfoProperty WaitingGradientEnabledProperty = sd.CreateStyleInfoProperty(typeof(bool), "WaitingGradientEnabled");
        ////
        ////        ///// <summary>
        ////        ///// Provides information about the <see cref="GridProgressBarInfo.WaitingGradientInterval"/> property.
        ////        ///// </summary>
        ////        public readonly static StyleInfoProperty WaitingGradientIntervalProperty = sd.CreateStyleInfoProperty(typeof(int), "WaitingGradientInterval");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.ForeSegments"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ForeSegmentsProperty = sd.CreateStyleInfoProperty(typeof(bool), "ForeSegments");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.StretchMultGrad"/> property.
        /// </summary>
        public readonly static StyleInfoProperty StretchMultGradProperty = sd.CreateStyleInfoProperty(typeof(bool), "StretchMultGrad");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.MultipleColors"/> property.
        /// </summary>
        public readonly static StyleInfoProperty MultipleColorsProperty = sd.CreateStyleInfoProperty(typeof(Color[]), "MultipleColors");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.GradientStartColor"/> property.
        /// </summary>
        public readonly static StyleInfoProperty GradientStartColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "GradientStartColor");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.GradientEndColor"/> property.
        /// </summary>
        public readonly static StyleInfoProperty GradientEndColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "GradientEndColor");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.TubeStartColor"/> property.
        /// </summary>
        public readonly static StyleInfoProperty TubeStartColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "TubeStartColor");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.TubeEndColor"/> property.
        /// </summary>
        public readonly static StyleInfoProperty TubeEndColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "TubeEndColor");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.BackSegments"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BackSegmentsProperty = sd.CreateStyleInfoProperty(typeof(bool), "BackSegments");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.BackMultipleColors"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BackMultipleColorsProperty = sd.CreateStyleInfoProperty(typeof(Color[]), "BackMultipleColors");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.BackGradientStartColor"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BackGradientStartColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "BackGradientStartColor");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.BackGradientEndColor"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BackGradientEndColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "BackGradientEndColor");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.BackTubeStartColor"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BackTubeStartColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "BackTubeStartColor");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.BackTubeEndColor"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BackTubeEndColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "BackTubeEndColor");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.StretchImage"/> property.
        /// </summary>
        public readonly static StyleInfoProperty StretchImageProperty = sd.CreateStyleInfoProperty(typeof(bool), "StretchImage");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.ForegroundImage"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ForegroundImageProperty = sd.CreateStyleInfoProperty(typeof(Image), "ForegroundImage");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.BackgroundImage"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BackgroundImageProperty = sd.CreateStyleInfoProperty(typeof(Image), "BackgroundImage");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.SegmentWidth"/> property.
        /// </summary>
        public readonly static StyleInfoProperty SegmentWidthProperty = sd.CreateStyleInfoProperty(typeof(int), "SegmentWidth");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.FontColor"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FontColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "FontColor");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.ForeColor"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ForeColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "ForeColor");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.TextVisible"/> property.
        /// </summary>
        public readonly static StyleInfoProperty TextVisibleProperty = sd.CreateStyleInfoProperty(typeof(bool), "TextVisible");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.TextStyle"/> property.
        /// </summary>
        public readonly static StyleInfoProperty TextStyleProperty = sd.CreateStyleInfoProperty(typeof(ProgressBarTextStyles), "TextStyle");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.ProgressOrientation"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ProgressOrientationProperty = sd.CreateStyleInfoProperty(typeof(Orientation), "ProgressOrientation");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.TextShadow"/> property.
        /// </summary>
        public readonly static StyleInfoProperty TextShadowProperty = sd.CreateStyleInfoProperty(typeof(bool), "TextShadow");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.ProgressStyle"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ProgressStyleProperty = sd.CreateStyleInfoProperty(typeof(ProgressBarStyles), "ProgressStyle");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.ProgressFallbackStyle"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ProgressFallbackStyleProperty = sd.CreateStyleInfoProperty(typeof(ProgressBarStyles), "ProgressFallbackStyle");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.BackgroundStyle"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BackgroundStyleProperty = sd.CreateStyleInfoProperty(typeof(ProgressBarBackgroundStyles), "BackgroundStyle");

        /// <summary>
        /// Provides information about the <see cref="GridProgressBarInfo.BackgroundFallbackStyle"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BackgroundFallbackStyleProperty = sd.CreateStyleInfoProperty(typeof(ProgressBarBackgroundStyles), "BackgroundFallbackStyle");

        ////        /// <summary>
        ////        /// Provides information about the <see cref="GridProgressBarInfo.ProgressOrientation"/> property.
        ////        /// </summary>
        ////        public readonly static StyleInfoProperty ProgressOrientationProperty = sd.CreateStyleInfoProperty(typeof(Orientation), "ProgressOrientation");
        
        ////public Border3DStyle Border3DStyle
        ////public BorderStyle BorderStyle
        ////public ButtonBorderStyle BorderSingle
        ////public Color BorderColor
        
        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        /// <overload>
        /// Initializes a new <see cref="GridProgressBarInfoStore"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridProgressBarInfoStore"/>.
        /// </summary>
        public GridProgressBarInfoStore()
        {
        }

        static GridProgressBarInfoStore()
        {
            ////NumberFormatInfoObjectProperty.SerializeXmlBehavior = SerializeXmlBehavior.Skip;
        }

        /// <summary>
        /// Initializes a new <see cref="GridProgressBarInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        private GridProgressBarInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
        }

        //// Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        //// Calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>Creates an exact copy of the current object.</summary>
        /// <returns>A duplicate of the current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridProgressBarInfoStore();
            CopyTo(target);
            return target;
        }
    }

    /// <summary>
    /// Provides a <see cref="StyleInfoSubObjectBase"/> object for progressbar properties in a cell. 
    /// <para/>Each property in this sub object can be configured individually. Properties that
    /// have not been initialized will inherit default values from a base style.
    /// </summary>
    public class GridProgressBarInfo : StyleInfoSubObjectBase
    {
        //// Static Fields
        private static GridProgressBarInfo defaultProgressBarInfo;

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridProgressBarInfo(identity, store as GridProgressBarInfoStore);
            }

            return new GridProgressBarInfo(identity);
        }
        
        // Constructors.

        /// <summary>
        /// Initializes a new empty <see cref="GridProgressBarInfo"/> object.
        /// </summary>
        [DebuggerStepThrough()]
        public GridProgressBarInfo()
            : base(new GridProgressBarInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridProgressBarInfo"/>  object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridProgressBarInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridProgressBarInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridProgressBarInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridProgressBarInfo"/>  object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridProgressBarInfo"/>.</param>
        /// <param name="store">A <see cref="GridProgressBarInfoStore"/> that holds data for this <see cref="GridProgressBarInfo"/>.
        /// All changes in this style object will saved in the <see cref="GridProgressBarInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public GridProgressBarInfo(StyleInfoSubObjectIdentity identity, GridProgressBarInfoStore store)
            : base(identity, store)
        {
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // Calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>Creates an exact copy of the current object.</summary>
        /// <param name="newOwner">The new owner style object for the copied object.</param>
        /// <param name="sip">An identifier for this object.</param>
        /// <returns>Copied object.</returns>
        [DebuggerStepThrough()]
        public override IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip)
        {
            return new GridProgressBarInfo(newOwner.CreateSubObjectIdentity(sip), (GridProgressBarInfoStore)Store.Clone());
        }

        // Default.

        /// <summary>
        /// Gets a default <see cref="GridProgressBarInfo"/> to be used with a default style.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridStyleInfo.Default"/> of the <see cref="GridStyleInfo"/> class
        /// will return the validation info that this method generates through its
        /// overriden version of <see cref="GetDefaultStyle"/>.
        /// <para/>
        /// Default settings are: <para/>
        /// <list type="table">
        /// <listheader><term>Property</term><description>Value</description></listheader>
        /// <item><term><see cref="ProgressValue"/></term><description>50</description></item>
        /// <item><term><see cref="Minimum"/></term><description>0</description></item>
        /// <item><term><see cref="Maximum"/></term><description>100</description></item>
        /// <item><term><see cref="Step"/></term><description>10</description></item>
        /// <item><term><see cref="ForeSegments"/></term><description>True</description></item>
        /// <item><term><see cref="StretchMultGrad"/></term><description>True</description></item>
        /// <item><term><see cref="MultipleColors"/></term><description>False</description></item>
        /// <item><term><see cref="GradientStartColor"/></term><description>Color.Red</description></item>
        /// <item><term><see cref="GradientEndColor"/></term><description>Color.Lime</description></item>
        /// <item><term><see cref="TubeStartColor"/></term><description>Color.Red</description></item>
        /// <item><term><see cref="TubeEndColor"/></term><description>Color.Black</description></item>
        /// <item><term><see cref="BackSegments"/></term><description>False</description></item>
        /// <item><term><see cref="BackMultipleColors"/></term><description>False</description></item>
        /// <item><term><see cref="BackGradientStartColor"/></term><description>Color.LightGray</description></item>
        /// <item><term><see cref="BackGradientEndColor"/></term><description>Color.White</description></item>
        /// <item><term><see cref="BackTubeStartColor"/></term><description>Color.LightGray</description></item>
        /// <item><term><see cref="BackTubeEndColor"/></term><description>Color.White</description></item>
        /// <item><term><see cref="StretchImage"/></term><description>True</description></item>
        /// <item><term><see cref="BackgroundImage"/></term><description>NULL</description></item>
        /// <item><term><see cref="SegmentWidth"/></term><description>12</description></item>
        /// <item><term><see cref="FontColor"/></term><description>Color.White</description></item>
        /// <item><term><see cref="ForeColor"/></term><description>Color.DarkCyan</description></item>
        /// <item><term><see cref="TextVisible"/></term><description>True</description></item>
        /// <item><term><see cref="TextStyle"/></term><description>ProgressBarTextStyles.Percentage</description></item>
        /// <item><term><see cref="TextShadow"/></term><description>True</description></item>
        /// <item><term><see cref="ProgressStyle"/></term><description>ProgressBarStyles.Constant</description></item>
        /// <item><term><see cref="ProgressFallbackStyle"/></term><description>ProgressBarStyles.Constant</description></item>
        /// <item><term><see cref="BackgroundStyle"/></term><description>ProgressBarBackgroundStyles.None</description></item>
        /// <item><term><see cref="BackgroundFallbackStyle"/></term><description>ProgressBarBackgroundStyles.None</description></item>
        /// </list>
        /// </remarks>
        public static GridProgressBarInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultProgressBarInfo == null)
                {
                    defaultProgressBarInfo = new GridProgressBarInfo();

                    //// Retrieve default values from an empty masked progress bar control.
                    ////                    ProgressBarAdv eb = new ProgressBarAdv();
                    ////                    defaultProgressBarInfo.ProgressValue = eb.Value;
                    ////                    defaultProgressBarInfo.Minimum = eb.Minimum;
                    ////                    defaultProgressBarInfo.Maximum = eb.Maximum;
                    ////                    defaultProgressBarInfo.Step = eb.Step;
                    ////                    ////defaultProgressBarInfo.WaitingGradientWidth = eb.WaitingGradientWidth;
                    ////                    ////defaultProgressBarInfo.WaitingGradientEnabled = eb.WaitingGradientEnabled;
                    ////                    ////defaultProgressBarInfo.WaitingGradientInterval = eb.WaitingGradientInterval;
                    ////                    defaultProgressBarInfo.ForeSegments = eb.ForeSegments;
                    ////                    defaultProgressBarInfo.StretchMultGrad = eb.StretchMultGrad;
                    ////                    defaultProgressBarInfo.MultipleColors = eb.MultipleColors;
                    ////                    defaultProgressBarInfo.GradientStartColor = eb.GradientStartColor;
                    ////                    defaultProgressBarInfo.GradientEndColor = eb.GradientEndColor;
                    ////                    defaultProgressBarInfo.TubeStartColor = eb.TubeStartColor;
                    ////                    defaultProgressBarInfo.TubeEndColor = eb.TubeEndColor;
                    ////                    defaultProgressBarInfo.BackSegments = eb.BackSegments;
                    ////                    defaultProgressBarInfo.BackMultipleColors = eb.BackMultipleColors;
                    ////                    defaultProgressBarInfo.BackGradientStartColor = eb.BackGradientStartColor;
                    ////                    defaultProgressBarInfo.BackGradientEndColor = eb.BackGradientEndColor;
                    ////                    defaultProgressBarInfo.BackTubeStartColor = eb.BackTubeStartColor;
                    ////                    defaultProgressBarInfo.BackTubeEndColor = eb.BackTubeEndColor;
                    ////                    defaultProgressBarInfo.StretchImage = eb.StretchImage;
                    ////                    defaultProgressBarInfo.ForegroundImage = eb.ForegroundImage;
                    ////                    defaultProgressBarInfo.BackgroundImage = eb.BackgroundImage;
                    ////                    defaultProgressBarInfo.SegmentWidth = eb.SegmentWidth;
                    ////                    defaultProgressBarInfo.FontColor = eb.FontColor;
                    ////                    defaultProgressBarInfo.ForeColor = eb.ForeColor;
                    ////                    defaultProgressBarInfo.TextVisible = eb.TextVisible;
                    ////                    defaultProgressBarInfo.TextStyle = eb.TextStyle;
                    ////                    ////                    defaultProgressBarInfo.ProgressOrientation = eb.ProgressOrientation;
                    ////                    defaultProgressBarInfo.TextShadow = eb.TextShadow;
                    ////                    defaultProgressBarInfo.ProgressStyle = eb.ProgressStyle;
                    ////                    defaultProgressBarInfo.ProgressFallbackStyle = eb.ProgressFallbackStyle;
                    ////                    defaultProgressBarInfo.BackgroundStyle = eb.BackgroundStyle;
                    ////                    defaultProgressBarInfo.BackgroundFallbackStyle = eb.BackgroundFallbackStyle;
                    ////                    //                    defaultProgressBarInfo.ProgressOrientation = eb.ProgressOrientation;

                    defaultProgressBarInfo.ProgressValue = 50;
                    defaultProgressBarInfo.Minimum = 0;
                    defaultProgressBarInfo.Maximum = 100;
                    defaultProgressBarInfo.Step = 10;
                    defaultProgressBarInfo.ForeSegments = true;
                    defaultProgressBarInfo.StretchMultGrad = true;
                    defaultProgressBarInfo.MultipleColors = new System.Drawing.Color[0];
                    defaultProgressBarInfo.GradientStartColor = Color.Red;
                    defaultProgressBarInfo.GradientEndColor = Color.Lime;
                    defaultProgressBarInfo.TubeStartColor = Color.Red;
                    defaultProgressBarInfo.TubeEndColor = Color.Black;
                    defaultProgressBarInfo.BackSegments = false;
                    defaultProgressBarInfo.BackMultipleColors = new System.Drawing.Color[0];
                    defaultProgressBarInfo.BackGradientStartColor = Color.LightGray;
                    defaultProgressBarInfo.BackGradientEndColor = Color.White;
                    defaultProgressBarInfo.BackTubeStartColor = Color.LightGray;
                    defaultProgressBarInfo.BackTubeEndColor = Color.White;
                    defaultProgressBarInfo.StretchImage = true;
                    defaultProgressBarInfo.ForegroundImage = null;
                    defaultProgressBarInfo.BackgroundImage = null;
                    defaultProgressBarInfo.SegmentWidth = 12;
                    defaultProgressBarInfo.FontColor = Color.White;
                    defaultProgressBarInfo.ForeColor = Color.DarkCyan;
                    defaultProgressBarInfo.TextVisible = true;
                    defaultProgressBarInfo.TextStyle = ProgressBarTextStyles.Percentage;
                    defaultProgressBarInfo.ProgressOrientation = Orientation.Horizontal;
                    defaultProgressBarInfo.TextShadow = true;
                    defaultProgressBarInfo.ProgressStyle = ProgressBarStyles.Constant;
                    defaultProgressBarInfo.ProgressFallbackStyle = ProgressBarStyles.Constant;
                    defaultProgressBarInfo.BackgroundStyle = ProgressBarBackgroundStyles.None;
                    defaultProgressBarInfo.BackgroundFallbackStyle = ProgressBarBackgroundStyles.None;
                }

                return defaultProgressBarInfo;
            }
        }

        /// <summary>
        /// Returns <see cref="GridProgressBarInfo.Default"/>.
        /// </summary>
        /// <returns>A <see cref="GridProgressBarInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }

        // Properties.
        #region ProgressValue
        /// <summary>
        /// Gets or sets the value between Minimum and Maximum.
        /// </summary>
        /// <remarks>
        /// This value represents the progress state of the ProgessBar. For default it is set to 50, minimum=0, and maximum=100 ( 50% ).
        /// </remarks>
        [Category("Behavior")]
        ////        [DefaultValue(50)]
        [Description("The current value between the minimum and maximum values")]
        public int ProgressValue
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridProgressBarInfoStore.ProgressValueProperty);
            }
           
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.ProgressValueProperty, value);
            }
        }
      
        /// <summary>
        /// Resets the <see cref="ProgressValue"/> property.
        /// </summary>       
        [DebuggerStepThrough()]
        public void ResetProgressValue()
        {
            ResetValue(GridProgressBarInfoStore.ProgressValueProperty);
        }
      
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeProgressValue()
        {
            return HasValue(GridProgressBarInfoStore.ProgressValueProperty);
        }
       
        /// <summary>
        /// Gets a value indicating whether <see cref="ProgressValue"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasProgressValue
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.ProgressValueProperty);
            }
        }
        #endregion
        #region Minimum
        /// <summary>
        /// Gets or sets the lower boundary for the value.
        /// </summary>
        /// <remarks>
        /// By default its value is 0 which means that the Value of the ProgressBar can not take values lower than 0.
        /// </remarks>
        [Category("Behavior")]
        ////        [DefaultValue(0)]
        [Description("The lower bound of the range of the ProgressBar")]
        public int Minimum
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridProgressBarInfoStore.MinimumProperty);
            }
         
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.MinimumProperty, value);
            }
        }
       
        /// <summary>
        /// Resets the <see cref="Minimum"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetMinimum()
        {
            ResetValue(GridProgressBarInfoStore.MinimumProperty);
        }
       
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeMinimum()
        {
            return HasValue(GridProgressBarInfoStore.MinimumProperty);
        }
      
        /// <summary>
        /// Gets a value indicating whether <see cref="Minimum"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasMinimum
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.MinimumProperty);
            }
        }
        #endregion
        #region Maximum
        /// <summary>
        /// Gets or sets the upper boundary for the value.
        /// </summary>
        /// <remarks>
        /// By default its value is 100 which means that the Value of the ProgressBar can not take values higher than 100.
        /// </remarks>
        [Category("Behavior")]
        ////        [DefaultValue(100)]
        [Description("The higher bound of the range of the ProgressBar")]
        public int Maximum
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridProgressBarInfoStore.MaximumProperty);
            }
           
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.MaximumProperty, value);
            }
        }
       
        /// <summary>
        /// Resets the <see cref="Maximum"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetMaximum()
        {
            ResetValue(GridProgressBarInfoStore.MaximumProperty);
        }
       
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeMaximum()
        {
            return HasValue(GridProgressBarInfoStore.MaximumProperty);
        }
       
        /// <summary>
        /// Gets a value indicating whether <see cref="Maximum"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasMaximum
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.MaximumProperty);
            }
        }
       
        #endregion
        #region Step
        /// <summary>
        /// Gets or sets the value to increment when Increment() and Decrement() methods.
        /// </summary>
        /// <remarks>
        /// By default its value is 10 which means that when Increment() is called the Value of the ProgressBar is incremented by 10.
        /// </remarks>
        [Category("Behavior")]
        ////        [DefaultValue(10)]
        [Description("The amount to increment the value of the ProgressBar when Increment() is called")]
        public int Step
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridProgressBarInfoStore.StepProperty);
            }
           
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.StepProperty, value);
            }
        }
       
        /// <summary>
        /// Resets the <see cref="Step"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetStep()
        {
            ResetValue(GridProgressBarInfoStore.StepProperty);
        }
       
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeStep()
        {
            return HasValue(GridProgressBarInfoStore.StepProperty);
        }
       
        /// <summary>
        /// Gets a value indicating whether <see cref="Step"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasStep
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.StepProperty);
            }
        }
        #endregion
        ////        #region WaitingGradientWidth
        ////        ///// <summary>
        ////        ///// Determines the width of the waiting gradient.
        ////        ///// </summary>
        ////        [Description("Determines the width of the waiting gradient.")]
        ////        [Category("Foreground Gradient")]
        ////        public int WaitingGradientWidth
        ////        {
        ////            [DebuggerStepThrough()] 
        ////            get 
        ////            {
        ////                return (int) GetValue(GridProgressBarInfoStore.WaitingGradientWidthProperty);
        ////            }
        ////            [DebuggerStepThrough()] 
        ////            set 
        ////            {
        ////                SetValue(GridProgressBarInfoStore.WaitingGradientWidthProperty, value);
        ////            }
        ////        }
        ////        ///// <summary>
        ////        ///// Resets the <see cref="WaitingGradientWidth"/> property.
        ////        ///// </summary>
        ////        [DebuggerStepThrough()] public void ResetWaitingGradientWidth()
        ////        {
        ////            ResetValue(GridProgressBarInfoStore.WaitingGradientWidthProperty);
        ////        }
        ////        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        ////        private bool ShouldSerializeWaitingGradientWidth()
        ////        {
        ////            return HasValue(GridProgressBarInfoStore.WaitingGradientWidthProperty);
        ////        }
        ////        ///// <summary>
        ////        ///// Checks if <see cref="WaitingGradientWidth"/> property is initialized.
        ////        ///// </summary>
        ////        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ////        public bool HasWaitingGradientWidth
        ////        {
        ////            [DebuggerStepThrough()] 
        ////            get
        ////            {
        ////                return HasValue(GridProgressBarInfoStore.WaitingGradientWidthProperty);
        ////            }
        ////        }
        ////        #endregion
        ////        #region WaitingGradientEnabled
        ////        ///// <summary>
        ////        ///// Determines if the waiting gradient is enabled.
        ////        ///// </summary>
        ////        [Description("Determines if the waiting gradient is enabled.")]
        ////        [Category("Foreground Gradient")]
        ////        [DefaultValue(false)]
        ////        public bool WaitingGradientEnabled
        ////        {
        ////            [DebuggerStepThrough()] 
        ////            get 
        ////            {
        ////                return (bool) GetValue(GridProgressBarInfoStore.WaitingGradientEnabledProperty);
        ////            }
        ////            [DebuggerStepThrough()] 
        ////            set 
        ////            {
        ////                SetValue(GridProgressBarInfoStore.WaitingGradientEnabledProperty, value);
        ////            }
        ////        }
        ////        ///// <summary>
        ////        ///// Resets the <see cref="WaitingGradientEnabled"/> property.
        ////        ///// </summary>
        ////        [DebuggerStepThrough()] public void ResetWaitingGradientEnabled()
        ////        {
        ////            ResetValue(GridProgressBarInfoStore.WaitingGradientEnabledProperty);
        ////        }
        ////        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        ////        private bool ShouldSerializeWaitingGradientEnabled()
        ////        {
        ////            return HasValue(GridProgressBarInfoStore.WaitingGradientEnabledProperty);
        ////        }
        ////        ///// <summary>
        ////        ///// Checks if <see cref="WaitingGradientEnabled"/> property is initialized.
        ////        ///// </summary>
        ////        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ////        public bool HasWaitingGradientEnabled
        ////        {
        ////            [DebuggerStepThrough()] 
        ////            get
        ////            {
        ////                return HasValue(GridProgressBarInfoStore.WaitingGradientEnabledProperty);
        ////            }
        ////        }
        ////        #endregion
        ////        #region WaitingGradientInterval
        ////        ///// <summary>
        ////        ///// Determines the interval of the waiting gradient.
        ////        ///// </summary>
        ////        [Description("Determines the interval of the waiting gradient.")]
        ////        [Category("Foreground Gradient")]
        ////        [DefaultValue(10)]
        ////        public int WaitingGradientInterval
        ////        {
        ////            [DebuggerStepThrough()] 
        ////            get 
        ////            {
        ////                return (int) GetValue(GridProgressBarInfoStore.WaitingGradientIntervalProperty);
        ////            }
        ////            [DebuggerStepThrough()] 
        ////            set 
        ////            {
        ////                SetValue(GridProgressBarInfoStore.WaitingGradientIntervalProperty, value);
        ////            }
        ////        }
        ////        ///// <summary>
        ////        ///// Resets the <see cref="WaitingGradientInterval"/> property.
        ////        ///// </summary>
        ////        [DebuggerStepThrough()] public void ResetWaitingGradientInterval()
        ////        {
        ////            ResetValue(GridProgressBarInfoStore.WaitingGradientIntervalProperty);
        ////        }
        ////        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        ////        private bool ShouldSerializeWaitingGradientInterval()
        ////        {
        ////            return HasValue(GridProgressBarInfoStore.WaitingGradientIntervalProperty);
        ////        }
        ////        ///// <summary>
        ////        ///// Checks if <see cref="WaitingGradientInterval"/> property is initialized.
        ////        ///// </summary>
        ////        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ////        public bool HasWaitingGradientInterval
        ////        {
        ////            [DebuggerStepThrough()] 
        ////            get
        ////            {
        ////                return HasValue(GridProgressBarInfoStore.WaitingGradientIntervalProperty);
        ////            }
        ////        }
        ////        #endregion

        #region ForeSegments
        /// <summary>
        /// Gets or sets a value indicating whether the foreground is segmented.
        /// </summary>
        /// <remarks>
        /// By default its value is True which means that the foreground will be drawn segmented.
        /// </remarks>
        [Category("Foreground Gradient")]
        ////        [DefaultValue(true)]
        [Description("Determines if the foreground is segmented")]
        public bool ForeSegments
        {
            [DebuggerStepThrough()]
            get
            {
                return (bool)GetValue(GridProgressBarInfoStore.ForeSegmentsProperty);
            }
          
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.ForeSegmentsProperty, value);
            }
        }
       
        /// <summary>
        /// Resets the <see cref="ForeSegments"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetForeSegments()
        {
            ResetValue(GridProgressBarInfoStore.ForeSegmentsProperty);
        }
      
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeForeSegments()
        {
            return HasValue(GridProgressBarInfoStore.ForeSegmentsProperty);
        }
       
        /// <summary>
        /// Gets a value indicating whether <see cref="ForeSegments"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasForeSegments
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.ForeSegmentsProperty);
            }
        }
        #endregion
      
        #region StretchMultGrad
        /// <summary>
        /// Gets or sets a value indicating whether the multiple gradient is compressed if the value is smaller than maximum.
        /// </summary>
        /// <remarks>
        /// By default its value is True which means that if the Value is less than maximum, the multiple gradient is compressed.
        /// </remarks>
        [Category("Foreground Gradient")]
        ////        [DefaultValue(true)]
        [Description("Determines if the multiple gradient will be stretched.")]
        public bool StretchMultGrad
        {
            [DebuggerStepThrough()]
            get
            {
                return (bool)GetValue(GridProgressBarInfoStore.StretchMultGradProperty);
            }
           
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.StretchMultGradProperty, value);
            }
        }
       
        /// <summary>
        /// Resets the <see cref="StretchMultGrad"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetStretchMultGrad()
        {
            ResetValue(GridProgressBarInfoStore.StretchMultGradProperty);
        }
       
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeStretchMultGrad()
        {
            return HasValue(GridProgressBarInfoStore.StretchMultGradProperty);
        }
       
        /// <summary>
        /// Gets a value indicating whether <see cref="StretchMultGrad"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasStretchMultGrad
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.StretchMultGradProperty);
            }
        }
        
        #endregion
        #region MultipleColors
        /// <summary>
        /// Gets or sets the colors of the foreground multiple gradient when ForegroundStyle is Multiple Gradient.
        /// </summary>
        /// <remarks>
        ///    By default its value is an empty Color array. You can add Colors to the multiple gradient by modifying this property.
        /// </remarks>
        [Category("Foreground Gradient")]
        [Description("The array of colors used in the multiple gradient of the foreground.")]
        public Color[] MultipleColors
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color[])GetValue(GridProgressBarInfoStore.MultipleColorsProperty);
            }
           
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.MultipleColorsProperty, value);
            }
        }
        
        /// <summary>
        /// Resets the <see cref="MultipleColors"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetMultipleColors()
        {
            ResetValue(GridProgressBarInfoStore.MultipleColorsProperty);
        }
        
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeMultipleColors()
        {
            return HasValue(GridProgressBarInfoStore.MultipleColorsProperty);
        }
       
        /// <summary>
        /// Gets a value indicating whether <see cref="MultipleColors"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasMultipleColors
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.MultipleColorsProperty);
            }
        }
       
        #endregion
        #region GradientStartColor
        /// <summary>
        /// Gets or sets the start color of the foreground gradient when ForegroundStyle is Gradient.
        /// </summary>
        [Category("Foreground Gradient")]
        [Description("The start color of the dual gradient of the foreground.")]
        public Color GradientStartColor
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(GridProgressBarInfoStore.GradientStartColorProperty);
            }
           
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.GradientStartColorProperty, value);
            }
        }
       
        /// <summary>
        /// Resets the <see cref="GradientStartColor"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetGradientStartColor()
        {
            ResetValue(GridProgressBarInfoStore.GradientStartColorProperty);
        }
       
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeGradientStartColor()
        {
            return HasValue(GridProgressBarInfoStore.GradientStartColorProperty);
        }
      
        /// <summary>
        /// Gets a value indicating whether <see cref="GradientStartColor"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasGradientStartColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.GradientStartColorProperty);
            }
        }
        
        #endregion
        #region GradientEndColor
        /// <summary>
        /// Gets or sets the end color of the foreground gradient when ForegroundStyle is Gradient.
        /// </summary>
        [Category("Foreground Gradient")]
        [Description("The end color of the dual gradient of the foreground.")]
        public Color GradientEndColor
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(GridProgressBarInfoStore.GradientEndColorProperty);
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.GradientEndColorProperty, value);
            }
        }
       
        /// <summary>
        /// Resets the <see cref="GradientEndColor"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetGradientEndColor()
        {
            ResetValue(GridProgressBarInfoStore.GradientEndColorProperty);
        }
       
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeGradientEndColor()
        {
            return HasValue(GridProgressBarInfoStore.GradientEndColorProperty);
        }
        
        /// <summary>
        /// Gets a value indicating whether <see cref="GradientEndColor"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasGradientEndColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.GradientEndColorProperty);
            }
        }
        #endregion
        #region TubeStartColor
        /// <summary>
        /// Gets or sets the start color of the foreground tube when ForegroundStyle is Tube.
        /// </summary>
        [Category("Foreground Gradient")]
        [Description("The middle color of the tube of the foreground.")]
        public Color TubeStartColor
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(GridProgressBarInfoStore.TubeStartColorProperty);
            }
           
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.TubeStartColorProperty, value);
            }
        }
        
        /// <summary>
        /// Resets the <see cref="TubeStartColor"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetTubeStartColor()
        {
            ResetValue(GridProgressBarInfoStore.TubeStartColorProperty);
        }
       
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTubeStartColor()
        {
            return HasValue(GridProgressBarInfoStore.TubeStartColorProperty);
        }
      
        /// <summary>
        /// Gets a value indicating whether <see cref="TubeStartColor"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTubeStartColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.TubeStartColorProperty);
            }
        }
        #endregion
        #region TubeEndColor
        /// <summary>
        /// Gets or sets the end color of the foreground tube when ForegroundStyle is Tube.
        /// </summary>
        [Category("Foreground Gradient")]
        [Description("The outer color of the tube of the foreground.")]
        public Color TubeEndColor
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(GridProgressBarInfoStore.TubeEndColorProperty);
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.TubeEndColorProperty, value);
            }
        }
       
        /// <summary>
        /// Resets the <see cref="TubeEndColor"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetTubeEndColor()
        {
            ResetValue(GridProgressBarInfoStore.TubeEndColorProperty);
        }
       
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTubeEndColor()
        {
            return HasValue(GridProgressBarInfoStore.TubeEndColorProperty);
        }
       
        /// <summary>
        /// Gets a value indicating whether <see cref="TubeEndColor"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTubeEndColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.TubeEndColorProperty);
            }
        }
        #endregion
        #region BackSegments
        /// <summary>
        /// Gets or sets a value indicating whether the background is segmented.
        /// </summary>
        /// <remarks>
        /// By default its value is False.
        /// </remarks>
        [Category("Background Gradient")]
        ////        [DefaultValue(true)]
        [Description("Determines if the background is segmented.")]
        public bool BackSegments
        {
            [DebuggerStepThrough()]
            get
            {
                return (bool)GetValue(GridProgressBarInfoStore.BackSegmentsProperty);
            }
           
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.BackSegmentsProperty, value);
            }
        }
       
        /// <summary>
        /// Resets the <see cref="BackSegments"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetBackSegments()
        {
            ResetValue(GridProgressBarInfoStore.BackSegmentsProperty);
        }
       
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBackSegments()
        {
            return HasValue(GridProgressBarInfoStore.BackSegmentsProperty);
        }
       
        /// <summary>
        /// Gets a value indicating whether <see cref="BackSegments"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBackSegments
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.BackSegmentsProperty);
            }
        }
       
        #endregion
        #region BackMultipleColors
        /// <summary>
        /// Gets or sets the colors of the background multiple gradient when BackgroundStyle is Multiple Gradient.
        /// </summary>
        /// <remarks>
        /// By default its value is an empty array of colors.
        /// </remarks>
        [Category("Background Gradient")]
        [Description("The array of colors used to draw the multiple gradient of the background.")]
        public Color[] BackMultipleColors
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color[])GetValue(GridProgressBarInfoStore.BackMultipleColorsProperty);
            }
          
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.BackMultipleColorsProperty, value);
            }
        }
       
        /// <summary>
        /// Resets the <see cref="BackMultipleColors"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetBackMultipleColors()
        {
            ResetValue(GridProgressBarInfoStore.BackMultipleColorsProperty);
        }
       
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBackMultipleColors()
        {
            return HasValue(GridProgressBarInfoStore.BackMultipleColorsProperty);
        }
       
        /// <summary>
        /// Gets a value indicating whether <see cref="BackMultipleColors"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBackMultipleColors
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.BackMultipleColorsProperty);
            }
        }
       
        #endregion
        #region BackGradientStartColor
        /// <summary>
        /// Gets or sets the start color of the background gradient when BackgroundStyle is Gradient or Vertical Gradient.
        /// </summary>
        [Category("Background Gradient")]
        [Description("The start color of the dual gradient of the background.")]
        public Color BackGradientStartColor
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(GridProgressBarInfoStore.BackGradientStartColorProperty);
            }
           
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.BackGradientStartColorProperty, value);
            }
        }
       
        /// <summary>
        /// Resets the <see cref="BackGradientStartColor"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetBackGradientStartColor()
        {
            ResetValue(GridProgressBarInfoStore.BackGradientStartColorProperty);
        }
       
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBackGradientStartColor()
        {
            return HasValue(GridProgressBarInfoStore.BackGradientStartColorProperty);
        }
        
        /// <summary>
        /// Gets a value indicating whether <see cref="BackGradientStartColor"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBackGradientStartColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.BackGradientStartColorProperty);
            }
        }
        #endregion
        
        #region BackGradientEndColor
        /// <summary>
        /// Gets or sets the end color of the background gradient when BackgroundStyle is Gradient or Vertical Gradient.
        /// </summary>
        [Category("Background Gradient")]
        [Description("The end color of the dual gradient of the background.")]
        public Color BackGradientEndColor
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(GridProgressBarInfoStore.BackGradientEndColorProperty);
            }
          
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.BackGradientEndColorProperty, value);
            }
        }
       
        /// <summary>
        /// Resets the <see cref="BackGradientEndColor"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetBackGradientEndColor()
        {
            ResetValue(GridProgressBarInfoStore.BackGradientEndColorProperty);
        }
       
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBackGradientEndColor()
        {
            return HasValue(GridProgressBarInfoStore.BackGradientEndColorProperty);
        }
       
        /// <summary>
        /// Gets a value indicating whether <see cref="BackGradientEndColor"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBackGradientEndColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.BackGradientEndColorProperty);
            }
        }
        #endregion
        #region BackTubeStartColor
        /// <summary>
        /// Gets or sets the start color of the background tube when BackgroundStyle is Tube.
        /// </summary>
        [Category("Background Gradient")]
        [Description("The middle color of the tube of the background.")]
        public Color BackTubeStartColor
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(GridProgressBarInfoStore.BackTubeStartColorProperty);
            }
           
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.BackTubeStartColorProperty, value);
            }
        }
      
        /// <summary>
        /// Resets the <see cref="BackTubeStartColor"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetBackTubeStartColor()
        {
            ResetValue(GridProgressBarInfoStore.BackTubeStartColorProperty);
        }
       
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBackTubeStartColor()
        {
            return HasValue(GridProgressBarInfoStore.BackTubeStartColorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="BackTubeStartColor"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBackTubeStartColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.BackTubeStartColorProperty);
            }
        }

        #endregion
        #region BackTubeEndColor
        /// <summary>
        /// Gets or sets the end color of the background tube when BackgroundStyle is Tube.
        /// </summary>
        [Category("Background Gradient")]
        [Description("The outer color of the tube of the background.")]
        public Color BackTubeEndColor
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(GridProgressBarInfoStore.BackTubeEndColorProperty);
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.BackTubeEndColorProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="BackTubeEndColor"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetBackTubeEndColor()
        {
            ResetValue(GridProgressBarInfoStore.BackTubeEndColorProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBackTubeEndColor()
        {
            return HasValue(GridProgressBarInfoStore.BackTubeEndColorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="BackTubeEndColor"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBackTubeEndColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.BackTubeEndColorProperty);
            }
        }

        #endregion
        #region StretchImage
        /// <summary>
        /// Gets or sets a value indicating whether the foreground image will be stretched.
        /// </summary>
        /// <remarks>
        /// By default its value is True.
        /// </remarks>
        [Category("Appearance")]
        ////        [DefaultValue(true)]
        [Description("Determines if the foreground image will be stretched.")]
        public bool StretchImage
        {
            [DebuggerStepThrough()]
            get
            {
                return (bool)GetValue(GridProgressBarInfoStore.StretchImageProperty);
            }
           
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.StretchImageProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="StretchImage"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetStretchImage()
        {
            ResetValue(GridProgressBarInfoStore.StretchImageProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeStretchImage()
        {
            return HasValue(GridProgressBarInfoStore.StretchImageProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="StretchImage"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasStretchImage
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.StretchImageProperty);
            }
        }

        #endregion
        #region ForegroundImage
        /// <summary>
        /// Gets or sets the image to draw on the foreground when ProgressStyle is Image.
        /// </summary>
        [Category("Appearance")]
        ////        [DefaultValue(true)]
        [Description("The image used to draw the foreground.")]
        public Image ForegroundImage
        {
            [DebuggerStepThrough()]
            get
            {
                return (Image)GetValue(GridProgressBarInfoStore.ForegroundImageProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.ForegroundImageProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="ForegroundImage"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetForegroundImage()
        {
            ResetValue(GridProgressBarInfoStore.ForegroundImageProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeForegroundImage()
        {
            return HasValue(GridProgressBarInfoStore.ForegroundImageProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="ForegroundImage"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasForegroundImage
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.ForegroundImageProperty);
            }
        }

        #endregion
        #region BackgroundImage
        /// <summary>
        /// Gets or sets the image to draw on the foreground when ProgressStyle is Image.
        /// </summary>
        [Category("Appearance")]
        ////        [DefaultValue(true)]
        [Description("The image used to draw the foreground.")]
        public Image BackgroundImage
        {
            [DebuggerStepThrough()]
            get
            {
                return (Image)GetValue(GridProgressBarInfoStore.BackgroundImageProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.BackgroundImageProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="BackgroundImage"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetBackgroundImage()
        {
            ResetValue(GridProgressBarInfoStore.BackgroundImageProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBackgroundImage()
        {
            return HasValue(GridProgressBarInfoStore.BackgroundImageProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="BackgroundImage"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBackgroundImage
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.BackgroundImageProperty);
            }
        }

        #endregion
        #region SegmentWidth
        /// <summary>
        /// Gets or sets the width of the segments.
        /// </summary>
        /// <remarks>By default it`s value is 12.</remarks>
        [Category("Appearance")]
        [Description("The width of the segments.")]
        public int SegmentWidth
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridProgressBarInfoStore.SegmentWidthProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.SegmentWidthProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="SegmentWidth"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetSegmentWidth()
        {
            ResetValue(GridProgressBarInfoStore.SegmentWidthProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeSegmentWidth()
        {
            return HasValue(GridProgressBarInfoStore.SegmentWidthProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="SegmentWidth"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasSegmentWidth
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.SegmentWidthProperty);
            }
        }

        #endregion
        #region FontColor
        /// <summary>
        /// Gets or sets the color of the font.
        /// </summary>
        [Category("Appearance")]
        [Description("The color of the font used to draw the text of the ProgressBar.")]
        public Color FontColor
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(GridProgressBarInfoStore.FontColorProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.FontColorProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="FontColor"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetFontColor()
        {
            ResetValue(GridProgressBarInfoStore.FontColorProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeFontColor()
        {
            return HasValue(GridProgressBarInfoStore.FontColorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="FontColor"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFontColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.FontColorProperty);
            }
        }

        #endregion
        #region ForeColor
        /// <summary>
        /// Gets or sets the color used to draw the foreground in segment mode and constant mode.
        /// </summary>
        [Description("The color used to draw the foreground in segment mode and constant mode.")]
        public Color ForeColor
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(GridProgressBarInfoStore.ForeColorProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.ForeColorProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="ForeColor"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetForeColor()
        {
            ResetValue(GridProgressBarInfoStore.ForeColorProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeForeColor()
        {
            return HasValue(GridProgressBarInfoStore.ForeColorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="ForeColor"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasForeColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.ForeColorProperty);
            }
        }

        #endregion
        #region TextVisible
        /// <summary>
        /// Gets or sets a value indicating whether the text is visible.
        /// </summary>
        [Category("Appearance")]
        ////        [DefaultValue(true)]
        [Description("Determines if the text of the Progressbar is visible.")]
        public bool TextVisible
        {
            [DebuggerStepThrough()]
            get
            {
                return (bool)GetValue(GridProgressBarInfoStore.TextVisibleProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.TextVisibleProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="TextVisible"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetTextVisible()
        {
            ResetValue(GridProgressBarInfoStore.TextVisibleProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTextVisible()
        {
            return HasValue(GridProgressBarInfoStore.TextVisibleProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="TextVisible"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTextVisible
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.TextVisibleProperty);
            }
        }

        #endregion
        #region TextStyle
        /// <summary>
        /// Gets or sets the style of the text:
        ///    -Percentage
        ///    -Value (Ex:  70/150 )
        /// </summary>
        [Category("Appearance")]
        ////        [DefaultValue(ProgressBarTextStyles.Percentage)]
        [Description("Determines the style of the text.")]
        public ProgressBarTextStyles TextStyle
        {
            [DebuggerStepThrough()]
            get
            {
                return (ProgressBarTextStyles)GetValue(GridProgressBarInfoStore.TextStyleProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.TextStyleProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="TextStyle"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetTextStyle()
        {
            ResetValue(GridProgressBarInfoStore.TextStyleProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTextStyle()
        {
            return HasValue(GridProgressBarInfoStore.TextStyleProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="TextStyle"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTextStyle
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.TextStyleProperty);
            }
        }

        #endregion
        #region ProgressOrientation
        /// <summary>
        /// Gets or sets the orientation of the text.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(Orientation.Horizontal)]
        [Description("Determines the orientation of the text.")]
        public Orientation ProgressOrientation
        {
            [DebuggerStepThrough()]
            get
            {
                return (Orientation)GetValue(GridProgressBarInfoStore.ProgressOrientationProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.ProgressOrientationProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="ProgressOrientation"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetProgressOrientation()
        {
            ResetValue(GridProgressBarInfoStore.ProgressOrientationProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeProgressOrientation()
        {
            return HasValue(GridProgressBarInfoStore.ProgressOrientationProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="ProgressOrientation"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasProgressOrientation
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.ProgressOrientationProperty);
            }
        }

        #endregion
        #region TextShadow
        /// <summary>
        /// Gets or sets a value indicating whether the text shadow is visible.
        /// </summary>
        [Category("Appearance")]
        ////        [DefaultValue(true)]
        [Description("Determines if the text shadow is visible.")]
        public bool TextShadow
        {
            [DebuggerStepThrough()]
            get
            {
                return (bool)GetValue(GridProgressBarInfoStore.TextShadowProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.TextShadowProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="TextShadow"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetTextShadow()
        {
            ResetValue(GridProgressBarInfoStore.TextShadowProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTextShadow()
        {
            return HasValue(GridProgressBarInfoStore.TextShadowProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="TextShadow"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTextShadow
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.TextShadowProperty);
            }
        }

        #endregion
        #region ProgressStyle
        /// <summary>
        /// Gets or sets the style of the foreground:
        ///  -Constant
        ///     -Gradient
        ///  -Multiple gradient
        ///  -Tube
        ///  -Image
        ///     -System
        /// </summary>
        /// <remarks>
        /// By default its value is Constant.
        /// </remarks>
        [Category("Appearance")]
        ////        [DefaultValue(ProgressBarStyles.Constant)]
        [Description("Determines the foreground drawing style.")]
        public ProgressBarStyles ProgressStyle
        {
            [DebuggerStepThrough()]
            get
            {
                return (ProgressBarStyles)GetValue(GridProgressBarInfoStore.ProgressStyleProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.ProgressStyleProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="ProgressStyle"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetProgressStyle()
        {
            ResetValue(GridProgressBarInfoStore.ProgressStyleProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeProgressStyle()
        {
            return HasValue(GridProgressBarInfoStore.ProgressStyleProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="ProgressStyle"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasProgressStyle
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.ProgressStyleProperty);
            }
        }

        #endregion
        #region ProgressFallbackStyle
        /// <summary>
        /// Gets or sets the style of the foreground when ProgressStyle is System and the system can not support Themes.
        /// </summary>
        [Category("Appearance")]
        ////        [DefaultValue(ProgressBarStyles.Constant)]
        [Description("Determines the foreground drawing style if System is selected and the Themes are not supported by the machine.")]
        public ProgressBarStyles ProgressFallbackStyle
        {
            [DebuggerStepThrough()]
            get
            {
                return (ProgressBarStyles)GetValue(GridProgressBarInfoStore.ProgressFallbackStyleProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.ProgressFallbackStyleProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="ProgressFallbackStyle"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetProgressFallbackStyle()
        {
            ResetValue(GridProgressBarInfoStore.ProgressFallbackStyleProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeProgressFallbackStyle()
        {
            return HasValue(GridProgressBarInfoStore.ProgressFallbackStyleProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="ProgressFallbackStyle"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasProgressFallbackStyle
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.ProgressFallbackStyleProperty);
            }
        }

        #endregion
        #region BackgroundStyle
        /// <summary>
        /// Gets or sets the style of the background. It can have the following values:
        /// -Image
        ///    -Gradient
        /// -Vertical gradient
        /// -Tube
        ///    -Multiple gradient
        ///    -System
        ///    -None
        /// </summary>
        /// <remarks>
        /// By default its value is None.
        /// </remarks>
        [Category("Appearance")]
        ////        [DefaultValue(ProgressBarBackgroundStyles.None)]
        [Description("Determines the background style.")]
        public ProgressBarBackgroundStyles BackgroundStyle
        {
            [DebuggerStepThrough()]
            get
            {
                return (ProgressBarBackgroundStyles)GetValue(GridProgressBarInfoStore.BackgroundStyleProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.BackgroundStyleProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="BackgroundStyle"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetBackgroundStyle()
        {
            ResetValue(GridProgressBarInfoStore.BackgroundStyleProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBackgroundStyle()
        {
            return HasValue(GridProgressBarInfoStore.BackgroundStyleProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="BackgroundStyle"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBackgroundStyle
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.BackgroundStyleProperty);
            }
        }
        #endregion
        #region BackgroundFallbackStyle
        /// <summary>
        /// Gets or sets the style of the background when BackgroundStyle is set to System and the system can not support Themes.
        /// </summary>
        /// <remarks>
        /// By default its value is None.
        /// </remarks>
        [Category("Appearance")]
        ////        [DefaultValue(ProgressBarBackgroundStyles.None)]
        [Description("Determines the background style when System mode is selected and the machine doesn`t support Themes.")]
        public ProgressBarBackgroundStyles BackgroundFallbackStyle
        {
            [DebuggerStepThrough()]
            get
            {
                return (ProgressBarBackgroundStyles)GetValue(GridProgressBarInfoStore.BackgroundFallbackStyleProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridProgressBarInfoStore.BackgroundFallbackStyleProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="BackgroundFallbackStyle"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetBackgroundFallbackStyle()
        {
            ResetValue(GridProgressBarInfoStore.BackgroundFallbackStyleProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBackgroundFallbackStyle()
        {
            return HasValue(GridProgressBarInfoStore.BackgroundFallbackStyleProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="BackgroundFallbackStyle"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBackgroundFallbackStyle
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridProgressBarInfoStore.BackgroundFallbackStyleProperty);
            }
        }

        #endregion
        ////        #region ProgressOrientation
        ////        ///// <summary>
        ////        ///// Determines the horizontal or vertical style of the progress bar.
        ////        ///// </summary>
        ////        ///// <remarks>
        ////        ///// By default its value is Horizontal.
        ////        ///// </remarks>
        ////        [Category("Appearance")]
        ////        [DefaultValue(Orientation.Horizontal)]
        ////        [Description("Determines the ProgressBar orientation.")]
        ////        public Orientation ProgressOrientation
        ////        {
        ////            [DebuggerStepThrough()] 
        ////            get 
        ////            {
        ////                return (Orientation) GetValue(GridProgressBarInfoStore.ProgressOrientationProperty);
        ////            }
        ////            [DebuggerStepThrough()] 
        ////            set 
        ////            {
        ////                SetValue(GridProgressBarInfoStore.ProgressOrientationProperty, value);
        ////            }
        ////        }
        ////        ///// <summary>
        ////        ///// Resets the <see cref="ProgressOrientation"/> property.
        ////        ///// </summary>
        ////        [DebuggerStepThrough()] public void ResetProgressOrientation()
        ////        {
        ////            ResetValue(GridProgressBarInfoStore.ProgressOrientationProperty);
        ////        }
        ////        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        ////        private bool ShouldSerializeProgressOrientation()
        ////        {
        ////            return HasValue(GridProgressBarInfoStore.ProgressOrientationProperty);
        ////        }
        ////        ///// <summary>
        ////        ///// Checks if <see cref="ProgressOrientation"/> property is initialized.
        ////        ///// </summary>
        ////        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ////        public bool HasProgressOrientation
        ////        {
        ////            [DebuggerStepThrough()] 
        ////            get
        ////            {
        ////                return HasValue(GridProgressBarInfoStore.ProgressOrientationProperty);
        ////            }
        ////        }
        ////        #endregion
    }
}
