using DocumentManager.ComponentService;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UFUAEditor.ComponentService;
using Utilities;
using ViewModelLib;
using OPCUAViewModel;
using Utilities.Converters;
using DevExpress.Spreadsheet;
#if !NET_STANDARD
using System.Windows.Data;
#endif

namespace ExpressionManager
{
    public class ExpressionEntity : IDisposable
    {
        #region Declarations
        Object lockObject = new Object();

        readonly IDocument documentParent;
        readonly MonitoredItemViewModel referenceItemViewModel;
        readonly MonitoredItemViewModel monitoredItemViewModel;
        readonly ExpressionValueConverter converter;

        List<PropertyObserver<OPCUAEntityReference>> listObserverReferences = new List<PropertyObserver<OPCUAEntityReference>>();
        Dictionary<MonitoredItemViewModel, PropertyObserver<MonitoredItemViewModel>> mapObserverValues = new Dictionary<MonitoredItemViewModel, PropertyObserver<MonitoredItemViewModel>>();
        List<OPCUAEntityReference> inuseVariableValueReferencesExpression = new List<OPCUAEntityReference>();
        Dictionary<String, Object> mapDynamics = new Dictionary<String, Object>();

        bool isInitialized;
        bool isExecuted;
        bool isTerminated;

        bool bCalculating;
        bool bConvertBack;

        int indexArray = -1;
        int indexBit = -1;

        Type targetType;

        long inUseCounter;
        String lastValue;
        Exception lastException;
        String internalParserError;

        static List<String> validStatusCodes = new List<String>()
        {
            new Opc.Ua.StatusCode(Opc.Ua.StatusCodes.Good).ToString(),
            new Opc.Ua.StatusCode(Opc.Ua.StatusCodes.UncertainLastUsableValue).ToString()
        };
        #endregion

        #region Constructors
        internal ExpressionEntity(string formula, string reverseFormula)
        {
            converter = new ExpressionValueConverter()
            {
                Formula = formula,
                ReverseFormula = reverseFormula
            };
        }

        internal ExpressionEntity(IDocument parent, IWorkbook calcEngine, MonitoredItemViewModel variable, string formula, string reverseFormula)
        {
            documentParent = parent;
            referenceItemViewModel = variable;
            monitoredItemViewModel = new MonitoredItemViewModel(reference: variable);
            monitoredItemViewModel.DataValue = new DataValue(StatusCodes.BadWaitingForInitialData);
            converter = new ExpressionValueConverter()
            {
                CalcEngine = calcEngine,
                Formula = formula,
                ReverseFormula = reverseFormula
            };
            converter.ThrowExceptions = true;
            converter.ParserErrorEvent += Converter_ParserErrorEvent;
        }
        #endregion

        #region Events
        public event EventHandler ParserError;
        void OnParserError()
        {
            var e = ParserError;
            if (e != null)
                e(this, EventArgs.Empty);
        }

        public event EventHandler ExecutionError;
        void OnExecutionError()
        {
            var e = ExecutionError;
            if (e != null)
                e(this, EventArgs.Empty);
        }

        internal event EventHandler Invalidated;
        void OnInvalidated()
        {
            var e = Invalidated;
            if (e != null)
                e(this, EventArgs.Empty);
        }
        #endregion

        #region Properties
        public MonitoredItemViewModel ReferenceVariable
        {
            get
            {
                return referenceItemViewModel;
            }
        }

        public MonitoredItemViewModel TempVariable
        {
            get
            {
                return monitoredItemViewModel;
            }
        }

        public IValueConverter Converter
        {
            get
            {
                return converter;
            }
        }

        public String Formula
        {
            get
            {
                return converter.Formula;
            }
        }

        public String ReverseFormula
        {
            get
            {
                return converter.ReverseFormula;
            }
        }

        public List<String> ListParsedVariables
        {
            get
            {
                return converter.GetAllParsedVariables();
            }
        }

        internal bool IsInitialized
        {
            get
            {
                return isInitialized;
            }
        }

        internal bool IsValid
        {
            get
            {
                return converter.Type != ExpressionType.none && converter.Type != ExpressionType.error;
            }
        }

        internal bool IsExecuted
        {
            get
            {
                return isExecuted;
            }
        }

        internal bool InUse
        {
            get
            {
                return Interlocked.Read(ref inUseCounter) > 0;
            }
        }
        #endregion

        #region Methods
        internal long SetInUse(bool inUse)
        {
            long current = inUse ? Interlocked.Increment(ref inUseCounter) : Interlocked.Decrement(ref inUseCounter);
            return current;
        }

