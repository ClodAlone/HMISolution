using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Converters;
using ScreenManager.ComponentService;
using Utilities;
using Utilities.WPF;
using WPFUtilities;
namespace ScreenManager.Popups
{
    /// <summary>
    /// Interaction logic for TextProperties.xaml
    /// </summary>
    public partial class TextProperties : UserControl
    {
        readonly ScreenManagerComponent EditorComponent;
        readonly new DocumentManager.ComponentService.IDocument Parent;
        public string startText;

        public TextProperties(ScreenManagerComponent c, DocumentManager.ComponentService.IDocument parent)
        {
            InitializeComponent();

            EditorComponent = c;
            Parent = parent;

            FontStyleHelper.InitializeFontComboBox(fontNameBox);
            FontStyleHelper.InitializeFontSizeComboBox(fontSizeBox);
            FontStyleHelper.InitializeFontStyleComboBox(fontStyleBox);
            FontStyleHelper.InitializeFontWeightComboBox(fontWeightBox);
        }

        //private static void InitializeFontComboBox(ComboBox r_Combo)
        //{
        //    var list = (from c in Fonts.SystemFontFamilies
        //                orderby c.Source
        //                select c).ToList();
        //    foreach (FontFamily fontFamily in list)
        //    {
        //        ComboBoxItem item = new ComboBoxItem { Content = fontFamily };
        //        item.FontFamily = fontFamily;
        //        r_Combo.Items.Add(item);
        //    }
        //}

        //private static void InitializeFontSizeComboBox(ComboBox r_combo)
        //{
        //    int[] sizes = new int[27] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 28, 36, 48, 72, 82, 92, 102, 112, 122, 132, 142, 160, 180, 200, 220, 240 };
        //    for (int i = 0, cnt = sizes.Length; i < cnt; ++i)
        //    {
        //        ComboBoxItem item = new ComboBoxItem { Content = sizes[i] };
        //        r_combo.Items.Add(item);
        //    }
        //}

        internal void OnFontSizeSelectionChanged(int fontSize)
        {
            var element = DataContext as Control;
            if (element != null)
            {
                List<Control> listControl = (from c in element.GetChildrenOfType<Control>()
                                             where !(c.ReadLocalValue(Control.FontSizeProperty) is BindingExpression)
                                             select c).ToList();
                if (!(element.ReadLocalValue(Control.FontSizeProperty) is BindingExpression))
                    listControl.Add(element);
                listControl.ForEach(control =>
                {
                   control.FontSize = fontSize;
                });

                //List<Control> listFontSettingsControl = (from control in element.GetChildrenOfType<Control>() 
                //                   where (!(control is HeaderedContentControl) &&
                //        control is ContentControl && !((control as ContentControl).Content is String)) select control).ToList();

                //if (!(element is HeaderedContentControl) &&
                //        element is ContentControl && !((element as ContentControl).Content is String))
                //    listFontSettingsControl.Add(element);
                //listFontSettingsControl.ForEach(control =>
                //{
                //        var t = control.GetType();
                //        var listProperty = GetCurrentPropertyList(t);
                //        if (listProperty.Count > 0)
                //        {
                //            listProperty.ForEach(X =>
                //            {
                //                p = t.GetProperty(X.Name);
                //                fs = (p.GetValue((UIElement)control) as FontSettings).Clone();
                //                fs.FontSize = fontSize;
                //                p.SetValue((UIElement)control, fs);
                //            });
                //        }
                //});            
            }
        }
        internal void OnFontStyleSelectionChanged(FontStyle fontStyle)
        {
            if (fontStyle == null)
                return;

            var element = DataContext as Control;
            if (element != null)
            {
                List<Control> listControl = (from c in element.GetChildrenOfType<Control>()
                                             where !(c.ReadLocalValue(Control.FontStyleProperty) is BindingExpression)
                                             select c).ToList();
                if (!(element.ReadLocalValue(Control.FontStyleProperty) is BindingExpression))
                    listControl.Add(element);
                listControl.ForEach(control =>
                {
                    control.FontStyle = fontStyle;
                });
            }
        }

