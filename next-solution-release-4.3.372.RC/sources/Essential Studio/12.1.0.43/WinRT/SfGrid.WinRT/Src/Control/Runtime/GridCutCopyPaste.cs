#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Syncfusion.Data;
using Syncfusion.Data.Extensions;
using System.Text;
using System.Reflection;
#if WinRT
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Syncfusion.WinRT.Data;
#endif
using System.Windows;

namespace Syncfusion.UI.Xaml.Grid
{
    public class GridCutCopyPaste : IGridCopyPaste, IDisposable
    {
        #region Fields

        private SfDataGrid dataGrid;

        #endregion

        #region Ctor

        public GridCutCopyPaste(SfDataGrid dataGrid)
        {
            this.dataGrid = dataGrid;
        }

        #endregion

        #region Private Methods

        private void CopyTextToClipBoard(bool cut)
        {
#if WinRT
            var datapackage = new DataPackage();
#elif WPF
            var dataObject = new DataObject();
#endif
            StringBuilder text = new StringBuilder();


            if ((dataGrid.GridCopyPasteOption & GridCopyPasteOption.CopyData) == GridCopyPasteOption.CopyData || (dataGrid.GridCopyPasteOption & GridCopyPasteOption.CutData) == GridCopyPasteOption.CutData)
            {
                if ((dataGrid.GridCopyPasteOption & GridCopyPasteOption.IncludeHeaders) == GridCopyPasteOption.IncludeHeaders)
                {
                    dataGrid.Columns.ForEach(col =>
                    {
                        if (!col.IsHidden)
                            text = text.Append(col.MappingName + "\t");
                    });
                    text = text.Append("\r\n");
                }
                this.dataGrid.SelectedItems.ForEach(item =>
                {
                    foreach (var column in dataGrid.Columns)
                    {
                        if (column.IsHidden)
                            continue;
                        if (!column.IsUnbound)
                        {
                            var unBoundText = this.dataGrid.View.GetPropertyAccessProvider().GetValue(item, column.MappingName);
                            if (unBoundText != null)
                                text = text.Append(unBoundText.ToString());
                            else
                                text = text.Append(string.Empty);
                        }
                        else
                            text = text.Append(this.dataGrid.GetUnBoundCellValue(column, item).ToString());
                        text = text.Append("\t");
                    }
                    text = text.Append("\r\n");
                });
                var action = cut == false ? ClipBoardAction.Copy : ClipBoardAction.Cut;
#if WinRT
                if (text.Length > 0)
                    datapackage.SetText(text.ToString());
                var args = this.dataGrid.RaiseCopyContentEvent(new GridCopyPasteEventArgs(false, datapackage, action, this.dataGrid));
                
#elif WPF
                if (text.Length > 0)
                    dataObject.SetText(text.ToString());
                var args = this.dataGrid.RaiseCopyContentEvent(new GridCopyPasteEventArgs(false, dataObject, action, this.dataGrid));
#else
                var args = this.dataGrid.RaiseCopyContentEvent(new GridCopyPasteEventArgs(false,text.ToString(),action, this.dataGrid));
#endif




                if (!args.Handled)
                {
#if WinRT
                    if (args.DataPackage != null && args.DataPackage != datapackage)

#elif WPF
                    if (args.DataObject != null && args.DataObject != dataObject)
#else
                    if(args.ClipBoardText != text.ToString() && args.ClipBoardText != null)
#endif
                    {
                        if (args.ClipBoardAction == ClipBoardAction.Cut)
                            ClearCellsByCut();
#if WinRT
                        Clipboard.SetContent(args.DataPackage);
#elif WPF
                        Clipboard.SetDataObject(args.DataObject);
#else
                        Clipboard.SetText(args.ClipBoardText);
#endif
                    }
                    else
                    {
                        if (cut)
                        {
                            ClearCellsByCut();
                        }
#if WinRT
                        Clipboard.SetContent(datapackage);
#elif WPF
                        Clipboard.SetDataObject(dataObject);
#else
                        Clipboard.SetText(text.ToString());
#endif
                    }
                }
            }
        }

