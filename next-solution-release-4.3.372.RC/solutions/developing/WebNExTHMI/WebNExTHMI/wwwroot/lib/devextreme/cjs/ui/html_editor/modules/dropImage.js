/**
 * DevExtreme (cjs/ui/html_editor/modules/dropImage.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _devextremeQuill = _interopRequireDefault(require("devextreme-quill"));
var _events_engine = _interopRequireDefault(require("../../../events/core/events_engine"));
var _index = require("../../../events/utils/index");
var _iterator = require("../../../core/utils/iterator");
var _browser = _interopRequireDefault(require("../../../core/utils/browser"));
var _window = require("../../../core/utils/window");
var _base = _interopRequireDefault(require("./base"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _assertThisInitialized(self) {
    if (void 0 === self) {
        throw new ReferenceError("this hasn't been initialised - super() hasn't been called")
    }
    return self
}

function _inheritsLoose(subClass, superClass) {
    subClass.prototype = Object.create(superClass.prototype);
    subClass.prototype.constructor = subClass;
    _setPrototypeOf(subClass, superClass)
}

function _setPrototypeOf(o, p) {
    _setPrototypeOf = Object.setPrototypeOf || function(o, p) {
        o.__proto__ = p;
        return o
    };
    return _setPrototypeOf(o, p)
}
var DropImageModule = _base.default;
if (_devextremeQuill.default) {
    DropImageModule = function(_BaseModule) {
        _inheritsLoose(DropImageModule, _BaseModule);

        function DropImageModule(quill, options) {
            var _this;
            _this = _BaseModule.call(this, quill, options) || this;
            var widgetName = _this.editorInstance.NAME;
            _events_engine.default.on(_this.quill.root, (0, _index.addNamespace)("drop", widgetName), _this._dropHandler.bind(_assertThisInitialized(_this)));
            _events_engine.default.on(_this.quill.root, (0, _index.addNamespace)("paste", widgetName), _this._pasteHandler.bind(_assertThisInitialized(_this)));
            return _this
        }
        var _proto = DropImageModule.prototype;
        _proto._dropHandler = function(e) {
            var _dataTransfer$files;
            var dataTransfer = e.originalEvent.dataTransfer;
            var hasFiles = null === dataTransfer || void 0 === dataTransfer ? void 0 : null === (_dataTransfer$files = dataTransfer.files) || void 0 === _dataTransfer$files ? void 0 : _dataTransfer$files.length;
            this.saveValueChangeEvent(e);
            e.preventDefault();
            if (hasFiles) {
                this._getImage(dataTransfer.files, this._addImage.bind(this))
            }
        };
        _proto._pasteHandler = function(e) {
            var _clipboardData$items, _this2 = this;
            var clipboardData = e.originalEvent.clipboardData;
            this.saveValueChangeEvent(e);
            if (!clipboardData) {
                return
            }
            var hasDataItems = null === (_clipboardData$items = clipboardData.items) || void 0 === _clipboardData$items ? void 0 : _clipboardData$items.length;
            var isHtmlData = clipboardData.getData("text/html");
            if (!isHtmlData && hasDataItems) {
                this._getImage(clipboardData.items, (function(imageData) {
                    if (_this2._isBrowserSupportImagePaste(_browser.default)) {
                        return
                    }
                    _this2._addImage(imageData)
                }))
            }
        };
        _proto._isBrowserSupportImagePaste = function(_ref) {
            var mozilla = _ref.mozilla,
                chrome = _ref.chrome,
                version = _ref.version;
            return mozilla || chrome && version > 82
        };
        _proto._isImage = function(file) {
            return !!file.type.match(/^image\/(a?png|bmp|gif|p?jpe?g|svg|vnd\.microsoft\.icon|webp)/i)
        };
        _proto._getImage = function(files, callback) {
            var _this3 = this;
            var window = (0, _window.getWindow)();
            (0, _iterator.each)(files, (function(index, file) {
                if (!_this3._isImage(file)) {
                    return
                }
                var reader = new window.FileReader;
                reader.onload = function(_ref2) {
                    var target = _ref2.target;
                    callback(target.result)
                };
                var readableFile = file.getAsFile ? file.getAsFile() : file;
                if (readableFile instanceof window.Blob) {
                    reader.readAsDataURL(readableFile)
                }
            }))
        };
        _proto._addImage = function(data) {
            var selection = this.quill.getSelection();
            var pasteIndex = selection ? selection.index : this.quill.getLength();
            this.quill.insertEmbed(pasteIndex, "extendedImage", data, "user")
        };
        return DropImageModule
    }(_base.default)
}
var _default = DropImageModule;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
