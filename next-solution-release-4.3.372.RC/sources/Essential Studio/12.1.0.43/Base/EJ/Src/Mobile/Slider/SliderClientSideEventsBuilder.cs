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
    /// Class for Slider clientSide Events
    /// </summary>
    public class MobileSliderClientSideEventsBuilder
    {
        #region Fields

        /// <summary>
        /// The m slider model
        /// </summary>
        private MobileSliderProperties mSliderModel;
        #endregion

        #region Constructor



        /// <summary>
        /// Initializes a new instance of the <see cref="MobileSliderClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mSliderModel">The m slider model.</param>
        public MobileSliderClientSideEventsBuilder(MobileSliderProperties mSliderModel)
        {
            this.mSliderModel = mSliderModel;
        }
        #endregion

        #region Events


        /// <summary>
        /// Starts the specified start.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public MobileSliderClientSideEventsBuilder Start(string start)
        {
            mSliderModel.Start = start;
            return this;
        }



        /// <summary>
        /// Stops the specified stop.
        /// </summary>
        /// <param name="stop">The stop.</param>
        /// <returns></returns>
        public MobileSliderClientSideEventsBuilder Stop(string stop)
        {
            mSliderModel.Stop = stop;
            return this;
        }



        /// <summary>
        /// Changes the specified change.
        /// </summary>
        /// <param name="change">The change.</param>
        /// <returns></returns>
        public MobileSliderClientSideEventsBuilder Change(string change)
        {
            mSliderModel.Change = change;
            return this;
        }

        /// <summary>
        /// Slides the specified slide.
        /// </summary>
        /// <param name="slide">The slide.</param>
        /// <returns></returns>
        public MobileSliderClientSideEventsBuilder Slide(string slide)
        {
            mSliderModel.Slide = slide;
            return this;
        }


        /// <summary>
        /// Loads the specified load.
        /// </summary>
        /// <param name="load">The load.</param>
        /// <returns></returns>
        public MobileSliderClientSideEventsBuilder Load(string load)
        {
            mSliderModel.Load = load;
            return this;
        }

        #endregion
    }
}
