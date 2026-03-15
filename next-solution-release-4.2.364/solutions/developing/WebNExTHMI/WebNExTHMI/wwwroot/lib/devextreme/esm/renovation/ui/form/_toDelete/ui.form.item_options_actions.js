/**
 * DevExtreme (esm/renovation/ui/form/_toDelete/ui.form.item_options_actions.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import ItemOptionAction from "./ui.form.item_option_action";
import {
    data
} from "../../core/element_data";
import {
    extend
} from "../../core/utils/extend";
import {
    getFullOptionName
} from "./ui.form.utils";
class WidgetOptionItemOptionAction extends ItemOptionAction {
    tryExecute() {
        var {
            value: value
        } = this._options;
        var instance = this.findInstance();
        if (instance) {
            instance.option(value);
            return true
        }
        return false
    }
}
class TabOptionItemOptionAction extends ItemOptionAction {
    tryExecute() {
        var tabPanel = this.findInstance();
        if (tabPanel) {
            var {
                optionName: optionName,
                item: item,
                value: value
            } = this._options;
            var itemIndex = this._itemsRunTimeInfo.findItemIndexByItem(item);
            if (itemIndex >= 0) {
                tabPanel.option(getFullOptionName("items[".concat(itemIndex, "]"), optionName), value);
                return true
            }
        }
        return false
    }
}
class TabsOptionItemOptionAction extends ItemOptionAction {
    tryExecute() {
        var tabPanel = this.findInstance();
        if (tabPanel) {
            var {
                value: value
            } = this._options;
            tabPanel.option("dataSource", value);
            return true
        }
        return false
    }
}
class ValidationRulesItemOptionAction extends ItemOptionAction {
    tryExecute() {
        var {
            item: item
        } = this._options;
        var instance = this.findInstance();
        var validator = instance && data(instance.$element()[0], "dxValidator");
        if (validator && item) {
            var filterRequired = item => "required" === item.type;
            var oldContainsRequired = (validator.option("validationRules") || []).some(filterRequired);
            var newContainsRequired = (item.validationRules || []).some(filterRequired);
            if (!oldContainsRequired && !newContainsRequired || oldContainsRequired && newContainsRequired) {
                validator.option("validationRules", item.validationRules);
                return true
            }
        }
        return false
    }
}
class CssClassItemOptionAction extends ItemOptionAction {
    tryExecute() {
        var $itemContainer = this.findItemContainer();
        var {
            previousValue: previousValue,
            value: value
        } = this._options;
        if ($itemContainer) {
            $itemContainer.removeClass(previousValue).addClass(value);
            return true
        }
        return false
    }
}
var tryCreateItemOptionAction = (optionName, itemActionOptions) => {
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
        case "template":
        case "tabTemplate":
        case "title":
            return new TabOptionItemOptionAction(extend(itemActionOptions, {
                optionName: optionName
            }));
        case "tabs":
            return new TabsOptionItemOptionAction(itemActionOptions);
        default:
            return null
    }
};
export default tryCreateItemOptionAction;