        private void ClearCellsByCut()
        {
            var selectedRecords = new List<object>(this.dataGrid.SelectedItems.ToArray());
            var provider = this.dataGrid.View.GetPropertyAccessProvider();
            var properyCollection = this.dataGrid.View.GetItemProperties();
            selectedRecords.ForEach(item =>
            {
                foreach (var col in dataGrid.Columns)
                {
                    Type type;
                    if (col.IsHidden)
                        continue;
                    if (col.IsUnbound)
                    {
                        var unBoundText = this.dataGrid.GetUnBoundCellValue(col, item).ToString();
                        if (unBoundText != null)
                            type = unBoundText.GetType();
                        else
                            continue;
                    }
                    else
                    {
                        var cellText = provider.GetValue(item, col.MappingName);
                        if (cellText != null)
                            type = cellText.GetType();
                        else
                            continue;
                    }
#if !WPF
                    if (!col.IsUnbound && !properyCollection.Find(col.MappingName, false).CanWrite)
#else
                    if (!col.IsUnbound && properyCollection.Find(col.MappingName, false).IsReadOnly)
#endif
                        continue;
#if WinRT
                    if (type.IsValueType())
#else
                    if (type.IsValueType)
#endif
                        provider.SetValue(item, col.MappingName, Activator.CreateInstance(type));
                    else if (type.FullName == "System.String")
                        provider.SetValue(item, col.MappingName, string.Empty);
                    
                }
            });
        }

