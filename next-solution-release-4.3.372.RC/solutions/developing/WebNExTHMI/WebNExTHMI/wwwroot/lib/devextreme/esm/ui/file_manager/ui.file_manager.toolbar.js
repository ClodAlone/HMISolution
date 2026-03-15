/**
 * DevExtreme (esm/ui/file_manager/ui.file_manager.toolbar.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getWidth
} from "../../core/utils/size";
import $ from "../../core/renderer";
import {
    extend
} from "../../core/utils/extend";
import {
    isDefined,
    isString
} from "../../core/utils/type";
import {
    ensureDefined
} from "../../core/utils/common";
import messageLocalization from "../../localization/message";
import {
    extendAttributes
} from "./ui.file_manager.common";
import Widget from "../widget/ui.widget";
import Toolbar from "../toolbar";
import "../drop_down_button";
var FILE_MANAGER_TOOLBAR_CLASS = "dx-filemanager-toolbar";
var FILE_MANAGER_GENERAL_TOOLBAR_CLASS = "dx-filemanager-general-toolbar";
var FILE_MANAGER_FILE_TOOLBAR_CLASS = "dx-filemanager-file-toolbar";
var FILE_MANAGER_TOOLBAR_SEPARATOR_ITEM_CLASS = FILE_MANAGER_TOOLBAR_CLASS + "-separator-item";
var FILE_MANAGER_TOOLBAR_VIEWMODE_ITEM_CLASS = FILE_MANAGER_TOOLBAR_CLASS + "-viewmode-item";
var FILE_MANAGER_TOOLBAR_HAS_LARGE_ICON_CLASS = FILE_MANAGER_TOOLBAR_CLASS + "-has-large-icon";
var FILE_MANAGER_VIEW_SWITCHER_POPUP_CLASS = "dx-filemanager-view-switcher-popup";
var DEFAULT_ITEM_CONFIGS = {
    showNavPane: {
        location: "before"
    },
    create: {
        location: "before",
        compactMode: {
            showText: "inMenu",
            locateInMenu: "auto"
        }
    },
    upload: {
        location: "before",
        compactMode: {
            showText: "inMenu",
            locateInMenu: "auto"
        }
    },
    refresh: {
        location: "after",
        showText: "inMenu",
        cssClass: FILE_MANAGER_TOOLBAR_HAS_LARGE_ICON_CLASS,
        compactMode: {
            showText: "inMenu",
            locateInMenu: "auto"
        }
    },
    switchView: {
        location: "after"
    },
    download: {
        location: "before",
        compactMode: {
            showText: "inMenu",
            locateInMenu: "auto"
        }
    },
    move: {
        location: "before",
        compactMode: {
            showText: "inMenu",
            locateInMenu: "auto"
        }
    },
    copy: {
        location: "before",
        compactMode: {
            showText: "inMenu",
            locateInMenu: "auto"
        }
    },
    rename: {
        location: "before",
        compactMode: {
            showText: "inMenu",
            locateInMenu: "auto"
        }
    },
    delete: {
        location: "before",
        compactMode: {
            showText: "inMenu"
        }
    },
    clearSelection: {
        location: "after",
        locateInMenu: "never",
        compactMode: {
            showText: "inMenu"
        }
    },
    separator: {
        location: "before"
    }
};
var DEFAULT_ITEM_ALLOWED_PROPERTIES = ["visible", "location", "locateInMenu", "disabled", "showText"];
var DEFAULT_ITEM_ALLOWED_OPTION_PROPERTIES = ["accessKey", "elementAttr", "height", "hint", "icon", "stylingMode", "tabIndex", "text", "width"];
var ALWAYS_VISIBLE_TOOLBAR_ITEMS = ["separator", "switchView"];
var REFRESH_ICON_MAP = {
    default: "dx-filemanager-i dx-filemanager-i-refresh",
    progress: "dx-filemanager-i dx-filemanager-i-progress",
    success: "dx-filemanager-i dx-filemanager-i-done",
    error: "dx-filemanager-i dx-filemanager-i-danger"
};
var REFRESH_ITEM_PROGRESS_MESSAGE_DELAY = 500;
class FileManagerToolbar extends Widget {
    _init() {
        super._init();
        this._generalToolbarVisible = true;
        this._refreshItemState = {
            message: "",
            status: "default"
        }
    }
    _initMarkup() {
        this._createItemClickedAction();
        this._$viewSwitcherPopup = $("<div>").addClass(FILE_MANAGER_VIEW_SWITCHER_POPUP_CLASS);
        this._generalToolbar = this._createToolbar(this.option("generalItems"), !this._generalToolbarVisible);
        this._fileToolbar = this._createToolbar(this.option("fileItems"), this._generalToolbarVisible);
        this._$viewSwitcherPopup.appendTo(this.$element());
        this.$element().addClass(FILE_MANAGER_TOOLBAR_CLASS + " " + FILE_MANAGER_GENERAL_TOOLBAR_CLASS)
    }
    _render() {
        super._render();
        var toolbar = this._getVisibleToolbar();
        this._checkCompactMode(toolbar)
    }
    _clean() {
        delete this._commandManager;
        delete this._itemClickedAction;
        delete this._$viewSwitcherPopup;
        delete this._generalToolbar;
        delete this._fileToolbar;
        super._clean()
    }
    _dimensionChanged(dimension) {
        if (!dimension || "height" !== dimension) {
            var toolbar = this._getVisibleToolbar();
            this._checkCompactMode(toolbar)
        }
    }
    _getVisibleToolbar() {
        return this._generalToolbarVisible ? this._generalToolbar : this._fileToolbar
    }
    _createToolbar(items, hidden) {
        var toolbarItems = this._getPreparedItems(items);
        var $toolbar = $("<div>").appendTo(this.$element());
        var result = this._createComponent($toolbar, Toolbar, {
            items: toolbarItems,
            visible: !hidden,
            onItemClick: args => this._raiseItemClicked(args)
        });
        result.compactMode = false;
        return result
    }
    _getPreparedItems(items) {
        items = items.map(item => {
            var extendedItem = item;
            if (isString(item)) {
                extendedItem = {
                    name: item
                }
            }
            var commandName = extendedItem.name;
            var preparedItem = this._configureItemByCommandName(commandName, extendedItem);
            preparedItem.originalItemData = item;
            if ("separator" !== commandName) {
                this._setItemVisibleAvailable(preparedItem)
            }
            return preparedItem
        });
        this._updateSeparatorsVisibility(items);
        return items
    }
    _updateSeparatorsVisibility(items, toolbar) {
        var hasModifications = false;
        var menuItems = this._getMenuItems(toolbar);
        var hasItemsBefore = {
            before: false,
            center: false,
            after: false
        };
        var itemGroups = {
            before: this._getItemsInGroup(items, menuItems, "before"),
            center: this._getItemsInGroup(items, menuItems, "center"),
            after: this._getItemsInGroup(items, menuItems, "after")
        };
        items.forEach((item, i) => {
            var itemLocation = item.location;
            if ("separator" === item.name) {
                var isSeparatorVisible = hasItemsBefore[itemLocation] && this._groupHasItemsAfter(itemGroups[itemLocation]);
                if (item.visible !== isSeparatorVisible) {
                    hasModifications = true;
                    item.visible = isSeparatorVisible
                }
                hasItemsBefore[itemLocation] = false
            } else {
                if (!this._isItemInMenu(menuItems, item)) {
                    hasItemsBefore[itemLocation] = hasItemsBefore[itemLocation] || item.visible
                }
                itemGroups[itemLocation].shift()
            }
        });
        if (toolbar && hasModifications) {
            toolbar.repaint()
        }
        return hasModifications
    }
    _getMenuItems(toolbar) {
        var result = toolbar ? toolbar._getMenuItems() : [];
        return result.map(menuItem => menuItem.originalItemData)
    }
    _isItemInMenu(menuItems, item) {
        return !!menuItems.length && "never" !== ensureDefined(item.locateInMenu, "never") && -1 !== menuItems.indexOf(item.originalItemData)
    }
    _getItemsInGroup(items, menuItems, groupName) {
        return items.filter(item => item.location === groupName && !this._isItemInMenu(menuItems, item))
    }
    _groupHasItemsAfter(items) {
        for (var i = 0; i < items.length; i++) {
            if ("separator" !== items[i].name && items[i].visible) {
                return true
            }
        }
        return false
    }
    _configureItemByCommandName(commandName, item) {
        var _result$options;
        var result = {};
        var command = this._commandManager.getCommandByName(commandName);
        if (command) {
            result = this._createCommandItem(command)
        }
        switch (commandName) {
            case "separator":
                result = this._createSeparatorItem();
                break;
            case "switchView":
                result = this._createViewModeItem()
        }
        if (this._isDefaultItem(commandName)) {
            var defaultConfig = DEFAULT_ITEM_CONFIGS[commandName];
            extend(true, result, defaultConfig);
            var resultCssClass = result.cssClass || "";
            extendAttributes(result, item, DEFAULT_ITEM_ALLOWED_PROPERTIES);
            if (isDefined(item.options)) {
                extendAttributes(result.options, item.options, DEFAULT_ITEM_ALLOWED_OPTION_PROPERTIES)
            }
            extendAttributes(result.options, item, ["text", "icon"]);
            if (item.cssClass) {
                resultCssClass = "".concat(resultCssClass, " ").concat(item.cssClass)
            }
            if (resultCssClass) {
                result.cssClass = resultCssClass
            }
            if (!isDefined(item.visible)) {
                result._autoHide = true
            }
            if ("dxButton" === result.widget) {
                if ("inMenu" === result.showText && !isDefined(result.options.hint)) {
                    result.options.hint = result.options.text
                }
                if (result.compactMode && !isDefined(result.options.hint)) {
                    this._configureHintForCompactMode(result)
                }
            }
        } else {
            extend(true, result, item);
            if (!result.widget) {
                result.widget = "dxButton"
            }
            if ("dxButton" === result.widget && !result.compactMode && !result.showText && result.options && result.options.icon && result.options.text) {
                result.compactMode = {
                    showText: "inMenu"
                }
            }
        }
        if (commandName && !result.name) {
            extend(result, {
                name: commandName
            })
        }
        result.location = ensureDefined(result.location, "before");
        if (!isDefined(null === (_result$options = result.options) || void 0 === _result$options ? void 0 : _result$options.stylingMode)) {
            if ("dxButton" === result.widget) {
                extend(true, result, {
                    options: {
                        stylingMode: "text"
                    }
                })
            }
            if ("dxSelectBox" === result.widget) {
                extend(true, result, {
                    options: {
                        stylingMode: "filled"
                    }
                })
            }
        }
        return result
    }
    _isDefaultItem(commandName) {
        return !!DEFAULT_ITEM_CONFIGS[commandName]
    }
    _createCommandItem(command) {
        return {
            widget: "dxButton",
            options: {
                text: command.text,
                hint: command.hint,
                commandText: command.text,
                icon: command.icon,
                stylingMode: "text",
                onClick: e => this._executeCommand(command)
            }
        }
    }
    _createSeparatorItem() {
        return {
            template: (data, index, element) => {
                $(element).addClass(FILE_MANAGER_TOOLBAR_SEPARATOR_ITEM_CLASS)
            }
        }
    }
    _createViewModeItem() {
        var commandItems = ["details", "thumbnails"].map(name => {
            var {
                text: text,
                icon: icon
            } = this._commandManager.getCommandByName(name);
            return {
                name: name,
                text: text,
                icon: icon
            }
        });
        var selectedIndex = "thumbnails" === this.option("itemViewMode") ? 1 : 0;
        return {
            cssClass: FILE_MANAGER_TOOLBAR_VIEWMODE_ITEM_CLASS,
            widget: "dxDropDownButton",
            options: {
                items: commandItems,
                keyExpr: "name",
                selectedItemKey: this.option("itemViewMode"),
                displayExpr: " ",
                hint: commandItems[selectedIndex].text,
                stylingMode: "text",
                showArrowIcon: false,
                useSelectMode: true,
                dropDownOptions: {
                    container: this._$viewSwitcherPopup
                },
                onItemClick: e => this._executeCommand(e.itemData.name)
            }
        }
    }
    _configureHintForCompactMode(item) {
        item.options.hint = "";
        item.compactMode.options = item.compactMode.options || {};
        item.compactMode.options.hint = item.options.text
    }
    _checkCompactMode(toolbar) {
        if (toolbar.compactMode) {
            this._toggleCompactMode(toolbar, false)
        }
        var useCompactMode = this._toolbarHasItemsOverflow(toolbar);
        if (toolbar.compactMode !== useCompactMode) {
            if (!toolbar.compactMode) {
                this._toggleCompactMode(toolbar, useCompactMode)
            }
            toolbar.compactMode = useCompactMode
        } else if (toolbar.compactMode) {
            this._toggleCompactMode(toolbar, true)
        }
    }
    _toolbarHasItemsOverflow(toolbar) {
        var toolbarWidth = getWidth(toolbar.$element());
        var itemsWidth = toolbar._getItemsWidth();
        return toolbarWidth < itemsWidth
    }
    _toggleCompactMode(toolbar, useCompactMode) {
        var hasModifications = false;
        var items = toolbar.option("items");
        items.forEach(item => {
            if (item.compactMode) {
                var optionsSource = null;
                if (useCompactMode) {
                    item.saved = this._getCompactModeOptions(item, item._available);
                    optionsSource = item.compactMode
                } else {
                    optionsSource = item.saved
                }
                var options = this._getCompactModeOptions(optionsSource, item._available);
                extend(true, item, options);
                hasModifications = true
            }
        });
        hasModifications = this._updateSeparatorsVisibility(items) || hasModifications;
        if (hasModifications) {
            toolbar.repaint()
        }
        this._updateSeparatorsVisibility(items, toolbar)
    }
    _getCompactModeOptions(_ref, available) {
        var {
            showText: showText,
            locateInMenu: locateInMenu,
            options: options
        } = _ref;
        return {
            visible: available,
            showText: ensureDefined(showText, "always"),
            locateInMenu: ensureDefined(locateInMenu, "never"),
            options: {
                hint: null === options || void 0 === options ? void 0 : options.hint
            }
        }
    }
    _ensureAvailableCommandsVisible(toolbar) {
        var hasModifications = false;
        var items = toolbar.option("items");
        items.forEach(item => {
            if ("separator" !== item.name) {
                var itemVisible = item._available;
                this._setItemVisibleAvailable(item);
                if (item._available !== itemVisible) {
                    hasModifications = true
                }
            }
        });
        hasModifications = this._updateSeparatorsVisibility(items) || hasModifications;
        if (hasModifications) {
            toolbar.repaint()
        }
        this._updateSeparatorsVisibility(items, toolbar)
    }
    _setItemVisibleAvailable(item) {
        var _item$originalItemDat;
        var originalVisible = null === (_item$originalItemDat = item.originalItemData) || void 0 === _item$originalItemDat ? void 0 : _item$originalItemDat.visible;
        item._available = this._isToolbarItemAvailable(item);
        item.visible = isDefined(originalVisible) ? originalVisible : item._available
    }
    _fileToolbarHasEffectiveItems() {
        var items = this._fileToolbar.option("items");
        return items.some(item => this._isFileToolbarItemAvailable(item))
    }
    _executeCommand(command) {
        this._commandManager.executeCommand(command)
    }
    _isToolbarItemAvailable(toolbarItem) {
        if (!this._isDefaultItem(toolbarItem.name) || !toolbarItem._autoHide) {
            return ensureDefined(toolbarItem.visible, true)
        }
        if ("refresh" === toolbarItem.name) {
            return this._generalToolbarVisible || !!this._isRefreshVisibleInFileToolbar
        }
        if (ALWAYS_VISIBLE_TOOLBAR_ITEMS.indexOf(toolbarItem.name) > -1) {
            return true
        }
        return this._isCommandAvailable(toolbarItem.name)
    }
    _isFileToolbarItemAvailable(_ref2) {
        var {
            name: name,
            visible: visible
        } = _ref2;
        return !this._isDefaultItem(name) && ensureDefined(visible, true) || "clearSelection" !== name && "refresh" !== name && this._isCommandAvailable(name)
    }
    _isCommandAvailable(name) {
        return this._commandManager.isCommandAvailable(name, this.option("contextItems"))
    }
    _updateItemInToolbar(toolbar, commandName, options) {
        toolbar.beginUpdate();
        var items = toolbar.option("items");
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            if (item.name === commandName) {
                toolbar.option("items[".concat(i, "]"), options);
                break
            }
        }
        toolbar.endUpdate()
    }
    _raiseItemClicked(args) {
        var changedArgs = extend(true, {}, args);
        changedArgs.itemData = args.itemData.originalItemData;
        this._itemClickedAction(changedArgs)
    }
    _createItemClickedAction() {
        this._itemClickedAction = this._createActionByOption("onItemClick")
    }
    _getDefaultOptions() {
        return extend(super._getDefaultOptions(), {
            commandManager: null,
            generalItems: [],
            fileItems: [],
            contextItems: [],
            itemViewMode: "details",
            onItemClick: null
        })
    }
    _optionChanged(args) {
        var name = args.name;
        switch (name) {
            case "commandManager":
            case "itemViewMode":
            case "generalItems":
            case "fileItems":
                this.repaint();
                break;
            case "contextItems":
                this._update();
                break;
            case "onItemClick":
                this._itemClickedAction = this._createActionByOption(name);
                break;
            default:
                super._optionChanged(args)
        }
    }
    updateItemPermissions() {
        this.repaint();
        this._restoreRefreshItemState()
    }
    _restoreRefreshItemState() {
        this.updateRefreshItem(this._refreshItemState.message, this._refreshItemState.status)
    }
    updateRefreshItem(message, status) {
        var generalToolbarOptions = null;
        var text = messageLocalization.format("dxFileManager-commandRefresh");
        var showText = "inMenu";
        this._isRefreshVisibleInFileToolbar = false;
        this._refreshItemState = {
            message: message,
            status: status
        };
        if ("default" === status) {
            generalToolbarOptions = {
                options: {
                    icon: REFRESH_ICON_MAP.default
                }
            }
        } else {
            generalToolbarOptions = {
                options: {
                    icon: REFRESH_ICON_MAP[status]
                }
            };
            this._isRefreshVisibleInFileToolbar = true;
            text = message;
            showText = "always"
        }
        var fileToolbarOptions = extend({}, generalToolbarOptions, {
            visible: this._isRefreshVisibleInFileToolbar
        });
        this._applyRefreshItemOptions(generalToolbarOptions, fileToolbarOptions);
        this._refreshItemTextTimeout = this._updateRefreshItemText("progress" === status, text, showText)
    }
    _updateRefreshItemText(isDeferredUpdate, text, showText) {
        var options = {
            showText: showText,
            options: {
                text: text
            }
        };
        if (isDeferredUpdate) {
            return setTimeout(() => {
                this._applyRefreshItemOptions(options);
                this._refreshItemTextTimeout = void 0
            }, REFRESH_ITEM_PROGRESS_MESSAGE_DELAY)
        } else {
            if (this._refreshItemTextTimeout) {
                clearTimeout(this._refreshItemTextTimeout)
            }
            this._applyRefreshItemOptions(options);
            return
        }
    }
    _applyRefreshItemOptions(generalToolbarOptions, fileToolbarOptions) {
        if (!fileToolbarOptions) {
            fileToolbarOptions = extend({}, generalToolbarOptions)
        }
        this._updateItemInToolbar(this._generalToolbar, "refresh", generalToolbarOptions);
        this._updateItemInToolbar(this._fileToolbar, "refresh", fileToolbarOptions)
    }
    _update() {
        var showGeneralToolbar = 0 === this.option("contextItems").length || !this._fileToolbarHasEffectiveItems();
        if (this._generalToolbarVisible !== showGeneralToolbar) {
            this._generalToolbar.option("visible", showGeneralToolbar);
            this._fileToolbar.option("visible", !showGeneralToolbar);
            this._generalToolbarVisible = showGeneralToolbar;
            this.$element().toggleClass(FILE_MANAGER_GENERAL_TOOLBAR_CLASS, showGeneralToolbar);
            this.$element().toggleClass(FILE_MANAGER_FILE_TOOLBAR_CLASS, !showGeneralToolbar)
        }
        var toolbar = this._getVisibleToolbar();
        this._ensureAvailableCommandsVisible(toolbar);
        this._checkCompactMode(toolbar)
    }
    get _commandManager() {
        return this.option("commandManager")
    }
}
export default FileManagerToolbar;
