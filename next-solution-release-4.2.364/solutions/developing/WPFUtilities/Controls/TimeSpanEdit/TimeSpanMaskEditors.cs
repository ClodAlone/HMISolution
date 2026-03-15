using System;
using System.Globalization;

namespace WPFUtilities.Controls
{
    #region Base

    public enum MaskPart {
        Fractional,
        Seconds,
        Minutes,
        Hours,
        Days
    }

    public abstract class TimeSpanMaskFormatElement {
        public abstract string Format(TimeSpan formattedTimeSpan, bool isNegative);
        public virtual bool Editable { get { return false; } }
        public virtual bool SupportsNegative { get { return false; } }
        public object EditGroup { get; set; }

        protected TimeSpanMaskFormatElement() { }
    }

    public class TimeSpanMaskFormatElementLiteral : TimeSpanMaskFormatElement {
        protected string fLiteral;
        public string Literal { get { return fLiteral; } }
        public override string Format(TimeSpan formattedTimeSpan, bool isNegative) {
            return Literal; //optionalModeDepth
        }
        public TimeSpanMaskFormatElementLiteral(string mask)
            : base() {
            this.fLiteral = mask;
        }
    }

    public class TimeSpanMaskFormatElementNonEditable : TimeSpanMaskFormatElement {
        string fMask;
        public string Mask { get { return fMask; } }
        public override string Format(TimeSpan formattedTimeSpan, bool isNegative) {
            return IsCorrectMask(Mask) ?
                formattedTimeSpan.ToString(fMask.Length == 1 ? '%' + fMask : fMask, CultureInfo.InvariantCulture) :
                Mask;
        }
        public TimeSpanMaskFormatElementNonEditable(string mask)
            : base() {
            this.fMask = mask;
        }

        protected virtual bool IsCorrectMask(string mask) {
            if (string.IsNullOrWhiteSpace(mask))
                return false;

            switch (mask[0]) {
                case 'd':
                case 'D':
                    return Mask.Length <= 8;
                case 'h':
                case 'H':
                case 'm':
                case 'M':
                case 's':
                case 'S':
                    return Mask.Length <= 2;
                case 'f':
                case 'F':
                    return Mask.Length <= FractionalHelper.FRACTIONAL_MASK_MAX_LENGTH;
                case 'c':
                case 'g':
                case 'G':
                    return true;
                default:
                    return false;
            }
        }
    }

    public abstract class TimeSpanMaskFormatElementEditable : TimeSpanMaskFormatElementNonEditable {
        public static readonly long MaxDays;
        public static readonly long MaxHours;
        public static readonly long MaxMinutes;
        public static readonly long MaxSeconds;
        public static readonly long MaxMilliseconds;
        public static readonly long MaxTicks;
        public override bool Editable { get { return true; } }

        static TimeSpanMaskFormatElementEditable() {
            MaxDays = (long)Math.Floor(TimeSpan.MaxValue.TotalDays);
            MaxHours = (long)Math.Floor(TimeSpan.MaxValue.TotalHours);
            MaxMinutes = (long)Math.Floor(TimeSpan.MaxValue.TotalMinutes);
            MaxSeconds = (long)Math.Floor(TimeSpan.MaxValue.TotalSeconds);
            MaxMilliseconds = (long)Math.Floor(TimeSpan.MaxValue.TotalMilliseconds);
            MaxTicks = 9999999; //TimeSpan.MaxValue.Ticks;
        }
        public abstract TimeSpanElementEditor CreateElementEditor(TimeSpan editedTimeSpan);
        public abstract TimeSpan ApplyElement(long result, TimeSpan editedTimeSpan);
        public virtual TimeSpan StepUp(TimeSpan formattedTimeSpan, bool isNegative) { return isNegative ? formattedTimeSpan.Negate() : formattedTimeSpan; }
        public virtual TimeSpan StepDown(TimeSpan formattedTimeSpan, bool isNegative) { return isNegative ? formattedTimeSpan.Negate() : formattedTimeSpan; }
        protected TimeSpanMaskFormatElementEditable(string mask) : base(mask) {}
    }
    
