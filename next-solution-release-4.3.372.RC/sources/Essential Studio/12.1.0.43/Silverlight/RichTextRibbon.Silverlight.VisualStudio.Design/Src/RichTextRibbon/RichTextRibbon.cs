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
using Microsoft.Windows.Design.Metadata;

namespace Syncfusion.RichTextRibbon.Silverlight.VisualStudio.Design.RichTextRibbon
{
    internal static class RichTextRibbon
    {
        public static readonly TypeIdentifier TypeId = new TypeIdentifier("Syncfusion.Windows.Tools.Controls.RichTextRibbon");
        public static readonly PropertyIdentifier TextCommandProperty = new PropertyIdentifier(TypeId, "Command");        
    }
}
