#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.Windows.Edit
{
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class XMLLanguage : MarkupLanguageBase
    {
        #region Constructor

        /// <summary>
        ///
        /// </summary>
        /// <param name="control"></param>
        public XMLLanguage(EditControl control)
            : base(control)
        {
            ResourceDictionary dictionary = new ResourceDictionary();
            dictionary.Source = new Uri("/Syncfusion.Edit.Wpf;component/Languages/LanguageResources.xaml", UriKind.Relative);
            IsSplitTextToWords = true;
            this.Lexem = dictionary["XMLLexems"] as LexemCollection;
            this.Formats = dictionary["XAMLFormats"] as FormatsCollection;
            this.Name = "XML";
            this.FileExtension = ".xml";
            this.TextForeground = Brushes.Brown;
            this.BlockStart = @"<[\w:.]+";
            this.BlockEnd = @"</[\w:.]+";
            this.ApplyColoring = true;
            this.CaseSensitive = false;
            this.SupportsOutlining = true;
        }

        #endregion Constructor
    }
}