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
#if WINRT_USING
using System.Threading.Tasks; 
#endif
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Controls;

namespace Syncfusion.UI.Xaml.Diagram
{
    internal sealed partial class AnnotationEditorWrapper : WrapperBase
    {
        public AnnotationEditor View { get; set; }

        public AnnotationEditorWrapper(IAnnotation source, SharedData shared)
            : base(shared)
        {
            Source = source;
            if (View != null)
            {
                View.SetSharedData(shared);
            }
        }

        protected override void SharedDataInitialized()
        {
        }

        protected override void SourceChanged()
        {
            if (_mSource is AnnotationEditor)
            {
                View = _mSource as AnnotationEditor;
                View.DataContext = _mSource;
                (View as IView).SetBusinessObject(_mSource);
            }
            else
            {
                View = new AnnotationEditor();
                View.DataContext = _mSource;
                (View as IView).SetBusinessObject(_mSource);
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {

        }
    }
}
