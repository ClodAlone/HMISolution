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
    public class MobileDialogClientSideEventsBuilder
    {
        #region Fields
        /// <summary>
        /// The m Dialog model
        /// </summary>
        private MobileDialogProperties mobileDialogModel;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileDialogClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mobileDialogModel">The m Dialog model.</param>
        public MobileDialogClientSideEventsBuilder(MobileDialogProperties mobileDialogModel)
        {
            this.mobileDialogModel = mobileDialogModel;
        }
        #endregion

        #region Events
        /// <summary>
        /// Called when [buttonTap].
        /// </summary>
        /// <param name="onButtonTap">The buttonTap.</param>
        /// <returns></returns>
        public MobileDialogClientSideEventsBuilder ButtonTap(string buttonTap)
        {
            mobileDialogModel.ButtonTap = buttonTap;
            return this;
        }

        /// <summary>
        /// Called when [open].
        /// </summary>
        /// <param name="onOpen">The open.</param>
        /// <returns></returns>
        public MobileDialogClientSideEventsBuilder Open(string open)
        {
            mobileDialogModel.Open = open;
            return this;
        }

        /// <summary>
        /// Called when [beforeClose].
        /// </summary>
        /// <param name="onBeforeClose">The beforeClose.</param>
        /// <returns></returns>
        public MobileDialogClientSideEventsBuilder BeforeClose(string beforeClose)
        {
            mobileDialogModel.BeforeClose = beforeClose;
            return this;
        }

        /// <summary>
        /// Called when [close].
        /// </summary>
        /// <param name="onClose">The close.</param>
        /// <returns></returns>
        public MobileDialogClientSideEventsBuilder Close(string close)
        {
            mobileDialogModel.Close = close;
            return this;
        }
        
        #endregion
    }
}
