/**
 * DevExtreme (esm/ui/file_manager/ui.file_manager.dialog.folder_chooser.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import {
    extend
} from "../../core/utils/extend";
import messageLocalization from "../../localization/message";
import {
    getMapFromObject
} from "./ui.file_manager.common";
import FileManagerDialogBase from "./ui.file_manager.dialog";
import FileManagerFilesTreeView from "./ui.file_manager.files_tree_view";
var FILE_MANAGER_DIALOG_FOLDER_CHOOSER = "dx-filemanager-dialog-folder-chooser";
var FILE_MANAGER_DIALOG_FOLDER_CHOOSER_POPUP = "dx-filemanager-dialog-folder-chooser-popup";
class FileManagerFolderChooserDialog extends FileManagerDialogBase {
    show() {
        var _this$_filesTreeView;
        this._resetDialogSelectedDirectory();
        null === (_this$_filesTreeView = this._filesTreeView) || void 0 === _this$_filesTreeView ? void 0 : _this$_filesTreeView.refresh();
        super.show()
    }
    switchToCopyDialog(targetItemInfos) {
        this._targetItemInfos = targetItemInfos;
        this._setTitle(messageLocalization.format("dxFileManager-dialogDirectoryChooserCopyTitle"));
        this._setButtonText(messageLocalization.format("dxFileManager-dialogDirectoryChooserCopyButtonText"))
    }
    switchToMoveDialog(targetItemInfos) {
        this._targetItemInfos = targetItemInfos;
        this._setTitle(messageLocalization.format("dxFileManager-dialogDirectoryChooserMoveTitle"));
        this._setButtonText(messageLocalization.format("dxFileManager-dialogDirectoryChooserMoveButtonText"))
    }
    _getDialogOptions() {
        return extend(super._getDialogOptions(), {
            contentCssClass: FILE_MANAGER_DIALOG_FOLDER_CHOOSER,
            popupCssClass: FILE_MANAGER_DIALOG_FOLDER_CHOOSER_POPUP
        })
    }
    _createContentTemplate(element) {
        super._createContentTemplate(element);
        this._filesTreeView = this._createComponent($("<div>"), FileManagerFilesTreeView, {
            getDirectories: this.option("getDirectories"),
            getCurrentDirectory: () => this._getDialogSelectedDirectory(),
            onDirectoryClick: e => this._onFilesTreeViewDirectoryClick(e),
            onFilesTreeViewContentReady: () => this._toggleUnavailableLocationsDisabled(true)
        });
        this._$contentElement.append(this._filesTreeView.$element())
    }
    _getDialogResult() {
        var result = this._getDialogSelectedDirectory();
        return result ? {
            folder: result
        } : result
    }
    _getDefaultOptions() {
        return extend(super._getDefaultOptions(), {
            getItems: null
        })
    }
    _getDialogSelectedDirectory() {
        return this._selectedDirectoryInfo
    }
    _resetDialogSelectedDirectory() {
        this._selectedDirectoryInfo = null
    }
    _onFilesTreeViewDirectoryClick(_ref) {
        var {
            itemData: itemData
        } = _ref;
        this._selectedDirectoryInfo = itemData;
        this._filesTreeView.updateCurrentDirectory()
    }
    _onPopupShown() {
        this._toggleUnavailableLocationsDisabled(true);
        super._onPopupShown()
    }
    _onPopupHidden() {
        this._toggleUnavailableLocationsDisabled(false);
        super._onPopupHidden()
    }
    _toggleUnavailableLocationsDisabled(isDisabled) {
        if (!this._filesTreeView) {
            return
        }
        var locations = this._getLocationsToProcess(isDisabled);
        this._filesTreeView.toggleDirectoryExpandedStateRecursive(locations.locationsToExpand[0], isDisabled).then(() => this._filesTreeView.toggleDirectoryLineExpandedState(locations.locationsToCollapse, !isDisabled).then(() => locations.locationKeysToDisable.forEach(key => this._filesTreeView.toggleNodeDisabledState(key, isDisabled))))
    }
    _getLocationsToProcess(isDisabled) {
        var expandLocations = {};
        var collapseLocations = {};
        this._targetItemInfos.forEach(itemInfo => {
            if (itemInfo.parentDirectory) {
                expandLocations[itemInfo.parentDirectory.getInternalKey()] = itemInfo.parentDirectory
            }
            if (itemInfo.fileItem.isDirectory) {
                collapseLocations[itemInfo.getInternalKey()] = itemInfo
            }
        });
        var expandMap = getMapFromObject(expandLocations);
        var collapseMap = getMapFromObject(collapseLocations);
        return {
            locationsToExpand: isDisabled ? expandMap.values : [],
            locationsToCollapse: isDisabled ? collapseMap.values : [],
            locationKeysToDisable: expandMap.keys.concat(...collapseMap.keys)
        }
    }
}
export default FileManagerFolderChooserDialog;
