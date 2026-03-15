/**
 * DevExtreme (esm/ui/grid_core/ui.grid_core.header_panel.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import Toolbar from "../toolbar";
import {
    ColumnsView
} from "./ui.grid_core.columns_view";
import {
    noop
} from "../../core/utils/common";
import {
    isDefined,
    isString
} from "../../core/utils/type";
import messageLocalization from "../../localization/message";
import "../drop_down_menu";
import {
    extend
} from "../../core/utils/extend";
import {
    getPathParts
} from "../../core/utils/data";
var HEADER_PANEL_CLASS = "header-panel";
var TOOLBAR_BUTTON_CLASS = "toolbar-button";
var TOOLBAR_ARIA_LABEL = "-ariaToolbar";
var HeaderPanel = ColumnsView.inherit({
    _getToolbarItems: function() {
        return []
    },
    _getButtonContainer: function() {
        return $("<div>").addClass(this.addWidgetPrefix(TOOLBAR_BUTTON_CLASS))
    },
    _getToolbarButtonClass: function(specificClass) {
        var secondClass = specificClass ? " " + specificClass : "";
        return this.addWidgetPrefix(TOOLBAR_BUTTON_CLASS) + secondClass
    },
    _getToolbarOptions: function() {
        var userToolbarOptions = this.option("toolbar");
        var options = {
            toolbarOptions: {
                items: this._getToolbarItems(),
                visible: null === userToolbarOptions || void 0 === userToolbarOptions ? void 0 : userToolbarOptions.visible,
                disabled: null === userToolbarOptions || void 0 === userToolbarOptions ? void 0 : userToolbarOptions.disabled,
                onItemRendered: function(e) {
                    var itemRenderedCallback = e.itemData.onItemRendered;
                    if (itemRenderedCallback) {
                        itemRenderedCallback(e)
                    }
                }
            }
        };
        var userItems = null === userToolbarOptions || void 0 === userToolbarOptions ? void 0 : userToolbarOptions.items;
        options.toolbarOptions.items = this._normalizeToolbarItems(options.toolbarOptions.items, userItems);
        this.executeAction("onToolbarPreparing", options);
        if (options.toolbarOptions && !isDefined(options.toolbarOptions.visible)) {
            var toolbarItems = options.toolbarOptions.items;
            options.toolbarOptions.visible = !!(null !== toolbarItems && void 0 !== toolbarItems && toolbarItems.length)
        }
        return options.toolbarOptions
    },
    _normalizeToolbarItems(defaultItems, userItems) {
        var defaultProps = {
            location: "after"
        };
        var isArray = Array.isArray(userItems);
        if (!isDefined(userItems)) {
            return defaultItems
        }
        if (!isArray) {
            userItems = [userItems]
        }
        var defaultButtonsByNames = {};
        defaultItems.forEach(button => {
            defaultButtonsByNames[button.name] = button
        });
        var normalizedItems = userItems.map(button => {
            var needHideButton = false;
            if (isString(button)) {
                button = {
                    name: button
                };
                needHideButton = true
            }
            if (isDefined(button.name)) {
                if (isDefined(defaultButtonsByNames[button.name])) {
                    button = extend(true, {}, defaultButtonsByNames[button.name], button)
                } else if (needHideButton) {
                    button.visible = false
                }
            }
            return extend(true, {}, defaultProps, button)
        });
        return isArray ? normalizedItems : normalizedItems[0]
    },
    _renderCore: function() {
        if (!this._toolbar) {
            var $headerPanel = this.element();
            $headerPanel.addClass(this.addWidgetPrefix(HEADER_PANEL_CLASS));
            var label = messageLocalization.format(this.component.NAME + TOOLBAR_ARIA_LABEL);
            var $toolbar = $("<div>").attr("aria-label", label).appendTo($headerPanel);
            this._toolbar = this._createComponent($toolbar, Toolbar, this._toolbarOptions)
        } else {
            this._toolbar.option(this._toolbarOptions)
        }
    },
    _columnOptionChanged: noop,
    _handleDataChanged: function() {
        if (this._requireReady) {
            this.render()
        }
    },
    init: function() {
        this.callBase();
        this.createAction("onToolbarPreparing", {
            excludeValidators: ["disabled", "readOnly"]
        })
    },
    render: function() {
        this._toolbarOptions = this._getToolbarOptions();
        this.callBase.apply(this, arguments)
    },
    setToolbarItemDisabled: function(name, optionValue) {
        var toolbarInstance = this._toolbar;
        if (toolbarInstance) {
            var items = toolbarInstance.option("items") || [];
            var itemIndex = items.indexOf(items.filter((function(item) {
                return item.name === name
            }))[0]);
            if (itemIndex >= 0) {
                var itemOptionPrefix = "items[" + itemIndex + "]";
                if (toolbarInstance.option(itemOptionPrefix + ".options")) {
                    toolbarInstance.option(itemOptionPrefix + ".options.disabled", optionValue)
                } else {
                    toolbarInstance.option(itemOptionPrefix + ".disabled", optionValue)
                }
            }
        }
    },
    updateToolbarDimensions: function() {
        var _this$_toolbar;
        null === (_this$_toolbar = this._toolbar) || void 0 === _this$_toolbar ? void 0 : _this$_toolbar.updateDimensions()
    },
    getHeaderPanel: function() {
        return this.element()
    },
    getHeight: function() {
        return this.getElementHeight()
    },
    optionChanged: function(args) {
        if ("onToolbarPreparing" === args.name) {
            this._invalidate();
            args.handled = true
        }
        if ("toolbar" === args.name) {
            args.handled = true;
            if (this._toolbar) {
                var parts = getPathParts(args.fullName);
                var optionName = args.fullName.replace(/^toolbar\./, "");
                if (1 === parts.length) {
                    var toolbarOptions = this._getToolbarOptions();
                    this._toolbar.option(toolbarOptions)
                } else if ("items" === parts[1]) {
                    if (2 === parts.length) {
                        var _toolbarOptions = this._getToolbarOptions();
                        this._toolbar.option("items", _toolbarOptions.items)
                    } else if (3 === parts.length) {
                        var normalizedItem = this._normalizeToolbarItems(this._getToolbarItems(), args.value);
                        this._toolbar.option(optionName, normalizedItem)
                    } else if (parts.length >= 4) {
                        this._toolbar.option(optionName, args.value)
                    }
                } else {
                    this._toolbar.option(optionName, args.value)
                }
            }
        }
        this.callBase(args)
    },
    isVisible: function() {
        return this._toolbarOptions && this._toolbarOptions.visible
    },
    allowDragging: noop
});
export var headerPanelModule = {
    defaultOptions: function() {
        return {}
    },
    views: {
        headerPanel: HeaderPanel
    },
    extenders: {
        controllers: {
            resizing: {
                _updateDimensionsCore: function() {
                    this.callBase.apply(this, arguments);
                    this.getView("headerPanel").updateToolbarDimensions()
                }
            }
        }
    }
};
