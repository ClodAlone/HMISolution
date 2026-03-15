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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Documents;

namespace Syncfusion.Windows.Chart
{
    class ContentPanel:WrapPanel
    {
        #region Properties
        internal double constraintHeight = 0d;
        internal double constraintWidth = 0d;
        internal ContentPresenter contentPresenter = null;
        internal TransformGroup group = new TransformGroup();

        public static readonly DependencyProperty IntersectActionProperty
      = DependencyProperty.Register("IntersectAction", typeof(TextIntersectActions), typeof(ContentPanel), new FrameworkPropertyMetadata(TextIntersectActions.Shrink));
        public TextIntersectActions IntersectAction
        {
            get { return (TextIntersectActions)this.GetValue(ContentPanel.IntersectActionProperty); }
            set { this.SetValue(ContentPanel.IntersectActionProperty, value); }
        }

        #endregion

        #region Constructor
        public ContentPanel()
        {

        }
        #endregion

        internal Size finalSize_ = new Size();
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            foreach (UIElement ele in this.Children)
            {
                Canvas canvas = ele as Canvas;
                DependencyObject obj = VisualTreeHelper.GetChild(canvas, 0);
                TextBlock block = obj as TextBlock;
                finalSize_ = finalSize;
                if (contentPresenter != null)
                {
                    ArrangeElement(block, finalSize_);
                }
                else
                {
                    block.Loaded += new RoutedEventHandler(block_Loaded);
                }
            }
            base.ArrangeOverride(finalSize);           
            return finalSize;            
        }

