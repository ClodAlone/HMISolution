using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ScreenManager.ComponentService;
using ScreenSettings.Entities;
using System.Windows.Controls.Primitives;
using Utilities;
using Utilities.WPF;
using UFInterfaces.Editors;
using DevExpress.Xpf.Core;
using System.Collections.Generic;

namespace ScreenManager.Popups
{
    /// <summary>
    /// Interaction logic for SmartProperties.xaml
    /// </summary>
    public partial class SmartProperties : UserControl, IDisposable
    {
        readonly ScreenManagerComponent EditorComponent;
        readonly new DocumentManager.ComponentService.IDocument Parent;
        public ScreenEntity Entity { get; set; }
        public int SelectedTab { get; set; }

        FrameworkElement content;
        FrameworkElement element;
        public FrameworkElement Element
        {
            get
            {
                return element;
            }
            set
            {
                if (element == value)
                    return;

                element = value;

                if (element is ContentControl && !(element is UserControl) &&
                    (element as ContentControl).Content is UIElement)
                {
                    content = (element as ContentControl).Content as FrameworkElement;
                }
                else
                    content = null;
            }
        }
        public FrameworkElement ContentElement
        {
            get
            {
                return content ?? Element;
            }
        }

        bool bLoaded;
        bool bChangingFont;
        bool bSelectingFont;
        public string startText;
        public TextProperties textEditor;
        Window Owner;
        DXTabItem tabItemCommandor;
        DXTabItem tabItemAnimator;

