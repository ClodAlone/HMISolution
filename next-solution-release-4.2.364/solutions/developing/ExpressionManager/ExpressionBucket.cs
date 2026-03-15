using DocumentManager.ComponentService;
using DocumentManager.ComponentService.Helpers;
using DevExpress.Spreadsheet;
using log4net;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace ExpressionManager
{
    public class ExpressionBucket : IDisposable
    {
        #region Declarations
        readonly IDocument parent;
        readonly Dictionary<MonitoredItemViewModel, Dictionary<String, ExpressionEntity>> mapExpressions = new Dictionary<MonitoredItemViewModel, Dictionary<String, ExpressionEntity>>();
        readonly Dictionary<MonitoredItemViewModel, ExpressionEntity> mapTempVariables = new Dictionary<MonitoredItemViewModel, ExpressionEntity>();
        //readonly Dictionary<OPCUAEntityReference, PropertyObserver<OPCUAEntityReference>> mapObservers = new Dictionary<OPCUAEntityReference, PropertyObserver<OPCUAEntityReference>>();
        //readonly Dictionary<MonitoredItemViewModel, MonitoredItemViewModel> mapReferenceVariables = new Dictionary<MonitoredItemViewModel, MonitoredItemViewModel>();

        Object lockObject = new Object();
        Thread mainThread;
        AutoResetEvent processEvent;
        IWorkbook calcEngine;

#if !NET_STANDARD
        internal static readonly ILog log = LogManager.GetLogger(Properties.Resources.ExpressionManagerLog);
#else
        internal static readonly ILog log = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.ExpressionManagerLog);
#endif

        const string RegexExpressionKey = @"\s";

        bool bDisposed;
        #endregion

        #region Constructors
        private ExpressionBucket(IDocument parent)
        {
            this.parent = parent;
        }
        #endregion

        #region Singleton
        static Dictionary<IDocument, ExpressionBucket> singletonInstance = new Dictionary<IDocument, ExpressionBucket>();
        static public ExpressionBucket GetInstance(IDocument parent)
        {
            lock (singletonInstance)
            {
                var rootParent = DocumentHelper.GetRootParent(parent, false);
                if (!singletonInstance.ContainsKey(rootParent))
                    singletonInstance.Add(rootParent, new ExpressionBucket(rootParent));
                return singletonInstance[rootParent];
            }
        }

        static public bool IsRunning(IDocument parent)
        {
            lock (singletonInstance)
            {
                var rootParent = DocumentHelper.GetRootParent(parent, false);
                return singletonInstance.ContainsKey(rootParent);
            }
        }
        #endregion

        #region Public Methods
        //public void RegisterExpression(IDocument parent, OPCUAEntityReference reference, string formula, string reverseFormula)
        //{
        //    if (mapObservers.ContainsKey(reference))
        //        throw new Exception("Cannot register twice the same reference expression.");

        //    var observer = new PropertyObserver<OPCUAEntityReference>(reference)
        //        .RegisterHandler(n => n.MonitoredItemViewModel, n =>
        //        {
        //            if (n.MonitoredItemViewModel != null)
        //            {
        //                var expressionEntity = AddExpression(parent, n.MonitoredItemViewModel, formula, reverseFormula);
        //                expressionEntity.SetInUse(true);
        //            }
        //        });
        //    //reference.Dispose += 
        //    mapObservers.Add(reference, observer);

        //}

        public ExpressionEntity AddExpression(MonitoredItemViewModel variable, string formula, string reverseFormula)
        {
            return AddExpression(variable, formula, reverseFormula, null);
        }

        public ExpressionEntity AddExpression(MonitoredItemViewModel variable, string formula, string reverseFormula, IDictionary<String, String> aliasItems)
        {
            EnsureThread();

            if (aliasItems != null)
            {
                formula = ReplaceAliasItems(formula, aliasItems);
                reverseFormula = ReplaceAliasItems(reverseFormula, aliasItems);
            }

            lock (lockObject)
            {
                if (!mapExpressions.ContainsKey(variable))
                    mapExpressions.Add(variable, new Dictionary<String, ExpressionEntity>());
                var expressions = mapExpressions[variable];
                var key = GetUniqueKey(formula, reverseFormula);
                if (!expressions.ContainsKey(key))
                {
                    var expressionEntity = new ExpressionEntity(parent, calcEngine, variable, formula, reverseFormula);
                    expressions.Add(key, expressionEntity);
                    mapTempVariables.Add(expressionEntity.TempVariable, expressionEntity);
                    //mapReferenceVariables.Add(expressionEntity.TempVariable, variable);
                    expressionEntity.Invalidated += ExpressionEntity_Invalidated;
                }

                expressions[key].SetInUse(true);
                if (!expressions[key].IsInitialized)
                    processEvent?.Set();

                return expressions[key];
            }
        }

        public static String CheckExpression(string formula, string reverseFormula)
        {
            return CheckExpression(formula, reverseFormula, -1);
        }

        public static String CheckExpression(string formula, string reverseFormula, int maxVariables = -1)
        {
            var expressionEntity = new ExpressionEntity(formula, reverseFormula);
            try
            {
                expressionEntity.Initialize();
                var error = expressionEntity.GetParserError();
                if (!String.IsNullOrEmpty(error))
                    return String.Format(Properties.Resources.ExpressionSyntaxError, error);
                else if (!expressionEntity.IsValid || (maxVariables >= 0 && expressionEntity.ListParsedVariables.Count > maxVariables))
                    return Properties.Resources.ExpressionSyntaxInvalid;
                return null;
            }
            finally
            {
                expressionEntity.Terminate();
            }
        }

        public ExpressionEntity GetExpression(MonitoredItemViewModel tempVariable)
        {
            lock (lockObject)
            {
                if (mapTempVariables.ContainsKey(tempVariable))
                    return mapTempVariables[tempVariable];
            }

            return null;
        }

        public List<ExpressionEntity> GetAllExpression()
        {
            lock (lockObject)
            {
                return mapTempVariables.Values.ToList();
            }
        }

        //public MonitoredItemViewModel GetReferenceVariable(MonitoredItemViewModel tempVariable)
        //{
        //    lock (lockObject)
        //    {
        //        if (mapReferenceVariables.ContainsKey(tempVariable))
        //            return mapReferenceVariables[tempVariable];
        //    }

        //    return null;
        //}

        #region IDisposable
        public void Dispose()
        {
            Thread thread = null;
            lock (lockObject)
            {
                if (bDisposed)
                    return;
                bDisposed = true;
                
                thread = mainThread;
                processEvent?.Set();
            }

            if (thread != null)
                thread.Join();

            lock (lockObject)
            {
                calcEngine?.Dispose();
                processEvent?.Dispose();
                RemoveExpressionEntities(mapTempVariables.Values.ToList());
                mapExpressions.Clear();
                mapTempVariables.Clear();
            }

            lock (singletonInstance)
                singletonInstance.Remove(parent);
        }
        #endregion
        #endregion

        #region Private Methods
        void EnsureThread()
        {
            lock (lockObject)
            {
                if (bDisposed)
                    return;

                if (mainThread == null)
                {
                    calcEngine = Utilities.Converters.ExpressionValueConverter.CreateCalcEngine();
                    processEvent = new AutoResetEvent(false);
                    mainThread = new Thread(() =>
                    {
                        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                        while (!bDisposed)
                        {
                            processEvent.WaitOne();

                            List<ExpressionEntity> listAllExpressions = null;
                            lock (lockObject)
                            {
                                listAllExpressions = mapTempVariables.Values.ToList();
                            }

                            var listToInitiliaze = (from c in listAllExpressions
                                                    where !c.IsInitialized
                                                    select c).ToList();
                            var listToCalculate = (from c in listAllExpressions
                                                   where c.IsInitialized && c.InUse && !c.IsExecuted
                                                   select c).ToList();
                            var listToRemove = (from c in listAllExpressions
                                                where (c.IsInitialized || !c.IsValid) && !c.InUse
                                                select c).ToList();

                            listToInitiliaze.ForEach((expressionEntity) => expressionEntity.Initialize());
                            listToCalculate.ForEach((expressionEntity) => expressionEntity.Calculate());
                            listToRemove.ForEach((expressionEntity) => expressionEntity.Terminate());
                            RemoveExpressionEntities(listToRemove);
                        }
                    });
                    mainThread.IsBackground = true;
                    mainThread.Start();
                }
            }
        }

        void RemoveExpressionEntities(List<ExpressionEntity> expressionEntities)
        {
            if (expressionEntities.Count > 0)
            {
                lock (lockObject)
                {
                    expressionEntities.ForEach((expressionEntity) =>
                    {
                        var key = GetUniqueKey(expressionEntity);
                        mapTempVariables.Remove(expressionEntity.TempVariable);
                    //mapReferenceVariables.Remove(expressionEntity.TempVariable);
                    if (mapExpressions.ContainsKey(expressionEntity.ReferenceVariable))
                            mapExpressions[expressionEntity.ReferenceVariable].Remove(key);
                        if (mapExpressions[expressionEntity.ReferenceVariable].Count == 0)
                            mapExpressions.Remove(expressionEntity.ReferenceVariable);
                        expressionEntity.Invalidated -= ExpressionEntity_Invalidated;
                        expressionEntity.Terminate();
                    });
                }
            }
        }

        void ExpressionEntity_Invalidated(object sender, EventArgs e)
        {
            processEvent?.Set();
        }

        String ReplaceAliasItems(string formula, IDictionary<String, String> mapAliasItems)
        {
            if (!String.IsNullOrEmpty(formula))
            {
                foreach (var key in mapAliasItems.Keys)
                {
                    var searchPath = Utilities.NamespaceTableConverter.GetRelativePathValue(key);
                    var replacePath = Utilities.NamespaceTableConverter.GetRelativePathValue(mapAliasItems[key]);
                    if (searchPath != replacePath)
                        formula = Regex.Replace(formula, Regex.Escape(searchPath), replacePath, RegexOptions.IgnoreCase);
                }
            }

            return formula;
        }

        String GetUniqueKey(ExpressionEntity expression)
        {
            return GetUniqueKey(expression.Formula, expression.ReverseFormula);
        }

        String GetUniqueKey(string formula, string reverseFormula)
        {
            var uniqueKey = Regex.Replace(formula, RegexExpressionKey, String.Empty).ToLower();
            if (!String.IsNullOrEmpty(reverseFormula))
                uniqueKey = string.Format("{0}@{1}", uniqueKey, Regex.Replace(reverseFormula, RegexExpressionKey, String.Empty).ToLower());
            return uniqueKey;
        }
        #endregion
    }
}