        private object ConvertToType(object value, Type type)
        {
#if WinRT
            var method = type.GetTypeInfo().DeclaredMethods.Where(x => x.Name.Equals("TryParse"));

#else
            var method = type.GetMethods().Where(x => x.Name.Equals("TryParse"));
#endif
            if (method.Count() > 0)
            {
                var methodinfo = method.FirstOrDefault();
                object[] args = { value.ToString(), null };
                if ((bool)methodinfo.Invoke(null, args))
                    return args[1];
                return null;
            }
            return null;
        }

#if WinRT
        public async void PasteTextToGridRow()
#else
        private void PasteTextToGridRow()
#endif
        {
#if WinRT
            DataPackageView dataPackage = null;
            dataPackage = Clipboard.GetContent();
            DataPackage package = new DataPackage();
            if (dataPackage.AvailableFormats.Count <= 0)
                return;
            package.SetText(await dataPackage.GetTextAsync());
            var args = this.dataGrid.RaisePasteContentEvent(new GridCopyPasteEventArgs(false, package, ClipBoardAction.Paste, this.dataGrid));
            if (!args.Handled)
            {
                if (args.DataPackage != null)
                    dataPackage = args.DataPackage.GetView();
            }
#elif WPF
            IDataObject dataObject = null;
            dataObject = Clipboard.GetDataObject();
            var args = this.dataGrid.RaisePasteContentEvent(new GridCopyPasteEventArgs(false, dataObject, ClipBoardAction.Paste, this.dataGrid));
            if (!args.Handled && this.dataGrid.SelectedItem!=null)
            {
                if (args.DataObject != null && this.dataGrid.SelectedItem != null)
                    dataObject = args.DataObject;
            }
#else
            string ClipBoardText = string.Empty;
            ClipBoardText = Clipboard.GetText();
            var args = this.dataGrid.RaisePasteContentEvent(new GridCopyPasteEventArgs(false, ClipBoardText, ClipBoardAction.Paste,this.dataGrid));
            if (!args.Handled)
            {
                if (args.ClipBoardText != string.Empty)
                    ClipBoardText = args.ClipBoardText;
            }
#endif
            else
                return;




            if ((dataGrid.GridCopyPasteOption & GridCopyPasteOption.PasteData) == GridCopyPasteOption.PasteData)
            {
#if WinRT
                if (dataPackage.Contains(StandardDataFormats.Text))
                {
                    var clipBoardContent = await dataPackage.GetTextAsync();
#elif WPF
                if (dataObject.GetDataPresent(DataFormats.UnicodeText))
                {
                    var clipBoardContent = dataObject.GetData(DataFormats.UnicodeText) as string;
#else
                if(!Clipboard.GetText().Equals(string.Empty))
                {
                    var clipBoardContent = ClipBoardText;
#endif
                    var records = clipBoardContent.Split('\n');
                    if ((dataGrid.GridCopyPasteOption & GridCopyPasteOption.IncludeHeaders) == GridCopyPasteOption.IncludeHeaders)
                        records = records.Skip(1).ToArray();
                    var selectedRecords = new List<object>();
                    var startIndex = 0;
                    var provider = this.dataGrid.View.GetPropertyAccessProvider();
                    var properyCollection = this.dataGrid.View.GetItemProperties();
                    if (this.dataGrid.View.GroupDescriptions.Count == 0)
                    {
                        records.ForEach(rec =>
                        {
                            if (!rec.Equals(string.Empty) && this.dataGrid.View.Records.IndexOfRecord(this.dataGrid.SelectedItem) + startIndex < this.dataGrid.View.Records.Count)
                                selectedRecords.Add(this.dataGrid.View.Records[this.dataGrid.View.Records.IndexOfRecord(this.dataGrid.SelectedItem) + startIndex].Data);
                            startIndex++;
                        });
                    }
                    else
                    {
                        records.ForEach(rec =>
                            {
                                var recordEntry = this.dataGrid.View.Records.GetRecord(this.dataGrid.SelectedItem);
                                var recordIndex = this.dataGrid.View.TopLevelGroup.DisplayElements.IndexOf(recordEntry);
                                if (!rec.Equals(string.Empty) && this.dataGrid.View.TopLevelGroup.DisplayElements.Count > (recordIndex + startIndex) && this.dataGrid.View.TopLevelGroup.DisplayElements[recordIndex +startIndex] is RecordEntry)
                                    selectedRecords.Add((this.dataGrid.View.TopLevelGroup.DisplayElements[recordIndex + startIndex] as RecordEntry).Data);
                                startIndex++;
                            });
                    }
                    startIndex = 0;
                    foreach (var record in records)
                    {
                        if (!record.Equals(string.Empty) && this.dataGrid.VisualContainer.ScrollRows.LineCount > this.dataGrid.SelectedIndex +startIndex && selectedRecords.Count > startIndex)
                        {
                            var rowData = selectedRecords[startIndex];
                            startIndex++;
                            var text = record.Split('\t');
                            foreach (var column in this.dataGrid.Columns)
                            {
                                Type type;
                                if (column.IsUnbound)
                                {
                                    var unBoundText = this.dataGrid.GetUnBoundCellValue(column, rowData).ToString();
                                    if (unBoundText == null)
                                        continue;
                                    type = unBoundText.GetType();
                                }
                                else
                                {
                                    var cellText = provider.GetValue(rowData, column.MappingName);
                                    if (cellText == null)
                                        continue;
                                    type = cellText.GetType();
                                }
#if !WPF
                                if (!column.IsUnbound && !properyCollection.Find(column.MappingName, false).CanWrite)
#else
                                if (!column.IsUnbound && properyCollection.Find(column.MappingName, false).IsReadOnly)                                    
#endif
                                    continue;


                                if (text.Length > this.dataGrid.Columns.IndexOf(column))
                                {
                                    if (ConvertToType(text[this.dataGrid.Columns.IndexOf(column)], type) != null || type == text[this.dataGrid.Columns.IndexOf(column)].GetType())
                                    {
#if !WinRT
                                        if (column is GridTimeSpanColumn)
                                        {
                                            TimeSpan value;
                                            TimeSpan.TryParse(text[this.dataGrid.Columns.IndexOf(column)], out value);
                                            provider.SetValue(rowData, column.MappingName, value);
                                        }
                                        else if (!(column is GridUnBoundColumn))
                                        {
#else
                                        if (!(column is GridUnBoundColumn))
                                        {
#endif
#if !SILVERLIGHT
                                            var value = Convert.ChangeType(text[this.dataGrid.Columns.IndexOf(column)], type);
#else
                                            var value = Convert.ChangeType(text[this.dataGrid.Columns.IndexOf(column)], type, null);
#endif
                                            provider.SetValue(rowData, column.MappingName, value);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    this.dataGrid.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.Paste, selectedRecords));
                }
            }

        }

        #endregion

        #region IGridCopyPaste Implementations

        public void Copy()
        {
            this.CopyTextToClipBoard(false);
        }

        public void Cut()
        {
            if ((this.dataGrid.GridCopyPasteOption & GridCopyPasteOption.CutData) == GridCopyPasteOption.CutData)
                this.CopyTextToClipBoard(true);
        }

        public void Paste()
        {
            this.PasteTextToGridRow();
        }

        #endregion


        public void Dispose()
        {
            this.dataGrid = null;
        }
    }

    public interface IGridCopyPaste
    {
        void Copy();
        void Cut();
        void Paste();
    }
}
