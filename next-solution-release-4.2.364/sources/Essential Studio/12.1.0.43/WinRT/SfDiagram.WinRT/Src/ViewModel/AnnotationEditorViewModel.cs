#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup; 
using Windows.UI.Xaml.Controls; 
#else
using System.Windows.Controls;
using System.Windows.Markup; 
#endif
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Utility;

namespace Syncfusion.UI.Xaml.Diagram
{
    public partial class AnnotationEditorViewModel : IAnnotation, INodeAnnotation, IConnectorAnnotation, INotifyPropertyChanged
    {
        private static DataTemplate viewTemplate;
        private static DataTemplate editTemplate;
        static AnnotationEditorViewModel()
        {
            const string vTemplate = "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
                                     "<TextBlock Text=\"{Binding Path=Content, Mode=TwoWay}\"/>" +
                                     "</DataTemplate>";
#if WINRT
            const string eTemplate = "<DataTemplate" +
                                     " xmlns:util=\"using:Syncfusion.UI.Xaml.Diagram.Utility\"" +
                                     " xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
                                      "<TextBox util:FocusUtility.FocusOnLoad=\"True\" ManipulationMode=\"None\" AcceptsReturn=\"True\" Text=\"{Binding Path=Content, Mode=TwoWay}\"/>" +
                                      "</DataTemplate>"; 
#else
            const string eTemplate = "<DataTemplate" +
                                     " xmlns:util=\"clr-namespace:Syncfusion.UI.Xaml.Diagram.Utility\"" +
                                     " xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
                                     "<TextBox AcceptsReturn=\"True\" Text=\"{Binding Path=Content, Mode=TwoWay}\"/>" +
                                     "</DataTemplate>";
#endif
            viewTemplate = vTemplate.LoadXaml() as DataTemplate;
            editTemplate = eTemplate.LoadXaml() as DataTemplate;
        }

        public AnnotationEditorViewModel()
        {
            ViewTemplate = viewTemplate;
            EditTemplate = editTemplate;
        }

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
