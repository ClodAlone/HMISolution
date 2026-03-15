/**
 * DevExtreme (esm/ui/validation_summary.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import registerComponent from "../core/component_registrator";
import eventsEngine from "../events/core/events_engine";
import {
    grep
} from "../core/utils/common";
import {
    extend
} from "../core/utils/extend";
import {
    each,
    map
} from "../core/utils/iterator";
import ValidationEngine from "./validation_engine";
import CollectionWidget from "./collection/ui.collection_widget.edit";
var VALIDATION_SUMMARY_CLASS = "dx-validationsummary";
var ITEM_CLASS = VALIDATION_SUMMARY_CLASS + "-item";
var ITEM_DATA_KEY = VALIDATION_SUMMARY_CLASS + "-item-data";
var ValidationSummary = CollectionWidget.inherit({
    _getDefaultOptions() {
        return extend(this.callBase(), {
            focusStateEnabled: false,
            noDataText: null
        })
    },
    _setOptionsByReference() {
        this.callBase();
        extend(this._optionsByReference, {
            validationGroup: true
        })
    },
    _init() {
        this.callBase();
        this._initGroupRegistration()
    },
    _initGroupRegistration() {
        var $element = this.$element();
        var group = this.option("validationGroup") || ValidationEngine.findGroup($element, this._modelByElement($element));
        var groupConfig = ValidationEngine.addGroup(group);
        this._unsubscribeGroup();
        this._groupWasInit = true;
        this._validationGroup = group;
        this.groupSubscription = this._groupValidationHandler.bind(this);
        groupConfig.on("validated", this.groupSubscription)
    },
    _unsubscribeGroup() {
        var groupConfig = ValidationEngine.getGroupConfig(this._validationGroup);
        groupConfig && groupConfig.off("validated", this.groupSubscription)
    },
    _getOrderedItems(validators, items) {
        var orderedItems = [];
        each(validators, (function(_, validator) {
            var foundItems = grep(items, (function(item) {
                if (item.validator === validator) {
                    return true
                }
            }));
            if (foundItems.length) {
                orderedItems = orderedItems.concat(foundItems)
            }
        }));
        return orderedItems
    },
    _groupValidationHandler(params) {
        var items = this._getOrderedItems(params.validators, map(params.brokenRules, (function(rule) {
            return {
                text: rule.message,
                validator: rule.validator,
                index: rule.index
            }
        })));
        this.validators = params.validators;
        each(this.validators, (_, validator) => {
            if (validator._validationSummary !== this) {
                var handler = this._itemValidationHandler.bind(this);
                validator.on("validated", handler);
                validator.on("disposing", (function() {
                    validator.off("validated", handler);
                    validator._validationSummary = null;
                    handler = null
                }));
                validator._validationSummary = this
            }
        });
        this.option("items", items)
    },
    _itemValidationHandler(_ref) {
        var {
            isValid: isValid,
            validator: validator,
            brokenRules: brokenRules
        } = _ref;
        var items = this.option("items");
        var itemsChanged = false;
        var itemIndex = 0;
        var _loop = function() {
            var item = items[itemIndex];
            if (item.validator === validator) {
                var foundRule = grep(brokenRules || [], (function(rule) {
                    return rule.index === item.index
                }))[0];
                if (isValid || !foundRule) {
                    items.splice(itemIndex, 1);
                    itemsChanged = true;
                    return "continue"
                }
                if (foundRule.message !== item.text) {
                    item.text = foundRule.message;
                    itemsChanged = true
                }
            }
            itemIndex++
        };
        while (itemIndex < items.length) {
            var _ret = _loop();
            if ("continue" === _ret) {
                continue
            }
        }
        each(brokenRules, (function(_, rule) {
            var foundItem = grep(items, (function(item) {
                return item.validator === validator && item.index === rule.index
            }))[0];
            if (!foundItem) {
                items.push({
                    text: rule.message,
                    validator: validator,
                    index: rule.index
                });
                itemsChanged = true
            }
        }));
        if (itemsChanged) {
            items = this._getOrderedItems(this.validators, items);
            this.option("items", items)
        }
    },
    _initMarkup() {
        this.$element().addClass(VALIDATION_SUMMARY_CLASS);
        this.callBase()
    },
    _optionChanged(args) {
        switch (args.name) {
            case "validationGroup":
                this._initGroupRegistration();
                break;
            default:
                this.callBase(args)
        }
    },
    _itemClass: () => ITEM_CLASS,
    _itemDataKey: () => ITEM_DATA_KEY,
    _postprocessRenderItem(params) {
        eventsEngine.on(params.itemElement, "click", (function() {
            params.itemData.validator && params.itemData.validator.focus && params.itemData.validator.focus()
        }))
    },
    _dispose() {
        this.callBase();
        this._unsubscribeGroup()
    }
});
registerComponent("dxValidationSummary", ValidationSummary);
export default ValidationSummary;
