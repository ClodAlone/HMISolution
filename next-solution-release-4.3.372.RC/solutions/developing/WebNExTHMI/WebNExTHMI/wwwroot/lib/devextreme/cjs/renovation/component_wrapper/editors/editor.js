/**
 * DevExtreme (cjs/renovation/component_wrapper/editors/editor.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _type = require("../../../core/utils/type");
var _component = _interopRequireDefault(require("../common/component"));
var _validation_engine = _interopRequireDefault(require("../../../ui/validation_engine"));
var _extend = require("../../../core/utils/extend");
var _renderer = _interopRequireDefault(require("../../../core/renderer"));
var _element_data = require("../../../core/element_data");
var _callbacks = _interopRequireDefault(require("../../../core/utils/callbacks"));
var _editor = _interopRequireDefault(require("../../../ui/editor/editor"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
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
var INVALID_MESSAGE_AUTO = "dx-invalid-message-auto";
var VALIDATION_TARGET = "dx-validation-target";
var Editor = function(_Component) {
    _inheritsLoose(Editor, _Component);

    function Editor() {
        return _Component.apply(this, arguments) || this
    }
    var _proto = Editor.prototype;
    _proto.getProps = function() {
        var _this = this;
        var props = _Component.prototype.getProps.call(this);
        props.onFocusIn = function() {
            var isValidationMessageShownOnFocus = "auto" === _this.option("validationMessageMode");
            if (isValidationMessageShownOnFocus) {
                var $validationMessageWrapper = (0, _renderer.default)(".dx-invalid-message.dx-overlay-wrapper");
                null === $validationMessageWrapper || void 0 === $validationMessageWrapper ? void 0 : $validationMessageWrapper.removeClass(INVALID_MESSAGE_AUTO);
                if (_this.showValidationMessageTimeout) {
                    clearTimeout(_this.showValidationMessageTimeout)
                }
                _this.showValidationMessageTimeout = setTimeout((function() {
                    null === $validationMessageWrapper || void 0 === $validationMessageWrapper ? void 0 : $validationMessageWrapper.addClass(INVALID_MESSAGE_AUTO)
                }), 150)
            }
        };
        props.saveValueChangeEvent = function(e) {
            _this._valueChangeEventInstance = e
        };
        return props
    };
    _proto._createElement = function(element) {
        _Component.prototype._createElement.call(this, element);
        this.showValidationMessageTimeout = void 0;
        this.validationRequest = (0, _callbacks.default)();
        (0, _element_data.data)(this.$element()[0], VALIDATION_TARGET, this)
    };
    _proto._render = function() {
        var _this$option;
        null === (_this$option = this.option("_onMarkupRendered")) || void 0 === _this$option ? void 0 : _this$option()
    };
    _proto._initializeComponent = function() {
        _Component.prototype._initializeComponent.call(this);
        this._valueChangeAction = this._createActionByOption("onValueChanged", {
            excludeValidators: ["disabled", "readOnly"]
        })
    };
    _proto._initOptions = function(options) {
        _Component.prototype._initOptions.call(this, options);
        this.option(_validation_engine.default.initValidationOptions(options))
    };
    _proto._getDefaultOptions = function() {
        return (0, _extend.extend)(_Component.prototype._getDefaultOptions.call(this), {
            validationMessageOffset: {
                h: 0,
                v: 0
            },
            validationTooltipOptions: {}
        })
    };
    _proto._bindInnerWidgetOptions = function(innerWidget, optionsContainer) {
        var _this2 = this;
        var innerWidgetOptions = (0, _extend.extend)({}, innerWidget.option());
        var syncOptions = function() {
            return _this2._silent(optionsContainer, innerWidgetOptions)
        };
        syncOptions();
        innerWidget.on("optionChanged", syncOptions)
    };
    _proto._raiseValidation = function(value, previousValue) {
        var areValuesEmpty = !(0, _type.isDefined)(value) && !(0, _type.isDefined)(previousValue);
        if (value !== previousValue && !areValuesEmpty) {
            this.validationRequest.fire({
                value: value,
                editor: this
            })
        }
    };
    _proto._raiseValueChangeAction = function(value, previousValue) {
        var _this$_valueChangeAct;
        null === (_this$_valueChangeAct = this._valueChangeAction) || void 0 === _this$_valueChangeAct ? void 0 : _this$_valueChangeAct.call(this, {
            element: this.$element(),
            previousValue: previousValue,
            value: value,
            event: this._valueChangeEventInstance
        });
        this._valueChangeEventInstance = void 0
    };
    _proto._optionChanged = function(option) {
        var name = option.name,
            previousValue = option.previousValue,
            value = option.value;
        if (name && void 0 !== this._getActionConfigs()[name]) {
            this._addAction(name)
        }
        switch (name) {
            case "value":
                this._raiseValidation(value, previousValue);
                this._raiseValueChangeAction(value, previousValue);
                break;
            case "onValueChanged":
                this._valueChangeAction = this._createActionByOption("onValueChanged", {
                    excludeValidators: ["disabled", "readOnly"]
                });
                break;
            case "isValid":
            case "validationError":
            case "validationErrors":
            case "validationStatus":
                this.option(_validation_engine.default.synchronizeValidationOptions(option, this.option()))
        }
        _Component.prototype._optionChanged.call(this, option)
    };
    _proto.reset = function() {
        var _this$_getDefaultOpti = this._getDefaultOptions(),
            value = _this$_getDefaultOpti.value;
        this.option({
            value: value
        })
    };
    _proto._dispose = function() {
        _Component.prototype._dispose.call(this);
        (0, _element_data.data)(this.element(), VALIDATION_TARGET, null);
        if (this.showValidationMessageTimeout) {
            clearTimeout(this.showValidationMessageTimeout)
        }
    };
    return Editor
}(_component.default);
exports.default = Editor;
var prevIsEditor = _editor.default.isEditor;
var newIsEditor = function(instance) {
    return prevIsEditor(instance) || instance instanceof Editor
};
Editor.isEditor = newIsEditor;
_editor.default.isEditor = newIsEditor;
module.exports = exports.default;
module.exports.default = exports.default;
