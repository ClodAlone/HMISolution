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
    /// Class for ProgressBar Property Builder
    /// </summary>
    public class MobileProgressBarPropertiesBuilder
    {
        #region Fields
        private ProgressBar mProgressBar;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileProgressBarPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mProgressBar">The m progress bar.</param>
        public MobileProgressBarPropertiesBuilder(ProgressBar mProgressBar)
        {
            this.mProgressBar = new ProgressBar(mProgressBar.ID, mProgressBar.MProgressBarModel);

        }
        #endregion

        #region Builder


        /// <summary>
        /// Renders the mode.
        /// </summary>
        /// <param name="renderMode">The render mode.</param>
        /// <returns></returns>
        public MobileProgressBarPropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mProgressBar.MProgressBarModel.RenderMode = renderMode;
            return this;
        }


        /// <summary>
        /// Themes the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public MobileProgressBarPropertiesBuilder Theme(Theme theme)
        {
            mProgressBar.MProgressBarModel.Theme = theme;
            return this;
        }


        /// <summary>
        /// Values the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public MobileProgressBarPropertiesBuilder Value(int value)
        {
            mProgressBar.MProgressBarModel.Value = value;
            return this;
        }


        /// <summary>
        /// Percentages the specified percentage.
        /// </summary>
        /// <param name="percentage">The percentage.</param>
        /// <returns></returns>
        public MobileProgressBarPropertiesBuilder Percentage(int percentage)
        {
            mProgressBar.MProgressBarModel.Percentage = percentage;
            return this;
        }


        /// <summary>
        /// Steps the value.
        /// </summary>
        /// <param name="stepValue">The step value.</param>
        /// <returns></returns>
        public MobileProgressBarPropertiesBuilder StepValue(int stepValue )
        {
            mProgressBar.MProgressBarModel.StepValue = stepValue;
            return this;
        }


        /// <summary>
        /// Minimums the specified minimum.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <returns></returns>
        public MobileProgressBarPropertiesBuilder Min(int min)
        {
            mProgressBar.MProgressBarModel.Min = min;
            return this;
        }

        /// <summary>
        /// Maximums the specified maximum.
        /// </summary>
        /// <param name="max">The maximum.</param>
        /// <returns></returns>
        public MobileProgressBarPropertiesBuilder Max(int max)
        {
            mProgressBar.MProgressBarModel.Max = max;
            return this;
        }

        /// <summary>
        /// Texts the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public MobileProgressBarPropertiesBuilder Text(string text)
        {
            mProgressBar.MProgressBarModel.Text = text;
            return this;
        }

        /// <summary>
        /// Allows the custom text.
        /// </summary>
        /// <param name="allowCustomText">if set to <c>true</c> [allow custom text].</param>
        /// <returns></returns>
        public MobileProgressBarPropertiesBuilder AllowCustomText(bool allowCustomText)
        {
            mProgressBar.MProgressBarModel.AllowCustomText = allowCustomText;
            return this;
        }

        /// <summary>
        /// Called when [custom text].
        /// </summary>
        /// <param name="onCustomText">The on custom text.</param>
        /// <returns></returns>
        public MobileProgressBarPropertiesBuilder OnCustomText(string onCustomText)
        {
            mProgressBar.MProgressBarModel.OnCustomText = onCustomText;
            return this;
        }

        /// <summary>
        /// Widthes the specified width.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public MobileProgressBarPropertiesBuilder Width(double width)
        {
            mProgressBar.MProgressBarModel.Width = width;
            return this;
        }

        /// <summary>
        /// Heights the specified height.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public MobileProgressBarPropertiesBuilder Height(double height)
        {
            mProgressBar.MProgressBarModel.Height = height;
            return this;
        }

        /// <summary>
        /// Orientations the specified _orientation.
        /// </summary>
        /// <param name="_orientation">The _orientation.</param>
        /// <returns></returns>
        public MobileProgressBarPropertiesBuilder Orientation(Orientation _orientation)
        {
            mProgressBar.MProgressBarModel.Orientation = _orientation;
            return this;
        }

        /// <summary>
        /// Enableds the specified enabled.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <returns></returns>
        public MobileProgressBarPropertiesBuilder Enabled(bool enabled)
        {
            mProgressBar.MProgressBarModel.Enabled = enabled;
            return this;
        }

        public MobileProgressBarPropertiesBuilder ClientSideEvents(Action<MobileProgressBarClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MobileProgressBarClientSideEventsBuilder(this.mProgressBar.MProgressBarModel);
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
            return new HtmlString(mProgressBar.Render().ToString());
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
