#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.Win32;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Edit
{
    /// <summary>
    /// Represents Edit control. EditControl class contains properties and methods for
    /// users to customize and perform various activities related to EditControl.
    /// </summary>
    /// <remarks>
    /// <para>EditControl is an text or source code editor control. It can be used to
    /// create and modify text and source code files of supported file types.
    /// EditControl can be used to create editor applications like notepad or Microsoft
    /// Visual Studio IDE.</para>
    /// </remarks>
    /// <example>
    /// <para><b>XAML</b></para>
    /// <para></para>
    /// <para>&lt;Window x:Class=&quot;SampleApplication.Window1&quot;</para>
    /// <para> xmlns=&quot;http://
    /// schemas.microsoft.com/winfx/2006/xaml/presentation&quot;</para>
    /// <para> xmlns:x=&quot;http:// schemas.microsoft.com/winfx/2006/xaml&quot;</para>
    /// <para>    Title=&quot;Window1&quot; Height=&quot;300&quot; Width=&quot;300&quot;
    /// xmlns:syncfusion=&quot;http:// schemas.syncfusion.com/wpf&quot;&gt;</para>
    /// <para>    &lt;Grid&gt;</para>
    /// <para>        &lt;syncfusion:EditControl
    /// Name=&quot;editControl1&quot;/&gt;</para>
    /// <para>    &lt;/Grid&gt;</para>
    /// <para>&lt;/Window&gt;</para>
    /// <para></para>
    /// <para><b>C#</b></para>
    /// <para></para>
    /// <para>EditControl editcontrol = new EditControl();</para>
    /// </example>
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(true)]
#endif
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
    Type = typeof(EditControl), XamlResource = "/Syncfusion.Edit.Wpf;component/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(EditControl), XamlResource = "/Syncfusion.Edit.Wpf;component/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(EditControl), XamlResource = "/Syncfusion.Edit.Wpf;component/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(EditControl), XamlResource = "/Syncfusion.Edit.Wpf;component/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(EditControl), XamlResource = "/Syncfusion.Edit.Wpf;component/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(EditControl), XamlResource = "/Syncfusion.Edit.Wpf;component/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(EditControl), XamlResource = "/Syncfusion.Edit.Wpf;component/Themes/MetroStyle.xaml")]
    /// <summary>
    ///
    /// </summary>
    [ContentProperty("Text")]
    public class EditControl : Control
    {
        #region Local Variables

        /// <summary>
        /// object of LineItemsCollection
        /// </summary>
        private LineItemsCollection mlines;

        /// <summary>
        /// internal bool variable to identify if the Lines collection need to be re-initialized.
        /// </summary>
        internal bool isReinitializeLines;

        /// <summary>
        /// internal instance of EditScrollControl to store the reference of template child.
        /// </summary>
        internal EditScrollControl scrollControl;

        /// <summary>
        /// string variable to store copied or cut text when run in partial trust mode. Default value is empty
        /// </summary>
        private string clipboardtext = string.Empty;

        /// <summary>
        /// internal variable to check if there are still lines being added.
        /// </summary>
        internal bool isAddingLinesCompleted;

        /// <summary>
        /// internal variable to store the list of file extension of all languages supported.
        /// </summary>
        internal List<string> knownLanguages = null;

        /// <summary>
        /// internal variable to denote if the enter key has pressed.
        /// </summary>
        internal bool IsEnterKeyPressed = false;

        /// <summary>
        /// internal variable to denote if the space key has pressed.
        /// </summary>
        internal bool IsSpaceKeyPressed = false;

        /// <summary>
        /// internal variable to denote if the paste operation has perform.
        /// </summary>
        internal bool IsPaste = false;

        /// <summary>
        /// private variable to raise the expand items method when the cursor is idle.
        /// </summary>
        private DispatcherTimer inputtimer = null;

        /// <summary>
        /// local variable to hold reference to pin button in find results tab.
        /// </summary>
        private ToggleButton pinButton = null;

        /// <summary>
        /// local variable to hold reference to find results tabcontrol.
        /// </summary>
        private TabControl findResultsTab = null;

        /// <summary>
        /// local variable to hold reference to find results tabitem.
        /// </summary>
        private TabItem item = null;

        /// <summary>
        /// local variable to hold reference to listbox in the find results tab.
        /// </summary>
        private ListBox findResultsList = null;

        /// <summary>
        /// local variable to hold reference to find results titlebar.
        /// </summary>
        private Border findResultsTitleBar = null;

        /// <summary>
        /// local variable to hold reference to find replace popup control.
        /// </summary>
        private Window findReplaceWindow = null;

        /// <summary>
        /// local variable to hold reference to scrollviewer in the control template.
        /// </summary>
        private ScrollViewer viewer = null;

        /// <summary>
        /// internal variable to hold reference to listbox in the intellisense popup.
        /// </summary>
        internal ListBox intellisenseBox;

        /// <summary>
        /// internal variable to hold reference to intellisense popup.
        /// </summary>
        internal Popup intellisensePopup;

        /// <summary>
        /// internal variable to denote if the intellisense box is open.
        /// </summary>
        internal bool isIntellisenseBoxOpen = false;

        /// <summary>
        /// internal variable that denotes the selected intellisenseitem.
        /// </summary>
        internal EditTypeInfo selectedIntellisenseItem = null;

        /// <summary>
        /// internal variable to hold filter string typed by user
        /// </summary>
        internal string intellisenseFilterString = string.Empty;

        /// <summary>
        /// local DispatcherTimer to apply hide and view animations for find results tab.
        /// </summary>
        private DispatcherTimer findResultsTimer = null;

        /// <summary>
        /// local bool variable to identify if the find results tab has to be hidden.
        /// </summary>
        private bool isHideFindResultsTab = false;

        /// <summary>
        /// Local variable to store the intellisense args values.
        /// </summary>
        private EditIntellisenseArgs intellisenseArgs = null;

        /// <summary>
        /// Local variable to store the replace control
        /// </summary>
        private FindReplaceControl replaceControl = null;

        /// <summary>
        /// Local variable to store the previous selected intellisense item
        /// </summary>
        private IIntellisenseItem previousSelectedItem = null;

        /// <summary>
        /// Local variable to identify if the insert key is toggled.
        /// </summary>
        private bool isInsertKeyToggled = false;

        internal bool isIndentSelected = false;

        /// <summary>
        /// Local variable to identify if the assembly initialization process is going on.
        /// </summary>
        internal bool isAssemblyInitializing = false;

        internal bool AddUndoManager = true;

        /// <summary>
        /// When FindReplaceWindowOpened
        /// </summary>
        public event RoutedEventHandler FindReplaceWindowOpened;

        /// <summary>
        ///  When FindReplaceWindowClosed
        /// </summary>
        public event RoutedEventHandler FindReplaceWindowClosed;

        #endregion Local Variables

        #region Events Declaration

        /// <summary>
        /// Occurs when text in the EditControl is selected.
        /// </summary>
        public event PropertyChangedCallback SelectedTextChanged;

        /// <summary>
        /// Occurs when text in the EditControl is gets changed.
        /// </summary>
        public event PropertyChangedCallback TextChanged;

        /// <summary>
        /// Occurs when DocumentSource property gets changed.
        /// </summary>
        public event PropertyChangedCallback DocumentSourceChanged;

        /// <summary>
        /// Occurs before the IntellisenseBox is displayed.
        /// </summary>
        public event IntellisenseBoxEventHandler IntellisenseBoxOpening;

        /// <summary>
        /// Occurs when a intellisense drills down to sub -items
        /// </summary>
        public event IntellisenseBoxEventHandler IntellisenseDrillDown;

        #endregion Events Declaration

        #region Dependency Properties

        /// <summary>
        /// Gets or sets a value indicating text in EditControl.
        /// </summary>
        /// <remarks>
        /// <para>By default, it is set to string.Empty.</para>
        /// </remarks>
        /// <example>
        /// <para><b>XAML</b></para>
        /// <para> </para>
        /// <para>&lt;syncfusion:EditControl Name=&quot;editControl1&quot;
        /// Text=&quot;Setting Text Property&quot;/&gt;</para>
        /// <para> </para>
        /// <para><b>C#</b></para>
        /// <para> </para>
        /// <para>editControl1.Text = @&quot;Setting Text Property&quot;;</para>
        /// </example>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Gets or sets Language for the text in EditControl. Syntax highlighting and
        /// outlining of text in EditControl are performed based on the language set using
        /// this property.
        /// </summary>
        /// <remarks>
        /// <para>Specifies the Document language.</para>
        /// </remarks>
        /// <value>
        /// <para>Type: <see
        /// cref="T:Syncfusion.Windows.Edit.Languages">Syncfusion.Windows.Edit.Languages</see></para>
        /// </value>
        /// <example>
        /// <para><b>XAML</b></para>
        /// <para></para>
        /// <para>&lt;syncfusion:EditControl Name=&quot;editControl1&quot;
        /// DocumentLanguage=&quot;CSharp&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>editControl1.DocumentLanguage = Languages.VisualBasic;</para>
        /// </example>
        public Languages DocumentLanguage
        {
            get
            {
                return (Languages)GetValue(DocumentLanguageProperty);
            }

            set
            {
                SetValue(DocumentLanguageProperty, value);
            }
        }

        /// <summary>
        /// Gets and sets the height of each line in EditControl.
        /// </summary>
        /// <remarks>
        /// <para>The LineHeight property specifies the line height in the
        /// EditControl.</para>
        /// </remarks>
        public double LineHeight
        {
            get
            {
                return (double)GetValue(LineHeightProperty);
            }

            internal set
            {
                SetValue(LineHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets required width of the control. This property is used for internal
        /// to arrange and measure the control.
        /// </summary>
        /// <remarks>
        /// PreferredWidth calculate the preferred width of the EditControl.
        /// </remarks>
        internal double PreferredWidth
        {
            get
            {
                return (double)GetValue(PreferredWidthProperty);
            }

            set
            {
                SetValue(PreferredWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets source for EditControl. The filename specified will be loaded in
        /// the EditControl.
        /// </summary>
        /// <remarks>
        /// <para>Document Source of the EditControl.</para>
        /// </remarks>
        /// <example>
        /// <para><b>XAML</b></para>
        /// <para></para>
        /// <para>&lt;syncfusion:EditControl Name=&quot;editControl1&quot;
        /// DocumentSource=&quot;C:\test.txt&quot;&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>editControl1.DocumentSource = &quot;C:\test.txt&quot;;</para>
        /// </example>
        public string DocumentSource
        {
            get
            {
                return (string)GetValue(DocumentSourceProperty);
            }

            set
            {
                SetValue(DocumentSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether line number to be displayed or not.
        /// </summary>
        /// <remarks>
        /// <para>ShowLineNumber property checks the Line number visibility.</para>
        /// </remarks>
        /// <value>
        /// <para>Set <b>True</b> if Line number has to be displayed; otherwise,
        /// <b>false</b>. By default it is set to true.</para>
        /// </value>
        /// <example>
        /// <para><b>XAML</b></para>
        /// <para></para>
        /// <para>&lt;syncfusion:EditControl Name=&quot;editControl1&quot;
        /// ShowLineNumber=&quot;True&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>editControl1.ShowLineNumber = false;</para>
        /// </example>
        public bool ShowLineNumber
        {
            get
            {
                return (bool)GetValue(ShowLineNumberProperty);
            }

            set
            {
                SetValue(ShowLineNumberProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether read only mode is enabled or not.
        /// </summary>
        /// <remarks>
        /// <para>Set<b> True</b> if this read only has to be enabled; otherwise,
        /// <b>false</b>. By default, it is set to false.</para>
        /// </remarks>
        /// <example>
        /// <para><b>XAML</b></para>
        /// <para></para>
        /// <para>&lt;syncfusion:EditControl Name=&quot;editControl1&quot;
        /// IsReadOnly=&quot;True&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>editControl1.IsReadOnly = true;</para>
        /// </example>
        public bool IsReadOnly
        {
            get
            {
                return (bool)GetValue(IsReadOnlyProperty);
            }

            set
            {
                SetValue(IsReadOnlyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating Visibility of Horizontal ScrollBar.
        /// </summary>
        /// <remarks>
        /// <para>By default, it is set to Auto, wherein the ScrollBar will be visible when required.</para>
        /// </remarks>
        /// <example>
        /// <para><b>XAML</b></para>
        /// <para></para>
        /// <para>&lt;syncfusion:EditControl Name=&quot;editControl1&quot;
        /// HorizontalScrollBarVisibility=&quot;Visible&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>editControl1.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;</para>
        /// </example>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating Visibility of Vertical ScrollBar.
        /// </summary>
        /// <remarks>
        /// <para>By default, it is set to Auto, wherein the ScrollBar will be visible when required.</para>
        /// </remarks>
        /// <example>
        /// <para><b>XAML</b></para>
        /// <para></para>
        /// <para>&lt;syncfusion:EditControl Name=&quot;editControl1&quot;
        /// VerticalScrollBarVisibility=&quot;Visible&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>editControl1.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;</para>
        /// </example>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating the selected text of EditControl.
        /// </summary>
        /// <remarks>
        /// <para>It is readonly property</para>
        /// </remarks>
        /// <example>
        /// <para><b>XAML</b></para>
        /// <para> </para>
        /// <para>&lt;TextBox x:Name=&quot;textBox1&quot; Text=&quot;{Binding
        /// ElementName=editControl1, Path=SelectedText}&quot;/&gt;</para>
        /// <para> </para>
        /// <para><b>C#</b></para>
        /// <para> </para>
        /// <para>textBox1.Text = editControl1.SelectedText;</para>
        /// </example>
        public string SelectedText
        {
            get { return (string)GetValue(SelectedTextProperty); }
            internal set { SetValue(SelectedTextProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Outlining of text in EditControl should
        /// be applied.
        /// </summary>
        /// <remarks>
        /// <para>EnableOutlining property enables the outlining of text in EditControl.  It
        /// set <b>True</b> if outlining of the text in EditControl to be enabled ;
        /// otherwise, <b>false</b>. By default, it is set to true.</para>
        /// </remarks>
        /// <example>
        /// <para><b>XAML</b></para>
        /// <para></para>
        /// <para>&lt;syncfusion:EditControl Name=&quot;EditControl1&quot;
        /// EnableOutlining=&quot;True&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>EditControl1.EnableOutlining = false;</para>
        /// </example>
        public bool EnableOutlining
        {
            get
            {
                return (bool)GetValue(EnableOutliningProperty);
            }

            set
            {
                SetValue(EnableOutliningProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether built-in context menu should be
        /// displayed.
        /// </summary>
        /// <remarks>
        /// <para>ShowDefaultContextMenu sets <b>True</b> if built-in context menu has to be
        /// displayed; otherwise, <b>false</b>. By default, it is set to true. </para>
        /// </remarks>
        /// <example>
        /// <para><b>XAML</b></para>
        /// <para></para>
        /// <para>&lt;syncfusion:EditControl Name=&quot;EditControl1&quot;
        /// ShowDefaultContextMenu=&quot;False&quot;&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>EditControl1.ShowDefaultMenu = false;</para>
        /// </example>
        public bool ShowDefaultContextMenu
        {
            get
            {
                return (bool)GetValue(ShowDefaultContextMenuProperty);
            }

            set
            {
                SetValue(ShowDefaultContextMenuProperty, value);
            }
        }

        /// <summary>
        /// Get or set a value indicating whether Find And Replace Window has to be shown or not
        /// </summary>
        /// /// <para>ShowFindAndReplace property returns the Visibility mode of Find And replace.</para>
        /// <remark></remark>
        /// <value>
        /// <para>Set <b>True</b> if Find And Replace window has to be displayed;
        /// otherwise, <b>False</b>. By default, it is set to True.</para>
        /// </value>
        /// <example>
        /// <para><b>XAML</b></para>
        /// <para></para>
        /// <para>&lt;syncfusion:EditControl Name=&quot;EditControl1&quot;
        /// ShowFindAndReplace=&quot;True&quot;&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>EditControl1.ShowFindAndReplace = True;</para>
        /// </example>
        public bool ShowFindAndReplace
        {
            get
            {
                return (bool)GetValue(ShowFindAndReplaceProperty);
            }

            set
            {
                SetValue(ShowFindAndReplaceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance supports Undo operation or
        /// not.
        /// </summary>
        /// <remarks>
        /// <para><b>XAML</b></para>
        /// <para>&lt;EditControl x:Name=&quot;editControl1&quot;
        /// IsUndoEnabled=&quot;True&quot;/&gt;</para>
        /// <para> </para>
        /// <para><b>C#</b></para>
        /// <para>EditControl editControl1 = new EditControl();</para>
        /// <para>editControl1.IsUndoEnabled = true;</para>
        /// </remarks>
        /// <value>
        /// <see langword="true"/> if this instance allows undo operations to be performed ;
        /// otherwise, <see langword="false"/>.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.Windows.Edit.EditControl.IsRedoEnabled">IsRedoEnabled
        /// Property</seealso>
        public bool IsUndoEnabled
        {
            get { return (bool)GetValue(IsUndoEnabledProperty); }
            set { SetValue(IsUndoEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance supports Redo operation or
        /// not.
        /// </summary>
        /// <remarks>
        /// <para><b>XAML</b></para>
        /// <para>&lt;EditControl x:Name=&quot;editControl1&quot;
        /// IsRedoEnabled=&quot;True&quot;/&gt;</para>
        /// <para> </para>
        /// <para><b>C#</b></para>
        /// <para>EditControl editControl1 = new EditControl();</para>
        /// <para>editControl1.IsRedoEnabled = true;</para>
        /// </remarks>
        /// <value>
        /// <see langword="true"/> if this instance allows undo operations to be performed ;
        /// otherwise, <see langword="false"/>.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.Windows.Edit.EditControl.IsUndoEnabled">IsUndoEnabled
        /// Property</seealso>
        public bool IsRedoEnabled
        {
            get { return (bool)GetValue(IsRedoEnabledProperty); }
            set { SetValue(IsRedoEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating the number of spaces to included when the tab
        /// key is pressed.
        /// </summary>
        /// <remarks>
        /// By default, its value is set to 5.
        /// </remarks>
        /// <value>
        /// System.Int
        /// </value>
        /// <seealso cref="P:Syncfusion.Windows.Edit.EditControl.AcceptsTab">Accepts Tab
        /// Property</seealso>
        public int TabSpaces
        {
            get { return (int)GetValue(TabSpacesProperty); }
            set { SetValue(TabSpacesProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating the Visibility mode of the find results tab.
        /// </summary>
        /// <remarks>
        /// <para><b>XAML</b></para>
        /// <para>&lt;EditControl x:Name=&quot;editControl&quot;
        /// FindResultsTabVisibility=&quot;Visible&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.FindResultsTabVisibility = TabVisibility.Collapsed;</para>
        /// </remarks>
        /// <value>
        /// <see
        /// cref="T:Syncfusion.Windows.Edit.TabVisibility">T:Syncfusion.Windows.Edit.TabVisibility</see>
        /// </value>
        /// <seealso
        /// cref="T:Syncfusion.Windows.Edit.TabVisibility">T:Syncfusion.Windows.Edit.TabVisibility</seealso>
        public TabVisibility FindResultsTabVisibility
        {
            get { return (TabVisibility)GetValue(FindResultsTabVisibilityProperty); }
            set { SetValue(FindResultsTabVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating desired height of the find results tab..
        /// </summary>
        /// <remarks>
        /// <para><b>XAML</b></para>
        /// <para>&lt;EditControl x:Name=&quot;editControl1&quot;
        /// FindResultsTabHeight=&quot;200&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.FindResultsTabHeight=&quot;150&quot;;</para>
        /// <para></para>
        /// <para>By default, this property has a value of 150.</para>
        /// </remarks>
        /// <value>
        /// System.Double
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.Windows.Edit.EditControl.FindResultsTabVisibility">P:Syncfusion.Windows.Edit.EditControl.FindResultsTabVisibility</seealso>
        public double FindResultsTabHeight
        {
            get { return (double)GetValue(FindResultsTabHeightProperty); }
            set { SetValue(FindResultsTabHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Find Results Tab is closed or not .
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the find results tab in the EditControl is closed;
        /// otherwise, <see langword="false"/>.
        /// </value>
        public bool IsFindResultsTabClosed
        {
            get { return (bool)GetValue(IsFindResultsTabClosedProperty); }
            set { SetValue(IsFindResultsTabClosedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a collection of Uri specifying the prebuilt assemblies that can be
        /// used by the EditControl to display Classes, Properties, Events and Methods in
        /// the Auto intellisense mode.
        /// </summary>
        /// <remarks>
        /// List&lt;Uri&gt; uriList = new List&lt;Uri&gt;();
        /// <para>uriList.Add(new
        /// Uri(&quot;C:\Assemblies\Syncfusion.Edit.WPF.dll&quot;,UriKind.Absolute));</para>
        /// <para>uriList.Add(new
        /// Uri(&quot;C:\Assemblies\Syncfusion.Shared.WPF.dll&quot;,UriKind.Absolute));</para>
        /// <para>editControl1.AssemblyReferences = uriList;</para>
        /// </remarks>
        public IEnumerable<Uri> AssemblyReferences
        {
            get { return (IEnumerable<Uri>)GetValue(AssemblyReferencesProperty); }
            set { SetValue(AssemblyReferencesProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Intellisense is enabled in this instance
        /// of EditControl .
        /// </summary>
        /// <remarks>
        /// <para><b>XAML</b></para>
        /// <para>&lt;EditControl x:Name=&quot;editControl1&quot;
        /// EnableIntellisense=&quot;True&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.EnableIntellisense=false;</para>
        /// </remarks>
        /// <value>
        /// <see langword="true"/> if this instance support intellisense; otherwise, <see
        /// langword="false"/>.
        /// </value>
        public bool EnableIntellisense
        {
            get { return (bool)GetValue(EnableIntellisenseProperty); }
            set { SetValue(EnableIntellisenseProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating the intellisense mode. It supports two modes
        /// viz, Auto and Custom. In Auto mode, intellisense items are auto generated based
        /// on the language lexems, assembly references included. Where as in the Custom
        /// mode, users have to set the IntellisenseCustomItemsSource property to specify
        /// the items to be displayed in the intellisense.
        /// </summary>
        /// <remarks>
        /// <b>XAML</b>
        /// <para>&lt;EditControl x:Name=&quot;editControl1&quot;
        /// IntellisenseMode=&quot;Auto&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.IntellisenseMode = IntellisenseMode.Custom;</para>
        /// </remarks>
        /// <seealso
        /// cref="P:Syncfusion.Windows.Edit.EditControl.IntellisenseCustomItemsSource">P:Syncfusion.Windows.Edit.EditControl.IntellisenseCustomItemsSource</seealso>
        public IntellisenseMode IntellisenseMode
        {
            get { return (IntellisenseMode)GetValue(IntellisenseModeProperty); }
            set { SetValue(IntellisenseModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets an instance of LanguageBase type indicating the custom language.
        /// </summary>
        /// <seealso
        /// cref="T:Syncfusion.Windows.Edit.LanguageBase">T:Syncfusion.Windows.Edit.LanguageBase</seealso>
        /// <seealso
        /// cref="T:Syncfusion.Windows.Edit.ProceduralLanguageBase">T:Syncfusion.Windows.Edit.ProceduralLanguageBase</seealso>
        /// <seealso
        /// cref="T:Syncfusion.Windows.Edit.MarkupLanguageBase">T:Syncfusion.Windows.Edit.MarkupLanguageBase</seealso>
        public LanguageBase CustomLanguage
        {
            get
            {
                return (LanguageBase)GetValue(CustomLanguageProperty);
            }

            set
            {
                SetValue(CustomLanguageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance of EditControl accepts tab
        /// key in the content.
        /// </summary>
        /// <remarks>
        /// <para><b>XAML</b></para>
        /// <para>&lt;EditControl x:Name=&quot;editControl1&quot;
        /// AcceptsTab=&quot;True&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.AcceptsTab=false;</para>
        /// <para></para>
        /// <para>By default Accepts Tab property is set to true.</para>
        /// </remarks>
        /// <value>
        /// <see langword="true"/> if the control has to add additional spaces based on
        /// TabSpaces property; otherwise, <see langword="false"/> to move the focus to next
        /// focusable control.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.Windows.Edit.EditControl.AcceptsTab">AcceptsTab</seealso>
        public bool AcceptsTab
        {
            get { return (bool)GetValue(AcceptsTabProperty); }
            set { SetValue(AcceptsTabProperty, value); }
        }

        /// <summary>
        /// Gets or sets a DataTemplate to be applied to a IntellisenseBox.
        /// </summary>
        public DataTemplate IntellisenseItemTemplate
        {
            get { return (DataTemplate)GetValue(IntellisenseItemTemplateProperty); }
            set { SetValue(IntellisenseItemTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets a Style to be applied to a IntellisenseBox.
        /// </summary>
        public Style IntellisenseBoxStyle
        {
            get { return (Style)GetValue(IntellisenseBoxStyleProperty); }
            set { SetValue(IntellisenseBoxStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a collection of Business object inherited from IIntellisenseItem to
        /// be dispalyed in the IntellisenseBox.
        /// </summary>
        public IEnumerable IntellisenseCustomItemsSource
        {
            get { return (IEnumerable)GetValue(IntellisenseCustomItemsSourceProperty); }
            set { SetValue(IntellisenseCustomItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Hidden or Visible displayed.
        /// </summary>
        /// <remarks>
        /// <para>ShowOutlining property returns the Visibility mode of outlining.</para>
        /// </remarks>
        /// <value>
        /// <para>Set <b>Visible</b> if built-in context menu has to be displayed;
        /// otherwise, <b>Hidden</b>. By default, it is set to Visible.</para>
        /// </value>
        /// <example>
        /// <para><b>XAML</b></para>
        /// <para></para>
        /// <para>&lt;syncfusion:EditControl Name=&quot;EditControl1&quot;
        /// ShowDefaultContextMenu=&quot;Visible&quot;&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>EditControl1.ShowDefaultMenu = Visible;</para>
        /// </example>
        [Obsolete("This property is no longer in use and will be removed in future releases")]
        public Visibility ShowOutlining
        {
            get
            {
                return (Visibility)GetValue(ShowOutliningProperty);
            }

            set
            {
                SetValue(ShowOutliningProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating desired height of the Intellisense popup..
        /// </summary>
        /// <remarks>
        /// <para><b>XAML</b></para>
        /// <para>&lt;EditControl x:Name=&quot;editControl1&quot;
        /// IntellisensePopupHeight=&quot;200&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.IntellisensePopupHeight=&quot;150&quot;;</para>
        /// <para></para>
        /// <para>By default, this property has a value of 150.</para>
        /// </remarks>
        /// <value>
        /// System.Double
        /// </value>
        public double IntellisensePopupHeight
        {
            get { return (double)GetValue(IntellisensePopupHeightProperty); }
            set { SetValue(IntellisensePopupHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating desired width of Intellisense Popup.
        /// </summary>
        /// <remarks>
        /// <para><b>XAML</b></para>
        /// <para>&lt;EditControl x:Name=&quot;editControl1&quot;
        /// IntellisensePopupWidth=&quot;200&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.IntellisensePopupWidth=&quot;150&quot;;</para>
        /// <para></para>
        /// <para>By default, this property has a value of 150.</para>
        /// </remarks>
        /// <value>
        /// System.Double
        /// </value>
        public double IntellisensePopupWidth
        {
            get { return (double)GetValue(IntellisensePopupWidthProperty); }
            set { SetValue(IntellisensePopupHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Brush to be applied for Cursor.
        /// </summary>
        /// <remarks>
        /// <b>XAML</b>
        /// <para>&lt;syncfusion:EditControl x:Name=&quot;editControl1&quot;
        /// CaretBrush=&quot;Red&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.CaretBrush = Brushes.Green;</para>
        /// </remarks>
        public Brush CaretBrush
        {
            get { return (Brush)GetValue(CaretBrushProperty); }
            set { SetValue(CaretBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the vertical line in the expand collapse
        /// area should be displayed or not.
        /// </summary>
        /// <remarks>
        /// <b>XAML</b>
        /// <para>&lt;syncfusion:EditControl x:Name=&quot;editControl1&quot;
        /// ShowBlockIndicatorLine=&quot;False&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.ShowBlockIndicatorLine = true;</para>
        /// </remarks>
        /// <value>
        /// <see langword="true"/> if the vertical line has to be displayed; otherwise, <see
        /// langword="false"/>.
        /// </value>
        public bool ShowBlockIndicatorLine
        {
            get { return (bool)GetValue(ShowBlockIndicatorLineProperty); }
            set { SetValue(ShowBlockIndicatorLineProperty, value); }
        }

        /// <summary>
        /// Gets or sets to applied to the vertical line in the expand or collapse area.
        /// </summary>
        /// <remarks>
        /// <b>XAML</b>
        /// <para>&lt;syncfusion:EditControl x:Name=&quot;editControl1&quot;
        /// BlockIndicatorLineStroke=&quot;Red&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.BlockIndicatorLineStroke = Brushes.Green;</para>
        /// </remarks>
        public Brush BlockIndicatorLineStroke
        {
            get { return (Brush)GetValue(BlockIndicatorLineStrokeProperty); }
            set { SetValue(BlockIndicatorLineStrokeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the thickness of the line in the expand collapse area of
        /// EditControl.
        /// </summary>
        /// <remarks>
        /// <b>XAML</b>
        /// <para>&lt;syncfusion:EditControl x:Name=&quot;editControl1&quot;
        /// BlockIndicatorLineThickness=&quot;2&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.BlockIndicatorLineThickness=1d;</para>
        /// </remarks>
        public double BlockIndicatorLineThickness
        {
            get { return (double)GetValue(BlockIndicatorLineThicknessProperty); }
            set { SetValue(BlockIndicatorLineThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets background brush for selected text.
        /// </summary>
        /// <remarks>
        /// <b>XAML</b>
        /// <para>&lt;syncfusion:EditControl x:Name=&quot;editControl1&quot;
        /// SelectionBackground=&quot;DarkGray&quot;/&gt;</para>
        /// <para> </para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.SelectionBackground = Brushes.DodgerBlue;</para>
        /// </remarks>
        /// <value>
        /// By default, this property is set to DodgerBlue SolidColorBrush.
        /// </value>
        public Brush SelectionBackground
        {
            get { return (Brush)GetValue(SelectionBackgroundProperty); }
            set { SetValue(SelectionBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets text foreground for selected text.
        /// </summary>
        /// <remarks>
        /// <b>XAML</b>
        /// <para>&lt;syncfusion:EditControl x:Name=&quot;editControl1&quot;
        /// SelectionForeground=&quot;Yellow&quot;/&gt;</para>
        /// <para> </para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.SelectionForeground = Brushes.Green; </para>
        /// </remarks>
        /// <value>
        /// By Default, this property value is set to White. This brush value gets applied
        /// when IsSelectionForegroundEnabled property is set to true else, text foreground for
        /// selected text will be applied from default text foreground or based on syntax
        /// highlighting colors.
        /// </value>
        public Brush SelectionForeground
        {
            get { return (Brush)GetValue(SelectionForegroundProperty); }
            set { SetValue(SelectionForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance will apply foreground for
        /// selected text using SelectionForeground property.
        /// </summary>
        /// <remarks>
        /// <b>XAML</b>
        /// <para>&lt;syncfusion:EditControl x:Name=&quot;editControl1&quot;
        /// IsSelectionForegroundEnabled=&quot;False&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.IsSelectionForegroundEnabled = true;</para>
        /// </remarks>
        /// <value>
        /// <see langword="true"/> if this instance show apply SelectionForeground to
        /// selected; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsSelectionForegroundEnabled
        {
            get { return (bool)GetValue(IsSelectionForegroundEnabledProperty); }
            set { SetValue(IsSelectionForegroundEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets a Brush value that is applied as background of LineNumberArea.
        /// </summary>
        /// <remarks>
        /// <b>XAML</b>
        /// <para>&lt;syncfusion:EditControl x:Name=&quot;editControl1&quot;
        /// LineNumberAreaBackground=&quot;LightGreen&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.LineNumberAreaBackground=Brushes.Yellow;</para>
        /// </remarks>
        /// <value>
        /// By default, its value is set to Brushes.WhiteSmoke.
        /// </value>
        public Brush LineNumberAreaBackground
        {
            get { return (Brush)GetValue(LineNumberAreaBackgroundProperty); }
            set { SetValue(LineNumberAreaBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the width of the line number area has to
        /// be calculated automatically based on the number of lines.
        /// </summary>
        /// <remarks>
        /// <b>XAML</b>
        /// <para>&lt;syncfusion:EditControl x:Name=&quot;editControl1&quot;
        /// IsAutoLineNumberAreaWidthEnabled=&quot;False&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.IsAutoLineNumberAreaWidthEnabled = false;</para>
        /// </remarks>
        /// <value>
        /// <see langword="true"/> if this instance has to calculate the line number area
        /// width; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsAutoLineNumberAreaWidthEnabled
        {
            get { return (bool)GetValue(IsAutoLineNumberAreaWidthEnabledProperty); }
            set { SetValue(IsAutoLineNumberAreaWidthEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating the Width of the line number area. This width
        /// gets applied only when IsAutoLineNumberAreaWidthEnabled property is set to
        /// false.
        /// </summary>
        /// <remarks>
        /// <b>XAML</b>
        /// <para>&lt;syncfusion:EditControl x:Name=&quot;editControl1&quot;
        /// LineNumberAreaWidth=&quot;50&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.LineNumberAreaWidth = 40d;</para>
        /// </remarks>
        public double LineNumberAreaWidth
        {
            get { return (double)GetValue(LineNumberAreaWidthProperty); }
            set { SetValue(LineNumberAreaWidthProperty, value); }
        }

        /// <summary>
        /// Gets the actual width of line number area irrespective of
        /// IsAutoLineNumberAreaWidthEnabled.
        /// </summary>
        public double ActualLineNumberAreaWidth
        {
            get
            {
                return (double)GetValue(ActualLineNumberAreaWidthProperty);
            }
            internal set
            {
                if (this.ScrollControl != null)
                {
                    this.ScrollControl.columnwidths[0] = value;
                }
                SetValue(ActualLineNumberAreaWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets line number text foreground.
        /// </summary>
        /// <remarks>
        /// <b>XAML</b>
        /// <para>&lt;syncfusion:EditControl x:Name=&quot;editControl1&quot;
        /// LineNumberTextForeground=&quot;Yellow&quot;/&gt;</para>
        /// <para> </para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.LineNumberTextForeground = Brushes.Green; </para>
        /// </remarks>
        /// <value>
        /// By Default, this property value is set to Black
        /// </value>
        public Brush LineNumberTextForeground
        {
            get { return (Brush)GetValue(LineNumberTextForegroundProperty); }
            set { SetValue(LineNumberTextForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets width of the outlining (expand collapse) area.
        /// </summary>
        /// <remarks>
        /// <b>XAML</b>
        /// <para>&lt;syncfusion:EditControl x:Name=&quot;editControl1&quot;
        /// OutliningAreaWidth=&quot;40&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.OutliningAreaWidth = 50d;</para>
        /// </remarks>
        /// <value>
        /// By default, its value is set to 20.
        /// </value>
        public double OutliningAreaWidth
        {
            get { return (double)GetValue(OutliningAreaWidthProperty); }
            set { SetValue(OutliningAreaWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the background Brush to be applied to outlining (expand collapse)
        /// area.
        /// </summary>
        /// <remarks>
        /// <b>XAML</b>
        /// <para>&lt;syncfusion:EditControl x:Name=&quot;editControl1&quot;
        /// OutliningAreaBackground=&quot;LightBlue&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.OutliningAreaBackground=Brushes.Yellow;</para>
        /// </remarks>
        /// <value>
        /// By default, its value is set to Brushes.Transparent.
        /// </value>
        public Brush OutliningAreaBackground
        {
            get { return (Brush)GetValue(OutliningAreaBackgroundProperty); }
            set { SetValue(OutliningAreaBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the changes done to the text needs to
        /// highlighted or not.
        /// </summary>
        /// <remarks>
        /// <b>XAML</b>
        /// <para>&lt;syncfusion:EditControl x:Name=&quot;editControl1&quot;
        /// IsTrackChangesEnabled=&quot;False&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl.IsTrackChangesEnabled=true;</para>
        /// </remarks>
        /// <value>
        /// <see langword="true"/> if this instance should highlight the changes done to the
        /// text ; otherwise, <see langword="false"/>.
        /// <para></para>
        /// <para>By default, its value is set to true.</para>
        /// </value>
        public bool IsTrackChangesEnabled
        {
            get { return (bool)GetValue(IsTrackChangesEnabledProperty); }
            set { SetValue(IsTrackChangesEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Brush to be applied in the changes indicator for saved line.
        /// </summary>
        /// <remarks>
        /// <b>XAML</b>
        /// <para>&lt;syncfusion:EditControl x:Name=&quot;editControl1&quot;
        /// SavedLineIndicatorBrush=&quot;CadetBlue&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.SavedLineIndicatorBrush = Brushes.Magenta;</para>
        /// </remarks>
        /// <value>
        /// By default, its value is set to LightGreen.
        /// </value>
        public Brush SavedLineIndicatorBrush
        {
            get { return (Brush)GetValue(SavedLineIndicatorBrushProperty); }
            set { SetValue(SavedLineIndicatorBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Brush to be applied to changes indicator for a modified line
        /// that has not yet been saved.
        /// </summary>
        /// <remarks>
        ///  <b>XAML</b>
        /// <para>&lt;syncfusion:EditControl x:Name=&quot;editControl1&quot;
        /// ModifiedLineIndicatorBrush=&quot;CadetBlue&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.ModifiedLineIndicatorBrush = Brushes.Magenta;</para>
        /// </remarks>
        /// <value>
        /// By default, its value is set to Brushes.Yellow
        /// </value>
        public Brush ModifiedLineIndicatorBrush
        {
            get { return (Brush)GetValue(ModifiedLineIndicatorBrushProperty); }
            set { SetValue(ModifiedLineIndicatorBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Width of the changes indicator.
        /// </summary>
        /// <remarks>
        /// <b>XAML</b>
        /// <para>&lt;syncfusion:EditControl x:Name=&quot;editControl1&quot;
        /// ChangesIndicatorWidth=&quot;10&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para>editControl1.ChangesIndicatorWidth = 3d;</para>
        /// </remarks>
        /// <value>
        /// By default, its value is set to 4.
        /// </value>
        public double ChangesIndicatorWidth
        {
            get { return (double)GetValue(ChangesIndicatorWidthProperty); }
            set { SetValue(ChangesIndicatorWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the indenting options of the EditControl.
        /// </summary>
        /// <remarks>
        /// Supports three types of indentations and Smart indenting options is set as
        /// default value for this property.
        /// </remarks>
        public IndentingOptions IndentingOptions
        {
            get { return (IndentingOptions)GetValue(IndentingOptionsProperty); }
            set { SetValue(IndentingOptionsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this auto indentation is supported or
        /// not.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if this instance supports auto indentation; otherwise,
        /// <see langword="false"/>.
        /// </value>
        public bool IsAutoIndentationEnabled
        {
            get { return (bool)GetValue(IsAutoIndentationEnabledProperty); }
            set { SetValue(IsAutoIndentationEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets foreground brush value for ellipsis text and border.
        /// </summary>
        public Brush CollapsedTextForeground
        {
            get { return (Brush)GetValue(CollapsedTextForegroundProperty); }
            set { SetValue(CollapsedTextForegroundProperty, value); }
        }

        /// <summary>
        /// Occurs when [SelectionChanged].
        /// </summary>
        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        /// <summary>
        /// Gets or sets indicating whether allow to drag text or not.
        /// </summary>
        public bool AllowDragDrop
        {
            get { return (bool)GetValue(AllowDragDropProperty); }
            set { SetValue(AllowDragDropProperty, value); }
        }

        #endregion Dependency Properties

        #region Dependency Property Keys

        /// <summary>
        /// DependencyProperty for AllowDragDrop
        /// </summary>
        public static readonly DependencyProperty AllowDragDropProperty =
            DependencyProperty.Register("AllowDragDrop", typeof(bool), typeof(EditControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// DependencyProperty for AcceptsTab
        /// </summary>
        public static readonly DependencyProperty AcceptsTabProperty =
            DependencyProperty.Register("AcceptsTab", typeof(bool), typeof(EditControl), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// DependencyProperty for IntellisenseMode
        /// </summary>
        public static readonly DependencyProperty IntellisenseModeProperty =
            DependencyProperty.Register("IntellisenseMode", typeof(IntellisenseMode), typeof(EditControl), new FrameworkPropertyMetadata(IntellisenseMode.Auto));

        /// <summary>
        /// DependencyProperty for IntellisenseItemTemplate
        /// </summary>
        public static readonly DependencyProperty IntellisenseItemTemplateProperty =
            DependencyProperty.Register("IntellisenseItemTemplate", typeof(DataTemplate), typeof(EditControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// DependencyProperty for IntellisenseBoxStyle
        /// </summary>
        public static readonly DependencyProperty IntellisenseBoxStyleProperty =
            DependencyProperty.Register("IntellisenseBoxStyle", typeof(Style), typeof(EditControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// DependencyProperty for CustomLanguage
        /// </summary>
        public static readonly DependencyProperty CustomLanguageProperty = DependencyProperty.Register("CustomLanguage", typeof(LanguageBase), typeof(EditControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCustomLanguageChanged)));

        /// <summary>
        /// DependencyProperty for IntellisenseCustomItemsSource
        /// </summary>
        public static readonly DependencyProperty IntellisenseCustomItemsSourceProperty =
            DependencyProperty.Register("IntellisenseCustomItemsSource", typeof(IEnumerable), typeof(EditControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnIntellisenseCustomSourceChanged)));

        /// <summary>
        /// DependencyProperty for EnableIntellisense
        /// </summary>
        public static readonly DependencyProperty EnableIntellisenseProperty =
            DependencyProperty.Register("EnableIntellisense", typeof(bool), typeof(EditControl), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// DependencyProperty for AssemblyReferences
        /// </summary>
        public static readonly DependencyProperty AssemblyReferencesProperty =
            DependencyProperty.Register("AssemblyReferences", typeof(IEnumerable<Uri>), typeof(EditControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAssemblyReferencesChanged)));

        /// <summary>
        /// DependencyProperty for ShowFindAndReplace
        /// </summary>
        public static readonly DependencyProperty ShowFindAndReplaceProperty = DependencyProperty.Register("ShowFindAndReplace", typeof(bool), typeof(EditControl), new PropertyMetadata(true));

        /// <summary>
        /// DependencyProperty for HorizontalScrollBarVisibility
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(EditControl), new FrameworkPropertyMetadata(ScrollBarVisibility.Auto));

        /// <summary>
        /// DependencyProperty for ShowDefaultContextMenu
        /// </summary>
        public static readonly DependencyProperty ShowDefaultContextMenuProperty = DependencyProperty.Register("ShowDefaultContextMenu", typeof(bool), typeof(EditControl), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnShowDefaultContextMenuChanged)));

        /// <summary>
        /// DependencyProperty for IsFindResultsTabClosed
        /// </summary>
        public static readonly DependencyProperty IsFindResultsTabClosedProperty =
            DependencyProperty.Register("IsFindResultsTabClosed", typeof(bool), typeof(EditControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// DependencyProperty for FindResultsTabHeight
        /// </summary>
        public static readonly DependencyProperty FindResultsTabHeightProperty =
            DependencyProperty.Register("FindResultsTabHeight", typeof(double), typeof(EditControl), new FrameworkPropertyMetadata(150d, new PropertyChangedCallback(OnFindResultsTabHeightChanged), new CoerceValueCallback(CoerceWidthOrHeightProperty)));

        /// <summary>
        /// DependencyProperty for FindResultsTabVisibility
        /// </summary>
        public static readonly DependencyProperty FindResultsTabVisibilityProperty =
            DependencyProperty.Register("FindResultsTabVisibility", typeof(TabVisibility), typeof(EditControl), new FrameworkPropertyMetadata(TabVisibility.Auto));

        /// <summary>
        /// DependencyProperty for TabSpacesProperty
        /// </summary>
        public static readonly DependencyProperty TabSpacesProperty =
            DependencyProperty.Register("TabSpaces", typeof(int), typeof(EditControl), new FrameworkPropertyMetadata(4));

        /// <summary>
        /// DependencyProperty for IsUndoEnabledProperty
        /// </summary>
        public static readonly DependencyProperty IsUndoEnabledProperty =
            DependencyProperty.Register("IsUndoEnabled", typeof(bool), typeof(EditControl), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// DependencyProperty for IsRedoEnabledProperty
        /// </summary>
        public static readonly DependencyProperty IsRedoEnabledProperty =
            DependencyProperty.Register("IsRedoEnabled", typeof(bool), typeof(EditControl), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// DependencyProperty for EnableOutlining
        /// </summary>
        public static readonly DependencyProperty EnableOutliningProperty = DependencyProperty.Register("EnableOutlining", typeof(bool), typeof(EditControl), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnEnableOutliningChanged)));

        /// <summary>
        /// DependencyProperty for SelectedTextProperty
        /// </summary>
        public static readonly DependencyProperty SelectedTextProperty =
            DependencyProperty.Register("SelectedText", typeof(string), typeof(EditControl), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnSelectedTextChanged)));

        /// <summary>
        /// DependencyProperty for VerticalScrollBarVisibility
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(EditControl), new FrameworkPropertyMetadata(ScrollBarVisibility.Auto));

        /// <summary>
        /// DependencyProperty for DocumentSource
        /// </summary>
        public static readonly DependencyProperty DocumentSourceProperty = DependencyProperty.Register("DocumentSource", typeof(string), typeof(EditControl), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnSourceChanged)));

        /// <summary>
        /// DependencyProperty for PreferredWidth
        /// </summary>
        internal static readonly DependencyProperty PreferredWidthProperty = DependencyProperty.Register("PreferredWidth", typeof(double), typeof(EditControl), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnPreferredWidthChanged)));

        /// <summary>
        /// DependencyProperty for LineHeight
        /// </summary>
        public static readonly DependencyProperty LineHeightProperty = DependencyProperty.Register("LineHeight", typeof(double), typeof(EditControl), new FrameworkPropertyMetadata((SystemFonts.MessageFontSize + 2), new PropertyChangedCallback(OnLineHeightChanged)));

        /// <summary>
        /// DependencyProperty for DocumentLanguage
        /// </summary>
        public static readonly DependencyProperty DocumentLanguageProperty = DependencyProperty.Register("DocumentLanguage", typeof(Languages), typeof(EditControl), new FrameworkPropertyMetadata(Languages.Text, new PropertyChangedCallback(OnLanguageChanged)));

        /// <summary>
        /// DependencyProperty for Text
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(EditControl), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnTextChanged)));

        /// <summary>
        /// DependencyProperty for ShowLineNumber
        /// </summary>
        public static readonly DependencyProperty ShowLineNumberProperty = DependencyProperty.Register("ShowLineNumber", typeof(bool), typeof(EditControl), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnShowLineNumberChanged)));

        /// <summary>
        /// DependencyProperty for IsReadOnly
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(EditControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// DependencyProperty for ShowOutlining
        /// </summary>
        public static readonly DependencyProperty ShowOutliningProperty = DependencyProperty.Register("ShowOutlining", typeof(Visibility), typeof(EditControl), new FrameworkPropertyMetadata(Visibility.Visible));

        /// <summary>
        /// DependencyProperty for IntellisensePopupHeight
        /// </summary>
        public static readonly DependencyProperty IntellisensePopupHeightProperty =
            DependencyProperty.Register("IntellisensePopupHeight", typeof(double), typeof(EditControl), new FrameworkPropertyMetadata(200d, new PropertyChangedCallback(OnIntellisensePopupHeightChanged), new CoerceValueCallback(OnCoerceIntellisensePopupHeight)));

        /// <summary>
        /// DependencyProperty for IntellisensePopupWidth
        /// </summary>
        public static readonly DependencyProperty IntellisensePopupWidthProperty =
            DependencyProperty.Register("IntellisensePopupWidth", typeof(double), typeof(EditControl), new FrameworkPropertyMetadata(250d, new PropertyChangedCallback(OnIntellisensePopupHeightChanged), new CoerceValueCallback(OnCoerceIntellisensePopupHeight)));

        /// <summary>
        /// DependencyProperty for CaretBrush Property
        /// </summary>
        public static readonly DependencyProperty CaretBrushProperty =
            DependencyProperty.Register("CaretBrush", typeof(Brush), typeof(EditControl), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// DependencyProperty for ShowBlockIndicatorLine Property
        /// </summary>
        public static readonly DependencyProperty ShowBlockIndicatorLineProperty =
            DependencyProperty.Register("ShowBlockIndicatorLine", typeof(bool), typeof(EditControl), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// DependencyProperty for BlockIndicatorLineStroke Property
        /// </summary>
        public static readonly DependencyProperty BlockIndicatorLineStrokeProperty =
            DependencyProperty.Register("BlockIndicatorLineStroke", typeof(Brush), typeof(EditControl), new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// DependencyProperty for BlockIndicatorLineThickness Property
        /// </summary>
        public static readonly DependencyProperty BlockIndicatorLineThicknessProperty =
            DependencyProperty.Register("BlockIndicatorLineThickness", typeof(double), typeof(EditControl), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// DependencyProperty for SelectionBackground Property
        /// </summary>
        public static readonly DependencyProperty SelectionBackgroundProperty =
            DependencyProperty.Register("SelectionBackground", typeof(Brush), typeof(EditControl), new FrameworkPropertyMetadata(Brushes.DodgerBlue, new PropertyChangedCallback(OnFontPropertiesChanged)));

        /// <summary>
        /// DependencyProperty for SelectionForeground Property
        /// </summary>
        public static readonly DependencyProperty SelectionForegroundProperty =
            DependencyProperty.Register("SelectionForeground", typeof(Brush), typeof(EditControl), new FrameworkPropertyMetadata(Brushes.White, new PropertyChangedCallback(OnFontPropertiesChanged)));

        /// <summary>
        /// DependencyProperty for IsSelectionForegroundEnabled Property
        /// </summary>
        public static readonly DependencyProperty IsSelectionForegroundEnabledProperty =
            DependencyProperty.Register("IsSelectionForegroundEnabled", typeof(bool), typeof(EditControl), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnFontPropertiesChanged)));

        /// <summary>
        /// DependencyProperty for LineNumberAreaBackground Property
        /// </summary>
        public static readonly DependencyProperty LineNumberAreaBackgroundProperty =
            DependencyProperty.Register("LineNumberAreaBackground", typeof(Brush), typeof(EditControl), new FrameworkPropertyMetadata(Brushes.WhiteSmoke));

        /// <summary>
        /// DependencyProperty for IsAutoLineNumberAreaWidthEnabled Property
        /// </summary>
        public static readonly DependencyProperty IsAutoLineNumberAreaWidthEnabledProperty =
            DependencyProperty.Register("IsAutoLineNumberAreaWidthEnabled", typeof(bool), typeof(EditControl), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnLineNumberPropertyChanged)));

        /// <summary>
        /// DependencyProperty for LineNumberAreaBackground Property
        /// </summary>
        public static readonly DependencyProperty LineNumberAreaWidthProperty =
            DependencyProperty.Register("LineNumberAreaWidth", typeof(double), typeof(EditControl), new FrameworkPropertyMetadata(40d, new PropertyChangedCallback(OnLineNumberPropertyChanged), new CoerceValueCallback(CoerceWidthOrHeightProperty)));

        /// <summary>
        /// DependencyProperty for ActualLineNumberAreaWidth Property
        /// </summary>
        public static readonly DependencyProperty ActualLineNumberAreaWidthProperty =
            DependencyProperty.Register("ActualLineNumberAreaWidth", typeof(double), typeof(EditControl), new FrameworkPropertyMetadata(0d));

        /// <summary>
        /// DependencyProperty for LineNumberTextForeground Property
        /// </summary>
        public static readonly DependencyProperty LineNumberTextForegroundProperty =
            DependencyProperty.Register("LineNumberTextForeground", typeof(Brush), typeof(EditControl), new FrameworkPropertyMetadata(Brushes.Black));

        /// <summary>
        /// DependencyProperty for OutliningAreaWidth Property
        /// </summary>
        public static readonly DependencyProperty OutliningAreaWidthProperty =
            DependencyProperty.Register("OutliningAreaWidth", typeof(double), typeof(EditControl), new FrameworkPropertyMetadata(10d, new PropertyChangedCallback(OnOutliningAreaWidthChanged), new CoerceValueCallback(CoerceWidthOrHeightProperty)));

        /// <summary>
        /// DependencyProperty for OutliningAreaBackground Property
        /// </summary>
        public static readonly DependencyProperty OutliningAreaBackgroundProperty =
            DependencyProperty.Register("OutliningAreaBackground", typeof(Brush), typeof(EditControl), new FrameworkPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// DependencyProperty for IsTrackChangesEnabled Property
        /// </summary>
        public static readonly DependencyProperty IsTrackChangesEnabledProperty =
            DependencyProperty.Register("IsTrackChangesEnabled", typeof(bool), typeof(EditControl), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// DependencyProperty for SavedLineIndicatorBrush Property
        /// </summary>
        public static readonly DependencyProperty SavedLineIndicatorBrushProperty =
            DependencyProperty.Register("SavedLineIndicatorBrush", typeof(Brush), typeof(EditControl), new FrameworkPropertyMetadata(Brushes.LightGreen));

        /// <summary>
        /// DependencyProperty for ModifiedLineIndicatorBrush Property
        /// </summary>
        public static readonly DependencyProperty ModifiedLineIndicatorBrushProperty =
            DependencyProperty.Register("ModifiedLineIndicatorBrush", typeof(Brush), typeof(EditControl), new FrameworkPropertyMetadata(Brushes.Yellow));

        /// <summary>
        /// DependencyProperty for ChangesIndicatorWidth Property
        /// </summary>
        public static readonly DependencyProperty ChangesIndicatorWidthProperty =
            DependencyProperty.Register("ChangesIndicatorWidth", typeof(double), typeof(EditControl), new FrameworkPropertyMetadata(4d, new PropertyChangedCallback(OnChangesIndicatorWidthChanged), new CoerceValueCallback(CoerceWidthOrHeightProperty)));

        /// <summary>
        /// DependencyProperty for CollapsedTextForeground Property
        /// </summary>
        public static readonly DependencyProperty CollapsedTextForegroundProperty =
            DependencyProperty.Register("CollapsedTextForeground", typeof(Brush), typeof(EditControl), new FrameworkPropertyMetadata(Brushes.DarkGray, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// DependencyProperty for IsAutoIndentationEnabled Property
        /// </summary>
        public static readonly DependencyProperty IsAutoIndentationEnabledProperty =
            DependencyProperty.Register("IsAutoIndentationEnabled", typeof(bool), typeof(EditControl), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// DependencyProperty for IndentingOptions Property
        /// </summary>
        public static readonly DependencyProperty IndentingOptionsProperty =
            DependencyProperty.Register("IndentingOptions", typeof(IndentingOptions), typeof(EditControl), new FrameworkPropertyMetadata(IndentingOptions.Smart));

        /// <summary>
        /// Bubbled RoutedEvent Identifies the SelectionChanged.
        /// </summary>
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EditControl));

        #endregion Dependency Property Keys

        #region Properties

        /// <summary>
        /// Gets the collection of lines in EditControl.
        /// </summary>
        public LineItemsCollection Lines
        {
            get
            {
                return mlines;
            }

            internal set
            {
                mlines = value;
            }
        }

        /// <summary>
        /// Gets an instance of LanguageBase based on the DocumentLanguage property.
        /// </summary>
        public LanguageBase CurrentLanguage
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the line number where the cursor is currently located.
        /// </summary>
        /// <remarks>
        /// <para>LineNumber property returns the current line number.</para>
        /// </remarks>
        /// <example>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>int i = editControl1.LineNumber;</para>
        /// </example>
        public int LineNumber
        {
            get
            {
                return scrollControl.LineNumber + 1;
            }
        }

        /// <summary>
        /// Gets current index of text where the cursor is located in EditControl pane.
        /// </summary>
        /// <remarks>
        /// <para>CursorIndex returns the caret index.</para>
        /// </remarks>
        /// <example>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>int index = editControl1.CursorIndex;</para>
        /// </example>
        public int CursorIndex
        {
            get
            {
                return scrollControl.CaretIndex;
            }
        }

        /// <summary>
        /// Gets or sets a instance of FindOptions indicating the options selected in the FindReplace Window.
        /// </summary>
        public FindOptions FindOptions
        {
            get;
            set;
        }

        /// <summary>
        /// FindReplaceWindow readonly property
        /// </summary>
        public Window FindReplaceWindow
        {
            get
            {
                return findReplaceWindow;
            }
        }

        /// <summary>
        /// Gets an instance of EditScrollControl in the template.
        /// </summary>
        internal EditScrollControl ScrollControl
        {
            get
            {
                return scrollControl;
            }
        }

        /// <summary>
        /// Gets or sets an instance of UndoManager
        /// </summary>
        internal UndoManager UndoManager
        {
            get;
            set;
        }

        /// <summary>
        /// Get or sets a value indicating the Search results.
        /// </summary>
        public SearchResult SearchResults
        {
            get;
            set;
        }

        #endregion Properties

        #region Property Changed Callbacks

        /// <summary>
        /// Gets called when Text property of the EditControl gets changed. Also it triggers
        /// the TextChanged Event of the EditControl.
        /// </summary>
        /// <param name="sender">DepdendencyObject, returns EditControl</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var editControl = sender as EditControl;
            if (args.OldValue != args.NewValue)
            {
                editControl.OnTextChanged(args);
            }
        }

        /// <summary>
        /// Occurs when the DocumentLanguage property is changed
        /// </summary>
        /// <param name="d">The DependencyObject, Return EditControl on OnLanguageChanged event is raised</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EditControl control = d as EditControl;
            control.InitializeLanguage(control.DocumentLanguage);
            if (control.Text != string.Empty)
            {
                control.CurrentLanguage.SplitTextToLines();
            }
        }

        /// <summary>
        /// Occurs when the LineHeight property is changed
        /// <param name="d">The DependencyObject, Return EditControl on OnLanguageChanged event is raised</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        /// </summary>
        private static void OnLineHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Occurs when the DocumentSource property gets changed.
        /// </summary>
        /// <param name="d">The DependencyObject, Return EditControl on SourceChanged event is raised</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                EditControl instance = (EditControl)d;
                if (instance.checkSource == false)
                    instance.LoadFile(e.NewValue.ToString());
                instance.OnSourceChanged(e);
            }
        }

        /// <summary>
        /// Occurs when the DocumentSource property gets changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.DocumentSourceChanged != null)
            {
                this.DocumentSourceChanged(this, e);
            }
        }

        /// <summary>
        /// Occurs when the PreferredWidth property gets changed.
        /// </summary>
        /// <param name="obj">The DependencyObject, Return EditControl on SourceChanged event is raised</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnPreferredWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EditControl control = (EditControl)obj;
            if (control.ScrollControl != null)
            {
                //control.ScrollControl.TextAreaWidth = Math.Max(control.PreferredWidth, 10);
                control.ScrollControl.columnwidths[4] = control.PreferredWidth;
            }
        }

        /// <summary>
        /// Occurs when the FontFamily property gets changed.
        /// </summary>
        /// <param name="obj">The DependencyObject, Return EditControl when its Font property changed event is raised</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFontPropertiesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            EditControl control = obj as EditControl;
            if (e.OldValue != e.NewValue)
            {
                control.OnFontPropertiesChanged(e);
            }
        }

        /// <summary>
        /// Occurs when the FontFamily and FontSize property gets changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnFontPropertiesChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.CurrentLanguage != null)
            {
                if (this.Lines.Count > 0)
                {
                    this.LineHeight = this.Lines[0].CalculateItemHeight(this.Lines[0].Text);
                }
                this.CurrentLanguage.CalculatePreferredWidth();
                this.CurrentLanguage.isUpdating = true;
                for (int i = 0; i < this.Lines.Count; i++)
                {
                    if (i == this.Lines.Count - 1)
                    {
                        this.CurrentLanguage.isUpdating = false;
                    }
                    this.CurrentLanguage.ResetLine(this.Lines[i]);
                }
            }

            if (this.ScrollControl != null)
            {
                if (this.ScrollControl.TextSelectionPointer != null)
                {
                    SelectionPointer pointer = this.ScrollControl.TextSelectionPointer;
                    this.ScrollControl.ClearSelection();
                    this.ScrollControl.UpdateSelectionPointer(pointer.StartLine, pointer.EndLine, pointer.StartIndex, pointer.EndIndex);
                }

                if (this.ScrollControl.Caret != null)
                {
                    this.ScrollControl.CaretIndex = this.ScrollControl.Caret.MoveTo(0);
                }
            }
        }

        /// <summary>
        /// Occurs when the SelectedText property gets changed.
        /// </summary>
        /// <param name="obj">The DependencyObject, represents EditControl</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EditControl control = obj as EditControl;
            control.OnSelectedTextChanged(args);
        }

        /// <summary>
        /// Occurs when the SelectedText property gets changed.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelectedTextChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.SelectedTextChanged != null)
            {
                this.SelectedTextChanged(this, args);
            }
            RaiseEvent(new RoutedEventArgs(EditControl.SelectionChangedEvent));
        }

        /// <summary>
        /// Occurs when the ShowLineNumber property gets changed.
        /// </summary>
        /// <param name="obj">The DependencyObject, represents EditControl</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnShowLineNumberChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EditControl control = obj as EditControl;
            if (control.ScrollControl == null)
            {
                return;
            }
            control.ScrollControl.ScrollColumns.SetLineResize(0, control.ScrollControl.CalculateLineNumberColumnWidth());
        }

        /// <summary>
        /// Occurs when the EnableOutlining property gets changed.
        /// </summary>
        /// <param name="obj">The DependencyObject, represents EditControl</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnEnableOutliningChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EditControl control = obj as EditControl;
            if (control.ScrollControl == null)
            {
                return;
            }
            control.ScrollControl.ScrollColumns.SetLineResize(2, control.EnableOutlining ? 20 : 0);
        }

        /// <summary>
        /// Occurs when the ShowLineNumber property gets changed.
        /// </summary>
        /// <param name="obj">The DependencyObject, represents EditControl</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnShowDefaultContextMenuChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EditControl control = obj as EditControl;
            if (control.ScrollControl == null)
            {
                return;
            }

            ResourceDictionary dictionary = new ResourceDictionary();
            dictionary.Source = new Uri(@"/Syncfusion.Edit.Wpf;component/Themes/Generic.xaml", UriKind.Relative);

            if (control.ShowDefaultContextMenu)
            {
                control.ContextMenu = dictionary["contextmenu"] as ContextMenu;
            }
            else
            {
                control.ContextMenu = null;
            }
        }

        /// <summary>
        /// Occurs when the IntellisenseCustomSource property gets changed.
        /// </summary>
        /// <param name="obj">The DependencyObject, represents EditControl</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIntellisenseCustomSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EditControl instance = obj as EditControl;
            if (instance.IntellisenseMode == IntellisenseMode.Custom && instance.intellisenseBox != null)
            {
                instance.intellisenseBox.ItemsSource = instance.IntellisenseCustomItemsSource;
            }
        }

        /// <summary>
        /// Occurs when custom language property gets changed. Updates the CurrentLanguage property and
        /// updates visuals to apply colors based on the custom language
        /// </summary>
        /// <param name="d">The DependencyObject, Return EditControl on OnCustomLanguageChanged event is raised</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCustomLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EditControl instance = (EditControl)d;
            if (instance.CustomLanguage != null && instance.DocumentLanguage == Languages.Custom)
            {
                instance.CurrentLanguage = instance.CustomLanguage;
                instance.CurrentLanguage.SplitTextToLines();
            }
        }

        /// <summary>
        /// Coerces the FindResultsTabHeight  property.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>The base value</returns>
        private static object CoerceWidthOrHeightProperty(DependencyObject d, object baseValue)
        {
            double value = double.Parse(baseValue.ToString());
            return (double.IsNaN(value) || double.IsInfinity(value) || value < 0) ? 0d : baseValue;
        }

        /// <summary>
        /// Occurs when FindResultsTabHeight property gets changed.
        /// </summary>
        /// <param name="obj">The DependencyObject, Return EditControl on OnCustomLanguageChanged event is raised</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFindResultsTabHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
        }

        /// <summary>
        /// Coerces the IntellisensePopupHeight  property.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>The base value</returns>
        private static object OnCoerceIntellisensePopupHeight(DependencyObject d, object baseValue)
        {
            double value = double.Parse(baseValue.ToString());
            return (double.IsNaN(value) || double.IsInfinity(value)) ? 0d : baseValue;
        }

        /// <summary>
        /// Occurs when IntellisensePopupHeight property gets changed.
        /// </summary>
        /// <param name="obj">The DependencyObject, Return EditControl on OnCustomLanguageChanged event is raised</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIntellisensePopupHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
        }

        /// <summary>
        /// Occurs when FindResultsTabHeight property gets changed.
        /// </summary>
        /// <param name="obj">The DependencyObject, Return EditControl on OnCustomLanguageChanged event is raised</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAssemblyReferencesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null && args.NewValue is ObservableCollection<Uri>)
            {
                ObservableCollection<Uri> collection = args.NewValue as ObservableCollection<Uri>;
                EditControl control = obj as EditControl;
                control.InitializeAssembliesCollectionChanged(collection);
            }

            if (args.OldValue != null && args.OldValue is ObservableCollection<Uri>)
            {
                ObservableCollection<Uri> collection = args.OldValue as ObservableCollection<Uri>;
                EditControl control = obj as EditControl;
                control.DeInitializeAssembliesCollectionChanged(collection);
            }
        }

        private void DeInitializeAssembliesCollectionChanged(ObservableCollection<Uri> collection)
        {
            if (collection != null)
            {
                collection.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(collection_CollectionChanged);
            }
        }

        private void InitializeAssembliesCollectionChanged(ObservableCollection<Uri> collection)
        {
            if (collection != null)
            {
                collection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(collection_CollectionChanged);
            }
        }

        private void collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.CurrentLanguage.SupportsIntellisense)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add || e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove || e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    if (!isAssemblyInitializing)
                    {
                        this.CurrentLanguage.InitializeNamespaces(this.AssemblyReferences);
                    }
                }
            }
        }

        /// <summary>
        /// DependencyPropertyChangedCallback for line number area related properties such
        /// aas LineNumberAreaWidth and IsAutoLineNumberAreaWidthEnabled
        /// </summary>
        /// <param name="obj">represents EditControl</param>
        /// <param name="args">represents DependencyPropertyChangedEventArgs</param>
        private static void OnLineNumberPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EditControl control = obj as EditControl;
            if (control.ScrollControl != null && control.IsAutoLineNumberAreaWidthEnabled)
            {
                control.ScrollControl.CalculateLineNumberColumnWidth();
                control.ScrollControl.InvalidateVisual(true);
            }
            else if (control.ScrollControl != null)
            {
                control.ActualLineNumberAreaWidth = control.LineNumberAreaWidth;
                control.ScrollControl.InvalidateVisual(true);
            }
        }

        /// <summary>
        /// DependencyPropertyChangedCallback for OutliningAreaWidth DependencyProperty.
        /// </summary>
        /// <param name="obj">represents EditControl</param>
        /// <param name="args">represents DependencyPropertyChangedEventArgs</param>
        private static void OnOutliningAreaWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EditControl control = obj as EditControl;
            if (control.ScrollControl != null)
            {
                if (control.EnableOutlining)
                {
                    control.ScrollControl.columnwidths[2] = control.OutliningAreaWidth;
                }
                else
                {
                    control.ScrollControl.columnwidths[2] = 0d;
                }
                control.ScrollControl.InvalidateVisual(true);
            }
        }

        /// <summary>
        /// DependencyPropertyChangedCallback for ChangesIndicatorWidth DependencyProperty.
        /// </summary>
        /// <param name="obj">represents EditControl</param>
        /// <param name="args">represents DependencyPropertyChangedEventArgs</param>
        private static void OnChangesIndicatorWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EditControl control = obj as EditControl;
            if (control.ScrollControl != null)
            {
                if (control.IsTrackChangesEnabled)
                {
                    control.ScrollControl.columnwidths[1] = control.ChangesIndicatorWidth;
                }
                else
                {
                    control.ScrollControl.columnwidths[1] = 0d;
                }
                control.ScrollControl.InvalidateVisual(true);
            }
        }

        /// <summary>
        /// A method to specify that the adding or removing assemblies to the Assembly references property has been started.
        /// </summary>
        public void BeginAssemblyInit()
        {
            isAssemblyInitializing = true;
        }

        /// <summary>
        /// A method to specify that the adding or removing assemblies to the Assembly references property has been completed.
        /// </summary>
        public void EndAssemblyInit()
        {
            isAssemblyInitializing = false;
            this.CurrentLanguage.InitializeNamespaces(this.AssemblyReferences);
        }

        #endregion Property Changed Callbacks

        #region Constructor

        /// <summary>
        /// Initializes static members of the <see cref="EditControl"/> class.
        /// </summary>
        static EditControl()
        {
            EnvironmentTest.ValidateLicense(typeof(EditControl));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditControl), new FrameworkPropertyMetadata(typeof(EditControl)));
            FontFamilyProperty.OverrideMetadata(typeof(EditControl), new FrameworkPropertyMetadata(OnFontPropertiesChanged));
            FontSizeProperty.OverrideMetadata(typeof(EditControl), new FrameworkPropertyMetadata(OnFontPropertiesChanged));
            ForegroundProperty.OverrideMetadata(typeof(EditControl), new FrameworkPropertyMetadata(OnFontPropertiesChanged));
        }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Edit.EditControl">EditControl</see> class.
        /// </summary>
        public EditControl()
        {
            EnvironmentTest.ValidateLicense(typeof(EditControl));
            mlines = new LineItemsCollection();
            mlines.Add(new LineItem(string.Empty));

            this.Loaded += new RoutedEventHandler(EditControl_Loaded);
            this.Unloaded += EditControl_Unloaded;

            // this.Lines.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Lines_CollectionChanged);
            // this.Lines.OnLineItemTextchanged += new LineItemsCollection.LineItemTextchanged(Lines_OnLineItemTextchanged);
            isReinitializeLines = true;
            if (this.CurrentLanguage == null)
            {
                this.InitializeLanguage(this.DocumentLanguage);
            }
            this.FindOptions = new FindOptions(this);
            this.UndoManager = new UndoManager(this);
            InitializeCommandBindings();
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
        }

        private void Lines_OnLineItemTextchanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.isReinitializeLines)
            {
                string text = GetText();
                if (this.Text != text)
                    this.Text = text;
                this.isReinitializeLines = false;
            }
        }

        /// <summary>
        /// Sets the focus of the control whent the control gets loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void EditControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Lines.Count > 0 && scrollControl != null)
            {
                scrollControl.MoveCursorToLineItem(0);
                if (!FocusManager.GetIsFocusScope(this))
                {
                    if (this.ScrollControl.Caret != null)
                    {
                        this.ScrollControl.Caret.Visibility = Visibility.Collapsed;
                    }
                }
                this.CurrentLanguage.ApplyExpandItems();
            }
        }

        #endregion Constructor

        #region Implementation

        /// <summary>
        /// Occurs the TextChanged event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        internal void OnTextChanged(DependencyPropertyChangedEventArgs args)
        {
            if (scrollControl != null)
            {
                SelectionPointer temppointer = null;
                bool isselected = false;
                bool selectall = false;
                string temptxt = string.Empty;
                int currentLineNumber = LineNumber - 1;
                int currentCursorIndex = CursorIndex;
                if (SelectedText != string.Empty)
                {
                    temppointer = this.ScrollControl.TextSelectionPointer;
                    isselected = true;
                    temptxt = SelectedText;
                }
                if (AddUndoManager)
                {
                    String tempText = "";
                    UndoManager.IsNewUndoItem = true;
                    if (LineNumber > 0 && Lines.Count > LineNumber && CursorIndex > Lines[LineNumber - 1].Text.Length - CursorIndex && Lines[LineNumber - 1].Text.Length > CursorIndex)
                        tempText = Lines[LineNumber - 1].Text.Substring(CursorIndex, Lines[LineNumber - 1].Text.Length - CursorIndex);
                    else
                    {
                        if (this.Text.Length > currentCursorIndex)
                            tempText = this.Text.Substring(currentCursorIndex, this.Text.Length - currentCursorIndex);
                    }
                    UndoManager.Add(new EditAction()
                    {
                        Action = ActionType.Type,
                        Text = tempText,
                        LineNumber = currentLineNumber + 1,
                        CursorIndex = CursorIndex,
                        IsSelected = isselected,
                        SelectedText = temptxt,
                        Pointer = temppointer,
                        IsSelectAll = selectall
                    });
                }
            }
            if (this.CurrentLanguage == null)
            {
                InitializeLanguage(this.DocumentLanguage);
            }

            if (this.isReinitializeLines)
            {
                this.CurrentLanguage.SplitTextToLines();
            }

            if (this.TextChanged != null)
            {
                this.TextChanged(this, args);
            }
        }

        /// <summary>
        /// Helper method to include all the supported file types to FileOpen or FileSave dialog
        /// </summary>
        /// <param name="dialog">The dialog.</param>
        private void AddFiltersToOpenDialog(FileDialog dialog)
        {
            string filter = string.Empty;
            if (knownLanguages == null)
            {
                knownLanguages = new List<string>();
            }

            knownLanguages.Clear();
            if (this.CurrentLanguage != null)
            {
                filter = this.CurrentLanguage.Name + " (*" + this.CurrentLanguage.FileExtension + ")|*" + this.CurrentLanguage.FileExtension;
                knownLanguages.Add(this.CurrentLanguage.FileExtension);
            }
            var values = Enum.GetValues(typeof(Languages));
            foreach (Languages item in values)
            {
                if (item != this.DocumentLanguage)
                {
                    LanguageBase language = this.GetLanguageBase(item);
                    if (language != null)
                    {
                        filter = filter + "|" + language.Name + " (*" + language.FileExtension + ")|*" + language.FileExtension;
                        knownLanguages.Add(language.FileExtension);
                    }
                }
            }

            dialog.Filter = filter.TrimStart('|').TrimEnd('|');
        }

        /// <summary>
        /// Helper method to get the LanguageBase object based on the Language Enum
        /// </summary>
        /// <param name="item">represents Language item</param>
        /// <returns>an instance of LanguageBase based on the language selected.</returns>
        private LanguageBase GetLanguageBase(Languages item)
        {
            switch (item)
            {
                case Languages.Text:
                    return new TextLanguage(this);
                case Languages.CSharp:
                    return new CSharpLanguage(this);
                case Languages.VisualBasic:
                    return new VBLanguage(this);
                case Languages.XAML:
                    return new XAMLLanguage(this);
                case Languages.XML:
                    return new XMLLanguage(this);
                case Languages.SQL:
                    return new SQLLanguage(this);
                case Languages.Custom:
                    return this.CustomLanguage;
                default:
                    return null;
            }
        }

        /// <summary>
        /// LoadFile method is used to open a file in the EditControl. It shows up a
        /// OpenFileDialog in order for the users to select the file to be opened using
        /// EditControl and returns a bool value stating the whether file open was
        /// successful.
        /// </summary>
        /// <remarks>
        /// <para>Load the specified file from the opendialog box.</para>
        /// </remarks>
        /// <returns>
        /// <para>Return true when the file loaded, otherwise return false</para>
        /// </returns>
        /// <example>
        /// <para><b>C#</b></para>
        /// <para>editControl1.LoadFile();</para>
        /// </example>
        public bool LoadFile()
        {
            OpenFileDialog m_opendialog = new OpenFileDialog();
            AddFiltersToOpenDialog(m_opendialog);
            m_opendialog.CheckFileExists = true;
            m_opendialog.CheckPathExists = true;
            m_opendialog.ShowDialog();
            isReinitializeLines = true;
            if (m_opendialog.FileName != string.Empty)
            {
                return LoadFile(m_opendialog.FileName);
            }

            return false;
        }

        /// <summary>
        /// This method does not display OpenFileDialog and loads the file specified as the
        /// parameter of the method. It returns a bool value stating the file open was
        /// successful.
        /// </summary>
        /// <remarks>
        /// <para>Load the specified filename into the EditControl.</para>
        /// </remarks>
        /// <param name="fileName">String type, specifies the name of the file to be
        /// loaded.</param>
        /// <returns>
        /// <para>Returns a bool value stating the file open was successful.</para>
        /// </returns>
        /// <example>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>editControl1.LoadFile(&quot;C:\test.txt&quot;);</para>
        /// </example>
        public bool LoadFile(string fileName)
        {
            return LoadFile(fileName, false, false);
        }

        /// <summary>
        /// Overload of LoadFile function with Encoding as additional argument
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>
        /// Returns true when File loaded, otherwise return false.
        /// </returns>
        internal bool LoadFile(string fileName, Encoding encoding)
        {
            return LoadFile(fileName, false, false, encoding);
        }

        /// <summary>
        /// Overload of LoadFile function with Convert and Shared as additional arguments
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="convert">if set to <c>true</c> [convert].</param>
        /// <param name="shared">if set to <c>true</c> [shared].</param>
        /// <returns>Returns true when File loaded, otherwise return false.</returns>
        internal bool LoadFile(string fileName, bool convert, bool shared)
        {
            return LoadFile(fileName, convert, shared, Encoding.UTF8);
        }

        /// <summary>
        ///
        /// </summary>
        public bool checkSource = false;

        /// <summary>
        /// Function used to load the content from the file to the control
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="convert">if set to <c>true</c> [convert].</param>
        /// <param name="shared">if set to <c>true</c> [shared].</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>Returns true when File length greater than 0, otherwise return false.</returns>

        internal bool LoadFile(string fileName, bool convert, bool shared, Encoding encoding)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("File Name");
            }

            if (fileName.Length == 0)
            {
                return false;
            }

            bool result = false;
            Paragraph para = new Paragraph();
            using (StreamReader sr = new StreamReader(fileName))
            {
                string line;
                line = sr.ReadToEnd();
                if (line != string.Empty)
                {
                    this.isReinitializeLines = true;
                    if (line.Contains("\t") && TabSpaces != 0)
                    {
                        string t = "";
                        for (int m = 0; m < TabSpaces; m++) t = t + " ";
                        line = line.Replace("\t", t);
                    }
                    Text = line;
                    this.ChangeLinesState(LineModificationState.Modified, LineModificationState.Unchanged);
                    result = true;
                }
                else if (line == string.Empty)
                {
                    Text = line;
                    this.ChangeLinesState(LineModificationState.Modified, LineModificationState.Unchanged);
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            checkSource = true;
            DocumentSource = fileName;
            checkSource = false;
            return result;
        }

        /// <summary>
        /// Helper method for Tab key activity
        /// </summary>
        private void ExecuteTab()
        {
            SelectionPointer temppointer = null;
            bool isselected = false;
            bool selectall = false;
            string temptxt = string.Empty;
            int currentLineNumber = LineNumber - 1;
            int currentCursorIndex = CursorIndex;
            if (SelectedText != string.Empty)
            {
                if (this.CurrentLanguage.IsIndentSelectionOnTabEnabled)
                {
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        this.DecreaseIndent(this.ScrollControl.TextSelectionPointer, this.TabSpaces);
                    }
                    else
                    {
                        this.IncreaseIndent(this.ScrollControl.TextSelectionPointer, this.TabSpaces);
                    }
                    return;
                }
                else
                {
                    temppointer = this.ScrollControl.TextSelectionPointer;
                    isselected = true;
                    temptxt = SelectedText;
                    selectall = this.ScrollControl.isSelectedAll;
                    this.ScrollControl.RemoveSelectedText(this.ScrollControl.TextSelectionPointer);
                    this.ScrollControl.ClearSelection();
                    this.ScrollControl.ScrollRows.ScrollInView(temppointer.StartLine);
                    this.ScrollControl.MoveCursorToLineItem(temppointer.StartLine);
                    if (this.ScrollControl.Caret != null)
                    {
                        this.ScrollControl.CaretIndex = this.ScrollControl.Caret.MoveToLocation(temppointer.StartIndex);
                    }
                    currentCursorIndex = temppointer.StartIndex;
                    currentLineNumber = temppointer.StartLine;
                }
            }

            if (this.isIntellisenseBoxOpen && this.intellisenseBox.SelectedItem != null)
            {
                intellisenseArgs.SelectedItem = this.intellisenseBox.SelectedItem as IIntellisenseItem;
                this.UpdateSelectedIntellisenseItem(this.intellisenseBox.SelectedItem as IIntellisenseItem);
                return;
            }
            else if (this.isIntellisenseBoxOpen && this.intellisenseBox.SelectedItem == null)
            {
                WordDetails word = this.ScrollControl.GetCurrentWord(currentCursorIndex);
                if (word != null && previousSelectedItem != null)
                {
                    IIntellisenseItem item = this.CurrentLanguage.GetMatchingIntellisenseItem(previousSelectedItem.Text);
                    if (item != null)
                    {
                        this.UpdateSelectedIntellisenseItem(item);
                        return;
                    }
                }

                if (word != null)
                {
                    IIntellisenseItem item = this.CurrentLanguage.GetItemStartingWith(word.Text.Substring(0, 1));
                    if (item != null)
                    {
                        this.UpdateSelectedIntellisenseItem(item);
                        return;
                    }
                }
            }

            StringBuilder tabString = new StringBuilder(string.Empty);
            for (int i = 0; i < this.TabSpaces; i++)
            {
                tabString.Append(" ");
            }

            if (Keyboard.GetKeyStates(Key.Insert) == KeyStates.Toggled)
            {
                this.Lines[currentLineNumber].Text = InsertionManager.ReplaceText(this.Lines[currentLineNumber].Text, tabString.ToString(), currentCursorIndex);
            }
            else
            {
                this.Lines[currentLineNumber].Text = InsertionManager.InsertText(this.Lines[currentLineNumber].Text, tabString.ToString(), currentCursorIndex);
            }
            AddUndoManager = false;
            UndoManager.IsNewUndoItem = true;
            UndoManager.Add(new EditAction()
            {
                Action = ActionType.Tab,
                Text = tabString.ToString(),
                LineNumber = currentLineNumber + 1,
                CursorIndex = currentCursorIndex,
                IsSelected = isselected,
                SelectedText = temptxt,
                Pointer = temppointer,
                IsSelectAll = selectall
            });
            AddUndoManager = true;
            if (scrollControl.Caret != null)
            {
                scrollControl.CaretIndex = scrollControl.Caret.MoveTo(this.TabSpaces);
                if (this.ScrollControl.Caret.CaretPosition.X > this.ScrollControl.HorizontalOffset && this.ScrollControl.Caret.CaretPosition.X > this.ScrollControl.ViewportWidth)
                {
                    this.ScrollControl.SetHorizontalOffset(this.ScrollControl.Caret.CaretPosition.X);
                }
            }
        }

        /// <summary>
        /// Helper method for Insert new line operation (Ctrl + Enter)
        /// </summary>
        private void ExecuteInsertNewLine()
        {
            if (this.isIntellisenseBoxOpen && this.intellisenseBox.SelectedItem != null)
            {
                this.UpdateSelectedIntellisenseItem(this.intellisenseBox.SelectedItem as IIntellisenseItem);
                this.CurrentLanguage.HideIntellisensePopup();
                return;
            }

            int currentLineNumber = this.LineNumber - 1;
            int currentIndex = this.CursorIndex;
            LineItem newItem = new LineItem();
            newItem.SetCursorOnLoad = true;
            newItem.SetCursorIndex = 0;
            this.ScrollControl.CaretIndex = 0;
            AddUndoManager = false;
            UndoManager.IsNewUndoItem = true;
            UndoManager.Add(new EditAction()
            {
                Action = ActionType.Enter,
                LineNumber = currentLineNumber + 1,
                CursorIndex = mlines[currentLineNumber].Text.Length,
                NewLine = true,
                IsSelected = false,
                SelectedText = string.Empty,
                Pointer = null,
                IsSelectAll = false,
                Text = mlines[currentLineNumber].Text
            });

            this.isAddingLinesCompleted = true;
            this.ScrollControl.rowheights.InsertLines(currentLineNumber, 1, null);
            this.Lines.Insert(currentLineNumber, newItem);
            this.ScrollControl.ScrollRows.ScrollInView(currentLineNumber + 1);
            this.CurrentLanguage.ApplyExpandItems();
            this.isReinitializeLines = false;
            this.Text = this.GetText();
            isReinitializeLines = true;
            AddUndoManager = true;
        }

        /// <summary>
        /// Helper method to intialize CurrentLanguage property based on the
        /// DocumentLanguage property.
        /// </summary>
        /// <remarks>
        /// Whenever a new language support is added to EditControl, its corresponding
        /// initialization need to be done in this method
        /// </remarks>
        /// <param name="languages">represents the DocumentLanguage property</param>
        private void InitializeLanguage(Languages languages)
        {
            switch (languages)
            {
                case Languages.Text:
                    this.CurrentLanguage = new TextLanguage(this);
                    break;

                case Languages.CSharp:
                    this.CurrentLanguage = new CSharpLanguage(this);
                    break;

                case Languages.VisualBasic:
                    this.CurrentLanguage = new VBLanguage(this);
                    break;

                case Languages.XAML:
                    this.CurrentLanguage = new XAMLLanguage(this);
                    break;

                case Languages.XML:
                    this.CurrentLanguage = new XMLLanguage(this);
                    break;

                case Languages.SQL:
                    this.CurrentLanguage = new SQLLanguage(this);
                    break;
            }
        }

        /// <summary>
        /// Helper method to insert text in the clipboard. This method gets called when Paste command is executed.
        /// </summary>
        /// <param name="line">The line number.</param>
        /// <param name="cursorindex">The cursorindex.</param>
        /// <returns>Returns the text count from the startindex to end index.</returns>
        private int AddClipboardText(int line, int cursorindex)
        {
            AddUndoManager = false;
            bool selectall = false;
            bool isselected = false;
            int index = cursorindex;
            SelectionPointer selecttext = null;
            string selectedtext = string.Empty;
            if (this.SelectedText != string.Empty)
            {
                if (this.Lines.Count > this.ScrollControl.TextSelectionPointer.EndLine)
                {
                    isselected = true;
                    selectedtext = SelectedText;
                    selecttext = ScrollControl.TextSelectionPointer;
                    line = ScrollControl.TextSelectionPointer.StartLine;
                    index = ScrollControl.TextSelectionPointer.StartIndex;
                    this.CurrentLanguage.isThreadRunning = true;
                    this.ScrollControl.RemoveSelectedText(this.ScrollControl.TextSelectionPointer);
                }

                selectall = ScrollControl.isSelectedAll;
                this.ScrollControl.ClearSelection();
                this.ScrollControl.TextSelectionPointer = null;
            }

            string strtext = string.Empty;
            if (EnvironmentTest.IsSecurityGranted)
            {
                DataObject obj = (DataObject)Clipboard.GetDataObject();
                if (Clipboard.GetData(DataFormats.Text.ToString()) == null)
                {
                    return 0;
                }

                strtext = Clipboard.GetData(DataFormats.Text.ToString()).ToString();
            }
            else
            {
                if (clipboardtext == string.Empty)
                {
                    return 0;
                }

                strtext = clipboardtext;
            }

            this.isReinitializeLines = false;
            SelectionPointer undopointer = PasteText(line, index, strtext);
            UndoManager.IsNewUndoItem = true;
            UndoManager.Add(new EditAction()
            {
                MultilinePointer = undopointer,
                Pointer = selecttext,
                IsSelected = isselected,
                CursorIndex = undopointer.StartIndex,
                LineNumber = undopointer.StartLine + 1,
                Action = ActionType.Paste,
                Text = strtext,
                SelectedText = selectedtext,
                IsSelectAll = selectall
            });
            this.CurrentLanguage.isThreadRunning = false;
            this.CurrentLanguage.ApplyExpandItems();
            AddUndoManager = true;
            return undopointer.EndLine - undopointer.StartLine + 1;
        }

        /// <summary>
        /// Helper method to add text to the control based on Action performed
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>Returns the cursor index.</returns>
        internal int AddText(EditAction action)
        {
            if (action.NewLine)
            {
                LineItem item = mlines[action.LineNumber - 1];
                LineItem newItem = new LineItem();
                newItem.Text = item.Text.Substring(Math.Min(action.CursorIndex, item.Text.Length));
                item.Text = item.Text.Substring(0, Math.Min(action.CursorIndex, item.Text.Length));

                this.Lines.Insert(action.LineNumber, newItem);
                return 0;
            }
            else
            {
                LineItem item = this.Lines[action.LineNumber - 1];
                item.Text = InsertionManager.InsertText(item.Text, action.Text, action.CursorIndex);
            }

            return action.CursorIndex;
        }

        /// <summary>
        /// Helper method to perform undo operation for enter key
        /// </summary>
        /// <param name="action">The Edit action.</param>
        /// <returns>Returns the Cursor index</returns>
        internal int UndoEnter(EditAction action)
        {
            int cursorindex = 0;
            LineItem item = this.Lines[action.LineNumber - 1];
            cursorindex = item.Text.Length;
            if (item.LineNumber == action.LineNumber)
            {
                item.Text = item.Text + this.Lines[action.LineNumber].Text;
            }
            else
            {
                item.Text = this.Lines[action.LineNumber].Text;
            }

            if (item.Text != string.Empty)
            {
                //if (this.ScrollControl.Caret != null)
                //{
                //    this.ScrollControl.Caret.MoveToLocation(item.Text.Length);
                //}
                //cursorindex = item.Text.Length;
            }
            else
            {
                item.Text = item.Text + this.Lines[action.LineNumber].Text;
            }
            //this.RemoveLineItemChildren(this.panel.Children.IndexOf(this.Lines[action.LineNumber]));
            this.Lines.RemoveAt(action.LineNumber);
            //this.UpdateLineNumber(action.LineNumber + 1, -1);
            if (action.IsSelected)
            {
                var pointer = PasteText(action.LineNumber - 1, action.CursorIndex, action.SelectedText);
                this.ScrollControl.UpdateSelectionPointer(pointer.StartLine, pointer.EndLine, pointer.StartIndex, pointer.EndIndex);
            }
            return cursorindex;
        }

        /// <summary>
        /// Helper method to paste the text that is selected before performing any action
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>Returns the End pointer of the pasted text</returns>
        internal int PasteSelectedText(EditAction action)
        {
            PasteText(action.LineNumber - 1, action.Pointer.StartIndex, action.SelectedText);
            return action.Pointer.EndIndex;
        }

        /// <summary>
        /// Helper method to remove text based on the edit action
        /// </summary>
        /// <param name="action">The Edit action.</param>
        /// <returns>Returns the Cursor index</returns>
        internal int RemoveText(EditAction action)
        {
            LineItem item = this.Lines[action.LineNumber - 1];
            if (item.Text != string.Empty)
            {
                if (item.Text.Length >= action.CursorIndex + action.Text.Length)
                {
                    item.Text = item.Text.Remove(action.CursorIndex, action.Text.Length);
                }
            }
            return action.CursorIndex;
        }

        /// <summary>
        /// Helper method to perform Redo opertion for enter key
        /// </summary>
        /// <param name="action">The Edit Action.</param>
        /// <returns>Returns the Cursor index</returns>
        internal int RedoEnter(EditAction action)
        {
            if (action.IsSelected)
            {
                this.ScrollControl.RemoveSelectedText(action.Pointer);
            }

            LineItem item = mlines[action.LineNumber - 1];
            LineItem newItem = new LineItem();

            if (item.Text != string.Empty)
            {
                if (item.LineNumber == action.LineNumber)
                {
                    newItem.Text = item.Text.Remove(action.CursorIndex - action.Text.Length, action.Text.Length);
                    item.Text = action.Text;
                }
            }

            newItem.SetCursorOnLoad = true;
            newItem.SetCursorIndex = 0;

            mlines.Insert(action.LineNumber, newItem);

            if (action.Action != ActionType.Enter)
            {
                mlines[LineNumber - 1].Text = string.Empty;
                this.ScrollControl.CurrentLineItem.Text = string.Empty;
            }

            return action.CursorIndex;
        }

        /// <summary>
        /// Helper method to paste operation
        /// </summary>
        /// <param name="line">The line number.</param>
        /// <param name="cursorindex">The cursorindex.</param>
        /// <param name="strtext">The start text.</param>
        /// <returns>Returns the Selection pointer</returns>
        internal SelectionPointer PasteText(int line, int cursorindex, string strtext)
        {
            //bool lineinserted = false;
            int startlinelength = this.Lines[line].Text.Length;
            int difflen = startlinelength - cursorindex;
            SelectionPointer returnpointer = new SelectionPointer();
            returnpointer.StartLine = line;
            returnpointer.StartIndex = cursorindex;
            strtext = InsertionManager.InsertText(this.Lines[line].Text, strtext, cursorindex);
            string[] strline = Regex.Split(strtext, this.CurrentLanguage.SplitLinesRegex);
            isAddingLinesCompleted = false;
            this.CurrentLanguage.isUpdating = true;
            for (int i = 0; i < strline.Length; i++)
            {
                if (i == strline.Length - 1)
                {
                    isAddingLinesCompleted = true;
                    this.CurrentLanguage.isUpdating = false;
                }
                if (strline[i].Contains("\t") && TabSpaces != 0)
                {
                    string t = "";
                    for (int m = 0; m < TabSpaces; m++) t = t + " ";
                    strline[i] = strline[i].Replace("\t", t);
                }
                if (i == 0)
                {
                    IsPaste = true;
                    this.Lines[line].Text = strline[i].ToString();

                    if (strline.Length == 1)
                    {
                        this.ScrollControl.MoveCursorToLineItem(line);
                        if (this.ScrollControl.Caret != null)
                        {
                            this.ScrollControl.CaretIndex = this.ScrollControl.Caret.MoveTo(strline[i].Length - startlinelength);
                        }
                        else
                        {
                            this.Lines[line].SetCursorIndex = strline[i].Length - startlinelength;
                            this.Lines[line].SetCursorOnLoad = true;
                        }
                        returnpointer.EndIndex = cursorindex + strline[i].Length - startlinelength;
                    }
                }
                else
                {
                    if (i == strline.Length)
                    {
                        if ((line + i) < this.Lines.Count)
                        {
                            this.Lines[line + i].Text = strline[i].ToString();
                        }
                    }
                    else
                    {
                        LineItem item = new LineItem(strline[i].ToString());
                        if (i == strline.Length - 1)
                        {
                            item.SetCursorOnLoad = true;
                            item.SetCursorIndex = strline[i].ToString().Length - difflen;
                            returnpointer.EndIndex = strline[i].ToString().Length - difflen;
                        }
                        this.ScrollControl.rowheights.InsertLines(line + i, 1, null);
                        this.Lines.Insert(line + i, item);
                        //lineinserted = true;
                    }
                }
            }

            returnpointer.EndLine = line + strline.Length - 1;
            //lineinserted = false;
            if (returnpointer.StartLine > returnpointer.EndLine)
            {
                int temp = returnpointer.StartLine;
                returnpointer.StartLine = returnpointer.EndLine;
                returnpointer.EndLine = temp;
                temp = returnpointer.EndIndex;
                returnpointer.EndIndex = returnpointer.StartIndex;
                returnpointer.StartIndex = temp;
            }

            if (returnpointer.StartLine == returnpointer.EndLine)
            {
                if (returnpointer.StartIndex > returnpointer.EndIndex)
                {
                    int temp = returnpointer.EndIndex;
                    returnpointer.EndIndex = returnpointer.StartIndex;
                    returnpointer.StartIndex = temp;
                }
            }

            this.ScrollControl.ScrollRows.ScrollInView(returnpointer.EndLine);
            return returnpointer;
        }

        /// <summary>
        /// Helper method that returns the text in the control
        /// </summary>
        /// <returns>Returns the all text of the presenter</returns>
        internal string GetText()
        {
            StringBuilder str = new StringBuilder(string.Empty);
            foreach (LineItem item in this.Lines)
            {
                str.Append(item.Text);
                if (item.LineNumber < this.Lines.Count)
                {
                    str.Append("\r\n");
                }
            }
            return str.ToString();
        }

        /// <summary>
        /// Helper method that returns the text in the control with in start and end index
        /// </summary>
        /// <returns>Returns the all text of the presenter</returns>
        public string GetTextRange(int start, int end)
        {
            StringBuilder str = new StringBuilder(string.Empty);
            for (int i = start; i <= end && i >= 0 && i < this.Lines.Count; i++)
            {
                var item = this.Lines[i];
                str.Append(item.Text);
                if (item.LineNumber < this.Lines.Count && i < end)
                {
                    str.Append("\r\n");
                }
            }
            return str.ToString();
        }

        /// <summary>
        /// Save method is used to save the text in the EditControl under a file name with
        /// different supported file types.
        /// </summary>
        /// <remarks>
        /// <para>When Save method is called a SaveFileDialog method is displayed in order
        /// for the users to provide the file name and select target locations.</para>
        /// </remarks>
        /// <returns>
        /// <para>Returns true when Save Dialog box opened otherwise return false</para>
        /// </returns>
        /// <example>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>EditControl1.Save();</para>
        /// </example>
        public bool SaveFile()
        {
            Stream stream;
            SaveFileDialog m_savedialog = new SaveFileDialog();
            //AddFiltersToOpenDialog(m_savedialog);
            if (m_savedialog.ShowDialog() == false)
            {
                return false;
            }

            if (m_savedialog.FileName.IndexOf(".") < 0 && m_savedialog.FilterIndex < knownLanguages.Count)
            {
                m_savedialog.FileName = m_savedialog.FileName + knownLanguages[m_savedialog.FilterIndex].ToString();
            }

            if ((stream = m_savedialog.OpenFile()) != null)
            {
                StreamWriter streamtext = new StreamWriter(stream);
                streamtext.Write(Text);
                streamtext.Close();
                stream.Close();
                ChangeLinesState(LineModificationState.Modified, LineModificationState.Saved);
            }
            return true;
        }

        /// <summary>
        /// Save method is used to save the text in the EditControl under the file name
        /// specified
        /// </summary>
        /// <param name="FileName">represents the file name under which the text needs to be
        /// saved.</param>
        /// <returns>
        /// <para>Returns true when Save Dialog box opened otherwise return false</para>
        /// </returns>
        /// <example>
        /// <para><b>C#</b></para>
        /// <para> </para>
        /// <para>EditControl1.Save(&quot;C:\test.txt&quot;);</para>
        /// </example>
        public bool SaveFile(string FileName)
        {
            if (FileName.Trim() != string.Empty)
            {
                FileInfo info = new FileInfo(FileName);
                if (info.Exists)
                {
                    StreamWriter writer = new StreamWriter(FileName);
                    writer.Write(this.Text);
                    writer.Close();
                    ChangeLinesState(LineModificationState.Modified, LineModificationState.Saved);
                    return true;
                }
            }
            return false;
        }

        internal void ChangeLinesState(LineModificationState fromState, LineModificationState toState)
        {
            if (this.Lines.Count > 0)
            {
                var lines = this.Lines.Where(item => item.LineState == fromState);
                foreach (LineItem item in lines)
                {
                    item.LineState = toState;
                }
            }
        }

        /// <summary>
        /// Returns true or false based in order for the ExpandAll command to be executed
        /// </summary>
        /// <returns>Return true when the language is not text otherwise return false.</returns>
        internal bool GetExpandAllCanExecute()
        {
            var p = from line in this.Lines
                    where line.ContainsLines == true
                    select line;
            int count = p.Count<LineItem>();

            p = from line in mlines
                where line.ContainsLines == true && line.IsExpanded == true
                select line;
            int expcount = p.Count<LineItem>();
            if (count == expcount || DocumentLanguage == Languages.Text)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Helper method to append empty string at the begining of the line's text.
        /// </summary>
        /// <param name="line">represents the line number</param>
        /// <param name="indentsize">represents the number of spaces to be included for an indent.</param>
        internal void IncreaseIndent(LineItem line, int indentsize)
        {
            if (line.Text.Trim() != String.Empty)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < indentsize; i++)
                {
                    stringBuilder.Append(" ");
                }

                int wordStartIndex = 0;
                var words = line.WordsCollection.Where(word => word.Text.Trim() != string.Empty).OrderBy(word => word.StartIndex);
                if (words.Count() > 0)
                {
                    wordStartIndex = words.ElementAt(0) != null ? words.ElementAt(0).StartIndex : 0;
                }

                line.Text = line.Text.Insert(0, stringBuilder.ToString());
                int lineNo = this.Lines.IndexOf(line);
                if (wordStartIndex == this.CursorIndex || isIndentSelected)
                {
                    if (isIndentSelected)
                    {
                        SelectionPointer pointer = this.ScrollControl.TextSelectionPointer;
                        this.ScrollControl.ClearSelection();
                        this.ScrollControl.UpdateSelectionPointer(lineNo, lineNo, pointer.StartIndex, pointer.EndIndex + indentsize);
                        isIndentSelected = true;
                    }
                    else
                    {
                        this.ScrollControl.UpdateSelectionPointer(lineNo, lineNo, wordStartIndex, wordStartIndex + indentsize);
                        isIndentSelected = true;
                    }
                }
                else
                {
                    isIndentSelected = false;
                }
            }
        }

        /// <summary>
        /// Helper method to append empty string at the begining of the all the lines with in the selected range
        /// </summary>
        /// <param name="selection">represents the text selection area</param>
        /// <param name="indentsize">represents the number of spaces to be included for an indent.</param>
        internal void IncreaseIndent(SelectionPointer selection, int indentsize)
        {
            if (selection == null)
                return;

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < indentsize; i++)
            {
                stringBuilder.Append(" ");
            }

            for (int i = selection.StartLine; i <= selection.EndLine; i++)
            {
                var line = this.Lines[i];
                if (line.Text.Trim() != String.Empty)
                {
                    line.Text = line.Text.Insert(0, stringBuilder.ToString());
                }
            }
            var selectionPtr = selection;
            this.ScrollControl.ClearSelection();
            this.ScrollControl.UpdateSelectionPointer(selectionPtr.StartLine, selectionPtr.EndLine, selectionPtr.StartIndex + indentsize, selectionPtr.EndIndex + indentsize);
            this.SelectedText = this.ScrollControl.GetSelectedText(this.ScrollControl.TextSelectionPointer);
            if (this.ScrollControl.Caret != null)
            {
                this.ScrollControl.CaretIndex = this.ScrollControl.Caret.MoveTo(indentsize);
            }
        }

        /// <summary>
        /// Helper method to remove appended empty string at the begining of the line's text.
        /// </summary>
        /// <param name="line">represents the line number</param>
        /// <param name="indentsize">represents the number of spaces to be included for an indent.</param>
        internal void DecreaseIndent(LineItem line, int indentsize)
        {
            if (line.Text.Trim() != String.Empty)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < indentsize; i++)
                {
                    stringBuilder.Append(" ");
                }

                int wordStartIndex = 0;
                var words = line.WordsCollection.Where(word => word.Text.Trim() != string.Empty).OrderBy(word => word.StartIndex);
                if (words.Count() > 0)
                {
                    wordStartIndex = words.ElementAt(0) != null ? words.ElementAt(0).StartIndex : 0;
                }

                if (line.Text.IndexOf(stringBuilder.ToString()) == 0)
                {
                    line.Text = line.Text.Substring(indentsize);

                    if (this.ScrollControl.Caret != null)
                    {
                        this.ScrollControl.CaretIndex = this.ScrollControl.Caret.MoveTo(indentsize * -1);
                    }
                }

                if (this.isIndentSelected)
                {
                    SelectionPointer pointer = this.ScrollControl.TextSelectionPointer;
                    this.ScrollControl.ClearSelection();
                    if (pointer.EndIndex - indentsize > 0)
                    {
                        this.ScrollControl.UpdateSelectionPointer(pointer.StartLine, pointer.EndLine, this.ScrollControl.CaretIndex, Math.Max(pointer.EndIndex - indentsize, 0));
                        isIndentSelected = true;
                    }
                }
            }
        }

        /// <summary>
        /// Helper method to append empty string at the begining of the all the lines with in the selected range
        /// </summary>
        /// <param name="selection">represents the text selection area</param>
        /// <param name="indentsize">represents the number of spaces to be included for an indent.</param>
        internal void DecreaseIndent(SelectionPointer selection, int indentsize)
        {
            if (selection == null)
                return;

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < indentsize; i++)
            {
                stringBuilder.Append(" ");
            }
            bool isUpdated = false;
            bool iscurrentLineUpdated = false;
            for (int i = selection.StartLine; i <= selection.EndLine; i++)
            {
                var line = this.Lines[i];
                if (line.Text.Trim() != String.Empty)
                {
                    if (line.Text.IndexOf(stringBuilder.ToString()) >= 0)
                    {
                        line.Text = line.Text.Substring(indentsize);
                        line.SelectionStartIndex -= indentsize;
                        line.SelectionEndIndex -= indentsize;
                        line.SelectionStartIndex = line.SelectionStartIndex < 0 ? 0 : line.SelectionStartIndex;
                        line.SelectionEndIndex = line.SelectionEndIndex < 0 ? 0 : line.SelectionEndIndex;
                        isUpdated = true;
                        if (this.LineNumber == (i + 1))
                        {
                            iscurrentLineUpdated = true;
                        }
                    }
                }
            }
            if (isUpdated)
            {
                var selectionPtr = selection;
                this.ScrollControl.ClearSelection();
                this.ScrollControl.UpdateSelectionPointer(selectionPtr.StartLine, selectionPtr.EndLine, this.Lines[selectionPtr.StartLine].SelectionStartIndex, this.Lines[selectionPtr.EndLine].SelectionEndIndex);
                this.SelectedText = this.ScrollControl.GetSelectedText(this.ScrollControl.TextSelectionPointer);
                if (this.ScrollControl.Caret != null && iscurrentLineUpdated)
                {
                    this.ScrollControl.CaretIndex = this.ScrollControl.Caret.MoveTo(indentsize * -1);
                }
            }
        }

        /// <summary>
        /// Helper method to initialize Find Replace Window
        /// </summary>
        private void InitializeFindObject()
        {
            if (replaceControl == null)
            {
                replaceControl = new FindReplaceControl();
            }

            replaceControl.DataContext = this.FindOptions;
            this.FindOptions.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(FindOptions_PropertyChanged);
            this.SearchResults = new SearchResult();

            pinButton = this.GetTemplateChild("PART_PinButton") as ToggleButton;
            pinButton.Click += new RoutedEventHandler(pinButton_Click);
            Grid findResultsGrid = this.GetTemplateChild("PART_FindAllReferencesGrid") as Grid;
            if (findResultsGrid != null)
            {
                findResultsGrid.MouseLeave += new MouseEventHandler(findResultsGrid_MouseLeave);
            }

            findResultsTab = this.GetTemplateChild("PART_FindAllReferencesTab") as TabControl;
            if (findResultsTab != null)
            {
                findResultsTab.SelectionChanged += new SelectionChangedEventHandler(findResultsTab_SelectionChanged);
                findResultsTab.MouseLeave += new MouseEventHandler(findResultsTab_MouseLeave);
                item = findResultsTab.Items.Count > 0 ? findResultsTab.Items[0] as TabItem : null;
                if (item != null)
                {
                    item.MouseMove += new MouseEventHandler(item_MouseMove);
                }
            }

            findResultsList = this.GetTemplateChild("PART_FindAllReferencesList") as ListBox;
            if (findResultsList != null)
            {
                findResultsList.ItemsSource = this.SearchResults.FindAllResult;
                findResultsList.MouseDoubleClick += new MouseButtonEventHandler(list_MouseDoubleClick);
                findResultsList.MouseMove += new MouseEventHandler(findResultsList_MouseMove);
            }

            findResultsTitleBar = this.GetTemplateChild("PART_FindAllReferencesTitleBar") as Border;

            Button closeButton = this.GetTemplateChild("PART_CloseButton") as Button;
            closeButton.Click += new RoutedEventHandler(closeButton_Click);
        }

        private void findResultsTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// ExpandLine method
        /// </summary>
        /// <param name="index"></param>
        public void ExpandLine(int index)
        {
            if (index >= this.Lines.Count)
            {
                return;
            }

            var item = this.Lines[index];
            item.IsExpanded = true;
            this.ScrollControl.UpdateExpandStatus(item.StartLine - 1, item.EndLine - 1, false);
            this.ScrollControl.InvalidateVisual();
        }

        /// <summary>
        /// ExpandLineUpTopLevel method
        /// </summary>
        /// <param name="index"></param>
        public void ExpandLineUpTopLevel(int index)
        {
            LineItem tempItem = this.Lines[index];
            int parIndex = tempItem.ParentLineNumber - 1;
            while (parIndex >= 0 && parIndex < this.Lines.Count)
            {
                tempItem = this.Lines[parIndex];
                tempItem.IsExpanded = true;
                this.ScrollControl.UpdateExpandStatus(tempItem.StartLine - 1, tempItem.EndLine - 1, false);
                parIndex = tempItem.ParentLineNumber - 1;
            }
            this.ScrollControl.InvalidateVisual(true);
        }

        /// <summary>
        /// Helper method to initialize IntellisensePopup
        /// </summary>
        private void InitializeIntellisensePopup()
        {
            intellisensePopup = this.GetTemplateChild("PART_IntellisensePopup") as Popup;
            intellisenseBox = this.GetTemplateChild("PART_IntellisenseBox") as ListBox;
            if (intellisensePopup != null)
            {
                intellisensePopup.AllowsTransparency = true;
                intellisensePopup.StaysOpen = true;
                intellisenseBox.MouseDoubleClick += new MouseButtonEventHandler(intellisenseBox_MouseDoubleClick);
                intellisensePopup.PlacementTarget = this;
                intellisensePopup.Closed += new EventHandler(intellisensePopup_Closed);
                intellisenseBox.SelectionChanged += new SelectionChangedEventHandler(intellisenseBox_SelectionChanged);
            }
        }

        private void intellisenseBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (intellisenseBox.SelectedItem == null && intellisenseBox.ItemsSource != null && e.RemovedItems.Count > 0)
            {
                previousSelectedItem = e.RemovedItems[0] as IIntellisenseItem;
            }
        }

        private void intellisensePopup_Closed(object sender, EventArgs e)
        {
            this.isIntellisenseBoxOpen = false;
        }

        /// <summary>
        /// Helper method to update the selected intellisense item contents
        /// </summary>
        /// <param name="info"></param>
        internal void UpdateSelectedIntellisenseItem(IIntellisenseItem info)
        {
            if (info == null)
                return;

            WordDetails word = this.ScrollControl.GetCurrentWord(this.CursorIndex);
            string tempStr = this.Lines[this.LineNumber - 1].Text;
            string replaceStr = info.Text;
            int cursorIndex = this.CursorIndex;
            string replaceText = string.Empty;
            if (word != null)
            {
                replaceText = word.Text;
                int startIndex = word.StartIndex;
                int textLength = word.Text.Length;
                if (word.EndIndex == cursorIndex
                   && (word.Text.EndsWith(this.CurrentLanguage.IntellisenseDrillDownChar.ToString()) || word.Text.EndsWith(".")))
                {
                    startIndex = cursorIndex;
                    textLength = 0;
                    replaceText = "";
                }
                if (word.Text != " " && word.Text != ".")
                {
                    tempStr = tempStr.Remove(startIndex, textLength);
                    tempStr = tempStr.Insert(startIndex, info.Text);
                    cursorIndex = startIndex;
                }
                else
                {
                    tempStr = tempStr.Insert(word.EndIndex, info.Text);
                    cursorIndex = word.EndIndex;
                }
            }
            else
            {
                tempStr = info.Text;
            }

            this.Lines[this.LineNumber - 1].Text = tempStr;
            AddUndoManager = false;
            if (info.Text != replaceText)
            {
                UndoManager.IsNewUndoItem = true;

                UndoManager.Add(new EditAction()
                {
                    Action = ActionType.Replace,
                    Text = replaceText,
                    LineNumber = this.LineNumber,
                    CursorIndex = cursorIndex,
                    SelectedText = replaceStr,
                    IsSelected = false
                });
            }

            this.ScrollControl.MoveCursorToLineItem(this.LineNumber - 1);
            this.ScrollControl.CaretIndex = this.ScrollControl.Caret.MoveToLocation(cursorIndex + info.Text.Length);
            this.CurrentLanguage.HideIntellisensePopup();
            this.Focus();
            this.isReinitializeLines = false;
            this.Text = this.GetText();
            isReinitializeLines = true;
            AddUndoManager = true;
        }

        /// <summary>
        /// A method to select single line or multiline text with in the specified range.
        /// </summary>
        /// <remarks>
        /// All the parameters are 0 based index values.
        /// </remarks>
        /// <param name="startLine">represents the selection start line number.</param>
        /// <param name="endLine">represents the selection end line number</param>
        /// <param name="startIndex">represents the selection start index.</param>
        /// <param name="endIndex">represents the selection end index.</param>
        public void SelectLines(int startLine, int endLine, int startIndex, int endIndex)
        {
            this.ScrollControl.ClearSelection();
            this.ScrollControl.ScrollRows.ScrollInView(startLine);
            this.ScrollControl.MoveCursorToLineItem(startLine);

            if (this.ScrollControl.Caret != null)
            {
                this.ScrollControl.Caret.MoveToLocation(startIndex);
            }
            else
            {
                this.Lines[startLine].SetCursorOnLoad = true;
                this.Lines[startLine].SetCursorIndex = startIndex;
            }

            this.ScrollControl.UpdateSelectionPointer(startLine, endLine, startIndex, endIndex);
            this.SelectedText = this.scrollControl.GetSelectedText(this.scrollControl.TextSelectionPointer);
        }

        internal void AutoIndentAllLines()
        {
            if (IsAutoIndentationEnabled && this.CurrentLanguage.SupportsOutlining)
            {
                this.CurrentLanguage.isThreadRunning = true;
                for (int i = 0; i < Lines.Count; i++)
                {
                    this.ApplyIndentation(i + 1, false);
                }
                this.CurrentLanguage.isThreadRunning = false;
                this.ScrollControl.InvalidateVisual(true);
            }
        }

        #endregion Implementation

        #region Events

        /// <summary>
        /// Event handler to update the visibility of caret when scrolled horizontally
        /// </summary>
        /// <param name="sender">represents the ScrollViewer in the control template</param>
        /// <param name="e">represents ScrollChangedEventArgs</param>
        private void viewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.HorizontalChange != 0)
            {
                if (this.ScrollControl.Caret != null)
                {
                    double x = this.ScrollControl.Caret.CaretPosition.X;
                    if (x >= e.HorizontalOffset && x <= (e.HorizontalOffset + e.ViewportWidth))
                    {
                        this.ScrollControl.Caret.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.ScrollControl.Caret.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        /// <summary>
        /// Event handler to update the text when an item is double clicked on the intellisense box.
        /// </summary>
        /// <param name="sender">represents the listbox in the intellisense popup</param>
        /// <param name="e">represents MouseButtonEventArgs</param>
        private void intellisenseBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UpdateSelectedIntellisenseItem(this.intellisenseBox.SelectedItem as IIntellisenseItem);
        }

        /// <summary>
        /// Event handler to update the focus
        /// </summary>
        /// <param name="sender">represents the ScrollViewer in the control template</param>
        /// <param name="e">represents RoutedEventArgs</param>
        private void viewer_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.ScrollControl.Caret != null)
            {
                this.ScrollControl.Caret.Visibility = Visibility.Collapsed;
            }

            if (this.isIntellisenseBoxOpen)
            {
                IInputElement element = Keyboard.FocusedElement;
                ScrollViewer tempViewer = this.GetTemplateChild("PART_Intellisense_Scroll") as ScrollViewer;
                if (!((element is ListBoxItem && (element as ListBoxItem).Content is IIntellisenseItem) || element.Equals(this.intellisenseBox) || element.Equals(this.intellisensePopup) || element.Equals(tempViewer)))
                {
                    this.CurrentLanguage.HideIntellisensePopup();
                }
            }
        }

        /// <summary>
        /// Event handler to update the focus
        /// </summary>
        /// <param name="sender">represents the ScrollViewer in the control template</param>
        /// <param name="e">represents RoutedEventArgs</param>
        private void viewer_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.ScrollControl.Caret != null)
            {
                this.ScrollControl.Caret.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Event handler to update the <see cref="Text"/> on LineItemsColletionChange.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">represents NotifyCollectionChangedEventArgs</param>
        private void Lines_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.isReinitializeLines)
            {
                string text = GetText();
                if (this.Text != text && text != "")
                    this.Text = text;
                this.isReinitializeLines = false;
            }
        }

        #endregion Events

        #region Overrides

        /// <summary>
        /// This method gets called when the control template is applied
        /// </summary>
        public override void OnApplyTemplate()
        {
            scrollControl = this.GetTemplateChild("PART_ScrollControl") as EditScrollControl;
            if (scrollControl != null)
            {
                scrollControl.InitializeValues(this);
            }
            viewer = this.GetTemplateChild("PART_Scroll") as ScrollViewer;
            if (viewer != null)
            {
                viewer.GotFocus += new RoutedEventHandler(viewer_GotFocus);
                viewer.LostFocus += new RoutedEventHandler(viewer_LostFocus);
                viewer.ScrollChanged += new ScrollChangedEventHandler(viewer_ScrollChanged);
            }

            InitializeFindObject();
            InitializeIntellisensePopup();

            ResourceDictionary dictionary = new ResourceDictionary();
            dictionary.Source = new Uri(@"/Syncfusion.Edit.Wpf;component/Themes/Generic.xaml", UriKind.Relative);

            if (this.ShowDefaultContextMenu)
            {
                this.ContextMenu = dictionary["contextmenu"] as ContextMenu;
            }
            //else
            //{
            //    this.ContextMenu = null;
            //}
            this.CurrentLanguage.InitializeNamespaces(this.AssemblyReferences);
            base.OnApplyTemplate();
        }

        /// <summary>
        /// ContextMenuOpening override event to restrict displaying context menu in the linenumber and expand collapse region
        /// </summary>
        /// <param name="e"></param>
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            var index = this.ScrollControl.ScrollColumns.VisiblePointToLineIndex(e.CursorLeft);
            if (index < 3)
            {
                e.Handled = true;
                return;
            }
            this.Focus();
            base.OnContextMenuOpening(e);
        }

        /// <summary>
        /// Override to perform operations when text is entered in the control.
        /// In this override, Text input is captured and operations are performed based on the text input.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.TextCompositionEventArgs"/> that contains the event data.</param>
        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            KeyboardDevice device = e.Device as KeyboardDevice;
            if (device.Modifiers == ModifierKeys.Control || device.Modifiers == ModifierKeys.Alt || device.Modifiers == ModifierKeys.Windows || IsReadOnly)
            {
                return;
            }

            if (Keyboard.IsKeyDown(Key.Enter) && this.Lines[LineNumber - 1].Text != "" && DocumentLanguage == Languages.XAML)
            {
                this.CurrentLanguage.SplitTextToLines();
            }

            if (Keyboard.IsKeyDown(Key.Enter))
                IsEnterKeyPressed = true;
            if (inputtimer == null)
            {
                inputtimer = new DispatcherTimer();
                inputtimer.Interval = TimeSpan.FromMilliseconds(1000d);
                inputtimer.Tick += new EventHandler(inputtimer_Tick);
            }
            inputtimer.Stop();

            SelectionPointer temppointer = null;
            bool isselected = false;
            bool selectall = false;
            string temptxt = string.Empty;
            int currentLineNumber = LineNumber - 1;
            int currentCursorIndex = CursorIndex;
            if (SelectedText != string.Empty)
            {
                temppointer = this.ScrollControl.TextSelectionPointer;
                isselected = true;
                temptxt = SelectedText;
                selectall = this.ScrollControl.isSelectedAll;
                this.ScrollControl.RemoveSelectedText(this.ScrollControl.TextSelectionPointer);
                this.ScrollControl.ClearSelection();
                this.ScrollControl.ScrollRows.ScrollInView(temppointer.StartLine);
                this.ScrollControl.MoveCursorToLineItem(temppointer.StartLine);
                if (this.ScrollControl.Caret != null)
                {
                    this.ScrollControl.CaretIndex = this.ScrollControl.Caret.MoveToLocation(temppointer.StartIndex);
                    currentLineNumber = temppointer.StartLine;
                    currentCursorIndex = this.ScrollControl.CaretIndex;
                }
                else
                {
                    currentLineNumber = temppointer.StartLine;
                    currentCursorIndex = temppointer.StartIndex;
                }
            }

            if (e.Text != "\r")
            {
                if (e.Text == "\b")
                {
                    return;
                }

                if (this.EnableIntellisense && (this.CurrentLanguage.SupportsIntellisense || this.IntellisenseMode == IntellisenseMode.Custom))
                {
                    WordDetails prevWord = this.ScrollControl.GetPreviousWord(currentCursorIndex);

                    if (!this.isIntellisenseBoxOpen && e.Text != this.CurrentLanguage.IntellisenseDrillDownChar.ToString())
                    {
                        intellisenseArgs = new EditIntellisenseArgs();
                        intellisenseArgs.Assemblies = this.AssemblyReferences != null ? new List<Uri>(this.AssemblyReferences) : null;
                        intellisenseArgs.TextInput = e.Text;
                        intellisenseArgs.LineIndex = currentLineNumber;
                        intellisenseArgs.CursorIndex = currentCursorIndex;
                        if (ValidateShowIntellisenseBox(this.CurrentLanguage.Name, this.Lines[currentLineNumber].Text))
                        {
                            this.CurrentLanguage.ShowIntellisenseBox(intellisenseArgs);
                            int count = this.CurrentLanguage.ApplyFilterToIntellisense(e.Text);
                            if(count >0)
                            isIntellisenseBoxOpen = true;
                        }
                    }

                    char[] array = e.Text.ToCharArray();

                    if ((array.Length > 0 && this.CurrentLanguage.IntellisenseCommitCharacters != string.Empty && (this.CurrentLanguage.IntellisenseCommitCharacters.ToCharArray().Contains(array[0]))) || (this.CurrentLanguage.CommitsIntellisenseItemOnSpaceBar && e.Text == " "))
                    {
                        if (intellisenseArgs != null)
                        {
                            intellisenseArgs.SelectedItem = this.intellisenseBox.SelectedItem as IIntellisenseItem;
                        }

                        if (this.intellisenseBox.SelectedItem != null)
                        {
                            this.UpdateSelectedIntellisenseItem(this.intellisenseBox.SelectedItem as IIntellisenseItem);
                            currentCursorIndex = this.CursorIndex;
                        }
                        if (intellisenseArgs != null)
                        {
                            intellisenseArgs.SelectedItem = null;
                        }

                        if (intellisenseArgs == null)
                        {
                            intellisenseArgs = new EditIntellisenseArgs();
                        }

                        this.CurrentLanguage.HideIntellisensePopup();
                    }
                }

                if (currentLineNumber >= 0 && !this.Lines[currentLineNumber].IsExpanded)
                {
                    string collapsedText = this.CurrentLanguage.GetCollapsedItemText(this.Lines[currentLineNumber]);
                    var listener = this.Lines[currentLineNumber].GetPreprocessorType();
                    int ellipsisIndex = this.Lines[currentLineNumber].ContainsPreprocessor && listener != null ? this.Lines[currentLineNumber].Text.IndexOf(listener.BlockStart) : collapsedText.IndexOf(this.CurrentLanguage.EllipsisText);
                    int ellipsisLength = listener != null ? this.Lines[currentLineNumber].PreprocessorText.Length : this.CurrentLanguage.EllipsisText.Length;
                    int endIndex = this.CurrentLanguage.GetSelectionEndIndex(this.Lines[currentLineNumber]);
                    if (ellipsisIndex >= 0 && currentCursorIndex >= ellipsisIndex + ellipsisLength)
                    {
                        this.ExpandLine(currentLineNumber);
                        this.ScrollControl.ScrollRows.ScrollInView(this.Lines[currentLineNumber].EndLine - 1);
                        this.ScrollControl.MoveCursorToLineItem(this.Lines[currentLineNumber].EndLine - 1);
                        if (this.ScrollControl.Caret != null)
                        {
                            this.ScrollControl.Caret.MoveToLocation(endIndex);
                        }
                        else
                        {
                            var endItem = this.Lines[this.Lines[currentLineNumber].EndLine - 1];
                            endItem.SetCursorIndex = endIndex;
                            endItem.SetCursorOnLoad = true;
                        }
                        currentLineNumber = this.Lines[currentLineNumber].EndLine - 1;
                        currentCursorIndex = endIndex;
                        if (this.isIntellisenseBoxOpen)
                        {
                            this.CurrentLanguage.HideIntellisensePopup();
                        }
                    }
                }

                if (isInsertKeyToggled)
                {
                    this.Lines[currentLineNumber].Text = InsertionManager.ReplaceText(this.Lines[currentLineNumber].Text, e.Text, currentCursorIndex);
                }
                else
                {
                    this.Lines[currentLineNumber].Text = InsertionManager.InsertText(this.Lines[currentLineNumber].Text, e.Text, currentCursorIndex);
                }

                if (this.EnableIntellisense && (this.CurrentLanguage.SupportsIntellisense || this.IntellisenseMode == IntellisenseMode.Custom))
                {
                    if (e.Text == this.CurrentLanguage.IntellisenseDrillDownChar.ToString())
                    {
                        if (intellisenseArgs == null)
                        {
                            intellisenseArgs = new EditIntellisenseArgs();
                        }

                        intellisenseArgs.Assemblies = this.AssemblyReferences != null ? new List<Uri>(this.AssemblyReferences) : null;
                        intellisenseArgs.TextInput = e.Text;
                        intellisenseArgs.LineIndex = currentLineNumber;
                        intellisenseArgs.SelectedItem = this.intellisenseBox.SelectedItem as IIntellisenseItem;
                        intellisenseArgs.CursorIndex = currentCursorIndex;
                        intellisenseArgs.Cancel = false;
                        if (e.Text == this.CurrentLanguage.IntellisenseDrillDownChar.ToString())
                        {
                            intellisenseArgs.CurrentScope = this.CurrentLanguage.GetScopeDefinition(currentLineNumber);
                            this.CurrentLanguage.OnDrillDownIntellisense(intellisenseArgs);
                        }
                    }
                    else
                    {
                        WordDetails word = this.ScrollControl.GetCurrentWord(currentCursorIndex + 1);
                        if (word != null)
                        {
                            this.CurrentLanguage.ApplyFilterToIntellisense(word.Text);
                        }
                    }
                }
                AddUndoManager = false;
                UndoManager.IsNewUndoItem = true;
                UndoManager.Add(new EditAction()
                {
                    Action = ActionType.Type,
                    Text = e.Text,
                    LineNumber = currentLineNumber + 1,
                    CursorIndex = currentCursorIndex,
                    IsSelected = isselected,
                    SelectedText = temptxt,
                    Pointer = temppointer,
                    IsSelectAll = selectall
                });

                if (this.ScrollControl.Caret != null)
                {
                    scrollControl.CaretIndex = scrollControl.Caret.MoveTo(1);
                }
                else
                {
                    this.Lines[currentLineNumber].SetCursorIndex = currentCursorIndex + 1;
                    this.Lines[currentLineNumber].SetCursorOnLoad = true;
                }

                //if (this.ScrollControl.Caret != null && this.ScrollControl.Caret.CaretPosition.X > this.ScrollControl.HorizontalOffset && this.ScrollControl.Caret.CaretPosition.X > this.ScrollControl.ViewportWidth)
                //{
                //    this.ScrollControl.SetHorizontalOffset(this.ScrollControl.Caret.CaretPosition.X);
                //}
                inputtimer.Start();
            }
            else
            {
                if (this.isIntellisenseBoxOpen && this.intellisenseBox.SelectedItem != null)
                {
                    this.UpdateSelectedIntellisenseItem(this.intellisenseBox.SelectedItem as IIntellisenseItem);
                    this.CurrentLanguage.HideIntellisensePopup();
                    return;
                }

                LineItem newItem = new LineItem();
                LineItem currentItem = mlines[currentLineNumber];
                int ellipsisIndex = currentItem.Text.Length;
                string collapsedText = this.CurrentLanguage.GetCollapsedItemText(currentItem);
                BlockListener listener = currentItem.GetPreprocessorType();
                if (currentItem.ContainsPreprocessor)
                {
                    if (listener != null)
                    {
                        ellipsisIndex = currentItem.Text.IndexOf(listener.BlockStart);
                    }
                }
                else
                {
                    ellipsisIndex = collapsedText.IndexOf(this.CurrentLanguage.EllipsisText);
                }

                if (mlines[currentLineNumber].IsExpanded || (!currentItem.IsExpanded && currentCursorIndex <= ellipsisIndex))
                {
                    if (currentCursorIndex < mlines[currentLineNumber].Text.Length)
                    {
                        newItem.Text = mlines[currentLineNumber].Text.Substring(currentCursorIndex);
                        mlines[currentLineNumber].Text = mlines[currentLineNumber].Text.Substring(0, currentCursorIndex);
                    }
                    else
                    {
                        newItem.Text = string.Empty;
                        newItem.LineState = LineModificationState.Modified;
                    }

                    if (this.CurrentLanguage.SupportsOutlining)
                    {
                        var expandInfo = mlines[currentLineNumber].GetLineItemExpandDetails();
                        if (expandInfo != null)
                        {
                            newItem.CopyExpandDetails(mlines[currentLineNumber]);
                            newItem.StartLine = expandInfo.StartLine;
                            newItem.EndLine = expandInfo.EndLine;
                            mlines[currentLineNumber].IsExpanded = true;
                            mlines[currentLineNumber].ContainsLines = false;
                        }
                    }
                }
                else
                {
                    int endIndex = ellipsisIndex;
                    if (currentItem.ContainsPreprocessor && listener != null)
                    {
                        endIndex = ellipsisIndex + listener.BlockStart.Length;
                    }
                    else
                    {
                        endIndex = ellipsisIndex + this.CurrentLanguage.EllipsisText.Length;
                    }

                    if (currentCursorIndex >= endIndex)
                    {
                        this.ExpandLine(currentLineNumber);
                        currentLineNumber = currentItem.EndLine - 1;
                        if (currentCursorIndex < collapsedText.Length)
                        {
                            int linendIndex = this.CurrentLanguage.GetCollapsedItemSelectionEndIndex(currentItem, currentItem.Text.Length);
                            newItem.Text = mlines[currentLineNumber].Text.Substring(linendIndex);
                            mlines[currentLineNumber].Text = mlines[currentLineNumber].Text.Substring(0, linendIndex);
                        }
                        else
                        {
                            newItem.Text = string.Empty;
                        }
                    }
                }

                this.CurrentLanguage.HideIntellisensePopup();
                newItem.SetCursorOnLoad = true;
                newItem.SetCursorIndex = 0;
                this.ScrollControl.CaretIndex = 0;
                AddUndoManager = false;
                UndoManager.IsNewUndoItem = true;
                UndoManager.Add(new EditAction()
                {
                    Action = ActionType.Enter,
                    LineNumber = currentLineNumber + 1,
                    CursorIndex = mlines[currentLineNumber].Text.Length,
                    NewLine = true,
                    IsSelected = isselected,
                    SelectedText = temptxt,
                    Pointer = temppointer,
                    IsSelectAll = selectall,
                    Text = mlines[currentLineNumber].Text
                });
                this.isAddingLinesCompleted = true;
                this.ScrollControl.rowheights.InsertLines(currentLineNumber + 1, 1, null);
                this.Lines.Insert(currentLineNumber + 1, newItem);
                this.ScrollControl.ScrollRows.ScrollInView(currentLineNumber + 1);
                this.ApplyIndentation(currentLineNumber + 2, true);
                this.CurrentLanguage.ApplyExpandItems();
            }
            this.CurrentLanguage.OnTextInput(currentLineNumber, e);
            inputtimer.Start();
            this.isReinitializeLines = false;
            this.Text = this.GetText();
            isReinitializeLines = true;
            AddUndoManager = true;
        }

        private bool ValidateShowIntellisenseBox(string lang, string text)
        {
            bool returnValue = true;
            if (lang == "CSharp" && (text.Contains('/')))
                returnValue = false;
            else if (lang == "Visual Basic" && (text.Contains('`')))
                returnValue = false;
            else if (lang == "XAML" && (text.Contains("&lt;!--")))
                returnValue = false;
            else if (lang == "SQL" && (text.Contains("--") || text.Contains("/*")))
                returnValue = false;
            else if (lang == "XML" && (text.Contains("///")))
                returnValue = false;
            else
                returnValue = true;
            return returnValue;
        }

        private void ApplyIndentation(int lineNumber, bool setCursor)
        {
            int spaceCount = 0;
            int indentlevel = this.CurrentLanguage.GetIndentLevel(lineNumber - 1);
            switch (this.IndentingOptions)
            {
                case IndentingOptions.None:
                default:
                    return;

                case IndentingOptions.Block:
                    if (indentlevel > 0)
                    {
                        spaceCount = (indentlevel - 1) * this.TabSpaces;
                    }
                    break;

                case IndentingOptions.Smart:
                    if (indentlevel > 0)
                    {
                        spaceCount = (indentlevel) * this.TabSpaces;
                    }
                    break;
            }

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < spaceCount; i++)
            {
                builder.Append(" ");
            }

            var lineItem = this.Lines[lineNumber - 1];
            lineItem.Text = lineItem.Text.TrimStart().Insert(0, builder.ToString());
            AddUndoManager = false;
            UndoManager.IsNewUndoItem = true;
            UndoManager.Add(new EditAction()
            {
                Action = ActionType.Type,
                Text = builder.ToString(),
                LineNumber = lineNumber,
                CursorIndex = 0,
                IsSelected = false,
                SelectedText = string.Empty,
                Pointer = null,
                IsSelectAll = false
            });

            if (setCursor)
            {
                lineItem.SetCursorOnLoad = true;
                lineItem.SetCursorIndex = spaceCount;
            }
            AddUndoManager = true;
        }

        private int inputTimerValue = 0;

        private void inputtimer_Tick(object sender, EventArgs e)
        {
            inputTimerValue += 1;
            if (inputTimerValue == 3)
            {
                this.CurrentLanguage.ApplyExpandItems();
                inputtimer.Stop();
                inputTimerValue = 0;
            }
        }

        /// <summary>
        /// override to perform operations when key is down
        /// In this override method key inputs are captured and tab inputs are applied
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            this.AddUndoManager = false;
            KeyboardDevice device = e.Device as KeyboardDevice;
            if (Keyboard.FocusedElement != this.viewer)
            {
                base.OnPreviewKeyDown(e);
                return;
            }

            if (device.Modifiers == ModifierKeys.Control)
            {
                if (this.isIntellisenseBoxOpen)
                {
                    this.intellisensePopup.Opacity = 0.4;
                    this.intellisenseBox.Opacity = 0.4;
                }
            }
            else
            {
                if (this.isIntellisenseBoxOpen)
                {
                    this.intellisensePopup.Opacity = 1;
                    this.intellisenseBox.Opacity = 1;
                }
            }

            if (e.Key == Key.Tab)
            {
                if (device.Modifiers != ModifierKeys.None && device.Modifiers != ModifierKeys.Shift)
                {
                    return;
                }
                if (this.AcceptsTab)
                {
                    this.ExecuteTab();
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Escape)
            {
                if (this.isIntellisenseBoxOpen)
                {
                    this.CurrentLanguage.HideIntellisensePopup();
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Space)
                IsSpaceKeyPressed = true;
            else if (e.Key == Key.Insert && device.Modifiers == ModifierKeys.None)
            {
                isInsertKeyToggled = !isInsertKeyToggled;
            }
            else
            {
                this.ScrollControl.KeyDownPreview(e);
            }
        }

        /// <summary>
        /// override to perform operations when key is up
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            if (this.isIntellisenseBoxOpen)
            {
                this.intellisensePopup.Opacity = 1;
                this.intellisenseBox.Opacity = 1;
            }
            this.ScrollControl.KeyUpPreview(e);
            this.AddUndoManager = true;
        }

        /// <summary>
        /// override to update the focus of the control when control gets Focussed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (this.viewer != null)
            {
                this.viewer.Focus();
            }
            base.OnGotFocus(e);
        }

        /// <summary>
        /// override to update the focus of the control when control loses Focussed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
        }

        #endregion Overrides

        #region Command Executes and CanExecutes

        /// <summary>
        /// Updates AutoIndent command's can execute property
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnAutoIndentCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            EditControl control = target as EditControl;
            args.CanExecute = control.IsAutoIndentationEnabled && control.CurrentLanguage.SupportsOutlining;
        }

        /// <summary>
        /// Occurs when AutoIndent command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnAutoIndentExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl control = target as EditControl;
            control.AutoIndentAllLines();
        }

        /// <summary>
        /// Occurs when Copy command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnCopyExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl editor = (EditControl)target;
            try
            {
                if (EnvironmentTest.IsSecurityGranted)
                {
                    Clipboard.SetText(editor.SelectedText);
                }
            }
            catch
            {
                editor.clipboardtext = editor.SelectedText;
            }
            editor.HideIntellisense();
            editor.clipboardtext = editor.SelectedText;
        }

        /// <summary>
        /// occurs when Cut command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnCutExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl editor = (EditControl)target;
            editor.HideIntellisense();
            int index = editor.ScrollControl.TextSelectionPointer.StartIndex;
            int startLine = editor.ScrollControl.TextSelectionPointer.StartLine;
            try
            {
                if (EnvironmentTest.IsSecurityGranted)
                {
                    Clipboard.SetText(editor.SelectedText);
                }
            }
            catch
            {
            }

            editor.clipboardtext = editor.SelectedText;
            editor.AddUndoManager = false;
            editor.UndoManager.IsNewUndoItem = true;
            editor.UndoManager.Add(new EditAction()
            {
                Action = ActionType.Cut,
                Pointer = editor.ScrollControl.TextSelectionPointer,
                LineNumber = editor.ScrollControl.TextSelectionPointer.StartLine + 1,
                CursorIndex = editor.ScrollControl.CaretIndex,
                Text = editor.clipboardtext,
                IsSelectAll = editor.ScrollControl.isSelectedAll,
                IsSelected = true,
                SelectedText = editor.SelectedText
            });
            editor.ScrollControl.RemoveSelectedText(editor.ScrollControl.TextSelectionPointer);
            editor.ScrollControl.ScrollRows.ScrollInView(startLine);
            editor.ScrollControl.MoveCursorToLineItem(startLine);
            if (editor.ScrollControl.Caret != null)
            {
                editor.ScrollControl.CaretIndex = editor.ScrollControl.Caret.MoveToLocation(index);
            }
            else
            {
                editor.Lines[startLine].SetCursorIndex = index;
                editor.Lines[startLine].SetCursorOnLoad = true;
            }
            editor.isReinitializeLines = false;
            editor.Text = editor.GetText();
            editor.CurrentLanguage.ApplyExpandItems();
            editor.isReinitializeLines = true;
            editor.AddUndoManager = true;
        }

        /// <summary>
        /// Updates Cut command's can execute property
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnCutCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            EditControl editor = (EditControl)target;
            if (editor.SelectedText != string.Empty && !editor.IsReadOnly)
            {
                args.CanExecute = true;
            }
            else
            {
                args.CanExecute = false;
            }
        }

        /// <summary>
        /// Updates CanExecute of Delete command
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnDeleteCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            EditControl editor = (EditControl)target;
            args.CanExecute = !editor.IsReadOnly;
        }

        /// <summary>
        /// Occurs when the delete command is executed.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnDeleteExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl control = (EditControl)target;
            control.CurrentLanguage.ExecuteDeleteText();
        }

        /// <summary>
        /// Updates CanExecute of the Backspace command
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnBackCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            EditControl editor = (EditControl)target;
            args.CanExecute = !editor.IsReadOnly;
        }

        /// <summary>
        /// Occurs when backspace button is pressed or backspace command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnBackExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl control = (EditControl)target;
            control.CurrentLanguage.ExecuteBackspace();
        }

        /// <summary>
        /// Ignore command is created to restrict other idle keyboard shortcuts
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnIgnoreCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = false;
        }

        /// <summary>
        /// Ignore command is created to restrict other idle keyboard shortcuts
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnIgnoreExecute(object target, ExecutedRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Occurs when the paste command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnPasteExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl editor = (EditControl)target;
            editor.HideIntellisense();
            editor.AddClipboardText(editor.LineNumber - 1, editor.CursorIndex); // editor.AddClipboardText(editor.LineNumber - 1, editor.CursorIndex);
            editor.AddUndoManager = false;
            editor.isReinitializeLines = false;
            editor.Text = editor.GetText();
            editor.CurrentLanguage.ApplyExpandItems();
            editor.isReinitializeLines = true;
            editor.AddUndoManager = true;
        }

        /// <summary>
        /// Updates can execute arguement of paste command
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnPasteCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            EditControl editor = (EditControl)target;
            if (editor.IsReadOnly)
            {
                args.CanExecute = false;
            }
            else
            {
                args.CanExecute = true;
            }
        }

        /// <summary>
        /// Updates canexecute argument of open command
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnOpenCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            if (EnvironmentTest.IsSecurityGranted)
            {
                args.CanExecute = true;
            }
            else
            {
                args.CanExecute = false;
            }
        }

        /// <summary>
        /// Updates canexecute argument of Save command
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnSaveCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            if (EnvironmentTest.IsSecurityGranted)
            {
                args.CanExecute = true;
            }
            else
            {
                args.CanExecute = false;
            }
        }

        /// <summary>
        /// Updates canexecute argument of new command.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnNewCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        /// <summary>
        /// Occurs when the open command is executed.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnOpenExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl editor = (EditControl)target;
            editor.HideIntellisense();
            editor.LoadFile();
            editor.ScrollControl.ScrollRows.ScrollInView(0);
            editor.ScrollControl.ScrollToLeftEnd();
            editor.ScrollControl.MoveCursorToLineItem(0);
            if (editor.ScrollControl.Caret != null)
            {
                editor.ScrollControl.Caret.MoveToBegin();
            }
            else
            {
                if (editor.Lines.Count > 0)
                {
                    editor.Lines[0].SetCursorOnLoad = true;
                    editor.Lines[0].SetCursorIndex = 0;
                }
            }
        }

        /// <summary>
        /// Occurs when save command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnSaveExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl editor = (EditControl)target;
            editor.HideIntellisense();
            if (EnvironmentTest.IsSecurityGranted)
            {
                if (editor.DocumentSource.Trim() == string.Empty)
                {
                    editor.SaveFile();
                }
                else
                {
                    editor.SaveFile(editor.DocumentSource);
                }
            }
        }

        /// <summary>
        /// Occurs when new command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnNewExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl editor = (EditControl)target;
            editor.AddUndoManager = false;
            editor.Lines.Clear();
            editor.ScrollControl.ClearSelection();
            editor.HideIntellisense();
            editor.ScrollControl.ResetItems();
            editor.isReinitializeLines = true;
            LineItem item = new LineItem(string.Empty);
            item.SetCursorIndex = 0;
            item.SetCursorOnLoad = true;
            editor.Lines.Add(item);
            editor.isReinitializeLines = false;
            editor.Text = editor.GetText();
            editor.AddUndoManager = true;
            editor.isReinitializeLines = true;
            editor.UndoManager.ClearActions();
        }

        /// <summary>
        /// Updates the CanExecute of the SelectAll command
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectAllCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            EditControl editor = (EditControl)target;
            args.CanExecute = !editor.ScrollControl.isSelectedAll;
        }

        /// <summary>
        /// Occurs when SelectAll command is executed.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectAllExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl editor = (EditControl)target;
            editor.HideIntellisense();
            if (editor.EnableIntellisense && editor.isIntellisenseBoxOpen)
            {
                editor.CurrentLanguage.HideIntellisensePopup();
            }
            editor.ScrollControl.ExecuteSelectAll();
        }

        /// <summary>
        /// Updates the CanExecute of the Undo command
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnUndoCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            EditControl control = target as EditControl;
            args.CanExecute = control.IsUndoEnabled && control.UndoManager.ActionCount() > 0;
        }

        /// <summary>
        /// Occurs when Undo command is executed.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnUndoExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl control = target as EditControl;
            control.HideIntellisense();
            control.AddUndoManager = false;
            control.UndoManager.UndoAction();
            control.isReinitializeLines = false;
            control.Text = control.GetText();
            control.CurrentLanguage.ApplyExpandItems();
            control.isReinitializeLines = true;
            control.AddUndoManager = true;
            control.ScrollControl.InvalidateVisual(true);
        }

        /// <summary>
        /// Updates CanExecute of Redo command
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnRedoCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            EditControl control = target as EditControl;
            args.CanExecute = control.IsRedoEnabled && control.UndoManager.RedoCount() > 0;
        }

        /// <summary>
        /// Occurs when Redo command is executed.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnRedoExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl control = target as EditControl;
            control.AddUndoManager = false;
            control.UndoManager.RedoAction();
            control.isReinitializeLines = false;
            control.Text = control.GetText();
            control.CurrentLanguage.ApplyExpandItems();
            control.isReinitializeLines = true;
            control.AddUndoManager = true;
            control.ScrollControl.InvalidateVisual(true);
        }

        /// <summary>
        /// Occurs when ExpandAll command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnExpandAllExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl control = (EditControl)target;
            control.HideIntellisense();
            if (control.ScrollControl != null)
            {
                control.ScrollControl.ExpandAllItems();
                control.ScrollControl.MoveCursorToLineItem(0);
            }
        }

        /// <summary>
        /// Updates CanExecute for ExpandAll command
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnExpandAllCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = ((EditControl)target).GetExpandAllCanExecute() && ((EditControl)target).EnableOutlining;
        }

        /// <summary>
        /// Updates CanExecute for CollapseAll command
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnCollapseAllCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            var p = from line in instance.Lines
                    where line.ContainsLines && line.IsExpanded
                    select line;
            instance.HideIntellisense();
            if (p.Count<LineItem>() > 0 && instance.CurrentLanguage.SupportsOutlining && instance.EnableOutlining)
            {
                args.CanExecute = true;
            }
            else
            {
                args.CanExecute = false;
            }
        }

        /// <summary>
        /// Occurs when CollapseAll command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnCollapseAllExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl control = (EditControl)target;
            control.HideIntellisense();
            var items = from line in control.Lines
                        where line.ContainsLines
                        select line;

            foreach (LineItem item in items)
            {
                control.ScrollControl.UpdateExpandStatus(control.Lines.IndexOf(item) + 1, item.EndLine - 1, true);
                item.IsExpanded = false;
            }
            control.ScrollControl.InvalidateVisual(true);
            //collapseallflag = true;
        }

        /// <summary>
        /// Updates CanExecute for ParentControl command
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnFindAndReplaceCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            args.CanExecute = (instance.Text != String.Empty) && instance.ShowFindAndReplace;
        }

        /// <summary>
        /// Updates CanExecute for Replace command
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnReplaceCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            args.CanExecute = (instance.Text != String.Empty) && instance.ShowFindAndReplace && !instance.IsReadOnly;
        }

        /// <summary>
        /// Occurs when Find command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnFindExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            instance.HideIntellisense();
            instance.OpenFindWindow(Tabs.FindTab);
        }

        /// <summary>
        /// Occurs when Replace command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnReplaceExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            instance.HideIntellisense();
            instance.OpenFindWindow(Tabs.ReplaceTab);
        }

        /// <summary>
        /// Occurs when Search command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnSearchExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            instance.HideIntellisense();
            if (instance.FindOptions != null)
            {
                if (instance.FindOptions.IsSelectionSelected == true)
                {
                    instance.SearchResults.IsDirty = true;
                    instance.FindOptions.IsSelectionSelected = false;
                }
                instance.ExecuteFindNext();
            }
        }

        /// <summary>
        /// Updates CanExecute for Searchinselected command
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnSearchInSelectedCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            args.CanExecute = instance.SelectedText != string.Empty && instance.ScrollControl.TextSelectionPointer != null;
        }

        /// <summary>
        /// Occurs when Search in Selected command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnSearchInSelectedExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            instance.HideIntellisense();
            if (instance.FindOptions != null)
            {
                if (!instance.FindOptions.IsSelectionSelected)
                {
                    instance.FindOptions.IsSelectionSelected = true;
                }

                if (instance.SearchResults.EditTextSelection == null)
                {
                    instance.SearchResults.EditTextSelection = instance.ScrollControl.TextSelectionPointer;
                }

                instance.ExecuteFindNext();
            }
        }

        /// <summary>
        /// Occurs when FindAllReferences command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnFindAllReferencesExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            instance.HideIntellisense();
            if (instance.FindOptions.FindText != string.Empty)
            {
                instance.ExecuteFindAll();
            }
        }

        /// <summary>
        ///  Can execute event handler for Show intellisense
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnShowIntellisenseCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            if (instance.IntellisenseMode == IntellisenseMode.Custom)
            {
                args.CanExecute = instance.IntellisenseCustomItemsSource != null;
            }
            else
            {
                args.CanExecute = instance.CurrentLanguage != null && instance.CurrentLanguage.SupportsIntellisense && instance.EnableIntellisense;
            }
        }

        /// <summary>
        /// Occurs when ShowIntellisense command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnShowIntellisenseExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            EditIntellisenseArgs intelliArgs = new EditIntellisenseArgs()
            {
                Assemblies = instance.AssemblyReferences != null ? new List<Uri>(instance.AssemblyReferences) : null,
                LineIndex = instance.LineNumber - 1,
                CursorIndex = instance.CursorIndex
            };
            instance.CurrentLanguage.ShowIntellisenseBox(intelliArgs);
            instance.isIntellisenseBoxOpen = true;
        }

        /// <summary>
        /// Occurs when InsertNewLine command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnInsertNewLineExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            instance.ExecuteInsertNewLine();
        }

        /// <summary>
        /// Can Execute event handler for InsertNewLine command
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnInsertNewLineCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            args.CanExecute = true;
        }

        /// <summary>
        /// Occurs when InsertNewLine command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectCurrentWordExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            instance.ExecuteSelectCurrentWord();
        }

        private void ExecuteSelectCurrentWord()
        {
            WordDetails item = this.ScrollControl.GetCurrentWord(this.CursorIndex);
            if (item != null)
            {
                if (item != null && item.Text == " ")
                {
                    item = this.ScrollControl.GetCurrentWord(this.CursorIndex + 1);
                }

                if (item != null)
                {
                    this.ScrollControl.CaretIndex = this.ScrollControl.Caret.MoveToLocation(item.StartIndex + item.Text.Length);
                    this.ScrollControl.isSelectionstarted = true;
                    this.ScrollControl.UpdateSelectionPointer(this.LineNumber - 1, this.LineNumber - 1, item.StartIndex, item.StartIndex + item.Text.Length);
                    this.ScrollControl.ParentEditControl.SelectedText = item.Text;
                }
            }
        }

        /// <summary>
        /// Can Execute event handler for InsertNewLine command
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectCurrentWordCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            args.CanExecute = true;
        }

        /// <summary>
        /// Can Execute event handler for increase indent command
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnIncreaseIndentCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            args.CanExecute = true;
        }

        /// <summary>
        /// Occurs when increase indent command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnIncreaseIndentExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            instance.HideIntellisense();
            instance.AddUndoManager = false;
            if (instance.ScrollControl.TextSelectionPointer != null && instance.SelectedText != string.Empty)
            {
                var pointer = instance.ScrollControl.TextSelectionPointer;
                instance.IncreaseIndent(instance.ScrollControl.TextSelectionPointer, instance.TabSpaces);
                instance.UndoManager.IsNewUndoItem = true;
                instance.UndoManager.Add(new EditAction()
                {
                    Action = ActionType.IncreaseIndent,
                    Pointer = pointer,
                    Text = instance.SelectedText,
                    IsSelected = true,
                    LineNumber = instance.ScrollControl.CurrentLineItem.LineNumber
                });
            }
            else
            {
                instance.IncreaseIndent(instance.ScrollControl.CurrentLineItem, instance.TabSpaces);
                instance.UndoManager.IsNewUndoItem = true;
                instance.UndoManager.Add(new EditAction()
                {
                    Action = ActionType.IncreaseIndent,
                    LineNumber = instance.ScrollControl.CurrentLineItem.LineNumber,
                    Text = instance.ScrollControl.CurrentLineItem.Text,
                    IsSelected = false
                });
            }
            instance.AddUndoManager = true;
        }

        /// <summary>
        /// Can Execute event handler for decrease indent command
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnDecreaseIndentCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            args.CanExecute = true;
        }

        /// <summary>
        /// Occurs when decrease indent command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnDecreaseIndentExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            instance.HideIntellisense();
            instance.AddUndoManager = false;
            if (instance.ScrollControl.TextSelectionPointer != null && instance.SelectedText != string.Empty)
            {
                var pointer = instance.ScrollControl.TextSelectionPointer;
                instance.DecreaseIndent(instance.ScrollControl.TextSelectionPointer, instance.TabSpaces);
                instance.UndoManager.IsNewUndoItem = true;
                instance.UndoManager.Add(new EditAction()
                {
                    Action = ActionType.DecreaseIndent,
                    Pointer = pointer,
                    Text = instance.SelectedText,
                    IsSelected = true,
                    LineNumber = instance.ScrollControl.CurrentLineItem.LineNumber
                });
            }
            else
            {
                instance.DecreaseIndent(instance.ScrollControl.CurrentLineItem, instance.TabSpaces);
                instance.UndoManager.IsNewUndoItem = true;
                instance.UndoManager.Add(new EditAction()
                {
                    Action = ActionType.DecreaseIndent,
                    LineNumber = instance.ScrollControl.CurrentLineItem.LineNumber,
                    Text = instance.ScrollControl.CurrentLineItem.Text,
                    IsSelected = false
                });
            }
            instance.AddUndoManager = true;
        }

        /// <summary>
        /// Can Execute event handler for comment selection command
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnCommentSelectionCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            args.CanExecute = true;
        }

        /// <summary>
        /// Occurs when comment selection command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnCommentSelectionExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            instance.HideIntellisense();
            if (instance.ScrollControl.TextSelectionPointer != null && instance.SelectedText != string.Empty)
            {
                instance.CurrentLanguage.OnCommentCommandExecute(instance.ScrollControl.TextSelectionPointer);
            }
            else if (instance.ScrollControl.CurrentLineItem != null)
            {
                instance.CurrentLanguage.OnCommentCommandExecute(instance.ScrollControl.CurrentLineItem);
            }
            instance.isReinitializeLines = false;
            instance.Text = instance.GetText();
            instance.CurrentLanguage.ApplyExpandItems();
            instance.isReinitializeLines = true;
            instance.ScrollControl.InvalidateVisual(true);
        }

        /// <summary>
        /// Can Execute event handler for comment selection command
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnUncommentSelectionCanExecute(object target, CanExecuteRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            args.CanExecute = true;
            if (instance.ScrollControl != null)
                instance.ScrollControl.InvalidateVisual(true);
        }

        /// <summary>
        /// Occurs when comment selection command is executed
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnUncommentSelectionExecute(object target, ExecutedRoutedEventArgs args)
        {
            EditControl instance = (EditControl)target;
            instance.HideIntellisense();
            if (instance.ScrollControl.TextSelectionPointer != null && instance.SelectedText != string.Empty)
            {
                instance.CurrentLanguage.OnUncommentCommandExecute(instance.ScrollControl.TextSelectionPointer);
            }
            else if (instance.ScrollControl.CurrentLineItem != null)
            {
                instance.CurrentLanguage.OnUncommentCommandExecute(instance.ScrollControl.CurrentLineItem);
            }
            instance.isReinitializeLines = false;
            instance.Text = instance.GetText();
            instance.CurrentLanguage.ApplyExpandItems();
            instance.isReinitializeLines = true;
            instance.ScrollControl.InvalidateVisual(true);
        }

        /// <summary>
        /// Helper method to initialize commands shortcut keys
        /// </summary>
        private void InitializeCommandBindings()
        {
            EditCommands.InitializeCommandKeyGestures();

            CommandBinding cutcommand = new CommandBinding(EditCommands.Cut, new ExecutedRoutedEventHandler(OnCutExecute), new CanExecuteRoutedEventHandler(OnCutCanExecute));
            CommandBinding copycommand = new CommandBinding(EditCommands.Copy, new ExecutedRoutedEventHandler(OnCopyExecute), new CanExecuteRoutedEventHandler(OnCutCanExecute));
            CommandBinding pastecommand = new CommandBinding(EditCommands.Paste, new ExecutedRoutedEventHandler(OnPasteExecute), new CanExecuteRoutedEventHandler(OnPasteCanExecute));
            CommandBinding opencommand = new CommandBinding(EditCommands.Open, new ExecutedRoutedEventHandler(OnOpenExecute), new CanExecuteRoutedEventHandler(OnOpenCanExecute));
            CommandBinding savecommand = new CommandBinding(EditCommands.Save, new ExecutedRoutedEventHandler(OnSaveExecute), new CanExecuteRoutedEventHandler(OnSaveCanExecute));
            CommandBinding newcommand = new CommandBinding(EditCommands.New, new ExecutedRoutedEventHandler(OnNewExecute), new CanExecuteRoutedEventHandler(OnNewCanExecute));
            CommandBinding selectallcommand = new CommandBinding(EditCommands.SelectAll, new ExecutedRoutedEventHandler(OnSelectAllExecute), new CanExecuteRoutedEventHandler(OnSelectAllCanExecute));
            CommandBinding undocommand = new CommandBinding(EditCommands.Undo, new ExecutedRoutedEventHandler(OnUndoExecute), new CanExecuteRoutedEventHandler(OnUndoCanExecute));
            CommandBinding redocommand = new CommandBinding(EditCommands.Redo, new ExecutedRoutedEventHandler(OnRedoExecute), new CanExecuteRoutedEventHandler(OnRedoCanExecute));
            CommandBinding delcommand = new CommandBinding(EditCommands.Delete, new ExecutedRoutedEventHandler(OnDeleteExecute), new CanExecuteRoutedEventHandler(OnDeleteCanExecute));
            CommandBinding backcommand = new CommandBinding(EditCommands.Backspace, new ExecutedRoutedEventHandler(OnBackExecute), new CanExecuteRoutedEventHandler(OnBackCanExecute));
            CommandBinding ignorecommand = new CommandBinding(EditCommands.IgnoreKeys, new ExecutedRoutedEventHandler(OnIgnoreExecute), new CanExecuteRoutedEventHandler(OnIgnoreCanExecute));
            CommandBinding expandcommand = new CommandBinding(EditCommands.ExpandAll, new ExecutedRoutedEventHandler(OnExpandAllExecute), new CanExecuteRoutedEventHandler(OnExpandAllCanExecute));
            CommandBinding collapsecommand = new CommandBinding(EditCommands.CollapseAll, new ExecutedRoutedEventHandler(OnCollapseAllExecute), new CanExecuteRoutedEventHandler(OnCollapseAllCanExecute));
            CommandBinding findcommand = new CommandBinding(EditCommands.Find, new ExecutedRoutedEventHandler(OnFindExecute), new CanExecuteRoutedEventHandler(OnFindAndReplaceCanExecute));
            CommandBinding replacecommand = new CommandBinding(EditCommands.Replace, new ExecutedRoutedEventHandler(OnReplaceExecute), new CanExecuteRoutedEventHandler(OnReplaceCanExecute));
            CommandBinding searchcommand = new CommandBinding(EditCommands.Search, new ExecutedRoutedEventHandler(OnSearchExecute), new CanExecuteRoutedEventHandler(OnFindAndReplaceCanExecute));
            CommandBinding searchinselectedcommand = new CommandBinding(EditCommands.SearchInSelected, new ExecutedRoutedEventHandler(OnSearchInSelectedExecute), new CanExecuteRoutedEventHandler(OnSearchInSelectedCanExecute));
            CommandBinding findallreferences = new CommandBinding(EditCommands.FindAllReferences, new ExecutedRoutedEventHandler(OnFindAllReferencesExecute), new CanExecuteRoutedEventHandler(OnFindAndReplaceCanExecute));
            CommandBinding showIntellisense = new CommandBinding(EditCommands.ShowIntellisense, new ExecutedRoutedEventHandler(OnShowIntellisenseExecute), new CanExecuteRoutedEventHandler(OnShowIntellisenseCanExecute));
            CommandBinding increaseIndent = new CommandBinding(EditCommands.IncreaseIndent, new ExecutedRoutedEventHandler(OnIncreaseIndentExecute), new CanExecuteRoutedEventHandler(OnIncreaseIndentCanExecute));
            CommandBinding decreaseIndent = new CommandBinding(EditCommands.DecreaseIndent, new ExecutedRoutedEventHandler(OnDecreaseIndentExecute), new CanExecuteRoutedEventHandler(OnDecreaseIndentCanExecute));
            CommandBinding commentSelection = new CommandBinding(EditCommands.CommentSelection, new ExecutedRoutedEventHandler(OnCommentSelectionExecute), new CanExecuteRoutedEventHandler(OnCommentSelectionCanExecute));
            CommandBinding uncommentSelection = new CommandBinding(EditCommands.UncommentSelection, new ExecutedRoutedEventHandler(OnUncommentSelectionExecute), new CanExecuteRoutedEventHandler(OnUncommentSelectionCanExecute));
            CommandBinding insertNewLine = new CommandBinding(EditCommands.InsertNewLine, new ExecutedRoutedEventHandler(OnInsertNewLineExecute), new CanExecuteRoutedEventHandler(OnInsertNewLineCanExecute));
            CommandBinding selectCurrentWord = new CommandBinding(EditCommands.SelectCurrentWord, new ExecutedRoutedEventHandler(OnSelectCurrentWordExecute), new CanExecuteRoutedEventHandler(OnSelectCurrentWordCanExecute));
            CommandBinding autoIndent = new CommandBinding(EditCommands.AutoIndent, new ExecutedRoutedEventHandler(OnAutoIndentExecute), new CanExecuteRoutedEventHandler(OnAutoIndentCanExecute));

            CommandManager.RegisterClassCommandBinding(typeof(EditControl), cutcommand);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), copycommand);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), pastecommand);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), opencommand);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), savecommand);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), newcommand);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), selectallcommand);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), undocommand);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), redocommand);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), delcommand);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), backcommand);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), ignorecommand);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), expandcommand);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), collapsecommand);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), findcommand);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), replacecommand);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), searchcommand);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), searchinselectedcommand);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), findallreferences);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), showIntellisense);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), increaseIndent);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), decreaseIndent);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), commentSelection);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), uncommentSelection);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), insertNewLine);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), selectCurrentWord);
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), autoIndent);

            CommandManager.RegisterClassCommandBinding(typeof(EditControl), new CommandBinding(ApplicationCommands.Cut, new ExecutedRoutedEventHandler(OnCutExecute), new CanExecuteRoutedEventHandler(OnCutCanExecute)));
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), new CommandBinding(ApplicationCommands.Copy, new ExecutedRoutedEventHandler(OnCopyExecute), new CanExecuteRoutedEventHandler(OnCutCanExecute)));
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), new CommandBinding(ApplicationCommands.Paste, new ExecutedRoutedEventHandler(OnPasteExecute), new CanExecuteRoutedEventHandler(OnPasteCanExecute)));
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), new CommandBinding(ApplicationCommands.Open, new ExecutedRoutedEventHandler(OnOpenExecute), new CanExecuteRoutedEventHandler(OnOpenCanExecute)));
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), new CommandBinding(ApplicationCommands.Save, new ExecutedRoutedEventHandler(OnSaveExecute), new CanExecuteRoutedEventHandler(OnSaveCanExecute)));
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), new CommandBinding(ApplicationCommands.New, new ExecutedRoutedEventHandler(OnNewExecute), new CanExecuteRoutedEventHandler(OnNewCanExecute)));
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), new CommandBinding(ApplicationCommands.SelectAll, new ExecutedRoutedEventHandler(OnSelectAllExecute), new CanExecuteRoutedEventHandler(OnSelectAllCanExecute)));
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), new CommandBinding(ApplicationCommands.Undo, new ExecutedRoutedEventHandler(OnUndoExecute), new CanExecuteRoutedEventHandler(OnUndoCanExecute)));
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), new CommandBinding(ApplicationCommands.Redo, new ExecutedRoutedEventHandler(OnRedoExecute), new CanExecuteRoutedEventHandler(OnRedoCanExecute)));
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), new CommandBinding(ApplicationCommands.Delete, new ExecutedRoutedEventHandler(OnDeleteExecute), new CanExecuteRoutedEventHandler(OnDeleteCanExecute)));
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), new CommandBinding(ApplicationCommands.Find, new ExecutedRoutedEventHandler(OnFindExecute), new CanExecuteRoutedEventHandler(OnFindAndReplaceCanExecute)));
            CommandManager.RegisterClassCommandBinding(typeof(EditControl), new CommandBinding(ApplicationCommands.Replace, new ExecutedRoutedEventHandler(OnReplaceExecute), new CanExecuteRoutedEventHandler(OnReplaceCanExecute)));
        }

        private void HideIntellisense()
        {
            if (this.EnableIntellisense && this.isIntellisenseBoxOpen)
            {
                this.CurrentLanguage.HideIntellisensePopup();
            }
        }

        #endregion Command Executes and CanExecutes

        #region Find and Replace Codes

        internal void SetActiveTab(Tabs activetab)
        {
            switch (activetab)
            {
                case Tabs.FindTab:
                    {
                        FindOptions.IsFindTabActive = true;
                        FindOptions.IsReplaceTabActive = false;
                        FindOptions.IsFindSymbolTabActive = false;
                        break;
                    }

                case Tabs.ReplaceTab:
                    {
                        FindOptions.IsReplaceTabActive = true;
                        FindOptions.IsFindSymbolTabActive = false;
                        FindOptions.IsFindTabActive = false;
                        break;
                    }

                case Tabs.FindSymbolTab:
                    {
                        FindOptions.IsReplaceTabActive = false;
                        FindOptions.IsFindSymbolTabActive = true;
                        FindOptions.IsFindTabActive = false;
                        break;
                    }
            }
        }

        /// <summary>
        /// Finds all  the occurrences of the FindText specified in FindOptions and displays the list
        /// of occurences in the Find Symbol Results tab in EditControl.
        /// </summary>
        /// <remarks>
        /// FindOptions property of the EditControl can be used to set the Find related
        /// parameters
        /// </remarks>
        public void ExecuteFindAll()
        {
            this.FindAllOccurences();
            ListBox list = this.GetTemplateChild("PART_FindAllReferencesList") as ListBox;
            if (list != null)
            {
                list.ItemsSource = this.SearchResults.FindAllResult;
                list.MouseDoubleClick += new MouseButtonEventHandler(list_MouseDoubleClick);
                list.Focusable = true;
                list.SelectionChanged += new SelectionChangedEventHandler(list_SelectionChanged);
                this.IsFindResultsTabClosed = false;
                if (this.SearchResults.FindAllResult != null && this.SearchResults.FindAllResult.Count() > 0)
                {
                    this.FindOptions.StatusMessage = string.Format(@"{0} matches found", this.SearchResults.FindAllResult.Count());
                }
                else
                {
                    this.FindOptions.StatusMessage = @"Search found no results";
                }
            }
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox box = sender as ListBox;
            if (box != null)
            {
                FocusManager.SetFocusedElement(this, box);
                Keyboard.Focus(box);
            }
            e.Handled = true;
        }

        private void list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox list = sender as ListBox;
            if (list.SelectedItem != null)
            {
                FindResult result = list.SelectedItem as FindResult;
                this.ScrollControl.ScrollRows.ScrollInView(result.LineNumber - 1);
                this.ScrollControl.MoveCursorToLineItem(result.LineNumber - 1);
                if (this.ScrollControl.Caret != null)
                {
                    this.ScrollControl.CaretIndex = this.ScrollControl.Caret.MoveToBegin();
                }
                else
                {
                    this.Lines[result.LineNumber - 1].SetCursorOnLoad = true;
                    this.Lines[result.LineNumber - 1].SetCursorIndex = 0;
                }
            }
        }

        /// <summary>
        /// Replaces all  the occurrences of the FindText specified in FindOptions with ReplaceText
        /// </summary>
        /// <remarks>
        /// FindOptions property of the EditControl can be used to set the Find related
        /// parameters
        /// </remarks>
        public void ExecuteReplaceAll()
        {
            ReInitializeSearchResults();
            RegexOptions options = this.FindOptions.IsMatchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
            if (this.SearchResults.MatchingLineItem.Count() > 0)
            {
                EditAction action = new EditAction();
                action.Action = ActionType.ReplaceAll;
                LineItem lineitem = this.SearchResults.MatchingLineItem.ElementAt(0);
                Dictionary<LineItem, MatchCollection> replacedItems = new Dictionary<LineItem, MatchCollection>();
                int count = 0;
                int selectionend = 0;
                if (this.FindOptions.IsSelectionSelected)
                {
                    selectionend = this.SearchResults.EditTextSelection.EndIndex;
                }

                foreach (LineItem item in this.SearchResults.MatchingLineItem)
                {
                    MatchCollection matches = Regex.Matches(item.Text, this.SearchResults.CaptureText, options);
                    foreach (Match match in matches)
                    {
                        string tempTxt = item.Text.Remove(match.Index, this.FindOptions.FindText.Length);
                        tempTxt = tempTxt.Insert(match.Index, this.FindOptions.ReplaceText);
                        item.Text = tempTxt;
                        if (this.FindOptions.IsSelectionSelected)
                        {
                            int lineindex = this.Lines.IndexOf(item);
                            if (lineindex == this.SearchResults.EditTextSelection.EndLine)
                            {
                                selectionend = selectionend - this.FindOptions.FindText.Length + this.FindOptions.ReplaceText.Length;
                            }
                        }
                        count += 1;
                    }
                    replacedItems.Add(item, matches);
                }

                action.LineNumber = lineitem.LineNumber;
                action.CursorIndex = this.GetFirstMatchingIndex(lineitem.Text, this.SearchResults.CaptureText, options);
                action.ReplacedItems = replacedItems;
                action.Text = this.SearchResults.CaptureText;
                action.SelectedText = this.FindOptions.ReplaceText;
                action.IsSelected = this.ScrollControl.TextSelectionPointer != null;
                action.Pointer = this.ScrollControl.TextSelectionPointer;
                this.UndoManager.Add(action);
                this.isReinitializeLines = false;
                this.Text = this.GetText();
                this.CurrentLanguage.ApplyExpandItems();
                isReinitializeLines = true;
                if (this.FindOptions.IsSelectionSelected)
                {
                    SelectionPointer pointer = this.ScrollControl.TextSelectionPointer;
                    this.ScrollControl.ClearSelection();
                    this.ScrollControl.UpdateSelectionPointer(pointer.StartLine, pointer.EndLine, pointer.StartIndex, selectionend);
                    this.ScrollControl.MoveCursorToLineItem(pointer.EndLine);
                    if (this.ScrollControl.Caret != null)
                    {
                        this.ScrollControl.CaretIndex = this.ScrollControl.Caret.MoveToLocation(selectionend);
                    }
                }
                this.FindOptions.StatusMessage = string.Format("{0} occureances of the the text specified have been replaced", count);
                SearchResults.IsDirty = true;
            }
            else
            {
                this.FindOptions.StatusMessage = "There are no more occurances of the text specified";
            }
        }

        /// <summary>
        /// Replaces the immediate occurrence of FindText specified in FindOptions with ReplaceText
        /// </summary>
        /// <remarks>
        /// FindOptions property of the EditControl can be used to set the Find related
        /// parameters
        /// </remarks>
        public void ExecuteReplace()
        {
            int replaceindex = 0;
            if (this.SearchResults.IsDirty || this.SearchResults == null)
            {
                this.ReInitializeSearchResults();
            }

            if (!this.SearchResults.IsTextFound && this.SearchResults.MatchingLineItem.Count() > 0)
            {
                this.ExecuteFindNext();
            }
            if (this.FindOptions.IsSelectionSelected)
            {
                replaceindex = this.SearchResults.EditTextSelection.StartIndex;
            }
            else
            {
                replaceindex = this.CursorIndex;
            }
            if (this.SearchResults.IsTextFound)
            {
                LineItem currentLine = this.SearchResults.CurrentLineItem;
                RegexOptions options = this.FindOptions.IsMatchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
                string tempTxt = currentLine.Text.Remove(replaceindex, this.FindOptions.FindText.Length);
                this.SearchResults.CursorIndex = replaceindex;
                tempTxt = tempTxt.Insert(replaceindex, this.FindOptions.ReplaceText);
                currentLine.Text = tempTxt;
                AddUndoManager = false;
                UndoManager.IsNewUndoItem = true;
                UndoManager.Add(new EditAction()
                {
                    Action = ActionType.Replace,
                    Text = SearchResults.CaptureText,
                    LineNumber = this.Lines.IndexOf(currentLine) + 1,
                    SelectedText = this.FindOptions.ReplaceText,
                    CursorIndex = this.SearchResults.CursorIndex,
                    IsSelected = false,
                });
                this.Lines[currentLine.LineNumber - 1] = currentLine;
                this.CurrentLanguage.ApplyExpandItems();
                this.isReinitializeLines = false;
                this.Text = this.GetText();
                isReinitializeLines = true;
                AddUndoManager = false;
                if (this.SearchResults.CurrentIndex == this.SearchResults.MatchingLineItem.Count() - 1)
                {
                    this.SearchResults.IsDirty = true;
                    FindOptions.StatusMessage = "Find reached the starting point of the text specified";
                    this.ScrollControl.ClearSelection();
                }
                else
                {
                    ExecuteFindNext();
                }
            }
            else
            {
                FindOptions.StatusMessage = "There are no more occurrences of the text specified";
            }
        }

        /// <summary>
        /// Finds the next occurrence of FindText specified in FindOptions
        /// </summary>
        /// <remarks>
        /// FindOptions property of the EditControl can be used to set the Find related
        /// parameters
        /// </remarks>
        public void ExecuteFindNext()
        {
            if (SearchResults.IsDirty)
            {
                this.ReInitializeSearchResults();
            }

            if (SearchResults.MatchingLineItem != null && SearchResults.MatchingLineItem.Count() > 0)
            {
                int index = SearchResults.CurrentIndex;
                int cursorIndex = SearchResults.CursorIndex;
                int itemCount = SearchResults.MatchingLineItem.Count();
                RegexOptions options = FindOptions.IsMatchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
                bool foundnextMatch = false;
                while (!foundnextMatch)
                {
                    if (index == itemCount)
                    {
                        var visibleItems = SearchResults.MatchingLineItem.Where(lineItem => this.ScrollControl.ScrollRows.IsLineVisible(this.Lines.IndexOf(lineItem)));
                        if (visibleItems.Count() > 0)
                        {
                            index = 0;
                            FindOptions.StatusMessage = "Find reached the starting point of the text specified";
                        }
                        else
                        {
                            FindOptions.StatusMessage = "There are no more occurrences of the text specified";
                            return;
                        }
                    }

                    LineItem item = SearchResults.MatchingLineItem.ElementAt(index);
                    int itemIndex = this.Lines.IndexOf(item);
                    int tempindex = 0;
                    var lineVisible = !this.ScrollControl.rowheights.GetHidden(itemIndex, out tempindex);

                    if ((!lineVisible || (item.ContainsLines && !item.IsExpanded)) && this.FindOptions.IsIncludeHiddenText)
                    {
                        ExpandLineUpTopLevel(itemIndex);
                        lineVisible = true;
                    }
                    else if (lineVisible && item.ContainsLines && !item.IsExpanded)
                    {
                        lineVisible = false;
                    }

                    if (lineVisible)
                    {
                        if (!FindOptions.FindHistory.Contains(FindOptions.FindText))
                        {
                            FindOptions.FindHistory.Push(FindOptions.FindText);
                        }
                        MatchCollection matches = Regex.Matches(item.Text, SearchResults.CaptureText, options);
                        foreach (Match match in matches)
                        {
                            if (match.Index > cursorIndex)
                            {
                                bool selectionCheck = false;
                                if (FindOptions.IsSelectionSelected)
                                {
                                    int lineindex = this.Lines.IndexOf(item);
                                    if ((this.SearchResults.EditTextSelection.StartLine < lineindex && this.SearchResults.EditTextSelection.EndLine > lineindex) || (this.SearchResults.EditTextSelection.StartLine == lineindex && match.Index >= this.SearchResults.EditTextSelection.StartIndex) || (this.SearchResults.EditTextSelection.EndLine == lineindex && match.Index < this.SearchResults.EditTextSelection.EndIndex))
                                    {
                                        selectionCheck = true;
                                    }
                                    else
                                    {
                                        selectionCheck = false;
                                    }
                                }
                                else
                                {
                                    selectionCheck = true;
                                }

                                if (selectionCheck)
                                {
                                    SearchResults.CurrentSelection = new SelectionPointer() { StartLine = item.LineNumber, StartIndex = match.Index, EndIndex = match.Index + match.Value.Length, EndLine = item.LineNumber };
                                    SearchResults.CurrentLineItem = item;
                                    SearchResults.CursorIndex = match.Index;
                                    SearchResults.CurrentIndex = index;
                                    SearchResults.IsTextFound = true;
                                    this.FindOptions.StatusMessage = "";
                                    var mainItemIndex = this.Lines.IndexOf(item);
                                    this.ScrollControl.ScrollRows.ScrollInView(mainItemIndex);
                                    this.ScrollControl.MoveCursorToLineItem(mainItemIndex);
                                    if (this.ScrollControl.Caret != null)
                                    {
                                        this.ScrollControl.CaretIndex = this.ScrollControl.Caret.MoveToLocation(SearchResults.CursorIndex);
                                    }
                                    else
                                    {
                                        item.SetCursorIndex = SearchResults.CursorIndex;
                                        item.SetCursorOnLoad = true;
                                    }
                                    this.SearchResults.IsSelectionChanged = true;
                                    this.ScrollControl.isSelectionstarted = true;
                                    this.ScrollControl.UpdateSelectionPointer(mainItemIndex, mainItemIndex, SearchResults.CursorIndex, SearchResults.CursorIndex + match.Value.Length);
                                    foundnextMatch = true;
                                    SearchResults.IsDirty = false;
                                    break;
                                }
                            }
                        }
                    }
                    index += 1;
                    cursorIndex = -1;
                }
            }
            else
            {
                this.FindOptions.StatusMessage = "The specified text cannot be found";
            }
        }

        /// <summary>
        /// Finds  all the occurrences of FindText specified in FindOptions. Results can be obtained using SearchResults.FindAllResult property of EditControl class
        /// </summary>
        /// <remarks>
        /// FindOptions property of the EditControl can be used to set the Find related
        /// parameters
        /// </remarks>
        public void FindAllOccurences()
        {
            List<LineItem> linesCollection = new List<LineItem>(this.Lines);
            int currentLineNumber = this.ScrollControl.CurrentLineItem != null ? this.ScrollControl.CurrentLineItem.LineNumber : 1;

            string captureRegex = FindOptions.IsWholeWordChecked ? string.Format(@"\b{0}\b", Regex.Escape(FindOptions.FindText)) : Regex.Escape(FindOptions.FindText);
            RegexOptions options = FindOptions.IsMatchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
            if (!FindOptions.IsPrefix)
            {
                if (FindOptions.IsSelectionSelected)
                {
                    this.SearchResults.FindAllResult = from item in linesCollection
                                                       let selectionIndex = GetFirstMatchingIndex(item, captureRegex, options)
                                                       where (Regex.Match(item.Text, captureRegex, options).Success && selectionIndex >= 0)
                                                       select new FindResult()
                                                       {
                                                           LineNumber = linesCollection.IndexOf(item) + 1,
                                                           Index = selectionIndex,
                                                           Text = item.Text
                                                       };
                }
                else
                {
                    this.SearchResults.FindAllResult = from item in linesCollection
                                                       where Regex.Match(item.Text, captureRegex, options).Success
                                                       select new FindResult()
                                                       {
                                                           LineNumber = linesCollection.IndexOf(item) + 1,
                                                           Index = GetFirstMatchingIndex(item.Text, captureRegex, options),
                                                           Text = item.Text
                                                       };
                }
            }
            else
            {
                this.SearchResults.FindAllResult = from item in linesCollection
                                                   where (this.FindOptions.IsMatchCase && item.Text.StartsWith(FindOptions.FindText)) || (!this.FindOptions.IsMatchCase && item.Text.ToLower().StartsWith(FindOptions.FindText.ToLower()))
                                                   select new FindResult()
                                                   {
                                                       LineNumber = linesCollection.IndexOf(item) + 1,
                                                       Index = GetFirstMatchingIndex(item.Text, captureRegex, options),
                                                       Text = item.Text
                                                   };
            }
        }

        private int GetFirstMatchingIndex(string input, string captureRegex, RegexOptions options)
        {
            int index = 0;
            MatchCollection matches = Regex.Matches(input, captureRegex, options);
            if (matches.Count > 0)
            {
                index = matches[0].Index;
            }
            return index;
        }

        private int GetFirstMatchingIndex(LineItem input, string captureRegex, RegexOptions options)
        {
            int index = -1;
            MatchCollection matches = Regex.Matches(input.Text, captureRegex, options);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (this.FindOptions.IsSelectionSelected)
                    {
                        int lineindex = this.Lines.IndexOf(input);
                        if ((this.SearchResults.EditTextSelection.StartLine < lineindex && this.SearchResults.EditTextSelection.EndLine > lineindex) || (this.SearchResults.EditTextSelection.StartLine == lineindex && match.Index >= this.SearchResults.EditTextSelection.StartIndex) || (this.SearchResults.EditTextSelection.EndLine == lineindex && match.Index < this.SearchResults.EditTextSelection.EndIndex && this.SearchResults.EditTextSelection.EndIndex >= match.Index + this.FindOptions.FindText.Length))
                        {
                            index = match.Index;
                            break;
                        }
                    }
                    else
                    {
                        index = match.Index;
                        break;
                    }
                }
            }
            return index;
        }

        private void ReInitializeSearchResults()
        {
            List<LineItem> linesCollection = new List<LineItem>();
            int currentLineNumber = this.ScrollControl.CurrentLineItem != null ? this.ScrollControl.CurrentLineItem.LineNumber : 1;
            SearchResults.IsTextFound = false;
            if (FindOptions.IsSearchUp)
            {
                if (this.FindOptions.IsSelectionSelected)
                {
                    linesCollection.AddRange(Lines.Where(line => Lines.IndexOf(line) >= this.SearchResults.EditTextSelection.StartLine && Lines.IndexOf(line) <= this.SearchResults.EditTextSelection.EndLine).OrderByDescending(line => Lines.IndexOf(line)));
                }
                else
                {
                    linesCollection.Add(this.Lines[currentLineNumber - 1]);
                    if (currentLineNumber >= 1)
                    {
                        linesCollection.AddRange(Lines.Where(line => Lines.IndexOf(line) < currentLineNumber - 1).OrderByDescending(line => Lines.IndexOf(line)));
                    }
                    linesCollection.AddRange(Lines.Where(line => Lines.IndexOf(line) > currentLineNumber - 1).OrderByDescending(line => Lines.IndexOf(line)));
                }
            }
            else
            {
                if (this.FindOptions.IsSelectionSelected)
                {
                    if (this.SearchResults.EditTextSelection != null)
                    {
                        linesCollection.AddRange(Lines.Where(line => Lines.IndexOf(line) >= this.SearchResults.EditTextSelection.StartLine && Lines.IndexOf(line) <= this.SearchResults.EditTextSelection.EndLine));
                    }
                }
                else
                {
                    linesCollection.Add(this.Lines[currentLineNumber - 1]);
                    if (currentLineNumber >= 1)
                    {
                        linesCollection.AddRange(Lines.Where(line => Lines.IndexOf(line) > currentLineNumber - 1));
                    }
                    linesCollection.AddRange(Lines.Where(line => Lines.IndexOf(line) < currentLineNumber - 1));
                }
            }
            IEnumerable<LineItem> matchItem = null;
            if (this.FindOptions.IsSelectionSelected)
            {
                if (this.FindOptions.IsMatchWholeWord)
                {
                    string captureRegex = Regex.Escape(FindOptions.FindText) + @"|(?=(\W|\b))" + Regex.Escape(FindOptions.FindText) + @"\b";
                    RegexOptions options = FindOptions.IsMatchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
                    matchItem = from line in linesCollection
                                let match = Regex.Match(line.Text, captureRegex, options)
                                where match.Success && this.MatchWithinSelectedRange(line, match.Index, match.Value)
                                select line;
                    SearchResults.CaptureText = FindOptions.FindText;
                }
                else
                {
                    string captureRegex = Regex.Escape(FindOptions.FindText);
                    RegexOptions options = FindOptions.IsMatchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
                    matchItem = from line in linesCollection
                                let match = Regex.Match(line.Text, captureRegex, options)
                                where match.Success && this.MatchWithinSelectedRange(line, match.Index, match.Value)
                                select line;
                    SearchResults.CaptureText = captureRegex;
                }
            }
            else
            {
                if (this.FindOptions.IsMatchWholeWord)
                {
                    string captureRegex = Regex.Escape(FindOptions.FindText) + @"|(?=(\W|\b))" + Regex.Escape(FindOptions.FindText) + @"\b";
                    RegexOptions options = FindOptions.IsMatchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
                    matchItem = linesCollection.Where(line => Regex.Match(line.Text, captureRegex, options).Success);
                    SearchResults.CaptureText = captureRegex;
                    SearchResults.CaptureText = FindOptions.FindText;
                }
                else
                {
                    string captureRegex = Regex.Escape(FindOptions.FindText);
                    RegexOptions options = FindOptions.IsMatchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
                    matchItem = linesCollection.Where(line => Regex.Match(line.Text, captureRegex, options).Success);
                    SearchResults.CaptureText = captureRegex;
                }
            }

            if (matchItem.Count() > 0)
            {
                SearchResults.IsDirty = false;
                SearchResults.MatchingLineItem = new List<LineItem>(matchItem);
                SearchResults.CurrentIndex = 0;
                SearchResults.CursorIndex = -1;
            }
            else
            {
                SearchResults.MatchingLineItem = new List<LineItem>();
                SearchResults.CurrentIndex = -1;
                SearchResults.CursorIndex = -1;
                SearchResults.IsTextFound = false;
            }
        }

        internal bool MatchWithinSelectedRange(LineItem item, int matchIndex, string matchText)
        {
            int lineindex = this.Lines.IndexOf(item);
            if ((this.SearchResults.EditTextSelection.StartLine < lineindex && this.SearchResults.EditTextSelection.EndLine > lineindex) ||
                (this.SearchResults.EditTextSelection.StartLine == lineindex && matchIndex >= this.SearchResults.EditTextSelection.StartIndex) ||
                (this.SearchResults.EditTextSelection.EndLine == lineindex && matchIndex < this.SearchResults.EditTextSelection.EndIndex && this.SearchResults.EditTextSelection.EndIndex >= matchIndex + matchText.Length))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Helper method to open the Find window
        /// </summary>
        /// <param name="activetab">Active Tab of Find And Replace Window</param>
        private void OpenFindWindow(Tabs activetab)
        {
            Grid grid = null;
            if (findReplaceWindow != null && findReplaceWindow.IsVisible)
            {
                findReplaceWindow.Activate();
                return;
            }
            else if (findReplaceWindow != null)
            {
                findReplaceWindow.Content = null;
                findReplaceWindow.Activated -= findReplaceWindow_Activated;
                findReplaceWindow.Loaded -= new RoutedEventHandler(findReplaceWindow_Loaded);
                replaceControl = null;
                findReplaceWindow = null;
                grid = null;
                this.FindOptions.StatusMessage = "Ready";
            }

            CreateFindAndReplaceWindow();

            if (replaceControl == null)
            {
                replaceControl = new FindReplaceControl();
                replaceControl.DataContext = this.FindOptions;
            }

            if (grid == null)
            {
                grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                grid.Children.Add(replaceControl);
            }

            this.SetActiveTab(activetab);
            findReplaceWindow.Content = grid;
            findReplaceWindow.WindowStyle = WindowStyle.ToolWindow;
            if (Application.Current != null)
                findReplaceWindow.Owner = Application.Current.MainWindow;
            findReplaceWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            findReplaceWindow.ShowInTaskbar = false;
            findReplaceWindow.Title = "Find and Replace";
            findReplaceWindow.SizeToContent = SizeToContent.WidthAndHeight;
            findReplaceWindow.ResizeMode = ResizeMode.NoResize;

            if (this.ScrollControl.TextSelectionPointer != null)
            {
                if (ScrollControl.TextSelectionPointer.StartLine == ScrollControl.TextSelectionPointer.EndLine)
                {
                    FindOptions.FindText = this.SelectedText.TrimStart();
                    FindOptions.FindHistory.Push(FindOptions.FindText);
                }
                else
                {
                    FindOptions.IsSelectionSelected = true;
                }
            }
            findReplaceWindow.Loaded += new RoutedEventHandler(findReplaceWindow_Loaded);
            findReplaceWindow.Show();
        }

        private void findReplaceWindow_Closed(object sender, EventArgs e)
        {
            findReplaceWindow.Closed -= findReplaceWindow_Closed;
            if (FindReplaceWindowClosed != null)
                FindReplaceWindowClosed(sender, new RoutedEventArgs());
        }

        private void findReplaceWindow_Activated(object sender, EventArgs e)
        {
            findReplaceWindow.Activated -= findReplaceWindow_Activated;
            if (FindReplaceWindowOpened != null)
                FindReplaceWindowOpened(sender, new RoutedEventArgs());
        }

        private void EditControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= EditControl_Unloaded;
            replaceControl = null;
            findReplaceWindow = null;
        }

        private void CreateFindAndReplaceWindow()
        {
            if (findReplaceWindow == null)
            {
                string visualStyle = SkinStorage.GetVisualStyle(this).ToString();
                if (visualStyle == "Default")
                {
                    findReplaceWindow = new Window();
                }
                else
                {
                    findReplaceWindow = new ChromelessWindow();
                    SkinStorage.SetVisualStyle(findReplaceWindow, visualStyle);
                }

                if (findReplaceWindow != null)
                {
                    findReplaceWindow.Closed += findReplaceWindow_Closed;
                    findReplaceWindow.Activated += findReplaceWindow_Activated;
                }
            }
        }

        private void findReplaceWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FocusManager.SetFocusedElement(findReplaceWindow, replaceControl);
            Keyboard.Focus(replaceControl);
        }

        private void FindOptions_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DropDownListSelectedIndex")
            {
                if (FindOptions.DropDownListSelectedIndex == 0)
                {
                    this.SetActiveTab(Tabs.FindTab);
                }
                else if (FindOptions.DropDownListSelectedIndex == 1)
                {
                    this.SetActiveTab(Tabs.FindSymbolTab);
                }
            }
            this.SearchResults.IsSelectionChanged = false;
            if (this.SearchResults.EditTextSelection != null)
            {
                this.ScrollControl.UpdateSelectionPointer(this.SearchResults.EditTextSelection.StartLine, this.SearchResults.EditTextSelection.EndLine, this.SearchResults.EditTextSelection.StartIndex, this.SearchResults.EditTextSelection.EndIndex);
            }
            this.SearchResults.IsDirty = true;
        }

        private void findResultsGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            ApplyHideAnimation();
        }

        private void findResultsTab_MouseLeave(object sender, MouseEventArgs e)
        {
            if (findResultsTimer == null)
            {
                findResultsTimer = new DispatcherTimer();
                findResultsTimer.Interval = TimeSpan.FromMilliseconds(500);
                findResultsTimer.Tick += new EventHandler(findResultsTimer_Tick);
            }
            isHideFindResultsTab = true;
            findResultsTimer.Start();
        }

        private void findResultsTimer_Tick(object sender, EventArgs e)
        {
            if (isHideFindResultsTab)
            {
                ApplyHideAnimation();
                isHideFindResultsTab = false;
            }
            findResultsTimer.Stop();
            findResultsTimer.Tick -= new EventHandler(findResultsTimer_Tick);
            findResultsTimer = null;
        }

        private void findResultsList_MouseMove(object sender, MouseEventArgs e)
        {
            isHideFindResultsTab = false;
        }

        private void item_MouseMove(object sender, MouseEventArgs e)
        {
            ApplyPinAnimation();
        }

        private void ApplyHideAnimation()
        {
            if (pinButton != null && pinButton.IsChecked == false && this.findResultsList.ActualHeight == this.FindResultsTabHeight)
            {
                DoubleAnimation animation = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(200)));
                findResultsList.BeginAnimation(TabControl.HeightProperty, animation);
                findResultsTitleBar.Visibility = Visibility.Collapsed;
            }
        }

        private void ApplyPinAnimation()
        {
            if (pinButton != null && pinButton.IsChecked == false && this.findResultsList.ActualHeight != this.FindResultsTabHeight)
            {
                DoubleAnimation animation = new DoubleAnimation(this.FindResultsTabHeight, new Duration(TimeSpan.FromMilliseconds(200)));
                findResultsList.BeginAnimation(TabControl.HeightProperty, animation);
                findResultsTitleBar.Visibility = Visibility.Visible;
            }
        }

        private void pinButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton button = sender as ToggleButton;
            ResourceDictionary dictionary = new ResourceDictionary();
            dictionary.Source = new Uri(@"/Syncfusion.Edit.Wpf;component/Themes/Generic.xaml", UriKind.Relative);
            if (dictionary != null)
            {
                if (button.IsChecked == true)
                {
                    button.Style = dictionary["PinnedToggleButtonStyle"] as Style;
                }
                else
                {
                    button.Style = dictionary["UnPinned"] as Style;
                }
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsFindResultsTabClosed)
            {
                this.IsFindResultsTabClosed = false;
            }
            else
            {
                this.IsFindResultsTabClosed = true;
            }
        }

        #endregion Find and Replace Codes

        #region Intellisense Events

        /// <summary>
        /// Helper method to raise IntellisenseBoxOpening event
        /// </summary>
        /// <param name="args">represents the EditIntellisenseArgs value</param>
        internal void RaiseIntellisenseBoxOpeningEvent(EditIntellisenseArgs args)
        {
            if (this.IntellisenseBoxOpening != null)
            {
                this.IntellisenseBoxOpening(this, args);
            }
        }

        /// <summary>
        /// Helper method to raise IntellisenseDrillDownEvent event
        /// </summary>
        /// <param name="args">represents the EditIntellisenseArgs value</param>
        internal void RaiseIntellisenseDrillDownEvent(EditIntellisenseArgs args)
        {
            if (this.IntellisenseDrillDown != null)
            {
                this.IntellisenseDrillDown(this, args);
            }
        }

        #endregion Intellisense Events
    }
}