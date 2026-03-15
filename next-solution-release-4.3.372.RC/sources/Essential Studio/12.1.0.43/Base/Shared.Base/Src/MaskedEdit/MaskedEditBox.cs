#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Localization;

namespace Syncfusion.Windows.Forms.Tools
{

    /// <summary>
    /// The MaskedEditBox control provides restricted data input as well
    /// as formatted data output. This control supplies visual cues about
    /// the type of data being entered or displayed.
    /// </summary>
    /// <remarks>
    /// The MaskedEditBox is derived from <see cref="System.Windows.Forms.TextBox"/>
    /// and is fully compatible with the textbox control.
    /// <para>
    /// The MaskedEditBox control generally behaves as a textbox control with
    /// enhancements for optional masked input and formatted output. If you do
    /// not use an input mask, the MaskedEditBox control behaves much like a
    /// textbox.
    /// </para>
    /// <para>
    /// If you define an input mask using the Mask property, each character
    /// position in the MaskedEditBox control maps to either a placeholder
    /// of a specified type or a literal character. Literal characters, or
    /// literals, can give visual cues about the type of data being used.
    /// For example, the parentheses surrounding the area code of a telephone
    /// number are literals: (919).
    /// </para>
    /// <para>
    /// If you attempt to enter a character that conflicts with the input mask,
    /// the control generates a ValidationError event. The input mask prevents
    /// you from entering invalid characters into the control.
    /// </para>
    /// <para>
    /// The MaskedEditBox control provides full support for the Windows Forms
    /// designer and you can just drag-and-drop the control and set the properties.
    /// </para>
    /// <para>
    /// The MaskedEditBox control provides full support for data binding. The 
    /// <see cref="ClipMode"/> property has to be set to <see cref="ClipModes.ExcludeLiterals"/>
    /// when the MaskedEditBox's <see cref="Text"/> property is bound to a 
    /// DataColumn that only accepts numerical data.
    /// </para>
    /// <para>
    /// The <see cref="DataGroups"/> property provides the ability to break down the MaskedEditBox's content into different data groups.
    /// The data groups can be defined through the designer and is defined by the length of the data group. For example, a mask of type
    /// (###) ### - #### Ext 9999 representing a telephone number can be broken down into 3 data groups with the names "AreaCode", "PhoneNumber"
    /// and "Extension" by setting the group lengths to be 5, 11, 9. You can access the DataGroups and the values they hold through an index based accessor or
    /// name based accessor.
    /// 
    /// </para>
    /// </remarks>
    /// <example>
    /// <coderef file="c:\syncfusion\essential suite\tools\samples\quick start\MaskedEditDemo\CS\MainForm.cs" name="MaskedEditBox InitializeComponent" lang="CS">
    /// <code lang="C#">
    /// 
    ///             // InitializeComponent
    ///             // Create the Masked edit box control:
    ///             this.maskedEditBox1 = new MaskedEditBox();
    /// 
    ///             // Specifies if the prompt character can be entered:
    ///             this.maskedEditBox1.AllowPrompt = false;
    /// 
    ///             // The mask string:
    ///             this.maskedEditBox1.Mask = "&gt;?&lt;????????????";
    /// 
    ///             // The max length is set based on the mask:
    ///             this.maskedEditBox1.MaxLength = 13;
    /// 
    ///             // The clip mode specifies if the literals are included:
    ///             this.maskedEditBox1.ClipMode = ClipModes.IncludeLiterals;
    /// 
    ///             // The date time format:
    ///             this.maskedEditBox1.TimeSeparator = ':';
    ///             this.maskedEditBox1.DateSeparator = '-';
    /// 
    ///             // The number format:
    ///             this.maskedEditBox1.DecimalSeparator = '.';
    ///             this.maskedEditBox1.ThousandSeparator = ',';
    /// 
    ///             // Add the MaskedEditBox control to the form:
    ///             this.Controls.Add(this.maskedEditBox1);
    ///             </code></coderef>
    /// 
    ///             <coderef file="c:\syncfusion\essential suite\tools\samples\quick start\MaskedEditDemo\VB\MainForm.vb" name="MaskedEditBox InitializeComponent" lang="VB"><code lang="VB">
    ///            ' InitializeComponent
    ///            ' Create the Masked edit box control:
    ///            Me.maskedEditBox1 = New MaskedEditBox()
    ///            ' Specifies if the prompt character can be entered:
    ///            Me.maskedEditBox1.AllowPrompt = False
    ///            ' The mask string:
    ///            Me.maskedEditBox1.Mask = "&gt;?&lt;????????????"
    ///            ' The max length is set based on the mask:
    ///            Me.maskedEditBox1.MaxLength = 13
    ///            ' The clip mode specifies if the literals are included:
    ///            Me.maskedEditBox1.ClipMode = ClipModes.IncludeLiterals
    ///            ' The date time format:
    ///            Me.maskedEditBox1.TimeSeparator = Microsoft.VisualBasic.ChrW(58)
    ///            Me.maskedEditBox1.DateSeparator = Microsoft.VisualBasic.ChrW(45)
    ///            ' The number format:
    ///            Me.maskedEditBox1.DecimalSeparator = Microsoft.VisualBasic.ChrW(46)
    ///            Me.maskedEditBox1.ThousandSeparator = Microsoft.VisualBasic.ChrW(44)
    ///            ' Add the MaskedEditBox control to the form:
    ///            Me.Controls.Add(Me.maskedEditBox1)</code></coderef>
    /// </example>
    [
    ToolboxItem(true),
    System.Drawing.ToolboxBitmap(typeof(Syncfusion.Windows.Forms.PopupControlContainer), "ToolboxIcons.MaskedEditBox.bmp"),
    Description("Provides restricted data input as well as formatted data output.")
    ]
    public class MaskedEditBox : TextBoxExt
    {
        private bool m_bIsTextByInputChanged = false;

        /// <summary>
        /// Indicates whether TextBox content is modified, when Mask is used.
        /// </summary>
        private bool m_bModified = false;

        /// <summary>
        /// The mask string that specifies the mask behavior.
        /// </summary>
        private string maskString = String.Empty;

        /// <summary>
        /// The mask string to be displayed in the masked edit textbox.
        /// </summary>
        private string maskDisplay;

        /// <summary>
        /// The display string with the masks replaced.
        /// </summary>
        private string displayString;

        /// <summary>
        /// The prompt character.
        /// </summary>
        private int promptCharacterInt = 95;

        /// <summary>
        /// The prompt character when the control is in passive mode.
        /// </summary>
        private int passivePromptCharacterInt = 32;

        /// <summary>
        /// The padding character.
        /// </summary>
        private int paddingCharacterInt = 32;

        /// <summary>
        /// String for holding undo buffer.
        /// </summary>
        private string undoBufferText;

        /// <summary>
        /// String for holding redo buffer.
        /// </summary>
        private string redoBufferText;

        /// <summary>
        /// The insert mode.
        /// </summary>
        private bool insertMode = true;

        /// <summary>
        /// Indicates whether the prompt character can input in a mask.
        /// position
        /// </summary>
        private bool allowPrompt = false;

        /// <summary>
        /// The static internal list of mask characters.
        /// </summary>
        private static ArrayList maskList;

        /// <summary>
        /// Used internally for holding the special masks that don't take up a 
        /// mask position in the string.
        /// </summary>
        private Hashtable specialMasks;

        /// <summary>
        /// How to apply case sensitivity.
        /// </summary>
        private CasingNormalize casingNormalize;

        /// <summary>
        /// NumberFormatInfo object for handling globalization.
        /// </summary>
        private NumberFormatInfo numberFormatInfoObject;

        /// <summary>
        /// DateFormatInfo object for handling globalization.
        /// </summary>
        private DateTimeFormatInfo dateTimeFormatInfoObject;

        /// <summary>
        /// The usage mode for the MaskedEditBox.
        /// </summary>
        private MaskedUsageMode usageMode;

        /// <summary>
        /// Indicaqtes whether the control has been clicked.
        /// </summary>
        private bool focusClick = true;

        /// <summary>
        /// The maximum value that the control can take.
        /// </summary>
        private decimal maxValue = decimal.MaxValue;

        /// <summary>
        /// The minimum value that the control can take.
        /// </summary>
        private decimal minValue = 0m;

        /// <summary>
        /// Internal variable to track if the control is setting the Text property
        /// to a different value internally. In this case, the TextChanged event will
        /// not be raised. This solves the problem with the TextChanged event being 
        /// fired even when the user has not made any changes.
        /// </summary>
        private bool internalTextChange = false;

        /// <summary>
        /// The text that will be used to initailize the MaskedEdit.
        /// </summary>
        private string initText = String.Empty;

        /// <summary>
        /// The mask string for initialization.
        /// </summary>
        private string initMask = String.Empty;

        /// <summary>
        /// The MaxLength for initialization.
        /// </summary>
        private int initMaxLength = 0;

        /// <summary>
        /// Date separator value set during initialization.
        /// </summary>
        private char initDateSeparator;

        /// <summary>
        /// Used to differentiate when the actual display string is different
        /// from the internal display string (primarily because the prompt character
        /// can be set to NULL).
        /// </summary>
        private string internalDisplayString;


        /// <summary>
        /// SortedList with mappings of literal and mask characters between display
        /// and internal strings.
        /// </summary>
        private SortedList listDisplayToInternalString;

        /// <summary>
        /// Indicating whether the control use sequentially display mask's characters.
        /// </summary>
        private bool m_bSequentially = false;

        /// <summary>
        /// ValidationError event will be raised when an unacceptable
        /// character is encountered as input.
        /// </summary>
        /// <remarks>
        /// This event can be handled and you can do your processing based
        /// on the information provided. The ValidationErrorEventArgs object
        /// will provide the invalid text that was input and also the position
        /// within that text where the error occurred.
        /// </remarks>
        [Description("ValidationError event will be raised when an unacceptable character is encountered as input.")
        ]
        public event ValidationErrorEventHandler ValidationError;


        /// <summary>
        /// Used whenever the NULL character is the PromptCharacter.
        /// </summary>
        private int adjustedSelectionLength = -1;

        /// <summary>
        /// Used whenever the NULL character is the PromptCharacter.
        /// </summary>
        private int adjustedSelectionStart = -1;

        /// <summary>
        /// Internally holds the last used prompt character.
        /// </summary>
        private char lastUsedPromptChar = (char)0;

        /// <summary>
        /// Manages the state of the MaskedEditBox.
        /// </summary>
        private MaskedEditState internalState = MaskedEditState.None;

        /// <summary>
        /// MaskSatisfied event will be raised when the required fields in 
        /// a mask have been satisfied after new text has been entered / the
        /// text changes.
        /// </summary>
        /// <remarks>
        /// This event will be raised only when there are entries that 
        /// require input have been filled. This even can be used to move
        /// to another control after the mask rules have been satisfied.
        /// </remarks>
        [Description("MaskSatisfied event will be raised when the required fields in a mask have been satisfied after new text has been entered / the text changes")
        ]
        public event EventHandler MaskSatisfied;


        /// <summary>
        /// Handle this event to provide custom behavior to any of the 
        /// mask characters.
        /// </summary>
        /// This event is raised for each valid mask position that is
        /// being filled. In the event that the event is not handled (by
        /// setting the Handled property of the event information), the 
        /// normal logic will be used for validating input.
        [Description("Handle this event to provide custom behavior to any mask characters.")
        ]
        public event MaskCustomValidateEventHandler MaskCustomValidate;

        /// <summary>
        /// The ClipMode to use when returning the contents of the textbox
        /// (with or without literals).
        /// </summary>
        private ClipModes clipMode = ClipModes.IncludeLiterals;

        /// <summary>
        /// The integer value to offset the mask characters by.
        /// </summary>
        static int maskOffset;

        /// <summary>
        /// The context menu for the textbox.
        /// </summary>
        private ContextMenu editMenu;

        /// <summary>
        /// Menu items.
        /// </summary>
        private MenuItem miUndo, miCut, miCopy, miPaste, miDelete, miSelectAll;

        /// <summary>
        /// Indicates whether the locale default values have to be used for the
        /// NumberFormatInfo object.
        /// </summary>
        private bool useLocaleDefault;

        /// <summary>
        /// The data groups collection.
        /// </summary>
        private MaskedEditDataGroupInfoCollection dataGroups;

        /// <summary>
        /// The internal data groups collection.
        /// </summary>
        private MaskedEditDataGroupInfoCollection internalDataGroups;

        /// <summary>
        /// The currently selected culture.
        /// </summary>
        private CultureInfo selectedCulture = new CultureInfo(CultureInfo.CurrentCulture.LCID);

        /// <summary>
        /// Modifier for the culture.
        /// </summary>
        private SpecialCultureValues specialCultureValue = SpecialCultureValues.CurrentCulture;

        /// <summary>
        /// List of cultures that we consider to be RightToLeft.
        /// </summary>
        private ArrayList rightToLeftCultures = new ArrayList();

        /// <summary>
        /// Indicates whether the UseUserOverride value is to be set when creating
        /// the CultureInfo.
        /// </summary>
        private bool useUserOverride = true;


        /// <summary>
        /// Use this to check that the Focused property doesn't get
        /// set when we handle OnEnter.
        /// </summary>
        private bool hasFocus = false;


        /// <summary>
        /// Internal bool to tell the FormattedText method to return just the Text without
        /// any padding. This is used when there is a NULL prompt as the base.Text call fails
        /// when the Text property returns a value longer (?) than the one being set.
        /// </summary>
        private bool returnActualText = false;


        /// <summary>
        /// Indicates whether the cursor is to be positioned near the decimal
        /// when the control receives focus.
        /// </summary>
        private SpecialCursorPosition m_positionAt = SpecialCursorPosition.FirstPosition;

        /// <summary>
        /// Hashtable to hold positions.
        /// </summary>
        private Hashtable customFilledPositions;

        /// <summary>
        /// To enable KeyPress and KeyDown events to be handled 
        /// normally.
        /// </summary>
        private bool supressKeyPress = false;
        private bool supressKeyDown = false;

        /// <summary>
        /// Pulls the Character on the next data position on delete, pays caution to the mask.
        /// </summary>
        private bool pullCharOnDelete = false;

        /// <summary>
        /// Input mode.
        /// </summary>
        private MaskInputMode inputMode = MaskInputMode.OvertypeOnly;

        /// <summary>
        /// Support for culture initialization.
        /// </summary>
        private CultureInfo initCulture = new CultureInfo(CultureInfo.CurrentCulture.LCID);

        /// <summary>
        /// Property changed event handler.
        /// </summary>
        [
        Browsable(false),
        Syncfusion.Documentation.DocumentationExclude()
        ]
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This method raises the ValidationError event.
        /// </summary>
        /// <param name="invalidText">The text that was input.</param>
        /// <param name="startPosition">The start position of the error.</param>
        /// <remarks>
        /// See the <see cref="OnValidationError"/> method for more information.
        /// </remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void RaiseValidationError(string invalidText, int startPosition)
        {
            ValidationErrorArgs arg = new ValidationErrorArgs(invalidText, startPosition);
            this.OnValidationError(arg);
        }


        /// <summary>
        /// This method raises the MaskSatisfied event.
        /// </summary>
        /// <remarks>
        /// See the <see cref="OnMaskSatisfied"/> method for more information.
        /// </remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void RaiseMaskSatisfied()
        {
            this.OnMaskSatisfied(EventArgs.Empty);
        }

        /// <summary>
        /// Invokes the ValidationError event.
        /// <param name="args">A ValidationErrorEventArgs that contains the event data.</param>
        /// <remarks>
        /// The OnValidationError method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.
        /// 
        /// <note type="note">Inheritors: When overriding OnValidationError in a derived
        /// class, be sure to call the base class's OnValidationError method so that
        /// registered delegates receive the event.</note>
        /// 
        /// </remarks>
        /// </summary>		
        protected virtual void OnValidationError(ValidationErrorArgs args)
        {
            if (this.ValidationError != null)
                this.ValidationError(this, args);
        }

        /// <summary>
        /// Invokes the MaskSatisfied event.
        /// <param name="args">An EventArgs that contains the event data.</param>
        /// <remarks>
        /// The OnMaskSatisfied method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.
        /// 
        /// <note type="note">Inheritors: When overriding OnMaskSatisfied in a derived
        /// class, be sure to call the base class's OnMaskSatisfied method so that
        /// registered delegates receive the event.</note>
        /// 
        /// </remarks>
        /// </summary>		
        protected virtual void OnMaskSatisfied(EventArgs args)
        {
            if (this.MaskSatisfied != null)
                this.MaskSatisfied(this, args);
        }

        /// <summary>
        /// Creates an object of type MaskedEditBox and initializes it.
        /// </summary>
        /// <remarks>
        /// The default Mask is initialized to an empty string and this will
        /// result in the MaskedEditBox acting the same as a text box initially.
        /// </remarks>
        public MaskedEditBox()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(MaskedEditBox));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            this.rightToLeftCultures.Add("ar");
            this.rightToLeftCultures.Add("fa");
            this.rightToLeftCultures.Add("he");
            this.rightToLeftCultures.Add("ur");
            this.rightToLeftCultures.Add("syr");
            this.rightToLeftCultures.Add("div");

