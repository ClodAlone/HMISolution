using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using OPCUAViewModel;
#if !NET_STANDARD
using Utilities.WPF;
using UIMsgBoxAlertService.ComponentService;
using WPFUtilities.Converters;
using AuditTrace;
#else
using System.Timers;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Controls;
using System.Windows.Threading;
using Converters;
#endif
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif
using UFInterfaces;
using Utilities;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;
using System.Windows;
using System.Globalization;
using Opc.Ua;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using ViewModelLib;
#if !NET_STANDARD
using System.Windows.Data;
#endif
using ExpressionManager;

namespace CommandManager
{
#if !WINDOWS_UWP && !NET_STANDARD
    [TypeConverter(typeof(LocalizedEnumConverter))]
#endif
    public enum ValueCommandType
    {
        Set,
        Increase,
        Decrease,
        Toggle,
        Impulsive,
        ImpulsiveLatch,
        TransferValue,
        NumericPad,
        AlphaNumericPad,
        ResetStatistics,
        AppendValue,
        AppendDecimalONOFF,
        RemoveValue,
        SwapSign
    }

    internal class WriteTask
    {
        #region Declarations
        readonly ValueCommand owner;
        readonly Action action;
        #endregion

        #region Constructors
        public WriteTask(ValueCommand owner, Action action)
        {
            this.owner = owner;
            this.action = action;
        }
        #endregion

        #region Events
        public event EventHandler Executed;
        void OnExecuted()
        {
            var e = Executed;
            if (e != null)
                e(this, EventArgs.Empty);
        }

        public event EventHandler<Exception> Faulted;
        void OnFaulted(Exception ex)
        {
            var e = Faulted;
            if (e != null)
                e(this, ex);
        }
        #endregion

        #region Public Proprties
        public ValueCommand Owner
        {
            get
            {
                return owner;
            }
        }
        #endregion

