#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Syncfusion.Windows.Forms.Tools;
using System.Drawing;
using System.ComponentModel.Design;

namespace Syncfusion.Windows.Forms
{
    #region Interface
    /// <summary>
    /// Interface for SkinManager
    /// </summary>
    public interface IVisualStyle
    {
        string VisualTheme
        {
            get;
            set;
        }
    }
    #endregion

    #region Enum
    /// <summary>
    /// Holds the option for the VisualTheme
    /// </summary>
    public enum VisualTheme
    {
        Managed,
        Office2007Blue,
        Office2007Silver,
        Office2007Black,
        Office2010Blue,
        Office2010Silver,
        Office2010Black,
        Metro
    }
    #endregion

    #region Icon
    [
    ToolboxItem(true),
    ToolboxBitmap(typeof(SkinManager), "ToolboxIcons.SkinManager.bmp"),
    Description("Set same VisualTheme for All Controls.")
    ]
    #endregion

    #region SkinManager

    public partial class SkinManager : Component
    {
        #region Class Members
        /// <summary>
        /// Contains a  control.
        /// </summary>
        Control control;
        /// <summary>
        /// Contains a component.
        /// </summary>
        Component component;
        #endregion

        #region Properties
        /// <summary>
        /// Get or Set the control to Change the skin.
        /// </summary>
        ///  
        [
          Description("Assign Control to change the skin "),
          Category("Control")
        ]
        public Component Controls
        {
            get
            {
               return component ;
            }
            set
            {
                if (component != value)
                {
                    component = value;
                    control = component as Control;
                    String visualStyle = Convert.ToString(visualTheme);
                    IVisualStyle style = component as IVisualStyle;
                    if (style != null)
                        style.VisualTheme = visualStyle;
                    if (control != null)
                        SetVisualStyle(control, visualTheme);
                }
            }
        }
        /// <summary>
        /// Get or Set the VisualTheme for the control.
        /// </summary>
        ///  
        [
          Description("VisualTheme for the Control"),
          Category("Appearance")
        ]
        VisualTheme visualTheme;
        public VisualTheme  VisualTheme
        {
            get {
                    return visualTheme;
                }
            set {
                if (visualTheme != value)
                {
                    visualTheme = value;
                    String visualStyle = Convert.ToString(visualTheme);
                    IVisualStyle style = component as IVisualStyle;
                    if (style != null)
                        style.VisualTheme = visualStyle;
                    if (control != null)
                        SetVisualStyle(control, visualTheme);
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="container"></param>
        public static bool ContainsSkinManager = false;
        public SkinManager(IContainer container)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(SkinManager));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            container.Add(this);
            if (this.Container.Components != null)
            {
               ComponentCollection clln = this.Container.Components;
                foreach (IComponent obj in clln)
                {
                    // If an instance of the SkinManager already exists for this designerhost,
                    // then throw an exception.
                    if (obj is SkinManager)// && (obj != this))
                    {
                        if (ContainsSkinManager )
                        {
                            container.Remove(this);
                            throw new ApplicationException("Only one instance of the SkinManager can exist on a form.");
                        }
                        else
                            ContainsSkinManager  = true;
                    }
                }
            }
           //InitializeComponent();
        }
        #endregion

        #region Methods
        [BrowsableAttribute(true)]
        public event ControlEventHandler ControlAdded;
        public static string visualStyle;
        /// <summary>
        /// Set the VisualTheme for them for the parent control by Control and Visual Theme.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="visualTheme"></param>
        public static void SetVisualStyle(Control control,   VisualTheme visualTheme)
        {
            visualStyle = Convert.ToString(visualTheme);
            if (control is Form)
                control.ControlAdded += new ControlEventHandler(control_ControlAdded);
            if(control is Office2007Form)
                SetVisualStyle(control, visualStyle);
            SetVisualStyle(control, visualStyle);
          
        }
        public static void control_ControlAdded(object sender, ControlEventArgs e)
        {
            if (ContainsSkinManager)
                SetVisualStyle(e.Control, visualStyle);
                        
        }
        /// <summary>
        /// Set the VisualTheme for them for the parent control.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="visualTheme"></param>
        public static void SetVisualStyle(Control control, String visualStyle)
        {
            IVisualStyle style = control as IVisualStyle;
            if (style != null)
                style.VisualTheme = visualStyle;
            IThemedControl IThemeCtrl = control as IThemedControl;
            if (IThemeCtrl != null)
                IThemeCtrl.ThemesEnabled = true;
            IterateComponents(control, visualStyle);
            IterateControls(control, visualStyle);
        }
        /// <summary>
        /// Iterations for the VisualTheme for the child control and components.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="visualStyle"></param>
        static void IterateControls(Control control, string visualStyle)
        {
            foreach (Control ctrl in control.Controls)
            {
                IVisualStyle style = ctrl as IVisualStyle;
                if (style != null)
                {
                    style.VisualTheme = visualStyle;
                    ctrl.Refresh();
                }
                IThemedControl IThemeCtrl = ctrl as IThemedControl;
                if (IThemeCtrl != null)
                    IThemeCtrl.ThemesEnabled = true;
                IterateComponents(ctrl, visualStyle);
                IterateControls(ctrl, visualStyle);
            }
        }

        /// <summary>
        /// Iterations for the VisualTheme for the Components.
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="visualStyle"></param>
        static void IterateComponents(Control ctrl, string visualStyle)
        {
            try
            {
                FieldInfo info = ctrl.GetType().GetField("components", BindingFlags.Instance  | BindingFlags.NonPublic);
                if (info != null)
                {
                    IContainer container = info.GetValue(ctrl) as IContainer;
                    if (container != null)
                    {
                        foreach (Component component in container.Components)
                        {
                            IVisualStyle style = component as IVisualStyle;
                            if (style != null)
                                style.VisualTheme = visualStyle;
                        }
                    }
                }
            }
            catch (Exception)
            { }
        }
        #endregion

        
        protected override void Dispose(bool disposing)
        {
             if(control != null)
                 control.ControlAdded -= new ControlEventHandler(control_ControlAdded);
             ContainsSkinManager = false;
        }
    }
    #endregion
}
