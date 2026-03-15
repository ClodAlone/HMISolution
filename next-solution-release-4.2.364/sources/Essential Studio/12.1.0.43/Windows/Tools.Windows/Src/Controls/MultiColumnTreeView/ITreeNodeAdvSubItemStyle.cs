#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System.Drawing;

using Syncfusion.Drawing;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    public interface ITreeNodeAdvSubItemStyle
    {
        string BaseStyle { get; set; }

        void ResetBaseStyle();

        bool ShouldSerializeBaseStyle();

        Font Font { get; set; }

        void ResetFont();

        bool ShouldSerializeFont();

        string Text { get; set; }
        void ResetText();

        bool ShouldSerializeText();

        Color TextColor { get; set; }

        void ResetTextColor();

        bool ShouldSerializeTextColor();

        string HelpText { get; set; }

        void ResetHelpText();

        bool ShouldSerializeHelpText();

        object Tag { get; set; }

        void ResetTag();

        bool ShouldSerializeTag();

        Image LeftImage { get; set; }

        void ResetLeftImage();

        bool ShouldSerializeLeftImage();

        int[] LeftImageIndices { get; set; }

        void ResetLeftImageIndices();

        bool ShouldSerializeLeftImageIndices();

        int LeftImagePadding { get; set; }
 
        void ResetLeftImagePadding();

        bool ShouldSerializeLeftImagePadding();

        Image RightImage { get; set; }

        void ResetRightImage();

        bool ShouldSerializeRightImage();

        int[] RightImageIndices { get; set; }

        void ResetRightImageIndices();

        bool ShouldSerializeRightImageIndices();

        int RightImagePadding { get; set; }

        void ResetRightImagePadding();
      
        bool ShouldSerializeRightImagePadding();
        
        BrushInfo Background { get; set; }
     
        void ResetBackground();
       
        bool ShouldSerializeBackground();
    }
}