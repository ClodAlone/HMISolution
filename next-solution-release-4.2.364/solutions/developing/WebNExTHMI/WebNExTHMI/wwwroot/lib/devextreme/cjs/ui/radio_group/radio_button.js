/**
 * DevExtreme (cjs/ui/radio_group/radio_button.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _events_engine = _interopRequireDefault(require("../../events/core/events_engine"));
var _devices = _interopRequireDefault(require("../../core/devices"));
var _extend = require("../../core/utils/extend");
var _component_registrator = _interopRequireDefault(require("../../core/component_registrator"));
var _editor = _interopRequireDefault(require("../editor/editor"));
var _index = require("../../events/utils/index");
var _click = require("../../events/click");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var RADIO_BUTTON_CLASS = "dx-radiobutton";
var RADIO_BUTTON_ICON_CLASS = "dx-radiobutton-icon";
var RADIO_BUTTON_ICON_DOT_CLASS = "dx-radiobutton-icon-dot";
var RADIO_BUTTON_CHECKED_CLASS = "dx-radiobutton-checked";
var RADIO_BUTTON_ICON_CHECKED_CLASS = "dx-radiobutton-icon-checked";
var RadioButton = _editor.default.inherit({
    _supportedKeys: function() {
        return (0, _extend.extend)(this.callBase(), {
            space: function(e) {
                e.preventDefault();
                this._clickAction({
                    event: e
                })
            }
        })
    },
    _getDefaultOptions: function() {
        return (0, _extend.extend)(this.callBase(), {
            hoverStateEnabled: true,
            activeStateEnabled: true,
            value: false
        })
    },
    _canValueBeChangedByClick: function() {
        return true
    },
    _defaultOptionsRules: function() {
        return this.callBase().concat([{
            device: function() {
                return "desktop" === _devices.default.real().deviceType && !_devices.default.isSimulator()
            },
            options: {
                focusStateEnabled: true
            }
        }])
    },
    _init: function() {
        this.callBase();
        this.$element().addClass(RADIO_BUTTON_CLASS)
    },
    _initMarkup: function() {
        this.callBase();
        this._renderIcon();
        this._renderCheckedState(this.option("value"));
        this._renderClick();
        this.setAria("role", "radio")
    },
    _renderIcon: function() {
        this._$icon = (0, _renderer.default)("<div>").addClass(RADIO_BUTTON_ICON_CLASS);
        (0, _renderer.default)("<div>").addClass(RADIO_BUTTON_ICON_DOT_CLASS).appendTo(this._$icon);
        this.$element().append(this._$icon)
    },
    _renderCheckedState: function(checked) {
        this.$element().toggleClass(RADIO_BUTTON_CHECKED_CLASS, checked).find("." + RADIO_BUTTON_ICON_CLASS).toggleClass(RADIO_BUTTON_ICON_CHECKED_CLASS, checked);
        this.setAria("checked", checked)
    },
    _renderClick: function() {
        var eventName = (0, _index.addNamespace)(_click.name, this.NAME);
        this._clickAction = this._createAction(function(args) {
            this._clickHandler(args.event)
        }.bind(this));
        _events_engine.default.off(this.$element(), eventName);
        _events_engine.default.on(this.$element(), eventName, function(e) {
            this._clickAction({
                event: e
            })
        }.bind(this))
    },
    _clickHandler: function(e) {
        this._saveValueChangeEvent(e);
        this.option("value", true)
    },
    _optionChanged: function(args) {
        switch (args.name) {
            case "value":
                this._renderCheckedState(args.value);
                this.callBase(args);
                break;
            default:
                this.callBase(args)
        }
    }
});
(0, _component_registrator.default)("dxRadioButton", RadioButton);
var _default = RadioButton;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
