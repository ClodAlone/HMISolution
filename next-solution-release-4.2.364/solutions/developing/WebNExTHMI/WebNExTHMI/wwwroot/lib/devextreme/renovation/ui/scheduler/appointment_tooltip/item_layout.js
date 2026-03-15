/**
 * DevExtreme (renovation/ui/scheduler/appointment_tooltip/item_layout.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.TooltipItemLayoutProps = exports.TooltipItemLayout = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _marker = require("./marker");
var _button = require("../../button");
var _item_content = require("./item_content");
var _get_current_appointment = _interopRequireDefault(require("./utils/get_current_appointment"));
var _default_functions = require("./utils/default_functions");
var _excluded = ["className", "getTextAndFormatDate", "index", "item", "itemContentTemplate", "onDelete", "onHide", "showDeleteButton", "singleAppointment"];

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
var viewFunction = function(viewModel) {
    var ItemContentTemplate = viewModel.props.itemContentTemplate;
    return viewModel.props.itemContentTemplate ? ItemContentTemplate({
        model: {
            appointmentData: viewModel.props.item.data,
            targetedAppointmentData: viewModel.currentAppointment
        },
        index: viewModel.props.index
    }) : (0, _inferno.normalizeProps)((0, _inferno.createVNode)(1, "div", "dx-tooltip-appointment-item ".concat(viewModel.props.className), [(0, _inferno.createComponentVNode)(2, _marker.Marker), (0, _inferno.createComponentVNode)(2, _item_content.TooltipItemContent, {
        text: viewModel.formattedContent.text,
        formattedDate: viewModel.formattedContent.formatDate
    }), viewModel.props.showDeleteButton && (0, _inferno.createVNode)(1, "div", "dx-tooltip-appointment-item-delete-button-container", (0, _inferno.createComponentVNode)(2, _button.Button, {
        className: "dx-tooltip-appointment-item-delete-button",
        icon: "trash",
        stylingMode: "text",
        onClick: viewModel.onDeleteButtonClick
    }), 2)], 0, _extends({}, viewModel.restAttributes)))
};
exports.viewFunction = viewFunction;
var TooltipItemLayoutProps = Object.defineProperties({
    className: "",
    index: 0,
    showDeleteButton: true,
    getTextAndFormatDate: _default_functions.defaultGetTextAndFormatDate
}, {
    item: {
        get: function() {
            return {
                data: {}
            }
        },
        configurable: true,
        enumerable: true
    },
    singleAppointment: {
        get: function() {
            return {}
        },
        configurable: true,
        enumerable: true
    }
});
exports.TooltipItemLayoutProps = TooltipItemLayoutProps;
var getTemplate = function(TemplateProp) {
    return TemplateProp && (TemplateProp.defaultProps ? function(props) {
        return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, TemplateProp, _extends({}, props)))
    } : TemplateProp)
};
var TooltipItemLayout = function(_BaseInfernoComponent) {
    _inheritsLoose(TooltipItemLayout, _BaseInfernoComponent);

    function TooltipItemLayout(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        _this.__getterCache = {};
        return _this
    }
    var _proto = TooltipItemLayout.prototype;
    _proto.componentWillUpdate = function(nextProps, nextState, context) {
        if (this.props.item !== nextProps.item || this.props.onDelete !== nextProps.onDelete || this.props.onHide !== nextProps.onHide || this.props.singleAppointment !== nextProps.singleAppointment) {
            this.__getterCache.onDeleteButtonClick = void 0
        }
        if (this.props.getTextAndFormatDate !== nextProps.getTextAndFormatDate || this.props.item !== nextProps.item) {
            this.__getterCache.formattedContent = void 0
        }
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                itemContentTemplate: getTemplate(props.itemContentTemplate)
            }),
            currentAppointment: this.currentAppointment,
            onDeleteButtonClick: this.onDeleteButtonClick,
            formattedContent: this.formattedContent,
            restAttributes: this.restAttributes
        })
    };
    _createClass(TooltipItemLayout, [{
        key: "currentAppointment",
        get: function() {
            var item = this.props.item;
            return (0, _get_current_appointment.default)(item)
        }
    }, {
        key: "onDeleteButtonClick",
        get: function() {
            var _this2 = this;
            if (void 0 !== this.__getterCache.onDeleteButtonClick) {
                return this.__getterCache.onDeleteButtonClick
            }
            return this.__getterCache.onDeleteButtonClick = (_this2$props = _this2.props, item = _this2$props.item, onDelete = _this2$props.onDelete, onHide = _this2$props.onHide, singleAppointment = _this2$props.singleAppointment, function(e) {
                null === onHide || void 0 === onHide ? void 0 : onHide();
                e.event.stopPropagation();
                null === onDelete || void 0 === onDelete ? void 0 : onDelete(item.data, singleAppointment)
            });
            var _this2$props, item, onDelete, onHide, singleAppointment
        }
    }, {
        key: "formattedContent",
        get: function() {
            var _this3 = this;
            if (void 0 !== this.__getterCache.formattedContent) {
                return this.__getterCache.formattedContent
            }
            return this.__getterCache.formattedContent = (_this3$props = _this3.props, getTextAndFormatDate = _this3$props.getTextAndFormatDate, item = _this3$props.item, data = item.data, getTextAndFormatDate(data, _this3.currentAppointment));
            var _this3$props, getTextAndFormatDate, item, data
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.className, _this$props.getTextAndFormatDate, _this$props.index, _this$props.item, _this$props.itemContentTemplate, _this$props.onDelete, _this$props.onHide, _this$props.showDeleteButton, _this$props.singleAppointment, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return TooltipItemLayout
}(_inferno2.BaseInfernoComponent);
exports.TooltipItemLayout = TooltipItemLayout;
TooltipItemLayout.defaultProps = TooltipItemLayoutProps;
