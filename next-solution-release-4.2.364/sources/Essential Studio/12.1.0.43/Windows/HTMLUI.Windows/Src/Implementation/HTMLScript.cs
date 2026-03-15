#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Reflection;
using System.Runtime.Serialization;
using Syncfusion.Scripting;
using Syncfusion.Windows.Forms.HTMLUI;

#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Inherits Script class. The Script class encapsulates the script source
    ///  code, the reference and global items (the IVsaItem instances)
    ///  that the .NET script engine expects to be initialized with.
    /// </summary>
    [Serializable]
    public class HTMLScript
      : Script, ISerializable
    {
        #region Class constants
        /// <summary>
        /// Name of the document variable in scripts.
        /// </summary>
        internal const string DEF_VAR_NAME = "Document";

        /// <summary>
        /// Namespace of the document instance.
        /// </summary>
        private const string DEF_DOC_NAMESPACE = "Syncfusion.Windows.Forms.HTMLUI.IInputHTML";

        /// <summary>
        /// List of assemblies attached automatically to the engine on execution.
        /// </summary>
        public static readonly string[] DEF_ASSEMBLIES = new string[]
    {
      "System.IO",
      "System.Xml",
      "System.Drawing",
      "System.Collections",
      AxecAsmName
    };
        #endregion

        #region Class members
        /// <summary>
        /// Element-owner of current script.
        /// </summary>
        private BaseElement m_element;

        /// <summary>
        /// Indicates whether source was changed.
        /// </summary>
        private bool m_bSourceChanged;
        #endregion

        #region Class static properties
        /// <summary>
        /// Gets the name of the executing assembly.
        /// </summary>
        private static string AxecAsmName
        {
            get
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                string asmName = asm.FullName;
                int index = asmName.IndexOf(',');
                asmName = asmName.Substring(0, index);

                return asmName;
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the owner of the current script.
        /// </summary>
        internal BaseElement Owner
        {
            get
            {
                return m_element;
            }
            set
            {
                if (m_element == null)
                {
                    m_element = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the source text was changed.
        /// </summary>
        internal bool SourceChanged
        {
            get
            {
                return m_bSourceChanged;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the HTMLScript class
        /// </summary>
        public HTMLScript()
        {
            //// Add references to needed assembiles.
            foreach (string assembly in DEF_ASSEMBLIES)
            {
                AddReference(new AssemblyDescriptor(assembly, String.Empty, String.Empty));
            }

            //// Add global event sources which will be visible
            //// from the script and
            //// accessed from GetEventSourceInstance method.
            ////AddEventSource( DEF_VAR_NAME, DEF_DOC_NAMESPACE );

            //// Add global variables which will be visible
            //// from the script and
            //// accessed from GetGlobalInstance method.
            AddGlobalInstance(DEF_VAR_NAME, DEF_DOC_NAMESPACE);
        }

        /// <summary>
        /// Initializes a new instance of the HTMLScript class
        /// </summary>
        /// <param name="element">Owner of the script code.</param>
        public HTMLScript(BaseElement element)
            : this()
        {
            if (element == null)
                throw new ArgumentNullException("element");

            m_element = element;
        }

        /// <summary>
        /// Initializes a new instance of the HTMLScript class
        /// </summary>
        /// <param name="info">Holds all data for serialization / deserialization object.</param>
        /// <param name="context">Describes source and destination of serialization.</param>
        protected HTMLScript(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Overridden. Triggers when the source is changed.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        protected override void OnSourceTextChanged(ScriptEventArgs evtArgs)
        {
            m_bSourceChanged = true;
            base.OnSourceTextChanged(evtArgs);
            m_bSourceChanged = false;
        }
        #endregion
    }
}
