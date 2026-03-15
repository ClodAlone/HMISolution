using DocumentManager.ComponentService;
using OPCUAViewModelService.ComponentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using UFInterfaces.Editors;
using UFUAEditor.ComponentService;
using Utilities;

namespace UFUAEditor.Extensions
{
    internal static class TagEntityReferenceExtensions
    {
        public static List<TagIdentifier> Edit(this UFUAModel.TagEntityReference tag, Window owner, SelectionMode selectionMode = SelectionMode.SingleRow, FilterType filterType = FilterType.None, TargetType targetType = TargetType.None, bool bPrototypesTab = false)
        {
            IDocument Document = UFUAEditorManagerComponent.ufuaEditorManagerComponent.Workspace.ContextDocument;
            if (Document != null)
            {
                var dispatcher = owner?.Dispatcher;
                if (dispatcher != null)
                {
                    UFUAEditorManagerComponent.ufuaEditorManagerComponent.Workspace.IsBusy = true;
                    dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        UFUAEditorManagerComponent.ufuaEditorManagerComponent.Workspace.IsBusy = false;
                    });
                }

                var ufuaeditor = UFUAEditorManagerComponent.ufuaEditorManagerComponent.GetAddressSpaceControl(Document, selectionType: SelectionType.TagEntityReference, selectionMode: selectionMode, filterType: filterType, targetType: targetType, bPrototypesTab: bPrototypesTab, bRefreshAS: false, bNewControl: false);
                if (ufuaeditor == null)
                    return null;
                ufuaeditor.DataContext = tag;

                ufuaeditor.ClearValue(FrameworkElement.WidthProperty);
                ufuaeditor.ClearValue(FrameworkElement.HeightProperty);

                GeneralDialogContent wnd = new GeneralDialogContent(ufuaeditor)
                {
                    DialogKeepContent = true,
                    Title = Properties.Resources.BrowseForAnyTypeTitle,
                    Owner = owner,
                    HelpLink = "TagEditor"
                };

                if (wnd.ShowDialog() == true && ufuaeditor != null) 
                {
                    var refList = new List<TagIdentifier>();
                    var editor = ufuaeditor as ISelectEntityReference ?? ((ufuaeditor.Content as DevExpress.Xpf.Core.DXTabControl)?.SelectedItem as DevExpress.Xpf.Core.DXTabItem)?.Content as ISelectEntityReference;
                    if (editor != null)
                    {
                        if (editor.SelectedReferences != null && editor.SelectedReferences.Count > 0)
                        {
                            foreach (var tagRef in editor.SelectedReferences)
                            {
                                if (tagRef as TagIdentifier != null)
                                    refList.Add((TagIdentifier)tagRef);
                                else if (tagRef is UFUAModel.TagEntityReference)
                                    refList.Add(new TagIdentifier(tagRef as UFUAModel.TagEntityReference));
                            }
                        }
                        else if (editor.SelectedReference != null && editor.SelectedReference is UFUAModel.TagEntityReference)
                            refList.Add(new TagIdentifier((UFUAModel.TagEntityReference)editor.SelectedReference));
                    }
                    return refList.Count > 0 ? refList : null;
                }
            }

            return null;
        }
    }
}