    public abstract class TimeSpanElementEditor {
        public abstract string DisplayText { get; }
        public abstract bool IsMinimum { get; }
        public abstract bool IsMaximum { get; }
        public abstract bool FinalOperatorInsert { get; }
        public abstract bool Insert(string inserted);
        public abstract bool ShouldSkipInsert(string inserted);
        public abstract bool Delete();
        public abstract long GetResult();
        public abstract bool SpinUp();
        public abstract bool SpinDown();
    }
    public class TimeSpanNumericRangeElementEditor : TimeSpanElementEditor {
        long fMinValue;
        long fMaxValue;
        int fMinDigits;
        int fMaxDigits;
        long fCurrentValue;
        protected int digitsEntered;
        protected bool Touched { get { return digitsEntered > 0; } }
        public long MinValue { get { return fMinValue; } }
        public long MaxValue { get { return fMaxValue; } }
        public virtual int MinDigits { get { return fMinDigits; } }
        public int MaxDigits { get { return fMaxDigits; } }
        public long CurrentValue { get { return fCurrentValue; } }
        protected void SetUntouchedValue(long newValue) {
            this.fCurrentValue = newValue;
            this.digitsEntered = 0;
        }
        public TimeSpanNumericRangeElementEditor(long initialValue, long minValue, long maxValue, int minDigits, int maxDigits)
            : this(minValue, maxValue, minDigits, maxDigits) {
            SetUntouchedValue(initialValue);
        }
        public TimeSpanNumericRangeElementEditor(long minValue, long maxValue, int minDigits, int maxDigits)
            : base() {
            this.fMinValue = minValue;
            this.fMaxValue = maxValue;
            this.fMinDigits = minDigits;
            this.fMaxDigits = maxDigits;
            this.SetUntouchedValue(minValue);
        }
        public override string DisplayText { get { return CurrentValue.ToString("d" + MinDigits.ToString("d2", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture); } }
        public override bool Insert(string inserted) {
            if (inserted.Length == 0)
                return Delete();
            string work = Touched ? fCurrentValue.ToString("d", CultureInfo.InvariantCulture) : string.Empty;
            int digitsCurrentlyEntered = 0;
            foreach (char ch in inserted) {
                if (ch >= '0' && ch <= '9') {
                    work += ch;
                    ++digitsCurrentlyEntered;
                }
                else {
                    return false;
                }
            }
            if (work.Length > MaxDigits)
                work = work.Substring(work.Length - MaxDigits);
            while (long.Parse(work, CultureInfo.InvariantCulture) > MaxValue)
                work = work.Substring(1);
            long nextValue = long.Parse(work, CultureInfo.InvariantCulture);
            fCurrentValue = nextValue;
            digitsEntered += digitsCurrentlyEntered;
            return true;
        }
        public override bool ShouldSkipInsert(string inserted) { return false; }
        public override bool Delete() {
            if (CurrentValue == MinValue && Touched == false)
                return false;
            this.SetUntouchedValue(MinValue);
            return true;
        }
        public override bool SpinUp() {
            long work = CurrentValue;
            if (work >= MaxValue)
                work = MinValue;
            else
                work++;

            if (work != CurrentValue || Touched) {
                this.SetUntouchedValue(work);
                return true;
            }
            else {
                return false;
            }
        }
        public override bool SpinDown() {
            long work = CurrentValue;
            if (work <= MinValue)
                work = MaxValue;
            else
                work--;

            if (work != CurrentValue || Touched) {
                this.SetUntouchedValue(work);
                return true;
            }
            else {
                return false;
            }
        }
        public override long GetResult() {
            if (CurrentValue >= MinValue && CurrentValue <= MaxValue)
                return CurrentValue;
            else
                return MinValue;
        }
        public override bool IsMinimum { get { return GetResult() == MinValue; } }
        public override bool IsMaximum { get { return GetResult() == MaxValue; } }
        public override bool FinalOperatorInsert {
            get {
                if (!Touched)
                    return false;
                if (CurrentValue > 0 && CurrentValue * 10 > MaxValue)
                    return true;
                if (MaxDigits > 0 && digitsEntered >= MaxDigits)
                    return true;
                return false;
            }
        }
    }

    #endregion

    #region Groups
    public class TimeSpanMaskFormatElementGroup : TimeSpanMaskFormatElement {
        public override string Format(TimeSpan formattedTimeSpan, bool isNegative) {
            return string.Empty;
        }
    }
    public class TimeSpanMaskFormatElementStartGroup : TimeSpanMaskFormatElementGroup { }
    public class TimeSpanMaskFormatElementEndGroup : TimeSpanMaskFormatElementGroup { }
    #endregion

    #region Sign

    public class TimeSpanElementEditor_Sign : TimeSpanElementEditor {
        int fResult;
        public const string SignDesignator = "-";

        public override string DisplayText { get { return fResult < 0 ? SignDesignator : ""; } }

