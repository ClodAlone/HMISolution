// <copyright file="AutoComplete.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
#if !Silverlight4
using System.Threading.Tasks;
#endif

using System.Diagnostics.CodeAnalysis;
using System.Collections.ObjectModel;
using System.Windows;

#if WPF

using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Input;
namespace Syncfusion.Windows.Controls.Input
#elif WINDOWS_PHONE || WINDOWS_PHONE_7
using System.Windows.Media;
using System.Windows.Threading;
    
using Syncfusion.WP.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
namespace Syncfusion.WP.Controls.Input
#elif SILVERLIGHT
using System.Windows.Input;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media;
namespace Syncfusion.Tools.Controls.Input
#else
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Media;    
using Windows.System.Threading;
namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    [TemplateVisualState(Name = "Top", GroupName = "PopupStates")]
    [TemplateVisualState(Name = "Bottom", GroupName = "PopupStates")]
    //[ClassReference(IsReviewed = true)]
    public partial class SfTextBoxExt : TextBox
    {
        #region Variables

        private bool canappend = true;

        private bool cansuggest = true;

        private bool isCustom = false;

        private List<object> appendItems;

        private int appendindex;

        private string tempText;

        private string originaltextforsuggestion = String.Empty;



        internal ObservableCollection<object> collection = new ObservableCollection<object>();

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Returns a value if set
        /// </summary>
        /// <value>
        /// <c>true</c> if instance is created ; otherwise, <c>false</c>.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ClassReference(IsReviewed = false)]
        public bool AllowPointerEvents
        {
            get { return (bool)GetValue(AllowPointerEventsProperty); }
            set { SetValue(AllowPointerEventsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AllowPointerEvents.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowPointerEventsProperty =
            DependencyProperty.Register("AllowPointerEvents", typeof(bool), typeof(SfTextBoxExt), new PropertyMetadata(false));



        /// <summary>
        /// Gets or sets an IEnumerable source used to generate the content of the
        /// AutoComplete
        /// </summary>
        /// <remarks>
        /// The default value is null.
        /// </remarks>
        /// <value>
        /// The auto complete source.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public IEnumerable AutoCompleteSource
        {
            get { return (IEnumerable)GetValue(AutoCompleteSourceProperty); }
            set { SetValue(AutoCompleteSourceProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AutoCompleteSource.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AutoCompleteSourceProperty =
            DependencyProperty.Register("AutoCompleteSource", typeof(IEnumerable), typeof(SfTextBoxExt), new PropertyMetadata(null, OnAutoCompleteSourceChanged));

        /// <summary>
        /// Gets or sets the <see cref="N:Windows.UI.Xaml.Controls.DataTemplate"/> that is
        /// used to display the content of the suggestion items with auto complete.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.AutoCompleteItemsPanel"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.AutoCompleteItemTemplateSelector"/>
        [ClassReference(IsReviewed = false)]
        public DataTemplate AutoCompleteItemTemplate
        {
            get { return (DataTemplate)GetValue(AutoCompleteItemTemplateProperty); }
            set { SetValue(AutoCompleteItemTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AutoCompleteItemTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AutoCompleteItemTemplateProperty =
            DependencyProperty.Register("AutoCompleteItemTemplate", typeof(DataTemplate), typeof(SfTextBoxExt), new PropertyMetadata(null));

        #if WPF || WINRT
        /// <summary>
        /// Gets or sets a selection object that changes the DataTemplate to apply for 
        /// content, based on processing information about the content item or its
        /// container at run time.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public DataTemplateSelector AutoCompleteItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(AutoCompleteItemTemplateSelectorProperty); }
            set { SetValue(AutoCompleteItemTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AutoCompleteItemTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AutoCompleteItemTemplateSelectorProperty =
            DependencyProperty.Register("AutoCompleteItemTemplateSelector", typeof(DataTemplateSelector), typeof(SfTextBoxExt), new PropertyMetadata(null));
#endif

        /// <summary>
        /// Gets or sets <see cref="N:Windows.UI.Xaml.Controls.ItemsPanelTemplate"/> that
        /// defines the panel to use for the layout of the items.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.AutoCompleteItemTemplate"/>
        [ClassReference(IsReviewed = false)]
        public ItemsPanelTemplate AutoCompleteItemsPanel
        {
            get { return (ItemsPanelTemplate)GetValue(AutoCompleteItemsPanelProperty); }
            set { SetValue(AutoCompleteItemsPanelProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AutoCompleteItemsPanel.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AutoCompleteItemsPanelProperty =
            DependencyProperty.Register("AutoCompleteItemsPanel", typeof(ItemsPanelTemplate), typeof(SfTextBoxExt), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets a value indicating whether this instance is <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SuggestionBox"/> open.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is suggestion open; otherwise, <c>false</c>. The default value is false.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxStyle"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxPlacement"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxPosition"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionIndex"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionMode"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.Suggestions"/>
        [ClassReference(IsReviewed = false)]
        public bool IsSuggestionOpen
        {
            get { return (bool)GetValue(IsSuggestionOpenProperty); }
            set{ SetValue(IsSuggestionOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsSuggestionOpen.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSuggestionOpenProperty =
            DependencyProperty.Register("IsSuggestionOpen", typeof(bool), typeof(SfTextBoxExt), new PropertyMetadata(false, new PropertyChangedCallback(OnIsSuggestionOpenChanged)));


        /// <summary>
        /// Gets the <see cref="N:Windows.Foundation.Point">Point</see> that position the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SuggestionBox"/>.
        /// </summary>
        /// <value>
        /// The default values is null.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxStyle"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxPlacement"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionIndex"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionMode"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.Suggestions"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.IsSuggestionOpen"/>
        [ClassReference(IsReviewed = false)]
        public Point SuggestionBoxPosition
        {
            get { return (Point)GetValue(SuggestionBoxPositionProperty); }
            internal set { SetValue(SuggestionBoxPositionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SuggestionBoxPosition.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SuggestionBoxPositionProperty =
            DependencyProperty.Register("SuggestionBoxPosition", typeof(Point), typeof(SfTextBoxExt), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the search item path is used to search the <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.Suggestions"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxStyle"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxPlacement"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxPosition"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionIndex"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionMode"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.IsSuggestionOpen"/>
        [ClassReference(IsReviewed = false)]
        public string SearchItemPath
        {
            get { return (string)GetValue(SearchItemPathProperty); }
            set { SetValue(SearchItemPathProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SearchItemPath.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SearchItemPathProperty =
            DependencyProperty.Register("SearchItemPath", typeof(string), typeof(SfTextBoxExt), new PropertyMetadata(String.Empty));

        /// <summary>
        /// Gets the list of suggestions associated with <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionMode"/>.
        /// </summary>
        /// <value>
        /// The suggestions.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxStyle"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxPlacement"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxPosition"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionIndex"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionMode"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.IsSuggestionOpen"/>
        [ClassReference(IsReviewed = false)]
        public IEnumerable Suggestions
        {
            get { return (IEnumerable)GetValue(SuggestionsProperty); }
            internal set { SetValue(SuggestionsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Suggestions.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SuggestionsProperty =
            DependencyProperty.Register("Suggestions", typeof(IEnumerable), typeof(SfTextBoxExt), new PropertyMetadata(null, new PropertyChangedCallback(OnSuggestionsChanged)));



        /// <summary>
        /// Gets the index of the suggestion.
        /// </summary>
        /// <value>
        /// The default values is 0.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxStyle"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxPlacement"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxPosition"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionMode"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.Suggestions"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.IsSuggestionOpen"/>
        [ClassReference(IsReviewed = false)]
        public int SuggestionIndex
        {
            get { return (int)GetValue(SuggestionIndexProperty); }
            internal set { SetValue(SuggestionIndexProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SuggestionIndex.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SuggestionIndexProperty =
            DependencyProperty.Register("SuggestionIndex", typeof(int), typeof(SfTextBoxExt), new PropertyMetadata(-1));

        /// <summary>
        /// Gets or sets <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.AutoCompleteMode"/> that
        /// display the <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.Suggestions"/> in several
        /// ways.
        /// </summary>
        /// <value>
        /// The default value is <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.AutoCompleteMode.None">AutoCompleteMode.None</see>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public AutoCompleteMode AutoCompleteMode
        {
            get { return (AutoCompleteMode)GetValue(AutoCompleteModeProperty); }
            set { SetValue(AutoCompleteModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AutoCompleteMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AutoCompleteModeProperty =
            DependencyProperty.Register("AutoCompleteMode", typeof(AutoCompleteMode), typeof(SfTextBoxExt), new PropertyMetadata(AutoCompleteMode.None));

        /// <summary>
        /// Gets or sets the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SuggestionBoxPlacement"/> to display
        /// the <see cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.Suggestions"/>.
        /// </summary>
        /// <value>
        /// The default value is <see
        /// cref="F:Syncfusion.UI.Xaml.Controls.Input.SuggestionBoxPlacement.Bottom">SuggestionBoxPlacement.Bottom</see>.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxStyle"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.Suggestions"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxPosition"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionIndex"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionMode"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.IsSuggestionOpen"/>
        [ClassReference(IsReviewed = false)]
        public SuggestionBoxPlacement SuggestionBoxPlacement
        {
            get { return (SuggestionBoxPlacement)GetValue(SuggestionBoxPlacementProperty); }
            set { SetValue(SuggestionBoxPlacementProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AutoCompleteMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SuggestionBoxPlacementProperty =
            DependencyProperty.Register("SuggestionBoxPlacement", typeof(SuggestionBoxPlacement), typeof(SfTextBoxExt), new PropertyMetadata(SuggestionBoxPlacement.Bottom));

        /// <summary>
        /// Gets or sets the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SuggestionMode"/> to filter the <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.Suggestions"/>
        /// </summary>
        /// <value>
        /// The suggestion mode.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxStyle"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxPlacement"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxPosition"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionIndex"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.IsSuggestionOpen"/>
        [ClassReference(IsReviewed = false)]
        public SuggestionMode SuggestionMode
        {
            get { return (SuggestionMode)GetValue(SuggestionModeProperty); }
            set { SetValue(SuggestionModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AutoCompleteMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SuggestionModeProperty =
            DependencyProperty.Register("SuggestionMode", typeof(SuggestionMode), typeof(SfTextBoxExt), new PropertyMetadata(SuggestionMode.StartsWith));


        /// <summary>
        /// Gets or sets a value indicating whether to consider the lower or upper case
        /// letters for suggestions.
        /// </summary>
        /// <remarks>
        /// The default value is false.
        /// </remarks>
        /// <value>
        /// <c>true</c> if suggestions are displayed by typed letter case ; otherwise, <c>false</c>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public bool IgnoreCase
        {
            get { return (bool)GetValue(IgnoreCaseProperty); }
            set { SetValue(IgnoreCaseProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IgnoreCase.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IgnoreCaseProperty =
            DependencyProperty.Register("IgnoreCase", typeof(bool), typeof(SfTextBoxExt), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the minimum number of prefix characters.
        /// </summary>
        public int MinimumPrefixCharacters
        {
            get { return (int)GetValue(MinimumPrefixCharactersProperty); }
            set { SetValue(MinimumPrefixCharactersProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinimumPrefixCharacters.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinimumPrefixCharactersProperty =
            DependencyProperty.Register("MinimumPrefixCharacters", typeof(int), typeof(SfTextBoxExt), new PropertyMetadata(1, new PropertyChangedCallback(OnMinimumPrefixCharactersChanged)));

        /// <summary>
        /// Gets or sets the delay in popup
        /// </summary>
        public TimeSpan PopupDelay
        {
            get { return (TimeSpan)GetValue(PopupDelayProperty); }
            set { SetValue(PopupDelayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for PopupDelay.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PopupDelayProperty =
            DependencyProperty.Register("PopupDelay", typeof(TimeSpan), typeof(SfTextBoxExt), new PropertyMetadata(new TimeSpan(0, 0, 0, 0, 0)));


        /// <summary>
        /// Gets or sets the height of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SuggestionBox"/>.
        /// </summary>
        /// <value>
        /// The default values is 200.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxDropDownHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(SfTextBoxExt), new PropertyMetadata(200.0));


        /// <summary>
        /// Gets or sets the style with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SuggestionBox"/>.
        /// </summary>
        /// <value>
        /// The default values is null.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.Suggestions"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxPlacement"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionBoxPosition"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionIndex"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.SuggestionMode"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.IsSuggestionOpen"/>
        [ClassReference(IsReviewed = false)]
        public Style SuggestionBoxStyle
        {
            get { return (Style)GetValue(SuggestionBoxStyleProperty); }
            set { SetValue(SuggestionBoxStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SuggestionBoxStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SuggestionBoxStyleProperty =
            DependencyProperty.Register("SuggestionBoxStyle", typeof(Style), typeof(SfTextBoxExt), new PropertyMetadata(null));

        
        /// <summary>
        /// Returns a string as a delimiter
        /// </summary>
        public string Delimeter
        {
            get { return (string)GetValue(DelimeterProperty); }
            set { SetValue(DelimeterProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Delimeter.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DelimeterProperty =
            DependencyProperty.Register("Delimeter", typeof(string), typeof(SfTextBoxExt), new PropertyMetadata(null,new PropertyChangedCallback(OnDeliMeterChanged)));


        /// <summary>
        /// Used to determine whether the complete suggestion list should be displayed when an empty textbox is focused.
        /// </summary>
        /// <value>The default value is false</value>
        public bool ShowSuggestionsOnFocus
        {
            get { return (bool)GetValue(ShowSuggestionsOnFocusProperty); }
            set { SetValue(ShowSuggestionsOnFocusProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowSuggestionsOnFocus.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowSuggestionsOnFocusProperty =
            DependencyProperty.Register("ShowSuggestionsOnFocus", typeof(bool), typeof(SfTextBoxExt), new PropertyMetadata(false));

        

        /// <summary>
        /// Returns the value of the Selected item
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.SfTextBoxExt"/>
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(SfTextBoxExt), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemChanged)));  

        
        
        /// <summary>
        /// Returns a value based on SuggestionPredicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="search"></param>
        /// <param name="item"></param>
        /// <returns>
        /// <value>
        /// <c>true</c> if instance is created ; otherwise, <c>false</c>.
        /// </value>
        /// </returns>
        public delegate bool SuggestionPredicate<T>(string search, T item);

        /// <summary>
        /// Returns a SuggestionPredicate on filtering
        /// </summary>
        public SuggestionPredicate<object> Filter; 
                
        #endregion

        #region Helper methods

        private bool FilterSuggestions(string search, object item)
        {
            return (Filter(search, item));
        }
        private void DecrementAppendItem()
        {
            if (appendindex != 0)
            {
                appendindex--;
            }
            else
            {
                appendindex = appendItems.Count - 1;
            }
            AppendText();
        }

        private void IncrementAppendItem()
        {
            if (appendindex != appendItems.Count - 1)
            {
                appendindex++;
            }
            else
            {
                appendindex = 0;
            }
            AppendText();
        }

        private void SuggestionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AutoCompleteMode == AutoCompleteMode.SuggestAppend || AutoCompleteMode == AutoCompleteMode.Suggest)
            {
                if (PART_SuggestionBox.Items.Count > 0)
                {
                    if (PART_SuggestionBox.SelectedIndex != -1)
                    {
                        if (Delimeter != null && collection.Count > 1)
                        {
                           Text = UpdateText();
                        }
                        else
                        {
                            Text = GetStringFromSearchItemPath(PART_SuggestionBox.SelectedItem, SearchItemPath);
                        }
                        SelectionStart = Text.Length;
                        cansuggest = false;
                    }
                }
            }
        }

        private void UpdateScrollViewer()
        {
            if (PART_SuggestionBox != null && PART_SuggestionBox.scrollViewer != null)
            {
                if (PART_SuggestionBox.SelectedIndex == 0 ||
                    PART_SuggestionBox.SelectedIndex == -1)
                    PART_SuggestionBox.scrollViewer.ScrollToVerticalOffset(0);
                else if (PART_SuggestionBox.SelectedIndex == PART_SuggestionBox.Items.Count - 1)
                    PART_SuggestionBox.scrollViewer.ScrollToVerticalOffset(PART_SuggestionBox.SelectedIndex);
            }
        }

        private String UpdateText()
        {
            String splitText = String.Empty;
            for (int i = 0; i < collection.Count - 1; i++)
            {
                splitText = splitText + collection[i].ToString() + Delimeter;
            }

            splitText = splitText + GetStringFromSearchItemPath(PART_SuggestionBox.SelectedItem, SearchItemPath);
            return splitText;
        }

        private void OpenSuggestion()
        {
         if (PART_SuggestionBox != null && (isFocussed || ShowSuggestionsOnFocus))
            {
                //Setting visual templates...
                if (AutoCompleteItemTemplate == null)
                {
                    PART_SuggestionBox.DisplayMemberPath = SearchItemPath;
                }
                else
                {
                    PART_SuggestionBox.ItemTemplate = AutoCompleteItemTemplate;
                    #if !(SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7)
                    if (AutoCompleteItemTemplateSelector != null)
                    {
                        PART_SuggestionBox.ItemTemplateSelector = AutoCompleteItemTemplateSelector;
                    }
                    #endif
                }

                //Setting panel...
                if (AutoCompleteItemsPanel != null)
                {
                    PART_SuggestionBox.ItemsPanel = AutoCompleteItemsPanel;
                }

                PART_SuggestionBox.autoComplete = this;

                if (!IsSuggestionOpen && SuggestionBoxPlacement != SuggestionBoxPlacement.None
                    && AutoCompleteMode != AutoCompleteMode.Append)
                {
                    //Hide popup until items get filled up...
                    PART_Popup.Opacity = 0.0;
#if WPFSILVERLIGHT
                    if (PART_SuggestionBox.Items.Count > 0)
                      Dispatcher.BeginInvoke(new Action(delegate
                  {
                      IsSuggestionOpen = true;
                  }));
#else
                    IsSuggestionOpen = true;
#endif

#if SILVERLIGHT
                    UpdatePopup();
#endif

                    if (!PART_SuggestionBox.ActualWidth.Equals(ActualWidth))
                        PART_SuggestionBox.MinWidth = ActualWidth;

                    //PART_SuggestionBox.MaxHeight = MaxDropDownHeight;
                }

            }
        }
#if SILVERLIGHT
        private void UpdatePopup()
        {
            if (PART_Popup != null && SuggestionBoxPlacement == SuggestionBoxPlacement.Top)
            {
                GeneralTransform gt = this.TransformToVisual(this);
                Point p = gt.Transform(new Point(0, this.ActualHeight));
                PART_SuggestionBox.UpdateLayout();
                Point offset = new Point(0,  - (PART_SuggestionBox.DesiredSize.Height)-p.Y);
                PART_Popup.VerticalOffset = offset.Y;

            }
        }
#endif
        private void UpdateSuggestionBox()
        {
            if (Suggestions == null || ((ICollection)Suggestions).Count == 0)
            {
                IsSuggestionOpen = false;
                return;
            }

            if (PART_SuggestionBox != null && PART_Popup!=null)
            {
                if (SuggestionBoxPlacement == SuggestionBoxPlacement.Top)
                {
#if WPF
                    PART_Popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Top;
#endif
                    PART_SuggestionBox.UpdateLayout();

                    var point = new Point();
                    #if WPFSILVERLIGHT
                    GeneralTransform buttonTransform = this.TransformToVisual(this);
                    #else
                    GeneralTransform buttonTransform = this.TransformToVisual(null);
                    #endif
                    #if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                    Point relativepoint = buttonTransform.Transform(new Point());
                    #else
                    Point relativepoint = buttonTransform.TransformPoint(new Point());
                    #endif
                    if (relativepoint.Y < PART_SuggestionBox.ActualHeight)
                    {
                        point.X = 0;
                    }
                    else
                    {
                        point.Y = -(PART_SuggestionBox.ActualHeight + ActualHeight);
                        point.X = 0;
                    }

                    //For Compatibility. Do not remove this line...
                    SuggestionBoxPosition = point;

                    PART_Popup.HorizontalOffset = point.X;
                    PART_Popup.VerticalOffset = point.Y;

                    //Items get filled up. Now show the popup...
                    PART_Popup.Opacity = 1;
                }
                else if (SuggestionBoxPlacement == SuggestionBoxPlacement.Bottom)
                {
#if WPF
                    PART_Popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
#endif
                    PART_SuggestionBox.UpdateLayout();
                    var point = new Point();
                    #if WPFSILVERLIGHT
                    GeneralTransform buttonTransform = this.TransformToVisual(this);
                    #else
                    GeneralTransform buttonTransform = this.TransformToVisual(null);
                    #endif
                    #if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                    Point relativepoint = buttonTransform.Transform(new Point());
                    #else
                    Point relativepoint = buttonTransform.TransformPoint(new Point());
                    #endif
                    #if WPF
                    double windowHeight = Application.Current.MainWindow.ActualHeight;
                    #elif SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                    double windowHeight = Application.Current.RootVisual.RenderSize.Height;
                    #else
                    double windowHeight = Window.Current.CoreWindow.Bounds.Height;
                    #endif
                    if ((relativepoint.Y + PART_SuggestionBox.ActualHeight + this.ActualHeight) >= windowHeight)
                    {
                        point.Y = -(PART_SuggestionBox.ActualHeight + ActualHeight);
                        point.X = 0;
                    }

                    //For Compatibility. Do not remove this line...
                    SuggestionBoxPosition = point;

                    PART_Popup.HorizontalOffset = point.X;
                    PART_Popup.VerticalOffset = point.Y;

                    //Items get filled up. Now show the popup...
                    PART_Popup.Opacity = 1;
                }

            }
        }
        #if (WPFSILVERLIGHT || WINRT)
        #if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        #else
        private async void OnTextChanged(object sender, TextChangedEventArgs e)
        #endif
        {
            UpdateWatermark();
            int selectionStart = SelectionStart;


            if (AutoCompleteSource != null)
            {

                var list = new List<object>();
                var stringcomparision = StringComparison.CurrentCulture;
                if (SuggestionMode == SuggestionMode.StartsWith || SuggestionMode == SuggestionMode.Contains || SuggestionMode == SuggestionMode.StartsWithOrdinal || SuggestionMode == SuggestionMode.ContainsOrdinal ||
                    SuggestionMode == SuggestionMode.Equals || SuggestionMode == SuggestionMode.EqualsOrdinal || SuggestionMode == SuggestionMode.None || SuggestionMode == SuggestionMode.Custom)
                    stringcomparision = StringComparison.OrdinalIgnoreCase;
                list.AddRange(from object item in AutoCompleteSource
                              where item != null
                              let strvalue = GetStringFromSearchItemPath(item, SearchItemPath)
                              where
                                  !String.IsNullOrEmpty(strvalue) &&
                                  strvalue.Equals(Text, stringcomparision)
                              select item);
                if (list.Count > 0)
                {
                    if (this.PART_SuggestionBox != null && this.PART_SuggestionBox.SelectedIndex >= 0 && list.Count > this.PART_SuggestionBox.SelectedIndex)
                        SelectedItem = list[this.PART_SuggestionBox.SelectedIndex];
                    else
                        SelectedItem = list[0];
                    if (SelectedText.Equals(string.Empty) && stringcomparision!=StringComparison.CurrentCulture && Text.Equals(GetStringFromSearchItemPath(SelectedItem, SearchItemPath), stringcomparision))
                    {
                        Text=GetStringFromSearchItemPath(SelectedItem, SearchItemPath);
                        SelectionStart = Text.Length;
                    }
                }
                else
                {
                    SelectedItem = null;
                }
                if (Delimeter != null)
                {
                    collection.Clear();
                    String splitText= String.Empty;
                    String temp = Text;
                    foreach (char c in temp)
                    {
                        if (!c.ToString().Equals(Delimeter))
                        {
                            splitText = splitText + c;
                        }
                        else
                        {                            
                            collection.Add(splitText);
                            splitText = string.Empty;
                        }

                    }
                    if (!String.IsNullOrEmpty(splitText) && Text.Contains(Delimeter) && !String.IsNullOrWhiteSpace(splitText))
                    {
                        collection.Add(splitText);
                    }

                }
                if (PopupDelay == TimeSpan.Zero)
                {
                    if (Delimeter != null)
                    {
                        if (!Text.EndsWith(Delimeter))
                        {
                            #if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                            Dispatcher.BeginInvoke(new Action(FilterSuggestions));
                            #else
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, FilterSuggestions);
                            #endif
                        }
                        else
                        {
                            if (IsSuggestionOpen)
                                IsSuggestionOpen = false;
                        }
                    }
                    else
                    {
                        if (Text.Length >= MinimumPrefixCharacters)
                        {
                            #if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                            if (cansuggest)
                            {
                                Dispatcher.BeginInvoke(new Action(FilterSuggestions));
                            }
                            #else
                            if(cansuggest)
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, FilterSuggestions);
                            #endif
                        }
                        else
                        {
                            if(ShowSuggestionsOnFocus)
#if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                                 Dispatcher.BeginInvoke(new Action(FilterSuggestions));
#else
                                 await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, FilterSuggestions);
#endif                          
                            if (IsSuggestionOpen)
                                IsSuggestionOpen = false;
                        }
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(Text))
                    {
                        popupTimer.Stop();
                        popupTimer.Tick -= PopupTimerTick;
                        if (IsSuggestionOpen)
                            IsSuggestionOpen = false;
                    }

                    if (popupTimer == null)
                    {
                        popupTimer = new DispatcherTimer();
                    }
                    popupTimer.Interval = PopupDelay;
                    popupTimer.Tick += PopupTimerTick;
                    popupTimer.Start();
                }
            }
        }
#endif
#if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        private void PopupTimerTick(object sender, object e)
#else
        private async void PopupTimerTick(object sender, object e)
        #endif
        {
            popupTimer.Stop();
            popupTimer.Tick -= PopupTimerTick;

            if (!String.IsNullOrEmpty(Text))
            {
                if (Text.Length >= MinimumPrefixCharacters)
                {
                    #if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                    Dispatcher.BeginInvoke(new Action(FilterSuggestions));
                    #else
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, FilterSuggestions);
                    #endif
                }
                else
                {
                    if (IsSuggestionOpen)
                        IsSuggestionOpen = false;
                }
            }
        }

        private string GetStringFromSearchItemPath(object item, string path)
        {
            if (!String.IsNullOrEmpty(path))
            {
                #if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                PropertyInfo info = item.GetType().GetProperties().ToList().FirstOrDefault(_info => MatchPropertyInfo(_info, path));
                if (info != null && info.GetValue(item,null) != null)
                {
                    String trimmedText = info.GetValue(item,null).ToString().TrimStart();
                    return trimmedText;
                }
                #else
                PropertyInfo info = item.GetType().GetRuntimeProperties().ToList().FirstOrDefault(_info => MatchPropertyInfo(_info, path));
                if (info != null && info.GetValue(item) != null)
                {
                    String trimmedText = info.GetValue(item).ToString().TrimStart();
                    return trimmedText;
                }
                #endif
                else
                {
                    if (item != null)
                    {
                        return item.ToString().TrimStart();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            else
            {
                return item.ToString();
            }
        }

        private bool MatchPropertyInfo(PropertyInfo info, string path)
        {
            if (info.Name == path)
            {
                return true;
            }
            return false;
        }
        
        private void AppendText()
        {
            if (appendItems.Count > 0)
            {
                string appendtext = GetStringFromSearchItemPath(appendItems[appendindex], SearchItemPath).ToString();
                tempText = appendtext;
                string originaltext = String.Empty;
                if (SelectionLength > 0)
                {
                    if (Delimeter != null && collection.Count > 1)
                    {
                        int index = Math.Abs(SelectionLength - collection[collection.Count - 1].ToString().Length);
                        originaltext = GetStringFromSearchItemPath(collection[collection.Count - 1].ToString().Remove(index, SelectionLength),SearchItemPath);
                    }
                    else
                    {
                        originaltext = Text.Remove(SelectionStart, SelectionLength);
                    }
                }
                else
                {
                    if (Delimeter != null && collection.Count > 1)
                    {
                        originaltext = GetStringFromSearchItemPath(collection[collection.Count - 1], SearchItemPath);
                    }
                    else
                    {
                        originaltext = Text;
                    }
                }
                if (originaltext!=string.Empty && appendtext.StartsWith(originaltext, StringComparison.OrdinalIgnoreCase))
                    AppendText(originaltext, appendtext);
            }
        }

        private void AppendText(string originaltext, string appendtext)
        {
            if (SuggestionMode == SuggestionMode.StartsWith || SuggestionMode == SuggestionMode.Contains ||
                SuggestionMode == SuggestionMode.StartsWithOrdinal || SuggestionMode == SuggestionMode.ContainsOrdinal ||
                SuggestionMode == SuggestionMode.Equals || SuggestionMode == SuggestionMode.EqualsOrdinal ||
                SuggestionMode == SuggestionMode.None || SuggestionMode == SuggestionMode.Custom)
            {
                appendtext = appendtext.Remove(appendtext.IndexOf(originaltext, StringComparison.OrdinalIgnoreCase), originaltext.Length);
            }
            else
            {
                appendtext = appendtext.Remove(appendtext.IndexOf(originaltext), originaltext.Length);
            }
#if SILVERLIGHT && ! WINDOWS_PHONE && ! WINDOWS_PHONE_7
            int selectionStart = SelectionStart;
            SelectedText = appendtext;
            Select(selectionStart,appendtext.Length);
#else
            SelectedText = appendtext;
#endif
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Filters the <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.Suggestions"/> based the
        /// text.
        /// </summary>
        /// <remarks>
        /// The Return value is void.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        #if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        public void FilterSuggestions()
        #else
        public async void FilterSuggestions()
        #endif
        {
            if (AutoCompleteMode != AutoCompleteMode.None && AutoCompleteSource != null)
            {
                if (Suggestions != null && String.IsNullOrEmpty(Text))
                {
                    Suggestions = null;
                }

                if (String.IsNullOrEmpty(Text) && IsSuggestionOpen)
                {
                    IsSuggestionOpen = false;
                }

                if (canappend && (ShowSuggestionsOnFocus || ((!String.IsNullOrEmpty(Text) && !String.IsNullOrWhiteSpace(Text)))) && AutoCompleteMode == AutoCompleteMode.Append && String.IsNullOrEmpty(this.SelectedText))
                {
                    if (Text.Length >= MinimumPrefixCharacters || ShowSuggestionsOnFocus)
                    {
#if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                    var filtermodel = BeginFilter(AutoCompleteMode, SuggestionMode, AutoCompleteSource, SearchItemPath, Text, Delimeter);
#else
                        var filtermodel = await BeginFilter(AutoCompleteMode, SuggestionMode, AutoCompleteSource, SearchItemPath, Text, Delimeter);
#endif

                        if (appendItems == null)
                        {
                            appendItems = new List<object>();
                        }

                        if (((ICollection)filtermodel.Appends).Count > 0)
                        {
                            appendItems = ((IEnumerable<object>)filtermodel.Appends).ToList();
                            appendindex = 0;
                            AppendText();
                        }
                    }
                }

                if (cansuggest && ((ShowSuggestionsOnFocus || (!String.IsNullOrEmpty(Text) && !String.IsNullOrWhiteSpace(Text))) || SuggestionMode == SuggestionMode.None) && AutoCompleteMode == AutoCompleteMode.Suggest)
                {
                    if (Text.Length >= MinimumPrefixCharacters || ShowSuggestionsOnFocus)
                    {
#if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                    var filtermodel = BeginFilter(AutoCompleteMode, SuggestionMode, AutoCompleteSource, SearchItemPath, Text,Delimeter);
#else
                        var filtermodel = await BeginFilter(AutoCompleteMode, SuggestionMode, AutoCompleteSource, SearchItemPath, Text, Delimeter);
#endif

                        if (((ICollection)filtermodel.Suggestions).Count > 0)
                        {
                            originaltextforsuggestion = Text;
                        }
                        else
                        {
                            IsSuggestionOpen = false;
                        }

                        Suggestions = filtermodel.Suggestions;
                        UpdateSuggestionBox();
#if SILVERLIGHT
                    UpdatePopup();
#endif
                    }
                }

                if (!isCustom &&  (ShowSuggestionsOnFocus || ((!String.IsNullOrEmpty(Text) && !String.IsNullOrWhiteSpace(Text)))) && AutoCompleteMode == AutoCompleteMode.SuggestAppend && String.IsNullOrEmpty(this.SelectedText))
                {
                    if (Text.Length >= MinimumPrefixCharacters || ShowSuggestionsOnFocus)
                    {
#if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                    var filtermodel = BeginFilter(AutoCompleteMode, SuggestionMode, AutoCompleteSource, SearchItemPath, Text, Delimeter);
#else
                        var filtermodel = await BeginFilter(AutoCompleteMode, SuggestionMode, AutoCompleteSource, SearchItemPath, Text, Delimeter);
#endif

                        if (appendItems == null)
                        {
                            appendItems = new List<object>();
                        }

                        if (filtermodel.Appends != null && ((ICollection)filtermodel.Appends).Count > 0 && canappend)
                        {
                            appendItems = ((IEnumerable<object>)filtermodel.Appends).ToList();
                            appendindex = 0;
                            AppendText();
                        }

                        if (cansuggest)
                        {

                            if (filtermodel.Suggestions != null && ((ICollection)filtermodel.Suggestions).Count > 0)
                            {
                                originaltextforsuggestion = Text;
                                cansuggest = false;
                            }
                            else
                            {
                                IsSuggestionOpen = false;
                            }
                            Suggestions = filtermodel.Suggestions;
                            UpdateSuggestionBox();
#if SILVERLIGHT
                    UpdatePopup();
#endif
                        }
                    }
                }
                if (SuggestionMode == SuggestionMode.None)
                {
                    cansuggest = true;
                    //UpdateSuggestionBox(); 
                }
                UpdateScrollViewer();
            }
        }
        #if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        private FilterModel BeginFilter(AutoCompleteMode autoCompleteMode, SuggestionMode suggestionMode, IEnumerable source, string path, string text, String delimeter)
        #else
        private async Task<FilterModel> BeginFilter(AutoCompleteMode autoCompleteMode, SuggestionMode suggestionMode, IEnumerable source, string path, string text,String delimeter)
        #endif
        {
#if !Silverlight4
            var task = new TaskCompletionSource<FilterModel>();
#endif
            #if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            Dispatcher.BeginInvoke(new Action(OpenSuggestion));
#if !Silverlight4
            task.SetResult(ApplySuggestionMode(autoCompleteMode, suggestionMode, source, path, text, delimeter));
#endif
            #else
            await ThreadPool.RunAsync(delegate
                                          {
                                              Dispatcher.RunAsync(CoreDispatcherPriority.Normal, OpenSuggestion).AsTask();
                                              task.SetResult(ApplySuggestionMode(autoCompleteMode, suggestionMode,
                                                                                 source, path, text,delimeter));
                                              //Dispatcher.RunAsync(CoreDispatcherPriority.Normal, UpdateSuggestionBox);
                                          });
            #endif
#if !Silverlight4
            return task.Task.Result;
#else
            return ApplySuggestionMode(autoCompleteMode, suggestionMode, source, path, text, delimeter);
#endif
        }

        private FilterModel ApplySuggestionMode(AutoCompleteMode autoCompleteMode, SuggestionMode suggestionMode, IEnumerable source, string path, string text, String delimeter)
        {
            var suggestions = new List<object>();
            var appends = new List<object>();

            if (delimeter != null && collection.Count > 1)
            {
                text = GetStringFromSearchItemPath(collection[collection.Count - 1],path);
            }
            //Let's work for Append mode first. Append and SuggestAppend only needs "appends" collection.
            if (autoCompleteMode == AutoCompleteMode.Append || autoCompleteMode == AutoCompleteMode.SuggestAppend)
            {
                var list = new List<object>();

                //text will append only for "StartsWith" option. So let's retrict the append only to following four options.
                //1. StartsWith
                //2. StartsWithCaseSensitive
                //3. StartsWithOrdinal
                //4. StartsWithOrdinalCaseSensitive
                if (suggestionMode == SuggestionMode.StartsWith)
                {
                    list.AddRange(from object item in source
                                  where item != null
                                  let strvalue = GetStringFromSearchItemPath(item, path)
                                  where
                                      !String.IsNullOrEmpty(strvalue) &&
                                      strvalue.StartsWith(text, StringComparison.CurrentCultureIgnoreCase) &&
                                      !String.Equals(text, strvalue, StringComparison.CurrentCultureIgnoreCase)
                                  select item);
                }
                else if (suggestionMode == SuggestionMode.StartsWithCaseSensitive)
                {
                    list.AddRange(from object item in source
                                  where item != null
                                  let strvalue = GetStringFromSearchItemPath(item, path)
                                  where
                                      !String.IsNullOrEmpty(strvalue) &&
                                      strvalue.StartsWith(text, StringComparison.CurrentCulture) &&
                                      !String.Equals(text, strvalue, StringComparison.CurrentCulture)
                                  select item);
                }
                else if (suggestionMode == SuggestionMode.StartsWithOrdinal)
                {
                    list.AddRange(from object item in source
                                  where item != null
                                  let strvalue = GetStringFromSearchItemPath(item, path)
                                  where
                                      !String.IsNullOrEmpty(strvalue) &&
                                      strvalue.StartsWith(text, StringComparison.OrdinalIgnoreCase) &&
                                      !String.Equals(text, strvalue, StringComparison.OrdinalIgnoreCase)
                                  select item);
                }
                else if (suggestionMode == SuggestionMode.StartsWithOrdinalCaseSensitive)
                {
                    list.AddRange(from object item in source
                                  where item != null
                                  let strvalue = GetStringFromSearchItemPath(item, path)
                                  where
                                      !String.IsNullOrEmpty(strvalue) &&
                                      strvalue.StartsWith(text, StringComparison.Ordinal) &&
                                      !String.Equals(text, strvalue, StringComparison.Ordinal)
                                  select item);
                }

                appends = list;
            }

            //Now it is Suggest time. Suggest and SuggestAppend need "suggestions" collection.
            if (autoCompleteMode == AutoCompleteMode.Suggest || autoCompleteMode == AutoCompleteMode.SuggestAppend)
            {
                //text Suggestion will work for all suggestion modes. And we already have the collection for four "StartsWith" modes. :). 
                //But we have to check for Suggest mode.
                if (suggestionMode == SuggestionMode.StartsWith ||
                    suggestionMode == SuggestionMode.StartsWithCaseSensitive ||
                    suggestionMode == SuggestionMode.StartsWithOrdinal ||
                    suggestionMode == SuggestionMode.StartsWithOrdinalCaseSensitive)
                {
                    suggestions = appends;

                    if (autoCompleteMode == AutoCompleteMode.Suggest)
                    {
                        var list = new List<object>();

                        if (suggestionMode == SuggestionMode.StartsWith)
                        {
                            list.AddRange(from object item in source
                                          where item != null
                                          let strvalue = GetStringFromSearchItemPath(item, path)
                                          where
                                              !String.IsNullOrEmpty(strvalue) &&
                                              strvalue.StartsWith(text, StringComparison.CurrentCultureIgnoreCase) &&
                                              !String.Equals(text, strvalue, StringComparison.CurrentCultureIgnoreCase)
                                          select item);
                        }
                        else if (suggestionMode == SuggestionMode.StartsWithCaseSensitive)
                        {
                            list.AddRange(from object item in source
                                          where item != null
                                          let strvalue = GetStringFromSearchItemPath(item, path)
                                          where
                                              !String.IsNullOrEmpty(strvalue) &&
                                              strvalue.StartsWith(text, StringComparison.CurrentCulture) &&
                                              !String.Equals(text, strvalue, StringComparison.CurrentCulture)
                                          select item);
                        }
                        else if (suggestionMode == SuggestionMode.StartsWithOrdinal)
                        {
                            list.AddRange(from object item in source
                                          where item != null
                                          let strvalue = GetStringFromSearchItemPath(item, path)
                                          where
                                              !String.IsNullOrEmpty(strvalue) &&
                                              strvalue.StartsWith(text, StringComparison.OrdinalIgnoreCase) &&
                                              !String.Equals(text, strvalue, StringComparison.OrdinalIgnoreCase)
                                          select item);
                        }
                        else if (suggestionMode == SuggestionMode.StartsWithOrdinalCaseSensitive)
                        {
                            list.AddRange(from object item in source
                                          where item != null
                                          let strvalue = GetStringFromSearchItemPath(item, path)
                                          where
                                              !String.IsNullOrEmpty(strvalue) &&
                                              strvalue.StartsWith(text, StringComparison.Ordinal) &&
                                              !String.Equals(text, strvalue, StringComparison.Ordinal)
                                          select item);
                        }

                        suggestions = list;
                    }
                }
                else
                {
                    var list = new List<object>();

                    //Following modes should work with suggestion.
                    if (suggestionMode == SuggestionMode.Contains)
                    {
                        list.AddRange(from object item in source
                                      where item != null
                                      let strvalue = GetStringFromSearchItemPath(item, path)
                                      where
                                          !String.IsNullOrEmpty(strvalue) &&
                                          strvalue.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 &&
                                          !String.Equals(text, strvalue, StringComparison.CurrentCultureIgnoreCase)
                                      select item);
                    }
                    else if (suggestionMode == SuggestionMode.ContainsCaseSensitive)
                    {
                        list.AddRange(from object item in source
                                      where item != null
                                      let strvalue = GetStringFromSearchItemPath(item, path)
                                      where
                                          !String.IsNullOrEmpty(strvalue) && strvalue.IndexOf(text, StringComparison.CurrentCulture) >= 0 &&
                                      !String.Equals(strvalue, text, StringComparison.CurrentCulture)
                                      select item); 
                    }
                    else if (suggestionMode == SuggestionMode.ContainsOrdinal)
                    {
                        list.AddRange(from object item in source
                                      where item != null
                                      let strvalue = GetStringFromSearchItemPath(item, path)
                                      where
                                          !String.IsNullOrEmpty(strvalue) && strvalue.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                      !String.Equals(strvalue, text, StringComparison.OrdinalIgnoreCase)
                                      select item); 

                    }
                    else if (suggestionMode == SuggestionMode.ContainsOrdinalCaseSensitive)
                    {
                        list.AddRange(from object item in source
                                      where item != null
                                      let strvalue = GetStringFromSearchItemPath(item, path)
                                      where
                                          !String.IsNullOrEmpty(strvalue) && strvalue.IndexOf(text, StringComparison.Ordinal) >= 0 &&
                                      !String.Equals(strvalue, text, StringComparison.Ordinal)
                                      select item);                        

                    }
                    else if (suggestionMode == SuggestionMode.StartsWithCaseSensitive)
                    {
                        list.AddRange(from object item in source
                                      where item != null
                                      let strvalue = GetStringFromSearchItemPath(item, path)
                                      where
                                          !String.IsNullOrEmpty(strvalue) &&
                                          strvalue.IndexOf(text, StringComparison.CurrentCulture) >= 0 &&
                                          !String.Equals(text, strvalue, StringComparison.CurrentCulture)
                                      select item);
                    }
                    else if (suggestionMode == SuggestionMode.StartsWithOrdinal)
                    {
                        list.AddRange(from object item in source
                                      where item != null
                                      let strvalue = GetStringFromSearchItemPath(item, path)
                                      where
                                          !String.IsNullOrEmpty(strvalue) &&
                                          strvalue.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                          !String.Equals(text, strvalue, StringComparison.OrdinalIgnoreCase)
                                      select item);
                    }
                    else if (suggestionMode == SuggestionMode.StartsWithOrdinalCaseSensitive)
                    {
                        list.AddRange(from object item in source
                                      where item != null
                                      let strvalue = GetStringFromSearchItemPath(item, path)
                                      where
                                          !String.IsNullOrEmpty(strvalue) &&
                                          strvalue.IndexOf(text, StringComparison.Ordinal) >= 0 &&
                                          !String.Equals(text, strvalue, StringComparison.Ordinal)
                                      select item);
                    }
                    else if (suggestionMode == SuggestionMode.Equals)
                    {
                        list.AddRange(from object item in source
                                      where item != null
                                      let strvalue = GetStringFromSearchItemPath(item, path)
                                      where
                                          !String.IsNullOrEmpty(strvalue) &&
                                          String.Equals(text, strvalue, StringComparison.CurrentCultureIgnoreCase)
                                      select item);
                    }
                    else if (suggestionMode == SuggestionMode.EqualsCaseSensitive)
                    {
                        list.AddRange(from object item in source
                                      where item != null
                                      let strvalue = GetStringFromSearchItemPath(item, path)
                                      where
                                          !String.IsNullOrEmpty(strvalue) &&
                                          String.Equals(text, strvalue, StringComparison.CurrentCulture)
                                      select item);
                    }
                    else if (suggestionMode == SuggestionMode.EqualsOrdinal)
                    {
                        list.AddRange(from object item in source
                                      where item != null
                                      let strvalue = GetStringFromSearchItemPath(item, path)
                                      where
                                          !String.IsNullOrEmpty(strvalue) &&
                                          String.Equals(text, strvalue, StringComparison.OrdinalIgnoreCase)
                                      select item);
                    }
                    else if (suggestionMode == SuggestionMode.EqualsOrdinalCaseSensitive)
                    {
                        list.AddRange(from object item in source
                                      where item != null
                                      let strvalue = GetStringFromSearchItemPath(item, path)
                                      where
                                          !String.IsNullOrEmpty(strvalue) &&
                                          String.Equals(text, strvalue, StringComparison.Ordinal)
                                      select item);
                    }
                    else if (suggestionMode == SuggestionMode.Custom && Filter!=null)
                    {
                        list.AddRange(source.Cast<object>().Where(item => Filter(text, item)));
                    }

                    suggestions = list;
                }
            }

            if (suggestionMode == SuggestionMode.None)
            {
                suggestions = (from object item in source
                              where item != null
                              select item).ToList();
            }

            var model = new FilterModel();
            model.Appends = appends;
            model.Suggestions = suggestions;
            return model;
        }


        #endregion

        #region Override Methods
#if WPF
        ScrollViewer scrollviewer;
        /// <summary>
        /// Occurs when the key is pressed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
#elif SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Occurs when the key is pressed
        /// </summary>
        /// <param name="e"></param>
            protected override void OnKeyDown(KeyEventArgs e)
#else
        /// <summary>
        /// Occurs when the key is pressed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
#endif
        {
            canappend = true;

            if ((AutoCompleteMode == AutoCompleteMode.Suggest || AutoCompleteMode == AutoCompleteMode.SuggestAppend))
            {
#if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                if (e.Key == Key.Enter)
#else
                if (e.Key == Windows.System.VirtualKey.Enter)
#endif
                {
                    if (IsSuggestionOpen)
                    {
                        IsSuggestionOpen = false;
                        SelectionStart = Text.Length;
                        SelectionLength = 0;
                        e.Handled = true;
                        return;
                    }
                    cansuggest = true;

                }
#if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                else if (e.Key == Key.Down)
#else
                else if (e.Key == Windows.System.VirtualKey.Down)
#endif
                {
                    if (IsSuggestionOpen)
                    {
                        if (SuggestionIndex == Suggestions.Cast<object>().ToList().Count - 1)
                        {
                            if (string.IsNullOrEmpty(originaltextforsuggestion))
                                SuggestionIndex = 0;
                            else
                            {
                                SuggestionIndex = -1;
                                Text = originaltextforsuggestion;
                                SelectionStart = Text.Length;
                                SelectionLength = 0;
                            }
                        }
                        else
                        {
                            SuggestionIndex++;
                        }
#if WPFSILVERLIGHT
                        PART_SuggestionBox.SelectedIndex = SuggestionIndex;
#endif
#if WPF
                        //To move the selection item in to view
                        UpdateSelectionOnKeyDown(SuggestionIndex);
#endif
                        canappend = false;
                        cansuggest = false;
                        e.Handled = true;
                        return;
                    }
                }
#if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                else if (e.Key == Key.Up)
#else
                else if (e.Key == Windows.System.VirtualKey.Up)
#endif
                {
                    if (IsSuggestionOpen)
                    {
                        if (SuggestionIndex == -1)
                        {
                            SuggestionIndex = Suggestions.Cast<object>().ToList().Count - 1;
#if WINRT
                            if (PART_SuggestionBox != null)
                            {
                                ScrollViewer scrollviewer = GetVisualChild<ScrollViewer>(PART_SuggestionBox);
                                if (scrollviewer != null)
                                    scrollviewer.ScrollToVerticalOffset(SuggestionIndex);
                            }
#endif
                        }
                        else
                        {
                            SuggestionIndex--;
                            if (SuggestionIndex == -1)
                            {
                                if (string.IsNullOrEmpty(originaltextforsuggestion))
                                    SuggestionIndex = Suggestions.Cast<object>().ToList().Count - 1;
                                else
                                {
                                    Text = originaltextforsuggestion;
                                    SelectionStart = Text.Length;
                                    SelectionLength = 0;
                                }
                            }
                        }
#if WPFSILVERLIGHT
                        PART_SuggestionBox.SelectedIndex = SuggestionIndex;
#endif
#if WPF
                        //To move the selection item in to view
                        UpdateSelectionOnKeyDown(SuggestionIndex);
#endif
                        canappend = false;
                        cansuggest = false;
                        e.Handled = true;
                        return;
                    }
                }
                else
                {
#if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                    if (e.Key == Key.Escape && IsSuggestionOpen)
#else
                    if (e.Key == Windows.System.VirtualKey.Escape && IsSuggestionOpen)
#endif
                    {
                        IsSuggestionOpen = false;
                        e.Handled = true;
                        return;
                    }
                    cansuggest = true;
                }
            }

            if (AutoCompleteMode != AutoCompleteMode.None)
            {
                if (AutoCompleteMode == AutoCompleteMode.Append || AutoCompleteMode == AutoCompleteMode.SuggestAppend)
                {
                    if (AutoCompleteMode == AutoCompleteMode.Append)
                    {
#if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                        if (e.Key == Key.Down && appendItems != null && appendItems.Count > 0)
#else
                        if (e.Key == Windows.System.VirtualKey.Down && appendItems != null && appendItems.Count > 0)
#endif
                        {
                            IncrementAppendItem();
                            canappend = false;
                            e.Handled = true;
                            return;
                        }
#if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                        if (e.Key == Key.Up && appendItems != null && appendItems.Count > 0)
#else
                        if (e.Key == Windows.System.VirtualKey.Up && appendItems != null && appendItems.Count > 0)
#endif
                        {
                            DecrementAppendItem();
                            canappend = false;
                            e.Handled = true;
                            return;
                        }
                    }
#if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                    if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Enter)
                    {
                        if (e.Key == Key.Enter)
#else
                        if (e.Key == Windows.System.VirtualKey.Back || e.Key == Windows.System.VirtualKey.Delete || e.Key == Windows.System.VirtualKey.Enter)
                        {
                        if (e.Key == Windows.System.VirtualKey.Enter)
#endif
                        {

#if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                            if (e.Key == Key.Enter && appendItems != null && appendItems.Count > 0)
#else
                        if (e.Key == Windows.System.VirtualKey.Enter && appendItems != null && appendItems.Count > 0)
#endif
                            {
                                SelectionStart = Text.Length;
                            }
                            canappend = false;
                        }
                        else
                        {
#if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                            if (e.Key != Key.Up && e.Key != Key.Down)
#else
                        if (e.Key != Windows.System.VirtualKey.Up && e.Key != Windows.System.VirtualKey.Down)

#endif
                            {
                                #if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                                if (e.Key == Key.Back)
                                #else
                                if (e.Key == Windows.System.VirtualKey.Back)
                                #endif
                                {
                                    canappend = false;
                                }
                                else
                                {
                                    canappend = true;
                                }
                            }
                        }

                        if (appendItems != null && appendItems.Count > 0)
                        {
                            appendItems.Clear();
                        }

                    }
                }
            }

            base.OnKeyDown(e);
        }

#if WINRT
        private static T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            T child = default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                object v = (object)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v as DependencyObject);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
#endif
        /// <summary>
        /// Occurs when the pointer is pressed
        /// </summary>
        /// <param name="e"></param>
#if WPF
        protected override void OnMouseDown(MouseButtonEventArgs e)      
#elif SILVERLIGHT
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
#else
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
#endif
        {
#if WPF
            base.OnMouseDown(e);
#elif SILVERLIGHT
            base.OnMouseLeftButtonDown(e);
#else
            base.OnPointerPressed(e);
#endif
        }

        /// <summary>
        /// Occurs when the pointer is released
        /// </summary>
        /// <param name="e"></param>
        #if WPF
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
 	        base.OnMouseUp(e);
        }
        #elif SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
        }
        #else
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (!AllowPointerEvents)
            {
                base.OnPointerReleased(e);
            }
        }
        #endif
        #endregion

        #region Callback Methods

        /// <summary>
        /// Occurs when the value of MinimumPrefixCharacters have changed
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnMinimumPrefixCharactersChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((SfTextBoxExt)obj != null && (int)args.NewValue < 1)
            {
                throw new ArgumentOutOfRangeException("MinimumPrefixCharacters should be greater than 0");
            }
        }

        /// <summary>
        /// Occurs when the Suggestions have changed.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnSuggestionsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((SfTextBoxExt)obj != null)
            {
                ((SfTextBoxExt)obj).OnSuggestionsChanged(args);
            }
        }

        /// <summary>
        /// Occurs when the Suggestions have changed.
        /// </summary>
        /// <param name="args"></param>
        protected void OnSuggestionsChanged(DependencyPropertyChangedEventArgs args)
        {
            if (SuggestionsChanged != null)
            {
                SuggestionsChanged(this, args);
            }
        }

        /// <summary>
        /// Occurs when the Selected Item has changed.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((SfTextBoxExt)obj != null)
            {
                ((SfTextBoxExt)obj).OnSelectedItemChanged(args);
            }
        }

        /// <summary>
        /// Occurs when the Selected Item has changed.
        /// </summary>
        /// <param name="args"></param>
        protected void OnSelectedItemChanged(DependencyPropertyChangedEventArgs args)
       {
           if ((String.IsNullOrEmpty(Text) || String.IsNullOrWhiteSpace(Text)) && SelectedItem != null)
           {
               Text = GetStringFromSearchItemPath(args.NewValue, SearchItemPath);
           }
            if (SelectedItemChanged != null)
            {
                SelectedItemChanged(this, args);
            }
        }

        /// <summary>
        /// Occurs when the IsSuggestionOpen has changed.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnIsSuggestionOpenChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((SfTextBoxExt)obj != null)
            {
                ((SfTextBoxExt)obj).OnIsSuggestionOpenChanged(args);
            }
        }

        /// <summary>
        /// Occurs when the IsSuggestionOpen has changed.
        /// </summary>
        /// <param name="args"></param>
        protected void OnIsSuggestionOpenChanged(DependencyPropertyChangedEventArgs args)
        {
            if ((bool)(args.NewValue))
            {
                if (SuggestionPopupOpened != null)
                {
                    SuggestionPopupOpened(this, args);
                }
            }
            else
            {
                if (SuggestionPopupClosed != null)
                {
                    SuggestionPopupClosed(this, args);
                }
            }
            #if !(WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7)
            if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
            {
                if (SuggestionBoxPlacement == SuggestionBoxPlacement.Bottom)
                {
                    VisualStateManager.GoToState(this, "BottomDark", true);
                }
                else if (SuggestionBoxPlacement == SuggestionBoxPlacement.Top)
                {
                    VisualStateManager.GoToState(this, "TopDark", true);
                }
            }
            else
            {
                if (SuggestionBoxPlacement == SuggestionBoxPlacement.Bottom)
                {
                    VisualStateManager.GoToState(this, "BottomLight", true);
                }
                else if (SuggestionBoxPlacement == SuggestionBoxPlacement.Top)
                {
                    VisualStateManager.GoToState(this, "TopLight", true);
                }
            }
            #endif
        }

#if WPF
        private void UpdateSelectionOnKeyDown(int index)
        {
            if (PART_Popup.IsOpen && index >= 0 && index <= this.PART_SuggestionBox.Items.Count-1)
            {
                if (PART_SuggestionBox.ItemContainerGenerator.ContainerFromIndex(index) == null)
                {
                    if (scrollviewer != null)
                        scrollviewer.ScrollToVerticalOffset(index);
                    PART_SuggestionBox.UpdateLayout();
                }
                if(PART_SuggestionBox.ItemContainerGenerator.ContainerFromIndex(index)!=null)
                    (PART_SuggestionBox.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem).BringIntoView();
            }
        }
#endif
        /// <summary>
        /// Invoked when the Delimeter has changed.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnDeliMeterChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SfTextBoxExt instance = (SfTextBoxExt)obj;
            if (instance != null)
                instance.OnDelimeterChanged(args);            
        }

        /// <summary>
        /// Invoked when the AutoCompleteSource has changed.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnAutoCompleteSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SfTextBoxExt instance = (SfTextBoxExt)obj;
            if (instance != null)
            {
                instance.Suggestions = null;
                instance.SelectedText = string.Empty;
                instance.SelectedItem = null;
                instance.SuggestionIndex = -1;
                instance.Text = string.Empty;
            }
        }
#if WPF
        [CLSCompliant(false)]
#endif
        /// <summary>
        /// Prepares the text when the Delimeter has changed.
        /// </summary>
        /// <param name="args"></param>
        protected void OnDelimeterChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue != null && args.NewValue != null)
            {
                foreach (char ch in Text)
                {
                    if (ch == Convert.ToChar(args.OldValue))
                    {
                        int index = Text.IndexOf(ch);
                        StringBuilder builder = new StringBuilder(Text);
                        builder[index] = Convert.ToChar(args.NewValue);
                        Text = builder.ToString();
                    }
                }
            }
        }

        #endregion

        #region Events

       


        /// <summary>
        /// Occurs when <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.Suggestions"/> changed.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public event PropertyChangedCallback SuggestionsChanged;




        /// <summary>
        /// Occurs when <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.Suggestions"/> changed.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public event PropertyChangedCallback SelectedItemChanged;

        /// <summary>
        /// Occurs when suggestion box popup is opened.
        /// </summary>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.IsSuggestionOpen"/>
        [ClassReference(IsReviewed = false)]
        public event PropertyChangedCallback SuggestionPopupOpened;

        /// <summary>
        /// Occurs when suggestion box popup is closed.
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.IsSuggestionOpen"/>
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public event PropertyChangedCallback SuggestionPopupClosed;

        #endregion
    }

    /// <summary>
    /// Represents a class for the Filter Model.
    /// </summary>
    public class FilterModel
    {
        /// <summary>
        /// Gets or sets the Suggestions.
        /// </summary>
        public IEnumerable Suggestions { get; set; }

        /// <summary>
        /// Gets or sets the Appends.
        /// </summary>
        public IEnumerable Appends { get; set; }
    }
}
