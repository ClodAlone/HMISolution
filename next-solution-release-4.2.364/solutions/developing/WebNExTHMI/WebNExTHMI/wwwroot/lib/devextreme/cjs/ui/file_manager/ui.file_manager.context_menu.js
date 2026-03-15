/**
 * DevExtreme (cjs/ui/file_manager/ui.file_manager.context_menu.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _extend = require("../../core/utils/extend");
var _type = require("../../core/utils/type");
var _common = require("../../core/utils/common");
var _ui = _interopRequireDefault(require("../widget/ui.widget"));
var _ui2 = _interopRequireDefault(require("../context_menu/ui.context_menu"));
var _uiFile_manager = require("./ui.file_manager.common");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
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
var FILEMANAGER_CONTEXT_MEMU_CLASS = "dx-filemanager-context-menu";
var DEFAULT_CONTEXT_MENU_ITEMS = {
    create: {},
    upload: {},
    download: {},
    rename: {},
    move: {},
    copy: {},
    delete: {},
    refresh: {
        beginGroup: true
    }
};
var DEFAULT_ITEM_ALLOWED_PROPERTIES = ["beginGroup", "closeMenuOnClick", "disabled", "icon", "selectable", "selected", "text", "visible"];
var FileManagerContextMenu = function(_Widget) {
    _inheritsLoose(FileManagerContextMenu, _Widget);

    function FileManagerContextMenu() {
        return _Widget.apply(this, arguments) || this
    }
    var _proto = FileManagerContextMenu.prototype;
    _proto._initMarkup = function() {
        var _this = this;
        this._initActions();
        this._isVisible = false;
        var $menu = (0, _renderer.default)("<div>").appendTo(this.$element());
        this._contextMenu = this._createComponent($menu, _ui2.default, {
            cssClass: FILEMANAGER_CONTEXT_MEMU_CLASS,
            showEvent: "",
            onItemClick: function(args) {
                return _this._onContextMenuItemClick(args.itemData.name, args)
            },
            onShowing: function(e) {
                return _this._onContextMenuShowing(e)
            },
            onShown: function() {
                return _this._onContextMenuShown()
            },
            onHidden: function() {
                return _this._onContextMenuHidden()
            }
        });
        _Widget.prototype._initMarkup.call(this)
    };
    _proto.showAt = function(fileItems, element, event, target) {
        var itemData = target.itemData,
            itemElement = target.itemElement,
            _target$isActionButto = target.isActionButton,
            isActionButton = void 0 === _target$isActionButto ? false : _target$isActionButto;
        if (this._isVisible) {
            this._onContextMenuHidden()
        }
        this._menuShowingContext = {
            targetElement: itemElement,
            itemData: itemData,
            fileItems: fileItems,
            event: event,
            isActionButton: isActionButton
        };
        var position = {
            of: element,
            at: "top left",
            my: "top left",
            offset: ""
        };
        if (event) {
            position.offset = event.offsetX + " " + event.offsetY
        } else {
            position.my = "left top";
            position.at = "left bottom";
            position.boundaryOffset = "1"
        }
        this._contextMenu.option({
            target: element,
            position: position
        });
        this._contextMenu.show()
    };
    _proto.createContextMenuItems = function(fileItems, contextMenuItems, targetFileItem) {
        var _this2 = this;
        this._targetFileItems = fileItems;
        this._targetFileItem = (0, _type.isDefined)(targetFileItem) ? targetFileItem : null === fileItems || void 0 === fileItems ? void 0 : fileItems[0];
        var result = [];
        var itemArray = contextMenuItems || this.option("items");
        itemArray.forEach((function(srcItem) {
            var commandName = (0, _type.isString)(srcItem) ? srcItem : srcItem.name;
            var item = _this2._configureItemByCommandName(commandName, srcItem, fileItems, _this2._targetFileItem);
            if (_this2._isContextMenuItemAvailable(item, fileItems)) {
                result.push(item)
            }
        }));
        return result
    };
    _proto._isContextMenuItemAvailable = function(menuItem, fileItems) {
        if (!this._isDefaultItem(menuItem.name) || !menuItem._autoHide) {
            return (0, _common.ensureDefined)(menuItem.visible, true)
        }
        if (this._isIsolatedCreationItemCommand(menuItem.name) && fileItems && fileItems.length) {
            return false
        }
        return this._commandManager.isCommandAvailable(menuItem.name, fileItems)
    };
    _proto._isIsolatedCreationItemCommand = function(commandName) {
        return ("create" === commandName || "upload" === commandName) && this.option("isolateCreationItemCommands")
    };
    _proto._isDefaultItem = function(commandName) {
        return !!DEFAULT_CONTEXT_MENU_ITEMS[commandName]
    };
    _proto._configureItemByCommandName = function(commandName, item, fileItems, targetFileItem) {
        if (!this._isDefaultItem(commandName)) {
            var res = (0, _extend.extend)(true, {}, item);
            res.originalItemData = item;
            this._addItemClickHandler(commandName, res);
            if (Array.isArray(item.items)) {
                res.items = this.createContextMenuItems(fileItems, item.items, targetFileItem)
            }
            return res
        }
        var result = this._createMenuItemByCommandName(commandName);
        var defaultConfig = DEFAULT_CONTEXT_MENU_ITEMS[commandName];
        (0, _extend.extend)(result, defaultConfig);
        result.originalItemData = item;
        (0, _uiFile_manager.extendAttributes)(result, item, DEFAULT_ITEM_ALLOWED_PROPERTIES);
        if (!(0, _type.isDefined)(result.visible)) {
            result._autoHide = true
        }
        if (commandName && !result.name) {
            (0, _extend.extend)(result, {
                name: commandName
            })
        }
        return result
    };
    _proto._createMenuItemByCommandName = function(commandName) {
        var _this$_commandManager = this._commandManager.getCommandByName(commandName),
            text = _this$_commandManager.text,
            icon = _this$_commandManager.icon;
        var menuItem = {
            name: commandName,
            text: text,
            icon: icon
        };
        this._addItemClickHandler(commandName, menuItem);
        return menuItem
    };
    _proto._addItemClickHandler = function(commandName, contextMenuItem) {
        var _this3 = this;
        contextMenuItem.onItemClick = function(args) {
            return _this3._onContextMenuItemClick(commandName, args)
        }
    };
    _proto._onContextMenuItemClick = function(commandName, args) {
        var _this$_targetFileItem;
        var changedArgs = (0, _extend.extend)(true, {}, args);
        changedArgs.itemData = args.itemData.originalItemData;
        changedArgs.fileSystemItem = null === (_this$_targetFileItem = this._targetFileItem) || void 0 === _this$_targetFileItem ? void 0 : _this$_targetFileItem.fileItem;
        changedArgs.viewArea = this.option("viewArea");
        this._actions.onItemClick(changedArgs);
        if (this._isDefaultItem(commandName)) {
            var targetFileItems = this._isIsolatedCreationItemCommand(commandName) ? null : this._targetFileItems;
            this._commandManager.executeCommand(commandName, targetFileItems)
        }
    };
    _proto._initActions = function() {
        this._actions = {
            onContextMenuHidden: this._createActionByOption("onContextMenuHidden"),
            onContextMenuShowing: this._createActionByOption("onContextMenuShowing"),
            onItemClick: this._createActionByOption("onItemClick")
        }
    };
    _proto._onContextMenuShowing = function(e) {
        if (this._isVisible) {
            this._onContextMenuHidden(true)
        }
        e = (0, _extend.extend)(e, this._menuShowingContext, {
            options: this.option(),
            cancel: false
        });
        this._actions.onContextMenuShowing(e);
        if (!e.cancel) {
            var items = this.createContextMenuItems(this._menuShowingContext.fileItems, null, this._menuShowingContext.fileSystemItem);
            this._contextMenu.option("dataSource", items)
        }
    };
    _proto.tryUpdateVisibleContextMenu = function() {
        if (this._isVisible) {
            var items = this.createContextMenuItems(this._targetFileItems);
            this._contextMenu.option("dataSource", items)
        }
    };
    _proto._onContextMenuShown = function() {
        this._isVisible = true
    };
    _proto._onContextMenuHidden = function(preserveContext) {
        this._isVisible = false;
        if (!preserveContext) {
            this._menuShowingContext = {}
        }
        this._contextMenu.option("visible", false);
        this._raiseContextMenuHidden()
    };
    _proto._raiseContextMenuHidden = function() {
        this._actions.onContextMenuHidden()
    };
    _proto._getDefaultOptions = function() {
        return (0, _extend.extend)(_Widget.prototype._getDefaultOptions.call(this), {
            commandManager: null,
            onContextMenuHidden: null,
            onItemClick: null
        })
    };
    _proto._optionChanged = function(args) {
        var name = args.name;
        switch (name) {
            case "commandManager":
                this.repaint();
                break;
            case "items":
                this.tryUpdateVisibleContextMenu();
                break;
            case "onItemClick":
            case "onContextMenuShowing":
            case "onContextMenuHidden":
                this._actions[name] = this._createActionByOption(name);
                break;
            default:
                _Widget.prototype._optionChanged.call(this, args)
        }
    };
    _createClass(FileManagerContextMenu, [{
        key: "_commandManager",
        get: function() {
            return this.option("commandManager")
        }
    }]);
    return FileManagerContextMenu
}(_ui.default);
var _default = FileManagerContextMenu;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
