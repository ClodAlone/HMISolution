#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.HTMLUI;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for custom controls in the document.
    /// It allows users to create controls from an HTML document.
    /// </summary>
    [ElementTag(TagName.Custom)]
    public class CUSTOMElementImpl : BaseElement
    {
        #region Class constants
        /// <summary>
        /// Name of the Tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Custom;

        /// <summary>
        /// Attribute name for assembly loading.
        /// </summary>
        private const string DEF_ASM_NAME = "assembly";

        /// <summary>
        /// Attribute name for class loading.
        /// </summary>
        private const string DEF_CLASS_NAME = "class";

        /// <summary>
        /// Supported events.
        /// </summary>
        private static string[] DEF_SUPP_EVENTS;

        /// <summary>
        /// Holds all events which this class supports.
        /// </summary>
        private static Hashtable m_eventHash;
        #endregion

        #region Class members
        /// <summary>
        /// Type of custom control.
        /// </summary>
        private Type m_controlType;

        /// <summary>
        /// Custom control instance.
        /// </summary>
        private Control m_control;
        #endregion

        #region Class properties
        /// <summary>
        /// Returns an array of supported events.
        /// </summary>
        public override string[] SupportedEvents
        {
            get
            {
                return DEF_SUPP_EVENTS;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes static members of the CUSTOMElementImpl class 
        /// </summary>
        static CUSTOMElementImpl()
        {
            Type type = typeof(CUSTOMElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Initializes a new instance of the CUSTOMElementImpl class
        /// </summary>
        /// <param name="parent">Parent element for this object.</param>
        public CUSTOMElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }

        #endregion

        #region Class Overrides
        /// <summary>
        /// Overridden. Returns an instance of the event.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <returns>Event object.</returns>
        protected override IHTMLEvent CreateEventInternal(string name)
        {
            return new HashElementEvents(this, m_eventHash, name);
        }

        /// <summary>
        /// Overridden. Calculates the size of the element for rendering.
        /// </summary>
        /// <returns>Size object</returns>
        protected override Size CalculateSizeInternal()
        {
            this.Size = DefaultCalculateSizeInternal();

            return this.Size;
        }

        /// <summary>
        /// Overridden. Calculates the position of the element for rendering.
        /// </summary>
        protected override void CalculatePositionInternal()
        {
            BaseElement parent = (BaseElement)this.Parent;
            this.CurrentPosition = parent.CurrentPosition;

            CalculateChildPositions(this.CurrentPosition, parent.Bounds);
        }

        /// <summary>
        /// Overridden. Calculates the format of the element from the array of possible formats.
        /// </summary>
        protected override void CalculateFormatInternal()
        {
            DefaultCalculateFormatInternal();
        }

        /// <summary>
        /// Overridden. Initialization of the object.
        /// </summary>
        protected internal override void InitializeElement()
        {
            base.InitializeElement();

            Assembly controlAsm = LoadAssembly();

            if (controlAsm != null)
            {
                m_control = CreateInstance(controlAsm);

                if (m_control != null)
                {
                    InitializeControl(m_control);

                    //// Create custom control object.
                    new CustomControlBase(this, m_control);
                    ////this.Control.PreRenderDocument += new PreRenderDocumentEventHandler( Control_PreRenderDocument );
                }
            }
        }
        #endregion

        #region Class event handlers
        /// <summary>
        /// Handles pre-render events of the control.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Control_PreRenderDocument(object sender, PreRenderDocumentArgs e)
        {
            new CustomControlBase(this, m_control);

            // Detach handler.
            this.Control.PreRenderDocument -= new PreRenderDocumentEventHandler(Control_PreRenderDocument);
        }

        #endregion

        #region Class utility methods
        /// <summary>
        /// Loads the assembly.
        /// </summary>
        /// <returns>Assembly containing custom control if loaded; otherwise NULL.</returns>
        private Assembly LoadAssembly()
        {
            Assembly controlAsm = null;
            IHTMLAttribute attr = this.Attributes[DEF_ASM_NAME];

            if (attr != null && attr.Value != null && attr.Value.Length > 0)
            {
                controlAsm = SearchInLoaded(attr.Value);

                if (controlAsm == null)
                {
                    controlAsm = LoadAssemblyFromPartialName(attr.Value);
                }

                if (controlAsm == null)
                {
                    controlAsm = LoadAssemblyFromFileName(attr.Value);
                }

                if (controlAsm == null)
                {
                    controlAsm = LoadAssemblyFromFullName(attr.Value);
                }
            }

            return controlAsm;
        }

        /// <summary>
        /// Searches the specified assembly in the loaded current domain assemblies.
        /// </summary>
        /// <param name="name">Name of the assembly.</param>
        /// <returns>Assembly if found; Null otherwise.</returns>
        private Assembly SearchInLoaded(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly asm = null;

            for (int i = 0, len = assemblies.Length; i < len; i++)
            {
                asm = assemblies[i];

                if (asm.GetName().Name == name) return asm;
            }

            return null;
        }

        /// <summary>
        /// Loads the assembly from full name of assembly.
        /// </summary>
        /// <param name="name">Full name of the assembly.</param>
        /// <returns>Loaded assembly or NULL.</returns>
        private Assembly LoadAssemblyFromFullName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            Assembly result = null;

            try
            {
                result = Assembly.Load(name);
            }
            catch (FileNotFoundException fe)
            {
                Debug.WriteLine(fe.Message + Environment.NewLine + fe.StackTrace);
            }
            catch (BadImageFormatException be)
            {
                Debug.WriteLine(be.Message + Environment.NewLine + be.StackTrace);
            }
            catch (SecurityException se)
            {
                Debug.WriteLine(se.Message + Environment.NewLine + se.StackTrace);
            }
            catch (PathTooLongException pe)
            {
                Debug.WriteLine(pe.Message + Environment.NewLine + pe.StackTrace);
            }

            return result;
        }

        /// <summary>
        /// Loads the assembly from the path to the assembly.
        /// </summary>
        /// <param name="path">Path to the assembly.</param>
        /// <returns>Loaded assembly or NULL.</returns>
        private Assembly LoadAssemblyFromFileName(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (path.Length == 0)
                throw new ArgumentException("path - string can not be empty");

            Assembly result = null;

            try
            {
                result = Assembly.LoadFrom(path);
            }
            catch (FileNotFoundException fe)
            {
                Debug.WriteLine(fe.Message + Environment.NewLine + fe.StackTrace);
            }
            catch (BadImageFormatException be)
            {
                Debug.WriteLine(be.Message + Environment.NewLine + be.StackTrace);
            }
            catch (SecurityException se)
            {
                Debug.WriteLine(se.Message + Environment.NewLine + se.StackTrace);
            }
            catch (PathTooLongException pe)
            {
                Debug.WriteLine(pe.Message + Environment.NewLine + pe.StackTrace);
            }

            return result;
        }
#pragma warning disable
        /// <summary>
        /// Loads the assembly from partial name of the assembly.
        /// </summary>
        /// <param name="name">Partial name of the assembly.</param>
        /// <returns>Loaded assembly or NULL.</returns>
        private Assembly LoadAssemblyFromPartialName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            Assembly result = null;

            try
            {
                result = Assembly.LoadWithPartialName(name);
            }
            catch (FileNotFoundException fe)
            {
                Debug.WriteLine(fe.Message + Environment.NewLine + fe.StackTrace);
            }
            catch (BadImageFormatException be)
            {
                Debug.WriteLine(be.Message + Environment.NewLine + be.StackTrace);
            }
            catch (SecurityException se)
            {
                Debug.WriteLine(se.Message + Environment.NewLine + se.StackTrace);
            }
            catch (PathTooLongException pe)
            {
                Debug.WriteLine(pe.Message + Environment.NewLine + pe.StackTrace);
            }

            return result;
        }
#pragma warning enable
        /// <summary>
        /// Creates an instance of the control.
        /// </summary>
        /// <param name="asm">Assembly containing type of custom control.</param>
        /// <returns>Created control or NULL.</returns>
        private Control CreateInstance(Assembly asm)
        {
            Control control = null;

            if (asm != null)
            {
                IHTMLAttribute attr = this.Attributes[DEF_CLASS_NAME];

                if (attr != null && attr.Value != null && attr.Value.Length > 0)
                {
                    try
                    {
                        string typeName = attr.Value;
                        m_controlType = asm.GetType(typeName);

                        if (m_controlType == null)
                        {
                            typeName = GetAssemblyName(asm) + "." + typeName;
                            m_controlType = asm.GetType(typeName);
                        }

                        object obj = asm.CreateInstance(typeName, false);

                        if ((obj != null) && (obj is Control))
                        {
                            control = obj as Control;
                        }
                    }
                    catch (MissingMethodException me)
                    {
                        Debug.WriteLine(me.Message + Environment.NewLine + me.StackTrace);
                    }
                    catch (ArgumentException ae)
                    {
                        Debug.WriteLine(ae.Message + Environment.NewLine + ae.StackTrace);
                    }
                }
            }

            return control;
        }

        /// <summary>
        /// Sets all properties to the custom control.
        /// </summary>
        /// <param name="control">Custom control instance.</param>
        private void InitializeControl(Control control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            if (m_controlType != null)
            {
                IHTMLAttribute attr = null;
                TypeConverter converter = new TypeConverter();

                for (int i = 0, len = this.Attributes.Count; i < len; i++)
                {
                    attr = this.Attributes[i];

                    if (attr != null && attr.Value != null && attr.Value.Length > 0)
                    {
                        // Get corresponding property.
                        PropertyInfo prop = GetProperty(attr.Name);

                        // Check if property can set accessor.
                        if (prop != null && prop.CanWrite)
                        {
                            try
                            {
                                AttributeToken type = AttributeParser.DetectType(attr.Value);
                                object value = AttributeParser.GetSimpleValue(attr.Value, type);

                                if (value != null) prop.SetValue(control, value, null);
                            }
                            catch (MethodAccessException ne)
                            {
                                Debug.WriteLine(ne.Message + Environment.NewLine + ne.StackTrace);
                            }
                            catch (ArgumentException ae)
                            {
                                Debug.WriteLine(ae.Message + Environment.NewLine + ae.StackTrace);
                            }
                            catch (TargetException te)
                            {
                                Debug.WriteLine(te.Message + Environment.NewLine + te.StackTrace);
                            }
                            catch (TargetParameterCountException pe)
                            {
                                Debug.WriteLine(pe.Message + Environment.NewLine + pe.StackTrace);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves property info from custom control type.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>Property info object or NULL.</returns>
        private PropertyInfo GetProperty(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            PropertyInfo result = null;

            if (m_controlType != null)
            {
                try
                {
                    result = m_controlType.GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                }
                catch (AmbiguousMatchException ae)
                {
                    Debug.WriteLine(ae.Message + Environment.NewLine + ae.StackTrace);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns short name of the assembly.
        /// </summary>
        /// <param name="asm">Assembly object.</param>
        /// <returns>Short name of the assembly.</returns>
        private string GetAssemblyName(Assembly asm)
        {
            if (asm == null)
                throw new ArgumentNullException("asm");

            string asmName = asm.FullName;
            int index = asmName.IndexOf(',');
            asmName = asmName.Substring(0, index);

            return asmName;
        }
        #endregion
    }
}