        #region Public Methods
        public void Execute()
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                OnFaulted(ex);
            }
            finally
            {
                OnExecuted();
            }
        }
        #endregion
    }

    [DataContract(Name = "ValueCommand", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class ValueCommand : CommandManager
    {
        #region Declarations

        CallMethodCommand resetStatisticsCommand;
#if !NET_STANDARD
        AuditTraceViewModel auditTraceViewModel;
        MonitoredItemViewModel subscribedMinItemViewModel;
        MonitoredItemViewModel subscribedMaxItemViewModel;
        PropertyObserver<OPCUAEntityReference> observer;
#endif
        PropertyObserver<OPCUAEntityReference> observerItemViewModel;
        ExpressionEntity expressionEntity;
#if !NET_STANDARD
        bool bAuditTreceFetched;
        bool bAuditTraceDataContextSubscribed;
        bool bExpressionEntityDataContextSubscribed;
#endif

        static List<MonitoredItemViewModel> listAppendDecimalONOFF = new List<MonitoredItemViewModel>();

        #endregion
        #region Properties

        [DataMember(EmitDefaultValue = false)]
        [Browsable(false)]
        public int? SVGTransferReferenceId { get; set; }
        [DataMember(EmitDefaultValue = false)]
        [Browsable(false)]
        public int? SVGMinValueReferenceId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [Browsable(false)]
        public int? SVGMaxValueReferenceId { get; set; }


        [DataMember]
        String _value;
        public String Value
        {
            get { return _value; }
            set
            {
                if (_value == value)
                    return;
                _value = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("Value");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        //        [DataMember]
        //        public String expression;
        //        public String Expression
        //        {
        //            get { return expression; }
        //            set
        //            {
        //                if (value == expression)
        //                    return;
        //                expression = value;
        //#if !SILVERLIGHT
        //                OnPropertyChanged("Expression");
        //#endif
        //            }
        //        }

        [DataMember]
        public String reverseExpression;
        public String ReverseExpression
        {
            get { return reverseExpression; }
            set
            {
                if (value == reverseExpression)
                    return;
                reverseExpression = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("ReverseExpression");
#endif
            }
        }

        [DataMember]
        public bool decimalUnaware;
        public bool DecimalUnaware
        {
            get { return decimalUnaware; }
            set
            {
                if (value == decimalUnaware)
                    return;
                decimalUnaware = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("DecimalUnaware");
#endif
            }
        }

        [DataMember]
        ValueCommandType type;
        public virtual ValueCommandType Type
        {
            get
            {
                return type;
            }
            set
            {
                if (type == value)
                    return;
                type = value;
                MinValue = MaxValue = Decimals = 0;

#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("Type");
                OnPropertyChanged("CommandSummary");
                OnPropertyVisiblityChanged("Type");
#endif
            }
        }
        [DataMember]
        public bool useEUnit = true;
        public bool UseEUnit
        {
            get { return useEUnit; }
            set
            {
                if (value == useEUnit)
                    return;
                useEUnit = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("UseEUnit");
#endif
            }
        }

        [DataMember]
        private bool isRelative = true;
        public bool IsRelative
        {
            get { return isRelative; }
            set
            {
                if (isRelative == value)
                    return;
                isRelative = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("IsRelative");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        [DataMember]
        private double x = -1;
        public double X
        {
            get
            {
                return x;
            }
            set
            {
                if (x == value)
                    return;
                x = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("X");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        [DataMember]
        private double y = -1;
        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                if (y == value)
                    return;
                y = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("Y");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        [DataMember]
        double minValue;
        public double MinValue
        {
            get
            {
                return minValue;
            }
            set
            {
                if (minValue == value)
                    return;
                minValue = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("CommandSummary");
#endif
            }
        }
        [DataMember]
        double maxValue;
        public double MaxValue
        {
            get
            {
                return maxValue;
            }
            set
            {
                if (maxValue == value)
                    return;
                maxValue = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("CommandSummary");
#endif
            }
        }
        [DataMember]
        OPCUAEntityReference tagMinValue;
        public OPCUAEntityReference TagMinValue
        {
            get { return tagMinValue; }
            set
            {
                if (value == tagMinValue)
                    return;
                tagMinValue = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("TagMinValue");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }
        [DataMember]
        OPCUAEntityReference tagMaxValue;
        public OPCUAEntityReference TagMaxValue
        {
            get { return tagMaxValue; }
            set
            {
                if (value == tagMaxValue)
                    return;
                tagMaxValue = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("TagMaxValue");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }
        [DataMember]
        uint decimals;
        public uint Decimals
        {
            get
            {
                return decimals;
            }
            set
            {
                if (decimals == value)
                    return;
                decimals = value;

#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        [DataMember]
        int _time;
        public int Time
        {
            get { return _time; }
            set
            {
                if (_time == value)
                    return;
                _time = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("Time");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        [DataMember]
        bool _synchronous;
        public bool Synchronous
        {
            get { return _synchronous; }
            set
            {
                if (_synchronous == value)
                    return;
                _synchronous = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("Synchronous");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        [DataMember]
        bool _aspassword;
        public bool AsPassword
        {
            get { return _aspassword; }
            set
            {
                if (_aspassword == value)
                    return;
                _aspassword = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("AsPassword");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        [DataMember]
        OPCUAEntityReference transferToTag;
        public OPCUAEntityReference TransferToTag
        {
            get { return transferToTag; }
            set
            {
                if (value == transferToTag)
                    return;
                transferToTag = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("TransferToTag");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        #endregion
        [OnDeserializing]
        private void PreInitialize(StreamingContext context)
        {
            x = -1;
            y = -1;
        }

        String GetWriteValue()
        {
            var stringValue = "0";
            MonitoredItemViewModel model = null;
            if (expressionEntity != null)
                model = expressionEntity.TempVariable;
            else
                model = GetItemViewModel();
            //if (model == null)
            //    return stringValue;
            if (model == null || model.DataValue == null ||
                StatusCode.IsBad(model.DataValue.StatusCode))
                throw new Exception(Properties.Resources.CannotWriteOrReadTag);

            stringValue = model.InvariantCultureValue;
            switch (Type)
            {
                case ValueCommandType.TransferValue:
                    return stringValue;
                case ValueCommandType.Toggle:
                    {
                        double value = 0;
                        try
                        {
                            value = Convert.ToDouble(stringValue);
                        }
                        catch (Exception)
                        {
                            try
                            {
                                value = Convert.ToBoolean(stringValue) == true ? 1 : 0;
                            }
                            catch (Exception)
                            {
                                return stringValue;
                            }
                        }

                        if (value == 0)
                            value = 1;
                        else
                            value = 0;

                        if (value == 1 && !String.IsNullOrEmpty(Value))
                            return Value;
                        return value.ToString();
                    }
                case ValueCommandType.Impulsive:
                case ValueCommandType.ImpulsiveLatch:
                case ValueCommandType.Set:
                    if (Value == null && model.DataValue != null && model.DataValue.Value != null &&
                        model.DataValue.Value.GetType() == typeof(String))
                        Value = "";
                    return Value;
                case ValueCommandType.AppendDecimalONOFF:
                    if (model != null)
                    {
                        lock (listAppendDecimalONOFF)
                        {
                            if (listAppendDecimalONOFF.Contains(model))
                                listAppendDecimalONOFF.Remove(model);
                            else
                                listAppendDecimalONOFF.Add(model);
                        }
                    }
                    return null;
                case ValueCommandType.AppendValue:
                    if (model.DataValue != null && model.DataValue.Value != null)
                    {
                        bool bAppendDecimal = false;
                        lock (listAppendDecimalONOFF)
                        {
                            bAppendDecimal = listAppendDecimalONOFF.Contains(model);
                        }

                        var value = String.Format(CultureInfo.InvariantCulture, "{0}", model.DataValue.Value);
                        CultureInfo culture = new CultureInfo(System.Globalization.CultureInfo.CurrentCulture.Name, true);
                        String separator = culture.NumberFormat.NumberDecimalSeparator;

                        bool notStringType = model.DataValue.Value.GetType() != typeof(String);
                        if (notStringType && !value.Contains(separator))
                            separator = ".";

                        if (!DecimalUnaware)
                        {
                            var split = value.Split(separator[0]);
                            if (bAppendDecimal)
                            {
                                if (split.Length > 1)
                                    return String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}", split[0], separator, split[1], Value);
                                else
                                    return String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", split[0], separator, Value);
                            }
                            else
                            {
                                if (split.Length > 1)
                                    return String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}", split[0], Value, separator, split[1]);
                                else
                                    return String.Format(CultureInfo.InvariantCulture, "{0}{1}", split[0], Value);
                            }
                        }
                        else
                            return String.Format(CultureInfo.InvariantCulture, "{0}{1}", value, Value);
                    }
                    else
                        return null;
                case ValueCommandType.RemoveValue:
                    if (model.DataValue != null && model.DataValue.Value != null)
                    {
                        bool bAppendDecimal = false;
                        lock (listAppendDecimalONOFF)
                        {
                            bAppendDecimal = listAppendDecimalONOFF.Contains(model);
                        }

                        var value = String.Format(CultureInfo.InvariantCulture, "{0}", model.DataValue.Value);
                        CultureInfo culture = new CultureInfo(System.Globalization.CultureInfo.CurrentCulture.Name, true);
                        String separator = culture.NumberFormat.NumberDecimalSeparator;

                        bool notStringType = model.DataValue.Value.GetType() != typeof(String);
                        if (notStringType && !value.Contains(separator))
                            separator = ".";

                        if (!DecimalUnaware)
                        {
                            var split = value.Split(separator[0]);
                            if (bAppendDecimal)
                            {
                                if (split.Length > 1)
                                {
                                    if (split[1].Length > 0)
                                        return String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", split[0], separator,
                                                                split[1].Remove(split[1].Length - 1, 1));
                                    else
                                        return String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", split[0], separator, split[1]);
                                }
                                else
                                    return String.Format(CultureInfo.InvariantCulture, "{0}", split[0]);
                            }
                            else
                            {
                                if (split.Length > 1)
                                {
                                    if (split[0].Length > 0)
                                        return String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", split[0].Remove(split[0].Length - 1, 1),
                                                                separator, split[1]);
                                    else
                                        return String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", split[0], separator, split[1]);
                                }
                                else
                                {
                                    var v = String.Format(CultureInfo.InvariantCulture, "{0}", model.DataValue.Value);
                                    if (v.Length > 0)
                                    {
                                        var r = v.Remove(v.Length - 1, 1);
                                        if (r.Length == 0)
                                        {
                                            if (!(model.DataValue.Value is String))
                                                return "0";
                                        }

                                        return r;
                                    }
                                    return v;
                                }
                            }
                        }
                        else
                        {
                            if (value.Length > 0)
                                return value.Remove(value.Length - 1, 1);
                            else
                                return null;
                        }
                    }
                    else
                        return null;
                case ValueCommandType.SwapSign:
                    if (model.DataValue != null && model.DataValue.Value != null)
                    {
                        var v = String.Format(CultureInfo.InvariantCulture, "{0}", model.DataValue.Value);
                        if (v.Length > 0)
                        {
                            if (v[0] == '-')
                                v = String.Format(CultureInfo.InvariantCulture, "{0}", v.Substring(1));
                            else
                                v = String.Format(CultureInfo.InvariantCulture, "-{0}", v);
                        }
                        return v;
                    }
                    else
                        return null;
                case ValueCommandType.Increase:
                    {
                        try
                        {
                            double value = Convert.ToDouble(stringValue, CultureInfo.InvariantCulture);
                            double value2 = Convert.ToDouble(Value, CultureInfo.InvariantCulture);
                            var result = value + value2;
                            result = Math.Max(result, model.Range.Low);
                            result = Math.Min(result, model.Range.High);
                            return Convert.ToString(result, CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                        }

                        return stringValue;
                    }
                case ValueCommandType.Decrease:
                    {
                        try
                        {
                            double value = Convert.ToDouble(stringValue, CultureInfo.InvariantCulture);
                            double value2 = Convert.ToDouble(Value, CultureInfo.InvariantCulture);
                            var result = value - value2;
                            result = Math.Max(result, model.Range.Low);
                            result = Math.Min(result, model.Range.High);
                            return Convert.ToString(result, CultureInfo.InvariantCulture);
                        }
                        catch (Exception ex)
                        {
                        }

                        return stringValue;
                    }
                case ValueCommandType.NumericPad:
                    {
#if !NET_STANDARD
                        Window owner = null;
                        if (Control != null)
                            owner = Control.FindParent<Window>();
                        if (owner == null)
                        {
                            var ie = Keyboard.FocusedElement as DependencyObject;
                            if (ie != null)
                                owner = Window.GetWindow(ie);
                            if (owner == null)
                                throw new Exception(Properties.Resources.PadCommandNotSupported);
                        }

                        double? value = null;
                        double? minValue = null;
                        double? maxValue = null;

                        if (UseEUnit && model.HasRange)
                        {
                            minValue = model.Range.Low;
                            maxValue = model.Range.High;
                        }
                        else
                        {
                            if (TagMinValue != null)
                            {
                                var rangeModel = GetMinItemViewModel();
                                if (rangeModel == null || rangeModel.DataValue == null ||
                                    StatusCode.IsBad(rangeModel.DataValue.StatusCode))
                                    throw new Exception(Properties.Resources.CannotReadMinTag);

                                try
                                {
                                    minValue = Convert.ToDouble(rangeModel.InvariantCultureValue, CultureInfo.InvariantCulture);
                                }
                                catch
                                {
                                    minValue = MinValue;
                                }

                            }

                            if (TagMaxValue != null)
                            {
                                var rangeModel = GetMaxItemViewModel();
                                if (rangeModel == null || rangeModel.DataValue == null ||
                                    StatusCode.IsBad(rangeModel.DataValue.StatusCode))
                                    throw new Exception(Properties.Resources.CannotReadMaxTag);

                                try
                                {
                                    maxValue = Convert.ToDouble(rangeModel.InvariantCultureValue, CultureInfo.InvariantCulture);
                                }
                                catch
                                {
                                    maxValue = MaxValue;
                                }
                            }

                            if (!minValue.HasValue)
                                minValue = MinValue < MaxValue ? MinValue : MaxValue;
                            if (!maxValue.HasValue)
                                maxValue = MinValue < MaxValue ? MaxValue : MinValue;

                            if (minValue == maxValue)
                            {
                                minValue = null;
                                maxValue = null;
                            }
                        }

                        try
                        {
                            value = Convert.ToDouble(stringValue, CultureInfo.InvariantCulture);
                        }
                        catch
                        { }

                        if (value.HasValue)
                        {
                            uint k = 1;
                            var numFormat = GetFormat(Decimals);
                            if (Decimals > 0 && MustForceDecimals(model))
                                k = (uint)Math.Pow(10, Decimals);
                            value = value / k;
                            var padValue = String.Format(CultureInfo.CurrentCulture, numFormat, value);
                            var ret = AsPassword ? Pads.Pads.ShowNumericPasswordPad(X, Y, IsRelative, padValue, owner, min: minValue, max: maxValue, title: OpcuaEntityReference?.HumanReadable) : Pads.Pads.ShowNumericPad(X, Y, IsRelative, padValue, owner, min: minValue, max: maxValue, title: OpcuaEntityReference?.HumanReadable);
                            if (ret == null)
                                return ret;
                            
                            try
                            {
                                value = Convert.ToDouble(ret, CultureInfo.CurrentCulture);
                                return String.Format(CultureInfo.InvariantCulture, numFormat, value * k);
                            }
                            catch
                            { }
                        }
                        else
                            return AsPassword ? Pads.Pads.ShowNumericPasswordPad(X, Y, IsRelative, stringValue, owner, min: minValue, max: maxValue, title: OpcuaEntityReference?.HumanReadable) : Pads.Pads.ShowNumericPad(X, Y, IsRelative, stringValue, owner, min: minValue, max: maxValue, title: OpcuaEntityReference?.HumanReadable);

                        return stringValue;
#else
                        throw new Exception(Properties.Resources.PadCommandNotSupported);
#endif
                    }
                case ValueCommandType.AlphaNumericPad:
                    {
#if !NET_STANDARD
                        Window owner = null;
                        if (Control != null)
                            owner = Control.FindParent<Window>();
                        if (owner == null)
                        {
                            var ie = Keyboard.FocusedElement as DependencyObject;
                            if (ie != null)
                                owner = Window.GetWindow(ie);
                            if (owner == null)
                                throw new Exception(Properties.Resources.PadCommandNotSupported);
                        }

                        int? maxValue = null;

                        if (TagMaxValue != null)
                        {
                            var rangeModel = GetMaxItemViewModel();
                            if (rangeModel == null || rangeModel.DataValue == null ||
                                StatusCode.IsBad(rangeModel.DataValue.StatusCode))
                                throw new Exception(Properties.Resources.CannotReadMaxTag);

                            try
                            {
                                maxValue = Convert.ToInt32(rangeModel.InvariantCultureValue, CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                maxValue = (int)MaxValue;
                            }
                        }
                        else if (MinValue != MaxValue)
                        {
                            maxValue = (int)MaxValue;
                        }
                        else if (model.HasRange)
                        {
                            maxValue = (int)model.Range.High;
                        }
                        return AsPassword ? Pads.Pads.ShowPasswordPad(X, Y, IsRelative, stringValue, owner, max: maxValue, title: OpcuaEntityReference?.HumanReadable) : Pads.Pads.ShowAlphaNumericPad(X, Y, IsRelative, stringValue, owner, max: maxValue, title: OpcuaEntityReference?.HumanReadable);
#else
                        throw new Exception(Properties.Resources.PadCommandNotSupported);
#endif
                    }
            }

            return String.Empty;
        }
        private string GetFormat(uint pointPrecision)
        {
            StringBuilder stringformat = new StringBuilder();
            if (pointPrecision > 0)
            {
                stringformat.Append(".");
                for (int i = 0; i < pointPrecision; i++)
                    stringformat.Append("0");
            }
            return $"{{0:0{stringformat.ToString()}}}";
        }

        MonitoredItemViewModel subscribedModel;
        MonitoredItemViewModel GetItemViewModel(OPCUAEntityReference reference = null)
        {
            if (reference == null)
                reference = OpcuaEntityReference;
            MonitoredItemViewModel monitor = null;
#if !NET_STANDARD
            if (reference == null)
            {
                if (Entity != null && Entity.ContainedObject is FrameworkElement)
                {
                    var fe = Entity.ContainedObject as FrameworkElement;
                    MonitoredItemViewModel ret = null;
                    fe.Dispatcher.InvokeIfRequired(() =>
                    {
                        ret = fe.DataContext as MonitoredItemViewModel;
                    });

                    if (ret != null)
                        return ret;
                    if (Entity.ContainedObject is ContentControl)
                    {
                        var control = Entity.ContainedObject as ContentControl;
                        control.Dispatcher.InvokeIfRequired(() =>
                        {
                            if (control.Content is FrameworkElement)
                                monitor = (control.Content as FrameworkElement).DataContext as MonitoredItemViewModel;
                        });
                    }
                }
            }
            else
#else
            if (reference != null)
#endif
                monitor = reference.MonitoredItemViewModel;
            if (expressionEntity != null && reference == OpcuaEntityReference)
                monitor = expressionEntity.TempVariable;
            if (monitor != null && !monitor.IsValid)
                return null;

            if (subscribedModel != monitor)
            {
                lock (lockObject)
                {
                    if (subscribedModel != null)
                        subscribedModel.PropertyChanged -= SubscribedModel_PropertyChanged;

                    subscribedModel = monitor;
                    if (subscribedModel != null)
                        subscribedModel.PropertyChanged += SubscribedModel_PropertyChanged;
                }
            }

            return monitor;
        }

        StatusCode lastStatusCode;
        DataValue lastDataValue;
#if !NET_STANDARD
        DispatcherOperation dpUpdate;
#endif
        object lockObject;
        private void SubscribedModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataValue")
            {
                bool bForceDispatcherOperation = false;
                lock (lockObject)
                {
                    if (subscribedModel == null)
                        return;

                    bForceDispatcherOperation = lastDataValue == null ||
                        subscribedModel.DataValue != null && subscribedModel.DataValue.StatusCode != lastStatusCode;
                    lastDataValue = subscribedModel.DataValue;
                    if (subscribedModel.DataValue != null)
                        lastStatusCode = subscribedModel.DataValue.StatusCode;
#if !NET_STANDARD
                    if (dpUpdate == null || bForceDispatcherOperation ||
                        dpUpdate.Status == DispatcherOperationStatus.Completed ||
                        dpUpdate.Status == DispatcherOperationStatus.Aborted)
                    {
                        if (dpUpdate != null && dpUpdate.Status != DispatcherOperationStatus.Aborted &&
                            dpUpdate.Status != DispatcherOperationStatus.Completed)
                            dpUpdate.Abort();

                        if (Control != null)
                        {
                            dpUpdate = Control.Dispatcher.BeginInvokeAsynchronouslyInBackground(Control as FrameworkElement, () =>
                            {
                                lock (lockObject)
                                {
                                    if (subscribedModel == null)
                                        return;
                                }

                                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                            });
                        }
                    }
#endif
                }
            }
        }

        MonitoredItemViewModel GetMinItemViewModel()
        {
            MonitoredItemViewModel monitor = null;
            if (TagMinValue != null)
                monitor = TagMinValue.MonitoredItemViewModel;

            if (monitor != null && !monitor.IsValid)
                return null;
#if !NET_STANDARD
            if (Control != null && subscribedMinItemViewModel != monitor)
            {
                lock (lockObject)
                {
                    if (subscribedMinItemViewModel != null)
                        subscribedMinItemViewModel.PropertyChanged -= SubscribedMinMaxItemViewModel_PropertyChanged;

                    subscribedMinItemViewModel = monitor;
                    if (subscribedMinItemViewModel != null)
                        subscribedMinItemViewModel.PropertyChanged += SubscribedMinMaxItemViewModel_PropertyChanged;
                }
            }
#endif
            return monitor;
        }

        MonitoredItemViewModel GetMaxItemViewModel()
        {
            MonitoredItemViewModel monitor = null;
            if (TagMaxValue != null)
                monitor = TagMaxValue.MonitoredItemViewModel;

            if (monitor != null && !monitor.IsValid)
                return null;
#if !NET_STANDARD
            if (Control != null && subscribedMaxItemViewModel != monitor)
            {
                lock (lockObject)
                {
                    if (subscribedMaxItemViewModel != null)
                        subscribedMaxItemViewModel.PropertyChanged -= SubscribedMinMaxItemViewModel_PropertyChanged;

                    subscribedMaxItemViewModel = monitor;
                    if (subscribedMaxItemViewModel != null)
                        subscribedMaxItemViewModel.PropertyChanged += SubscribedMinMaxItemViewModel_PropertyChanged;
                }
            }
#endif
            return monitor;
        }

#if !NET_STANDARD
        void SubscribedMinMaxItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataValue")
            {
                InvalidateRequerySuggestedAsynchronously();
            }
        }
#endif

#if !NET_STANDARD
        void PrepareAuditTrace(OPCUAEntityReference reference = null)
        {
            if (reference == null)
                reference = OpcuaEntityReference;

            if (reference == null)
            {
                if (Entity != null)
                {
                    FrameworkElement fe = null;
                    if (Entity.ContainedObject is ContentControl)
                    {
                        var control = Entity.ContainedObject as ContentControl;
                        fe = control.Content as FrameworkElement;
                    }

                    if (fe == null)
                        fe = Entity.ContainedObject as FrameworkElement;

                    if (fe != null && !bAuditTraceDataContextSubscribed)
                    {
                        bAuditTraceDataContextSubscribed = true;
                        fe.DataContextChanged += (s, e) =>
                        {
                            if (e.NewValue is MonitoredItemViewModel)
                            {
                                TerminateAuditTrace();
                                IValueConverter converter = null;
                                var monitoredItemViewModel = e.NewValue as MonitoredItemViewModel;
                                if (monitoredItemViewModel.ReferenceViewModel != null)
                                {
                                    var expressionEntity = ExpressionBucket.GetInstance(Parent).GetExpression(monitoredItemViewModel);
                                    converter = expressionEntity?.Converter;
                                    monitoredItemViewModel = monitoredItemViewModel.ReferenceViewModel;
                                }
                                if (monitoredItemViewModel.monitoredItem != null)
                                {
                                    auditTraceViewModel = new AuditTraceViewModel(monitoredItemViewModel, Entity, Parent, SessionName)
                                    {
                                        Control = Control,
                                        Converter = converter,
                                        ConverterParameter = mapDynamics
                                    };
                                    auditTraceViewModel.AuditPropertiesFetched += OnAuditFetched;
                                }
                                else
                                    bAuditTreceFetched = true;
                            }
                        };
                    }
                }
            }
            else
            {
                var monitoredItemViewModel = reference.MonitoredItemViewModel;
                if (monitoredItemViewModel != null)
                {
                    TerminateAuditTrace();
                    if (monitoredItemViewModel.monitoredItem != null)
                    {
                        IValueConverter converter = null;
                        if (expressionEntity != null && reference == OpcuaEntityReference)
                            converter = expressionEntity.Converter;
                        auditTraceViewModel = new AuditTraceViewModel(monitoredItemViewModel, Entity, Parent, SessionName)
                        {
                            Control = Control,
                            Converter = converter,
                            ConverterParameter = mapDynamics
                        };
                        auditTraceViewModel.AuditPropertiesFetched += OnAuditFetched;
                    }
                    else
                        bAuditTreceFetched = true;
                }
                else
                {
                    observer = new PropertyObserver<OPCUAEntityReference>(reference)
                    .RegisterHandler(n => n.MonitoredItemViewModel, n =>
                    {
                        TerminateAuditTrace();
                        IValueConverter converter = null;
                        if (expressionEntity != null && reference == OpcuaEntityReference)
                            converter = expressionEntity.Converter;
                        if (n.MonitoredItemViewModel.monitoredItem != null)
                        {
                            auditTraceViewModel = new AuditTraceViewModel(n.MonitoredItemViewModel, Entity, Parent, SessionName)
                            {
                                Control = Control,
                                Converter = converter,
                                ConverterParameter = mapDynamics
                            };
                            auditTraceViewModel.AuditPropertiesFetched += OnAuditFetched;
                        }
                        else
                            bAuditTreceFetched = true;
                    });
                }
            }
        }

        void TerminateAuditTrace()
        {
            if (observer != null)
                observer.Dispose();

            if (auditTraceViewModel != null)
            {
                auditTraceViewModel.AuditPropertiesFetched -= OnAuditFetched;
                auditTraceViewModel.Dispose();
                auditTraceViewModel = null;
            }

            bAuditTreceFetched = false;
        }

        void OnAuditFetched(object s, EventArgs ev)
        {
            bAuditTreceFetched = true;
            InvalidateRequerySuggestedAsynchronously();
        }

        void InvalidateRequerySuggestedAsynchronously()
        {
            if (Control == null)
                return;

            lock (lockObject)
            {
                if (dpUpdate == null ||
                    dpUpdate.Status == DispatcherOperationStatus.Completed ||
                    dpUpdate.Status == DispatcherOperationStatus.Aborted)
                {
                    dpUpdate = Control.Dispatcher.BeginInvokeAsynchronouslyInBackground(Control as FrameworkElement, () =>
                    {
                        System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                    });
                }
            }
        }
#endif

        private bool MustForceDecimals(MonitoredItemViewModel monitor)
        {
            if (monitor == null)
                return false;
            BuiltInType builtInType = MonitoredItemViewModel.GetBuiltInType(monitor.DataType);
            return TypeInfo.IsNumericType(builtInType) && builtInType != BuiltInType.Double && builtInType != BuiltInType.Float;
        }

        String pendingWrite;
        static List<WriteTask> pendingTask = new List<WriteTask>();
        static Task writePendingTask;

        void Execute(bool silent, bool isBlindExecuting = false)
        {
            if (
#if !NET_STANDARD
                IsAccessDenied() || 
#endif
                !CanExecute())
                return;

            if (Type == ValueCommandType.ResetStatistics)
            {
                if (resetStatisticsCommand != null)
                {
                    var model = GetItemViewModel();
                    if (model.ReferenceViewModel != null)
                        model = model.ReferenceViewModel;
                    if (model != null && model.monitoredItem != null)
                    {
                        resetStatisticsCommand.InputParameters.Clear();
                        resetStatisticsCommand.InputParameters.Add(new Variant(model.monitoredItem.ResolvedNodeId));

                        if (Synchronous || isBlindExecuting)
                        {
                            var result = resetStatisticsCommand.RemoteExecute();
                            if (result != null)
                            {
                                var msg = result.ex.InnerException != null ? result.ex.InnerException.Message : result.ex.Message;
                                if (!silent)
                                    ShowError(msg);
                                if (isBlindExecuting)
                                    throw result.ex;
                            }
                        }
                        else
                            resetStatisticsCommand.Execute();
                    }
                }
            }
            else
            {
                string value = GetWriteValue();
                if (value == null || !String.IsNullOrEmpty(pendingWrite) && pendingWrite == value)
                    return;
                pendingWrite = value;
#if !NET_STANDARD
                if (auditTraceViewModel != null && auditTraceViewModel.IsAuditTraceEnabled)
                {
                    auditTraceViewModel.SetValue(value);
                    pendingWrite = null;
                    return;
                }
#endif
                var model = GetItemViewModel();
                if (Type == ValueCommandType.TransferValue)
                    model = GetItemViewModel(TransferToTag);
                if (model != null)
                {
                    var action = new Action(() =>
                    {
                        if (expressionEntity == null || (Type == ValueCommandType.TransferValue && expressionEntity.TempVariable != model))
                            model.WriteValue(value);
                        else
                            expressionEntity.WriteValue(value, CultureInfo.InvariantCulture);
                        pendingWrite = null;
                    });

                    WriteTask task = null;
                    if (Synchronous || isBlindExecuting)
                    {
                        try
                        {
                            action();
                        }
                        catch (Exception ex)
                        {
                            pendingWrite = null;

                            var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                            if (!silent)
                                ShowError(msg);
                            if (isBlindExecuting)
                                throw;
                        }
                    }
                    else
                    {
                        task = new WriteTask(this, action);
                    }

                    if (Type == ValueCommandType.Impulsive ||
                        Type == ValueCommandType.ImpulsiveLatch)
                    {
                        var action2 = new Action(() =>
                        {
#if !NET_STANDARD
                            if (timer != null)
                                timer.Stop();
#else
                            lock (lockObject)
                            {
                                if (timer != null)
                                {
                                    timer.Stop();
                                    timer.Dispose();
                                    timer = null;
                                }
                            }
#endif
                            if (Time > 0)
                            {
#if !NET_STANDARD
                                timer = new DispatcherTimer();
                                timer.Interval = TimeSpan.FromMilliseconds(Time);
                                timer.Tick += (o, e) =>
#else
                                timer = new Timer();
                                timer.Interval = Time;
                                timer.AutoReset = false;
                                timer.Elapsed += (o, e) =>
#endif
                                {
#if !NET_STANDARD
                                    if (timer != null)
                                    {
                                        timer.Stop();
                                        timer = null;
                                    }
#else
                                    lock (lockObject)
                                    {
                                        if (timer != null)
                                        {
                                            timer.Stop();
                                            timer.Dispose();
                                            timer = null;
                                        }
                                    }
#endif
                                    if (model != null)
                                    {
                                        //var task10 = Task.Factory.StartNew(() =>
                                        var action10 = new Action(() =>
                                        {
                                            try
                                            {
                                                if (expressionEntity == null || (Type == ValueCommandType.TransferValue && expressionEntity.TempVariable != model))
                                                    model.WriteValue(0);
                                                else
                                                    expressionEntity.WriteValue(0, CultureInfo.InvariantCulture);
                                            }
                                            catch { }
                                        });

                                        AddNewWriteTask(action10);
                                    }
                                };
                                timer.Start();
                            }
                        });

                        if (task != null)
                        {
                            var context = System.Threading.SynchronizationContext.Current;
                            task.Executed += (s, e) =>
                            {
                                context.Post(o => action2(), null);
                            };
                        }
                        else
                        {
                            try
                            {
#if !NET_STANDARD
                                if (currentDispatcher != null)
                                    currentDispatcher.InvokeIfRequired(() => action2());
                                else
#endif
                                action2();
                            }
                            catch (Exception ex)
                            {
                                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                                if (!silent)
                                    ShowError(msg);
                            }
                        }
                    }

                    if (task != null)
                    {
                        var context = System.Threading.SynchronizationContext.Current;
                        task.Faulted += (s, e) =>
                        {
                            context.Post(o => 
                            {
                                pendingWrite = null;

                                var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
                                if (!silent)
                                    ShowError(msg);
                                else
                                    logCommands.Error(msg);
                            }, null);
                        };
                        AddNewWriteTask(task);
                    }
                }
            }
        }

        void AddNewWriteTask(Action action)
        {
            lock (pendingTask)
            {
                var task = new WriteTask(this, action);
                pendingTask.Add(task);
                EnsureTaskExecution();
            }
        }

        void AddNewWriteTask(WriteTask task)
        {
            lock (pendingTask)
            {
                pendingTask.Add(task);
                EnsureTaskExecution();
            }
        }

        void EnsureTaskExecution()
        {
            if (writePendingTask == null)
            {
                writePendingTask = Task.Factory.StartNew(() =>
                {
                    WriteTask task = null;
                    lock (pendingTask)
                    {
                        task = pendingTask[0];
                        pendingTask.Remove(task);
                    }

                    task.Execute();
                });

                writePendingTask.ContinueWith((T) =>
                {
                    lock (pendingTask)
                    {
                        writePendingTask = null;
                        if (pendingTask.Count > 0)
                            EnsureTaskExecution();
                    }
                });
            }
        }

        void TerminatePendingTasks()
        {
            List<WriteTask> tasks = null;
            lock (pendingTask)
            {
                tasks = (from c in pendingTask where c.Owner == this select c).ToList();
                tasks.ForEach(task => pendingTask.Remove(task));
            }

            tasks.ForEach(task => task.Execute());
        }

        private void ShowError(String msg)
        {
            var error = String.Format(Properties.Resources.ValueCommandError,
                OpcuaEntityReference != null ? OpcuaEntityReference.HumanReadable : "DataContext", msg);
#if !NET_STANDARD
            if (Control != null)
            {
#if !WINDOWS_UWP
                Control.Dispatcher.BeginInvokeIfRequired(() =>
#else
                        RunOnUIThread.RunIfRequired(() =>
#endif
                {
                    var ui = Parent?.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    if (ui != null)
                        ui.ShowError(error);
#if !WINDOWS_UWP
                    else
                        System.Windows.MessageBox.Show(error, Name);
#endif
                });
            }
            else
#endif
            {
#if !NET_STANDARD
                var ui = Parent?.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (ui != null)
                    ui.ShowError(error);
#if !WINDOWS_UWP
                else
                    System.Windows.MessageBox.Show(error, Name);
#endif
#else
                logCommands.Error(error);
#endif
            }
        }

        #region Overrides
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override List<OPCUAEntityReference> ListTags
        {
            get
            {
                var ret = base.ListTags;
                if (Type == ValueCommandType.TransferValue &&
                    TransferToTag != null)
                    ret.Add(TransferToTag);
                if (TagMinValue != null &&
                    Type == ValueCommandType.NumericPad)
                    ret.Add(TagMinValue);
                if (TagMaxValue != null &&
                    (Type == ValueCommandType.AlphaNumericPad || Type == ValueCommandType.NumericPad))
                    ret.Add(TagMaxValue);
                return ret;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override void UpdateTags(OPCUAEntityReference source, OPCUAEntityReference dest)
        {
            base.UpdateTags(source, dest);

            if (Type == ValueCommandType.TransferValue)
            {
                if (TransferToTag != null && TransferToTag.HumanReadableNoProject == source.HumanReadableNoProject)
                    TransferToTag = dest;
            }
            else if (Type == ValueCommandType.NumericPad)
            {
                if (TagMinValue != null && TagMinValue.HumanReadableNoProject == source.HumanReadableNoProject)
                    TagMinValue = dest;
            }
            else if (Type == ValueCommandType.AlphaNumericPad || Type == ValueCommandType.NumericPad)
            {
                if (TagMaxValue != null && TagMaxValue.HumanReadableNoProject == source.HumanReadableNoProject)
                    TagMaxValue = dest;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override Dictionary<OPCUAEntityReference, String> TagsMap
        {
            get
            {
                var ret = base.TagsMap;
                if (Type == ValueCommandType.TransferValue &&
                    TransferToTag != null)
                    ret.Add(TransferToTag, Properties.Resources.ValueCommandTransferValueTag);
                if (TagMinValue != null &&
                    Type == ValueCommandType.NumericPad)
                    ret.Add(TagMinValue, Properties.Resources.ValueCommandMinValueTag);
                if (TagMaxValue != null &&
                    (Type == ValueCommandType.AlphaNumericPad || Type == ValueCommandType.NumericPad))
                    ret.Add(TagMaxValue, Properties.Resources.ValueCommandMaxValueTag);
                return ret;
            }
        }
#if !WINDOWS_UWP
#if !NET_STANDARD
        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "SVGTransferReferenceId")
                    return false;
                else if (propertyName == "X" || propertyName == "Y" || propertyName == "IsRelative")
                {
                    return Type == ValueCommandType.NumericPad || Type == ValueCommandType.AlphaNumericPad;
                }
                else if (propertyName == "Time")
                {
                    return Type == ValueCommandType.Impulsive || Type == ValueCommandType.ImpulsiveLatch;
                }
                else if (propertyName == "DecimalUnaware")
                {
                    return Type == ValueCommandType.AppendValue || Type == ValueCommandType.RemoveValue;
                }
                else if (propertyName == "Expression")
                {
                    return Type != ValueCommandType.ResetStatistics;
                }
                else if (propertyName == "DelayCommandSecs")
                {
                    return Type != ValueCommandType.Impulsive && Type != ValueCommandType.ImpulsiveLatch;
                }
                else if (propertyName == "TransferToTag")
                    return Type == ValueCommandType.TransferValue;
                else if (propertyName == "Value")
                    return Type != ValueCommandType.TransferValue && Type != ValueCommandType.AlphaNumericPad &&
                           Type != ValueCommandType.NumericPad && Type != ValueCommandType.ResetStatistics && 
                           Type != ValueCommandType.RemoveValue && Type != ValueCommandType.SwapSign;
                else if (propertyName == "MaxValue" || propertyName == "TagMaxValue" || propertyName == "ReverseExpression" || propertyName == "AsPassword")
                {
                    return Type == ValueCommandType.AlphaNumericPad ||
                           Type == ValueCommandType.NumericPad;
                }
                else if (propertyName == "MinValue" || propertyName == "TagMinValue" || propertyName == "Decimals" || propertyName == "UseEUnit")
                {
                    return Type == ValueCommandType.NumericPad;
                }

                return base[propertyName];
            }
        }
#endif
        public override String CommandSummary
        {
            get
            {
                if (OpcuaEntityReference == null || !OpcuaEntityReference.IsValid)
                    return String.Empty;
                //return OpcuaEntityReference.HumanReadable;
                if (OpcuaEntityReference.ReadablePath == null || OpcuaEntityReference.AppName == null)
                    return OpcuaEntityReference.HumanReadable;
                //return OpcuaEntityReference.HumanReadable;
                Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                UInt16 ns = (UInt16)(n.Count + 2 - 1);
                string oldChars = string.Format("{0}:", ns);
                return string.Format("{0} ({1})", (OpcuaEntityReference.ReadablePath).Replace(oldChars, ""), OpcuaEntityReference.AppName);
            }
        }
#endif

        public override String Name
        {
            get
            {
                return Properties.Resources.ValueName;
            }
        }

        public override bool Init(IEntityReference entity, IDocument parent, String sessionname)
        {
            base.Init(entity, parent, sessionname);

            lockObject = new object();
#if !NET_STANDARD
            if (Control != null)
            {
                if (Type == ValueCommandType.Impulsive || 
                    Type == ValueCommandType.ImpulsiveLatch)
                {
#if !WINDOWS_UWP
                    Control.PreviewTouchDown += Control_TouchDown;
                    Control.PreviewTouchUp += Control_TouchUp;
                    Control.PreviewMouseDown += Control_MouseDown;
                    Control.PreviewMouseUp += Control_MouseUp;
                    //Control.GotMouseCapture += Control_GotMouseCapture;
                    Control.MouseLeave += Control_MouseLeave;
                    Control.TouchLeave += Control_TouchLeave;
#else
                    Control.PointerPressed += Control_MouseDown;
                    Control.PointerReleased += Control_MouseUp;
#endif
                }
            }
#endif

            if (!String.IsNullOrEmpty(Expression) || OpcuaEntityReference != null)
                sExpression = Expression;
            if (!String.IsNullOrEmpty(reverseExpression) || OpcuaEntityReference != null)
                sReverseExpression = reverseExpression;

            var ret = false;
            if (OpcuaEntityReference != null)
            {
                if (!String.IsNullOrEmpty(sExpression))
                {
                    observerItemViewModel = new PropertyObserver<OPCUAEntityReference>(OpcuaEntityReference)
                    .RegisterHandler(n => n.MonitoredItemViewModel, n =>
                    {
                        if (expressionEntity != null)
                        {
                            expressionEntity.ParserError -= ExpressionEntity_ParserError;
                            expressionEntity.ExecutionError -= ExpressionEntity_ExecutionError;
                            expressionEntity.Dispose();
                            expressionEntity = null;
                        }

                        if (n.MonitoredItemViewModel != null)
                        {
                            expressionEntity = ExpressionBucket.GetInstance(parent).AddExpression(n.MonitoredItemViewModel, sExpression, sReverseExpression, mapCurrentParameteItems);
                            expressionEntity.ParserError += ExpressionEntity_ParserError;
                            expressionEntity.ExecutionError += ExpressionEntity_ExecutionError;
                        }
                    });
                }

                OpcuaEntityReference.Resolve(SessionName, parent);
                OpcuaEntityReference.SetInUse(Entity, true);

                ret = true;
            }
#if !NET_STANDARD
            else if (!String.IsNullOrEmpty(sExpression))
            {
                if (Entity != null)
                {
                    FrameworkElement fe = null;
                    if (Entity.ContainedObject is ContentControl)
                    {
                        var control = Entity.ContainedObject as ContentControl;
                        fe = control.Content as FrameworkElement;
                    }

                    if (fe == null)
                        fe = Entity.ContainedObject as FrameworkElement;

                    if (fe != null)
                    {
                        var action = new Action(() =>
                        {
                            if (expressionEntity != null)
                            {
                                expressionEntity.ParserError -= ExpressionEntity_ParserError;
                                expressionEntity.ExecutionError -= ExpressionEntity_ExecutionError;
                                expressionEntity.Dispose();
                                expressionEntity = null;
                            }

                            var monitoredItemViewModel = fe.DataContext as MonitoredItemViewModel;
                            if (monitoredItemViewModel != null)
                            {
                                expressionEntity = ExpressionBucket.GetInstance(parent).AddExpression(monitoredItemViewModel.ReferenceViewModel ?? monitoredItemViewModel, sExpression, sReverseExpression, mapCurrentParameteItems);
                                expressionEntity.ParserError += ExpressionEntity_ParserError;
                                expressionEntity.ExecutionError += ExpressionEntity_ExecutionError;
                            }
                        });

                        if (!bExpressionEntityDataContextSubscribed)
                        {
                            bExpressionEntityDataContextSubscribed = true;
                            fe.DataContextChanged += (s, e) => action();
                        }
                        if (fe.DataContext != null)
                            action();
                    }
                }
            }
#endif

            if (Type == ValueCommandType.TransferValue && TransferToTag != null)
            {
                TransferToTag.Resolve(SessionName, parent);
                TransferToTag.SetInUse(Entity, true);
#if !NET_STANDARD
                PrepareAuditTrace(TransferToTag);
#endif
            }
#if !NET_STANDARD
            else if (Type != ValueCommandType.Impulsive && Type != ValueCommandType.ImpulsiveLatch)
                PrepareAuditTrace();
            else
                bAuditTreceFetched = true;
#endif

            if (Type == ValueCommandType.NumericPad && TagMinValue != null)
            {
                TagMinValue.Resolve(SessionName, parent);
                TagMinValue.SetInUse(Entity, true);
            }
            if ((Type == ValueCommandType.NumericPad || Type == ValueCommandType.AlphaNumericPad) &&
                TagMaxValue != null)
            {
                TagMaxValue.Resolve(SessionName, parent);
                TagMaxValue.SetInUse(Entity, true);
            }

            if (Type == ValueCommandType.ResetStatistics)
            {
                var editor = Parent?.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                if (editor != null)
                {
                    var opcString = editor.GetNodeIdEntityReference(Parent,
                        UFUAServerInfo.BrowserNames.ResetStatistics,
                        UFUAServerInfo.Guids.RootTagsGuid.ToString());
                    if (!String.IsNullOrEmpty(opcString))
                    {
                        var resetStatisticsMethod = opcString.FromXml<OPCUAEntityReference>();
                        resetStatisticsCommand = new CallMethodCommand()
                        {
#if !NET_STANDARD
                            Control = Control,
#endif
                            OpcuaEntityReference = resetStatisticsMethod,
                            InputParameters = new VariantCollection()
                        };
                        resetStatisticsCommand.Init(entity, parent, sessionname);
                    }
                }
            }

            return ret;
        }

        public override void RefreshAllEntityReferences()
        {
            base.RefreshAllEntityReferences();

            if (TransferToTag != null)
            {
                var xml = TransferToTag.ToXml();
                TransferToTag = xml.FromXml<OPCUAEntityReference>();
            }
            if (TagMinValue != null)
            {
                var xml = TagMinValue.ToXml();
                TagMinValue = xml.FromXml<OPCUAEntityReference>();
            }
            if (TagMaxValue != null)
            {
                var xml = TagMaxValue.ToXml();
                TagMaxValue = xml.FromXml<OPCUAEntityReference>();
            }
        }

        public override bool CanExecute()
        {
            if (Type == ValueCommandType.ResetStatistics)
            {
                if (resetStatisticsCommand == null || !resetStatisticsCommand.CanExecute())
                    return false;

                var model = GetItemViewModel();
                return model != null && model.monitoredItem != null;
            }
            else
            {
                if (!base.CanExecute()
#if !NET_STANDARD
                    || !bAuditTreceFetched
#endif
                    )
                    return false;
                var model = GetItemViewModel();
                var bCanExecute = /*OpcuaEntityReference != null || */model != null && model.DataValue != null &&
                    !StatusCode.IsBad(model.DataValue.StatusCode);
                if (!bCanExecute)
                    return false;

                if (Type == ValueCommandType.NumericPad &&
                    TagMinValue != null)
                {
                    var rangeModel = GetMinItemViewModel();
                    bCanExecute = rangeModel != null && rangeModel.DataValue != null &&
                        !StatusCode.IsBad(rangeModel.DataValue.StatusCode);
                }
                if (!bCanExecute)
                    return false;

                if ((Type == ValueCommandType.NumericPad || Type == ValueCommandType.AlphaNumericPad) &&
                    TagMaxValue != null)
                {
                    var rangeModel = GetMaxItemViewModel();
                    bCanExecute = rangeModel != null && rangeModel.DataValue != null &&
                        !StatusCode.IsBad(rangeModel.DataValue.StatusCode);
                }

                return bCanExecute;
            }
        }

        void ExpressionEntity_ParserError(object sender, EventArgs e)
        {
            if (OnExpressionParserError != null)
                OnExpressionParserError(sender, e);
        }

        void ExpressionEntity_ExecutionError(object sender, EventArgs e)
        {
            if (OnExpressionExecutionError != null)
                OnExpressionExecutionError(sender, e);
        }

#if !NET_STANDARD
        //private void Control_GotMouseCapture(object sender, MouseEventArgs e)
        //{
        //    if (bExecuteImpulsive && Control != null)
        //        Mouse.Capture(null);
        //}

        private void Control_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CheckImpulsiveCommandRelease();
        }

        private void Control_TouchLeave(object sender, TouchEventArgs e)
        {
            CheckImpulsiveCommandRelease();
        }

        void Control_MouseUp(object sender,
#if !WINDOWS_UWP
            System.Windows.Input.MouseButtonEventArgs e)
#else
            Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            CheckImpulsiveCommandRelease();
        }

        bool CheckImpulsiveCommandRelease()
        {
            if (Type == ValueCommandType.Impulsive ||
                Type == ValueCommandType.ImpulsiveLatch && Time == 0)
            {
                if (!bExecuteImpulsive)
                    return false;
                bExecuteImpulsive = false;
                // e.Handled = true;

                var model = GetItemViewModel();
                if (model != null)
                {
                    if (timer != null)
                    {
                        timer.Stop();
                        timer = null;
                    }

                    var action = new Action(() =>
                    {
                        try
                        {
                            if (expressionEntity == null || (Type == ValueCommandType.TransferValue && expressionEntity.TempVariable != model))
                                model.WriteValue(0);
                            else
                                expressionEntity.WriteValue(0, CultureInfo.InvariantCulture);
                        }
                        catch { }
                    });

                    if (Synchronous)
                    {
                        action();
                    }
                    else
                    {
                        AddNewWriteTask(action);
                    }
                }
                /*
                if (Type == ValueCommandType.Impulsive ||
                    Type == ValueCommandType.ImpulsiveLatch && Time == 0)
#if !WINDOWS_UWP
                    System.Windows.Input.Mouse.Capture(null);
#else
                    Control.ReleasePointerCapture(e.Pointer);
#endif
                */

                return true;
            }

            return false;
        }

        bool bExecutingImpulsive;
        void Control_MouseDown(object sender,
#if !WINDOWS_UWP
            System.Windows.Input.MouseButtonEventArgs e)
#else
            Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            Control_MouseOrTouchDown();
        }

#if !WINDOWS_UWP
        void Control_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            Control_MouseOrTouchDown(e);
        }

        void Control_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            bool bExecuted = CheckImpulsiveCommandRelease();

            if (e != null && bExecuted)
                isTouchUpHandled = true;
        }

        bool bExecuteImpulsive;
        void Control_MouseOrTouchDown(TouchEventArgs e = null)
        {
            if (Type == ValueCommandType.Impulsive ||
                Type == ValueCommandType.ImpulsiveLatch)
            {
                if (e != null)
                    isTouchDownHandled = true;

                /*
                if (Type == ValueCommandType.Impulsive ||
                    Type == ValueCommandType.ImpulsiveLatch && Time == 0)
#if !WINDOWS_UWP
                    System.Windows.Input.Mouse.Capture(Control);
#else
                    Control.CapturePointer(e.Pointer);
#endif
                */
                bExecutingImpulsive = true;
                try
                {
                    Execute();
                    bExecuteImpulsive = true;
                }
                catch (Exception ex)
                {
                    var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    ShowError(msg);
                }
                finally
                {
                    bExecutingImpulsive = false;
                }
            }
        }
#endif
#endif

        public override RemoteExecute RemoteExecute()
        {
            if (Type == ValueCommandType.Impulsive || Type == ValueCommandType.ImpulsiveLatch || Type == ValueCommandType.NumericPad || Type == ValueCommandType.AlphaNumericPad)
                return new RemoteExecute() { ex = new Exception(String.Format(Properties.Resources.RemoteCommandNotSupported, CommandSummary)) };

            try
            {
                Execute(true);
            }
            catch (Exception exception)
            {
                return new RemoteExecute() { ex = exception };
            }
            return null;
        }

        #region Validations
#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override String PerformValidation(String propertyName)
        {
            return base.PerformValidation(propertyName);
        }
#endif
        #endregion
        public override void BlindExecute()
        {
            if (
#if !NET_STANDARD
                IsAccessDenied() || 
#endif
                !CanExecute()
#if !NET_STANDARD
                || !CanExecuteDelayCommand()
#endif
                )
                return;
#if !NET_STANDARD
            if (!Synchronous &&
                Type != ValueCommandType.NumericPad &&
                Type != ValueCommandType.AlphaNumericPad)
#endif
            {
                Execute(true, isBlindExecuting: true);
            }
#if !NET_STANDARD
            else
            {
                ExecuteOnUserInterface(new Action(() => 
                {
                    Execute(true, isBlindExecuting: true);
                }));
            }
#endif
        }

#if !NET_STANDARD
        DispatcherTimer timer;
#else
        Timer timer;
#endif
        public override void Execute()
        {
            if (
#if !NET_STANDARD
                IsAccessDenied() || 
#endif
                !CanExecute()
#if !NET_STANDARD
                || !CanExecuteDelayCommand()
#endif
                )
                return;

            try
            {
#if !NET_STANDARD
                if (Type == ValueCommandType.Impulsive ||
                    Type == ValueCommandType.ImpulsiveLatch)
                {
                    if (bExecutingImpulsive)
                        Execute(false);
                }
                else
#endif
                Execute(false);
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
#if !NET_STANDARD
                ShowError(msg);
#else
                logCommands.Error(msg);
#endif
            }
        }

        public override void Terminate()
        {
            if (!bInitialized)
                return;

            if (resetStatisticsCommand != null)
                resetStatisticsCommand.Terminate();

            var model = GetItemViewModel();
            lock (listAppendDecimalONOFF)
            {
                if (listAppendDecimalONOFF.Contains(model))
                    listAppendDecimalONOFF.Remove(model);
            }

            bool bTimerStopped = false;
#if !NET_STANDARD
            if (timer != null)
            {
                bTimerStopped = true;
                timer.Stop();
                timer = null;
            }
#else
            lock (lockObject)
            {
                if (timer != null)
                {
                    bTimerStopped = true;
                    timer.Stop();
                    timer.Dispose();
                    timer = null;
                }
            }
#endif
            if (bTimerStopped)
            {
                try
                {
                    if (model != null)
                    {
                        TerminatePendingTasks();
                        if (expressionEntity == null || (Type == ValueCommandType.TransferValue && expressionEntity.TempVariable != model))
                            model.WriteValue(0);
                        else
                            expressionEntity.WriteValue(0, CultureInfo.InvariantCulture);
                    }
                }
                catch { }
            }

            if (OpcuaEntityReference != null)
                OpcuaEntityReference.SetInUse(Entity, false);
            if (observerItemViewModel != null)
            {
                observerItemViewModel.Dispose();
                observerItemViewModel = null;
            }
            if (expressionEntity != null)
            {
                expressionEntity.ParserError -= ExpressionEntity_ParserError;
                expressionEntity.ExecutionError -= ExpressionEntity_ExecutionError;
                expressionEntity.Dispose();
                expressionEntity = null;
            }

            if (Type == ValueCommandType.TransferValue && TransferToTag != null)
                TransferToTag.SetInUse(Entity, false);

            if (Type == ValueCommandType.NumericPad && TagMinValue != null)
                TagMinValue.SetInUse(Entity, false);
            if ((Type == ValueCommandType.NumericPad || Type == ValueCommandType.AlphaNumericPad) &&
                TagMaxValue != null)
                TagMaxValue.SetInUse(Entity, false);

            lock (lockObject)
            {
                if (subscribedModel != null)
                {
                    subscribedModel.PropertyChanged -= SubscribedModel_PropertyChanged;
                    subscribedModel = null;
                }
#if !NET_STANDARD
                if (subscribedMinItemViewModel != null)
                {
                    subscribedMinItemViewModel.PropertyChanged -= SubscribedMinMaxItemViewModel_PropertyChanged;
                    subscribedMinItemViewModel = null;
                }
                if (subscribedMaxItemViewModel != null)
                {
                    subscribedMaxItemViewModel.PropertyChanged -= SubscribedMinMaxItemViewModel_PropertyChanged;
                    subscribedMaxItemViewModel = null;
                }

                if (dpUpdate != null &&
                    dpUpdate.Status != DispatcherOperationStatus.Aborted &&
                    dpUpdate.Status != DispatcherOperationStatus.Completed)
                    dpUpdate.Abort();
#endif
            }
#if !NET_STANDARD
            if (Control != null)
            {
#if !WINDOWS_UWP
                Control.PreviewTouchDown -= Control_TouchDown;
                Control.PreviewTouchUp -= Control_TouchUp;
                Control.PreviewMouseDown -= Control_MouseDown;
                Control.PreviewMouseUp -= Control_MouseUp;
                //Control.GotMouseCapture -= Control_GotMouseCapture;
                Control.MouseLeave -= Control_MouseLeave;
                Control.TouchLeave -= Control_TouchLeave;
#else
                Control.PointerPressed -= Control_MouseDown;
                Control.PointerReleased -= Control_MouseUp;
#endif
            }
#endif

#if !NET_STANDARD
            TerminateAuditTrace();
#endif
            base.Terminate();
        }
        #endregion
    }
}
