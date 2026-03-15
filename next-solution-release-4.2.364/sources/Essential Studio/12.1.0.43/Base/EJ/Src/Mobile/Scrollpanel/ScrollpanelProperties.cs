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
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Mobile;

namespace Syncfusion.JavaScript.Mobile.Models
{
    
    public class MobileScrollpanelProperties : IMobileBase
    {
        #region Fields

        private RenderMode renderMode = RenderMode.Auto;
        private Theme theme = Theme.Auto;
        private bool resizeScrollbar = false;
        private int targetHeight = 0;
        private int targetWidth = 0;
        private int scrollHeight = 0;
        private int scrollWidth = 0;
        private bool fadeScrollbar = true;
        private bool shrinkScrollbar = false;
        private bool setAutoHeight = true;
        private bool relative = true;
        private int wheelSpeed = 16;
        private bool interactiveScrollbars = true;
        private bool enabled = true;
        private bool checkDOMChanges = false;
        private bool freeScroll = false;
        private bool hrScroll = false;
        private bool verScroll = true;
        private string eventPassthrough = "";
        private string translateZ = "";
        private int zoomMin = 1;
        private int zoomMax = 6;
        private bool adjustFixedPosition = true;
        private int startZoom = 1;
        private int startX = 0;
        private int startY = 0;
        private bool useDisplacement = false;
        private int displacementValue = 94;
        private int displacementTime = 800;
        private bool disablePointer = false;
        private bool disableMouse = false;
        private bool disableTouch = false;
        private int directionLockThreshold = 5;
        private bool momentum = true;
        private bool bounce = true;
        private int bounceTime = 450;
        private string bounceEasing = "";
        private bool preventDefault = true;
        private bool useTransition = true;
        private bool useTransform = true;
        private bool scrollbars = true;
        private bool mouseWheel = true;
        private bool enableKeys = true;
        private bool zoom = false;
        private bool nativeScroll = (UserAgent.IsAndroid() && UserAgent.IsLowerAndroid() || UserAgent.IsWindows() && UserAgent.IsMobile() || UserAgent.IsIOS7()) ? false : (UserAgent.IsDevice() || UserAgent.IsWindows() && !UserAgent.IsMobile()) ? true : false;
        private bool invertWheel = true;

        private string target="";
             

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the render mode.
        /// </summary>
        /// <value>
        /// The render mode.
        /// </value>
        [JsonProperty("renderMode")]
        [DefaultValue(RenderMode.Auto)]
        public RenderMode RenderMode { get { return renderMode; } set { renderMode = value; } }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>
        /// The theme.
        /// </value>
        [JsonProperty("theme")]
        [DefaultValue(Theme.Auto)]
        public Theme Theme { get { return theme; } set { theme = value; } }
       
        /// <summary>
        /// Gets or sets a value indicating whether [resize scrollbar].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [resize scrollbar]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("resizeScrollbar")]
        [DefaultValue(false)]
        public bool ResizeScrollbar { get { return resizeScrollbar; } set { resizeScrollbar = value; } }

        /// <summary>
        /// Gets or sets the height of the target.
        /// </summary>
        /// <value>
        /// The height of the target.
        /// </value>
        [JsonProperty("targetHeight")]
        [DefaultValue(0)]
        public int TargetHeight { get { return targetHeight; } set { targetHeight = value; } }

        /// <summary>
        /// Gets or sets the width of the target.
        /// </summary>
        /// <value>
        /// The width of the target.
        /// </value>
        [JsonProperty("targetWidth")]
        [DefaultValue(0)]
        public int TargetWidth { get { return targetWidth; } set { targetWidth = value; } }

        /// <summary>
        /// Gets or sets the height of the scroll.
        /// </summary>
        /// <value>
        /// The height of the scroll.
        /// </value>
        [JsonProperty("scrollHeight")]
        [DefaultValue(0)]
        public int ScrollHeight { get { return scrollHeight; } set { scrollHeight = value; } }

