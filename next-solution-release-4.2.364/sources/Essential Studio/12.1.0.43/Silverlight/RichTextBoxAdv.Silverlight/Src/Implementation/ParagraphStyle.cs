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
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    public class ParagraphStyle:DependencyObject,INotifyPropertyChanged
    {
        private RichTextBoxAdv richTextBox;
        private bool isStyleChanged = false;
        private TextAlignment textAlignment = System.Windows.TextAlignment.Left;
        private double leftIndent;
        private double rightIndent;
        private double afterSpacing;
        private double beforeSpacing;
        private double lineSpaing;
        private ListType listType;

        /// <summary>
        /// Initializes the new instance of ParagraphStyle class
        /// </summary>
        /// <param name="richTextBoxAdv"></param>
        public ParagraphStyle(RichTextBoxAdv richTextBoxAdv)
        {
            richTextBox = richTextBoxAdv;
        }

        
        /// <summary>
        /// 
        /// </summary>
        public RichTextBoxAdv RichTextBox
        {
            get
            {
                return richTextBox;
            }
        }

#if !WPF
        /// <summary>
        /// Gets the text alignment of the current paragraph
        /// </summary>
        public TextAlignment TextAlignment
        {
            get
            {
                return textAlignment;
            }
            internal set
            {
                isStyleChanged = textAlignment != value;
                textAlignment = value;
                OnPropertyChanged("TextAlignment");
                OnStyleChanged();
            }
        }

        /// <summary>
        /// Gets ListType of the current paragraph
        /// </summary>
        public ListType ListType
        {
            get
            {
                return listType;
            }
            internal set
            {
                isStyleChanged = listType != value;
                listType = value;
                OnPropertyChanged("ListType");
                OnStyleChanged();
            }
        }

        /// <summary>
        /// Gets the line spacing of the current paragraph
        /// </summary>
        public double LineSpacing
        {
            get
            {
                return lineSpaing;
            }
            internal set
            {
                isStyleChanged = lineSpaing != value;
                lineSpaing = value;
                OnPropertyChanged("LineSpacing");
                OnStyleChanged();
            }
        }

        /// <summary>
        /// Gets the after spacing of the current paragraph
        /// </summary>
        public double AfterSpacing
        {
            get
            {
                return afterSpacing;
            }
            internal set
            {
                isStyleChanged = afterSpacing != value;
                afterSpacing = value;
                OnPropertyChanged("AfterSpacing");
                OnStyleChanged();
            }
        }

        /// <summary>
        /// Gets the before spacing of the current paragraph
        /// </summary>
        public double BeforeSpacing
        {
            get
            {
                return beforeSpacing;
            }
            internal set
            {
                isStyleChanged = beforeSpacing != value;
                beforeSpacing = value;
                OnPropertyChanged("BeforeSpacing");
                OnStyleChanged();
            }
        }

        /// <summary>
        /// Gets the left indent of the current paragraph
        /// </summary>
        public double LeftIndent
        {
            get
            {
                return leftIndent;
            }
            internal set
            {
                isStyleChanged = leftIndent != value;
                leftIndent = value;
                OnPropertyChanged("LeftIndent");
                OnStyleChanged();
            }
        }

        /// <summary>
        /// Gets the right indent of the current paragraph
        /// </summary>
        public double RightIndent
        {
            get
            {
                return rightIndent;
            }
            internal set
            {
                isStyleChanged = rightIndent != value;
                rightIndent = value;
                OnPropertyChanged("RightIndent");
                OnStyleChanged();
            }
        }
#endif
#if WPF

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            internal set { SetValue(TextAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(ParagraphStyle), new FrameworkPropertyMetadata(TextAlignment.Left,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,new PropertyChangedCallback(OnTextAlignmentChanged)));


        private static void OnTextAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ParagraphStyle parastyle = d as ParagraphStyle;
            parastyle.OnTextAlignmentChanged(e);
        }

        private void OnTextAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            isStyleChanged = e.NewValue != e.OldValue;
            OnStyleChanged();
        }

        public ListType ListType
        {
            get { return (ListType)GetValue(ListTypeProperty); }
            internal set { SetValue(ListTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ListType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListTypeProperty =
            DependencyProperty.Register("ListType", typeof(ListType), typeof(ParagraphStyle), new FrameworkPropertyMetadata(ListType.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnListTypeChanged)));

        private static void OnListTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ParagraphStyle parastyle = d as ParagraphStyle;
            parastyle.OnListTypeChanged(e);
        }

        private void OnListTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            isStyleChanged = e.NewValue != e.OldValue;
            OnStyleChanged();
        }


        public double LineSpacing
        {
            get { return (double)GetValue(LineSpacingProperty); }
            internal set { SetValue(LineSpacingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineSpacing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineSpacingProperty =
            DependencyProperty.Register("LineSpacing", typeof(double), typeof(ParagraphStyle), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnLineSpacingChanged)));

        private static void OnLineSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ParagraphStyle parastyle = d as ParagraphStyle;
            parastyle.OnLineSpacingChanged(e);
        }

        private void OnLineSpacingChanged(DependencyPropertyChangedEventArgs e)
        {
            isStyleChanged = e.NewValue != e.OldValue;
            OnStyleChanged();
        }

        public double AfterSpacing
        {
            get { return (double)GetValue(AfterSpacingProperty); }
            internal set { SetValue(AfterSpacingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AfterSpacing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AfterSpacingProperty =
            DependencyProperty.Register("AfterSpacing", typeof(double), typeof(ParagraphStyle), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnAfterSpacingChanged)));

        private static void OnAfterSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ParagraphStyle parastyle = d as ParagraphStyle;
            parastyle.OnAfterSpacingChanged(e);
        }

        private void OnAfterSpacingChanged(DependencyPropertyChangedEventArgs e)
        {
            isStyleChanged = e.NewValue != e.OldValue;
            OnStyleChanged();
        }

        public double BeforeSpacing
        {
            get { return (double)GetValue(BeforeSpacingProperty); }
            internal set { SetValue(BeforeSpacingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BeforeSpacing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BeforeSpacingProperty =
            DependencyProperty.Register("BeforeSpacing", typeof(double), typeof(ParagraphStyle), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnBeforeSpacingChanged)));

        private static void OnBeforeSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ParagraphStyle parastyle = d as ParagraphStyle;
            parastyle.OnBeforeSpacingChanged(e);
        }

        private void OnBeforeSpacingChanged(DependencyPropertyChangedEventArgs e)
        {
            isStyleChanged = e.NewValue != e.OldValue;
            OnStyleChanged();
        }

        public double LeftIndent
        {
            get { return (double)GetValue(LeftIndentProperty); }
            internal set { SetValue(LeftIndentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftIndent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftIndentProperty =
            DependencyProperty.Register("LeftIndent", typeof(double), typeof(ParagraphStyle), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnLeftIndentChanged)));

        private static void OnLeftIndentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ParagraphStyle parastyle = d as ParagraphStyle;
            parastyle.OnLeftIndentChanged(e);
        }

        private void OnLeftIndentChanged(DependencyPropertyChangedEventArgs e)
        {
            isStyleChanged = e.NewValue != e.OldValue;
            OnStyleChanged();
        }
        
        public double RightIndent
        {
            get { return (double)GetValue(RightIndentProperty); }
            internal set { SetValue(RightIndentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightIndent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightIndentProperty =
            DependencyProperty.Register("RightIndent", typeof(double), typeof(ParagraphStyle), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnRightIndentChanged)));

        private static void OnRightIndentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ParagraphStyle parastyle = d as ParagraphStyle;
            parastyle.OnRightIndentChanged(e);
        }

        private void OnRightIndentChanged(DependencyPropertyChangedEventArgs e)
        {
            isStyleChanged = e.NewValue != e.OldValue;
            OnStyleChanged();
        }

#endif

        /// <summary>
        /// 
        /// </summary>
        private void OnStyleChanged()
        {
            if (isStyleChanged)
            {
                richTextBox.OnStyleChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void SetDefaultStyle()
        {
            BeforeSpacing = 0d;
            AfterSpacing = 0d;
            TextAlignment = TextAlignment.Left;
            LineSpacing = 1d;
            LeftIndent = 0d;
            RightIndent = 0d;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="para"></param>
        internal void SetParagraphStyle(ParagraphAdv para)
        {
            if (para != null)
            {
                LineSpacing = para.LineSpacing;
                LeftIndent = para.LeftIndent;
                RightIndent = para.RightIndent;
                AfterSpacing = para.AfterSpacing;
                BeforeSpacing = para.BeforeSpacing;
                TextAlignment = para.TextAlignment;
            }
        }

        internal void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
;