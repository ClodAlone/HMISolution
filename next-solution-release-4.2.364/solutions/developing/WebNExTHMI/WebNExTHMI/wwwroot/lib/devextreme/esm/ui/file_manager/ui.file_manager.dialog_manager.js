/**
 * DevExtreme (esm/ui/file_manager/ui.file_manager.dialog_manager.js)
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
import FileManagerNameEditorDialog from "./ui.file_manager.dialog.name_editor";
import FileManagerFolderChooserDialog from "./ui.file_manager.dialog.folder_chooser";
import FileManagerDeleteItemDialog from "./ui.file_manager.dialog.delete_item";
class FileManagerDialogManager {
    constructor($element, options) {
        this._$element = $element;
        this._options = options;
        var baseDialogOptions = {
            onClosed: this._options.onDialogClosed,
            rtlEnabled: this._options.rtlEnabled
        };
        var $chooseFolderDialog = $("<div>").appendTo(this._$element);
        this._chooseDirectoryDialog = new FileManagerFolderChooserDialog($chooseFolderDialog, extend(baseDialogOptions, this._options.chooseDirectoryDialog));
        var $renameDialog = $("<div>").appendTo(this._$element);
        this._renameItemDialog = new FileManagerNameEditorDialog($renameDialog, extend(baseDialogOptions, {
            title: messageLocalization.format("dxFileManager-dialogRenameItemTitle"),
            buttonText: messageLocalization.format("dxFileManager-dialogRenameItemButtonText")
        }));
        var $createDialog = $("<div>").appendTo(this._$element);
        this._createItemDialog = new FileManagerNameEditorDialog($createDialog, extend(baseDialogOptions, {
            title: messageLocalization.format("dxFileManager-dialogCreateDirectoryTitle"),
            buttonText: messageLocalization.format("dxFileManager-dialogCreateDirectoryButtonText")
        }));
        var $deleteItemDialog = $("<div>").appendTo(this._$element);
        this._deleteItemDialog = new FileManagerDeleteItemDialog($deleteItemDialog, baseDialogOptions)
    }
    getCopyDialog(targetItemInfos) {
        this._chooseDirectoryDialog.switchToCopyDialog(targetItemInfos);
        return this._chooseDirectoryDialog
    }
    getMoveDialog(targetItemInfos) {
        this._chooseDirectoryDialog.switchToMoveDialog(targetItemInfos);
        return this._chooseDirectoryDialog
    }
    getRenameItemDialog() {
        return this._renameItemDialog
    }
    getCreateItemDialog() {
        return this._createItemDialog
    }
    getDeleteItemDialog() {
        return this._deleteItemDialog
    }
    updateDialogRtl(value) {
        [this._chooseDirectoryDialog, this._renameItemDialog, this._createItemDialog, this._deleteItemDialog].forEach(dialog => {
            dialog.option("rtlEnabled", value)
        })
    }
}
export default FileManagerDialogManager;
