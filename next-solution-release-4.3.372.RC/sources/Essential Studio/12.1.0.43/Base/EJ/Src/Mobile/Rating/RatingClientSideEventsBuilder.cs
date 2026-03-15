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
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    /// <summary>
    /// Class for Rating clientSide Events
    /// </summary>
    public class MobileRatingClientSideEventsBuilder
    {
        #region Fields

        private MobileRatingProperties mRatingModel;
        #endregion

        #region Constructor


        /// <summary>
        /// Initializes a new instance of the <see cref="MobileRatingClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mRatingModel">The m rating model.</param>
        public MobileRatingClientSideEventsBuilder(MobileRatingProperties mRatingModel)
        {
            this.mRatingModel = mRatingModel;
        }
        #endregion

        #region Events


        /// <summary>
        /// Clicks the specified click.
        /// </summary>
        /// <param name="click">The click.</param>
        /// <returns></returns>
        public MobileRatingClientSideEventsBuilder Click(string click)
        {
            mRatingModel.Click = click;
            return this;
        }


        /// <summary>
        /// Moves the specified move.
        /// </summary>
        /// <param name="move">The move.</param>
        /// <returns></returns>
        public MobileRatingClientSideEventsBuilder Move(string move)
        {
            mRatingModel.Move = move;
            return this;
        }


        /// <summary>
        /// Completes the specified value changed.
        /// </summary>
        /// <param name="valueChanged">The value changed.</param>
        /// <returns></returns>
        public MobileRatingClientSideEventsBuilder ValueChanged(string valueChanged)
        {
            mRatingModel.ValueChanged = valueChanged;
            return this;
        }
        #endregion
    }
}
