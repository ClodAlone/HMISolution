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
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{

    public class MobileListboxIOS7PropertiesBuilder
    {
        #region Fields
        private MobileListboxProperties MobileListboxModel { get; set; }
        #endregion

        #region Constructor
        public MobileListboxIOS7PropertiesBuilder(MobileListboxProperties MobileListboxModel)
        {
            this.MobileListboxModel = MobileListboxModel;
        }
        #endregion

        #region Builder

      
        public MobileListboxIOS7PropertiesBuilder Inline(bool inline)
        {
            this.MobileListboxModel.IOS7.Inline = inline;
            return this;
        }
        #endregion
    }

    public class MobileListboxWindowsPropertiesBuilder
    {
        #region Fields
        private MobileListboxProperties MobileListboxModel { get; set; }
        #endregion

        #region Constructor
        public MobileListboxWindowsPropertiesBuilder(MobileListboxProperties MobileListboxModel)
        {
            this.MobileListboxModel = MobileListboxModel;
        }
        #endregion

        #region Builder
        
        public MobileListboxWindowsPropertiesBuilder RenderDefault(bool renderDefault)
        {
            this.MobileListboxModel.Windows.RenderDefault = renderDefault;
            return this;
        }

        public MobileListboxWindowsPropertiesBuilder PreventSkewing(bool prevent)
        {
            this.MobileListboxModel.Windows.PreventSkewing = prevent;
            return this;
        }

        #endregion

    }
}
