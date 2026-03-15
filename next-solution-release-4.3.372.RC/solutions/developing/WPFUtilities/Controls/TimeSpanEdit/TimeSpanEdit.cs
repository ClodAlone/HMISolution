using DevExpress.Data.Mask;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Services;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Settings.Extension;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace WPFUtilities.Controls
{
    // TimeSpan masks - https://msdn.microsoft.com/en-us/library/ee372286(v=vs.110).aspx
    public class TimeSpanEdit : ButtonEdit
    {
        #region Dependency Properties
        public static readonly DependencyProperty TimeSpanProperty;
        public static readonly DependencyProperty MinValueProperty;
        public static readonly DependencyProperty MaxValueProperty;
        #endregion

        #region Properties
        protected new TimeSpanEditStrategy EditStrategy {
            get { return base.EditStrategy as TimeSpanEditStrategy; }
        }
        protected new TimeSpanEditSettings Settings { get { return (TimeSpanEditSettings)base.Settings; } }

        protected override MaskType DefaultMaskType { get { return (MaskType)7; } }

        public TimeSpan TimeSpan {
            get { return (TimeSpan)GetValue(TimeSpanProperty); }
            set { SetValue(TimeSpanProperty, value); }
        }
        public TimeSpan? MaxValue {
            get { return (TimeSpan?)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public TimeSpan? MinValue {
            get { return (TimeSpan?)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        #endregion

        #region Constructors
        static TimeSpanEdit() {
            Type ownerType = typeof(TimeSpanEdit);
            //DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));

            TimeSpanProperty = DependencyPropertyManager.Register("TimeSpan", typeof(TimeSpan), ownerType,
                new FrameworkPropertyMetadata(TimeSpan.Zero,
                                              FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                              new PropertyChangedCallback(OnTimeSpanPropertyChanged),
                                              new CoerceValueCallback(OnCoerceTimeSpanProperty),
                                              true,
                                              UpdateSourceTrigger.LostFocus));

            MinValueProperty = DependencyPropertyManager.RegisterAttached("MinValue", typeof(TimeSpan?), ownerType,
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnMinValueChanged)));
            MaxValueProperty = DependencyPropertyManager.RegisterAttached("MaxValue", typeof(TimeSpan?), ownerType,
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnMaxValueChanged)));

            //DisplayFormatStringProperty.AddOwner(ownerType, new FrameworkPropertyMetadata("c")); //Replaced with MaskUseAsDisplayFormat
            MaskTypeProperty.AddOwner(ownerType, new FrameworkPropertyMetadata((MaskType)7)); //There is no MaskType for TimeSpan
            MaskProperty.AddOwner(ownerType, new FrameworkPropertyMetadata("c"));
            AllowNullInputProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(true));
            MaskUseAsDisplayFormatProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(true));
        }
        #endregion

        protected static object OnCoerceTimeSpanProperty(DependencyObject obj, object value) {
            return ((TimeSpanEdit)obj).CoerceTimeSpanProperty(value);
        }
        protected static void OnTimeSpanPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            ((TimeSpanEdit)obj).OnTimeSpanChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        protected virtual object CoerceTimeSpanProperty(object value) {
            return EditStrategy.CoerceTimeSpan(value);
        }

        protected virtual void OnTimeSpanChanged(TimeSpan oldValue, System.TimeSpan newValue) {
            EditStrategy.TimeSpanChanged(oldValue, newValue);
        }

        protected static void OnMinValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            ((TimeSpanEdit)obj).OnMinValueChanged((TimeSpan?)e.NewValue);
        }
        protected static void OnMaxValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            ((TimeSpanEdit)obj).OnMaxValueChanged((TimeSpan?)e.NewValue);
        }
        protected virtual void OnMinValueChanged(TimeSpan? value) {
            EditStrategy.MinValueChangedEx(value);
        }
        protected virtual void OnMaxValueChanged(TimeSpan? value) {
            EditStrategy.MaxValueChangedEx(value);
        }
        protected override MaskType[] GetSupportedMaskTypes() {
            return new[] { (MaskType)7 };
        }

        protected override BaseEditSettings CreateEditorSettings() {
            return new TimeSpanEditSettings();
        }
        protected override EditStrategyBase CreateEditStrategy() {
            return new TimeSpanEditStrategy(this);
        }

        protected override ActualPropertyProvider CreateActualPropertyProvider() {
            return new TimeSpanEditPropertyProvider(this);
        }

        protected override TextInputSettingsBase CreateTextInputSettings() {
            return new TimeSpanMaskSettings(this);
        }
    }

    public class TimeSpanEditPropertyProvider : ButtonEditPropertyProvider {
        private new TimeSpanEdit Editor { get { return (TimeSpanEdit)base.Editor; } }
        public TimeSpanEditPropertyProvider(TimeSpanEdit editor) : base(editor) { }
    }

    public class TimeSpanEditStrategy : RangeEditorStrategy<TimeSpan> {
        public override bool ShouldRoundToBounds { get { return false; } }

        new TimeSpanEdit Editor { get { return (TimeSpanEdit)base.Editor; } }
        public TimeSpanEditStrategy(TimeSpanEdit editor)
            : base(editor) {
        }
        protected override void RegisterUpdateCallbacks() {
            base.RegisterUpdateCallbacks();
            PropertyUpdater.Register(TimeSpanEdit.TimeSpanProperty, baseValue => baseValue, Correct);
        }
        protected override object GetDefaultValue() {
            return TimeSpan.Zero;
        }
        public override object CoerceMaskType(MaskType maskType) {
            return (MaskType)7;
        }
        public virtual object CoerceTimeSpan(object baseValue) {
            return CoerceValue(TimeSpanEdit.TimeSpanProperty, CreateValueConverter(baseValue).Value);
        }
        public virtual void TimeSpanChanged(TimeSpan oldTimeSpan, TimeSpan TimeSpan) {
            if (ShouldLockUpdate)
                return;
            SyncWithValue(TimeSpanEdit.TimeSpanProperty, oldTimeSpan, TimeSpan);
        }
        public void SetTimeSpan(TimeSpan editValue, UpdateEditorSource updateEditorSource) {
            ValueContainer.SetEditValue(editValue, updateEditorSource);
            TextInputService.SetInitialEditValue(editValue);
        }
        protected override EditorSpecificValidator CreateEditorValidatorService() {
            return new TimeSpanEditValidator(Editor);
        }
        protected override RangeEditorService CreateRangeEditService() {
            return new TimeSpanEditRangeService(Editor);
        }

        protected override DevExpress.Xpf.Editors.Native.MinMaxUpdateHelper CreateMinMaxHelper() {
            return new MinMaxUpdateHelper(Editor, TimeSpanEdit.MinValueProperty, DateEdit.MaxValueProperty);
        }

        protected override TimeSpan GetMaxValue() {
            if (Editor.MaxValue.HasValue)
                return Editor.MaxValue.Value;
            return TimeSpan.MaxValue;
        }

        protected override TimeSpan GetMinValue() {
            if (Editor.MinValue.HasValue)
                return Editor.MinValue.Value;
            return TimeSpan.MinValue;
        }

        protected internal void MaxValueChangedEx(TimeSpan? value) {
            MaxValueChanged(value);
        }
        protected internal void MinValueChangedEx(TimeSpan? value) {
            MinValueChanged(value);
        }
    }

    public class TimeSpanEditValidator : EditorSpecificValidator {
        public TimeSpanEditValidator(BaseEdit editor)
            : base(editor) {
        }
        protected override StrategyValidatorBase CreateValidator() {
            return new RangedValueValidator<TimeSpan>((TimeSpanEdit)OwnerEdit);
        }
    }

    public class TimeSpanEditRangeService : RangeEditorService {
        public override bool ShouldRoundToBounds { get { return EditStrategy.ShouldRoundToBounds; } }
        new TimeSpanEditStrategy EditStrategy { get { return (TimeSpanEditStrategy)base.EditStrategy; } }
        public TimeSpanEditRangeService(BaseEdit editor)
            : base(editor) {
        }
        public override object CorrectToBounds(object maskValue) {
            return EditStrategy.Correct(maskValue);
        }
    }

    public class TimeSpanMaskSettings : TextInputMaskSettings {
        new TimeSpanEdit OwnerEdit {
            get { return (TimeSpanEdit)base.OwnerEdit; }
        }

        public TimeSpanMaskSettings(TimeSpanEdit editor)
            : base(editor) {
		}

        protected override MaskManager CreateDefaultMaskManager() {
            CultureInfo managerCultureInfo = OwnerEdit.MaskCulture;
            if (managerCultureInfo == null)
                managerCultureInfo = CultureInfo.CurrentCulture;
            string editMask = OwnerEdit.Mask;
            if (editMask == null)
                editMask = String.Empty;

            return new TimeSpanMaskManager(editMask, /*false*/true, managerCultureInfo, OwnerEdit.AllowNullInput, OwnerEdit.MinValue, OwnerEdit.MaxValue);
        }

        
    }


    public partial class TimeSpanEditSettings : ButtonEditSettings {
        public static readonly DependencyProperty MinValueProperty;
        public static readonly DependencyProperty MaxValueProperty;

        static TimeSpanEditSettings() {
            Type ownerType = typeof(TimeSpanEditSettings);
            MinValueProperty = DependencyPropertyManager.Register("MinValue", typeof(TimeSpan?), typeof(TimeSpanEditSettings), new PropertyMetadata(null, OnMinValuePropertyChanged));
            MaxValueProperty = DependencyPropertyManager.Register("MaxValue", typeof(TimeSpan?), typeof(TimeSpanEditSettings), new PropertyMetadata(null, OnMaxValuePropertyChanged));
            DisplayFormatProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata("c"));
            MaskTypeProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata((MaskType)7));
            //MaskProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata("c"));
            MaskUseAsDisplayFormatProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(true));
            AllowNullInputProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(true));

            RegisterCustomEdit();
        }
        protected static void OnMinValuePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            ((TimeSpanEditSettings)obj).OnMinValueChanged();
        }
        protected static void OnNullValuePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            ((TimeSpanEditSettings)obj).OnNullValueChanged();
        }
        protected static void OnMaxValuePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            ((TimeSpanEditSettings)obj).OnMaxValueChanged();
        }

        public TimeSpanEditSettings() { }
        private void SetDefaultDisplayFormat() {
            if (this.HasDefaultValue(DisplayFormatProperty)) DisplayFormat = "c";
        }
        void SetDefaultMask() {
            if (this.HasDefaultValue(MaskProperty)) Mask = "c";
        }

        public TimeSpan? MinValue {
            get { return (TimeSpan?)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public TimeSpan? MaxValue {
            get { return (TimeSpan?)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        protected virtual void OnMinValueChanged() {
        }
        protected virtual void OnMaxValueChanged() {
        }
        protected virtual void OnNullValueChanged() {
        }
        protected override void OnInitialized(EventArgs e) {
            base.OnInitialized(e);
            SetDefaultDisplayFormat();
        }
        protected override void AssignToEditCore(IBaseEdit edit) {
            base.AssignToEditCore(edit);
            TimeSpanEdit de = edit as TimeSpanEdit;
            if (de == null) return;
            SetValueFromSettings(MinValueProperty, () => de.MinValue = MinValue);
            SetValueFromSettings(MaxValueProperty, () => de.MaxValue = MaxValue);
            SetValueFromSettings(NullValueProperty, () => de.NullValue = NullValue);
        }

        protected override ButtonInfoBase CreateDefaultButtonInfo() {
            return new SpinButtonInfo { IsDefaultButton = true };
        }

        public static void RegisterCustomEdit() {
            EditorSettingsProvider.Default.RegisterUserEditor(typeof(TimeSpanEdit),
                typeof(TimeSpanEditSettings),
                () => new TimeSpanEdit(),
                () => new TimeSpanEditSettings());
        }
    }

    public class TimeSpanSettingsExtension : ButtonSettingsExtension {
        public TimeSpan? MinValue { get; set; }
        public TimeSpan? MaxValue { get; set; }
        public object NullValue { get; set; }
        public TimeSpanSettingsExtension() {
            MaskType = (MaskType)7;
            Mask = "c";
            DisplayFormat = "c";
            MinValue = (TimeSpan?)DateEditSettings.MinValueProperty.DefaultMetadata.DefaultValue;
            MaxValue = (TimeSpan?)DateEditSettings.MaxValueProperty.DefaultMetadata.DefaultValue;
        }
        protected override ButtonEditSettings CreateButtonEditSettings() {
            TimeSpanEditSettings de = new TimeSpanEditSettings();
            de.NullValue = NullValue;
            de.MinValue = MinValue;
            de.MaxValue = MaxValue;
            return de;
        }
    }

}
