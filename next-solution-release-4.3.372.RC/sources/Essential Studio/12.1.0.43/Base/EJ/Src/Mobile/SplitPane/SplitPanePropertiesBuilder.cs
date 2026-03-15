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
    /// SplitPane Properties Builder
    /// </summary>
    public class MobileSplitPanePropertiesBuilder
    {
        #region Fields
        private SplitPane mSplitPane;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileSplitPanePropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mSplitPane">The m SplitPane.</param>
        public MobileSplitPanePropertiesBuilder(SplitPane mSplitPane)
        {
            this.mSplitPane = new SplitPane(mSplitPane.ID, mSplitPane.MobileSplitPaneModel);

        }
        #endregion

        #region Builder
        /// <summary>
        /// Renders the mode.
        /// </summary>
        /// <param name="renderMode">The render mode.</param>
        /// <returns></returns>
        public MobileSplitPanePropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mSplitPane.MobileSplitPaneModel.RenderMode = renderMode;
            return this;
        }

        /// <summary>
        /// Specifies the template for leftPane
        /// </summary>
        /// <param name="renderMode">Template</param>
        /// <returns></returns>
        public MobileSplitPanePropertiesBuilder LeftPaneTemplate(Func<object, object> template)
        {
            mSplitPane.MobileSplitPaneModel.LeftPaneTemplate.RazorViewTemplate = template;
            return this;
        }

        /// <summary>
        /// Specifies the template for rightPane
        /// </summary>
        /// <param name="renderMode">Template</param>
        /// <returns></returns>
        public MobileSplitPanePropertiesBuilder RightPaneTemplate(Func<object, object> template)
        {
            mSplitPane.MobileSplitPaneModel.RightPaneTemplate.RazorViewTemplate = template;
            return this;
        }

        /// <summary>
        /// Specifies the template for leftPane
        /// </summary>
        /// <param name="renderMode">Template</param>
        /// <returns></returns>
        public MobileSplitPanePropertiesBuilder LeftPaneTemplate(Action template)
        {
            mSplitPane.MobileSplitPaneModel.LeftPaneTemplate.WebFormContentTemplate = template;
            return this;
        }

        /// <summary>
        /// Specifies the template for rightPane
        /// </summary>
        /// <param name="renderMode">Template</param>
        /// <returns></returns>
        public MobileSplitPanePropertiesBuilder RightPaneTemplate(Action template)
        {
            mSplitPane.MobileSplitPaneModel.RightPaneTemplate.WebFormContentTemplate = template;
            return this;
        }

        /// <summary>
        /// Specifies the rightheader title
        /// </summary>
        /// <param name="renderMode">Title</param>
        /// <returns></returns>
        public MobileSplitPanePropertiesBuilder RightHeaderTitle(string title)
        {
            mSplitPane.MobileSplitPaneModel.RightHeaderTitle = title;
            return this;
        }

        /// <summary>
        /// Specifies the leftheader title
        /// </summary>
        /// <param name="renderMode">Title</param>
        /// <returns></returns>
        public MobileSplitPanePropertiesBuilder LeftHeaderTitle(string title)
        {
            mSplitPane.MobileSplitPaneModel.LeftHeaderTitle = title;
            return this;
        }

        /// <summary>
        /// Themes the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public MobileSplitPanePropertiesBuilder Theme(Theme theme)
        {
            mSplitPane.MobileSplitPaneModel.Theme = theme;
            return this;
        }

        /// <summary>
        /// Ioes the s7.
        /// </summary>
        /// <param name="ios7Model">The ios7 model.</param>
        /// <returns></returns>
        public MobileSplitPanePropertiesBuilder IOS7(Action<MobileSplitPaneIOS7PropertiesBuilder> ios7Model)
        {
            var builder = new MobileSplitPaneIOS7PropertiesBuilder(this.mSplitPane.MobileSplitPaneModel);
            if (ios7Model != null)
                ios7Model.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Androids the specified android model.
        /// </summary>
        /// <param name="androidModel">The android model.</param>
        /// <returns></returns>
        public MobileSplitPanePropertiesBuilder Android(Action<MobileSplitPaneAndroidPropertiesBuilder> androidModel)
        {
            var builder = new MobileSplitPaneAndroidPropertiesBuilder(this.mSplitPane.MobileSplitPaneModel);
            if (androidModel != null)
                androidModel.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Windowses the specified windows model.
        /// </summary>
        /// <param name="windowsModel">The windows model.</param>
        /// <returns></returns>
        public MobileSplitPanePropertiesBuilder Windows(Action<MobileSplitPaneWindowsPropertiesBuilder> windowsModel)
        {
            var builder = new MobileSplitPaneWindowsPropertiesBuilder(this.mSplitPane.MobileSplitPaneModel);
            if (windowsModel != null)
                windowsModel.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Clients the side events.
        /// </summary>
        /// <param name="clientSideEvents">The client side events.</param>
        /// <returns></returns>
        public MobileSplitPanePropertiesBuilder ClientSideEvents(Action<MobileSplitPaneClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MobileSplitPaneClientSideEventsBuilder(this.mSplitPane.MobileSplitPaneModel);
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
            return new HtmlString(mSplitPane.Render().ToString());
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