        public SmartProperties(ScreenManagerComponent editorComponent, ScreenEntity entity, 
            FrameworkElement fe, DocumentManager.ComponentService.IDocument parent)
        {
            Element = fe;
            Entity = entity;
            Parent = parent;

            EditorComponent = editorComponent;
            InitializeComponent();

            Loaded += (o, e) =>
                {
                    bLoaded = true;
                    SelectActiveTab();
                    name.Visibility = DataContext != null ? Visibility.Visible : Visibility.Collapsed;
                    toolbar.Visibility = DataContext == null ? Visibility.Visible : Visibility.Collapsed;
                    Owner = this.FindParent<Window>();
                    if (Owner != null)
                        Owner.Closing += OnParentWindowClosing;
                };
            Unloaded += (o, e) =>
                {
                    bLoaded = false;
                    if (Owner != null)
                        Owner.Closing -= OnParentWindowClosing;
                };

            tabItemCommandor = new DXTabItem()
            {
                Header = Properties.Resources.SmartPropCommands,
                IsSelected = false
            };
            tabItemAnimator = new DXTabItem()
            {
                Header = Properties.Resources.SmartPropAnimations,
                IsSelected = false
            };
            var tabItemText = new DXTabItem()
            {
                Header = Properties.Resources.SmartPropText,
                IsSelected = true
            };
            var tabItemProperties = new DXTabItem()
            {
                Header = Properties.Resources.SmartPropProperties,
                IsSelected = false
            };
            var tabItemItemControlSource = new DXTabItem()
            {
                Header = Properties.Resources.SmartPropItemsSource,
                IsSelected = false
            };
            var tabItem3D = new DXTabItem()
            {
                Header = Properties.Resources.SmartProp3D,
                IsSelected = false
            };
            var tabItemEffects = new DXTabItem()
            {
                Header = Properties.Resources.SmartPropEffects,
                IsSelected = false
            };
            var tabInnerScreen = new DXTabItem()
            {
                Header = Properties.Resources.SmartProp3DInnerScreen,
                IsSelected = false
            };
        
            tabControl.SelectionChanged += (o, e) =>
                {
                    using (new WaitCursor())
                    {
                        if (e.NewSelectedItem == tabItemText)
                        {
                            if (tabItemText.Content == null)
                            {
                                bSelectingFont = true;
                                var textProp = new TextProperties(EditorComponent, Parent)
                                {
                                    DataContext = ContentElement
                                };
                                textEditor = textProp;
                                startText = textProp.startText;

                                 textProp.fontSizeBox.SelectionChanged += (ob, ev) =>
                                {
                                    if (!bSelectingFont && textProp.fontSizeBox.SelectedValue != null && textProp.fontSizeBox.SelectedValue is ComboBoxItem)
                                        textProp.OnFontSizeSelectionChanged(Convert.ToInt16((textProp.fontSizeBox.SelectedValue as ComboBoxItem).Content.ToString()));
                                };

                                textProp.fontSizeBox.PreviewKeyDown += (ob, ev) =>
                                {
                                    bChangingFont = true;
                                };

                                textProp.fontSizeBox.AddHandler(TextBoxBase.TextChangedEvent,
                                    new TextChangedEventHandler(fontSizeBox_TextChanged));


                                textProp.fontNameBox.SelectionChanged += (ob, ev) =>
                                {
                                    if (!bSelectingFont && textProp.fontNameBox.SelectedValue != null && textProp.fontNameBox.SelectedValue is ComboBoxItem)
                                        textProp.OnFontNameSelectionChanged((textProp.fontNameBox.SelectedValue as ComboBoxItem).Content as FontFamily);
                                };

                                textProp.fontStyleBox.SelectionChanged += (ob, ev) =>
                                {
                                    if (!bSelectingFont && textProp.fontStyleBox.SelectedValue != null && textProp.fontStyleBox.SelectedValue is ComboBoxItem)
                                        textProp.OnFontStyleSelectionChanged((FontStyle)(textProp.fontStyleBox.SelectedValue as ComboBoxItem).Content);
                                };

                                textProp.fontWeightBox.SelectionChanged += (ob, ev) =>
                                {
                                    if (!bSelectingFont && textProp.fontWeightBox.SelectedValue != null && textProp.fontWeightBox.SelectedValue is ComboBoxItem)
                                        textProp.OnFontWeightSelectionChanged((FontWeight)(textProp.fontWeightBox.SelectedValue as ComboBoxItem).Content);
                                };

                                //textProp.fontSizeBox.PreviewKeyUp += (ob, ev) =>
                                //{
                                //    if (!bSelectingFont && !string.IsNullOrEmpty(textProp.fontSizeBox.Text))
                                //        try
                                //        {
                                //            textProp.OnFontSizeSelectionChanged(Convert.ToInt16(textProp.fontSizeBox.Text));
                                //        }
                                //        catch (Exception)
                                //        {
                                //        }
                                //};

                                textProp.toggleU1.Checked += (ob, ev) =>
                                {
                                    var newValue = textProp.toggleU1.IsChecked.HasValue && textProp.toggleU1.IsChecked.Value;
                                    Entity.TagDecorators = BitOperations.SetBitValue((byte)TextDecorators.Underline, entity.TagDecorators, newValue);
                                };

                                textProp.ClearValue(FrameworkElement.WidthProperty);
                                textProp.ClearValue(FrameworkElement.HeightProperty);

                                tabItemText.Content = textProp;
                                bSelectingFont = false;
                            }
                            else
                            {
                                bSelectingFont = true;
                                (tabItemText.Content as FrameworkElement).DataContext = ContentElement;
                                bSelectingFont = false;
                            }
                        }
                        else if (e.NewSelectedItem == tabItemProperties)
                        {
                            if (tabItemProperties.Content == null && EditorComponent.PropertyControl != null)
                            {
                                var propertyControl = EditorComponent.PropertyControl.control;
                                propertyControl.ClearValue(FrameworkElement.WidthProperty);
                                propertyControl.ClearValue(FrameworkElement.HeightProperty);

                                tabItemProperties.Content = propertyControl;
                            }
                        }
                        else if (e.NewSelectedItem == tabItemItemControlSource)
                        {
                            if (tabItemItemControlSource.Content == null)
                            {
                                var itemControlSourceProp = new ItemControlSourceProperties(EditorComponent, Parent)
                                {
                                    DataContext = Entity
                                };

                                itemControlSourceProp.ClearValue(FrameworkElement.WidthProperty);
                                itemControlSourceProp.ClearValue(FrameworkElement.HeightProperty);

                                tabItemItemControlSource.Content = itemControlSourceProp;
                            }
                            else
                                (tabItemItemControlSource.Content as FrameworkElement).DataContext = Entity;
                        }
                        else if (e.NewSelectedItem == tabItemCommandor)
                        {
                            if (tabItemCommandor.Content == null)
                            {
                                var commandor = EditorComponent.CommandExplorer.control;
                                commandor.ClearValue(FrameworkElement.WidthProperty);
                                commandor.ClearValue(FrameworkElement.HeightProperty);

                                commandor.DataContext = Entity;
                                tabItemCommandor.Content = commandor;
                            }
                            else
                                (tabItemCommandor.Content as FrameworkElement).DataContext = Entity;
                        }
                        else if (e.NewSelectedItem == tabItemAnimator)
                        {
                            if (tabItemAnimator.Content == null)
                            {
                                var animator = EditorComponent.AnimationExplorer.control;
                                animator.ClearValue(FrameworkElement.WidthProperty);
                                animator.ClearValue(FrameworkElement.HeightProperty);

                                animator.DataContext = Entity;
                                tabItemAnimator.Content = animator;
                            }
                            else
                                (tabItemAnimator.Content as FrameworkElement).DataContext = Entity;
                        }
                        else if (e.NewSelectedItem == tabItem3D)
                        {
                            if (tabItem3D.Content == null)
                            {
                                _3DTransformSliders efx3D = new _3DTransformSliders
                                {
                                    DataContext = Element
                                };

                                efx3D.ClearValue(FrameworkElement.WidthProperty);
                                efx3D.ClearValue(FrameworkElement.HeightProperty);

                                tabItem3D.Content = efx3D;
                            }
                            else
                                (tabItem3D.Content as FrameworkElement).DataContext = Element;
                        }
                        else if (e.NewSelectedItem == tabInnerScreen)
                        {
                            if (tabInnerScreen.Content == null)
                            {
                                _3DInnerScreen innerScreen = new _3DInnerScreen(EditorComponent)
                                {
                                    DataContext = Entity
                                };

                                innerScreen.ClearValue(FrameworkElement.WidthProperty);
                                innerScreen.ClearValue(FrameworkElement.HeightProperty);

                                tabInnerScreen.Content = innerScreen;
                            }
                            else
                                (tabInnerScreen.Content as FrameworkElement).DataContext = Entity;
                        }
                        else if (e.NewSelectedItem == tabItemEffects)
                        {
                            if (tabItemEffects.Content == null)
                            {
                                if (EditorComponent.PropertyControl != null)
                                {
                                    EffectSettings efx = new EffectSettings(EditorComponent.PropertyControl)
                                    {
                                        DataContext = Element
                                    };
                                    efx.ClearValue(FrameworkElement.WidthProperty);
                                    efx.ClearValue(FrameworkElement.HeightProperty);

                                    tabItemEffects.Content = efx;
                                }
                            }
                            else
                                (tabItemEffects.Content as FrameworkElement).DataContext = Element;
                        }
                    }
                };


            DataContextChanged += (o, e) =>
                {
                    name.Text = Element.Name;

                    FrameworkElement subElement = Element;
                    if (Entity.SourceSymbolLinked && 
                        Element is ContentControl && !(Element is UserControl) && (Element as ContentControl).Content is FrameworkElement)
                    {
                        subElement = (Element as ContentControl).Content as FrameworkElement;
                    }

                    tabControl.UpdateLayout();
                    tabControl.BeginInit();
                    List<Viewport3D> listViewport3Ds = new List<Viewport3D>();
                    if (SelectedTab == 1)
                    {
                        if (!tabControl.Items.Contains(tabItemAnimator))
                            tabControl.Items.Add(tabItemAnimator);
                    }
                    else if (SelectedTab == 2)
                    {
                        if (!tabControl.Items.Contains(tabItemCommandor))
                            tabControl.Items.Add(tabItemCommandor);
                    }
                    else if (SelectedTab == 3)
                    {
                        Type t = subElement.GetType();
                        if (t.GetProperty("ItemsSource") != null || t.GetProperty("DataSource") != null)
                        // if (fe is ItemsControl)
                        {
                            if (!tabControl.Items.Contains(tabItemItemControlSource))
                                tabControl.Items.Add(tabItemItemControlSource);
                        }
                        else if (tabControl.Items.Contains(tabItemItemControlSource))
                        {
                            if (tabItemItemControlSource.IsSelected)
                                tabItemText.IsSelected = true;
                            tabControl.Items.Remove(tabItemItemControlSource);
                        }
                    }
                    else
                    {
                        if (!tabControl.Items.Contains(tabItemText))
                            tabControl.Items.Add(tabItemText);
                        if (!tabControl.Items.Contains(tabItemProperties))
                            tabControl.Items.Add(tabItemProperties);

                        Type t = subElement.GetType();
                        if (t.GetProperty("ItemsSource") != null || t.GetProperty("DataSource") != null)
                        // if (fe is ItemsControl)
                        {
                            if (!tabControl.Items.Contains(tabItemItemControlSource))
                                tabControl.Items.Add(tabItemItemControlSource);
                        }
                        else if (tabControl.Items.Contains(tabItemItemControlSource))
                        {
                            if (tabItemItemControlSource.IsSelected)
                                tabItemText.IsSelected = true;
                            tabControl.Items.Remove(tabItemItemControlSource);
                        }

                        // if (subElement is ICommandSource && !(subElement is ToggleButton) || Entity.Element3D != null)
                        {
                            if (!tabControl.Items.Contains(tabItemCommandor))
                                tabControl.Items.Add(tabItemCommandor);
                        }
                        /*
                        else if (tabControl.Items.Contains(tabItemCommandor))
                        {
                            if (tabItemCommandor.IsSelected)
                                tabItemText.IsSelected = true;
                            tabControl.Items.Remove(tabItemCommandor);
                        }
                        */

                        if (!tabControl.Items.Contains(tabItemAnimator))
                            tabControl.Items.Add(tabItemAnimator);

                        if (tabItemProperties.IsSelected == true && tabItemProperties.Content != null && 
                            EditorComponent.PropertyControl != null)
                        {
                            //var propertyControl = EditorComponent.PropertyControl.control;
                            //propertyControl.ClearValue(FrameworkElement.WidthProperty);
                            //propertyControl.ClearValue(FrameworkElement.HeightProperty);

                            //tabItemProperties.Content = propertyControl;
                            EditorComponent.PropertyControl.SetControlSelection(tabItemProperties.Content as UserControl, Entity);
                        }

                        if (Entity.Element3D != null)
                        {
                            if (!tabControl.Items.Contains(tabInnerScreen))
                                tabControl.Items.Add(tabInnerScreen);
                        }
                        else if (tabControl.Items.Contains(tabInnerScreen))
                        {
                            if (tabInnerScreen.IsSelected)
                                tabItemText.IsSelected = true;
                            tabControl.Items.Remove(tabInnerScreen);
                        }

                        listViewport3Ds = subElement.GetChildrenOfType<Viewport3D>().ToList();
                        if (subElement is Viewport3D)
                            listViewport3Ds.Add(subElement as Viewport3D);
                        if (listViewport3Ds.Count > 0)
                        {
                            if (!tabControl.Items.Contains(tabItem3D))
                                tabControl.Items.Add(tabItem3D);
                        }
                        else if (tabControl.Items.Contains(tabItem3D))
                        {
                            if (tabItem3D.IsSelected)
                                tabItemText.IsSelected = true;
                            tabControl.Items.Remove(tabItem3D);
                        }

                        if (!tabControl.Items.Contains(tabItemEffects) && EditorComponent.PropertyControl != null)
                            tabControl.Items.Add(tabItemEffects);
                    }


                    // tabControl.SelectedIndex = 0;
                    tabControl.EndInit();
                    bSelectingFont = true;

                    foreach (DXTabItem tab in tabControl.Items)
                    {
                        if (tab.Content is FrameworkElement)
                        {
                            var element = tab.Content as FrameworkElement;
                            if (element is TextProperties)
                                element.DataContext = ContentElement;
                            else if (element is EffectSettings)
                                element.DataContext = DataContext;
                            else if (element is _3DTransformSliders)
                            {
                                if (listViewport3Ds.Count > 0)
                                    element.DataContext = listViewport3Ds[0];
                                else
                                    element.DataContext = DataContext;
                            }
                            else
                                element.DataContext = Entity;
                        }
                    }
                    bSelectingFont = false;

                    // tabItemText.IsSelected = true;
                };

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    tabControl.SelectedIndex = 1;
                    tabControl.SelectedIndex = 0;
                });
        }

        void SelectActiveTab()
        {
            switch (SelectedTab)
            {
                case 1:
                    foreach (DXTabItem item in tabControl.Items)
                    {
                        if (item != tabItemAnimator)
                        {
                            item.Visibility = Visibility.Collapsed;
                        }
                        tabItemAnimator.IsSelected = true;
                    }
                    break;
                case 2:
                    foreach (DXTabItem item in tabControl.Items)
                    {
                        if (item != tabItemCommandor)
                        {
                            item.Visibility = Visibility.Collapsed;
                        }
                        tabItemCommandor.IsSelected = true;
                    }
                    break;
                default:
                    foreach (DXTabItem item in tabControl.Items)
                    {
                        item.Visibility = Visibility.Visible;
                    }
                    break;
            }
        }

        void OnParentWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var dlg = sender as GeneralDialog;
            if (dlg?.DialogResult != true)
            {
                RestoreTextEditorText();
            }
            else
            {
                var parent = Parent as IDocumentTranslator;
                if (ContentElement != null && parent != null && !parent.IsTranslated(ContentElement))
                    (Parent.ActiveView as ScreenEditorView)?.UpdateStringIdAndTranslate(ContentElement, true);
            }
        }

        public void RestoreTextEditorText()
        {
            if (textEditor != null && startText != null)
                textEditor.TextEditor.Text = startText;
        }

        private void fontWeightBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bChangingFont && !bSelectingFont && (sender as ComboBox).SelectedItem != null)
                try
                {
                    bChangingFont = false;
                    var textProp = (sender as ComboBox).FindParent<TextProperties>();
                    textProp.OnFontWeightSelectionChanged((FontWeight)(sender as ComboBox).SelectedItem);
                }
                catch (Exception)
                {
                }
        }

        private void fontStyleBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bChangingFont && !bSelectingFont && (sender as ComboBox).SelectedItem != null)
                try
                {
                    bChangingFont = false;
                    var textProp = (sender as ComboBox).FindParent<TextProperties>();
                    textProp.OnFontStyleSelectionChanged((FontStyle)(sender as ComboBox).SelectedItem);
                }
                catch (Exception)
                {
                }
        }

        private void fontNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bChangingFont && !bSelectingFont && (sender as ComboBox).SelectedItem != null)
                try
                {
                    bChangingFont = false;
                    var textProp = (sender as ComboBox).FindParent<TextProperties>();
                    textProp.OnFontNameSelectionChanged((sender as ComboBox).SelectedItem as FontFamily);
                }
                catch (Exception)
                {
                }
        }


        private void fontSizeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bChangingFont && !bSelectingFont && !string.IsNullOrEmpty((sender as ComboBox).Text))
                try
                {
                    bChangingFont = false;
                    var textProp = (sender as ComboBox).FindParent<TextProperties>();
                    textProp.OnFontSizeSelectionChanged(Convert.ToInt16((sender as ComboBox).Text));
                }
                catch (Exception)
                {
                }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            var Owner = this.FindAncestor<Popup>();
            if (Owner == null)
                return;
            Owner.IsOpen = false;
        }
        public void Dispose()
        {
            foreach (DXTabItem item in tabControl.Items)
            {
                if (item.Content is TextProperties)
                {
                    try
                    {
                        var textProp = item.Content as TextProperties;
                        textProp.fontSizeBox.RemoveHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                                              new System.Windows.Controls.TextChangedEventHandler(fontSizeBox_TextChanged));
                    }
                    catch (Exception)
                    {
                        
                    }
                }

                if (item.Content is IDisposable)
                    (item.Content as IDisposable).Dispose();
            }
            tabControl.Items.Clear();
            tabControl.Dispose();
        }
    }
}
