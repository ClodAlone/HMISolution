// <copyright file="TextBoxExt.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
#if WPF
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Controls.Input
#elif WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;

namespace Syncfusion.WP.Controls.Input
#elif SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Tools.Controls.Input

#else
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Syncfusion.UI.Xaml.Controls.Data;

namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Represents a Windows 8 extended textbox. The TextBoxExt control allows the user
    /// to enter text in an application. This control has additional functionality that
    /// is not found in the standard Windows 8 text box control, including Watermark
    /// support and AutoComplete.
    /// </summary>
    /// <remarks>
    /// The TextBoxExt is inherited from <see
    /// cref="N:Windows.UI.Xaml.Controls.TextBox">TextBox</see>.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
#if WPFSILVERLIGHT || WINDOWS_PHONE||WINDOWS_PHONE_7
    public partial class SfTextBoxExt : TextBox
#else
    public partial class SfTextBoxExt : TextBox, IDataValidator
#endif
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfTextBoxExt()
        {
#if WPF
            if (EnvironmentTestInput.IsSecurityGranted)
            {
                EnvironmentTestInput.StartValidateLicense(typeof(SfTextBoxExt));
            }
#endif
            DefaultStyleKey = typeof(SfTextBoxExt);    
            this.TextChanged += OnTextChanged;
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            this.Loaded += SfTextBoxExt_Loaded;
            this.Unloaded += SfTextBoxExt_Unloaded;
#endif
        }

     
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        void SfTextBoxExt_Unloaded(object sender, RoutedEventArgs e)
        {
            this.TextChanged -= OnTextChanged;
            this.Loaded -= SfTextBoxExt_Loaded;
            this.Unloaded -= SfTextBoxExt_Unloaded;
        }

        void SfTextBoxExt_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsSuggestionOpen)
            {
                if (ShowSuggestionsOnFocus)
                {
                    this.FilterSuggestions();
                }
                UpdateSuggestionBox();
                OpenSuggestion();
            }
        }
#endif

#if ! (WPFSILVERLIGHT || WINRT)
        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateWatermark();
        }
#endif 
        #endregion

        #region Variables

        private bool isFocussed;

#if WPFSILVERLIGHT || WINRT

        internal SuggestionBox PART_SuggestionBox;
        internal DispatcherTimer popupTimer;
        internal Popup PART_Popup;
#endif


        #endregion

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7 || WPFSILVERLIGHT)
        #region Helper Methods
        
        /// <summary>
        /// Validate the states
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.VisualStates"/>
        /// </summary>
        /// <param name="args"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Validate(ValidationEventArgs args)
        {
            if(args.HasError)
            {
                VisualStateManager.GoToState(this, "HasError", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "NoError", true);
            }
        }

        #endregion
#endif
        #region Override Methods

        /// <summary>
        /// Occurs when focus is obtained.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            if (ShowSuggestionsOnFocus && AutoCompleteSource!=null)
            {
                if (AutoCompleteMode == AutoCompleteMode.SuggestAppend || AutoCompleteMode == AutoCompleteMode.Suggest)
                    cansuggest = true;
                this.FilterSuggestions();
            }
#endif
            isFocussed = true;
            UpdateWatermark();
            base.OnGotFocus(e);
        }

        /// <summary>
        /// Occurs when focus is lost.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            isFocussed = false;
            UpdateWatermark();            
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            if (IsSuggestionOpen)
            {
                IsSuggestionOpen = false;
            }            
            if (!string.IsNullOrEmpty(tempText) && PART_SuggestionBox != null && SelectedItem!=null && PART_SuggestionBox.SelectedItem == null && !Text.Equals(tempText[appendindex]))
            {
                    Text = tempText;
            }               
            
#endif
            base.OnLostFocus(e);
        }

        /// <summary>
        /// Initialises all the child elements of
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt"/> control.
        /// </summary>
#if WPFSILVERLIGHT || WINDOWS_PHONE||WINDOWS_PHONE_7
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            PART_Watermark = GetTemplateChild("PART_Watermark") as FrameworkElement;


#if WPFSILVERLIGHT || WINRT
            PART_Popup = GetTemplateChild("PART_Popup") as Popup;
#if WPF
            if(PART_Popup !=null)
            {
                PART_Popup.Opened -= PART_Popup_Opened;
                PART_Popup.Opened += PART_Popup_Opened;
            }
#endif
            PART_SuggestionBox = GetTemplateChild("PART_SuggestionBox") as SuggestionBox;
            if (PART_SuggestionBox != null)
            {
#if SILVERLIGHT
                PART_SuggestionBox.Loaded += PART_SuggestionBox_Loaded;
#endif
                PART_SuggestionBox.SelectionChanged += SuggestionChanged;
            }
#endif
            UpdateWatermark();
            base.OnApplyTemplate();
        }
#if SILVERLIGHT
        void PART_SuggestionBox_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePopup();
        }
#endif
#if WPF
        void PART_Popup_Opened(object sender, EventArgs e)
        {
            scrollviewer = VisualUtils.FindDescendant(PART_SuggestionBox, typeof(ScrollViewer)) as ScrollViewer;
            UpdateSelectionOnKeyDown(PART_SuggestionBox.SelectedIndex);
        }
#endif
        #endregion

    }
}
