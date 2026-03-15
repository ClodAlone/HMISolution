using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using UFInterfaces.Animatable;
using Utilities;
using Utilities.WPF;
using DevExpress.Xpf.Core;
using AnimationManager;
using System.Windows.Controls.Primitives;
using WPFUtilities;

namespace AnimationExplorer.Controls
{
    /// <summary>
    /// Interaction logic for GridControl.xaml
    /// </summary>
    public partial class GridControl : UserControl, IDisposable
    {
        ObservableCollection<AnimationManager.AnimationManager> listAnimation = new ObservableCollection<AnimationManager.AnimationManager>();
        IAnimatable Animatable;
        AnimationEditorUI Container;
        internal Controls.PropertyControl propertyControl;
        bool isWebHMIProject;
        bool bIsActive = true;
        public GridControl(AnimationEditorUI container, bool isWebHMIProject = false)
        {
            InitializeComponent();

            /*
            var style = ApplicationPropertiesHelper.GetProperty("CurrentSkin") as String;
            switch (style)
            {
                case "Blend": ThemeManager.SetThemeName(this, "MetropolisDark"); break;
                case "VS2010": ThemeManager.SetThemeName(this, "VisualStudio2010"); break;
                case "Office2010Black": ThemeManager.SetThemeName(this, "Office2010Black"); break;
                case "Office2007Silver":
                case "Office2010Silver": ThemeManager.SetThemeName(this, "Office2007Silver"); break;
                case "Office2007Blue":
                case "Office2010Blue": ThemeManager.SetThemeName(this, "Office2007Blue"); break;
                default: ThemeManager.SetThemeName(this, "DevExpressStyle"); break;
            }
            */
            this.isWebHMIProject = isWebHMIProject;
            Container = container;

            //gridControl.Model.Options.ListBoxSelectionMode = GridSelectionMode.MultiExtended;
            //gridControl.Model.Options.AllowSelection = GridSelectionFlags.Any & ~GridSelectionFlags.Cell & ~GridSelectionFlags.Column & ~GridSelectionFlags.Table;
            //gridControl.Model.CurrencyManager.CurrentRecordSelectionChanged += CurrencyManager_CurrentRecordSelectionChanged;
            // gridControl.SourceType = typeof(AnimationManager.AnimationManager);
            gridControl.ItemsSource = listAnimation;

            Loaded += (o, e) =>
                {
                    var selector = this.FindParent<Selector>();
                    if (selector != null && selector.SelectedItem is ContentControl &&
                        (selector.SelectedItem as ContentControl).Content == Container)
                        RefreshList();
                };
            Unloaded += (o, e) =>
                {
                    if (bIgnoreUnloaded)
                        bIgnoreUnloaded = false;
                    else
                        CancelCurrentAnimationPreview();
                };
        }

        public void SetIsActive(bool bSet)
        {
            bIsActive = bSet;
            if (bIsActive)
                StartDemoAnimation();
        }

        bool bIgnoreUnloaded;
        public void RefreshList()
        {
            bIgnoreUnloaded = true;
            gridControl.RefreshData();
            //Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
            //    {
                    StartDemoAnimation();
                //});
        }

        public void PropagateChanges()
        {
            CancelCurrentAnimationPreview();
            Animatable.AnimationList = listAnimation;
        }

        AnimationManager.AnimationManager oldAnimation;
        private void CancelCurrentAnimationPreview()
        {
            if (oldAnimation != null)
            {
                oldAnimation.Stop();
                oldAnimation = null;
            }

            var am = tableView.FocusedRow as AnimationManager.AnimationManager;
            if (am != null)
                am.Stop();
        }

        //void CurrencyManager_CurrentRecordSelectionChanged(object sender, GridDataCurrentRecordSelectionChangedEventArgs args)
        //{
        //    CancelCurrentAnimationPreview();

        //    if (tableView.FocusedRow != null)
        //    {
        //        AnimationManager.AnimationManager am = tableView.FocusedRow as AnimationManager.AnimationManager;
        //        am.Demo();
        //        oldAnimation = am;
        //    }
        //}

        private void tableView_FocusedRowChanged(object sender, DevExpress.Xpf.Grid.FocusedRowHandleChangedEventArgs e)
        {
            UpdateSelection();
        }

        private void tableView_SelectionChanged(object sender, DevExpress.Xpf.Grid.GridSelectionChangedEventArgs e)
        {
            UpdateSelection();
        }

