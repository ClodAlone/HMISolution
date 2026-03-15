#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives

#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    internal interface ITreeNodeAdvPaintFilter
    {
        bool OnBeforeNodePaint(TreeNodeAdvPaintEventArgs e);
        bool OnNodeBackgroundPaint(TreeNodeAdvPaintBackgroundEventArgs e);
        void OnAfterNodePaint(TreeNodeAdvPaintEventArgs e);
    }
}