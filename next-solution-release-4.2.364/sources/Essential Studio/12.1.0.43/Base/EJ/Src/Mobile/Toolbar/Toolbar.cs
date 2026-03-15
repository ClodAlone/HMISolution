#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using Syncfusion.JavaScript.Mobile.Models;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript.Mobile
{
    public class Toolbar : Control
    {
        #region fields

        private string pluginString = "data-ej-";


        /// <summary>
        /// Gets or sets the mobile toolbar model.
        /// </summary>
        /// <value>
        /// The mobile toolbar model.
        /// </value>
        public MobileToolbarProperties MobileToolbarModel
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
            get { return "ejmToolbar"; }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        protected override object Model
        {
            get { return this.MobileToolbarModel; }
        }

        #endregion

        #region Contructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Toolbar"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="propModel">The property model.</param>
        public Toolbar(string id, MobileToolbarProperties propModel)
        {
            this.ID = id;
            this.MobileToolbarModel = propModel;
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
            container.Add(RenderItemContainer(this.MobileToolbarModel.Items));
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
            container.Add(RenderItemContainer(this.MobileToolbarModel.Items));
            return new HtmlString(container.ToString());
        }

        /// <summary>
        /// Renders the item container.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        internal HtmlTag RenderItemContainer(List<MobileToolbarBaseItem> items)
        {
            HtmlTag tag = new HtmlTag("ul");
            foreach (MobileToolbarBaseItem item in items)
            {
                HtmlTag liTag = new HtmlTag("li");
                liTag.Attributes(CreateUnObtrusiveDataDictionary(item, this.ID, pluginString)); 
                tag.Add(liTag);
            }
            return tag;
        }

        #endregion
    }

    [DataContract]
    public enum ToolbarPosition
    {
        [EnumMember(Value = "normal")]
        Normal,
        [EnumMember(Value = "fixed")]
        Fixed
    }
}