        /// <summary>
        /// Gets or sets the width of the scroll.
        /// </summary>
        /// <value>
        /// The width of the scroll.
        /// </value>
        [JsonProperty("scrollWidth")]
        [DefaultValue(0)]
        public int ScrollWidth { get { return scrollWidth; } set { scrollWidth = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [fade scrollbar].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [fade scrollbar]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("fadeScrollbar")]
        [DefaultValue(true)]
        public bool FadeScrollbar { get { return fadeScrollbar; } set { fadeScrollbar = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [shrink scrollbar].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [shrink scrollbar]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("shrinkScrollbar")]
        [DefaultValue(false)]
        public bool ShrinkScrollbar { get { return shrinkScrollbar; } set { shrinkScrollbar = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [set automatic height].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [set automatic height]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("setAutoHeight")]
        [DefaultValue(true)]
        public bool SetAutoHeight { get { return setAutoHeight; } set { setAutoHeight = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileScrollpanelProperties"/> is relative.
        /// </summary>
        /// <value>
        ///   <c>true</c> if relative; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("relative")]
        [DefaultValue(true)]
        public bool Relative { get { return relative; } set { relative = value; } }

        /// <summary>
        /// Gets or sets the wheel speed.
        /// </summary>
        /// <value>
        /// The wheel speed.
        /// </value>
        [JsonProperty("wheelSpeed")]
        [DefaultValue(16)]
        public int WheelSpeed { get { return wheelSpeed; } set { wheelSpeed = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [interactive scrollbars].
        /// </summary>
        /// <value>
        /// <c>true</c> if [interactive scrollbars]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("interactiveScrollbars")]
        [DefaultValue(true)]
        public bool InteractiveScrollbars { get { return interactiveScrollbars; } set { interactiveScrollbars = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileScrollpanelProperties"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled { get { return enabled; } set { enabled = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [check DOM changes].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [check DOM changes]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("checkDOMChanges")]
        [DefaultValue(false)]
        public bool CheckDOMChanges { get { return checkDOMChanges; } set { checkDOMChanges = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [free scroll].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [free scroll]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("freeScroll")]
        [DefaultValue(false)]
        public bool FreeScroll { get { return freeScroll; } set { freeScroll = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [hr scroll].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [hr scroll]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("hrScroll")]
        [DefaultValue(false)]
        public bool HrScroll { get { return hrScroll; } set { hrScroll = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [ver scroll].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [ver scroll]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("verScroll")]
        [DefaultValue(false)]
        public bool VerScroll { get { return verScroll; } set { verScroll = value; } }

        /// <summary>
        /// Gets or sets the event passthrough.
        /// </summary>
        /// <value>
        /// The event passthrough.
        /// </value>
        [JsonProperty("eventPassthrough")]
        [DefaultValue("")]
        public string EventPassthrough { get; set; }

        /// <summary>
        /// Gets or sets the translate z.
        /// </summary>
        /// <value>
        /// The translate z.
        /// </value>
        [JsonProperty("translateZ")]
        [DefaultValue("")]
        public string TranslateZ { get { return translateZ; } set { translateZ = value; } }

        /// <summary>
        /// Gets or sets the zoom minimum.
        /// </summary>
        /// <value>
        /// The zoom minimum.
        /// </value>
        [JsonProperty("zoomMin")]
        [DefaultValue(1)]
        public int ZoomMin { get { return zoomMin; } set { zoomMin = value; } }

        /// <summary>
        /// Gets or sets the zoom maximum.
        /// </summary>
        /// <value>
        /// The zoom maximum.
        /// </value>
        [JsonProperty("zoomMax")]
        [DefaultValue(6)]
        public int ZoomMax { get { return zoomMax; } set { zoomMax = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [adjust fixed position].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [adjust fixed position]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("adjustFixedPosition")]
        [DefaultValue(true)]
        public bool AdjustFixedPosition { get { return adjustFixedPosition; } set { adjustFixedPosition = value; } }

        /// <summary>
        /// Gets or sets the start zoom.
        /// </summary>
        /// <value>
        /// The start zoom.
        /// </value>
        [JsonProperty("startZoom")]
        [DefaultValue(1)]
        public int StartZoom { get { return startZoom; } set { startZoom = value; } }

        /// <summary>
        /// Gets or sets the start x.
        /// </summary>
        /// <value>
        /// The start x.
        /// </value>
        [JsonProperty("startX")]
        [DefaultValue(0)]
        public int StartX { get { return startX; } set { startX = value; } }

        /// <summary>
        /// Gets or sets the start y.
        /// </summary>
        /// <value>
        /// The start y.
        /// </value>
        [JsonProperty("startY")]
        [DefaultValue(0)]
        public int StartY { get { return startY; } set { startY = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [use displacement].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use displacement]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("useDisplacement")]
        [DefaultValue(false)]
        public bool UseDisplacement { get { return useDisplacement; } set { useDisplacement = value; } }

        /// <summary>
        /// Gets or sets the displacement value.
        /// </summary>
        /// <value>
        /// The displacement value.
        /// </value>
        [JsonProperty("displacementValue")]
        [DefaultValue(94)]
        public int DisplacementValue { get { return displacementValue; } set { displacementValue = value; } }

        /// <summary>
        /// Gets or sets the displacement time.
        /// </summary>
        /// <value>
        /// The displacement time.
        /// </value>
        [JsonProperty("displacementTime")]
        [DefaultValue(800)]
        public int DisplacementTime { get { return displacementTime; } set { displacementTime = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [disable pointer].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [disable pointer]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("disablePointer")]
        [DefaultValue(false)]
        public bool DisablePointer { get { return disablePointer; } set { disablePointer = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [disable mouse].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [disable mouse]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("disableMouse")]
        [DefaultValue(false)]
        public bool DisableMouse { get { return disableMouse; } set { disableMouse = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [disable touch].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [disable touch]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("disableTouch")]
        [DefaultValue(false)]
        public bool DisableTouch { get { return disableTouch; } set { disableTouch = value; } }

        /// <summary>
        /// Gets or sets the direction lock threshold.
        /// </summary>
        /// <value>
        /// The direction lock threshold.
        /// </value>
        [JsonProperty("directionLockThreshold")]
        [DefaultValue(5)]
        public int DirectionLockThreshold { get { return directionLockThreshold; } set { directionLockThreshold = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileScrollpanelProperties"/> is momentum.
        /// </summary>
        /// <value>
        ///   <c>true</c> if momentum; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("momentum")]
        [DefaultValue(true)]
        public bool Momentum { get { return momentum; } set { momentum = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileScrollpanelProperties"/> is bounce.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bounce; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("bounce")]
        [DefaultValue(true)]
        public bool Bounce { get { return bounce; } set { bounce = value; } }

        /// <summary>
        /// Gets or sets the bounce time.
        /// </summary>
        /// <value>
        /// The bounce time.
        /// </value>
        [JsonProperty("bounceTime")]
        [DefaultValue(450)]
        public int BounceTime { get { return bounceTime; } set { bounceTime = value; } }

        /// <summary>
        /// Gets or sets the bounce easing.
        /// </summary>
        /// <value>
        /// The bounce easing.
        /// </value>
        [JsonProperty("bounceEasing")]
        [DefaultValue("")]
        public string BounceEasing { get { return bounceEasing; } set { bounceEasing = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [prevent default].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [prevent default]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("preventDefault")]
        [DefaultValue(true)]
        public bool PreventDefault { get { return preventDefault; } set { preventDefault = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [use transition].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use transition]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("useTransition")]
        [DefaultValue(true)]
        public bool UseTransition { get { return useTransition; } set { useTransition = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [use transform].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use transform]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("useTransform")]
        [DefaultValue(true)]
        public bool UseTransform { get { return useTransform; } set { useTransform = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileScrollpanelProperties"/> is scrollbars.
        /// </summary>
        /// <value>
        ///   <c>true</c> if scrollbars; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("scrollbars")]
        [DefaultValue(true)]
        public bool Scrollbars { get { return scrollbars; } set { scrollbars = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [mouse wheel].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [mouse wheel]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("mouseWheel")]
        [DefaultValue(true)]
        public bool MouseWheel { get { return mouseWheel; } set { mouseWheel = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [enable keys].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable keys]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("enableKeys")]
        [DefaultValue(true)]
        public bool EnableKeys { get { return enableKeys; } set { enableKeys = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileScrollpanelProperties"/> is zoom.
        /// </summary>
        /// <value>
        ///   <c>true</c> if zoom; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("zoom")]
        [DefaultValue(false)]
        public bool Zoom { get { return zoom; } set { zoom = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [native scroll].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [native scroll]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("nativeScroll")]
        public bool NativeScroll { get { return nativeScroll; } set { nativeScroll = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [invert wheel].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [invert wheel]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("invertWheel")]
        [DefaultValue(true)]
        public bool InvertWheel { get { return invertWheel; } set { invertWheel = value; } }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        [JsonProperty("target")]
        [DefaultValue("")]
        public string Target { get { return target; } set { target = value; } }


        /// <summary>
        /// Gets or sets the scroll start.
        /// </summary>
        /// <value>
        /// The scroll start.
        /// </value>
        [JsonProperty("scrollStart")]
        [DefaultValue("")]
        public string ScrollStart { get; set; }

        /// <summary>
        /// Gets or sets the scroll move.
        /// </summary>
        /// <value>
        /// The scroll move.
        /// </value>
        [JsonProperty("scrollMove")]
        [DefaultValue("")]
        public string ScrollMove { get; set; }

        /// <summary>
        /// Gets or sets the scroll end.
        /// </summary>
        /// <value>
        /// The scroll end.
        /// </value>
        [JsonProperty("scrollEnd")]
        [DefaultValue("")]
        public string ScrollEnd { get; set; }

        /// <summary>
        /// Gets or sets the before scroll start.
        /// </summary>
        /// <value>
        /// The before scroll start.
        /// </value>
        [JsonProperty("beforeScrollStart")]
        [DefaultValue("")]
        public string BeforeScrollStart { get; set; }

        /// <summary>
        /// Gets or sets the zoom start.
        /// </summary>
        /// <value>
        /// The zoom start.
        /// </value>
        [JsonProperty("zoomStart")]
        [DefaultValue("")]
        public string ZoomStart { get; set; }

        /// <summary>
        /// Gets or sets the zoom end.
        /// </summary>
        /// <value>
        /// The zoom end.
        /// </value>
        [JsonProperty("zoomEnd")]
        [DefaultValue("")]
        public string ZoomEnd { get; set; }

        #endregion      

         #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileScrollpanelProperties"/> class.
        /// </summary>
        public MobileScrollpanelProperties()
        {
            
        }
        #endregion

    }
}
