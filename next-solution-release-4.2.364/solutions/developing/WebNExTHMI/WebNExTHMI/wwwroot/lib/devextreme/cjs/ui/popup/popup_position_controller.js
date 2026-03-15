/**
 * DevExtreme (cjs/ui/popup/popup_position_controller.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.PopupPositionController = void 0;
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _translator = require("../../animation/translator");
var _window = require("../../core/utils/window");
var _overlay_position_controller = require("../overlay/overlay_position_controller");
var _excluded = ["fullScreen", "forceApplyBindings"];

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _extends() {
    _extends = Object.assign || function(target) {
        for (var i = 1; i < arguments.length; i++) {
            var source = arguments[i];
            for (var key in source) {
                if (Object.prototype.hasOwnProperty.call(source, key)) {
                    target[key] = source[key]
                }
            }
        }
        return target
    };
    return _extends.apply(this, arguments)
}

function _objectWithoutProperties(source, excluded) {
    if (null == source) {
        return {}
    }
    var target = _objectWithoutPropertiesLoose(source, excluded);
    var key, i;
    if (Object.getOwnPropertySymbols) {
        var sourceSymbolKeys = Object.getOwnPropertySymbols(source);
        for (i = 0; i < sourceSymbolKeys.length; i++) {
            key = sourceSymbolKeys[i];
            if (excluded.indexOf(key) >= 0) {
                continue
            }
            if (!Object.prototype.propertyIsEnumerable.call(source, key)) {
                continue
            }
            target[key] = source[key]
        }
    }
    return target
}

function _objectWithoutPropertiesLoose(source, excluded) {
    if (null == source) {
        return {}
    }
    var target = {};
    var sourceKeys = Object.keys(source);
    var key, i;
    for (i = 0; i < sourceKeys.length; i++) {
        key = sourceKeys[i];
        if (excluded.indexOf(key) >= 0) {
            continue
        }
        target[key] = source[key]
    }
    return target
}

function _defineProperties(target, props) {
    for (var i = 0; i < props.length; i++) {
        var descriptor = props[i];
        descriptor.enumerable = descriptor.enumerable || false;
        descriptor.configurable = true;
        if ("value" in descriptor) {
            descriptor.writable = true
        }
        Object.defineProperty(target, descriptor.key, descriptor)
    }
}

function _createClass(Constructor, protoProps, staticProps) {
    if (protoProps) {
        _defineProperties(Constructor.prototype, protoProps)
    }
    if (staticProps) {
        _defineProperties(Constructor, staticProps)
    }
    Object.defineProperty(Constructor, "prototype", {
        writable: false
    });
    return Constructor
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
var window = (0, _window.getWindow)();
var PopupPositionController = function(_OverlayPositionContr) {
    _inheritsLoose(PopupPositionController, _OverlayPositionContr);

    function PopupPositionController(_ref) {
        var _this;
        var fullScreen = _ref.fullScreen,
            forceApplyBindings = _ref.forceApplyBindings,
            args = _objectWithoutProperties(_ref, _excluded);
        _this = _OverlayPositionContr.call(this, args) || this;
        _this._props = _extends({}, _this._props, {
            fullScreen: fullScreen,
            forceApplyBindings: forceApplyBindings
        });
        _this._lastPositionBeforeFullScreen = void 0;
        return _this
    }
    var _proto = PopupPositionController.prototype;
    _proto.positionContent = function() {
        if (this._props.fullScreen) {
            (0, _translator.move)(this._$content, {
                top: 0,
                left: 0
            });
            this.detectVisualPositionChange()
        } else {
            var _this$_props$forceApp, _this$_props;
            null === (_this$_props$forceApp = (_this$_props = this._props).forceApplyBindings) || void 0 === _this$_props$forceApp ? void 0 : _this$_props$forceApp.call(_this$_props);
            if (!this._shouldRenderContentInitialPosition && this._lastPositionBeforeFullScreen) {
                (0, _translator.move)(this._$content, this._lastPositionBeforeFullScreen);
                this._lastPositionBeforeFullScreen = void 0;
                this.detectVisualPositionChange()
            } else {
                _OverlayPositionContr.prototype.positionContent.call(this)
            }
        }
    };
    _proto._getWrapperCoveredElement = function() {
        if (this._props.fullScreen) {
            return (0, _renderer.default)(window)
        }
        return _OverlayPositionContr.prototype._getWrapperCoveredElement.call(this)
    };
    _proto._fullScreenEnabled = function() {
        this.restorePositionOnNextRender(false);
        this._lastPositionBeforeFullScreen = this._visualPosition
    };
    _proto._fullScreenDisabled = function() {
        this.restorePositionOnNextRender(false)
    };
    _createClass(PopupPositionController, [{
        key: "fullScreen",
        set: function(fullScreen) {
            this._props.fullScreen = fullScreen;
            if (fullScreen) {
                this._fullScreenEnabled()
            } else {
                this._fullScreenDisabled()
            }
        }
    }]);
    return PopupPositionController
}(_overlay_position_controller.OverlayPositionController);
exports.PopupPositionController = PopupPositionController;
