#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Silverlight.Grid.Olap
{
    /// <summary>
    /// Implements a GroupBox control
    /// </summary>
    
    public class GroupBox :  ContentControl 
    { 
        
        private RectangleGeometry FullRect; 
        private RectangleGeometry HeaderRect; 
        private ContentControl HeaderContainer; 
        
        public GroupBox() 
        { 
            DefaultStyleKey = typeof(GroupBox);
            this.SizeChanged += GroupBox_SizeChanged;
        } 
        
        public override void OnApplyTemplate() 
        { 
            base.OnApplyTemplate(); 
            
            FullRect = (RectangleGeometry)GetTemplateChild("FullRect"); 
            HeaderRect = (RectangleGeometry)GetTemplateChild("HeaderRect"); 
            HeaderContainer = (ContentControl)GetTemplateChild("HeaderContainer");
            HeaderContainer.SizeChanged += HeaderContainer_SizeChanged;
        } 
        
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(GroupBox), null); 
        public object Header { 
            get { return GetValue(HeaderProperty); } 
            set { SetValue(HeaderProperty, value); } 
        } 
        
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(GroupBox), null); 
        public DataTemplate HeaderTemplate { 
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); } 
            set { SetValue(HeaderTemplateProperty, value); } 
        }

        private void GroupBox_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            FullRect.Rect = new Rect(new Point(), e.NewSize);
        }

        private void HeaderContainer_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            HeaderRect.Rect = new Rect(new Point(HeaderContainer.Margin.Left, 0), e.NewSize);
        } 
    } 
}
