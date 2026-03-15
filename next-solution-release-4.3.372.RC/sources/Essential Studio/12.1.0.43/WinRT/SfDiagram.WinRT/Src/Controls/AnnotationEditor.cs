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
using System.Windows;
#if WINRT_USING
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media; 
#else
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
#endif
#if WPF
using DoubleTappedRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
#elif SILVERLIGHT
using DoubleTappedRoutedEventArgs = System.Windows.Input.GestureEventArgs;
#endif
using Syncfusion.UI.Xaml.Diagram.Utility;
using System.ComponentModel;
using Syncfusion.UI.Xaml.Diagram.Panels;
using Syncfusion.UI.Xaml.Diagram.Controller;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.UI.Xaml.Diagram.Controls
{
#if !WINRT
    [DesignTimeVisible(false)] 
#endif
    public partial class AnnotationEditor : Control, IView
    {
#if WPF
        static AnnotationEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnnotationEditor), new FrameworkPropertyMetadata(typeof(AnnotationEditor)));
        } 
#endif

        public AnnotationEditor()
        {
#if !WPF
            this.DefaultStyleKey = typeof(AnnotationEditor); 
#endif

            //Binding dataContentBinding = new Binding();
            ////dataContentBinding.Path = new PropertyPath("Content");
            ////dataContentBinding.Source = this;
            ////dataContentBinding.Mode = BindingMode;
            //dataContentBinding.RelativeSource = new RelativeSource();
            //dataContentBinding.RelativeSource.Mode = RelativeSourceMode.Self;
            //SetBinding(DataContextProperty, dataContentBinding);
#if WINRT
            this.DoubleTapped += ContentEditor_DoubleTapped; 
#elif WPF
            this.MouseDoubleClick += ContentEditor_DoubleTapped;
#elif SILVERLIGHT
            this.DoubleTap += ContentEditor_DoubleTapped;
#endif

            _mAlignTransform = new CompositeTransform();
#if WPF
            this.RenderTransform = _mAlignTransform.Transform; 
#else 
            this.RenderTransform = _mAlignTransform; 
#endif
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(null, null);
            }
        }
        
        private SharedData _mSharedData;

        internal void SetSharedData(SharedData sharedData)
        {
            _mSharedData = sharedData;
        }

        internal Size _mAvailableSize;
        internal Size _mDesiredSize;
        internal Size _mActualSize;
        private CompositeTransform _mAlignTransform;

        internal Point? AnnotateAt
        {
            get { return _annotateAt; }
            set
            {
                _annotateAt = value;
                InvalidateArrange();
            }
        }
        internal double AnotationAngle;
        //internal HorizontalAlignment _mAlign;

        protected override Size MeasureOverride(Size availableSize)
        {
            _mAvailableSize = availableSize;
            Size newSize = base.MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
            if (newSize != _mDesiredSize)
            {
                _mDesiredSize = newSize;
                _mAnnotationPanel.ForceInvalidateMeasure();
            }
            return new Size(0, 0);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _mActualSize = base.ArrangeOverride(_mDesiredSize);
            //_mAlignTransform.Rotation = AnotationAngle;
            Point _mPos = new Point(0,0);
            if (AnnotateAt != null)
            {
                _mAvailableSize = new Size(0,0);
                _mPos = AnnotateAt.Value;
            }
            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    _mAlignTransform.TranslateX = _mPos.X - _mAvailableSize.Valid().Width / 2;
                    break;
                case HorizontalAlignment.Right:
                    _mAlignTransform.TranslateX = _mPos.X + _mAvailableSize.Valid().Width / 2 - _mActualSize.Width;
                    break;
                case HorizontalAlignment.Center:
                case HorizontalAlignment.Stretch:
                    _mAlignTransform.TranslateX = _mPos.X - _mActualSize.Width / 2;
                    break;
            }
            switch (VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    _mAlignTransform.TranslateY = _mPos.Y - _mAvailableSize.Valid().Height/2;
                    break;
                case VerticalAlignment.Bottom:
                    _mAlignTransform.TranslateY = _mPos.Y + _mAvailableSize.Valid().Height / 2 - _mActualSize.Height;
                    break;
                case VerticalAlignment.Center:
                case VerticalAlignment.Stretch:
                    _mAlignTransform.TranslateY = _mPos.Y - _mActualSize.Height / 2;
                    break;
            }
            return new Size(0, 0);
        }

        private ContentPresenter _mPART_ContentPresenter;
        private AnnotationPanel _mAnnotationPanel;
        private Point? _annotateAt;

        private void DoApplyTemplate()
        {
            _mPART_ContentPresenter = GetTemplateChild("PART_ContentPresenter") as ContentPresenter;
            _mAnnotationPanel = this.FindVisualParent<AnnotationPanel>();
            UpdateTemplate();
        }
        
        void ContentEditor_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            Mode = ContentEditorMode.Edit;
        }

        protected virtual void StartEditing()
        {
            if (_mPART_ContentPresenter != null)
            {
                _mPART_ContentPresenter.ContentTemplate = EditTemplate;
            }
        }

        protected virtual void CloseEditing()
        {
            if (_mPART_ContentPresenter != null)
            {
                _mPART_ContentPresenter.ContentTemplate = ViewTemplate;
            }
        }

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(AnnotationEditor), new PropertyMetadata(null, OnContentChanged));

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public DataTemplate ViewTemplate
        {
            get { return (DataTemplate)GetValue(ViewTemplateProperty); }
            set { SetValue(ViewTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewTemplateProperty =
            DependencyProperty.Register("ViewTemplate", typeof(DataTemplate), typeof(AnnotationEditor), new PropertyMetadata(null));

        public DataTemplate EditTemplate
        {
            get { return (DataTemplate)GetValue(EditTemplateProperty); }
            set { SetValue(EditTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EditTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditTemplateProperty =
            DependencyProperty.Register("EditTemplate", typeof(DataTemplate), typeof(AnnotationEditor), new PropertyMetadata(null));
        

        public ContentEditorMode Mode
        {
            get { return (ContentEditorMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentEditorModeMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(ContentEditorMode), typeof(AnnotationEditor), new PropertyMetadata(ContentEditorMode.View, OnModeChanged));

        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnnotationEditor editor = (AnnotationEditor) d;
            editor.UpdateTemplate();
        }

        private void UpdateTemplate()
        {
            if (Mode == ContentEditorMode.Edit)
            {
                StartEditing();
                if (_mSharedData != null)
                {
                    _mSharedData.Graph.CurrentEditor = this;
                }
            }
            else
            {
                CloseEditing();
            }
        }

        //public object GetSource()
        //{
        //    return (object) GetValue(SourceProperty);
        //}

        //public void SetSource(object value)
        //{
        //    SetValue(SourceProperty, value);
        //}

        //// Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty SourceProperty =
        //    DependencyProperty.RegisterAttached("Source", typeof(object), typeof(ContentEditor), new PropertyMetadata(null));


        private void BindToINode(IAnnotation source)
        {
            if (source != null)
            {
                Binding bind;
                foreach (var property in AnnotationEditorConstants.AnnotationEditorProperties)
                {
                    bind = new Binding();
                    bind.Path = new PropertyPath(property.Item2);
                    bind.Mode = BindingMode.TwoWay;
                    //bind.Source = source;
                    this.SetBinding(property.Item1, bind);
                }
            }
        }

        private object BusinessObject { get; set; }

        void IView.SetBusinessObject(object item)
        {
            this.BusinessObject = item;
            if (BusinessObject is IAnnotation)
            {
                BindToINode(BusinessObject as IAnnotation);
            }
        }

        public object Key
        {
            get { return GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Key.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(object), typeof(AnnotationEditor), new PropertyMetadata(null));
        
        public object ID
        {
            get { return GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IDProperty =
            DependencyProperty.Register("ID", typeof(object), typeof(AnnotationEditor), new PropertyMetadata(null));
        
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public enum ContentEditorMode
    {
        View,
        Edit
    }

    public interface IAnnotation
    {
        object Content { get; set; }
        DataTemplate EditTemplate { get; set; }
        DataTemplate ViewTemplate { get; set; }
        ContentEditorMode Mode { get; set; }
    }

    public interface INodeAnnotation : IAnnotation
    {
        HorizontalAlignment HorizontalAlignment { get; set; }
        VerticalAlignment VerticalAlignment { get; set; }
    }

    public interface IConnectorAnnotation : IAnnotation
    {
        ConnectorAnnotationAlignment Alignment { get; set; }
        //ConnectorAnnotationOrientation Orientation { get; set; }
    }

    public enum ConnectorAnnotationAlignment
    {
        Center,
        Source,
        Target
    }

    //public enum ConnectorAnnotationOrientation
    //{
    //    Auto,
    //    Horizontal,
    //    Vertical
    //}
}
