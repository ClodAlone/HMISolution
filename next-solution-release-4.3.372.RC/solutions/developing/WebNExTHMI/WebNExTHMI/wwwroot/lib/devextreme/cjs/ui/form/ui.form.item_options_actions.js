/**
 * DevExtreme (cjs/ui/form/ui.form.item_options_actions.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _uiForm = _interopRequireDefault(require("./ui.form.item_option_action"));
var _element_data = require("../../core/element_data");
var _extend = require("../../core/utils/extend");
var _uiForm2 = require("./ui.form.utils");

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
var WidgetOptionItemOptionAction = function(_ItemOptionAction) {
    _inheritsLoose(WidgetOptionItemOptionAction, _ItemOptionAction);

    function WidgetOptionItemOptionAction() {
        return _ItemOptionAction.apply(this, arguments) || this
    }
    var _proto = WidgetOptionItemOptionAction.prototype;
    _proto.tryExecute = function() {
        var value = this._options.value;
        var instance = this.findInstance();
        if (instance) {
            instance.option(value);
            return true
        }
        return false
    };
    return WidgetOptionItemOptionAction
}(_uiForm.default);
var TabOptionItemOptionAction = function(_ItemOptionAction2) {
    _inheritsLoose(TabOptionItemOptionAction, _ItemOptionAction2);

    function TabOptionItemOptionAction() {
        return _ItemOptionAction2.apply(this, arguments) || this
    }
    var _proto2 = TabOptionItemOptionAction.prototype;
    _proto2.tryExecute = function() {
        var tabPanel = this.findInstance();
        if (tabPanel) {
            var _this$_options = this._options,
                optionName = _this$_options.optionName,
                item = _this$_options.item,
                value = _this$_options.value;
            var itemIndex = this._itemsRunTimeInfo.findItemIndexByItem(item);
            if (itemIndex >= 0) {
                tabPanel.option((0, _uiForm2.getFullOptionName)("items[".concat(itemIndex, "]"), optionName), value);
                return true
            }
        }
        return false
    };
    return TabOptionItemOptionAction
}(_uiForm.default);
var GroupItemTemplateChangedAction = function(_ItemOptionAction3) {
    _inheritsLoose(GroupItemTemplateChangedAction, _ItemOptionAction3);

    function GroupItemTemplateChangedAction() {
        return _ItemOptionAction3.apply(this, arguments) || this
    }
    var _proto3 = GroupItemTemplateChangedAction.prototype;
    _proto3.tryExecute = function() {
        var preparedItem = this.findPreparedItem();
        if (null != preparedItem && preparedItem._prepareGroupItemTemplate && preparedItem._renderGroupContentTemplate) {
            preparedItem._prepareGroupItemTemplate(this._options.item.template);
            preparedItem._renderGroupContentTemplate();
            return true
        }
        return false
    };
    return GroupItemTemplateChangedAction
}(_uiForm.default);
var TabsOptionItemOptionAction = function(_ItemOptionAction4) {
    _inheritsLoose(TabsOptionItemOptionAction, _ItemOptionAction4);

    function TabsOptionItemOptionAction() {
        return _ItemOptionAction4.apply(this, arguments) || this
    }
    var _proto4 = TabsOptionItemOptionAction.prototype;
    _proto4.tryExecute = function() {
        var tabPanel = this.findInstance();
        if (tabPanel) {
            var value = this._options.value;
            tabPanel.option("dataSource", value);
            return true
        }
        return false
    };
    return TabsOptionItemOptionAction
}(_uiForm.default);
var ValidationRulesItemOptionAction = function(_ItemOptionAction5) {
    _inheritsLoose(ValidationRulesItemOptionAction, _ItemOptionAction5);

    function ValidationRulesItemOptionAction() {
        return _ItemOptionAction5.apply(this, arguments) || this
    }
    var _proto5 = ValidationRulesItemOptionAction.prototype;
    _proto5.tryExecute = function() {
        var item = this._options.item;
        var instance = this.findInstance();
        var validator = instance && (0, _element_data.data)(instance.$element()[0], "dxValidator");
        if (validator && item) {
            var filterRequired = function(item) {
                return "required" === item.type
            };
            var oldContainsRequired = (validator.option("validationRules") || []).some(filterRequired);
            var newContainsRequired = (item.validationRules || []).some(filterRequired);
            if (!oldContainsRequired && !newContainsRequired || oldContainsRequired && newContainsRequired) {
                validator.option("validationRules", item.validationRules);
                return true
            }
        }
        return false
    };
    return ValidationRulesItemOptionAction
}(_uiForm.default);
var CssClassItemOptionAction = function(_ItemOptionAction6) {
    _inheritsLoose(CssClassItemOptionAction, _ItemOptionAction6);

    function CssClassItemOptionAction() {
        return _ItemOptionAction6.apply(this, arguments) || this
    }
    var _proto6 = CssClassItemOptionAction.prototype;
    _proto6.tryExecute = function() {
        var $itemContainer = this.findItemContainer();
        var _this$_options2 = this._options,
            previousValue = _this$_options2.previousValue,
            value = _this$_options2.value;
        if ($itemContainer) {
            $itemContainer.removeClass(previousValue).addClass(value);
            return true
        }
        return false
    };
    return CssClassItemOptionAction
}(_uiForm.default);
var tryCreateItemOptionAction = function(optionName, itemActionOptions) {
    var _itemActionOptions$it;
    switch (optionName) {
        case "editorOptions":
        case "buttonOptions":
            return new WidgetOptionItemOptionAction(itemActionOptions);
        case "validationRules":
            return new ValidationRulesItemOptionAction(itemActionOptions);
        case "cssClass":
            return new CssClassItemOptionAction(itemActionOptions);
        case "badge":
        case "disabled":
        case "icon":
        case "tabTemplate":
        case "title":
            return new TabOptionItemOptionAction((0, _extend.extend)(itemActionOptions, {
                optionName: optionName
            }));
        case "tabs":
            return new TabsOptionItemOptionAction(itemActionOptions);
        case "template":
            if ("group" === (null === (_itemActionOptions$it = itemActionOptions.item) || void 0 === _itemActionOptions$it ? void 0 : _itemActionOptions$it.itemType)) {
                return new GroupItemTemplateChangedAction(itemActionOptions)
            } else {
                return new TabOptionItemOptionAction((0, _extend.extend)(itemActionOptions, {
                    optionName: optionName
                }))
            }
            default:
                return null
    }
};
var _default = tryCreateItemOptionAction;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
