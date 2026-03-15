#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.CodeDom;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Resources;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Contains the standart commands.
    /// </summary>
    public static class ChartCommands
    {
        #region Internal types
        /// <summary>
        /// The ChartCommandInternal class.
        /// </summary>
        class ChartCommandInternal : ChartCommand
        {
            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="ChartCommandInternal"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="title">The title.</param>
            /// <param name="icon">The icon.</param>
            public ChartCommandInternal(string name, string title, Image icon)
                : base(name, title, icon)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ChartCommandInternal"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="icon">The icon.</param>
            public ChartCommandInternal(string name, Image icon)
                : base(name, icon)
            {
            }
            #endregion

            #region Implementation
            /// <summary>
            /// Executes command.
            /// </summary>
            /// <param name="chart">The chart.</param>
            /// <param name="parameter">The parameter.</param>
            public override void Execute(ChartControl chart, string parameter)
            {
                chart.ExecuteCommand(this, parameter);
            }

            /// <summary>
            /// Determines whether the command is toggled.
            /// </summary>
            /// <param name="chart">The chart.</param>
            /// <param name="parameter">The parameter.</param>
            /// <returns>
            /// 	<c>true</c> if the specified chart is toggled; otherwise, <c>false</c>.
            /// </returns>
            public override bool IsToggled(ChartControl chart, string parameter)
            {
                return chart.IsCommandToggled(this, parameter);
            }
            #endregion
        }
        #endregion

        #region Members
        private static readonly ChartCommand c_save;
        private static readonly ChartCommand c_copy;
        private static readonly ChartCommand c_print;
        private static readonly ChartCommand c_printPriview;
        private static readonly ChartCommand c_zoomIn;
        private static readonly ChartCommand c_zoomOut;
        private static readonly ChartCommand c_resetZooming;
        private static readonly ChartCommand c_showLegend;
        private static readonly ChartCommand c_3dMode;
        private static readonly ChartCommand c_autoHighlight;
        private static readonly ChartCommand c_toggleXZooming;
        private static readonly ChartCommand c_toggleYZooming;
        private static readonly ChartCommand c_togglePanning;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the "save" command.
        /// </summary>
        /// <value>The "save" command.</value>
        public static ChartCommand Save
        {
            get
            {
                return c_save;
            }
        }

        /// <summary>
        /// Gets the "copy" command.
        /// </summary>
        /// <value>The "copy" command.</value>
        public static ChartCommand Copy
        {
            get
            {
                return c_copy;
            }
        }

        /// <summary>
        /// Gets the "print" command.
        /// </summary>
        /// <value>The "print" command.</value>
        public static ChartCommand Print
        {
            get
            {
                return c_print;
            }
        }

        /// <summary>
        /// Gets the "print preview" command.
        /// </summary>
        /// <value>The "print preview" command.</value>
        public static ChartCommand PrintPriview
        {
            get
            {
                return c_printPriview;
            }
        }

        /// <summary>
        /// Gets the "zoom in" command.
        /// </summary>
        /// <value>The "zoom in" command.</value>
        public static ChartCommand ZoomIn
        {
            get
            {
                return c_zoomIn;
            }
        }

        /// <summary>
        /// Gets the "zoom out" command.
        /// </summary>
        /// <value>The "zoom out" command.</value>
        public static ChartCommand ZoomOut
        {
            get
            {
                return c_zoomOut;
            }
        }

        /// <summary>
        /// Gets the "reset zooming" command.
        /// </summary>
        /// <value>The "reset zooming" command.</value>
        public static ChartCommand ResetZooming
        {
            get
            {
                return c_resetZooming;
            }
        }

        /// <summary>
        /// Gets the "show legend" command.
        /// </summary>
        /// <value>The "show legend" command.</value>
        public static ChartCommand ShowLegend
        {
            get
            {
                return c_showLegend;
            }
        }

        /// <summary>
        /// Gets the "toggle 3D" command.
        /// </summary>
        /// <value>The "toggle 3D" command.</value>
        public static ChartCommand Toggle3D
        {
            get
            {
                return c_3dMode;
            }
        }

        /// <summary>
        /// Gets the "auto highlight" command.
        /// </summary>
        /// <value>The "auto highlight" command.</value>
        public static ChartCommand AutoHighlight
        {
            get
            {
                return c_autoHighlight;
            }
        }

        /// <summary>
        /// Gets the "toggle x zooming" command.
        /// </summary>
        /// <value>The "toggle x zooming" command.</value>
        public static ChartCommand ToggleXZooming
        {
            get
            {
                return c_toggleXZooming;
            }
        }

        /// <summary>
        /// Gets the "toggle y zooming" command.
        /// </summary>
        /// <value>The "toggle y zooming" command.</value>
        public static ChartCommand ToggleYZooming
        {
            get
            {
                return c_toggleYZooming;
            }
        }

        /// <summary>
        /// Gets the toggle panning.
        /// </summary>
        /// <value>The toggle panning.</value>
        public static ChartCommand TogglePanning
        {
            get
            {
                return c_togglePanning;
            }
        }
        #endregion

        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of <see cref="ChartCommands"/> class.
        /// </summary>
        static ChartCommands()
        {
            c_save = new ChartCommandInternal("Save", "Save", ChartCommandsImages.Save);
            c_copy = new ChartCommandInternal("Copy", "Copy", ChartCommandsImages.Copy);
            c_print = new ChartCommandInternal("Print", "Print", ChartCommandsImages.Print);
            c_printPriview = new ChartCommandInternal("PrintPriview", "Print priview", ChartCommandsImages.PrintPreview);
            c_zoomIn = new ChartCommandInternal("ZoomIn", "Zoom in", ChartCommandsImages.ZoomIn);
            c_zoomOut = new ChartCommandInternal("ZoomOut", "Zoom out", ChartCommandsImages.ZoomOut);
            c_resetZooming = new ChartCommandInternal("ResetZooming", "Reset zoom", ChartCommandsImages.ResetZooming);
            c_showLegend = new ChartCommandInternal("ShowLegend", "Show legend", ChartCommandsImages.ShowLegend);
            c_3dMode = new ChartCommandInternal("Toggle3D", "Toogle mode", ChartCommandsImages.Series3D);
            c_autoHighlight = new ChartCommandInternal("AutoHighlight", "Auto highlight", ChartCommandsImages.AutoHighlight);
            c_toggleXZooming = new ChartCommandInternal("ToogleXZooming", "Enable X Zooming", ChartCommandsImages.XZooming);
            c_toggleYZooming = new ChartCommandInternal("ToogleYZooming", "Enable Y Zooming", ChartCommandsImages.YZooming);
            c_togglePanning = new ChartCommandInternal("TooglePanning", "Enable Panning", ChartCommandsImages.Panning);
        }
        #endregion
    }

    /// <summary>
    /// Defines a command. 
    /// </summary>
    /// <remarks>
    /// Commands can be used for determinate the functionality of GUI elements.
    /// </remarks>
    /// <seealso cref="ChartToolBarCommandItem"/>
    [Editor(typeof(ChartCommandEditor), typeof(UITypeEditor))]
    [DesignerSerializer(typeof(ChartCommandCodeDomSerializer), typeof(CodeDomSerializer))]
    public abstract class ChartCommand
    {
        #region Members
        private string m_name;
        private string m_text;
        private Image m_image;
        #endregion

        #region Proeprties
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// Gets the image of icon.
        /// </summary>
        /// <value>The image of icon.</value>
        public Image Image
        {
            get { return m_image; }
        }

        /// <summary>
        /// Gets the command title.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return m_text;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartCommand"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="icon">The icon.</param>
        public ChartCommand(string name, Image icon)
            : this(name, name, icon)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartCommand"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="title">The title.</param>
        /// <param name="icon">The icon.</param>
        public ChartCommand(string name, string title, Image icon)
        {
            m_name = name;
            m_text = title;
            m_image = icon;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Executes command.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="parameter">The parameter.</param>
        public abstract void Execute(ChartControl chart, string parameter);

        /// <summary>
        /// Determines whether the command is toggled.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        ///      <c>true</c> if the specified chart is toggled; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool IsToggled(ChartControl chart, string parameter);
        #endregion
    }

    /// <summary>
    /// The ChartCommandEditor class.
    /// </summary>
    class ChartCommandEditor : ChartDropDownUIEditor
    {
        #region Implementation
        /// <summary>
        /// Indicates whether the specified context supports painting a representation of an object's value within the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that can be used to gain additional context information.</param>
        /// <returns>
        /// true if <see cref="M:System.Drawing.Design.UITypeEditor.PaintValue(System.Object,System.Drawing.Graphics,System.Drawing.Rectangle)"></see> is implemented; otherwise, false.
        /// </returns>
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Paints a representation of the value of an object using the specified <see cref="T:System.Drawing.Design.PaintValueEventArgs"></see>.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Drawing.Design.PaintValueEventArgs"></see> that indicates what to paint and where to paint it.</param>
        public override void PaintValue(PaintValueEventArgs e)
        {
            ChartCommand command = e.Value as ChartCommand;

            if (command != null)
            {
                e.Graphics.DrawImage(command.Image, e.Bounds);
            }

            base.PaintValue(e);
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <returns>Returns ChartCommands collection.</returns>
       protected override IList GetValues()
        {
            ChartCommand[] commands = new ChartCommand[]{
                ChartCommands.Save,
                ChartCommands.Copy,
                ChartCommands.Print,
                ChartCommands.PrintPriview,
                ChartCommands.ShowLegend,
                ChartCommands.Toggle3D,
                ChartCommands.ZoomIn,
                ChartCommands.ZoomOut,
                ChartCommands.ResetZooming,
                ChartCommands.AutoHighlight,
                ChartCommands.ToggleXZooming,
                ChartCommands.ToggleYZooming,
                ChartCommands.TogglePanning
            };

            return commands;
        }

        /// <summary>
        /// Gets the item text.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns Item's Text.</returns>
        protected override string GetItemText(object item)
        {
            return (item as ChartCommand).Name;
        }
        #endregion
    }

    /// <summary>
    /// The ChartCommandCodeDomSerializer class.
    /// </summary>
    class ChartCommandCodeDomSerializer : CodeDomSerializer
    {
        #region Implementation
        /// <summary>
        /// Serializes the specified object into a CodeDOM object.
        /// </summary>
        /// <param name="manager">The serialization manager to use during serialization.</param>
        /// <param name="value">The object to serialize.</param>
        /// <returns>
        /// A CodeDOM object representing the object that has been serialized.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">manager or value is null.</exception>
        public override object Serialize(IDesignerSerializationManager manager, object value)
        {
            CodeExpression result = null;
            CodeExpression targetExpression = new CodeTypeReferenceExpression(typeof(ChartCommands));

            foreach (System.Reflection.PropertyInfo property in typeof(ChartCommands).GetProperties())
            {
                if (property.GetValue(null, null) == value)
                {
                    result = new CodePropertyReferenceExpression(targetExpression, property.Name);
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Deserializes the specified serialized CodeDOM object into an object.
        /// </summary>
        /// <param name="manager">A serialization manager interface that is used during the deserialization process.</param>
        /// <param name="codeObject">A serialized CodeDOM object to deserialize.</param>
        /// <returns>The deserialized CodeDOM object.</returns>
        /// <exception cref="T:System.ArgumentNullException">manager or codeObject is null.</exception>
        /// <exception cref="T:System.ArgumentException">codeObject is an unsupported code element.</exception>
        public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
        {
            return null;
        }
        #endregion
    }
}