        internal void OnFontWeightSelectionChanged(FontWeight fontWeight)
        {
            if (fontWeight == null)
                return;

            var element = DataContext as Control;
            if (element != null)
            {
                List<Control> listControl = (from c in element.GetChildrenOfType<Control>()
                                             where !(c.ReadLocalValue(Control.FontWeightProperty) is BindingExpression)
                                             select c).ToList();
                if (!(element.ReadLocalValue(Control.FontWeightProperty) is BindingExpression))
                    listControl.Add(element);
                listControl.ForEach(control =>
                {
                    control.FontWeight = fontWeight;
                });
            }
        }

        internal void OnFontNameSelectionChanged(FontFamily fontFamily)
        {
            if (fontFamily == null)
                return;

            var element = DataContext as Control;
            if (element != null)
            {
                List<Control> listControl = (from c in element.GetChildrenOfType<Control>()
                                             where !(c.ReadLocalValue(Control.FontFamilyProperty) is BindingExpression)
                                             select c).ToList();
                if (!(element.ReadLocalValue(Control.FontFamilyProperty) is BindingExpression))
                    listControl.Add(element);
                listControl.ForEach(control =>
                {
                  control.FontFamily = fontFamily;
                });

                //List<Control> listFontSettingsControl = (from control in element.GetChildrenOfType<Control>() 
                //                   where (!(control is HeaderedContentControl) &&
                //        control is ContentControl && !((control as ContentControl).Content is String)) select control).ToList();

                //if (!(element is HeaderedContentControl) &&
                //        element is ContentControl && !((element as ContentControl).Content is String))
                //    listFontSettingsControl.Add(element);
                //listFontSettingsControl.ForEach(control =>
                //{
                //        var t = control.GetType();
                //        var listProperty = GetCurrentPropertyList(t);
                //        if (listProperty.Count > 0)
                //        {
                //            listProperty.ForEach(X =>
                //            {
                //                p = t.GetProperty(X.Name);
                //                fs = (p.GetValue((UIElement)control) as FontSettings).Clone();
                //                fs.FontFamily = fontFamily;
                //                p.SetValue((UIElement)control, fs);
                //            });
                //        }
                //});            
            }
        }

        List<ContentControl> currentList;
        private void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (currentList == null)
                return;

            currentList.ForEach(child =>
            {
                if (child.Content is String)
                {
                    var text = TextEditor.Text.Replace("\\n", System.Environment.NewLine);
                    child.Content = text;
                }
            });
        }

