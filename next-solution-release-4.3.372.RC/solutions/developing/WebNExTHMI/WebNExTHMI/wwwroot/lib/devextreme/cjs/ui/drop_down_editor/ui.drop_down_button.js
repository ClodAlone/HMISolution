/**
 * DevExtreme (cjs/ui/drop_down_editor/ui.drop_down_button.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _extend = require("../../core/utils/extend");
var _events_engine = _interopRequireDefault(require("../../events/core/events_engine"));
var _message = _interopRequireDefault(require("../../localization/message"));
var _button = _interopRequireDefault(require("../text_box/texteditor_button_collection/button"));
var _button2 = _interopRequireDefault(require("../button"));

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
var DROP_DOWN_EDITOR_BUTTON_CLASS = "dx-dropdowneditor-button";
var DROP_DOWN_EDITOR_BUTTON_VISIBLE = "dx-dropdowneditor-button-visible";
var BUTTON_MESSAGE = "dxDropDownEditor-selectLabel";
var DropDownButton = function(_TextEditorButton) {
    _inheritsLoose(DropDownButton, _TextEditorButton);

    function DropDownButton(name, editor, options) {
        var _this;
        _this = _TextEditorButton.call(this, name, editor, options) || this;
        _this.currentTemplate = null;
        return _this
    }
    var _proto = DropDownButton.prototype;
    _proto._attachEvents = function(instance) {
        var editor = this.editor;
        instance.option("onClick", (function(e) {
            !editor.option("openOnFieldClick") && editor._openHandler(e)
        }));
        _events_engine.default.on(instance.$element(), "mousedown", (function(e) {
            if (editor.$element().is(".dx-state-focused")) {
                e.preventDefault()
            }
        }))
    };
    _proto._create = function() {
        var editor = this.editor;
        var $element = (0, _renderer.default)("<div>");
        var options = this._getOptions();
        this._addToContainer($element);
        var instance = editor._createComponent($element, _button2.default, (0, _extend.extend)({}, options, {
            elementAttr: {
                "aria-label": _message.default.format(BUTTON_MESSAGE)
            }
        }));
        this._legacyRender(editor.$element(), $element, options.visible);
        return {
            $element: $element,
            instance: instance
        }
    };
    _proto._getOptions = function() {
        var editor = this.editor;
        var visible = this._isVisible();
        var isReadOnly = editor.option("readOnly");
        var options = {
            focusStateEnabled: false,
            hoverStateEnabled: false,
            activeStateEnabled: false,
            useInkRipple: false,
            disabled: isReadOnly,
            visible: visible
        };
        this._addTemplate(options);
        return options
    };
    _proto._isVisible = function() {
        var editor = this.editor;
        return _TextEditorButton.prototype._isVisible.call(this) && editor.option("showDropDownButton")
    };
    _proto._legacyRender = function($editor, $element, isVisible) {
        $editor.toggleClass(DROP_DOWN_EDITOR_BUTTON_VISIBLE, isVisible);
        if ($element) {
            $element.removeClass("dx-button").addClass(DROP_DOWN_EDITOR_BUTTON_CLASS)
        }
    };
    _proto._isSameTemplate = function() {
        return this.editor.option("dropDownButtonTemplate") === this.currentTemplate
    };
    _proto._addTemplate = function(options) {
        if (!this._isSameTemplate()) {
            options.template = this.editor._getTemplateByOption("dropDownButtonTemplate");
            this.currentTemplate = this.editor.option("dropDownButtonTemplate")
        }
    };
    _proto.update = function() {
        var shouldUpdate = _TextEditorButton.prototype.update.call(this);
        if (shouldUpdate) {
            var editor = this.editor,
                instance = this.instance;
            var $editor = editor.$element();
            var options = this._getOptions();
            null === instance || void 0 === instance ? void 0 : instance.option(options);
            this._legacyRender($editor, null === instance || void 0 === instance ? void 0 : instance.$element(), options.visible)
        }
    };
    return DropDownButton
}(_button.default);
exports.default = DropDownButton;
module.exports = exports.default;
module.exports.default = exports.default;
