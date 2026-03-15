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
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Automation.Peers;
    using Syncfusion.Silverlight.Shared;
    using Syncfusion.Windows.Controls;
    using Syncfusion.Windows.Shared;

    /// <summary>
    /// Defines the Arrow Styles
    /// </summary>
    public enum ArrowStyles
    {
        /// <summary>
        /// Default Arrow
        /// </summary>
        Default,

        /// <summary>
        /// Double Arrow
        /// </summary>
        Arrow,

        /// <summary>
        /// Solid Arrow
        /// </summary>
        SolidArrow,

        /// <summary>
        /// Triangle Arrow
        /// </summary>
        Triangle,

        /// <summary>
        /// Solid Arrow
        /// </summary>
        SingleArrow,

        /// <summary>
        /// Arrow Head
        /// </summary>
        ArrowHead
    }

    /// <summary>
    /// Represents a TaskBarItem. Used as a  container for displaying items in the
    /// TaskBar Control.
    /// </summary>
    /// <example>
    /// 	<para><b>Creating TaskBar Control in XAML</b></para>
    /// 	<para></para>
    /// 	<code>&lt;UserControl x:Class="SilverlightSampleBrowser.CurrencyTextBoxDemo"
    /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="clr-     namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.Silverlight"     Height="600" Width="850"&gt;</code>
    /// 	<para></para>
    /// 	<para><c>&lt;StackPanel x:Name="StackPanel1"&gt;</c></para>
    /// 	<para></para>
    /// 	<code>&lt;syncfusion:TaskBar
    /// x:Name="taskbar1"
    /// 
    /// GroupOrientation="Horizontal"&gt;</code>
    /// 	<para></para>
    /// 	<code>&lt;taskbar:TaskBarItem
    /// HeaderHeight="30"
    /// ArrowStyle="Default"
    /// Name="item1"
    /// IsHeaderShown="True"
    /// IsExpanded="True" &gt;
    /// &lt;taskbar:TaskBarItem.Header &gt;</code>
    /// 	<para></para>
    /// 	<code>                                &lt;TextBlock Text="Personal Info" /&gt;
    /// &lt;/taskbar:TaskBarItem.Header&gt;
    /// &lt;taskbar:TaskBarItem.Content&gt;
    /// &lt;StackPanel Orientation="Horizontal" &gt;
    /// &lt;TextBlock  Text="Name     : " /&gt;
    /// &lt;TextBox Width="100" Height="25" Text="Sync"/&gt;</code>
    /// 	<para></para>
    /// 	<code>&lt;/StackPanel&gt;
    /// &lt;/taskbar:TaskBarItem.Content&gt;
    /// &lt;/taskbar:TaskBarItem&gt;
    /// &lt;/StackPanel&gt;
    /// &lt;/UserControl&gt;</code>
    /// 	<para></para>
    /// 	<para></para>
    /// 	<para><b>Creating TaskBar Control using C#</b></para>
    /// 	<para></para>
    /// 	<para>namespace Sample1</para>
    /// 	<para>{</para>
    /// 	<para>    public partial class TaskBarDemo : UserControl</para>
    /// 	<para>    {</para>
    /// 	<para>        public TaskBarDemo()</para>
    /// 	<para>        {</para>
    /// 	<para>            InitializeComponent();</para>
    /// 	<para>             TaskBar taskBar1 = new TaskBar();</para>
    /// 	
    /// 	<para>taskBar1.GroupOrientation=Orientation.Horizontal;</para>
    /// 	<para>StackPanel1.Children.Add(currencyTextBox); </para>
    /// 	<para></para>
    /// 	<para>TaskBarItem itm1 = new TaskBarItem(); </para>
    /// 	<para>TextBlock HeaderText = new TextBlock(); </para>
    /// 	<para>HeaderText.Text = "Personal Info"; </para>
    /// 	<para>itm1.Header = HeaderText; </para>
    /// 	<para>StackPanel contentPanel = new StackPanel(); </para>
    /// 	<para>TextBlock contentName = new TextBlock(); </para>
    /// 	<para>contentName.Text = "Name"; </para>
    /// 	<para>TextBox ContentValue = new TextBox(); </para>
    /// 	<para>ContentValue.Text = "Sync"; </para>
    /// 	<para>contentPanel.Children.Add(contentName); </para>
    /// 	<para>contentPanel.Children.Add(ContentValue); </para>
    /// 	<para>itm1.Content = contentPanel; </para>
    /// 	<para>taskbar1.Items.Add(itm1); </para>
    /// 	<para> } </para>
    /// 	<para>} </para>
    /// 	<para>}</para>
    /// </example>
    [CLSCompliant(false)]
    public class TaskBarItem : HeaderedContentControl
    {
        #region Public Dependency Properties
        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.TaskBarItem.ArrowStyleProperty">ArrowStyle</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty ArrowStyleProperty = DependencyProperty.Register("ArrowStyle", typeof(ArrowStyles), typeof(TaskBarItem), new PropertyMetadata(ArrowStyles.Default, OnArrowStyleChanged));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.TaskBarItem.HeaderHeightProperty">HeaderHeight</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty HeaderHeightProperty = DependencyProperty.Register("HeaderHeight", typeof(int), typeof(TaskBarItem), new PropertyMetadata(0, OnHeaderHeightChanged));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.TaskBarItem.IsExpandedProperty">IsExpanded</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(TaskBarItem), new PropertyMetadata(true, OnIsExpandedChanged));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.TaskBarItem.IsHeaderShownProperty">IsHeaderShown</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty IsHeaderShownProperty = DependencyProperty.Register("IsHeaderShown", typeof(bool), typeof(TaskBarItem), new PropertyMetadata(true, OnIsHeaderShownChanged));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.TaskBarItem.ItemContentTemplate">IsHeaderShown</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty
          ItemContentTemplateProperty = DependencyProperty.Register("ItemContentTemplate", typeof(DataTemplate), typeof(TaskBarItem), new PropertyMetadata(null));


        internal string Theme
        {
            get { return (string)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsWindows7Theme.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register("Theme", typeof(string), typeof(TaskBarItem), new PropertyMetadata(string.Empty));
        


        #endregion Public Dependency Properties

        #region Private Fields

        /// <summary>
        /// The Head Border
        /// </summary>
        internal Border headborder;

        /// <summary>
        /// The Content Border
        /// </summary>
        internal Border contentborder;

        /// <summary>
        /// The Content Grid
        /// </summary>
        internal Grid contGrid;

        /// <summary>
        /// The Head Grid
        /// </summary>
        internal Grid headgrid;

        /// <summary>
        /// The Content Presenter
        /// </summary>
        internal ContentPresenter ct;

        /// <summary>
        /// The Outer Grid
        /// </summary>
        private Grid outGrid;

        /// <summary>
        /// The Dock Panel
        /// </summary>
        internal DockPanel subhead;

        /// <summary>
        /// The Toggle Button
        /// </summary>
        internal ToggleButton tbtn;

        /// <summary>
        /// TaskBarItem's ContentPresenter
        /// </summary>
        internal ContentPresenter taskbarcontent;

        internal Brush togglebrush = null;

        private bool isMouseover = false;

        #endregion Private Fields

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.TaskBarItem">TaskBarItem</see> class
        /// </summary>
        public TaskBarItem()
        {
            DefaultStyleKey = typeof(TaskBarItem);
        }
        #endregion Constructor

        #region Public Events
        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.TaskBarItem.ArrowStyle">ArrowStyle</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback ArrowStyleChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.TaskBarItem.HeaderHeight">HeaderHeight</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback HeaderHeightChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.TaskBarItem.IsExpanded">IsExpanded</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback IsExpandedChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.TaskBarItem.IsHeaderShown">IsHeaderShown</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback IsHeaderShownChanged;

        #endregion Public Events

        #region Public Properties
        /// <summary>
        /// Gets or sets a value indicating the ArrowStyle.
        /// Default value is Double Arrow.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:TaskBarItem Name=&quot;taskbaritem&quot;
        /// ArrowStyle=&quot;Triangle&quot;&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>taskbaritem.ArrowStyle=ArrowStyle.Triangle;  </para>
        /// </remarks>
        /// <value>
        /// Type: ArrowStyle
        /// </value>
        public ArrowStyles ArrowStyle
        {
            get
            {
                return (ArrowStyles)GetValue(ArrowStyleProperty);
            }

            set
            {
                SetValue(ArrowStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the height for the header
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:TaskBarItem Name=&quot;taskbaritem&quot;
        /// HeaderHeight=&quot;20&quot;&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>taskbaritem.HeaderHeight=20;</para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.int64.aspx">System.Int64</a>
        /// </value>
        public int HeaderHeight
        {
            get { return (int)GetValue(HeaderHeightProperty); }
            set { SetValue(HeaderHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the content should be shown or not.
        /// Default value is True.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:TaskBarItem Name=&quot;taskbaritem&quot;
        /// IsExpanded=&quot;true&quot;&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>taskbaritem.IsExpanded=true;</para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.boolean.aspx">System.Boolean</a>
        /// </value>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Header should be visible or not.
        /// Default Value is True.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:TaskBarItem Name=&quot;taskbaritem&quot;
        /// IsHeaderShown=&quot;true&quot;&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>taskbaritem.IsHeaderShown=true; </para>
        /// </remarks>
        /// <value>
        /// Type: <a
        /// href="http://msdn.microsoft.com/en-us/library/system.boolean.aspx">System.Boolean</a>
        /// </value>
        public bool IsHeaderShown
        {
            get { return (bool)GetValue(IsHeaderShownProperty); }
            set { SetValue(IsHeaderShownProperty, value); }
        }


        /// <summary>
        /// Gets or sets the item content template.
        /// </summary>
        /// <value>The item content template.</value>
        public DataTemplate ItemContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ItemContentTemplateProperty);
            }
            set
            {
                SetValue(ItemContentTemplateProperty, value);
            }
        }
        
        #endregion public Properties

        #region Public Methods
        /// <summary>
        /// Method used to change the ArrowStyle in runtime 
        /// </summary>
        /// <param name="arrowstyle">
        /// Represents selected ArrowStyle
        /// </param>
        public void changeArrow(ArrowStyles arrowstyle)
        {
            if (this.tbtn != null)
            {
                ControlTemplate ctt = null;
                string path = string.Empty;
                string width = string.Empty;
                string height = string.Empty;
                string gridheight = string.Empty;

                string angle = string.Empty;
                if (arrowstyle == ArrowStyles.Arrow)
                {
                    width = "Auto";
                    height = "Auto";
                    path = "F1 M 767.538,213.187C 767.317,213.442 766.961,213.449 766.743,213.203L 765.281,211.582C 765.062,211.336 765.065,210.929 765.286,210.674L 769.861,206.319C 769.9,206.274 769.943,206.237 769.989,206.207C 770.205,206.063 770.483,206.092 770.665,206.298L 775.094,210.71C 775.312,210.956 775.309,211.363 775.087,211.618L 773.722,213.078C 773.5,213.333 773.144,213.339 772.926,213.093L 770.214,210.601L 767.538,213.187 Z";
                }
                else if (arrowstyle == ArrowStyles.Default)
                {
                    if (Theme == "Blend")
                    {
                        width = "Auto";
                        height = "Auto";
                        path = "F1 M 203.361,55.0235L 203.455,52.9922L 205.33,53.0547L 211.017,58.461L 216.611,52.961L 218.486,53.1172L 218.58,54.8672L 211.361,61.961L 210.642,61.961L 203.361,55.0235 Z M 210.939,51.3958L 213.083,53.5392L 210.939,55.6826L 208.796,53.5392L 210.939,51.3958 Z ";
                    }
                    else if (Theme == "Windows7")
                    {
                        width = "Auto";
                        height = "Auto";
                        path = "F1 M -323.29,655.834L -314.5,655.789L -314.5,657.702L -316.276,657.702L -316.276,659.344L -318.053,659.344L -318.053,661.164L -319.737,661.164L -319.737,659.341L -321.559,659.341L -321.559,657.611L -323.29,657.611L -323.29,655.834 Z";
                    }
                    else if (Theme == "Office10")
                    {
                        width = "Auto";
                        height = "Auto";
                        path = "F1 M 82.3992,95.3123L 82.3992,93.2915L 83.4041,93.2915L 83.4041,92.3071L 84.4041,92.3071L 84.4041,91.2915L 85.404,91.2915L 85.404,90.2915L 86.4147,90.2915L 86.4147,89.281L 87.4147,89.281L 87.4147,90.2915L 88.4197,90.2915L 88.4197,91.2915L 89.4041,91.2915L 89.4041,92.3071L 90.4042,92.3071L 90.4042,93.2915L 91.4199,93.2915L 91.4199,95.3123L 89.42,95.3123L 89.42,94.328L 88.4044,94.328L 88.4044,93.3123L 87.4042,93.3123L 87.4042,92.3123L 87.4039,92.3123L 86.4198,92.3123L 86.404,92.3123L 86.404,93.3123L 85.404,93.3123L 85.404,94.3279L 84.3991,94.3279L 84.3991,95.3123L 82.3992,95.3123 Z";
                    }
                    else
                    {
                        width = "Auto";
                        height = "Auto";
                        //path = "F1 M 137.213,65.1772L 137.213,64.2709L 136.15,64.2709L 136.15,63.2397L 135.148,63.2397L 135.148,62.2084L 137.179,62.2084L 137.179,63.2397L 138.181,63.2397L 138.181,64.2553L 139.154,64.2553L 139.154,63.2397L 140.154,63.2397L 140.154,62.2084L 142.185,62.2084L 142.185,63.2397L 141.185,63.2397L 141.185,64.2709L 140.197,64.2709L 140.197,65.1772L 139.15,65.1772L 139.15,66.2397L 138.166,66.2397L 138.166,65.1772L 137.213,65.1772 Z M 137.15,60.9897L 137.15,60.0834L 136.088,60.0834L 136.088,59.0522L 135.086,59.0522L 135.086,58.0209L 137.117,58.0209L 137.117,59.0522L 138.119,59.0522L 138.119,60.0678L 139.092,60.0678L 139.092,59.0522L 140.092,59.0522L 140.092,58.0209L 142.123,58.0209L 142.123,59.0522L 141.123,59.0522L 141.123,60.0834L 140.135,60.0834L 140.135,60.9897L 139.088,60.9897L 139.088,62.0522L 138.103,62.0522L 138.103,60.9897L 137.15,60.9897 Z ";
                        path = "F1 M 767.538,213.187C 767.317,213.442 766.961,213.449 766.743,213.203L 765.281,211.582C 765.062,211.336 765.065,210.929 765.286,210.674L 769.861,206.319C 769.9,206.274 769.943,206.237 769.989,206.207C 770.205,206.063 770.483,206.092 770.665,206.298L 775.094,210.71C 775.312,210.956 775.309,211.363 775.087,211.618L 773.722,213.078C 773.5,213.333 773.144,213.339 772.926,213.093L 770.214,210.601L 767.538,213.187 Z";
                    }
                }
                else if (arrowstyle == ArrowStyles.SolidArrow)
                {
                    width = "Auto";
                    height = "Auto";
                    path = "F1 M 0.000000,0.786621 L 0.588867,0.786621 L 0.588867,0.000000 L 2.933594,0.000000 L 2.933594,0.786621 L 3.523438,0.786621 L 1.760742,2.143066 L 0.000000,0.786621 Z";
                }
                else if (arrowstyle == ArrowStyles.ArrowHead)
                {
                    width = "Auto";
                    height = "Auto";
                    path = "F1 M 9.781250,30.094727 L 19.550781,0.000000 L 12.129883,3.822266 L 9.775391,3.934570 L 7.420410,3.822266 L 0.000000,0.000000 L 9.769531,30.094727 L 9.769531,30.130859 L 9.775391,30.112305 L 9.781250,30.130859 L 9.781250,30.094727 Z";
                }
                else if (arrowstyle == ArrowStyles.Triangle)
                {
                    width = "Auto";
                    height = "Auto";
                    path = "F1 M 1.700195,0.000000 L 0.000000,5.075195 L 3.400391,5.075195 L 1.700195,0.000000 L 0.000000,5.075195 L 3.400391,5.075195 L 1.700195,0.000000 Z";
                }
                else if (arrowstyle == ArrowStyles.SingleArrow)
                {
                    width = "Auto";
                    height = "Auto";
                    path = "F1 M 20.666992,20.083496 L 10.333008,30.750000 L 0.000000,20.083496 L 0.000000,28.083008 L 10.333008,38.082031 L 10.333008,38.083008 L 10.333008,38.082031 L 10.333984,38.083008 L 10.333984,38.082031 L 20.666992,28.083008 L 20.666992,20.083496 Z";
                }

                //var brush = Application.Current.Resources["MouseOverBrushKey"] as LinearGradientBrush;
                //string fillcolor = brush.ToString();
                string fillcolor = @"<LinearGradientBrush StartPoint=""0,0"" EndPoint=""1,1"">
                                                                        <GradientStop Color=""Black"" Offset=""0""/>
                                                                        <GradientStop Color=""Black"" Offset=""0.5""/>
                                                                        <GradientStop Color=""Black"" Offset=""0.5""/>
                                                                        <GradientStop Color=""Black"" Offset=""0.659348""/>
                                                                        <GradientStop Color=""Black"" Offset=""1""/>
                                                                    </LinearGradientBrush>";

                angle = "180";

                if (Convert.ToInt16(this.tbtn.Tag) == 1 || Convert.ToInt16(this.tbtn.Tag) == 2)
                {
                    fillcolor = @"<LinearGradientBrush StartPoint=""0.5,1"" EndPoint=""0.5,0"">
                                    <GradientStop Color=""#CCCCCC"" Offset=""1""/>
                                    <GradientStop Color=""#A0A0A0"" Offset=""0.65512820512820518""/>
                                    <GradientStop Color=""#999999"" Offset=""0.6499999""/>
                                    <GradientStop Color=""#DBDBDB"" Offset=""0""/>
                                    <GradientStop Color=""#DBDBDB"" Offset=""0""/>
                                </LinearGradientBrush>";
                    if (arrowstyle == ArrowStyles.Triangle && Convert.ToInt16(this.tbtn.Tag) == 1)
                    {
                        angle = "90";
                        path = "F1 M 0.000000,0.000000 L 0.000000,255.623535 L 255.233398,127.811523 L 0.000000,0.000000 Z";
                    }
                }               
                gridheight = (this.HeaderHeight - 15).ToString();
                ctt = (ControlTemplate)XamlReader.Load(@"<ControlTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:vsm=""clr-namespace:System.Windows;assembly=System.Windows"" TargetType=""ToggleButton""><Grid x:Name=""Root"" Background=""Transparent""  RenderTransformOrigin=""0.5,0.5"" Margin=""3"" Height=""" + gridheight + @""" Width=""" + gridheight + @""">            
                                                                
                                                            <vsm:VisualStateManager.VisualStateGroups>
                                                                <vsm:VisualStateGroup x:Name=""CommonStates"">
                                                                    <vsm:VisualState x:Name=""Normal""/>
                                                                    <vsm:VisualState x:Name=""MouseOver"">
                                                                        <Storyboard>                                                                        
                                                                            <DoubleAnimationUsingKeyFrames Storyboard.TargetName=""UncheckedVisual"" Storyboard.TargetProperty=""Opacity"">
                                                                                <SplineDoubleKeyFrame KeyTime=""0"" Value=""1""/>
                                                                            </DoubleAnimationUsingKeyFrames> 
                                                                                                                                                    </Storyboard>
                                                                    </vsm:VisualState>
                                                                    <vsm:VisualState x:Name=""Disabled"">
                                                                    </vsm:VisualState>
                                                                </vsm:VisualStateGroup>
                                                                <vsm:VisualStateGroup x:Name=""CheckStates"">
                                                                    
                                                                    <vsm:VisualState x:Name=""Unchecked"">
                                                                        <Storyboard>
                                                                            <DoubleAnimationUsingKeyFrames BeginTime=""00:00:00"" Storyboard.TargetName=""UncheckedVisual"" Storyboard.TargetProperty=""(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)"">
                                                                                <SplineDoubleKeyFrame KeyTime=""00:00:00"" Value=""" + angle + @"""/>
                                                                            </DoubleAnimationUsingKeyFrames>
                                                                            
                                                                        </Storyboard>
                                                                    </vsm:VisualState>
                                                                    <vsm:VisualState x:Name=""Checked"">
                                                                        <Storyboard>

                                                                        </Storyboard>
                                                                    </vsm:VisualState>
                                                                </vsm:VisualStateGroup>
                                                            </vsm:VisualStateManager.VisualStateGroups>                                                        
                                                       <Path x:Name=""UncheckedVisual"" Width=""" + width + @""" Height=""" + height + @""" Stretch=""Fill"" Data=""" + path
                                                    + @""" RenderTransformOrigin=""0.5,0.5"" Opacity=""0.2"">
                                                                
                                                      <Path.RenderTransform>  <TransformGroup><ScaleTransform/> <SkewTransform/>
                                                                        <RotateTransform/>
                                                                        <TranslateTransform/>
                                                                    </TransformGroup>
                                                                </Path.RenderTransform>
                                                                <Path.Fill>" + fillcolor +

                                                            @"</Path.Fill>
                                                            </Path>
                                                                                                
                                                        </Grid>

                                                    </ControlTemplate> ");
                this.tbtn.Template = ctt;
            }
        }

        /// <summary>
        /// Applies the Template for the File Upload control
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.outGrid = this.GetTemplateChild("MainGrid") as Grid;
            this.headgrid = this.GetTemplateChild("HeaderGrid") as Grid;
            this.subhead = this.GetTemplateChild("Head") as Syncfusion.Windows.Controls.DockPanel;
            this.contGrid = this.GetTemplateChild("ContentGrid") as Grid;
            this.headborder = this.GetTemplateChild("HeaderBorder") as Border;
            this.contentborder = this.GetTemplateChild("ContentBorder") as Border;
            this.tbtn = this.GetTemplateChild("ExpanderButton") as ToggleButton;
            SolidColorBrush tbtncolor = (SolidColorBrush)this.tbtn.Background;
            string togglebuttonColor = tbtncolor.Color.ToString();
            if (tbtn != null)
            {
                this.tbtn.HorizontalAlignment = HorizontalAlignment.Center;
            }
            this.ct = this.GetTemplateChild("HeaderContent") as ContentPresenter;
            this.taskbarcontent = this.GetTemplateChild("TaskBarContent") as ContentPresenter;
            ControlTemplate ctt = null;
            string path = string.Empty;
            string width = string.Empty;
            string height = string.Empty;
            string gridheight = string.Empty;            

//            string fillcolor = @"<LinearGradientBrush StartPoint=""0,0"" EndPoint=""1,1"">
//                                                                        <GradientStop Color=""Black"" Offset=""0""/>
//                                                                        <GradientStop Color=""Black"" Offset=""0.5""/>
//                                                                        <GradientStop Color=""Black"" Offset=""0.5""/>
//                                                                        <GradientStop Color=""Black"" Offset=""0.659348""/>
//                                                                        <GradientStop Color=""Black"" Offset=""1""/>
//                                                                    </LinearGradientBrush>";

            string angle = "180";
            if (Theme == "Blend")
            {
                this.Foreground = new SolidColorBrush(Colors.White);             
            }
            else
            {
                this.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (this.tbtn != null)
            {
                if (Convert.ToInt16(this.tbtn.Tag) == 1 || Convert.ToInt16(this.tbtn.Tag) == 2)
                {
//                    fillcolor = @"<LinearGradientBrush StartPoint=""0.5,1"" EndPoint=""0.5,0"">
//                                    <GradientStop Color=""#CCCCCC"" Offset=""1""/>
//                                    <GradientStop Color=""#A0A0A0"" Offset=""0.65512820512820518""/>
//                                    <GradientStop Color=""#999999"" Offset=""0.6499999""/>
//                                    <GradientStop Color=""#DBDBDB"" Offset=""0""/>
//                                    <GradientStop Color=""#DBDBDB"" Offset=""0""/>
//                                </LinearGradientBrush>";
                    this.Foreground = new SolidColorBrush(Colors.Red);
                }
            }

            if (this.ArrowStyle == ArrowStyles.Default)
            {                
                if (Theme=="Blend")
                {
                    width = "Auto";
                    height = "Auto";
                    path = "F1 M 203.361,55.0235L 203.455,52.9922L 205.33,53.0547L 211.017,58.461L 216.611,52.961L 218.486,53.1172L 218.58,54.8672L 211.361,61.961L 210.642,61.961L 203.361,55.0235 Z M 210.939,51.3958L 213.083,53.5392L 210.939,55.6826L 208.796,53.5392L 210.939,51.3958 Z ";
                }
                else if (Theme == "Windows7")
                {
                    width="Auto";
                    height="Auto";
                    path = "F1 M -323.29,655.834L -314.5,655.789L -314.5,657.702L -316.276,657.702L -316.276,659.344L -318.053,659.344L -318.053,661.164L -319.737,661.164L -319.737,659.341L -321.559,659.341L -321.559,657.611L -323.29,657.611L -323.29,655.834 Z";
                }
                else if (Theme == "Office10")
                {
                    width = "Auto";
                    height = "Auto";
                    path = "F1 M 82.3992,95.3123L 82.3992,93.2915L 83.4041,93.2915L 83.4041,92.3071L 84.4041,92.3071L 84.4041,91.2915L 85.404,91.2915L 85.404,90.2915L 86.4147,90.2915L 86.4147,89.281L 87.4147,89.281L 87.4147,90.2915L 88.4197,90.2915L 88.4197,91.2915L 89.4041,91.2915L 89.4041,92.3071L 90.4042,92.3071L 90.4042,93.2915L 91.4199,93.2915L 91.4199,95.3123L 89.42,95.3123L 89.42,94.328L 88.4044,94.328L 88.4044,93.3123L 87.4042,93.3123L 87.4042,92.3123L 87.4039,92.3123L 86.4198,92.3123L 86.404,92.3123L 86.404,93.3123L 85.404,93.3123L 85.404,94.3279L 84.3991,94.3279L 84.3991,95.3123L 82.3992,95.3123 Z";
                }
                else
                {
                    width = "Auto";
                    height = "Auto";
                    path = "F1 M 137.213,65.1772L 137.213,64.2709L 136.15,64.2709L 136.15,63.2397L 135.148,63.2397L 135.148,62.2084L 137.179,62.2084L 137.179,63.2397L 138.181,63.2397L 138.181,64.2553L 139.154,64.2553L 139.154,63.2397L 140.154,63.2397L 140.154,62.2084L 142.185,62.2084L 142.185,63.2397L 141.185,63.2397L 141.185,64.2709L 140.197,64.2709L 140.197,65.1772L 139.15,65.1772L 139.15,66.2397L 138.166,66.2397L 138.166,65.1772L 137.213,65.1772 Z M 137.15,60.9897L 137.15,60.0834L 136.088,60.0834L 136.088,59.0522L 135.086,59.0522L 135.086,58.0209L 137.117,58.0209L 137.117,59.0522L 138.119,59.0522L 138.119,60.0678L 139.092,60.0678L 139.092,59.0522L 140.092,59.0522L 140.092,58.0209L 142.123,58.0209L 142.123,59.0522L 141.123,59.0522L 141.123,60.0834L 140.135,60.0834L 140.135,60.9897L 139.088,60.9897L 139.088,62.0522L 138.103,62.0522L 138.103,60.9897L 137.15,60.9897 Z ";
                }
            }
            else if (this.ArrowStyle == ArrowStyles.Arrow)
            {
                width = "Auto";
                height = "Auto";
                //path = "F1 M 0.000000,1.425781 L 0.000000,0.836914 L 0.834473,1.670898 L 0.834473,0.000000 L 1.251465,0.000000 L 1.251465,1.670898 L 2.086914,0.836914 L 2.086914,1.425781 L 1.042969,2.468750 L 0.000000,1.425781 Z";
                path = "F1 M 767.538,213.187C 767.317,213.442 766.961,213.449 766.743,213.203L 765.281,211.582C 765.062,211.336 765.065,210.929 765.286,210.674L 769.861,206.319C 769.9,206.274 769.943,206.237 769.989,206.207C 770.205,206.063 770.483,206.092 770.665,206.298L 775.094,210.71C 775.312,210.956 775.309,211.363 775.087,211.618L 773.722,213.078C 773.5,213.333 773.144,213.339 772.926,213.093L 770.214,210.601L 767.538,213.187 Z";
            }
    
           else if (this.ArrowStyle == ArrowStyles.SolidArrow)
            {
                width = "Auto";
                height = "Auto";
                path = "F1 M 0.000000,0.786621 L 0.588867,0.786621 L 0.588867,0.000000 L 2.933594,0.000000 L 2.933594,0.786621 L 3.523438,0.786621 L 1.760742,2.143066 L 0.000000,0.786621 Z";
            }
            else if (this.ArrowStyle == ArrowStyles.ArrowHead)
            {
                width = "Auto";
                height = "Auto";
                path = "F1 M 9.781250,30.094727 L 19.550781,0.000000 L 12.129883,3.822266 L 9.775391,3.934570 L 7.420410,3.822266 L 0.000000,0.000000 L 9.769531,30.094727 L 9.769531,30.130859 L 9.775391,30.112305 L 9.781250,30.130859 L 9.781250,30.094727 Z";
            }
            else if (this.ArrowStyle == ArrowStyles.Triangle)
            {
                width = "Auto";
                height = "Auto";
               
                if (Convert.ToInt16(this.tbtn.Tag) == 1)
                {
                    path = "F1 M 0.000000,0.000000 L 0.000000,255.623535 L 255.233398,127.811523 L 0.000000,0.000000 Z";
                }
                else
                {
                    path = "F1 M 16.797852,26.131836 L 0.000000,26.131836 L 0.000000,27.913086 L 8.392578,35.858398 L 8.392578,35.870117 L 8.398438,35.864258 L 8.405273,35.870117 L 8.405273,35.858398 L 16.797852,27.913086 L 16.797852,26.131836 Z";
                }
            }
            else if (this.ArrowStyle == ArrowStyles.SingleArrow)
            {
                width = "Auto";
                height = "Auto";
                path = "F1 M 20.666992,20.083496 L 10.333008,30.750000 L 0.000000,20.083496 L 0.000000,28.083008 L 10.333008,38.082031 L 10.333008,38.083008 L 10.333008,38.082031 L 10.333984,38.083008 L 10.333984,38.082031 L 20.666992,28.083008 L 20.666992,20.083496 Z";
            }

            gridheight = (this.HeaderHeight - 15).ToString();
            string Fill = string.Empty;
            string borderFill = string.Empty;
            Fill = "Transparent";
            borderFill = "Transparent";
            if (Theme == "Windows7")
            {
                Fill = "#FFF8D275";
                borderFill = "#FFC19D47";              
            }            

            ctt = (ControlTemplate)XamlReader.Load(@"<ControlTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:vsm=""clr-namespace:System.Windows;assembly=System.Windows"" TargetType=""ToggleButton""><Grid x:Name=""Root"" Background=""Transparent"" RenderTransformOrigin=""0.5,0.5"" Margin=""2"" Height=""" + gridheight + @""" Width=""" + gridheight + @""">            
                                                            <vsm:VisualStateManager.VisualStateGroups>
                                                                <vsm:VisualStateGroup x:Name=""CommonStates"">
                                                                    <vsm:VisualState x:Name=""Normal""/>
                                                                    <vsm:VisualState x:Name=""MouseOver"">
                                                                        <Storyboard>                                                                          
                                                                           <DoubleAnimationUsingKeyFrames Storyboard.TargetName=""UncheckedVisual"" Storyboard.TargetProperty=""Opacity"">
                                                                                <SplineDoubleKeyFrame KeyTime=""0"" Value=""1""/>
                                                                            </DoubleAnimationUsingKeyFrames>                                                                                                                                                                                                                                                                     
                                                                        </Storyboard>
                                                                    </vsm:VisualState>
                                                                    <vsm:VisualState x:Name=""Disabled"">
                                                                    </vsm:VisualState>
                                                                </vsm:VisualStateGroup>
                                                                <vsm:VisualStateGroup x:Name=""CheckStates"">                                                                    
                                                                    <vsm:VisualState x:Name=""Unchecked"">
                                                                        <Storyboard>
                                                                            <DoubleAnimationUsingKeyFrames BeginTime=""00:00:00"" Storyboard.TargetName=""UncheckedVisual"" Storyboard.TargetProperty=""(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)"">
                                                                                <SplineDoubleKeyFrame KeyTime=""00:00:00"" Value=""" + angle + @"""/>
                                                                            </DoubleAnimationUsingKeyFrames>                                                                            
                                                                        </Storyboard>
                                                                    </vsm:VisualState>
                                                                    <vsm:VisualState x:Name=""Checked"">
                                                                        <Storyboard> 
                                                                        </Storyboard>
                                                                    </vsm:VisualState>
                                                                </vsm:VisualStateGroup>
                                                            </vsm:VisualStateManager.VisualStateGroups>                                                               
                                                       <Path x:Name=""UncheckedVisual"" Height=""" + height + @""" Width=""" + width + @""" Fill=""" + togglebuttonColor + @""" Stretch=""Fill"" Data=""" + path + @"""
                                                   RenderTransformOrigin=""0.5,0.5"" Opacity=""1"">                                                                                                                      
                                                                   <Path.RenderTransform>  <TransformGroup><ScaleTransform/> <SkewTransform/>
                                                                        <RotateTransform/>
                                                                        <TranslateTransform/>
                                                                    </TransformGroup>
                                                                </Path.RenderTransform>                                                                                                                                                                                       
                                                            </Path>                                                                                                                                   
                                                        </Grid>
                                                    </ControlTemplate> ");
            if (this.tbtn != null)
            {                
                this.tbtn.Template = ctt;
            }
            if (this.IsHeaderShown == false)
            {
                this.headgrid.Visibility = Visibility.Collapsed;
            }

            TaskBarItemAutomationPeer peer = TaskBarItemAutomationPeer.CreatePeerForElement(this) as TaskBarItemAutomationPeer;
            if (this.IsExpanded)
            {
                peer.Expand();
            }
            else
            {
                peer.Collapse();
            }

            if (this.headgrid != null && this.tbtn != null)
            {
                this.headgrid.Height = this.HeaderHeight;
                this.headgrid.MouseEnter += new MouseEventHandler(subhead_MouseEnter);
                this.headgrid.MouseLeave += new MouseEventHandler(subhead_MouseLeave);
                this.headgrid.MouseMove += new MouseEventHandler(subhead_MouseEnter);
                this.headgrid.MouseLeftButtonUp += new MouseButtonEventHandler(this.HeadGrid_MouseLeftButtonUp);
                this.tbtn.Click += new RoutedEventHandler(this.tbtn_Click);               
            }
        }
      
            
        /// <summary>
        /// Handles the MouseLeave event of the subhead control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void subhead_MouseLeave(object sender, MouseEventArgs e)
        {
            this.isMouseover = false;
          //  this.subhead.Background = new SolidColorBrush(Colors.Transparent);
            UpdateVisualState(this.isMouseover);
        }

        /// <summary>
        /// Handles the MouseEnter event of the subhead control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void subhead_MouseEnter(object sender, MouseEventArgs e)
        {
            this.isMouseover = true;
            //this.subhead.Background = new SolidColorBrush(Colors.White);
            UpdateVisualState(this.isMouseover);                
        }

        /// <summary>
        /// Method to set the visual state transition.
        /// </summary>
        /// <param name="useTransitions">Whether to apply transition or not.</param>
        /// <param name="stateNames">Which state transition is to be applied</param>
        private void GoToState(bool useTransitions, params string[] stateNames)
        {
            if (stateNames != null)
            {
                foreach (string str in stateNames)
                {
                    if (VisualStateManager.GoToState(this.tbtn, str, useTransitions))
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Method to update VisualState.
        /// </summary>
        /// <param name="useTransitions">Boolean value to know whether the transition is to be applied or not.</param>
        private void UpdateVisualState(bool useTransitions)
        {
            if (this.isMouseover)
            {
                this.GoToState(useTransitions, new string[] { "MouseOver" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Normal" });
            }
        }

        #endregion Public Methods

        #region Protected Virtual Methods

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.TaskBarItem.ArrowStyleChanged">ArrowStyleChanged</see> event.
        /// </summary>
        /// <param name="e" >Property change details, such as old value and new value</param>
        protected virtual void OnArrowStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.ArrowStyleChanged != null)
            {
                this.ArrowStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.TaskBarItem.HeaderHeightChanged">HeaderHeightChanged</see> event.
        /// </summary>
        /// <param name="e" >Property change details, such as old value and new value</param>
        protected virtual void OnHeaderHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.HeaderHeightChanged != null)
            {
                this.HeaderHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.TaskBarItem.IsExpandedChanged">IsExpandedChanged</see> event.
        /// </summary>
        /// <param name="e" >Property change details, such as old value and new value</param>
        protected virtual void OnIsExpandedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsExpandedChanged != null)
            {
                this.IsExpandedChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.TaskBarItem.IsHeaderShownChanged">IsHeaderShownChanged</see> event.
        /// </summary>
        /// <param name="e" >Property change details, such as old value and new value</param>
        protected virtual void OnIsHeaderShownChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsHeaderShownChanged != null)
            {
                this.IsHeaderShownChanged(this, e);
            }
        }

        #endregion Protected Virtual Methods

        #region Private Static Mehtods

        /// <summary>
        /// Calls OnArrowStyleChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current TaskBarItem Instance</param>
        /// <param name="e" >Property change details, such as old value and new value</param>
        private static void OnArrowStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskBarItem instance = (TaskBarItem)d;
            if (e.NewValue != e.OldValue)
            {
                instance.OnArrowStyleChanged(e);
                instance.changeArrow(instance.ArrowStyle);
            }
        }

        /// <summary>
        /// Calls OnHeaderHeightChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current TaskBarItem Instance</param>
        /// <param name="e" >Property change details, such as old value and new value</param>
        private static void OnHeaderHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskBarItem instance = (TaskBarItem)d;
            if (e.NewValue != e.OldValue)
            {
                instance.OnHeaderHeightChanged(e);
            }

            if (instance.headgrid != null)
            {
                instance.headgrid.Height = (int)e.NewValue;
                if (instance.IsExpanded == false)
                {
                    instance.headgrid.Width = double.NaN;
                }

                instance.changeArrow(instance.ArrowStyle);
            }
        }

        /// <summary>
        /// Calls OnIsExpandedChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current TaskBarItem Instance</param>
        /// <param name="e" >Property change details, such as old value and new value</param>
        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskBarItem instance = (TaskBarItem)d;
            if (e.NewValue != e.OldValue)
            {
                instance.OnIsExpandedChanged(e);
            }

            TaskBarItemAutomationPeer peer = TaskBarItemAutomationPeer.CreatePeerForElement(instance) as TaskBarItemAutomationPeer;
            if (instance.headborder != null)
            {
                if (instance.IsExpanded)
                {
                    peer.Expand();
                }
                else
                {
                    peer.Collapse();
                }
            }
        }

        /// <summary>
        /// Calls OnIsHeaderShownChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current TaskBarItem Instance</param>
        /// <param name="e" >Property change details, such as old value and new value</param>
        private static void OnIsHeaderShownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskBarItem instance = (TaskBarItem)d;
            if (e.NewValue != e.OldValue)
            {
                instance.OnIsHeaderShownChanged(e);
            }

            if (instance.headborder != null)
            {
                if (instance.IsHeaderShown == false)
                {
                    instance.headgrid.Visibility = Visibility.Collapsed;
                }
                else if (instance.IsHeaderShown == true)
                {
                    instance.headgrid.Visibility = Visibility.Visible;
                }
            }
        }

        #endregion Private Static Mehtods

        #region Private Methods

        /// <summary>
        /// Provides handling for when the Toggle buton Click Event takes place
        /// </summary>
        /// <param name="sender">The Toggle Button</param>
        /// <param name="e">Event Argument</param>
        private void tbtn_Click(object sender, RoutedEventArgs e)
        {
            this.IsExpanded = !(bool)this.IsExpanded;
            if ((bool)this.IsExpanded)
            {
                this.contGrid.Visibility = Visibility.Visible;
                this.headgrid.Width = double.NaN;
            }
            else
            {
                if (this.headgrid.ActualWidth <= this.contGrid.ActualWidth)
                {
                    this.headgrid.Width = this.contGrid.ActualWidth;
                }

                this.contGrid.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Provides handling for when the Header Grid MouseLeftButonUp takes place
        /// </summary>
        /// <param name="sender">The Header Grid Onject</param>
        /// <param name="e">Event Argument</param>
        private void HeadGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.IsExpanded = !(bool)this.IsExpanded;
            if ((bool)this.IsExpanded)
            {
                this.tbtn.IsChecked = false;
                this.contGrid.Visibility = Visibility.Visible;
                this.headgrid.Width = double.NaN;
            }
            else
            {
                if (this.headgrid.ActualWidth <= this.contGrid.ActualWidth)
                {
                    this.headgrid.Width = this.contGrid.ActualWidth;
                }

                this.contGrid.Visibility = Visibility.Collapsed;
                this.tbtn.IsChecked = true;
            }

            this.isMouseover = true;
            UpdateVisualState(this.isMouseover);
        }
        #endregion Private Methods

        /// <summary>
        /// When implemented in a derived class, returns class-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> implementations for the Silverlight automation infrastructure.
        /// </summary>
        /// <returns>
        /// The class-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> subclass to return.
        /// </returns>
        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            return new TaskBarItemAutomationPeer(this);
        }
    }
}