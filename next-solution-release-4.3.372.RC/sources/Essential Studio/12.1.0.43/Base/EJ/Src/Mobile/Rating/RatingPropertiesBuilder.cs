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
using System.Data;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Mobile;
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    /// <summary>
    /// Class for Rating Property Builder
    /// </summary>
    public class MobileRatingPropertiesBuilder
    {
        #region Fields
        private Rating mRating;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileRatingPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mRating">The m rating.</param>
        public MobileRatingPropertiesBuilder(Rating mRating)
        {
            this.mRating = new Rating(mRating.ID, mRating.MRatingModel);

        }
        #endregion

        #region Builder



        /// <summary>
        /// Renders the mode.
        /// </summary>
        /// <param name="renderMode">The render mode.</param>
        /// <returns></returns>
        public MobileRatingPropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mRating.MRatingModel.RenderMode = renderMode;
            return this;
        }



        /// <summary>
        /// Themes the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public MobileRatingPropertiesBuilder Theme(Theme theme)
        {
            mRating.MRatingModel.Theme = theme;
            return this;
        }



        /// <summary>
        /// Currents the value.
        /// </summary>
        /// <param name="currentValue">The current value.</param>
        /// <returns></returns>
        public MobileRatingPropertiesBuilder CurrentValue(int currentValue)
        {
            mRating.MRatingModel.CurrentValue = currentValue;
            return this;
        }



        /// <summary>
        /// Minimums the value.
        /// </summary>
        /// <param name="minValue">The minimum value.</param>
        /// <returns></returns>
        public MobileRatingPropertiesBuilder MinValue(int minValue)
        {
            mRating.MRatingModel.MinValue = minValue;
            return this;
        }



        /// <summary>
        /// Maximums the value.
        /// </summary>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns></returns>
        public MobileRatingPropertiesBuilder MaxValue(int maxValue )
        {
            mRating.MRatingModel.MaxValue = maxValue;
            return this;
        }



        /// <summary>
        /// Increments the step.
        /// </summary>
        /// <param name="incrementStep">The increment step.</param>
        /// <returns></returns>
        public MobileRatingPropertiesBuilder IncrementStep(double incrementStep)
        {
            mRating.MRatingModel.IncrementStep = incrementStep;
            return this;
        }


        /// <summary>
        /// Shapes the width.
        /// </summary>
        /// <param name="shapeWidth">Width of the shape.</param>
        /// <returns></returns>
        public MobileRatingPropertiesBuilder ShapeWidth(int shapeWidth)
        {
            mRating.MRatingModel.ShapeWidth = shapeWidth;
            return this;
        }


        /// <summary>
        /// Shapes the height.
        /// </summary>
        /// <param name="shapeHeight">Height of the shape.</param>
        /// <returns></returns>
        public MobileRatingPropertiesBuilder ShapeHeight(int shapeHeight)
        {
            mRating.MRatingModel.ShapeHeight = shapeHeight;
            return this;
        }


        /// <summary>
        /// Spaces the between shapes.
        /// </summary>
        /// <param name="spaceBetweenShapes">The space between shapes.</param>
        /// <returns></returns>
        public MobileRatingPropertiesBuilder SpaceBetweenShapes(int spaceBetweenShapes)
        {
            mRating.MRatingModel.SpaceBetweenShapes = spaceBetweenShapes;
            return this;
        }


        /// <summary>
        /// Reads the only.
        /// </summary>
        /// <param name="readOnly">if set to <c>true</c> [read only].</param>
        /// <returns></returns>
        public MobileRatingPropertiesBuilder ReadOnly(bool readOnly)
        {
            mRating.MRatingModel.ReadOnly = readOnly;
            return this;
        }


        /// <summary>
        /// Orientations the specified orientation.
        /// </summary>
        /// <param name="_orientation">The orientation.</param>
        /// <returns></returns>
        public MobileRatingPropertiesBuilder Orientation(Orientation orientation)
        {
            mRating.MRatingModel.Orientation = orientation;
            return this;
        }

        /// <summary>
        /// Precisions the specified precisison.
        /// </summary>
        /// <param name="precisison">The precisison.</param>
        /// <returns></returns>
        public MobileRatingPropertiesBuilder Precision(Precisions precisison)
        {
            mRating.MRatingModel.Precision = precisison;
            return this;
        }

        /// <summary>
        /// Shapes the specified shape.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <returns></returns>
        public MobileRatingPropertiesBuilder Shape(Shape shape)
        {
            mRating.MRatingModel.Shape = shape;
            return this;
        }

        /// <summary>
        /// Enableds the specified enabled.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <returns></returns>
        public MobileRatingPropertiesBuilder Enabled(bool enabled)
        {
            mRating.MRatingModel.Enabled = enabled;
            return this;
        }


        /// <summary>
        /// Clients the side events.
        /// </summary>
        /// <param name="clientSideEvents">The client side events.</param>
        /// <returns></returns>
        public MobileRatingPropertiesBuilder ClientSideEvents(Action<MobileRatingClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MobileRatingClientSideEventsBuilder(this.mRating.MRatingModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }

        #endregion

        #region Render


        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns></returns>
        public HtmlString Render()
        {
            return new HtmlString(mRating.Render().ToString());
        }


        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override String ToString()
        {
            return Render().ToString();
        }
        #endregion
    }
}