            this.maskString = String.Empty;
            this.displayString = String.Empty;
            this.maskDisplay = String.Empty;
            this.promptCharacterInt = 32;
            this.passivePromptCharacterInt = 32;
            this.undoBufferText = String.Empty;
            this.redoBufferText = String.Empty;
            this.specialMasks = new Hashtable();
            this.customFilledPositions = new Hashtable();
            this.numberFormatInfoObject = null;
            this.dateTimeFormatInfoObject = null;
            this.casingNormalize = CasingNormalize.changeToBoth;
            this.usageMode = MaskedUsageMode.Normal;
            this.dataGroups = new MaskedEditDataGroupInfoCollection(this);
            this.internalDataGroups = new MaskedEditDataGroupInfoCollection(this);
            this.listDisplayToInternalString = new SortedList();

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            InitializeContextMenu();
        }

        /// <summary>
        /// Signals the object that the initialization is completed.
        /// </summary>
        public override void EndInit()
        {
            base.EndInit();
            this.PassivePromptCharacterInt = this.PassivePromptCharacterInt; // Refresh lastUsedPromptChar.
        }
        /// <summary>
        /// Static constructor for initialization.
        /// </summary>
        static MaskedEditBox()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(MaskedEditBox));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            MaskedEditBox.maskOffset = 256;

            if (MaskedEditBox.maskList == null)
                MaskedEditBox.maskList = new ArrayList();
            else
                MaskedEditBox.maskList.Clear();

            MaskedEditBox.maskList.Add('#');	//# Digit placeholder. 
            MaskedEditBox.maskList.Add('.');	//. Decimal placeholder. The actual character used is the one specified as the decimal placeholder in your international settings. 
            //  This character is treated as a literal for masking purposes. 
            MaskedEditBox.maskList.Add(',');	//, Thousands separator. The actual character used is the one specified as the thousands separator in your international settings. This character is treated as a literal for masking purposes. 
            MaskedEditBox.maskList.Add(':');	//: Time separator. The actual character used is the one specified as the time separator in your international settings. This character is treated as a literal for masking purposes. 
            MaskedEditBox.maskList.Add('/');	// Date separator. The actual character used is the one specified as the date separator in your international settings. This character is treated as a literal for masking purposes. 
            MaskedEditBox.maskList.Add('\\');   // Treat the next character in the mask string as a literal. This allows you to include the '#', '&', 'A', and '?' characters in the mask. This character is treated as a literal for masking purposes. 
            MaskedEditBox.maskList.Add('&');	//& Character placeholder. Valid values for this placeholder are ANSI characters in the following ranges: 32-126 and 128-255. 
            MaskedEditBox.maskList.Add('>');	//> Convert all the characters that follow to uppercase. 
            MaskedEditBox.maskList.Add('<');	//< Convert all the characters that follow to lowercase. 
            MaskedEditBox.maskList.Add('A');	//A Alphanumeric character placeholder (entry required). For example: a  z, A  Z, or 0  9. 
            MaskedEditBox.maskList.Add('a');	//a Alphanumeric character placeholder (entry optional). 
            MaskedEditBox.maskList.Add('9');	//9 Digit placeholder (entry optional). For example: 0  9. 
            MaskedEditBox.maskList.Add('C');	//C Character or space placeholder (entry optional). This operates exactly like the & placeholder, and ensures compatibility with Microsoft Access. 
            MaskedEditBox.maskList.Add('?');	//? Letter placeholder. For example: a  z or A  Z. 
            MaskedEditBox.maskList.Add('y');	//l Letter placeholder. For example: a  z or A  Z. (optional)
            MaskedEditBox.maskList.Add('x');	//x Hexadecimal entry optional.
            MaskedEditBox.maskList.Add('X');	//X Hexadecimal entry required.
        }

        /// <summary>
        /// Initializes the context menu.
        /// </summary>
        private void InitializeContextMenu()
        {
            // Context menu.
            editMenu = new ContextMenu();

            // Undo menu item.

            miUndo = new MenuItem(SR.GetString(SR.Undo, this));
            miUndo.Click += new EventHandler(HandleMenuUndoClick);
            miUndo.Shortcut = Shortcut.CtrlZ;
            editMenu.MenuItems.Add(miUndo);
            editMenu.MenuItems.Add("-");

            // Cut menu item.

            miCut = new MenuItem(SR.GetString(SR.Cut,this));
            miCut.Click += new EventHandler(HandleMenuCutClick);
            miCut.Shortcut = Shortcut.CtrlX;
            editMenu.MenuItems.Add(miCut);

            // Copy menu item.

            miCopy = new MenuItem(SR.GetString(SR.Copy,this));
            miCopy.Click += new EventHandler(HandleMenuCopyClick);
            miCopy.Shortcut = Shortcut.CtrlC;
            editMenu.MenuItems.Add(miCopy);

            // Paste menu item.

            miPaste = new MenuItem(SR.GetString(SR.Paste,this));
            miPaste.Click += new EventHandler(HandleMenuPasteClick);
            miPaste.Shortcut = Shortcut.CtrlV;
            editMenu.MenuItems.Add(miPaste);

            // Delete menu item.

            miDelete = new MenuItem(SR.GetString(SR.Delete,this));
            miDelete.Click += new EventHandler(HandleMenuDeleteClick);
            miDelete.Shortcut = Shortcut.Del;
            editMenu.MenuItems.Add(miDelete);
            editMenu.MenuItems.Add("-");

            // Select all menu items.

            miSelectAll = new MenuItem(SR.GetString(SR.SelectAll,this));
            miSelectAll.Click += new EventHandler(HandleMenuSelectAllClick);
            miSelectAll.Shortcut = Shortcut.CtrlA;
            editMenu.MenuItems.Add(miSelectAll);
        }


        /// <summary>
        /// Sets the internal state of the control.
        /// </summary>
        /// <param name="newState">The new state of the control.</param>
        private void SetMaskedEditState(MaskedEditState newState)
        {
            this.SetMaskedEditState(newState, true);
        }

        /// <summary>
        /// Sets the internal state of the control.
        /// </summary>
        /// <param name="newState">The new state of the control.</param>
        /// <param name="updateDisplay">Indicates whether the display is to be updated.</param>
        private void SetMaskedEditState(MaskedEditState newState, bool updateDisplay)
        {
            if (this.internalState != newState)
            {
                if (newState == MaskedEditState.EditState)
                {
                    // Enter edit state.
                    this.AdjustStringAndSelectionForNullPrompt();
                    this.internalState = newState;
                }
                else if (newState == MaskedEditState.NormalState)
                {
                    this.internalState = newState;

                    // Here we need the actual prompt character.
                    char localPromptCharacter = '\0';

                    if (this.hasFocus == true)
                        localPromptCharacter = (char)this.promptCharacterInt;
                    else
                        localPromptCharacter = (char)this.passivePromptCharacterInt;

                    this.internalDisplayString = this.DisplayString;

                    if (localPromptCharacter == '\0' || this.Sequentially)
                    {
                        int selectionStart = this.AdjustedSelectionStart;
                        int selectionLength = this.AdjustedSelectionLength;
                        int newSelectionStart, newSelectionLength = 0;

                        this.DisplayString = this.GetDisplayStringForNullPrompt(this.displayString);

                        if (updateDisplay)
                        {
                            returnActualText = true;
                            this.internalTextChange = true;
                            // Now set the selection start and selection length for the truncated string.
                            newSelectionStart = this.GetValidMappedPosition(selectionStart);
                            newSelectionLength = this.GetValidMappedPosition(selectionStart + selectionLength) - newSelectionStart;

                            RefreshDisplay();
                            this.SetSelection(newSelectionStart, newSelectionLength);
                            this.internalTextChange = false;
                            returnActualText = false;
                        }

                    }
                    else
                        this.listDisplayToInternalString.Clear();
                }
            }
        }

        /// <summary>
        /// Handles the popup menu.
        /// </summary>
        /// <param name="sender">The context menu.</param>
        /// <param name="ea">The event information.</param>
        private void HandleContextMenuPopup(object sender, EventArgs ea)
        {
            this.Focus();

            miUndo.Enabled = this.CanUndo;

            if (this.Enabled == false || this.ReadOnly == true)
            {
                miCut.Enabled = false;
                miPaste.Enabled = false;
                miDelete.Enabled = false;

                if (this.SelectionLength > 0)
                    miCopy.Enabled = true;
                else
                    miCopy.Enabled = false;

            }
            else
            {
                if (this.SelectionLength > 0)
                {
                    miCut.Enabled = true;
                    miCopy.Enabled = true;
                }
                else
                {
                    miCut.Enabled = false;
                    miCopy.Enabled = false;
                }
                if (this.Text.Length > 0)
                    miDelete.Enabled = true;
                else
                    miDelete.Enabled = false;

                DataObject dobject = (DataObject)Clipboard.GetDataObject();
                miPaste.Enabled = dobject.GetDataPresent(typeof(string));
            }

            if (this.SelectionLength == this.Text.Length)
                miSelectAll.Enabled = false;
            else
                miSelectAll.Enabled = true;
        }

        /// <summary>
        /// Handles the Undo menu click.
        /// </summary>
        /// <param name="sender">The undo menu.</param>
        /// <param name="ea">The event data.</param>
        private void HandleMenuUndoClick(object sender, EventArgs ea)
        {
            this.Undo();
            this.ClearUndo();
        }

        /// <summary>
        /// Handles the Cut menu.
        /// </summary>
        /// <param name="sender">Cut menu.</param>
        /// <param name="ea">The event data.</param>
        private void HandleMenuCutClick(object sender, EventArgs ea)
        {
            this.Cut();
        }

        /// <summary>
        /// Handles the Copy menu.
        /// </summary>
        /// <param name="sender">Copy menu.</param>
        /// <param name="ea">The event data.</param>
        private void HandleMenuCopyClick(object sender, EventArgs ea)
        {
            this.Copy();
        }

        /// <summary>
        /// Handles the Paste menu.
        /// </summary>
        /// <param name="sender">Paste menu.</param>
        /// <param name="ea">The event data.</param>
        private void HandleMenuPasteClick(object sender, EventArgs ea)
        {
            this.Paste();
        }

        /// <summary>
        /// Handles the Delete menu.
        /// </summary>
        /// <param name="sender">Delete menu.</param>
        /// <param name="ea">The event data.</param>
        private void HandleMenuDeleteClick(object sender, EventArgs ea)
        {
            if (this.IsMaskActive() == true && !this.ReadOnly)
            {
                this.internalTextChange = true;
                this.SetMaskedEditState(MaskedEditState.EditState);
                this.HandleDeleteKey();
                this.SetMaskedEditState(MaskedEditState.NormalState);
                this.internalTextChange = false;
                this.OnTextChanged(new EventArgs());
            }
        }

        /// <summary>
        /// Handles the Select All menu.
        /// </summary>
        /// <param name="sender">Select All menu.</param>
        /// <param name="ea">The event data.</param>
        private void HandleMenuSelectAllClick(object sender, EventArgs ea)
        {
            this.SelectAll();
        }


        /// <summary>
        /// Invoke to initialize the masked edit control after 
        /// setting its properties.
        /// </summary>
        private void InitializeMaskEdit()
        {
            // Change the NumberFormat also.
            this.SetMaskedEditState(MaskedEditState.EditState);

            // Set Mask before because Culture set property changes it.
            this.internalTextChange = true;
            this.Mask = this.initMask;
            this.internalTextChange = false;
            this.casingNormalize = CasingNormalize.changeToBoth;

            if (this.SpecialCultureValue != SpecialCultureValues.CurrentCulture)
                this.Culture = this.initCulture;
            else
                this.ApplyRightToLeft();

            this.internalTextChange = true;

            if (this.maskString == null || this.maskString.Length == 0)
                this.Text = this.initText;
            this.internalTextChange = false;

            if (this.initMaxLength != 0)
                this.MaxLength = this.initMaxLength;

            // Set the default data groups if control is in NumericMode and there is 
            // 1 decimal separator.
            if (this.UsageMode == MaskedUsageMode.Numeric)
            {
                int decimalPos = 0;
                this.internalDataGroups.Clear();

                decimalPos = this.maskDisplay.IndexOf(this.DecimalSeparator);

                if (decimalPos != -1)
                {
                    if (this.maskDisplay.IndexOf(this.DecimalSeparator, decimalPos + 1) == -1)
                    {
                        // Only 1 decimal separator.
                        if (decimalPos != 0)
                        {
                            this.internalDataGroups.Add(new MaskedEditDataGroupInfo("Number", decimalPos, MaskGroupAlignment.Right));
                            this.internalDataGroups.Add(new MaskedEditDataGroupInfo("Decimal", this.maskDisplay.Length - decimalPos, MaskGroupAlignment.Left));
                        }
                        else
                            this.internalDataGroups.Add(new MaskedEditDataGroupInfo("Decimal", this.maskDisplay.Length, MaskGroupAlignment.Left));

                    }
                }
                else if (this.maskDisplay.Length > 0)
                {
                    this.internalDataGroups.Add(new MaskedEditDataGroupInfo("Number", this.maskDisplay.Length, MaskGroupAlignment.Right));
                }

            }

            this.AccessibleName = "MaskedEditBox";
            this.AccessibleDescription = "MaskedEdit TextBox";
            this.AccessibleRole = AccessibleRole.Text;
            this.SetMaskedEditState(MaskedEditState.NormalState);

        }

        /// <summary>
        /// Internal helper function for getting mask characters
        /// that rely on globalized settings.
        /// </summary>
        /// <param name="type">The MaskCharType value.</param>
        /// <returns>Char representing the globalized value.</returns>
        private char GetLocalizedValue(MaskCharTypes type)
        {
            char returnChar = ' ';

            switch (type)
            {
                case MaskCharTypes.maskCharDecimal:
                    returnChar = this.DecimalSeparator;
                    break;

                case MaskCharTypes.maskCharDateSep:
                    returnChar = this.DateSeparator;
                    break;

                case MaskCharTypes.maskCharThousands:
                    returnChar = this.ThousandSeparator;
                    break;

                case MaskCharTypes.maskCharTimeSep:
                    returnChar = this.TimeSeparator;
                    break;

                default:
                    returnChar = (char)MaskedEditBox.maskList[(int)type];
                    return returnChar;
            }

            return returnChar;
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (this.IsMaskActive() == true)
                    this.ContextMenu.Popup -= new EventHandler(this.HandleContextMenuPopup);
            }
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion

        /// <summary>
        /// Indicates whether TextBox content is modified, when Mask is used.
        /// </summary>
        public new bool Modified
        {
            get
            {
                return (base.Modified || m_bModified);
            }
            set
            {
                base.Modified = value;
                m_bModified = value;
            }
        }

        /// <summary>
        /// Use this property to define the mask string for the MaskedEditBox
        /// control.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The control can
        /// distinguish between numeric and alphabetic characters for
        /// validation, but cannot check for valid content, such as the
        /// correct month or time of day.
        /// </para>
        /// <list type="table">
        /// <listheader><term>Mask</term><description>Description</description></listheader>
        /// <item><term>Empty String</term><description>(Default) No mask. Acts like a text box.</description></item>
        /// <item><term>##-???-##</term><description>Medium date (US). Example: 17-Apr-02</description></item>
        /// <item><term>##-##-##</term><description>Short date (US). Example: 04-17-02</description></item>
        /// <item><term>##:## ??</term><description>Medium time. Example: 10:14 PM</description></item>
        /// <item><term>##:##</term><description>Short time. Example: 22:14</description></item>
        /// </list>
        /// <para>
        /// The input mask can consist of the following characters.
        /// </para>
        /// <para>
        /// <list type="table">
        /// <listheader><term>Mask character</term><description>Description</description></listheader>
        /// <item><term>#</term><description>Digit placeholder.</description></item>
        /// <item><term>.</term><description>Decimal placeholder. The actual character used is the one specified as the decimal placeholder in your international settings. This character is treated as a literal for masking purposes.</description></item>
        /// <item><term>,</term><description>Thousands separator. The actual character used is the one specified as the thousands separator in your international settings. This character is treated as a literal for masking purposes.</description></item>
        /// <item><term>:</term><description>Time separator. The actual character used is the one specified as the time separator in your international settings. This character is treated as a literal for masking purposes.</description></item>
        /// <item><term>/</term><description>Date separator. The actual character used is the one specified as the date separator in your international settings. This character is treated as a literal for masking purposes.</description></item>
        /// <item><term>\</term><description>Treat the next character in the mask string as a literal. This allows you to include the '#', &amp;, 'A', and '?' characters in the mask. This character is treated as a literal for masking purposes.</description></item>
        /// <item><term>&amp;</term><description>Character placeholder. Valid values for this placeholder are ANSI characters in the following ranges: 32-126 and 128-255.</description></item>
        /// <item><term>&gt;</term><description>Convert all the characters that follow to uppercase.</description></item>
        /// <item><term>&lt;</term><description>Convert all the characters that follow to lowercase.</description></item>
        /// <item><term>A</term><description>Alphanumeric character placeholder (entry required). For example: a,  z, A  Z, 0, or  9.</description></item>
        /// <item><term>a</term><description>Alphanumeric character placeholder (entry optional).</description></item>
        /// <item><term>9</term><description>Digit placeholder (entry optional). For example: 0,  9.</description></item>
        /// <item><term>C</term><description>Character or space placeholder (entry optional). This operates exactly like the &amp; placeholder, and ensures compatibility with Microsoft Access.</description></item>
        /// <item><term>?</term><description>Letter placeholder. For example: a  z or A  Z.</description></item>
        /// <item><term>Literal</term><description>All other symbols are displayed as literals; that is, as themselves.</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        [
        Browsable(true),
        DefaultValue(""),
        Category("Behavior"),
        Description("Use this property to define the mask string for the MaskedEditBox control.")
        ]
        public string Mask
        {
            get
            {
                return this.maskDisplay;
            }
            set
            {
                if (this.Initializing)
                {
                    this.initMask = value;
                    if (value != null || value != String.Empty)
                    {
                        this.SetMaskedEditState(MaskedEditState.EditState);
                        this.maskDisplay = value;
                        this.maskString = ExtractMaskValues(value);
                        this.SetMaxLength(maskString.Length);
                        this.Reset();
                        this.RefreshDisplay(this.DisplayString, false);
                        this.SetMaskedEditState(MaskedEditState.NormalState);
                        this.OnPropertyChanged(new PropertyChangedEventArgs("Mask"));
                        if (this.ContextMenu == null)
                        {
                            this.ContextMenu = this.editMenu;
                            this.ContextMenu.Popup += new EventHandler(this.HandleContextMenuPopup);
                        }
                        else
                            this.ContextMenu = this.editMenu;
                    }
                }
                else
                {
                    if (value == null || value == String.Empty)
                    {
                        this.maskDisplay = String.Empty;
                        this.maskString = String.Empty;
                        this.internalDisplayString = String.Empty;
                        this.displayString = String.Empty;
                        if (this.ContextMenu != null)
                        {
                            this.ContextMenu.Popup -= new EventHandler(this.HandleContextMenuPopup);
                            this.ContextMenu = null;
                        }
                        this.MaxLength = 32767;
                        this.SetBaseText(String.Empty);
                    }
                    else
                    {
                        if (this.maskDisplay != value)
                        {
                        this.SetMaskedEditState(MaskedEditState.EditState);
                        this.maskDisplay = value;
                        this.maskString = ExtractMaskValues(value);
                        this.SetMaxLength(maskString.Length);
                        string dispString = this.DisplayString;
                        this.Reset();
                        this.RefreshDisplay(this.DisplayString, false);
                        if (dispString != string.Empty)
                            this.SetExternalText(dispString, 0, true, true);
                        this.SetMaskedEditState(MaskedEditState.NormalState);
                        this.OnPropertyChanged(new PropertyChangedEventArgs("Mask"));
                        if (this.ContextMenu == null)
                        {
                            this.ContextMenu = this.editMenu;
                            this.ContextMenu.Popup += new EventHandler(this.HandleContextMenuPopup);
                        }
                        else
                            this.ContextMenu = this.editMenu;

                        this.initMask = value;
                    }
                    }
                }

            }
        }

        /// <summary>
        /// Indicates whether the cursor is to be positioned at the decimal separator (if any) when the
        /// control receives focus.
        /// </summary>
        /// <remarks>The value will be False by default.</remarks>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Obsolete
        ]
        public bool PositionAtDecimal
        {
            get
            {
                bool bValueToReturn = false;
                if (this.PositionAt == SpecialCursorPosition.Decimal)
                    bValueToReturn = true;
                return bValueToReturn;
            }

            set
            {
                bool bValue = value;
                if (bValue)
                    this.PositionAt = SpecialCursorPosition.Decimal;
                else
                    this.PositionAt = SpecialCursorPosition.FirstMaskPosition;
            }

        }

        [
        Browsable(true),
        DefaultValue(SpecialCursorPosition.FirstPosition),
        Category("Behavior"),
        Description("Use this property to define MaskedEditBox control cursor position behavior on getting focus."),
        ]
        public SpecialCursorPosition PositionAt
        {
            get
            {
                return m_positionAt;
            }

            set
            {
                if (m_positionAt != value)
                {
                    m_positionAt = value;
                }
            }
        }

        /// <summary>
        /// Adjusts the MaxLength property based on the Mask string.
        /// </summary>
        [Category(@"Behavior"),
        Description("The Maximum length of the input string."),
        RefreshProperties(RefreshProperties.Repaint)]
        public override int MaxLength
        {
            set
            {

                if (this.Initializing)
                {
                    base.MaxLength = value;
                    this.initMaxLength = value;
                }
                else
                {
                    int maskLength = this.maskString.Length;
                    int hiddenMasks = this.Mask.Length - maskLength;

                    // Adjust the mask to be in sync with the MaxLength.
                    if (maskLength == 0)
                    {
                        base.MaxLength = value;
                    }
                    else if (maskLength < value)
                    {
                        char lastMaskCharacter = this.GetLastMaskCharacter();
                        // Pad the mask with the last mask character.
                        if (lastMaskCharacter != '\0')
                            this.Mask = this.Mask.PadRight(value + hiddenMasks, lastMaskCharacter);
                    }
                    else if (maskLength > value)
                    {
                        // Truncate the mask.
                        this.Mask = this.Mask.Remove(value, maskLength - value);
                    }
                    else
                        base.MaxLength = value;

                    this.initMaxLength = value;
                }

            }
        }

        /// <summary>
        /// Returns the last mask character.
        /// </summary>
        /// <returns></returns>
        private char GetLastMaskCharacter()
        {
            char lastMaskCharacter = '\0';

            if (this.maskString.Length > 0)
                lastMaskCharacter = this.maskString[this.maskString.Length - 1];

            if ((int)lastMaskCharacter > -MaskedEditBox.maskOffset)
                lastMaskCharacter = (char)((int)lastMaskCharacter - MaskedEditBox.maskOffset);
            return lastMaskCharacter;
        }

        /// <summary>
        /// Internal helper function to set the maximum length.
        /// </summary>
        /// <param name="maskLength">The new mask length.</param>
        private void SetMaxLength(int maskLength)
        {
            base.MaxLength = maskLength;
        }

        /// <summary>
        /// Gets / sets the display string.
        /// </summary>
        private string DisplayString
        {
            get
            {
                return this.displayString;
            }

            set
            {
                if (value != null)
                {
                    this.displayString = value;
                }
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="args">The argument for the PropertyChanged event.</param>
        private void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, args);
            }
        }

        /// <summary>
        /// Overrides and prevents the event from being raised if the control is
        /// being initialized.
        /// </summary>
        /// <param name="arg">The event args.</param>
        protected override void OnTextChanged(EventArgs arg)
        {
            m_bModified = m_bIsTextByInputChanged;
            m_bIsTextByInputChanged = false;

            if (this.internalTextChange == false)
                base.OnTextChanged(arg);
        }

        /// <summary>
        /// Gets / sets the character that will be used instead of mask characters when
        /// the mask position has not been filled.
        /// </summary>
        /// <remarks>
        /// The default value for the prompt character is set to '-'.
        /// </remarks>
        [
        Browsable(true),
        DefaultValue('_'),
        Category("Appearance"),
        Description("Specifies the character that will be used instead of mask characters when the mask position has not been filled."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public char PromptCharacter
        {
            get
            {
                return (char)this.promptCharacterInt;
            }
            set
            {
                if (this.Initializing == true)
                {
                    this.promptCharacterInt = (int)value;
                }
                else
                {
                    this.promptCharacterInt = (int)value;
                    this.SetMaskedEditState(MaskedEditState.EditState);
                    // Refresh the masked edit with the new prompt.
                    if (this.hasFocus == true)
                        this.RefreshPromptCharacter();
                    this.SetMaskedEditState(MaskedEditState.NormalState);
                }

            }
        }


        /// <summary>
        /// Gets / sets the integer version of the PromptCharacter.
        /// </summary>
        /// <remarks>
        /// This will not be visible through the designer. Can be set through code.
        /// This will be persisted by the designer.
        /// </remarks>
        [
        Browsable(false),
        DefaultValue(32)
        ]
        public int PromptCharacterInt
        {
            get
            {
                return this.promptCharacterInt;
            }

            set
            {
                this.PromptCharacter = (char)value;
            }
        }

        /// <summary>
        /// Gets / sets the character that will be used instead of mask characters when
        /// the mask position has not been filled when the Text property is accessed.
        /// </summary>
        /// <remarks>
        /// The default value for the prompt character is set to '-'.
        /// </remarks>
        [
        Browsable(true),
        DefaultValue(' '),
        Category("Appearance"),
        Description("Specifies the character that will be used instead of mask characters when the mask position has not been filled when the Text property is used."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public char PaddingCharacter
        {
            get
            {
                return (char)this.paddingCharacterInt;
            }
            set
            {
                this.paddingCharacterInt = (int)value;
            }
        }

        /// <summary>
        /// Gets / sets the integer version of the PaddingCharacter.
        /// </summary>
        /// <remarks>
        /// This will not be visible through the designer. Can be set through code.
        /// This will be persisted by the designer.
        /// </remarks>
        [
        Browsable(false),
        DefaultValue(32)
        ]
        public int PaddingCharacterInt
        {

            get
            {
                return this.paddingCharacterInt;
            }

            set
            {
                this.PaddingCharacter = (char)value;
            }
        }


        /// <summary>
        /// Gets / sets the usage mode for the MaskedEditBox.
        /// </summary>
        /// <remarks>
        /// The UsageMode defines the behavior of the MaskedEditBox 
        /// to accomodate specialized input for numbers.
        /// </remarks>
        [
        Browsable(true),
        DefaultValue(MaskedUsageMode.Normal),
        Category("Behavior"),
        Description("Specifies if the MaskedEditBox is to behave as a numeric control.")
        ]
        public MaskedUsageMode UsageMode
        {
            get
            {
                return this.usageMode;
            }
            set
            {
                this.usageMode = value;
            }
        }

        /// <summary>
        /// Gets / sets the input mode of the 
        /// control.
        /// </summary>
        /// <remarks>
        /// The InputMode defines the behavior of the MaskedEditBox 
        /// to accommodate specialized input for numbers etc.
        /// </remarks>
        [
        Browsable(true),
        DefaultValue(MaskInputMode.OvertypeOnly),
        Category("Behavior"),
        Description("Specifies the input mode for the MaskedEditBox.")
        ]
        public MaskInputMode InputMode
        {
            get
            {
                return this.inputMode;
            }
            set
            {
                this.inputMode = value;
            }
        }

        /// <summary>
        /// Pulls the next data position on delete, pays attention to the mask.
        /// </summary>
        [Browsable(true), DefaultValue(false), Description("Pulls the next data position on delete, pays attention to the mask")]
        public bool PullCharOnDelete
        {
            get { return pullCharOnDelete; }
            set { pullCharOnDelete = value; }
        }

        /// <summary>
        /// Gets / sets the culture that is to be used for formatting the currency display.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Localizable(true),
        RefreshProperties(RefreshProperties.Repaint),
        Description("The current culture for the MaskedEditBox control."),
        Editor(typeof(Syncfusion.Windows.Forms.Tools.Design.MaskedEditCultureEditor), typeof(System.Drawing.Design.UITypeEditor))
        ]
        public CultureInfo Culture
        {
            get
            {
                return this.selectedCulture;
            }

            set
            {
                if (this.SpecialCultureValue == SpecialCultureValues.None)
                    this.selectedCulture = value;
                else if (this.SpecialCultureValue == SpecialCultureValues.CurrentCulture)
                    this.selectedCulture = CultureInfo.CurrentCulture;
                else if (this.SpecialCultureValue == SpecialCultureValues.UICulture)
                    this.selectedCulture = CultureInfo.CurrentUICulture;
                else if (this.SpecialCultureValue == SpecialCultureValues.InstalledCulture)
                    this.selectedCulture = CultureInfo.InstalledUICulture;

                this.initCulture = value;

                if (this.Initializing == false)
                {
                    // Change the NumberFormat also.
                    this.ApplyRightToLeft();
                    this.NumberFormatInfoObject = this.Culture.NumberFormat;
                    this.DateTimeFormatInfoObject = this.Culture.DateTimeFormat;
                }
            }
        }

        private bool ShouldSerializeCulture()
        {
            if (this.SpecialCultureValue == SpecialCultureValues.None)
                return true;
            else
                return false;
        }

        private void ResetCulture()
        {
            this.SpecialCultureValue = SpecialCultureValues.None;
            this.Culture = CultureInfo.CurrentCulture;
        }


        /// <summary>
        /// Gets / sets the mode for the cultures.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        RefreshProperties(RefreshProperties.Repaint),
        Description("The special culture value will override the current culture if set to value other than none.")
        ]
        public SpecialCultureValues SpecialCultureValue
        {
            get
            {
                return this.specialCultureValue;
            }

            set
            {
                this.specialCultureValue = value;
            }
        }

        private bool ShouldSerializeSpecialCultureValue()
        {
            if (this.SpecialCultureValue != SpecialCultureValues.CurrentCulture)
                return true;
            else
                return false;
        }

        private void ResetSpecialCultureValue()
        {
            this.SpecialCultureValue = SpecialCultureValues.CurrentCulture;
            this.Culture = CultureInfo.CurrentCulture;
        }


        /// <summary>
        /// Gets / sets the character that will be used instead of mask characters when
        /// the mask position has not been filled (when the control does not have the focus).
        /// </summary>
        /// <remarks>
        /// The default value for the prompt character is set to '\0'.
        /// </remarks>
        [
        Browsable(true),
        DefaultValue(' '),
        Category("Appearance"),
        Description("The prompt character that will serve as placeholder for mask characters when the control does not have the focus."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public char PassivePromptCharacter
        {
            get
            {
                return (char)this.passivePromptCharacterInt;
            }
            set
            {
                if (this.Initializing == true)
                {
                    this.passivePromptCharacterInt = (int)value;
                }
                else
                {
                    this.passivePromptCharacterInt = (int)value;
                    this.SetMaskedEditState(MaskedEditState.EditState);
                    if (this.Initializing == false)
                    {
                        if (this.hasFocus == false)
                        {
                            this.RefreshPromptCharacter();
                        }
                    }
                    this.SetMaskedEditState(MaskedEditState.NormalState);
                }

            }
        }

        /// <summary>
        /// Gets / sets the integer version of the PassivePromptCharacter.
        /// </summary>
        /// <remarks>
        /// This will not be visible through the designer. Can be set through code.
        /// This will be persisted by the designer.
        /// </remarks>
        [
        Browsable(false),
        DefaultValue(32)
        ]
        public int PassivePromptCharacterInt
        {

            get
            {
                return this.passivePromptCharacterInt;
            }

            set
            {
                this.PassivePromptCharacter = (char)value;
            }
        }

        /// <summary>
        /// Returns the text in the MaskedEditBox control, excluding literal 
        /// characters of the input mask. 
        /// </summary>
        /// <remarks>
        /// For example if the content of the MaskedEditBox is 
        /// 99-222-9999, the mask is ##-###-###, 
        /// the ClipText will return 992229999.
        /// </remarks>
        [
        Browsable(false),
        Category("Behavior"),
        Description("Returns the text in the MaskedEditBox control, excluding literal characters of the input mask.")
        ]
        public string ClipText
        {
            get
            {
                string displayText = String.Empty;
                int adjSelectionStart = 0;
                int adjSelectionLength = 0;
                this.internalTextChange = true;
                //added for StackOverFlowException
                this.returnActualText = true;
                displayText = this.DisplayString;
                // We only need the adjusted display text.
                this.GetAdjustedValuesForNullPrompt(ref displayText, ref adjSelectionStart, ref adjSelectionLength);
                string clipText = GetClipText(displayText, 0, this.maskString.Length);
                this.internalTextChange = false;
                //added
                this.returnActualText = false;
                return clipText;
            }
        }

        /// <summary>
        /// Returns the clip text.
        /// </summary>
        /// <param name="startPosition">The starting position for the selection.</param>
        /// <param name="endPosition">The end position for the selection.</param>
        /// <returns>Returns the clip text.</returns>
        private string GetClipText(string displayText, int startPosition, int endPosition)
        {
            int index = 0;
            string selectedData = String.Empty;

            if (endPosition > this.maskString.Length)
                endPosition = this.maskString.Length;

            if (this.maskString != String.Empty)
            {
                for (index = startPosition; index < endPosition; index++)
                    if (!IsLiteralCharacter(this.maskString[index]))
                        selectedData += displayText[index];

                // Replace the prompt character with the padding character.
                StringBuilder builder = new StringBuilder();
                builder.Append(selectedData);
                char localPromptChar = '\0';
                if (this.hasFocus == true)
                    localPromptChar = this.GetPromptCharacter();
                else
                    localPromptChar = this.GetPassivePromptCharacter();

                if (this.paddingCharacterInt == 0)
                {
                    int deletedPlacesCount = 0;
                    int length = builder.Length;
                    for (int i = 0; i < length; i++)
                    {
                        if (builder[i - deletedPlacesCount] == localPromptChar)
                        {
                            builder.Remove(i - deletedPlacesCount, 1);
                            deletedPlacesCount++;
                        }
                    }
                }
                else
                    builder.Replace(localPromptChar, this.PaddingCharacter);

                selectedData = builder.ToString();
            }
            else
            {
                return displayText;
            }

            return selectedData;
        }

        /// <summary>
        /// The MaskedEditBox can be defined to hold multiple data groups.
        /// </summary>
        /// <remarks>
        /// The DataGroups can be added through the designer. DataGroups are
        /// defined by the length of the group. The value of the group can be
        /// accessed by the index of the group or through the name of the group.
        /// <para>
        /// A MaskedEditBox with a mask (###) ###-#### Ext 9999 representing a 
        /// US phone number and extension can be defined to have 3 groups - 
        /// representing the AreaCode, PhoneNumber, and Extension. The groups would
        /// have lengths of 5, 11 and 9 in that order.
        /// </para>
        /// <para>
        /// The DataGroup's value is affected by the <see cref="ClipMode"/> property.
        /// </para>
        /// </remarks>
        [
        Browsable(true),
        Category("Behavior"),
        Description("The data groups that can be used for splitting up the text."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        ]
        public MaskedEditDataGroupInfoCollection DataGroups
        {
            get
            {
                return this.dataGroups;
            }
        }

        /// <summary>
        /// Overloaded. Returns the value of a DataGroup as a string.
        /// </summary>
        [
        Browsable(false),
        Category("Behavior"),
        Description("The data groups that can be used for splitting up the text.")
        ]
        public string GetDataGroupValue(string formttedText, MaskedEditDataGroupInfoCollection currentDataGroups, int index)
        {
            return this.GetDataGroupValue(formttedText, currentDataGroups, index, this.ClipMode);
        }

        /// <summary>
        /// Returns the value of a DataGroup as a string.
        /// </summary>
        [
        Browsable(false),
        Category("Behavior"),
        Description("The data groups that can be used for splitting up the text.")
        ]
        public string GetDataGroupValue(MaskedEditDataGroupInfoCollection currentDataGroups, int index)
        {
            return this.GetDataGroupValue(this.FormattedText, currentDataGroups, index, this.ClipMode);
        }

        /// <summary>
        /// Returns the value for the DataGroup.
        /// </summary>
        /// <param name="currentDataGroups">The current data group collection.</param>
        /// <param name="index">The index of the DataGroup.</param>
        /// <param name="clipMode">The current clipmode.</param>
        /// <returns>A string with the value of the DataGroup.</returns>
        private string GetDataGroupValue(string formattedText, MaskedEditDataGroupInfoCollection currentDataGroups, int index, ClipModes clipMode)
        {
            ArrayList dataGroupArrayList;
            int startIndex;
            int groupLength;
            string groupValue;
            string[] groupValueArray;

            dataGroupArrayList = new ArrayList();
            startIndex = 0;

            foreach (MaskedEditDataGroupInfo groupInfo in currentDataGroups)
            {
                groupLength = Math.Min(groupInfo.DataGroupSize, formattedText.Length - startIndex);

                if (clipMode == ClipModes.IncludeLiterals)
                    groupValue = formattedText.Substring(startIndex, groupLength);
                else
                    groupValue = GetClipText(this.DisplayString, startIndex, startIndex + groupLength);
                dataGroupArrayList.Add(groupValue);
                startIndex += groupLength;
            }

            groupValueArray = ((string[])(dataGroupArrayList.ToArray(typeof(string))));
            return groupValueArray[index];
        }


        /// <summary>
        /// Returns the text for the DataGroup.
        /// </summary>
        /// <param name="currentDataGroups">The current data group collection.</param>
        /// <param name="index">The index of the DataGroup.</param>
        /// <param name="fullText">The full text.</param>
        /// <returns>The text for the DataGroup.</returns>
        private string GetDataGroupText(MaskedEditDataGroupInfoCollection currentDataGroups, int index, string fullText)
        {
            string formattedText;
            ArrayList dataGroupArrayList;
            int startIndex;
            int groupLength;
            string groupValue;
            string[] groupValueArray;

            formattedText = fullText;

            dataGroupArrayList = new ArrayList();
            startIndex = 0;

            foreach (MaskedEditDataGroupInfo groupInfo in currentDataGroups)
            {
                groupLength = Math.Min(groupInfo.DataGroupSize, formattedText.Length - startIndex);

                groupValue = formattedText.Substring(startIndex, groupLength);

                dataGroupArrayList.Add(groupValue);
                startIndex += groupLength;
            }

            groupValueArray = ((string[])(dataGroupArrayList.ToArray(typeof(string))));
            return groupValueArray[index];
        }

        /// <summary>
        /// Returns the offset for the DataGroup from the beginning of the string.
        /// </summary>
        /// <param name="currentDataGroups">The current data group collection.</param>
        /// <param name="groupIndex">The index of the DataGroup.</param>
        /// <returns>The offset for the current DataGroup.</returns>
        private int GetDataGroupOffset(MaskedEditDataGroupInfoCollection currentDataGroups, int groupIndex)
        {
            int groupOffset = 0;
            int groupCount = 0;

            foreach (MaskedEditDataGroupInfo groupInfo in currentDataGroups)
            {
                if (groupCount == groupIndex)
                    break;

                groupOffset = groupOffset + groupInfo.DataGroupSize;
                groupCount++;
            }

            return groupOffset;
        }


        /// <summary>
        /// Returns the formatted text with the formatting.
        /// </summary>
        /// <remarks>
        /// For example, if the text in the MaskedEditBox is (999)999-9999, the
        /// FormattedText property will give (999)999-9999.
        /// </remarks>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public string FormattedText
        {
            get
            {
                string selectedData = base.Text;
                int deletedPlacesCount = 0;
                bool nullPrompt = false;

                if (this.hasFocus == true)
                {
                    if (this.promptCharacterInt == 0)
                        nullPrompt = true;
                }
                else
                {
                    if (this.passivePromptCharacterInt == 0)
                        nullPrompt = true;
                }

                if (nullPrompt == true || this.Sequentially)
                {
                    // Get the actual string.
                    selectedData = this.internalDisplayString;
                }

                if (selectedData == null)
                    selectedData = String.Empty;

                if (selectedData.Length > 0 && selectedData.Length == this.maskString.Length && returnActualText == false)
                {
                    // Finally replace the prompt character with the padding character.
                    StringBuilder builder = new StringBuilder();
                    builder.Append(selectedData);

                    for (int index = 0; index < this.maskString.Length; index++)
                    {
                        // Mask characters.
                        if (IsLiteralCharacter(this.maskString[index]) == false)
                        {
                            if (this.IsMaskPositionFilled(selectedData, index) == false)
                            {
                                if (this.paddingCharacterInt == 0)
                                {
                                    builder.Remove(index - deletedPlacesCount, 1);
                                    deletedPlacesCount++;
                                }
                                else
                                {
                                    builder[index - deletedPlacesCount] = this.PaddingCharacter;
                                }
                            }
                        }
                    }

                    selectedData = builder.ToString();
                }

                return selectedData;
            }
        }

        /// <summary>
        ///	Gets / sets the format of the text that will be returned by the MaskedEditBox
        ///	control. The nature of the formatting is set through the <see cref="ClipModes"/> type.
        /// </summary>
        /// <remarks>
        /// See the <see cref="ClipModes"/> type for the possible values and more information.
        /// </remarks>
        [
        Browsable(true),
        Description("Specifies the format of the text that will be returned by the MaskedEditBox control."),
        Category("Behavior"),
        DefaultValue(ClipModes.IncludeLiterals)
        ]
        public ClipModes ClipMode
        {
            get
            {
                return this.clipMode;
            }
            set
            {
                this.clipMode = value;
            }
        }

        /// <summary>
        /// Gets / sets the character to use when a thousands separator position is specified.
        /// </summary>
        /// <remarks>
        /// This value is initially set from the <see cref="System.Globalization.NumberFormatInfo"/> 
        /// and can be changed based on your requirements or based on the locale.
        /// </remarks>
        [
        Browsable(true),
        Category("Appearance"),
        Description("The character to use when a thousands separator position is specified.")
        ]
        public char ThousandSeparator
        {
            get
            {
                string ret = this.NumberFormatInfoObject.NumberGroupSeparator;
                return Convert.ToChar(ret);
            }

            set
            {
                if (value != '\0')
                {

                    this.NumberFormatInfoObject.NumberGroupSeparator = Convert.ToString(value);
                    if (this.Initializing == false)
                        this.Mask = this.maskDisplay;
                }
            }
        }

        /// <summary>
        /// Indicates whether the ThousandSeparator should not be serialized if
        /// the UseLocaleDefault property is set.
        /// </summary>
        /// <returns>True if the </returns>
        private bool ShouldSerializeThousandSeparator()
        {
            if (this.Culture.NumberFormat.NumberGroupSeparator != Convert.ToString(this.ThousandSeparator))
                return true;
            else
                return false;

        }

        /// <summary>
        /// Resets the value to the culture specific value.
        /// </summary>
        private void ResetThousandSeparator()
        {
            this.ThousandSeparator = Convert.ToChar(this.Culture.NumberFormat.NumberGroupSeparator);
        }

        /// <summary>
        /// Gets / sets the character to use when a date separator position is specified.
        /// </summary>
        /// <remarks>
        /// This value is initially set from the <see cref="System.Globalization.DateTimeFormatInfo"/> 
        /// and can be changed based on your requirements or based on the locale.
        /// </remarks>
        [
        Browsable(true),
        Category("Appearance"),
        Description("The character to use when a date separator position is specified.")
        ]
        public char DateSeparator
        {
            get
            {
                string ret = this.DateTimeFormatInfoObject.DateSeparator;
                return Convert.ToChar(ret);
            }

            set
            {
                if (value != '\0')
                {
                    if (this.Initializing == true)
                        this.initDateSeparator = value;
                    else
                    {
                        this.DateTimeFormatInfoObject.DateSeparator = Convert.ToString(value);
                        this.Mask = this.maskDisplay;

                        this.initDateSeparator = value;
                    }
                }
            }
        }

        /// <summary>
        /// Indicates whether the DateSeparator should not be serialized if
        /// the UseLocaleDefault property is set.
        /// </summary>
        /// <returns> </returns>
        private bool ShouldSerializeDateSeparator()
        {
            if (this.Culture.DateTimeFormat.DateSeparator != Convert.ToString(this.DateSeparator))
                return true;
            else
                return false;

        }

        /// <summary>
        /// Resets the value to the culture specific value.
        /// </summary>
        private void ResetDateSeparator()
        {
            this.DateSeparator = Convert.ToChar(this.Culture.DateTimeFormat.DateSeparator);
        }

        /// <summary>
        /// Gets / sets the character to use when a time separator position is specified.
        /// </summary>
        /// <remarks>
        /// This value is initially set from the <see cref="System.Globalization.DateTimeFormatInfo"/> 
        /// and can be changed based on your requirements or based on the locale.
        /// </remarks>
        [
        Browsable(true),
        Category("Appearance"),
        Description("The character to use when a time separator position is specified.")
        ]
        public char TimeSeparator
        {
            get
            {
                string ret = this.DateTimeFormatInfoObject.TimeSeparator;
                return Convert.ToChar(ret);
            }

            set
            {
                if (value != '\0')
                {

                    this.DateTimeFormatInfoObject.TimeSeparator = Convert.ToString(value);
                    if (this.Initializing == false)
                        this.Mask = this.maskDisplay;
                }
            }
        }

        /// <summary>
        /// Indicates whether the TimeSeparator should not be serialized if
        /// the UseLocaleDefault property is set.
        /// </summary>
        /// <returns> </returns>
        private bool ShouldSerializeTimeSeparator()
        {
            if (this.Culture.DateTimeFormat.TimeSeparator != Convert.ToString(this.TimeSeparator))
                return true;
            else
                return false;

        }

        /// <summary>
        /// Resets the value to the culture specific value.
        /// </summary>
        private void ResetTimeSeparator()
        {
            this.TimeSeparator = Convert.ToChar(this.Culture.DateTimeFormat.TimeSeparator);
        }

        /// <summary>
        /// Indicates whether the individual globalization property changes
        /// are to be ignored. If set to True, the individual values will
        /// be ignored and the locale default will be used.
        /// </summary>
        [
        Browsable(true),
		DefaultValue(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Category("Appearance"),
        Description("Specify if the individual globalization property changes are to be ignored."),
        RefreshProperties(RefreshProperties.Repaint),
        ]
        public bool UseLocaleDefault
        {
            get
            {
                return this.useLocaleDefault;
            }

            set
            {
                this.useLocaleDefault = value;
                if (this.Initializing == false)
                    this.Mask = this.maskDisplay;
            }
        }


        /// <summary>
        /// Gets / sets the character to use when a decimal separator position is specified.
        /// </summary>
        /// <remarks>
        /// This value is initially set from the <see cref="System.Globalization.NumberFormatInfo"/> 
        /// and can be changed based on your requirements or based on the locale.
        /// </remarks>
        [
        Browsable(true),
        Category("Appearance"),
        Description("The character to use when a decimal seprator position is specified.")
        ]
        public char DecimalSeparator
        {
            get
            {
                string ret = this.NumberFormatInfoObject.NumberDecimalSeparator;
                return Convert.ToChar(ret);
            }

            set
            {
                if (value != '\0')
                {
                    this.NumberFormatInfoObject.NumberDecimalSeparator = Convert.ToString(value);

                    if (this.Initializing == false)
                        this.Mask = this.maskDisplay;
                }
            }
        }

        /// <summary>
        /// Indicates whether the DecimalSeparator should not be serialized if
        /// the UseLocaleDefault property is set.
        /// </summary>
        /// <returns>True if the </returns>
        private bool ShouldSerializeDecimalSeparator()
        {
            if (this.Culture.NumberFormat.NumberDecimalSeparator != Convert.ToString(this.DecimalSeparator))
                return true;
            else
                return false;

        }

        /// <summary>
        /// Resets the value to the culture specific value.
        /// </summary>
        private void ResetDecimalSeparator()
        {
            this.DecimalSeparator = Convert.ToChar(this.Culture.NumberFormat.NumberDecimalSeparator);
        }

        /// <summary>
        /// Gets / sets the <see cref="System.Globalization.NumberFormatInfo"/> provides the 
        /// necessary globalization information for the properties that rely on these
        /// settings.
        /// </summary>
        /// <remarks>
        /// This value is initially set from the <see cref="System.Globalization.NumberFormatInfo.CurrentInfo"/> 
        /// and can be changed based on your requirements.
        /// </remarks>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Category("Appearance"),
        Description("The NumberFormatInfo object that will be used for formatting the number related symbols.")
        ]
        public NumberFormatInfo NumberFormatInfoObject
        {
            get
            {
                if (this.numberFormatInfoObject == null)
                {
                    this.numberFormatInfoObject = new NumberFormatInfo();
                    this.numberFormatInfoObject.NumberDecimalSeparator = this.Culture.NumberFormat.NumberDecimalSeparator;
                    this.numberFormatInfoObject.NumberGroupSeparator = this.Culture.NumberFormat.NumberGroupSeparator;
                    this.numberFormatInfoObject.NumberGroupSizes = this.Culture.NumberFormat.NumberGroupSizes;
                }

                return this.numberFormatInfoObject;
            }

            set
            {
                this.numberFormatInfoObject = new NumberFormatInfo();
                NumberFormatInfo localObject = new NumberFormatInfo();

                if (value != null)
                    localObject = value;
                else
                {
                    localObject = this.Culture.NumberFormat;
                }

                this.numberFormatInfoObject.NumberDecimalSeparator = localObject.NumberDecimalSeparator;
                this.numberFormatInfoObject.NumberGroupSeparator = localObject.NumberGroupSeparator;
                this.numberFormatInfoObject.NumberGroupSizes = localObject.NumberGroupSizes;

                if (this.Initializing == false)
                    this.Mask = this.maskDisplay;
            }
        }

        /// <summary>
        /// Gets / sets the <see cref="System.Globalization.DateTimeFormatInfo"/> provides the 
        /// necessary globalization information for the properties that rely on the datetime
        /// settings.
        /// </summary>
        /// <remarks>
        /// This value is initially set from the <see cref="System.Globalization.DateTimeFormatInfo.CurrentInfo"/> 
        /// and can be changed based on your requirements.
        /// </remarks>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Category("Appearance"),
        Description("The DateTimeFormatInfo object that will be used for formatting the date related symbols.")
        ]
        public DateTimeFormatInfo DateTimeFormatInfoObject
        {
            get
            {
                if (this.dateTimeFormatInfoObject == null || this.dateTimeFormatInfoObject.IsReadOnly == true)
                {
                    this.dateTimeFormatInfoObject = new DateTimeFormatInfo();
                    this.dateTimeFormatInfoObject.DateSeparator = this.Culture.DateTimeFormat.DateSeparator;
                    this.dateTimeFormatInfoObject.TimeSeparator = this.Culture.DateTimeFormat.TimeSeparator;
                }

                return this.dateTimeFormatInfoObject;
            }

            set
            {
                this.dateTimeFormatInfoObject = new DateTimeFormatInfo();
                DateTimeFormatInfo localObject = new DateTimeFormatInfo();

                if (value != null)
                    localObject = value;
                else
                {
                    localObject = this.Culture.DateTimeFormat;
                }

                this.dateTimeFormatInfoObject.DateSeparator = localObject.DateSeparator;
                this.dateTimeFormatInfoObject.TimeSeparator = localObject.TimeSeparator;

                if (this.Initializing == false)
                    this.Mask = this.maskDisplay;
            }
        }

        /// <summary>
        /// Indicates whether the prompt character can be allowed to be entered as an 
        /// input character.
        /// </summary>
        /// <remarks>
        /// Set the value to True if the prompt character can be entered by user. 
        /// </remarks>
        [
        Browsable(true),
        Category("Behavior"),
        Description("This property specifies if the prompt character can be allowed to be entered as a input character."),
        DefaultValue(false)
        ]
        public bool AllowPrompt
        {
            get
            {
                return this.allowPrompt;
            }
            set
            {
                this.allowPrompt = value;
            }
        }


        /// <summary>
        /// If the mask string is empty, we do not apply any rules.
        /// This helper function checks whether the mask is active.
        /// </summary>
        /// <returns>True if the mask is active; False otherwise.</returns>
        protected bool IsMaskActive()
        {
            if (this.maskString == null || this.maskString.Length < 1)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Indicates whether the Undo operation is possible at this time.
        /// </summary>
        /// <remarks>
        /// The MaskedEditBox maintains its own undo mechanism and hence
        /// needs to implement this method to be compatible with the Undo
        /// mechanism.
        /// </remarks>
        [
        Browsable(false)
        ]
        public new bool CanUndo
        {
            get
            {
                if (this.IsMaskActive() == false)
                    return base.CanUndo;
                else if (this.undoBufferText == this.DisplayString)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Gets / sets the Maximum Value that can be set through the MaskedEditBox.
        /// </summary>
        /// <remarks>
        /// This value is enforced only if the <see cref="UsageMode"/> property
        /// is set to <see cref="MaskedUsageMode.Numeric"/>. The mask string 
        /// should be set to contain numeric characters. All literal characters
        /// will be ignored while computing the value of the contents of the
        /// MaskedEditBox. The default value is set to <see cref="Decimal.MaxValue"/>.
        /// </remarks>
        [
        Category("Behavior"), Description("Maximum Value that can be set through the MaskedEditBox.")
        ]
        public decimal MaxValue
        {
            get
            {
                return this.maxValue;
            }

            set
            {
                this.maxValue = value;
            }
        }

        /// <summary>
        /// Indicates whether the MaxValue property should be serialized.
        /// </summary>
        /// <returns>True if the value is not equal to <see cref="decimal.MaxValue"/>.</returns>
        private bool ShouldSerializeMaxValue()
        {
            if (this.maxValue != decimal.MaxValue)
                return true;
            else
                return false;

        }

        /// <summary>
        /// Resets the value to the default.
        /// </summary>
        private void ResetMaxValue()
        {
            this.MaxValue = decimal.MaxValue;
        }


        public new Color BackColor
        {
            get
            {
                return base.BackColor;
            }

            set
            {
                base.BackColor = value;
            }
        }

        private bool ShouldSerializeBackColor()
        {
            if (this.BackColor == SystemColors.Window)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Resets BackColor of the control to its default value.
        /// </summary>
        public override void ResetBackColor()
        {
            this.BackColor = SystemColors.Window;
        }

        /// <summary>
        /// Gets / sets the Minimum Value that can be set through the MaskedEditBox.
        /// </summary>
        /// <remarks>
        /// This value is enforced only if the <see cref="UsageMode"/> property
        /// is set to <see cref="MaskedUsageMode.Numeric"/>. The mask string 
        /// should be set to contain numeric characters. All literal characters
        /// will be ignored while computing the value of the contents of the
        /// MaskedEditBox. The default value is set to <see cref="Decimal.MaxValue"/>.
        /// </remarks>
        [
        Category("Behavior"), Description("Minimum Value that can be set through the MaskedEditBox.")
        ]
        public decimal MinValue
        {
            get
            {
                return this.minValue;
            }

            set
            {
                if (value > 0)
                    this.minValue = value;
            }
        }


        /// <summary>
        /// Indicates whether the MinValue property should be serialized.
        /// </summary>
        /// <returns>True if the value is not equal to <see cref="decimal.MaxValue"/>.</returns>
        private bool ShouldSerializeMinValue()
        {
            if (this.minValue != 0m)
                return true;
            else
                return false;

        }

        /// <summary>
        /// Resets the value to the default.
        /// </summary>
        private void ResetMinValue()
        {
            this.MinValue = 0m;
        }

        /// <summary>
        /// Resets the MaskedEditBox control to its initial position.
        /// </summary>
        /// <remarks>
        /// Invoke this method when you need to reinitialize the MaskededitBox.
        /// The Mask property will not be lost because of calling this method.
        /// </remarks>
        public void Reset()
        {
            string dispString = String.Empty;
            this.ApplyMaskLogic(this.maskString, ref dispString);
            this.DisplayString = dispString;

            this.RefreshPromptCharacter();

            this.MoveCursorPosition(-1, 1, false);
        }

        /// <summary>
        /// Refreshes the display based on the current display
        /// characteristics. 
        /// </summary>
        /// <remarks>
        /// Changes made to the Mask property or any other property that affects the
        /// display of the formatted string will be picked up by invoking this method.
        /// </remarks>
        public void RefreshDisplay()
        {
            this.RefreshDisplay(this.DisplayString, true);
        }

        protected void RefreshDisplay(string dispString, bool updateUndo)
        {
            if (this.Mask != String.Empty)
            {
                SetBaseText(dispString);
                if (updateUndo)
                {
                    this.undoBufferText = this.redoBufferText;
                    this.redoBufferText = dispString;
                }
            }
        }


        /// <summary>
        /// Returns the valid mapped position.
        /// </summary>
        /// <param name="positionInFullString">The index in the full string.</param>
        /// <returns>The valid mapped position.</returns>
        private int GetValidMappedPosition(int positionInFullString)
        {
            int validPosition = positionInFullString;
            int index = 0;
            bool matchFound = false;
            int spacesIgnored = 0;

            if (validPosition != 0)
            {

                if (this.listDisplayToInternalString.Count > 0)
                {
                    if (positionInFullString >= this.listDisplayToInternalString.Count)
                    {
                        positionInFullString = this.listDisplayToInternalString.Count - 1;
                        spacesIgnored++;
                    }

                    index = positionInFullString;

                    while (index >= 0 && matchFound == false)
                    {
                        int mappedValue = 0;

                        mappedValue = (int)this.listDisplayToInternalString.GetByIndex(index);
                        if (mappedValue != -1)
                        {
                            validPosition = mappedValue;
                            matchFound = true;
                        }
                        else
                        {
                            index--;
                            spacesIgnored++;
                        }
                    }

                    if (matchFound == false)
                        validPosition = 0;
                }

                if (spacesIgnored > 0)
                    validPosition++;
            }

            return validPosition;
        }

        /// <summary>
        /// Gets / sets the adjusted SelectionStart when the PromptCharacter or PassivePromptCharacter
        /// is NULL.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        private int AdjustedSelectionStart
        {
            get
            {
                if (this.listDisplayToInternalString.Count == 0)
                    return this.SelectionStart;
                else
                    return this.adjustedSelectionStart;
            }

            set
            {
                if (this.listDisplayToInternalString.Count > 0)
                    this.adjustedSelectionStart = value;

                this.SelectionStart = value;
            }
        }


        /// <summary>
        /// Gets / sets the adjusted SelectionLength when the PromptCharacter or PassivePromptCharacter
        /// is NULL.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        private int AdjustedSelectionLength
        {
            get
            {
                if (this.listDisplayToInternalString.Count == 0)
                    return this.SelectionLength;
                else
                    return this.adjustedSelectionLength;
            }

            set
            {
                if (this.listDisplayToInternalString.Count > 0)
                    this.adjustedSelectionLength = value;
                this.SelectionLength = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control use sequentially display mask's characters.
        /// </summary>
        [
        Category("Behavior"),
        DefaultValue(false),
        Description("Indicating whether the control use sequentially display mask's characters.")
        ]
        public bool Sequentially
        {
            get
            {
                return m_bSequentially;
            }
            set
            {
                if (value != m_bSequentially)
                {
                    m_bSequentially = value;
                }
            }
        }

        /// <summary>
        /// Handles character deletion as MS MaskedTextBox.
        /// </summary>
        /// <param name="currentDisplayText"></param>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <param name="insertMode"></param>
        /// <param name="ignorePromptCharacters"></param>
        /// <param name="backSpace"></param>
        /// <returns></returns>
        private bool HandleCharDelete(string currentDisplayText, int startPosition, int endPosition, bool insertMode, bool ignorePromptCharacters,bool backSpace)
        {
            char localPromptChar = this.GetCurrentPromptCharacter();
            int maskLength = this.maskString.Length;
            int maskPosition = 0;
            char chData;

            if (!backSpace)
            {
                if (startPosition >= 0 && DisplayString[startPosition] == localPromptChar)
                {
                    // If the current position is unfilled, move to a filled position.
                    if (currentDisplayText[startPosition] == localPromptChar)
                    {
                        int emptySpaces = 0;
                        int oldStartPosition = startPosition;

                        while (startPosition != -1 && startPosition < this.maskString.Length && DisplayString[startPosition] == localPromptChar)
                        {
                            emptySpaces++;
                            oldStartPosition = startPosition;
                            startPosition = this.GetNextDataPos(startPosition);

                            if (startPosition == -1)
                            {
                                startPosition = oldStartPosition;
                                emptySpaces--;
                                break;
                            }
                        }

                        if (emptySpaces > 0)
                            endPosition = endPosition + emptySpaces;
                    }
                }

                //startPosition = GetNextDataPos(startPosition);
            }
            else
            {
                
            }
            int newStringPosition = startPosition;
            string resultString = string.Empty;

            if (startPosition == -1)
                return true;

            if (startPosition == endPosition && !backSpace)
                startPosition = GetNextDataPos(startPosition);

            string newString = string.Empty;

            if (this.SelectionLength > 0)
            {
                newString = RemoveText(currentDisplayText, endPosition, startPosition);
            }
            else
                newString = RemoveText(currentDisplayText, startPosition, endPosition);

             if (IsLiteralCharacter(this.maskString[maskPosition]))
                maskPosition = GetNextDataPos(maskPosition);

            if (maskPosition >= maskLength || maskPosition == -1)
                return true;

            for (newStringPosition = 0; newStringPosition < newString.Length; newStringPosition++)
            {
                if (maskPosition >= maskLength || maskPosition == -1)
                    break;

                if (!IsLiteralCharacter(this.maskString[maskPosition]))
                {
                    chData = newString[newStringPosition];

                    if (IsCharValid(newStringPosition, chData))
                    {
                        continue;
                    }
                    else if (chData == this.maskString[newStringPosition] && IsLiteralCharacter(chData))
                    {
                        continue;
                    }
                    else if (chData == GetCurrentPromptCharacter())
                    {
                        continue;
                    }
                    else
                        return true;
                }
            }

            this.SetExternalText(newString, 0,false,false);
            this.SetSelection(startPosition - 1 ,0);
            return true;
        }

        /// <summary>
        /// Returns the adjusted position when there is a NULL prompt.
        /// </summary>
        /// <param name="currentSelectionPos">The current position.</param>
        /// <returns>The adjusted position.</returns>
        private int GetAdjustedPositionForNullPrompt(int currentSelectionPos)
        {
            int returnSelectionPos = -1;
            int spacesAhead = 0;
            bool matchFound = false;

            if (currentSelectionPos == 0)
                return 0;

            if (this.listDisplayToInternalString.ContainsValue(currentSelectionPos))
                returnSelectionPos = this.listDisplayToInternalString.IndexOfValue(currentSelectionPos);
            else
            {
                returnSelectionPos = currentSelectionPos;
                // The value is not found.
                while (returnSelectionPos >= 0)
                {
                    returnSelectionPos--;
                    spacesAhead++;

                    if (this.listDisplayToInternalString.ContainsValue(returnSelectionPos))
                    {
                        returnSelectionPos = this.listDisplayToInternalString.IndexOfValue(returnSelectionPos);
                        matchFound = true;
                        break;
                    }
                }

                // This case shouldn't happen.
                if (matchFound == false)
                {
                    spacesAhead = 0;
                    returnSelectionPos = currentSelectionPos;
                }
            }
            return Math.Max(returnSelectionPos + spacesAhead, 0);
        }

        /// <summary>
        /// Indicates whether the in parameter is a literal character.
        /// </summary>
        /// <param name="charIn">The character to check.</param>
        /// <returns>True if the input character is a mask character.</returns>
        /// <remarks>
        /// All characters that are accepted as input are subjected to
        /// this check.
        /// </remarks>
        protected bool IsLiteralCharacter(char charIn)
        {
            return (charIn < MaskedEditBox.maskOffset);
        }

        /// <summary>
        /// Looks up the mask character in the mask list and returns the
        /// index so that the masking logic knows what to do with it.
        /// </summary>
        /// <param name="charIn">The character to be identified.</param>
        /// <returns>The index to the mask in the MaskedEditBox.maskList collection.</returns>
        /// <remarks>
        /// This is a helper method that takes a character and looks up the character
        /// in the list of recognized mask characters and returns the result.
        /// </remarks>
        protected int GetMaskCharValue(char charIn)
        {
            int maskCharValue = -1;

            if (MaskedEditBox.maskList.Contains(charIn))
                maskCharValue = MaskedEditBox.maskList.IndexOf(charIn);

            return maskCharValue;
        }

        /// <summary>
        /// Overrides the <see cref="Control.OnEnter"/> method.
        /// </summary>
        /// <param name="arg">The event data.</param>
        /// <remarks>
        /// This method is overriden in order to refresh the display between the passive
        /// and active prompt characters.
        /// </remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void OnEnter(EventArgs arg)
        {
            base.OnEnter(arg);

            if (this.maskString.Length > 0 && this.DesignMode == false)
            {
                this.hasFocus = true;
                this.internalTextChange = true;
                this.SetMaskedEditState(MaskedEditState.EditState);

                this.RefreshPromptCharacter();

                switch (this.PositionAt)
                {
                    case SpecialCursorPosition.Decimal:
                        PositionNearDecimal();
                        break;
                    case SpecialCursorPosition.FirstMaskPosition:
                        PositionAtFirstMaskPosition();
                        break;
                }
                this.SetMaskedEditState(MaskedEditState.NormalState);
                this.internalTextChange = false;
            }

        }

        /// <summary>
        /// Overrides the OnClick method.
        /// </summary>
        /// <remarks>
        /// Position the cursor when the user clicks inside the control.
        /// </remarks>
        /// <param name="arg">The event data.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void OnClick(EventArgs arg)
        {
            base.OnClick(arg);
            if (this.maskString.Length > 0 && this.DesignMode == false)
            {
                if (focusClick == true)
                {
                    switch (this.PositionAt)
                    {
                        case SpecialCursorPosition.Decimal:
                            PositionNearDecimal();
                            break;
                        case SpecialCursorPosition.FirstMaskPosition:
                            PositionAtFirstMaskPosition();
                            break;
                        case SpecialCursorPosition.FirstPosition:
                            PositionAtFirstPos();
                            break;
                    }
                    focusClick = false;
                }
            }
        }

        /// <summary>
        /// Overrides the <see cref="System.Windows.Forms.Control.OnLostFocus"/> method.
        /// </summary>
        /// <param name="arg">The event data.</param>
        /// <remarks>
        /// This method is overriden in order to refresh the display between the passive
        /// and active prompt characters.
        /// </remarks>
        protected override void OnLeave(EventArgs arg)
        {
            base.OnLeave(arg);
            if (this.maskString.Length > 0 && this.DesignMode == false)
            {
                this.internalTextChange = true;
                this.hasFocus = false;
                m_bIsTextByInputChanged = Modified;
                this.SetMaskedEditState(MaskedEditState.EditState);
                ApplyAlignmentToGroups();
                m_bIsTextByInputChanged = Modified;
                this.SetMaskedEditState(MaskedEditState.NormalState);
                this.RefreshPromptCharacter();
                focusClick = true;//Since the control is losing focus we can take the next click to be a focus click
                this.internalTextChange = false;
            }
        }

        /// <summary>
        /// Applies RightToLeft based on the current culture.
        /// </summary>
        protected void ApplyRightToLeft()
        {
            if (this.rightToLeftCultures.Contains(this.Culture.Parent.Name))
            {
                if (this.RightToLeft != RightToLeft.Yes)
                    this.RightToLeft = RightToLeft.Yes;
            }
            else
            {
                if (this.RightToLeft != RightToLeft.No)
                    this.RightToLeft = RightToLeft.No;
            }
        }

        /// <summary>
        /// Extracts the mask values from the mask string that was input by the user and initializes
        /// the internal variables that will be used in applying the mask logic.
        /// </summary>
        /// <param name="maskIn">The mask string input by the user.</param>
        /// <returns>The extracted mask string.</returns>]
        /// <remarks>
        /// The mask string is split up into three different types of
        /// characters for applying the masking logic - they are literals, special
        /// masks (&gt; and &lt;) and other masks.
        /// <para>
        /// &gt; and &lt; are treated as special masks because they do not occupy a
        /// position by themselves but dictate the formatting of other adjoining characters.
        /// </para>
        /// </remarks>
        protected string ExtractMaskValues(string maskIn)
        {
            string extractedMask = "";
            MaskCharTypes maskCharType = 0;
            int maskValue = 0;
            char maskChar = ' ';
            bool escapeActive = false;

            this.specialMasks.Clear();
            int cancelledSpecialMasks = 0;
            int escapeMasks = 0;

            if (maskIn != null)
            {
                int maskLength = maskIn.Length;
                int index = 0;

                for (index = 0; index < maskLength; index++)
                {
                    maskChar = maskIn[index];
                    maskValue = this.GetMaskCharValue(maskChar);
                    if (maskValue >= 0)
                    {
                        if (escapeActive == true)
                        {
                            extractedMask += maskChar;
                            escapeActive = false;
                            escapeMasks++;
                        }
                        else
                        {
                            maskCharType = (MaskCharTypes)maskValue;
                            if (maskCharType == MaskCharTypes.maskCharEscape)
                            {
                                escapeActive = true;
                            }
                            else if ((maskCharType == MaskCharTypes.maskCharLowercase) || (maskCharType == MaskCharTypes.maskCharUppercase))
                            {
                                int maskIndex = Math.Max(0, index - this.specialMasks.Count - cancelledSpecialMasks - escapeMasks);
                                if (this.specialMasks.ContainsKey(maskIndex))
                                {
                                    this.specialMasks.Remove(maskIndex);
                                    cancelledSpecialMasks++;
                                }
                                this.specialMasks.Add(maskIndex, maskChar);
                            }
                            else if (maskCharType == MaskCharTypes.maskCharDateSep ||
                                maskCharType == MaskCharTypes.maskCharDecimal ||
                                maskCharType == MaskCharTypes.maskCharThousands ||
                                maskCharType == MaskCharTypes.maskCharTimeSep)
                            {
                                char currentChar = this.GetLocalizedValue(maskCharType);
                                extractedMask += currentChar;
                            }
                            else
                                extractedMask += (char)(maskChar + MaskedEditBox.maskOffset);
                        }
                    }
                    else
                    {
                        extractedMask += maskChar;
                        escapeActive = false;
                    }
                }
            }

            // Return the extracted mask.
            return extractedMask;
        }
       
        /// <summary>
        /// Refreshes the display with the current prompt character. 
        /// </summary>
        /// <remarks>
        /// This method changes the display based on the new prompt character by replacing
        /// the old prompt character with the new prompt character.
        /// </remarks>
        protected void RefreshPromptCharacter()
        {
            string currentString = this.DisplayString;
            int deletedPlacesCount = 0;//Applies only when newPrompt is '\0'

            char localNewPromptChar = this.GetCurrentPromptCharacter();

            try
            {
                if (this.maskString != null && this.maskString != String.Empty)
                {
                    if (this.lastUsedPromptChar != localNewPromptChar)
                    {
                        int index = 0;
                        for (index = 0; index < this.maskString.Length; index++)
                        {
                            // Mask characters
                            if (IsLiteralCharacter(this.maskString[index]) == false)
                            {
                                if (this.IsMaskPositionFilled(currentString, index - deletedPlacesCount) == false)
                                {
                                    StringBuilder builder = new StringBuilder();
                                    builder.Append(currentString);
                                    builder[index] = localNewPromptChar;
                                    currentString = builder.ToString();
                                }
                            }
                            else// Literal characters
                            {
                                StringBuilder builder1 = new StringBuilder();
                                builder1.Append(currentString);
                                builder1[index - deletedPlacesCount] = this.maskString[index];
                                currentString = builder1.ToString();
                            }
                        }
                    }
                    this.lastUsedPromptChar = localNewPromptChar;
                    this.DisplayString = currentString;
                    this.RefreshDisplay(this.DisplayString, false);
                }
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// Applies the masking rules and prepares the display string 
        /// based on the mask string.
        /// </summary>
        /// <param name="maskStringIn">The mask string.</param>
        /// <param name="displayString">The display string.</param>
        /// <remarks>
        /// You will not need to call this method directly. It is invoked by
        /// by methods that need to refresh the display.
        /// </remarks>
        protected void ApplyMaskLogic(string maskStringIn, ref string displayString)
        {
            int index = 0;
            int maskLength = 0;
            char currentChar = '\0';
            char localPromptChar = '\0';

            displayString = null;

            if (maskStringIn != null)
            {
                maskLength = maskStringIn.Length;
                for (index = 0; index < maskLength; index++)
                {
                    currentChar = maskStringIn[index];
                    if (IsLiteralCharacter(currentChar))
                        displayString += currentChar;
                    else
                    {
                        if (this.hasFocus == true)
                            localPromptChar = this.GetPromptCharacter();
                        else
                            localPromptChar = this.GetPassivePromptCharacter();

                        if (localPromptChar != '\0')
                            displayString += localPromptChar;
                    }
                }
            }
        }

        /// <summary>
        /// Moves the cursor position depending on the action.
        /// </summary>
        /// <param name="currentPosition">The current position of the cursor.</param>
        /// <param name="direction">The direction which the cursor needs to be moved.</param>
        /// <param name="selected">Boolean value specifying if there is selected text.</param>
        /// <returns>The position of the cursor after setting it.</returns>
        /// <remarks>
        /// The MaskedEditBox control automatically adjusts its cursor position when
        /// there is a change in the content of the text box.
        /// </remarks>
        protected int MoveCursorPosition(int currentPosition, int direction, bool selected)
        {
            int selectIncrement = 0;
            int newPosition = 0;
            int maskLength = 0;

            // The cursor position is moved by one position if text is currently selected.
            if (selected)
                selectIncrement = 1;

            // We do nothing if the mask is inactive.
            if (this.IsMaskActive() == false)
            {
                this.SetSelection(currentPosition, 0);
                return currentPosition;
            }


            maskLength = this.maskString.Length;

            // We will fix the new position to be the current position.
            newPosition = currentPosition;

            // Based on the direction we will have to position the cursor.
            switch (direction)
            {
                // Move forward.
                case 1:
                    {
                        newPosition = GetNextDataPos(currentPosition);
                        if (newPosition == -1)
                        {
                            if (currentPosition != -1)
                            {
                                if (IsLiteralCharacter(this.maskString[currentPosition]))
                                    this.SetSelection(currentPosition, selectIncrement);
                                else
                                    this.SetSelection(currentPosition + 1, selectIncrement);
                            }
                            else
                                this.SetSelection(0, 0);
                        }
                        else
                            this.SetSelection(newPosition, selectIncrement);
                        break;
                    }

                // Move backward.
                case -1:
                    {
                        newPosition = GetPrevDataPos(currentPosition, true);//Get the unfilled position

                        //if the new position is > 0
                        if (newPosition >= 0)
                            this.SetSelection(newPosition, selectIncrement);
                        break;
                    }
            }

            // Return a valid value for the position or return -1.
            if (newPosition < 0 || newPosition >= maskLength)
                return -1;
            else
                return newPosition;
        }

        /// <summary>
        /// This method is a helper method for getting the next available data entry
        /// position.
        /// </summary>
        /// <param name="currentPosition">The current position. Checks from here onward.</param>
        /// <returns>Returns the next available position.</returns>
        protected int GetNextDataPos(int currentPosition)
        {
            int newPosition = currentPosition;
            int maskLength = this.maskString.Length;

            if (maskLength < 1)
                return currentPosition;

            if (newPosition < maskLength)
            {
                newPosition++;
                while (newPosition < maskLength)
                {
                    if (IsLiteralCharacter(this.maskString[newPosition]))
                        newPosition++;
                    else
                        break;
                }

                if (newPosition >= maskLength)
                    newPosition = -1;
            }

            return newPosition;
        }

        /// <summary>
        /// Returns the index of the first empty position before the current position. 
        /// </summary>
        /// <returns>Index of the first empty position.</returns>
        private int GetFirstPrevDataPos(string displayText, int currentPosition)
        {
            char localPromptChar = '\0';
            int lastValidPosition = currentPosition;

            if (this.maskString == null)
                return lastValidPosition;

            int newPosition = currentPosition;

            if (this.maskString.Length < 1)
                return lastValidPosition;

            localPromptChar = this.GetCurrentPromptCharacter();

            newPosition--;

            if (newPosition >= this.maskString.Length)
                newPosition = this.maskString.Length - 1;

            while (newPosition >= 0)
            {
                if (IsLiteralCharacter(this.maskString[newPosition]))
                {
                    newPosition++;
                    lastValidPosition = newPosition;
                    break;
                }
                else
                {
                    if (displayText[newPosition] == localPromptChar)
                    {
                        // empty position
                        lastValidPosition = newPosition;
                        newPosition--;
                    }
                    else
                    {
                        // Non empty position
                        break;
                    }
                }
            }

            return lastValidPosition;
        }

        /// <summary>
        /// This method is a helper method for getting the next available data entry position(backward).
        /// </summary>
        /// <param name="currentPosition">The current position. Checks from here backward.</param>
        /// <returns>The next available position(backward).</returns>
        protected int GetPrevDataPos(int currentPosition)
        {
            return GetPrevDataPos(currentPosition, false);//No need to move to a unfilled position.
        }

        /// <summary>
        /// Returns the previous data entry position.
        /// </summary>
        /// <param name="currentPosition">The current position.</param>
        /// <param name="bUnfilled">Indicates whether the new position has to be unfilled.</param>
        /// <returns>The new position.</returns>
        protected int GetPrevDataPos(int currentPosition, bool bUnfilled)
        {
            char localPromptChar = '\0';
            int promptCharInt = 0;

            if (this.maskString == null)
                return currentPosition;

            int newPosition = currentPosition;

            if (this.maskString.Length < 1)
                return currentPosition;

            if (this.hasFocus)
            {
                localPromptChar = this.GetPromptCharacter();
                promptCharInt = this.promptCharacterInt;
            }
            else
            {
                localPromptChar = this.GetPassivePromptCharacter();
                promptCharInt = this.passivePromptCharacterInt;
            }

            newPosition--;
            while (newPosition >= 0)
            {
                if (IsLiteralCharacter(this.maskString[newPosition]))
                    newPosition--;
                else
                {
                    if (bUnfilled == true)
                    {
                        if (promptCharInt == 0)
                            break;
                        else if (this.DisplayString.Length > 0 && this.DisplayString[newPosition] == this.GetPromptCharacter())
                            break;
                        else
                            newPosition--;
                    }
                    else
                        break;
                }
            }

            return newPosition;
        }


        /// <summary>
        /// Sets the cursor and also selects text for the specified
        /// length based on the parameters passed in.
        /// </summary>
        /// <param name="startPosition">The beginning position for the selection.</param>
        /// <param name="selectionLength">The length of the selection.</param>
        protected void SetSelection(int startPosition, int selectionLength)
        {
            this.AdjustedSelectionStart = Math.Max(0, startPosition);
            this.AdjustedSelectionLength = Math.Max(0, selectionLength);
        }

        /// <summary>
        /// Returns the current PromptCharacter.
        /// </summary>
        /// <returns>The current prompt character.</returns>
        private char GetCurrentPromptCharacter()
        {
            if (this.hasFocus == true)
                return this.GetPromptCharacter();
            else
                return this.GetPassivePromptCharacter();
        }

        /// <summary>
        /// Inserts a new character into the displayed text.
        /// </summary>
        /// <param name="startPosition">The start position to insert the character.</param>
        /// <param name="newChar">The new character to be inserted.</param>
        /// <param name="updateDisplay">Indicates whether the display is to be updated.</param>
        /// <returns>The changed text.</returns>
        protected string InsertChar(int startPosition, char newChar, bool updateDisplay)
        {
            return this.InsertChar(this.DisplayString, startPosition, newChar, updateDisplay);
        }

        /// <summary>
        /// Inserts a new character into the displayed text.
        /// </summary>
        /// <param name="currentDisplayText">The current display text.</param>
        /// <param name="startPosition">The start position to insert the character.</param>
        /// <param name="newChar">The new character to be inserted.</param>
        /// <param name="updateDisplay">Indicates whether the display is to be updated.</param>
        /// <returns>The changed text.</returns>
        protected string InsertChar(string currentDisplayText, int startPosition, char newChar, bool updateDisplay)
        {
            int lastDataPos;
            int index;
            StringBuilder builder = null;
            char localPromptCharacter = GetCurrentPromptCharacter();

            if (startPosition < 0 || startPosition > this.maskString.Length)
            {
                return currentDisplayText;
            }

            //Get the index of the last data position.
            lastDataPos = GetPrevDataPos(this.maskString.Length);

            index = lastDataPos;
            while (index >= 0)
            {
                if (localPromptCharacter != '\0' && this.DisplayString[index] == localPromptCharacter)
                    index = GetPrevDataPos(index);
                else if (localPromptCharacter == '\0')
                {
                    break;
                }
                else
                    break;
            }

            if (index == lastDataPos)
            {
                if (this.DisplayString[startPosition] == localPromptCharacter)
                {
                    builder = new StringBuilder();
                    builder.Append(currentDisplayText);
                    builder[startPosition] = newChar;
                    currentDisplayText = builder.ToString();
                }
                return currentDisplayText;
            }
            else
            {
                while (index >= startPosition)
                {
                    builder = new StringBuilder();
                    builder.Append(currentDisplayText);
                    builder[GetNextDataPos(index)] = currentDisplayText[index];
                    currentDisplayText = builder.ToString();

                    //Move index to the previous data position.                                
                    index = GetPrevDataPos(index);
                }

                builder = new StringBuilder();
                builder.Append(currentDisplayText);
                //Set the input char in the starting position.
                char chMask;
                chMask = this.maskString[startPosition];
                int maskValue = this.GetMaskCharValue((char)(chMask - MaskedEditBox.maskOffset));
                MaskCharTypes maskType = (MaskCharTypes)maskValue;
                

                if (maskType == MaskCharTypes.maskCharDigitRequired && newChar == (char)Keys.Space)
                {
                    builder[startPosition] = this.GetPromptCharacter();
                    char chCurrent = builder[startPosition];
                    char chEnd = builder[this.maskDisplay.Length - 1];

                    if (chCurrent != this.GetCurrentPromptCharacter() && chEnd == this.GetCurrentPromptCharacter())
                    {
                        builder.Insert(0, this.GetCurrentPromptCharacter());
                        builder.Remove(builder.Length - 1, 1);
                    }
                }
                else
                builder[startPosition] = newChar;
                currentDisplayText = builder.ToString();

                this.SetCustomFilledPosition(newChar, startPosition);

                if (updateDisplay)
                {
                    this.DisplayString = currentDisplayText;
                    //Refresh the display.
                    RefreshDisplay(this.DisplayString, false);

                    //Set the selection foward one data position.
                    int nextPosition = GetNextDataPos(startPosition);
                    if (nextPosition != -1)
                        this.SetSelection(nextPosition, 0);
                    else
                        this.SetSelection(startPosition + 1, 0);
                }

                return currentDisplayText;
            }
        }

        protected void SetCustomFilledPosition(char newChar, int index)
        {
            if (this.customFilledPositions.ContainsKey(index))
                this.customFilledPositions.Remove(index);

            // Check if the added character is a PromptCharacter.
            if (this.AllowPrompt == true && newChar <= maskOffset && (newChar == this.GetPromptCharacter() || newChar == this.GetPassivePromptCharacter()))
            {
                this.customFilledPositions.Add(index, newChar);
            }
        }

        /// <summary>
        /// Applies the special masks to the current character - this is for the 
        /// upper case and lower case masks that are not part of the mask string.
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="currentChar"></param>
        /// <returns></returns>
        protected char ApplySpecialMasks(int currentPosition, char currentChar)
        {
            // Account for < and > masks.
            for (int index = currentPosition; index >= 0; index--)
            {
                if (this.specialMasks.ContainsKey(index) == true)
                {
                    int maskValue = this.GetMaskCharValue((char)this.specialMasks[index]);
                    MaskCharTypes maskCharType = (MaskCharTypes)maskValue;
                    if (maskCharType == MaskCharTypes.maskCharLowercase)
                        this.casingNormalize = CasingNormalize.changeToLowerOnly;
                    else if (maskCharType == MaskCharTypes.maskCharUppercase)
                        this.casingNormalize = CasingNormalize.changeToUpperOnly;

                    break;
                }
            }

            if (this.casingNormalize != CasingNormalize.changeToBoth)
                currentChar = this.ApplyCasing(currentChar, casingNormalize);

            return currentChar;
        }

        /// <summary>
        /// This method is invoked when the decimal key is pressed.
        /// </summary>
        /// <returns>True if the key is handled; False otherwise.</returns>
        /// <remarks>
        /// The defined behavior for this key is to jump to the position immediately
        /// after the decimal position.
        /// </remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool HandleDecimalKey()
        {
            int decimalPosition = 0;
            int currentPosition = this.AdjustedSelectionStart;

            decimalPosition = this.DisplayString.IndexOf(this.DecimalSeparator, currentPosition, this.DisplayString.Length - currentPosition);
            if (decimalPosition != -1)
            {
                decimalPosition = this.GetNextDataPos(decimalPosition);
                this.SetSelection(decimalPosition, 0);
            }
            return true;
        }

        /// <summary>
        /// Applies group alignment.
        /// </summary>
        private void ApplyAlignmentToGroups()
        {
            if (this.UsageMode == MaskedUsageMode.Numeric && this.DataGroups.Count == 0 && this.internalDataGroups.Count > 0)
            {
                for (int index = 0; index < this.internalDataGroups.Count; index++)
                    ApplyAlignmentToGroup(this.internalDataGroups, index);
            }
            else
            {
                for (int index = 0; index < this.DataGroups.Count; index++)
                    ApplyAlignmentToGroup(this.DataGroups, index);
                this.RefreshDisplay();
            }
        }


        /// <summary>
        /// Applies the alignment logic and moves characters within the group if possible.
        /// In the case that there is a NULL PromptCharacter, the DisplayString should be
        /// adjusted before invoking this method for aligning. This method will assume that
        /// the number of characters in the MaskString will be equal to the number of 
        /// characters in the DisplayString. SpecialMasks will have to be considered separately
        /// in the case that they exist within a group.
        /// </summary>
        /// <param name="currentDataGroups">The current data groups collection.</param>
        /// <param name="groupIndex">The index of the group.</param>
        private void ApplyAlignmentToGroup(MaskedEditDataGroupInfoCollection currentDataGroups, int groupIndex)
        {
            string groupDisplayText = this.GetDataGroupText(currentDataGroups, groupIndex, this.DisplayString);
            int groupOffset = this.GetDataGroupOffset(currentDataGroups, groupIndex);
            string localDisplayString = this.DisplayString;
            int counter = 0;
            bool matchFound = false;
            string newDisplayString = String.Empty;

            // Save the initial group text.
            string initialGroupText = groupDisplayText;

            if (currentDataGroups[groupIndex].DataGroupAlignment == MaskGroupAlignment.Left)
            {
                for (counter = 0; counter < groupDisplayText.Length; counter++)
                {
                    // Point A.

                    if (this.IsLiteralCharacter(this.maskString[groupOffset + counter]) == false)
                    {
                        if (this.IsMaskPositionFilled(groupDisplayText, counter) == false)
                        {
                            // A mask position is empty.
                            int index = counter + 1;
                            while (matchFound == false && index < groupDisplayText.Length)
                            {
                                if (this.IsLiteralCharacter(this.maskString[groupOffset + index]) == false)
                                {
                                    // Get the character at this place and see if it can
                                    // be moved to the empty position.
                                    char currentChar = groupDisplayText[index];

                                    if (currentChar != LastUsedPromptChar)
                                    {
                                        if (this.IsCharValid(groupOffset + counter, currentChar))
                                        {
                                            // Copy over to previous position.
                                            StringBuilder builder = new StringBuilder();
                                            builder.Append(groupDisplayText);
                                            builder[index] = this.LastUsedPromptChar;
                                            builder[counter] = currentChar;
                                            groupDisplayText = builder.ToString();

                                            this.SetCustomFilledPosition(currentChar, counter);
                                            this.RemoveFromCustomFilledList(index);

                                            break;
                                        }
                                        else
                                            break;//Go to point A.
                                    }
                                }
                                index++;
                            }
                        }
                    }
                }
            }
            else if (currentDataGroups[groupIndex].DataGroupAlignment == MaskGroupAlignment.Right)
            {
                for (counter = groupDisplayText.Length - 1; counter >= 0; counter--)
                {
                    // Point A

                    if (this.IsLiteralCharacter(this.maskString[groupOffset + counter]) == false)
                    {
                        if (this.IsMaskPositionFilled(groupDisplayText, counter) == false)
                        {
                            // A mask position is empty.
                            int index = counter - 1;
                            while (matchFound == false && index >= 0)
                            {
                                if (this.IsLiteralCharacter(this.maskString[groupOffset + index]) == false)
                                {
                                    // Get the character at this place and see if it can
                                    // be moved to the empty position.
                                    char currentChar = groupDisplayText[index];
                                    if (currentChar != LastUsedPromptChar)
                                    {
                                        if (this.IsCharValid(groupOffset + counter, currentChar))
                                        {
                                            // Copy over to previous position.
                                            StringBuilder builder = new StringBuilder();
                                            builder.Append(groupDisplayText);
                                            builder[index] = LastUsedPromptChar;
                                            builder[counter] = currentChar;
                                            groupDisplayText = builder.ToString();

                                            this.SetCustomFilledPosition(currentChar, counter);
                                            this.RemoveFromCustomFilledList(index);

                                            break;
                                        }
                                        else
                                            break;//Go to point A.
                                    }
                                }
                                index--;
                            }
                        }
                    }
                }
            }
            else if (currentDataGroups[groupIndex].DataGroupAlignment == MaskGroupAlignment.Center)
            {
                int totalMaskFields = 0;
                int filledMaskFields = 0;
                int unFilledMaskFields = 0;

                // Align the data to the left within the group first.
                for (counter = 0; counter < groupDisplayText.Length; counter++)
                {
                    // Point A

                    if (this.IsLiteralCharacter(this.maskString[groupOffset + counter]) == false)
                    {
                        totalMaskFields++;
                        filledMaskFields++;

                        if (this.IsMaskPositionFilled(groupDisplayText, counter) == false)
                        {
                            filledMaskFields--;
                            // A mask position is empty.
                            int index = counter + 1;
                            while (matchFound == false && index < groupDisplayText.Length)
                            {
                                if (this.IsLiteralCharacter(this.maskString[groupOffset + index]) == false)
                                {
                                    // Get the character at this place and see if it can
                                    // be moved to the empty position.
                                    char currentChar = groupDisplayText[index];

                                    if (currentChar != LastUsedPromptChar)
                                    {
                                        if (this.IsCharValid(groupOffset + counter, currentChar))
                                        {
                                            // Copy over to previous position.
                                            StringBuilder builder = new StringBuilder();
                                            builder.Append(groupDisplayText);
                                            builder[index] = LastUsedPromptChar;
                                            builder[counter] = currentChar;
                                            groupDisplayText = builder.ToString();

                                            this.SetCustomFilledPosition(currentChar, counter);
                                            this.RemoveFromCustomFilledList(index);

                                            break;
                                        }
                                        else
                                            break;//Go to point A.
                                    }
                                }
                                index++;
                            }
                        }
                    }
                }

                // Check the gaps at the beginning and end.
                unFilledMaskFields = totalMaskFields - filledMaskFields;

                if (unFilledMaskFields > 1)
                {
                    int emptyHalf = unFilledMaskFields / 2;

                    int emptyMasksAtRight = 0;

                    // Get the number of unfilled mask positions at the end.
                    for (counter = groupDisplayText.Length - 1; counter > 0; counter--)
                    {
                        if (this.IsLiteralCharacter(this.maskString[groupOffset + counter]) == false)
                        {
                            if (this.IsMaskPositionFilled(groupDisplayText, counter) == false)
                            {
                                // A mask position is empty.
                                emptyMasksAtRight++;
                            }
                            else
                                break;
                        }
                    }

                    int spacesToBeAdjusted = emptyMasksAtRight - emptyHalf;

                    // Now we have to try and move the above number of items to the right.
                    if (spacesToBeAdjusted > 0)
                    {
                        for (counter = groupDisplayText.Length - 1; counter >= 0; counter--)
                        {
                            if (this.IsLiteralCharacter(this.maskString[groupOffset + counter]) == false)
                            {
                                if (this.IsMaskPositionFilled(groupDisplayText, counter) == true)
                                {
                                    // A mask position is filled, try to move it towards the right
                                    // by spacesToBeAdjusted spaces.
                                    int i = counter;
                                    while (i >= 0)
                                    {
                                        if (this.IsCharValid(groupOffset + i + spacesToBeAdjusted, groupDisplayText[i]))
                                        {
                                            // Copy over to previous position.
                                            StringBuilder builder = new StringBuilder();
                                            builder.Append(groupDisplayText);
                                            builder[i] = LastUsedPromptChar;
                                            builder[i + spacesToBeAdjusted] = groupDisplayText[i];

                                            this.SetCustomFilledPosition(groupDisplayText[i], i + spacesToBeAdjusted);
                                            this.RemoveFromCustomFilledList(i);

                                            groupDisplayText = builder.ToString();
                                            i--;
                                        }
                                        else
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }



            //Finally check if the group text has changed.
            if (groupDisplayText != initialGroupText)
            {
                // Set the new group text to where the group text is.
                if (groupOffset != 0)
                    newDisplayString = localDisplayString.Substring(0, groupOffset);

                newDisplayString = newDisplayString + groupDisplayText;

                if (groupOffset + groupDisplayText.Length < localDisplayString.Length)
                    newDisplayString = newDisplayString + localDisplayString.Substring(groupOffset + groupDisplayText.Length, localDisplayString.Length - groupOffset - groupDisplayText.Length);

                this.DisplayString = newDisplayString;
            }
        }

        /// <summary>
        /// Use this everywhere except in RefreshPromptCharacter.
        /// </summary>
        private char LastUsedPromptChar
        {
            get
            {
                if (lastUsedPromptChar != '\0')
                    return lastUsedPromptChar;
                else
                    return ' ';
            }
        }

        /// <summary>
        /// Returns the first empty position.
        /// </summary>
        /// <param name="startPosition">The start position.</param>
        /// <returns>The first empty position.</returns>
        private int GetFirstEmptyPosition(int startPosition)
        {
            // Find valid mask positions and push characters to the
            // left - this only works till a non matching mask is encountered.
            string formattedText = this.FormattedText;
            int i = 0;

            for (i = startPosition; i >= 0; i--)
            {
                if (!this.IsLiteralCharacter(maskString[i]))
                {
                    if (formattedText[i] == this.GetPromptCharacter())
                        break;
                }
            }
            return i;
        }

        /// <summary>
        /// Intercepts the Key messages.
        /// </summary>
        /// <param name="m">The message data.</param>
        /// <returns>True if the key is handled; False otherwise.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override bool ProcessKeyMessage(ref Message m)
        {
            string prevText = Text;
            bool textChanged = false;

            if (this.ReadOnly || this.maskString == null || this.maskString.Length < 1)
                return base.ProcessKeyMessage(ref m);

            int keyData = (int)m.WParam | (int)ModifierKeys;
            int modifier = (int)ModifierKeys;

            if (this.IsMaskActive() == true)
            {
                if (m.Msg == 0x102) //winw.WM_CHAR
                {
                    this.supressKeyPress = true;

                    if (keyData != 8 && keyData != 13)
                    {
                        this.SetMaskedEditState(MaskedEditState.EditState);

                        if ((char)keyData == this.DecimalSeparator && this.UsageMode == MaskedUsageMode.Numeric)
                        {
                            this.HandleDecimalKey();
                            this.SetMaskedEditState(MaskedEditState.NormalState);

                            m_bIsTextByInputChanged = (prevText != Text);
                            return base.ProcessKeyMessage(ref m);

                        }

                        if (IsCharValid(this.SelectionStart, (char)keyData))
                        {
                            char currentChar = this.ApplySpecialMasks(this.SelectionStart, (char)keyData);
                            this.HandleCharacterInput(currentChar);
                            textChanged = true;

                            m_bIsTextByInputChanged = (prevText != Text);
                        }
                        else if ((char)keyData == this.DateSeparator || (char)keyData == this.TimeSeparator ||
                            (char)keyData == this.DecimalSeparator)
                        {
                            int specialCharacterPosition = 0;
                            int currentPosition = this.AdjustedSelectionStart;

                            specialCharacterPosition = this.DisplayString.IndexOf((char)keyData, currentPosition, this.DisplayString.Length - currentPosition);
                            if (specialCharacterPosition != -1)
                            {
                                specialCharacterPosition = this.GetNextDataPos(specialCharacterPosition);
                                this.SetSelection(specialCharacterPosition, 0);
                            }
                        }
                        else if (this.CausesValidation == true)
                        {
                            StringBuilder builder = new StringBuilder();
                            builder.Append(this.DisplayString);
                            if (this.SelectionStart < this.DisplayString.Length)
                                builder[this.SelectionStart] = (char)keyData;
                            else
                                builder.Append((char)keyData);
                            string invalidText = builder.ToString();
                            if (keyData != 8)//Backspace
                                this.RaiseValidationError(invalidText, this.SelectionStart);
                        }

                        this.SetMaskedEditState(MaskedEditState.NormalState);

                        m_bIsTextByInputChanged = (prevText != Text);

                        if (textChanged)
                            this.OnTextChanged(new EventArgs());
                        if (this.InputMode == MaskInputMode.Normal)
                        {
                            if (this.insertMode == false)
                                this.SelectionLength = 1;
                            else if (this.SelectionLength == 1)
                                this.SelectionLength = 0;
                        }

                    }
                    return base.ProcessKeyMessage(ref m);
                }
                else if (m.Msg == 0x100) //winw.WM_KEYDOWN
                {
                    if (modifier != (int)Keys.Shift)
                    {
                        switch (keyData & (int)Keys.KeyCode)
                        {
                            case (int)Keys.Back:
                                this.internalTextChange = true;
                                this.SetMaskedEditState(MaskedEditState.EditState);
                                this.HandleBackspaceKey();
                                this.SetMaskedEditState(MaskedEditState.NormalState);
                                this.internalTextChange = false;

                                m_bIsTextByInputChanged = (prevText != Text);

                                this.OnTextChanged(new EventArgs());
                                this.supressKeyDown = true;
                                return base.ProcessKeyMessage(ref m);

                            case (int)Keys.Delete:
                                this.internalTextChange = true;
                                this.SetMaskedEditState(MaskedEditState.EditState);
                                this.HandleDeleteKey();
                                this.SetMaskedEditState(MaskedEditState.NormalState);
                                this.internalTextChange = false;

                                m_bIsTextByInputChanged = (prevText != Text);

                                this.OnTextChanged(new EventArgs());
                                this.supressKeyDown = true;
                                return base.ProcessKeyMessage(ref m);

                            case (int)Keys.Insert:
                                if (this.InputMode == MaskInputMode.Normal)
                                    this.insertMode = !this.insertMode;
                                if (this.InputMode == MaskInputMode.Normal)
                                {
                                    if (this.insertMode == false)
                                        this.SelectionLength = 1;
                                    else if (this.SelectionLength == 1)
                                        this.SelectionLength = 0;
                                }

                                break;

                            default:
                                break;
                        }//end switch
                    }
                }
            }

            return base.ProcessKeyMessage(ref m);
        }

        /// <summary>
        /// Overrides to suppress KeyPress. Only the KeyPress event is raised - no other
        /// processing is done.
        /// </summary>
        /// <param name="m">The message.</param>
        /// <returns>True if the message is a KeyPress; otherwise the base class handles this.</returns>
        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            if (this.supressKeyPress == true)
            {
                int keyData = (int)m.WParam | (int)ModifierKeys;
                this.supressKeyPress = false;
                this.OnKeyPress(new KeyPressEventArgs((char)keyData));
                m.Result = (IntPtr)3;
                return true;
            }
            else if (this.supressKeyDown == true)
            {
                int keyData = (int)m.WParam | (int)ModifierKeys;
                this.supressKeyDown = false;
                this.OnKeyDown(new KeyEventArgs((Keys)keyData));
                m.Result = (IntPtr)3;
                return true;
            }
            else
                return base.ProcessKeyEventArgs(ref m);
        }


        /// <summary>
        /// Removes the text selected or specified by the parameters.
        /// </summary>
        /// <param name="currentDisplayText">The current string to be modified.</param>
        /// <param name="startPosition">The start position of the selection.</param>
        /// <param name="endPosition">The end position of the selection.</param>
        /// <returns>The modified string.</returns>
        protected string RemoveText(string currentDisplayText, int startPosition, int endPosition)
        {
            int index;
            StringBuilder builder = new StringBuilder();

            if (startPosition < 0 || startPosition > this.maskString.Length)
            {
                return currentDisplayText;
            }

            //If the To position is on a literal position,
            //then try to move it foward
            if (endPosition < this.maskString.Length)
            {
                if (this.IsLiteralCharacter(this.maskString[endPosition]))
                {
                    endPosition = this.GetNextDataPos(endPosition);
                    //If the to pos is now greater than the from pos,
                    //there was no data to pull.
                    if (endPosition > startPosition)
                        return currentDisplayText;
                }
            }

            //If the start position is at the end.
            if (startPosition == this.maskString.Length)
                startPosition = -1;
            else if (this.IsLiteralCharacter(this.maskString[startPosition]))
                startPosition = this.GetNextDataPos(startPosition);

            if (endPosition >= 0 && endPosition < this.maskString.Length)
            {
                index = endPosition;

                while (startPosition >= 0)
                {
                    char newChar = currentDisplayText[startPosition];

                    //Move the character.
                    builder = new StringBuilder();
                    builder.Append(currentDisplayText);
                    builder[index] = currentDisplayText[startPosition];
                    currentDisplayText = builder.ToString();

                    this.SetCustomFilledPosition(newChar, index);

                    //Move the pointers.
                    startPosition = this.GetNextDataPos(startPosition);
                    index = this.GetNextDataPos(index);
                }
            }
            else
                index = startPosition;

            //Clear out the rest of the data from the index on.
            while (index >= 0)
            {
                builder = new StringBuilder();
                builder.Append(currentDisplayText);
                builder[index] = this.GetCurrentPromptCharacter();
                currentDisplayText = builder.ToString();

                RemoveFromCustomFilledList(index);

                index = this.GetNextDataPos(index);
            }

            return currentDisplayText;
        }

        /// <summary>
        /// Removes the text selected or specified by the parameters within a group. In this case, 
        /// a group is defined as an area separated by a DecimalSeparator, Date Separator or
        /// Time Separator.
        /// </summary>
        /// <param name="currentDisplayText">The current string to be modified.</param>
        /// <param name="startPosition">The start position of the selection.</param>
        /// <param name="endPosition">The end position of the selection.</param>
        /// <returns>The modified string.</returns>
        protected string RemoveTextWithinGroup(string currentDisplayText, int startPosition, int endPosition)
        {
            int index;
            char localPromptChar = this.GetCurrentPromptCharacter();
            StringBuilder builder = new StringBuilder();

            if (startPosition < 0 || startPosition > this.maskString.Length)
            {
                return currentDisplayText;
            }

            if (endPosition >= 0)
            //If the to position is on a literal position,
            //then try to move it foward.
            if (this.IsLiteralCharacter(this.maskString[endPosition]))
            {
                endPosition = this.GetNextDataPos(endPosition);
                //If the to pos is now greater than the from pos,
                //there was no data to pull.
                if (endPosition > startPosition)
                    return currentDisplayText;
            }

            //If the start position is at the end.
            if (startPosition == this.maskString.Length)
                startPosition = -1;
            else if (this.IsLiteralCharacter(this.maskString[startPosition]))
                startPosition = this.GetNextDataPos(startPosition);

            if (PullCharOnDelete)
            {
                index = startPosition;

                while (startPosition >= 0 && endPosition >= 0 && startPosition < endPosition && endPosition < this.maskString.Length)
                {
                    char newChar = currentDisplayText[endPosition];
                    //Move the character.
                    builder = new StringBuilder();
                    builder.Append(currentDisplayText);
                    builder[index] = currentDisplayText[endPosition];
                    currentDisplayText = builder.ToString();

                    this.SetCustomFilledPosition(newChar, index);
                    //Move the pointers.
                    endPosition = this.GetNextDataPos(endPosition);
                    index = this.GetNextDataPos(index);
                }

                if (endPosition == -1)
                {
                    while (index >= 0)
                    {
                        builder = new StringBuilder();
                        builder.Append(currentDisplayText);
                        builder[index] = localPromptChar;
                        currentDisplayText = builder.ToString();

                        RemoveFromCustomFilledList(index);

                        index = this.GetNextDataPos(index);
                    }
                }
                else
                {
                    while (index >= 0 && index < this.maskString.Length)
                    {
                        builder = new StringBuilder();
                        builder.Append(currentDisplayText);
                        builder[index] = localPromptChar;
                        currentDisplayText = builder.ToString();

                        RemoveFromCustomFilledList(index);

                        index = this.GetNextDataPos(index);
                    }
                }
            }
            else
            {
                index = endPosition;

                while (startPosition >= 0 && startPosition < endPosition)
                {
                    char newChar = currentDisplayText[startPosition];
                    //Move the character.
                    builder = new StringBuilder();
                    builder.Append(currentDisplayText);
                    builder[index] = currentDisplayText[startPosition];
                    currentDisplayText = builder.ToString();

                    this.SetCustomFilledPosition(newChar, index);
                    //Move the pointers.
                    startPosition = this.GetNextDataPos(startPosition);
                    index = this.GetNextDataPos(index);
                }

                //Clear out the rest of the data from the index on.
                if (startPosition == -1)
                {
                    while (index >= 0)
                    {
                        builder = new StringBuilder();
                        builder.Append(currentDisplayText);
                        builder[index] = localPromptChar;
                        currentDisplayText = builder.ToString();

                        RemoveFromCustomFilledList(index);

                        index = this.GetNextDataPos(index);
                    }
                }
                else
                {
                    while (index >= 0 && index < (int)Math.Max(endPosition, startPosition))
                    {
                        builder = new StringBuilder();
                        builder.Append(currentDisplayText);
                        builder[index] = localPromptChar;
                        currentDisplayText = builder.ToString();

                        RemoveFromCustomFilledList(index);

                        index = this.GetNextDataPos(index);
                    }
                }
            }

            return currentDisplayText;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected void RemoveFromCustomFilledList(int index)
        {
            if (this.customFilledPositions.ContainsKey(index))
                this.customFilledPositions.Remove(index);
        }

        /// <summary>
        /// Removes the selected text.
        /// </summary>
        /// <param name="startPosition">The start position of the selection.</param>
        /// <param name="endPosition">The end position of the selection.</param>
        /// <returns>The modified string.</returns>
        protected string RemoveText(int startPosition, int endPosition)
        {
            return this.RemoveText(this.DisplayString, startPosition, endPosition);
        }

        /// <summary>
        /// Indicates whether the backspace key has been pressed.
        /// </summary>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool HandleBackspaceKey()
        {
            int endPosition = 0;
            int startPosition = 0;

            // Special calls for SelectionStart and SelectionLength.
            endPosition = this.AdjustedSelectionStart + this.AdjustedSelectionLength;
            startPosition = this.AdjustedSelectionStart;

            if (this.AdjustedSelectionLength > 0)
            {
                if(PullCharOnDelete)
                    HandleCharDelete(this.DisplayString, startPosition, endPosition, false, true, true);
                else
                    this.DisplayString = this.RemoveTextWithinGroup(this.DisplayString, endPosition, startPosition);

                if (this.promptCharacterInt == 0)
                {
                    string dispString = this.DisplayString.Replace(this.GetPromptCharacter(), ' ');
                    this.RefreshDisplay(dispString, false);
                }
                else
                    RefreshDisplay();

                startPosition = GetNextDataPos(startPosition - 1);
                if (startPosition == -1)
                    startPosition = GetPrevDataPos(this.maskString.Length) + 1;
                this.SetSelection(startPosition, 0);
            }
            else
            {
                int previousPosition = GetPrevDataPos(startPosition);

                if (previousPosition != -1)
                {
                    if (PullCharOnDelete)
                        HandleCharDelete(this.DisplayString,startPosition,previousPosition, false, true, true);
                    else
                        this.DisplayString = this.RemoveTextWithinGroup(this.DisplayString, startPosition, previousPosition);

                    if (this.promptCharacterInt == 0)
                    {
                        string dispString = this.DisplayString.Replace(this.GetPromptCharacter(), ' ');
                        this.RefreshDisplay(dispString, false);
                    }
                    else
                        RefreshDisplay();
                    this.SetSelection(previousPosition, 0);
                }
            }

            return true;
        }

        private void GetAdjustedValuesForNullPrompt(ref string adjDisplayString, ref int adjSelectionStart, ref int adjSelectionLength)
        {
            if (this.listDisplayToInternalString.Count > 0)
            {
                adjDisplayString = this.internalDisplayString;
                adjSelectionStart = this.GetAdjustedPositionForNullPrompt(this.SelectionStart);
                adjSelectionLength = Math.Max(0, this.GetAdjustedPositionForNullPrompt(this.SelectionStart + this.SelectionLength) - adjSelectionStart);

                if (adjSelectionStart <= this.listDisplayToInternalString.Count - 1)
                    adjSelectionStart = this.GetFirstPrevDataPos(adjDisplayString, adjSelectionStart);
            }
            else
            {
                adjDisplayString = this.DisplayString;
                adjSelectionStart = Math.Max(this.SelectionStart, 0);
                adjSelectionLength = Math.Max(this.SelectionLength, 0);
            }
        }

        /// <summary>
        /// Adjusts the DisplayString for internal manipulations when the PromptCharacter/
        /// PassivePromptCharacter is NULL.
        /// </summary>
        private void AdjustStringAndSelectionForNullPrompt()
        {
            this.returnActualText = true;

            if (this.listDisplayToInternalString.Count > 0)
            {
                int localSelectionStart = 0;
                int localSelectionLength = 0;

                localSelectionStart = this.GetAdjustedPositionForNullPrompt(this.SelectionStart);
                localSelectionLength = Math.Max(0, this.GetAdjustedPositionForNullPrompt(this.SelectionStart + this.SelectionLength) - localSelectionStart);

                this.DisplayString = this.internalDisplayString;
                if (localSelectionStart <= this.listDisplayToInternalString.Count - 1)
                    localSelectionStart = this.GetFirstPrevDataPos(this.DisplayString, localSelectionStart);

                this.AdjustedSelectionStart = localSelectionStart;
                this.AdjustedSelectionLength = localSelectionLength;
            }
            else
            {
                this.AdjustedSelectionStart = Math.Max(this.SelectionStart, 0);
                this.AdjustedSelectionLength = Math.Max(this.SelectionLength, 0);
            }
            this.returnActualText = false;
        }



        /// <summary>
        /// Indicates whether the delete key has been pressed.
        /// </summary>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool HandleDeleteKey()
        {
            // Only handle the Delete key if the Mask is currently active.
            if (this.IsMaskActive() == false)
            {
                return false;
            }

            // The prompt character can be different based on the state of the control. Get the currently used
            // prompt character (Could be PromptCharacter or PassivePromptCharacter).
            char localPromptChar = this.GetCurrentPromptCharacter();

            // Get the adjusted selection position and length - this will be different only if the 
            // current prompt character is NULL.
            int endPosition = this.AdjustedSelectionStart + this.AdjustedSelectionLength;
            int startPosition = this.AdjustedSelectionStart;

            // If the selection starts after the maximum possible length - there is nothing to delete.
            if (startPosition >= this.maskString.Length)
                return false;

            if (PullCharOnDelete)
                return HandleCharDelete(this.DisplayString, startPosition, endPosition, true, false, false);

            // If text is currently selected.
            if (this.AdjustedSelectionLength > 0)
            {
                this.DisplayString = this.RemoveTextWithinGroup(this.DisplayString, endPosition, startPosition);
                RefreshDisplay();
                startPosition = GetNextDataPos(startPosition - 1);
                if (startPosition == -1)
                    startPosition = GetPrevDataPos(this.maskString.Length) + 1;
                this.SetSelection(startPosition, 0);
            }
            // If test is not selected.
            else
            {
                // Position the cursor at the first filled data position
                // First position at the first data position (filled or unfilled).
                if (this.IsLiteralCharacter(this.maskString[startPosition]) == true)
                {
                    startPosition = GetNextDataPos(startPosition);
                    endPosition = startPosition + this.AdjustedSelectionLength;
                }
                if(startPosition >= 0)
                {
                // If the current position is unfilled, move to a filled position.
                if (DisplayString[startPosition] == localPromptChar)
                {
                    int emptySpaces = 0;
                    int oldStartPosition = startPosition;

                    while (startPosition != -1 && startPosition < this.maskString.Length && DisplayString[startPosition] == localPromptChar)
                    {
                        emptySpaces++;
                        oldStartPosition = startPosition;
                        startPosition = this.GetNextDataPos(startPosition);

                        if (startPosition == -1)
                        {
                            startPosition = oldStartPosition;
                            emptySpaces--;
                            break;
                        }
                    }

                    if (emptySpaces > 0)
                        endPosition = endPosition + emptySpaces;
                }
                 }

                // Get the next possible position.
                int nextPosition = GetNextDataPos(startPosition);

                // There is a valid next position.
                if (nextPosition != -1)
                {
                    if (startPosition < this.maskString.Length)
                    {
                        this.DisplayString = this.RemoveTextWithinGroup(this.DisplayString, nextPosition, startPosition);
                    }
                }
                else// There is no next position.
                    this.DisplayString = this.RemoveText(startPosition, startPosition + 1);

                if (startPosition < this.maskString.Length)
                {
                    if (this.promptCharacterInt == 0)
                    {
                        string dispString = this.DisplayString.Replace(this.GetPromptCharacter(), ' ');
                        this.RefreshDisplay(dispString, false);
                    }
                    else
                        RefreshDisplay();
                    this.SetSelection(startPosition, 0);
                }
            }

            return true;
        }



        /// <summary>
        /// Handles a character input. 
        /// </summary>
        /// <param name="charInput">The character that was input.</param>
        /// <returns>True if the character was successfully inserted.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool HandleCharacterInput(char charInput)
        {
            bool keyHandled = true;
            int currentPosition = 0;
            int endPosition = 0;
            int initialPosition = 0;
            bool textChanged = true;

            if ((Control.ModifierKeys & Keys.Control) != Keys.None)
                return false;

            if (this.ReadOnly)
                return false;

            // Set the start and end points.
            currentPosition = this.AdjustedSelectionStart;
            initialPosition = currentPosition;
            endPosition = this.AdjustedSelectionStart + this.AdjustedSelectionLength;

            // Stage 1 - Position the cursor in this stage depending on
            // whether text is selected or not and based on the curent
            // position.
            if (this.SelectionLength > 0)
            {
                if (IsLiteralCharacter(this.maskString[currentPosition]))
                    currentPosition = this.MoveCursorPosition(currentPosition, 1, false);

                if (currentPosition >= endPosition || currentPosition == -1)
                    return false;
            }
            else
            {
                currentPosition = this.MoveCursorPosition(currentPosition - 1, 1, false);
                if (currentPosition == -1)
                    return false;
            }

            // Stage 2 - Check if the input character is a valid character and
            // add it to the contents.
            if (IsCharValid(currentPosition, charInput))
            {
                if (this.InputMode == MaskInputMode.Normal)
                {
                    if (this.insertMode && (currentPosition <= endPosition))
                        this.DisplayString = this.RemoveText(endPosition, currentPosition);
                    else if ((currentPosition + 1) < endPosition)
                        this.DisplayString = this.RemoveText(endPosition, currentPosition + 1);
                }
                else
                {
                    if ((currentPosition + 1) < endPosition)
                        this.DisplayString = this.RemoveText(endPosition, currentPosition + 1);
                }


                if (this.InputMode == MaskInputMode.Normal)
                {
                    char chMask;
                    chMask = this.maskString[currentPosition];
                    int maskValue = this.GetMaskCharValue((char)(chMask - MaskedEditBox.maskOffset));
                    MaskCharTypes maskType = (MaskCharTypes)maskValue;

                    if (this.insertMode && this.IsMaskPositionFilled(this.DisplayString, currentPosition) == true)
                    {
                        string currentDisplayString = this.DisplayString;
                        this.DisplayString = this.InsertChar(currentPosition, charInput, false);
						if((!(maskType == MaskCharTypes.maskCharDigitRequired && charInput == (char)Keys.Space)) && this.DisplayString == currentDisplayString)
                        {
                            currentPosition = initialPosition;
                            textChanged = false;
                        }
                        else
                            this.SetCustomFilledPosition(charInput, currentPosition);
                    }
                    else
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append(this.DisplayString);
                        if(!(maskType == MaskCharTypes.maskCharDigitRequired && charInput == (char)Keys.Space))
                        builder[currentPosition] = charInput;
                        this.DisplayString = builder.ToString();

                        this.SetCustomFilledPosition(charInput, currentPosition);
                    }
                }
                else
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(this.DisplayString);
                    char chMask;
                    chMask = this.maskString[currentPosition];                    
                    int maskValue = this.GetMaskCharValue((char)(chMask - MaskedEditBox.maskOffset));
                    MaskCharTypes maskType = (MaskCharTypes) maskValue;                    

                    if (maskType == MaskCharTypes.maskCharDigitRequired && charInput == (char)Keys.Space)
                    {
                        char chCurrent = builder[currentPosition];
                        char chEnd = builder[this.maskDisplay.Length - 1];

                        if (chCurrent != this.GetCurrentPromptCharacter() && chEnd == this.GetCurrentPromptCharacter())
                        {
                            builder.Insert(currentPosition, charInput);
                            builder.Remove(builder.Length - 1, 1);
                        }
                        else
                            builder[currentPosition] = charInput;
                    }
                    else
                        builder[currentPosition] = charInput;

                    this.DisplayString = builder.ToString();

                    this.SetCustomFilledPosition(charInput, currentPosition);
                }
                this.internalTextChange = true;
                if (this.promptCharacterInt == 0)
                {
                    string dispString = this.DisplayString.Replace(this.GetPromptCharacter(), ' ');
                    this.RefreshDisplay(dispString, false);
                }
                else
                    RefreshDisplay();
                this.internalTextChange = false;
                if (textChanged)
                    currentPosition = this.MoveCursorPosition(currentPosition, 1, false);
            }
            else
            {
                if (charInput == this.DateSeparator || charInput == this.TimeSeparator ||
                    charInput == this.DecimalSeparator)
                {
                    // Jump to that position.
                    currentPosition = this.DisplayString.IndexOf(charInput, (int)Math.Max(currentPosition - 1, 0));
                    currentPosition = this.GetNextDataPos(currentPosition);
                    currentPosition = this.MoveCursorPosition(currentPosition, 1, false);
                }
                else if (this.CausesValidation == true)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(this.DisplayString);
                    if (currentPosition < this.DisplayString.Length)
                        builder[currentPosition] = charInput;
                    else
                        builder.Append(charInput);
                    string invalidText = builder.ToString();
                    this.RaiseValidationError(invalidText, currentPosition);
                }

                keyHandled = false;
            }

            this.Validate(false);
            return keyHandled;
        }

        /// <summary>
        /// Changes the character case according to the casingType passed in and
        /// returns the character with the correct casing. Use with the special masks
        /// for upper case and lower case.
        /// </summary>
        /// <param name="currentChar">The current character.</param>
        /// <param name="casingType">The casing change to be applied.</param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected char ApplyCasing(char currentChar, CasingNormalize casingType)
        {
            string utilityString = null;

            if (casingType == CasingNormalize.changeToLowerOnly)
            {
                utilityString = Convert.ToString(currentChar);
                utilityString = utilityString.ToLower();
                currentChar = Convert.ToChar(utilityString);
            }
            else if (casingType == CasingNormalize.changeToUpperOnly)
            {
                utilityString = Convert.ToString(currentChar);
                utilityString = utilityString.ToUpper();
                currentChar = Convert.ToChar(utilityString);
            }

            return currentChar;
        }

        /// <summary>
        /// Positions the cursor next to a decimal separator if present.
        /// </summary>
        private void PositionNearDecimal()
        {
            this.SetMaskedEditState(MaskedEditState.EditState);

            int decimalPosition = 0;
            int startPosition = 0;

            decimalPosition = this.DisplayString.IndexOf(this.DecimalSeparator, startPosition, this.DisplayString.Length - startPosition);
            if (decimalPosition >= 0)
            {
                int nextPosition = this.GetNextDataPos(decimalPosition);
                if (nextPosition >= 0)
                    this.SetSelection(nextPosition, 0);
            }
            this.SetMaskedEditState(MaskedEditState.NormalState);

        }

        private void PositionAtFirstMaskPosition()
        {
            this.SetMaskedEditState(MaskedEditState.EditState);
            int nFirstMaskPosition = GetFirstMaskPosition();

            this.SelectionStart = nFirstMaskPosition;
            this.SetMaskedEditState(MaskedEditState.NormalState);
        }

        private int GetFirstMaskPosition()
        {
            int nPositionToReturn = 0;
            string strMask = this.maskString;

            for (int nIndex = 0; nIndex < strMask.Length; ++nIndex)
            {
                //If the position is on a literal char, it is not valid.
                if (!this.IsLiteralCharacter(strMask[nIndex]))
                {
                    nPositionToReturn = nIndex;
                    break;
                }
            }

            return nPositionToReturn;
        }

        /// <summary>
        /// Positions the cursor at the first data position.
        /// </summary>
        private void PositionAtFirstPos()
        {
            int firstPosition = 0;

            firstPosition = this.GetNextDataPos(0);
            if (firstPosition != -1)
            {
                this.MoveCursorPosition(firstPosition, -1, false);
            }

        }

        /// <summary>
        /// Applies the current CharacterCasing settings in effect for this 
        /// textbox.
        /// </summary>
        /// <param name="currentChar">The current character.</param>
        /// <param name="casingType">The casing change to be applied.</param>
        /// <returns>The changed character.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected char NormalizeCasing(char currentChar, CasingNormalize casingType)
        {
            string utilityString = null;

            if (this.CharacterCasing == CharacterCasing.Upper && casingType != CasingNormalize.changeToLowerOnly)
            {
                utilityString = Convert.ToString(currentChar);
                utilityString = utilityString.ToUpper();
                currentChar = Convert.ToChar(utilityString);
            }
            else if (this.CharacterCasing == CharacterCasing.Lower && casingType != CasingNormalize.changeToUpperOnly)
            {
                utilityString = Convert.ToString(currentChar);
                utilityString = utilityString.ToLower();
                currentChar = Convert.ToChar(utilityString);
            }

            return currentChar;
        }

        /// <summary>
        /// Raises the MaskCustomValidate event.
        /// </summary>
        /// <param name="args">The event data.</param>
        protected void OnMaskCustomValidate(MaskCustomValidateArgs args)
        {
            if (this.MaskCustomValidate != null)
                this.MaskCustomValidate(this, args);
        }

        /// <summary>
        /// Indicates whether the character is valid for this position based on the
        /// mask.
        /// </summary>
        /// <param name="currentPosition">The current position.</param>
        /// <param name="currentChar">The current character.</param>
        /// <returns>True if the character is acceptable; False otherwise.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool IsCharValid(int currentPosition, char currentChar)
        {

            if (this.IsMaskActive() == false)
                return false;

            if (currentChar == this.GetPromptCharacter())
            {
                if (this.AllowPrompt == false)
                    return false;
            }

            //If we are at the end of the input mask, this char is not valid.
            if ((int)currentPosition >= this.maskString.Length || currentPosition < 0)
                return false;

            char chMask;
            bool returnValue = true;

            // This case adjustment will occur irrespective.
            currentChar = this.NormalizeCasing(currentChar, CasingNormalize.changeToBoth);
            chMask = this.maskString[currentPosition];

            //If the position is on a literal char, it is not valid.
            if (IsLiteralCharacter(chMask))
            {
                if (currentPosition < this.maskString.Length)
                    currentPosition = this.GetNextDataPos(currentPosition);
                else
                    returnValue = false;

                if (currentPosition == -1)
                    returnValue = false;
                else
                    chMask = this.maskString[currentPosition];
            }

            if (returnValue)
            {
                // Raise the event so that users can handle the masks in a specific manner.
                MaskCustomValidateArgs args = new MaskCustomValidateArgs(currentPosition, currentChar, (char)(chMask - MaskedEditBox.maskOffset));
                this.OnMaskCustomValidate(args);

                if (args.Handled == true)
                {
                    returnValue = args.Accepted;
                }
                else
                {

                    int maskValue = this.GetMaskCharValue((char)(chMask - MaskedEditBox.maskOffset));
                    MaskCharTypes maskType = (MaskCharTypes)maskValue;
                    switch (maskType)
                    {
						case MaskCharTypes.maskCharDigitRequired: //'#'numerics & white space only
                        case MaskCharTypes.maskCharDigitOptional: //'9'numerics (optional)
                            {
                            if (maskType == MaskCharTypes.maskCharDigitRequired && Char.IsWhiteSpace(currentChar))
                            {
                                char chCurrent = this.DisplayString[currentPosition];
                                char chEnd = this.DisplayString[this.DisplayString.Length - 1];
                                if (chCurrent != this.GetCurrentPromptCharacter() && chEnd != this.GetCurrentPromptCharacter())
                                    returnValue = false;
                                else
                                    returnValue = true;
                            }
							else if(Char.IsDigit(currentChar))
                                    returnValue = true;
                                else
                                    returnValue = false;
                                break;
                            }

                        case MaskCharTypes.maskCharLetterRequired: // '?'alphas only
                        case MaskCharTypes.maskCharLetterOptional: // 'y' letters only (optional)
                            {
                                if (Char.IsLetter(currentChar))
                                    returnValue = true;
                                else
                                    returnValue = false;
                                break;
                            }

                        case MaskCharTypes.maskCharAplhaNumericRequired: //'A'alpanumerics only
                        case MaskCharTypes.maskCharAlphaNumericOptional: //'a'alpanumerics only (optional)
                            {
                                if (Char.IsLetterOrDigit(currentChar))
                                    returnValue = true;
                                else if (Char.IsWhiteSpace(currentChar) && allowPrompt && PromptCharacter == ' ')
                                    returnValue = true;
                                else
                                    returnValue = false;

                                break;
                            }

                        case MaskCharTypes.maskCharCharacterRequired://'&': any letter, digit, symbol, punctuation or whitespace
                        case MaskCharTypes.maskCharCharacterOptional://'C': any letter, digit, symbol, punctuation or whitespace (optional)
                            {
                                if (Char.IsLetterOrDigit(currentChar)
                                    || Char.IsSymbol(currentChar)
                                    || Char.IsPunctuation(currentChar)
                                    || Char.IsWhiteSpace(currentChar)
                                    )
                                    returnValue = true;
                                else
                                    returnValue = false;

                                break;
                            }

                        case MaskCharTypes.maskCharHexaDecimalOptional://'x': //Hex optional
                        case MaskCharTypes.maskCharHexaDecimalRequired://'X' : Hex required
                            {
                                if ((currentChar >= 'a' && currentChar <= 'f')
                                    || (currentChar >= 'A' && currentChar <= 'F')
                                    || (currentChar >= '0' && currentChar <= '9')
                                    )
                                    returnValue = true;
                                else
                                    returnValue = false;

                                break;
                            }

                        default:
                            returnValue = false;
                            break;

                    }//end switch
                }
            }

            //CheckForMaxAndMin(currentPosition, charInput)

            if (this.UsageMode == MaskedUsageMode.Numeric && returnValue == true)
            {
                // Check if the character can be allowed without
                // changing the actual content of the text box.
                string changedString = this.internalDisplayString;
                int endPosition = this.SelectionStart + this.SelectionLength;

                if (this.insertMode && ((currentPosition + 1) <= endPosition))
                    changedString = this.RemoveText(endPosition, currentPosition);
                else if ((currentPosition + 1) < endPosition)
                    changedString = this.RemoveText(endPosition, currentPosition + 1);

                if (this.insertMode)
                    changedString = this.InsertChar(changedString, currentPosition, currentChar, false);
                else
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(changedString);
                    builder[currentPosition] = currentChar;
                    changedString = builder.ToString();
                }

                returnValue = this.CheckMinMax(changedString, false);
            }
            return returnValue;
        }

        protected bool CheckMinMax(string changedString, bool ignoreLength)
        {
            // Get the number part length for numericValue, minValue, and maxValue.
            bool returnValue = true;
            decimal numericValue = 0.0m;

            int numericValueLength, minValueLength, maxValueLength;
            numericValueLength = this.GetNumberPartLength(changedString);
            minValueLength = this.GetNumberPartLength(this.MinValue);
            maxValueLength = this.GetNumberPartLength(this.MaxValue);
            numericValue = this.GetDecimalValue(changedString);

            if (ignoreLength == false)
            {
                if (numericValueLength >= minValueLength)
                {
                    if (numericValue < this.MinValue)
                        returnValue = false;
                }

                if (numericValueLength >= maxValueLength)
                {
                    if (numericValue > this.MaxValue)
                        returnValue = false;
                }
                else
                    returnValue = true;
            }
            else
            {
                if (numericValue < this.MinValue || numericValue > this.MaxValue)
                    returnValue = false;
            }

            return returnValue;
        }

        private int GetNumberPartLength(decimal numberValue)
        {
            return this.GetNumberPartLength(numberValue.ToString());
        }

        private int GetNumberPartLength(string numberText)
        {
            int numberPartLength = 0;
            numberText = this.GetNumericText(numberText);
            int decimalSeparator = numberText.IndexOf(this.DecimalSeparator);
            if (decimalSeparator != -1)
                numberPartLength = decimalSeparator;
            else
                numberPartLength = numberText.Length;

            return numberPartLength;
        }

        /// <summary>
        /// Returns the decimal value of the displayed text.
        /// </summary>
        /// <param name="currentDisplayText">The currently displayed text.</param>
        /// <returns>The decimal value.</returns>
        protected decimal GetDecimalValue(string currentDisplayText)
        {
            decimal currentValue = 0m;
            string numericText = this.GetNumericText(currentDisplayText);

            try
            {
                if (numericText == String.Empty)
                    currentValue = 0.0m;
                else
                    currentValue = Convert.ToDecimal(numericText);
            }
            catch
            {
                currentValue = 0.0m;
            }

            return currentValue;
        }

        /// <summary>
        /// Returns the numeric text.
        /// </summary>
        /// <param name="currentText">The current text.</param>
        /// <returns>The numeric text.</returns>
        protected string GetNumericText(string currentText)
        {
            string numericText = String.Empty;
            int maxLength = Math.Min(this.maskString.Length, currentText.Length);

            for (int index = 0; index < maxLength; index++)
            {
                if (!IsLiteralCharacter(this.maskString[index]) || this.maskString[index] == this.DecimalSeparator)
                {
                    if (this.maskString[index] == this.DecimalSeparator && currentText[index] == this.DecimalSeparator)
                    {
                        numericText += currentText[index];
                    }
                    else if (currentText[index] != this.GetPromptCharacter() && currentText[index] >= '0' && currentText[index] <= '9')
                    {
                        numericText += currentText[index];
                    }
                }
            }

            return numericText;
        }


        /// <summary>
        /// Sets external text from the 
        /// clipboard to the MaskedEditBox.
        /// </summary>
        /// <param name="externalText">The text to be pasted.</param>
        /// <param name="beginPos">The beginning position.</param>
        /// <param name="insertMode">Indicates whether the text is to be inserted.</param>
        /// <param name="ignorePromptCharacters">Indicates whether to ignore prompt characters.</param>
        /// <returns>True if the text was set successfully; False otherwise.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool SetExternalText(string externalText, int beginPos, bool insertMode, bool ignorePromptCharacters)
        {
            int externalTextCount = 0;
            int maskCount = beginPos;
            int maskLength = this.maskString.Length;
            int externalTextLength = externalText.Length;
            char chData;

            if (maskLength == 0)
            {
                SetBaseText(externalText);
                return false;
            }

            if (beginPos < 0 || beginPos >= maskLength)
            {
                return false;
            }

            // If the start position is at the beginning.
            if (beginPos == 0)
            {
                string dispString = String.Empty;
                this.ApplyMaskLogic(this.maskString, ref dispString);
                this.DisplayString = dispString;
            }

            if (IsLiteralCharacter(this.maskString[maskCount]))
                maskCount = GetNextDataPos(maskCount);

            if (maskCount >= maskLength || maskCount == -1)
                return false;

            for (externalTextCount = 0; externalTextCount < externalTextLength; externalTextCount++)
            {
                if (maskCount >= maskLength || maskCount == -1)
                    break;

                if (!IsLiteralCharacter(this.maskString[maskCount]))
                {
                    chData = externalText[externalTextCount];

                    if (IsCharValid(maskCount, chData))
                    {
                        char currentChar = this.ApplySpecialMasks(maskCount, chData);

                        if (insertMode)
                        {

                            if (char.IsWhiteSpace(currentChar) && this.maskString[externalTextCount] == ' ')
                            {
                                maskCount--;
                            }
                            this.DisplayString = this.InsertChar(maskCount, currentChar, false);

                        }
                        else
                        {
                            StringBuilder builder = new StringBuilder();
                            builder.Append(this.DisplayString);
                            builder[maskCount] = currentChar;
                            this.DisplayString = builder.ToString();

                        }
                        maskCount = this.GetNextDataPos(maskCount);
                    }
                    else
                    {
                        if (chData == this.DateSeparator || chData == this.TimeSeparator ||
                            chData == this.DecimalSeparator)
                        {
                            // Jump to that position.
                            int specialPosition = this.DisplayString.IndexOf(chData, (int)Math.Max(maskCount - 1, 0));
                            if (specialPosition != -1)
                            {
                                maskCount = specialPosition;
                                maskCount = this.GetNextDataPos(maskCount);
                            }
                        }
                        else if (!ignorePromptCharacters && chData == this.GetPromptCharacter())
                            maskCount = this.GetNextDataPos(maskCount);
                    }
                }
            }

            this.ApplyAlignmentToGroups();
            this.RefreshDisplay();
            this.SetSelection(maskCount, 0);
            return true;
        }

        /// <summary>
        /// Internal helper for getting the PromptCharacter.
        /// </summary>
        /// <returns>The prompt character.</returns>
        private char GetPromptCharacter()
        {
            if (this.promptCharacterInt == 0)
                return (char)(32 + MaskedEditBox.maskOffset);
            else
                return (char)this.promptCharacterInt;
        }

        /// <summary>
        /// Internal helper for getting the PassivePromptCharacter.
        /// </summary>
        /// <returns>The passive prompt character.</returns>
        private char GetPassivePromptCharacter()
        {
            if (this.passivePromptCharacterInt == 0)
                return (char)(32 + MaskedEditBox.maskOffset);
            else
                return (char)this.passivePromptCharacterInt;
        }

        /// <summary>
        /// This method will Undo the previous operation.
        /// </summary>
        /// <remarks>
        /// The MaskedEditBox maintains its own undo mechanism.
        /// </remarks>
        public new void Undo()
        {
            if (IsMaskActive() == false)
            {
                base.Undo();
                return;
            }

            if (this.CanUndo == false)
                return;

            this.DisplayString = this.undoBufferText;
            RefreshDisplay();
            return;
        }

        /// <summary>
        /// Copies the content of the MaskEditBox to the clipboard.
        /// </summary>
        /// <remarks>
        /// The ClipMode property dictates what gets copied.
        /// For example, if the content of MaskedEditBox is (919)481 1974, the
        /// following will be the strings copied to the clipboard depending
        /// on the ClipMode property:
        /// <para>
        /// ClipModes.IncludeLiterals - (919)481 1974
        /// ClipModes.ExcludeLiterals - 9194811974
        /// </para>
        /// </remarks>
        public new void Copy()
        {
            if (this.ClipMode == ClipModes.IncludeLiterals || (this.IsMaskActive() == false))
            {
                base.Copy();
                return;
            }

            string clipText = this.GetClipText(this.DisplayString, this.SelectionStart, this.SelectionStart + this.SelectionLength);
            Clipboard.SetDataObject(clipText);
        }


        /// <summary>
        /// Handles the pasting of data from the clipboard into the
        /// MaskededitBox control.
        /// </summary>
        /// <remarks>
        /// The method takes into account the nature of the text in the clipboard
        /// and tries to normalize the text. It will accomodate as much as possible
        /// depending on the current length of the text and the total length allowed.
        /// If text with acceptable and unacceptable characters are found in the
        /// clipboard, the unacceptable data is omitted when the data is inserted.
        /// </remarks>
        public new void Paste()
        {
            if (this.IsMaskActive() == false || this.ReadOnly == true)
            {
                base.Paste();
                return;
            }

            this.SetMaskedEditState(MaskedEditState.EditState);

            int nextDataPos = 0;
            string clipBoardData = null;
            int startPosition = this.AdjustedSelectionStart;
            int endPosition = this.AdjustedSelectionStart + this.AdjustedSelectionLength;

            if (this.SelectionLength > 0)
                this.DisplayString = this.RemoveText(endPosition, startPosition);

            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.Text))
            {
                clipBoardData = data.GetData(DataFormats.Text).ToString();

                if (clipBoardData != null)
                {
                    this.SetExternalText(clipBoardData, startPosition, true, true);
                }
            }
            else if (IsLiteralCharacter(this.maskString[startPosition]))
            {
                nextDataPos = GetNextDataPos(this.SelectionStart);
                if (nextDataPos != -1)
                    startPosition = nextDataPos;

                this.SetSelection(startPosition, 0);
            }

            this.SetMaskedEditState(MaskedEditState.NormalState);

            return;
        }

        /// <summary>
        /// Cuts data from the MaskedEditBox and pastes it into
        /// the clipboard.
        /// </summary>
        /// <remarks>
        /// The MaskedEditBox deletes the selected text in this case and then
        /// copies the deleted text to the clipboard. The ClipMode property dictates
        /// whether the literal characters are copied to the clipboard or not.
        /// </remarks>
        public new void Cut()
        {
            if (this.IsMaskActive() == false || this.ReadOnly == true)
            {
                base.Cut();
                return;
            }

            this.Copy();

            this.SetMaskedEditState(MaskedEditState.EditState);

            int selectionStart = this.AdjustedSelectionStart;
            int selectionEnd = this.AdjustedSelectionStart + this.AdjustedSelectionLength;

            if (selectionEnd != selectionStart)
            {
                this.DisplayString = this.RemoveText(selectionEnd, selectionStart);
                if (this.promptCharacterInt == 0)
                {
                    string dispString = this.DisplayString.Replace(this.GetPromptCharacter(), ' ');
                    this.RefreshDisplay(dispString, false);
                }
                else
                    RefreshDisplay();
            }

            this.SetMaskedEditState(MaskedEditState.NormalState);

            this.SetSelection(selectionStart, 0);
            return;

        }

        /// <summary>
        /// Clears the mask edit back to its initial state.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public new void Clear()
        {
            base.Clear();
            this.RefreshDisplay();
        }

        /// <summary>
        /// Sets the text property of the MaskedEditBox without raising the
        /// TextChanged event. 
        /// </summary>
        /// <param name="newText">The new text string.</param>
        /// <remarks>
        /// This method is provided as an alternative to being able to
        /// set the Text property through the designer.
        /// </remarks>
        public void SetInitialText(string newText)
        {
            this.internalTextChange = true;
            this.Text = newText;
            this.internalTextChange = false;
        }

        /// <summary>
        /// Overrides the <see cref="TextBox.Text"/> property.
        /// </summary>
        /// <remarks>
        /// This method is overriden in order to intercept and normalize 
        /// external text that is not in a format acceptable to the 
        /// MaskedEditBox.
        /// This property value will not be persisted in the designer. Set the Text property
        /// after initialization through code or use the SetInitialText method to set the
        /// value without raising the TextChanged event.
        /// </remarks>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Do not set the Text property for the MaskedEditBox through the designer.")
        ]
        public override string Text
        {
            get
            {
                if (this.ClipMode == ClipModes.IncludeLiterals || this.returnActualText == true)
                    return this.FormattedText;
                else
                {
                    return this.ClipText;
                }
            }

            set
            {
                if (value == null)
                    value = String.Empty;

                if (this.Initializing)
                {
                    this.initText = value;
                }
                else
                {
                    string oldText = base.Text;
                    try
                    {
                        bool initialTextChangeState = this.internalTextChange;
                        this.internalTextChange = true;
                        this.SetMaskedEditState(MaskedEditState.EditState);
                        this.customFilledPositions.Clear();
                        this.SetExternalText(value, 0, true, true);
                        this.SetMaskedEditState(MaskedEditState.NormalState);
                        this.internalTextChange = false;
                        if (initialTextChangeState == false)
                            this.OnTextChanged(new EventArgs());
                        this.internalTextChange = initialTextChangeState;

                        this.initText = value;
                    }
                    catch
                    {
                        SetBaseText(oldText);
                    }
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Text"));
                }
            }
        }

        /// <summary>
        /// Internal helper function to directly set the base control Text property.
        /// </summary>
        /// <param name="val">The new value.</param>
        private void SetBaseText(string val)
        {
            bool bModeChanged = false;
            bool bPaddingCharChanged = false;
            int oldPaddingCharacterInt = this.PaddingCharacterInt;

            if (this.ClipMode == ClipModes.ExcludeLiterals)
            {
                this.ClipMode = ClipModes.IncludeLiterals;
                bModeChanged = true;
            }

            if (this.PaddingCharacterInt == this.PromptCharacterInt || this.PaddingCharacterInt == this.PassivePromptCharacterInt || this.Text == val)
            {
                this.PaddingCharacterInt = Math.Min(this.PromptCharacterInt, this.PassivePromptCharacterInt) + 1;
                bPaddingCharChanged = true;
            }
            SetTextBoxText(val);
            if (bModeChanged)
                this.ClipMode = ClipModes.ExcludeLiterals;
            if (bPaddingCharChanged)
                this.PaddingCharacterInt = oldPaddingCharacterInt;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected void SetTextBoxText(string text)
        {
            IntPtr textPtr = Marshal.StringToHGlobalAuto(text);
            NativeMethods.SendMessage(this.Handle, NativeMethods.WM_SETTEXT, IntPtr.Zero, textPtr);
            Marshal.FreeHGlobal(textPtr);
        }

        /// <summary>
        /// The UseUserOverride parameter for CultureInfo.
        /// </summary>
        [
        Browsable(true),
        Category("Culture"),
        DefaultValue(true), Description("The UseUserOverride parameter for CultureInfo.")
        ]
        public bool UseUserOverride
        {
            get
            {
                return this.useUserOverride;
            }

            set
            {
                this.useUserOverride = value;
            }
        }


        /// <summary>
        /// See <see cref="Multiline"/> property. The MaskedEditBox does
        /// not support multiline controls.
        /// </summary>
        [
        Browsable(false)
        ]
        public override bool Multiline
        {
            get
            {
                return false;
            }

            set
            {
                //We do not accept this property.
                base.Multiline = false;
            }
        }

        /// <summary>
        /// See <see cref="WordWrap"/>property. The MaskedEditBox does
        /// not support multiline controls.
        /// </summary>
		[Browsable(false), DefaultValue(false)]
        public new bool WordWrap
        {
            get
            {
                return false;
            }
            set
            {
                base.WordWrap = false;
            }
        }

        /// <summary>
        /// See <see cref="ScrollBars"/>property. The MaskedEditBox does
        /// not support multiline controls.
        /// </summary>
        [
        Browsable(false)
        ]
        public new ScrollBars ScrollBars
        {
            get
            {
                return ScrollBars.None;
            }
            set
            {
                base.ScrollBars = ScrollBars.None;
            }
        }

        /// <summary>
        /// See <see cref="Lines"/> property. The MaskedEditBox does
        /// not support multiline controls.
        /// </summary>
        public new string[] Lines
        {
            get
            {
                ArrayList array = new ArrayList();
                array.Add(base.Text);
                return ((string[])(array.ToArray(typeof(string))));
            }

            set
            {
                this.Text = value[0];
            }
        }

        /// <summary>
        /// Overrides <see cref="System.Windows.Forms.Control.OnValidating"/> method.
        /// </summary>
        /// <param name="args">The event data.</param>
        /// <remarks>
        /// Raises the <see cref="ValidationError"/> event when there is unacceptable
        /// text in the control.
        /// </remarks>
        protected override void OnValidating(System.ComponentModel.CancelEventArgs args)
        {
            base.OnValidating(args);
            this.Validate(true);
        }

        /// <summary>
        /// Validates the control.
        /// </summary>
        /// <param name="bRaiseValidationError">Specifies if the validation error is to be raised.</param>
        public void Validate(bool bRaiseValidationError)
        {
            bool bValidateRaised = false;
            char currentPromptChar = '\0';

            if (this.hasFocus == true)
                currentPromptChar = this.GetPromptCharacter();
            else
                currentPromptChar = this.GetPassivePromptCharacter();

            if (this.maskString != null && this.maskString != String.Empty)
            {
                int maskLength = this.maskString.Length;
                char maskChar = '\0';
                MaskCharTypes maskCharType = new MaskCharTypes();
                char displayChar = '\0';

                if (bValidateRaised == false)
                {
                    for (int index = 0; index < maskLength; index++)
                    {
                        // Get the mask character.
                        maskChar = this.maskString[index];
                        if (index < this.DisplayString.Length)
                            displayChar = this.DisplayString[index];
                        else
                            displayChar = currentPromptChar;

                        if (this.IsLiteralCharacter(maskChar) == false)
                        {
                            // A mask character.
                            maskCharType = (MaskCharTypes)this.GetMaskCharValue((char)(maskChar - MaskedEditBox.maskOffset));

                            switch (maskCharType)
                            {
                                case MaskCharTypes.maskCharAplhaNumericRequired:
                                case MaskCharTypes.maskCharDigitRequired:
                                case MaskCharTypes.maskCharCharacterRequired:
                                case MaskCharTypes.maskCharLetterRequired:
                                    {
									if(displayChar == currentPromptChar && !this.AllowPrompt)
                                        {
                                            if (bRaiseValidationError)
                                            {
                                                ValidationErrorArgs arg = new ValidationErrorArgs(this.DisplayString, index, "A required mask character is empty.");
                                                this.OnValidationError(arg);
                                            }
                                            index = maskLength;// To break the loop
                                            bValidateRaised = true;
                                        }
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }

                if (bValidateRaised == false)
                {
                    this.RaiseMaskSatisfied();
                }

                if (this.UsageMode == MaskedUsageMode.Numeric && bRaiseValidationError)
                {
                    bool validate = this.CheckMinMax(this.DisplayString, true);
                    if (validate == false)
                    {
                        ValidationErrorArgs arg = new ValidationErrorArgs(this.DisplayString, 0, "The entered value does not meet the MinValue and MaxValue constraints.");
                        this.OnValidationError(arg);
                    }
                }

            }
        }


        /// <summary>
        /// Returns the display string when the current prompt character is NULL.
        /// </summary>
        /// <param name="internalText">The internal text.</param>
        /// <returns>The new display string.</returns>
        private string GetDisplayStringForNullPrompt(string internalText)
        {
            int charIndex = 0;
            string localDisplayText = String.Empty;
            char localPromptCharacter = this.GetCurrentPromptCharacter();
            int omittedSpacesCount = 0;
            this.listDisplayToInternalString.Clear();

            string localClipText = String.Empty;
            bool stopDisplay = false;

            // Loop through the internal string and substitute each 
            for (charIndex = 0; charIndex < this.maskString.Length; charIndex++)
            {
                if (!IsLiteralCharacter(this.maskString[charIndex]))
                {
                    if (this.IsMaskPositionFilled(internalText, charIndex) == true)// The mask character has been filled
                    {
                        localDisplayText += internalText[charIndex];
                        localClipText += internalText[charIndex];
                        this.listDisplayToInternalString.Add(charIndex, charIndex - omittedSpacesCount);

                        if (localClipText.Length == GetClipLenght(this.ClipText))
                        {
                            stopDisplay = true;
                        }
                    }
                    else//Unfilled mask character:
                    {
                        this.listDisplayToInternalString.Add(charIndex, -1);
                        omittedSpacesCount++;
                    }
                }
                else//Literal character:
                {
                    if (!(this.Sequentially && stopDisplay) ||
                        (omittedSpacesCount == 0 && charIndex <= this.maskString.Length - 1))
                    {
                        localDisplayText += internalText[charIndex];
                    }
                    this.listDisplayToInternalString.Add(charIndex, charIndex - omittedSpacesCount);
                }
            }

            if (this.Sequentially && localClipText.Length == 0)
            {
                localDisplayText = String.Empty;
            }

            return localDisplayText;
        }

        /// <summary>
        /// Gets length of the clean text, text without <see cref="PaddingCharacter"/>, <see cref="PassivePromptCharacter"/>
        /// and <see cref="PromptCharacter"/> characters.
        /// </summary>
        private int GetClipLenght(string clip)
        {
            int clipLenght = 0;

            if (clip != null && clip.Length > 0)
            {
                for (int i = 0; i < clip.Length; ++i)
                {
                    if (clip[i] != this.PaddingCharacter &&
                        clip[i] != this.PassivePromptCharacter &&
                        clip[i] != this.PromptCharacter)
                    {
                        ++clipLenght;
                    }
                }
            }

            return clipLenght;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool IsMaskPositionFilled(string currentString, int index)
        {
            bool b = false;

            if (this.customFilledPositions.ContainsKey(index) == true)
            {
                if (currentString[index] == (char)this.customFilledPositions[index])
                {
                    b = true;
                }
            }

            if (this.lastUsedPromptChar != currentString[index])
                b = true;

            return b;
        }
    }

    /// <summary>
    /// This class is used to send event data for a <see cref="MaskedEditBox.ValidationError"/>
    /// event.
    /// </summary>
    /// <remarks>
    /// The required pieces of information for the <see cref="MaskedEditBox.ValidationError"/> event 
    /// are the invalid text and the position of the error text within the invalid text.
    /// </remarks>
    public class MaskSatisfiedEventArgs : EventArgs
    {
        /// <summary>
        /// The invalid text.
        /// </summary>
        private string invalidText;

        /// <summary>
        /// The start position of the error.
        /// </summary>
        private int startPosition;

        /// <summary>
        /// Creates an object of type ValidationErrorArgs.
        /// </summary>
        /// <param name="invalidText">The invalid text that would have resulted if this error had not been intercepted.</param>
        /// <param name="startPosition">The index position with the invalid text where the change occurred.</param>
        public MaskSatisfiedEventArgs(string invalidText, int startPosition)
        {
            this.invalidText = invalidText;
            this.startPosition = startPosition;
        }

        /// <summary>
        /// Returns the invalid text as it would have been if the error had not intercepted it.
        /// </summary>
        public string InvalidText
        {
            get { return this.invalidText; }
        }

        /// <summary>
        /// Returns the location of the invalid input within the
        /// invalid text.
        /// </summary>
        public int StartPosition
        {
            get { return this.startPosition; }
        }
    }


    /// <summary>
    /// The DataGroup that will specify the behavior and formatting of a 
    /// DataGroup within the MaskedEditBox control.
    /// </summary>
    [
    TypeConverter(typeof(Syncfusion.Windows.Forms.Tools.Design.MaskedEditDataGroupInfoConverter)),
    Serializable(),
    DesignTimeVisible(false),
    ToolboxItem(false)
    ]
    public sealed class MaskedEditDataGroupInfo : Component
    {
        /// <summary>
        /// The text of the column header.
        /// </summary>
        private MaskGroupAlignment dataGroupAlignment;

        /// <summary>
        /// The minimum width for the column header.
        /// </summary>
        private int dataGroupSize;

        /// <summary>
        /// The name of the Datagroup.
        /// </summary>
        private string dataGroupName;

        /// <summary>
        /// The string value of the DataGroup.
        /// </summary>
        private string dataGroupValue = String.Empty;

        /// <summary>
        /// Collection of datagroups.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal MaskedEditDataGroupInfoCollection dataGroups;

        /// <summary>
        /// Gets / sets the collection of datagroups.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public MaskedEditDataGroupInfoCollection DataGroupsCollection
        {
            get
            {
                return dataGroups;
            }
            set
            {
                dataGroups = value;
            }
        }


        /// <summary>
        /// Overloaded. Initializes an object of type MaskedEditDataGroupInfo. 
        /// </summary>
        /// <remarks>
        /// The MaskedEditDataGroupInfo class holds the information needed to intialize 
        /// a data group for the <see cref="MaskedEditBox"/> control.
        /// </remarks>
        public MaskedEditDataGroupInfo()
        {
            this.dataGroupName = String.Empty;
            this.dataGroupSize = 0;
            this.dataGroupAlignment = MaskGroupAlignment.None;
        }


        /// <summary>
        /// Initializes an object of type MaskedEditDataGroupInfo.
        /// </summary>
        /// <param name="dataGroupName">The name of the DataGroup.</param>
        /// <param name="dataGroupSize">The DataGroup size.</param>
        /// <param name="dataGroupAlignment">The DataGroup alignment.</param>
        public MaskedEditDataGroupInfo(string dataGroupName, int dataGroupSize, MaskGroupAlignment dataGroupAlignment)
        {
            this.dataGroupName = dataGroupName;
            this.dataGroupSize = dataGroupSize;
            this.dataGroupAlignment = dataGroupAlignment;
        }

        /// <summary>
        /// PropertyChanged event handler.
        /// </summary>
        [
        Browsable(false),
        Syncfusion.Documentation.DocumentationExclude()
        ]
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="args"></param>
        private void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (this.PropertyChanged != null)
            {
                try
                {
                    PropertyChanged(this, args);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Gets / sets the size of the DataGroup.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance")
        ]
        public int DataGroupSize
        {
            get
            {
                return this.dataGroupSize;
            }

            set
            {
                if (value > 0)
                    this.dataGroupSize = value;
            }
        }

        /// <summary>
        /// Returns the value of the DataGroup.
        /// </summary>
        [
        Browsable(false),
        Category("Data")
        ]
        public string DataGroupValue
        {
            get
            {
                if (this.dataGroups != null)
                {
                    this.dataGroupValue = this.dataGroups.GetDataGroupValue(this);
                    if (this.dataGroupValue == null)
                        this.dataGroupValue = String.Empty;
                }
                else
                {
                    this.dataGroupValue = String.Empty;
                }

                return this.dataGroupValue;
            }
        }


        /// <summary>
        /// Gets / sets the name of the DataGroup.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance")
        ]
        public string DataGroupName
        {
            get
            {
                return this.dataGroupName;
            }

            set
            {
                this.dataGroupName = value;
            }
        }

        /// <summary>
        /// Gets / sets the alignment of the DataGroup.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance")
        ]
        public MaskGroupAlignment DataGroupAlignment
        {
            get
            {
                return this.dataGroupAlignment;
            }

            set
            {
                this.dataGroupAlignment = value;
            }
        }
    }


    /// <summary>
    /// Collection of <see cref="MaskedEditDataGroupInfo"/> objects. Used in the
    /// <see cref="MaskedEditBox.DataGroups"/> property of the <see cref="MaskedEditBox"/>
    /// control.
    /// </summary>
    /// <remarks>The MaskedEditDataGroupInfoCollection is a set of objects 
    /// each of which hold information required to create a data group in a 
    /// <see cref="MaskedEditBox"/>.</remarks>
    [
    Serializable
    ]
    public class MaskedEditDataGroupInfoCollection : CollectionBase
    {
        [Syncfusion.Documentation.DocumentationExclude()]
        private MaskedEditBox owner;

        /// <summary>
        /// Gets / sets the owner of this collection.
        /// </summary>
        public MaskedEditBox Owner
        {
            get
            {
                return owner;
            }
            set
            {
                owner = value;
            }
        }

        /// <summary>
        /// Gets / sets the Indexer property for the MaskedEditDataGroupInfoCollection class.
        /// </summary>
        /// <remarks>Get the <see cref="MaskedEditDataGroupInfo"/> object based on the index
        /// in the collection.</remarks>
        public MaskedEditDataGroupInfo this[int index]
        {
            get
            {
                return (MaskedEditDataGroupInfo)(this.List[index]);
            }

            set
            {
                Insert(index, (MaskedEditDataGroupInfo)value);
            }
        }

        /// <summary>
        /// Gets / sets the Indexer property for the MaskedEditDataGroupInfoCollection class.
        /// </summary>
        /// <remarks>Get the <see cref="MaskedEditDataGroupInfo"/> object based on the index
        /// in the collection.</remarks>
        public MaskedEditDataGroupInfo this[string dataGroupName]
        {
            get
            {
                int index = 0;
                index = GetIndexByName(dataGroupName);
                if (index != -1)
                    return (MaskedEditDataGroupInfo)(this.List[index]);
                else
                    return new MaskedEditDataGroupInfo();
            }

            set
            {
                int index = 0;
                index = GetIndexByName(dataGroupName);
                if (index != -1)
                    Insert(index, (MaskedEditDataGroupInfo)value);
            }
        }

        /// <summary>
        /// Returns the index based on the name.
        /// </summary>
        /// <param name="groupName">The name of the data group.</param>
        /// <returns>Index value.</returns>
        private int GetIndexByName(string groupName)
        {

            foreach (MaskedEditDataGroupInfo info in this.List)
            {
                if (info.DataGroupName == groupName)
                    return this.List.IndexOf(info);
            }

            return -1;
        }

        /// <summary>
        /// Inserts the <see cref="MaskedEditDataGroupInfo"/> into the collection 
        /// at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the item is to be inserted.</param>
        /// <param name="item">The <see cref="MaskedEditDataGroupInfo"/> to be inserted.</param>
        public void Insert(int index, MaskedEditDataGroupInfo item)
        {
            this.List.Insert(index, item);
        }

        /// <summary>
        /// Overrides OnInsert.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void OnInsert(int index, object value)
        {
            base.OnInsert(index, value);
        }


        /// <summary>
        /// Overrides OnInsertComplete.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void OnInsertComplete(int index, object value)
        {
            base.OnInsertComplete(index, value);

            MaskedEditDataGroupInfo item = value as MaskedEditDataGroupInfo;
            item.dataGroups = this;
            if (this.owner != null && this.owner.Parent != null && owner.Parent.Container != null)
                owner.Parent.Container.Add(item);
        }

        /// <summary>
        /// Overloaded. Creates an instance of the MaskedEditDataGroupInfoCollection class.
        /// </summary>
        /// <param name="maskedEdit">The <see cref="MaskedEditBox"/> control that
        /// contains this collection.</param>
        public MaskedEditDataGroupInfoCollection(MaskedEditBox maskedEdit)
        {
            this.owner = maskedEdit;
        }

        /// <summary>
        /// Creates an instance of the MaskedEditDataGroupInfoCollection class.
        /// </summary>
        public MaskedEditDataGroupInfoCollection()
        {
        }

        /// <summary>
        /// Adds one object to the collection.
        /// </summary>
        /// <param name="dataGroup">The <see cref="MaskedEditDataGroupInfo"/> object to be added.</param>
        /// <returns>The count of the list items.</returns>
        public int Add(MaskedEditDataGroupInfo dataGroup)
        {
            if (dataGroup != null)
            {
                dataGroup.dataGroups = this;
                return this.List.Add(dataGroup);
            }
            else
                return -1;
        }

        /// <summary>
        /// Removes <see cref="MaskedEditDataGroupInfo"/> objects from the
        /// collection. 
        /// </summary>
        /// <param name="dataGroup">The MaskedEditDataGroupInfo object to remove.</param>
        public void Remove(MaskedEditDataGroupInfo dataGroup)
        {
            if (this.List.Contains(dataGroup) == true)
            {
                this.List.Remove(dataGroup);
                if (this.owner != null && this.owner.Parent != null)
                    this.owner.Parent.Container.Remove(dataGroup);
                dataGroup.dataGroups = null;
            }
        }


        /// <summary>
        /// Indicates whether the collection contains a specific 
        /// MaskedEditDataGroupInfo entry.
        /// </summary>
        /// <param name="dataGroup">The MaskedEditDataGroupInfo to locate in the DataGroups.</param>
        /// <returns>True if the MaskedEditDataGroupInfo entry is found in the collection; false otherwise.</returns>
        public bool Contains(MaskedEditDataGroupInfo dataGroup)
        {
            return this.List.Contains(dataGroup);
        }

        /// <summary>
        /// Copies all the elements of the current one-dimensional array to the specified one-dimensional array 
        /// starting at the specified destination array index.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(MaskedEditDataGroupInfo[] array, int index)
        {
            this.List.CopyTo(array, index);
        }


        /// <summary>
        /// Returns the value of the DataGroup.
        /// </summary>
        /// <returns>String value of the DataGroup.</returns>
        public string GetDataGroupValue(MaskedEditDataGroupInfo val)
        {
            if (owner == null)
                return "";
            return this.owner.GetDataGroupValue(this, this.List.IndexOf(val));
        }

    }


    /// <summary>
    /// Provides data for an event that can be handled by a subscriber and overrides the events default behavior.
    /// </summary>
    public class MaskCustomValidateArgs : EventArgs
    {
        /// <summary>
        /// Feedback.
        /// </summary>
        private bool handled;
        private bool accepted;

        /// <summary>
        /// Values based on which event handler can set return values.
        /// </summary>
        private int currentIndex;
        private char currentCharacter;
        private char currentMaskCharacter;

        /// <summary>
        /// Overloaded. Initializes a new instance of the MaskCustomValidationArgs class with the Handled property set to False.
        /// </summary>
        public MaskCustomValidateArgs()
        {
            handled = false;
        }

        /// <summary>
        /// Initializes a new instance of the SyncfusionHandledEventArgs class with the Handled and Accepted
        /// properties set to the given value.
        /// </summary>
        public MaskCustomValidateArgs(bool handled, bool accepted)
        {
            this.handled = handled;
            this.accepted = accepted;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentIndex"></param>
        /// <param name="currentCharacter"></param>
        /// <param name="currentMaskCharacter"></param>
        public MaskCustomValidateArgs(int currentIndex, char currentCharacter, char currentMaskCharacter)
        {
            this.currentIndex = currentIndex;
            this.currentCharacter = currentCharacter;
            this.currentMaskCharacter = currentMaskCharacter;

            this.accepted = false;
            this.handled = false;
        }


        /// <summary>
        /// Indicates whether the event has been handled and no further processing of the event should happen.
        /// </summary>
        public bool Handled
        {
            get
            {
                return handled;
            }
            set
            {
                handled = value;
            }
        }

        /// <summary>
        /// Indicates whether the event has been handled and no further processing of the event should happen.
        /// </summary>
        public bool Accepted
        {
            get
            {
                return accepted;
            }
            set
            {
                accepted = value;
            }
        }


        /// <summary>
        /// Returns the current position. It will be a valid mask position.
        /// </summary>
        public int CurrentIndex
        {
            get
            {
                return currentIndex;
            }
        }

        /// <summary>
        /// Returns the current character.
        /// </summary>
        public char CurrentCharacter
        {
            get
            {
                return currentCharacter;
            }
        }

        /// <summary>
        /// Returns the current Mask Character.
        /// </summary>
        public char CurrentMaskCharacter
        {
            get
            {
                return currentMaskCharacter;
            }
        }

    }

    /// <summary>
    /// Represents a method that handles a <see cref="MaskCustomValidateArgs"/> event of a 
    /// <see cref="MaskedEditBox"/>.
    /// </summary>
    public delegate void MaskCustomValidateEventHandler(object sender, MaskCustomValidateArgs e);

    /// <summary>
    /// Specifies the internal state of the MaskedEditBox.
    /// </summary>
    internal enum MaskedEditState
    {
        /// <summary>
        /// Normal State.
        /// </summary>
        NormalState = 0,
        /// <summary>
        /// Edit State.
        /// </summary>
        EditState,
        /// <summary>
        /// Initial State.
        /// </summary>
        None
    }

}
