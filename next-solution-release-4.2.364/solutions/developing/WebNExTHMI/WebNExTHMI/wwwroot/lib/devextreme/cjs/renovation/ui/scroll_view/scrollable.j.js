/**
 * DevExtreme (cjs/renovation/ui/scroll_view/scrollable.j.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _component_registrator = _interopRequireDefault(require("../../../core/component_registrator"));
var _scrollable = require("../../component_wrapper/navigation/scrollable");
var _scrollable2 = require("./scrollable");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _toConsumableArray(arr) {
    return _arrayWithoutHoles(arr) || _iterableToArray(arr) || _unsupportedIterableToArray(arr) || _nonIterableSpread()
}

function _nonIterableSpread() {
    throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.")
}

function _unsupportedIterableToArray(o, minLen) {
    if (!o) {
        return
    }
    if ("string" === typeof o) {
        return _arrayLikeToArray(o, minLen)
    }
    var n = Object.prototype.toString.call(o).slice(8, -1);
    if ("Object" === n && o.constructor) {
        n = o.constructor.name
    }
    if ("Map" === n || "Set" === n) {
        return Array.from(o)
    }
    if ("Arguments" === n || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) {
        return _arrayLikeToArray(o, minLen)
    }
}

function _iterableToArray(iter) {
    if ("undefined" !== typeof Symbol && null != iter[Symbol.iterator] || null != iter["@@iterator"]) {
        return Array.from(iter)
    }
}

function _arrayWithoutHoles(arr) {
    if (Array.isArray(arr)) {
        return _arrayLikeToArray(arr)
    }
}

function _arrayLikeToArray(arr, len) {
    if (null == len || len > arr.length) {
        len = arr.length
    }
    for (var i = 0, arr2 = new Array(len); i < len; i++) {
        arr2[i] = arr[i]
    }
    return arr2
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
var Scrollable = function(_ScrollableWrapper) {
    _inheritsLoose(Scrollable, _ScrollableWrapper);

    function Scrollable() {
        return _ScrollableWrapper.apply(this, arguments) || this
    }
    var _proto = Scrollable.prototype;
    _proto.content = function() {
        var _this$viewRef;
        return this._toPublicElement(null === (_this$viewRef = this.viewRef) || void 0 === _this$viewRef ? void 0 : _this$viewRef.content.apply(_this$viewRef, arguments))
    };
    _proto.container = function() {
        var _this$viewRef2;
        return this._toPublicElement(null === (_this$viewRef2 = this.viewRef) || void 0 === _this$viewRef2 ? void 0 : _this$viewRef2.container.apply(_this$viewRef2, arguments))
    };
    _proto.scrollTo = function(targetLocation) {
        var _this$viewRef3;
        return null === (_this$viewRef3 = this.viewRef) || void 0 === _this$viewRef3 ? void 0 : _this$viewRef3.scrollTo.apply(_this$viewRef3, arguments)
    };
    _proto.scrollBy = function(distance) {
        var _this$viewRef4;
        return null === (_this$viewRef4 = this.viewRef) || void 0 === _this$viewRef4 ? void 0 : _this$viewRef4.scrollBy.apply(_this$viewRef4, arguments)
    };
    _proto.updateHandler = function() {
        var _this$viewRef5;
        return null === (_this$viewRef5 = this.viewRef) || void 0 === _this$viewRef5 ? void 0 : _this$viewRef5.updateHandler.apply(_this$viewRef5, arguments)
    };
    _proto.release = function() {
        var _this$viewRef6;
        return null === (_this$viewRef6 = this.viewRef) || void 0 === _this$viewRef6 ? void 0 : _this$viewRef6.release.apply(_this$viewRef6, arguments)
    };
    _proto.refresh = function() {
        var _this$viewRef7;
        return null === (_this$viewRef7 = this.viewRef) || void 0 === _this$viewRef7 ? void 0 : _this$viewRef7.refresh.apply(_this$viewRef7, arguments)
    };
    _proto.scrollToElement = function(element, offset) {
        var _this$viewRef8;
        var params = [this._patchElementParam(element), offset];
        return null === (_this$viewRef8 = this.viewRef) || void 0 === _this$viewRef8 ? void 0 : _this$viewRef8.scrollToElement.apply(_this$viewRef8, _toConsumableArray(params.slice(0, arguments.length)))
    };
    _proto.scrollHeight = function() {
        var _this$viewRef9;
        return null === (_this$viewRef9 = this.viewRef) || void 0 === _this$viewRef9 ? void 0 : _this$viewRef9.scrollHeight.apply(_this$viewRef9, arguments)
    };
    _proto.scrollWidth = function() {
        var _this$viewRef10;
        return null === (_this$viewRef10 = this.viewRef) || void 0 === _this$viewRef10 ? void 0 : _this$viewRef10.scrollWidth.apply(_this$viewRef10, arguments)
    };
    _proto.scrollOffset = function() {
        var _this$viewRef11;
        return null === (_this$viewRef11 = this.viewRef) || void 0 === _this$viewRef11 ? void 0 : _this$viewRef11.scrollOffset.apply(_this$viewRef11, arguments)
    };
    _proto.scrollTop = function() {
        var _this$viewRef12;
        return null === (_this$viewRef12 = this.viewRef) || void 0 === _this$viewRef12 ? void 0 : _this$viewRef12.scrollTop.apply(_this$viewRef12, arguments)
    };
    _proto.scrollLeft = function() {
        var _this$viewRef13;
        return null === (_this$viewRef13 = this.viewRef) || void 0 === _this$viewRef13 ? void 0 : _this$viewRef13.scrollLeft.apply(_this$viewRef13, arguments)
    };
    _proto.clientHeight = function() {
        var _this$viewRef14;
        return null === (_this$viewRef14 = this.viewRef) || void 0 === _this$viewRef14 ? void 0 : _this$viewRef14.clientHeight.apply(_this$viewRef14, arguments)
    };
    _proto.clientWidth = function() {
        var _this$viewRef15;
        return null === (_this$viewRef15 = this.viewRef) || void 0 === _this$viewRef15 ? void 0 : _this$viewRef15.clientWidth.apply(_this$viewRef15, arguments)
    };
    _proto.getScrollElementPosition = function(targetElement, direction, offset) {
        var _this$viewRef16;
        var params = [this._patchElementParam(targetElement), direction, offset];
        return null === (_this$viewRef16 = this.viewRef) || void 0 === _this$viewRef16 ? void 0 : _this$viewRef16.getScrollElementPosition.apply(_this$viewRef16, _toConsumableArray(params.slice(0, arguments.length)))
    };
    _proto.startLoading = function() {
        var _this$viewRef17;
        return null === (_this$viewRef17 = this.viewRef) || void 0 === _this$viewRef17 ? void 0 : _this$viewRef17.startLoading.apply(_this$viewRef17, arguments)
    };
    _proto.finishLoading = function() {
        var _this$viewRef18;
        return null === (_this$viewRef18 = this.viewRef) || void 0 === _this$viewRef18 ? void 0 : _this$viewRef18.finishLoading.apply(_this$viewRef18, arguments)
    };
    _proto._getActionConfigs = function() {
        return {
            onVisibilityChange: {},
            onStart: {},
            onEnd: {},
            onBounce: {},
            scrollLocationChange: {},
            onScroll: {},
            onUpdated: {},
            onPullDown: {},
            onReachBottom: {}
        }
    };
    _createClass(Scrollable, [{
        key: "_propsInfo",
        get: function() {
            return {
                twoWay: [],
                allowNull: [],
                elements: [],
                templates: [],
                props: ["useNative", "useSimulatedScrollbar", "refreshStrategy", "inertiaEnabled", "useKeyboard", "showScrollbar", "scrollByThumb", "onVisibilityChange", "onStart", "onEnd", "onBounce", "scrollLocationChange", "aria", "addWidgetClass", "disabled", "height", "width", "visible", "rtlEnabled", "classes", "direction", "bounceEnabled", "scrollByContent", "pullDownEnabled", "reachBottomEnabled", "forceGeneratePockets", "needScrollViewContentWrapper", "needScrollViewLoadPanel", "needRenderScrollbars", "onScroll", "onUpdated", "onPullDown", "onReachBottom", "pullingDownText", "pulledDownText", "refreshingText", "reachBottomText"]
            }
        }
    }, {
        key: "_viewComponent",
        get: function() {
            return _scrollable2.Scrollable
        }
    }]);
    return Scrollable
}(_scrollable.ScrollableWrapper);
exports.default = Scrollable;
(0, _component_registrator.default)("dxScrollable", Scrollable);
module.exports = exports.default;
module.exports.default = exports.default;
