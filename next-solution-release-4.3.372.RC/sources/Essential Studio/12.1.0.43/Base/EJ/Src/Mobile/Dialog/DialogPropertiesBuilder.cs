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
   public class MobileDialogPropertiesBuilder
    {
        #region Fields
        private Dialog mobileDialog;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileDialogPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="Rotator">The m tab.</param>
        public MobileDialogPropertiesBuilder(Dialog mobileDialog)
        {
            this.mobileDialog = new Dialog(mobileDialog.ID, mobileDialog.MobileDialogModel);

        }
        #endregion

        #region Builder

        /// <summary>
        /// Renders the mode.
        /// </summary>
        /// <param name="renderMode">The render mode.</param>
        /// <returns></returns>
        public MobileDialogPropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mobileDialog.MobileDialogModel.RenderMode = renderMode;
            return this;
        }

        /// <summary>
        /// Themes the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public MobileDialogPropertiesBuilder Theme(Theme theme)
        {
            mobileDialog.MobileDialogModel.Theme = theme;
            return this;
        }

        /// <summary>
        /// Allows the scrolling.
        /// </summary>
        /// <param name="allowScrolling">if set to <c>true</c> [allow scrolling].</param>
        /// <returns></returns>
        public MobileDialogPropertiesBuilder AllowScrolling(bool allowScrolling)
        {
            mobileDialog.MobileDialogModel.AllowScrolling = allowScrolling;
            return this;
        }

        /// <summary>
        /// autoOpen.
        /// </summary>
        /// <param name="autoOpen">if set to <c>true</c> [autoOpen].</param>
        /// <returns></returns>
        public MobileDialogPropertiesBuilder AutoOpen(bool autoOpen)
        {
            mobileDialog.MobileDialogModel.AutoOpen = autoOpen;
            return this;
        }
        /// <summary>
        /// CssClass.
        /// </summary>
        /// <param name="CssClass">if set to [CssClass].</param>
        /// <returns></returns>
        public MobileDialogPropertiesBuilder CssClass(string cssClass)
        {
            mobileDialog.MobileDialogModel.CssClass = cssClass;
            return this;
        }

        /// <summary>
        /// modal.
        /// </summary>
        /// <param name="modal">if set to <c>true</c> [modal].</param>
        /// <returns></returns>
        public MobileDialogPropertiesBuilder Modal(bool modal)
        {
            mobileDialog.MobileDialogModel.Modal = modal;
            return this;
        }
       
        /// <summary>
        /// showButton.
        /// </summary>
        /// <param name="showButton">if set to <c>true</c> [showButton].</param>
        /// <returns></returns>
        public MobileDialogPropertiesBuilder ShowButton(bool showButton)
        {
            mobileDialog.MobileDialogModel.ShowButton = showButton;
            return this;
        }

        /// <summary>
        /// checkDOMChanges.
        /// </summary>
        /// <param name="checkDOMChanges">checkDOMChanges.</param>
        /// <returns></returns>
        public MobileDialogPropertiesBuilder CheckDOMChanges(bool checkDOMChanges)
        {
            mobileDialog.MobileDialogModel.CheckDOMChanges = checkDOMChanges;
            return this;
        }

        /// <summary>
        /// dialogMode.
        /// </summary>
        /// <param name="dialogMode">dialogMode.</param>
        /// <returns></returns>
        public MobileDialogPropertiesBuilder DialogMode(DialogMode dialogMode)
        {
            mobileDialog.MobileDialogModel.DialogMode = dialogMode;
            return this;
        }

        /// <summary>
        /// leftButtonCaption.
        /// </summary>
        /// <param name="leftButtonCaption">leftButtonCaption.</param>
        /// <returns></returns>
        public MobileDialogPropertiesBuilder LeftButtonCaption(string leftButtonCaption)
        {
            mobileDialog.MobileDialogModel.LeftButtonCaption = leftButtonCaption;
            return this;
        } 
        /// <summary>
        /// rightButtonCaption.
        /// </summary>
        /// <param name="rightButtonCaption">rightButtonCaption.</param>
        /// <returns></returns>
        public MobileDialogPropertiesBuilder RightButtonCaption(string rightButtonCaption)
        {
            mobileDialog.MobileDialogModel.RightButtonCaption = rightButtonCaption;
            return this;
        } 
        /// <summary>
        /// title.
        /// </summary>
        /// <param name="title">title.</param>
        /// <returns></returns>
        public MobileDialogPropertiesBuilder Title(string title)
        {
            mobileDialog.MobileDialogModel.Title = title;
            return this;
        } 
        /// <summary>
        /// template.
        /// </summary>
        /// <param name="template">template.</param>
        /// <returns></returns>
        public MobileDialogPropertiesBuilder Template(string template)
        {
            mobileDialog.MobileDialogModel.Template = template;
            return this;
        } 
        /// <summary>
        /// targetHeight.
        /// </summary>
        /// <param name="targetHeight">targetHeight.</param>
        /// <returns></returns>
        public MobileDialogPropertiesBuilder TargetHeight(int targetHeight)
        {
            mobileDialog.MobileDialogModel.TargetHeight = targetHeight;
            return this;
        }

        /// <summary>
        /// content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>DialogBuilder.</returns>
        public MobileDialogPropertiesBuilder Content(Action<MobileDialogProperties> content)
        {
            mobileDialog.MobileDialogModel.Content.WebFormDataTemplate = content;
            return this;
        }
        /// <summary>
        /// Content.
        /// </summary>
        /// <param name="Content">The Content.</param>
        /// <returns>DialogBuilder.</returns>
        public MobileDialogPropertiesBuilder Content(Func<MobileDialogProperties, object> content)
        {
            mobileDialog.MobileDialogModel.Content.RazorViewTemplate = content;
            return this;
        }

        /// <summary>
        /// Clients the side events.
        /// </summary>
        /// <param name="clientSideEvents">The client side events.</param>
        /// <returns></returns>
        public MobileDialogPropertiesBuilder ClientSideEvents(Action<MobileDialogClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MobileDialogClientSideEventsBuilder(this.mobileDialog.MobileDialogModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        /// <summary>
        /// Windowses the specified windows model.
        /// </summary>
        /// <param name="windowsModel">The windows model.</param>
        /// <returns></returns>
        public MobileDialogPropertiesBuilder Windows(Action<DialogRenderModePropertiesBuilder> windowsModel)
        {
            var builder = new DialogRenderModePropertiesBuilder(this.mobileDialog.MobileDialogModel);
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
            return new HtmlString(mobileDialog.Render().ToString());
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

