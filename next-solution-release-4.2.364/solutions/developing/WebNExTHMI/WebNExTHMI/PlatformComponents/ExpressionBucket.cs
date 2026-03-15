using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Converters;
using DevExpress.Spreadsheet;

namespace WebNExTHMI.PlatformComponents
{
    //Expressions must reside in the same single thread
    public class ExpressionEvent: EventArgs
    {
        public OPCUAEntityReference reference;
        public ExpressionValueConverter converter;
        public Object value;
        public bool convertBack;
        public String formula;
        public String error;
        public BuiltInType? originalType;
    }

    public class ExpressionBucket : IDisposable
    {
        Dictionary<ComparableTuple<OPCUAEntityReference, bool>, Tuple<ExpressionValueConverter, Object, bool, Dictionary<string, object>>> mapReferenceConvertersToCalculate =
            new Dictionary<ComparableTuple<OPCUAEntityReference, bool>, Tuple<ExpressionValueConverter, Object, bool, Dictionary<string, object>>>();

        Dictionary<OPCUAEntityReference, ExpressionValueConverter> mapAddedReferenceConverters =
            new Dictionary<OPCUAEntityReference, ExpressionValueConverter>();
        Dictionary<OPCUAEntityReference, ExpressionValueConverter> mapRemovedReferenceConverters =
            new Dictionary<OPCUAEntityReference, ExpressionValueConverter>();

        bool bInitialized;
        bool bDisposed;
        Object lockObject = new Object();
        AutoResetEvent processEvent = new AutoResetEvent(false);
        Task mainTask;
        IWorkbook calcEngine;

        #region Events
        public event EventHandler<ExpressionEvent> ParserErrorEvent;
        void OnParserError(ExpressionEvent ee)
        {
            var e = ParserErrorEvent;
            if (e != null)
                e(null, ee);
        }

        public event EventHandler<ExpressionEvent> ParsedFormula;
        void OnParsedFormula(ExpressionEvent ee)
        {
            var e = ParsedFormula;
            if (e != null)
                e(null, ee);
        }

        public event EventHandler<ExpressionEvent> ExpressionEvaluated;
        void OnExpressionEvaluated(ExpressionEvent ee)
        {
            var e = ExpressionEvaluated;
            if (e != null)
                e(null, ee);
        }
        #endregion

        public ExpressionBucket()
        {
            Init();
        }

        public void AddExpression(OPCUAEntityReference reference, ExpressionValueConverter converter)
        {
            Init();

            lock (lockObject)
            {
                converter.CalcEngine = calcEngine;
                if (!mapAddedReferenceConverters.ContainsKey(reference))
                {
                    mapAddedReferenceConverters.Add(reference, converter);
                    processEvent.Set();
                }
            }
        }

        public void RemoveExpression(OPCUAEntityReference reference, ExpressionValueConverter converter)
        {
            lock (lockObject)
            {
                if (!mapRemovedReferenceConverters.ContainsKey(reference))
                {
                    mapRemovedReferenceConverters.Add(reference, converter);
                    processEvent.Set();
                }
            }
        }

        public void CalculateExpression(OPCUAEntityReference reference, ExpressionValueConverter converter,
            Object value, Dictionary<string, object> mapDynamics, bool convertBack = false)
        {
            lock (lockObject)
            {
                var key = new ComparableTuple<OPCUAEntityReference, bool>(reference, convertBack);
                if (mapReferenceConvertersToCalculate.ContainsKey(key))
                    mapReferenceConvertersToCalculate.Remove(key);

                mapReferenceConvertersToCalculate.Add(key, new Tuple<ExpressionValueConverter, Object, bool, Dictionary<string, object>>(converter, value, convertBack, mapDynamics));
                processEvent.Set();
            }
        }

