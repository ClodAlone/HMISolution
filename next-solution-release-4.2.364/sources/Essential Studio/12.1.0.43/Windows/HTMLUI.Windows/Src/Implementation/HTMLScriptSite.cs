#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;

using Syncfusion.Scripting;
using Syncfusion.Windows.Forms.HTMLUI;

#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Inherits ScriptSite class from Scripting for returning by application
    /// provided event source and global instance objects whenever the script
    /// engine calls for it through the equivalent IVsaSite method.
    /// </summary>
    public sealed class HTMLScriptSite : ScriptSite
    {
        #region Class members
        /// <summary>
        /// Control instance.
        /// </summary>
        private HTMLUIControl m_control;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the parent document for this script site.
        /// </summary>
        internal InputHTML Document
        {
            get
            {
                return m_control.ThreadDocument;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Prevents a default instance of the HTMLScriptSite class from being created. 
        /// </summary>
        private HTMLScriptSite()
        {
        }

        /// <summary>
        /// Initializes a new instance of the HTMLScriptSite class
        /// </summary>
        /// <param name="control">Control instance.</param>
        public HTMLScriptSite(HTMLUIControl control)
            : this()
        {
            if (control == null)
                throw new ArgumentNullException("control");

            m_control = control;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Overridden. Returns a reference to an event source previously added to a script engine using
        /// the IVsaCodeItem.AddEventSource method.
        /// </summary>
        /// <param name="itemName">Name of the object.</param>
        /// <param name="eventSourceName">Name of the variable, registered previously
        ///  by AddEventSource method.</param>
        /// <returns>Object from your host to user's script.</returns>
        public override object GetEventSourceInstance(string itemName, string eventSourceName)
        {
            if (eventSourceName.Equals(HTMLScript.DEF_VAR_NAME))
            {
                return m_control.ThreadDocument;
            }

            return null;
        }

        /// <summary>
        /// Overridden. Returns a reference to a global item such as the host-provided application object.
        /// </summary>
        /// <param name="globInstanceName">Name of the variable, registered previously
        ///  by AddGlobalInstance method.</param>
        /// <returns>Global object.</returns>
        public override object GetGlobalInstance(string globInstanceName)
        {
            if (globInstanceName.Equals(HTMLScript.DEF_VAR_NAME))
            {
                return m_control.ThreadDocument;
            }

            return null;
        }
        #endregion
    }
}
