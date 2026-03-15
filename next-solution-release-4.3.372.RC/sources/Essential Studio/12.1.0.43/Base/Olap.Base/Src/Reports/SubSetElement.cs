//-------------------------------------------------------------------------------------------------
// <copyright file="SubSetElement.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;


#if !SILVERLIGHT
using Syncfusion.Olap.Common;

namespace Syncfusion.Olap.Reports
#else
using Syncfusion.OlapSilverlight.Common;

namespace Syncfusion.OlapSilverlight.Reports
    
#endif
{
#if !SILVERLIGHT
    /// <summary>
    /// Represents the sub set element information.
    /// </summary>
    [Serializable]
    public class SubsetElement : Element, ICloneable<SubsetElement>
#else
    /// <summary>
    /// Represents the sub set element information.
    /// </summary>
    public class SubsetElement : Element
#endif
    {
        private int _startIndex;
        private int _endIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubsetElement"/> class.
        /// </summary>
        public SubsetElement()
        {
            this.StartIndex = 0;
            this.EndIndex = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubsetElement"/> class.
        /// </summary>
        /// <param name="endIndex">The end index.</param>
        public SubsetElement(int endIndex)
        {
            this.StartIndex = 0;
            this.EndIndex = endIndex;
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubsetElement"/> class.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        public SubsetElement(int startIndex, int endIndex)
        {
            this.StartIndex = startIndex;
            this.EndIndex = endIndex;
        }

        #region Public Properties
        /// <summary>
        /// Gets or sets the axis.
        /// </summary>
        /// <value>The axis.</value>
        internal AxisPosition Axis { get; set; }

        /// <summary>
        /// Gets or sets the start index.
        /// </summary>
        /// <value>The start index.</value>
        public int StartIndex
        {
            get
            {
                return _startIndex;
            }

            set
            {
                if (value < 0)
                {
                    throw new Exception("Start Index of SubsetElement should be greater than zero");
                }

                _startIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the end index.
        /// </summary>
        /// <value>The end index.</value>
        public int EndIndex
        {
            get
            {
                return _endIndex;
            }

            set
            {
                if (value < 0)
                {
                    throw new Exception("End Index of SubsetElement should be greater than zero");
                }

                _endIndex = value;
            }
        }
        #endregion

#if !SILVERLIGHT
        #region Public Methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of <see cref="SubsetElement"/>.</returns>
        public new SubsetElement Clone()
        {
            SubsetElement subsetElement = new SubsetElement();
            subsetElement.Axis = this.Axis;
            subsetElement.StartIndex = this.StartIndex;
            subsetElement.EndIndex = this.EndIndex;
            return subsetElement;
        }

        #endregion
#endif
    }
}