        void block_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock block = sender as TextBlock;
            if (block.DesiredSize.Height != 0 && block.DesiredSize.Width != 0)
            {
                DependencyObject obj = VisualTreeHelper.GetChild(block, 0);
                obj = VisualTreeHelper.GetChild(obj, 0);
                ContentPresenter presenter = obj as ContentPresenter;
                contentPresenter = presenter;
                ArrangeElement(block, finalSize_);
            }
        }

          private void ArrangeElement(TextBlock block, Size finalSize)
          {
            if (contentPresenter != null)
            {
                if (contentPresenter.ContentTemplate != null && contentPresenter.ContentTemplate.HasContent)
                {
                    if (((contentPresenter.ContentTemplate.LoadContent() as FrameworkElement).HorizontalAlignment == HorizontalAlignment.Stretch) && ((contentPresenter.ContentTemplate.LoadContent() as FrameworkElement).VerticalAlignment == VerticalAlignment.Stretch))
                    {                    
                        TranslateTransform translate = new TranslateTransform();
                        block.Measure(new Size(2000, 2000));
                        if (group.Children.Count > 0)
                            translate.X = constraintWidth;
                        else
                            translate.X = 0;

                        translate.Y = 0;
                        group.Children.Add(translate);
                       
                        DependencyObject obj = VisualTreeHelper.GetParent(block);
                        Canvas canvas = obj as Canvas;
                        block.Measure(new Size(2000, 2000));

                        block.Measure(finalSize);
                        if (block.RenderTransform is TransformGroup)
                        {
                            if ((block.RenderTransform as TransformGroup).Children.Count > 1)
                            {
                                block.Arrange(new Rect(new Size(finalSize.Height, finalSize.Width)));
                                canvas.Arrange(new Rect(new Size(finalSize.Height, finalSize.Width)));
                                contentPresenter.Arrange(new Rect(new Size(finalSize.Height, finalSize.Width)));
                            }
                            else
                            {
                                block.Arrange(new Rect(finalSize));
                                canvas.Arrange(new Rect(finalSize));
                                contentPresenter.Arrange(new Rect(finalSize));
                            }
                        }

                    }               
                }
            }
          }
        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            foreach (UIElement ele in this.Children)
            {
                double fontsize = 0d;
                if (ele is Canvas)
                {
                    group = new TransformGroup();
                    Canvas canvas1 = ele as Canvas;
                    RotateTransform rotate1 = new RotateTransform();
                    rotate1.Angle = 0;
                    canvas1.Children[0].RenderTransform = rotate1;
                    canvas1.RenderTransform = rotate1;
                    Canvas.SetLeft(canvas1.Children[0], 0);
                    ele.Measure(new Size(2000, 2000));
                 
                    Canvas canvas = ele as Canvas;
                    DependencyObject obj = VisualTreeHelper.GetChild(canvas, 0);
                    TextBlock text = obj as TextBlock;
                    if (this.TemplatedParent != null && (this.TemplatedParent is HeatMapItem) && (this.TemplatedParent as HeatMapItem).ParentHeatMapControl != null)
                    {
                        HeatMapControl control = ((this.TemplatedParent as HeatMapItem).ParentHeatMapControl) as HeatMapControl;
                        if (control != null)
                        {
                            text.FontSize = control.LabelFontSize<0 || double.IsNaN(control.LabelFontSize)? 12: control.LabelFontSize;
                        }
                    }
                    text.Opacity = 1;
                    text.Measure(new Size(2000, 2000));
                    
                    fontsize = text.FontSize;
                    double width = text.DesiredSize.Width;

                    switch (this.IntersectAction)
                    {
                        case TextIntersectActions.Wrap:
                            string prevText = string.Empty;
                            //prevText = text.DataContext.ToString();
                            if (text.Text.Contains("\r\n"))
                            {
                               text.DataContext = text.Text.Replace("\r\n", " ");
                               text.Text = text.DataContext.ToString();
                            }
                            string data = text.DataContext.ToString();
                            List<string> bitData = new List<string>();
                            text.Measure(new Size(2000, 2000));
                            if (text.DesiredSize.Width > constraint.Width && text.DesiredSize.Height + 10 < constraint.Height)
                            {
                                while (data.Contains(" "))
                                {
                                    int index = data.IndexOf(" ");
                                    bitData.Add(data.Remove(index));
                                    data = data.Substring(index + 1);
                                    TextBlock newBlock = new TextBlock();
                                    newBlock.Text = data;
                                    newBlock.Measure(new Size(2000, 2000));
                                    double newwidth = newBlock.DesiredSize.Width;
                                    if (newwidth < constraint.Width && newBlock.DesiredSize.Height + 10 < constraint.Height)
                                        break;
                                }
                                bitData.Add(data);
                                text.DataContext = string.Empty;
                                if (bitData.Count > 1)
                                {
                                    text.Text = string.Empty;
                                    int i = 0;
                                    foreach (var item in bitData)
                                    {
                                        text.Inlines.Add(item);
                                        if (i < bitData.Count - 1)
                                        {
                                            text.Inlines.Add(new LineBreak());
                                        }
                                        i++;
                                    }
                                }
                            }
                            break;

                        case TextIntersectActions.Shrink:
                            #region ShrinkingText
                            if (canvas.Children[0].DesiredSize.Width > constraint.Width && canvas.Children[0].DesiredSize.Width + 10 < constraint.Height)
                            {
                                double width1 = canvas.Children[0].DesiredSize.Height;
                                double height1 = canvas.Children[0].DesiredSize.Width;
                                this.Orientation = Orientation.Vertical;
                                if (canvas != null && canvas.Children.Count > 0)
                                {
                                    width1 = canvas.Children[0].DesiredSize.Height;
                                    RotateTransform rotate = new RotateTransform();
                                    rotate.Angle = 90;
                                    group.Children.Add(rotate);
                                    constraintHeight = constraint.Height;
                                    constraintWidth = constraint.Width;

                                }
                                if (contentPresenter != null)
                                {
                                    setAllignment(text);
                                }
                                else
                                {
                                    text.Loaded += new RoutedEventHandler(text_Loaded);
                                    text.Unloaded += new RoutedEventHandler(text_Unloaded);
                                }
                                return new Size(width1, height1);
                            }
                            else
                            {
                                while (width + 10 > constraint.Width)
                                {
                                    if (text.FontSize > 1)
                                    {
                                        text.FontSize = text.FontSize - 0.5;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    text.Measure(new Size(2000, 2000));
                                    width = text.DesiredSize.Width;

                                    if (canvas.Children[0].DesiredSize.Width > constraint.Width && canvas.Children[0].DesiredSize.Width + 10 < constraint.Height)
                                    {
                                        double width1 = canvas.Children[0].DesiredSize.Height;
                                        double height1 = canvas.Children[0].DesiredSize.Width;
                                        this.Orientation = Orientation.Vertical;
                                        if (canvas != null && canvas.Children.Count > 0)
                                        {
                                            width1 = canvas.Children[0].DesiredSize.Height;
                                            RotateTransform rotate = new RotateTransform();
                                            rotate.Angle = 90;
                                            group.Children.Add(rotate);
                                            constraintHeight = constraint.Height;
                                            constraintWidth = constraint.Width;

                                        }
                                        if (contentPresenter != null)
                                        {
                                            setAllignment(text);
                                        }
                                        else
                                        {
                                            text.Loaded += new RoutedEventHandler(text_Loaded);
                                            text.Unloaded += new RoutedEventHandler(text_Unloaded);
                                        }
                                        return new Size(width1, height1);
                                    }
                                }

                            }
                            while (width + 30 < constraint.Width)
                            {
                                if (text.FontSize < 14)
                                {
                                    text.FontSize = text.FontSize + 0.5;
                                }
                                else
                                {
                                    break;
                                }
                                text.Measure(new Size(2000, 2000));
                                width = text.DesiredSize.Width;
                            }
#endregion
                            break;
                    }
                    constraintHeight = constraint.Height;
                    constraintWidth = constraint.Width;
                    
                    text.Loaded += new RoutedEventHandler(text_Loaded);
                    text.Unloaded += new RoutedEventHandler(text_Unloaded);
                    if (contentPresenter != null)
                    {
                        setAllignment(text);
                    }
                    if (text.FontSize < 2)
                    {
                        text.Opacity = 0;
                    }
                    
                 
                }      
            }
            return base.MeasureOverride(constraint);
        }

       
        void setAllignment(TextBlock block)
        {
            if (contentPresenter.ContentTemplate != null && contentPresenter.ContentTemplate.HasContent)
            {
           
                if ((contentPresenter.ContentTemplate.LoadContent() as FrameworkElement).HorizontalAlignment == HorizontalAlignment.Left)
                {
                    TranslateTransform translate = new TranslateTransform();
                    if (group.Children.Count > 0)                   
                        translate.X =  block.DesiredSize.Height;
                    else
                        translate.X = 0;
                    group.Children.Add(translate);
                }
                if ((contentPresenter.ContentTemplate.LoadContent() as FrameworkElement).HorizontalAlignment == HorizontalAlignment.Right)
                {
                    TranslateTransform translate = new TranslateTransform();
                    block.Measure(new Size(2000, 2000));
                    if (group.Children.Count > 0)
                    {
                        translate.X = (constraintWidth);
                    }
                    else
                    {
                        translate.X = constraintWidth - block.DesiredSize.Width;
                    }
                    group.Children.Add(translate);
                }
                if (((contentPresenter.ContentTemplate.LoadContent() as FrameworkElement).HorizontalAlignment == HorizontalAlignment.Center))
                {
                    TranslateTransform translate = new TranslateTransform();
                    block.Measure(new Size(2000, 2000));
                    if (group.Children.Count > 0)
                        translate.X = (constraintWidth / 2) + (block.DesiredSize.Height/2);
                    else
                        translate.X = (constraintWidth / 2) - (block.DesiredSize.Width / 2);
                    group.Children.Add(translate);
                  
                }
                if ((contentPresenter.ContentTemplate.LoadContent() as FrameworkElement).VerticalAlignment == VerticalAlignment.Top)
                {
                    TranslateTransform translate = new TranslateTransform();
                    translate.Y = 0;
                    group.Children.Add(translate);
                }
                if ((contentPresenter.ContentTemplate.LoadContent() as FrameworkElement).VerticalAlignment == VerticalAlignment.Bottom)
                {
                    TranslateTransform translate = new TranslateTransform();
                    block.Measure(new Size(2000, 2000));
                    if (group.Children.Count > 1)
                    {
                        translate.Y = constraintHeight - block.DesiredSize.Width;
                    }
                    else
                    {
                        translate.Y = constraintHeight - block.DesiredSize.Height;
                    }
                    group.Children.Add(translate);
                }
                if (((contentPresenter.ContentTemplate.LoadContent() as FrameworkElement).VerticalAlignment == VerticalAlignment.Center))
                {                    
                    TranslateTransform translate = new TranslateTransform();
                    block.Measure(new Size(2000, 2000));
                    if (group.Children.Count > 1)
                    {
                        translate.Y =  (constraintHeight / 2) - (block.DesiredSize.Width / 2);
                    }
                    else
                    {
                        translate.Y = (constraintHeight / 2) - (block.DesiredSize.Height / 2);
                    }
                    group.Children.Add(translate);
                  
                }
               
                block.RenderTransform = group;
            }
            else if(contentPresenter.Content != null)
            {
                Type c = contentPresenter.Content.GetType();
                TranslateTransform translate = new TranslateTransform();
                block.Measure(new Size(2000, 2000));
                if (group.Children.Count > 0)
                    translate.X = (constraintWidth / 2) + (block.DesiredSize.Height / 2);
                else
                    translate.X = (constraintWidth / 2) - (block.DesiredSize.Width / 2);
                group.Children.Add(translate);
                block.RenderTransform = group;

                translate = new TranslateTransform();
                block.Measure(new Size(2000, 2000));
                if (group.Children.Count > 1)
                {
                    translate.Y = (constraintHeight / 2) - (block.DesiredSize.Width / 2);
                }
                else
                {
                    translate.Y = (constraintHeight / 2) - (block.DesiredSize.Height / 2);
                }
                group.Children.Add(translate);

                block.RenderTransform = group;

            }
        }
    
        void text_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock block = sender as TextBlock;
            if (this.TemplatedParent != null && (this.TemplatedParent is HeatMapItem) && (this.TemplatedParent as HeatMapItem).ParentHeatMapControl != null)
            {
                HeatMapControl control = ((this.TemplatedParent as HeatMapItem).ParentHeatMapControl) as HeatMapControl;
                if (control != null)
                {             
                    Binding binding = new Binding();
                    binding.Path = new PropertyPath(HeatMapControl.LabelFontSizeProperty);
                    binding.Source = this;
                    binding.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(block, TextBlock.FontSizeProperty, binding);
                }
            }
            
            if (block.DesiredSize.Height != 0 && block.DesiredSize.Width != 0)
            {
                DependencyObject obj = VisualTreeHelper.GetChild(block, 0);
                obj = VisualTreeHelper.GetChild(obj, 0);
                ContentPresenter presenter = obj as ContentPresenter;
                contentPresenter = presenter;
                setAllignment(block);
            }
        }
        void text_Unloaded(object sender, RoutedEventArgs e)
        {
            group.Children.Clear();
        }
    }
}
