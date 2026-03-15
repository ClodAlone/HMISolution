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
    public class MobileScrollpanelPropertiesBuilder
    {
        #region Fields
        private MobileScrollpanel mScrollpanel;        
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileScrollpanelPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="MobileScrollpanel">The mobile scrollpanel.</param>
        public MobileScrollpanelPropertiesBuilder(MobileScrollpanel MobileScrollpanel)
        {
            this.mScrollpanel = new MobileScrollpanel(MobileScrollpanel.ID, MobileScrollpanel.MobileScrollpanelModel);
        }
        #endregion

        #region Builder

        /// <summary>
        /// Renders the mode.
        /// </summary>
        /// <param name="renderMode">The render mode.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mScrollpanel.MobileScrollpanelModel.RenderMode = renderMode;
            return this;
        }

        /// <summary>
        /// Themes the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder Theme(Theme theme)
        {
            mScrollpanel.MobileScrollpanelModel.Theme = theme;
            return this;
        }

        /// <summary>
        /// Resizes the scrollbar.
        /// </summary>
        /// <param name="resizeScrollbar">if set to <c>true</c> [resize scrollbar].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder ResizeScrollbar(bool resizeScrollbar)
        {
            mScrollpanel.MobileScrollpanelModel.ResizeScrollbar = resizeScrollbar;
            return this;
        }

        /// <summary>
        /// Targets the height.
        /// </summary>
        /// <param name="targetHeight">Height of the target.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder TargetHeight(int targetHeight)
        {
            mScrollpanel.MobileScrollpanelModel.TargetHeight = targetHeight;
            return this;
        }

        /// <summary>
        /// Targets the width.
        /// </summary>
        /// <param name="targetWidth">Width of the target.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder TargetWidth(int targetWidth)
        {
            mScrollpanel.MobileScrollpanelModel.TargetWidth = targetWidth;
            return this;
        }

        /// <summary>
        /// Scrolls the height.
        /// </summary>
        /// <param name="scrollHeight">Height of the scroll.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder ScrollHeight(int scrollHeight)
        {
            mScrollpanel.MobileScrollpanelModel.ScrollHeight = scrollHeight;
            return this;
        }

        /// <summary>
        /// Scrolls the width.
        /// </summary>
        /// <param name="scrollWidth">Width of the scroll.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder ScrollWidth(int scrollWidth)
        {
            mScrollpanel.MobileScrollpanelModel.ScrollWidth = scrollWidth;
            return this;
        }

        /// <summary>
        /// Fades the scrollbar.
        /// </summary>
        /// <param name="fadeScrollbar">if set to <c>true</c> [fade scrollbar].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder FadeScrollbar(bool fadeScrollbar)
        {
            mScrollpanel.MobileScrollpanelModel.FadeScrollbar = fadeScrollbar;
            return this;
        }
        /// <summary>
        /// Shrinks the scrollbar.
        /// </summary>
        /// <param name="shrinkScrollbar">if set to <c>true</c> [shrink scrollbar].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder ShrinkScrollbar(bool shrinkScrollbar)
        {
            mScrollpanel.MobileScrollpanelModel.ShrinkScrollbar = shrinkScrollbar;
            return this;
        }

        /// <summary>
        /// Sets the height of the automatic.
        /// </summary>
        /// <param name="setAutoHeight">if set to <c>true</c> [set automatic height].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder SetAutoHeight(bool setAutoHeight)
        {
            mScrollpanel.MobileScrollpanelModel.SetAutoHeight = setAutoHeight;
            return this;
        }

        /// <summary>
        /// Relatives the specified relative.
        /// </summary>
        /// <param name="relative">if set to <c>true</c> [relative].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder Relative(bool relative)
        {
            mScrollpanel.MobileScrollpanelModel.Relative = relative;
            return this;
        }

        /// <summary>
        /// Wheels the speed.
        /// </summary>
        /// <param name="wheelSpeed">The wheel speed.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder WheelSpeed(int wheelSpeed)
        {
            mScrollpanel.MobileScrollpanelModel.WheelSpeed = wheelSpeed;
            return this;
        }

        /// <summary>
        /// Interactives the scrollbars.
        /// </summary>
        /// <param name="interactiveScrollbars">if set to <c>true</c> [interactive scrollbars].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder InteractiveScrollbars(bool interactiveScrollbars)
        {
            mScrollpanel.MobileScrollpanelModel.InteractiveScrollbars = interactiveScrollbars;
            return this;
        }

        /// <summary>
        /// Enableds the specified enabled.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder Enabled(bool enabled)
        {
            mScrollpanel.MobileScrollpanelModel.Enabled = enabled;
            return this;
        }

        /// <summary>
        /// Checks the DOM changes.
        /// </summary>
        /// <param name="checkDOMChanges">if set to <c>true</c> [check DOM changes].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder CheckDOMChanges(bool checkDOMChanges)
        {
            mScrollpanel.MobileScrollpanelModel.CheckDOMChanges = checkDOMChanges;
            return this;
        }

        /// <summary>
        /// Frees the scroll.
        /// </summary>
        /// <param name="freeScroll">if set to <c>true</c> [free scroll].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder FreeScroll(bool freeScroll)
        {
            mScrollpanel.MobileScrollpanelModel.FreeScroll = freeScroll;
            return this;
        }

        /// <summary>
        /// Hrs the scroll.
        /// </summary>
        /// <param name="hrScroll">if set to <c>true</c> [hr scroll].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder HrScroll(bool hrScroll)
        {
            mScrollpanel.MobileScrollpanelModel.HrScroll = hrScroll;
            return this;
        }

        /// <summary>
        /// Vers the scroll.
        /// </summary>
        /// <param name="verScroll">if set to <c>true</c> [ver scroll].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder VerScroll(bool verScroll)
        {
            mScrollpanel.MobileScrollpanelModel.VerScroll = verScroll;
            return this;
        }

        /// <summary>
        /// Events the passthrough.
        /// </summary>
        /// <param name="eventPassthrough">The event passthrough.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder EventPassthrough(string eventPassthrough)
        {
            mScrollpanel.MobileScrollpanelModel.EventPassthrough = eventPassthrough;
            return this;
        }

        /// <summary>
        /// Translates the z.
        /// </summary>
        /// <param name="translateZ">The translate z.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder TranslateZ(string translateZ)
        {
            mScrollpanel.MobileScrollpanelModel.TranslateZ = translateZ;
            return this;
        }

        /// <summary>
        /// Zooms the minimum.
        /// </summary>
        /// <param name="zoomMin">The zoom minimum.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder ZoomMin(int zoomMin)
        {
            mScrollpanel.MobileScrollpanelModel.ZoomMin = zoomMin;
            return this;
        }

        /// <summary>
        /// Zooms the maximum.
        /// </summary>
        /// <param name="zoomMax">The zoom maximum.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder ZoomMax(int zoomMax)
        {
            mScrollpanel.MobileScrollpanelModel.ZoomMax = zoomMax;
            return this;
        }

        /// <summary>
        /// Adjusts the fixed position.
        /// </summary>
        /// <param name="adjustFixedPosition">if set to <c>true</c> [adjust fixed position].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder AdjustFixedPosition(bool adjustFixedPosition)
        {
            mScrollpanel.MobileScrollpanelModel.AdjustFixedPosition = adjustFixedPosition;
            return this;
        }

        /// <summary>
        /// Starts the zoom.
        /// </summary>
        /// <param name="startZoom">The start zoom.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder StartZoom(int startZoom)
        {
            mScrollpanel.MobileScrollpanelModel.StartZoom = startZoom;
            return this;
        }

        /// <summary>
        /// Starts the x.
        /// </summary>
        /// <param name="startX">The start x.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder StartX(int startX)
        {
            mScrollpanel.MobileScrollpanelModel.StartX = startX;
            return this;
        }

        /// <summary>
        /// Starts the y.
        /// </summary>
        /// <param name="startY">The start y.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder StartY(int startY)
        {
            mScrollpanel.MobileScrollpanelModel.StartY = startY;
            return this;
        }

        /// <summary>
        /// Uses the displacement.
        /// </summary>
        /// <param name="useDisplacement">if set to <c>true</c> [use displacement].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder UseDisplacement(bool useDisplacement)
        {
            mScrollpanel.MobileScrollpanelModel.UseDisplacement = useDisplacement;
            return this;
        }

        /// <summary>
        /// Displacements the value.
        /// </summary>
        /// <param name="displacementValue">The displacement value.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder DisplacementValue(int displacementValue)
        {
            mScrollpanel.MobileScrollpanelModel.DisplacementValue = displacementValue;
            return this;
        }

        /// <summary>
        /// Displacements the time.
        /// </summary>
        /// <param name="displacementTime">The displacement time.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder DisplacementTime(int displacementTime)
        {
            mScrollpanel.MobileScrollpanelModel.DisplacementTime = displacementTime;
            return this;
        }

        /// <summary>
        /// Disables the pointer.
        /// </summary>
        /// <param name="disablePointer">if set to <c>true</c> [disable pointer].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder DisablePointer(bool disablePointer)
        {
            mScrollpanel.MobileScrollpanelModel.DisablePointer = disablePointer;
            return this;
        }

        /// <summary>
        /// Disables the mouse.
        /// </summary>
        /// <param name="disableMouse">if set to <c>true</c> [disable mouse].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder DisableMouse(bool disableMouse)
        {
            mScrollpanel.MobileScrollpanelModel.DisableMouse = disableMouse;
            return this;
        }

        /// <summary>
        /// Disables the touch.
        /// </summary>
        /// <param name="disableTouch">if set to <c>true</c> [disable touch].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder DisableTouch(bool disableTouch)
        {
            mScrollpanel.MobileScrollpanelModel.DisableTouch = disableTouch;
            return this;
        }

        /// <summary>
        /// Directions the lock threshold.
        /// </summary>
        /// <param name="directionLockThreshold">The direction lock threshold.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder DirectionLockThreshold(int directionLockThreshold)
        {
            mScrollpanel.MobileScrollpanelModel.DirectionLockThreshold = directionLockThreshold;
            return this;
        }

        /// <summary>
        /// Momentums the specified momentum.
        /// </summary>
        /// <param name="momentum">if set to <c>true</c> [momentum].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder Momentum(bool momentum)
        {
            mScrollpanel.MobileScrollpanelModel.Momentum = momentum;
            return this;
        }

        /// <summary>
        /// Bounces the specified bounce.
        /// </summary>
        /// <param name="bounce">if set to <c>true</c> [bounce].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder Bounce(bool bounce)
        {
            mScrollpanel.MobileScrollpanelModel.Bounce = bounce;
            return this;
        }

        /// <summary>
        /// Bounces the time.
        /// </summary>
        /// <param name="bounceTime">The bounce time.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder BounceTime(int bounceTime)
        {
            mScrollpanel.MobileScrollpanelModel.BounceTime = bounceTime;
            return this;
        }

        /// <summary>
        /// Bounces the easing.
        /// </summary>
        /// <param name="bounceEasing">The bounce easing.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder BounceEasing(string bounceEasing)
        {
            mScrollpanel.MobileScrollpanelModel.BounceEasing = bounceEasing;
            return this;
        }

        /// <summary>
        /// Prevents the default.
        /// </summary>
        /// <param name="preventDefault">if set to <c>true</c> [prevent default].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder PreventDefault(bool preventDefault)
        {
            mScrollpanel.MobileScrollpanelModel.PreventDefault = preventDefault;
            return this;
        }

        /// <summary>
        /// Uses the transition.
        /// </summary>
        /// <param name="useTransition">if set to <c>true</c> [use transition].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder UseTransition(bool useTransition)
        {
            mScrollpanel.MobileScrollpanelModel.UseTransition = useTransition;
            return this;
        }

        /// <summary>
        /// Uses the transform.
        /// </summary>
        /// <param name="useTransform">if set to <c>true</c> [use transform].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder UseTransform(bool useTransform)
        {
            mScrollpanel.MobileScrollpanelModel.UseTransform = useTransform;
            return this;
        }

        /// <summary>
        /// Scrollbarses the specified scrollbars.
        /// </summary>
        /// <param name="scrollbars">if set to <c>true</c> [scrollbars].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder Scrollbars(bool scrollbars)
        {
            mScrollpanel.MobileScrollpanelModel.Scrollbars = scrollbars;
            return this;
        }

        /// <summary>
        /// Mouses the wheel.
        /// </summary>
        /// <param name="mouseWheel">if set to <c>true</c> [mouse wheel].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder MouseWheel(bool mouseWheel)
        {
            mScrollpanel.MobileScrollpanelModel.MouseWheel = mouseWheel;
            return this;
        }

        /// <summary>
        /// Enables the keys.
        /// </summary>
        /// <param name="enableKeys">if set to <c>true</c> [enable keys].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder EnableKeys(bool enableKeys)
        {
            mScrollpanel.MobileScrollpanelModel.EnableKeys = enableKeys;
            return this;
        }

        public MobileScrollpanelPropertiesBuilder Zoom(bool zoom)
        {
            mScrollpanel.MobileScrollpanelModel.Zoom = zoom;
            return this;
        }

        /// <summary>
        /// Natives the scroll.
        /// </summary>
        /// <param name="nativeScroll">if set to <c>true</c> [native scroll].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder NativeScroll(bool nativeScroll)
        {
            mScrollpanel.MobileScrollpanelModel.NativeScroll = nativeScroll;
            return this;
        }

        /// <summary>
        /// Inverts the wheel.
        /// </summary>
        /// <param name="invertWheel">if set to <c>true</c> [invert wheel].</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder InvertWheel(bool invertWheel)
        {
            mScrollpanel.MobileScrollpanelModel.InvertWheel = invertWheel;
            return this;
        }

        /// <summary>
        /// Targets the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder Target(string target)
        {
            mScrollpanel.MobileScrollpanelModel.Target = target;
            return this;
        }   

        /// <summary>
        /// Clients the side events.
        /// </summary>
        /// <param name="clientSideEvents">The client side events.</param>
        /// <returns></returns>
        public MobileScrollpanelPropertiesBuilder ClientSideEvents(Action<MobileScrollpanelClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MobileScrollpanelClientSideEventsBuilder(this.mScrollpanel.MobileScrollpanelModel);
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
            return new HtmlString(mScrollpanel.Render().ToString());
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