        public override bool Insert(string inserted) {
            if (string.IsNullOrEmpty(inserted))
                Delete();
            else if (inserted.ToUpper() == SignDesignator)
                fResult = -1;
            else
                return false;

            return true;
        }

        public override bool ShouldSkipInsert(string inserted) { return !string.IsNullOrEmpty(inserted) && inserted != SignDesignator; }

        public override bool FinalOperatorInsert { get { return true; } }

        public override bool Delete() {
            if (fResult < 0) {
                fResult = 1;
                return true;
            }
            return false;
        }
        public override bool SpinUp() { return false; }
        public override bool SpinDown() { return false; }
        public override long GetResult() { return fResult; }
        public override bool IsMinimum { get { return false; } }
        public override bool IsMaximum { get { return false; } }
        public TimeSpanElementEditor_Sign(int initialValue) {
            this.fResult = initialValue;
        }
    }
    public class TimeSpanMaskFormatElement_Sign : TimeSpanMaskFormatElementEditable {
        public override bool SupportsNegative { get { return true; } }
        public override TimeSpanElementEditor CreateElementEditor(TimeSpan editedTimeSpan) {
            return new TimeSpanElementEditor_Sign(Math.Sign(editedTimeSpan.Ticks));
        }
        public override TimeSpan ApplyElement(long result, TimeSpan editedTimeSpan) {
            return TimeSpan.FromTicks(editedTimeSpan.Ticks * result);
        }
        public TimeSpanMaskFormatElement_Sign(string mask) : base(mask) { }

        public override string Format(TimeSpan formattedTimeSpan, bool isNegative) {
            return isNegative ? Mask : "";
        }
    }

    #endregion

    #region First Element

    public class TimeSpanMaskFormatElement_First : TimeSpanMaskFormatElementEditable {
        protected MaskPart MaskPart { get; private set; }

        public TimeSpanMaskFormatElement_First(string mask, MaskPart maskPart = MaskPart.Days) : base(mask) {
            MaskPart = maskPart;
        }
        public override TimeSpanElementEditor CreateElementEditor(TimeSpan editedTimeSpan) {
            long initialValue = 0;
            long maxValue = long.MaxValue;
            int maskMaxLength = 8;
            switch (MaskPart) {
                case MaskPart.Hours:
                    initialValue = (long)Math.Floor(editedTimeSpan.TotalHours);
                    maxValue = MaxHours;
                    maskMaxLength = 9;
                    break;
                case MaskPart.Minutes:
                    initialValue = (long)Math.Floor(editedTimeSpan.TotalMinutes);
                    maxValue = MaxMinutes;
                    maskMaxLength = 10;
                    break;
                case MaskPart.Seconds:
                    initialValue = (long)Math.Floor(editedTimeSpan.TotalSeconds);
                    maxValue = MaxSeconds;
                    maskMaxLength = 10;
                    break;
                case MaskPart.Fractional:
                    initialValue = FractionalHelper.ExtractTicks(editedTimeSpan, true);
                    maxValue = TimeSpan.MaxValue.Ticks;
                    maskMaxLength = 18;
                    break;
                default:
                    maxValue = MaxDays;
                    initialValue = (long)Math.Floor(editedTimeSpan.TotalDays);
                    break;
            }

            return new TimeSpanNumericRangeElementEditor(initialValue, 0, maxValue, 0, maskMaxLength);
        }
        public override TimeSpan ApplyElement(long result, TimeSpan editedTimeSpan) {
            double baseValue = 0d;
            TimeSpan incrementTimeSpan = TimeSpan.Zero;
            TimeSpan baseTimeSpan = editedTimeSpan;
            
            switch (MaskPart) {
                case MaskPart.Hours:
                    baseValue = Math.Floor(editedTimeSpan.TotalHours);
                    baseTimeSpan = editedTimeSpan.Subtract(TimeSpan.FromHours(baseValue));
                    incrementTimeSpan = TimeSpan.FromHours(result);
                    break;
                case MaskPart.Minutes:
                    baseValue = Math.Floor(editedTimeSpan.TotalMinutes);
                    baseTimeSpan = editedTimeSpan.Subtract(TimeSpan.FromMinutes(baseValue));
                    incrementTimeSpan = TimeSpan.FromMinutes(result);
                    break;
                case MaskPart.Seconds:
                    baseValue = Math.Floor(editedTimeSpan.TotalSeconds);
                    baseTimeSpan = editedTimeSpan.Subtract(TimeSpan.FromSeconds(baseValue));
                    incrementTimeSpan = TimeSpan.FromSeconds(result);
                    break;
                case MaskPart.Fractional:
                    baseValue = FractionalHelper.ExtractTicks(editedTimeSpan, true);
                    baseTimeSpan = editedTimeSpan.Subtract(TimeSpan.FromTicks((long)baseValue));
                    result = FractionalHelper.TicksFromValue(result, -1);
                    incrementTimeSpan = TimeSpan.FromTicks(result);
                    break;
                default:
                    baseValue = Math.Floor(editedTimeSpan.TotalDays);
                    baseTimeSpan = editedTimeSpan.Subtract(TimeSpan.FromDays(baseValue));
                    incrementTimeSpan = TimeSpan.FromDays(result);
                    break;
            }

            return baseTimeSpan.Add(incrementTimeSpan);
        }

