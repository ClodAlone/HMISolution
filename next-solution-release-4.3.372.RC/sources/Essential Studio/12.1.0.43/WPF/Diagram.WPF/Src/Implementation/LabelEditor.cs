// <copyright file="LabelEditor.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using System.ComponentModel;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents Before Label Edit Event argument class.
    /// </summary>
    internal class BeforeLabelEditEventArgs : EventArgs
    {
        #region Private members

        /// <summary>
        /// Used to store the header before edit.
        /// </summary>
        private object m_headerBeforeEdit;

        #endregion

        #region Public properies

        /// <summary>
        /// Gets or sets the Header before editing takes place.
        /// </summary>
        /// <value>
        /// Type: <see cref="object"/>
        /// Header value.
        /// </value>
        internal object HeaderBeforeEdit
        {
            get
            {
                return m_headerBeforeEdit;
            }

            set
            {
                m_headerBeforeEdit = value;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeLabelEditEventArgs"/> class.
        /// </summary>
        /// <param name="headerBeforeEdit">The header before edit.</param>
        public BeforeLabelEditEventArgs(object headerBeforeEdit)
        {
            m_headerBeforeEdit = headerBeforeEdit;
        }

        #endregion
    }

    /// <summary>
    /// Represents After Label Edit Event argument class.
    /// </summary>
    internal class AfterLabelEditEventArgs
    {
        #region Private members

        /// <summary>
        /// Used to store the header after edit.
        /// </summary>
        private object m_headerAfterEdit;

        #endregion

        #region Public properies

        /// <summary>
        /// Gets or sets the Header after editing takes place.
        /// </summary>
        /// <value>
        /// Type: <see cref="object"/>
        /// Header value.
        /// </value>
        internal object HeaderAfterEdit
        {
            get
            {
                return m_headerAfterEdit;
            }

            set
            {
                m_headerAfterEdit = value;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="AfterLabelEditEventArgs"/> class.
        /// </summary>
        /// <param name="headerAfterEdit">The header after edit.</param>
        public AfterLabelEditEventArgs(object headerAfterEdit)
        {
            m_headerAfterEdit = headerAfterEdit;
        }

        #endregion
    }

    /// <summary>
    /// Represents the Label Editor which allows to edit the labels of the Nodes and Connectors at run time.
    /// </summary>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public class LabelEditor : HeaderedContentControl
    {
        #region Private members

        /// <summary>
        /// Used to store Label editor.
        /// </summary>
        private LabelEditor m_editingItem = null;

        /// <summary>
        /// Used to store textbox instance.
        /// </summary>
        private TextBox m_editableHeader = null;

        /// <summary>
        /// Used to store clickinfo instance.
        /// </summary>
        private ClickInfo m_clickInfo;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="LabelEditor"/> class.
        /// </summary>
        static LabelEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LabelEditor), new FrameworkPropertyMetadata(typeof(LabelEditor)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelEditor"/> class.
        /// </summary>
        public LabelEditor()
        {
            Binding customTemplateBinding = new Binding();
            Binding useCustomTemplateBinding = new Binding();

            customTemplateBinding.Mode = BindingMode.TwoWay;
            useCustomTemplateBinding.Mode = BindingMode.TwoWay;

            customTemplateBinding.Source = this;
            useCustomTemplateBinding.Source = this;

            customTemplateBinding.Path = new PropertyPath(CustomEditableTemplateProperty);
            useCustomTemplateBinding.Path = new PropertyPath(UseCustomEditableTemplateProperty);

            SetBinding(LabelEditor.CustomEditableTemplateProperty, customTemplateBinding);
            SetBinding(LabelEditor.UseCustomEditableTemplateProperty, useCustomTemplateBinding);

            
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            this.Loaded += new RoutedEventHandler(LabelEditor_Loaded);
            base.OnInitialized(e);
        }

        void LabelEditor_Loaded(object sender, RoutedEventArgs e)
        {
            object n = GetNode(sender as LabelEditor);
            if (n is Node)
            {
                (n as Node).RectBounds = (n as Node).TransformRectBounds();
                if((n as Node).dc!=null)
                    (n as Node).dc.View.Page.InvalidateMeasure();
            }
            else if (n is LineConnector)
            {
                if((n as LineConnector).dc!=null)
                    (n as LineConnector).dc.View.Page.InvalidateMeasure();
            }
        }
        #endregion

        /// <summary>
        /// Represents the ClickInfo struct which contains the click details.
        /// </summary>
        private struct ClickInfo
        {
            /// <property name="flag" value="Finished" />
            /// <summary>
            /// Time when the last click on Node was performed.
            /// </summary>
            public DateTime LastNodeClick;

            /// <property name="flag" value="Finished" />
            /// <summary>
            /// Point where the last click on Node was performed.
            /// </summary>
            public Point LastNodePoint;

            /// <summary>
            /// Called when the mouse button is clicked twice.
            /// </summary>
            /// <param name="position">Mouse Position</param>
            /// <returns>true, if double clicked; false otherwise.</returns>
            public bool IsDoubleClick(Point position)
            {
                if (((DateTime.Now.Subtract(LastNodeClick).TotalMilliseconds < 500) && (Math.Abs((double)(LastNodePoint.X - position.X)) <= 2)) && (Math.Abs((double)(LastNodePoint.Y - position.Y)) <= 2))
                {
                    return true;
                }

                return false;
            }
        }

        #region Properties



        public TextDecorationCollection LabelTextDecorations
        {
            get { return (TextDecorationCollection)GetValue(LabelTextDecorationsProperty); }
            set { SetValue(LabelTextDecorationsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelTextDecorations.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelTextDecorationsProperty =
            DependencyProperty.Register("LabelTextDecorations", typeof(TextDecorationCollection), typeof(LabelEditor));



        /// <summary>
        /// Gets or sets the Selected Item.
        /// </summary>
        /// <value>
        /// Type: <see cref="object"/>
        /// Selected Item.
        /// </value>
        public object SelectedItem
        {
            get
            {
                return (object)GetValue(SelectedItemProperty);
            }

            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// Type: <see cref="string"/>
        /// String value.
        /// </value>
        public string Label
        {
            get
            {
                return (string)GetValue(LabelProperty);
            }

            set
            {
                SetValue(LabelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text width.
        /// </summary>
        public double TextWidth
        {
            get
            {
                return (double)GetValue(TextWidthProperty);
            }

            set
            {
                SetValue(TextWidthProperty, value);
            }
        }

        /// <summary>
        /// Checks whether CustomEditableTemplate is  been used or not.
        /// </summary>
        /// <param name="obj">The DependencyObject.</param>
        /// <returns>True, if CustomEditableTemplate is been used, false otherwise.</returns>
        public static bool GetUseCustomEditableTemplate(DependencyObject obj)
        {
            return (bool)obj.GetValue(UseCustomEditableTemplateProperty);
        }

        /// <summary>
        /// Sets the value of the UseCustomEditableTemplate dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetUseCustomEditableTemplate(DependencyObject obj, bool value)
        {
            obj.SetValue(UseCustomEditableTemplateProperty, value);
        }

        /// <summary>
        /// Gets the value of the CustomEditableTemplate dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject.</param>
        /// <returns>Data Template</returns>
        public static DataTemplate GetCustomEditableTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(CustomEditableTemplateProperty);
        }

        /// <summary>
        /// Sets the value of the CustomEditableTemplate dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        public static void SetCustomEditableTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(CustomEditableTemplateProperty, value);
        }

        /// <summary>
        /// Gets the value of the IsEditing dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject.</param>
        /// <returns>true, if IsEditing is true.</returns>
        public static bool GetIsEditing(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEditingProperty);
        }

        /// <summary>
        /// Sets the value of the IsEditing dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        protected internal static void SetIsEditing(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEditingPropertyKey, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether label can be edited or not.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True, if label can be edited, false otherwise.
        /// </value>
        public bool EnableLabelEdit
        {
            get { return (bool)GetValue(EnableLabelEditProperty); }
            set { SetValue(EnableLabelEditProperty, value); }
        }

        public bool EnableLabelDrag
        {
            get { return (bool)GetValue(EnableLabelDragProperty); }
            set { SetValue(EnableLabelDragProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the label.
        /// </summary>
        /// <value>The width of the label.</value>
        public double LabelWidth
        {
            get
            {
                return (double)GetValue(LabelWidthProperty);
            }

            set
            {
                SetValue(LabelWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the label.
        /// </summary>
        /// <value>The height of the label.</value>
        public double LabelHeight
        {
            get
            {
                return (double)GetValue(LabelHeightProperty);
            }

            set
            {
                SetValue(LabelHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label text wrapping.
        /// </summary>
        /// <value>The label text wrapping.</value>
        public TextWrapping LabelTextWrapping
        {
            get
            {
                return (TextWrapping)GetValue(LabelTextWrappingProperty);
            }

            set
            {
                SetValue(LabelTextWrappingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label foreground.
        /// </summary>
        /// <value>The label foreground.</value>
        public Brush LabelForeground
        {
            get
            {
                return (Brush)GetValue(LabelForegroundProperty);
            }

            set
            {
                SetValue(LabelForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label background.
        /// </summary>
        /// <value>The label background.</value>
        public Brush LabelBackground
        {
            get
            {
                return (Brush)GetValue(LabelBackgroundProperty);
            }

            set
            {
                SetValue(LabelBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size of the label font.
        /// </summary>
        /// <value>The size of the label font.</value>
        public double LabelFontSize
        {
            get
            {
                return (double)GetValue(LabelFontSizeProperty);
            }

            set
            {
                SetValue(LabelFontSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label font family.
        /// </summary>
        /// <value>The label font family.</value>
        public FontFamily LabelFontFamily
        {
            get
            {
                return (FontFamily)GetValue(LabelFontFamilyProperty);
            }

            set
            {
                SetValue(LabelFontFamilyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label font weight.
        /// </summary>
        /// <value>The label font weight.</value>
        public FontWeight LabelFontWeight
        {
            get
            {
                return (FontWeight)GetValue(LabelFontWeightProperty);
            }

            set
            {
                SetValue(LabelFontWeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label font style.
        /// </summary>
        /// <value>The label font style.</value>
        public FontStyle LabelFontStyle
        {
            get
            {
                return (FontStyle)GetValue(LabelFontStyleProperty);
            }

            set
            {
                SetValue(LabelFontStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label text trimming.
        /// </summary>
        /// <value>The label text trimming.</value>
        public TextTrimming LabelTextTrimming
        {
            get
            {
                return (TextTrimming)GetValue(LabelTextTrimmingProperty);
            }

            set
            {
                SetValue(LabelTextTrimmingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label text alignment.
        /// </summary>
        /// <value>The label text alignment.</value>
        public TextAlignment LabelTextAlignment
        {
            get
            {
                return (TextAlignment)GetValue(LabelTextAlignmentProperty);
            }

            set
            {
                SetValue(LabelTextAlignmentProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable multiline label].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable multiline label]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableMultilineLabel
        {
            get
            {
                return (bool)GetValue(EnableMultilineLabelProperty);
            }
            set
            {
                SetValue(EnableMultilineLabelProperty, value);
            }
        }

        public Point LabelPosition
        {
            get
            {
                return (Point)GetValue(LabelPositionProperty);
            }
            set
            {
                SetValue(LabelPositionProperty, value);
            }
        }        
        #endregion

        #region DPs

        /// <summary>
        /// Defines the LabelWidth property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelWidthProperty = DependencyProperty.Register("LabelWidth", typeof(double), typeof(LabelEditor));

        /// <summary>
        /// Defines the LabelHeight property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelHeightProperty = DependencyProperty.Register("LabelHeight", typeof(double), typeof(LabelEditor));

        /// <summary>
        /// Defines the LabelTextWrapping property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTextWrappingProperty = DependencyProperty.Register("LabelTextWrapping", typeof(TextWrapping), typeof(LabelEditor), new FrameworkPropertyMetadata(TextWrapping.NoWrap));

        /// <summary>
        /// Defines the LabelForeground property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelForegroundProperty = DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(LabelEditor), new FrameworkPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Defines the LabelBackground property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelBackgroundProperty = DependencyProperty.Register("LabelBackground", typeof(Brush), typeof(LabelEditor), new FrameworkPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Defines the LabelFontSize property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontSizeProperty = DependencyProperty.Register("LabelFontSize", typeof(double), typeof(LabelEditor), new FrameworkPropertyMetadata(11d));

        /// <summary>
        /// Defines the LabelFontFamily property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontFamilyProperty = DependencyProperty.Register("LabelFontFamily", typeof(FontFamily), typeof(LabelEditor), new FrameworkPropertyMetadata(new FontFamily("Arial")));

        /// <summary>
        /// Defines the LabelFontWeight property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontWeightProperty = DependencyProperty.Register("LabelFontWeight", typeof(FontWeight), typeof(LabelEditor), new FrameworkPropertyMetadata(FontWeights.SemiBold));

        /// <summary>
        /// Defines the LabelFontStyle property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontStyleProperty = DependencyProperty.Register("LabelFontStyle", typeof(FontStyle), typeof(LabelEditor), new FrameworkPropertyMetadata(FontStyles.Normal));

        /// <summary>
        /// Defines the LabelTextTrimming property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTextTrimmingProperty = DependencyProperty.Register("LabelTextTrimming", typeof(TextTrimming), typeof(LabelEditor), new FrameworkPropertyMetadata(TextTrimming.CharacterEllipsis));

        /// <summary>
        /// Defines the LabelTextAlignment property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTextAlignmentProperty = DependencyProperty.Register("LabelTextAlignment", typeof(TextAlignment), typeof(LabelEditor), new FrameworkPropertyMetadata(TextAlignment.Center));

        /// <summary>
        /// Defines the SelectedItem property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
         DependencyProperty.Register("SelectedItem", typeof(object), typeof(LabelEditor), new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// Defines the Label.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
         DependencyProperty.Register("Label", typeof(string), typeof(LabelEditor), new UIPropertyMetadata("Label"));

        /// <summary>
        /// Defines the TextWidth property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty TextWidthProperty =
         DependencyProperty.Register("TextWidth", typeof(double), typeof(LabelEditor), new FrameworkPropertyMetadata(0d));

        /// <summary>
        /// Defines the CustomEditableTemplate property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomEditableTemplateProperty =
            DependencyProperty.RegisterAttached("CustomEditableTemplate", typeof(DataTemplate), typeof(LabelEditor), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Defines the UseCustomEditableTemplate property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty UseCustomEditableTemplateProperty =
            DependencyProperty.RegisterAttached("UseCustomEditableTemplate", typeof(bool), typeof(LabelEditor), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Key through which  IsEditing property can be changed.  This is a dependency property key.
        /// </summary>
        protected static readonly DependencyPropertyKey IsEditingPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("IsEditing", typeof(bool), typeof(LabelEditor), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Defines the IsEditing property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEditingProperty = IsEditingPropertyKey.DependencyProperty;

        /// <summary>
        /// Defines the EnableLabelEdit property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableLabelEditProperty =
            DependencyProperty.Register("EnableLabelEdit", typeof(bool), typeof(LabelEditor), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnEnableLabelEditChanged)));

        public static readonly DependencyProperty EnableLabelDragProperty =
            DependencyProperty.Register("EnableLabelDrag", typeof(bool), typeof(LabelEditor), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnEnableLabelDragChanged)));
        
        public static readonly DependencyProperty EnableMultilineLabelProperty = DependencyProperty.Register("EnableMultilineLabel", typeof(bool), typeof(LabelEditor), new PropertyMetadata(false));

        public static readonly DependencyProperty LabelPositionProperty = DependencyProperty.Register("LabelPosition", typeof(Point), typeof(LabelEditor), new FrameworkPropertyMetadata(new Point(0, 0), new PropertyChangedCallback(OnLabelPositionChanged)));
  
        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when EnableLabelEdit property is changed.
        /// </summary>
        public event PropertyChangedCallback EnableLabelEditChanged;

        public event PropertyChangedCallback EnableLabelDragChanged;

        public event PropertyChangedCallback LabelPositionChanged;

        /// <summary>
        /// Event that is raised when UseCustomEditableTemplate property is changed.
        /// </summary>
        public event PropertyChangedCallback UseCustomEditableTemplateChanged;

        /// <summary>
        /// Event that is raised when CustomEditableTemplat property is changed.
        /// </summary>
        public event PropertyChangedCallback CustomEditableTemplateChanged;

        /// <summary>
        /// Event handler for the dependency property value changes .
        /// </summary>
        /// <param name="sender">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        internal delegate void BeforeLabelEditHandler(object sender, BeforeLabelEditEventArgs e);

        /// <summary>
        /// Event handler for the dependency property value changes .
        /// </summary>
        /// <param name="sender">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        internal delegate void AfterLabelEditHandler(object sender, AfterLabelEditEventArgs e);

        /// <summary>
        /// Event handler for the dependency property value changes .
        /// </summary>
        internal event BeforeLabelEditHandler BeforeLabelEdit;

        /// <summary>
        /// Event handler for the dependency property value changes .
        /// </summary>
        internal event AfterLabelEditHandler AfterLabelEdit;

        /// <summary>
        /// Complete editing process on the specifies TabItem.
        /// </summary>
        /// <param name="editableItem">TabItem which is editing in the current moment.</param>
        /// <param name="applyChanges">Specifies whether editing changes should be applied or no.</param>
        public void CompleteHeaderEdit(LabelEditor editableItem, bool applyChanges)
        {
            if (editableItem != null && EnableLabelEdit && LabelEditor.GetIsEditing(editableItem))
            {
                editableItem.CompleteHeaderEditInternal(editableItem, applyChanges);
            }
        }

        /// <summary>
        /// Calls OnEnableLabelEditChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnEnableLabelEditChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LabelEditor instance = (LabelEditor)d;
            instance.OnEnableLabelEditChanged(e);
        }

        private static void OnLabelPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {            
            LabelEditor instance = (LabelEditor)d;
            if (!instance.dragging)
            {
                var move = new TranslateTransform(instance.LabelPosition.X, instance.LabelPosition.Y);
                var group = new TransformGroup();
                group.Children.Add(move);
                instance.RenderTransform = group;
                instance.OnLabelPositionChanged(e);
            }
        }
        
        protected virtual void OnLabelPositionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LabelPositionChanged != null)
            {            
                LabelPositionChanged(this, e);
            }
        }

        private static void OnEnableLabelDragChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LabelEditor instance = (LabelEditor)d;            
            instance.OnEnableLabelDragChanged(e);
        }

        protected virtual void OnEnableLabelDragChanged(DependencyPropertyChangedEventArgs e)
        {
            if (EnableLabelDragChanged != null)
            {
                EnableLabelDragChanged(this, e);
            }
        }
        /// <summary>
        /// Calls OnEnableLabelEditChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnEnableLabelEditChanged(DependencyPropertyChangedEventArgs e)
        {
            if (EnableLabelEditChanged != null)
            {
                EnableLabelEditChanged(this, e);
            }
        }

        /// <summary>
        /// Calls FireBeforeLabelEdit method of the instance, notifies of the  property value changes.
        /// </summary>
        /// <param name="headerBeforeLabelEdit">The headerBeforeLabelEdit Object.</param>
        protected internal virtual void FireBeforeLabelEdit(object headerBeforeLabelEdit)
        {
            if (BeforeLabelEdit != null)
            {
                BeforeLabelEdit(this, new BeforeLabelEditEventArgs(headerBeforeLabelEdit));
            }
        }

        /// <summary>
        /// Calls FireAfterLabelEdit method of the instance, notifies of the  property value changes.
        /// </summary>
        /// <param name="headerAfterLabelEdit">The object.</param>
        protected internal virtual void FireAfterLabelEdit(object headerAfterLabelEdit)
        {
            if (AfterLabelEdit != null)
            {
                AfterLabelEdit(this, new AfterLabelEditEventArgs(headerAfterLabelEdit));
            }
        }

        /// <summary>
        /// Calls OnCustomEditableTemplateChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnUseCustomEditableTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LabelEditor instance = (LabelEditor)d;
            instance.OnUseCustomEditableTemplateChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises UseCustomEditableTemplateChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnUseCustomEditableTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (UseCustomEditableTemplateChanged != null)
            {
                UseCustomEditableTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnCustomEditableTemplateChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCustomEditableTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LabelEditor instance = (LabelEditor)d;
            instance.OnCustomEditableTemplateChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises CustomEditableTemplateChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCustomEditableTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CustomEditableTemplateChanged != null)
            {
                CustomEditableTemplateChanged(this, e);
            }
        }

        #endregion

        #region Implementation
        private bool isDrag = true;
        /// <summary>
        /// Complete editing process on the specified item.
        /// </summary>
        /// <param name="editableItem">Item which is editing in the current moment.</param>
        /// <param name="applyChanges">Specifies whether editing changes should be applied or no.</param>
        internal void CompleteHeaderEditInternal(LabelEditor editableItem, bool applyChanges)
        {
            if(!this.isDrag)
            {
                this.EnableLabelDrag=true;
                this.isDrag=true;
            }
            if (!LabelEditor.GetUseCustomEditableTemplate(editableItem) && m_editableHeader != null)
            {
                if (applyChanges)
                {
                    UpdateBinding(m_editableHeader);
                }

                RemoveDelegates(m_editableHeader);
                m_editableHeader = null;
            }
            else
            {
                if (!applyChanges)
                {
                    editableItem.Header = editableItem.Tag;
                    editableItem.Tag = null;
                }
            }

            RemoveDelegates(editableItem);
            if (m_editingItem != null)
            {
                FocusManager.SetIsFocusScope(editableItem, false);
                LabelEditor.SetIsEditing(editableItem, false);
                m_editingItem = null;
                this.FireAfterLabelEdit(editableItem.Label);
            }
        }

        /// <summary>
        /// Gets the  node.
        /// </summary>
        /// <param name="element">Framework element.</param>
        /// <returns>The parent object.</returns>
        public object GetNode(FrameworkElement element)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            while (parent != null)
            {
                if (parent is LineConnector || parent is Node)
                {
                    return parent as object;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        /// <summary>
        /// Launch editing process on the specified Node.
        /// </summary>
        /// <param name="item">Item which should be edited.</param>
        internal void LabelEditStartInternal(LabelEditor item)
        {
            if(this.EnableLabelDrag)
            {
                this.isDrag = false;
                this.EnableLabelDrag = false;
            }
            if (item != null)
            {
                this.FireBeforeLabelEdit(item.Label);
                ContentPresenter presenter = null;
                LabelEditor.SetIsEditing(item, true);
                item.UpdateLayout();

                object n = GetNode(item);
                if (n is Node)
                {
                    LabelRoutedEventArgs newEventArgs = new LabelRoutedEventArgs((string)(n as Node).Label, null, (n as Node));
                    newEventArgs.RoutedEvent = DiagramView.NodeStartLabelEditEvent;
                    RaiseEvent(newEventArgs);
                }

                if (n is LineConnector)
                {
                    LabelEditConnRoutedEventArgs newEventArgs;
                    if ((n as LineConnector).HeadNode as Node != null && (n as LineConnector).TailNode as Node != null)
                    {
                        newEventArgs = new LabelEditConnRoutedEventArgs((string)(n as LineConnector).Label, (n as LineConnector).HeadNode as Node, (n as LineConnector).TailNode as Node, n as LineConnector);
                    }
                    else
                        if ((n as LineConnector).HeadNode as Node == null && (n as LineConnector).TailNode as Node != null)
                        {
                            newEventArgs = new LabelEditConnRoutedEventArgs((string)(n as LineConnector).Label, (n as LineConnector).TailNode as Node, n as LineConnector);
                        }
                        else if ((n as LineConnector).HeadNode as Node != null && (n as LineConnector).TailNode as Node == null)
                        {
                            newEventArgs = new LabelEditConnRoutedEventArgs((string)(n as LineConnector).Label, n as LineConnector, (n as LineConnector).HeadNode as Node);
                        }
                        else
                        {
                            newEventArgs = new LabelEditConnRoutedEventArgs((string)(n as LineConnector).Label, n as LineConnector);
                        }

                    newEventArgs.RoutedEvent = DiagramView.ConnectorStartLabelEditEvent;
                    RaiseEvent(newEventArgs);
                }

                m_editingItem = item;
                item.Focus();
                FocusManager.SetIsFocusScope(item, true);
                foreach (Visual visual in VisualUtils.EnumChildrenOfType(item, typeof(ContentPresenter)))
                {
                    if ((visual as ContentPresenter).Name == "Content")
                    {
                        presenter = visual as ContentPresenter;
                    }
                }

                if (presenter != null)
                {
                    if (!LabelEditor.GetUseCustomEditableTemplate(item))
                    {
                        TextBox editableHeader = null;
                        foreach (Visual visual in VisualUtils.EnumChildrenOfType(presenter, typeof(TextBox)))
                        {
                            if ((visual as TextBox).Name == "EditableHeader")
                            {
                                editableHeader = visual as TextBox;
                            }
                        }

                        if (editableHeader != null)
                        {
                            m_editableHeader = editableHeader;
                            editableHeader.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnEditableTexBox_LostKeyboardFocus);
                            editableHeader.KeyDown += new KeyEventHandler(OnEditableTextBox_KeyDown);
                            FocusManager.SetFocusedElement(item, editableHeader);
                            editableHeader.SelectAll();
                        }
                    }
                    else
                    {
                        item.Tag = item.Header;
                    }
                }

                if (this.SelectedItem != item)
                {
                    this.SelectedItem = item;
                }
            }
        }

        /// <summary>
        /// Updates  binding explicit.
        /// </summary>
        /// <param name="target">Binding target.</param>
        private void UpdateBinding(TextBox target)
        {
            BindingExpression expression = target.GetBindingExpression(TextBox.TextProperty);
            expression.UpdateSource();
            double a = target.ActualWidth;
        }

        /// <summary>
        /// Removes delegates on the specified target.
        /// </summary>
        /// <param name="target">Delegates owner.</param>
        private void RemoveDelegates(Control target)
        {
            if (target is TextBox)
            {
                TextBox textBox = (TextBox)target;
                textBox.LostKeyboardFocus -= new KeyboardFocusChangedEventHandler(OnEditableTexBox_LostKeyboardFocus);
                textBox.KeyDown -= new KeyEventHandler(OnEditableTextBox_KeyDown);
            }
        }

        private bool dragging = false;
        private Point startPosition;
        private Point currentPosition;
        private Point previousPosition;
        private Transform previousTransform;

        private bool pressed = false;
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            object n = GetNode(this);
            if (e.LeftButton == MouseButtonState.Pressed)// && !(n as Node).dc.View.EnableDrawingTools)
            {
                if (n is Node &&  !(n as Node).dc.View.EnableDrawingTools)
                {
                    this.CaptureMouse();
                }
                else if (n is LineConnector && !(n as LineConnector).dc.View.EnableDrawingTools)
                {
                    this.CaptureMouse();
                }
                pressed = false;
            }
            else
            {
                pressed = true;
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            this.ReleaseMouseCapture();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            this.ReleaseMouseCapture();
            if (this.EnableLabelDrag)
            {
                object n = GetNode(this);
                if (n is Node)
                {
                    if ((n as Node).dc.View != null && !((n as Node).dc.View as DiagramView).EnableDrawingTools )
                    {
                        if (!(n as Node).mousedown)
                        {
                            if (pressed)
                            {
                                if (po != e.GetPosition(n as Node))
                                {
                                    if (this.LabelPosition == new Point(0, 0))
                                    {
                                        this.LabelPosition = new Point(this.currentPosition.X - this.startPosition.X, this.currentPosition.Y - this.startPosition.Y);
                                    }
                                    else
                                    {
                                        var mo = e.GetPosition(n as Node);
                                        if (this.LabelPosition.X < mo.X)
                                        {
                                            this.LabelPosition = new Point(this.LabelPosition.X + (mo.X - po.X), this.LabelPosition.Y);
                                        }
                                        else
                                        {
                                            this.LabelPosition = new Point(this.LabelPosition.X - (po.X - mo.X), this.LabelPosition.Y);
                                        }

                                        if (this.LabelPosition.Y < mo.Y)
                                        {
                                            this.LabelPosition = new Point(this.LabelPosition.X, this.LabelPosition.Y + (mo.Y - po.Y));
                                        }
                                        else
                                        {
                                            this.LabelPosition = new Point(this.LabelPosition.X, this.LabelPosition.Y - (po.Y - mo.Y));
                                        }
                                    }
                                }
                                pressed = false;
                            }
                        }
                    }
                }
            }
            base.OnMouseLeftButtonUp(e);
            this.dragging = false;
        }


        //private double RemoveDecimal(string str)
        //{
        //    string s1 = str;
        //    if (str.Contains('.'))
        //    {
        //        s1 = s1.Substring(0, s1.IndexOf('.', 0));
        //    }
        //    return Double.Parse(s1);
        //}
  
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.CaptureMouse();
            base.OnMouseLeftButtonDown(e);
            if (this.EnableLabelDrag && startPosition == new Point(0, 0))
            {                
                object n = GetNode(this);
                if (n is Node)
                {
                    if ((n as Node).mousedown == true)
                    {
                        this.startPosition = e.GetPosition(n as Node);
                    }

                }                
            }            
        }
       

           
        protected override void OnMouseMove(MouseEventArgs e)
        {
            object n = GetNode(this);
            if (n is Node && !(n as Node).dc.View.EnableDrawingTools)            
            {
                if (this.EnableLabelDrag)
                {                   
                    Node node = n as Node;
                    currentPosition = e.GetPosition(node);  
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        if ((n as Node).dc.View != null && !((n as Node).dc.View as DiagramView).EnableDrawingTools && (n as Node).mousedown)
                        {
                            this.dragging = true;
                            var move = new TranslateTransform(currentPosition.X - previousPosition.X, currentPosition.Y - previousPosition.Y);
                            if (this.Cursor == Cursors.Hand)
                            {
                                var group = new TransformGroup();
                                if (previousTransform != null)
                                {
                                    group.Children.Add(previousTransform);
                                }
                                group.Children.Add(move);
                                this.RenderTransform = group;
                            }

                            (n as Node).RectBounds = (n as Node).TransformRectBounds();
                            (n as Node).dc.View.Page.InvalidateMeasure();
                        }
                    }
                    else
                    {
                        this.Cursor = Cursors.Hand;
                    }
                    previousPosition = currentPosition;                    
                    previousTransform = this.RenderTransform;                    
                }
            }
            else if (n is LineConnector && !(n as LineConnector).dc.View.EnableDrawingTools)
            {
                LineConnector line = n as LineConnector;
                if ((line as LineConnector).IsLabelDragable)
                {
                    currentPosition = e.GetPosition((line as LineConnector).view.Page as DiagramPage);
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        if ((line as LineConnector).mousedown)
                        {
                            this.dragging = true;
                            line.LabelPosition = new Point(line.LabelPosition.X + (currentPosition.X - previousPosition.X), line.LabelPosition.Y + (currentPosition.Y - previousPosition.Y));
                            (n as LineConnector).dc.View.Page.InvalidateMeasure();
                        }
                    }
                    else
                    {
                        this.Cursor = Cursors.Hand;
                    }
                    previousPosition = currentPosition;
                }
            }
            base.OnMouseMove(e);            
        }

        Point po = new Point();
        /// <summary>
        /// Provides class handling for the PreviewMouseLeftButtonDown routed event that occurs when the left mouse 
        /// button is pressed while the mouse pointer is over this control.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs</param>
       
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            object n = GetNode(this);
            po = e.GetPosition(n as Node);
            base.OnPreviewMouseLeftButtonDown(e);

            LabelEditor item = (LabelEditor)VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(LabelEditor));
            if (e.Source == this)
            {
                if (this.EnableLabelEdit)
                {
                    Point position = e.GetPosition(this);

                    if (m_clickInfo.IsDoubleClick(position) && DiagramView.PageEdit)
                    {
                        LabelEditStartInternal(item);
                    }
                    else
                    {
                        m_clickInfo.LastNodePoint = e.GetPosition(this);
                    }

                    m_clickInfo.LastNodeClick = DateTime.Now;
                }
            }
            else if (e.Source != this)
            {
                bool applyChanges = true;
                CompleteHeaderEditInternal(m_editingItem, applyChanges);
            }

            //e.Handled = true;
        }
        #endregion

        #region Event handlers

        /// <summary>
        /// Calls OnEditableTextBox_KeyDown method of the instance, notifies of the sender value changes.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void OnEditableTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (m_editingItem != null)
            {
                TextBox t = sender as TextBox;
                TextWidth = t.ActualWidth;
                if (e.Key == Key.Enter || e.Key == Key.Escape)
                {
                    bool applyChanges = false;

                    if (e.Key == Key.Enter)
                    {
                        applyChanges = true;
                    }

                    CompleteHeaderEditInternal(m_editingItem, applyChanges);
                }
            }
        }

        /// <summary>
        /// Calls OnEditableTexBox_LostKeyboardFocus method of the instance, notifies of the sender value changes.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void OnEditableTexBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (m_editingItem != null && !(e.NewFocus is ContextMenu))
            {
                CompleteHeaderEditInternal(m_editingItem, true);
            }
        }
        #endregion
    }
}

