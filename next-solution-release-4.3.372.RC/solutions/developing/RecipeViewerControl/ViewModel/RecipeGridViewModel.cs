using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using UFRecipeSettings.Documents;
using UFUAEditor.ComponentService;
using Utilities;

namespace RecipeViewerControl.ViewModel
{
    internal class RecipeGridViewModel : Observable, IDisposable
    {
        #region Declarations
        internal readonly UFRecipeDocument recipeDocument;
        readonly DataSet dataSet;
        readonly ILog logTrace;

        DispatcherOperation dpOperation;
        #endregion

        #region Constructors
        public RecipeGridViewModel()
        {
            isBusy = true;
        }

        public RecipeGridViewModel(UFRecipeDocument recipeDocument, DataSet dataSet, Dictionary<string,List<Tuple<string, string>>> mapWrongValues)
        {
            this.recipeDocument = recipeDocument;
            this.dataSet = dataSet;
            this.logTrace = LogManager.GetLogger(recipeDocument.Title);
            this.mapWrongValues = mapWrongValues;
        }
        #endregion

        #region Properties
        ObservableCollection<RecipeValueViewModel> dataValues;
        public ObservableCollection<RecipeValueViewModel> DataValues
        {
            get
            {
                if (!IsReady)
                    return null;

                if (recipeDocument != null && dataValues == null)
                {
                    dataValues = new ObservableCollection<RecipeValueViewModel>();
                    recipeDocument.RecipeEntity.GetFlatDataValuesCollection().ForEach((value) =>
                    {
                        if (value.ArrayDimension > 0)
                        {
                            for (int ii = 0; ii < value.ArrayDimension; ii++)
                                dataValues.Add(new RecipeValueViewModel(value, this, ii));
                        }
                        else
                            dataValues.Add(new RecipeValueViewModel(value, this));
                    });
                }

                return dataValues;
            }
        }

        public DataSet DataSet
        {
            get
            {
                return dataSet;
            }
        }

        bool isReady;
        public bool IsReady
        {
            get
            {
                return recipeDocument != null && dataSet != null && isReady;
            }
            set
            {
                if (Set(ref isReady, value, "IsReady"))
                {
                    OnPropertyChanged("DataValues");
                }
            }
        }

        bool isBusy;
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                if (Set(ref isBusy, value, "IsBusy"))
                {
                    lastError = null;
                    OnPropertyChanged("WaitText");
                    OnPropertyChanged("LastError");
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        bool promptPad;
        public bool PromptPad
        {
            get
            {
                return promptPad;
            }
            set
            {
                Set(ref promptPad, value, "PromptPad");
            }
        }

        string waitText;
        public string WaitText
        {
            get
            {
                if (String.IsNullOrEmpty(waitText))
                    return UFRecipeLayout.Properties.Resources.WaitText;
                else
                    return waitText;
            }
            set
            {
                Set(ref waitText, value, "WaitText");
            }
        }

        string lastError;
        public string LastError
        {
            get
            {
                return lastError;
            }
            set
            {
                if (Set(ref lastError, value, "LastError"))
                {
                    if (logTrace != null && !String.IsNullOrEmpty(lastError))
                        logTrace.Error(lastError);
                    isBusy = false;
                    OnPropertyChanged("IsBusy");
                }
            }
        }

        IUFUAEditorManager ufuaEditorService;
        internal IUFUAEditorManager UFUAEditorService
        {
            get
            {
                if (ufuaEditorService == null)
                    ufuaEditorService = recipeDocument.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                return ufuaEditorService;
            }
        }

        private Dictionary<string, List<Tuple<string, string>>> mapWrongValues;
        /// <summary>
        /// The key represent the recipe ID
        /// The value is composed by the column name and the error
        /// </summary>
        public Dictionary<string, List<Tuple<string, string>>> MapWrongValues
        {
            get 
            { 
                return mapWrongValues; 
            }
            set 
            {
                //Create a new dictionary in order to avoid to lose the data with the Clear of the dictionary
                Dictionary<string, List<Tuple<string, string>>> temp = new Dictionary<string, List<Tuple<string, string>>>(value);
                if (mapWrongValues == null)
                    mapWrongValues = new Dictionary<string, List<Tuple<string, string>>>();
                mapWrongValues.Clear();

                if (temp != null)
                {
                    foreach(var item in temp)
                        mapWrongValues.Add(item.Key, item.Value);
                }
            }
        }
        #endregion

        #region Methods
        public void ExecuteAsync(Dispatcher dispatcher, Action action)
        {
            if (dpOperation != null && 
                (dpOperation.Status == DispatcherOperationStatus.Pending || 
                dpOperation.Status == DispatcherOperationStatus.Executing))
                return;

            dpOperation = dispatcher.BeginInvokeAsynchronously(action);
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (dpOperation != null)
                dpOperation.Abort();

            if (dataValues != null)
            {
                dataValues.ToList().ForEach((value) => value.Dispose());
                dataValues.Clear();
                dataValues = null;
            }
        }
        #endregion
    }
}
