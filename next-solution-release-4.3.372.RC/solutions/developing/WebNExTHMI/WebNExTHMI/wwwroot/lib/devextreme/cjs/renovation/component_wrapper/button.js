/**
 * DevExtreme (cjs/renovation/component_wrapper/button.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _validation_engine = _interopRequireDefault(require("../../ui/validation_engine"));
var _component = _interopRequireDefault(require("./common/component"));
var _icon = require("../../core/utils/icon");

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
var ButtonWrapper = function(_Component) {
    _inheritsLoose(ButtonWrapper, _Component);

    function ButtonWrapper() {
        return _Component.apply(this, arguments) || this
    }
    var _proto = ButtonWrapper.prototype;
    _proto.getDefaultTemplateNames = function() {
        return ["content"]
    };
    _proto.getSupportedKeyNames = function() {
        return ["space", "enter"]
    };
    _proto.getProps = function() {
        var _this = this;
        var props = _Component.prototype.getProps.call(this);
        props.onClick = function(_ref) {
            var event = _ref.event;
            _this._clickAction({
                event: event,
                validationGroup: _this._validationGroupConfig
            })
        };
        var iconType = (0, _icon.getImageSourceType)(props.icon);
        if ("svg" === iconType) {
            props.iconTemplate = this._createTemplateComponent((function() {
                return props.icon
            }))
        }
        return props
    };
    _proto._toggleActiveState = function(_, value) {
        var button = this.viewRef;
        value ? button.activate() : button.deactivate()
    };
    _proto._getSubmitAction = function() {
        var _this2 = this;
        var needValidate = true;
        var validationStatus = "valid";
        return this._createAction((function(_ref2) {
            var event = _ref2.event,
                submitInput = _ref2.submitInput;
            if (needValidate) {
                var validationGroup = _this2._validationGroupConfig;
                if (void 0 !== validationGroup && "" !== validationGroup) {
                    var validationResult = validationGroup.validate();
                    validationStatus = validationResult.status;
                    if ("pending" === validationResult.status) {
                        needValidate = false;
                        _this2.option("disabled", true);
                        validationResult.complete.then((function(_ref3) {
                            var status = _ref3.status;
                            _this2.option("disabled", false);
                            validationStatus = status;
                            "valid" === validationStatus && submitInput.click();
                            needValidate = true
                        }))
                    }
                }
            }
            "valid" !== validationStatus && event.preventDefault();
            event.stopPropagation()
        }))
    };
    _proto._initializeComponent = function() {
        _Component.prototype._initializeComponent.call(this);
        this._addAction("onSubmit", this._getSubmitAction());
        this._clickAction = this._createClickAction()
    };
    _proto._initMarkup = function() {
        _Component.prototype._initMarkup.call(this);
        var $content = this.$element().find(".dx-button-content");
        var $template = $content.children().filter(".dx-template-wrapper");
        var $input = $content.children().filter(".dx-button-submit-input");
        if ($template.length) {
            $template.addClass("dx-button-content");
            $template.append($input);
            $content.replaceWith($template)
        }
    };
    _proto._patchOptionValues = function(options) {
        return _Component.prototype._patchOptionValues.call(this, _extends({}, options, {
            templateData: options._templateData
        }))
    };
    _proto._findGroup = function() {
        var $element = this.$element();
        var validationGroup = this.option("validationGroup");
        return void 0 !== validationGroup && "" !== validationGroup ? validationGroup : _validation_engine.default.findGroup($element, this._modelByElement($element))
    };
    _proto._createClickAction = function() {
        return this._createActionByOption("onClick", {
            excludeValidators: ["readOnly"]
        })
    };
    _proto._optionChanged = function(option) {
        switch (option.name) {
            case "onClick":
                this._clickAction = this._createClickAction()
        }
        _Component.prototype._optionChanged.call(this, option)
    };
    _createClass(ButtonWrapper, [{
        key: "_validationGroupConfig",
        get: function() {
            return _validation_engine.default.getGroupConfig(this._findGroup())
        }
    }, {
        key: "_templatesInfo",
        get: function() {
            return {
                template: "content"
            }
        }
    }]);
    return ButtonWrapper
}(_component.default);
exports.default = ButtonWrapper;
module.exports = exports.default;
module.exports.default = exports.default;
