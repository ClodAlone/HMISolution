// <copyright file="DocumentContextMenu.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls.Resources;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents document context menu
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DocumentContextMenu : ContextMenu
    {
        #region Private members

        /// <summary>
        /// Represent object reference of Resource wrapper class.
        /// </summary>
        ResourceWrapper wrapper = new ResourceWrapper();

        /// <summary>
        /// Represents separator
        /// </summary>
        private Separator m_separator;
        
        /// <summary>
        /// Represents closeMenuItem
        /// </summary>
        private DocMenuItem m_closeMenuItem;
        
        /// <summary>
        /// Represents maximizeMenuItem
        /// </summary>
        private DocMenuItem m_maximizeMenuItem;
        
        /// <summary>
        /// Represents minimizeMenuItem
        /// </summary>
        private DocMenuItem m_minimizeMenuItem;
        
        /// <summary>
        /// Represents resizeMenuItem
        /// </summary>
        private DocMenuItem m_resizeMenuItem;
        
        /// <summary>
        /// Represents moveMenuItem
        /// </summary>
        private DocMenuItem m_moveMenuItem;
        
        /// <summary>
        /// Represents restoreMenuItem
        /// </summary>
        private DocMenuItem m_restoreMenuItem;
        
        /// <summary>
        /// Element which contains in window menu belongs to.
        /// </summary>
        private UIElement m_element = null;

        /// <summary>
        /// This varaible is a reference to language dictionary.
        /// </summary>
        //private ResourceDictionary langDictionary;

        private ResourceDictionary dictionary;

        MDIWindow window;
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when CustomItemStyle property is changed.
        /// </summary>
        public event PropertyChangedCallback CustomItemStyleChanged;
        
        /// <summary>
        /// Event that is raised when CustomSeparatorStyle property is changed.
        /// </summary>
        public event PropertyChangedCallback CustomSeparatorStyleChanged;
        
        /// <summary>
        /// Event that is raised when IconBrushes property is changed.
        /// </summary>
        public event PropertyChangedCallback IconBrushesChanged;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="DocumentContextMenu"/> class.
        /// </summary>
        static DocumentContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentContextMenu), new FrameworkPropertyMetadata(typeof(DocumentContextMenu)));
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentContextMenu"/> class.
        /// </summary>
        public DocumentContextMenu()
        {
            //langDictionary = new ResourceDictionary();
            //langDictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Themes/LangDictionary.xaml", UriKind.RelativeOrAbsolute);
            dictionary = new ResourceDictionary();
            dictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the value of the CustomItemStyle dependency property.
        /// </summary>
        /// <value>The custom item style.</value>
        public Style CustomItemStyle
        {
            get
            {
                return (Style)GetValue(CustomItemStyleProperty);
            }

            set
            {
                SetValue(CustomItemStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the CustomSeparatorStyle dependency property.
        /// </summary>
        /// <value>The custom separator style.</value>
        public Style CustomSeparatorStyle
        {
            get
            {
                return (Style)GetValue(CustomSeparatorStyleProperty);
            }

            set
            {
                SetValue(CustomSeparatorStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the IconBrushes dependency property.
        /// </summary>
        /// <value>The icon brushes.</value>
        public DictionaryList IconBrushes
        {
            get
            {
                return (DictionaryList)GetValue(IconBrushesProperty);
            }

            set
            {
                SetValue(IconBrushesProperty, value);
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Called when [context menu items changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnContextMenuItemsChanged(object sender, RoutedEventArgs args)
        {
            InitializeCustomContextMenuItems();
        }
        
        /// <summary>
        /// Initializes the custom context menu items.
        /// </summary>
        private void InitializeCustomContextMenuItems()
        {
            DocumentContextMenuItemsCollection collection = DocumentContainer.GetMDIContextMenuItemsCollection(m_element);
            AddCustomItemsExt(collection);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            DefaultItemsFactory();
           
            window = (MDIWindow)TemplatedParent;
            m_element = window.Content;
            InitializeCustomContextMenuItems();
            m_element.AddHandler(DocumentContainer.MDIContextMenuItemsCollectionPropertyChangedEvent, new RoutedEventHandler(OnContextMenuItemsChanged));
        }

        /// <summary>
        /// Raises CustomItemStyleChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCustomItemStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != CustomItemStyleChanged)
            {
                CustomItemStyleChanged(this, e);
            }
        }
        
        /// <summary>
        /// Raises CustomSeparatorStyleChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCustomSeparatorStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != CustomSeparatorStyleChanged)
            {
                CustomSeparatorStyleChanged(this, e);
            }

            if (m_separator != null)
            {
                ResourceDictionary rd = new ResourceDictionary();
                if (SkinStorage.GetVisualStyle(this) == "Office2007Blue")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2007BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                    SkinStorage.SetVisualStyle(m_separator as DependencyObject, "Office2007Blue");
                }
                else if (SkinStorage.GetVisualStyle(this) == "Blend")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/BlendStyle.xaml", UriKind.RelativeOrAbsolute);
                    SkinStorage.SetVisualStyle(m_separator as DependencyObject, "Blend");
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2007Silver")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2007SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                    SkinStorage.SetVisualStyle(m_separator as DependencyObject, "Office2007Silver");
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2007Black")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2007BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                    SkinStorage.SetVisualStyle(m_separator as DependencyObject, "Office2007Black");
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2003")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2003Style.xaml", UriKind.RelativeOrAbsolute);
                    SkinStorage.SetVisualStyle(m_separator as DependencyObject, "Office2003");
                }

                else if (SkinStorage.GetVisualStyle(this) == "SyncOrange")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/SyncOrangeStyle.xaml", UriKind.RelativeOrAbsolute);
                    SkinStorage.SetVisualStyle(m_separator as DependencyObject, "SyncOrange");
                }

                else if (SkinStorage.GetVisualStyle(this) == "ShinyRed")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/ShinyRedStyle.xaml", UriKind.RelativeOrAbsolute);
                    SkinStorage.SetVisualStyle(m_separator as DependencyObject, "ShinyRed");
                }
                else if (SkinStorage.GetVisualStyle(this) == "ShinyBlue")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/ShinyBlueStyle.xaml", UriKind.RelativeOrAbsolute);
                    SkinStorage.SetVisualStyle(m_separator as DependencyObject, "ShinyBlue");
                }
                else if (SkinStorage.GetVisualStyle(this) == "Default")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/vista.aero.xaml", UriKind.RelativeOrAbsolute);
                    SkinStorage.SetVisualStyle(m_separator as DependencyObject, "Default");
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2010Silver")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2010SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                    SkinStorage.SetVisualStyle(m_separator as DependencyObject, "Office2010Silver");
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2010Black")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2010BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                    SkinStorage.SetVisualStyle(m_separator as DependencyObject, "Office2010Black");
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2010Blue")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2010BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                    SkinStorage.SetVisualStyle(m_separator as DependencyObject, "Office2010Blue");
                }
                else if (SkinStorage.GetVisualStyle(this) == "Metro")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/MetroStyle.xaml", UriKind.RelativeOrAbsolute);
                    SkinStorage.SetVisualStyle(m_separator as DependencyObject, "Metro");
                }
                else if (SkinStorage.GetVisualStyle(this) == "Transparent")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/TransparentStyle.xaml", UriKind.RelativeOrAbsolute);
                    SkinStorage.SetVisualStyle(m_separator as DependencyObject, "Transparent");
                }
                m_separator.Style = rd["SeparatorStyle"] as Style;
            }
        }
        
        /// <summary>
        /// Raises IconBrushesChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIconBrushesChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != IconBrushesChanged)
            {
                IconBrushesChanged(this, e);
            }
        }

        /// <summary>
        /// Adds the custom items ext.
        /// </summary>
        /// <param name="collection">The collection.</param>
        private void AddCustomItemsExt(IList<DocMenuItem> collection)
        {
            Items.Clear();
            Items.Add(m_restoreMenuItem);
            Items.Add(m_moveMenuItem);
            Items.Add(m_resizeMenuItem);
            AddCustomItems(collection);
            Items.Add(m_minimizeMenuItem);
            Items.Add(m_maximizeMenuItem);
            Items.Add(m_separator);
            Items.Add(m_closeMenuItem);
        }

        /// <summary>
        /// Adds the custom items.
        /// </summary>
        /// <param name="collection">The collection.</param>
        private void AddCustomItems(IList<DocMenuItem> collection)
        {
            if (null == collection || 0 == collection.Count)
            {
                return;
            }

            DocumentContextMenu parent = (DocumentContextMenu)collection[0].Parent;

            foreach (DocMenuItem item in collection)
            {
                if (null != parent)
                {
                    parent.RemoveLogicalChild(item);
                }

                //if (null == item.Style)
                //{
                //    item.Style = CustomItemStyle;
                //}

                Items.Add(item);
            }
        }
        
        /// <summary>
        /// Gets the menu item factory.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="command">The command.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>DocMenuItem item</returns>
        private DocMenuItem GetMenuItemFactory(String header, ICommand command, String iconbrush)
        {
            DocMenuItem item = new DocMenuItem
            {
                Header = header,
                Command = command,
                CommandTarget = (IInputElement)DataContext,
            };         
					
					
            if (null != dictionary)
            {
                if(iconbrush=="close")
                {
                item.IconBrush = (DrawingBrush)dictionary["brushCloseButtonGray"];
                item.ActiveIconBrush = (DrawingBrush)dictionary["brushCloseButtonGray"];
                item.DisableIconBrush = (DrawingBrush)dictionary["brushCloseButtonGray"];
                }
                 if(iconbrush=="maximize")
                {
                item.IconBrush = (DrawingBrush)dictionary["brushMaximizeButtonGray"];
                item.ActiveIconBrush = (DrawingBrush)dictionary["brushMaximizeButtonGray"];
                item.DisableIconBrush = (DrawingBrush)dictionary["brushMaximizeButtonGray"];
                }              
                
                 if (iconbrush == "minimize")
                {
                item.IconBrush = (DrawingBrush)dictionary["brushMinimizeButtonGray"];
                item.ActiveIconBrush = (DrawingBrush)dictionary["brushMinimizeButtonGray"];
                item.DisableIconBrush = (DrawingBrush)dictionary["brushMinimizeButtonGray"];
                }
                 if (iconbrush == "restore")
                {
                item.IconBrush = (DrawingBrush)dictionary["brushRestoreButtonGray"];
                item.ActiveIconBrush = (DrawingBrush)dictionary["brushRestoreButtonGray"];
                item.DisableIconBrush = (DrawingBrush)dictionary["brushRestoreButtonGray"];
                }
            }

            return item;
        }

        /// <summary>
        /// Defaults the items factory.
        /// </summary>
        private void DefaultItemsFactory()
        {
            m_restoreMenuItem = GetMenuItemFactory(wrapper.MDIRestore, DocumentContainer.RestoreDocumentCommand, "restore"); //GetMenuItemFactory(langDictionary["MDIRestore"] as string, DocumentContainer.RestoreDocumentCommand, "restore");
            m_moveMenuItem = GetMenuItemFactory(wrapper.MDIMove, DocumentContainer.BeginDocumentMovingCommand, "move"); //GetMenuItemFactory(langDictionary["MDIMove"] as string, DocumentContainer.BeginDocumentMovingCommand, "move");
            m_resizeMenuItem = GetMenuItemFactory(wrapper.MDIResize  , DocumentContainer.BeginDocumentResizingCommand, "resize");//GetMenuItemFactory(langDictionary["MDIResize"] as string, DocumentContainer.BeginDocumentResizingCommand, "resize");
            m_minimizeMenuItem = GetMenuItemFactory(wrapper.MDIMinimize, DocumentContainer.MinimizeDocumentCommand, "minimize");//GetMenuItemFactory(langDictionary["MDIMinimize"] as string, DocumentContainer.MinimizeDocumentCommand, "minimize");
            m_maximizeMenuItem = GetMenuItemFactory(wrapper.MDIMaximize, DocumentContainer.MaximizeDocumentCommand, "maximize");//GetMenuItemFactory(langDictionary["MDIMaximize"] as string, DocumentContainer.MaximizeDocumentCommand, "maximize");
            m_closeMenuItem = GetMenuItemFactory(wrapper.MDIClose, DocumentContainer.HideDocumentCommand, "close");//GetMenuItemFactory(langDictionary["MDIClose"] as string, DocumentContainer.HideDocumentCommand, "close");
                    
          

            m_separator = new Separator
            {
                Style = CustomSeparatorStyle,
            };
        }

        /// <summary>
        /// Calls OnCustomItemStyleChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCustomItemStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContextMenu instance = (DocumentContextMenu)d;
            instance.OnCustomItemStyleChanged(e);
        }
        
        /// <summary>
        /// Calls OnCustomSeparatorStyleChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCustomSeparatorStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContextMenu instance = (DocumentContextMenu)d;
            instance.OnCustomSeparatorStyleChanged(e);
        }
        /// <summary>
        /// Calls OnIconBrushesChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIconBrushesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContextMenu instance = (DocumentContextMenu)d;
            instance.OnIconBrushesChanged(e);
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Presents a style for custom items.
        /// </summary>
        public static readonly DependencyProperty CustomItemStyleProperty = DependencyProperty.Register("CustomItemStyle", typeof(Style), typeof(DocumentContextMenu), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCustomItemStyleChanged)));
        
        /// <summary>
        /// Presents a style for custom separators.
        /// </summary>
        public static readonly DependencyProperty CustomSeparatorStyleProperty = DependencyProperty.Register("CustomSeparatorStyle", typeof(Style), typeof(DocumentContextMenu), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCustomSeparatorStyleChanged)));
        
        /// <summary>
        /// Presents icon brushes list.
        /// </summary>
        public static readonly DependencyProperty IconBrushesProperty = DependencyProperty.Register("IconBrushes", typeof(DictionaryList), typeof(DocumentContextMenu), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnIconBrushesChanged)));
        #endregion
    }
}