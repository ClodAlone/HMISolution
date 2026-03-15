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
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript.Models
{
    public class Position
    {
        #region Fields
        private String X = null;
        private String Y = null;
        #endregion

        #region Properties
        [JsonProperty("X")]
        [DefaultValue(null)]
        public String XValue
        {
            get { return this.X; }
            set { this.X = value; }
        }
        [JsonProperty("Y")]
        [DefaultValue(null)]
        public String YValue
        {
            get { return this.Y; }
            set { this.Y = value; }
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript
{
   

    public class PositionBuilder
    {
        private Position position = new Position();
        public PositionBuilder(Position position)
        {
            this.position = position;
        }
        public PositionBuilder XValue(String X)
        {
            this.position.XValue = X;
            return this;
        }
        public PositionBuilder YValue(String Y)
        {
            this.position.YValue = Y;
            return this;
        }
      
    }
}
