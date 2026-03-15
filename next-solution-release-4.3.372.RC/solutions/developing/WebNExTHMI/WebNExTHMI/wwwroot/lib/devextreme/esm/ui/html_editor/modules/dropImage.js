/**
 * DevExtreme (esm/ui/html_editor/modules/dropImage.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Quill from "devextreme-quill";
import eventsEngine from "../../../events/core/events_engine";
import {
    addNamespace
} from "../../../events/utils/index";
import {
    each
} from "../../../core/utils/iterator";
import browser from "../../../core/utils/browser";
import {
    getWindow
} from "../../../core/utils/window";
import BaseModule from "./base";
var DropImageModule = BaseModule;
if (Quill) {
    DropImageModule = class extends BaseModule {
        constructor(quill, options) {
            super(quill, options);
            var widgetName = this.editorInstance.NAME;
            eventsEngine.on(this.quill.root, addNamespace("drop", widgetName), this._dropHandler.bind(this));
            eventsEngine.on(this.quill.root, addNamespace("paste", widgetName), this._pasteHandler.bind(this))
        }
        _dropHandler(e) {
            var _dataTransfer$files;
            var dataTransfer = e.originalEvent.dataTransfer;
            var hasFiles = null === dataTransfer || void 0 === dataTransfer ? void 0 : null === (_dataTransfer$files = dataTransfer.files) || void 0 === _dataTransfer$files ? void 0 : _dataTransfer$files.length;
            this.saveValueChangeEvent(e);
            e.preventDefault();
            if (hasFiles) {
                this._getImage(dataTransfer.files, this._addImage.bind(this))
            }
        }
        _pasteHandler(e) {
            var _clipboardData$items;
            var {
                clipboardData: clipboardData
            } = e.originalEvent;
            this.saveValueChangeEvent(e);
            if (!clipboardData) {
                return
            }
            var hasDataItems = null === (_clipboardData$items = clipboardData.items) || void 0 === _clipboardData$items ? void 0 : _clipboardData$items.length;
            var isHtmlData = clipboardData.getData("text/html");
            if (!isHtmlData && hasDataItems) {
                this._getImage(clipboardData.items, imageData => {
                    if (this._isBrowserSupportImagePaste(browser)) {
                        return
                    }
                    this._addImage(imageData)
                })
            }
        }
        _isBrowserSupportImagePaste(_ref) {
            var {
                mozilla: mozilla,
                chrome: chrome,
                version: version
            } = _ref;
            return mozilla || chrome && version > 82
        }
        _isImage(file) {
            return !!file.type.match(/^image\/(a?png|bmp|gif|p?jpe?g|svg|vnd\.microsoft\.icon|webp)/i)
        }
        _getImage(files, callback) {
            var window = getWindow();
            each(files, (index, file) => {
                if (!this._isImage(file)) {
                    return
                }
                var reader = new window.FileReader;
                reader.onload = _ref2 => {
                    var {
                        target: target
                    } = _ref2;
                    callback(target.result)
                };
                var readableFile = file.getAsFile ? file.getAsFile() : file;
                if (readableFile instanceof window.Blob) {
                    reader.readAsDataURL(readableFile)
                }
            })
        }
        _addImage(data) {
            var selection = this.quill.getSelection();
            var pasteIndex = selection ? selection.index : this.quill.getLength();
            this.quill.insertEmbed(pasteIndex, "extendedImage", data, "user")
        }
    }
}
export default DropImageModule;
