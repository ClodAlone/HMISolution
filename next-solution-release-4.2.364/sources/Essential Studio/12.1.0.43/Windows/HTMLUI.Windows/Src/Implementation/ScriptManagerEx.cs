#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms.Design;
using System.Xml;
using Microsoft.Vsa;
using Syncfusion.HTMLUI.Base.Utility;
using Syncfusion.Scripting;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that controls one block of script.
    /// </summary>
    [ToolboxItem(false)]
    public sealed class ScriptManagerEx
      : ScriptingManager, IDisposable
    {
        #region Class constants
        /// <summary>
        /// Name of the class after script compiling.
        /// </summary>
        internal const string DEF_CLASS_NAME = "Script";

        /// <summary>
        /// Config file extension.
        /// </summary>
        private const string DEF_CONFIG_NAME = ".config.xml";

        /// <summary>
        /// Category at the designer.
        /// </summary>
        private const string DEF_SCRIPT_CATEGORY = "Scripting";
        #endregion

        #region Class members
        /// <summary>
        /// Indicates whether script must run just after compilation.
        /// </summary>
        private bool m_autoRun;

        /// <summary>
        /// Indicates whether script source is embedded or a separate file.
        /// </summary>
        private bool m_isEmbeded;

        /// <summary>
        /// Path to file with script code.
        /// </summary>
        private string m_fileName;

        /// <summary>
        /// Type object for ScriptLanguage enum type.
        /// </summary>
        private Type m_langType;

        /// <summary>
        /// Indicates whether script code is from external file.
        /// </summary>
        private bool m_bFromFile;

        /// <summary>
        /// Indicates whether the object is in Quite mode.
        /// </summary>
        private bool m_bQuietMode;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets a value indicating whether the script must be run just after compilation.
        /// </summary>
        [Browsable(true), Category(DEF_SCRIPT_CATEGORY), DefaultValue(false), Description("Indicates if script must run just after compilation.")]
        public bool AutoRun
        {
            get
            {
                return m_autoRun;
            }
            set
            {
                if (m_autoRun != value)
                {
                    m_autoRun = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the script source is embedded or a separate file.
        /// </summary>
        [Browsable(true), Category(DEF_SCRIPT_CATEGORY), DefaultValue(true), Description("Indicates if script source is embedded or in separate file.")]
        public bool IsEmbeded
        {
            get
            {
                return m_isEmbeded;
            }
            set
            {
                if (m_isEmbeded != value)
                {
                    m_isEmbeded = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the path to file with script code.
        /// </summary>
        [Browsable(true), Category(DEF_SCRIPT_CATEGORY), DefaultValue(""), Editor(typeof(FileNameEditor), typeof(UITypeEditor)), Description("Indicates path to file with script code.")]
        public string File
        {
            get
            {
                return m_fileName;
            }
            set
            {
                if (m_fileName != value)
                {
                    m_fileName = LoadFromFile(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Quiet mode for the object.
        /// </summary>
        [Browsable(false)]
        public bool QuietMode
        {
            get
            {
                return m_bQuietMode;
            }
            set
            {
                if (m_bQuietMode != value)
                {
                    m_bQuietMode = value;
                }
            }
        }

        /// <summary>
        /// Gets the type of the ScriptLanguage class.
        /// </summary>
        internal Type LangType
        {
            get
            {
                return m_langType;
            }
        }

        /// <summary>
        /// Sets the path to the script file.
        /// </summary>
        internal string Path
        {
            set
            {
                if (m_fileName != value)
                {
                    m_fileName = value;
                    m_bFromFile = true;
                }
            }
        }
#pragma warning disable
        /// <summary>
        /// Gets the string representation of the active language.
        /// </summary>
        internal string LanguageStr
        {
            get
            {
                return this.Script.Language.ToString(CultureInfo.InvariantCulture);
            }
        }
        #endregion
#pragma warning enable
        #region Class Initialize/Finalize methods
        /// <summary>
        /// Prevents a default instance of the ScriptManagerEx class from being created
        /// </summary>
        private ScriptManagerEx()
            : base()
        {
            InitializeObject();
        }

        /// <summary>
        /// Initializes a new instance of the ScriptManagerEx class
        /// </summary>
        /// <param name="scriptSite">Script site object.</param>
        /// <param name="script">Owner of the current script.</param>
        public ScriptManagerEx(HTMLScriptSite scriptSite, HTMLScript script)
            : base(scriptSite, script)
        {
            script.Name = DEF_CLASS_NAME;
            InitializeObject();
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Compiles script and runs script if property 'AutoRun' is enabled.
        /// </summary>
        /// <returns>Returns True if script was successfully compiled; false otherwise.</returns>
        public bool Compile()
        {
            bool isCompiled = base.CompileScript();

            if (isCompiled && this.AutoRun)
            {
                base.RunScript();
            }

            return isCompiled;
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Overridden. Raised when script has been chnged.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        protected override void OnScriptChanged(ScriptEventArgs evtArgs)
        {
            base.OnScriptChanged(evtArgs);

            HTMLScript script = evtArgs.Script as HTMLScript;

            if (script.SourceChanged)
            {
                ScriptCodeChanged(this.Script.SourceText);
            }
        }
#pragma warning disable
        /// <summary>
        /// Overridden. Raises event that compiles error occured and infills collection of errors.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        protected override void OnCompileError(VsaErrorEventArgs evtArgs)
        {
            base.OnCompileError(evtArgs);

            IVsaError error = evtArgs.Error;

            string message = "Error at Line: " + error.Line +
              ", Column: (" + error.StartColumn + " " + error.EndColumn + " ); " +
              "Code: " + error.LineText + "; " +
              "Description: " + error.Description;
            (this.ScriptSite as HTMLScriptSite).Document.AddCompileError(message);
        }
        #endregion
#pragma warning enable
        #region Class utility methods
        /// <summary>
        /// Sets the script code to source of script.
        /// </summary>
        /// <param name="code">code source.</param>
        internal void ScriptCodeChanged(string code)
        {
            if (this.QuietMode) return;

            if (code == null)
                throw new ArgumentNullException("code");

            if (m_bFromFile || (m_fileName.Length > 0 && !this.IsEmbeded))
            {
                SaveToFile(code);
            }
            else
            {
                SaveToElement(code);
            }
        }

        /// <summary>
        /// Saves script code to file.
        /// </summary>
        /// <param name="code">Code source.</param>
        private void SaveToFile(string code)
        {
            if (code == null)
                throw new ArgumentNullException("code");

            if (code.Length == 0)
                throw new ArgumentException("code - string can not be empty");

            if (!(m_bFromFile || (m_fileName.Length > 0 && !this.IsEmbeded))) return;

            using (FileStream fs = new FileStream(m_fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                byte[] data = Encoding.UTF8.GetBytes(code);
                fs.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Saves script to script owner or to a new element.
        /// </summary>
        /// <param name="code">Code source.</param>
        private void SaveToElement(string code)
        {
            if (code == null)
                throw new ArgumentNullException("code");

            HTMLScript script = this.Script as HTMLScript;
            //// Save script to previous place.
            if (script != null && script.Owner.InnerHTML != code)
            {
                script.Owner.InnerHTML = code;
            }
        }

        /// <summary>
        /// Initializes an object.
        /// </summary>
        private void InitializeObject()
        {
            m_langType = typeof(ScriptLanguages);
            m_fileName = string.Empty;
            m_isEmbeded = true;
        }

        /// <summary>
        /// Loads script code from the file.
        /// </summary>
        /// <param name="path">Path to the file with source code.</param>
        /// <returns>Full path to the file if it exists; empty string otherwise.</returns>
        private string LoadFromFile(string path)
        {
            if (path == null || path.Length == 0) return string.Empty;

            HTMLScript scriptObj = this.Script as HTMLScript;

            if (scriptObj == null) return string.Empty;

            SCRIPTElementImpl elm = scriptObj.Owner as SCRIPTElementImpl;

            if (elm == null) 
                return string.Empty;

            string fullPath = string.Empty;

            if (Utilities.IsFileExists(scriptObj.Owner.Document.CurrentDirectory, path, out fullPath))
            {
                using (TextReader tr = new StreamReader(fullPath))
                {
                    scriptObj.SourceText = tr.ReadToEnd();
                }
                m_isEmbeded = false;
            }

            return fullPath;
        }
        #endregion
    }
}