        void UpdateSelection()
        {
            if (propertyControl.contentAnimation.Content is IDisposable)
                (propertyControl.contentAnimation.Content as IDisposable).Dispose();

            if (tableView.FocusedRow != null)
            {
                AnimationManager.AnimationManager am = tableView.FocusedRow as AnimationManager.AnimationManager;
                var userControl = am.Editor;
                userControl.DataContext = am;
                userControl.ClearValue(FrameworkElement.WidthProperty);
                userControl.ClearValue(FrameworkElement.HeightProperty);
                propertyControl.contentAnimation.Content = userControl;

                //Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
                //    {
                        StartDemoAnimation();
                    //});
            }
            else
                propertyControl.contentAnimation.Content = null;

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                btnDelete.IsEnabled = tableView.FocusedRow != null;
            });
        }

        internal void StartDemoAnimation()
        {
            CancelCurrentAnimationPreview();
            if (!bIsActive)
                return;

            if (tableView.FocusedRow == null && listAnimation.Count > 0)
                gridControl.SelectedItem = listAnimation[0];
            if (tableView.FocusedRow != null)
            {
                AnimationManager.AnimationManager am = tableView.FocusedRow as AnimationManager.AnimationManager;
                am.Demo();
                oldAnimation = am;
            }
        }

        private void addNewAnimation_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var btn = sender as Button;
                var type = btn.Tag as String;
                AnimationManager.AnimationManager am = AnimationManager.AnimationManager.CreateFrom(type);
                var typeExpected = am.ExpectingControl();
                bool bSupportType = true;
                if (typeExpected != null)
                {
                    bSupportType = false;
                    var typeElement = Animatable.Element.GetType();
                    foreach(var el in typeExpected)
                    {
                        if (el.IsAssignableFrom(typeElement))
                        {
                            bSupportType = true;
                            break;
                        }
                    }
                }
                if (!bSupportType)
                {
                    var expected = new StringBuilder();
                    foreach(var el in typeExpected)
                    {
                        if (expected.Length > 0)
                            expected.Append(", ");
                        expected.Append(el.Name);
                    }
                    MessageBox.Show(String.Format(Properties.Resources.ExpectingType, am.Name, expected.ToString()));
                }
                else
                {
                    if (am.NotSupportedControl(Animatable.Element))
                    {
                        MessageBox.Show(Properties.Resources.NotSupportedControl);
                    }
                    else
                    {
                        am.Control = Animatable.Element;
                        am.Control3D = Animatable.Element3D;
                        listAnimation.Add(am);
                        tableView.FocusedRow = am;
                    }
                }
            }
        }

        private void control_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CancelCurrentAnimationPreview();

            gridControl.ItemsSource = null;
            listAnimation.Clear();
            Animatable = DataContext as IAnimatable;
            if (Animatable == null)
                return;

            toolbar.BeginInit();
            toolbar.Children.Clear();
            var VisibleHMIAnimations = WebHMIDesignHelper.WebHMIHelper.VisibleHMIAnimations;
            foreach (var s in AnimationManager.AnimationManager.LoadAnimationTypes())
            {
                if (isWebHMIProject && !VisibleHMIAnimations.Contains(AnimationManager.AnimationManager.GetAnimationTypeName(s)))
                    continue;
                AnimationManager.AnimationManager am = AnimationManager.AnimationManager.CreateFrom(s);
                if (Animatable.Element3D != null && !am.Is3D || Animatable.Element != null && !am.Is2D)
                    continue;

                var btn = new Button();
                btn.Click += addNewAnimation_Click;
                btn.Tag = s;

                if (am.Image != null)
                    btn.Content = new Image() { Source = am.Image };
                else
                    btn.Content = String.Format("+ {0}", am.Name);

                btn.ToolTip = String.Format(Properties.Resources.AddNewPrefix, am.Name);
                btn.HorizontalContentAlignment = HorizontalAlignment.Left;
                btn.BorderThickness = new Thickness(0);
                toolbar.Children.Add(btn);
            }
            toolbar.EndInit();

            foreach (var v in Animatable.AnimationList)
                listAnimation.Add(v as AnimationManager.AnimationManager);

            gridControl.ItemsSource = listAnimation;
            if (listAnimation.Count > 0)
                tableView.FocusedRow = listAnimation.First();
            StartDemoAnimation();
        }

        private void DeleteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CancelCurrentAnimationPreview();

            btnDelete.IsEnabled = false;
            e.Handled = true;
            if (tableView.FocusedRow != null && listAnimation.Contains(tableView.FocusedRow))
                listAnimation.Remove(tableView.FocusedRow as AnimationManager.AnimationManager);

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                btnDelete.IsEnabled = tableView.FocusedRow != null;
            });
        }

        private void DeleteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tableView.FocusedRow != null;
        }

        private void UserControl_DragOver(object sender, DragEventArgs e)
        {
            if (!(e.Data.GetDataPresent("Item")) || e.Data.GetDataPresent(typeof(RecordDragDropData)))
                return;

            SetDragDropEffects(e);
            e.Handled = true;
        }

        private void SetDragDropEffects(DragEventArgs e)
        {
            //if the CTRL key is down, treat this as a copy
            e.Effects = (e.KeyStates & DragDropKeyStates.ControlKey) != 0 && (e.AllowedEffects & DragDropEffects.Copy) != 0 ? DragDropEffects.Copy : (e.AllowedEffects & DragDropEffects.Move) != 0 ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            FrameworkElement targetElement = sender as FrameworkElement;

            //see if ISF is present in the drag and drop IDataObject
            if (e.Data.GetDataPresent(typeof(RecordDragDropData)))
            {
                // int nRow = GridView.GetRowHandleByTreeElement(e.OriginalSource as DependencyObject);
                if (tableView.FocusedRow != null)
                {
                    AnimationManager.AnimationManager am = tableView.FocusedRow as AnimationManager.AnimationManager;

                    Point dropPosition = e.GetPosition(targetElement);
                    var data = e.Data.GetData(typeof(RecordDragDropData)) as RecordDragDropData;
                    if (data == null)
                        return;

                    foreach (var v in data.Records)
                    {
                        var subitem = v as TreeItemControl;
                        if (subitem == null || subitem.TreeItemInnerObject == null)
                            continue;

                        if (am.OnDropReference(subitem.TreeItemInnerObject))
                        {
                            break;
                        }
                    }
                }
            }

        }

        private void UserControl_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (!e.EscapePressed)
                return;

            e.Action = DragAction.Cancel;
            e.Handled = true;
        }

        public void Dispose()
        {
            CancelCurrentAnimationPreview();
            listAnimation.Clear();

            if (propertyControl.contentAnimation.Content is IDisposable)
                (propertyControl.contentAnimation.Content as IDisposable).Dispose();
        }

        private void toolbar_LayoutUpdated(object sender, EventArgs e)
        {
            var listWidths = (from c in toolbar.Children.OfType<Button>()
                              orderby c.ActualWidth descending
                              select c.ActualWidth).ToList();
            if (listWidths.Count > 0 && listWidths[0] > 0)
            {
                foreach (Button btn in toolbar.Children)
                    btn.Width = listWidths[0];
            }
        }

        private void MoveUpCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = tableView.FocusedRow as AnimationManager.AnimationManager;
            int index = listAnimation.IndexOf(command);
            listAnimation.Remove(command);
            listAnimation.Insert(index - 1, command);
            tableView.FocusedRow = command;
        }

        private void MoveDownCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = tableView.FocusedRow as AnimationManager.AnimationManager;
            int index = listAnimation.IndexOf(command);
            listAnimation.Remove(command);
            listAnimation.Insert(index + 1, command);
            tableView.FocusedRow = command;
        }

        private void MoveUpCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tableView.FocusedRow != null && listAnimation.Contains(tableView.FocusedRow) && listAnimation.IndexOf(tableView.FocusedRow as AnimationManager.AnimationManager) > 0;
        }

        private void MoveDownCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tableView.FocusedRow != null && listAnimation.Contains(tableView.FocusedRow) && listAnimation.IndexOf(tableView.FocusedRow as AnimationManager.AnimationManager) < listAnimation.Count - 1;
        }

        private void CopyCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = tableView.FocusedRow as AnimationManager.AnimationManager;
            var list = new AnimationManagerList();
            list.Add(command);
            var str = list.ToXml();
            Clipboard.SetText(str, TextDataFormat.UnicodeText);
        }

        private void PasteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = tableView.FocusedRow as AnimationManager.AnimationManager;
            int index = listAnimation.Count;
            if (command != null && listAnimation.Contains(command))
                index = listAnimation.IndexOf(command);

            var str = Clipboard.GetText(TextDataFormat.UnicodeText);
            var list = str.FromXml<AnimationManagerList>();
            var element = Animatable.Element;
            var animationList = (from am in list where !am.NotSupportedControl(element) select am).ToList();
            foreach (var cmd in animationList)
            {
                cmd.Control = element;
                cmd.Control3D = Animatable.Element3D;

                listAnimation.Insert(index, cmd);
            }
            if(animationList.Count < list.Count)
                MessageBox.Show(Properties.Resources.NotAllAnimationPasted);
            if (animationList.Count > 0)
                tableView.FocusedRow = list[list.Count - 1];
        }

        private void CopyCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tableView.FocusedRow != null;
        }

        private void PasteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                string str = Clipboard.GetText(TextDataFormat.UnicodeText);
                e.CanExecute = str.FromXml<AnimationManagerList>() != null;
            }
            catch (Exception ex)
            {
                e.CanExecute = false;
            }
        }

        private void CopyAllCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var list = new AnimationManagerList();
            foreach(var am in listAnimation)
                list.Add(am);
            var str = list.ToXml();
            Clipboard.SetText(str, TextDataFormat.UnicodeText);
        }

        private void CopyAllCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = listAnimation.Count > 0;
        }
    }
}
