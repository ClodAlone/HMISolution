#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Olap
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Syncfusion.Olap.Data;
    using Syncfusion.Olap.Manager;
    using Syncfusion.Windows.Shared.Olap;
    using System.Windows.Controls.Primitives;
    using Syncfusion.Windows.Shared;
    using Syncfusion.Olap.Reports;

    /// <summary>
    /// Interaction logic for SplitButton.xaml
    /// </summary>
    public class SplitButton : Control
    {
        #region Dependency Property Implementation

        /// <summary>
        /// Host Dependency Property Implementation
        /// </summary>
        public static readonly DependencyProperty HostProperty =
            DependencyProperty.Register("Host", typeof(AxisElementBuilder), typeof(SplitButton), new UIPropertyMetadata(null));

        /// <summary>
        /// MetaTreeNode Dependency Property Implementation
        /// </summary>
        public static readonly DependencyProperty MetaTreeNodeProperty =
            DependencyProperty.Register("MetaTreeNode", typeof(MetaTreeNode), typeof(SplitButton), new UIPropertyMetadata(null));

        /// <summary>
        /// Text Dependency Property Implementation
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SplitButton), new UIPropertyMetadata(SplitButton.TextChanged));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>The host.</value>
        public AxisElementBuilder Host
        {
            get { return (AxisElementBuilder)GetValue(HostProperty); }
            set { SetValue(HostProperty, value); }
        }

        /// <summary>
        /// Gets or sets the meta tree node.
        /// </summary>
        /// <value>The meta tree node.</value>
        public MetaTreeNode MetaTreeNode
        {
            get { return (MetaTreeNode)GetValue(MetaTreeNodeProperty); }
            set { SetValue(MetaTreeNodeProperty, value); }
        }
        internal Border InternalBorder
        {
            get;
            set;
        }
        public Button InternalButton
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public IOlapDataManager OlapDataManager
        {
            get
            {
                if (this.Host != null)
                {
                    return this.Host.OlapDataManager;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion

        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));

        }


        public override void OnApplyTemplate()
        {
            InternalBorder = GetTemplateChild("PART_SplitButton_Border") as Border;
            InternalButton = GetTemplateChild("PART_SplitButton") as Button;
            Popup popup = GetTemplateChild("PART_OptionPopup") as Popup;
            Button moveup = GetTemplateChild("PART_Moveup") as Button;
            Button movedown = GetTemplateChild("PART_Movedown") as Button;
            Button delete = GetTemplateChild("PART_Delete") as Button;
            if (this.InternalBorder != null && popup != null)
            {
                PopupDelayService service = new PopupDelayService(InternalBorder, popup);
                popup.Opened += delegate(object sender, EventArgs e)
                {
                    if (moveup != null && movedown != null)
                    {
                        //// Updating the enable status
                        moveup.IsEnabled = GetMoveupStatus();
                        movedown.IsEnabled = GetModedownStatus();
                        moveup.Click += delegate(object sen, RoutedEventArgs e1)
                        {
                            if (this.Host != null)
                            {
                                int currentIndex = this.Host.MetaTreeNodes.IndexOf(this.MetaTreeNode);
                                if (currentIndex > 0)
                                {
                                    //// backing the host, because soon after the index change the host binding will be broken
                                    AxisElementBuilder axisBuilder = this.Host;
                                    //// decrementing the index
                                    this.Host.MetaTreeNodes.Move(currentIndex, currentIndex - 1);
                                    //// refreshing the OLAP data manager with latest items
                                    axisBuilder.RefreshElements(null, true);
                                    moveup.IsEnabled = GetMoveupStatus();
                                }
                            }

                        };
                        movedown.Click += delegate(object sen, RoutedEventArgs e1)
                        {
                            if (this.Host != null)
                            {
                                int itemsCount = this.Host.listBoxAxisElements.Items.Count;
                                int currentIndex = this.Host.MetaTreeNodes.IndexOf(this.MetaTreeNode);
                                if (itemsCount > currentIndex)
                                {
                                    //// backing the host, because soon after the index change the host binding will be broken
                                    AxisElementBuilder axisBuilder = this.Host;
                                    //// decrementing the index
                                    axisBuilder.MetaTreeNodes.Move(currentIndex, currentIndex + 1);
                                    //// refreshing the OLAP data manager with latest items
                                    axisBuilder.RefreshElements(null, true);
                                    movedown.IsEnabled = GetModedownStatus();
                                }
                            }
                        };
                    }
                    if (delete != null)
                    {
                        delete.Click += delegate(object sen, RoutedEventArgs e1)
                        {
                            if (this.Host != null)
                            {
                                //// backing the host, because soon after the index change the host binding will be broken
                                AxisElementBuilder axisBuilder = this.Host;
                                axisBuilder.MetaTreeNodes.Remove(this.MetaTreeNode);
                                //// refreshing the OLAP data manager with latest items
                                axisBuilder.RefreshElements(null, true);
                            }
                        };
                    }
                };
            }
            if (InternalButton != null)
                InternalButton.Click += new RoutedEventHandler(button_Click);
            base.OnApplyTemplate();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (this.InternalBorder != null)
            {
                if (double.IsNaN(this.InternalBorder.Width))
                    this.InternalBorder.Measure(new Size(double.MaxValue, double.MaxValue));
                return new Size(this.InternalBorder.DesiredSize.Width, 25);
            }
            return new Size(50, 24);
        }

        #region Private Methods

        /// <summary>
        /// Handles the Click event of the Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                /// Reset the IsSelecte/Checked state
                this.MetaTreeNode.RevertIsSelectedChanged(true);

                //Get VisualStyle
                String visualStyle = SkinStorage.GetVisualStyle(this).ToString();    
 
                /// Adding the child nodes to temporary observable collection for passing 
                /// into the editors
                ObservableCollection<MetaTreeNode> metaTreeNodes = new ObservableCollection<MetaTreeNode>();
                foreach (MetaTreeNode metaTreeNode in this.MetaTreeNode.ChildNodes)
                {
                    metaTreeNodes.Add(metaTreeNode);
                }
                //// If the current node is measure then open MeasureEditor
                if (this.Text == PropertyConstants.MeasrueNodeName)
                {
                    MeasureEditor measureEditor = new MeasureEditor(this, metaTreeNodes);
                    measureEditor.FlowDirection = this.FlowDirection;
                    Point val = this.PointToScreen(new Point(this.Margin.Top, this.Margin.Left));
                    measureEditor.Top = val.Y + this.ActualHeight + 2;
                    measureEditor.Left = val.X;
                    Window parentWindow = Common.GetParentWindow<Window>(this);
                    if (parentWindow != null)
                    {
                        measureEditor.Owner = parentWindow;
                        measureEditor.ShowInTaskbar = false;
                    }
                    SkinStorage.SetVisualStyle(measureEditor, visualStyle); 
                    measureEditor.ShowDialog();
                    
                }
                else if (this.MetaTreeNode.NodeType == MetaTreeNodeType.CalculatedMember)
                {   
                    CalcMemberEditor calcMemberEditor = new CalcMemberEditor(this);
                    calcMemberEditor.FlowDirection = this.FlowDirection;
                    SkinStorage.SetVisualStyle(calcMemberEditor, SkinStorage.GetVisualStyle(this));
                    Point val = this.PointToScreen(new Point(this.Margin.Top, this.Margin.Left));
                    calcMemberEditor.Top = val.Y + this.ActualHeight + 2;
                    calcMemberEditor.Left = val.X;
                    calcMemberEditor.AutoExecute = this.Host.AutoExecute;
                    calcMemberEditor.ShowDialog();
                }
               else if (this.MetaTreeNode.NodeType != MetaTreeNodeType.NamedSet)
                {
                    //TODO: Added below code since Active pivot is not yet include the union function support for Amazon cube. so avoid to filter the members in either row or column axis.
                    //if (this.OlapDataManager.CurrentCubeName != "Amazon" || this.OlapDataManager.DataProvider.ProviderName != Syncfusion.Olap.DataProvider.Providers.ActivePivot || this.Host.Axis == Syncfusion.Olap.Reports.AxisPosition.Slicer)
                    {
                        //// Else opening the member editor
                        MemberEditor memberEditor = new MemberEditor(this, metaTreeNodes);
                        memberEditor.FlowDirection = this.FlowDirection;
                        memberEditor.PopupTitle = this.Text;
                        Point val = this.PointToScreen(new Point(this.Margin.Top, this.Margin.Left));
                        memberEditor.Top = val.Y + this.ActualHeight + 2;
                        memberEditor.Left = val.X;
                        Window parentWindow = Common.GetParentWindow<Window>(this);
                        if (parentWindow != null)
                        {
                            memberEditor.Owner = parentWindow;
                            memberEditor.ShowInTaskbar = false;
                        }
                        SkinStorage.SetVisualStyle(memberEditor, visualStyle);
                        memberEditor.ShowDialog();
                    }
                    //else
                    //{
                    //    MessageBox.Show("Filtering is not supported currently for dimensions in Categorical(Column) and Series(Row) axes.", "OlapClient");
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception Message");
            }
        }

        private bool GetMoveupStatus()
        {
            if (this.Host != null)
            {
                int itemsCount = this.Host.listBoxAxisElements.Items.Count;
                if (itemsCount == 1)
                    return false;
                int index = this.Host.MetaTreeNodes.IndexOf(this.MetaTreeNode);
                if (index > 0)
                    return true;
            }
            return false;
        }

        private bool GetModedownStatus()
        {
            if (this.Host != null)
            {
                int itemsCount = this.Host.listBoxAxisElements.Items.Count;
                if (itemsCount == 1)
                    return false;
                int index = this.Host.MetaTreeNodes.IndexOf(this.MetaTreeNode);
                if (itemsCount - 1 > index)
                    return true;
            }
            return false;
        }


        #endregion

        #region Public static Methods

        /// <summary>
        /// Texts the changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void TextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is SplitButton)
            {
                //SplitButton btn = (SplitButton)dependencyObject;
                //btn.tbSplitButton.Text = btn.Text;
            }
        }

        #endregion
    }
}
