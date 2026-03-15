#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Edit
{
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class TextLanguage : LanguageBase
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="control"></param>
        public TextLanguage(EditControl control)
            : base(control)
        {
            this.Name = "Text";
            this.FileExtension = ".txt";
            this.ApplyColoring = false;
            this.TextForeground = control.Foreground;
            this.SupportsOutlining = false;
            this.SplitWordsRegex = @"\w+[\s|\W]|[\s|\W]+";
            this.IsIndentSelectionOnTabEnabled = false;
        }
    }
}