using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.LayoutControl;
using Utilities;
using Utilities.WPF;
using UFRecipeSettings.UFRecipeModel;
using UFRecipeExecutionContext;
using UFRecipeLayout.LayoutItemControls;
using UFRecipeSettings.Helpers;
using UFRecipeLayout.Helpers;
using ViewModelLib;
using Opc.Ua;
using UFRecipeSettings.Documents;
using log4net;
using CommonControls;
using System.Threading;
using System.Windows.Controls.Primitives;
using UIMsgBoxAlertService.ComponentService;
using UFUAModel.Extensions;
using System.ComponentModel;
using StringManager.ComponentService;
using System.Globalization;
using DataReader.Extensions;
using System.Text.RegularExpressions;

namespace UFRecipeLayout
{
    /// <summary>
    /// Interaction logic for LayoutRecipeEditor.xaml
    /// </summary>
    public partial class LayoutRecipeEditor : UserControl, IDisposable
    {
        #region Declarations
        readonly UFRecipeDocument RecipeDocument;
        readonly DataSet dataSet;

        readonly List<Task> pendingTask = new List<Task>();
        CancellationTokenSource ctsPendingTask;

        readonly static String[] PropertyXamlWriter = new String[] 
        {
            "AddColonToLabel", 
            "ElementSpace", 
            "IsRequired", 
            "IsTabStop", 
            "IsEnabled", 
            "IsHitTestVisible",
            "IsCollapsible", 
            "Focusable", 
            "Opacity",
            "Visibility",
            "LabelHorizontalAlignment", 
            "LabelVerticalAlignment", 
            "LabelPosition", 
            "Background", 
            "Foreground", 
            "BorderBrush", 
            "BorderThickness", 
            "Margin", 
            "FontFamily", 
            "FontSize", 
            "FontStretch", 
            "FontStyle", 
            "FontWeight", 
            "HorizontalContentAlignment", 
            "VerticalContentAlignment", 
            "FlowDirection", 
            "MinWidth", 
            "MaxWidth", 
            "MinHeight", 
            "MaxHeight", 
            "ToolTip",

            // Custom Dependency Properties
            "ControlType",
            "EnumOptions",
            "ShowIcon",
            "Caption",
            "UnitName",
            "MinValue",
            "MaxValue",
            "DecimalDigits",
            "MaxLength"
        };

        List<IDataValueUI> listDataValueUI;
        IRecipeIndexUI RecipeIndexUI;
        ICommandUI ReloadCommandUI;
        ICommandUI AddCommandUI;
        ICommandUI SaveCommandUI;
        ICommandUI RemoveCommandUI;
        ICommandUI ImportCommandUI;
        ICommandUI ExportCommandUI;
        ICommandUI ReadCommandUI;
        ICommandUI WriteCommandUI;
        ILog logTrace;
        BitmapImage reloadIcon;
        BitmapImage removeIcon;
        BitmapImage saveIcon;
        BitmapImage addIcon;
        BitmapImage importIcon;
        BitmapImage exportIcon;
        BitmapImage readIcon;
        BitmapImage writeIcon;

        BitmapImage readIconOk;
        BitmapImage reloadIconOk;
        BitmapImage addIconOk;
        BitmapImage saveIconOk;
        BitmapImage removeIconOk;
        BitmapImage importIconOk;
        BitmapImage exportIconOk;
        BitmapImage writeIconOk;
        BitmapImage readIconError;
        BitmapImage reloadIconError;
        BitmapImage addIconError;
        BitmapImage saveIconError;
        BitmapImage removeIconError;
        BitmapImage importIconError;
        BitmapImage exportIconError;
        BitmapImage writeIconError;

        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);

        bool isNewRecipeEntering;

        Window wndParent;
        bool IsRuntime;
        bool bLoaded;
        #endregion

        #region Constructors
        public LayoutRecipeEditor()
        {
            InitializeComponent();

            IsEditRecipeEnabled =
                IsInsertRecipeEnabled =
                IsRemoveRecipeEnabled =
                IsImportRecipeEnabled =
                IsExportRecipeEnabled =
                IsReadRecipeEnabled =
                IsWriteRecipeEnabled =
                IsRuntime = false;

            //logPanel.Visibility = System.Windows.Visibility.Visible;

            layoutItems.IsCustomization = false;
            
            CommonConstructor();
        }

        public LayoutRecipeEditor(DataSet dataset, UFRecipeDocument doc, bool runtime = false)
        {
            InitializeComponent();

            DataContext = dataset;
            dataSet = dataset;
            RecipeDocument = doc;

            ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, RecipeDocument);
            
            reloadIcon = TryFindResource("UFRLReload") as BitmapImage;
            removeIcon = TryFindResource("UFRLRemove") as BitmapImage;
            saveIcon = TryFindResource("UFRLSave") as BitmapImage;
            addIcon = TryFindResource("UFRLAdd") as BitmapImage;
            importIcon = TryFindResource("UFRLImport") as BitmapImage;
            exportIcon = TryFindResource("UFRLExport") as BitmapImage;
            readIcon = TryFindResource("UFRLRead") as BitmapImage;
            writeIcon = TryFindResource("UFRLWrite") as BitmapImage;

            reloadIconOk = TryFindResource("UFRLReloadOk") as BitmapImage;
            removeIconOk = TryFindResource("UFRLRemoveOk") as BitmapImage;
            saveIconOk = TryFindResource("UFRLSaveOk") as BitmapImage;
            addIconOk = TryFindResource("UFRLAddOk") as BitmapImage;
            importIconOk = TryFindResource("UFRLImportOk") as BitmapImage;
            exportIconOk = TryFindResource("UFRLExportOk") as BitmapImage;
            readIconOk = TryFindResource("UFRLReadOk") as BitmapImage;
            writeIconOk = TryFindResource("UFRLWriteOk") as BitmapImage;

            reloadIconError = TryFindResource("UFRLReloadError") as BitmapImage;
            removeIconError = TryFindResource("UFRLRemoveError") as BitmapImage;
            saveIconError = TryFindResource("UFRLSaveError") as BitmapImage;
            addIconError = TryFindResource("UFRLAddError") as BitmapImage;
            importIconError = TryFindResource("UFRLImportError") as BitmapImage;
            exportIconError = TryFindResource("UFRLExportError") as BitmapImage;
            readIconError = TryFindResource("UFRLReadError") as BitmapImage;
            writeIconError = TryFindResource("UFRLWriteError") as BitmapImage;

            IsEditRecipeEnabled = 
            IsInsertRecipeEnabled = 
            IsRemoveRecipeEnabled = 
            IsImportRecipeEnabled =
            IsExportRecipeEnabled = 
            IsReadRecipeEnabled = 
            IsWriteRecipeEnabled = 
            IsRuntime = runtime;

            //logPanel.Visibility = System.Windows.Visibility.Visible;

            layoutItems.IsCustomization = false;