        internal void Initialize()
        {
            if (IsInitialized)
                return;

            converter.ParseFormula();
            if (referenceItemViewModel == null)
                return;
            else if (!IsValid)
            {
                OnParserError();
                return;
            }

            int arrayIndex;
            int bitNumber;
            var type = ExpressionValueConverter.GetFormulaType(converter.Formula, out arrayIndex, out bitNumber);
            if (type == ExpressionType.Bit)
                indexBit = bitNumber;
            else if (type == ExpressionType.Array || type == ExpressionType.ArrayPlusBit)
            {
                indexArray = arrayIndex;
                indexBit = bitNumber;
            }

            if (referenceItemViewModel != null)
            {
                var observerValue = new PropertyObserver<MonitoredItemViewModel>(referenceItemViewModel);
                mapObserverValues.Add(referenceItemViewModel, observerValue);
                observerValue.RegisterHandler(m => m.DataValue, m =>
                {
                    if (bCalculating)
                        return;

                    if (m.DataValue != null)
                    {
                        lock (lockObject)
                        {
                            isExecuted = false;
                        }

                        OnInvalidated();
                    }
                });

                observerValue = new PropertyObserver<MonitoredItemViewModel>(monitoredItemViewModel);
                mapObserverValues.Add(monitoredItemViewModel, observerValue);
                observerValue.RegisterHandler(m => m.DataValue, m =>
                {
                    if (bCalculating)
                        return;

                    if (m.DataValue != null && m.Value != lastValue)
                    {
                        lock (lockObject)
                        {
                            isExecuted = false;
                            bConvertBack = true;
                        }

                        OnInvalidated();
                    }
                });
            }

            var ufuaEditor = documentParent?.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (ufuaEditor != null)
            {
                var listVariables = converter.GetAllParsedVariables();
                listVariables.ForEach(variable =>
                {
                    var erString = ufuaEditor.GetTagEntityReference(documentParent, variable, null, inExecution: true);
                    OPCUAEntityReference reference = null;
                    if (!String.IsNullOrEmpty(erString))
                        reference = erString.FromXml<OPCUAEntityReference>();

                    if (reference == null)
                    {
                        internalParserError = String.Format(Properties.Resources.ExpressionTagMissing, variable);
                        OnParserError();
                        return;
                    }

                    lock (lockObject)
                    {
                        if (isTerminated)
                            return;

                        if (reference != null && !inuseVariableValueReferencesExpression.Contains(reference))
                        {
                            var observer = new PropertyObserver<OPCUAEntityReference>(reference)
                                .RegisterHandler(n => n.MonitoredItemViewModel, n =>
                                {
                                    if (n.MonitoredItemViewModel == null || isTerminated)
                                        return;

                                    try
                                    {
                                        if (n.MonitoredItemViewModel.NodeIdModel != null &&
                                            !n.MonitoredItemViewModel.NodeIdModel.IsEventNotifier)
                                        {
                                            var datavalue = n.MonitoredItemViewModel.DataValue;
                                            if (datavalue?.Value != null)
                                            {
                                                lock (lockObject)
                                                {
                                                    UpdateMapDynamics(reference);
                                                    isExecuted = false;
                                                }

                                                OnInvalidated();
                                            }
                                        }
                                    }
                                    catch { }

                                    lock (lockObject)
                                    {
                                        if (isTerminated)
                                            return;

                                        var observerMonitoredModel = new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel);
                                        if (mapObserverValues.ContainsKey(n.MonitoredItemViewModel))
                                        {
                                            mapObserverValues[n.MonitoredItemViewModel].Dispose();
                                            mapObserverValues.Remove(n.MonitoredItemViewModel);
                                        }
                                        mapObserverValues.Add(n.MonitoredItemViewModel, observerMonitoredModel);
                                        observerMonitoredModel.RegisterHandler(m => m.DataValue, m =>
                                        {
                                            if (m.DataValue != null)
                                            {
                                                lock (lockObject)
                                                {
                                                    UpdateMapDynamics(reference);
                                                    isExecuted = false;
                                                }

                                                OnInvalidated();
                                            }
                                        });
                                    }
                                });

                            listObserverReferences.Add(observer);
                            inuseVariableValueReferencesExpression.Add(reference);
                        }
                    }

                    if (reference != null)
                    {
                        reference.Resolve(documentParent.Title, documentParent);
                        reference.SetInUse(referenceItemViewModel, true);
                    }
                });
            }

            isInitialized = true;
            Calculate();
        }

