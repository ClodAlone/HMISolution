#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Tools.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Xml;
    using Syncfusion.Windows.Controls;
    using System.ComponentModel;

    /// <summary>
    /// Represents a TaskBar Control, used for grouping of items
    /// </summary>
    /// <remarks>
    /// The Task Bar control allows creating collapsible panels that can be used to host
    /// a collection of command items or snippets of information pertaining to the
    /// current context.
    /// </remarks>
    /// <example>
    /// <para><b>Creating TaskBar Control in XAML</b></para>
    /// <para></para>
    /// <code>&lt;UserControl x:Class=&quot;SilverlightSampleBrowser.CurrencyTextBoxDemo&quot;
    ///    xmlns=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot;
    ///     xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot;
    ///     xmlns:syncfusion=&quot;clr-     namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.Silverlight&quot;     Height=&quot;600&quot; Width=&quot;850&quot;&gt;</code>
    /// <para></para>
    /// <code>&lt;StackPanel x:Name=&quot;StackPanel1&quot;&gt;
    /// &lt;syncfusion:TaskBar     x:Name=&quot;taskbar1&quot;
    ///                                          BorderThickness=&quot;5&quot;
    ///                                          
    ///                                          GroupOrientation=&quot;Horizontal&quot;
    ///                                          Margin=&quot;2&quot;
    ///                                          HorizontalAlignment=&quot;Center&quot;
    ///                                          VerticalAlignment=&quot;Center&quot;/&gt;&lt;/StackPanel&gt;
    /// &lt;/UserControl&gt;</code>
    /// <para></para>
    /// <para></para>
    /// <para><b>Creating TaskBar Control using C#</b></para>
    /// <para></para>
    /// <code lang="C#">namespace Sample1
    /// {
    ///     public partial class TaskBarDemo : UserControl
    ///     {
    ///         public TaskBarDemo()
    ///         {
    ///             InitializeComponent();
    ///              TaskBar taskBar1 = new TaskBar();</code>
    /// <para></para>
    /// <para>          <code lang="C#">
    ///  </code></para>
    /// <para></para>
    /// <para></para>
    /// <para></para>taskBar1.GroupOrientation=Orientation.Horizontal; 
    /// <para></para>
    /// <para></para>
    /// <para></para> StackPanel1.Children.Add(currencyTextBox); 
    /// <para></para>
    /// <para></para>
    /// <para></para>       } 
    /// <para></para>} 
    /// <para></para>}
    /// </example>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
        Type = typeof(TaskBar), XamlResource = "/Syncfusion.Theming.Blend;component/TaskBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(TaskBar), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/TaskBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(TaskBar), XamlResource = "/Syncfusion.Theming.Office2007Black;component/TaskBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(TaskBar), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/TaskBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(TaskBar), XamlResource = "/Syncfusion.Theming.Default;component/TaskBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(TaskBar), XamlResource = "/Syncfusion.Theming.Office2003;component/TaskBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
       Type = typeof(TaskBar), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/TaskBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(TaskBar), XamlResource = "/Syncfusion.Theming.Office2010Black;component/TaskBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(TaskBar), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/TaskBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
       Type = typeof(TaskBar), XamlResource = "/Syncfusion.Theming.Windows7;component/TaskBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
       Type = typeof(TaskBar), XamlResource = "/Syncfusion.Theming.VS2010;component/TaskBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
      Type = typeof(TaskBar), XamlResource = "/Syncfusion.Theming.Metro;component/TaskBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent ,
  Type = typeof(TaskBar), XamlResource = "/Syncfusion.Theming.Transparent;component/TaskBar.xaml")]
    public partial class TaskBar : ItemsControl
    {
        #region Public Dependency Properties


        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.TaskBar.GroupOrientationProperty">GroupOrientation</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty GroupOrientationProperty =
             DependencyProperty.Register("GroupOrientation", typeof(Orientation), typeof(TaskBar), new PropertyMetadata(Orientation.Horizontal, OnGroupOrientationChanged));


        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.TaskBar.ItemContainerStyleProperty">ItemContainerStyle</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(TaskBar), new PropertyMetadata(null));



        

        #endregion Public Dependency Properties.

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.TaskBar">TaskBar</see> class
        /// </summary>
        public TaskBar()
        {
            this.DefaultStyleKey = typeof(TaskBar);
        }

        /// <summary>
        /// Initializes the <see cref="TaskBar"/> class.
        /// </summary>
        static TaskBar()
        {
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                Syncfusion.Windows.Shared.LoadDependentAssemblies load = new Syncfusion.Windows.Shared.LoadDependentAssemblies();
                load = null;
            }
        }
        #endregion Constructor

        #region Public Events



        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.TaskBar.GroupOrientation">CurrencyDecimalDigits</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback GroupOrientationChanged;

        #endregion Public Events

        #region Public Properties


        /// <summary>
        /// Gets or sets a value indicates the Group Orientation for the  TaskBar Control
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para><c>&lt;Syncfusion:TaskBar Name=&quot;taskBar1&quot; GroupOrientation=&quot;Horizontal&quot;/&gt;</c></para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>TaskBar taskBar1=new Taskbar(); </para>
        /// <para>taskBar1.GroupOrientat=Orientation.Horizontal;</para>
        /// </remarks>
        public Orientation GroupOrientation
        {
            get
            {
                return (Orientation)GetValue(GroupOrientationProperty);
            }

            set
            {
                SetValue(GroupOrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the style of the ItemContainer of target type TaskBarItem
        /// </summary>
        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        #endregion Public Properties


        //StackPanel stk;
        #region Public Methods

        /// <summary>
        /// Applies the Template for the File Upload control
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ItemsPanelTemplate ipt = null;
            if (this.GroupOrientation == Orientation.Horizontal)
            {
                ipt = (ItemsPanelTemplate)XamlReader.Load(@"<ItemsPanelTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""    xmlns:local=""clr-namespace:Syncfusion.Tools"" >"
                                                                  + @"<StackPanel Name=""stpanel"" Orientation=""Horizontal""/> </ItemsPanelTemplate>");
            }
            else if (this.GroupOrientation == Orientation.Vertical)
            {
                ipt = (ItemsPanelTemplate)XamlReader.Load(@"<ItemsPanelTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""    xmlns:local=""clr-namespace:Syncfusion.Tools"" >"
                                                               + @"<StackPanel Name=""stpanel"" Orientation=""Vertical""/> </ItemsPanelTemplate>");
            }

            this.ItemsPanel = ipt;
            TaskBarItem[] items = new TaskBarItem[this.Items.Count];
            if (this.GroupOrientation == Orientation.Horizontal)
            {
                int i, it = 0;
                for (i = 0; i <this.Items.Count; i++)
                {
                    items[it++] = (TaskBarItem)this.Items[i];
                }

                this.Items.Clear();
                for (i = 0; i < it; i++)
                {
                    this.Items.Add(items[i]);
                }
            }
        }
        #endregion Public Methods

        #region Protected Override Methods
        /// <summary>
        /// Creates a TaskBarItem to use to display content.
        /// </summary>
        /// <returns>
        /// A new TaskBarItem to use as a container for content.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            TaskBarItem tbi = new TaskBarItem();
            this.Visibility = Visibility.Visible;
            return tbi;
        }

        /// <summary>
        /// Determines whether the specified item is its own container or can be
        /// its own container.
        /// </summary>
        /// <param name="item">The object to evaluate.</param>
        /// <returns>
        /// A value indicating whether the item is a TaskBarItem or not.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TaskBarItem;
        }

        /// <summary>
        /// Measures the Sizes Needed for the Control
        /// </summary>
        /// <param name="availableSize">Indicates the AvailableSize</param>
        /// <returns>Returns the Size available</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Prepares the specified container to display the specified item.
        /// </summary>
        /// <param name="element">
        /// Container element used to display the specified item.
        /// </param>
        /// <param name="item">Specified item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            TaskBarItem taskbarItem = (TaskBarItem)element;
            if (taskbarItem != null)
            {
                if (!(item is TaskBarItem))
                {
                    taskbarItem.Header = item;
                }

                if (taskbarItem.Style == null && this.ItemContainerStyle != null)
                {
                    Binding binding = new Binding("ItemContainerStyle");
                    binding.Source = this;
                    BindingOperations.SetBinding(taskbarItem, TaskBarItem.StyleProperty, binding);
                }
            }
        }
        #endregion Protected Override Methods

        #region Protected virtual Methods



        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.TaskBar.GroupOrientationChanged">GroupOrientationChanged</see> event.
        /// </summary>
        /// <param name="e" >Property change details, such as old value and new value</param>
        protected virtual void OnGroupOrientationChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.GroupOrientationChanged != null)
            {
                this.GroupOrientationChanged(this, e);
            }
        }
        #endregion Protected virtual Methods

        #region Private Static Methods


        /// <summary>
        /// Calls OnGroupOrientationChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current TaskBar Instance</param>
        /// <param name="e" >Property change details, such as old value and new value</param>
        private static void OnGroupOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskBar instance = (TaskBar)d;
            instance.OnGroupOrientationChanged(e);
        }
        #endregion Private Static Methods

        #region Private Methods


        #endregion Private Methods


        /// <summary>
        /// When implemented in a derived class, returns class-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> implementations for the Silverlight automation infrastructure.
        /// </summary>
        /// <returns>
        /// The class-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> subclass to return.
        /// </returns>
        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            return new TaskBarAutomationPeer(this);
        }
    }
}