            CommonConstructor();
        }

        void CommonConstructor()
        {
            // Create a name scope for the stackpanel.
            NameScope.SetNameScope(layoutItems, new NameScope());

            Loaded += (s, e) =>
            {
                if (bLoaded)
                    return;

                bLoaded = true;

                var wnd = this.FindParent<Window>();
                if (wnd != null)
                {
                    wndParent = wnd;
                    wnd.Closing += wnd_Closing;
                }

                if (IsRuntime)
                {
#if !DEBUG
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            var mode = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxLpZ9HLAzv1ul2RsoVlDyXw=="/* RCP */);
                            if (mode == false)
                            {
                                IsEditRecipeEnabled = false;
                                IsInsertRecipeEnabled = false;
                                IsRemoveRecipeEnabled = false;
                                IsReadRecipeEnabled = false;
                                IsWriteRecipeEnabled = false;

                                logLicense.Warn(Properties.Resources.NoRecipeLicense);
                            }

                            txtMode.Visibility = mode ? Visibility.Collapsed : Visibility.Visible;
                        });
#endif

                    if (StringEditorManager != null)
                    {
                        ChangeLanguage();
                        StringEditorManager.CultureChanged += StringEditor_CultureChanged;
                    }

                    if (dataSet != null)
                    {
                        // preintializing data row filter
                        for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                        {
                            dataSet.Tables[cc].DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                            dataSet.Tables[cc].DefaultView.RowFilter = String.Empty;
                        }

                        using (var viewRecipes = new DataView(dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)]))
                        {
                            viewRecipes.Sort = String.Format("[{0}] ASC", DataSetHelper.ColumnName(RecipeDocument.RecipeEntity));
                            if (viewRecipes.Count > 0)
                            {
                                Guid guid = Guid.Empty;
                                if (Guid.TryParse(viewRecipes[0].Row[viewRecipes.Table.PrimaryKey[0].ColumnName].ToString(), out guid))
                                {
                                    if (RecipeIndexUI != null)
                                        RecipeIndexUI.SelectedRecipeId = guid;

                                    for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                                    {
                                        if (dataSet.Tables[cc].DefaultView.RowStateFilter != DataViewRowState.CurrentRows)
                                            dataSet.Tables[cc].DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                                        dataSet.Tables[cc].DefaultView.RowFilter = String.Format("{0}='{1}'",
                                                                                    dataSet.Tables[cc].PrimaryKey[0].ColumnName,
                                                                                    guid);
                                    }
                                }
                            }
                        }
                    }
                }
            };

            Unloaded += (s, e) =>
            {
                if (!bLoaded)
                    return;

                bLoaded = false;

                if (wndParent != null)
                {
                    wndParent.Closing -= wnd_Closing;
                    wndParent = null;
                }

                if (StringEditorManager != null)
                    StringEditorManager.CultureChanged -= StringEditor_CultureChanged;
            };
        }

        void wnd_Closing(object sender, CancelEventArgs e)
        {
            if (wndParent == null)
                return;

            if (newRecipeWindow.Visibility == System.Windows.Visibility.Visible)
            {
                e.Cancel = true;
            }
            else if (wndParent.DialogResult == true)
            {
                CustomDialogResults dlgResult = CustomDialogResults.No;
                if (dataSet != null && dataSet.HasChanges() &&
                    UIInterface != null)
                {
                    dlgResult = UIInterface.ShowYesNoCancel(Properties.Resources.AskSaveChanges, CustomDialogIcons.Question);
                }

                if (dlgResult == CustomDialogResults.Cancel)
                    e.Cancel = true;
                else if (dlgResult == CustomDialogResults.No)
                    wndParent.DialogResult = false;
            }
        }
        #endregion

        #region Event Handlers
        
        public event EventHandler<RecipeCommandEventArgs> ExecutedCommand;
        void OnExecutedCommand(object sender, RecipeCommandEventArgs e)
        {
            EventHandler<RecipeCommandEventArgs> temp = ExecutedCommand;
            if (temp != null)
                temp(sender, e);
        }

        DependencyPropertyDescriptor FindLayoutDependencyPropertyDescriptorFromName(FrameworkElement fe, string name)
        {
            DependencyPropertyDescriptor dpd = null;
            if (dpd == null && fe is RecipeEditValueLayoutItem)
                dpd = DependencyPropertyDescriptor.FromName(name, typeof(RecipeEditValueLayoutItem), fe.GetType());
            if (dpd == null && fe is RecipeCommandButtonLayoutItem)
                dpd = DependencyPropertyDescriptor.FromName(name, typeof(RecipeCommandButtonLayoutItem), fe.GetType());
            if (dpd == null && fe is RecipeLayoutGroup)
                dpd = DependencyPropertyDescriptor.FromName(name, typeof(RecipeLayoutGroup), fe.GetType());
            if (dpd == null && fe is RecipeLayoutItem)
                dpd = DependencyPropertyDescriptor.FromName(name, typeof(RecipeLayoutItem), fe.GetType());
            if (dpd == null && fe is LayoutItem)
                dpd = DependencyPropertyDescriptor.FromName(name, typeof(LayoutItem), fe.GetType());
            if (dpd == null && fe is LayoutGroup)
                dpd = DependencyPropertyDescriptor.FromName(name, typeof(LayoutGroup), fe.GetType());
            return dpd;
        }

        bool IsAssignable(FrameworkElement fe)
        {
            if (fe is LayoutItem)
                return typeof(LayoutItem).IsAssignableFrom(fe.GetType());
            else if (fe is LayoutGroup)
                return typeof(LayoutGroup).IsAssignableFrom(fe.GetType());

            return false;
        }

        void OnWriteElementToXML(object sender, LayoutControlWriteElementToXMLEventArgs e)
        {
            if (IsAssignable(e.Element))
            {
                foreach (var name in PropertyXamlWriter)
                {
                    DependencyPropertyDescriptor dpd = FindLayoutDependencyPropertyDescriptorFromName(e.Element, name);
                    if (dpd != null && dpd.IsBrowsable && !dpd.IsReadOnly)
                    {
                        if (dpd.PropertyType == typeof(Brush) || dpd.PropertyType == typeof(String[]))
                        {
                            var value = e.Element.GetValue(dpd.DependencyProperty);
                            if (value == null)
                                continue;

                            var sw = System.Xaml.XamlServices.Save(value);
                            e.Xml.WriteAttributeString(dpd.Name, sw);
                            continue;
                        }

                        try
                        {
                            e.Element.WritePropertyToXML(e.Xml, dpd.DependencyProperty, dpd.Name);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("WriteElementToXML : Cannot write property '{0}' (exception : {1})", dpd.Name, ex.Message);
                        }
                    }
                }
            }
        }

        void OnReadElementFromXML(object sender, LayoutControlReadElementFromXMLEventArgs e)
        {
            if (IsAssignable(e.Element))
            {
                foreach (var name in PropertyXamlWriter)
                {
                    DependencyPropertyDescriptor dpd = FindLayoutDependencyPropertyDescriptorFromName(e.Element, name);
                    if (dpd != null && dpd.IsBrowsable && !dpd.IsReadOnly)
                    {
                        if (dpd.PropertyType == typeof(Brush) || dpd.PropertyType == typeof(String[]))
                        {
                            string value = e.Xml[dpd.Name];
                            if (value == null)
                                continue;

                            try
                            {
                                e.Element.SetValue(dpd.DependencyProperty, System.Xaml.XamlServices.Parse(value));
                                continue;
                            }
                            catch { }

                            // The lines bellow are leaved for compatibility reason with older versions.
                            try
                            {
                                e.Element.SetValue(dpd.DependencyProperty, new BrushConverter().ConvertFromString(value));
                                continue;
                            }
                            catch { }
                        }
                        else
                        {
                            try
                            {
                                e.Element.ReadPropertyFromXML(e.Xml, dpd.DependencyProperty, dpd.Name, dpd.PropertyType);
                            }
                            catch (Exception ex)
                            { 
                                Debug.WriteLine("ReadElementFromXML : Cannot read property '{0}' (exception : {1})", dpd.Name, ex.Message);
                            }

                            if (dpd.Name == "ToolTip" 
                                && e.Element.ToolTip is String &&
                                (e.Element.ToolTip as String) == String.Empty)
                            {
                                e.Element.ToolTip = null;
                            }
                        }
                    }
                }
            }

            if (IsRuntime && dataSet != null)
            {
                if (e.Element is LayoutItem)
                {
                    var item = (e.Element as LayoutItem);
                    if (item.Content is IRecipeIndexUI)
                    {
                        RecipeIndexUI = (item.Content as IRecipeIndexUI);
                        RecipeIndexUI.SetBinding(DataSetHelper.TableName(RecipeDocument.RecipeEntity),
                                                                    DataSetHelper.ColumnName(RecipeDocument.RecipeEntity),
                                                                    DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity));
                        RecipeIndexUI.AllowEdit = IsInsertRecipeEnabled;
                        RecipeIndexUI.MaxLength = RecipeDocument.RecipeEntity.MaxLength;

                        RecipeIndexUI.RecipeTextChanged += RecipeIndexUI_RecipeTextChanged;
                        RecipeIndexUI.RecipeSelectionChanged += RecipeIndexUI_RecipeSelectionChanged;
                    }
                    else if (item.Content is IDataValueUI)
                    {
                        var itemUI = (item.Content as IDataValueUI);
                        if (listDataValueUI == null)
                            listDataValueUI = new List<IDataValueUI>();
                        listDataValueUI.Add(itemUI);
                        var value = UFRecipeLayout.Helpers.DataBindingHelper.FindDataValue(RecipeDocument.RecipeEntity, e.Element.Name);
                        if (value != null)
                        {
                            var path = UFRecipeLayout.Helpers.DataBindingHelper.GetDataValuePath(RecipeDocument.RecipeEntity, e.Element.Name);
                            if (!String.IsNullOrEmpty(path))
                                itemUI.SetBinding(path, value.DataType);
                        }
                        else
                        {
                            var path = UFRecipeLayout.Helpers.DataBindingHelper.GetDataValuePath(RecipeDocument.RecipeEntity, e.Element.Name);
                            if (!String.IsNullOrEmpty(path))
                                itemUI.SetBinding(path, UFUAModel.DataType.String);
                        }

                        itemUI.AllowEdit = IsEditRecipeEnabled;
                    }
                    else if (item.Content is ICommandUI)
                    {
                        var itemUI = (item.Content as ICommandUI);
                        if (e.Element.Name == Utilities.WPF.DependencyObjectExtensions.AdaptName(LayoutControlPersonalityGuids.NewCommand.ToString()))
                        {
                            AddCommandUI = itemUI;
                            itemUI.SetIcon(addIcon);
                            var relayCmd = new RelayCommand(
                                param =>
                                {
                                    ResetCommandState();

                                    SetBusy(true);

                                    if (ctsPendingTask == null)
                                        ctsPendingTask = new CancellationTokenSource();
                                    CancellationToken token = ctsPendingTask.Token;

                                    String[] recipenames = null;
                                    var task = Task.Factory.StartNew(() =>
                                    {
                                        // new recipe
                                        DataView viewRecipes = new DataView(dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)]);
                                        if (viewRecipes.Count > 0)
                                        {
                                            int count = viewRecipes.Count;
                                            recipenames = new String[count];
                                            for (int ii = 0; ii < viewRecipes.Count; ii++)
                                            {
                                                token.ThrowIfCancellationRequested();

                                                recipenames[ii] = viewRecipes[ii].Row[DataSetHelper.ColumnName(RecipeDocument.RecipeEntity)].ToString().Trim();
                                            }
                                        }
                                    }, token);

                                    task.ContinueWith(ret =>
                                    {
                                        pendingTask.Remove(task);
                                        if (pendingTask.Count == 0)
                                            SetBusy(false);

                                        if (token.IsCancellationRequested)
                                            return;
                                        else if (pendingTask.Count == 0)
                                            System.Windows.Input.CommandManager.InvalidateRequerySuggested();

                                        if (ret.Exception != null)
                                        {
                                            SetAddCmdState(true, ret.Exception.InnerException.Message);
                                            if (UIInterface != null)
                                            {
                                                if (ret.Exception.InnerException is WarningException)
                                                    UIInterface.ShowWarning(ret.Exception.InnerException.Message);
                                                else
                                                    UIInterface.ShowError(ret.Exception.InnerException.Message);
                                            }
                                        }
                                        else
                                        {
                                            newRecipeControl.Content = new UserControlNewRecipeName(recipenames, RecipeDocument.RecipeEntity.MaxLength);
                                            newRecipeWindow.Visibility = System.Windows.Visibility.Visible;
                                            layoutItems.IsEnabled = false;
                                        }
                                    }, TaskScheduler.FromCurrentSynchronizationContext());

                                    pendingTask.Add(task);
                                },
                                param =>
                                {
                                    return IsInsertRecipeEnabled && pendingTask.Count == 0;
                                });

                            itemUI.SetBinding(relayCmd);
                        }
                        else if (e.Element.Name == Utilities.WPF.DependencyObjectExtensions.AdaptName(LayoutControlPersonalityGuids.ReloadCommand.ToString()))
                        {
                            ReloadCommandUI = itemUI;
                            itemUI.SetIcon(reloadIcon);
                            var relayCmd = new RelayCommand(
                                param =>
                                {
                                    ResetCommandState();

                                    // reload recipe
                                    if (RecipeIndexUI != null)
                                    {
                                        var task = CheckAndAskSavePendingChanges();

                                        SetBusy(true);

                                        if (ctsPendingTask == null)
                                            ctsPendingTask = new CancellationTokenSource();
                                        CancellationToken token = ctsPendingTask.Token;

                                        var recipeId = RecipeIndexUI.SelectedRecipeId;
                                        var action = new Action(() =>
                                        {
                                        // notify command execution
                                        var recipeEventArgs = new RecipeCommandEventArgs()
                                            {
                                                RecipeID = recipeId,
                                                CommandType = EditCommandType.Reload,
                                                CancellationToken = token
                                            };
                                            OnExecutedCommand(itemUI, recipeEventArgs);
                                            if (recipeEventArgs.exception != null)
                                                throw recipeEventArgs.exception;
                                        });

                                        if (task != null)
                                        {
                                            task = task.ContinueWith((T) =>
                                            {
                                                action();
                                            }, token);
                                        }
                                        else
                                        {
                                            task = Task.Factory.StartNew(() =>
                                            {
                                                action();
                                            }, token);
                                        }

                                        task.ContinueWith(ret =>
                                        {
                                            pendingTask.Remove(task);
                                            if (pendingTask.Count == 0)
                                                SetBusy(false);

                                            if (token.IsCancellationRequested)
                                                return;
                                            else if (pendingTask.Count == 0)
                                                System.Windows.Input.CommandManager.InvalidateRequerySuggested();

                                            RecipeIndexUI.SelectedRecipeId = recipeId;
                                            if (ret.Exception != null)
                                            {
                                                SetReloadCmdState(true, ret.Exception.InnerException.Message);
                                                if (UIInterface != null)
                                                {
                                                    if (ret.Exception.InnerException is WarningException)
                                                        UIInterface.ShowWarning(ret.Exception.InnerException.Message);
                                                    else
                                                        UIInterface.ShowError(ret.Exception.InnerException.Message);
                                                }
                                            }
                                        }, TaskScheduler.FromCurrentSynchronizationContext());

                                        pendingTask.Add(task);
                                    }
                                },
                                param =>
                                {
                                    return pendingTask.Count == 0;
                                });

                            itemUI.SetBinding(relayCmd);
                        }
                        else if (e.Element.Name == Utilities.WPF.DependencyObjectExtensions.AdaptName(LayoutControlPersonalityGuids.SaveCommand.ToString()))
                        {
                            SaveCommandUI = itemUI;
                            itemUI.SetIcon(saveIcon);
                            var relayCmd = new RelayCommand(
                                param =>
                                {
                                    ResetCommandState();

                                    // save recipe
                                    if (RecipeIndexUI != null)
                                    {
                                        if (!layoutItems.ValidateBindings())
                                        {
                                            SetSaveCmdState(true, Properties.Resources.InvalidBindingWarning);
                                            if (UIInterface != null)
                                                UIInterface.ShowWarning(Properties.Resources.InvalidBindingWarning);
                                            return;
                                        }

                                        SetBusy(true);

                                        if (ctsPendingTask == null)
                                            ctsPendingTask = new CancellationTokenSource();
                                        CancellationToken token = ctsPendingTask.Token;

                                        RecipeCommandEventArgs recipeEventArgs = null;
                                        var task = Task.Factory.StartNew(() =>
                                        {
                                            // notify command execution
                                            recipeEventArgs = new RecipeCommandEventArgs()
                                            {
                                                RecipeID = RecipeIndexUI.SelectedRecipeId,
                                                CommandType = EditCommandType.Save,
                                                CancellationToken = token
                                            };
                                            OnExecutedCommand(itemUI, recipeEventArgs);
                                            if (recipeEventArgs.exception != null)
                                                throw recipeEventArgs.exception;
                                        }, token);

                                        task.ContinueWith(ret =>
                                        {
                                            pendingTask.Remove(task);
                                            if (pendingTask.Count == 0)
                                                SetBusy(false);

                                            if (token.IsCancellationRequested)
                                                return;
                                            else if (pendingTask.Count == 0)
                                                System.Windows.Input.CommandManager.InvalidateRequerySuggested();

                                            if (ret.Exception != null)
                                            {
                                                SetSaveCmdState(true, ret.Exception.InnerException.Message);
                                                if (UIInterface != null)
                                                {
                                                    if (ret.Exception.InnerException is WarningException)
                                                        UIInterface.ShowWarning(ret.Exception.InnerException.Message);
                                                    else
                                                        UIInterface.ShowError(ret.Exception.InnerException.Message);
                                                }
                                            }
                                            else if (RecipeIndexUI != null)
                                                RecipeIndexUI.SelectedRecipeId = recipeEventArgs.RecipeID;
                                        }, TaskScheduler.FromCurrentSynchronizationContext());

                                        pendingTask.Add(task);
                                    }
                                },
                                param =>
                                {
                                    return IsInsertRecipeEnabled && pendingTask.Count == 0;
                                });

                            itemUI.SetBinding(relayCmd);
                        }
                        else if (e.Element.Name == Utilities.WPF.DependencyObjectExtensions.AdaptName(LayoutControlPersonalityGuids.ImportCommand.ToString()))
                        {
                            ImportCommandUI = itemUI;
                            itemUI.SetIcon(importIcon);
                            var relayCmd = new RelayCommand(
                                param =>
                                {
                                    ResetCommandState();

                                    // import recipe
                                    if (RecipeIndexUI != null)
                                    {
                                        string file = String.Empty;
                                        Ookii.Dialogs.Wpf.VistaOpenFileDialog dialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
                                        dialog.CheckFileExists = false;
                                        dialog.Title = Properties.Resources.LayoutImportRecipeCommandTitle;
                                        dialog.ValidateNames = true;
                                        dialog.Filter = Properties.Resources.CSVFilter;
                                        if (dialog.ShowDialog() != true)
                                            return;

                                        file = dialog.FileName;
                                        if (String.IsNullOrEmpty(file) || !System.IO.File.Exists(file))
                                        {
                                            if (UIInterface != null)
                                                UIInterface.ShowWarning(Properties.Resources.ImportFileWarning);
                                            return;
                                        }

                                        SetBusy(true);

                                        if (ctsPendingTask == null)
                                            ctsPendingTask = new CancellationTokenSource();
                                        CancellationToken token = ctsPendingTask.Token;

                                        var task = Task.Factory.StartNew(() =>
                                        {
                                            using (System.IO.StreamReader readFile = new System.IO.StreamReader(file))
                                            {
                                                var recipeEntity = RecipeDocument.RecipeEntity;
                                                var recipeId = dataSet.Tables[DataSetHelper.TableName(recipeEntity)].DefaultView[0].Row[DataSetHelper.PrimaryKeyName(recipeEntity)].ToString();
                                                var recipePrimaryKeyName = DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity);
                                                var recipeName = RecipeDocument.RecipeEntity.Name;

                                                readFile.ReadLine(); //writeFile.WriteLine(Properties.Resources.ExportWhiteSpace);
                                                var line = readFile.ReadLine(); //writeFile.WriteLine(string.Format(Properties.Resources.ExportItemSelected, recipeName));
                                                if (!line.Contains(string.Format("{{{0}}}", recipeName)))
                                                    throw new WarningException(Properties.Resources.BadImportRecipeName);
                                                readFile.ReadLine(); //writeFile.WriteLine(Properties.Resources.ExportWhiteSpace);

                                                StringBuilder retColumns = new StringBuilder();
                                                StringBuilder retValues = new StringBuilder();

                                                var guid = Guid.Empty;
                                                if (Guid.TryParse(recipeId, out guid))
                                                {
                                                    for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                                                    {
                                                        token.ThrowIfCancellationRequested();

                                                        DataView dataView = new DataView(dataSet.Tables[cc]);

                                                        String rowFilter = null;
                                                        rowFilter = String.Format("[{0}]='{1}'", dataView.Table.PrimaryKey[0].ColumnName, guid);

                                                        dataView.RowFilter = rowFilter;
                                                        dataView.RowStateFilter = DataViewRowState.CurrentRows;

                                                        readFile.ReadLine(); //writeFile.WriteLine();
                                                        line = readFile.ReadLine(); //writeFile.WriteLine(string.Format(Properties.Resources.ExportGroup, dataView.Table.TableName));
                                                        if (!line.Contains(string.Format("{{{0}}}", dataView.Table.TableName)))
                                                        {
                                                            throw new WarningException(Properties.Resources.BadImportRecipeFormat);
                                                        }
                                                        readFile.ReadLine(); //writeFile.WriteLine();


                                                        retColumns.Append(readFile.ReadLine()); //writeFile.WriteLine(retColumns.ToString());
                                                        retValues.Append(readFile.ReadLine()); //writeFile.WriteLine(retValues.ToString());

                                                        var columnlist = retColumns.ToString().Split(';');
                                                        var valuelist = retValues.ToString().Split(';');
                                                        foreach (DataRowView rowView in dataView)
                                                        {
                                                            token.ThrowIfCancellationRequested();

                                                            foreach (DataColumn column in rowView.Row.Table.Columns)
                                                            {
                                                                token.ThrowIfCancellationRequested();
                                                                 
                                                                if (column.DataType != typeof(System.DateTime) && column.ColumnName != recipePrimaryKeyName && column.ColumnName != recipeName)
                                                                {
                                                                    if (columnlist.Contains(column.ColumnName))
                                                                    {
                                                                        try
                                                                        {
                                                                            string v = valuelist.ElementAt(Array.IndexOf(columnlist, column.ColumnName));
                                                                            rowView[column.ColumnName] = TypeExtensions.ChangeType(v, column.DataType, force: true);
                                                                        }
                                                                        catch
                                                                        { }
                                                                    }
                                                                }
                                                            }

                                                        }

                                                        retColumns.Clear();
                                                        retValues.Clear();
                                                    }

                                                    for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                                                    {
                                                        token.ThrowIfCancellationRequested();

                                                        if (dataSet.Tables[cc].DefaultView.RowStateFilter != DataViewRowState.CurrentRows)
                                                            dataSet.Tables[cc].DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                                                        dataSet.Tables[cc].DefaultView.RowFilter = String.Empty;
                                                        dataSet.Tables[cc].DefaultView.RowFilter = String.Format("[{0}]='{1}'",
                                                                                                    dataSet.Tables[cc].PrimaryKey[0].ColumnName,
                                                                                                    guid);
                                                    }

                                                    var recipeEventArgs = new RecipeCommandEventArgs()
                                                    {
                                                        RecipeID = guid,
                                                        CommandType = EditCommandType.Import,
                                                        CancellationToken = token
                                                    };
                                                    OnExecutedCommand(itemUI, recipeEventArgs);
                                                    if (recipeEventArgs.exception != null)
                                                        throw recipeEventArgs.exception;
                                                }
                                                else
                                                {
                                                    throw new WarningException(Properties.Resources.ImportActionWarning);
                                                }
                                            }
                                        }, token);

                                        task.ContinueWith(ret =>
                                        {
                                            pendingTask.Remove(task);
                                            if (pendingTask.Count == 0)
                                                SetBusy(false);

                                            if (token.IsCancellationRequested)
                                                return;
                                            else if (pendingTask.Count == 0)
                                                System.Windows.Input.CommandManager.InvalidateRequerySuggested();

                                            if (ret.Exception != null)
                                            {
                                                SetImportCmdState(true, ret.Exception.InnerException.Message);
                                                if (UIInterface != null)
                                                {
                                                    if (ret.Exception.InnerException is WarningException)
                                                        UIInterface.ShowWarning(ret.Exception.InnerException.Message);
                                                    else
                                                        UIInterface.ShowError(ret.Exception.InnerException.Message);
                                                }
                                            }

                                        }, TaskScheduler.FromCurrentSynchronizationContext());

                                        pendingTask.Add(task);
                                    } 
                                },
                                param =>
                                {
                                    return IsImportRecipeEnabled && pendingTask.Count == 0;
                                });

                            itemUI.SetBinding(relayCmd);
                        }
                        else if (e.Element.Name == Utilities.WPF.DependencyObjectExtensions.AdaptName(LayoutControlPersonalityGuids.ExportCommand.ToString()))
                        {
                            ExportCommandUI = itemUI;
                            itemUI.SetIcon(exportIcon);
                            var relayCmd = new RelayCommand(
                                param =>
                                {
                                    ResetCommandState();

                                    // export recipe
                                    if (RecipeIndexUI != null)
                                    {
                                        if (!layoutItems.ValidateBindings())
                                        {
                                            SetExportCmdState(true, Properties.Resources.InvalidBindingWarning);
                                            if (UIInterface != null)
                                                UIInterface.ShowWarning(Properties.Resources.InvalidBindingWarning);
                                            return;
                                        }

                                        string file = String.Empty;
                                        Ookii.Dialogs.Wpf.VistaSaveFileDialog dialog = new Ookii.Dialogs.Wpf.VistaSaveFileDialog();
                                        dialog.Title = Properties.Resources.LayoutImportRecipeCommandTitle;
                                        dialog.OverwritePrompt = false;
                                        dialog.ValidateNames = true;
                                        dialog.AddExtension = true;
                                        dialog.DefaultExt = "csv";
                                        dialog.Filter = Properties.Resources.CSVFilter;
                                        if (dialog.ShowDialog() != true)
                                            return;

                                        file = dialog.FileName;
                                        if (String.IsNullOrEmpty(file))
                                            return;

                                        if (System.IO.File.Exists(file) && UIInterface != null)
                                        {
                                            var ret = UIInterface.ShowYesNoCancel(Properties.Resources.ExportFileWarning, CustomDialogIcons.Question);

                                            if (ret == CustomDialogResults.Cancel)
                                                return;
                                            else if (ret == CustomDialogResults.No)
                                            {
                                                int i = 1;
                                                while (System.IO.File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), string.Format("{0}{1}{2}", System.IO.Path.GetFileNameWithoutExtension(file), i.ToString(), System.IO.Path.GetExtension(file).ToLower()))))
                                                {
                                                    i++;
                                                }
                                                file = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), string.Format("{0}{1}{2}", System.IO.Path.GetFileNameWithoutExtension(file), i.ToString(), System.IO.Path.GetExtension(file).ToLower()));
                                            }
                                        }

                                        SetBusy(true);

                                        if (ctsPendingTask == null)
                                            ctsPendingTask = new CancellationTokenSource();
                                        CancellationToken token = ctsPendingTask.Token;

                                        var task = Task.Factory.StartNew(() =>
                                        {
                                            using (System.IO.StreamWriter writeFile = new System.IO.StreamWriter(file))
                                            {
                                                var recipeEntity = RecipeDocument.RecipeEntity;
                                                var recipeId = dataSet.Tables[DataSetHelper.TableName(recipeEntity)].DefaultView[0].Row[DataSetHelper.PrimaryKeyName(recipeEntity)].ToString();
                                                var recipePrimaryKeyName = DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity);
                                                var recipeName = RecipeDocument.RecipeEntity.Name;

                                                writeFile.WriteLine(Properties.Resources.ExportWhiteSpace);
                                                writeFile.WriteLine(string.Format(Properties.Resources.ExportRecipeHeader, recipeName));
                                                writeFile.WriteLine(Properties.Resources.ExportWhiteSpace);

                                                StringBuilder retColumns = new StringBuilder();
                                                StringBuilder retValues = new StringBuilder();

                                                var guid = Guid.Empty;
                                                if (Guid.TryParse(recipeId, out guid))
                                                {
                                                    for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                                                    {
                                                        token.ThrowIfCancellationRequested();

                                                        DataView dataView = new DataView(dataSet.Tables[cc]);

                                                        String rowFilter = null;
                                                        rowFilter = String.Format("[{0}]='{1}'", dataView.Table.PrimaryKey[0].ColumnName, guid);

                                                        dataView.RowFilter = rowFilter;
                                                        dataView.RowStateFilter = DataViewRowState.CurrentRows;

                                                        writeFile.WriteLine();
                                                        writeFile.WriteLine(string.Format(Properties.Resources.ExportGroupHeader, dataView.Table.TableName));
                                                        writeFile.WriteLine();

                                                        foreach (DataRowView rowView in dataView)
                                                        {
                                                            token.ThrowIfCancellationRequested();

                                                            foreach (DataColumn column in rowView.Row.Table.Columns)
                                                            {
                                                                token.ThrowIfCancellationRequested();

                                                                if (column.DataType != typeof(System.DateTime) && column.ColumnName != recipePrimaryKeyName && column.ColumnName != recipeName)
                                                                {
                                                                    if (retColumns.Length > 0)
                                                                        retColumns.Append(';');
                                                                    retColumns.Append(column.ColumnName);
                                                                    string v = Convert.ToString(rowView[column.ColumnName], CultureInfo.InvariantCulture);
                                                                    if (retValues.Length > 0)
                                                                        retValues.Append(';');
                                                                    retValues.Append(v);
                                                                }
                                                            }

                                                        }

                                                        writeFile.WriteLine(retColumns.ToString());
                                                        writeFile.WriteLine(retValues.ToString());

                                                        retColumns.Clear();
                                                        retValues.Clear();
                                                    }

                                                    writeFile.WriteLine();
                                                    writeFile.WriteLine(Properties.Resources.ExportWhiteSpace);

                                                    var recipeEventArgs = new RecipeCommandEventArgs()
                                                    {
                                                        RecipeID = guid,
                                                        CommandType = EditCommandType.Export,
                                                        CancellationToken = token
                                                    };
                                                    OnExecutedCommand(itemUI, recipeEventArgs);
                                                    if (recipeEventArgs.exception != null)
                                                        throw recipeEventArgs.exception;
                                                }
                                            }
                                        }, token);

                                        task.ContinueWith(ret =>
                                        {
                                            pendingTask.Remove(task);
                                            if (pendingTask.Count == 0)
                                                SetBusy(false);

                                            if (token.IsCancellationRequested)
                                                return;
                                            else if (pendingTask.Count == 0)
                                                System.Windows.Input.CommandManager.InvalidateRequerySuggested();

                                            if (ret.Exception != null)
                                            {
                                                SetExportCmdState(true, ret.Exception.InnerException.Message);
                                                if (UIInterface != null)
                                                {
                                                    if (ret.Exception.InnerException is WarningException)
                                                        UIInterface.ShowWarning(ret.Exception.InnerException.Message);
                                                    else
                                                        UIInterface.ShowError(ret.Exception.InnerException.Message);
                                                }
                                            }
                                        }, TaskScheduler.FromCurrentSynchronizationContext());

                                        pendingTask.Add(task);
                                    } 
                                },
                                param =>
                                {
                                    return IsExportRecipeEnabled && pendingTask.Count == 0;
                                });

                            itemUI.SetBinding(relayCmd);
                        }
                        else if (e.Element.Name == Utilities.WPF.DependencyObjectExtensions.AdaptName(LayoutControlPersonalityGuids.RemoveCommand.ToString()))
                        {
                            RemoveCommandUI = itemUI;
                            itemUI.SetIcon(removeIcon);
                            var relayCmd = new RelayCommand(
                                param =>
                                {
                                    ResetCommandState();

                                    // remove recipe
                                    if (dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)].DefaultView.Count == 0)
                                        return;

                                    if (UIInterface == null ||
                                        UIInterface.ShowYesNo(Properties.Resources.AskDeleteRecipe,
                                        CustomDialogIcons.Question) == CustomDialogResults.Yes)
                                    {
                                        SetBusy(true);

                                        if (ctsPendingTask == null)
                                            ctsPendingTask = new CancellationTokenSource();
                                        CancellationToken token = ctsPendingTask.Token;

                                        var guid = Guid.Empty;
                                        var task = Task.Factory.StartNew(() =>
                                        {
                                            // notify command execution
                                            var recipeId = dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)].DefaultView[0].Row[DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity)].ToString();
                                            if (Guid.TryParse(recipeId, out guid))
                                            {
                                                // delete row in the main table and in all parent tables
                                                dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)].DefaultView[0].Row.Delete();

                                                var recipeEventArgs = new RecipeCommandEventArgs()
                                                {
                                                    RecipeID = guid,
                                                    CommandType = EditCommandType.Delete,
                                                    CancellationToken = token
                                                };
                                                OnExecutedCommand(itemUI, recipeEventArgs);
                                                if (recipeEventArgs.exception != null)
                                                    throw recipeEventArgs.exception;
                                                else
                                                {
                                                    using (var viewRecipes = new DataView(dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)]))
                                                    {
                                                        viewRecipes.Sort = String.Format("[{0}] ASC", DataSetHelper.ColumnName(RecipeDocument.RecipeEntity));
                                                        if (viewRecipes.Count > 0)
                                                        {
                                                            if (Guid.TryParse(viewRecipes[0].Row[viewRecipes.Table.PrimaryKey[0].ColumnName].ToString(), out guid))
                                                            {
                                                                for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                                                                {
                                                                    token.ThrowIfCancellationRequested();

                                                                    if (dataSet.Tables[cc].DefaultView.RowStateFilter != DataViewRowState.CurrentRows)
                                                                        dataSet.Tables[cc].DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                                                                    dataSet.Tables[cc].DefaultView.RowFilter = String.Format("{0}='{1}'",
                                                                                                                dataSet.Tables[cc].PrimaryKey[0].ColumnName,
                                                                                                                guid);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }, token);

                                        task.ContinueWith(ret =>
                                        {
                                            pendingTask.Remove(task);
                                            if (pendingTask.Count == 0)
                                                SetBusy(false);

                                            if (token.IsCancellationRequested)
                                                return;
                                            else if (pendingTask.Count == 0)
                                                System.Windows.Input.CommandManager.InvalidateRequerySuggested();

                                            if (ret.Exception != null)
                                            {
                                                SetRemoveCmdState(true, ret.Exception.InnerException.Message);
                                                if (UIInterface != null)
                                                {
                                                    if (ret.Exception.InnerException is WarningException)
                                                        UIInterface.ShowWarning(ret.Exception.InnerException.Message);
                                                    else
                                                        UIInterface.ShowError(ret.Exception.InnerException.Message);
                                                }
                                            }
                                            else if (RecipeIndexUI != null)
                                                    RecipeIndexUI.SelectedRecipeId = guid;
                                        }, TaskScheduler.FromCurrentSynchronizationContext());

                                        pendingTask.Add(task);
                                    }
                                },
                                param =>
                                {
                                    return IsRemoveRecipeEnabled && pendingTask.Count == 0;
                                });

                            itemUI.SetBinding(relayCmd);
                        }
                        else if (e.Element.Name == Utilities.WPF.DependencyObjectExtensions.AdaptName(LayoutControlPersonalityGuids.WriteCommand.ToString()))
                        {
                            WriteCommandUI = itemUI;
                            itemUI.SetIcon(writeIcon);
                            if (WriteEntityHelper != null)
                                WriteEntityHelper.Control = e.Element;
                            var relayCmd = new RelayCommand(
                                param =>
                                {
                                    ResetCommandState();

                                    // write recipe
                                    if (dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)].DefaultView.Count == 0)
                                        return;

                                    if (!layoutItems.ValidateBindings())
                                    {
                                        SetWriteCmdState(true, Properties.Resources.InvalidBindingWarning);
                                        if (UIInterface != null)
                                            UIInterface.ShowWarning(Properties.Resources.InvalidBindingWarning);
                                        return;
                                    }

                                    SetBusy(true);

                                    if (ctsPendingTask == null)
                                        ctsPendingTask = new CancellationTokenSource();
                                    CancellationToken token = ctsPendingTask.Token;

                                    var task = Task.Factory.StartNew(() =>
                                    {
                                        // notify command execution
                                        var guid = Guid.Empty;
                                        ExecutionResult result = new ExecutionResult() { ResultState = true };
                                        var recipeId = dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)].DefaultView[0].Row[DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity)].ToString();
                                        if (Guid.TryParse(recipeId, out guid))
                                        {
                                            if (WriteEntityHelper != null)
                                            {
                                                VariantCollection values = new VariantCollection();
                                                if (RecipeDocument.RecipeEntity.IsWritable())
                                                {
                                                    // first write the datavalues without group with starting address
                                                    var writabledatavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                                                              where c.IsWritable() && String.IsNullOrEmpty(c.StartingAddress)
                                                                              select c).ToList();

                                                    if (writabledatavalues.Count > 0)
                                                    {
                                                        token.ThrowIfCancellationRequested();

                                                        values = DataSetHelper.GetDBValues(writabledatavalues, dataSet, guid);
                                                        result = WriteEntityHelper.Execute(RecipeDocument.RecipeEntity.StartingAddress, values.ToArray());
                                                    }
                                                }

                                                if (result.ResultState)
                                                {
                                                    // next write the datavalues inside group with starting address
                                                    var validgroups = (from c in RecipeDocument.RecipeEntity.Groups
                                                                       where c.IsWritable()
                                                                       select c).ToList();

                                                    foreach (var recipegroup in validgroups)
                                                    {
                                                        token.ThrowIfCancellationRequested();

                                                        var writabledatavalues = (from c in recipegroup.DataValues
                                                                                  where c.UseInCommunication && String.IsNullOrEmpty(c.StartingAddress)
                                                                                  orderby c.OID ascending
                                                                                  select c).ToList();

                                                        if (writabledatavalues.Count > 0)
                                                        {
                                                            values = DataSetHelper.GetDBValues(writabledatavalues, dataSet, guid);
                                                            result = WriteEntityHelper.Execute(recipegroup.StartingAddress, values.ToArray());
                                                        }

                                                        if (!result.ResultState)
                                                            break;
                                                    }
                                                }

                                                if (result.ResultState)
                                                {
                                                    // end write the datavalues with starting address
                                                    var writabledatavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                                                              where c.IsWritable() && !String.IsNullOrEmpty(c.StartingAddress)
                                                                              select c).ToList();

                                                    List<UFDataValueEntity> list = new List<UFDataValueEntity>();
                                                    foreach (var datavalue in writabledatavalues)
                                                    {
                                                        token.ThrowIfCancellationRequested();

                                                        list.Add(datavalue);
                                                        values = DataSetHelper.GetDBValues(list, dataSet, guid);
                                                        result = WriteEntityHelper.Execute(datavalue.StartingAddress, values.ToArray());

                                                        if (!result.ResultState)
                                                            break;

                                                        list.Clear();
                                                    }
                                                }

                                                if (result.ResultState)
                                                {
                                                    var writabledatavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                                                              where c.IsTagIOReferenceValid()
                                                                              select c).ToList();
                                                    if (writabledatavalues.Count > 0)
                                                    {

                                                        token.ThrowIfCancellationRequested();

                                                        values = DataSetHelper.GetDBValues(writabledatavalues, dataSet, guid, false);
                                                        result = WriteEntityHelper.Execute(values.ToArray());
                                                    }
                                                }
                                            }
                                        }

                                        if (result.ExceptionInfo != null)
                                        {
                                            throw result.ExceptionInfo;
                                        }
                                        else if (result.ResultState)
                                        {
                                            if (guid != Guid.Empty)
                                            {
                                                var recipeEventArgs = new RecipeCommandEventArgs()
                                                {
                                                    RecipeID = guid,
                                                    CommandType = EditCommandType.Write,
                                                    CancellationToken = token
                                                };
                                                OnExecutedCommand(itemUI, recipeEventArgs);
                                                if (recipeEventArgs.exception != null)
                                                    throw recipeEventArgs.exception;
                                            }
                                        }
                                        else
                                        {
                                            throw new WarningException(Properties.Resources.WriteCommandExecutionResultWarning);
                                        }
                                    }, token);

                                    task.ContinueWith(ret =>
                                    {
                                        pendingTask.Remove(task);
                                        if (pendingTask.Count == 0)
                                            SetBusy(false);

                                        if (token.IsCancellationRequested)
                                            return;
                                        else if (pendingTask.Count == 0)
                                            System.Windows.Input.CommandManager.InvalidateRequerySuggested();

                                        if (ret.Exception != null)
                                        {
                                            string error = String.Format(Properties.Resources.WriteCommandExecutionResultError, ret.Exception.InnerException.Message);
                                            if (logTrace == null)
                                                logTrace = LogManager.GetLogger(RecipeDocument.Title);
                                            logTrace.Error(error);
                                            SetWriteCmdState(true, error);
                                            if (UIInterface != null)
                                            {
                                                if (ret.Exception.InnerException is WarningException)
                                                    UIInterface.ShowWarning(ret.Exception.InnerException.Message);
                                                else
                                                    UIInterface.ShowError(ret.Exception.InnerException.Message);
                                            }
                                        }
                                        else
                                        {
                                            SetWriteCmdState(false, Properties.Resources.WriteCommandExecutionResultOK);
                                            if (logTrace == null)
                                                logTrace = LogManager.GetLogger(RecipeDocument.Title);
                                            logTrace.Info(Properties.Resources.WriteCommandExecutionResultOK);
                                        }
                                    }, TaskScheduler.FromCurrentSynchronizationContext());

                                    pendingTask.Add(task);
                                    
                                },
                                param =>
                                {
                                    return IsWriteRecipeEnabled && pendingTask.Count == 0 && WriteEntityHelper != null && WriteEntityHelper.CanExecute();
                                });

                            itemUI.SetBinding(relayCmd);
                        }
                        else if (e.Element.Name == Utilities.WPF.DependencyObjectExtensions.AdaptName(LayoutControlPersonalityGuids.ReadCommand.ToString()))
                        {
                            ReadCommandUI = itemUI;
                            itemUI.SetIcon(readIcon);
                            if (ReadEntityHelper != null)
                                ReadEntityHelper.Control = e.Element;
                            var relayCmd = new RelayCommand(
                                param =>
                                {
                                    ResetCommandState();

                                    // read recipe
                                    if (dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)].DefaultView.Count == 0)
                                        return;

                                    SetBusy(true);

                                    if (ctsPendingTask == null)
                                        ctsPendingTask = new CancellationTokenSource();
                                    CancellationToken token = ctsPendingTask.Token;

                                    var task = Task.Factory.StartNew(() =>
                                    {
                                        // notify command execution
                                        var guid = Guid.Empty;
                                        ExecutionResult result = new ExecutionResult() { ResultState = true };
                                        var recipeId = dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)].DefaultView[0].Row[DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity)].ToString();
                                        if (Guid.TryParse(recipeId, out guid))
                                        {
                                            if (ReadEntityHelper != null)
                                            {
                                                VariantCollection values = new VariantCollection();
                                                if (RecipeDocument.RecipeEntity.IsReadable())
                                                {
                                                    // first read the datavalues without group with starting address
                                                    var writabledatavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                                                              where c.IsReadable() && String.IsNullOrEmpty(c.StartingAddress)
                                                                              select c).ToList();

                                                    if (writabledatavalues.Count > 0)
                                                    {
                                                        token.ThrowIfCancellationRequested();

                                                        values = DataSetHelper.GetDBValues(writabledatavalues, dataSet, guid);
                                                        result = ReadEntityHelper.Execute(RecipeDocument.RecipeEntity.StartingAddress, values.ToArray());
                                                        if (result.ResultState)
                                                            DataSetHelper.SetDBValues(writabledatavalues, dataSet, result.OutputValues);
                                                    }
                                                }

                                                if (result.ResultState)
                                                {
                                                    // next read the datavalues inside group with starting address
                                                    var validgroups = (from c in RecipeDocument.RecipeEntity.Groups
                                                                       where c.IsReadable()
                                                                       select c).ToList();

                                                    foreach (var recipegroup in validgroups)
                                                    {
                                                        token.ThrowIfCancellationRequested();

                                                        var writabledatavalues = (from c in recipegroup.DataValues
                                                                                  where c.UseInCommunication && String.IsNullOrEmpty(c.StartingAddress)
                                                                                  orderby c.OID ascending
                                                                                  select c).ToList();

                                                        if (writabledatavalues.Count > 0)
                                                        {
                                                            values = DataSetHelper.GetDBValues(writabledatavalues, dataSet, guid);
                                                            result = ReadEntityHelper.Execute(recipegroup.StartingAddress, values.ToArray());
                                                        }

                                                        if (!result.ResultState)
                                                            break;

                                                        DataSetHelper.SetDBValues(writabledatavalues, dataSet, result.OutputValues);
                                                    }
                                                }

                                                if (result.ResultState)
                                                {
                                                    // end read the datavalues with starting address
                                                    var writabledatavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                                                              where c.IsReadable() && !String.IsNullOrEmpty(c.StartingAddress)
                                                                              select c).ToList();

                                                    List<UFDataValueEntity> list = new List<UFDataValueEntity>();
                                                    foreach (var datavalue in writabledatavalues)
                                                    {
                                                        token.ThrowIfCancellationRequested();

                                                        list.Add(datavalue);
                                                        values = DataSetHelper.GetDBValues(list, dataSet, guid);
                                                        result = ReadEntityHelper.Execute(datavalue.StartingAddress, values.ToArray());

                                                        if (!result.ResultState)
                                                            break;

                                                        DataSetHelper.SetDBValues(list, dataSet, result.OutputValues);
                                                        list.Clear();
                                                    }
                                                }

                                                if (result.ResultState)
                                                {
                                                    var readabledatavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                                                              where c.IsTagIOReferenceValid()
                                                                              select c).ToList();
                                                    if (readabledatavalues.Count > 0)
                                                    {
                                                        token.ThrowIfCancellationRequested();

                                                        values = DataSetHelper.GetDBValues(readabledatavalues, dataSet, guid, false);
                                                        result = ReadEntityHelper.Execute(values.ToArray());
                                                        if (result.ResultState)
                                                            DataSetHelper.SetDBValues(readabledatavalues, dataSet, result.OutputValues, false);
                                                    }
                                                }
                                            }
                                        }

                                        if (result.ExceptionInfo != null)
                                        {
                                            throw result.ExceptionInfo;
                                        }
                                        else if (result.ResultState)
                                        {
                                            if (guid != Guid.Empty)
                                            {
                                                for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                                                {
                                                    token.ThrowIfCancellationRequested();

                                                    if (dataSet.Tables[cc].DefaultView.RowStateFilter != DataViewRowState.CurrentRows)
                                                        dataSet.Tables[cc].DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                                                    dataSet.Tables[cc].DefaultView.RowFilter = String.Empty;
                                                    dataSet.Tables[cc].DefaultView.RowFilter = String.Format("[{0}]='{1}'",
                                                                                                dataSet.Tables[cc].PrimaryKey[0].ColumnName,
                                                                                                guid);
                                                }

                                                var recipeEventArgs = new RecipeCommandEventArgs()
                                                {
                                                    RecipeID = guid,
                                                    CommandType = EditCommandType.Read,
                                                    CancellationToken = token
                                                };
                                                OnExecutedCommand(itemUI, recipeEventArgs);
                                                if (recipeEventArgs.exception != null)
                                                    throw recipeEventArgs.exception;
                                            }
                                        }
                                        else
                                        {
                                            throw new WarningException(Properties.Resources.ReadCommandExecutionResultWarning);
                                        }
                                    }, token);

                                    task.ContinueWith(ret =>
                                    {
                                        pendingTask.Remove(task);
                                        if (pendingTask.Count == 0)
                                            SetBusy(false);

                                        if (token.IsCancellationRequested)
                                            return;
                                        else if (pendingTask.Count == 0)
                                            System.Windows.Input.CommandManager.InvalidateRequerySuggested();

                                        if (ret.Exception != null)
                                        {
                                            string error = String.Format(Properties.Resources.ReadCommandExecutionResultError, ret.Exception.InnerException.Message);
                                            if (logTrace == null)
                                                logTrace = LogManager.GetLogger(RecipeDocument.Title);
                                            logTrace.Error(error);
                                            SetReadCmdState(true, error);
                                            if (UIInterface != null)
                                            {
                                                if (ret.Exception.InnerException is WarningException)
                                                    UIInterface.ShowWarning(ret.Exception.InnerException.Message);
                                                else
                                                    UIInterface.ShowError(ret.Exception.InnerException.Message);
                                            }
                                        }
                                        else
                                        {
                                            SetReadCmdState(false, Properties.Resources.WriteCommandExecutionResultOK);
                                            if (logTrace == null)
                                                logTrace = LogManager.GetLogger(RecipeDocument.Title);
                                            logTrace.Info(Properties.Resources.ReadCommandExecutionResultOK);
                                        }
                                    }, TaskScheduler.FromCurrentSynchronizationContext());

                                    pendingTask.Add(task);
                                },
                                param =>
                                {
                                    return IsReadRecipeEnabled && pendingTask.Count == 0 && ReadEntityHelper != null && ReadEntityHelper.CanExecute();
                                });

                            itemUI.SetBinding(relayCmd);
                        }
                    }
                }
            }
        }

        private void newRecipe_Ok(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            CloseNewRecipeForm(true);
        }

        private void newRecipe_Cancel(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            CloseNewRecipeForm(false);
        }

        private void LayoutRecipeEditor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (newRecipeWindow.Visibility == System.Windows.Visibility.Visible)
            {
                if (e.Key == Key.Enter)
                {
                    e.Handled = true;
                    CloseNewRecipeForm(true);
                }
                else if (e.Key == Key.Escape)
                {
                    e.Handled = true;
                    CloseNewRecipeForm(false);
                }
            }
        }

        private void RecipeIndexUI_RecipeTextChanged(object sender, RecipeTextChangedEventArgs e)
        {
            ResetCommandState();

            Guid newguid = Guid.NewGuid();
            var recipes = new Dictionary<String, String>();

            using (var viewRecipes = new DataView(dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)]))
            {
                if (viewRecipes.Count > 0)
                {
                    int count = viewRecipes.Count;
                    for (int ii = 0; ii < viewRecipes.Count; ii++)
                    {
                        var name = viewRecipes[ii].Row[DataSetHelper.ColumnName(RecipeDocument.RecipeEntity)].ToString().Trim();
                        recipes[name] = viewRecipes[ii].Row[DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity)].ToString();
                    }
                }
            }

            if (!String.IsNullOrEmpty(e.RecipeName) && recipes.ContainsKey(e.RecipeName) && Guid.TryParse(recipes[e.RecipeName], out newguid))
                RecipeIndexUI.SelectedRecipeId = newguid;

            if (String.IsNullOrEmpty(e.RecipeName) || recipes.ContainsKey(e.RecipeName))
                return;

            AddNewRecipe(e.RecipeName, copyValues: true);
        }

        private void RecipeIndexUI_RecipeSelectionChanged(object sender, RecipeSelectionEventArgs e)
        {
            ResetCommandState();

            Task task = null;
            if (!isNewRecipeEntering)
                task = CheckAndAskSavePendingChanges(e.RecipeID);

            var action = new Action(() =>
            {
                try
                {
                    DataRow parentRow = null;
                    for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                    {
                        if (dataSet.Tables[cc].DefaultView.RowStateFilter != DataViewRowState.CurrentRows)
                            dataSet.Tables[cc].DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                        dataSet.Tables[cc].DefaultView.RowFilter = String.Format("[{0}]='{1}'",
                        dataSet.Tables[cc].PrimaryKey[0].ColumnName,
                        e.RecipeID);

                        // ensure a valid row in the table
                        if (dataSet.Tables[cc].DefaultView.Count == 0)
                        {
                            DataRowView rowView = dataSet.Tables[cc].DefaultView.AddNew();
                            rowView.BeginEdit();
                            if (parentRow != null)
                                rowView.Row.SetParentRow(parentRow);
                            rowView.Row[dataSet.Tables[cc].PrimaryKey[0]] = e.RecipeID;
                            rowView.EndEdit();
                        }
                        else if (dataSet.Tables[cc].TableName == DataSetHelper.TableName(RecipeDocument.RecipeEntity))
                            parentRow = dataSet.Tables[cc].DefaultView[0].Row;
                    }
                }
                catch (Exception ex)
                {
                    string error = String.Format(Properties.Resources.AddNewRecipeFailed, e.RecipeID, ex.Message);
                    if (logTrace == null)
                        logTrace = LogManager.GetLogger(RecipeDocument.Title);
                    logTrace.Error(error);
                    if (UIInterface != null)
                        UIInterface.ShowError(error);
                }

                if (!isNewRecipeEntering && RecipeIndexUI != null)
                    RecipeIndexUI.SelectedRecipeId = e.RecipeID;
            });

            if (task != null)
            {
                task.ContinueWith((T) =>
                {
                    action();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
                action();
        }


        void CloseNewRecipeForm(bool bOK)
        {
            if (bOK)
            {
                var usercontrol = newRecipeControl.Content as UserControlNewRecipeName;
                if (usercontrol == null || !usercontrol.ValidateBindings())
                    return;

                var task = CheckAndAskSavePendingChanges();

                var action = new Action(() =>
                {
                    AddNewRecipe(usercontrol.RecipeName, true);
                });

                if (task != null)
                {
                    task.ContinueWith((T) =>
                    {
                        action();
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                else
                    action();
            }

            newRecipeWindow.Visibility = System.Windows.Visibility.Collapsed;
            newRecipeControl.Content = null;
            layoutItems.IsEnabled = true;
        }

        void AddNewRecipe(String recipeName, bool copyValues = false)
        {
            SetBusy(true);

            isNewRecipeEntering = true;

            if (ctsPendingTask == null)
                ctsPendingTask = new CancellationTokenSource();
            CancellationToken token = ctsPendingTask.Token;

            Guid newguid = Guid.NewGuid();
            var task = Task.Factory.StartNew(() =>
            {
                DataRow parentRow = null;
                for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                {
                    DataView view = new DataView(dataSet.Tables[cc]);
                    DataRowView rowView = view.AddNew();
                    rowView.BeginEdit();
                    if (dataSet.Tables[cc].TableName == DataSetHelper.TableName(RecipeDocument.RecipeEntity))
                    {
                        parentRow = rowView.Row;
                        rowView.Row[dataSet.Tables[cc].Columns[DataSetHelper.ColumnName(RecipeDocument.RecipeEntity)]] = recipeName;
                        rowView.Row[dataSet.Tables[cc].Columns[DataSetHelper.CreationDateTimeColumnName(RecipeDocument.RecipeEntity)]] = DateTime.UtcNow;
                    }
                    else if (parentRow != null)
                        rowView.Row.SetParentRow(parentRow);
                    rowView.Row[dataSet.Tables[cc].PrimaryKey[0]] = newguid;

                    // copy all column values
                    if (copyValues)
                    {
                        for (int ii = 0; ii < rowView.Row.Table.Columns.Count; ii++)
                        {
                            token.ThrowIfCancellationRequested();

                            if (dataSet.Tables[cc].DefaultView.Count == 0 ||
                                rowView.Row.Table.PrimaryKey.Contains(rowView.Row.Table.Columns[ii]) ||
                                (rowView.Row.Table.TableName == DataSetHelper.TableName(RecipeDocument.RecipeEntity) &&
                                rowView.Row.Table.Columns[ii].ColumnName == DataSetHelper.ColumnName(RecipeDocument.RecipeEntity)) ||
                                rowView.Row.Table.Columns[ii].ColumnName == DataSetHelper.CreationDateTimeColumnName(RecipeDocument.RecipeEntity) ||
                                rowView.Row.Table.Columns[ii].ColumnName == DataSetHelper.ActivationDateTimeColumnName(RecipeDocument.RecipeEntity))
                                continue;

                            rowView.Row[ii] = dataSet.Tables[cc].DefaultView[0].Row[ii];
                        }
                    }

                    rowView.EndEdit();
                }

                var recipeEventArgs = new RecipeCommandEventArgs()
                {
                    RecipeID = newguid,
                    CommandType = EditCommandType.New,
                    CancellationToken = token
                };
                OnExecutedCommand(this, recipeEventArgs);
                if (recipeEventArgs.exception != null)
                    throw recipeEventArgs.exception;
            }, token);

            task.ContinueWith(ret =>
            {
                pendingTask.Remove(task);
                if (pendingTask.Count == 0)
                    SetBusy(false);

                if (token.IsCancellationRequested)
                {
                    isNewRecipeEntering = false;
                    return;
                }
                else if (pendingTask.Count == 0)
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();

                if (ret.Exception != null)
                {
                    SetAddCmdState(true, ret.Exception.InnerException.Message);
                    if (UIInterface != null)
                    {
                        if (ret.Exception.InnerException is WarningException)
                            UIInterface.ShowWarning(ret.Exception.InnerException.Message);
                        else
                            UIInterface.ShowError(ret.Exception.InnerException.Message);
                    }
                }
                if (RecipeIndexUI != null)
                    RecipeIndexUI.SelectedRecipeId = newguid;
                else
                {
                    for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                    {
                        if (dataSet.Tables[cc].DefaultView.RowStateFilter != DataViewRowState.CurrentRows)
                            dataSet.Tables[cc].DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                        dataSet.Tables[cc].DefaultView.RowFilter = String.Format("[{0}]='{1}'",
                                                                    dataSet.Tables[cc].PrimaryKey[0].ColumnName,
                                                                    newguid);
                    }
                }

                isNewRecipeEntering = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());

            pendingTask.Add(task);
        }

        void StringEditor_CultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                ChangeLanguage();
            });
        }

        #endregion

        #region Properties
        public LayoutControl LayoutItems
        { 
            get 
            { 
                return layoutItems;
            } 
        }

        public bool IsLayoutEmpty
        {
            get
            {
                for (int ii = 0; ii < layoutItems.Children.Count; ii++)
                {
                    if (layoutItems.Children[ii] is LayoutGroup || layoutItems.Children[ii] is LayoutItem)
                        return false;
                }

                return true;
            }
        }

        bool _IsInsertRecipeEnabled;
        public bool IsInsertRecipeEnabled 
        {
            get
            {
                return _IsInsertRecipeEnabled;
            }
            set
            {
                if (_IsInsertRecipeEnabled == value)
                    return;
                _IsInsertRecipeEnabled = value;

                if (RecipeIndexUI != null)
                    RecipeIndexUI.AllowEdit = _IsInsertRecipeEnabled;
            }
        }

        bool _IsEditRecipeEnabled;
        public bool IsEditRecipeEnabled {
            get
            {
                return _IsEditRecipeEnabled;
            }
            set
            {
                if (_IsEditRecipeEnabled == value)
                    return;
                _IsEditRecipeEnabled = value;

                if (listDataValueUI != null)
                {
                    foreach (var itemUI in listDataValueUI)
                        itemUI.AllowEdit = _IsEditRecipeEnabled;
                }
            }
        }

        public bool IsRemoveRecipeEnabled { get; set; }

        public bool IsImportRecipeEnabled { get; set; }

        public bool IsExportRecipeEnabled { get; set; }

        public bool IsReadRecipeEnabled { get; set; }

        public bool IsWriteRecipeEnabled { get; set; }

        public RecipeEntityHelper ReadEntityHelper { get; set; }

        public RecipeEntityHelper WriteEntityHelper { get; set; }

        IStringEditorManager stringEditorManager;
        IStringEditorManager StringEditorManager
        {
            get
            {
                if (stringEditorManager == null && RecipeDocument != null)
                {
                    stringEditorManager = RecipeDocument.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                }

                return stringEditorManager;
            }
        }

        IUIMsgBoxAlertService uiinterface;
        IUIMsgBoxAlertService UIInterface
        {
            get
            {
                if (uiinterface == null && RecipeDocument != null)
                {
                    uiinterface = RecipeDocument.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                }

                return uiinterface;
            }
        }

        #endregion

        #region Public Methods
        public void AddAvailableItem(FrameworkElement item, object parent = null)
        {
            if (parent != null)
            { 
                var groups = (from c in layoutItems.AvailableItems/*.AsParallel()*/
                              where c.Tag == parent
                              select c).ToList();

                if (groups.Count > 0)
                {
                    var group = groups[0] as IPanel;
                    group.Children.Add(item);
                    layoutItems.RegisterNameRecursive(item);
                }
            }
            else if (!layoutItems.AvailableItems.Contains(item))
            {
                layoutItems.AvailableItems.Add(item);
                layoutItems.RegisterNameRecursive(item);
            };
        }

        public void RemoveAvailableItem(object tag)
        {
            // first search in available items
            FrameworkElement item = FindAvailableItemFromTag(tag);
            if (item == null)
            {
                // next search in the layout control
                item = FindAvailableItemFromTag(tag, layoutItems);
            }

            if (item != null)
            {
                var parent = item.GetParent();
                if (parent != null && parent is IPanel)
                    (parent as IPanel).Children.Remove(item);
                else
                    layoutItems.AvailableItems.Remove(item);
                layoutItems.UnregisterNameRecursive(item);
            }
        }

        public void UpdateAvailableItem(UFDataValueEntity tag)
        { 
            // first search in available items
            FrameworkElement item = FindAvailableItemFromTag(tag);
            if (item == null)
            {
                // next search in the layout control
                item = FindAvailableItemFromTag(tag, layoutItems);
            }

            if (item != null && item is RecipeEditValueLayoutItem)
            {
                var editValueLayoutItem = item as RecipeEditValueLayoutItem;
                var layoutitem = LayoutControlHelper.CreateLayoutItemControl(tag, editValueLayoutItem.ControlType) as RecipeLayoutItem;
                var uie = layoutitem.Content;
                layoutitem.Content = null;
                editValueLayoutItem.Content = uie;
                editValueLayoutItem.SetContentValues();
            }
        }

        public void AddAvailableItems(FrameworkElements items)
        {
            ClearAvailableItems();

            items.ForEach(item =>
            {
                layoutItems.AvailableItems.Add(item);
                layoutItems.RegisterNameRecursive(item);
            });
        }

        public void ClearAvailableItems()
        {
            layoutItems.AvailableItems.ForEach(item =>
            {
                layoutItems.UnregisterNameRecursive(item);
            });

            layoutItems.GetChildren(false).ForEach(item =>
            {
                layoutItems.UnregisterNameRecursive(item);
            });

            layoutItems.AvailableItems.Clear();
            layoutItems.Children.Clear();
        }

        public void CleanAvailableItems()
        {
            if (layoutItems.AvailableItems.Count > 0)
            {
                var availableitems = (from c in layoutItems.AvailableItems
                                      where c.Tag == null
                                      select c).ToList();
                
                availableitems.ForEach(item => { layoutItems.AvailableItems.Remove(item); });
            }
        }

        Dictionary<UIElement, String> mapControlId;
        Dictionary<UIElement, String> mapControlToolTip;
        public void ChangeLanguage()
        {
            if (StringEditorManager != null)
            {
                if (mapControlId == null)
                    mapControlId = new Dictionary<UIElement, String>();
                if (mapControlToolTip == null)
                    mapControlToolTip = new Dictionary<UIElement, String>();

                Regex pattern = new Regex("['*.:;,\t\r\n]");
                Regex pattern1 = new Regex("[ ]");
                using (var cursor = new WaitCursor())
                {
                    var cultures = StringEditorManager.GetListStringForCulture(RecipeDocument, StringEditorManager.GetActiveCulture(RecipeDocument));

                    if (cultures == null || cultures.Count == 0)
                        return;

                    var groups = layoutItems.GetChildrenOfType<LayoutGroup>();
                    var groupsText = (from c in groups where c.Header is String && !String.IsNullOrEmpty(c.Header as String) select c).ToList();
                    groupsText.ForEach(control =>
                    {
                        var text = control.Header as String;
                        if (!mapControlId.ContainsKey(control))
                            mapControlId.Add(control, text);
                        string _control = pattern1.Replace(pattern.Replace(mapControlId[control], ""), "_");
                        string _id = $"_RecipeViewer_{_control}";
                        if (cultures.ContainsKey(_id) &&
                            !String.IsNullOrEmpty(cultures[_id]))
                        {
                            control.Header = cultures[_id];
                        }
                        else if (cultures.ContainsKey(mapControlId[control]) &&
                                !String.IsNullOrEmpty(cultures[mapControlId[control]]))
                            control.Header = cultures[mapControlId[control]];
                        else
                            control.Header = mapControlId[control];
                    });

                    var groupsTooltips = (from c in groups where c.ToolTip is String && !String.IsNullOrEmpty(c.ToolTip as String) select c).ToList();
                    groupsTooltips.ForEach(control =>
                    {
                        var text = control.ToolTip as String;
                        if (!mapControlToolTip.ContainsKey(control))
                            mapControlToolTip.Add(control, text);

                        string _control = pattern1.Replace(pattern.Replace(mapControlToolTip[control], ""), "_");
                        string _id = $"_RecipeViewer_{_control}";
                        if (cultures.ContainsKey(_id) &&
                            !String.IsNullOrEmpty(cultures[_id]))
                        {
                            control.ToolTip = cultures[_id];
                        }
                        else if (cultures.ContainsKey(mapControlToolTip[control]) &&
                                !String.IsNullOrEmpty(cultures[mapControlToolTip[control]]))
                            control.ToolTip = cultures[mapControlToolTip[control]];
                        else
                            control.ToolTip = mapControlToolTip[control];
                    });

                    var items = layoutItems.GetChildrenOfType<LayoutItem>();
                    var itemsText = (from c in items where c.Label is String && !String.IsNullOrEmpty(c.Label as String) select c).ToList();
                    itemsText.ForEach(control =>
                    {
                        var text = control.Label as String;
                        if (!mapControlId.ContainsKey(control))
                            mapControlId.Add(control, text);

                        string _control = pattern1.Replace(pattern.Replace(mapControlId[control], ""), "_");
                        string _id = $"_RecipeViewer_{_control}";
                        if (cultures.ContainsKey(_id) &&
                            !String.IsNullOrEmpty(cultures[_id]))
                        {
                            control.Label = cultures[_id];
                        }
                        else if (cultures.ContainsKey(mapControlId[control]) &&
                                !String.IsNullOrEmpty(cultures[mapControlId[control]]))
                            control.Label = cultures[mapControlId[control]];
                        else
                            control.Label = mapControlId[control];
                    });

                    var itemsTooltips = (from c in items where c.ToolTip is String && !String.IsNullOrEmpty(c.ToolTip as String) select c).ToList();
                    itemsTooltips.ForEach(control =>
                    {
                        var text = control.ToolTip as String;
                        if (!mapControlToolTip.ContainsKey(control))
                            mapControlToolTip.Add(control, text);

                        string _control = pattern1.Replace(pattern.Replace(mapControlToolTip[control], ""), "_");
                        string _id = $"_RecipeViewer_{_control}";
                        if (cultures.ContainsKey(_id) &&
                            !String.IsNullOrEmpty(cultures[_id]))
                        {
                            control.ToolTip = cultures[_id];
                        }
                        else if (cultures.ContainsKey(mapControlToolTip[control]) &&
                                !String.IsNullOrEmpty(cultures[mapControlToolTip[control]]))
                            control.ToolTip = cultures[mapControlToolTip[control]];
                        else
                            control.ToolTip = mapControlToolTip[control];
                    });

                    var itemsCommandButton = layoutItems.GetChildrenOfType<RecipeCommandButtonLayoutItem>();
                    var itemsCommandText = (from c in itemsCommandButton where c.Caption is String && !String.IsNullOrEmpty(c.Caption as String) select c).ToList();
                    itemsCommandText.ForEach(control =>
                    {
                        var text = control.Caption as String;
                        if (!mapControlId.ContainsKey(control))
                            mapControlId.Add(control, text);

                        string _control = pattern1.Replace(pattern.Replace(mapControlId[control], ""), "_");
                        string _id = $"_RecipeViewer_{_control}";
                        if (cultures.ContainsKey(_id) &&
                            !String.IsNullOrEmpty(cultures[_id]))
                        {
                            control.Caption = cultures[_id];
                        }
                        else if (cultures.ContainsKey(mapControlId[control]) &&
                                !String.IsNullOrEmpty(cultures[mapControlId[control]]))
                            control.Caption = cultures[mapControlId[control]];
                        else
                            control.Caption = mapControlId[control];
                    });
                }
            }
        }
        #endregion

        #region Private Methods

        FrameworkElement FindAvailableItemFromTag(object tag, IPanel parent = null)
        {
            FrameworkElement item = null;
            if (parent == null)
            {
                var elements = (from c in layoutItems.AvailableItems
                                where c.Tag == tag
                                select c).ToList();

                if (elements.Count > 0)
                    return elements[0];

                var groups = (from c in layoutItems.AvailableItems
                              where c is IPanel
                              select c).ToList();

                foreach (var group in groups)
                {
                    item = FindAvailableItemFromTag(tag, group as IPanel);
                    if (item != null)
                        break;
                }
            }
            else if (parent is FrameworkElement)
            {
                var fe = parent as FrameworkElement;
                var elements = (from c in fe.GetVisualChildrenOfType<LayoutItem>()
                                where c.Tag == tag
                                select c).ToList();

                if (elements.Count > 0)
                    return elements[0];

                /*
                foreach (var child in parent.Children)
                {
                    if (child is FrameworkElement && (child as FrameworkElement).Tag == tag)
                        return (child as FrameworkElement);
                    else if (child is IPanel)
                    {
                        item = FindAvailableItemFromTag(tag, out owner, child as IPanel);
                        if (item != null)
                            break;
                    }
                }
                */
            }

            return item;
        }

        void ResetCommandState()
        {
            if (ReloadCommandUI != null)
            {
                ReloadCommandUI.SetToolTip(null);
                ReloadCommandUI.SetIcon(reloadIcon);
            }
            if (AddCommandUI != null)
            {
                AddCommandUI.SetToolTip(null);
                AddCommandUI.SetIcon(addIcon);
            }
            if (SaveCommandUI != null)
            {
                SaveCommandUI.SetToolTip(null);
                SaveCommandUI.SetIcon(saveIcon);
            }
            if (RemoveCommandUI != null)
            {
                RemoveCommandUI.SetToolTip(null);
                RemoveCommandUI.SetIcon(removeIcon);
            }
            if (ImportCommandUI != null)
            {
                ImportCommandUI.SetToolTip(null);
                ImportCommandUI.SetIcon(importIcon);
            }
            if (ExportCommandUI != null)
            {
                ExportCommandUI.SetToolTip(null);
                ExportCommandUI.SetIcon(exportIcon);
            }
            if (ReadCommandUI != null)
            {
                ReadCommandUI.SetToolTip(null);
                ReadCommandUI.SetIcon(readIcon);
            }
            if (WriteCommandUI != null)
            {
                WriteCommandUI.SetToolTip(null);
                WriteCommandUI.SetIcon(writeIcon);
            }
        }

        void SetReloadCmdState(bool bError, String tooltip)
        {
            if (ReloadCommandUI != null)
            {
                if (bError)
                {
                    ReloadCommandUI.SetToolTip(tooltip);
                    ReloadCommandUI.SetIcon(reloadIconError);
                }
                else
                {
                    ReloadCommandUI.SetToolTip(tooltip);
                    ReloadCommandUI.SetIcon(reloadIconOk);
                }
            }
        }

        void SetAddCmdState(bool bError, String tooltip)
        {
            if (AddCommandUI != null)
            {
                if (bError)
                {
                    AddCommandUI.SetToolTip(tooltip);
                    AddCommandUI.SetIcon(addIconError);
                }
                else
                {
                    AddCommandUI.SetToolTip(tooltip);
                    AddCommandUI.SetIcon(addIconOk);
                }
            }
        }

        void SetSaveCmdState(bool bError, String tooltip)
        {
            if (SaveCommandUI != null)
            {
                if (bError)
                {
                    SaveCommandUI.SetToolTip(tooltip);
                    SaveCommandUI.SetIcon(saveIconError);
                }
                else
                {
                    SaveCommandUI.SetToolTip(tooltip);
                    SaveCommandUI.SetIcon(saveIconOk);
                }
            }
        }

        void SetRemoveCmdState(bool bError, String tooltip)
        {
            if (RemoveCommandUI != null)
            {
                if (bError)
                {
                    RemoveCommandUI.SetToolTip(tooltip);
                    RemoveCommandUI.SetIcon(removeIconError);
                }
                else
                {
                    RemoveCommandUI.SetToolTip(tooltip);
                    RemoveCommandUI.SetIcon(removeIconOk);
                }
            }
        }

        void SetImportCmdState(bool bError, String tooltip)
        {
            if (ImportCommandUI != null)
            {
                if (bError)
                {
                    ImportCommandUI.SetToolTip(tooltip);
                    ImportCommandUI.SetIcon(importIconError);
                }
                else
                {
                    ImportCommandUI.SetToolTip(tooltip);
                    ImportCommandUI.SetIcon(importIconOk);
                }
            }
        }

        void SetExportCmdState(bool bError, String tooltip)
        {
            if (ExportCommandUI != null)
            {
                if (bError)
                {
                    ExportCommandUI.SetToolTip(tooltip);
                    ExportCommandUI.SetIcon(exportIconError);
                }
                else
                {
                    ExportCommandUI.SetToolTip(tooltip);
                    ExportCommandUI.SetIcon(exportIconOk);
                }
            }
        }

        object readToolTip;
        void SetReadCmdState(bool bError, String tooltip)
        {
            if (ReadCommandUI != null)
            {
                if (bError)
                {
                    ReadCommandUI.SetToolTip(tooltip);
                    ReadCommandUI.SetIcon(readIconError);
                }
                else
                {
                    ReadCommandUI.SetToolTip(tooltip);
                    ReadCommandUI.SetIcon(readIconOk);
                }
            }

            if (ReadEntityHelper != null)
            {
                var layoutItem = ReadEntityHelper.Control as LayoutItem;
                if (layoutItem != null)
                {
                    if (bError)
                    {
                        readToolTip = layoutItem.ToolTip;
                        layoutItem.ToolTip = null;
                    }
                    else
                    {
                        layoutItem.ToolTip = readToolTip;
                    }
                }
            }
        }

        object writeToolTip;
        void SetWriteCmdState(bool bError, String tooltip)
        {
            if (WriteCommandUI != null)
            {
                if (bError)
                {
                    WriteCommandUI.SetToolTip(tooltip);
                    WriteCommandUI.SetIcon(writeIconError);
                }
                else
                {
                    WriteCommandUI.SetToolTip(tooltip);
                    WriteCommandUI.SetIcon(writeIconOk);
                }
            }

            if (WriteEntityHelper != null)
            {
                var layoutItem = WriteEntityHelper.Control as LayoutItem;
                if (layoutItem != null)
                {
                    if (bError)
                    {
                        writeToolTip = layoutItem.ToolTip;
                        layoutItem.ToolTip = null;
                    }
                    else
                    {
                        layoutItem.ToolTip = writeToolTip;
                    }
                }
            }
        }

        void SetBusy(bool bSet)
        {
            busyPanel.Visibility = bSet ? Visibility.Visible : Visibility.Collapsed;
        }

        Task CheckAndAskSavePendingChanges()
        {
            return CheckAndAskSavePendingChanges(Guid.NewGuid());
        }

        Task CheckAndAskSavePendingChanges(Guid newRecipeID)
        {
            if (RecipeIndexUI == null || dataSet == null)
                return null;

            if (RecipeIndexUI.SelectedRecipeId != Guid.Empty &&
                RecipeIndexUI.SelectedRecipeId != newRecipeID &&
                (dataSet.HasChanges(DataRowState.Modified) || dataSet.HasChanges(DataRowState.Added)))
            {
                if (layoutItems.ValidateBindings())
                {
                    CustomDialogResults result = CustomDialogResults.No;
                    if (UIInterface != null)
                    {
                        result = UIInterface.ShowYesNo(Properties.Resources.AskSaveChanges, CustomDialogIcons.Question);
                    }

                    if (result == CustomDialogResults.Yes)
                    {
                        SetBusy(true);

                        if (ctsPendingTask == null)
                            ctsPendingTask = new CancellationTokenSource();
                        CancellationToken token = ctsPendingTask.Token;

                        var task = Task.Factory.StartNew(() =>
                        {
                            var recipeEventArgs = new RecipeCommandEventArgs()
                            {
                                RecipeID = RecipeIndexUI.SelectedRecipeId,
                                CommandType = EditCommandType.Save,
                                CancellationToken = token
                            };
                            OnExecutedCommand(this, recipeEventArgs);
                            if (recipeEventArgs.exception != null)
                                throw recipeEventArgs.exception;
                        }, token);

                        task.ContinueWith(ret =>
                        {
                            pendingTask.Remove(task);
                            if (pendingTask.Count == 0)
                                SetBusy(false);

                            if (token.IsCancellationRequested)
                                return;
                            else if (pendingTask.Count == 0)
                                System.Windows.Input.CommandManager.InvalidateRequerySuggested();

                            if (ret.Exception != null)
                            {
                                if (UIInterface != null)
                                {
                                    if (ret.Exception.InnerException is WarningException)
                                        UIInterface.ShowWarning(ret.Exception.InnerException.Message);
                                    else
                                        UIInterface.ShowError(ret.Exception.InnerException.Message);
                                }
                            }
                        }, TaskScheduler.FromCurrentSynchronizationContext());

                        pendingTask.Add(task);

                        return task;
                    }
                }

                SetBusy(true);

                if (ctsPendingTask == null)
                    ctsPendingTask = new CancellationTokenSource();
                CancellationToken token2 = ctsPendingTask.Token;

                var task2 = Task.Factory.StartNew(() =>
                {
                    var recipeEventArgs = new RecipeCommandEventArgs()
                    {
                        RecipeID = RecipeIndexUI.SelectedRecipeId,
                        CommandType = EditCommandType.Reject,
                        CancellationToken = token2
                    };
                    OnExecutedCommand(this, recipeEventArgs);
                    if (recipeEventArgs.exception != null)
                        throw recipeEventArgs.exception;
                }, token2);

                task2.ContinueWith(ret =>
                {
                    pendingTask.Remove(task2);
                    if (pendingTask.Count == 0)
                        SetBusy(false);

                    if (token2.IsCancellationRequested)
                        return;
                    else if (pendingTask.Count == 0)
                        System.Windows.Input.CommandManager.InvalidateRequerySuggested();

                    if (ret.Exception != null)
                    {
                        if (UIInterface != null)
                        {
                            if (ret.Exception.InnerException is WarningException)
                                UIInterface.ShowWarning(ret.Exception.InnerException.Message);
                            else
                                UIInterface.ShowError(ret.Exception.InnerException.Message);
                        }
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());

                pendingTask.Add(task2);

                return task2;
            }

            return null;
        }

        public BitmapImage GetBitmapImage(String image)
        {
            return TryFindResource(image) as BitmapImage;
        }

        void DisposeLayoutControls()
        {
            if (listDataValueUI != null)
            {
                foreach (var itemUI in listDataValueUI)
                {
                    if (itemUI is IDisposable)
                        (itemUI as IDisposable).Dispose();
                }
            }
            if (RecipeIndexUI != null)
            {
                RecipeIndexUI.RecipeTextChanged -= RecipeIndexUI_RecipeTextChanged;
                RecipeIndexUI.RecipeSelectionChanged -= RecipeIndexUI_RecipeSelectionChanged;
                if (RecipeIndexUI is IDisposable)
                    (RecipeIndexUI as IDisposable).Dispose();
            }
            if (ReloadCommandUI is IDisposable)
                (ReloadCommandUI as IDisposable).Dispose();
            if (AddCommandUI is IDisposable)
                (AddCommandUI as IDisposable).Dispose();
            if (SaveCommandUI is IDisposable)
                (SaveCommandUI as IDisposable).Dispose();
            if (RemoveCommandUI is IDisposable)
                (RemoveCommandUI as IDisposable).Dispose();
            if (ImportCommandUI is IDisposable)
                (ImportCommandUI as IDisposable).Dispose();
            if (ExportCommandUI is IDisposable)
                (ExportCommandUI as IDisposable).Dispose();
            if (ReadCommandUI is IDisposable)
                (ReadCommandUI as IDisposable).Dispose();
            if (WriteCommandUI is IDisposable)
                (WriteCommandUI as IDisposable).Dispose();
        }

        #endregion

        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            if (ctsPendingTask != null)
                ctsPendingTask.Cancel();

            if (pendingTask.Count > 0)
                Task.WaitAll(pendingTask.ToArray());

            if (ctsPendingTask != null)
                ctsPendingTask.Dispose();

            if (ReadEntityHelper != null)
                (ReadEntityHelper as IDisposable).Dispose();

            if (WriteEntityHelper != null)
                (WriteEntityHelper as IDisposable).Dispose();

            DisposeLayoutControls();
            ClearAvailableItems();
            layoutItems.Dispose();

            if (mapControlId != null)
                mapControlId.Clear();
        }
    }
}