        public override string Format(TimeSpan formattedTimeSpan, bool isNegative) {
            long value = 0;
            string mask = "";
            switch (MaskPart) {
                case MaskPart.Hours:
                    value = (long)Math.Floor(formattedTimeSpan.TotalHours);
                    mask = new String('#', 8);
                    break;
                case MaskPart.Minutes:
                    value = (long)Math.Floor(formattedTimeSpan.TotalMinutes);
                    mask += new String('#', 9);
                    break;
                case MaskPart.Seconds:
                    value = (long)Math.Floor(formattedTimeSpan.TotalSeconds);
                    mask = new String('#', 9);
                    break;
                case MaskPart.Fractional:
                    value = FractionalHelper.ExtractTicks(formattedTimeSpan, true);
                    mask = new String('#', 6/*18*/);
                    break;
                default:
                    value = (long)Math.Floor(formattedTimeSpan.TotalDays);
                    mask = new String('#', 7);
                    break;
            }
            mask += "0";

            var result = value.ToString(mask, CultureInfo.InvariantCulture);
            return result;
        }
    }

    #endregion

    #region Editors

    public class TimeSpanMaskFormatElement_D : TimeSpanMaskFormatElementEditable {
        public TimeSpanMaskFormatElement_D(string mask) : base(mask) { }
        public override TimeSpanElementEditor CreateElementEditor(TimeSpan editedTimeSpan) {
            return new TimeSpanNumericRangeElementEditor(editedTimeSpan.Days, 0, MaxDays, 0, Mask.Length);
        }
        public override TimeSpan ApplyElement(long result, TimeSpan editedTimeSpan) {
            return editedTimeSpan.Add(TimeSpan.FromDays(result - editedTimeSpan.Days));
        }
    }
    public class TimeSpanMaskFormatElement_H : TimeSpanMaskFormatElementEditable {
        public TimeSpanMaskFormatElement_H(string mask) : base(mask) { }
        public override TimeSpanElementEditor CreateElementEditor(TimeSpan editedTimeSpan) {
            return new TimeSpanNumericRangeElementEditor(editedTimeSpan.Hours, 0, 23, Mask.Length == 1 ? 1 : 2, 2);
        }
        public override TimeSpan ApplyElement(long result, TimeSpan editedTimeSpan) {
            return editedTimeSpan.Add(TimeSpan.FromHours(result - editedTimeSpan.Hours));
        }

        public override TimeSpan StepUp(TimeSpan formattedTimeSpan, bool isNegative) {
            formattedTimeSpan = formattedTimeSpan.Add(TimeSpan.FromDays(1));
            return base.StepUp(formattedTimeSpan, isNegative);
        }
        public override TimeSpan StepDown(TimeSpan formattedTimeSpan, bool isNegative) {
            formattedTimeSpan = formattedTimeSpan.Add(TimeSpan.FromDays(-1));
            return base.StepDown(formattedTimeSpan, isNegative);
        }
    }
    public class TimeSpanMaskFormatElement_M : TimeSpanMaskFormatElementEditable {
        public TimeSpanMaskFormatElement_M(string mask) : base(mask) {}
        public override TimeSpanElementEditor CreateElementEditor(TimeSpan editedTimeSpan) {
            return new TimeSpanNumericRangeElementEditor(editedTimeSpan.Minutes, 0, 59, Mask.Length == 1 ? 1 : 2, 2);
        }
        public override TimeSpan ApplyElement(long result, TimeSpan editedTimeSpan) {
            return editedTimeSpan.Add(TimeSpan.FromMinutes(result - editedTimeSpan.Minutes));
        }

