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

namespace Syncfusion.OlapGrid.WPF.VisualStudio.Design
{
    /// <summary>
    /// This is the presenter. This will be moved to the Shared.WPF. Name should be changed to OlapControlDesignerPresenter.
    /// This will be a backing class for a Window. Which is going to be the wizard.
    /// </summary>
    public sealed class OlapGridDesignerPresenter
    {
        #region Properties

        public CreateEditDialogView View
        {
            get;
            set;
        }

        public OlapGridDesignerModel Model
        {
            get;
            set;
        }

        #endregion

        #region Constructor

        public OlapGridDesignerPresenter(OlapGridDesignerModel model, CreateEditDialogView view)
        {
            this.View = view;
            this.Model = model;
        }

        #endregion
    }
}
