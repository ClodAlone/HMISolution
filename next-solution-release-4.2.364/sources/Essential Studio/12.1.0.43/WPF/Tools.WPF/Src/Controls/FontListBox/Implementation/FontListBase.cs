#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class FontListBase : Control
    {
        static internal object _fontSource = "System";

        /// <summary>
        /// Identifies the <see cref="AllFonts"/> dependency property.
        /// </summary>
        protected static readonly DependencyPropertyKey AllFontsPropertyKey =
            DependencyProperty.RegisterReadOnly("AllFonts", typeof(FontCollection), typeof(FontListBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAllFontsChanged), CoerceAllFontsProperty));

        /// <summary>
        /// Identifies the <see cref="AllFonts"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllFontsProperty = AllFontsPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets the collection of fonts. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="FontCollection"/>
        /// </value>
        /// <seealso cref="FontCollection"/>
        internal FontCollection AllFonts
        {
            get
            {
                return (FontCollection)GetValue(AllFontsProperty);
            }

            set
            {
                SetValue(AllFontsPropertyKey, value);
            }
        }

        /// <summary>
        /// Event that is raised when <see cref="AllFonts"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback AllFontsChanged;

        /// <summary>
        /// Calls OnAllFontsChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        private static void OnAllFontsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FontListBase instance = (FontListBase)d;
            instance.OnAllFontsChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="AllFontsChanged"/> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        protected virtual void OnAllFontsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AllFontsChanged != null)
            {
                AllFontsChanged(this, e);
            }
        }

        /// <summary>
        /// Coerces <see cref="AllFonts"/> property.
        /// Fulfils the logic before setting the <see cref="AllFonts"/> value.
        /// </summary>
        /// <param name="d">FontListBox instance to which this property belongs.</param>
        /// <param name="baseValue">New value.</param>
        /// <returns>Value that should be set.</returns>
        public static object CoerceAllFontsProperty(DependencyObject d, object baseValue)
        {
            FontListBase fontListBox = (FontListBase)d;

            FontCollection fontCollection = new FontCollection();
            XmlLanguage userLanguage = XmlLanguage.GetLanguage("en-US");
            if (baseValue == null || _fontSource.Equals("System"))
            {
                _fontSource = "System";
                if (fontListBox.AllFonts == null || (fontListBox.AllFonts != null && fontListBox.AllFonts.Count < Fonts.SystemFontFamilies.Count))
                {
                    foreach (FontFamily font in Fonts.SystemFontFamilies)
                    {
                        try
                        {
                            LanguageSpecificStringDictionary dictionary = font.FamilyNames;
                            if (dictionary.Keys.Contains(userLanguage))
                            {
                                //if (font.FamilyNames[userLanguage] == font.FamilyNames[userLanguage])
                                //{
                                fontCollection.Add(font);
                                //}
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                else if (fontListBox.AllFonts != null)
                {
                    fontCollection = fontListBox.AllFonts;
                }
                return fontCollection;
            }
            else
            {
                return baseValue;
            }
        }
    }
}