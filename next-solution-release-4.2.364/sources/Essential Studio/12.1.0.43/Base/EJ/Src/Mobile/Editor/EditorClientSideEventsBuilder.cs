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
    public class MobileEditorClientSideEventsBuilder
    {
        #region Fields

        private MobileEditorProperties mEditorModel;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileEditorClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mEditorModel">The m Editor model.</param>
        public MobileEditorClientSideEventsBuilder(MobileEditorProperties mEditorModel)
        {
            this.mEditorModel = mEditorModel;
        }
        #endregion

        #region Events

        /// <summary>
        /// Triggered during value change.
        /// </summary>
        /// <param name="change">The change.</param>
        /// <returns></returns>
        public MobileEditorClientSideEventsBuilder Change(string change)
        {
            mEditorModel.Change = change;
            return this;
        }

        #endregion
    }
}
