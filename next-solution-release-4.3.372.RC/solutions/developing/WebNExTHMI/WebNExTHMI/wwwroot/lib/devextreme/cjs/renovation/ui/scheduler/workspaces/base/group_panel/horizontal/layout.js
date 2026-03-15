/**
 * DevExtreme (cjs/renovation/ui/scheduler/workspaces/base/group_panel/horizontal/layout.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.HorizontalGroupPanelLayoutProps = exports.GroupPanelHorizontalLayout = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _row = require("./row");
var _group_panel_layout_props = require("../group_panel_layout_props");
var _excluded = ["className", "elementRef", "groupByDate", "groupPanelData", "height", "resourceCellTemplate", "styles"];

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
    var groupPanelItems = _ref.groupPanelItems,
        resourceCellTemplate = _ref.props.resourceCellTemplate;
    return (0, _inferno.createFragment)(groupPanelItems.map((function(group) {
        return (0, _inferno.createComponentVNode)(2, _row.Row, {
            groupItems: group,
            cellTemplate: resourceCellTemplate
        }, group[0].key)
    })), 0)
};
exports.viewFunction = viewFunction;
var HorizontalGroupPanelLayoutProps = _group_panel_layout_props.GroupPanelLayoutProps;
exports.HorizontalGroupPanelLayoutProps = HorizontalGroupPanelLayoutProps;
var getTemplate = function(TemplateProp) {
    return TemplateProp && (TemplateProp.defaultProps ? function(props) {
        return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, TemplateProp, _extends({}, props)))
    } : TemplateProp)
};
var GroupPanelHorizontalLayout = function(_BaseInfernoComponent) {
    _inheritsLoose(GroupPanelHorizontalLayout, _BaseInfernoComponent);

    function GroupPanelHorizontalLayout(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        _this.__getterCache = {};
        return _this
    }
    var _proto = GroupPanelHorizontalLayout.prototype;
    _proto.componentWillUpdate = function(nextProps, nextState, context) {
        if (this.props.groupPanelData !== nextProps.groupPanelData) {
            this.__getterCache.groupPanelItems = void 0
        }
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                resourceCellTemplate: getTemplate(props.resourceCellTemplate)
            }),
            groupPanelItems: this.groupPanelItems,
            restAttributes: this.restAttributes
        })
    };
    _createClass(GroupPanelHorizontalLayout, [{
        key: "groupPanelItems",
        get: function() {
            var _this2 = this;
            if (void 0 !== this.__getterCache.groupPanelItems) {
                return this.__getterCache.groupPanelItems
            }
            return this.__getterCache.groupPanelItems = (groupPanelData = _this2.props.groupPanelData, baseColSpan = groupPanelData.baseColSpan, groupPanelItems = groupPanelData.groupPanelItems, colSpans = groupPanelItems.reduceRight((function(currentColSpans, groupsRow, index) {
                var nextColSpans = currentColSpans;
                var currentLevelGroupCount = groupsRow.length;
                var previousColSpan = index === groupPanelItems.length - 1 ? baseColSpan : currentColSpans[index + 1];
                var previousLevelGroupCount = index === groupPanelItems.length - 1 ? currentLevelGroupCount : groupPanelItems[index + 1].length;
                var groupCountDiff = previousLevelGroupCount / currentLevelGroupCount;
                nextColSpans[index] = groupCountDiff * previousColSpan;
                return nextColSpans
            }), _toConsumableArray(new Array(groupPanelItems.length))), groupPanelItems.map((function(groupsRenderRow, index) {
                var colSpan = colSpans[index];
                return groupsRenderRow.map((function(groupItem) {
                    return _extends({}, groupItem, {
                        colSpan: colSpan
                    })
                }))
            })));
            var groupPanelData, baseColSpan, groupPanelItems, colSpans
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.className, _this$props.elementRef, _this$props.groupByDate, _this$props.groupPanelData, _this$props.height, _this$props.resourceCellTemplate, _this$props.styles, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return GroupPanelHorizontalLayout
}(_inferno2.BaseInfernoComponent);
exports.GroupPanelHorizontalLayout = GroupPanelHorizontalLayout;
GroupPanelHorizontalLayout.defaultProps = HorizontalGroupPanelLayoutProps;
