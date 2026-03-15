using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using Utilities;
using Utilities.WPF;
using WPFUtilities;

namespace StringManager.Controls
{
    /// <summary>
    /// Interaction logic for NewLocale.xaml
    /// </summary>
    public partial class NewLocale : UserControl, IDisposable
    {
        #region Declarations
        readonly StringManager.TranslationService.LanguageServiceClient client;
        readonly String AppID;
        bool bLoaded;
        #endregion

        public NewLocale(StringManager.TranslationService.LanguageServiceClient c,
                        String appId)
        {
            InitializeComponent();
            //gridDataControl.SourceType = typeof(CultureInfo);
            client = c;
            AppID = appId;
            ReadLocales();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (CustomFontHelper.CanApplyCustomFont())
            {
                gridDataControl.FontFamily = CustomFontHelper.GetCustomFontFamily();
                gridDataControl.FontSize = CustomFontHelper.GetCustomFontSize();
            }
        }

        async void ReadLocales()
        {
            var listAvailableLanguages = new List<CultureInfo>();

            // var listLanguages = await client.GetLanguagesForTranslateAsync(AppID);

            listAvailableLanguages = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();
            listAvailableLanguages.RemoveAll(culture => culture.IsNeutralCulture);
            if (listAvailableLanguages.Contains(CultureInfo.InvariantCulture))
                listAvailableLanguages.Remove(CultureInfo.InvariantCulture);

            listAvailableLanguages.Sort(CompareColtureInfo);
            //foreach (string languageCode in listLanguages)
            //{
            //    try
            //    {
            //        CultureInfo ci = new CultureInfo(languageCode); // use the TwoLetterISOLanguageName
            //        if (ci.IsNeutralCulture)
            //        {
            //            ci = new CultureInfo(String.Format("{0}-{1}", ci.TwoLetterISOLanguageName, ci.TwoLetterISOLanguageName));
            //        }
                    
            //        listAvailableLanguages.Add(ci);
            //    }
            //    catch (Exception ex)
            //    {
            //    }
            //}

            gridDataControl.ItemsSource = listAvailableLanguages;

            progressBar.Visibility = Visibility.Collapsed;
        }
        
        private static int CompareColtureInfo(CultureInfo x, CultureInfo y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0; //==
                else
                    return -1;// x < y
            }
            else
            {
                //x!= null
                if (y == null)
                    return 1; //x > y
                else
                {
                    return x.NativeName.CompareTo(y.NativeName);
                }
            }
        }

        private void gridDataControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var culture = gridDataControl.SelectedItem as CultureInfo;
            if (culture == null)
                return;

            var wnd = this.FindParent<Window>();
            if (wnd != null)
            {
                Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
                    {
                        try
                        {
                            wnd.DialogResult = true;
                            wnd.Close();
                        }
                        catch { }
                    });
            }
        }

        public CultureInfo GetSelected()
        {
            return gridDataControl.SelectedItem as CultureInfo;
        }

        public void Dispose()
        {
            // gridDataControl.Model.Dispose();
            try
            {
                gridDataControl.Dispose();
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
