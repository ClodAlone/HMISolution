using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TranslationHelpers;
using DocumentManager.ComponentService;
using StringManager.ComponentService;

namespace WPFUtilities.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for BitMaskEditor.xaml
    /// </summary>
    public partial class BitMaskEditor : UserControl
    {
        string stringPlaceolder = Properties.Settings.Default.AccessLevelPlaceHolder;

        public enum BitMaskType
        {
            Level,
            Area
        };

        public BitMaskType Type = BitMaskType.Area;
        public bool ShowInheritedButton = false;
        public IDocument Document;
        IStringEditorManager stringManager;
        public BitMaskEditor(bool useRuntimeSettings = false)
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (useRuntimeSettings)
                {
                    FontSize = WPFUtilities.Properties.Settings.Default.DialogControlsFontSize;
                    allBtn.Width = noneBtn.Width = btnInherited.Width = WPFUtilities.Properties.Settings.Default.ButtonsWidth;
                    allBtn.Height = noneBtn.Height = btnInherited.Height = WPFUtilities.Properties.Settings.Default.ButtonsHeight;
                }

                SetCheckLabel();
                if (!ShowInheritedButton)
                {
                    inheritedColumn.Width = new GridLength(0);
                    btnInherited.Visibility = Visibility.Collapsed;
                }
                else
                {
                    var model = DataContext as model;
                    if (model != null && !model.Value.HasValue)
                    {
                        btnInherited.IsChecked = true;
                        EnableAllCheckBoxes(false);
                        SetAllCheckBoxes(null);
                    }
                }
            };
        }

        private void SetCheckLabel()
        {
            IDictionary<String, String> StringList = null;
            if (!DesignerProperties.GetIsInDesignMode(this) && Document != null)
            {
                if (stringManager == null)
                    stringManager = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                if(stringManager != null)
                    StringList = stringManager?.GetListStringForCulture(Document, stringManager.GetActiveCulture(Document));
            }

            switch (Type)
            {
                case BitMaskType.Area:
                    Level1.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area1", StringList, Properties.Resources.Area1); 
                    Level2.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area2", StringList, Properties.Resources.Area2);
                    Level3.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area3", StringList, Properties.Resources.Area3);
                    Level4.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area4", StringList, Properties.Resources.Area4);
                    Level5.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area5", StringList, Properties.Resources.Area5);
                    Level6.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area6", StringList, Properties.Resources.Area6);
                    Level7.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area7", StringList, Properties.Resources.Area7);
                    Level8.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area8", StringList, Properties.Resources.Area8);
                    Level9.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area9", StringList, Properties.Resources.Area9);
                    Level10.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area10", StringList, Properties.Resources.Area10);
                    Level11.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area11", StringList, Properties.Resources.Area11);
                    Level12.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area12", StringList, Properties.Resources.Area12);
                    Level13.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area13", StringList, Properties.Resources.Area13);
                    Level14.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area14", StringList, Properties.Resources.Area14);
                    Level15.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area15", StringList, Properties.Resources.Area15);
                    Level16.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area16", StringList, Properties.Resources.Area16);
                    Level17.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area17", StringList, Properties.Resources.Area17);
                    Level18.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area18", StringList, Properties.Resources.Area18);
                    Level19.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area19", StringList, Properties.Resources.Area19);
                    Level20.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area20", StringList, Properties.Resources.Area20);
                    Level21.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area21", StringList, Properties.Resources.Area21);
                    Level22.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area22", StringList, Properties.Resources.Area22);
                    Level23.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area23", StringList, Properties.Resources.Area23);
                    Level24.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area24", StringList, Properties.Resources.Area24);
                    Level25.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area25", StringList, Properties.Resources.Area25);
                    Level26.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area26", StringList, Properties.Resources.Area26);
                    Level27.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area27", StringList, Properties.Resources.Area27);
                    Level28.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area28", StringList, Properties.Resources.Area28);
                    Level29.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area29", StringList, Properties.Resources.Area29);
                    Level30.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area30", StringList, Properties.Resources.Area30);
                    Level31.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Area31", StringList, Properties.Resources.Area31);
                    break;
                case BitMaskType.Level:
                    Level1.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level1", StringList, Properties.Resources.Level1);
                    Level2.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level2", StringList, Properties.Resources.Level2);
                    Level3.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level3", StringList, Properties.Resources.Level3);
                    Level4.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level4", StringList, Properties.Resources.Level4);
                    Level5.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level5", StringList, Properties.Resources.Level5);
                    Level6.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level6", StringList, Properties.Resources.Level6);
                    Level7.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level7", StringList, Properties.Resources.Level7);
                    Level8.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level8", StringList, Properties.Resources.Level8);
                    Level9.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level9", StringList, Properties.Resources.Level9);
                    Level10.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level10", StringList, Properties.Resources.Level10);
                    Level11.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level11", StringList, Properties.Resources.Level11);
                    Level12.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level12", StringList, Properties.Resources.Level12);
                    Level13.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level13", StringList, Properties.Resources.Level13);
                    Level14.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level14", StringList, Properties.Resources.Level14);
                    Level15.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level15", StringList, Properties.Resources.Level15);
                    Level16.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level16", StringList, Properties.Resources.Level16);
                    Level17.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level17", StringList, Properties.Resources.Level17);
                    Level18.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level18", StringList, Properties.Resources.Level18);
                    Level19.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level19", StringList, Properties.Resources.Level19);
                    Level20.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level20", StringList, Properties.Resources.Level20);
                    Level21.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level21", StringList, Properties.Resources.Level21);
                    Level22.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level22", StringList, Properties.Resources.Level22);
                    Level23.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level23", StringList, Properties.Resources.Level23);
                    Level24.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level24", StringList, Properties.Resources.Level24);
                    Level25.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level25", StringList, Properties.Resources.Level25);
                    Level26.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level26", StringList, Properties.Resources.Level26);
                    Level27.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level27", StringList, Properties.Resources.Level27);
                    Level28.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level28", StringList, Properties.Resources.Level28);
                    Level29.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level29", StringList, Properties.Resources.Level29);
                    Level30.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level30", StringList, Properties.Resources.Level30);
                    Level31.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Level31", StringList, Properties.Resources.Level31);
                    break;

            }
        }

        private void Click_All(object sender, RoutedEventArgs e)
        {
            btnInherited.IsChecked = false;
            EnableAllCheckBoxes(true);
            SetAllCheckBoxes(true);
        }

        private void Click_None(object sender, RoutedEventArgs e)
        {
            btnInherited.IsChecked = false;
            EnableAllCheckBoxes(true);
            SetAllCheckBoxes(false);
        }

        private void Click_Inherited(object sender, RoutedEventArgs e)
        {
            var bEnable = btnInherited.IsChecked == false;
            EnableAllCheckBoxes(bEnable);
            SetAllCheckBoxes(bEnable ? new Nullable<bool>(true) : null);
        }

        void SetAllCheckBoxes(bool? bSet)
        {
            (from c in stackPanel.Children.OfType<CheckBox>() select c).ToList().ForEach(check =>
            {
                if (bSet.HasValue && !bSet.Value)
                {
                    if (!check.IsChecked.HasValue || check.IsChecked == true)
                        check.IsChecked = false;
                }
                else
                    check.IsChecked = bSet;
            });
        }

        void EnableAllCheckBoxes(bool bEnable)
        {
            (from c in stackPanel.Children.OfType<CheckBox>() select c).ToList().ForEach(check =>
            {
                check.IsEnabled = bEnable;
            });
        }
    }
}