        public override TimeSpan StepUp(TimeSpan formattedTimeSpan, bool isNegative) {
            formattedTimeSpan = formattedTimeSpan.Add(TimeSpan.FromHours(1));
            return base.StepUp(formattedTimeSpan, isNegative);
        }
        public override TimeSpan StepDown(TimeSpan formattedTimeSpan, bool isNegative) {
            formattedTimeSpan = formattedTimeSpan.Add(TimeSpan.FromHours(-1));
            return base.StepDown(formattedTimeSpan, isNegative);
        }
    }
    public class TimeSpanMaskFormatElement_S : TimeSpanMaskFormatElementEditable {
        public TimeSpanMaskFormatElement_S(string mask) : base(mask) {}
        public override TimeSpanElementEditor CreateElementEditor(TimeSpan editedTimeSpan) {
            return new TimeSpanNumericRangeElementEditor(editedTimeSpan.Seconds, 0, 59, Mask.Length == 1 ? 1 : 2, 2);
        }
        public override TimeSpan ApplyElement(long result, TimeSpan editedTimeSpan) {
            return editedTimeSpan.Add(TimeSpan.FromSeconds(result - editedTimeSpan.Seconds));
        }

        public override TimeSpan StepUp(TimeSpan formattedTimeSpan, bool isNegative) {
            formattedTimeSpan = formattedTimeSpan.Add(TimeSpan.FromMinutes(1));
            return base.StepUp(formattedTimeSpan, isNegative);
        }
        public override TimeSpan StepDown(TimeSpan formattedTimeSpan, bool isNegative) {
            formattedTimeSpan = formattedTimeSpan.Add(TimeSpan.FromMinutes(-1));
            return base.StepDown(formattedTimeSpan, isNegative);
        }
    }
    public class TimeSpanMaskFormatElement_F : TimeSpanMaskFormatElementEditable {
        public TimeSpanMaskFormatElement_F(string mask) : base(mask) { }
        public override TimeSpanElementEditor CreateElementEditor(TimeSpan editedTimeSpan) {
            var maskLength = Math.Min(FractionalHelper.FRACTIONAL_MASK_MAX_LENGTH, Mask.Length);
            var maxValue = 9;
            for (int i = 1; i < maskLength; i++) {
                maxValue *= 10;
                maxValue += 9;
            }
            return new TimeSpanNumericRangeElementEditor(FractionalHelper.ExtractTicks(editedTimeSpan) / (long)Math.Pow(10, (FractionalHelper.FRACTIONAL_MASK_MAX_LENGTH - maskLength)), 0, maxValue, Mask[0] == 'F' ? 0 : maskLength, maskLength);
        }
        public override TimeSpan ApplyElement(long result, TimeSpan editedTimeSpan) {
            var maskLength = Math.Min(FractionalHelper.FRACTIONAL_MASK_MAX_LENGTH, Mask.Length);

            if (Mask[0] == 'F')
                result = FractionalHelper.TicksFromValue(result, 0);
            else
                result = FractionalHelper.TicksFromValue(result, maskLength);

            return editedTimeSpan.Add(TimeSpan.FromTicks(result - editedTimeSpan.Ticks % FractionalHelper.FRACTIONAL_MULTIPLIER));
        }

        public override TimeSpan StepUp(TimeSpan formattedTimeSpan, bool isNegative) {
            formattedTimeSpan = formattedTimeSpan.Add(TimeSpan.FromSeconds(1));
            return base.StepUp(formattedTimeSpan, isNegative);
        }
        public override TimeSpan StepDown(TimeSpan formattedTimeSpan, bool isNegative) {
            formattedTimeSpan = formattedTimeSpan.Add(TimeSpan.FromSeconds(-1));
            return base.StepDown(formattedTimeSpan, isNegative);
        }
    }

    public static class FractionalHelper {
        public static readonly int FRACTIONAL_MASK_MAX_LENGTH = 7;
        public static readonly long FRACTIONAL_MULTIPLIER = (long)Math.Pow(10, FRACTIONAL_MASK_MAX_LENGTH);

        public static long ExtractTicks(TimeSpan value, bool ignoreMask = false) {
            return ignoreMask ? value.Ticks : value.Ticks % FRACTIONAL_MULTIPLIER;
        }
        public static long TicksFromValue(long value, int maskLength) {
            int factor = 0;
            var numericCount = (int)Math.Floor(Math.Log10(value)) + 1;

            if (maskLength < 0)
                factor = 0;
            else if (maskLength == 0)
                factor = FRACTIONAL_MASK_MAX_LENGTH - numericCount;
            else
                factor = FRACTIONAL_MASK_MAX_LENGTH - maskLength;

            return (long)(Math.Pow(10, factor) * value);
        }
    }
    #endregion
}