        PropertyInfo p;
        // FontSettings fs;
        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext != null)
            {
                var element = DataContext as Control;
                toggleU1.IsChecked = false;
                toggleU2.IsChecked = false;
                bool bUnderline = false;

                if (element != null)
                {
                    var listControl = element.GetVisualChildrenOfType<TextBlock>().ToList();
                    if (listControl.Count == 0 && DataContext is TextBlock)
                        listControl.Add(DataContext as TextBlock);
                    var listControlBoxes = element.GetVisualChildrenOfType<TextBox>().ToList();
                    if (listControlBoxes.Count == 0 && DataContext is TextBox)
                        listControlBoxes.Add(DataContext as TextBox);

                   
                    listControl.ForEach(control =>
                    {
                        if (control.TextDecorations != null)
                        {
                            foreach (var item in control.TextDecorations)
                            {
                                if (item.Location == TextDecorationLocation.Underline)
                                {
                                    bUnderline = true;
                                    break;
                                }
                            }
                        }
                    });

                    listControlBoxes.ForEach(control =>
                    {
                        if (control.TextDecorations != null)
                        {
                            foreach (var item in control.TextDecorations)
                            {
                                if (item.Location == TextDecorationLocation.Underline)
                                {
                                    bUnderline = true;
                                    break;
                                }
                            }
                        }
                    });

                    toggleU2.IsChecked = toggleU1.IsChecked = bUnderline;
                }

                if (TextEditor.TextDecorations != null)
                {
                    var textDecorations = new TextDecorationCollection(TextEditor.TextDecorations);
                    foreach (var item in textDecorations)
                    {
                        if (item.Location == TextDecorationLocation.Underline)
                            TextEditor.TextDecorations.Remove(item);
                    }

                    if (bUnderline)
                    {
                        foreach (var item in TextDecorations.Underline)
                            TextEditor.TextDecorations.Add(item);
                    }
                }

                currentList = null;

                Type t = DataContext.GetType();
                p = t.GetProperty("Text");
                if (p != null && !(DataContext is ComboBox) && !(DataContext is ListBox))
                {
                    gridText.Visibility = Visibility.Visible;
                    gridText1.Visibility = Visibility.Visible;
                    gridText2.Visibility = Visibility.Collapsed;
                    MainGrid.RowDefinitions[0].Height = new GridLength(4.0, GridUnitType.Star);
                    //var binding = new Binding()
                    //{
                    //    Path = new PropertyPath("Text")
                    //};

                    //TextEditor.SetBinding(TextBox.TextProperty, binding);
                    TextEditor.Text = p.GetValue(DataContext) as String;
                    TextEditor.TextChanged += (o, ev) =>
                    {
                        if (p != null)
                        {
                            var text = TextEditor.Text.Replace("\\n", System.Environment.NewLine);
                            p.SetValue(DataContext, text);
                        }
                    };
                    startText = TextEditor.Text;
                    return;
                }
                if (DataContext is ContentControl && (DataContext as ContentControl).Content is String)
                {
                    gridText.Visibility = Visibility.Visible;
                    gridText1.Visibility = Visibility.Visible;
                    gridText2.Visibility = Visibility.Collapsed;
                    MainGrid.RowDefinitions[0].Height = new GridLength(4.0, GridUnitType.Star);

                    TextEditor.Text = (DataContext as ContentControl).Content as String;
                    TextEditor.TextChanged += (o, ev) =>
                        {
                            var contentControl = DataContext as ContentControl;
                            if (contentControl != null && contentControl.Content is String)
                            {
                                var text = TextEditor.Text.Replace("\\n", System.Environment.NewLine);
                                contentControl.Content = text;
                            }
                        };
                    startText = TextEditor.Text;
                    return;
                }

                if (!(DataContext is HeaderedContentControl) &&
                    DataContext is ContentControl && !((DataContext as ContentControl).Content is String))
                {
                    //var listProperty = GetCurrentPropertyList(t);
                    //if (listProperty.Count > 0)
                    //{
                    //    var prop = t.GetProperty(listProperty[0].Name);
                    //    fs = prop.GetValue(DataContext) as FontSettings;
                    //    fontNameBox.DataContext = fs;
                    //    fontNameBox.Text = fs.FontFamily.Source;
                    //    fontSizeBox.DataContext = fs;
                    //    fontSizeBox.Text = fs.FontSize.ToString();
                    //}

                    UIElement ue = (UIElement)DataContext;

                    currentList = (from c in ue.GetVisualChildrenOfType<ContentControl>()
                      where c.Content is String && c.Visibility == System.Windows.Visibility.Visible
                        select c).ToList();
                    if (currentList.Count != 1)
                    {
                        gridText.Visibility = Visibility.Collapsed;
                        gridText1.Visibility = Visibility.Collapsed;
                        gridText2.Visibility = Visibility.Visible;
                        MainGrid.RowDefinitions[0].Height = new GridLength(0.0, GridUnitType.Auto);
                        return;
                    }
                    currentList.ForEach(child =>
                     {
                         gridText.Visibility = Visibility.Visible;
                         gridText1.Visibility = Visibility.Visible;
                         gridText2.Visibility = Visibility.Collapsed;
                         MainGrid.RowDefinitions[0].Height = new GridLength(4.0, GridUnitType.Star);
                         TextEditor.Text = child.Content as String;
                         startText = TextEditor.Text;
                         return;
                     });
                    return;
                }

                if (DataContext is HeaderedContentControl && 
                    ((DataContext as HeaderedContentControl).Header is String ||
                     (DataContext as HeaderedContentControl).Header == null))
                {
                    gridText.Visibility = Visibility.Visible;
                    gridText1.Visibility = Visibility.Visible;
                    gridText2.Visibility = Visibility.Collapsed;
                    MainGrid.RowDefinitions[0].Height = new GridLength(4.0, GridUnitType.Star);
                    TextEditor.Text = (DataContext as HeaderedContentControl).Header as String;
                    TextEditor.TextChanged += (o, ev) =>
                    {
                        var headerControl = DataContext as HeaderedContentControl;
                        if (headerControl != null)
                        {
                            var text = TextEditor.Text.Replace("\\n", System.Environment.NewLine);
                            headerControl.Header = text;
                        }
                    };
                    startText = TextEditor.Text;
                    return;
                }
            }

            gridText.Visibility = Visibility.Collapsed;
            gridText1.Visibility = Visibility.Collapsed;
            gridText2.Visibility = Visibility.Visible;
            MainGrid.RowDefinitions[0].Height = new GridLength(0.0, GridUnitType.Auto);
        }
        
        private List<DependencyPropertyDescriptor>  GetCurrentPropertyList(Type t)
        {
            try
            {
                var listP = PropertyChangeNotifier.GetPropertyList(t);
                var listPD = (from c in listP.OfType<PropertyDescriptor>() select DependencyPropertyDescriptor.FromProperty(c)).Where(x => x != null && !x.IsReadOnly && x.PropertyType == typeof(FontSettings)).ToList();
                return listPD;
            }
            catch{
                return new List<DependencyPropertyDescriptor>();
            }
        }

        private void OnToggleBold(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var element = DataContext as Control;
            if (element != null)
            {
                List<Control> listControl = (from c in element.GetChildrenOfType<Control>()
                                             where !(c.ReadLocalValue(Control.FontWeightProperty) is BindingExpression)
                                             select c).ToList();
                if (!(element.ReadLocalValue(Control.FontSizeProperty) is BindingExpression))
                    listControl.Add(element);
                listControl.ForEach(control =>
                {
                    control.FontWeight = control.FontWeight != FontWeights.Bold ? FontWeights.Bold : FontWeights.Normal;
                });

                //List<Control> listFontSettingsControl = (from control in element.GetChildrenOfType<Control>()
                //                                         where (!(control is HeaderedContentControl) &&
                //                              control is ContentControl && !((control as ContentControl).Content is String))
                //                                         select control).ToList();

                //if (!(element is HeaderedContentControl) &&
                //        element is ContentControl && !((element as ContentControl).Content is String))
                //    listFontSettingsControl.Add(element);
                //listFontSettingsControl.ForEach(control =>
                //{
                //    var t = control.GetType();
                //    var listProperty = GetCurrentPropertyList(t);
                //    if (listProperty.Count > 0)
                //    {
                //        listProperty.ForEach(X =>
                //        {
                //            p = t.GetProperty(X.Name);
                //            fs = (p.GetValue((UIElement)control) as FontSettings).Clone();
                //            fs.FontWeight = fs.FontWeight != FontWeights.Bold ? FontWeights.Bold : FontWeights.Normal;
                //            p.SetValue((UIElement)control, fs);
                //        });
                //    }
                //});            
            }
        }
        void CanToggleBold(object sender, CanExecuteRoutedEventArgs e)
        {
            var element = DataContext as Control;
            if (element != null)
            {
                var listControl = element.GetChildrenOfType<Control>().ToList();
                if (listControl.Count == 0)
                    listControl.Add(element);
                e.CanExecute = listControl.Count > 0;
                var parameter = sender as ToggleButton;
                if (parameter != null)
                    parameter.IsChecked = listControl[0].FontWeight == FontWeights.Bold;
            }
            else
                e.CanExecute = false;
        }
        private void OnToggleItalic(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var element = DataContext as Control;
            if (element != null)
            {
                List<Control> listControl = (from c in element.GetChildrenOfType<Control>()
                                             where !(c.ReadLocalValue(Control.FontStyleProperty) is BindingExpression)
                                             select c).ToList();
                if (!(element.ReadLocalValue(Control.FontSizeProperty) is BindingExpression))
                    listControl.Add(element);
                listControl.ForEach(control =>
                {
                    control.FontStyle = control.FontStyle != FontStyles.Italic ? FontStyles.Italic : FontStyles.Normal;
                });

                //List<Control> listFontSettingsControl = (from control in element.GetChildrenOfType<Control>()
                //                                         where (!(control is HeaderedContentControl) &&
                //                              control is ContentControl && !((control as ContentControl).Content is String))
                //                                         select control).ToList();

                //if (!(element is HeaderedContentControl) &&
                //        element is ContentControl && !((element as ContentControl).Content is String))
                //    listFontSettingsControl.Add(element);
                //listFontSettingsControl.ForEach(control =>
                //{
                //    var t = control.GetType();
                //    var listProperty = GetCurrentPropertyList(t);
                //    if (listProperty.Count > 0)
                //    {
                //        listProperty.ForEach(X =>
                //        {
                //            p = t.GetProperty(X.Name);
                //            fs = (p.GetValue((UIElement)control) as FontSettings).Clone();
                //            fs.FontStyle = fs.FontStyle != FontStyles.Italic ? FontStyles.Italic : FontStyles.Normal;
                //            p.SetValue((UIElement)control, fs);
                //        });
                //    }
                //});
            }
        }
        void CanToggleItalic(object sender, CanExecuteRoutedEventArgs e)
        {
            var element = DataContext as Control;
            if (element != null)
            {
                var listControl = element.GetChildrenOfType<Control>().ToList();
                if (listControl.Count == 0)
                    listControl.Add(element);
                e.CanExecute = listControl.Count > 0;
                var parameter = sender as ToggleButton;
                if (parameter != null)
                    parameter.IsChecked = listControl[0].FontStyle == FontStyles.Italic;
            }
            else
                e.CanExecute = false;
        }

        private void OnToggleUnderline(object sender, ExecutedRoutedEventArgs e)
        {
            var element = DataContext as UIElement;
            if (element != null)
            {
                var listControl = element.GetVisualChildrenOfType<TextBlock>().ToList();
                if (listControl.Count == 0 && DataContext is TextBlock)
                    listControl.Add(DataContext as TextBlock);
                listControl.ForEach(control =>
                {
                    if (control.TextDecorations != null)
                    {
                        var textDecorations = new TextDecorationCollection(control.TextDecorations);
                        foreach (var item in textDecorations)
                        {
                            if (item.Location == TextDecorationLocation.Underline)
                                control.TextDecorations.Remove(item);
                        }

                        bool bAddUnderLine = control.TextDecorations.Count == textDecorations.Count;
                        if (bAddUnderLine)
                        {
                            foreach (var item in TextDecorations.Underline)
                            {
                                foreach (var decorator in control.TextDecorations)
                                    control.TextDecorations.Add(item);
                            }
                        }
                    }
                });

                var listControlBoxes = element.GetVisualChildrenOfType<TextBox>().ToList();
                if (listControlBoxes.Count == 0 && DataContext is TextBox)
                    listControlBoxes.Add(DataContext as TextBox);
                listControlBoxes.ForEach(control =>
                {
                    if (control.TextDecorations != null)
                    {
                        var textDecorations = new TextDecorationCollection(control.TextDecorations);
                        foreach (var item in textDecorations)
                        {
                            if (item.Location == TextDecorationLocation.Underline)
                                control.TextDecorations.Remove(item);
                        }

                        bool bAddUnderLine = control.TextDecorations.Count == textDecorations.Count;
                        if (bAddUnderLine)
                        {
                            foreach (var item in TextDecorations.Underline)
                            {
                                foreach (var decorator in control.TextDecorations)
                                    control.TextDecorations.Add(item);
                            }
                        }
                    }
                });

                element.InvalidateArrange();
                element.InvalidateMeasure();
            }
        }

        void CanToggleUnderline(object sender, CanExecuteRoutedEventArgs e)
        {
            var element = DataContext as UIElement;
            if (element != null)
            {
                var listControl = element.GetVisualChildrenOfType<TextBlock>().ToList();
                if (listControl.Count == 0 && DataContext is TextBlock)
                    listControl.Add(DataContext as TextBlock);
                var listControlBoxes = element.GetVisualChildrenOfType<TextBox>().ToList();
                if (listControlBoxes.Count == 0 && DataContext is TextBox)
                    listControlBoxes.Add(DataContext as TextBox);
                e.CanExecute = listControl.Count > 0 || listControlBoxes.Count > 0;
                var parameter = sender as ToggleButton;
                if (parameter != null)
                {
                    parameter.IsChecked = false;

                    listControl.ForEach(control =>
                    {
                        if (control.TextDecorations != null)
                        {
                            foreach (var item in control.TextDecorations)
                            {
                                if (item.Location == TextDecorationLocation.Underline)
                                {
                                    parameter.IsChecked = true;
                                    break;
                                }
                            }
                        }
                    });

                    listControlBoxes.ForEach(control =>
                    {
                        if (control.TextDecorations != null)
                        {
                            foreach (var item in control.TextDecorations)
                            {
                                if (item.Location == TextDecorationLocation.Underline)
                                {
                                    parameter.IsChecked = true;
                                    break;
                                }
                            }
                        }
                    });
                }
            }
            else
                e.CanExecute = false;
        }
        private void OnIncreaseFontSize(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var element = DataContext as Control;
            if (element != null)
            {
                List<Control> listControl = (from c in element.GetChildrenOfType<Control>()
                                             where !(c.ReadLocalValue(Control.FontSizeProperty) is BindingExpression)
                                             select c).ToList();
                if (!(element.ReadLocalValue(Control.FontSizeProperty) is BindingExpression))
                    listControl.Add(element);
                listControl.ForEach(control =>
                {
                    control.FontSize += 1;
                });

                //List<Control> listFontSettingsControl = (from control in element.GetChildrenOfType<Control>()
                //                                         where (!(control is HeaderedContentControl) &&
                //                              control is ContentControl && !((control as ContentControl).Content is String))
                //                                         select control).ToList();

                //if (!(element is HeaderedContentControl) &&
                //        element is ContentControl && !((element as ContentControl).Content is String))
                //    listFontSettingsControl.Add(element);
                //listFontSettingsControl.ForEach(control =>
                //{
                //    var t = control.GetType();
                //    var listProperty = GetCurrentPropertyList(t);
                //    if (listProperty.Count > 0)
                //    {
                //        listProperty.ForEach(X =>
                //        {
                //            p = t.GetProperty(X.Name);
                //            fs = (p.GetValue((UIElement)control) as FontSettings).Clone();
                //            fs.FontSize += 1;
                //            p.SetValue((UIElement)control, fs);
                //        });
                //    }
                //});
            }
        }
        void CanIncreaseFontSize(object sender, CanExecuteRoutedEventArgs e)
        {
            var element = DataContext as Control;
            if (element != null)
            {
                var listControl = element.GetChildrenOfType<Control>().ToList();
                if (listControl.Count == 0)
                    listControl.Add(element);
                e.CanExecute = listControl.Count > 0;
            }
            else
                e.CanExecute = false;
        }
        private void OnDecreaseFontSize(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var element = DataContext as Control;
            if (element != null)
            {
                List<Control> listControl = (from c in element.GetChildrenOfType<Control>()
                                             where !(c.ReadLocalValue(Control.FontSizeProperty) is BindingExpression)
                                             select c).ToList();
                if (!(element.ReadLocalValue(Control.FontSizeProperty) is BindingExpression))
                    listControl.Add(element);
                listControl.ForEach(control =>
                {
                    control.FontSize -= 1;
                });

                //List<Control> listFontSettingsControl = (from control in element.GetChildrenOfType<Control>()
                //                                         where (!(control is HeaderedContentControl) &&
                //                              control is ContentControl && !((control as ContentControl).Content is String))
                //                                         select control).ToList();

                //if (!(element is HeaderedContentControl) &&
                //        element is ContentControl && !((element as ContentControl).Content is String))
                //    listFontSettingsControl.Add(element);
                //listFontSettingsControl.ForEach(control =>
                //{
                //    var t = control.GetType();
                //    var listProperty = GetCurrentPropertyList(t);
                //    if (listProperty.Count > 0)
                //    {
                //        listProperty.ForEach(X =>
                //        {
                //            p = t.GetProperty(X.Name);
                //            fs = (p.GetValue((UIElement)control) as FontSettings).Clone();
                //            fs.FontSize -= 1;
                //            p.SetValue((UIElement)control, fs);
                //        });
                //    }
                //});
            }
        }
        void CanDecreaseFontSize(object sender, CanExecuteRoutedEventArgs e)
        {
            var element = DataContext as Control;
            if (element != null)
            {
                var listControl = element.GetChildrenOfType<Control>().ToList();
                if (listControl.Count == 0)
                    listControl.Add(element);
                e.CanExecute = listControl.Count > 0;
            }
            else
                e.CanExecute = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var stringEditor = EditorComponent.StringEditor.GetStringEditor(Parent);
            stringEditor.DataContext = TextEditor.Text;

            var Dialog = new GeneralDialogContent(stringEditor)
            {
                Owner = this.FindParent<Window>(),
                DialogKeepContent = true,
                Title = Properties.Resources.SelectStringEditor,
                HelpLink = "StringEditor"
            };
            if (Dialog.ShowDialog() != true)
                return;

            TextEditor.Text = stringEditor.DataContext as String;
            TextEditor.Focus();
            TextEditor.SelectAll();
        }

        private void Button_Clear(object sender, RoutedEventArgs e)
        {
            TextEditor.Text = String.Empty;
        }
    }
}
