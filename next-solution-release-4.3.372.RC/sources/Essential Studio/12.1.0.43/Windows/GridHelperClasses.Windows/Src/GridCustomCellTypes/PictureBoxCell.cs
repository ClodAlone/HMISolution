//-------------------------------------------------------------------------------------------------
// <copyright file="PictureBoxCell.cs" company="Syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    using System;
    using System.Drawing;
    using System.Collections;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Windows.Forms;
    using System.IO;

    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.Styles;

    /// <summary>
    /// Implements a data model for PictureBox cell.
    /// </summary>
    [Serializable]
    public class PictureBoxCellModel : GridGenericControlCellModel
    {
        /// <summary>
        /// Constructor for PictureBoxCellModel.
        /// </summary>
        /// <param name="grid">The grid model.</param>
        public PictureBoxCellModel(GridModel grid)
            : base(grid)
        {
            PictureBoxStyleProperties.Initialize();
        }
        /// <summary>
        /// sets variable for picture Box cell model
        /// </summary>
        /// <param name="info">SerializationInfo</param>
        /// <param name="context">StreamingContext</param>
        protected PictureBoxCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Creates renderer.
        /// </summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new PictureBoxCellRenderer(control, this);
        }
        /// <summary>
        /// Sets the preferred client size for grid
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="style">GridStyleInfo</param>
        /// <param name="queryBounds">GridQueryBounds</param>
        /// <returns></returns>
        protected override Size OnQueryPrefferedClientSize(Graphics g, int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            PictureBoxStyleProperties sp = new PictureBoxStyleProperties(style);
            Image img = sp.Image;
            if (img != null)
            {
                return img.Size;
            }

            return base.OnQueryPrefferedClientSize(g, rowIndex, colIndex, style, queryBounds);
        }

        /// <summary>
        /// Initializes picture box properties.
        /// </summary>
        /// <param name="c">Picture box.</param>
        /// <param name="style">Cell style information.</param>
        public static void InitializePictureBox(PictureBox c, GridStyleInfo style)
        {
            PictureBoxStyleProperties sp = new PictureBoxStyleProperties(style);
            c.SizeMode = sp.SizeMode;
            c.Image = sp.Image;
            c.BackColor = style.BackColor;
        }
    }

    /// <summary>
    /// Implements a renderer for PictureBox cell.
    /// </summary>
    public class PictureBoxCellRenderer : GridGenericControlCellRenderer
    {
        PictureBox drawPictureBox = new PictureBox();
        PictureBoxCellModel cm;

        /// <summary>
        /// Constructor for PictureBoxCellRenderer.
        /// </summary>
        /// <param name="grid">The grid control.</param>
        /// <param name="cellModel">The cell model.</param>
        public PictureBoxCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            this.cm = (PictureBoxCellModel) cellModel;
            SupportsFocusControl = false;
            FixControlParent(this.drawPictureBox);
        }
        /// <summary>
        /// Destroys the object
        /// </summary>
        /// <param name="disposing">bool</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.drawPictureBox.Dispose();
                this.drawPictureBox = null;
                this.cm = null;
            }

            base.Dispose(disposing);
        }
        /// <summary>
        /// Is Triggered when the the cells are drawnin cell.
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="clientRectangle">Rectangle</param>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="style">GridStyleInfo</param>
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            style.Control = this.drawPictureBox;
            PictureBoxCellModel.InitializePictureBox(this.drawPictureBox, style);
            base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
        }
        /// <summary>
        /// OnLayout
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="style">GridStyleInfo</param>
        /// <param name="innerBounds">Rectangle</param>
        /// <param name="buttonsBounds">Rectangle[]</param>
        /// <returns></returns>
        protected override Rectangle OnLayout(int rowIndex, int colIndex, GridStyleInfo style, Rectangle innerBounds, Rectangle[] buttonsBounds)
        {
            Rectangle r = base.OnLayout(rowIndex, colIndex, style, innerBounds, buttonsBounds);

            r.Inflate(-1, -1);

            return r;

            // center windows forms trackBar inside cell.
            // return GridUtil.CenterInRect(r, cm.trackBarControlSize);
        }
    }

    /// <summary>
    /// Provides custom properties specific for PictureBox control. All properties
    /// support the style inheritance mechanism. You can change these propeties
    /// in the property grid.
    /// </summary>
    public class PictureBoxStyleProperties : GridStyleInfoCustomProperties
    {
        // static initialization of property descriptors
        static Type t = typeof(PictureBoxStyleProperties);

        readonly static StyleInfoProperty ImageProperty = CreateStyleInfoProperty(t, "Image");
        readonly static StyleInfoProperty SizeModeProperty = CreateStyleInfoProperty(t, "SizeMode");

        // default settings for all properties this object holds
        static PictureBoxStyleProperties defaultObject;

        // initialize default settings for all properties in static ctor
        static PictureBoxStyleProperties()
        {
            // all properties must be initialized for the Default property
            defaultObject = new PictureBoxStyleProperties(GridStyleInfo.Default);
            defaultObject.SizeMode = PictureBoxSizeMode.Normal;
            defaultObject.Image = null;
        }

        /// <summary>
        /// Gets access to default values for this type
        /// </summary>
        public static PictureBoxStyleProperties Default
        {
            get
            {
                return defaultObject;
            }
        }

        /// <summary>
        /// Force static ctor being called at least once
        /// </summary>
        public static void Initialize()
        {
        }

        // explicit cast from GridStyleInfo to PictureBoxStyleProperties
        // (Note: this will only work for C#, Visual Basic does not support dynamic casts)

        /// <summary>
        /// Explicit cast from GridStyleInfo to this custom propety object
        /// </summary>
        /// <param name="style">Cell style information.</param>
        /// <returns>A new custom properties object.</returns>
        public static explicit operator PictureBoxStyleProperties(GridStyleInfo style)
        {
            return new PictureBoxStyleProperties(style);
        }

        /// <summary>
        /// Initializes a PictureBoxStyleProperties object with a style object that holds all data
        /// </summary>
        /// <param name="style">Cell style information.</param>
        public PictureBoxStyleProperties(GridStyleInfo style)
            : base(style)
        {
        }

        /// <summary>
        /// Initializes a PictureBoxStyleProperties object with an empty style object. Design
        /// time environment will use this ctor and later copy the values to a style object
        /// by calling style.CustomProperties.Add(gridExcelTipStyleProperties1)
        /// </summary>
        public PictureBoxStyleProperties()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets value to indicate how the image is displayed.
        /// </summary>
        /// <remarks>
        /// Valid values for this property are taken from the PictureBoxSizeMode
        /// enumeration. By default, in PictureBoxSizeMode.Normal mode, the Image
        /// is placed in the upper left corner of the PictureBox, and any part of
        /// the image too big for the PictureBox is clipped. Using the
        /// PictureBoxSizeMode.StretchImage value causes the image to stretch to
        /// fit the PictureBox.
        /// <para/>
        /// Using the PictureBoxSizeMode.AutoSize value causes the control to resize
        /// to always fit the image. Using the PictureBoxSizeMode.CenterImage value
        /// causes the image to be centered in the client area.
        /// <para/>
        /// </remarks>
        [Description("Indicates how the image is displayed."),
        Browsable(true),
        Category("PictureBox")]
        public PictureBoxSizeMode SizeMode
        {
            get
            {
                // TraceUtil.TraceCurrentMethodInfo();
                return (PictureBoxSizeMode)style.GetValue(SizeModeProperty);
            }

            set
            {
                // TraceUtil.TraceCurrentMethodInfo(value);
                style.SetValue(SizeModeProperty, value);
            }
        }

        /// <summary>
        /// Resets SizeMode state
        /// </summary>
        public void ResetSizeMode()
        {
            style.ResetValue(SizeModeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeSizeMode()
        {
            return style.HasValue(SizeModeProperty);
        }

        /// <summary>
        /// Gets a value indicating whether SizeMode state has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasSizeMode
        {
            get
            {
                return style.HasValue(SizeModeProperty);
            }
        }

        /// <summary>
        /// Gets or sets the image that the PictureBox displays.
        /// </summary>
        [Description("Gets or sets the image that the PictureBox displays."),
        Browsable(true),
        Category("PictureBox")]
        public Image Image
        {
            get
            {
                // TraceUtil.TraceCurrentMethodInfo();
                return (Image) style.GetValue(ImageProperty);
            }

            set
            {
                // TraceUtil.TraceCurrentMethodInfo(value);
                style.SetValue(ImageProperty, value);
            }
        }

        /// <summary>
        /// Resets Image state
        /// </summary>
        public void ResetImage()
        {
            style.ResetValue(ImageProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeImage()
        {
            return style.HasValue(ImageProperty);
        }

        /// <summary>
        /// Gets a value indicating whether image state has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasImage
        {
            get
            {
                return style.HasValue(ImageProperty);
            }
        }
    }
}
