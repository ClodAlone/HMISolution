/**
 * DevExtreme (renovation/component_wrapper/common/component.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _inferno = require("inferno");
var _keyboard_processor = _interopRequireDefault(require("../../../events/core/keyboard_processor"));
var _inferno_renderer = _interopRequireDefault(require("../../../core/inferno_renderer"));
var _renderer = _interopRequireDefault(require("../../../core/renderer"));
var _dom_adapter = _interopRequireDefault(require("../../../core/dom_adapter"));
var _dom_component = _interopRequireDefault(require("../../../core/dom_component"));
var _extend = require("../../../core/utils/extend");
var _element = require("../../../core/element");
var _type = require("../../../core/utils/type");
var _template_wrapper = require("./template_wrapper");
var _update_props_immutable = require("../utils/update_props_immutable");

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

function _slicedToArray(arr, i) {
    return _arrayWithHoles(arr) || _iterableToArrayLimit(arr, i) || _unsupportedIterableToArray(arr, i) || _nonIterableRest()
}

function _nonIterableRest() {
    throw new TypeError("Invalid attempt to destructure non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.")
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

function _arrayLikeToArray(arr, len) {
    if (null == len || len > arr.length) {
        len = arr.length
    }
    for (var i = 0, arr2 = new Array(len); i < len; i++) {
        arr2[i] = arr[i]
    }
    return arr2
}

function _iterableToArrayLimit(arr, i) {
    var _i = null == arr ? null : "undefined" !== typeof Symbol && arr[Symbol.iterator] || arr["@@iterator"];
    if (null == _i) {
        return
    }
    var _arr = [];
    var _n = true;
    var _d = false;
    var _s, _e;
    try {
        for (_i = _i.call(arr); !(_n = (_s = _i.next()).done); _n = true) {
            _arr.push(_s.value);
            if (i && _arr.length === i) {
                break
            }
        }
    } catch (err) {
        _d = true;
        _e = err
    } finally {
        try {
            if (!_n && null != _i.return) {
                _i.return()
            }
        } finally {
            if (_d) {
                throw _e
            }
        }
    }
    return _arr
}

function _arrayWithHoles(arr) {
    if (Array.isArray(arr)) {
        return arr
    }
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
var setDefaultOptionValue = function(options, defaultValueGetter) {
    return function(name) {
        if (Object.prototype.hasOwnProperty.call(options, name) && void 0 === options[name]) {
            options[name] = defaultValueGetter(name)
        }
    }
};
var ComponentWrapper = function(_DOMComponent) {
    _inheritsLoose(ComponentWrapper, _DOMComponent);

    function ComponentWrapper(element, options) {
        var _this;
        _this = _DOMComponent.call(this, element, options) || this;
        _this._shouldRaiseContentReady = false;
        _this.validateKeyDownHandler();
        return _this
    }
    var _proto = ComponentWrapper.prototype;
    _proto.validateKeyDownHandler = function() {
        var supportedKeyNames = this.getSupportedKeyNames();
        var hasComponentDefaultKeyHandlers = supportedKeyNames.length > 0;
        var hasComponentKeyDownMethod = "function" === typeof this._viewComponent.prototype.keyDown;
        if (hasComponentDefaultKeyHandlers && !hasComponentKeyDownMethod) {
            throw Error("Component's declaration must have 'keyDown' method.")
        }
    };
    _proto._checkContentReadyOption = function(fullName) {
        var contentReadyOptions = this._getContentReadyOptions().reduce((function(options, name) {
            options[name] = true;
            return options
        }), {});
        this._checkContentReadyOption = function(optionName) {
            return !!contentReadyOptions[optionName]
        };
        return this._checkContentReadyOption(fullName)
    };
    _proto._getContentReadyOptions = function() {
        return ["rtlEnabled"]
    };
    _proto._fireContentReady = function() {
        this._actionsMap.onContentReady({})
    };
    _proto._getDefaultOptions = function() {
        var _this2 = this;
        return (0, _extend.extend)(true, _DOMComponent.prototype._getDefaultOptions.call(this), this._viewComponent.defaultProps, this._propsInfo.twoWay.reduce((function(options, _ref) {
            var _extends2;
            var _ref2 = _slicedToArray(_ref, 3),
                name = _ref2[0],
                defaultName = _ref2[1],
                eventName = _ref2[2];
            return _extends({}, options, (_extends2 = {}, _defineProperty(_extends2, name, _this2._viewComponent.defaultProps[defaultName]), _defineProperty(_extends2, eventName, (function(value) {
                return _this2.option(name, value)
            })), _extends2))
        }), {}), this._propsInfo.templates.reduce((function(options, name) {
            return _extends({}, options, _defineProperty({}, name, null))
        }), {}))
    };
    _proto._initializeComponent = function() {
        var _this$_templateManage, _this3 = this;
        _DOMComponent.prototype._initializeComponent.call(this);
        null === (_this$_templateManage = this._templateManager) || void 0 === _this$_templateManage ? void 0 : _this$_templateManage.addDefaultTemplates(this.getDefaultTemplates());
        this._props = this._optionsWithDefaultTemplates(this.option());
        this._propsInfo.templates.forEach((function(template) {
            _this3._componentTemplates[template] = _this3._createTemplateComponent(_this3._props[template])
        }));
        Object.keys(this._getActionConfigsFull()).forEach((function(name) {
            return _this3._addAction(name)
        }));
        this._viewRef = (0, _inferno.createRef)();
        this.defaultKeyHandlers = this._createDefaultKeyHandlers()
    };
    _proto._initMarkup = function() {
        var props = this.getProps();
        this._renderWrapper(props)
    };
    _proto._renderWrapper = function(props) {
        var containerNode = this.$element()[0];
        if (!this._isNodeReplaced) {
            _inferno_renderer.default.onPreRender()
        }
        _inferno_renderer.default.render(this._viewComponent, props, containerNode, this._isNodeReplaced);
        if (!this._isNodeReplaced) {
            this._isNodeReplaced = true;
            _inferno_renderer.default.onAfterRender();
            this._shouldRaiseContentReady = true
        }
        if (this._shouldRaiseContentReady) {
            this._fireContentReady();
            this._shouldRaiseContentReady = false
        }
    };
    _proto._silent = function(name, value) {
        this._options.silent(name, value)
    };
    _proto._render = function() {};
    _proto._removeWidget = function() {
        _inferno_renderer.default.remove(this.$element()[0])
    };
    _proto._dispose = function() {
        this._removeWidget();
        _DOMComponent.prototype._dispose.call(this)
    };
    _proto._getAdditionalActionConfigs = function() {
        return {
            onContentReady: {
                excludeValidators: ["disabled", "readOnly"]
            }
        }
    };
    _proto._getAdditionalProps = function() {
        return []
    };
    _proto._patchOptionValues = function(options) {
        var _this4 = this;
        var _this$_propsInfo = this._propsInfo,
            allowNull = _this$_propsInfo.allowNull,
            elements = _this$_propsInfo.elements,
            props = _this$_propsInfo.props,
            twoWay = _this$_propsInfo.twoWay;
        var defaultProps = this._viewComponent.defaultProps;
        var children = options.children,
            onKeyboardHandled = options.onKeyboardHandled,
            ref = options.ref;
        var onKeyDown = onKeyboardHandled ? function(_, event_options) {
            onKeyboardHandled(event_options)
        } : void 0;
        var widgetProps = {
            ref: ref,
            children: children,
            onKeyDown: onKeyDown
        };
        [].concat(_toConsumableArray(props), _toConsumableArray(this._getAdditionalProps())).forEach((function(propName) {
            if (Object.prototype.hasOwnProperty.call(options, propName)) {
                widgetProps[propName] = options[propName]
            }
        }));
        allowNull.forEach(setDefaultOptionValue(widgetProps, (function() {
            return null
        })));
        Object.keys(defaultProps).forEach(setDefaultOptionValue(widgetProps, (function(name) {
            return defaultProps[name]
        })));
        twoWay.forEach((function(_ref3) {
            var _ref4 = _slicedToArray(_ref3, 2),
                name = _ref4[0],
                defaultName = _ref4[1];
            setDefaultOptionValue(widgetProps, (function() {
                return defaultProps[defaultName]
            }))(name)
        }));
        elements.forEach((function(name) {
            if (name in widgetProps) {
                var value = widgetProps[name];
                if ((0, _type.isRenderer)(value)) {
                    widgetProps[name] = _this4._patchElementParam(value)
                }
            }
        }));
        return widgetProps
    };
    _proto.getSupportedKeyNames = function() {
        return []
    };
    _proto.prepareStyleProp = function(props) {
        if ("string" === typeof props.style) {
            return _extends({}, props, {
                style: {},
                cssText: props.style
            })
        }
        return props
    };
    _proto.getProps = function() {
        var _this$elementAttr$cla, _elementAttr$class, _this5 = this;
        var _this$option = this.option(),
            elementAttr = _this$option.elementAttr;
        var options = this._patchOptionValues(_extends({}, this._props, {
            ref: this._viewRef,
            children: this._extractDefaultSlot(),
            aria: this._aria
        }));
        this._propsInfo.templates.forEach((function(template) {
            options[template] = _this5._componentTemplates[template]
        }));
        return this.prepareStyleProp(_extends({}, options, this.elementAttr, elementAttr, {
            className: [].concat(_toConsumableArray((null !== (_this$elementAttr$cla = this.elementAttr.class) && void 0 !== _this$elementAttr$cla ? _this$elementAttr$cla : "").split(" ")), _toConsumableArray((null !== (_elementAttr$class = null === elementAttr || void 0 === elementAttr ? void 0 : elementAttr.class) && void 0 !== _elementAttr$class ? _elementAttr$class : "").split(" "))).filter((function(c, i, a) {
                return c && a.indexOf(c) === i
            })).join(" ").trim(),
            class: ""
        }, this._actionsMap))
    };
    _proto._getActionConfigs = function() {
        return {}
    };
    _proto._getActionConfigsFull = function() {
        return _extends({}, this._getActionConfigs(), this._getAdditionalActionConfigs())
    };
    _proto.getDefaultTemplates = function() {
        var defaultTemplates = Object.values(this._templatesInfo);
        var result = {};
        defaultTemplates.forEach((function(template) {
            result[template] = "dx-renovation-template-mock"
        }));
        return result
    };
    _proto._optionsWithDefaultTemplates = function(options) {
        var templateOptions = Object.entries(this._templatesInfo).reduce((function(result, _ref5) {
            var _options$templateName;
            var _ref6 = _slicedToArray(_ref5, 2),
                templateName = _ref6[0],
                templateValue = _ref6[1];
            return _extends({}, result, _defineProperty({}, templateName, null !== (_options$templateName = options[templateName]) && void 0 !== _options$templateName ? _options$templateName : templateValue))
        }), {});
        return _extends({}, options, templateOptions)
    };
    _proto._init = function() {
        _DOMComponent.prototype._init.call(this);
        this.customKeyHandlers = {};
        this._actionsMap = {};
        this._aria = {};
        this._componentTemplates = {}
    };
    _proto._createDefaultKeyHandlers = function() {
        var _this6 = this;
        var result = {};
        var keys = this.getSupportedKeyNames();
        keys.forEach((function(key) {
            result[key] = function(e) {
                return _this6.viewRef.keyDown(_keyboard_processor.default.createKeyDownOptions(e))
            }
        }));
        return result
    };
    _proto._addAction = function(event, actionToAdd) {
        var action = actionToAdd;
        if (!action) {
            var actionByOption = this._createActionByOption(event, this._getActionConfigsFull()[event]);
            action = function(actArgs) {
                Object.keys(actArgs).forEach((function(name) {
                    if ((0, _type.isDefined)(actArgs[name]) && _dom_adapter.default.isNode(actArgs[name])) {
                        actArgs[name] = (0, _element.getPublicElement)((0, _renderer.default)(actArgs[name]))
                    }
                }));
                return actionByOption(actArgs)
            }
        }
        this._actionsMap[event] = action
    };
    _proto._optionChanged = function(option) {
        var fullName = option.fullName,
            name = option.name,
            value = option.value;
        (0, _update_props_immutable.updatePropsImmutable)(this._props, this.option(), name, fullName);
        if (this._propsInfo.templates.includes(name)) {
            this._componentTemplates[name] = this._createTemplateComponent(value)
        }
        if (name && this._getActionConfigsFull()[name]) {
            this._addAction(name)
        }
        this._shouldRaiseContentReady = this._shouldRaiseContentReady || this._checkContentReadyOption(fullName);
        _DOMComponent.prototype._optionChanged.call(this, option);
        this._invalidate()
    };
    _proto._extractDefaultSlot = function() {
        if (this.option("_hasAnonymousTemplateContent")) {
            return _inferno_renderer.default.createElement(_template_wrapper.TemplateWrapper, {
                template: this._getTemplate(this._templateManager.anonymousTemplateName),
                transclude: true
            })
        }
        return null
    };
    _proto._createTemplateComponent = function(templateOption) {
        if (!templateOption) {
            return
        }
        var template = this._getTemplate(templateOption);
        if ((0, _type.isString)(template) && "dx-renovation-template-mock" === template) {
            return
        }
        return function(model) {
            return _inferno_renderer.default.createElement(_template_wrapper.TemplateWrapper, {
                template: template,
                model: model
            })
        }
    };
    _proto._wrapKeyDownHandler = function(initialHandler) {
        var _this7 = this;
        return function(options) {
            var keyName = options.keyName,
                originalEvent = options.originalEvent,
                which = options.which;
            var keys = _this7.customKeyHandlers;
            var func = keys[keyName] || keys[which];
            if (void 0 !== func) {
                var handler = func.bind(_this7);
                var result = handler(originalEvent, options);
                if (!result) {
                    originalEvent.cancel = true;
                    return originalEvent
                }
            }
            return null === initialHandler || void 0 === initialHandler ? void 0 : initialHandler(originalEvent, options)
        }
    };
    _proto._toPublicElement = function(element) {
        return (0, _element.getPublicElement)((0, _renderer.default)(element))
    };
    _proto._patchElementParam = function(value) {
        try {
            var result = (0, _renderer.default)(value);
            var element = null === result || void 0 === result ? void 0 : result.get(0);
            return null !== element && void 0 !== element && element.nodeType ? element : value
        } catch (error) {
            return value
        }
    };
    _proto.repaint = function() {
        this._isNodeReplaced = false;
        this._shouldRaiseContentReady = true;
        this._removeWidget();
        this._refresh()
    };
    _proto._supportedKeys = function() {
        return _extends({}, this.defaultKeyHandlers, this.customKeyHandlers)
    };
    _proto.registerKeyHandler = function(key, handler) {
        this.customKeyHandlers[key] = handler
    };
    _proto.setAria = function(name, value) {
        this._aria[name] = value;
        this._initMarkup()
    };
    _createClass(ComponentWrapper, [{
        key: "_propsInfo",
        get: function() {
            return {
                allowNull: [],
                twoWay: [],
                elements: [],
                templates: [],
                props: []
            }
        }
    }, {
        key: "viewRef",
        get: function() {
            var _this$_viewRef;
            return null === (_this$_viewRef = this._viewRef) || void 0 === _this$_viewRef ? void 0 : _this$_viewRef.current
        }
    }, {
        key: "elementAttr",
        get: function() {
            var _this8 = this;
            if (!this._elementAttr) {
                var attributes = this.$element()[0].attributes;
                var attrs = Array.from(attributes).filter((function(attr) {
                    var _attributes$attr$name;
                    return !_this8._propsInfo.templates.includes(attr.name) && (null === (_attributes$attr$name = attributes[attr.name]) || void 0 === _attributes$attr$name ? void 0 : _attributes$attr$name.specified)
                })).reduce((function(result, _ref7) {
                    var name = _ref7.name,
                        value = _ref7.value;
                    var updatedAttributes = result;
                    updatedAttributes[name] = value;
                    return updatedAttributes
                }), {});
                this._elementAttr = attrs;
                this._storedClasses = this.$element()[0].getAttribute("class") || ""
            }
            var elemStyle = this.$element()[0].style;
            var style = {};
            for (var i = 0; i < elemStyle.length; i += 1) {
                style[elemStyle[i]] = elemStyle.getPropertyValue(elemStyle[i])
            }
            this._elementAttr.style = style;
            this._elementAttr.class = this._storedClasses;
            return this._elementAttr
        }
    }, {
        key: "_templatesInfo",
        get: function() {
            return {}
        }
    }]);
    return ComponentWrapper
}(_dom_component.default);
exports.default = ComponentWrapper;
ComponentWrapper.IS_RENOVATED_WIDGET = false;
ComponentWrapper.IS_RENOVATED_WIDGET = true;
module.exports = exports.default;
module.exports.default = exports.default;
