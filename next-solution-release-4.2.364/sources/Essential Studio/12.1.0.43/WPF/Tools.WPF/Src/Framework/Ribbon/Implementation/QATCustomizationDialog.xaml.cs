// <copyright file="QATCustomizationDialog.xaml.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls.Resources;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Pair source item class
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class PairSourceItems
    {
        #region Private members
        /// <summary>
        /// Represents the items source
        /// </summary>
        private ItemsControl m_source;

        /// <summary>
        /// Represents the Ribbon Items
        /// </summary>
        private List<IRibbonControl> m_items;
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public ItemsControl Source
        {
            get
            {
                return m_source;
            }

            set
            {
                m_source = value;
            }
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public List<IRibbonControl> Items
        {
            get
            {
                return m_items;
            }

            set
            {
                m_items = value;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="PairSourceItems"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public PairSourceItems(ItemsControl source)
        {
            m_source = source;
        }
        #endregion
    }

    /// <summary>
    /// Represents a QATCustomizationDialog window.
    /// </summary>
    /// <list type="table">
    /// 	<listheader>
    /// 		<term>Help Page</term>
    /// 		<description>Syntax</description>
    /// 	</listheader>
    /// 	<example>
    /// 		<list type="table">
    /// 			<listheader>
    /// 				<description>C#</description>
    /// 			</listheader>
    /// 			<example><code>public partial class QATCustomizationDialog : Window</code></example>
    /// 		</list>
    /// 		<para/>
    /// 		<list type="table">
    /// 			<listheader>
    /// 				<description>XAML Object Element Usage</description>
    /// 			</listheader>
    /// 			<example><code><![CDATA[<ribbon:QATCustomizationDialog Name="dialog" />]]></code></example>
    /// 		</list>
    /// 	</example>
    /// </list>
    /// <remarks>
    /// QATCustomizationDialog class represents a window that is displayed over the Ribbon for customization of QuickAccess toolbar items.
    /// </remarks>
    /// <example>
    /// 	<para/>This example shows how to create a ButtonPanel in C#.
    /// <code>
    /// QATCustomizationDialog dialog = new QATCustomizationDialog(IsQATBelow, SkinStorage.GetVisualStyle(this), m_blendColor, this);
    /// dialog.ShowDialog();
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public partial class QATCustomizationDialog : Window, IDisposable,INotifyPropertyChanged
    {
        #region Constants
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Protection property key.
        /// </summary>
        private const string CustomStylePrefix = "InternalCustom";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Default skin background brush name. 
        /// </summary>
        private const string DefaultBrushName = "BackgroundBrush.Silver";

        /// <summary>
        /// Blend skin background brush name
        /// </summary>
        private const string BlendBrushName = "BackgroundBrush.Blend";

        /// <summary>
        /// refers ResourceDictionary instance
        /// </summary>
        //ResourceDictionary lang = new ResourceDictionary();
    
       
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Group selected by default. 
        /// </summary>
        private string CommandGroupSelectedByDefault;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Ribbon menu group name.
        /// </summary>
        private string RibbonMenuGroup;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// String group separator.
        /// </summary>
        private const string GroupSeparator = "----------";

        /// <summary>
        /// Store the custom color.
        /// </summary>
        public static Color skinColor;

        /// <summary>
        /// Store the base style of the skin.
        /// </summary>
        public static bool baseStyle;

        /// <summary>
        /// Store the Base Skin
        /// </summary>
        public static string currentSkin = "Default";

        #endregion

        #region Private members
		/// <summary>
        /// Represent object reference of Resource wrapper class.
        /// </summary>
        ResourceWrapper wrapper = new ResourceWrapper();

        internal ObservableCollection<QuickAccessToolBarItem> referenceitems = new ObservableCollection<QuickAccessToolBarItem>();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Ribbon instance.
        /// </summary>
        private Ribbon m_ribbon;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Command collection.
        /// </summary>
        private InternalCommandManager m_commandManager;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Application menu instance.
        /// </summary>
        private ApplicationMenu m_appMenu;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Group items container. 
        /// </summary>
        private Dictionary<string, PairSourceItems> m_groupDictionary;
        #endregion

        #region Properties

        /// <summary>
        /// Gets the visual style.
        /// </summary>
        /// <value>The visual style.</value>
        protected internal string VisualStyle
        {
            get
            {
                return (string)GetValue(VisualStyleProperty);
            }

            internal set
            {
                SetValue(VisualStylePropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the custom color scheme brush.
        /// </summary>
        protected Brush CustomBrush
        {
            get
            {
                return (Brush)GetValue(CustomBrushProperty);
            }

            set
            {
                SetValue(CustomBrushPropertyKey, value);
            }
        }
        #endregion

        #region DP Properties
        /// <summary>
        /// Protection property key.
        /// </summary>
        protected static readonly DependencyPropertyKey VisualStylePropertyKey =
            DependencyProperty.RegisterReadOnly("VisualStyle", typeof(string), typeof(QATCustomizationDialog), new PropertyMetadata(string.Empty, OnVisualStylePropertyChanged));

        private static void OnVisualStylePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((QATCustomizationDialog)sender).VisualStyleChanged();
        }

        private void VisualStyleChanged()
        {
            //switch (this.VisualStyle)
            //{
            //    case "Office2007Black":
            //        this.PART_ListSource.ItemContainerStyle = this.Resources["BlackListBoxItemStyle"] as Style;
            //        this.PART_ListDestination.ItemContainerStyle = this.Resources["BlackListBoxItemStyle"] as Style;
            //        break;
            //    case "Office2007Silver":
            //        this.PART_ListSource.ItemContainerStyle = this.Resources["SilverListBoxItemStyle"] as Style;
            //        this.PART_ListDestination.ItemContainerStyle = this.Resources["SilverListBoxItemStyle"] as Style;
            //        break;
            //    case "Office2003":
            //        this.PART_ListSource.ItemContainerStyle = this.Resources["Office2003ListBoxItemStyle"] as Style;
            //        this.PART_ListDestination.ItemContainerStyle = this.Resources["Office2003ListBoxItemStyle"] as Style;
            //        break;
            //    case "ShinyRed":
            //        this.PART_ListSource.ItemContainerStyle = this.Resources["ShinyRedListBoxItemStyle"] as Style;
            //        this.PART_ListDestination.ItemContainerStyle = this.Resources["ShinyRedListBoxItemStyle"] as Style;
            //        break;
            //    case "ShinyBlue":
            //        this.PART_ListSource.ItemContainerStyle = this.Resources["ShinyBlueListBoxItemStyle"] as Style;
            //        this.PART_ListDestination.ItemContainerStyle = this.Resources["ShinyBlueListBoxItemStyle"] as Style;
            //        break;
            //    case "Blend":
            //        this.PART_ListDestination.ItemContainerStyle = this.Resources["BlendListBoxItemStyle"] as Style;
            //        this.PART_ListSource.ItemContainerStyle = this.Resources["BlendListBoxItemStyle"] as Style;
            //        break;
            //    case "SyncOrange":
            //        this.PART_ListDestination.ItemContainerStyle = this.Resources["SyncOrangeListBoxItemStyle"] as Style;
            //        this.PART_ListSource.ItemContainerStyle = this.Resources["SyncOrangeListBoxItemStyle"] as Style;
            //        break;
            //    case "Office2010Black":
            //        this.PART_ListDestination.ItemContainerStyle = this.Resources["Office2010ListBoxItemStyle"] as Style;
            //        this.PART_ListSource.ItemContainerStyle = this.Resources["Office2010ListBoxItemStyle"] as Style;
            //        this.PART_ComboBox.ItemContainerStyle = this.Resources["Office2010ComboBoxItemStyle"] as Style;
            //        this.PART_btnAdd.Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        this.PART_btnRemove .Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        this.PART_btnUp.Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        this.PART_btnDown.Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        this.PART_btnOk.Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        this.PART_btnReset.Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        this.PART_btnCancel.Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        break;

            //    case "Office2010Blue":
            //        this.PART_ListDestination.ItemContainerStyle = this.Resources["Office2010ListBoxItemStyle"] as Style;
            //        this.PART_ListSource.ItemContainerStyle = this.Resources["Office2010ListBoxItemStyle"] as Style;
            //        this.PART_ComboBox.ItemContainerStyle = this.Resources["Office2010ComboBoxItemStyle"] as Style;
            //        this.PART_btnAdd.Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        this.PART_btnRemove .Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        this.PART_btnUp.Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        this.PART_btnDown.Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        this.PART_btnOk.Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        this.PART_btnReset.Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        this.PART_btnCancel.Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        break;
            //    case "Office2010Silver":
            //        this.PART_ListDestination.ItemContainerStyle = this.Resources["Office2010ListBoxItemStyle"] as Style;
            //        this.PART_ListSource.ItemContainerStyle = this.Resources["Office2010ListBoxItemStyle"] as Style;
            //        this.PART_ComboBox.ItemContainerStyle = this.Resources["Office2010ComboBoxItemStyle"] as Style;
            //        this.PART_btnAdd.Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        this.PART_btnRemove .Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        this.PART_btnUp.Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        this.PART_btnDown.Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        this.PART_btnOk.Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        this.PART_btnReset.Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        this.PART_btnCancel.Style = this.Resources["Office2010ButtonStyle"] as Style;
            //        break;

            //    case "Default":
            //    case "Office2007Blue":
            //    default:
            //        this.PART_ListDestination.ItemContainerStyle = this.Resources["BlueListBoxItemStyle"] as Style;
            //        this.PART_ListSource.ItemContainerStyle = this.Resources["BlueListBoxItemStyle"] as Style;
            //        break;
            //}
        }

        /// <summary>
        /// Color scheme name.
        /// </summary>
        public static readonly DependencyProperty VisualStyleProperty = VisualStylePropertyKey.DependencyProperty;

        /// <summary>
        /// Background brush. 
        /// </summary>
        protected static readonly DependencyPropertyKey CustomBrushPropertyKey =
            DependencyProperty.RegisterReadOnly("CustomBrush", typeof(Brush), typeof(QATCustomizationDialog), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Customize dialog skin brush.
        /// </summary>
        public static readonly DependencyProperty CustomBrushProperty = CustomBrushPropertyKey.DependencyProperty;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="QATCustomizationDialog"/> class.
        /// </summary>
        /// <param name="isQATBelow">if set to <c>true</c> [is QAT below].</param>
        /// <param name="visualStyle">The visual style.</param>
        /// <param name="blendColor">Color of the blend.</param>
        /// <param name="ribbon">The ribbon.</param>
        internal QATCustomizationDialog(bool isQATBelow, string visualStyle, Color blendColor, Ribbon ribbon)
        {
            InitializeComponent();
            CommandGroupSelectedByDefault = wrapper.QATAllCommandsCaption;
            RibbonMenuGroup = wrapper.QATRibbonMenuCaption;
            this.Closed -= new EventHandler(QATCustomizationDialog_Closed);
            this.Closed += new EventHandler(QATCustomizationDialog_Closed);
            this.Unloaded += new RoutedEventHandler(QATCustomizationDialog_Unloaded);
            m_commandManager = ribbon.QuickAccessToolBar.InternalCommandManager;
            m_ribbon = ribbon;
            skinColor = blendColor;
            m_appMenu = ribbon.ApplicationMenu;
            m_groupDictionary = new Dictionary<string, PairSourceItems>();
            AccessKeyManager.RemoveAccessKeyPressedHandler(PART_lblChoose, new AccessKeyPressedEventHandler(OnAccessKeyPressed));
            AccessKeyManager.AddAccessKeyPressedHandler(PART_lblChoose, new AccessKeyPressedEventHandler(OnAccessKeyPressed));
            RibbonCommandManager.GroupDictionary[CommandGroupSelectedByDefault] = RibbonCommandManager.CommandDictionary;
            CustomBrush = (Brush)Resources[DefaultBrushName];
            if (visualStyle.Substring(visualStyle.Length - 5, 5) == "Blend")
            {
                baseStyle = true;
            }
            else
            {
                baseStyle = false;
            }

            if (baseStyle)
            {
                CustomBrush = (Brush)Resources[BlendBrushName];
            }

            Initialize(isQATBelow, visualStyle, blendColor);
            if (VisualStyle == CustomStylePrefix)
            {
                string skin = visualStyle.Replace("InternalCustom_", string.Empty);
                currentSkin = skin;
            }

            CreateGroups();
            m_ribbon.isQATDialogOpened = true;

            PART_ListDestination.ItemsSource = m_commandManager.Items;
            foreach (QuickAccessToolBarItem item in m_commandManager.Items)
                referenceitems.Add(item);
            PART_ComboBox.SelectedIndex = PART_ComboBox.Items.IndexOf(CommandGroupSelectedByDefault);
            if (PART_ComboBox.SelectedItem.ToString().Equals (wrapper.QATAllCommandsCaption))
                AddAllCommands();
            else
                PART_ListSource.ItemsSource = RibbonCommandManager.GroupDictionary[PART_ComboBox.SelectedItem as string].Values;
            if (PART_btnOk != null)
                PART_btnOk.Click -= new RoutedEventHandler(BtnOk_Click);
            if (PART_btnCancel != null)
                PART_btnCancel.Click -= new RoutedEventHandler(BtnCancel_Click);
            if (PART_btnAdd != null)
                PART_btnAdd.Click -= new RoutedEventHandler(BtnAdd_Click);
            if (PART_btnRemove != null)
                PART_btnRemove.Click -= new RoutedEventHandler(BtnRemove_Click);
            if (PART_btnUp != null)
                PART_btnUp.Click -= new RoutedEventHandler(BtnUp_Click);
            if (PART_btnDown != null)
                PART_btnDown.Click -= new RoutedEventHandler(BtnDown_Click);
            if (PART_btnReset != null)
                PART_btnReset.Click -= new RoutedEventHandler(PART_btnReset_Click);
            if (PART_ListSource != null)
                PART_ListSource.MouseDoubleClick -= new MouseButtonEventHandler(PART_ListSource_MouseDoubleClick);
            if (PART_ComboBox != null)
                PART_ComboBox.SelectionChanged -= new SelectionChangedEventHandler(PART_ComboBox_SelectionChanged);
            if (PART_ListDestination != null)
            {
                PART_ListDestination.MouseDoubleClick -= new MouseButtonEventHandler(PART_ListDestination_MouseDoubleClick);
                PART_ListDestination.SelectionChanged -= new SelectionChangedEventHandler(PART_ListDestination_SelectionChanged);
            }
            PART_btnOk.Click += new RoutedEventHandler(BtnOk_Click);
            PART_btnCancel.Click += new RoutedEventHandler(BtnCancel_Click);
            PART_btnAdd.Click += new RoutedEventHandler(BtnAdd_Click);
            PART_btnRemove.Click += new RoutedEventHandler(BtnRemove_Click);
            PART_btnUp.Click += new RoutedEventHandler(BtnUp_Click);
            PART_btnDown.Click += new RoutedEventHandler(BtnDown_Click);
            PART_btnReset.Click += new RoutedEventHandler(PART_btnReset_Click);
            PART_ListSource.MouseDoubleClick += new MouseButtonEventHandler(PART_ListSource_MouseDoubleClick);
            PART_ListDestination.MouseDoubleClick += new MouseButtonEventHandler(PART_ListDestination_MouseDoubleClick);
            PART_ComboBox.SelectionChanged += new SelectionChangedEventHandler(PART_ComboBox_SelectionChanged);
            PART_ListDestination.SelectionChanged += new SelectionChangedEventHandler(PART_ListDestination_SelectionChanged);
            this.Loaded -= new RoutedEventHandler(QATCustomizationDialog_Loaded);
            this.Loaded += new RoutedEventHandler(QATCustomizationDialog_Loaded);

            #region Customize Ribbon

            if (this.m_ribbon != null && this.m_ribbon.ShowCustomizeRibbon)
            {

                PART_CustomizeComboBox.SelectedIndex = PART_CustomizeComboBox.Items.IndexOf(CommandGroupSelectedByDefault);
                PART_CustomizeComboBox.SelectionChanged -= new SelectionChangedEventHandler(PART_CustomizeComboBox_SelectionChanged);
                PART_CustomizeComboBox.SelectionChanged += new SelectionChangedEventHandler(PART_CustomizeComboBox_SelectionChanged);

                (PART_MainListBox.Items[0] as ListBoxItem).Visibility = Visibility.Visible;
                AddAllRibbonTabs();
                PART_btnRibbonTabNew.Click += new RoutedEventHandler(PART_btnRibbonTabNew_Click);
                PART_btnRibbonBarNew.Click += new RoutedEventHandler(PART_btnRibbonBarNew_Click);
                PART_btnRename.Click += new RoutedEventHandler(PART_btnRename_Click);
                PART_btnRibbonTabAdd.Click += new RoutedEventHandler(PART_btnRibbonTabAdd_Click);
                PART_btnTabDown.Click += new RoutedEventHandler(PART_btnTabDown_Click);
                PART_btnTabUp.Click += new RoutedEventHandler(PART_btnTabUp_Click);
                PART_btnRibbonTabRemove.Click += new RoutedEventHandler(PART_btnRibbonTabRemove_Click);
                ribbonTreeView.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(ribbonTreeView_SelectedItemChanged);

                btn_DropDown.IsDropDownOpenChanged += new PropertyChangedCallback(btn_DropDown_IsDropDownOpenChanged);
               (btn_DropDown.Items[1] as RibbonMenuItem).Click += new RoutedEventHandler(Btn_ResetCustomization_Click);
               (btn_DropDown.Items[0] as RibbonMenuItem).Click += new RoutedEventHandler(Btn_ResetSelectedTabCustomization_Click);
                Btn_DeleteCustomization.Click += new RoutedEventHandler(Btn_DeleteCustomization_Click);

                PART_ListRibbonTabSource.SelectionChanged += new SelectionChangedEventHandler(PART_ListRibbonTabSource_SelectionChanged);
            }
            #endregion
        }

        void PART_ListRibbonTabSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PART_ListRibbonTabSource.SelectedItem is IRibbonControl)
            {
                if (PART_ListRibbonTabSource.SelectedItem is QuickAccessToolBarItem)
                {
                    IRibbonControl control = (PART_ListRibbonTabSource.SelectedItem as QuickAccessToolBarItem).ClonedElement as IRibbonControl;
                    if (control is RibbonBar)
                    {
                        PART_btnRibbonTabAdd.IsEnabled = false;
                    }
                    else
                        PART_btnRibbonTabAdd.IsEnabled = true;
                }
            }
        }

        void Btn_DeleteCustomization_Click(object sender, RoutedEventArgs e)
        {
           string message = "Are you sure want to delete Ribbon tabs/bars customizations?";

            var msgResult = MessageBox.Show(message, "Delete Ribbon tabs/bars Customization", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (msgResult == MessageBoxResult.No)
            {
                return;
            }
            else
            {
                if (ribbonTreeView.SelectedItem is RibbonTabs)
                {
                    bool isCustomTab = (ribbonTreeView.SelectedItem as RibbonTabs).IsCustomTab;
                    if (isCustomTab)
                    {
                        if (AddedRibbonTabs.Contains(ribbonTreeView.SelectedItem as RibbonTabs))
                        {
                            AddedRibbonTabs.Remove(ribbonTreeView.SelectedItem as RibbonTabs);
                        }
                        if (RibbonTabs.Contains(ribbonTreeView.SelectedItem as RibbonTabs))
                        {
                            int index = RibbonTabs.IndexOf(ribbonTreeView.SelectedItem as RibbonTabs);
                            if (m_ribbon.Items.Count > index)
                                m_ribbon.Items.RemoveAt(index);
                            if (index >= 0 && RibbonTabs.Count > index && RibbonTabs[index].IsChecked)
                            {
                                RibbonTabs[index].IsChecked = false;
                                if (RibbonTabs.Count > 0)
                                {
                                    RibbonTabs[0].IsChecked = true;
                                    m_ribbon.SelectedIndex = 0;
                                }
                                else
                                    m_ribbon.SelectedIndex = -1;
                            }
                            RibbonTabs.Remove(ribbonTreeView.SelectedItem as RibbonTabs);
                            ResetTabIndex(index);
                        }
                    }
                }
                else if (ribbonTreeView.SelectedItem is RibbonBars)
                {
                    bool isCustomBar = (ribbonTreeView.SelectedItem as RibbonBars).IsCustomBar;
                    if (isCustomBar)
                    {
                        if (InternalRibbonBarCollection.Contains(ribbonTreeView.SelectedItem as InternalRibbonBar))
                        {
                            InternalRibbonBarCollection.Remove(ribbonTreeView.SelectedItem as InternalRibbonBar);
                        }

                        foreach (var ribbonTabs in AddedRibbonTabs)
                        {
                            if (ribbonTabs.RibbonBars.Contains(ribbonTreeView.SelectedItem as RibbonBars))
                            {
                                ribbonTabs.RibbonBars.Remove(ribbonTreeView.SelectedItem as RibbonBars);
                                break;
                            }
                        }

                        foreach (var ribbonTab in RibbonTabs)
                        {
                            if (ribbonTab.RibbonBars.Contains(ribbonTreeView.SelectedItem as RibbonBars))
                            {
                                if (InternalRibbonBarCollection.Count > 0)
                                {
                                    int collectionindex = -1;
                                    foreach (InternalRibbonBar item in InternalRibbonBarCollection)
                                    {
                                        if (item.NewBar == (ribbonTreeView.SelectedItem as RibbonBars))
                                        {
                                            collectionindex = InternalRibbonBarCollection.IndexOf(item);
                                            break;
                                        }
                                    }
                                    if (collectionindex >= 0)
                                        InternalRibbonBarCollection.RemoveAt(collectionindex);
                                }

                                int index = ribbonTab.RibbonBars.IndexOf(ribbonTreeView.SelectedItem as RibbonBars);
                                ribbonTab.RibbonBars.Remove(ribbonTreeView.SelectedItem as RibbonBars);
                                if (index >= 0 && index < (m_ribbon.Items[ribbonTab.TabIndex] as RibbonTab).Items.Count)
                                    (m_ribbon.Items[ribbonTab.TabIndex] as RibbonTab).Items.RemoveAt(index);
                                break;
                            }
                        }


                    }
                }
            }
        }

        private void ResetTabIndex(int index)
        {
            foreach (var ribbonTab in AddedRibbonTabs)
            {
                if (ribbonTab.TabIndex > index)
                    ribbonTab.TabIndex -= 1;
            }

            foreach (var ribbonTab in RibbonTabs)
            {
                if (ribbonTab.TabIndex > index)
                    ribbonTab.TabIndex -= 1;
            }
        }

        //To Reset all the customizations
        void Btn_ResetCustomization_Click(object sender, RoutedEventArgs e)
        {
            string message = "Are you sure want to delete all Ribbon customizations?";

            var msgResult = MessageBox.Show(message, "Reset Ribbon Customization", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (msgResult == MessageBoxResult.Yes)
            {
                if (this.m_ribbon != null)
                {
                    this.m_ribbon.DeleteRibbonState();
                    AddedRibbonTabs.Clear();
                    InternalRibbonBarCollection.Clear();

                    ResetItems();
                }
            }
        }

        //To reset customization in selectedtab
        void Btn_ResetSelectedTabCustomization_Click(object sender,RoutedEventArgs e)
        {
            ResetSelectedTabHeader();
        }

        private void ResetSelectedTabHeader()
        {
            if (ribbonTreeView.SelectedItem is RibbonTabs)
            {
                var selectedTabs = (ribbonTreeView.SelectedItem as RibbonTabs);
                int tabIndex = selectedTabs.TabIndex;
                RibbonTab ribbonTab = m_ribbon.Items[tabIndex] as RibbonTab;

                string originalHeader = QATCustomizationDialog.GetTabOriginalHeader(ribbonTab);
                ribbonTab.Caption = originalHeader;
                selectedTabs.Caption = originalHeader;
                ResetSeletedTabGroups(ribbonTab, selectedTabs);
            }

        }

        private void ResetSeletedTabGroups(RibbonTab ribbonTab,RibbonTabs selectedTab)
        {
            //Reset barheader in RibbonBar
            foreach (RibbonBar ribbonBar in ribbonTab.Items)
            {
                string originalHeader = QATCustomizationDialog.GetBarOriginalHeader(ribbonBar);
                ribbonBar.Header = originalHeader;
            }

            Dictionary<int, int> ResetBarColletion = new Dictionary<int, int>(); 
            int barCount = 0;
            List<int> ResetBarList = new List<int>();

            //Reset barheader in RibbonBar collection
            foreach (var item in selectedTab.RibbonBars)
            {
                if (item.IsCustomBar)
                    ResetBarList.Add(item.BarIndex);

                RibbonBar bar = ribbonTab.Items.Count > item.BarIndex ? ribbonTab.Items[item.BarIndex] as RibbonBar : null;
                if (bar != null)
                {
                    if (item.IsCustomBar)
                    {
                        ribbonTab.Items.Remove(bar);                       
                    }
                    else
                    {
                        string originalHeader = QATCustomizationDialog.GetBarOriginalHeader(bar);
                        int originalIndex = QATCustomizationDialog.GetBarOriginalIndex(bar);
                        item.Header = originalHeader;
                        item.IsPositionChanged = false;
                        if (!ResetBarColletion.ContainsValue(originalIndex) && !ResetBarColletion.ContainsKey(barCount))
                            ResetBarColletion.Add(barCount, originalIndex);
                        barCount++;
                    }
                }
            }

            int tempCount = 0;
            ObservableCollection<RibbonBars> tempBarCollection = new ObservableCollection<RibbonBars>(selectedTab.RibbonBars);
            foreach (var item in ResetBarColletion)
            {
                RibbonBars ribbonBars = tempBarCollection[tempCount];
                selectedTab.RibbonBars.Remove(ribbonBars);
                selectedTab.RibbonBars.Insert(item.Value, ribbonBars);
                tempCount++;
            }           

            foreach (var item in ResetBarList)
            {
                RibbonBars bars = tempBarCollection[item];
                selectedTab.RibbonBars.Remove(bars);
                InternalRibbonBarCollection.Clear();
            }
           
        }

        ObservableCollection<RibbonTabs> ResetTabCollection = new ObservableCollection<RibbonTabs>();

        private void ResetItems()
        {
            foreach (var item in RibbonTabs)
            {
                bool isCustom = (item as RibbonTabs).IsCustomTab;
                if (isCustom)
                {
                    ResetTabCollection.Add(item as RibbonTabs);
                    if (item.IsChecked)
                    {
                        item.IsChecked = false;
                        if (RibbonTabs.Count > 0 && !RibbonTabs[0].IsCustomTab)
                        {
                            RibbonTabs[0].IsChecked = true;
                            m_ribbon.SelectedIndex = 0;
                        }
                        else
                            m_ribbon.SelectedIndex = -1;
                    }
                }
                else
                {
                    int tabindex=-1;
                    foreach (var tabitem in m_ribbon.Items)
                    {
                        if ((tabitem as RibbonTab).Caption == item.Caption)
                        {
                            tabindex = m_ribbon.Items.IndexOf(tabitem);
                            break;
                        }

                    }
                    if (tabindex >= 0)
                    {
                        string originalHeader = QATCustomizationDialog.GetTabOriginalHeader(m_ribbon.Items[tabindex] as RibbonTab);
                        item.Caption = originalHeader;
                    }
                    int barindex = -1;
                    foreach (var baritem in item.RibbonBars)
                    {
                        if (tabindex >= 0 && !baritem.IsCustomBar)
                        {
                            foreach (var bar in (m_ribbon.Items[tabindex] as RibbonTab).Items)
                            {
                                if ((bar as RibbonBar).Header == baritem.Header)
                                {
                                    barindex = (m_ribbon.Items[tabindex] as RibbonTab).Items.IndexOf(bar);
                                    break;
                                }

                            }
                            if (barindex >= 0)
                            {
                                string originalbarHeader = QATCustomizationDialog.GetBarOriginalHeader(((m_ribbon.Items[tabindex] as RibbonTab).Items[barindex] as RibbonBar));
                                baritem.Header = originalbarHeader;
                            }
                        }
                    }
                    
                }
            }

            foreach (var ribbonTab in ResetTabCollection)
            {
                int index = RibbonTabs.IndexOf(ribbonTab);
                if (m_ribbon.Items.Count > index && index >= 0)
                    m_ribbon.Items.RemoveAt(index);
                RibbonTabs.Remove(ribbonTab);
            }

            foreach (var tab in RibbonTabs)
            {
                foreach (var bar in tab.RibbonBars)
                {
                    if (bar.IsCustomBar)
                        ResetBarsCollection.Add(RibbonTabs.IndexOf(tab), tab.RibbonBars.IndexOf(bar));
                }
            }
            foreach (var bar in ResetBarsCollection)
            {
                RibbonTabs[bar.Key].RibbonBars.RemoveAt(bar.Value);
                //(m_ribbon.Items[bar.Key] as RibbonTab).Items.RemoveAt(bar.Value);
            }
            for (int i = 0; i < RibbonTabs.Count; i++)
            {
                for (int j = 0; j < RibbonTabs[i].RibbonBars.Count;j++ )
                {
                    RibbonBars baritem = RibbonTabs[i].RibbonBars[j];
                    int originalbarindex = QATCustomizationDialog.GetBarOriginalIndex((m_ribbon.Items[i] as RibbonTab).Items[j] as RibbonBar);
                    baritem.BarIndex = originalbarindex;
                    if (originalbarindex != j && originalbarindex != -1 && j != -1)
                    {
                        RibbonBar bar = ((m_ribbon.Items[i] as RibbonTab).Items[j]) as RibbonBar;
                        (m_ribbon.Items[i] as RibbonTab).Items.Remove(bar);
                        (m_ribbon.Items[i] as RibbonTab).Items.Insert(originalbarindex, bar);
                        j = 0;
                    }
                }
            }
            for (int i = 0; i < RibbonTabs.Count;i++ )
            {
                RibbonTabs tabitem = RibbonTabs[i];
                int originalindex = QATCustomizationDialog.GetTabOriginalIndex(m_ribbon.Items[i] as RibbonTab);
                tabitem.TabIndex = originalindex;
                if (originalindex != i && originalindex != -1)
                {
                    RibbonTab tab = m_ribbon.Items[i] as RibbonTab;
                    m_ribbon.Items.Remove(tab);
                    m_ribbon.Items.Insert(originalindex, tab);
                    i = 0;
                }
            }
            
            for (int i = 0; i < m_ribbon.Items.Count;i++ )
            {
                RibbonTab tabsitem = m_ribbon.Items[i] as RibbonTab;
                int index = -1;
                for (int j = 0; j < RibbonTabs.Count; j++)
                {
                    if (tabsitem is RibbonTab && (tabsitem as RibbonTab).Caption == RibbonTabs[j].Caption)
                    {
                        index = j;
                        break;
                    }
                }
                if (i != index && index != -1 && i != -1)
                {
                    RibbonTabs tab = RibbonTabs[index];
                    RibbonTabs.Remove(tab);
                    RibbonTabs.Insert(i, tab);
                    i = 0;
                }                
            }
            for (int i = 0; i < m_ribbon.Items.Count; i++)
            {
                RibbonTab tabsitem = m_ribbon.Items[i] as RibbonTab;
                for (int k = 0; k < (tabsitem as RibbonTab).Items.Count; k++)
                {
                    RibbonBar baritem = (tabsitem as RibbonTab).Items[k] as RibbonBar;
                    int barindex = -1;
                    foreach (RibbonBars bar in RibbonTabs[i].RibbonBars)
                    {
                        if ((baritem as RibbonBar).Header == bar.Header)
                        {
                            barindex = RibbonTabs[i].RibbonBars.IndexOf(bar);
                            break;
                        }
                    }
                    if (barindex != k && barindex != -1)
                    {
                        RibbonBars bar = RibbonTabs[i].RibbonBars[barindex];
                        RibbonTabs[i].RibbonBars.Remove(bar);
                        RibbonTabs[i].RibbonBars.Insert(k, bar);
                        k = 0;
                    }
                }
            }            
            for (int i = 0; i < RibbonTabs.Count;i++ )
            {
                RibbonTabs tab = RibbonTabs[i];
                tab.TabIndex = i;
                int index = -1;
                for (int j = 0; j < ribbonTreeView.Items.Count; j++)
                {
                    if ((tab as RibbonTabs).Caption == (ribbonTreeView.Items[j] as RibbonTabs).Caption)
                    {
                        index = j;
                        break;
                    }
                }
                if (i != index && index != -1)
                {
                    RibbonTabs item = ribbonTreeView.Items[index] as RibbonTabs;
                    ribbonTreeView.Items.Remove(item);
                    ribbonTreeView.Items.Insert(i, item);
                    i = 0;
                }
            }
            for (int i = 0; i < RibbonTabs.Count; i++)
            {
                RibbonTabs tab = RibbonTabs[i];
                for (int j = 0; j < tab.RibbonBars.Count; j++)
                {
                    RibbonBars bar = tab.RibbonBars[j];
                    bar.BarIndex = j;
                    int barindex = -1;
                    if (i < ribbonTreeView.Items.Count)
                    {
                        foreach (var bars in (ribbonTreeView.Items[i] as RibbonTabs).RibbonBars)
                        {
                            if (bar.Header == bars.Header)
                            {
                                barindex = (ribbonTreeView.Items[i] as RibbonTabs).RibbonBars.IndexOf(bars);
                                break;
                            }
                        }
                    }
                    if (j != barindex && barindex != -1)
                    {
                        RibbonBars bars = (ribbonTreeView.Items[i] as RibbonTabs).RibbonBars[barindex] as RibbonBars;
                        (ribbonTreeView.Items[i] as RibbonTabs).RibbonBars.Remove(bars);
                        (ribbonTreeView.Items[i] as RibbonTabs).RibbonBars.Insert(j, bars);
                        j = 0;
                    }
                }
            }
        }
        Dictionary<int,int> ResetBarsCollection = new Dictionary<int,int>();      

      

        void QATCustomizationDialog_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        void QATCustomizationDialog_Closed(object sender, EventArgs e)
        {
            if (DialogResult == false)
            {
                RemoveOnCancel(true);                
                Close();
            }
        }
       
        /// <summary>
        /// Handles the Loaded event of the QATCustomizationDialog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void QATCustomizationDialog_Loaded(object sender, RoutedEventArgs e)
        {

            this.Title = wrapper.CustomizeQATHeader;
            SkinStorage.SetVisualStyle(this, "Default");
            PART_btnOk.Focus();
            //if (VisualStyle == CustomStylePrefix)
            //{
            //    SkinStorage.SetVisualStyle(this, currentSkin);
            //    SolidColorBrush customcolor = new SolidColorBrush(skinColor);
            //    SkinManager.SetActiveColorScheme(this, customcolor);
            //    SkinManager.SetActiveColorScheme(grid, customcolor);
            //}
        }
        
        /// <summary>
        /// Initializes customize dialog.
        /// </summary>
        /// <param name="isQATBelow">Defines whether QAT is below the
        /// ribbon or above. </param>
        /// <param name="visualStyle">Current color scheme name.</param>
        /// <param name="blendColor">Custom color scheme main color.</param>
        private void Initialize(bool isQATBelow, string visualStyle, Color blendColor)
        {
            if (!String.IsNullOrEmpty(visualStyle))
            {
                if (visualStyle.Contains(CustomStylePrefix))
                {
                    VisualStyle = CustomStylePrefix;
                    if (CustomBrush != null)
                    {
                        ApplyColorSheme(blendColor);
                    }
                    else
                    {
                        throw new NullReferenceException("Default brush could not be found");
                    }
                }
                else
                {
                    //VisualStyle = visualStyle;
                    //SkinStorage.SetVisualStyle(this, visualStyle);
                }
            }

            if (isQATBelow)
            {
                PART_chkShowQATBelow.IsChecked = true;
            }
            else
            {
                PART_chkShowQATBelow.IsChecked = false;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Group forming.
        /// </summary>
        private void CreateGroups()
        {
            foreach (string key in RibbonCommandManager.GroupDictionary.Keys)
            {
                PART_ComboBox.Items.Add(key);
                PART_CustomizeComboBox.Items.Add(key);
            }

            if (m_appMenu != null)
            {
                m_groupDictionary.Add(RibbonMenuGroup, new PairSourceItems(m_appMenu));
                PART_ComboBox.Items.Add(GroupSeparator);
                PART_ComboBox.Items.Add(RibbonMenuGroup);

                PART_CustomizeComboBox.Items.Add(GroupSeparator);
                PART_CustomizeComboBox.Items.Add(RibbonMenuGroup);
            }

            PART_ComboBox.Items.Add(GroupSeparator);

            PART_CustomizeComboBox.Items.Add(GroupSeparator);

            if (m_ribbon.BackStage != null)
            {
                string key = m_ribbon.BackStageHeader + " " + wrapper.QATTabCaption;
                m_groupDictionary.Add(key, new PairSourceItems(m_ribbon.BackStage));
                PART_ComboBox.Items.Add(key);

                PART_CustomizeComboBox.Items.Add(key);
            }

            PART_ComboBox.Items.Add(GroupSeparator);

            PART_CustomizeComboBox.Items.Add(GroupSeparator);

            foreach (object item in m_ribbon.Items)
            {
                RibbonTab tab = null;
                if (item is RibbonTab)
                {
                    tab = (RibbonTab)item;
                }
                else
                {
                    tab = m_ribbon.ItemContainerGenerator.ContainerFromItem(item) as RibbonTab;
                }
                if (tab != null && tab.Visibility==Visibility.Visible && tab.ContextTabGroup == null)
                {
                    string key = tab.Caption + " " + wrapper.QATTabCaption;
                    int num = GetIdenticalItemsCount(key);
                    if (num > 0)
                    {
                        key = String.Concat(key, "_" + num.ToString());
                    }

                    m_groupDictionary.Add(key, new PairSourceItems(tab));
                    PART_ComboBox.Items.Add(key);

                    PART_CustomizeComboBox.Items.Add(key);


                }
            }

            PART_ComboBox.Items.Add(GroupSeparator);

            PART_CustomizeComboBox.Items.Add(GroupSeparator);
            foreach (ContextTabGroup contextGroup in m_ribbon.ContextTabGroups)
            {
                foreach (RibbonTab tab in contextGroup.RibbonTabs)
                {
                    string key = contextGroup.Label + " | " + tab.Caption + " " + wrapper.QATTabCaption;
                    int num = GetIdenticalItemsCount(key);
                    if (num > 0)
                    {
                        key = String.Concat(key, "_" + num.ToString());
                    }

                    m_groupDictionary.Add(key, new PairSourceItems(tab));
                    PART_ComboBox.Items.Add(key);

                    PART_CustomizeComboBox.Items.Add(key);
                }
            }
        }


        #region Customize Ribbon
        

        void PART_CustomizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string key = PART_CustomizeComboBox.SelectedItem as string;

            if (RibbonCommandManager.GroupDictionary.ContainsKey(key))
            {
                if (key.Equals(wrapper.QATAllCommandsCaption))
                    AddAllCommands();
                else
                {
                    Dictionary<ICommand, RibbonCommandProvider> commandDictionary = RibbonCommandManager.GroupDictionary[key];
                    if (commandDictionary != null)
                    {
                        PART_ListRibbonTabSource.ItemsSource = commandDictionary.Values;
                    }
                }
            }
            else
            {
                if (m_groupDictionary.ContainsKey(key))
                {
                    if ((m_groupDictionary[key] as PairSourceItems).Items != null)
                    {
                        PART_ListRibbonTabSource.ItemsSource = (m_groupDictionary[key] as PairSourceItems).Items;
                    }
                    else
                    {
                        if (key == RibbonMenuGroup)
                        {
                            InitializeRibbonMenuGroup();
                        }
                        else if (key.Contains(m_ribbon.BackStageHeader.ToString()))
                        {
                            InitializeBackStage(key);
                        }
                        else
                        {
                            InitializeTabGroup(key);
                        }

                        PART_ListRibbonTabSource.ItemsSource = (m_groupDictionary[key] as PairSourceItems).Items;
                    }
                }
                else
                {
                    PART_CustomizeComboBox.SelectedItem = PART_CustomizeComboBox.SelectionBoxItem;
                }
            }
            if (CheckForCustomizeEnable())
            {
                PART_btnRibbonTabAdd.IsEnabled = false;
            }
            else
            {
                PART_btnRibbonTabAdd.IsEnabled = true;
            }
        }     

        #endregion

        /// <summary>
        /// Gets the identical items count.
        /// </summary>
        /// <param name="s">The s value.</param>
        /// <returns> Identical Items Count</returns>
        private int GetIdenticalItemsCount(string s)
        {
            int num = 0;

            IEnumerator en = m_groupDictionary.Keys.GetEnumerator();
            while (en.MoveNext())
            {
                string str = (string)en.Current;
                if (str.StartsWith(s))
                {
                    num++;
                }
            }

            return num;
        }

        /// <summary>
        /// Application menu group forming. 
        /// </summary>
        private void InitializeRibbonMenuGroup()
        {
            PairSourceItems pair = new PairSourceItems(m_appMenu);
            List<IRibbonControl> groupList = new List<IRibbonControl>();
            pair.Items = groupList;
            InitializeRibbonMenuSubItems(pair.Source.Items, ref groupList);
            m_groupDictionary[RibbonMenuGroup] = pair;
        }

        /// <summary>
        /// Initializes the ribbon menu sub items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="groupList">The group list.</param>
        private void InitializeRibbonMenuSubItems(ItemCollection items, ref List<IRibbonControl> groupList)
        {
            foreach (object item in items)
            {
                if (!(item is Separator))
                {
                    if (!(item is RibbonMenuGroup))
                    {
                        if (Ribbon.GetShowInMoreCommands(item as DependencyObject))
                        {
                            QuickAccessToolBarItem qatItem = new QuickAccessToolBarItem(item as UIElement);
                            groupList.Add(qatItem as IRibbonControl);
                        }
                    }

                    if (item is MenuButtonBase || item is RibbonMenuGroup)
                    {
                        InitializeRibbonMenuSubItems((item as ItemsControl).Items, ref groupList);
                    }
                }
            }
        }

        /// <summary>
        /// Adds an item from source(left) list box to destination(right)
        /// list box
        /// </summary>
        private void AddItem()
        {            
            if (PART_ListSource.SelectedItem != null)
            {
                ArrayList olditems, newitems;
                olditems = new ArrayList();
                foreach (QuickAccessToolBarItem item in PART_ListDestination.Items)
                {
                    if (item != null)
                    {
                        olditems.Add(item.SourceElement);
                    }
                }

                QuickAccessToolBarItem selectedItem;
                if (PART_ListSource.SelectedItem is RibbonCommandProvider)
                {
                    selectedItem = new QuickAccessToolBarItem(PART_ListSource.SelectedItem as RibbonCommandProvider);
                }
                else
                {
                    selectedItem = (QuickAccessToolBarItem)PART_ListSource.SelectedItem;
                }

                if (m_commandManager.CanAdd(selectedItem))
                {
                    if (PART_ListDestination.SelectedItem != null)
                    {
                        m_commandManager.cancelInsert = false;
                        m_commandManager.Insert(PART_ListDestination.SelectedIndex + 1, selectedItem);
                                                
                        if (!m_ribbon.QATItems.ContainsKey(selectedItem.ClonedElement))
                            m_ribbon.QATItems.Add(selectedItem.ClonedElement , selectedItem .SourceElement);
                    }
                    else
                    {
                        m_commandManager.cancelInsert = false;
                        m_commandManager.Add(selectedItem);
                        m_commandManager.AddToCancel(selectedItem);

                        if (!m_ribbon.QATItems.ContainsKey(selectedItem.ClonedElement))
                            m_ribbon.QATItems.Add(selectedItem.ClonedElement, selectedItem.SourceElement);
                    }

                    PART_ListDestination.SelectedItem = selectedItem;

                    //PART_btnRemove.IsEnabled = true;

                    if (CheckForEnable() || PART_ListSource.Items.Count == 0)
                    {
                        PART_btnAdd.IsEnabled = false;
                    }

                    newitems = new ArrayList();
                    foreach (QuickAccessToolBarItem item in PART_ListDestination.Items)
                    {
                        if (item != null)
                        {
                            newitems.Add(item.SourceElement);
                        }
                    }
                    m_ribbon.FireQATItemsCollectionChanged(new QATItemsCollectionChangedEventArgs(olditems, newitems, QATItemsContainer.CustomizationDialog, QATAction.Add));
                    if (m_ribbon != null && m_ribbon.QuickAccessToolBar != null && selectedItem != null)
                        m_ribbon.QuickAccessToolBar.OnQATItemAdded(selectedItem.SourceElement as UIElement, selectedItem.SourceElement, selectedItem.ClonedElement as UIElement);
                }
                else
                {
                    QATAlertDialog alertDialog = new QATAlertDialog() { FlowDirection = this.FlowDirection };
                    alertDialog.ShowDialog();
                }
            }

        }

        /// <summary>
        /// Removes an item from destination list box 
        /// </summary>
        private void RemoveItem()
        {            
            int selectedIndex = PART_ListDestination.SelectedIndex;
            UIElement element = null;
            if (selectedIndex != -1)
            {
                element = (PART_ListDestination.SelectedItem as QuickAccessToolBarItem).ClonedElement;
            }

            m_commandManager.AddItemsIndexToCancel(selectedIndex, PART_ListDestination.SelectedItem as QuickAccessToolBarItem);
            m_commandManager.Remove(PART_ListDestination.SelectedItem as QuickAccessToolBarItem);

            if (element != null)
            {
                if (m_ribbon.QATItems.ContainsKey(element))
                    m_ribbon.QATItems.Remove(element);

                if (m_ribbon.QATInitialItems.Contains(element))
                    m_ribbon.QATInitialItemsString.Append(m_ribbon.QATInitialItems.IndexOf(element).ToString() + ",");

                if (m_ribbon != null && m_ribbon.QuickAccessToolBar != null && PART_ListDestination != null && PART_ListDestination.SelectedItem != null)
                    m_ribbon.QuickAccessToolBar.OnQATItemRemoved(PART_ListDestination.SelectedItem as UIElement, PART_ListDestination.SelectedItem, element);
            }

                if (selectedIndex > 0)
                {
                    PART_ListDestination.SelectedIndex = --selectedIndex;
                }
                else
                {
                    PART_ListDestination.SelectedIndex = 0;
                }
                if (PART_ListDestination.Items.Count < 1)
                    PART_btnRemove.IsEnabled = false;
                PART_btnAdd.IsEnabled = true;
         }

        /// <summary>
        /// Applies custom color scheme created with specified blend
        /// color.
        /// </summary>
        /// <param name="blendColor">Specified blend color.</param> 
        private void ApplyColorSheme(Color blendColor)
        {
            if (CustomBrush is LinearGradientBrush)
            {
                LinearGradientBrush brush = CustomBrush as LinearGradientBrush;
                LinearGradientBrush newBrush = new LinearGradientBrush();

                newBrush.StartPoint = brush.StartPoint;
                newBrush.EndPoint = brush.EndPoint;

                foreach (GradientStop stop in brush.GradientStops)
                {
                    GradientStop newStop = new GradientStop(RibbonColorScheme.GetColor(stop.Color, blendColor), stop.Offset);
                    newBrush.GradientStops.Add(newStop);
                }

                CustomBrush = newBrush;
            }
            else if (CustomBrush is SolidColorBrush)
            {
                SolidColorBrush newBrush = new SolidColorBrush();
                newBrush.Color = RibbonColorScheme.GetColor((CustomBrush as SolidColorBrush).Color, blendColor);
                CustomBrush = newBrush;
            }
        }

        /// <summary>
        /// Initializes the tab group.
        /// </summary>
        /// <param name="tabCaption">The tab caption.</param>
        private void InitializeTabGroup(string tabCaption)
        {
            RibbonTab tab = (RibbonTab)(m_groupDictionary[tabCaption] as PairSourceItems).Source;
            if (tab != null)
            {
                List<IRibbonControl> groupList = new List<IRibbonControl>();
                foreach (object barItem in tab.Items)
                {
                    RibbonBar bar = barItem as RibbonBar;
                    if (bar != null)
                    {
                        QuickAccessToolBarItem qatItem = new QuickAccessToolBarItem(bar as UIElement);
                        if (bar.ShowInMoreCommands && Ribbon.GetShowInMoreCommands(bar))
                            groupList.Add(qatItem as IRibbonControl);

                        if (bar.LauncherButton != null && bar.LauncherButton.Visibility == Visibility.Visible && bar.LauncherButton.Command != null)
                        {
                            qatItem = new QuickAccessToolBarItem(bar.LauncherButton);
                            if(Ribbon.GetShowInMoreCommands(bar.LauncherButton))
                                groupList.Add(qatItem as IRibbonControl);
                        }

                        foreach (object item in bar.Items)
                        {                           
                                if (!(item is Separator) && !(item is RibbonSeparator))
                                {
                                    if (item is ButtonPanel)
                                    {
                                        ButtonPanel panel = (ButtonPanel)item;
                                        foreach (object panelItem in panel.Items)
                                        {
                                            if (Ribbon.GetShowInMoreCommands(panelItem as DependencyObject))
                                            {
                                                qatItem = new QuickAccessToolBarItem(panelItem as UIElement);
                                                groupList.Add(qatItem as IRibbonControl);
                                            }
                                        }
                                    }
                                    else if (item is Panel)
                                    {
                                        Panel parentPanel = item as Panel;

                                        foreach (object panelItem in parentPanel.Children)
                                        {
                                            if (Ribbon.GetShowInMoreCommands(panelItem as DependencyObject))
                                            {
                                                qatItem = new QuickAccessToolBarItem(panelItem as UIElement);
                                                groupList.Add(qatItem as IRibbonControl);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Ribbon.GetShowInMoreCommands(item as DependencyObject))
                                        {
                                            qatItem = new QuickAccessToolBarItem(item as UIElement);
                                            if (item is UIElement && (item as UIElement).Visibility == Visibility.Visible)
                                                groupList.Add(qatItem as IRibbonControl);
                                        }
                                    }
                                }
                            
                        }
                    }
                }

                (m_groupDictionary[tabCaption] as PairSourceItems).Items = groupList;
                return;
            }
        }


        private void InitializeBackStage(string backStageCaption)
        {
            Backstage backStage = (Backstage)(m_groupDictionary[backStageCaption] as PairSourceItems).Source;
            if (backStage != null)
            {
                List<IRibbonControl> groupList = new List<IRibbonControl>();
                foreach (object backStageItem in backStage.Items)
                {
                    if (backStageItem is BackstageTabItem)
                    {
                        if ((backStageItem as BackstageTabItem).Content is Panel)
                        {
                            var children = ((backStageItem as BackstageTabItem).Content as Panel).Children;
                            foreach (var item in children)
                            {
                                if (item != null && Ribbon.GetShowInMoreCommands(item as DependencyObject))
                                {
                                    if (item is IRibbonControl)
                                    {
                                        QuickAccessToolBarItem qatItem = new QuickAccessToolBarItem(item as UIElement);
                                        if (item is DependencyObject && (item as IRibbonControl).Label != string.Empty)
                                            groupList.Add(qatItem as IRibbonControl);
                                    }
                                }
                            }
                        }
                        else
                        {
                            object item = (backStageItem as BackstageTabItem).Content;
                            if (item != null && Ribbon.GetShowInMoreCommands(item as DependencyObject))
                            {
                                if (item is IRibbonControl)
                                {
                                    QuickAccessToolBarItem qatItem = new QuickAccessToolBarItem(item as UIElement);
                                    if (item is DependencyObject && Ribbon.GetShowInMoreCommands(item as DependencyObject) && (item as IRibbonControl).Label != string.Empty)
                                        groupList.Add(qatItem as IRibbonControl);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (backStageItem is IRibbonControl)
                        {
                            QuickAccessToolBarItem qatItem = new QuickAccessToolBarItem(backStageItem as UIElement);

                            if (backStageItem is DependencyObject && Ribbon.GetShowInMoreCommands(backStageItem as DependencyObject) && (backStageItem is IRibbonControl) && (qatItem as IRibbonControl).Label != string.Empty)
                                groupList.Add(qatItem as IRibbonControl);
                        }
                    }
                }

                (m_groupDictionary[backStageCaption] as PairSourceItems).Items = groupList;
                return;
            }
        }

        #endregion

        #region Event handlers
        /// <summary>
        /// Occurs when the PART_btnDown button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An RoutedEventArgs that contains the
        /// event data.</param>
        private void BtnDown_Click(object sender, RoutedEventArgs e)
        {
            int index = PART_ListDestination.SelectedIndex;
            if (index != PART_ListDestination.Items.Count - 1)
            {
                m_commandManager.cancelInsert = true;
                QuickAccessToolBarItem item = m_commandManager.GetElementByIndex(PART_ListDestination.SelectedIndex);                
                m_commandManager.Remove(item);
                m_commandManager.Insert(index + 1, item);
                PART_ListDestination.SelectedItem = item;
            }
        }

        /// <summary>
        /// Occurs when the PART_btnUp button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An RoutedEventArgs that contains the
        /// event data.</param>
        private void BtnUp_Click(object sender, RoutedEventArgs e)
        {
            int index = PART_ListDestination.SelectedIndex;
            if (index != 0)
            {
                m_commandManager.cancelInsert = true;
                QuickAccessToolBarItem item = m_commandManager.GetElementByIndex(PART_ListDestination.SelectedIndex);
                m_commandManager.Remove(item);
                m_commandManager.Insert(index - 1, item);
                PART_ListDestination.SelectedItem = item;
            }
        }

        /// <summary>
        /// Occurs when the PART_btnRemove button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An RoutedEventArgs that contains the
        /// event data.</param>
        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            ArrayList olditems, newitems;
            m_commandManager.canRemove = true;
            olditems = new ArrayList();
            foreach (QuickAccessToolBarItem item in PART_ListDestination.Items)
            {
                olditems.Add(item.SourceElement);
            }
            RemoveItem();
            newitems = new ArrayList();
            foreach (QuickAccessToolBarItem item in PART_ListDestination.Items)
            {
                newitems.Add(item.SourceElement);
            }
            m_ribbon.FireQATItemsCollectionChanged(new QATItemsCollectionChangedEventArgs(olditems, newitems, QATItemsContainer.CustomizationDialog,QATAction.Remove));
        }

        /// <summary>
        /// Occurs when the PART_btnAdd button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An RoutedEventArgs that contains the
        /// event data.</param>
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddItem();
        }

        private bool CheckForEnable()
        {
            foreach (var item in PART_ListSource.Items)
            {
                QuickAccessToolBarItem selectedItem;
                if (item is RibbonCommandProvider)
                {
                    selectedItem = new QuickAccessToolBarItem(item as RibbonCommandProvider);
                }
                else
                {
                    selectedItem = (QuickAccessToolBarItem)item;
                }
                if (m_commandManager.CanAdd(selectedItem))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckForCustomizeEnable() 
        {
            foreach (var item in PART_ListRibbonTabSource.Items)
            {
                QuickAccessToolBarItem selectedItem;
                if (item is RibbonCommandProvider)
                {
                    selectedItem = new QuickAccessToolBarItem(item as RibbonCommandProvider);
                }
                else
                {
                    selectedItem = (QuickAccessToolBarItem)item;
                }
                if (m_commandManager.CanAdd(selectedItem))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Occurs when the PART_btnCancel button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An RoutedEventArgs that contains the
        /// event data.</param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            RemoveOnCancel(true);
            if (m_commandManager != null && m_commandManager.ResetFlag)
                m_commandManager.ResetFlag = false;
            DialogResult = false;
            Close();
            m_ribbon.isQATDialogOpened = false;
        }



        private  void RemoveOnCancel(bool cancelFlag)
        {
            foreach (QuickAccessToolBarItem ritem in m_commandManager.duplicateItems)
            {
                m_commandManager.ordered_Items.Add(ritem);
            }

            if (m_commandManager.cancel_items != null && cancelFlag)
            {
                if (m_commandManager.cancel_items.Count != 0)
                {
                    foreach (QuickAccessToolBarItem Qatitem in m_commandManager.cancel_items)
                    {
                        if (m_commandManager.changeItems.Contains(Qatitem))
                        {
                            this.m_commandManager.Remove(Qatitem);
                            m_commandManager.duplicateItems.Remove(Qatitem);
                            m_commandManager.ordered_Items.Remove(Qatitem);
                        }
                    }
                }
            }

            if (m_commandManager.cancelindex_Items != null && cancelFlag)
            {               
                if (m_commandManager.cancelindex_Items.Count != 0)
                {
                    foreach (QuickAccessToolBarItem Qatitem in m_commandManager.ordered_Items)
                    {
                        this.m_commandManager.Remove(Qatitem);
                        m_commandManager.duplicateItems.Remove(Qatitem);
                        m_commandManager.changeItems.Remove(Qatitem);
                    }                   
                }
                else
                {
                    foreach (QuickAccessToolBarItem Qatitem in m_commandManager.ordered_Items)
                    {
                        this.m_commandManager.Remove(Qatitem);
                        m_commandManager.duplicateItems.Remove(Qatitem);
                        m_commandManager.changeItems.Remove(Qatitem);
                    }                   
                }
                //foreach (QuickAccessToolBarItem toolbarItem in m_commandManager.ordered_Items)
                //{
                //    this.m_commandManager.Add(toolbarItem);
                //}
                foreach (QuickAccessToolBarItem item in referenceitems)
                {
                    this.m_commandManager.Add(item);
                }
                m_commandManager.cancelindex_Items.Clear();
            }
            m_commandManager.cancel_items.Clear();
            m_commandManager.cancelIndex = 0;
        }

        /// <summary>
        /// Occurs when the PART_btnOk button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An RoutedEventArgs that contains the
        /// event data.</param>
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            m_ribbon.ResetQATKeyTips();

            RemoveOnCancel(false);
            referenceitems.Clear();
            foreach (QuickAccessToolBarItem item in m_commandManager.Items)
                referenceitems.Add(item);
            if (m_commandManager != null && m_commandManager.ResetFlag)
                m_commandManager.ResetFlag = false;
            DialogResult = true;
            Close();
            m_ribbon.isQATDialogOpened = false;           

            foreach (InternalRibbonBar item in InternalRibbonBarCollection)
            {
                foreach (RibbonTab tabItem in m_ribbon.Items)
                {
                    int tabIndex = m_ribbon.ItemContainerGenerator.IndexFromContainer(tabItem);

                    if (item.TabIndex == tabIndex)
                    {
                        RibbonBar newBar = new RibbonBar();
                        newBar.Header = item.NewBar.Header;
                        QATCustomizationDialog.SetBarOriginalHeader(newBar, newBar.Header);
                        newBar.Tag = item.NewBar.IsCustomBar ? "True" : "False";
                        newBar.IsLauncherButtonVisible = false;
                        
                        tabItem.Items.Add(newBar);
                    }
                }
            }

            bool isChecked=false;

            foreach (RibbonTabs item in RibbonTabs)
            {
                if (item.TabIndex <= m_ribbon.Items.Count - 1)
                {
                    RibbonTab tab = m_ribbon.Items.Count > item.TabIndex ? (m_ribbon.Items[item.TabIndex] as RibbonTab) : null;
                    if (tab != null)
                    {
                        if (!item.IsChecked)
                        {
                            tab.Visibility = Visibility.Collapsed;
                            foreach (RibbonBar ribbonbar in tab.Items)
                            {
                                ribbonbar.Visibility = Visibility.Collapsed;
                            }
                        }
                        else
                        {
                            tab.Visibility = Visibility.Visible;
                            foreach (RibbonBar ribbonbar in tab.Items)
                            {
                                ribbonbar.Visibility = Visibility.Visible;
                            }

                        }

                        tab.Caption = item.Caption;
                        tab.Tag = item.IsCustomTab ? "True" : "False";

                        if (!isChecked && tab.Visibility == Visibility.Visible)
                        {
                            //tab.IsChecked = true;
                            isChecked = true;
                        }
                       
                    }
                }

            }

            ///To Change the RibbonBar Header
            foreach (RibbonTabs ribbonTab in RibbonTabs)
            {
                foreach (RibbonBars ribbonBar in ribbonTab.RibbonBars)
                {
                    if (ribbonTab.TabIndex <= m_ribbon.Items.Count - 1)
                    {
                        RibbonTab tab = m_ribbon.Items.Count > ribbonTab.TabIndex ? (m_ribbon.Items[ribbonTab.TabIndex] as RibbonTab) : null;
                        if (tab != null)
                        {
                            if (tab.Items.Count > ribbonBar.BarIndex)
                                (tab.Items[ribbonBar.BarIndex] as RibbonBar).Header = ribbonBar.Header;
                        }
                    }
                }
            }
           

            bool isitemsAdded = false;
            if (AddedRibbonTabs!=null && AddedRibbonTabs.Count > 0)
            {
                foreach (RibbonTabs tab in AddedRibbonTabs)
                {
                    if (tab.IsChecked)
                    {
                        RibbonTab newTab = new RibbonTab();
                        newTab.Caption = tab.Caption;
                        newTab.Tag = tab.IsCustomTab ? "True" : "False";
                        QATCustomizationDialog.SetTabOriginalHeader(newTab, newTab.Caption);

                        foreach (RibbonBars bar in tab.RibbonBars)
                        {
                            RibbonBar newBar = new RibbonBar();
                            newBar.Header = bar.Header;
                            newBar.Tag = bar.IsCustomBar ? "True" : "False";
                            newBar.IsLauncherButtonVisible = false;
                            QATCustomizationDialog.SetBarOriginalHeader(newBar, newBar.Header);

                            foreach (var item in bar.RibbonControl)
                            {
                                if (item is IRibbonControl)
                                {
                                    if (item is RibbonButton)
                                        (item as RibbonButton).SizeForm = SizeForm.Small;
                                }
                                UIElement control = CloneManager.CloneGeneral(item as UIElement, false) as UIElement;
                                if (!newBar.Items.Contains(control))
                                {
                                    newBar.Items.Add(control);
                                    isitemsAdded = true;
                                }
                            }
                           
                            newTab.Items.Add(newBar);
                        }

                        m_ribbon.Items.Add(newTab);
                    }
                }
            }
            if (!isitemsAdded)
            {
                foreach (InternalRibbonControl item in InternalRibbonControlCollection)
                {
                    RibbonTab tab = m_ribbon.Items.Count > item.TabIndex ? m_ribbon.Items[item.TabIndex] as RibbonTab : null;
                    if (tab != null && tab.Items.Count > 0)
                    {
                        RibbonBar bar = tab.Items.Count > item.BarIndex ? tab.Items[item.BarIndex] as RibbonBar : null;
                        if (bar != null)
                        {
                            if (item.RibbonControl is IRibbonControl)
                            {
                                if (item.RibbonControl is RibbonButton)
                                    (item.RibbonControl as RibbonButton).SizeForm = SizeForm.Small;
                            }

                            UIElement control = CloneManager.CloneGeneral(item.RibbonControl as UIElement, false) as UIElement;
                            if (!bar.Items.Contains(control))
                                bar.Items.Add(control);
                        }
                    }
                }
            }

            //Change the order of the Tabs
            int orderCount = 0;

            foreach (RibbonTabs item in RibbonTabs)
            {
                RibbonTab oldTab = null;

                foreach (RibbonTab ribbonTab in m_ribbon.Items)
                {
                    if (ribbonTab.Caption == item.Caption)
                    {
                        oldTab = ribbonTab;
                        break;
                    }
                }

                //RibbonTab oldTab = m_ribbon.Items.Count > item.TabIndex ? m_ribbon.Items[item.TabIndex] as RibbonTab : null;

                if (item.IsPositionChanged)
                {
                    m_ribbon.Items.Remove(oldTab);
                    m_ribbon.Items.Insert(orderCount, oldTab);
                }
                orderCount++;
            }

            //Change the order of the RibbonBars           
           
            foreach (RibbonTabs ribbonTab in RibbonTabs)
            {
                int barOrderCount = 0;
                foreach (RibbonBars ribbonBar in ribbonTab.RibbonBars)
                {
                    RibbonTab oldTab = null;
                    RibbonBar oldBar = null;

                    foreach (RibbonTab tab in m_ribbon.Items)
                    {
                        if (tab.Caption == ribbonTab.Caption)
                        {
                            oldTab = tab;
                            break;
                        }
                    }

                    if (oldTab != null)
                    {
                        foreach (RibbonBar bar in oldTab.Items)
                        {
                            if (bar.Header == ribbonBar.Header)
                            {
                                oldBar = bar;
                                break;
                            }
                        }
                    }
                    //RibbonTab oldTab = m_ribbon.Items.Count > ribbonTab.TabIndex ? m_ribbon.Items[ribbonTab.TabIndex] as RibbonTab : null;
                    //RibbonBar oldBar = oldTab != null ? (oldTab.Items.Count > ribbonBar.BarIndex ? oldTab.Items[ribbonBar.BarIndex] as RibbonBar : null) : null;

                    if (oldBar!=null)
                    {
                        oldTab.Items.Remove(oldBar);
                        oldTab.Items.Insert(barOrderCount, oldBar);
                    }
                    barOrderCount++;
                }
            }

            //Change the order of RibbonControl

            ObservableCollection<InternalRibbonControl> TempCollection = new ObservableCollection<InternalRibbonControl>();
            bool isEntered=false;

            foreach (RibbonTabs ribbonTab in RibbonTabs)
            {
               
                foreach (RibbonBars ribbonBar in ribbonTab.RibbonBars)
                {
                    RibbonTab oldTab = null;
                    RibbonBar oldBar = null;
                    int itemCount = 0;

                    foreach (RibbonTab tab in m_ribbon.Items)
                    {
                        if (tab.Caption == ribbonTab.Caption)
                        {
                            oldTab = tab;
                            break;
                        }
                    }

                    if (oldTab != null)
                    {
                        foreach (RibbonBar bar in oldTab.Items)
                        {
                            if (bar.Header == ribbonBar.Header && ribbonBar.IsCustomBar)
                            {
                                oldBar = bar;
                                break;
                            }
                        }
                    }

                    if (oldBar != null)
                    {

                        foreach (var ribbonControl in ribbonBar.RibbonControl)
                        {
                            foreach (var item in oldBar.Items)
                            {
                                if (item is IRibbonControl)
                                {
                                    string label = (item as IRibbonControl).Label;
                                    if (ribbonControl.Label == label)
                                    {
                                        InternalRibbonControl control = new InternalRibbonControl();

                                        int tabIndex = m_ribbon.Items.IndexOf(oldTab);
                                        int barIndex = oldTab.Items.IndexOf(oldBar);
                                        int ribbonControlIndex = itemCount;
                                        int originalIndex = oldBar.Items.IndexOf(ribbonControl);

                                        control.TabIndex = tabIndex;
                                        control.BarIndex = barIndex;
                                        control.RibbonControl = item as IRibbonControl;
                                        control.RibbonControlIndex = ribbonControlIndex;
                                        if (isEntered)
                                            control.IsNextBar = itemCount == 0 ? true : false;
                                        control.IsPositionChanged = originalIndex != ribbonControlIndex ? true : false;

                                        TempCollection.Add(control);

                                        itemCount++;
                                        isEntered=true;
                                    }
                                }
                            }
                        }
                    }
                   
                   
                }
            }


            if (TempCollection.Count > 0)
            {
                int tempCount = 0;                
                foreach (var item in TempCollection)
                {
                    RibbonTab tab = m_ribbon.Items.Count > item.TabIndex ? m_ribbon.Items[item.TabIndex] as RibbonTab : null;
                    if (tab != null)
                    {
                        RibbonBar bar = tab.Items.Count > item.BarIndex ? tab.Items[item.BarIndex] as RibbonBar : null;
                        if (bar != null)
                        {
                            if (item.IsNextBar)
                                tempCount = 0;

                            bar.Items.Remove(item.RibbonControl);
                            bar.Items.Insert(tempCount, item.RibbonControl);
                            tempCount++;
                        }
                    }
                }

            }


            foreach (InternalRibbonControl item in InternalRibbonControlRemovedCollection)
            {
                RibbonTab tab = m_ribbon.Items.Count > item.TabIndex ? m_ribbon.Items[item.TabIndex] as RibbonTab : null;
                if (tab != null)
                {
                    RibbonBar bar = tab.Items.Count > item.BarIndex ? tab.Items[item.BarIndex] as RibbonBar : null;
                    if (bar != null)
                    {
                        bar.Items.Remove(item.RibbonControl);
                    }
                }
            }

        }

        /// <summary>
        /// Occurs when the PART_btnReset button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An RoutedEventArgs that contains the
        /// event data.</param>
        private void PART_btnReset_Click(object sender, RoutedEventArgs e)
        {
            QATResetDialog resetDialog = new QATResetDialog() { FlowDirection = this.FlowDirection };
            resetDialog.ShowDialog();

            if (resetDialog.DialogResult.Value)
            {
                m_commandManager.Reset();
                
                //if (PART_ListDestination.Items.Count > 0)
                //    PART_btnRemove.IsEnabled = true;
                //else
                //    PART_btnRemove.IsEnabled = false;

                if (CheckForEnable())
                    PART_btnAdd.IsEnabled = false;
                else
                    PART_btnAdd.IsEnabled = true;
            }
        }

        /// <summary>
        /// Occurs when the PART_ListDestination is double clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An RoutedEventArgs that contains the
        /// event data.</param>
        private void PART_ListDestination_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement source = (FrameworkElement)e.OriginalSource;
            if (source != null && (source.TemplatedParent is ListBoxItem || (source.TemplatedParent as FrameworkElement).TemplatedParent is ListBoxItem))
            {
                RemoveItem();
            }
        }

        /// <summary>
        /// Occurs when the PART_ListSource is double clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An RoutedEventArgs that contains the
        /// event data.</param>
        private void PART_ListSource_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement source = (FrameworkElement)e.OriginalSource;
            if (source != null && (source.TemplatedParent is ListBoxItem || (source.TemplatedParent as FrameworkElement).TemplatedParent is ListBoxItem))
            {
                AddItem();
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the PART_ComboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void PART_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string key = PART_ComboBox.SelectedItem as string;

            if (RibbonCommandManager.GroupDictionary.ContainsKey(key))
            {
                if (key.Equals(wrapper.QATAllCommandsCaption))
                    AddAllCommands();
                else
                {
                    Dictionary<ICommand, RibbonCommandProvider> commandDictionary = RibbonCommandManager.GroupDictionary[key];
                    if (commandDictionary != null)
                    {
                        PART_ListSource.ItemsSource = commandDictionary.Values;
                    }
                }
            }
            else
            {
                if (m_groupDictionary.ContainsKey(key))
                {
                    if ((m_groupDictionary[key] as PairSourceItems).Items != null)
                    {
                        PART_ListSource.ItemsSource = (m_groupDictionary[key] as PairSourceItems).Items;
                    }
                    else
                    {
                        if (key == RibbonMenuGroup)
                        {
                            InitializeRibbonMenuGroup();
                        }
                        /*
                        else if (key.Contains(m_ribbon.BackStageHeader.ToString()))
                        {
                            InitializeBackStage(key);
                        }
                        */
                        else
                        {
                            InitializeTabGroup(key);
                        }

                        PART_ListSource.ItemsSource = (m_groupDictionary[key] as PairSourceItems).Items;
                    }
                }
                else
                {
                    PART_ComboBox.SelectedItem = PART_ComboBox.SelectionBoxItem;
                }
            }
            if (CheckForEnable())
            {
                PART_btnAdd.IsEnabled = false;
            }
            else
            {
                PART_btnAdd.IsEnabled = true;
            }
        }


        /// <summary>
        /// Adds all commands.
        /// </summary>
        private void AddAllCommands()
        {
            this.PART_ListSource.ItemsSource = null;
            List<IRibbonControl> groupList = new List<IRibbonControl>();
            bool isAllowAddQAT = true;
            ObservableCollection<object> ribbonQATCommandTags = new ObservableCollection<object>();
            if (m_commandManager.QuickAccessToolBar.Ribbon.IsQATOnceLoaded && m_commandManager.QuickAccessToolBar.AutoPersist == true)
            {
                foreach (var qatItem in m_commandManager.QuickAccessToolBar.Ribbon.QATInitialItems)
                {
                    object obj = Ribbon.GetRibbonQATCommandTag(qatItem);
                    isAllowAddQAT = true;
                    if (obj != null)
                    {
                        if (!ribbonQATCommandTags.Contains(Ribbon.GetRibbonQATCommandTag(qatItem)))
                        {
                            ribbonQATCommandTags.Add(Ribbon.GetRibbonQATCommandTag(qatItem));
                            isAllowAddQAT = true;
                        }
                        else
                            isAllowAddQAT = false;
                    }     
                    
                    QuickAccessToolBarItem item = new QuickAccessToolBarItem(qatItem);
                    if (isAllowAddQAT && Ribbon.GetShowInMoreCommands(qatItem))
                    {
                       if((item as IRibbonControl) != null && (item as IRibbonControl).Label != string.Empty )
                                 groupList.Add(item as IRibbonControl);
                    }
                }
            }
            else if(m_commandManager.QuickAccessToolBar.DefaultItems!=null)
                foreach (var qatItem in m_commandManager.QuickAccessToolBar.DefaultItems)
                {
                    object obj = Ribbon.GetRibbonQATCommandTag(qatItem);
                    isAllowAddQAT = true;
                    if (obj != null)
                    {
                        if (!ribbonQATCommandTags.Contains(Ribbon.GetRibbonQATCommandTag(qatItem)))
                        {
                            ribbonQATCommandTags.Add(Ribbon.GetRibbonQATCommandTag(qatItem));
                            isAllowAddQAT = true;
                        }
                        else
                            isAllowAddQAT = false;
                    }     
                    

                    QuickAccessToolBarItem item = new QuickAccessToolBarItem(qatItem);
                    if (isAllowAddQAT && (Ribbon.GetShowInMoreCommands(qatItem)))
                    {
                        if ((item as IRibbonControl) != null && (item as IRibbonControl).Label != string.Empty )
                        groupList.Add(item as IRibbonControl);
                    }
                }                  


            if(m_ribbon.BackStage!=null)
            {
                foreach (var backStageItem in m_ribbon.BackStage.Items)
                {
                    if (backStageItem is BackstageTabItem)
                    {
                        if ((backStageItem as BackstageTabItem).Content is Panel)
                        {
                            var children = ((backStageItem as BackstageTabItem).Content as Panel).Children;
                            foreach (var item in children)
                            {
                                if (item != null && Ribbon.GetShowInMoreCommands(item as DependencyObject))
                                {
                                    if (item is IRibbonControl)
                                    {
                                        QuickAccessToolBarItem qatItem = new QuickAccessToolBarItem(item as UIElement);
                                        if (item is DependencyObject && (item as IRibbonControl).Label != string.Empty)
                                            groupList.Add(qatItem as IRibbonControl);
                                    }
                                }
                            }
                        }
                        else
                        {
                            object item = (backStageItem as BackstageTabItem).Content;
                            if (item != null && Ribbon.GetShowInMoreCommands(item as DependencyObject))
                            {
                                if (item is IRibbonControl)
                                {
                                    QuickAccessToolBarItem qatItem = new QuickAccessToolBarItem(item as UIElement);
                                    if (item is DependencyObject && (item as IRibbonControl).Label != string.Empty)
                                        groupList.Add(qatItem as IRibbonControl);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (backStageItem is IRibbonControl)
                        {
                            QuickAccessToolBarItem qatItem = new QuickAccessToolBarItem(backStageItem as UIElement);

                            if (backStageItem is DependencyObject && Ribbon.GetShowInMoreCommands(backStageItem as DependencyObject) && (qatItem as IRibbonControl).Label != string.Empty)
                                groupList.Add(qatItem as IRibbonControl);
                        }
                    }
                }
            }
            
           
            foreach (var tabItem in m_ribbon.Items)
            {
                RibbonTab tab = null;
                if (tabItem is RibbonTab)
                {
                    tab = (RibbonTab)tabItem;
                }
                else
                {
                    tab = m_ribbon.ItemContainerGenerator.ContainerFromItem(tabItem) as RibbonTab;
                }
                bool isAllowAdd = true;
                object obj = Ribbon.GetRibbonQATCommandTag(tab);
                if (obj != null)
                {
                    if (!ribbonQATCommandTags.Contains(Ribbon.GetRibbonQATCommandTag(tab)))
                    {
                        ribbonQATCommandTags.Add(Ribbon.GetRibbonQATCommandTag(tab));
                        isAllowAdd = true;
                    }
                    else
                        isAllowAdd = false;
                }                
               
                if (tab != null && tab.Visibility==Visibility.Visible && isAllowAdd)
                {
                    foreach (object barItem in tab.Items)
                    {
                        RibbonBar bar = null;
                        if (barItem is RibbonBar)
                        {
                            bar = (RibbonBar)barItem;
                        }
                        else
                        {
                            RibbonTabItemsControl tabItemsControl = VisualUtils.FindDescendant(m_ribbon, typeof(RibbonTabItemsControl)) as RibbonTabItemsControl;
                            if (tabItemsControl != null)
                            {
                                bar = tabItemsControl.ItemContainerGenerator.ContainerFromItem(barItem) as RibbonBar;
                            }
                        }
                        bool isAllowAddBar = true;
                        object objBar = null;
                        if (bar != null)
                        {
                             objBar = Ribbon.GetRibbonQATCommandTag(bar);
                            if (objBar != null)
                            {
                                if (!ribbonQATCommandTags.Contains(Ribbon.GetRibbonQATCommandTag(bar)))
                                {
                                    ribbonQATCommandTags.Add(Ribbon.GetRibbonQATCommandTag(bar));
                                    isAllowAddBar = true;
                                }
                                else
                                    isAllowAddBar = false;
                            }
                        }
                        if (bar != null && isAllowAddBar)
                        {
                            QuickAccessToolBarItem qatItem = new QuickAccessToolBarItem(bar as UIElement);

                            if (bar.ShowInMoreCommands && Ribbon.GetShowInMoreCommands(bar))
                            {
                                if ((qatItem as IRibbonControl) != null && (qatItem as IRibbonControl).Label != string.Empty )
                                groupList.Add(qatItem as IRibbonControl);
                            }

                            if (bar.LauncherButton != null && bar.LauncherButton.Visibility == Visibility.Visible && bar.LauncherButton.Command != null)
                            {
                                qatItem = new QuickAccessToolBarItem(bar.LauncherButton);
                                if (Ribbon.GetShowInMoreCommands(bar.LauncherButton))
                                {                                   
                                    if ((qatItem as IRibbonControl) != null&& (qatItem as IRibbonControl).Label != string.Empty )
                                    groupList.Add(qatItem as IRibbonControl);
                                }
                            }

                            foreach (object item in bar.Items)
                            {
                                Object tempItem = null;
                                if (bar.ItemsSource != null)
                                {
                                    tempItem = bar.ItemContainerGenerator.ContainerFromItem(item);
                                    if(tempItem!=null)
                                        tempItem = VisualUtils.FindDescendant(tempItem as Visual, typeof(FrameworkElement));
                                }
                                else
                                {
                                    tempItem = item;
                                }
                                if (!(tempItem is Separator) && !(tempItem is RibbonSeparator))
                                    {
                                        if (tempItem is ButtonPanel)
                                        {
                                            ButtonPanel panel = (ButtonPanel)tempItem;
                                            bool isAllowAddPanel = true;
                                            object objPanel = Ribbon.GetRibbonQATCommandTag(panel);
                                            if (objPanel != null)
                                            {
                                                if (!ribbonQATCommandTags.Contains(Ribbon.GetRibbonQATCommandTag(panel)))
                                                {
                                                    ribbonQATCommandTags.Add(Ribbon.GetRibbonQATCommandTag(panel));
                                                    isAllowAddPanel = true;
                                                }
                                                else
                                                    isAllowAddPanel = false;
                                            }
                                            if (isAllowAddPanel)
                                            {
                                                foreach (object panelItem in panel.Items)
                                                {
                                                    if (Ribbon.GetShowInMoreCommands(panelItem as DependencyObject))
                                                    {
                                                        bool isAllowAddUIElement = true;
                                                        object objUIElement = Ribbon.GetRibbonQATCommandTag(panelItem as UIElement);
                                                        if (objUIElement != null)
                                                        {
                                                            if (!ribbonQATCommandTags.Contains(Ribbon.GetRibbonQATCommandTag(panelItem as UIElement)))
                                                            {
                                                                ribbonQATCommandTags.Add(Ribbon.GetRibbonQATCommandTag(panelItem as UIElement));
                                                                isAllowAddUIElement = true;
                                                            }
                                                            else
                                                                isAllowAddUIElement = false;
                                                        }
                                                        qatItem = new QuickAccessToolBarItem(panelItem as UIElement);
                                                        if (isAllowAddUIElement)
                                                        {
                                                            if (qatItem as IRibbonControl != null && (qatItem as IRibbonControl).Label != null )
                                                                groupList.Add(qatItem as IRibbonControl);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (tempItem is Panel)
                                        {
                                            Panel parentPanel = tempItem as Panel;

                                            foreach (object panelItem in parentPanel.Children)
                                            {
                                                if (Ribbon.GetShowInMoreCommands(panelItem as DependencyObject))
                                                {
                                                    qatItem = new QuickAccessToolBarItem(panelItem as UIElement);
                                                    groupList.Add(qatItem as IRibbonControl);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (tempItem!=null && Ribbon.GetShowInMoreCommands(tempItem as DependencyObject))
                                            {
                                                bool isAllowAddUIElement = true;
                                                object objUIElement = Ribbon.GetRibbonQATCommandTag(tempItem as UIElement);
                                                if (objUIElement != null)
                                                {
                                                    if (!ribbonQATCommandTags.Contains(Ribbon.GetRibbonQATCommandTag(tempItem as UIElement)))
                                                    {
                                                        ribbonQATCommandTags.Add(Ribbon.GetRibbonQATCommandTag(tempItem as UIElement));
                                                        isAllowAddUIElement = true;
                                                    }
                                                    else
                                                        isAllowAddUIElement = false;
                                                }
                                                qatItem = new QuickAccessToolBarItem(tempItem as UIElement);
                                                if(isAllowAddUIElement)
                                                    if ((tempItem as UIElement) != null && (tempItem as UIElement).Visibility == Visibility.Visible && (tempItem as IRibbonControl) != null && (tempItem as IRibbonControl).Label != string.Empty)
                                                        groupList.Add(qatItem as IRibbonControl);
                                            }
                                        }
                                    }
                                
                            }
                        }
                    }
                }
            }
            this.PART_ListSource.ItemsSource = groupList;
            this.PART_ListRibbonTabSource.ItemsSource = groupList;
        }

        /// <summary>
        /// Occurs when access key is pressed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An RoutedEventArgs that contains the
        /// event data.</param>
        private void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
        {
            if (e.Target != null && e.Target is ComboBox)
            {
                (e.Target as ComboBox).IsDropDownOpen = true;
            }
        }

        /// <summary>
        /// Occurs when the PART_ListDestination selection is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An RoutedEventArgs that contains the
        /// event data.</param>
        private void PART_ListDestination_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PART_ListDestination.SelectedItem != null)
            {
                PART_btnUp.IsEnabled = (PART_ListDestination.SelectedIndex > 0) ? true : false;
                PART_btnDown.IsEnabled = (PART_ListDestination.SelectedIndex < PART_ListDestination.Items.Count - 1) ? true : false;
                PART_btnRemove.IsEnabled = true;
            }
            else
            {
                PART_btnUp.IsEnabled = false;
                PART_btnDown.IsEnabled = false;
                PART_btnRemove.IsEnabled = false;
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Raises the KeyDown event.
        /// </summary>
        /// <param name="e">A KeyEventArgs that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        DialogResult = true;
                        break;
                    case Key.Escape:
                        DialogResult = false;
                        break;
                    default:
                        break;
                }

                Close();
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {

            if (e.Property == SkinStorage.VisualStyleProperty)
            {
                if (SkinStorage.GetVisualStyle(this).Contains("Office2010"))
                {
                    qatInnerBorder.Background = new SolidColorBrush(Colors.White);
                    qatInnerGrid.Background = new SolidColorBrush(Colors.White);
                    qatOuterBorder.Background = new SolidColorBrush(Colors.White);
                }

            }
            base.OnPropertyChanged(e);
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (PART_btnOk != null)
                PART_btnOk.Click -= new RoutedEventHandler(BtnOk_Click);
            if (PART_btnCancel != null)
                PART_btnCancel.Click -= new RoutedEventHandler(BtnCancel_Click);
            if (PART_btnAdd != null)
                PART_btnAdd.Click -= new RoutedEventHandler(BtnAdd_Click);
            if (PART_btnRemove != null)
                PART_btnRemove.Click -= new RoutedEventHandler(BtnRemove_Click);
            if (PART_btnUp != null)
                PART_btnUp.Click -= new RoutedEventHandler(BtnUp_Click);
            if (PART_btnDown != null)
                PART_btnDown.Click -= new RoutedEventHandler(BtnDown_Click);
            if (PART_btnReset != null)
                PART_btnReset.Click -= new RoutedEventHandler(PART_btnReset_Click);
            if (PART_ListSource != null)
            {
                PART_ListSource.MouseDoubleClick -= new MouseButtonEventHandler(PART_ListSource_MouseDoubleClick);
                //PART_ListSource.ItemsSource = null;
            }
            if (PART_ComboBox != null)
                PART_ComboBox.SelectionChanged -= new SelectionChangedEventHandler(PART_ComboBox_SelectionChanged);
            if (PART_ListDestination != null)
            {
                PART_ListDestination.MouseDoubleClick -= new MouseButtonEventHandler(PART_ListDestination_MouseDoubleClick);
                PART_ListDestination.SelectionChanged -= new SelectionChangedEventHandler(PART_ListDestination_SelectionChanged);
                //PART_ListDestination.ItemsSource = null;
            }
            AccessKeyManager.RemoveAccessKeyPressedHandler(PART_lblChoose, new AccessKeyPressedEventHandler(OnAccessKeyPressed));
            this.Closed -= new EventHandler(QATCustomizationDialog_Closed);
            this.Loaded -= new RoutedEventHandler(QATCustomizationDialog_Loaded);


            #region Customize Ribbon

            PART_btnRibbonTabNew.Click -= new RoutedEventHandler(PART_btnRibbonTabNew_Click);
            PART_btnRibbonBarNew.Click -= new RoutedEventHandler(PART_btnRibbonBarNew_Click);
            PART_btnRename.Click -= new RoutedEventHandler(PART_btnRename_Click);
            PART_btnRibbonTabAdd.Click -= new RoutedEventHandler(PART_btnRibbonTabAdd_Click);
            PART_btnTabDown.Click -= new RoutedEventHandler(PART_btnTabDown_Click);
            PART_btnTabUp.Click -= new RoutedEventHandler(PART_btnTabUp_Click);
            PART_btnRibbonTabRemove.Click -= new RoutedEventHandler(PART_btnRibbonTabRemove_Click);
            ribbonTreeView.SelectedItemChanged -= new RoutedPropertyChangedEventHandler<object>(ribbonTreeView_SelectedItemChanged);

            btn_DropDown.IsDropDownOpenChanged -= new PropertyChangedCallback(btn_DropDown_IsDropDownOpenChanged);
           
            (btn_DropDown.Items[1] as RibbonMenuItem).Click -= new RoutedEventHandler(Btn_ResetCustomization_Click);
            (btn_DropDown.Items[0] as RibbonMenuItem).Click -= new RoutedEventHandler(Btn_ResetSelectedTabCustomization_Click);
            Btn_DeleteCustomization.Click -= new RoutedEventHandler(Btn_DeleteCustomization_Click);

            PART_ListRibbonTabSource.SelectionChanged -= new SelectionChangedEventHandler(PART_ListRibbonTabSource_SelectionChanged);

            #endregion         
        }

      
        #endregion

        #region Customize Ribbon


        private ObservableCollection<RibbonTabs> _addedRibbonTabs;

        public ObservableCollection<RibbonTabs> AddedRibbonTabs
        {
            get
            {
                return _addedRibbonTabs;
            }
            set
            {
                _addedRibbonTabs = value;
                RaisePropertyChanged("AddedRibbonTabs");
            }
        }
        

        ObservableCollection<RibbonTabs> RibbonTabs = new ObservableCollection<RibbonTabs>();
        //ObservableCollection<RibbonTabs> AddedRibbonTabs = new ObservableCollection<RibbonTabs>();
        ObservableCollection<InternalRibbonBar> InternalRibbonBarCollection = new ObservableCollection<InternalRibbonBar>();
        ObservableCollection<InternalRibbonControl> InternalRibbonControlCollection = new ObservableCollection<InternalRibbonControl>();
        ObservableCollection<InternalRibbonControl> InternalRibbonControlRemovedCollection = new ObservableCollection<InternalRibbonControl>();


        #region Attached Properties for Customize Ribbon
        

        internal static string GetTabOriginalHeader(DependencyObject obj)
        {
            return (string)obj.GetValue(TabOriginalHeaderProperty);
        }

        internal static void SetTabOriginalHeader(DependencyObject obj, string value)
        {
            obj.SetValue(TabOriginalHeaderProperty, value);
        }

        // Using a DependencyProperty as the backing store for TabOriginalHeader.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TabOriginalHeaderProperty =
            DependencyProperty.RegisterAttached("TabOriginalHeader", typeof(string), typeof(QATCustomizationDialog), new UIPropertyMetadata(string.Empty));



        internal static string GetBarOriginalHeader(DependencyObject obj)
        {
            return (string)obj.GetValue(BarOriginalHeaderProperty);
        }

        internal static void SetBarOriginalHeader(DependencyObject obj, string value)
        {
            obj.SetValue(BarOriginalHeaderProperty, value);
        }

        // Using a DependencyProperty as the backing store for BarOriginalHeader.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty BarOriginalHeaderProperty =
            DependencyProperty.RegisterAttached("BarOriginalHeader", typeof(string), typeof(QATCustomizationDialog), new UIPropertyMetadata(string.Empty));

        

        internal static int GetBarOriginalIndex(DependencyObject obj)
        {
            return (int)obj.GetValue(BarOriginalIndexProperty);
        }

        internal static void SetBarOriginalIndex(DependencyObject obj, int value)
        {
            obj.SetValue(BarOriginalIndexProperty, value);
        }

        // Using a DependencyProperty as the backing store for BarOriginalIndex.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty BarOriginalIndexProperty =
            DependencyProperty.RegisterAttached("BarOriginalIndex", typeof(int), typeof(QATCustomizationDialog), new UIPropertyMetadata(-1));

        internal static int GetTabOriginalIndex(DependencyObject obj)
        {
            return (int)obj.GetValue(TabOriginalIndexProperty);
        }

        internal static void SetTabOriginalIndex(DependencyObject obj, int value)
        {
            obj.SetValue(TabOriginalIndexProperty, value);
        }

        // Using a DependencyProperty as the backing store for TabOriginalIndex.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TabOriginalIndexProperty =
            DependencyProperty.RegisterAttached("TabOriginalIndex", typeof(int), typeof(QATCustomizationDialog), new UIPropertyMetadata(-1));

        #endregion

        private void AddAllRibbonTabs() 
        {
            if (AddedRibbonTabs == null)
                AddedRibbonTabs = new ObservableCollection<Controls.RibbonTabs>();

            RibbonTab tab;
            int i=0;
            foreach (var item in m_ribbon.Items)
            {
                int barIndex = 0;

                if (!(item is RibbonTab))
                {
                    tab = m_ribbon.ItemContainerGenerator.ContainerFromItem(item) as RibbonTab;
                }
                else
                    tab = item as RibbonTab;

                if (QATCustomizationDialog.GetTabOriginalHeader(tab) == string.Empty)
                    QATCustomizationDialog.SetTabOriginalHeader(tab, tab.Caption);
                if (QATCustomizationDialog.GetBarOriginalIndex(tab) == -1)
                    QATCustomizationDialog.SetBarOriginalIndex(tab,m_ribbon.Items.IndexOf(tab));                        
                RibbonTabs tabs = new RibbonTabs();
                tabs.Caption = tab.Caption;
                tabs.IsRenamed = QATCustomizationDialog.GetTabOriginalHeader(tab) == tab.Caption ? false : true;
                tabs.IsChecked = tab.Visibility == Visibility.Visible ? true : false;
                tabs.TabIndex = i;
                if (tab.Tag != null)
                    tabs.IsCustomTab = tab.Tag.ToString() == "True" ? true : false;
                i++;

                if (tabs.RibbonBars == null)
                    tabs.RibbonBars = new ObservableCollection<RibbonBars>();

               

                foreach (var ribbonBar in tab.Items)
                {
                    if (ribbonBar is RibbonBar)
                    {
                        if (QATCustomizationDialog.GetBarOriginalHeader((ribbonBar as RibbonBar)) == string.Empty)
                            QATCustomizationDialog.SetBarOriginalHeader((ribbonBar as RibbonBar), (ribbonBar as RibbonBar).Header);
                        if (QATCustomizationDialog.GetBarOriginalIndex((ribbonBar as RibbonBar)) == -1)
                            QATCustomizationDialog.SetBarOriginalIndex(ribbonBar as RibbonBar, tab.Items.IndexOf(ribbonBar));
                        RibbonBars bars = new RibbonBars();
                        bars.Header = (ribbonBar as RibbonBar).Header;
                        bars.BarIndex = barIndex;
                        bars.IsRenamed = QATCustomizationDialog.GetBarOriginalHeader((ribbonBar as RibbonBar)) == (ribbonBar as RibbonBar).Header ? false : true;
                        bars.IsPositionChanged = QATCustomizationDialog.GetBarOriginalIndex((ribbonBar as RibbonBar)) == barIndex ? false : true;
                        barIndex++;
                        if ((ribbonBar as RibbonBar).Tag != null)
                            bars.IsCustomBar = (ribbonBar as RibbonBar).Tag.ToString() == "True" ? true : false;

                        if (bars.RibbonControl == null)
                            bars.RibbonControl = new ObservableCollection<IRibbonControl>();


                        foreach (var ribbonBarItem in (ribbonBar as RibbonBar).Items)
                        {
                            if (ribbonBarItem is IRibbonControl)
                            {
                                if (!(ribbonBarItem is RibbonCheckBox || ribbonBarItem is RibbonComboBox || ribbonBarItem is RibbonTextBox || ribbonBarItem is RibbonGallery || ribbonBarItem is RibbonGalleryItem || ribbonBarItem is RibbonRadioButton))
                                    bars.RibbonControl.Add(ribbonBarItem as IRibbonControl);
                            }

                            else if (ribbonBarItem is Panel)
                            {
                                foreach (var barItem in (ribbonBarItem as Panel).Children)
                                {
                                    if (barItem is IRibbonControl)
                                    {
                                        if (!(barItem is RibbonCheckBox || barItem is RibbonComboBox || barItem is RibbonTextBox || barItem is RibbonGallery || barItem is RibbonGalleryItem || barItem is RibbonRadioButton))
                                            bars.RibbonControl.Add(barItem as IRibbonControl);
                                    }
                                }
                            }
                            else if (ribbonBarItem is ItemsControl)
                            {
                                foreach (var barItem in (ribbonBarItem as ItemsControl).Items)
                                {
                                    if (barItem is IRibbonControl)
                                    {
                                        if (!(barItem is RibbonCheckBox || barItem is RibbonComboBox || barItem is RibbonTextBox || barItem is RibbonGallery || barItem is RibbonGalleryItem || barItem is RibbonRadioButton))
                                            bars.RibbonControl.Add(barItem as IRibbonControl);
                                    }
                                }
                            }


                        }
                        tabs.RibbonBars.Add(bars);
                    }
                }

                RibbonTabs.Add(tabs);
            }
            ribbonTreeView.ItemsSource = RibbonTabs;

            ribbonTreeView.ItemContainerGenerator.StatusChanged += (sender, e) =>
            {
                if ((sender as ItemContainerGenerator).Status == GeneratorStatus.ContainersGenerated)
                {
                    TreeViewItem firstItem = (sender as ItemContainerGenerator).ContainerFromIndex(0) as TreeViewItem;
                    if (firstItem != null)
                    {
                        firstItem.IsSelected = true;
                        firstItem.IsExpanded = true;
                    }
                }
            };
        }


        void PART_btnRibbonBarNew_Click(object sender, RoutedEventArgs e)
        {
            if (ribbonTreeView.SelectedItem is RibbonTabs)
            {
                string barCount = (ribbonTreeView.SelectedItem as RibbonTabs).RibbonBars.Count.ToString();
                RibbonBars bar = new RibbonBars();
                bar.Header = "New Bar" + (barCount == "0" ? string.Empty : barCount);
                bar.IsCustomBar = true;
                bar.BarIndex = (ribbonTreeView.SelectedItem as RibbonTabs).RibbonBars.Count;
                bar.RibbonControl = new ObservableCollection<IRibbonControl>();
                (ribbonTreeView.SelectedItem as RibbonTabs).RibbonBars.Add(bar);              

                InternalRibbonBar newBar = new InternalRibbonBar();
                newBar.TabIndex = (ribbonTreeView.SelectedItem as RibbonTabs).TabIndex;
                newBar.NewBar = bar;
                InternalRibbonBarCollection.Add(newBar);

                TreeViewItem item = ribbonTreeView.ItemContainerGenerator.ContainerFromItem(ribbonTreeView.SelectedItem) as TreeViewItem;
                if (item != null)
                {
                    item.IsExpanded = true;                    
                }

              
            }
        }

        void PART_btnRibbonTabNew_Click(object sender, RoutedEventArgs e)
        {
            int minusCount = 0;
            foreach (var item in m_ribbon.ContextTabGroups)
            {
                foreach (var ribbonTab in item.RibbonTabs)
                {
                    minusCount++;
                }
            }
            RibbonTab tab = new RibbonTab();
            tab.Caption = "New Tab";            
            QATCustomizationDialog.SetTabOriginalHeader(tab, tab.Caption);

            RibbonTabs newTabs = new RibbonTabs();
            newTabs.Caption = tab.Caption;
            newTabs.IsChecked = true;
            newTabs.IsCustomTab = true;
            newTabs.TabIndex = ribbonTreeView.Items.Count;//- minusCount;
            newTabs.RibbonBars= new ObservableCollection<RibbonBars>();

            RibbonTabs.Add(newTabs);

            ribbonTreeView.ItemsSource = RibbonTabs;

            (ribbonTreeView.ItemContainerGenerator.ContainerFromItem(newTabs) as TreeViewItem).IsSelected = true;
            PART_btnTabDown.IsEnabled = false;
            //temp collection for adding new ribbontab,ribbonbar
            AddedRibbonTabs.Add(newTabs);
        }

        void PART_btnRename_Click(object sender, RoutedEventArgs e)
        {          

            if (ribbonTreeView.SelectedItem is RibbonTabs)
            {
                QATCustomizeRibbonDialog ribbonDialog = new QATCustomizeRibbonDialog(ribbonTreeView.SelectedItem as RibbonTabs);
                ribbonDialog.Owner = this;
                ribbonDialog.DisplayName = (ribbonTreeView.SelectedItem as RibbonTabs).Caption;
                ribbonDialog.ShowDialog();
            }

            else if (ribbonTreeView.SelectedItem is RibbonBars)
            {
                QATCustomizeRibbonDialog ribbonDialog = new QATCustomizeRibbonDialog(ribbonTreeView.SelectedItem as RibbonBars);
                ribbonDialog.Owner = this;
                ribbonDialog.DisplayName = (ribbonTreeView.SelectedItem as RibbonBars).Header;
                ribbonDialog.ShowDialog();
            }
            else
            {
                QATCustomizeRibbonDialog ribbonDialog = new QATCustomizeRibbonDialog(ribbonTreeView.SelectedItem as IRibbonControl);
                ribbonDialog.Owner = this;
                ribbonDialog.DisplayName = (ribbonTreeView.SelectedItem as IRibbonControl).Label;
                ribbonDialog.ShowDialog();
            }
           
        }

        void PART_btnRibbonTabAdd_Click(object sender, RoutedEventArgs e)
        {
            if (ribbonTreeView.SelectedItem is RibbonBars)
            {
                bool isCustomBar = (ribbonTreeView.SelectedItem as RibbonBars).IsCustomBar;
                if (isCustomBar)
                {
                    if (PART_ListRibbonTabSource.SelectedItem != null)
                    {
                        IRibbonControl control = (PART_ListRibbonTabSource.SelectedItem as QuickAccessToolBarItem).ClonedElement as IRibbonControl;
                        if (control != null)
                        {
                            (ribbonTreeView.SelectedItem as RibbonBars).RibbonControl.Add(control);

                            foreach (RibbonTabs tab in RibbonTabs)
                            {
                                if (tab.RibbonBars.Contains(ribbonTreeView.SelectedItem as RibbonBars))
                                {
                                    InternalRibbonControl ribbonControl = new InternalRibbonControl();
                                    ribbonControl.TabIndex = tab.TabIndex;
                                    ribbonControl.BarIndex = (tab.RibbonBars.IndexOf(ribbonTreeView.SelectedItem as RibbonBars));
                                    ribbonControl.RibbonControl = control;

                                    InternalRibbonControlCollection.Add(ribbonControl);
                                }
                            }
                        }
                    }                   
                }
                else
                {
                    ShowMessage();
                }

            }
            else
            {
                ShowMessage();
            }
        }

        void PART_btnRibbonTabRemove_Click(object sender, RoutedEventArgs e)
        {
            if (ribbonTreeView.SelectedItem is IRibbonControl)
            {
                foreach (RibbonTabs tab in RibbonTabs)
                {
                    foreach (RibbonBars bar in tab.RibbonBars)
                    {
                        if (bar.RibbonControl.Contains(ribbonTreeView.SelectedItem as IRibbonControl))
                        {
                            InternalRibbonControl ribbonControl = new InternalRibbonControl();
                            ribbonControl.TabIndex = tab.TabIndex;
                            ribbonControl.BarIndex = (tab.RibbonBars.IndexOf(bar as RibbonBars));
                            ribbonControl.RibbonControl = ribbonTreeView.SelectedItem as IRibbonControl;

                            InternalRibbonControlRemovedCollection.Add(ribbonControl);

                            bar.RibbonControl.Remove(ribbonTreeView.SelectedItem as IRibbonControl);                         

                        }
                    }
                }
            }
        }           

        void ShowMessage()
        {
            string title = "Ribbon Customization";
            string message = "Commands can only be added to custom groups.\n\n";
            message += "To add a custom group,click the tab where you want the group to appear,and then click New Group. ";

            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        void PART_btnTabUp_Click(object sender, RoutedEventArgs e)
        {
            if (ribbonTreeView.SelectedItem is RibbonTabs)
            {
                int oldLocation = RibbonTabs.IndexOf(ribbonTreeView.SelectedItem as RibbonTabs);               
                if (oldLocation > 0)
                {
                    int newLocation = oldLocation - 1;
                    RibbonTabs.Move(oldLocation, newLocation);        

                    RibbonTabs tab=ribbonTreeView.SelectedItem as RibbonTabs;
                    tab.IsPositionChanged = tab.TabIndex != newLocation ? true : false;
                }
            }
            else if (ribbonTreeView.SelectedItem is RibbonBars)
            {
                var ribbonTab = GetRibbonTab(ribbonTreeView.SelectedItem as RibbonBars);
                if (ribbonTab != null)
                {
                    int oldLocation = ribbonTab.RibbonBars.IndexOf(ribbonTreeView.SelectedItem as RibbonBars);
                    if (oldLocation > 0)
                    {
                        int newLocation = oldLocation - 1;
                        ribbonTab.RibbonBars.Move(oldLocation, newLocation);

                        RibbonBars bar = ribbonTreeView.SelectedItem as RibbonBars;
                        bar.IsPositionChanged = bar.BarIndex != newLocation ? true : false;
                    }
                }
            }

            else if (ribbonTreeView.SelectedItem is IRibbonControl)
            {
                var ribbonBar = GetRibbonBar(ribbonTreeView.SelectedItem as IRibbonControl);
                if (ribbonBar != null)
                {
                    int oldLocation = ribbonBar.RibbonControl.IndexOf(ribbonTreeView.SelectedItem as IRibbonControl);
                    if (oldLocation > 0)
                    {
                        int newLocation = oldLocation - 1;
                        ribbonBar.RibbonControl.Move(oldLocation, newLocation);                       
                    }
                }
            }
            RefreshUpDownButton();
        }

        private RibbonTabs GetRibbonTab(RibbonBars ribbonBar)
        {
            RibbonTabs ribbonTab = null;

            foreach (var item in RibbonTabs)
            {
                if (item.RibbonBars.Contains(ribbonBar))
                {
                    ribbonTab = item;
                    break;
                }
            }
            return ribbonTab;
        }

        private RibbonBars GetRibbonBar(IRibbonControl ribbonControl)
        {
            RibbonBars bars = null;
            foreach (RibbonTabs tab in RibbonTabs)
            {
                foreach (RibbonBars bar in tab.RibbonBars)
                {
                    if (bar.RibbonControl.Contains(ribbonControl))
                    {
                        bars = bar;
                        break;
                    }                  
                }
            }
            return bars;
        }
       

        void PART_btnTabDown_Click(object sender, RoutedEventArgs e)
        {
            if (ribbonTreeView.SelectedItem is RibbonTabs)
            {
                int oldLocation = RibbonTabs.IndexOf(ribbonTreeView.SelectedItem as RibbonTabs);
                if (oldLocation < RibbonTabs.Count - 1)
                {
                    int newLocation = oldLocation + 1;
                    RibbonTabs.Move(oldLocation, newLocation);

                    RibbonTabs tab = ribbonTreeView.SelectedItem as RibbonTabs;
                    tab.IsPositionChanged = tab.TabIndex == newLocation ? false : true;
                }
            }

            else if (ribbonTreeView.SelectedItem is RibbonBars)
            {
                var ribbonTab = GetRibbonTab(ribbonTreeView.SelectedItem as RibbonBars);
                if (ribbonTab != null)
                {
                    int oldLocation = ribbonTab.RibbonBars.IndexOf(ribbonTreeView.SelectedItem as RibbonBars);
                    if (oldLocation < ribbonTab.RibbonBars.Count-1)
                    {
                        int newLocation = oldLocation + 1;
                        ribbonTab.RibbonBars.Move(oldLocation, newLocation);

                        RibbonBars bar = ribbonTreeView.SelectedItem as RibbonBars;
                        bar.IsPositionChanged = bar.BarIndex != newLocation ? true : false;
                    }
                }
            }
            else if (ribbonTreeView.SelectedItem is IRibbonControl)
            {
                var ribbonBar = GetRibbonBar(ribbonTreeView.SelectedItem as IRibbonControl);
                if (ribbonBar != null)
                {
                    int oldLocation = ribbonBar.RibbonControl.IndexOf(ribbonTreeView.SelectedItem as IRibbonControl);
                    if (oldLocation <ribbonBar.RibbonControl.Count-1)
                    {
                        int newLocation = oldLocation + 1;
                        ribbonBar.RibbonControl.Move(oldLocation, newLocation);
                    }
                }
            }

            RefreshUpDownButton();
        }


        void RefreshUpDownButton()
        {
            if (ribbonTreeView.SelectedItem is RibbonTabs)
            {
                TreeViewItem firstTab = ribbonTreeView.ItemContainerGenerator.ContainerFromItem(ribbonTreeView.SelectedItem) as TreeViewItem;
                int itemIndex = ribbonTreeView.ItemContainerGenerator.IndexFromContainer(firstTab);
                if (itemIndex == 0)
                {
                    PART_btnTabDown.IsEnabled = true;
                    PART_btnTabUp.IsEnabled = false;
                }
                else if (itemIndex == RibbonTabs.Count - 1)
                {
                    PART_btnTabUp.IsEnabled = true;
                    PART_btnTabDown.IsEnabled = false;
                }
                else
                {
                    PART_btnTabDown.IsEnabled = true;
                    PART_btnTabUp.IsEnabled = true;
                }             

            }

            else if (ribbonTreeView.SelectedItem is RibbonBars)
            {

                var ribbonTab = GetRibbonTab(ribbonTreeView.SelectedItem as RibbonBars);
                if (ribbonTab != null)
                {
                    int itemIndex = ribbonTab.RibbonBars.IndexOf(ribbonTreeView.SelectedItem as RibbonBars);

                    if (itemIndex == 0)
                    {
                        PART_btnTabDown.IsEnabled = true;
                        PART_btnTabUp.IsEnabled = false;
                    }
                    else if (itemIndex == ribbonTab.RibbonBars.Count - 1)
                    {
                        PART_btnTabUp.IsEnabled = true;
                        PART_btnTabDown.IsEnabled = false;
                    }
                    else
                    {
                        PART_btnTabDown.IsEnabled = true;
                        PART_btnTabUp.IsEnabled = true;
                    }
                }
            }
            else if (ribbonTreeView.SelectedItem is IRibbonControl)
            {
                var bar = GetRibbonBar(ribbonTreeView.SelectedItem as IRibbonControl);
                if (bar != null)
                {
                    int itemIndex = bar.RibbonControl.IndexOf(ribbonTreeView.SelectedItem as IRibbonControl);

                    if (itemIndex == 0)
                    {
                        if (bar.RibbonControl.Count == 1)
                        {
                            PART_btnTabDown.IsEnabled = false;
                            PART_btnTabUp.IsEnabled = false;
                        }
                        else
                        {
                            PART_btnTabDown.IsEnabled = true;
                            PART_btnTabUp.IsEnabled = false;
                        }
                    }
                    else if (itemIndex == bar.RibbonControl.Count - 1)
                    {
                        PART_btnTabUp.IsEnabled = true;
                        PART_btnTabDown.IsEnabled = false;
                    }
                    else
                    {
                        PART_btnTabDown.IsEnabled = true;
                        PART_btnTabUp.IsEnabled = true;
                    }
                }
            }
        }

        void ribbonTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (PART_ListRibbonTabSource.SelectedItem is RibbonCommandProvider)
            {
                PART_btnRibbonTabAdd.IsEnabled = false;
                return;
            }

            if (ribbonTreeView.SelectedItem is RibbonTabs)
            {
                TreeViewItem firstTab = ribbonTreeView.ItemContainerGenerator.ContainerFromItem(ribbonTreeView.SelectedItem) as TreeViewItem;
                int itemIndex = ribbonTreeView.ItemContainerGenerator.IndexFromContainer(firstTab);
                if (itemIndex == 0)
                {
                    PART_btnTabDown.IsEnabled = true;
                    PART_btnTabUp.IsEnabled = false;
                }
                else if (itemIndex == RibbonTabs.Count - 1)
                {
                    PART_btnTabUp.IsEnabled = true;
                    PART_btnTabDown.IsEnabled = false;
                }
                else
                {
                    PART_btnTabDown.IsEnabled = true;
                    PART_btnTabUp.IsEnabled = true;
                }
                PART_btnRename.IsEnabled = true;
                if (!(ribbonTreeView.SelectedItem as RibbonTabs).IsCustomTab)
                    Btn_DeleteCustomization.IsEnabled = false;
                else
                    Btn_DeleteCustomization.IsEnabled = true;
            }

            if (ribbonTreeView.SelectedItem is RibbonBars)
            {
                bool isCustomBar = (ribbonTreeView.SelectedItem as RibbonBars).IsCustomBar;
                PART_btnRename.IsEnabled = true;

                if (isCustomBar)
                {
                    PART_btnRibbonTabAdd.IsEnabled = true;
                    Btn_DeleteCustomization.IsEnabled = true;
                }
                else
                    Btn_DeleteCustomization.IsEnabled = false;
                var ribbonTab = GetRibbonTab(ribbonTreeView.SelectedItem as RibbonBars);
                if (ribbonTab != null)
                {
                    int itemIndex = ribbonTab.RibbonBars.IndexOf(ribbonTreeView.SelectedItem as RibbonBars);

                    if (itemIndex == 0)
                    {
                        if (ribbonTab.RibbonBars.Count == 1)
                        {
                            PART_btnTabDown.IsEnabled = false;
                            PART_btnTabUp.IsEnabled = false;
                        }
                        else
                        {
                            PART_btnTabDown.IsEnabled = true;
                            PART_btnTabUp.IsEnabled = false;
                        }
                    }
                    else if (itemIndex == ribbonTab.RibbonBars.Count - 1)
                    {
                        PART_btnTabUp.IsEnabled = true;
                        PART_btnTabDown.IsEnabled = false;
                    }
                    else
                    {
                        PART_btnTabDown.IsEnabled = true;
                        PART_btnTabUp.IsEnabled = true;
                    }
                }

            }
            else if (ribbonTreeView.SelectedItem is IRibbonControl)
            {
                Btn_DeleteCustomization.IsEnabled = false;
                PART_btnTabDown.IsEnabled = false;
                PART_btnTabUp.IsEnabled = false;

                foreach (RibbonTabs tab in RibbonTabs)
                {
                    foreach (RibbonBars bar in tab.RibbonBars)
                    {
                        if (bar.IsCustomBar && bar.RibbonControl.Contains(ribbonTreeView.SelectedItem as IRibbonControl))
                        {                           
                            PART_btnRename.IsEnabled = true;

                            int itemIndex = bar.RibbonControl.IndexOf(ribbonTreeView.SelectedItem as IRibbonControl);

                            if (itemIndex == 0)
                            {
                                if (bar.RibbonControl.Count == 1)
                                {
                                    PART_btnTabDown.IsEnabled = false;
                                    PART_btnTabUp.IsEnabled = false;
                                }
                                else
                                {
                                    PART_btnTabDown.IsEnabled = true;
                                    PART_btnTabUp.IsEnabled = false;
                                }
                            }
                            else if (itemIndex == bar.RibbonControl.Count - 1)
                            {
                                PART_btnTabUp.IsEnabled = true;
                                PART_btnTabDown.IsEnabled = false;
                            }
                            else
                            {
                                PART_btnTabDown.IsEnabled = true;
                                PART_btnTabUp.IsEnabled = true;
                            }
                            break;
                        }
                    }
                }

                foreach (RibbonTabs tab in RibbonTabs)
                {
                    foreach (RibbonBars bar in tab.RibbonBars)
                    {
                        if (!bar.IsCustomBar && bar.RibbonControl.Contains(ribbonTreeView.SelectedItem as IRibbonControl))
                        {
                            PART_btnRibbonTabRemove.IsEnabled = false;
                            PART_btnRename.IsEnabled = false;                            
                            return;
                        }
                    }
                }
                PART_btnRibbonTabRemove.IsEnabled = true;
            }
            else
            {
                PART_btnRibbonTabAdd.IsEnabled = false;
                PART_btnRibbonTabRemove.IsEnabled = false;
            }
        }


        void btn_DropDown_IsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                (btn_DropDown.Items[0] as RibbonMenuItem).IsEnabled = false;
                if (ribbonTreeView.SelectedItem is RibbonTabs)
                {
                    var ribbonTabs = ribbonTreeView.SelectedItem as RibbonTabs;

                    if (!ribbonTabs.IsCustomTab)
                    {
                        if (ribbonTabs.IsRenamed)
                        {
                            (btn_DropDown.Items[0] as RibbonMenuItem).IsEnabled = true;
                        }
                        else
                        {
                            foreach (var item in ribbonTabs.RibbonBars)
                            {
                                if (item.IsCustomBar || item.IsPositionChanged || item.IsRenamed)
                                {
                                    (btn_DropDown.Items[0] as RibbonMenuItem).IsEnabled = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }


        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }   

    public class RibbonTabs : INotifyPropertyChanged
    {

        private string _caption;

        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
                OnPropertyChanged("Caption");
            }
        }
       
        public bool IsChecked { get; set; }
        public int TabIndex { get; set; }
        public bool IsCustomTab { get; set; }
        public bool IsPositionChanged { get; set; }

        public bool IsRenamed { get; set; }

        public ObservableCollection<RibbonBars> RibbonBars { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

    public class RibbonBars : INotifyPropertyChanged
    {
        private string _header;

        public string Header
        {
            get
            {
                return _header;
            }
            set
            {
                _header = value;
                OnPropertyChanged("Header");
            }
        }


        public bool IsCustomBar { get; set; }

        public int BarIndex { get; set; }

        public int TabIndex { get; set; }

        public bool IsPositionChanged { get; set; }

        public bool IsRenamed { get; set; }

        public ObservableCollection<IRibbonControl> RibbonControl { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
  

    internal class InternalRibbonBar
    {
        internal int TabIndex { get; set; }
        internal RibbonBars NewBar { get; set; }
    }

    internal class InternalRibbonControl
    {
        internal int TabIndex { get; set; }
        internal int BarIndex { get; set; }
        internal IRibbonControl RibbonControl { get; set; }
        internal int RibbonControlIndex { get; set; }
        internal bool IsPositionChanged { get; set; }
        internal bool IsNextBar { get; set; }
    }

    internal class CustomTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class OpacityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {           
            if (value != null)
            {
                var parentElement = (value as FrameworkElement).Parent;
                if (parentElement is ButtonPanel)
                    parentElement = (parentElement as ButtonPanel).Parent;
                if (parentElement != null && parentElement is RibbonBar)
                {
                    if ((parentElement as RibbonBar).Tag != null)
                        return 1;
                    else
                        return 0.5;
                }
                else
                    return 1;
            }

            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
