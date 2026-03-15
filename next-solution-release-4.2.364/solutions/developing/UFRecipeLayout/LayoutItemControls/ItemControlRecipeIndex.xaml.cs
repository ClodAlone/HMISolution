using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UFRecipeLayout.Automations;
using Utilities;
using Utilities.WPF;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Threading;

namespace UFRecipeLayout.LayoutItemControls
{
    /// <summary>
    /// Interaction logic for ItemControlRecipeIndex.xaml
    /// </summary>
    public partial class ItemControlRecipeIndex : UserControl, IRecipeIndexUI, IPadSupport, IDataErrorInfo, IDisposable
    {
        #region Dependency Properties
        #region RunningOnServer
        public static readonly DependencyProperty RunningOnServerProperty = DependencyProperty.Register("RunningOnServer", typeof(Boolean), typeof(ItemControlRecipeIndex), new UIPropertyMetadata(true, new PropertyChangedCallback(OnRunningOnServerChanged), new CoerceValueCallback(OnCoerceRunningOnServer)));

        private static object OnCoerceRunningOnServer(DependencyObject o, object value)
        {
            ItemControlRecipeIndex control = o as ItemControlRecipeIndex;
            if (control != null)
                return control.OnCoerceRunningOnServer((Boolean)value);
            else
                return value;
        }

        private static void OnRunningOnServerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ItemControlRecipeIndex control = o as ItemControlRecipeIndex;
            if (control != null)
                control.OnRunningOnServerChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceRunningOnServer(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRunningOnServerChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            textRecipeIndex.IsEditable = !newValue;
            editRecipeNameWeb.Visibility = newValue ? Visibility.Visible : Visibility.Collapsed;
        }

