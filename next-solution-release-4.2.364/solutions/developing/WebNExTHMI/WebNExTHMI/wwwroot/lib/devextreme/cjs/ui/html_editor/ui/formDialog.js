/**
 * DevExtreme (cjs/ui/html_editor/ui/formDialog.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _renderer = _interopRequireDefault(require("../../../core/renderer"));
var _extend = require("../../../core/utils/extend");
var _popup = _interopRequireDefault(require("../../popup"));
var _form = _interopRequireDefault(require("../../form"));
var _deferred = require("../../../core/utils/deferred");
var _message = _interopRequireDefault(require("../../../localization/message"));
var _window = require("../../../core/utils/window");
var _devices = _interopRequireDefault(require("../../../core/devices"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var DIALOG_CLASS = "dx-formdialog";
var FORM_CLASS = "dx-formdialog-form";
var FormDialog = function() {
    function FormDialog(editorInstance, popupConfig) {
        this._editorInstance = editorInstance;
        this._popupUserConfig = popupConfig;
        this._renderPopup();
        this._attachOptionChangedHandler()
    }
    var _proto = FormDialog.prototype;
    _proto._renderPopup = function() {
        var editorInstance = this._editorInstance;
        var $container = (0, _renderer.default)("<div>").addClass(DIALOG_CLASS).appendTo(editorInstance.$element());
        var popupConfig = this._getPopupConfig();
        return editorInstance._createComponent($container, _popup.default, popupConfig)
    };
    _proto._attachOptionChangedHandler = function() {
        var _this$_popup, _this = this;
        null === (_this$_popup = this._popup) || void 0 === _this$_popup ? void 0 : _this$_popup.on("optionChanged", (function(_ref) {
            var name = _ref.name,
                value = _ref.value;
            if ("title" === name) {
                _this._updateFormLabel(value)
            }
        }))
    };
    _proto._escKeyHandler = function() {
        this._popup.hide()
    };
    _proto._addEscapeHandler = function(e) {
        e.component.registerKeyHandler("escape", this._escKeyHandler.bind(this))
    };
    _proto._isSmallScreen = function() {
        var screenFactor = (0, _window.hasWindow)() ? (0, _window.getCurrentScreenFactor)() : null;
        return "phone" === _devices.default.real().deviceType || "xs" === screenFactor
    };
    _proto._getPopupConfig = function() {
        var _this2 = this;
        return (0, _extend.extend)({
            onInitialized: function(e) {
                _this2._popup = e.component;
                _this2._popup.on("hiding", (function() {
                    _this2.deferred.reject()
                }));
                _this2._popup.on("shown", (function() {
                    _this2._form.focus()
                }))
            },
            deferRendering: false,
            focusStateEnabled: false,
            showCloseButton: false,
            wrapperAttr: {
                class: "dx-formdialog"
            },
            fullScreen: this._isSmallScreen(),
            contentTemplate: function(contentElem) {
                var $formContainer = (0, _renderer.default)("<div>").appendTo(contentElem);
                _this2._renderForm($formContainer, {
                    onEditorEnterKey: function(_ref2) {
                        var component = _ref2.component,
                            event = (_ref2.dataField, _ref2.event);
                        _this2.hide(component.option("formData"), event)
                    },
                    customizeItem: function(item) {
                        if ("simple" === item.itemType) {
                            item.editorOptions = (0, _extend.extend)(true, {}, item.editorOptions, {
                                onInitialized: _this2._addEscapeHandler.bind(_this2)
                            })
                        }
                    }
                })
            },
            toolbarItems: [{
                toolbar: "bottom",
                location: "after",
                widget: "dxButton",
                options: {
                    onInitialized: this._addEscapeHandler.bind(this),
                    text: _message.default.format("OK"),
                    onClick: function(_ref3) {
                        var event = _ref3.event;
                        _this2.hide(_this2._form.option("formData"), event)
                    }
                }
            }, {
                toolbar: "bottom",
                location: "after",
                widget: "dxButton",
                options: {
                    onInitialized: this._addEscapeHandler.bind(this),
                    text: _message.default.format("Cancel"),
                    onClick: function() {
                        _this2._popup.hide()
                    }
                }
            }]
        }, this._popupUserConfig)
    };
    _proto._renderForm = function($container, options) {
        $container.addClass(FORM_CLASS);
        this._form = this._editorInstance._createComponent($container, _form.default, options);
        this._updateFormLabel()
    };
    _proto._updateFormLabel = function(text) {
        var _this$_form;
        var label = null !== text && void 0 !== text ? text : this.popupOption("title");
        null === (_this$_form = this._form) || void 0 === _this$_form ? void 0 : _this$_form.$element().attr("aria-label", label)
    };
    _proto.show = function(formUserConfig) {
        if (this._popup.option("visible")) {
            return
        }
        this.deferred = new _deferred.Deferred;
        var formConfig = (0, _extend.extend)({}, formUserConfig);
        this._form.option(formConfig);
        this._popup.show();
        return this.deferred.promise()
    };
    _proto.hide = function(formData, event) {
        this.deferred.resolve(formData, event);
        this._popup.hide()
    };
    _proto.popupOption = function(optionName, optionValue) {
        return this._popup.option.apply(this._popup, arguments)
    };
    return FormDialog
}();
var _default = FormDialog;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
