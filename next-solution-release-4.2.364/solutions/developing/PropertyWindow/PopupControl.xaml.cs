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
using Utilities;
using WPFUtilities;

namespace PropertyControl
{
    /// <summary>
    /// Interaction logic for PopupControl.xaml
    /// </summary>
    public partial class PopupControl : UserControl, IDisposable
    {
        public PopupControl()
        {
            InitializeComponent();

            //var currentStyle = ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");
            //ThemeHelper.SetTheme(this, currentStyle);

            propertyControl.isPopup = true;
            propertyControl.btnOK.IsVisible = false;
            propertyControl.btnCancel.IsVisible = false;
            propertyControl.separator.IsVisible = false;

            SizeChanged += (o, e) =>
            {
                bool bVisible = ActualHeight > 200;
                propertyControl.stackTitle.Visibility = bVisible ? Visibility.Visible : Visibility.Collapsed;
                propertyControl.toolbarGroupingFiltering.Visibility = bVisible ? Visibility.Visible : Visibility.Collapsed;
                propertyControl.txtShortDesc.Visibility = bVisible ? Visibility.Visible : Visibility.Collapsed;
                propertyControl.txtLongDesc.Visibility = bVisible ? Visibility.Visible : Visibility.Collapsed;
            };
            //var style = ApplicationPropertiesHelper.GetProperty("CurrentSkin") as String;
            //if (style == "Blend")
            //{
            //    propertyControl.Resources.MergedDictionaries.Add(new Alloy());
            //}
        }

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            propertyControl.Dispose();
        }
        #endregion
    }
}
