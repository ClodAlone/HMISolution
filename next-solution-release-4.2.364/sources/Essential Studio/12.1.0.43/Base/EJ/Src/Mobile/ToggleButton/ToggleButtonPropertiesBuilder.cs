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
using System.Web;

namespace Syncfusion.JavaScript.Mobile
{
    /// <summary>
    /// Class for MobileToggleButtonPropertiesBuilder
    /// </summary>
    public class MobileToggleButtonPropertiesBuilder
    {

        #region Fields
        private ToggleButton mobileToggleButton;
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MToggleButtonPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mToggleButton">The m toggle button.</param>
        public MobileToggleButtonPropertiesBuilder(ToggleButton mobileToggleButton)
        {
            this.mobileToggleButton = new ToggleButton(mobileToggleButton.ID, mobileToggleButton.MobileToggleButtonModel);
        }
        #endregion


        #region Builder

        /// <summary>
        /// Renders the mode.
        /// </summary>
        /// <param name="renderMode">The render mode.</param>
        /// <returns></returns>
        public MobileToggleButtonPropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mobileToggleButton.MobileToggleButtonModel.RenderMode = renderMode;
            return this;
        }

        /// <summary>
        /// Themes the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public MobileToggleButtonPropertiesBuilder Theme(Theme theme)
        {
            mobileToggleButton.MobileToggleButtonModel.Theme = theme;
            return this;
        }

        /// <summary>
        /// Animates the specified animate.
        /// </summary>
        /// <param name="animate">if set to <c>true</c> [animate].</param>
        /// <returns></returns>
        public MobileToggleButtonPropertiesBuilder Animate(bool animate)
        {
            mobileToggleButton.MobileToggleButtonModel.Animate = animate;
            return this;
        }

        /// <summary>
        /// Toggles the state.
        /// </summary>
        /// <param name="toggleState">if set to <c>true</c> [toggle state].</param>
        /// <returns></returns>
        public MobileToggleButtonPropertiesBuilder ToggleState(bool toggleState)
        {
            mobileToggleButton.MobileToggleButtonModel.ToggleState = toggleState;
            return this;
        }

        /// <summary>
        /// Enableds the specified enabled.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <returns></returns>
        public MobileToggleButtonPropertiesBuilder Enabled(bool enabled)
        {
            mobileToggleButton.MobileToggleButtonModel.Enabled = enabled;
            return this;
        }

        /// <summary>
        /// Persists the specified persist.
        /// </summary>
        /// <param name="persist">if set to <c>true</c> [persist].</param>
        /// <returns></returns>
        public MobileToggleButtonPropertiesBuilder Persist(bool persist)
        {
            mobileToggleButton.MobileToggleButtonModel.Persist = persist;
            return this;
        }

        public MobileToggleButtonPropertiesBuilder ClientSideEvents(Action<MobileToggleButtonClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MobileToggleButtonClientSideEventsBuilder(this.mobileToggleButton.MobileToggleButtonModel);
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
            return new HtmlString(mobileToggleButton.Render().ToString());
        }
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override String ToString()
        {
            return Render().ToString();
        }
        #endregion
    
    }
}
