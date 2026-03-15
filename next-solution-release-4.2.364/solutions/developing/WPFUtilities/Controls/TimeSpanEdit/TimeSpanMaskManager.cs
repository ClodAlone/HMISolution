using DevExpress.Data.Mask;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WPFUtilities.Controls
{
    public class TimeSpanMaskManager : MaskManagerSelectAllEnhancer<TimeSpanMaskManagerCore> {
        public TimeSpanMaskManager(TimeSpanMaskManagerCore coreManager)
            : base(coreManager) { }
        public TimeSpanMaskManager(string mask, bool isOperatorMask, CultureInfo culture, bool allowNull)
            : this(new TimeSpanMaskManagerCore(mask, isOperatorMask, culture, allowNull)) { }

        protected override bool IsNestedCanSelectAll { get { return false; } }
        public static bool DoNotClearValueOnInsertAfterSelectAll = false;
        protected override bool MakeChange(Func<bool> changeWithTrueWhenSuccessfull) {
            if (IsSelectAllEnforced) {
                ClearSelectAllFlag();
                if (!DoNotClearValueOnInsertAfterSelectAll) {
                    Nested.ClearFromSelectAll();
                }
                base.MakeChange(changeWithTrueWhenSuccessfull);
                return true;
            }
            else {
                return base.MakeChange(changeWithTrueWhenSuccessfull);
            }
        }
        protected override bool MakeCursorOp(Func<bool> cursorOpWithTrueWhenSuccessfull) {
            if (IsSelectAllEnforced) {
                ClearSelectAllFlag();
                base.MakeCursorOp(cursorOpWithTrueWhenSuccessfull);
                return true;
            }
            else {
                return base.MakeCursorOp(cursorOpWithTrueWhenSuccessfull);
            }
        }
        public override bool Backspace() {
            if (IsSelectAllEnforced) {
                ClearSelectAllFlag();
                Nested.ClearFromSelectAll();
                return true;
            }
            else {
                return base.Backspace();
            }
        }
        public override bool Delete() {
            if (IsSelectAllEnforced) {
                ClearSelectAllFlag();
                Nested.ClearFromSelectAll();
                return true;
            }
            else {
                return base.Delete();
            }
        }
    }

    public class TimeSpanMaskManagerCore : MaskManager {
        protected readonly bool IsOperatorMask;
        protected TimeSpan? fCurrentValue;
        protected TimeSpan? fUndoValue;

        protected TimeSpanElementEditor fCurrentElementEditor;
        protected TimeSpanMaskFormatInfo fFormatInfo;
        protected internal TimeSpanMaskFormatInfo FormatInfo { get { return fFormatInfo; } }

        protected TimeSpan? CurrentValue { get { return fCurrentValue; } }
        protected TimeSpan? UndoValue { get { return fUndoValue; } }

        protected readonly CultureInfo MaskCulture;

        public TimeSpanMaskManagerCore(string mask, bool isOperatorMask, CultureInfo culture, bool allowNull) {
            this.AllowNull = allowNull;
            this.IsOperatorMask = isOperatorMask;
            this.fInitialMask = mask;
            this.MaskCulture = culture;
            fFormatInfo = new TimeSpanMaskFormatInfo(mask, MaskCulture);
            CursorHome(false);
        }
        
        protected TimeSpan? fInitialEditValue;
        protected int fSelectedFormatInfoIndex;
        protected string fInitialMask;
        protected readonly bool AllowNull = true;

        protected TimeSpan NonEmptyCurrentValue {
            get {
                if (CurrentValue.HasValue)
                    return CurrentValue.Value;
                else
                    return TimeSpan.FromTicks(1); //TimeSpan.Zero; // This does not allow to enter minus into an empty editor
            }
        }
        protected TimeSpan NonEmptyCurrentPositiveValue {
            get { return IsNegativeValue ? NonEmptyCurrentValue.Negate() : NonEmptyCurrentValue; }
        }
        protected bool IsNegativeValue { get { return NonEmptyCurrentValue.Ticks < 0; } }
        protected int SelectedFormatInfoIndex { get { return fSelectedFormatInfoIndex; } }
        protected TimeSpanMaskFormatElementEditable SelectedElement {
            get {
                if (SelectedFormatInfoIndex < 0)
                    return null;
                return (TimeSpanMaskFormatElementEditable)FormatInfo[SelectedFormatInfoIndex];
            }
        }
        protected bool IsElementEdited { get { return fCurrentElementEditor != null; } }
        protected TimeSpanElementEditor GetCurrentElementEditor() {
            if (SelectedElement != null) {
                bool canModify = RaiseModifyWithoutEditValueChange();
                if (!IsElementEdited && canModify) {
                    fCurrentElementEditor = SelectedElement.CreateElementEditor(NonEmptyCurrentPositiveValue);
                }
            }
            return fCurrentElementEditor;
        }
        protected void KillCurrentElementEditor() {
            fCurrentElementEditor = null;
        }
        protected bool ApplyCurrentElementEditor(bool raiseTextChanged = true, bool killCurrentElementEditor = true) {
            if (!IsElementEdited)
                return false;
            long editorResult = GetCurrentElementEditor().GetResult();
            if (killCurrentElementEditor) KillCurrentElementEditor();
            
            TimeSpan newValue = SelectedElement.ApplyElement(editorResult, NonEmptyCurrentPositiveValue);
            return ApplyNewValue(newValue, raiseTextChanged);
        }
        public override string GetCurrentEditText() {
            if (CurrentValue.HasValue)
                return CurrentValue.Value.ToString("c", CultureInfo.InvariantCulture);
            else
                return string.Empty;
        }
        public override void SetInitialEditText(string initialEditText) {
            KillCurrentElementEditor();
            TimeSpan? initialEditValue = null;
            if (!string.IsNullOrEmpty(initialEditText)) {
                try {
                    initialEditValue = TimeSpan.Parse(initialEditText, CultureInfo.InvariantCulture);
                }
                catch { }
            }
            SetInitialEditValue(initialEditValue);
        }
        TimeSpan cachedValue = TimeSpan.Zero;
        int cachedIndex = -1;
        int cachedDCP = -1;
        int cachedDSA = -1;
        string cachedDT = null;
        void VerifyCache() {
            if (!CurrentValue.HasValue)
                throw new InvalidOperationException();
            if (cachedValue != CurrentValue.Value || SelectedFormatInfoIndex != cachedIndex) {
                cachedIndex = SelectedFormatInfoIndex;
                cachedDCP = -1;
                cachedDSA = -1;
            }
            if (cachedValue != CurrentValue.Value) {
                cachedValue = CurrentValue.Value;
                cachedDT = null;
            }
        }
        public override string DisplayText {
            get {
                if (!CurrentValue.HasValue) {
                    return string.Empty;
                }

                if (IsElementEdited) {
                    var currentEditGroup = SelectedElement != null ? SelectedElement.EditGroup : null;
                    bool isNegative = IsNegativeValue;
                    return FormatInfo.Format(NonEmptyCurrentPositiveValue, 0, SelectedFormatInfoIndex - 1, isNegative, currentEditGroup) + GetCurrentElementEditor().DisplayText + FormatInfo.Format(NonEmptyCurrentPositiveValue, SelectedFormatInfoIndex + 1, FormatInfo.Count - 1, isNegative, currentEditGroup);
                }
                else if (!CurrentValue.HasValue) {
                    return string.Empty;
                }
                else {
                    VerifyCache();
                    if (cachedDT == null) {
                        cachedDT = FormatInfo.Format(NonEmptyCurrentPositiveValue, IsNegativeValue);
                    }
                    return cachedDT;
                }
            }
        }
        public override int DisplayCursorPosition {
            get {
                if (SelectedElement == null) {
                    return 0;
                }

                var currentEditGroup = SelectedElement != null ? SelectedElement.EditGroup : null;

                if (IsElementEdited) {
                    return FormatInfo.Format(NonEmptyCurrentPositiveValue, 0, SelectedFormatInfoIndex - 1, IsNegativeValue, currentEditGroup).Length + GetCurrentElementEditor().DisplayText.Length;
                }
                else if (!CurrentValue.HasValue) {
                    return 0;
                }
                else {
                    VerifyCache();
                    if (cachedDCP < 0) {
                        cachedDCP = FormatInfo.Format(NonEmptyCurrentPositiveValue, 0, SelectedFormatInfoIndex, IsNegativeValue, currentEditGroup).Length;
                    }
                    return cachedDCP;
                }
            }
        }
        public override int DisplaySelectionAnchor {
            get {
                if (SelectedElement == null) {
                    return 0;
                }

                var currentEditGroup = SelectedElement != null ? SelectedElement.EditGroup : null;

                if (IsElementEdited) {
                    return FormatInfo.Format(NonEmptyCurrentPositiveValue, 0, SelectedFormatInfoIndex - 1, IsNegativeValue, currentEditGroup).Length;
                }
                if (!CurrentValue.HasValue) {
                    return 0;
                }
                VerifyCache();
                if (cachedDSA < 0) {
                    cachedDSA = FormatInfo.Format(NonEmptyCurrentPositiveValue, 0, SelectedFormatInfoIndex - 1, IsNegativeValue, currentEditGroup).Length;
                }
                return cachedDSA;
            }
        }
        public override bool Insert(string insertion) {
            if (!IsElementEdited && insertion.Length > 3 && !System.Text.RegularExpressions.Regex.IsMatch(insertion, @"^(\d+|\p{L}+)$")) {
                try {
                    TimeSpan newValue = TimeSpan.ParseExact(insertion, fInitialMask, CultureInfo.InvariantCulture);
                    if (newValue == CurrentValue)
                        return false;
                    if (!RaiseEditTextChanging(newValue))
                        return false;
                    fUndoValue = CurrentValue;
                    fCurrentValue = newValue;
                    RaiseEditTextChanged();
                    return true;
                }
                catch {
                }
            }

            TimeSpanElementEditor currentEditor = GetCurrentElementEditor();
            bool wasEdited = IsElementEdited;
            if (currentEditor == null)
                return false;

            if (insertion.StartsWith(TimeSpanElementEditor_Sign.SignDesignator)) {
                var previousElementIndex = LeftEditableCursorIndex();
                if (previousElementIndex != -1 && previousElementIndex != SelectedFormatInfoIndex && FormatInfo[previousElementIndex] is TimeSpanMaskFormatElement_Sign && CursorLeft(true)) {
                    currentEditor = GetCurrentElementEditor();
                    wasEdited = IsElementEdited;
                }
            }

            if (currentEditor.ShouldSkipInsert(insertion) && CursorRight(true)) {
                currentEditor = GetCurrentElementEditor();
                wasEdited = IsElementEdited;
            }

            if (currentEditor.Insert(insertion)) {
                if (IsOperatorMask && currentEditor.FinalOperatorInsert) {
                    CursorRight(true);
                }
                if (String.IsNullOrEmpty(DisplayText))
                    ApplyCurrentElementEditor(true, false);
                return true;
            }
            else if (insertion == " ") {
                if (!CurrentValue.HasValue && !wasEdited) {
                    return ApplyCurrentElementEditor();
                }
                else {
                    return CursorRight(true);
                }
            }
            else if (IsNextSeparatorSkipInput(insertion))
                return CursorRight(false);
            else {
                if (!wasEdited)
                    KillCurrentElementEditor();
                return false;
            }
        }
        bool IsNextSeparatorSkipInput(string insertion) {
            if (insertion.Length <= 0)
                return false;
            if (SelectedFormatInfoIndex + 1 >= FormatInfo.Count)
                return false;
            if (FormatInfo[SelectedFormatInfoIndex + 1].Editable)
                return false;
            var nextSeparator = FormatInfo[SelectedFormatInfoIndex + 1].Format(NonEmptyCurrentPositiveValue, IsNegativeValue);
            return nextSeparator.StartsWith(insertion) || (insertion == "," && nextSeparator.StartsWith("."));
        }
        public override bool Delete() {
            return BackspaceOrDelete();
        }
        public override bool Backspace() {
            return BackspaceOrDelete();
        }
        bool BackspaceOrDelete() {
            if (!CurrentValue.HasValue && !IsElementEdited && AllowNull)
                return false;
            TimeSpanElementEditor currentEditor = GetCurrentElementEditor();
            if (currentEditor == null)
                return false;
            else
                return currentEditor.Delete();
        }
        public override bool CanUndo {
            get {
                return CurrentValue != UndoValue || IsElementEdited;
            }
        }
        public override bool Undo() {
            if (IsElementEdited) {
                KillCurrentElementEditor();
            }
            else {
                if (CurrentValue == UndoValue)
                    return false;
                if (!RaiseEditTextChanging(UndoValue))
                    return false;
                TimeSpan? temp = CurrentValue;
                fCurrentValue = UndoValue;
                fUndoValue = temp;
                RaiseEditTextChanged();
            }
            return true;
        }
        int GetFormatIndexFromPosition(int position) {
            int lastElement = -1;
            for (int i = 0; i < FormatInfo.Count; ++i) {
                if (FormatInfo[i].Editable) {
                    var currentEditGroup = SelectedElement != null ? SelectedElement.EditGroup : null;
                    if (FormatInfo.Format(NonEmptyCurrentPositiveValue, 0, i, IsNegativeValue, currentEditGroup).Length >= position) {
                        return i;
                    }
                    lastElement = i;
                }
            }
            return lastElement;
        }
        public override bool CursorToDisplayPosition(int newPosition, bool forceSelection) {
            ApplyCurrentElementEditor();
            if (!CurrentValue.HasValue)
                return false;
            int toIndex = GetFormatIndexFromPosition(newPosition);
            if (toIndex < 0)
                return false;
            if (forceSelection && toIndex > SelectedFormatInfoIndex)
                return false;
            fSelectedFormatInfoIndex = toIndex;
            return true;
        }

        public override bool CursorMoveNear(bool forceSelection, bool isNeededKeyCheck) {
            bool result = false;
            if (IsElementEdited) {
                if (!isNeededKeyCheck) {
                    result = true;
                    ApplyCurrentElementEditor();
                }
            }
            else if (!CurrentValue.HasValue) {
                return false;
            }
            for (int i = SelectedFormatInfoIndex - 1; i >= 0; --i) {
                if (FormatInfo[i].Editable) {
                    if (!isNeededKeyCheck) {
                        fSelectedFormatInfoIndex = i;
                    }
                    return true;
                }
            }
            return result;
        }
        public override bool CursorMoveFar(bool forceSelection, bool isNeededKeyCheck) {
            bool result = false;
            if (IsElementEdited) {
                if (!isNeededKeyCheck) {
                    result = true;
                    ApplyCurrentElementEditor();
                }
            }
            else if (!CurrentValue.HasValue) {
                return false;
            }
            for (int i = SelectedFormatInfoIndex + 1; i < FormatInfo.Count; ++i) {
                if (FormatInfo[i].Editable) {
                    if (!isNeededKeyCheck) {
                        fSelectedFormatInfoIndex = i;
                    }
                    return true;
                }
            }
            return result;
        }

        protected virtual int LeftEditableCursorIndex() {
            for (int i = SelectedFormatInfoIndex - 1; i >= 0; --i) {
                if (FormatInfo[i].Editable) {
                    return i;
                }
            }
            return FormatInfo[SelectedFormatInfoIndex].Editable ? SelectedFormatInfoIndex : -1;
        }
        protected virtual int RightEditableCursorIndex() {
            for (int i = SelectedFormatInfoIndex + 1; i < FormatInfo.Count; ++i) {
                if (FormatInfo[i].Editable) {
                    return i;
                }
            }
            return FormatInfo[SelectedFormatInfoIndex].Editable ? SelectedFormatInfoIndex : -1;
        }
        protected virtual bool IsFirstEditable {
            get {
                var previousIndex = LeftEditableCursorIndex();
                return previousIndex == -1 || previousIndex == fSelectedFormatInfoIndex;
            }
        }
        protected virtual bool IsLastEditable {
            get {
                var previousIndex = RightEditableCursorIndex();
                return previousIndex == -1 || previousIndex == fSelectedFormatInfoIndex;
            }
        }
        public override bool CursorHome(bool forceSelection) {
            ApplyCurrentElementEditor();
            fSelectedFormatInfoIndex = -1;
            for (int i = 0; i < FormatInfo.Count; ++i) {
                if (FormatInfo[i].Editable) {
                    fSelectedFormatInfoIndex = i;
                    break;
                }
            }
            return true;
        }
        public override bool CursorEnd(bool forceSelection) {
            ApplyCurrentElementEditor();
            if (!CurrentValue.HasValue)
                return CursorHome(forceSelection);
            fSelectedFormatInfoIndex = -1;
            for (int i = FormatInfo.Count - 1; i >= 0; --i) {
                if (FormatInfo[i].Editable) {
                    fSelectedFormatInfoIndex = i;
                    break;
                }
            }
            return true;
        }
        public override bool SpinUp() {
            TimeSpanElementEditor currentEditor = GetCurrentElementEditor();
            if (currentEditor == null)
                return false;
            else {
                var result = currentEditor.SpinUp();
                if (result && currentEditor.IsMinimum) {
                    ApplyCurrentElementEditor(false);
                    var newValue = SelectedElement.StepUp(NonEmptyCurrentPositiveValue, IsNegativeValue);
                    ApplyNewValue(newValue);
                }
                return result;
            }
        }
        public override bool SpinDown() {
            TimeSpanElementEditor currentEditor = GetCurrentElementEditor();
            if (currentEditor == null)
                return false;
            else {
                var result = currentEditor.SpinDown();
                if (result && currentEditor.IsMaximum) {
                    ApplyCurrentElementEditor(false);
                    var newValue = SelectedElement.StepDown(NonEmptyCurrentPositiveValue, IsNegativeValue);
                    ApplyNewValue(newValue);
                }
                return result;
            }
        }
        public override bool FlushPendingEditActions() {
            return ApplyCurrentElementEditor();
        }
        public override object GetCurrentEditValue() {
            return CurrentValue;
        }
        public void SetInitialEditValue(TimeSpan? initialEditValue) {
            KillCurrentElementEditor();
            fCurrentValue = initialEditValue;
            fUndoValue = initialEditValue;
            fInitialEditValue = initialEditValue;
            CursorHome(false);
        }
        public override void SetInitialEditValue(object initialEditValue) {
            if (initialEditValue is TimeSpan || initialEditValue == null) {
                SetInitialEditValue((TimeSpan?)initialEditValue);
            }
            else if (initialEditValue is TimeSpan) {
                SetInitialEditValue(new TimeSpan(((TimeSpan)initialEditValue).Ticks));
            }
            else {
                SetInitialEditText(string.Format(CultureInfo.InvariantCulture, "{0}", initialEditValue));
            }
        }
        
        public override void SelectAll() {
            CursorHome(false);
        }
        public void ClearFromSelectAll() {
            KillCurrentElementEditor();
            TimeSpan? newValue = null;
            if (!AllowNull)
                newValue = TimeSpan.Zero;
            if (newValue != CurrentValue) {
                if (RaiseEditTextChanging(newValue)) {
                    this.fUndoValue = CurrentValue;
                    this.fCurrentValue = newValue;
                    RaiseEditTextChanged();
                }
            }
            CursorHome(false);
        }

        protected bool ApplyNewValue(TimeSpan newValue, bool raiseTextChanged = true) {
            if (!SelectedElement.SupportsNegative && IsNegativeValue)
                newValue = newValue.Negate();

            if (CurrentValue != newValue) {
                if (!RaiseEditTextChanging(newValue))
                    return false;
                fUndoValue = CurrentValue;
                fCurrentValue = newValue;
                RaiseEditTextChanged();
            }
            return true;
        }
    }

    public class TimeSpanMaskFormatInfo : IEnumerable<TimeSpanMaskFormatElement> {
        public int Count { get { return innerList.Count; } }
        IEnumerator<TimeSpanMaskFormatElement> IEnumerable<TimeSpanMaskFormatElement>.GetEnumerator() {
            return innerList.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return innerList.GetEnumerator();
        }
        public TimeSpanMaskFormatElement this[int index] {
            get {
                return innerList[index];
            }
        }
        protected readonly IList<TimeSpanMaskFormatElement> innerList;
        protected DisplayTextProvider DisplayTextProvider;
        static int GetGroupLength(string mask) {
            for (int i = 1; i < mask.Length; ++i) {
                if (mask[i] != mask[0])
                    return i;
            }
            return mask.Length;
        }
        
        static IList<TimeSpanMaskFormatElement> ParseFormatString(string mask, IFormatProvider formatProvider) {
            List<TimeSpanMaskFormatElement> result = new List<TimeSpanMaskFormatElement>();
            string work = mask;
            bool isFirst = true;
            int optionalModeDepth = 0;

            while (work.Length > 0) {
                int elementLength = GetGroupLength(work);
                TimeSpanMaskFormatElement element = null;
                
                switch (work[0]) {
                    case '-':
                        if (isFirst && elementLength == 1)
                            element = new TimeSpanMaskFormatElement_Sign("-");
                        else {
                            elementLength = 1;
                            element = new TimeSpanMaskFormatElementLiteral("-");
                        }
                        break;
                    case 'd':
                    case 'D':
                        if (isFirst && elementLength == 1)
                            element = new TimeSpanMaskFormatElement_First(work.Substring(0, elementLength), MaskPart.Days);
                        else
                            element = new TimeSpanMaskFormatElement_D(work.Substring(0, elementLength));
                        isFirst = false;
                        break;
                    case 'h':
                    case 'H':
                        if (isFirst && elementLength == 1)
                            element = new TimeSpanMaskFormatElement_First(work.Substring(0, elementLength), MaskPart.Hours);
                        else
                            element = new TimeSpanMaskFormatElement_H(work.Substring(0, elementLength));
                        isFirst = false;
                        break;
                    case 'm':
                    case 'M':
                        if (isFirst && elementLength == 1)
                            element = new TimeSpanMaskFormatElement_First(work.Substring(0, elementLength), MaskPart.Minutes);
                        else
                            element = new TimeSpanMaskFormatElement_M(work.Substring(0, elementLength));
                        isFirst = false;
                        break;
                    case 's':
                    case 'S':
                        if (isFirst && elementLength == 1)
                            element = new TimeSpanMaskFormatElement_First(work.Substring(0, elementLength), MaskPart.Seconds);
                        else
                            element = new TimeSpanMaskFormatElement_S(work.Substring(0, elementLength));
                        isFirst = false;
                        break;
                    case 'f':
                    case 'F':
                        if (isFirst && elementLength == 1)
                            element = new TimeSpanMaskFormatElement_First(work.Substring(0, elementLength), MaskPart.Fractional);
                        else
                            element = new TimeSpanMaskFormatElement_F(work.Substring(0, elementLength));
                        isFirst = false;
                        break;
                    case '[':
                        optionalModeDepth++;
                        element = new TimeSpanMaskFormatElementStartGroup();
                        elementLength = 1;
                        break;
                    case ']':
                        optionalModeDepth--;
                        element = new TimeSpanMaskFormatElementEndGroup();
                        elementLength = 1;
                        break;
                    case '.':
                    case ':':
                    case '/':
                        elementLength = 1;
                        element = new TimeSpanMaskFormatElementNonEditable(work.Substring(0, 1));
                        break;
                    case '"':
                    case '\'':
                        string escaped = work.Replace(@"\\", @"--").Replace(@"\" + work[0], @"--");
                        int closingQuotePosition = escaped.IndexOf(work[0], 1);
                        if (closingQuotePosition <= 0)
                            throw new ArgumentException();
                        string quotedFormat = work.Substring(0, closingQuotePosition + 1);
                        string formattedQuotedText = TimeSpan.MinValue.ToString(quotedFormat);
                        element = new TimeSpanMaskFormatElementLiteral(formattedQuotedText);
                        elementLength = closingQuotePosition + 1;
                        break;
                    case '\\':
                        if (work.Length >= 2) {
                            element = new TimeSpanMaskFormatElementLiteral(work.Substring(1, 1));
                            elementLength = 2;
                        }
                        else
                            throw new ArgumentException(MaskExceptionsTexts.IncorrectMaskBackslashBeforeEndOfMask);
                        break;
                    default:
                        elementLength = 1;
                        element = new TimeSpanMaskFormatElementLiteral(work.Substring(0, 1));
                        break;
                }
                if (element != null)
                    result.Add(element);
                work = work.Substring(elementLength);
            }

            if (optionalModeDepth != 0)
                throw new ArgumentException(MaskExceptionsTexts.IncorrectMaskClosingSquareBracketExpected);

            return result;
        }
        static string ExpandFormat(string format, IFormatProvider formatProvider) {
            if (format == null || format.Length == 0)
                format = "c";

            if (format.Length == 1) {
                switch (format[0]) {
                    case 'c':
                    case 't':
                    case 'T':
                        return "[-][d.]hh:mm:ss[.fffffff]";
                    case 'g':
                        return "[-][d:]h:mm:ss[" + NumberFormatInfo.GetInstance(formatProvider).NumberDecimalSeparator + "FFFFFFF]";
                    case 'G':
                        return "[-]d:hh:mm:ss" + NumberFormatInfo.GetInstance(formatProvider).NumberDecimalSeparator + "fffffff";
                }
            }
            if (format.Length == 2 && format[0] == '%')
                format = format.Substring(1);
            return format;
        }
        public TimeSpanMaskFormatInfo(string mask, IFormatProvider formatProvider) {
            string expandedMask = ExpandFormat(mask, formatProvider);
            innerList = ParseFormatString(expandedMask, formatProvider);
            DisplayTextProvider = CreateDisplayTextProvider();
        }
        public string Format(TimeSpan formatted, bool isNegative) {
            return Format(formatted, 0, this.Count - 1, isNegative, null);
        }
        public string Format(TimeSpan formatted, int startFormatIndex, int endFormatIndex, bool isNegative, object currentEditGroup) {
            var builderResult = DisplayTextProvider.GetDisplayText(formatted, isNegative, currentEditGroup);
            return string.Join("", builderResult.Skip(startFormatIndex).Take(endFormatIndex - startFormatIndex + 1));
        }

        protected virtual DisplayTextProvider CreateDisplayTextProvider() {
            var providers = new List<DisplayTextProvider> { new DisplayTextProvider(null, false) };
            var currentProvider = providers[0];
            
            for (int i = 0; i <= this.Count - 1; ++i) {
                DisplayTextProvider newProvider = null;

                if (this[i] is TimeSpanMaskFormatElementStartGroup) {
                    newProvider = new DisplayTextProvider(null, true);
                    currentProvider.NestedProviders.Add(newProvider);
                    providers.Add(newProvider);
                    currentProvider = newProvider;

                    newProvider = new DisplayTextProvider(this[i], true);
                    currentProvider.NestedProviders.Add(newProvider);
                }
                else if (this[i] is TimeSpanMaskFormatElementEndGroup) {
                    newProvider = new DisplayTextProvider(this[i], providers.Count > 1);
                    currentProvider.NestedProviders.Add(newProvider);

                    providers.RemoveAt(providers.Count - 1);
                    currentProvider = providers.Last();
                }
                else {
                    newProvider = new DisplayTextProvider(this[i], providers.Count > 1);
                    this[i].EditGroup = newProvider.GetHashCode();
                    currentProvider.NestedProviders.Add(newProvider);
                }

            }

            return providers[0];
        }
    }

    public class DisplayTextProvider {
        public TimeSpanMaskFormatElement Element { get; private set; }
        public bool IsOptional { get; private set; }
        public List<DisplayTextProvider> NestedProviders { get; private set; }
        protected List<string> Result;

        public DisplayTextProvider(TimeSpanMaskFormatElement element, bool isOptional) {
            Element = element;
            IsOptional = isOptional;
            NestedProviders = new List<DisplayTextProvider>();
            Result = new List<string>();
        }
        public virtual List<string> GetDisplayText(TimeSpan formatted, bool isNegative, object currentEditGroup) {
            Result.Clear();

            var hasValues = !IsOptional;
            if (Element != null)
                Result.Add(Element.Format(formatted, isNegative));
            else {
                foreach (var provider in NestedProviders) {
                    var unit = provider.GetDisplayText(formatted, isNegative, currentEditGroup);
                    if (!hasValues && provider.Element != null && provider.Element.Editable && (unit[0].Any(c => c != '0') || currentEditGroup != null && provider.Element.EditGroup == currentEditGroup))
                        hasValues = true;
                    Result.AddRange(unit);
                }

                if (!hasValues) {
                    var length = Result.Count;
                    Result.Clear();
                    Result.AddRange(new string[length]);
                }
                hasValues = !IsOptional;
            }

            return Result;
        }
    }
}
