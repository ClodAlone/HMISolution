/**
 * DevExtreme (cjs/renovation/ui/scroll_view/internal/pocket/bottom.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.BottomPocketProps = exports.BottomPocket = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _load_indicator = require("../../../load_indicator");
var _consts = require("../../common/consts");
var _themes = require("../../../../../ui/themes");
var _combine_classes = require("../../../../utils/combine_classes");
var _message = _interopRequireDefault(require("../../../../../localization/message"));
var _excluded = ["bottomPocketRef", "reachBottomText", "visible"];

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
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

function _defineProperty(obj, key, value) {
    if (key in obj) {
        Object.defineProperty(obj, key, {
            value: value,
            enumerable: true,
            configurable: true,
            writable: true
        })
    } else {
        obj[key] = value
    }
    return obj
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
var viewFunction = function(viewModel) {
    var _viewModel$props = viewModel.props,
        bottomPocketRef = _viewModel$props.bottomPocketRef,
        reachBottomText = _viewModel$props.reachBottomText,
        reachBottomClasses = viewModel.reachBottomClasses;
    return (0, _inferno.createVNode)(1, "div", _consts.SCROLLVIEW_BOTTOM_POCKET_CLASS, (0, _inferno.createVNode)(1, "div", reachBottomClasses, [(0, _inferno.createVNode)(1, "div", _consts.SCROLLVIEW_REACHBOTTOM_INDICATOR_CLASS, (0, _inferno.createComponentVNode)(2, _load_indicator.LoadIndicator), 2), (0, _inferno.createVNode)(1, "div", _consts.SCROLLVIEW_REACHBOTTOM_TEXT_CLASS, (0, _inferno.createVNode)(1, "div", null, reachBottomText, 0), 2)], 4), 2, null, null, bottomPocketRef)
};
exports.viewFunction = viewFunction;
var BottomPocketProps = Object.defineProperties({
    visible: true
}, {
    reachBottomText: {
        get: function() {
            return (0, _themes.isMaterial)((0, _themes.current)()) ? "" : _message.default.format("dxScrollView-reachBottomText")
        },
        configurable: true,
        enumerable: true
    }
});
exports.BottomPocketProps = BottomPocketProps;
var BottomPocket = function(_BaseInfernoComponent) {
    _inheritsLoose(BottomPocket, _BaseInfernoComponent);

    function BottomPocket(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        return _this
    }
    var _proto = BottomPocket.prototype;
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            reachBottomClasses: this.reachBottomClasses,
            restAttributes: this.restAttributes
        })
    };
    _createClass(BottomPocket, [{
        key: "reachBottomClasses",
        get: function() {
            var _classesMap;
            var visible = this.props.visible;
            var classesMap = (_classesMap = {}, _defineProperty(_classesMap, _consts.SCROLLVIEW_REACHBOTTOM_CLASS, true), _defineProperty(_classesMap, "dx-state-invisible", !visible), _classesMap);
            return (0, _combine_classes.combineClasses)(classesMap)
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.bottomPocketRef, _this$props.reachBottomText, _this$props.visible, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return BottomPocket
}(_inferno2.BaseInfernoComponent);
exports.BottomPocket = BottomPocket;
BottomPocket.defaultProps = BottomPocketProps;
