/**
 * DevExtreme (esm/ui/file_manager/ui.file_manager.dialog.delete_item.js)
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
import ScrollView from "../scroll_view";
import FileManagerDialogBase from "./ui.file_manager.dialog";
var FILE_MANAGER_DIALOG_DELETE_ITEM = "dx-filemanager-dialog-delete-item";
var FILE_MANAGER_DIALOG_DELETE_ITEM_POPUP = "dx-filemanager-dialog-delete-item-popup";
class FileManagerDeleteItemDialog extends FileManagerDialogBase {
    show(_ref) {
        var {
            itemName: itemName,
            itemCount: itemCount
        } = _ref;
        var text = 1 === itemCount ? messageLocalization.format("dxFileManager-dialogDeleteItemSingleItemConfirmation", itemName) : messageLocalization.format("dxFileManager-dialogDeleteItemMultipleItemsConfirmation", itemCount);
        if (this._$text) {
            this._$text.text(text)
        } else {
            this._initialText = text
        }
        super.show()
    }
    _getDialogOptions() {
        return extend(super._getDialogOptions(), {
            title: messageLocalization.format("dxFileManager-dialogDeleteItemTitle"),
            buttonText: messageLocalization.format("dxFileManager-dialogDeleteItemButtonText"),
            contentCssClass: FILE_MANAGER_DIALOG_DELETE_ITEM,
            popupCssClass: FILE_MANAGER_DIALOG_DELETE_ITEM_POPUP,
            height: "auto",
            maxHeight: "80vh"
        })
    }
    _createContentTemplate(element) {
        super._createContentTemplate(element);
        this._$text = $("<div>").text(this._initialText).appendTo(this._$contentElement);
        this._createComponent(this._$contentElement, ScrollView, {
            width: "100%",
            height: "100%"
        })
    }
    _getDialogResult() {
        return {}
    }
}
export default FileManagerDeleteItemDialog;
