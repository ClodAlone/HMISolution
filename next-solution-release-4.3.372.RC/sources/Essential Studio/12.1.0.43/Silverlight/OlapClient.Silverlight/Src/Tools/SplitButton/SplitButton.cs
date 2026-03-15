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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.OlapSilverlight.Data;
using Syncfusion.OlapSilverlight.Manager;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Syncfusion.Silverlight.Client.Olap;
using Syncfusion.Windows.Controls.Theming;
using Syncfusion.OlapSilverlight.Reports;

namespace Syncfusion.Silverlight.Tools.Olap
{
    [DesignTimeVisible(false)]
    public class SplitButton : Control
    {
        #region Constructor

        public SplitButton()
        {
            this.DefaultStyleKey = typeof(SplitButton);
        }

        #endregion

        #region Public properties

        public OlapDataManager OlapDataManager
        {
            get
            {
                return this.Parent.OlapDataManager;
            }
        }

        public Button  InternalButton { get; set; }
        public TextBlock TextBlock { get; set; }
        private Popup ContextMenu { get; set; }

        public TextBlock Menu_Firts { get; private set; }
        public TextBlock Menu_Up { get; private set; }
        public TextBlock Menu_Down { get; private set; }
        public TextBlock Menu_Last { get; private set; }
        public TextBlock Menu_Delete { get; private set; }

        #endregion

        #region Dependency properties

        #region AllowMultiMemberSelection
        /// <summary>
        /// Gets or sets a value indicating whether [show checkbox in MemberEditor TreeView].
        /// </summary>
        /// <value><c>true</c> if [show checkbox in MemberEditor TreeView]; otherwise, <c>false</c>.</value>
        public bool AllowMultiMemberSelection
        {
            get { return (bool)GetValue(MultiMemberSelectionProperty); }
            set { SetValue(MultiMemberSelectionProperty, value); }

        }
        //using AllowMultiMemberSelection Dependency Property
        internal static readonly DependencyProperty MultiMemberSelectionProperty = DependencyProperty.Register("AllowMultiMemberSelection", typeof(bool), typeof(SplitButton), new PropertyMetadata(true));

        #endregion