        [Category("Style")]
        public Boolean RunningOnServer
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(RunningOnServerProperty);
            }
            set
            {
                SetValue(RunningOnServerProperty, value);
            }
        }
        #endregion

        #region ShowUnsavedMarker
        public static readonly DependencyProperty ShowUnsavedMarkerProperty = DependencyProperty.Register("ShowUnsavedMarker", typeof(Boolean), typeof(ItemControlRecipeIndex), new UIPropertyMetadata(true));

        [Category("Style")]
        public Boolean ShowUnsavedMarker
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(ShowUnsavedMarkerProperty);
            }
            set
            {
                SetValue(ShowUnsavedMarkerProperty, value);
            }
        }
        #endregion
        #endregion

        #region Declarations
        bool bBindingValidated;
        DataSet ds;
        String TableName;
        String ColumnName;
        String PrimaryKeyName;
        String TempRecipeName;
        public TextBox textEditBox { get; private set; }

        DispatcherOperation dpUpdateMarker;

        Guid lastRecipeIdSelected;
        #endregion

        public ItemControlRecipeIndex()
        {
            InitializeComponent();

            textRecipeIndex.Loaded += (s, e) => 
            { 
                e.Handled = true;
                var elements = textRecipeIndex.GetVisualChildrenOfType<TextBox>().ToList();
                if (elements.Count > 0)
                    textEditBox = elements[0];

                if (textEditBox != null)
                {
                    textEditBox.MaxLength = maxLenght;
                }
            };
        }

        public void ShowPad()
        {
            var owner = this.FindParent<Window>();
            if (owner == null)
            {
                var ie = Keyboard.FocusedElement as DependencyObject;
                if (ie != null)
                    owner = Window.GetWindow(ie);
            }

            if (owner != null)
            {
                int? maxLenght = null;
                if (MaxLength > 0)
                    maxLenght = MaxLength;

                var ret = Pads.Pads.ShowAlphaNumericPad(textEditBox.Text, owner, max: maxLenght);
                if (ret != null)
                    textEditBox.Text = ret;
            }
        }

        void AbortPendingOperations()
        {
            if (dpUpdateMarker != null &&
                dpUpdateMarker.Status != DispatcherOperationStatus.Aborted &&
                dpUpdateMarker.Status != DispatcherOperationStatus.Completed)
                dpUpdateMarker.Abort();
        }

        void ClearBinding()
        {
            BindingOperations.ClearAllBindings(textRecipeIndex);
        }

        bool InvalidBinding()
        {
            return !bBindingValidated || ds.Tables[TableName].DefaultView.Count == 0;
        }

        protected String PerformValidation(String propertyName)
        {
            if (InvalidBinding())
                return Properties.Resources.LayoutDataValueInvalidBinding;

            return null;
        }

        private void DataTable_ValueChanged(object sender, DataRowChangeEventArgs e)
        {
            AbortPendingOperations();

            dpUpdateMarker = Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                if (ds.HasChanges(DataRowState.Modified))
                    marker.Visibility = Visibility.Visible;
                else
                    marker.Visibility = Visibility.Collapsed;
            });
        }

        private void DataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add ||
                e.Action == DataRowAction.Delete ||
                e.Action == DataRowAction.Rollback)
                LoadRecipes();
        }

        private void DataTable_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            LoadRecipes();
        }

        void LoadRecipes()
        {
            Dispatcher.InvokeIfRequired(() =>
            {
                var recipeDataSet = textRecipeIndex.DataContext as RecipeIndexViewModel;
                if (recipeDataSet == null)
                    return;

                recipeDataSet.LoadRecipes();
            });
        }

        void NotifyRecipeTextChanged()
        {
            if (textRecipeIndex.SelectedValue != null)
            {
                TempRecipeName = null;
                return;
            }

            String currentRecipe = textRecipeIndex.Text.Trim();
            if (!String.IsNullOrEmpty(currentRecipe) && TempRecipeName != textRecipeIndex.Text)
            {
                TempRecipeName = textRecipeIndex.Text;
                OnRecipeTextChanged(this, new RecipeTextChangedEventArgs()
                {
                    RecipeName = TempRecipeName
                });
            }
        }

        void NotifySelectionChanged(Guid newGuid, Guid oldGuid)
        {
            if (newGuid != Guid.Empty && lastRecipeIdSelected != newGuid)
            {
                TempRecipeName = null;
                lastRecipeIdSelected = newGuid;
                var args = new RecipeSelectionEventArgs()
                {
                    RecipeID = newGuid
                };
                OnRecipeSelectionChanged(this, args);
                if (args.Cancel)
                    textRecipeIndex.SelectedValue = oldGuid;
            }
        }

        #region Custom automation peers

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new IndexAutomationPeer(this);
        }

        #endregion

        #region IRecipeIndexUI Members

        public event EventHandler<RecipeTextChangedEventArgs> RecipeTextChanged;
        void OnRecipeTextChanged(object sender, RecipeTextChangedEventArgs e)
        {
            EventHandler<RecipeTextChangedEventArgs> temp = RecipeTextChanged;
            if (temp != null)
                temp(sender, e);
        }

        public event EventHandler<RecipeSelectionEventArgs> RecipeSelectionChanged;
        void OnRecipeSelectionChanged(object sender, RecipeSelectionEventArgs e)
        {
            EventHandler<RecipeSelectionEventArgs> temp = RecipeSelectionChanged;
            if (temp != null)
                temp(sender, e);
        }

        public void SetBinding(String itemsourcepath, String displaypath, String valuepath)
        {
            ds = (DataContext as DataSet);
            TableName = itemsourcepath;
            ColumnName = displaypath;
            PrimaryKeyName = valuepath;

            if (ds != null &&
                !String.IsNullOrEmpty(TableName) &&
                !String.IsNullOrEmpty(ColumnName) &&
                !String.IsNullOrEmpty(PrimaryKeyName))
            {
                bBindingValidated = true;

                ds.Tables[TableName].RowChanged += DataTable_RowChanged;
                ds.Tables[TableName].RowDeleted += DataTable_RowDeleted;

                if (ShowUnsavedMarker)
                {
                    foreach (DataTable table in ds.Tables)
                        table.RowChanged += DataTable_ValueChanged;
                }

                textRecipeIndex.DataContext = new RecipeIndexViewModel(ds.Tables[TableName], PrimaryKeyName, ColumnName);

                var binding = new Binding()
                {
                    Path = new PropertyPath("Recipes"),
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    ValidatesOnDataErrors = true
                };

                textRecipeIndex.DisplayMemberPath = "Name";
                textRecipeIndex.SelectedValuePath = "UniqueId";
                textRecipeIndex.IsSynchronizedWithCurrentItem = true;

                textRecipeIndex.SelectionChanged += (s, e) => 
                {
                    e.Handled = true;
                    Guid newGuid = Guid.Empty;
                    if (e.AddedItems.Count > 0)
                    {
                        var recipeIndex = e.AddedItems[0] as RecipeIndex;
                        if (recipeIndex != null)
                            newGuid = recipeIndex.UniqueId;
                    }

                    Guid oldGuid = Guid.Empty;
                    if (e.RemovedItems.Count > 0)
                    {
                        var recipeIndex = e.RemovedItems[0] as RecipeIndex;
                        if (recipeIndex != null)
                            oldGuid = recipeIndex.UniqueId;
                    }

                    NotifySelectionChanged(newGuid, oldGuid);
                };

                textRecipeIndex.LostFocus += (s, e) =>
                {
                    e.Handled = true;
                    NotifyRecipeTextChanged();
                };

                BindingOperations.SetBinding(textRecipeIndex, ComboBox.ItemsSourceProperty, binding);
            }
        }

        public String TextValue
        {
            get
            {
                return textRecipeIndex.Text;
            }
            set
            {
                if (textRecipeIndex.Text == value)
                    return;

                textRecipeIndex.Text = value;
                NotifyRecipeTextChanged();
            }
        }

        public Guid SelectedRecipeId
        { 
            get
            {
                if (InvalidBinding())
                    return Guid.Empty;

                try
                {
                    return Guid.Parse(ds.Tables[TableName].DefaultView[0].Row[PrimaryKeyName].ToString());
                }
                catch
                {
                    return Guid.Empty;
                }
            }
            set
            {
                if (InvalidBinding())
                    return;

                Guid oldGuid = Guid.Empty;
                if (textRecipeIndex.SelectedValue != null)
                    Guid.TryParse(textRecipeIndex.SelectedValue.ToString(), out oldGuid);
                textRecipeIndex.SelectedValue = value;
                NotifySelectionChanged(value, oldGuid);
            }
        }

        public bool AllowEdit
        {
            get
            {
                return textRecipeIndex.IsEditable;
            }
            set
            {
                if (textRecipeIndex.IsEditable == value)
                    return;

                textRecipeIndex.IsEditable = value;
            }
        }

        int maxLenght;
        public int MaxLength
        {
            get
            {
                if (textEditBox != null)
                    return textEditBox.MaxLength;

                return maxLenght;
            }
            set
            {
                if (textEditBox == null)
                    maxLenght = value;
                else if (textEditBox.MaxLength != value && value > 0)
                {
                    textEditBox.MaxLength = value;
                }
            }
        }

        #endregion

        #region IDataErrorInfo
        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            AbortPendingOperations();

            if (bBindingValidated)
            {
                ds.Tables[TableName].RowChanged -= DataTable_RowChanged;
                ds.Tables[TableName].RowDeleted -= DataTable_RowDeleted;

                foreach (DataTable table in ds.Tables)
                    table.RowChanged -= DataTable_ValueChanged;
            }

            ClearBinding();
        }

        #endregion

    }
}
