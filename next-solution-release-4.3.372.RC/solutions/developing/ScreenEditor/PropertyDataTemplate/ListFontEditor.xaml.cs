using Converters;
using Converters.PropertyDataTemplate;
using DocumentManager.ComponentService;
using ScreenManager.ComponentService;
using ScreenManager.Controls;
using ScreenSettings;
using ScreenSettings.Entities;
using StringManager.ComponentService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utilities;
using Utilities.WPF;
using ViewModelLib;
using WPFUtilities;
using WPFUtilities.PropertyDataTemplate;

namespace ScreenManager.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ListUriLogicEditor.xaml
    /// </summary>
    public partial class ListFontEditor : UserControl
    {
        #region Declaration
        readonly ObservableCollection<CultureFontSettings> ListUri = new ObservableCollection<CultureFontSettings>();
        List<string> cultures;
        List<string> cultureused = new List<string>();
        #region Document
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(IDocument), typeof(ListFontEditor), new UIPropertyMetadata(null));
        public IDocument Document
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IDocument)GetValue(DocumentProperty);
            }
            set
            {
                SetValue(DocumentProperty, value);
            }
        }

        #endregion
        #endregion

        #region Constructors
        public ListFontEditor(Dictionary<string, FontSettings> listUri)
        {
            InitializeComponent();
            Document = ScreenManagerComponent.screenManagerComponent.Workspace.ContextDocument;
            var stringEditor = Document?.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
            cultures = stringEditor.GetListAvailableCultures(Document).ToList();
            if(listUri != null)
                listUri.Keys.ToList().ForEach(k =>
                {
                    if(cultures.Contains(k) && listUri[k] != null)
                    {
                        ListUri.Add(new CultureFontSettings(k, listUri[k]));
                        cultureused.Add(k);
                    }
                });
            gridControl.ItemsSource = ListUri;
        }
        #endregion

        #region Commands
        public static readonly RoutedCommand ClearCommand = new RoutedCommand();
        public static readonly RoutedCommand EditCommand = new RoutedCommand();
        #endregion

        #region Methods

        private void OnEditCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {
                var fontsettings = new FontSettings((ListUri.ElementAt(listIndex).Value));
                FontSettingsEditor fonteditor = new FontSettingsEditor(fontsettings);
                GeneralDialogContent Dialog = new GeneralDialogContent(fonteditor)
                {
                    Title = Properties.Resources.FontEditor,
                    Owner = this.FindParent<Window>(),
                    HelpLink = "FontEditor"
                };
                if (Dialog.ShowDialog() == true)
                {
                    (ListUri.ElementAt(listIndex).Value) = new FontSettings(fonteditor.UIFontSettings);
                    gridControl.ItemsSource = null;
                    gridControl.ItemsSource = ListUri;
                    tableView.FocusedRowHandle = focusedrow;
                    gridControl.RefreshRow(focusedrow);
                }
            }

        }

        private void OnCanEditCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

        }
        private void OnClearCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {
                (ListUri.ElementAt(listIndex).Value) = null;
                gridControl.ItemsSource = null;
                gridControl.ItemsSource = ListUri;
                tableView.FocusedRowHandle = focusedrow;
                gridControl.RefreshRow(focusedrow);
            }
        }

        private void OnCanClearCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        #region Properties
        public Dictionary<string, FontSettings> CurrentList
        {
            get
            {
                Dictionary<string, FontSettings> currentlist = new Dictionary<string, FontSettings>();
                ListUri?.ToList().ForEach(c =>
                {
                    if(c.Value != null)
                        currentlist.Add(c.CultureName, c.Value);
                });
                return currentlist;
            }
        }
        #endregion

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            int listIndex = gridControl.GetListIndexByRowHandle(tableView.FocusedRowHandle);
            if (listIndex >= 0)
            {
                string culture = ListUri[listIndex].CultureName;
                ListUri.RemoveAt(listIndex);
                if(culture.Contains(culture))
                    cultureused.Remove(culture);
            }

            gridControl.ItemsSource = null;
            gridControl.ItemsSource = ListUri;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if(cultures != null && cultures.Count > 0 && cultures.Count != cultureused.Count)
            {
                var control = new CulturesSelector(cultures);
                var Dialog = new GeneralDialogContent(control)
                {
                    DialogKeepContent = true,
                    Title = Properties.Resources.CultureFontSettingListDialogTitle,
                    Owner = this.FindParent<Window>(),
                    HelpLink = "CultureFontSettingList"
                };
                if (Dialog.ShowDialog() == true)
                {
                    List<string> cultures = control.GetSelectedCultures();
                    cultures.ForEach(culture =>
                    {
                        if (!cultureused.Contains(culture))
                        {
                            ListUri.Add(new CultureFontSettings(culture, new FontSettings()));
                            cultureused.Add(culture);
                            gridControl.ItemsSource = null;
                            gridControl.ItemsSource = ListUri;
                        }
                    });
                }
            }
        }
    }
#if !WINDOWS_UWP && !NET_STANDARD
    public class CultureFontSettings
    {
        public string _CultureName;
        public FontSettings _Value;
        public string CultureName
        {
            get
            {
                return _CultureName;
            }
            set
            {
                _CultureName = value;
            }
        }

        public FontSettings Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }
        public CultureFontSettings(string culture, FontSettings font)
        {
            CultureName = culture;
            Value = font;
        }
        public override string ToString()
        {
            return CultureName;
        }
    }
#endif
}
