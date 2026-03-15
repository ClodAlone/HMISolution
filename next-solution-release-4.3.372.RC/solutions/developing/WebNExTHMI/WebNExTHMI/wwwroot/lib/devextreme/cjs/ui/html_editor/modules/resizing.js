/**
 * DevExtreme (cjs/ui/html_editor/modules/resizing.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _renderer = _interopRequireDefault(require("../../../core/renderer"));
var _events_engine = _interopRequireDefault(require("../../../events/core/events_engine"));
var _click = require("../../../events/click");
var _index = require("../../../events/utils/index");
var _translator = require("../../../animation/translator");
var _devices = _interopRequireDefault(require("../../../core/devices"));
var _resizable = _interopRequireDefault(require("../../resizable"));
var _position = require("../../../core/utils/position");
var _devextremeQuill = _interopRequireDefault(require("devextreme-quill"));
var _base = _interopRequireDefault(require("./base"));
var _size = require("../../../core/utils/size");

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
var DX_RESIZE_FRAME_CLASS = "dx-resize-frame";
var DX_TOUCH_DEVICE_CLASS = "dx-touch-device";
var MODULE_NAMESPACE = "dxHtmlResizingModule";
var KEYDOWN_EVENT = (0, _index.addNamespace)("keydown", MODULE_NAMESPACE);
var SCROLL_EVENT = (0, _index.addNamespace)("scroll", MODULE_NAMESPACE);
var MOUSEDOWN_EVENT = (0, _index.addNamespace)("mousedown", MODULE_NAMESPACE);
var FRAME_PADDING = 1;
var ResizingModule = function(_BaseModule) {
    _inheritsLoose(ResizingModule, _BaseModule);

    function ResizingModule(quill, options) {
        var _this;
        _this = _BaseModule.call(this, quill, options) || this;
        _this.allowedTargets = options.allowedTargets || ["image"];
        _this.enabled = !!options.enabled;
        _this._hideFrameWithContext = _this.hideFrame.bind(_assertThisInitialized(_this));
        _this._framePositionChangedHandler = _this._prepareFramePositionChangedHandler();
        if (_this.enabled) {
            _this._attachEvents();
            _this._createResizeFrame()
        }
        return _this
    }
    var _proto = ResizingModule.prototype;
    _proto._attachEvents = function() {
        _events_engine.default.on(this.quill.root, (0, _index.addNamespace)(_click.name, MODULE_NAMESPACE), this._clickHandler.bind(this));
        _events_engine.default.on(this.quill.root, SCROLL_EVENT, this._framePositionChangedHandler);
        this.editorInstance.on("focusOut", this._hideFrameWithContext);
        this.quill.on("text-change", this._framePositionChangedHandler)
    };
    _proto._detachEvents = function() {
        _events_engine.default.off(this.quill.root, MODULE_NAMESPACE);
        this.editorInstance.off("focusOut", this._hideFrameWithContext);
        this.quill.off("text-change", this._framePositionChangedHandler)
    };
    _proto._clickHandler = function(e) {
        if (this._isAllowedTarget(e.target)) {
            if (this._$target === e.target) {
                return
            }
            this._$target = e.target;
            var $target = (0, _renderer.default)(this._$target);
            var minWidth = Math.max((0, _size.getOuterWidth)($target) - (0, _size.getWidth)($target), this.resizable.option("minWidth"));
            var minHeight = Math.max((0, _size.getOuterHeight)($target) - (0, _size.getHeight)($target), this.resizable.option("minHeight"));
            this.resizable.option({
                minWidth: minWidth,
                minHeight: minHeight
            });
            this.updateFramePosition();
            this.showFrame();
            this._adjustSelection()
        } else if (this._$target) {
            this.hideFrame()
        }
    };
    _proto._prepareFramePositionChangedHandler = function(e) {
        var _this2 = this;
        return function() {
            if (_this2._$target) {
                _this2.updateFramePosition()
            }
        }
    };
    _proto._adjustSelection = function() {
        if (!this.quill.getSelection()) {
            this.quill.setSelection(0, 0)
        }
    };
    _proto._isAllowedTarget = function(targetElement) {
        return this._isImage(targetElement)
    };
    _proto._isImage = function(targetElement) {
        return -1 !== this.allowedTargets.indexOf("image") && "IMG" === targetElement.tagName.toUpperCase()
    };
    _proto.showFrame = function() {
        this._$resizeFrame.show();
        _events_engine.default.on(this.quill.root, KEYDOWN_EVENT, this._handleFrameKeyDown.bind(this))
    };
    _proto._handleFrameKeyDown = function(e) {
        var keyName = (0, _index.normalizeKeyName)(e);
        if ("del" === keyName || "backspace" === keyName) {
            this._deleteImage()
        }
        this.hideFrame()
    };
    _proto.hideFrame = function() {
        this._$target = null;
        this._$resizeFrame.hide();
        _events_engine.default.off(this.quill.root, KEYDOWN_EVENT)
    };
    _proto.updateFramePosition = function() {
        var _getBoundingRect = (0, _position.getBoundingRect)(this._$target),
            height = _getBoundingRect.height,
            width = _getBoundingRect.width,
            targetTop = _getBoundingRect.top,
            targetLeft = _getBoundingRect.left;
        var _getBoundingRect2 = (0, _position.getBoundingRect)(this.quill.root),
            containerTop = _getBoundingRect2.top,
            containerLeft = _getBoundingRect2.left;
        var borderWidth = this._getBorderWidth();
        this._$resizeFrame.css({
            height: height,
            width: width,
            padding: FRAME_PADDING,
            top: targetTop - containerTop - borderWidth - FRAME_PADDING,
            left: targetLeft - containerLeft - borderWidth - FRAME_PADDING
        });
        (0, _translator.move)(this._$resizeFrame, {
            left: 0,
            top: 0
        })
    };
    _proto._getBorderWidth = function() {
        return parseInt(this._$resizeFrame.css("borderTopWidth"))
    };
    _proto._createResizeFrame = function() {
        var _this3 = this;
        if (this._$resizeFrame) {
            return
        }
        var _devices$current = _devices.default.current(),
            deviceType = _devices$current.deviceType;
        this._$resizeFrame = (0, _renderer.default)("<div>").addClass(DX_RESIZE_FRAME_CLASS).toggleClass(DX_TOUCH_DEVICE_CLASS, "desktop" !== deviceType).appendTo(this.editorInstance._getQuillContainer()).hide();
        _events_engine.default.on(this._$resizeFrame, MOUSEDOWN_EVENT, (function(e) {
            e.preventDefault()
        }));
        this.resizable = this.editorInstance._createComponent(this._$resizeFrame, _resizable.default, {
            _keepAspectRatio: true,
            onResize: function(e) {
                if (!_this3._$target) {
                    return
                }(0, _renderer.default)(_this3._$target).attr({
                    height: e.height,
                    width: e.width
                });
                _this3.updateFramePosition()
            }
        })
    };
    _proto._deleteImage = function() {
        if (this._isAllowedTarget(this._$target)) {
            var _Quill$find;
            null === (_Quill$find = _devextremeQuill.default.find(this._$target)) || void 0 === _Quill$find ? void 0 : _Quill$find.deleteAt(0)
        }
    };
    _proto.option = function(_option, value) {
        if ("mediaResizing" === _option) {
            this.handleOptionChangeValue(value);
            return
        }
        if ("enabled" === _option) {
            this.enabled = value;
            value ? this._attachEvents() : this._detachEvents()
        } else if ("allowedTargets" === _option && Array.isArray(value)) {
            this.allowedTargets = value
        }
    };
    _proto.clean = function() {
        this._detachEvents();
        this._$resizeFrame.remove();
        this._$resizeFrame = void 0
    };
    return ResizingModule
}(_base.default);
exports.default = ResizingModule;
module.exports = exports.default;
module.exports.default = exports.default;
