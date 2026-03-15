#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Mobile;
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    /// <summary>
    /// Class for SplitPane
    /// </summary>
    public class SplitPane : Control
    {
        #region Fields
        private string pluginString = "data-ej-";

        /// <summary>
        /// Gets or sets the mobile SplitPane model.
        /// </summary>
        /// <value>
        /// The mobile SplitPane model.
        /// </value>
        public MobileSplitPaneProperties MobileSplitPaneModel
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        /// <value>
        /// The name of the tag.
        /// </value>
        public override string TagName
        {
            get
            {
                return "div";
            }
        }
        /// <summary>
        /// Gets the name of the plugin.
        /// </summary>
        /// <value>
        /// The name of the plugin.
        /// </value>
        public override string PluginName
        {
            get { return "ejmSplitPane"; }
        }
        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        protected override object Model
        {
            get { return this.MobileSplitPaneModel; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SplitPane"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="propModel">The property model.</param>
        public SplitPane(String id, MobileSplitPaneProperties propModel)
        {
            this.ID = id;
            this.MobileSplitPaneModel = propModel;
        }
        #endregion

        #region Render
        /// <summary>
        /// Creates the container.
        /// </summary>
        /// <param name="controlId">The control identifier.</param>
        /// <returns></returns>
        public override HtmlString CreateContainer(string controlId)
        {
            HtmlTag container = new HtmlTag(TagName);
            container.Attributes("id", ID);
            return new HtmlString(String.Format(container.ToString()));
        }

        /// <summary>
        /// Creates the un obtrusive container.
        /// </summary>
        /// <param name="controlId">The control identifier.</param>
        /// <returns></returns>
        public override HtmlString CreateUnObtrusiveContainer(string controlId)
        {
            HtmlTag container = new HtmlTag(TagName);
            container.Attributes(CreateUnObtrusiveDataDictionary(this.Model, controlId, pluginString));
            container.Attributes("id", ID);
            container.Attributes("data-role", PluginName.ToLower());
            HtmlTag leftPane = new HtmlTag(TagName).Attributes("data-ej-layout","pane");
            HtmlTag rightPane = new HtmlTag(TagName).Attributes("data-ej-layout", "pane");
            leftPane.Add(new HtmlTag(TagName));
            rightPane.Add(new HtmlTag(TagName));
            container.Add(leftPane).Add(rightPane);
            if (this.MobileSplitPaneModel.LeftPaneTemplate != null)
            {
                if (this.MobileSplitPaneModel.LeftPaneTemplate.WebFormDataTemplate != null || this.MobileSplitPaneModel.LeftPaneTemplate.RazorViewTemplate != null)
                    this.MobileSplitPaneModel.LeftPaneTemplate.builder(null, leftPane);
            }
            if (this.MobileSplitPaneModel.RightPaneTemplate != null)
            {
                if (this.MobileSplitPaneModel.RightPaneTemplate.WebFormDataTemplate != null || this.MobileSplitPaneModel.RightPaneTemplate.RazorViewTemplate != null)
                    this.MobileSplitPaneModel.RightPaneTemplate.builder(null, rightPane);
            }
            return new HtmlString(container.ToString());
        }
        #endregion
    }
}
