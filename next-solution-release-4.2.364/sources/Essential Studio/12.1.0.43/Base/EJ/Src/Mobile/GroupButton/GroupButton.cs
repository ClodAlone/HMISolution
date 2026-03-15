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
    public class MobileGroupButton : Control
    {
        #region Fields

        private string pluginString = "data-ej-";

        /// <summary>
        /// Gets or sets the mobile group button model.
        /// </summary>
        /// <value>
        /// The mobile group button model.
        /// </value>
        public MobileGroupButtonProperties MobileGroupButtonModel
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
            get { return "ejmGroupButton"; }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        protected override object Model
        {
            get { return this.MobileGroupButtonModel; }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileGroupButton"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="propModel">The property model.</param>
        public MobileGroupButton(String id, MobileGroupButtonProperties propModel)
        {
            this.ID = id;
            this.MobileGroupButtonModel = propModel;
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
            container.Attributes("data-role", PluginName.ToLower());
            container.Attributes("data-ej-theme", this.MobileGroupButtonModel.Theme.ToString().ToLower());
            if (!string.IsNullOrEmpty(this.MobileGroupButtonModel.touchStart))
                container.Attributes("data-ej-touchstart", this.MobileGroupButtonModel.touchStart.ToString().ToLower());
            if (!string.IsNullOrEmpty(this.MobileGroupButtonModel.touchEnd))
                container.Attributes("data-ej-touchend", this.MobileGroupButtonModel.touchEnd.ToString().ToLower()); 
            foreach (MobileGroupButtonBaseItem item in this.MobileGroupButtonModel.Buttons)
            {
                HtmlTag tag = new HtmlTag("label");
                tag.Attributes(CreateUnObtrusiveDataDictionary(item, this.ID, pluginString)); 
                HtmlTag input = new HtmlTag("input", TagRenderMode.SelfClosing);
                input.Attributes("type", this.MobileGroupButtonModel.GroupButtonType.ToString());
                if (!string.IsNullOrEmpty(this.MobileGroupButtonModel.Name))
                    input.Attributes("name", this.MobileGroupButtonModel.Name);
                if (!string.IsNullOrEmpty(item.Text))
                    tag.Html(item.Text + input.ToString());
                container.Add(tag);
            }
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
            container.Attributes("id", ID);
            container.Attributes("data-role", PluginName.ToLower());
            container.Attributes(CreateUnObtrusiveDataDictionary(this.Model, controlId, pluginString));
            container.Attributes("data-ej-theme", this.MobileGroupButtonModel.Theme.ToString().ToLower());
            if (!string.IsNullOrEmpty(this.MobileGroupButtonModel.touchStart))
                container.Attributes("data-ej-touchstart", this.MobileGroupButtonModel.touchStart.ToString().ToLower());
            if (!string.IsNullOrEmpty(this.MobileGroupButtonModel.touchEnd))
                container.Attributes("data-ej-touchend", this.MobileGroupButtonModel.touchEnd.ToString().ToLower());
            foreach (MobileGroupButtonBaseItem item in this.MobileGroupButtonModel.Buttons)
            {
                HtmlTag tag = new HtmlTag("label");
                tag.Attributes(CreateUnObtrusiveDataDictionary(item, this.ID, pluginString)); 
                HtmlTag input = new HtmlTag("input",TagRenderMode.SelfClosing);
                input.Attributes("type", this.MobileGroupButtonModel.GroupButtonType.ToString());
                if (!string.IsNullOrEmpty(this.MobileGroupButtonModel.Name))
                    input.Attributes("name", this.MobileGroupButtonModel.Name);
                if (!string.IsNullOrEmpty(item.Text))
                    tag.Html(item.Text+input.ToString());
                container.Add(tag);
            }
            return new HtmlString(container.ToString());
        }
        #endregion

    }
}
