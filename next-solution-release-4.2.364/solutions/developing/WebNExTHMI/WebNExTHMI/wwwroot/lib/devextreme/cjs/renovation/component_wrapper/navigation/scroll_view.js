/**
 * DevExtreme (cjs/renovation/component_wrapper/navigation/scroll_view.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.ScrollViewWrapper = void 0;
var _component = _interopRequireDefault(require("../common/component"));
var _deferred = require("../../../core/utils/deferred");

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
var ScrollViewWrapper = function(_Component) {
    _inheritsLoose(ScrollViewWrapper, _Component);

    function ScrollViewWrapper() {
        return _Component.apply(this, arguments) || this
    }
    var _proto = ScrollViewWrapper.prototype;
    _proto.update = function() {
        var _this$viewRef;
        null === (_this$viewRef = this.viewRef) || void 0 === _this$viewRef ? void 0 : _this$viewRef.updateHandler();
        return (0, _deferred.Deferred)().resolve()
    };
    _proto.release = function(preventScrollBottom) {
        this.viewRef.release(preventScrollBottom);
        return (0, _deferred.Deferred)().resolve()
    };
    _proto.isRenovated = function() {
        return !!_component.default.IS_RENOVATED_WIDGET
    };
    _proto._dimensionChanged = function() {
        var _this$viewRef2;
        null === (_this$viewRef2 = this.viewRef) || void 0 === _this$viewRef2 ? void 0 : _this$viewRef2.updateHandler()
    };
    _proto._optionChanged = function(option) {
        var name = option.name;
        if ("useNative" === name) {
            this._isNodeReplaced = false
        }
        _Component.prototype._optionChanged.call(this, option)
    };
    return ScrollViewWrapper
}(_component.default);
exports.ScrollViewWrapper = ScrollViewWrapper;