        internal void Calculate()
        {
            lock (lockObject)
            {
                if (isTerminated || isExecuted)
                    return;

                isExecuted = true;
                bCalculating = true;
                try
                {
                    if (!IsReady())
                    {
                        var dataValue = new DataValue(StatusCodes.BadWaitingForInitialData);
                        monitoredItemViewModel.DataValue = dataValue;
                        return;
                    }

                    if (bConvertBack)
                    {
                        var dataValue = monitoredItemViewModel.DataValue;
                        var newValue = converter.ConvertBack(dataValue.Value, null, mapDynamics, System.Threading.Thread.CurrentThread.CurrentCulture);
                        if (newValue != null)
                        {
                            referenceItemViewModel.WriteValue(newValue, indexArray, indexBit);
                            newValue = converter.Convert(referenceItemViewModel.DataValue.Value, null, mapDynamics, System.Threading.Thread.CurrentThread.CurrentCulture);
                            monitoredItemViewModel.DataValue = new DataValue(new Variant(newValue), dataValue.StatusCode, DateTime.UtcNow);
                            lastValue = monitoredItemViewModel.Value;
                            lastException = null;
                        }
                    }
                    else
                    {
                        var dataValue = referenceItemViewModel.DataValue;
                        var newValue = converter.Convert(dataValue.Value, null, mapDynamics, System.Threading.Thread.CurrentThread.CurrentCulture);
                        if (newValue != null)
                        {
                            if (targetType == null)
                            {
                                if (indexBit >= 0)
                                    targetType = typeof(Boolean);
                                else if (indexArray >= 0)
                                {
                                    if (dataValue.Value is Array)
                                        targetType = (dataValue.Value as Array).GetType().GetElementType();
                                    else
                                        targetType = dataValue.Value.GetType();
                                }
                                else if (TypeInfo.IsNumericType(dataValue.WrappedValue.TypeInfo.BuiltInType))
                                    targetType = typeof(Double);
                                else
                                    targetType = typeof(String);
                            }
                            newValue = System.Convert.ChangeType(newValue, targetType);
                            monitoredItemViewModel.DataValue = new DataValue(new Variant(newValue), dataValue.StatusCode, DateTime.UtcNow);
                            lastValue = monitoredItemViewModel.Value;
                            lastException = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (bConvertBack)
                    {
                        monitoredItemViewModel.Value = lastValue;
                    }
                    else
                    {
                        var dataValue = new DataValue(StatusCodes.BadWaitingForInitialData);
                        monitoredItemViewModel.DataValue = dataValue;
                        lastValue = monitoredItemViewModel.Value;
                    }
                    //ExpressionBucket.log.ErrorFormat(Properties.Resources.ExpressionExecutionError, ex.Message);
                    lastException = ex;
                    OnExecutionError();
                }
                finally
                {
                    bCalculating = false;
                    bConvertBack = false;
                }
            }
        }

        internal void Terminate()
        {
            var inUseReferences = new List<OPCUAEntityReference>();

            lock (lockObject)
            {
                if (isTerminated)
                    return;
                isTerminated = true;

                inUseReferences.AddRange(inuseVariableValueReferencesExpression);
                inuseVariableValueReferencesExpression.Clear();

                listObserverReferences.ForEach((item) => item.Dispose());
                listObserverReferences.Clear();

                mapObserverValues.Values.ToList().ForEach((item) => item.Dispose());
                mapObserverValues.Clear();

                if (monitoredItemViewModel != null)
                    monitoredItemViewModel.Dispose();

                converter.ParserErrorEvent -= Converter_ParserErrorEvent;
                converter.Dispose();
            }

            inUseReferences.ForEach((item) => item.SetInUse(referenceItemViewModel, false));
        }

        void UpdateMapDynamics(OPCUAEntityReference reference)
        {
            if (IsValidQuality(reference.MonitoredItemViewModel?.Quality))
            {
                var value = reference.MonitoredItemViewModel.InvariantCultureValue;
                if (reference.NodeIdViewModel != null)
                {
                    try
                    {
                        mapDynamics[reference.NodeIdViewModel.BrowseName.Name] = value;
                        mapDynamics[reference.NodeIdViewModel.CompletePath] = value;
                    }
                    catch { }
                }
                else if (!String.IsNullOrEmpty(reference.RelativePath))
                    mapDynamics[reference.RelativePath] = value;
                mapDynamics[reference.HumanReadableNoProject] = value;
            }
        }

        bool IsValidQuality(String quality)
        {
            return !String.IsNullOrEmpty(quality) && validStatusCodes.Contains(quality);
        }

        bool IsReady()
        {
            if (referenceItemViewModel != null && !IsValidQuality(referenceItemViewModel.Quality))
                return false;

            foreach (var reference in inuseVariableValueReferencesExpression)
            {
                var monitoredItemViewModel = reference.MonitoredItemViewModel;
                if (monitoredItemViewModel == null || !IsValidQuality(monitoredItemViewModel.Quality))
                    return false;
            }

            return true;
        }

        void Converter_ParserErrorEvent(object sender, EventArgs e)
        {
            OnParserError();
        }

        public String GetParserError()
        {
            return internalParserError ?? converter.GetParserError();
        }

        public Exception GetExecutionError()
        {
            return lastException;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (SetInUse(false) == 0)
                OnInvalidated();
        }

        public void WriteValue(object value)
        {
            WriteValue(value, System.Threading.Thread.CurrentThread.CurrentCulture);
        }

        public void WriteValue(object value, System.Globalization.CultureInfo culture)
        {
            var newValue = converter.ConvertBack(value, null, mapDynamics, culture);
#if !NET_STANDARD
            if (newValue is System.Windows.Controls.ValidationResult)
            {
                var result = (newValue as System.Windows.Controls.ValidationResult);
                throw new Exception(result.ErrorContent as String);
            }
#else
            if (newValue is System.ComponentModel.DataAnnotations.ValidationResult)
            {
                var result = (newValue as System.ComponentModel.DataAnnotations.ValidationResult);
                throw new Exception(result.ErrorMessage);
            }
#endif
            else if (newValue != null)
            {
                referenceItemViewModel.WriteValue(newValue, indexArray, indexBit);
            }
        }
        #endregion
    }
}
