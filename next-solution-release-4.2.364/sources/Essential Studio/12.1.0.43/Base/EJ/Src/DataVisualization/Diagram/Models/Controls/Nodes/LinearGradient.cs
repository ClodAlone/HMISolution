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
using Syncfusion.JavaScript.DataVisualization.Models.Collections;

namespace Syncfusion.JavaScript.DataVisualization.Models.Controls
{
    /// <summary>
    /// The linear gradient class
    /// </summary>
    public class LinearGradient : ICloneable
    {

        #region Members
        private Collection _stops;
        private double _dX1;
        private double _dX2;
        private double _dY1;
        private double _dY2;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="LinearGradient"/> class.
        /// </summary>
        public LinearGradient()
        {
            _dX1 = 0;
            _dY1 = 0;
            _dX2 = 0;
            _dY2 = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearGradient"/> class.
        /// </summary>
        /// <param name="src">Object to copy.</param>
        public LinearGradient(LinearGradient src)
        {
            this.Stops = src.Stops;
            this.X1 = src.X1;
            this.X2 = src.X2;
            this.Y1 = src.Y1;
            this.Y2 = src.Y2;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the stops of linear gradient.
        /// </summary>
        public Collection Stops
        {
            get
            {
                if (_stops == null)
                    _stops = new Collection();
                return _stops;
            }
            set
            {
                if (_stops != value)
                    _stops = value;
            }
        }

        /// <summary>
        /// Gets or sets the x1 value of gradient vector.
        /// </summary>
        public double X1
        {
            get { return _dX1; }
            set
            {
                if (_dX1 != value)
                    _dX1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the x2 value of gradient vector.
        /// </summary>
        public double X2
        {
            get { return _dX2; }
            set
            {
                if (_dX2 != value)
                    _dX2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the y1 value of gradient vector.
        /// </summary>
        public double Y1
        {
            get { return _dY1; }
            set
            {
                if (_dY1 != value)
                    _dY1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the y2 value of gradient vector.
        /// </summary>
        public double Y2
        {
            get { return _dY2; }
            set
            {
                if (_dY2 != value)
                    _dY2 = value;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>Copy of the object this method is invoked against.</returns>
        public object Clone()
        {
            return new LinearGradient(this);
        }
        #endregion

         
    }

    /// <summary>
    /// The stop class for gradient
    /// </summary>
    [Serializable]
    public class Stop : ICloneable
    {
        #region Members
        private string _strColor;
        private double _dOffset;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="Stop"/> class.
        /// </summary>
        public Stop()
        {
            _strColor = "";
            _dOffset = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Stop"/> class.
        /// </summary>
        /// <param name="src">Object to copy.</param>
        public Stop(Stop src)
        {
            this.Color = src.Color;
            this.Offset = src.Offset;
        }

         
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the stop color.
        /// </summary>
        public string Color
        {
            get { return _strColor; }
            set
            {
                if (_strColor != value)
                    _strColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the stop offset.
        /// </summary>
        public double Offset
        {
            get { return _dOffset; }
            set
            {
                if (_dOffset != value)
                    _dOffset = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>Copy of the object this method is invoked against.</returns>
        public object Clone()
        {
            return new Stop(this);
        }
        #endregion

        
    }
}
