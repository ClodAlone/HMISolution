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
using System.Runtime.Serialization;

namespace Syncfusion.JavaScript.Mobile
{
    [DataContract]
    public enum FooterPosition
    {
        [EnumMember(Value = "fixed")]
        Fixed,
        [EnumMember(Value = "normal")]
        Normal
    }

    public class Footer : Control
    {
        #region Fields

        private string pluginString = "data-ej-";
        
        /// <summary>
        /// Gets or sets the m Footer model.
        /// </summary>
        /// <value>
        /// The m Footer model.
        /// </value>
        public MobileFooterProperties MobileFooterModel
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
            get { return "ejmFooter"; }
        }
        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        protected override object Model
        {
            get { return this.MobileFooterModel; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MFooter"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="propModel">The property model.</param>
        public Footer(String id, MobileFooterProperties propModel)
        {
            this.ID = id;
            this.MobileFooterModel = propModel;
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
            StringBuilder tag = new StringBuilder();

            tag.Append("<")
               .Append(TagName)
               .Append(" id=\"")
               .Append(controlId + "\"")
               .Append("></")
               .Append(TagName)
               .Append(">");
            return new HtmlString(String.Format(tag.ToString()));
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
            return new HtmlString(container.ToString());
        }

        #endregion
    }
}