        void Init()
        {
            lock (lockObject)
            {
                if (bInitialized)
                    return;
                bInitialized = true;
            }

            calcEngine = Utilities.Converters.ExpressionValueConverter.CreateCalcEngine();
            mainTask = Task.Run(() =>
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                while (!bDisposed)
                {
                    processEvent.WaitOne();

                    Dictionary<OPCUAEntityReference, ExpressionValueConverter> tempAdded = null;
                    Dictionary<OPCUAEntityReference, ExpressionValueConverter> tempRemoved = null;
                    Dictionary<ComparableTuple<OPCUAEntityReference, bool>, Tuple<ExpressionValueConverter, Object, bool, Dictionary<string, object>, BuiltInType?>> tempValues = null;

                    lock (lockObject)
                    {
                        if (mapAddedReferenceConverters.Count > 0)
                        {
                            tempAdded = new Dictionary<OPCUAEntityReference, ExpressionValueConverter>(mapAddedReferenceConverters);
                            mapAddedReferenceConverters.Clear();
                        }
                        if (mapRemovedReferenceConverters.Count > 0)
                        {
                            tempRemoved = new Dictionary<OPCUAEntityReference, ExpressionValueConverter>(mapRemovedReferenceConverters);
                            mapRemovedReferenceConverters.Clear();
                        }
                        if (mapReferenceConvertersToCalculate.Count > 0)
                        {
                            tempValues = new Dictionary<ComparableTuple<OPCUAEntityReference, bool>, Tuple<ExpressionValueConverter, Object, bool, Dictionary<string, object>, BuiltInType?>>();
                            foreach (var elem in mapReferenceConvertersToCalculate)
                            {
                                var originalType = (elem.Value.Item2 as DataValue)?.WrappedValue.TypeInfo.BuiltInType;
                                var valueTuple = new Tuple<ExpressionValueConverter, object, bool, Dictionary<string, object>, BuiltInType?>(elem.Value.Item1, elem.Value.Item2, elem.Value.Item3, elem.Value.Item4, originalType);
                                tempValues.Add(elem.Key, valueTuple);
                            }
                            mapReferenceConvertersToCalculate.Clear();
                        }
                    }

                    if (tempAdded != null)
                    {
                        foreach (var pair in tempAdded)
                        {
                            pair.Value.ParseFormula();
                            var parsererror = pair.Value.GetParserError();
                            if (!String.IsNullOrEmpty(parsererror))
                                OnParserError(new ExpressionEvent()
                                {
                                    reference = pair.Key,
                                    converter = pair.Value,
                                    error = parsererror,
                                    formula = pair.Value.Formula
                                });
                            else {
                                var e = new ExpressionEvent()
                                {
                                    reference = pair.Key,
                                    converter = pair.Value,
                                    formula = pair.Value.Formula
                                };
                                OnParsedFormula(e);
                                if (!String.IsNullOrEmpty(e.error))
                                    OnParserError(e);
                            }
                        }
                    }

                    if (tempRemoved != null)
                    {
                        foreach (var pair in tempRemoved)
                        {
                            lock (lockObject)
                            {
                                var key = new ComparableTuple<OPCUAEntityReference, bool>(pair.Key, false);
                                if (mapReferenceConvertersToCalculate.ContainsKey(key))
                                    mapReferenceConvertersToCalculate.Remove(key);
                            }

                            pair.Value.Dispose();
                        }
                    }

                    if (tempValues != null)
                    {
                        foreach (var pair in tempValues)
                        {
                            if (pair.Value.Item3)
                            {
                                object convertedValue;
                                lock (pair.Value.Item4)
                                {
                                    convertedValue = pair.Value.Item1.ConvertBack(pair.Value.Item2, null, pair.Value.Item4, CultureInfo.InvariantCulture);
                                }
                                OnExpressionEvaluated(new ExpressionEvent()
                                {
                                    reference = pair.Key.First,
                                    converter = pair.Value.Item1,
                                    value = convertedValue,
                                    convertBack = true,
                                    originalType = pair.Value.Item5
                                });
                            }
                            else
                            {
                                if (pair.Value.Item2 is DataValue)
                                {
                                    var dv = pair.Value.Item2 as DataValue;
                                    lock (pair.Value.Item4)
                                    {
                                        dv.Value = pair.Value.Item1.Convert(dv.Value, null, pair.Value.Item4, CultureInfo.InvariantCulture);
                                    }
                                    OnExpressionEvaluated(new ExpressionEvent()
                                    {
                                        reference = pair.Key.First,
                                        converter = pair.Value.Item1,
                                        value = dv,
                                        convertBack = false,
                                        originalType = pair.Value.Item5
                                    });
                                }
                                else
                                {
                                    object convertedValue;
                                    lock (pair.Value.Item4)
                                    {
                                        convertedValue = pair.Value.Item1.Convert(pair.Value.Item2, null, pair.Value.Item4, CultureInfo.InvariantCulture);
                                    }
                                    OnExpressionEvaluated(new ExpressionEvent()
                                    {
                                        reference = pair.Key.First,
                                        converter = pair.Value.Item1,
                                        value = convertedValue,
                                        convertBack = false,
                                        originalType = pair.Value.Item5
                                    });
                                }
                            }
                        }
                    }
                }
            });
        }

        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            try
            {
                if (mainTask != null)
                {
                    processEvent.Set();
                    mainTask.Wait();
                }
            }
            catch { }

            calcEngine?.Dispose();
            processEvent.Dispose();

            lock (lockObject)
            {
                mapAddedReferenceConverters.Clear();
                mapReferenceConvertersToCalculate.Clear();
                mapRemovedReferenceConverters.Clear();
            }
        }
    }

    public class ComparableTuple<T, T2>
    {
        public ComparableTuple(T first, T2 second)
        {
            First = first;
            Second = second;
        }
        public T First { get; set; }
        public T2 Second { get; set; }

        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }

        public override bool Equals(object other)
        {
            ComparableTuple<T, T2> t = other as ComparableTuple<T, T2>;
            return t != null && t.First.Equals(First) && t.Second.Equals(Second);
        }

    }
}
