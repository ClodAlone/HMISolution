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
    /// <summary>
    /// Class for Rating Control
    /// </summary>
    public class Rating : Control
    {
        #region Fields

        private string pluginString = "data-ej-";


        /// <summary>
        /// Gets or sets the m rating model.
        /// </summary>
        /// <value>
        /// The m rating model.
        /// </value>
        public MobileRatingProperties MRatingModel
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
            get { return "ejmRating"; }
        }


        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        protected override object Model
        {
            get { return this.MRatingModel; }
        }

        #endregion

        #region Constructor


        /// <summary>
        /// Initializes a new instance of the <see cref="Rating"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="propModel">The property model.</param>
        public Rating(String id, MobileRatingProperties propModel)
        {
            this.ID = id;
            this.MRatingModel = propModel;
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
            return new HtmlString(container.ToString());
        }
        #endregion

    }

    /// <summary>
    /// Enumaration for Shape Property
    /// </summary>
    [DataContract]
    public enum Shape
    {
        [EnumMember(Value = "star")]
        Star,
        [EnumMember(Value = "heart")]
        Heart,
        [EnumMember(Value = "circle")]
        Circle,
        [EnumMember(Value = "diamond")]
        Diamond,
        [EnumMember(Value = "pentagon")]
        Pentagon,
        [EnumMember(Value = "square")]
        Square,
        [EnumMember(Value = "triangel")]
        Triangle
    }

}