        public MetaTreeNode MetaTreeNode
        {
            get { return (MetaTreeNode)GetValue(MetaTreeNodeProperty); }
            set { SetValue(MetaTreeNodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MetaTreeNode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MetaTreeNodeProperty =
            DependencyProperty.Register("MetaTreeNode", typeof(MetaTreeNode), typeof(SplitButton), new PropertyMetadata(null));


        public AxisElementBuilder Parent
        {
            get { return (AxisElementBuilder)GetValue(AxisProperty); }
            set { SetValue(AxisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Axis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AxisProperty =
            DependencyProperty.Register("Axis", typeof(AxisElementBuilder), typeof(SplitButton), new PropertyMetadata(null));
      

        #endregion

        #region Overrided methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.InternalButton = this.GetTemplateChild("PART_SplitButton") as Button;
            this.ContextMenu = this.GetTemplateChild("PART_ContextMenu") as Popup;
            this.TextBlock = this.GetTemplateChild("txtblock") as TextBlock;
            ContextMenuLayOutupdate();
            this.Parent = this.FindParent(this as SplitButton) as AxisElementBuilder;
            if (this.Parent != null)
                this.AllowMultiMemberSelection = this.Parent.AllowMultiMemberSelection;
            if (this.InternalButton != null)
            {
                if (this.Parent != null && this.TextBlock != null && this.TextBlock.Text != null)
                {
                    if (this.MetaTreeNode.NodeType == MetaTreeNodeType.MeasureGroup)
                        this.InternalButton.IsEnabled = this.Parent.IsMeasureEditorOpen;
                    else
                        this.InternalButton.IsEnabled = this.Parent.IsMemberEditorOpen;
                }

                if (this.MetaTreeNode.NodeType != MetaTreeNodeType.NamedSet)
                {
                    this.InternalButton.MouseMove += new MouseEventHandler(InternalButton_MouseMove);
                    this.InternalButton.Click += InternalButton_Click;
                }
                else
                    this.InternalButton.Visibility = Visibility.Collapsed;
            }
            this.TextBlock.MouseMove += new MouseEventHandler(TextBlock_MouseMove);
            this.TextBlock.MouseLeave += new MouseEventHandler(SplitButton_MouseLeave);
            this.InternalButton.MouseLeave += new MouseEventHandler(SplitButton_MouseLeave);
            //this.AddHandler(Control.MouseLeftButtonUpEvent, new MouseButtonEventHandler(InternalButton_MouseLeftButtonUp), true);
        }

        void SplitButton_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        void TextBlock_MouseMove(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseOver", true);
        }

        void InternalButton_MouseMove(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "DropDownMouseOver", true);
        }

        private void ContextMenuLayOutupdate()
        {
            this.Menu_Up = this.GetTemplateChild("PART_Up") as TextBlock;
            this.Menu_Firts = this.GetTemplateChild("PART_First") as TextBlock;
            this.Menu_Down = this.GetTemplateChild("PART_Down") as TextBlock;
            this.Menu_Last = this.GetTemplateChild("PART_Last") as TextBlock;
            this.Menu_Delete = this.GetTemplateChild("PART_Delete") as TextBlock;
            TagEvents();
        }

        #endregion

        #region Private Methods

        private object FindParent(UIElement obj)
        {
            if (obj != null)
            {
                var T = VisualTreeHelper.GetParent(obj);
                if (T is AxisElementBuilder)
                    return T;
                else
                    return FindParent(T as UIElement);
            }
            else
                return null;
        }

        private void TagEvents()
        {
            //this.InternalButton.MouseRightButtonDown += InternalButton_MouseRightButtonDown;
            this.ContextMenu.LostFocus += ContextMenu_LostFocus;
            this.ContextMenu.CaptureMouse();
            this.Menu_Up.MouseLeftButtonDown += Menu_MouseLeftButtonDown;
            this.Menu_Firts.MouseLeftButtonDown += Menu_MouseLeftButtonDown;
            this.Menu_Down.MouseLeftButtonDown += Menu_MouseLeftButtonDown;
            this.Menu_Last.MouseLeftButtonDown += Menu_MouseLeftButtonDown;
            this.Menu_Delete.MouseLeftButtonDown += Menu_MouseLeftButtonDown;
        }
    
        #endregion

        #region Events

        #region Button click handler

        void InternalButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.ResourceDictionary source = new System.Windows.ResourceDictionary();
            //// Reset the IsSelecte/Checked state
            this.MetaTreeNode.RevertIsSelectedChanged(true);
            DependencyObject element = VisualTreeHelper.GetParent(this.Parent.Parent);
            while (element != null && !(element is Syncfusion.Silverlight.Client.Olap.OlapClient))
            {
                element = VisualTreeHelper.GetParent(element);
            }
            Syncfusion.Silverlight.Client.Olap.OlapClient olapClient = element as Syncfusion.Silverlight.Client.Olap.OlapClient;
            
            ObservableCollection<MetaTreeNode> metaTreeNodes = new ObservableCollection<MetaTreeNode>();
            //// Adding child nodes to an Observable collection to pass it to Member Editor
            foreach (MetaTreeNode child in this.MetaTreeNode.ChildNodes)
            {
                metaTreeNodes.Add(child);
            }

            if (this.MetaTreeNode.NodeType == MetaTreeNodeType.MeasureGroup)
            {
                //// Initializing the measure editor with the child nodes
                MeasureEditor measureEditor = new MeasureEditor(this, metaTreeNodes);
                measureEditor.FlowDirection = this.FlowDirection;
                measureEditor.VisualStyle = (Windows.Shared.VisualStyle)Enum.Parse(typeof(Windows.Shared.VisualStyle), this.Parent.SplitButtonVisualStyle.ToString(), true);
                measureEditor.Background = olapClient.Background;
                SkinManager.SetVisualStyle(measureEditor, (VisualStyle)Enum.Parse(typeof(VisualStyle), this.Parent.SplitButtonVisualStyle.ToString(), true));
                measureEditor.ShowDialog();
               
            }
           
            else if (this.MetaTreeNode.NodeType == MetaTreeNodeType.CalculatedMember)
            {
                source.Source = new Uri("/Syncfusion.OlapClient.Silverlight;component/Tools/CubeDimensionBrowser/CubeDimensionBrowser.xaml", UriKind.RelativeOrAbsolute);
                CalcMemberEditor calcMemberEditor = new CalcMemberEditor(this);
                
                if (olapClient != null && calcMemberEditor.CalcMeasureTreeView.OlapDataManager == null || this.OlapDataManager.CurrentCubeName != calcMemberEditor.CurrentCubeName)
                {
                    calcMemberEditor.CurrentCubeName = this.OlapDataManager.CurrentCubeName;
                    calcMemberEditor.AutoExecute = olapClient.AutoExecute;
                    calcMemberEditor.CalcMeasureTreeView.OlapDataManager = this.OlapDataManager;
                    calcMemberEditor.CalcMeasureTreeView.ItemsSource = olapClient.CubeDimensionBrowser.ItemsSource;
                    calcMemberEditor.MemberTypeText.ItemsSource = (olapClient.CubeDimensionBrowser.Items[0] as MetaTreeNode).ChildNodes.Where(i => i.NodeType == MetaTreeNodeType.Dimension);
                    calcMemberEditor.MemberTypeText.SelectedItem = calcMemberEditor.MemberTypeText.ItemsSource.OfType<MetaTreeNode>().Where(i => i.UniqueName == this.MetaTreeNode.UniqueName.Substring(0, this.MetaTreeNode.UniqueName.IndexOf(']') + 1)).FirstOrDefault();
                }
                switch (this.Parent.SplitButtonVisualStyle)
                {
                    case VisualStyle.Blend:
                        calcMemberEditor.VisualStyle = Windows.Shared.VisualStyle.Blend;
                        calcMemberEditor.CalcMeasureTreeView.Style = source["BlendTreeViewStyle"] as Style;
                        break;
                    case VisualStyle.Office2007Black:
                        calcMemberEditor.VisualStyle = Windows.Shared.VisualStyle.Office2007Black;
                        calcMemberEditor.CalcMeasureTreeView.Style = source["Office2007BlackCubeDimensionalBrowserStyle"] as Style;
                        break;
                    case VisualStyle.Office2007Blue:
                        calcMemberEditor.VisualStyle = Windows.Shared.VisualStyle.Office2007Blue;
                        calcMemberEditor.CalcMeasureTreeView.Style = source["Office2007BlueCubeDimensionalBrowserStyle"] as Style;
                        break;
                    case VisualStyle.Office2007Silver:
                        calcMemberEditor.VisualStyle = Windows.Shared.VisualStyle.Office2007Silver;
                        calcMemberEditor.CalcMeasureTreeView.Style = source["Office2007SilverCubeDimensionalBrowserStyle"] as Style;
                        break;
                    case VisualStyle.Metro:
                        calcMemberEditor.VisualStyle = Windows.Shared.VisualStyle.Metro;
                        calcMemberEditor.CalcMeasureTreeView.Style = source["MetroCubeDimensionalBrowserStyle"] as Style;
                        break;
                    case VisualStyle.Default:
                        calcMemberEditor.VisualStyle = Windows.Shared.VisualStyle.Default;
                        calcMemberEditor.CalcMeasureTreeView.Style = source["DefaultCubeDimensionalBrowserStyle"] as Style;
                        break;
                    case VisualStyle.Office2010Black:
                        calcMemberEditor.VisualStyle = Windows.Shared.VisualStyle.Office2010Black;
                        calcMemberEditor.CalcMeasureTreeView.Style = source["Office2010BlackCubeDimensionalBrowserStyle"] as Style;
                        break;
                    case VisualStyle.Office2010Blue:
                        calcMemberEditor.VisualStyle = Windows.Shared.VisualStyle.Office2010Blue;
                        calcMemberEditor.CalcMeasureTreeView.Style = source["Office2010BlueCubeDimensionalBrowserStyle"] as Style;
                        break;
                    case VisualStyle.Office2010Silver:
                        calcMemberEditor.VisualStyle = Windows.Shared.VisualStyle.Office2010Silver;
                        calcMemberEditor.CalcMeasureTreeView.Style = source["Office2010SilverCubeDimensionalBrowserStyle"] as Style;
                        break;
                    case VisualStyle.Transparent:
                        calcMemberEditor.VisualStyle = Windows.Shared.VisualStyle.Transparent;
                        calcMemberEditor.CalcMeasureTreeView.Style = source["TransparentCubeDimensionalBrowserStyle"] as Style;
                        break;
                    default:
                        break;
                }
                calcMemberEditor.Background = olapClient.Background;
                SkinManager.SetVisualStyle(calcMemberEditor, (VisualStyle)Enum.Parse(typeof(VisualStyle), calcMemberEditor.VisualStyle.ToString(), true));
                calcMemberEditor.ShowDialog();
            }
            else if (this.MetaTreeNode.NodeType != MetaTreeNodeType.NamedSet)
            {
                //TODO: Added below code since Active pivot is not yet include the union function support for Amazon cube. so avoid to filter the members in either row or column axis.
                //if (this.OlapDataManager.CurrentCubeName != "Amazon" || this.OlapDataManager.ProviderName != Providers.ActivePivot)
                {
                    source.Source = new Uri("/Syncfusion.OlapClient.Silverlight;component/Tools/SplitButton/SplitButton.xaml", UriKind.RelativeOrAbsolute);

                    ///// Initializing the member editor with the child nodes
                    MemberEditor memberEditor = new MemberEditor(this, this.MetaTreeNode);
                    memberEditor.FlowDirection = this.FlowDirection;
                    memberEditor.OlapDataManager = this.OlapDataManager;
                    memberEditor.AllowMultiMemberSelection = this.AllowMultiMemberSelection;

                    switch (this.Parent.SplitButtonVisualStyle)
                    {
                        case VisualStyle.Blend:
                            memberEditor.VisualStyle = Windows.Shared.VisualStyle.Blend;
                            memberEditor.MemberTree.Style = source["BlendTreeViewStyle"] as Style;
                            break;
                        case VisualStyle.Office2007Black:
                            memberEditor.VisualStyle = Windows.Shared.VisualStyle.Office2007Black;
                            memberEditor.MemberTree.Style = source["Office2007BlackTreeViewStyle"] as Style;
                            break;
                        case VisualStyle.Office2007Blue:
                            memberEditor.VisualStyle = Windows.Shared.VisualStyle.Office2007Blue;
                            memberEditor.MemberTree.Style = source["Office2007BlueTreeViewStyle"] as Style;
                            break;
                        case VisualStyle.Office2007Silver:
                            memberEditor.VisualStyle = Windows.Shared.VisualStyle.Office2007Silver;
                            memberEditor.MemberTree.Style = source["Office2007SilverTreeViewStyle"] as Style;
                            break;
                        case VisualStyle.Metro:
                            memberEditor.VisualStyle = Windows.Shared.VisualStyle.Metro;
                            memberEditor.MemberTree.Style = source["MetroTreeViewStyle"] as Style;
                            break;
                        case VisualStyle.Office2010Black:
                            memberEditor.VisualStyle = Windows.Shared.VisualStyle.Office2010Black;
                            memberEditor.MemberTree.Style = source["Office2010BlackTreeViewStyle"] as Style;
                            break;
                        case VisualStyle.Office2010Blue:
                            memberEditor.VisualStyle = Windows.Shared.VisualStyle.Office2010Blue;
                            memberEditor.MemberTree.Style = source["Office2010BlueTreeViewStyle"] as Style;
                            break;
                        case VisualStyle.Office2010Silver:
                            memberEditor.VisualStyle = Windows.Shared.VisualStyle.Office2010Silver;
                            memberEditor.MemberTree.Style = source["Office2010SilverTreeViewStyle"] as Style;
                            break;
                        case VisualStyle.Transparent:
                            memberEditor.VisualStyle = Windows.Shared.VisualStyle.Transparent;
                            memberEditor.MemberTree.Style = source["TransparentTreeViewStyle"] as Style;
                            break;
                        case VisualStyle.Default:
                            memberEditor.VisualStyle = Windows.Shared.VisualStyle.Default;
                            memberEditor.MemberTree.Style = source["DefaultTreeViewStyle"] as Style;
                            break;
                        default:
                            memberEditor.MemberTree.Style = source["DefaultTreeViewStyle"] as Style;
                            break;

                    }
                    memberEditor.Background = olapClient.Background;
                    SkinManager.SetVisualStyle(memberEditor, (VisualStyle)Enum.Parse(typeof(VisualStyle), memberEditor.VisualStyle.ToString(), true));
                    memberEditor.ShowDialog();
                }
                //else
                //{
                //    Syncfusion.Windows.Tools.Controls.WindowControl.ShowAlert("Filtering is not supported currently for dimensions in Categorical(Column) and Series(Row) axes.", "OlapClient", Syncfusion.Windows.Tools.Controls.DialogIcon.Information, Syncfusion.Windows.Tools.Controls.DialogButton.OK, null, Syncfusion.Windows.Tools.Controls.AnimationType.Zoom);
                //}

                if (this.OlapDataManager.DragDropManager != null)
                {
                    this.OlapDataManager.DragDropManager.DragDropPopup.IsOpen = false;
                    this.OlapDataManager.DragDropManager = null;
                }
            }
        }

        #endregion

        #region Context menu handler

        void InternalButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //ObservableCollection<MetaTreeNode> collection = this.Parent.ItemsSource as ObservableCollection<MetaTreeNode>;
            //this.Menu_Delete.IsHitTestVisible = false;
            //this.Menu_Delete.Opacity = 0.5;

            this.ContextMenu.IsOpen = true;
            ContextMenu.FlowDirection = System.Windows.FlowDirection.RightToLeft;
        }

        void ContextMenu_LostFocus(object sender, RoutedEventArgs e)
        {
            this.ContextMenu.IsOpen = false;
        }

        /// <summary>
        /// Handles the menu button press event of the context menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void Menu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock menu = (sender as TextBlock);
            ObservableCollection<MetaTreeNode> collection = this.Parent.MetaTreeNodes;
            int index = collection.IndexOf(this.MetaTreeNode);
            if (menu.Text.Equals("Move to first"))
            {
                collection.Remove(this.MetaTreeNode);
                collection.Insert(0, this.MetaTreeNode);
                this.Parent.ReAarrangeElemets(null);
            }
            else if (menu.Text.Equals("Move up"))
            {
                collection.Remove(this.MetaTreeNode);
                collection.Insert(index - 1, this.MetaTreeNode);
                this.Parent.ReAarrangeElemets(null);
            }
            else if (menu.Text.Equals("Move down"))
            {
                collection.Remove(this.MetaTreeNode);
                collection.Insert(index + 1, this.MetaTreeNode);
                this.Parent.ReAarrangeElemets(null);
            }
            else if (menu.Text.Equals("Move to last"))
            {
                int count = this.Parent.MetaTreeNodes.Count - 1;
                collection.Remove(this.MetaTreeNode);
                collection.Insert(count, this.MetaTreeNode);
                this.Parent.ReAarrangeElemets(null);
            }
            else if (menu.Text.Equals("Delete"))
            {
                this.Parent.ReAarrangeElemets(this.MetaTreeNode);
            }
            this.ContextMenu.IsOpen = false;
        }

        #endregion

        #region Drag drop handler

        void InternalButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.OlapDataManager.DragDropManager != null)
            {
                this.OlapDataManager.DragDropManager.DragDropPopup.IsOpen = false;
                this.OlapDataManager.DragDropManager = null;
            }
        }

        #endregion

        #endregion
    }
}
