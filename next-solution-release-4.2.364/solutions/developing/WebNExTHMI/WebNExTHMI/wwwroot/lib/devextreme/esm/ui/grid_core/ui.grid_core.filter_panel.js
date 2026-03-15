/**
 * DevExtreme (esm/ui/grid_core/ui.grid_core.filter_panel.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import {
    isDefined
} from "../../core/utils/type";
import modules from "./ui.grid_core.modules";
import gridUtils from "./ui.grid_core.utils";
import eventsEngine from "../../events/core/events_engine";
import messageLocalization from "../../localization/message";
import CheckBox from "../check_box";
import {
    getCurrentLookupValueText,
    getCustomOperation,
    getCurrentValueText,
    getField,
    getCaptionByOperation,
    getGroupValue,
    isCondition,
    isGroup
} from "../filter_builder/utils";
import {
    when,
    Deferred
} from "../../core/utils/deferred";
import {
    captionize
} from "../../core/utils/inflector";
import {
    registerKeyboardAction
} from "./ui.grid_core.accessibility";
var FILTER_PANEL_CLASS = "filter-panel";
var FILTER_PANEL_TEXT_CLASS = FILTER_PANEL_CLASS + "-text";
var FILTER_PANEL_CHECKBOX_CLASS = FILTER_PANEL_CLASS + "-checkbox";
var FILTER_PANEL_CLEAR_FILTER_CLASS = FILTER_PANEL_CLASS + "-clear-filter";
var FILTER_PANEL_LEFT_CONTAINER = FILTER_PANEL_CLASS + "-left";
var FILTER_PANEL_TARGET = "filterPanel";
var FilterPanelView = modules.View.inherit({
    isVisible: function() {
        return this.option("filterPanel.visible") && this.getController("data").dataSource()
    },
    init: function() {
        this.getController("data").dataSourceChanged.add(() => this.render())
    },
    _renderCore: function() {
        var $element = this.element();
        $element.empty().addClass(this.addWidgetPrefix(FILTER_PANEL_CLASS));
        var $leftContainer = $("<div>").addClass(this.addWidgetPrefix(FILTER_PANEL_LEFT_CONTAINER)).appendTo($element);
        if (this.option("filterValue") || this._filterValueBuffer) {
            $leftContainer.append(this._getCheckElement()).append(this._getFilterElement()).append(this._getTextElement());
            $element.append(this._getRemoveButtonElement())
        } else {
            $leftContainer.append(this._getFilterElement()).append(this._getTextElement())
        }
    },
    _getCheckElement: function() {
        var that = this;
        var $element = $("<div>").addClass(this.addWidgetPrefix(FILTER_PANEL_CHECKBOX_CLASS));
        that._createComponent($element, CheckBox, {
            value: that.option("filterPanel.filterEnabled"),
            onValueChanged: function(e) {
                that.option("filterPanel.filterEnabled", e.value)
            }
        });
        $element.attr("title", this.option("filterPanel.texts.filterEnabledHint"));
        return $element
    },
    _getFilterElement: function() {
        var that = this;
        var $element = $("<div>").addClass("dx-icon-filter");
        eventsEngine.on($element, "click", () => that._showFilterBuilder());
        registerKeyboardAction("filterPanel", that, $element, void 0, () => that._showFilterBuilder());
        that._addTabIndexToElement($element);
        return $element
    },
    _getTextElement: function() {
        var that = this;
        var $textElement = $("<div>").addClass(that.addWidgetPrefix(FILTER_PANEL_TEXT_CLASS));
        var filterText;
        var filterValue = that.option("filterValue");
        if (filterValue) {
            when(that.getFilterText(filterValue, that.getController("filterSync").getCustomFilterOperations())).done((function(filterText) {
                var customizeText = that.option("filterPanel.customizeText");
                if (customizeText) {
                    var customText = customizeText({
                        component: that.component,
                        filterValue: filterValue,
                        text: filterText
                    });
                    if ("string" === typeof customText) {
                        filterText = customText
                    }
                }
                $textElement.text(filterText)
            }))
        } else {
            filterText = that.option("filterPanel.texts.createFilter");
            $textElement.text(filterText)
        }
        eventsEngine.on($textElement, "click", () => that._showFilterBuilder());
        registerKeyboardAction("filterPanel", that, $textElement, void 0, () => that._showFilterBuilder());
        that._addTabIndexToElement($textElement);
        return $textElement
    },
    _showFilterBuilder: function() {
        this.option("filterBuilderPopup.visible", true)
    },
    _getRemoveButtonElement: function() {
        var that = this;
        var clearFilterValue = () => that.option("filterValue", null);
        var $element = $("<div>").addClass(that.addWidgetPrefix(FILTER_PANEL_CLEAR_FILTER_CLASS)).text(that.option("filterPanel.texts.clearFilter"));
        eventsEngine.on($element, "click", clearFilterValue);
        registerKeyboardAction("filterPanel", this, $element, void 0, clearFilterValue);
        that._addTabIndexToElement($element);
        return $element
    },
    _addTabIndexToElement: function($element) {
        if (!this.option("useLegacyKeyboardNavigation")) {
            var tabindex = this.option("tabindex") || 0;
            $element.attr("tabindex", tabindex)
        }
    },
    optionChanged: function(args) {
        switch (args.name) {
            case "filterValue":
                this._invalidate();
                this.option("filterPanel.filterEnabled", true);
                args.handled = true;
                break;
            case "filterPanel":
                this._invalidate();
                args.handled = true;
                break;
            default:
                this.callBase(args)
        }
    },
    _getConditionText: function(fieldText, operationText, valueText) {
        var result = "[".concat(fieldText, "] ").concat(operationText);
        if (isDefined(valueText)) {
            result += valueText
        }
        return result
    },
    _getValueMaskedText: function(value) {
        return Array.isArray(value) ? "('".concat(value.join("', '"), "')") : " '".concat(value, "'")
    },
    _getValueText: function(field, customOperation, value) {
        var deferred = new Deferred;
        var hasCustomOperation = customOperation && customOperation.customizeText;
        if (isDefined(value) || hasCustomOperation) {
            if (!hasCustomOperation && field.lookup) {
                getCurrentLookupValueText(field, value, data => {
                    deferred.resolve(this._getValueMaskedText(data))
                })
            } else {
                var displayValue = Array.isArray(value) ? value : gridUtils.getDisplayValue(field, value);
                when(getCurrentValueText(field, displayValue, customOperation, FILTER_PANEL_TARGET)).done(data => {
                    deferred.resolve(this._getValueMaskedText(data))
                })
            }
        } else {
            deferred.resolve("")
        }
        return deferred.promise()
    },
    getConditionText: function(filterValue, options) {
        var that = this;
        var operation = filterValue[1];
        var deferred = new Deferred;
        var customOperation = getCustomOperation(options.customOperations, operation);
        var operationText;
        var field = getField(filterValue[0], options.columns);
        var fieldText = field.caption || "";
        var value = filterValue[2];
        if (customOperation) {
            operationText = customOperation.caption || captionize(customOperation.name)
        } else if (null === value) {
            operationText = getCaptionByOperation("=" === operation ? "isblank" : "isnotblank", options.filterOperationDescriptions)
        } else {
            operationText = getCaptionByOperation(operation, options.filterOperationDescriptions)
        }
        this._getValueText(field, customOperation, value).done(valueText => {
            deferred.resolve(that._getConditionText(fieldText, operationText, valueText))
        });
        return deferred
    },
    getGroupText: function(filterValue, options, isInnerGroup) {
        var that = this;
        var result = new Deferred;
        var textParts = [];
        var groupValue = getGroupValue(filterValue);
        filterValue.forEach(item => {
            if (isCondition(item)) {
                textParts.push(that.getConditionText(item, options))
            } else if (isGroup(item)) {
                textParts.push(that.getGroupText(item, options, true))
            }
        });
        when.apply(this, textParts).done((function() {
            var text;
            for (var _len = arguments.length, args = new Array(_len), _key = 0; _key < _len; _key++) {
                args[_key] = arguments[_key]
            }
            if ("!" === groupValue[0]) {
                var groupText = options.groupOperationDescriptions["not" + groupValue.substring(1, 2).toUpperCase() + groupValue.substring(2)].split(" ");
                text = "".concat(groupText[0], " ").concat(args[0])
            } else {
                text = args.join(" ".concat(options.groupOperationDescriptions[groupValue], " "))
            }
            if (isInnerGroup) {
                text = "(".concat(text, ")")
            }
            result.resolve(text)
        }));
        return result
    },
    getFilterText: function(filterValue, customOperations) {
        var options = {
            customOperations: customOperations,
            columns: this.getController("columns").getFilteringColumns(),
            filterOperationDescriptions: this.option("filterBuilder.filterOperationDescriptions"),
            groupOperationDescriptions: this.option("filterBuilder.groupOperationDescriptions")
        };
        return isCondition(filterValue) ? this.getConditionText(filterValue, options) : this.getGroupText(filterValue, options)
    }
});
export var filterPanelModule = {
    defaultOptions: function() {
        return {
            filterPanel: {
                visible: false,
                filterEnabled: true,
                texts: {
                    createFilter: messageLocalization.format("dxDataGrid-filterPanelCreateFilter"),
                    clearFilter: messageLocalization.format("dxDataGrid-filterPanelClearFilter"),
                    filterEnabledHint: messageLocalization.format("dxDataGrid-filterPanelFilterEnabledHint")
                }
            }
        }
    },
    views: {
        filterPanelView: FilterPanelView
    },
    extenders: {
        controllers: {
            data: {
                optionChanged: function(args) {
                    switch (args.name) {
                        case "filterPanel":
                            this._applyFilter();
                            args.handled = true;
                            break;
                        default:
                            this.callBase(args)
                    }
                }
            }
        }
    }
};
