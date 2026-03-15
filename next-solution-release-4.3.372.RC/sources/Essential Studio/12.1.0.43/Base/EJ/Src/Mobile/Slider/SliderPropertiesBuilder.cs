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
    /// Class for Slider Property Builder
    /// </summary>
    public class MobileSliderPropertiesBuilder
    {
        #region Fields
        private Slider mSlider;
        #endregion

        #region Constructor


        /// <summary>
        /// Initializes a new instance of the <see cref="MobileSliderPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mSlider">The m slider.</param>
        public MobileSliderPropertiesBuilder(Slider mSlider)
        {
            this.mSlider = new Slider(mSlider.ID, mSlider.MSliderModel);

        }
        #endregion

        #region Builder




        /// <summary>
        /// Renders the mode.
        /// </summary>
        /// <param name="renderMode">The render mode.</param>
        /// <returns></returns>
        public MobileSliderPropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mSlider.MSliderModel.RenderMode = renderMode;
            return this;
        }


        /// <summary>
        /// Themes the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public MobileSliderPropertiesBuilder Theme(Theme theme)
        {
            mSlider.MSliderModel.Theme = theme;
            return this;
        }


        /// <summary>
        /// Starts the value.
        /// </summary>
        /// <param name="startValue">The start value.</param>
        /// <returns></returns>
        public MobileSliderPropertiesBuilder StartValue(int startValue)
        {
            mSlider.MSliderModel.StartValue= startValue;
            return this;
        }

        /// <summary>
        /// Ends the value.
        /// </summary>
        /// <param name="endValue">The end value.</param>
        /// <returns></returns>
        public MobileSliderPropertiesBuilder EndValue(int endValue)
        {
            mSlider.MSliderModel.EndValue = endValue;
            return this;
        }

        /// <summary>
        /// CSSs the class.
        /// </summary>
        /// <param name="cssClass">The CSS class.</param>
        /// <returns></returns>
        public MobileSliderPropertiesBuilder CssClass(string cssClass)
        {
            mSlider.MSliderModel.CssClass = cssClass;
            return this;
        }


        /// <summary>
        /// Ranges the specified range.
        /// </summary>
        /// <param name="range">if set to <c>true</c> [range].</param>
        /// <returns></returns>
        public MobileSliderPropertiesBuilder Range(bool range )
        {
            mSlider.MSliderModel.Range = range;
            return this;
        }



        /// <summary>
        /// Animates the specified animate.
        /// </summary>
        /// <param name="animate">if set to <c>true</c> [animate].</param>
        /// <returns></returns>
        public MobileSliderPropertiesBuilder Animate(bool animate)
        {
            mSlider.MSliderModel.Animate = animate;
            return this;
        }


        /// <summary>
        /// Animations the speed.
        /// </summary>
        /// <param name="animationSpeed">The animation speed.</param>
        /// <returns></returns>
        public MobileSliderPropertiesBuilder AnimationSpeed(int animationSpeed)
        {
            mSlider.MSliderModel.AnimationSpeed = animationSpeed;
            return this;
        }


        /// <summary>
        /// Reads the only.
        /// </summary>
        /// <param name="readOnly">if set to <c>true</c> [read only].</param>
        /// <returns></returns>
        public MobileSliderPropertiesBuilder ReadOnly(bool readOnly)
        {
            mSlider.MSliderModel.ReadOnly = readOnly;
            return this;
        }


        /// <summary>
        /// Steps the specified step.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns></returns>
        public MobileSliderPropertiesBuilder Step(int step)
        {
            mSlider.MSliderModel.Step = step;
            return this;
        }

        /// <summary>
        /// Valueses the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public MobileSliderPropertiesBuilder Values(int[] values)
        {
            mSlider.MSliderModel.Values = values;
            return this;
        }


        /// <summary>
        /// Values the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public MobileSliderPropertiesBuilder Value(int value)
        {
            mSlider.MSliderModel.Value = value;
            return this;
        }

        /// <summary>
        /// Orientations the specified orientation.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        /// <returns></returns>
        public MobileSliderPropertiesBuilder Orientation(Orientation orientation)
        {
            mSlider.MSliderModel.Orientation = orientation;
            return this;
        }

        /// <summary>
        /// Enableds the specified enabled.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <returns></returns>
        public MobileSliderPropertiesBuilder Enabled(bool enabled)
        {
            mSlider.MSliderModel.Enabled = enabled;
            return this;
        }

        /// <summary>
        /// Persists the specified persist.
        /// </summary>
        /// <param name="persist">if set to <c>true</c> [persist].</param>
        /// <returns></returns>
        public MobileSliderPropertiesBuilder Persist(bool persist)
        {
            mSlider.MSliderModel.Persist = persist;
            return this;
        }

        /// <summary>
        /// Ioes the s7.
        /// </summary>
        /// <param name="ios7Model">The ios7 model.</param>
        /// <returns></returns>
        public MobileSliderPropertiesBuilder IOS7(Action<MobileSliderIOS7PropertiesBuilder> ios7Model)
        {
            var builder = new MobileSliderIOS7PropertiesBuilder(this.mSlider.MSliderModel);
            if (ios7Model != null)
                ios7Model.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Windowses the specified windows model.
        /// </summary>
        /// <param name="windowsModel">The windows model.</param>
        /// <returns></returns>
        public MobileSliderPropertiesBuilder Windows(Action<MobileSliderWindowsPropertiesBuilder> windowsModel)
        {
            var builder = new MobileSliderWindowsPropertiesBuilder(this.mSlider.MSliderModel);
            if (windowsModel != null)
                windowsModel.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Clients the side events.
        /// </summary>
        /// <param name="clientSideEvents">The client side events.</param>
        /// <returns></returns>
        public MobileSliderPropertiesBuilder ClientSideEvents(Action<MobileSliderClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MobileSliderClientSideEventsBuilder(this.mSlider.MSliderModel);
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
            return new HtmlString(mSlider.Render().ToString());
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
