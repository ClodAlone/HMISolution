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

namespace DriverCodeBase.Extensions
{
    public static class TagEntityReferenceExtensions
    {
        public static UFUAModel.TagEntityReference Edit(this UFUAModel.TagEntityReference tag, System.Windows.Window owner)
        {
            IUFUAEditorManager Editor = null;
            IDocument Document = null;

            OPCUAViewModelComponent.QueryInterfaces();
            if (OPCUAViewModelComponent.ufuaEditorServiceAvailable &&
                OPCUAViewModelComponent.workspaceServiceAvailable)
            {
                Editor = OPCUAViewModelComponent.ufuaEditorService;
                Document = OPCUAViewModelComponent.workspaceService.ContextDocument;
            }

            OPCUAViewModelComponent.workspaceService.IsBusy = true;
            owner.Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                OPCUAViewModelComponent.workspaceService.IsBusy = false;
            });

            if (Editor != null && Document != null)
            {
                var ufuaeditor = Editor.GetAddressSpaceControl(Document);
                if (ufuaeditor == null || !(ufuaeditor is IAddressSpaceControl))
                    return null;
                (ufuaeditor as IAddressSpaceControl).SelectionType = SelectionType.TagEntityReference;
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
                    var editor = ufuaeditor as ISelectEntityReference;
                    if (editor != null && editor.SelectedReferences != null && editor.SelectedReferences.Count > 0)
                    {
                        var tagRef = editor.SelectedReferences.First();
                        return (tagRef as TagIdentifier)?.TagReference as UFUAModel.TagEntityReference;
                    }
                    return null;
                }
            }

            return null;
        }
    }
}
