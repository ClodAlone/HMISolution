/**
 * DevExtreme (cjs/renovation/ui/common/icon.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.IconProps = exports.Icon = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _icon = require("../../../core/utils/icon");
var _combine_classes = require("../../utils/combine_classes");
var _excluded = ["iconTemplate", "position", "source"];

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
var viewFunction = function(_ref) {
    var iconClassName = _ref.iconClassName,
        _ref$props = _ref.props,
        IconTemplate = _ref$props.iconTemplate,
        source = _ref$props.source,
        sourceType = _ref.sourceType;
    return (0, _inferno.createFragment)(["dxIcon" === sourceType && (0, _inferno.createVNode)(1, "i", iconClassName), "fontIcon" === sourceType && (0, _inferno.createVNode)(1, "i", iconClassName), "image" === sourceType && (0, _inferno.createVNode)(1, "img", iconClassName, null, 1, {
        alt: "",
        src: source
    }), IconTemplate && (0, _inferno.createVNode)(1, "i", iconClassName, IconTemplate({}), 0)], 0)
};
exports.viewFunction = viewFunction;
var IconProps = {
    position: "left",
    source: ""
};
exports.IconProps = IconProps;
var getTemplate = function(TemplateProp) {
    return TemplateProp && (TemplateProp.defaultProps ? function(props) {
        return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, TemplateProp, _extends({}, props)))
    } : TemplateProp)
};
var Icon = function(_BaseInfernoComponent) {
    _inheritsLoose(Icon, _BaseInfernoComponent);

    function Icon(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        return _this
    }
    var _proto = Icon.prototype;
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                iconTemplate: getTemplate(props.iconTemplate)
            }),
            sourceType: this.sourceType,
            cssClass: this.cssClass,
            iconClassName: this.iconClassName,
            restAttributes: this.restAttributes
        })
    };
    _createClass(Icon, [{
        key: "sourceType",
        get: function() {
            return (0, _icon.getImageSourceType)(this.props.source)
        }
    }, {
        key: "cssClass",
        get: function() {
            return "left" !== this.props.position ? "dx-icon-right" : ""
        }
    }, {
        key: "iconClassName",
        get: function() {
            var generalClasses = _defineProperty({
                "dx-icon": true
            }, this.cssClass, !!this.cssClass);
            var source = this.props.source;
            if ("dxIcon" === this.sourceType) {
                return (0, _combine_classes.combineClasses)(_extends({}, generalClasses, _defineProperty({}, "dx-icon-".concat(source), true)))
            }
            if ("fontIcon" === this.sourceType) {
                return (0, _combine_classes.combineClasses)(_extends({}, generalClasses, _defineProperty({}, String(source), !!source)))
            }
            if ("image" === this.sourceType) {
                return (0, _combine_classes.combineClasses)(generalClasses)
            }
            if ("svg" === this.sourceType) {
                return (0, _combine_classes.combineClasses)(_extends({}, generalClasses, {
                    "dx-svg-icon": true
                }))
            }
            return ""
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.iconTemplate, _this$props.position, _this$props.source, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return Icon
}(_inferno2.BaseInfernoComponent);
exports.Icon = Icon;
Icon.defaultProps = IconProps;
