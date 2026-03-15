#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.OlapChart.WPF.VisualStudio.Design
{
    /// <summary>
    /// Navigation buttons status in wizard control.
    /// </summary>
    interface IWizardNavigationButtonsStatus
    {
        bool CanBackButtonEnabled { get; set; }
        bool CanFinishButtonEnabled { get; set; }
        bool CanNextButtonEnabled { get; set; }
        void UpdateButtonStatus(bool back, bool next, bool finish);
    }
}
