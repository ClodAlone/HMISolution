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
    public class MobileRotatorPropertiesBuilder
    {
        #region Fields
        private Rotator mobileRotator;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileRotatorPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="Rotator">The m tab.</param>
        public MobileRotatorPropertiesBuilder(Rotator mobileRotator)
        {
            this.mobileRotator = new Rotator(mobileRotator.ID, mobileRotator.MobileRotatorModel);

        }
        #endregion

        #region Builder

        /// <summary>
        /// Renders the mode.
        /// </summary>
        /// <param name="renderMode">The render mode.</param>
        /// <returns></returns>
        public MobileRotatorPropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mobileRotator.MobileRotatorModel.RenderMode = renderMode;
            return this;
        }

        /// <summary>
        /// Themes the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public MobileRotatorPropertiesBuilder Theme(Theme theme)
        {
            mobileRotator.MobileRotatorModel.Theme = theme;
            return this;
        }

        /// <summary>
        /// showPager.
        /// </summary>
        /// <param name="showPager">if set to <c>true</c> [showPager].</param>
        /// <returns></returns>
        public MobileRotatorPropertiesBuilder Pager(bool pager)
        {
            mobileRotator.MobileRotatorModel.Pager = pager;
            return this;
        }
        /// <summary>
        /// CssClass.
        /// </summary>
        /// <param name="CssClass">if set to [CssClass].</param>
        /// <returns></returns>
        public MobileRotatorPropertiesBuilder CssClass(string cssClass)
        {
            mobileRotator.MobileRotatorModel.CssClass = cssClass;
            return this;
        }

        /// <summary>
        /// header.
        /// </summary>
        /// <param name="header">if set to <c>true</c> [header].</param>
        /// <returns></returns>
        public MobileRotatorPropertiesBuilder Header(bool header)
        {
            mobileRotator.MobileRotatorModel.Header = header;
            return this;
        }

        /// <summary>
        /// dataBinding.
        /// </summary>
        /// <param name="dataBinding">if set to <c>true</c> [dataBinding].</param>
        /// <returns></returns>
        public MobileRotatorPropertiesBuilder DataBinding(bool dataBinding)
        {
            mobileRotator.MobileRotatorModel.DataBinding = dataBinding;
            return this;
        }

        /// <summary>
        /// targetHeight.
        /// </summary>
        /// <param name="targetHeight">if set to [targetHeight].</param>
        /// <returns></returns>
        public MobileRotatorPropertiesBuilder TargetHeight(string targetHeight)
        {
            mobileRotator.MobileRotatorModel.TargetHeight = targetHeight;
            return this;
        }

        /// <summary>
        /// targetWidth.
        /// </summary>
        /// <param name="targetWidth">targetWidth.</param>
        /// <returns></returns>
        public MobileRotatorPropertiesBuilder TargetWidth(string targetWidth)
        {
            mobileRotator.MobileRotatorModel.TargetWidth = targetWidth;
            return this;
        }

        /// <summary>
        /// headerTitle.
        /// </summary>
        /// <param name="headerTitle">headerTitle.</param>
        /// <returns></returns>
        public MobileRotatorPropertiesBuilder HeaderTitle(string headerTitle)
        {
            mobileRotator.MobileRotatorModel.HeaderTitle = headerTitle;
            return this;
        }

        /// <summary>
        /// currentIndex.
        /// </summary>
        /// <param name="currentIndex">currentIndex.</param>
        /// <returns></returns>
        public MobileRotatorPropertiesBuilder CurrentIndex(int currentIndex)
        {
            mobileRotator.MobileRotatorModel.CurrentIndex = currentIndex;
            return this;
        }
        /// <summary>
        /// targetId.
        /// </summary>
        /// <param name="targetId">targetId.</param>
        /// <returns></returns>
        public MobileRotatorPropertiesBuilder TargetId(string targetId)
        {
            mobileRotator.MobileRotatorModel.TargetId = targetId;
            return this;
        }
        /// <summary>
        /// dataSource.
        /// </summary>
        /// <param name="dataSource">dataSource.</param>
        /// <returns></returns>
        public MobileRotatorPropertiesBuilder DataSource(object dataSource)
        {
            mobileRotator.MobileRotatorModel.DataSource = dataSource;
            return this;
        }
        /// <summary>
        /// pagerPosition.
        /// </summary>
        /// <param name="pagerPosition">pagerPosition.</param>
        /// <returns></returns>
        public MobileRotatorPropertiesBuilder PagerPosition(MobPager pagerPosition)
        {
            mobileRotator.MobileRotatorModel.PagerPosition = pagerPosition;
            return this;
        }       

        /// <summary>
        /// Clients the side events.
        /// </summary>
        /// <param name="clientSideEvents">The client side events.</param>
        /// <returns></returns>
        public MobileRotatorPropertiesBuilder ClientSideEvents(Action<MobileRotatorClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MobileRotatorClientSideEventsBuilder(this.mobileRotator.MobileRotatorModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        /// <summary>
        /// Windowses the specified windows model.
        /// </summary>
        /// <param name="windowsModel">The windows model.</param>
        /// <returns></returns>
        public MobileRotatorPropertiesBuilder Windows(Action<RotatorRenderModePropertiesBuilder> windowsModel)
        {
            var builder = new RotatorRenderModePropertiesBuilder(this.mobileRotator.MobileRotatorModel);
            if (windowsModel != null)
                windowsModel.Invoke(builder);
            return this;
        }

        #endregion Builder

        #region Render
        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns></returns>
        public HtmlString Render()
        {
            return new HtmlString(mobileRotator.Render().ToString());
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

