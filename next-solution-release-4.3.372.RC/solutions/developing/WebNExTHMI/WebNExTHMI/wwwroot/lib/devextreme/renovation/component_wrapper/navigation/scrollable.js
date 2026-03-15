/**
 * DevExtreme (renovation/component_wrapper/navigation/scrollable.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.ScrollableWrapper = void 0;
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
var ScrollableWrapper = function(_Component) {
    _inheritsLoose(ScrollableWrapper, _Component);

    function ScrollableWrapper() {
        return _Component.apply(this, arguments) || this
    }
    var _proto = ScrollableWrapper.prototype;
    _proto.handleMove = function(event) {
        this.viewRef.scrollableRef.handleMove(event)
    };
    _proto.update = function() {
        var _this$viewRef;
        null === (_this$viewRef = this.viewRef) || void 0 === _this$viewRef ? void 0 : _this$viewRef.updateHandler();
        return (0, _deferred.Deferred)().resolve()
    };
    _proto.isRenovated = function() {
        return !!_component.default.IS_RENOVATED_WIDGET
    };
    _proto._visibilityChanged = function() {};
    _proto._dimensionChanged = function() {
        var _this$viewRef2;
        null === (_this$viewRef2 = this.viewRef) || void 0 === _this$viewRef2 ? void 0 : _this$viewRef2.updateHandler()
    };
    _proto.$content = function() {
        return this.$element().find(".dx-scrollable-content").eq(0)
    };
    _proto._moveIsAllowed = function(event) {
        return this.viewRef.scrollableRef.moveIsAllowed(event)
    };
    _proto._prepareDirections = function(value) {
        this.viewRef.scrollableRef.prepareDirections(value)
    };
    _proto._optionChanged = function(option) {
        var name = option.name;
        if ("useNative" === name) {
            this._isNodeReplaced = false
        }
        _Component.prototype._optionChanged.call(this, option)
    };
    return ScrollableWrapper
}(_component.default);
exports.ScrollableWrapper = ScrollableWrapper;
