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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Syncfusion.Windows.Forms.HTMLUI;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Designer class of control.
    /// </summary>
    public sealed class HTMLUIControlDesigner : ParentControlDesigner
    {
        #region Class members
        /// <summary>
        /// DesignerVerbCollection instance
        /// </summary>
        private DesignerVerbCollection m_verbs;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets a value indicating DesignerVerbCollection instance
        /// </summary>
        public override DesignerVerbCollection Verbs
        {
            get
            {
                return m_verbs;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the HTMLUIControlDesigner class
        /// </summary>
        public HTMLUIControlDesigner()
        {
            m_verbs = new DesignerVerbCollection(new DesignerVerb[]
                {
                  new DesignerVerb( "Load from file...", new EventHandler( LoadDocumentFromFile ) )
                });
        }

#if SyncfusionFramework2_0
        /// <summary>
        /// v
        /// </summary>
        public DesignerActionListCollection Action_Lists;
        /// <summary>
        /// overriding DesignerActionListCollection ActionLists
        /// </summary>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == Action_Lists)
                {
                    Action_Lists = new DesignerActionListCollection();
                    Action_Lists.Add(new HTMLUIActionList(this.Component));
                }
                return Action_Lists;
            }
        }
#endif
        /// <summary>
        /// Finalizes an instance of the HTMLUIControlDesigner class
        /// </summary>
        ~HTMLUIControlDesigner()
        {
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Loads the document from the file.
        /// </summary>
        /// <param name="sender">Sender of event.</param>
        /// <param name="e">Event arguments.</param>
        private void LoadDocumentFromFile(object sender, EventArgs e)
        {
            HTMLUIControl html = (HTMLUIControl)this.Control;

            string oldStartupDoc = html.StartupDocument;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Select file for loading";
            dlg.DefaultExt = "htm";
            dlg.Filter = "HTML Files (*.htm;*.html)|*.htm;*.html|All Files (*.*)|*.*";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                html.StartupDocument = dlg.FileName;
            }

            string newStartupDoc = html.StartupDocument;
            this.RaiseComponentChanged(TypeDescriptor.GetProperties(html)["StartupDocument"], oldStartupDoc, newStartupDoc);
            html.RecalculateDocument();
        }
        #endregion
            
        #region Class overrides
        /// <summary>
        /// Overridden. Adjusts the set of properties the component will expose through a TypeDescriptor.
        /// </summary>
        /// <param name="properties">An IDictionary that contains the properties for the class of the component. </param>
        protected override void PreFilterProperties(IDictionary properties)
        {
            this.DrawGrid = false;

            base.PreFilterProperties(properties);
        }
        #endregion
    }
}