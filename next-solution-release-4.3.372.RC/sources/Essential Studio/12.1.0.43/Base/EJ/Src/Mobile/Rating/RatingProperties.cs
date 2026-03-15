#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Mobile;
using System.Runtime.Serialization;

namespace Syncfusion.JavaScript.Mobile.Models
{

    /// <summary>
    /// Class for Rating Properties 
    /// </summary>
    public class MobileRatingProperties : RatingPropertiesBase, IMobileBase 
    {
        #region Fields

        private RenderMode renderMode = RenderMode.Auto;
        private Theme theme = Theme.Auto;
        private double incrementStep = 1.0;
        private Shape shape = Shape.Star;
        private int spaceBetweenShapes = 15;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the click.
        /// </summary>
        /// <value>
        /// The click.
        /// </value>
        [JsonProperty("click")]
        public string Click { get; set; }



        /// <summary>
        /// Gets or sets the value changed.
        /// </summary>
        /// <value>
        /// The value changed.
        /// </value>
        [JsonProperty("valueChanged")]
        public string ValueChanged { get; set; }



        /// <summary>
        /// Gets or sets the increment step.
        /// </summary>
        /// <value>
        /// The increment step.
        /// </value>
        [JsonProperty("incrementStep")]
        [DefaultValue(1.0)]
        public double IncrementStep { get { return incrementStep; } set { incrementStep = value; } }




        /// <summary>
        /// Gets or sets the space between shapes.
        /// </summary>
        /// <value>
        /// The space between shapes.
        /// </value>
        [JsonProperty("spaceBetweenShapes")]
        [DefaultValue(15)]
        public int SpaceBetweenShapes { get { return spaceBetweenShapes; } set { spaceBetweenShapes = value; } }




        /// <summary>
        /// Gets or sets the move.
        /// </summary>
        /// <value>
        /// The move.
        /// </value>
        [JsonProperty("move")]
        [DefaultValue("")]
        public string Move { get; set; }



        /// <summary>
        /// Gets or sets the render mode.
        /// </summary>
        /// <value>
        /// The render mode.
        /// </value>
        [JsonProperty("renderMode")]
        [DefaultValue(RenderMode.Auto)]
        public RenderMode RenderMode { get { return renderMode; } set { renderMode = value; } }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>
        /// The theme.
        /// </value>
        [JsonProperty("theme")]
        [DefaultValue(Theme.Auto)]
        public Theme Theme { get { return theme; } set { theme = value; } }



        /// <summary>
        /// Gets or sets the shape.
        /// </summary>
        /// <value>
        /// The shape.
        /// </value>
        [JsonProperty("shape")]
        [DefaultValue(Shape.Star)]
        public Shape Shape { get { return shape; } set { shape = value; } }

        #endregion

        #region Constructor

        #endregion

    }


}
