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
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
#else
using Windows.UI.Xaml;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public class ThemeDictionaries : ResourceDictionary
    {
        private ResourceDictionary lightThemeDictionary;
        private ResourceDictionary darkThemeDictionary;

        private bool IsDarkTheme
        {
            get
            {
                return (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"]== Visibility.Visible;
            }
        }


        public ResourceDictionary LightThemeDictionary
        {
            get
            {
                return lightThemeDictionary;
            }
            set
            {
                lightThemeDictionary = value;

                if (!IsDarkTheme && value != null)
                {
                    MergedDictionaries.Add(value);
                }
            }
        }

        public ResourceDictionary DarkThemeDictionary
        {
            get
            {
                return darkThemeDictionary;
            }

            set
            {
                darkThemeDictionary=value;
                if (IsDarkTheme && value != null)
                {
                    MergedDictionaries.Add(value);
                }
            }
        }
    }
}
